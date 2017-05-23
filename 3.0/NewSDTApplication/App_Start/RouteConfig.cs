using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NewSDTApplication
{
    public class RouteConfig
    {
        //Code Start-Hita - 12/05/2017 - US250/TA1680 
        private RouteConfig()
        {
        }
        //Code End-Hita - 12/05/2017 - US250/TA1680 

        public static void RegisterRoutes(RouteCollection routes)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            logger.Debug("----------------------------------------------------------------- ");
            logger.Debug("RouteConfig;RegisterRoutes; " + HttpContext.Current.Session);

            if (HttpContext.Current.Session != null)
            {
                logger.Debug("RouteConfig;RegisterRoutes; " + HttpContext.Current.Session);
                HttpContext.Current.Session.Abandon();
            }
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
