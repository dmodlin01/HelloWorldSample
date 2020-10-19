using System;
using System.IO;
using CatalogServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Serilog;
using Serilog.Formatting.Compact;

//Sample for DI (to inject IMessageService implementation), Logging (with Serilog), Configuration settings
namespace HelloWorldConsole
{
    class Program
    {
        #region Configuration wiring

        /// <summary>
        /// Wiring for configuration retrieval (ability to retrieve from appsettings.json)
        /// </summary>
        /// <param name="configurationBuilder"></param>
        static void BuildConfig(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //connection to the appsettings.json
        }
        /// <summary>
        /// Uses configuration builder to build out the configuration
        /// </summary>
        /// <returns>Configuration collection</returns>
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            return builder.Build();
        }

        #endregion


        /// <summary>
        /// Method will load the configuration file and configure logging
        /// Logging to a file within [Temp path]\[LogFile]
        /// </summary>
        /// <param name="configuration">Configuration collection</param>
        private static void WireLogging(IConfiguration configuration)
        {
            var fileName = configuration.GetValue<string>("LogFile");
            var logfile = Path.Combine(Path.GetTempPath(), fileName); //location of the log file
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) //load logging settings from the configuration file
                .Enrich.FromLogContext() //enrich with contextual information
                .WriteTo.File(new RenderedCompactJsonFormatter(), logfile) //add file destination
                .WriteTo.Console() //add console destination
                .CreateLogger();
            Log.Logger.Information("Wired Logging for HelloWorldConsole");
        }

        /// <summary>
        /// Method registers types with interfaces for Dependency Injection
        /// Adds SeriLog as the ILogging provider
        /// </summary>
        /// <param name="configuration">Configuration collection</param>
        /// <returns></returns>
        private static IHost WireDependencyInjection(IConfiguration configuration)
        {
            var messageServiceSource = configuration.GetValue<string>("MessageServiceSource");
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    switch (messageServiceSource)
                    {
                        case "WebApiMessageService":
                            services.AddTransient<IMessageRepository, WebApiMessageRepository>();
                            services.AddTransient<MessageService<WebApiMessageService>, WebApiMessageService>();
                            break;
                        case "RepositoryMessageService":
                            services.AddTransient<IMessageRepository, MockMessageRepository>();
                            services.AddTransient<MessageService<RepositoryMessageService>, RepositoryMessageService>();
                            break;
                        default:
                            throw new Exception($"{messageServiceSource} Message Service is not supported.");
                   }
                })
                .UseSerilog()
                .Build();
            Log.Logger.Information("Wired DI");
            return host;
        }

        static void Main(string[] args)
        {
            try
            {
                var configuration = GetConfiguration();
                WireLogging(configuration);
                var host = WireDependencyInjection(configuration);
                var messageServiceSource = configuration.GetValue<string>("MessageServiceSource");

                string message;
                switch (messageServiceSource)
                {
                    case "WebApiMessageService":
                        var wevSvc = ActivatorUtilities.CreateInstance<WebApiMessageService>(host.Services); //create an instance of IMessageService from the DI container
                        message = wevSvc.GetMessage().Message;
                        break;
                    case "RepositoryMessageService":
                        var repSvc = ActivatorUtilities.CreateInstance<RepositoryMessageService>(host.Services); //create an instance of IMessageService from the DI container
                        message = repSvc.GetMessage().Message;
                        break;
                    default:
                        throw new Exception($"{messageServiceSource} Message Service is not supported.");
                }
  
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
