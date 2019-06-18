using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationExtensions
    {

        public static AuthorizationOptions SetupAuthorizationPolicies(this AuthorizationOptions options)
        {
            //https://docs.asp.net/en/latest/security/authorization/policies.html

            options.AddCloudscribeCoreDefaultPolicies();
            options.AddCloudscribeLoggingDefaultPolicy();

            options.AddPolicy(
                "FileManagerPolicy",
                authBuilder =>
                {
                    authBuilder.RequireRole("Administrators", "Content Administrators");
                });

            options.AddPolicy(
                "FileManagerDeletePolicy",
                authBuilder =>
                {
                    authBuilder.RequireRole("Administrators", "Content Administrators");
                });

            options.AddPolicy(
                "IdentityServerAdminPolicy",
                authBuilder =>
                {
                    authBuilder.RequireRole("Administrators");
                });

            options.AddPolicy(
            "FileUploadPolicy",
            authBuilder =>
            {
                authBuilder.RequireRole("Administrators", "Content Administrators");
            });

            // add other policies here 

            return options;
        }


    }
}
