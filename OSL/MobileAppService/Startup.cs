using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Swagger;

using OSL.MobileAppService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System;

namespace OSL.MobileAppService
{
    public class Startup
    {
        public static string ScopeRead;
        public static string ScopeWrite;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Authority = string.Format("https://login.microsoftonline.com/tfp/{0}/{1}/v2.0/",
          Configuration["Authentication:AzureAd:Tenant"], Configuration["Authentication:AzureAd:Policy"]);
                    options.Audience = Configuration["Authentication:AzureAd:ClientId"];
                    options.RequireHttpsMetadata = false;
                })
                .AddCookie("CookieScheme", options => {
                    options.AccessDeniedPath = "/login";
                    options.LoginPath = "/login";
                    options.Cookie.HttpOnly = false;
                    options.Cookie.Name = "auth";
                    options.Cookie.Path = "/";
                    options.Cookie.Expiration = TimeSpan.FromDays(14);
                    options.SlidingExpiration = true;
                });

            services.AddMvc();
            services.AddSingleton(Configuration);
            services.AddSingleton<DonationRepository, DonationRepository>();
            services.AddSingleton<UserRepository, UserRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            app.UseAuthentication();

            ScopeRead = Configuration["Authentication:AzureAd:ScopeRead"];
            ScopeWrite = Configuration["Authentication:AzureAd:ScopeWrite"];

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.Run(async context => context.Response.Redirect("/users"));
        }
    }
}
