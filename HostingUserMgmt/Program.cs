using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace HostingUserMgmt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingCtx, cfgBuilder) => {
                    var env = hostingCtx.HostingEnvironment;
                    cfgBuilder.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddJsonFile($"./secrets/secrets.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();
                })
                .UseSerilog((hostingCtx, logging) => {
                    logging
                    .ReadFrom.Configuration(hostingCtx.Configuration)
                    .WriteTo
                    .Console(new CompactJsonFormatter());
                })
                //.UseUrls() set by env variable
                .UseStartup<Startup>();
    }
}
