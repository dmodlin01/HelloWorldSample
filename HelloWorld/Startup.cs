using System.IO;
using System.Linq;
using AutoMap;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Serilog;
using Serilog.Formatting.Compact;

namespace HelloWorldWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //When testing via xUnit, the configuration would not be loaded (and the appsettings.json would not be available), as such need to build/load it
            if (!((ConfigurationRoot) configuration).Providers.Any(p => p is JsonConfigurationProvider))
            {
                configuration = GetConfiguration();
            }
            Configuration = configuration;
            WireLogging(configuration);
        }
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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
            //wire EF db context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HelloWorldConnection"),b=>b.MigrationsAssembly("Repositories"))
            );
            //wire AutoMapper for injection
            services.AddAutoMapper(c=>c.AddProfile<AutoMappingProfile>(),typeof(Startup));
            RegisterRepositories(services);
            services.AddControllers();
        }
        private static void RegisterRepositories(IServiceCollection services)
        {
            //services.AddScoped<IMessageRepository, MockMessageRepository>();
            services.AddScoped<IMessageRepository, EfMessageRepository>();
        }
        /// <summary>
        ///  Method will load the configuration file and configure logging
        /// Logging to a file within [Temp path]\[LogFile]
        /// </summary>
        /// <param name="configuration">Configuration collection</param>
        private static void WireLogging(IConfiguration configuration)
        {
            var fileName = configuration.GetValue<string>("LogFile");
            var logfile = Path.Combine(Path.GetTempPath(), fileName);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) //specifying the configuration settings
                .Enrich.FromLogContext() //enrich with contextual information
                .WriteTo.File(new RenderedCompactJsonFormatter(), logfile)
                .WriteTo.Console()
                .CreateLogger();
            Log.Logger.Information("Wired Logging for HelloWorldWebApi");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {   
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection(); //redirect to https
            app.UseStatusCodePages(); //adds support for text-only headers http Status codes (i.e. 400/404/500 and etc)
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization(); //not going to define
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); //going to use the default controller mapping
                
                ////can use the below to bypass controller/Action method
                //var message = new MessageDTO{Message = "Hello World!"};
                //var jsonMessage = JsonConvert.SerializeObject(message);
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync(jsonMessage);
                //});
            });
            
        }
    }

    
}
