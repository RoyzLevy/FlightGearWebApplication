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


            routes.MapRoute("save", "save/{ip}/{port}/{time}/{timeout}/{filePath}",
            defaults: new { controller = "Map", action = "save" });

            routes.MapRoute("display", "display/{ip}/{port}/{time}",
            defaults: new { controller = "Map", action = "display" , time = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Map", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
