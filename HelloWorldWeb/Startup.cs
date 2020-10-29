using AutoMap;
using AutoMapper;
using CatalogServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Repositories;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using HelloWorldWeb.AutoMap;
using IdentityServer4;

namespace HelloWorldWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            WireLogging(configuration);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); //remove the mapping of claims
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //wire Services and Repositories for DI
            RegisterServicesAndRepositories(services);
            services.AddControllersWithViews()
                 .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });
            services.AddHttpClient("HelloWorldApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44349/helloworld");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; //Cookie
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; //OpenIDConnect
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)//configure cookie handler
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:5001/"; //auth server (IDP)
                    options.ClientId = "helloworldwebclient";
                    options.ResponseType = "code";
                    //options.UsePkce = true; //default setting
                    options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId); //default value
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Profile); //default value
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Address); //to bring in address info
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Email); //to bring in e-mail info
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Phone);
                    //options.Scope.Add("roles");
                    //options.ClaimActions.Remove("nbf"); //remove the filter for the not before claim
                    //options.Scope.Add("imagegalleryapi");
                    //options.Scope.Add("subscriptionlevel");
                    //options.Scope.Add("country");
                    //options.Scope.Add("offline_access");
                    options.ClaimActions.Remove("phone_number"); //remove session id claim
                    options.ClaimActions.DeleteClaim("sid"); //remove session id claim
                    options.ClaimActions.DeleteClaim("idp"); //remove the idp claim
                    options.ClaimActions.DeleteClaim("s_hash");
                    options.ClaimActions.DeleteClaim("auth_time");
                    //options.ClaimActions.MapUniqueJsonKey("role", "role");
                    //options.ClaimActions.MapUniqueJsonKey("subscriptionlevel", "subscriptionlevel");
                    //options.ClaimActions.MapUniqueJsonKey("country", "country");
                    options.SaveTokens = true;
                    options.ClientSecret = "secret";
                    options.GetClaimsFromUserInfoEndpoint = true; //get user info claims from the IDP
                    //options.TokenValidationParameters = new TokenValidationParameters
                    //{
                    //    NameClaimType = JwtClaimTypes.GivenName,
                    //    RoleClaimType = JwtClaimTypes.Role
                    //};
                });
            //wire AutoMapper for injection
            services.AddAutoMapper(c => c.AddProfile<HelloWorldWebMappingProfile>(), typeof(Startup));
            //wire EF db context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HelloWorldConnection"), b => b.MigrationsAssembly("Repositories"))
            );
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); //use oidc middleware
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private static void RegisterServicesAndRepositories(IServiceCollection services)
        {
            services.AddScoped<IMessageRepository, WebApiMessageRepository>();
            //services.AddScoped<IMessageRepository, EFMessageRepository>();
            services.AddScoped<MessageService, WebApiMessageService>();
            services.AddScoped<IUserInfoRepository, IDPUserInfoRepository>();
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
            Log.Logger.Information("Wired Logging for HelloWorldWeb");
        }
    }
}
