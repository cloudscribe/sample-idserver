using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace Microsoft.AspNetCore.Builder
{
    public static class RoutingExtensions
    {
        public static IRouteBuilder UseCustomRoutes(this IRouteBuilder routes, bool useFolders)
        {
            routes.AddCloudscribeFileManagerRoutes();

            if (useFolders)
            {
                routes.MapRoute(
                   name: "foldererrorhandler",
                   template: "{sitefolder}/oops/error/{statusCode?}",
                   defaults: new { controller = "Oops", action = "Error" },
                   constraints: new { name = new cloudscribe.Core.Web.Components.SiteFolderRouteConstraint() }
                );

                routes.MapRoute(
                    name: "folderdefault",
                    template: "{sitefolder}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" },
                    constraints: new { name = new cloudscribe.Core.Web.Components.SiteFolderRouteConstraint() }
                    );

            }

            routes.MapRoute(
                name: "errorhandler",
                template: "oops/error/{statusCode?}",
                defaults: new { controller = "Oops", action = "Error" }
                );

            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}"
                );

            return routes;
        }

    }
}
