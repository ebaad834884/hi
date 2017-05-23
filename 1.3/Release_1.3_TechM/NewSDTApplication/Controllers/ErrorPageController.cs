using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewSDTApplication.Utilities;
namespace NewSDTApplication.Controllers
{
    public class ErrorPageController : Controller
    {
        //
        // GET: /ErrorPage/
        public ActionResult Index()
        {
            if (SessionHanlder.IsSessionExpired)
            {
                ViewBag.Exception = "Session Expired, Please Re-launch the SDT Application from Siebel";
                SessionHanlder.IsSessionExpired = false;
                SessionHanlder.CheckFirstHit = false;
            }
            return View();
        }

        public ActionResult SessionExpired()
        {
            ViewBag.Exception = "Session Expired, Please re launch the SDT Application";

            return View();
        }
        public ActionResult NotAuthorizedPage()
        {
            return View();
        }
        public ActionResult Error(int statusCode, Exception exception)
        {


            if (SessionHanlder.IsSessionExpired)
            {
                ViewBag.Exception = "Session got Expired,Please Re-launch the SDT Application from Siebel";
                SessionHanlder.IsSessionExpired = false;
                SessionHanlder.CheckFirstHit = false;
                return View();
            }

            else
            {
                Response.StatusCode = statusCode;
                ViewBag.StatusCode = statusCode + " Error";
                if (statusCode == 404)
                {
                    ViewBag.Exception = "Requested URL not found.";
                }
                else
                {
                    ViewBag.Exception = exception.Message;
                }

                return View();
            }
        }

        public ActionResult Errormsg()
        {

            string errorcode = TempData["ErrorCode"].ToString();
            string msg = TempData["Errormsg"].ToString();
            ViewBag.StatusCode = errorcode + " Error";

            if (errorcode == "404")
            {
                ViewBag.Exception = "Requested URL not found.";
            }
            else
            {
                ViewBag.Exception = msg;
            }


            return View("Error");
        }
    }
}