using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
            Configuration = configuration;
            WireLogging(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterRepositories(services);
            services.AddControllers();
        }
        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IMessageRepository, MockMessageRepository>();
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
