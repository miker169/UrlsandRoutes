namespace UrlsAndRoutes
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using UrlsAndRoutes.Infrasctructure;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = true;

            routes.IgnoreRoute("Content/{filename}.html");

            routes.MapRoute("DiskFile", "Content/StaticContent.html", new { controller = "Customer", action = "List" });

            routes.MapRoute(
                "ChromeRoute",
                "{*catchall}",
                new { controller = "Home", action = "Index" },
                new { customConstraint = new UserAgentConstraint("Chrome") },
                new[] {"URLsAndRoutes.Controllers"});

            routes.MapRoute(
                "MyRoute",
                "{controller}/{action}/{id}/{*catchall}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { controller = "^H.*", action="^Index$|About$",
                httpMethod = new HttpMethodConstraint("GET")},
                new[] { "URLsAndRoutes.Controllers" });


        }
    }
}