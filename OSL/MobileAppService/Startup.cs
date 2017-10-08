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
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = AuthenticationFailed
                    };
                });

            services.AddAuthentication("MyCookieAuthenticationScheme")
                .AddCookie("MyCookieAuthenticationScheme", options => {
                    options.AccessDeniedPath = "/forbidden";
                    options.LoginPath = "/login";
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

            app.UseAuthentication();

            ScopeRead = Configuration["Authentication:AzureAd:ScopeRead"];
            ScopeWrite = Configuration["Authentication:AzureAd:ScopeWrite"];

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.Run(async (context) => context.Response.Redirect("/swagger"));
        }

        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            var s = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.ContentLength = s.Length;
            arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }
    }
}
