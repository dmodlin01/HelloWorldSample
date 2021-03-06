using AutoMap;
using AutoMapper;
using HelloWorldWebAPI.Authorization;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
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
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace HelloWorldWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //When testing via xUnit, the configuration would not be loaded (and the appsettings.json would not be available), as such need to build/load it
            if (!((ConfigurationRoot)configuration).Providers.Any(p => p is JsonConfigurationProvider))
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
            //swagger
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HelloWorldWebAPI", Version = "v1" });
            });
            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()
           

            services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);
            AddAuthorization(services);
            services.AddHttpContextAccessor(); //needed to inject IHttpContextAccessor
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme) //use Bearer token authentication
                .AddIdentityServerAuthentication(o =>
                {
                    o.Authority =
                        "https://localhost:5001"; //auth server (IDP) is going to be used for authorization - validates access tokens
                    o.ApiName =
                        "helloworldapi"; //used to check if the api is a valid audience in the token (if token is intended for it)
                });
            //wire EF db context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HelloWorldConnection"), b => b.MigrationsAssembly("Repositories"))
            );
            //wire AutoMapper for injection
            services.AddAutoMapper(c => c.AddProfile<AutoMappingProfile>(), typeof(Startup));
            RegisterRepositories(services);

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
            app.UseAuthentication(); //use the middleware (defined in ConfigureServices) for Authentication
            app.UseAuthorization(); //use the middleware (defined in ConfigureServices) for Authorization
            //app.UseEndpoints(endpoints =>
            //{
            //    //endpoints.MapControllers(); //going to use the default controller mapping

            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelloWorldWebAPI V1");
            });
        }
        private static void AddAuthorization(IServiceCollection services)
        {
            var authSchemes = new[] { IdentityServerAuthenticationDefaults.AuthenticationScheme };
            var canAddMessagePolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(authSchemes)
                .RequireAuthenticatedUser()
                  .RequireAssertion(ctx =>
                  {
                      return ctx.User.HasClaim("role", "Admin") ||
                             ctx.User.HasClaim("userlevel", "senior") ||
                             ctx.User.HasClaim("userlevel", "mid");
                  })
                  .Build();

            var canEditMessagePolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(authSchemes)
                .RequireAuthenticatedUser()
                .RequireAssertion(ctx =>
                {
                    return ctx.User.HasClaim("role", "Admin") ||
                           ctx.User.HasClaim("userlevel", "senior") ||
                           (ctx.User.HasClaim("role", "PowerUser") && ctx.User.HasClaim("userlevel", "mid"));
                })
                .Build();

            var canDeleteMessagePolicy = new AuthorizationPolicyBuilder() //only admins or senior powerusers should be able to delete
                .AddAuthenticationSchemes(authSchemes)
                .RequireAuthenticatedUser()
                 .RequireAssertion(ctx =>
                 {
                     return ctx.User.HasClaim("role", "Admin") ||
                            (ctx.User.HasClaim("role", "PowerUser") && ctx.User.HasClaim("userlevel", "senior"));
                 })
                .Build();

            var canViewSpecificMessagePolicy = new AuthorizationPolicyBuilder() //only admins or senior powerusers should be able to delete
                .AddAuthenticationSchemes(authSchemes)
                .RequireAuthenticatedUser()
                 .AddRequirements(new MustBeMessageRecipientRequirement())
                .Build();

            services.AddScoped<IAuthorizationHandler, MustBeMessageRecipientHandler>(); //register the custom auth handler
            //add Policy Authorization
            services.AddAuthorization(authOpt =>
            {
                authOpt.AddPolicy("CanAddMessage", canAddMessagePolicy);
                authOpt.AddPolicy("CanEditMessage", canEditMessagePolicy);
                authOpt.AddPolicy("CanDeleteMessage", canDeleteMessagePolicy);
                authOpt.AddPolicy("CanViewSpecificMessage", canViewSpecificMessagePolicy);

            });
        }
    }


}
