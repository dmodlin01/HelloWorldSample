using AutoMapper;
using CatalogServices;
using HelloWorldWeb.AutoMap;
using HelloWorldWeb.HttpHandlers;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Repositories;
using Repositories.EF;
using Repositories.User;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

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
            //Add Attribute-based authorization
            AddAuthorization(services);
            services.AddHttpContextAccessor(); //needed to inject IHttpContextAccessor

            services.AddTransient<BearerTokenHandler>(); //add the delegating handler to provision requests with the BearerToken (access token)
            //Add Identity Server Client
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });
            //add Web Api client
            services.AddHttpClient("HelloWorldApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44349/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            })
                .AddHttpMessageHandler<BearerTokenHandler>(); //directing the requests to the API to be handled with the BearerTokenHandler
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; //Cookie
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; //OpenIDConnect
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.AccessDeniedPath = "/Authorization/AccessDenied";
                    }
                    )//configure cookie handler
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
                    options.Scope.Add("roles"); //Add roles scope (Identity Resource) to bring the user roles
                    options.Scope.Add("helloworldapi"); //add scope for API
                    options.Scope.Add("userlevel"); //Add userlevel scope (Identity Resource) to bring the userlevel claim (needed for attribute-based authorization)
                    options.ClaimActions.MapUniqueJsonKey("role", "role"); //create a mapping for the role claims
                    options.ClaimActions.MapUniqueJsonKey("userlevel", "userlevel");
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
                    //options.ClaimActions.MapUniqueJsonKey("subscriptionlevel", "subscriptionlevel");
                    //options.ClaimActions.MapUniqueJsonKey("country", "country");
                    options.SaveTokens = true;
                    options.ClientSecret = "secret";
                    options.GetClaimsFromUserInfoEndpoint = true; //get user info claims from the IDP
                    options.TokenValidationParameters = new TokenValidationParameters //map user roles to 
                    {
                        NameClaimType = JwtClaimTypes.GivenName,
                        RoleClaimType = JwtClaimTypes.Role
                    };
                });

            //wire AutoMapper for injection
            services.AddAutoMapper(c => c.AddProfile<HelloWorldWebMappingProfile>(), typeof(Startup));
            //wire EF db context
            services.AddDbContext<AppDbContext>(options =>
                //options.UseSqlServer(Configuration.GetConnectionString("HelloWorldConnection"), b => b.MigrationsAssembly("Repositories"))
                options.UseSqlServer(Configuration.GetConnectionString("HelloWorldConnection"), b => b.MigrationsAssembly("HelloWorldWeb"))
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
            services.AddScoped<IUserRepository, EfUserRepository>();
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

        /// <summary>
        /// Configure and add attribute-based authorization
        /// </summary>
        /// <param name="services"></param>
        private static void AddAuthorization(IServiceCollection services)
        {
            var authSchemes = new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme };
            var canAddMessagePolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(authSchemes)
                .RequireAuthenticatedUser()
                  //.RequireRole("Admin")
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
                //.RequireRole("Admin")
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

            var canManageUsersPolicy = new AuthorizationPolicyBuilder() //only admins with mid/senior level can manage users
                .AddAuthenticationSchemes(authSchemes)
                .RequireAuthenticatedUser()
                .RequireRole("Admin")
                .RequireAssertion(ctx =>
                {
                    return ctx.User.HasClaim("userlevel", "mid") || ctx.User.HasClaim("userlevel", "senior");
                })
               .Build();

            var canViewDeliveryInfoPolicy = new AuthorizationPolicyBuilder() //only admins with mid/senior level can manage users
                .AddAuthenticationSchemes(authSchemes)
                .RequireAuthenticatedUser()
                .RequireAssertion(ctx =>
                {
                    return ctx.User.HasClaim("role", "Admin") ||
                          (ctx.User.HasClaim("role", "PowerUser") && ctx.User.HasClaim("userlevel", "mid") || ctx.User.HasClaim("userlevel", "senior"));
                })
               .Build();

            //add Policy Authorization
            services.AddAuthorization(authOpt =>
            {
                authOpt.AddPolicy("CanAddMessage", canAddMessagePolicy);
                authOpt.AddPolicy("CanEditMessage", canEditMessagePolicy);
                authOpt.AddPolicy("CanDeleteMessage", canDeleteMessagePolicy);
                authOpt.AddPolicy("CanViewDeliveryInfo", canViewDeliveryInfoPolicy);
                authOpt.AddPolicy("CanManageUsers", canManageUsersPolicy);
            });
        }
    }
}
