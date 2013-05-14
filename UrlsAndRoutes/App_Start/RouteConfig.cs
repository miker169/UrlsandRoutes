namespace UrlsAndRoutes
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("", "x{controller}/{action}", new { controller = "Home", action = "Index" });

            routes.MapRoute("MyRoute", "{controller}/{action}",
                new {controller = "Home", action = "Index"});

            routes.MapRoute("", "Pulic/{controller}/{action}", new { controller = "Home", action = "Index" });
        }
    }
}