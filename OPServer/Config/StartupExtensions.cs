
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OPServer.Components.IdServer;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection SetupDataProtection(
            this IServiceCollection services,
            IConfiguration config,
            IHostingEnvironment environment
            )
        {
            // **** VERY IMPORTANT *****
            // https://www.cloudscribe.com/docs/configuring-data-protection
            // data protection keys are used to encrypt the auth token in the cookie
            // and also to encrypt social auth secrets and smtp password in the data storage
            // therefore we need keys to be persistent in order to be able to decrypt
            // if you move an app to different hosting and the keys change then you would have
            // to update those settings again from the Administration UI

            // for IIS hosting you should use a powershell script to create a keyring in the registry
            // per application pool and use a different application pool per app
            // https://docs.microsoft.com/en-us/aspnet/core/publishing/iis#data-protection
            // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?tabs=aspnetcore2x
            if (environment.IsProduction())
            {
                // If using Azure for production the uri with sas token could be stored in azure as environment variable or using key vault
                // but the keys go in azure blob storage per docs https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers
                // this is false by default you should set it to true in azure environment variables
                var useBlobStroageForDataProtection = config.GetValue<bool>("AppSettings:UseAzureBlobForDataProtection");
                // best to put this in azure environment variables instead of appsettings.json
                var storageConnectionString = config["AppSettings:DataProtectionBlobStorageConnectionString"];
                if (useBlobStroageForDataProtection && !string.IsNullOrWhiteSpace(storageConnectionString))
                {
                    var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(storageConnectionString);
                    var client = storageAccount.CreateCloudBlobClient();
                    var container = client.GetContainerReference("key-container");
                    // The container must exist before calling the DataProtection APIs.
                    // The specific file within the container does not have to exist,
                    // as it will be created on-demand.
                    container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
                    services.AddDataProtection()
                        .PersistKeysToAzureBlobStorage(container, "keys.xml");

                }
                else
                {
                    services.AddDataProtection();
                }
            }
            else
            {
                // dp_Keys folder should be added to .gitignore so the keys don't go into source control
                // ie add a line with: **/dp_keys/**
                // to your .gitignore file
                string pathToCryptoKeys = Path.Combine(environment.ContentRootPath, "dp_keys");
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new System.IO.DirectoryInfo(pathToCryptoKeys))
                    ;
            }

            return services;
        }

        public static IServiceCollection SetupDataStorage(
            this IServiceCollection services,
            IConfiguration config
            )
        {
            services.AddCloudscribeCoreNoDbStorage();
            services.AddCloudscribeLoggingNoDbStorage(config);

            return services;
        }

        public static IServiceCollection SetupCloudscribeFeatures(
            this IServiceCollection services,
            IConfiguration config
            )
        {
            services.AddCloudscribeLogging();
            services.AddCloudscribeCoreMvc(config);

            return services;
        }

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
                    services.AddTransient<ICorsPolicyService, IdServerCorsPolicy>();
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

        public static IServiceCollection SetupMvc(
            this IServiceCollection services,
            bool sslIsAvailable
            )
        {
            services.Configure<MvcOptions>(options =>
            {
                if (sslIsAvailable)
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                }
            });

            // it is recommended to use lower case urls
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddMvc()
                .AddRazorOptions(options =>
                {
                    options.AddCloudscribeViewLocationFormats();
                    options.AddCloudscribeCommonEmbeddedViews();
                    options.AddCloudscribeNavigationBootstrap3Views();
                    options.AddCloudscribeCoreBootstrap3Views();
                    options.AddCloudscribeFileManagerBootstrap3Views();
                    options.AddCloudscribeLoggingBootstrap3Views();
                    options.AddCloudscribeCoreIdentityServerIntegrationBootstrap3Views();

                    options.ViewLocationExpanders.Add(new cloudscribe.Core.Web.Components.SiteViewLocationExpander());
                });

            

            return services;
        }

        public static IServiceCollection SetupLocalization(
            this IServiceCollection services
            )
        {
            // optional but recommended if you need localization 
            // uncomment to use cloudscribe.Web.localization https://github.com/joeaudette/cloudscribe.Web.Localization
            //services.Configure<GlobalResourceOptions>(Configuration.GetSection("GlobalResourceOptions"));
            //services.AddSingleton<IStringLocalizerFactory, GlobalResourceManagerStringLocalizerFactory>();

            services.AddLocalization(options => options.ResourcesPath = "GlobalResources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("en-GB"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("fr"),
                };

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;

                // You can change which providers are configured to determine the culture for requests, or even add a custom
                // provider with your own logic. The providers will be asked in order to provide a culture for each request,
                // and the first to provide a non-null result that is in the configured supported cultures list will be used.
                // By default, the following built-in providers are configured:
                // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
                // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
                // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
                //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
                //{
                //  // My custom request culture logic
                //  return new ProviderCultureResult("en");
                //}));
            });

            return services;
        }

    }
}
