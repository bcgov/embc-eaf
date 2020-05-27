using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EMBC.ExpenseAuthorization.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var logger = ConfigureLogging();

            try
            {
                logger.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Web host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Added before AddUserSecrets to let user secrets override environment variables.
                    config.AddEnvironmentVariables();

                    var env = hostingContext.HostingEnvironment;
                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                });

            return builder;
        }
                        
        private static ILogger ConfigureLogging()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            configurationBuilder.AddEnvironmentVariables();

#if DEBUG
            // we dont know about the hosting environment yet, so we cant check if this is development
            configurationBuilder.AddJsonFile("appsettings.Development.json", optional: true);
            configurationBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true);
#endif
            var configuration = configurationBuilder.Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Logger = logger;

            return logger;
        }
    }
}
