using System;
using System.Net.Http;
using Autofac.Extensions.DependencyInjection;
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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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

            // check for splunk configuration
            var splunkUrl = hostingContext.Configuration.GetValue("SPLUNK_URL", string.Empty);
            var splunkToken = hostingContext.Configuration.GetValue("SPLUNK_TOKEN", string.Empty);

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

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return logger;
        }
    }
}
