using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using HostingUserMgmt.AppServices;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.AppServices.Mapping;
using HostingUserMgmt.Helpers.Authentication;
using HostingUserMgmt.Repository;
using HostingUserMgmt.Repository.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;
using Amazon;
using Amazon.KeyManagementService;
using HostingUserMgmt.Helpers.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace HostingUserMgmt {
    public class Startup {
        private const string AppPathPrefixEnvVar = "AppPathPrefix";
        private const string GoogleclientIdKey = "GoogleConfig:ClientId";
        private const string GoogleclientSecretKey = "GoogleConfig:ClientSecret";
        private readonly ILogger<Startup> logger;
        public Startup (IHostingEnvironment env, ILoggerFactory loggerFactory,
            IConfiguration configuration) {
            logger = loggerFactory.CreateLogger<Startup> ();
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public string AppPathPrefix => Environment.GetEnvironmentVariable (AppPathPrefixEnvVar) ?? string.Empty;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            CommonConfigureServices (services);
        }

        public void ConfigureProductionServices (IServiceCollection services) {

            services.Configure<ForwardedHeadersOptions> (options => {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.RequireHeaderSymmetry = false;
                options.KnownNetworks.Clear ();
                options.KnownProxies.Clear ();
            });

            CommonConfigureServices (services);

            services.AddDataProtection ()
                .PersistKeysToFileSystem (new System.IO.DirectoryInfo ("/home/app/keys"));
        }

        private void CommonConfigureServices (IServiceCollection services) {
            services.AddOptions ();
            services.Configure<AwsConfig> (Configuration.GetSection (nameof (AwsConfig)));
            services.Configure<GoogleConfig> (Configuration.GetSection (nameof (GoogleConfig)));
            services.Configure<CookiePolicyOptions> (options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication (ConfigureAuthentication)
                .AddCookie ()
                .AddGoogleOpenIdConnect (ConfigureGoogleOAuth);
            services.AddAuthorization ();

            ConfigureAutoMapper (services);
            ConfigureDependencyInjection (services);

            services.AddMvc (options => {
                    options.Filters.Add (new AutoValidateAntiforgeryTokenAttribute ());
                })
                .SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
        }

        public void ConfigureDevelopment (IApplicationBuilder app, IHostingEnvironment env) {
            if (!env.IsDevelopment ()) {
                throw new SystemException ("called 'ConfigureDevelopment' for non-development environment");
            }

            app.UseDeveloperExceptionPage ();
            app.UseHttpsRedirection ();

            CommonConfigure (app, env);
        }
        public void ConfigureProduction (IApplicationBuilder app, IHostingEnvironment env) {
            if (!env.IsProduction ()) {
                throw new SystemException ("called 'ConfigureProduction' for non-production environment");
            }
            app.UseForwardedHeaders ();

            CommonConfigure (app, env);
        }

        private void CommonConfigure (IApplicationBuilder app, IHostingEnvironment env) {
            app.UseAuthentication ();
            app.UsePathBase (AppPathPrefix);
            app.UseStaticFiles ();
            app.UseCookiePolicy ();

            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "api",
                    template: "api/{controller}/{action}/{id?}");
                routes.MapRoute (
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureAuthentication (AuthenticationOptions opts) {
            opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }

        private void ConfigureAutoMapper (IServiceCollection services) {
            Mapper.Initialize (cfg => {
                cfg.AddProfile (new RepoToViewModelProfile ());
                cfg.AddProfile (new ViewToRepoModelProfile ());
            });
            services.AddSingleton (Mapper.Configuration);
            services.AddScoped<IMapper> (sp =>
                new Mapper (sp.GetRequiredService<IConfigurationProvider> (), sp.GetService));
        }
        private void ConfigureDependencyInjection (IServiceCollection services) {
            services.AddDbContextPool<HostingManagementDbContext> (
                options => options.UseMySql (Configuration.GetConnectionString ("Database"),
                    mySqlOptions => {
                        mySqlOptions.ServerVersion (new Version (10, 3, 11), ServerType.MySql);
                    })
            );
            services.AddScoped<IEncryptionService, EncryptionService> ();
            services.AddScoped<IUserService, UserService> ();
            services.AddScoped<IUserRepository, UserRepository> ();

            services.AddScoped<IApiKeyService, ApiKeyService> ();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository> ();

            services.AddHttpContextAccessor ();
            services.AddScoped<ClaimsPrincipal> (serviceProvider =>
                serviceProvider.GetRequiredService<IHttpContextAccessor> ().HttpContext.User);

            services.AddSingleton<IAmazonKeyManagementService> (sp => {
                var opts = sp.GetRequiredService<IOptions<AwsConfig>> ();
                var cfg = opts?.Value ??
                    throw new ApplicationException ("Could not retrieve AwsConfig options");
                return new AmazonKeyManagementServiceClient (cfg.AccessKeyId, cfg.AccessKeySecret, RegionEndpoint.GetBySystemName (cfg.Region));
            });
        }

        private async Task UserInformationReceived (UserInformationReceivedContext context) {
            var userId = await context.HttpContext.RequestServices
                .GetRequiredService<IUserService> ()
                .AddOrUpdateUserOnLoginAsync (context.Principal.Claims);
            ((ClaimsIdentity) context.Principal.Identity).AddClaim (new Claim (AppClaimTypes.AppUserId, userId.ToString ()));
        }

        private void ConfigureGoogleOAuth (OpenIdConnectOptions oidcOptions) {
            var gcfg = new GoogleConfig ();
            Configuration.Bind (nameof (GoogleConfig), gcfg);
            oidcOptions.ClientId = gcfg.ClientId;
            oidcOptions.ClientSecret = gcfg.ClientSecret;
            oidcOptions.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            oidcOptions.GetClaimsFromUserInfoEndpoint = true;
            oidcOptions.SaveTokens = true;
            oidcOptions.ClaimActions.Add (new GoogleClaimsProcessor (AppClaimTypes.GoogleImageUrl));
            oidcOptions.Events = new OpenIdConnectEvents {
                OnUserInformationReceived = UserInformationReceived
            };
            oidcOptions.TokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        }
    }
}