using System;
using System.Net.Http;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace EMBC.ExpenseAuthorization.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var logger = GetProgramLogger();

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
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.WithMachineName()
                        .Enrich.WithProcessId()
                        .Enrich.WithProcessName()
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails()
                        ;

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        loggerConfiguration.WriteTo.Console();
                    }
                    else
                    {
                        loggerConfiguration.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());
                        var splunkUrl = hostingContext.Configuration.GetValue("SPLUNK_URL", string.Empty);
                        var splunkToken = hostingContext.Configuration.GetValue("SPLUNK_TOKEN", string.Empty);
                        if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
                        {
                            Log.Warning("Splunk logging sink is not configured properly, check SPLUNK_TOKEN and SPLUNK_URL env vars");
                        }
                        else
                        {
                            loggerConfiguration
                                .WriteTo.EventCollector(
                                    splunkHost: splunkUrl,
                                    eventCollectorToken: splunkToken,
                                    messageHandler: new HttpClientHandler
                                    {
                                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                                    },
                                    renderTemplate: false);
                        }
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    SetupConfigurationSources(config, hostingContext.HostingEnvironment);
                });

            return builder;
        }
         
        /// <summary>
        /// Gets the main program logger used outside of the hosting service.
        /// </summary>
        /// <returns></returns>
        private static ILogger GetProgramLogger()
        {
            var configurationBuilder = new ConfigurationBuilder();

            // we dont know the environment yet, so pass null
            SetupConfigurationSources(configurationBuilder, env: null);

            var configuration = configurationBuilder.Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

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

#if DEBUG
            try
            {
                // only use User Secrets in development

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
#endif
        }
    }
}
