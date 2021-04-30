using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace EMBC.ExpenseAuthorization.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // default to writing to console, other configuration will change this based on configuration
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();


            var logger = GetProgramLogger(args);

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
                .UseSerilog(ConfigureSerilogLogger)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

            return builder;
        }

        private static void ConfigureSerilogLogger(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();

            ConfigureSplunkLogging(hostingContext, loggerConfiguration);
        }

        private static void ConfigureSplunkLogging(LoggerConfiguration loggerConfiguration)
        {
            ConfigureSplunkLogging(null, loggerConfiguration);
        }

        private static void ConfigureSplunkLogging(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
        {
            // check for splunk configuration
            var splunkUrl = hostingContext != null 
                ? hostingContext.Configuration.GetValue("SPLUNK_URL", string.Empty)
                : Environment.GetEnvironmentVariable("SPLUNK_URL");

            var splunkToken = hostingContext != null
                ? hostingContext.Configuration.GetValue("SPLUNK_TOKEN", string.Empty)
                : Environment.GetEnvironmentVariable("SPLUNK_TOKEN");

            if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
            {
                Log.Warning("Splunk logging sink is not configured properly, check SPLUNK_TOKEN and SPLUNK_URL environment variables");
            }
            else
            {
                loggerConfiguration
                    .WriteTo.EventCollector(
                        splunkHost: splunkUrl,
                        eventCollectorToken: splunkToken,
                        messageHandler: new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true },
                        renderTemplate: false);
            }
        }

        /// <summary>
        /// Gets the main program logger used outside of the hosting service.
        /// </summary>
        /// <returns></returns>
        private static ILogger GetProgramLogger(string[] args)
        {
            // configure the program logger in the same way as CreateDefaultBuilder does
            string environmentName = GetEnvironmentName();

            bool IsDevelopment()
            {
                return string.Equals(environmentName, Environments.Development, StringComparison.OrdinalIgnoreCase);
            }

            string GetEnvironmentName()
            {
                string name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (string.IsNullOrEmpty(name))
                {
                    name = Environments.Production;
                }

                return name;
            }

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            if (IsDevelopment())
            {
                configurationBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true);
            }

            configurationBuilder.AddEnvironmentVariables();

            if (args != null)
            {
                configurationBuilder.AddCommandLine(args);
            }

            var configuration = configurationBuilder.Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            ConfigureSplunkLogging(loggerConfiguration);

            var logger = Log.Logger = loggerConfiguration.CreateLogger();

            return logger;
        }
    }
}
