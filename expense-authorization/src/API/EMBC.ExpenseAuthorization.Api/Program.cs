using System;
using Autofac.Extensions.DependencyInjection;
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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    SetupConfigurationSources(config, hostingContext.HostingEnvironment);
                });

            return builder;
        }
         
        /// <summary>
        /// Configures the globally shared logger.
        /// </summary>
        /// <returns></returns>
        private static ILogger ConfigureLogging()
        {
            var configurationBuilder = new ConfigurationBuilder();

            // we dont know the environment yet, so pass null
            SetupConfigurationSources(configurationBuilder, env: null);

            var configuration = configurationBuilder.Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Logger = logger;

            return logger;
        }


        private static void SetupConfigurationSources(IConfigurationBuilder configurationBuilder, IHostEnvironment env)
        {
            // Added before AddUserSecrets to let user secrets override environment variables.

            configurationBuilder.AddJsonFile("appsettings.json");

            if (env != null)
            {
                configurationBuilder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            }

            configurationBuilder.AddEnvironmentVariables();

            try
            {

                // User secrets override all other settings
                configurationBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true);
            }
            catch (InvalidOperationException)
            {
                // InvalidOperationException can be thrown by AddUserSecrets if the secret id exists but 
                // the framework can not determine an appropriate location for storing user secrets. It uses
                // the following environment variables / folders in this order
                //
                // APPDATA
                // HOME
                // SpecialFolder.ApplicationData
                // SpecialFolder.UserProfile
                // DOTNET_USER_SECRETS_FALLBACK_DIR
            }
        }
    }
}
