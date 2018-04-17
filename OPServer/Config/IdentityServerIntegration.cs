using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OPServer.Components.IdServer;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerIntegration
    {
        public static IServiceCollection SetupIdentityServerIntegrationAndCORSPolicy(
            this IServiceCollection services,
            IConfiguration config,
            IHostingEnvironment environment,
            ILogger log,
            bool sslIsAvailable,
            bool disableIdentityServer,
            out bool didSetupIdServer
            )
        {
            didSetupIdServer = false;

            if (!disableIdentityServer)
            {
                try
                {
                    //customization
                    // not sure these are needed, need to comment out and test xamarin app
                    //services.AddTransient<ICorsPolicyService, IdServerCorsPolicy>();
                    services.AddTransient<IRedirectUriValidator, IdServerRedirectValidator>();

                    var idsBuilder = services.AddIdentityServerConfiguredForCloudscribe(options =>
                    {
                        //https://github.com/IdentityServer/IdentityServer4/issues/501
                        //problem when using androidclient the requestcomes in as http://10.0.2.2:50405
                        //IDX10205: Issuer validation failed. Issuer: 'http://10.0.2.2:50405'. 
                        //Did not match: validationParameters.ValidIssuer: 'null' or validationParameters.ValidIssuers: 'http://localhost:50405'.
                        //surprisingly this change does not break the other clients in the demo
                        // it seems that for the xamarin client the issuer must match the authority
                        // but this does not seem to be the case for the other clients

                        options.IssuerUri = "http://10.0.2.2:50405";

                    }).AddCloudscribeCoreNoDbIdentityServerStorage()
                      .AddCloudscribeIdentityServerIntegrationMvc();

                    if (environment.IsProduction())
                    {
                        // *** IMPORTANT CONFIGURATION NEEDED HERE *** 
                        // can't use .AddDeveloperSigningCredential in production it will throw an error
                        // https://identityserver4.readthedocs.io/en/dev/topics/crypto.html
                        // https://identityserver4.readthedocs.io/en/dev/topics/startup.html#refstartupkeymaterial
                        // you need to create an X.509 certificate (can be self signed)
                        // on your server and configure the cert file path and password name in appsettings.json
                        // OR change this code to wire up a certificate differently
                        log.LogWarning("setting up identityserver4 for production");
                        var certPath = config.GetValue<string>("AppSettings:IdServerSigningCertPath");
                        var certPwd = config.GetValue<string>("AppSettings:IdServerSigningCertPassword");
                        if (!string.IsNullOrWhiteSpace(certPath) && !string.IsNullOrWhiteSpace(certPwd))
                        {
                            var cert = new X509Certificate2(
                            File.ReadAllBytes(certPath),
                            certPwd,
                            X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);

                            idsBuilder.AddSigningCredential(cert);
                            didSetupIdServer = true;
                        }
                        else
                        {
                            idsBuilder.AddDeveloperSigningCredential(); // don't use this for production
                            didSetupIdServer = true;
                        }

                    }
                    else
                    {
                        idsBuilder.AddDeveloperSigningCredential(); // don't use this for production
                        didSetupIdServer = true;
                    }

                    services.AddCors(options =>
                    {
                        // this defines a CORS policy called "default"
                        options.AddPolicy("default", policy =>
                        {
                            policy.AllowAnyOrigin()   //.WithOrigins("http://localhost:5010", "http://localhost:5011" , "http://localhost:5012", "http://localhost:50405", "https://localhost:44363")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                    });

                }
                catch (Exception ex)
                {
                    log.LogError($"failed to setup identityserver4 {ex.Message} {ex.StackTrace}");
                }

            }



            return services;
        }

        public static IServiceCollection SetupIdentityServerApiAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication()
               .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   //TO Test the xamarin client requires https so use IIS with the https url
                   //options.Authority = "http://127.0.0.1:50405";
                   options.Authority = "http://localhost:50405";

                   options.ApiName = "idserverapi";
                   //options.ApiSecret = "secret";
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;

               });

            return services;
        }

    }
}
