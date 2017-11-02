using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Photoblog.Data;
using Photoblog.Filters;
using Photoblog.Utils;
using System.Text;
using System.Threading.Tasks;

namespace Photoblog {
    public class Startup {

        readonly IConfigurationRoot _configuration;
        readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
            _hostingEnvironment = env;
        }

        public void ConfigureServices(IServiceCollection services) {
            var settings = _configuration.Get<Settings>();

            services.Configure<Settings>(_configuration);

            // Configure JWT authorization
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.AppSecret)),
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        // Look for authorization token in a cookie
                        // This is not the prefered way to do it, but it is required for images
                        if (context.Request.Cookies.TryGetValue("Authorization", out string token)) {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddMvc(o => {
                // Authorize all requests
                var authorizationPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                o.Filters.Add(new AuthorizeFilter(authorizationPolicy));

                o.Filters.Add(typeof(ExceptionHandlingFilter));
            });

            services.AddSingleton<IDataStore, FileDataStore>();
            services.AddTransient<IThumbnailProvider, CachedThumbnailProvider>();

            services.AddMemoryCache();
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app) {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseMvc();
        }

    }
}
