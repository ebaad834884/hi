using SDTBooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NewSDTApplication.Controllers;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace NewSDTApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_beginRequest()
        {
            if (Request.Cookies["AccessTokenObject"] == null)
            {
                HttpCookie aCookie = new HttpCookie("AccessTokenObject");
                Response.Cookies.Add(aCookie);

            }

        }
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        //protected void Application_Close()
        //{
        //    log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
        //    AreaRegistration.RegisterAllAreas();
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //    BundleConfig.RegisterBundles(BundleTable.Bundles);
        //}
        void Session_End(Object sender, EventArgs E)
        {

            //HttpContext.Current.RewritePath("ErrorPage/SessionExpired");
           // HttpContextWrapper contextWrapper = new HttpContextWrapper(this.Context);

           // var routeData = new RouteData();

           // routeData.Values.Add("controller", "ErrorPage");

           // routeData.Values.Add("action", "SessionExpired");

           // routeData.Values.Add("exception", "SessionExpired");
           //// Response.TrySkipIisCustomErrors = true;
           // IController controller = new ErrorPageController();
           // RequestContext requestContext = new RequestContext(contextWrapper, routeData);
           // controller.Execute(requestContext);
           // Response.End();
            //controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));

           // Response.End();

            // Clean up session resources
        }

        public void Application_Error(Object sender, EventArgs e)
        {

            Exception exception = Server.GetLastError();

            Server.ClearError();

            var routeData = new RouteData();

            routeData.Values.Add("controller", "ErrorPage");

            routeData.Values.Add("action", "Error");

            routeData.Values.Add("exception", exception);

            if (exception.GetType() == typeof(HttpException))
            {

                routeData.Values.Add("statusCode", ((HttpException)exception).GetHttpCode());

            }

            else
            {

                routeData.Values.Add("statusCode", 500);

            }

            Response.TrySkipIisCustomErrors = true;

            IController controller = new ErrorPageController();

            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                      
            Response.End();

        }
    }
}
