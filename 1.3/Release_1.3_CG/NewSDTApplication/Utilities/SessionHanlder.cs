using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace NewSDTApplication.Utilities
{
    public class SessionHanlder
    {
        public static bool CheckFirstHit { get; set; }
        public static bool IsSessionExpired { get; set; }
    }

    public class SessionExpireAttribute : ActionFilterAttribute
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            logger.Debug("---------------------START-------------------------------------------- ");
            logger.Debug("SessionExpire;OnActionExecuting; Start from here;");

            HttpContext ctx = HttpContext.Current;

            logger.Debug(HttpContext.Current.Session["SiebelHttpPostParams"]);
            if (HttpContext.Current.Session["SiebelHttpPostParams"] == null)
            {
                logger.Debug(HttpContext.Current.Request.Form["ACTIVITY_ID"]);
                if (string.IsNullOrEmpty(HttpContext.Current.Request.Form["ACTIVITY_ID"]))
                {
                    logger.Debug(SessionHanlder.IsSessionExpired);
                    if (SessionHanlder.IsSessionExpired == true)
                    {
                        filterContext.Result = new RedirectResult("~/Shared/Error");
                        SessionHanlder.IsSessionExpired = false;
                        logger.Debug(SessionHanlder.IsSessionExpired);
                        base.OnActionExecuting(filterContext);

                        return;
                    }
                    else
                    {
                        logger.Debug(SessionHanlder.IsSessionExpired);
                        SessionHanlder.IsSessionExpired = false;
                        logger.Debug(SessionHanlder.IsSessionExpired);
                        return;
                    }
                }
                logger.Debug(SessionHanlder.IsSessionExpired);
            }
          

            logger.Debug(HttpContext.Current.Request.Form["ACTIVITY_ID"]);
            logger.Debug("SessionExpire;OnActionExecuting; ACTIVITY_ID  End hit;");
            logger.Debug("---------------------------END-------------------------------------- ");


        }
    }
}