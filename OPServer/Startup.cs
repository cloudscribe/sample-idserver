using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace OPServer
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IHostingEnvironment env,
            ILogger<Startup> logger
            )
        {
            Configuration = configuration;
            Environment = env;
            _log = logger;
            SslIsAvailable = Configuration.GetValue<bool>("AppSettings:UseSsl");
            DisableIdentityServer = Configuration.GetValue<bool>("AppSettings:DisableIdentityServer");
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; set; }
        private bool SslIsAvailable { get; set; }
        private bool DisableIdentityServer { get; set; }
        private bool didSetupIdServer = false;
        private ILogger _log;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //// **** VERY IMPORTANT *****
            // This is a custom extension method in Config/DataProtection.cs
            // These settings require your review to correctly configur data protection for your environment
            services.SetupDataProtection(Configuration, Environment);
            
            services.AddAuthorization(options =>
            {
                //https://docs.asp.net/en/latest/security/authorization/policies.html
                //** IMPORTANT ***
                //This is a custom extension method in Config/Authorization.cs
                //That is where you can review or customize or add additional authorization policies
                options.SetupAuthorizationPolicies();

            });

            //// **** IMPORTANT *****
            // This is a custom extension method in Config/CloudscribeFeatures.cs
            services.SetupDataStorage(Configuration);

            //*** Important ***
            // This is a custom extension method in Config/IdentityServerIntegration.cs
            // You should review this and understand what it does before deploying to production
            services.SetupIdentityServerIntegrationAndCORSPolicy(
                Configuration,
                Environment,
                _log,
                SslIsAvailable,
                DisableIdentityServer,
                out didSetupIdServer
                );

            //*** Important ***
            // This is a custom extension method in Config/CloudscribeFeatures.cs
            services.SetupCloudscribeFeatures(Configuration);
            
            //*** Important ***
            // This is a custom extension method in Config/Localization.cs
            services.SetupLocalization();

           
            //*** Important ***
            // This is a custom extension method in Config/RoutingAndMvc.cs
            services.SetupMvc(SslIsAvailable);

            //*** Important ***
            // This is a custom extension method in Config/IdentityServerIntegration.cs
            services.SetupIdentityServerApiAuthentication();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IOptions<cloudscribe.Core.Models.MultiTenantOptions> multiTenantOptionsAccessor,
            IOptions<RequestLocalizationOptions> localizationOptionsAccessor
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/oops/Error");
            }

            app.UseForwardedHeaders();
            app.UseStaticFiles();

            app.UseRequestLocalization(localizationOptionsAccessor.Value);

            app.UseCors("default");

            var multiTenantOptions = multiTenantOptionsAccessor.Value;

            app.UseCloudscribeCore(
                    loggerFactory,
                    multiTenantOptions,
                    SslIsAvailable);

            if (!DisableIdentityServer && didSetupIdServer)
            {
                try
                {
                    app.UseIdentityServer();
                }
                catch (Exception ex)
                {
                    _log.LogError($"failed to setup identityserver4 {ex.Message} {ex.StackTrace}");
                }
            }

            app.UseMvc(routes =>
            {
                var useFolders = multiTenantOptions.Mode == cloudscribe.Core.Models.MultiTenantMode.FolderName;
                //*** IMPORTANT ***
                // this is in Config/RoutingExtensions.cs
                // you can change or add routes there
                routes.UseCustomRoutes(useFolders);

            });
            

        }
        
        
    }
}
