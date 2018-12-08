using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Security.Claims;
using System.Threading.Tasks;
using HostingUserMgmt.AppServices;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.Helpers.Authentication;
using HostingUserMgmt.Repository;
using HostingUserMgmt.Repository.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using AutoMapper;
using HostingUserMgmt.AppServices.Mapping;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace HostingUserMgmt
{
    public class Startup
    {
        private const string GoogleclientIdKey = "Google:ClientId";
        private const string GoogleclientSecretKey = "Google:ClientSecret";
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddJsonFile($"./secrets/secrets.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(ConfigureAuthentication)
            .AddCookie()
            .AddGoogle(opts => ConfigureGoogleAuthentication(opts, services));
            services.AddAuthorization();

            ConfigureAutoMapper(services);
            ConfigureDependencyInjection(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "api",
                    template: "api/{controller}/{action}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureAuthentication(AuthenticationOptions opts)
        {
            opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }

        private void ConfigureGoogleAuthentication(GoogleOptions opts,
            IServiceCollection services)
        {
            opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.ClientId = Configuration.GetValue<string>(GoogleclientIdKey);
            opts.ClientSecret = Configuration.GetValue<string>(GoogleclientSecretKey);
            opts.SaveTokens = true;
            opts.ClaimActions.Add(new GoogleClaimsProcessor(AppClaimTypes.GoogleImageUrl));
            opts.Events = new OAuthEvents
            {
                OnTicketReceived = ctx =>
                {
                    var uri = ctx.ReturnUri;
                    return Task.CompletedTask;
                },
                OnCreatingTicket = GoogleOnCreatingTicket,
                OnRemoteFailure = ctx => Task.CompletedTask
            };
        }
        private void ConfigureAutoMapper(IServiceCollection services)
        {
            Mapper.Initialize(cfg => {
                cfg.AddProfile(new RepoToViewModelProfile());
                cfg.AddProfile(new ViewToRepoModelProfile());
            });
            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp =>
                new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }
        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddDbContextPool<HostingManagementDbContext>(
                options => options.UseMySql(Configuration.GetConnectionString("Database"),
                    mySqlOptions =>
                    {
                        mySqlOptions.ServerVersion(new Version(10, 3, 11), ServerType.MySql);
                    })
            );
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddHttpContextAccessor();
            services.AddScoped<ClaimsPrincipal>(serviceProvider =>
                serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.User);
        }

        private async Task GoogleOnCreatingTicket(OAuthCreatingTicketContext context)
        {
            await context.HttpContext.RequestServices
                .GetRequiredService<IUserService>()
                .AddOrUpdateUserOnLogin(context.Identity.Claims);
        }
    }
}
