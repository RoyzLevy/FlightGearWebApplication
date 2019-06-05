using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FlightGearWebApp
{
    /// <summary>
    /// This class directs URL requests to various views through the mapController
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// This function directs a URL request to a matching view through the mapController
        /// </summary>
        /// <param name="routes">The requested URL</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}"); //TODO: can we remove this?

            // Route for timed displaying and saving plane route to file.
            routes.MapRoute("save", "save/{ip}/{port}/{time}/{timeout}/{filePath}",
            defaults: new { controller = "Map", action = "save" });

            // Route for timed Display of plane route.
            routes.MapRoute("display", "display/{ip}/{port}/{time}",
            defaults: new { controller = "Map", action = "display", time = UrlParameter.Optional });

            // Default route.
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Map", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
