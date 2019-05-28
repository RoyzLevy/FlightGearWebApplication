using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FlightGearWebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route for timed displaying and saving plane route to file.
            routes.MapRoute("save", "save/{ip}/{port}/{time}/{timeout}/{filePath}",
            defaults: new { controller = "Map", action = "save" });

            // Route for timed display of plane route.
            routes.MapRoute("display", "display/{ip}/{port}/{time}",
            defaults: new { controller = "Map", action = "display", time = UrlParameter.Optional });

            // Draw Route
            routes.MapRoute("draw", "draw",
            defaults: new { controller = "Map", action = "draw", time = UrlParameter.Optional });

            // Default route.
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Map", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
