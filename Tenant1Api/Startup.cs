using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization()
                ;

            services.AddCors();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:50405" ;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api1";
                    options.ApiSecret = "secret"; 
                });

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin(); 
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
