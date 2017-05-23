using NewSDTApplication.Models;
using NewSDTApplication.Utilities;
using SDTLogger;
using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Configuration;
using NewSDTApplication.ScheduleServiceDev1;
using System.Web;
using System.Text;
using System.Collections;
namespace SDTBooking.Controllers
{
    [SessionExpire]
    public class RequestAppointmentBookingController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        CountriesTimeZoneConversion objCountriesTimeZoneConversion;
        CallClickSerrvice objCallClickService;
        bool IsExtendedSlots;
        IEnumerable<AppointmentSlots> inSLASlotsLists;
        IEnumerable<AppointmentSlots> objAppSlotsWithESLS;
        //Addition of IP address in logs - US185 - 18/4/2017 by Ebaad (This line will provide us with Client IP Address)
        string IP = "IP: " + (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();

        /// <summary>
        /// Retrieves slots for the appointment request.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetAppointments()
        {
            logger.Debug("In RequestAppointmentBookingController GetAppointments method | Start" + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            DateTime startTime = DateTime.Now;
            IndextoRequest resindextoreq = new IndextoRequest();
            resindextoreq = (IndextoRequest)Session["IndextoRequest"];
            var objser = new NewSDTApplication.Utilities.CallClickSerrvice();
            var objserWithESLS = new NewSDTApplication.Utilities.CallClickSerrvice();
            IEnumerable<AppointmentSlots> updatedSlots = null;
            try
            {
                IsExtendedSlots = false;
                objCallClickService = new CallClickSerrvice();

                //GetESLSByStandardOperation will give ES and LS dates for fetching INSLA
                DateTime startTimeTx1 = DateTime.Now;
                var EsAndLS = objCallClickService.GetESLSByStandardOperation(resindextoreq.Duration);
                DateTime endTimeTx1 = DateTime.Now;
                TimeSpan Tx1 = endTimeTx1.Subtract(startTimeTx1);
                
                Session["EsAndLS"] = EsAndLS;
                if (EsAndLS != null)
                {
                    logger.Debug("In RequestAppointmentBooking controller GetAppointment Method to get ES and LS | Call to ClickCallService.cs (objCallClickService.GetESLSByStandardOperation) ~ " + IP + " ~ Request: with Duration: " + resindextoreq.Duration + " ~ Response : ES and LS: " + EsAndLS.Item1 + " , " + EsAndLS.Item2 + " ~ Call duration: " + Tx1.TotalMilliseconds );
                    if ((EsAndLS.Item1 != string.Empty && EsAndLS.Item2 != string.Empty) && (EsAndLS.Item1 != EsAndLS.Item2))
                    {

                        if (Session["SelectJobType"].ToString() != "Install")
                        {
                            DateTime startTimeTx2 = DateTime.Now;
                            inSLASlotsLists = await objser.ClickCallByOperation(Convert.ToDateTime(EsAndLS.Item1).ToString("dd/MM/yyyy HH:mm"), Convert.ToDateTime(EsAndLS.Item2).ToString("dd/MM/yyyy HH:mm"),
                               resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);
                            inSLASlotsLists.ToList().ForEach(cc => cc.SLA = "IN SLA");
                            DateTime endTimeTx2 = DateTime.Now;
                            TimeSpan Tx2 = endTimeTx2.Subtract(startTimeTx2);
                            logger.Debug("In RequestAppointmentBooking controller GetAppointment Method to retrieve INSLA slots for Installation | Call to ClickCallService.cs (objCallClickService.ClickCallByOperation) ~ " + IP + " ~ Request: with params: " + Convert.ToDateTime(EsAndLS.Item1).ToString("dd/MM/yyyy HH:mm") + " , " + Convert.ToDateTime(EsAndLS.Item2).ToString("dd/MM/yyyy HH:mm") + " ~ Call duration: " + Tx2.TotalMilliseconds);
                        }
                        else
                        {
                            DateTime startTimeTx3 = DateTime.Now;
                            inSLASlotsLists = await objser.ClickCallByOperation(Convert.ToDateTime(EsAndLS.Item1).ToString(), Convert.ToDateTime(EsAndLS.Item2).ToString(),
                                                       resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);
                            inSLASlotsLists.ToList().ForEach(cc => cc.SLA = "IN SLA");
                            DateTime endTimeTx3 = DateTime.Now;
                            TimeSpan Tx3 = endTimeTx3.Subtract(startTimeTx3);
                            logger.Debug("In RequestAppointmentBooking controller GetAppointment Method to retrieve INSLA slots | Call to ClickCallService.cs (objCallClickService.ClickCallByOperation) ~ " + IP + " ~ Request With params: " + Convert.ToDateTime(EsAndLS.Item1).ToString() + " , " + Convert.ToDateTime(EsAndLS.Item2).ToString() + " , " + resindextoreq.Duration + " , " + resindextoreq.profile + " , " + resindextoreq.PreferredFSEs + " , " + resindextoreq.FseSkillLevel + " , " + IsExtendedSlots + " ~ Response: List of INSLA Slots ~ Call duration: " + Tx3.TotalMilliseconds);
                        }
                    }
                }

                if (resindextoreq.earlyStart != string.Empty && resindextoreq.lateStart != string.Empty)
                {
                    if (Session["SelectJobType"].ToString() == "Install")
                    {
                        var appStart = Convert.ToDateTime(resindextoreq.earlyStart).ToString();
                        var appFinish = Convert.ToDateTime(resindextoreq.lateStart).ToString();
                        DateTime startTimeTx4 = DateTime.Now;
                        objAppSlotsWithESLS = await objser.ClickCallByOperation(appStart, appFinish, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);
                        DateTime endTimeTx4 = DateTime.Now;
                        TimeSpan Tx4 = endTimeTx4.Subtract(startTimeTx4);
                        logger.Debug("In RequestAppointmentBooking controller GetAppointment Method to retrieve slots for Installation | Call to ClickCallService.cs (objCallClickService.ClickCallByOperation) ~ " + IP + " ~ Request: With params: " + appStart + " , " + appFinish + " , " + resindextoreq.Duration + " , " + resindextoreq.profile + " , " + resindextoreq.PreferredFSEs + " , " + resindextoreq.FseSkillLevel + " , " + IsExtendedSlots + " ~ Response: Slots with ES and LS ~ Call duration: " + Tx4.TotalMilliseconds);
                    }
                    else
                    {
                        var appStart = DateTime.ParseExact(resindextoreq.earlyStart.TrimEnd(), "dd/MM/yyyy HH:mm", null);
                        var appFinish = DateTime.ParseExact(resindextoreq.lateStart.TrimEnd(), "dd/MM/yyyy HH:mm", null);
                        
                        DateTime startTimeTx5 = DateTime.Now;
                        objAppSlotsWithESLS = await objser.ClickCallByOperation(appStart.ToString("dd/MM/yyyy HH:mm"), appFinish.ToString("dd/MM/yyyy HH:mm"), resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);
                        DateTime endTimeTx5 = DateTime.Now;
                        TimeSpan Tx5 = endTimeTx5.Subtract(startTimeTx5);
                        logger.Debug("In RequestAppointmentBooking controller GetAppointment Method to retrieve slots | Call to ClickCallService.cs (objCallClickService.ClickCallByOperation) ~ " + IP + " ~ Request: With params: " + appStart.ToString("dd/MM/yyyy HH:mm") + " , " + appFinish.ToString("dd/MM/yyyy HH:mm") + " , " + resindextoreq.Duration + " , " + resindextoreq.profile + " , " + resindextoreq.PreferredFSEs + " , " + resindextoreq.FseSkillLevel + " ~ Response: Slots with ES and LS ~ Call duration: " + Tx5.TotalMilliseconds);
                    }
                }

                if (objAppSlotsWithESLS != null)
                {
                    //objAppSlotsWithESLS.Distinct()
                    //before
                    //request by user ES < EsAndLS.item1
                    if ((EsAndLS != null) && (EsAndLS.Item1 != string.Empty && EsAndLS.Item2 != string.Empty))
                    {
                        var beforeSLA = objAppSlotsWithESLS.Where(date => Convert.ToDateTime(date.EarlyStart)
                         < Convert.ToDateTime(EsAndLS.Item1));

                        //after
                        //request by user LS > EsAndLS.item2
                        var afterSLA = objAppSlotsWithESLS.Where(date => Convert.ToDateTime(date.LateStart)
                        > Convert.ToDateTime(EsAndLS.Item2));


                        afterSLA = (from lstafterSLA in afterSLA
                                    where !beforeSLA.Any(
                                                              x => x.EarlyStart == lstafterSLA.EarlyStart &&
                                                                   x.LateStart == lstafterSLA.LateStart)
                                    select lstafterSLA).ToList();


                        if (inSLASlotsLists != null)
                        {
                            var beforeSLASlots = (from lst1 in beforeSLA
                                                  where !inSLASlotsLists.Any(
                                                                            x => x.EarlyStart == lst1.EarlyStart &&
                                                                                 x.LateStart == lst1.LateStart)
                                                  select lst1).ToList();

                            var afterSLASlots = (from lst1 in afterSLA
                                                 where !inSLASlotsLists.Any(
                                                                           x => x.EarlyStart == lst1.EarlyStart &&
                                                                                x.LateStart == lst1.LateStart)
                                                 select lst1).ToList();

                            beforeSLASlots.ToList().ForEach(cc => cc.SLA = "Before SLA");
                            afterSLASlots.ToList().ForEach(cc => cc.SLA = "After SLA");

                            updatedSlots = inSLASlotsLists.Concat(beforeSLASlots).Concat(afterSLASlots);
                        }
                        else
                        {
                            beforeSLA.ToList().ForEach(cc => cc.SLA = "Before SLA");
                            afterSLA.ToList().ForEach(cc => cc.SLA = "After SLA");
                            updatedSlots = beforeSLA.Concat(afterSLA);
                        }
                    }
                    else
                    {
                        updatedSlots = objAppSlotsWithESLS.ToList();
                    }
                }
                else
                {
                    if (inSLASlotsLists != null)
                        updatedSlots = inSLASlotsLists.ToList();
                }

                if (resindextoreq.desiredDate == null) resindextoreq.desiredDate = string.Empty;
                if (!String.IsNullOrEmpty(resindextoreq.desiredDate))
                {
                    var desiredDt = DateTime.ParseExact(resindextoreq.desiredDate.TrimEnd(), "dd/MM/yyyy HH:mm", null);
                    var desiredDateSlots = updatedSlots.Where(x => (Convert.ToDateTime(x.EarlyStart) >= Convert.ToDateTime(desiredDt)) &&
          Convert.ToDateTime(x.LateStart) <= Convert.ToDateTime(desiredDt));

                    var nonDesiredDateSlots = (from lst1 in updatedSlots
                                               where !desiredDateSlots.Any(
                                                                         x => x.EarlyStart == lst1.EarlyStart &&
                                                                              x.LateStart == lst1.LateStart)
                                               select lst1).ToList();
                    updatedSlots = desiredDateSlots.Concat(nonDesiredDateSlots);
                }
                //Session["RequesAppointmentNormalSlots"] = updatedSlots;

                if (!string.IsNullOrEmpty(Session["objSiebelResponseforType"] as string))
                {
                    var eStart = "";
                    var lStart = "";

                    if (!string.IsNullOrEmpty(resindextoreq.earlyStart) && !string.IsNullOrEmpty(resindextoreq.lateStart))
                    {
                        if (Session["objSiebelResponseforType"].ToString() == "Installation")
                        {
                            eStart = Convert.ToDateTime(resindextoreq.earlyStart).ToString();
                            lStart = Convert.ToDateTime(resindextoreq.lateStart).ToString();
                        }
                        else
                        {
                            eStart = DateTime.Parse(resindextoreq.earlyStart, CultureInfo.GetCultureInfo("en-gb")).ToString();


                            lStart = DateTime.Parse(resindextoreq.lateStart, CultureInfo.GetCultureInfo("en-gb")).ToString();
                        }
                        if (resindextoreq.profile == "TWO HOURS")
                        {
                            updatedSlots = (from esList in updatedSlots
                                            where (Convert.ToDateTime(esList.EarlyStart) >= Convert.ToDateTime(eStart).AddHours(-1).AddMinutes(-59) &&
                                                Convert.ToDateTime(esList.LateStart) <= Convert.ToDateTime(lStart).AddHours(2))
                                            select esList
                                );
                        }
                        else if (resindextoreq.profile == "ONE HOUR")
                        {
                            updatedSlots = (from esList in updatedSlots
                                            where (Convert.ToDateTime(esList.EarlyStart) >= Convert.ToDateTime(eStart).AddMinutes(-59) &&
                                                Convert.ToDateTime(esList.LateStart) <= Convert.ToDateTime(lStart).AddHours(1))
                                            select esList
                                    );
                        }
                        else
                        {
                            if (Session["objSiebelResponseforType"].ToString() == "Installation")
                            {
                                eStart = Convert.ToDateTime(resindextoreq.earlyStart).ToShortDateString();
                            }
                            else
                            {
                                eStart = DateTime.Parse(resindextoreq.earlyStart, CultureInfo.GetCultureInfo("en-gb")).ToShortDateString();
                            }
                            updatedSlots = (from esList in updatedSlots
                                            where (Convert.ToDateTime(esList.EarlyStart) >= Convert.ToDateTime(eStart) &&
                                                Convert.ToDateTime(esList.LateStart) <= Convert.ToDateTime(lStart))
                                            select esList
                                    );
                        }
                    }


                }
                Session["RequesAppointmentNormalSlots"] = updatedSlots;
                var updatedlist = updatedSlots.Select((r, i) => new
                {
                    id = i,
                    score = Convert.ToInt32(r.Grade),//"900",
                    date = DateTime.Parse(r.EarlyStart, CultureInfo.GetCultureInfo("en-US")).ToString(),
                    AppStart = DateTime.Parse(r.EarlyStart, CultureInfo.GetCultureInfo("en-US")).ToShortTimeString(),
                    AppFinish = DateTime.Parse(r.LateStart, CultureInfo.GetCultureInfo("en-US")).ToShortTimeString(),
                    TentativeFse = r.PreferredFSE,
                    SSOID = r.SSOID,
                    RequiredFse = "false",
                    SLA = r.SLA,
                    Working = "Normal",
                    //deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code was used to fetch normal slots but it contained the POST data which is removed from thr Partial views as well)
                    //Bump = string.Empty,
                    SchedulingpolicyExtended = r.SchedulingpolicyExtendedSlots
                });


                if (!string.IsNullOrEmpty(Session["objSiebelResponseforType"] as string))
                {


                    //if (Session["objSiebelResponseforType"].ToString() != "PM")
                    //{
                    //    updatedlist = updatedlist.OrderByDescending(j => j.score);

                    //}
                    //Changed to order by asc with date
                    if (Session["objSiebelResponseforType"].ToString() != "PM")
                    {
                        updatedlist.OrderBy(d => DateTime.Parse(d.date));

                    }
                }
                // var jsonSerialiser = new JavaScriptSerializer();
                // var json = jsonSerialiser.Serialize(updatedlist);
                foreach (var item in updatedlist)
                {
                    logger.Debug("In RequestAppointmentBookingController GetAppointments method | Updated list Logs " + item.date + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                }

                updatedlist = updatedlist.GroupBy(p => new { p.date, p.AppStart, p.AppFinish }).Select(g => g.First()).ToList();
                var jsonData = new
                {
                    total = 1,
                    page = 1,
                    records = 10,
                    rows = updatedlist,
                };
                logger.Debug("In RequestAppointmentBookingController GetAppointments method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("In RequestAppointmentBookingController GetAppointments method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                inSLASlotsLists = new List<AppointmentSlots>();
                return Json(inSLASlotsLists, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RequestAppointment()
        {
            logger.Debug("In RequestAppointmentBookingController RequestAppointment method | Start" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            logger.Debug("In RequestAppointmentBookingController RequestAppointment method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            ViewBag.PartAddresses = Session["PartCount"];
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            //Response.Cache.SetNoStore();
            return View();
        }

        /// <summary>
        /// Make call to CountriesTimeZoneConversion to convert the time as per the timezone of region.Get ES & LS from CallClickService.
        /// </summary>
        /// <param name="EStart"></param>
        /// <param name="LStart"></param>
        /// <param name="Duration"></param>
        /// <param name="Profile"></param>
        /// <param name="TaskNotes"></param>
        /// <param name="SSOFse1"></param>
        /// <param name="SSOFse2"></param>
        /// <param name="SSOFse3"></param>
        /// <param name="fseSkill"></param>
        /// <param name="taskStatus"></param>
        /// <param name="AddressArray"></param>
        /// <param name="desiredDate"></param>
        /// <param name="partcomments"></param>
        /// <param name="JobType"></param>
        /// <param name="IsRequiredfse"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RequestAppointment(string EStart, string LStart, int Duration, string Profile, string TaskNotes, string SSOFse1, string SSOFse2, string SSOFse3, string fseSkill, string taskStatus, List<List<Array>> AddressArray, string desiredDate, string partcomments, string JobType, string IsRequiredfse, string SRDesc)
        {

            Session["PartToolReturn"] = "true";
            if (Session["ModifiedDuration"] != null)
                Session["ModifiedDuration"] = null;
            Session["IsRequiredfse"] = IsRequiredfse;
            var Conversion = (SiebelJsonToEntity)Session["SiebelData"];
            Session["SelectJobType"] = JobType;
            if (JobType == "Install")
            {
                Session["SelectInstallEarlyStart"] = desiredDate;
                EStart = desiredDate;
                EStart = (Convert.ToDateTime(DateTime.Parse(EStart, CultureInfo.GetCultureInfo("en-gb")).ToString())).ToString();
                LStart = (Convert.ToDateTime(EStart).AddDays(3)).ToString();
                Duration = Duration * 60;
                Session["SelectEarlyStart"] = EStart;
                Session["SelectLateStart"] = LStart;
                Session["SelectedProfile"] = "";

                // For Installlation to maintain values when navigated between pages.

                if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                    Session["SDTHomeEarlyStart"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                    Session["SDTHomeLateStart"] = null;


                Session["ModifiedDuration"] = null;
                Session["SDTHomeEarlyStart"] = EStart;
                Session["SDTHomeLateStart"] = LStart;
            }
            else
            {
                Session["SelectEarlyStart"] = EStart;
                Session["SelectLateStart"] = LStart;
                Session["SelectedProfile"] = Profile;


                if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                    Session["SDTHomeEarlyStart"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                    Session["SDTHomeLateStart"] = null;

                Session["SDTHomeEarlyStart"] = EStart;
                Session["SDTHomeLateStart"] = LStart;

                logger.Debug("In RequestAppointmentBookingController RequestAppointment Method | Start - Parameters EStart: " + EStart + " LStart: " + LStart + " Duration: " + Duration + " Profile: " + Profile + " TaskNotes: " + TaskNotes + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            }
            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
            
            DateTime startTimeTx6 = DateTime.Now;
            DateTime StartTime = objCountriesTimeZoneConversion.GetTimeZone(Conversion.serviceRequest.countryCode, Conversion.serviceRequest.shipToAddress);
            DateTime endTimeTx6 = DateTime.Now;
            TimeSpan Tx6 = endTimeTx6.Subtract(startTimeTx6);
            logger.Debug("In RequestAppointmentBooking controller RequestAppointment Method to get StartTime | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GetTimeZone) ~ " + IP + " ~ Request : Country code and ship to site address: " + Conversion.serviceRequest.countryCode + " " + Conversion.serviceRequest.shipToAddress + " ~ Response: " + StartTime + " ~ Call duration: " + Tx6.TotalMilliseconds);
            try
            {

                string ConcatStr = SSOFse1 + "," + SSOFse2 + "," + SSOFse3;
                string[] strarr = ConcatStr.Split(',');
                string[] filterarr = strarr.Distinct().ToArray();
                string preferredSSOIDs = string.Empty;
                foreach (string item in filterarr)
                {
                    preferredSSOIDs += item + ",";
                }
                preferredSSOIDs = preferredSSOIDs.Remove(preferredSSOIDs.Length - 1);

                if (Profile == "1 Hour")
                {
                    Profile = "ONE HOUR";
                }
                else if (Profile == "2 Hour")
                {
                    Profile = "TWO HOURS";
                }
                else if (Profile == "AM/PM")
                {
                    Profile = "AM-PM";
                }
                CallClickSerrvice objCallClickService = new CallClickSerrvice();
                IndextoRequest resindextoreq = new IndextoRequest();
                if (!string.IsNullOrEmpty(fseSkill))
                {
                    string[] fseSkillLevel = fseSkill.Split(',');
                    if (fseSkillLevel.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(fseSkillLevel[0]) && fseSkillLevel[0].ToUpper() != "NULL")
                        {
                            resindextoreq.FseSkillLevel = fseSkillLevel[0];
                        }
                    }
                }
                TempData["earlystart"] = resindextoreq.earlyStart = EStart;
                TempData["latestart"] = resindextoreq.lateStart = LStart;
                TempData["duration"] = resindextoreq.Duration = Duration;
                TempData["profile"] = resindextoreq.profile = Profile;
                TempData["taskNotes"] = resindextoreq.TaskNotes = TaskNotes;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                TempData["SRDes"] = resindextoreq.SRDescription = SRDesc;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                TempData["taskStatus"] = resindextoreq.TaskStatus = taskStatus;
                resindextoreq.PreferredFSEs = preferredSSOIDs;
                resindextoreq.addressArray = AddressArray; //added this part pick up
                resindextoreq.desiredDate = desiredDate;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                resindextoreq.SRDescription = SRDesc;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                if (!string.IsNullOrEmpty(partcomments))
                    resindextoreq.PartComments = partcomments;
                Session["IndextoRequest"] = resindextoreq;

                //GetESLSByStandardOperation will give ES and LS dates for fetching INSLA
                
                DateTime startTimeTx7 = DateTime.Now;
                var EsAndLS = objCallClickService.GetESLSByStandardOperation(Duration);
                DateTime endTimeTx7 = DateTime.Now;
                TimeSpan Tx7 = endTimeTx7.Subtract(startTimeTx7);                
                //Changes done by Raju Babu
                if (EsAndLS != null)
                {
                    logger.Debug("In RequestAppointmentBooking controller RequestAppointment Method to get ES and LS | Call to CallClickService.cs (CallClickService.GetESLSByStandardOperation) ~ " + IP + " ~ Request: With duration: " + Duration + " ~ Response: " + EsAndLS.Item1 + " , " + EsAndLS.Item2 + " ~ Call duration: " + Tx7.TotalMilliseconds);
                    if (EStart == "")
                    {
                        //Prajna - 04/06/2017 - US180/TA1308
                        //Prajna -04/11/2017  - De169 (Changed format to HH:mm from HH:mm:ss)
                        Session["SelectEarlyStart"] = Convert.ToDateTime(EsAndLS.Item1).ToString("dd/MM/yyyy HH:mm"); //EsAndLS.Item1;                      
                    }
                    if (LStart == "")
                    {
                        ////Prajna - 04/06/2017 - US180/TA1308
                        //Prajna -04/11/2017  - De169 (Changed format to HH:mm from HH:mm:ss)
                        Session["SelectLateStart"] = Convert.ToDateTime(EsAndLS.Item2).ToString("dd/MM/yyyy HH:mm");//EsAndLS.Item2;
                    }
                    //Prajna US85 changes  starts (For same ES and LS)
                    //if (EsAndLS.Item1 == EsAndLS.Item2)
                    //{
                    //    if (Conversion.serviceRequest.srType == "Corrective Repair" && Conversion.serviceRequest.gEMSEntitlementFlag == "Y")
                    //    {
                    //        return Json(new { HasError = false, Message = "modifyStatus" });
                    //    }
                    //}
                    //Prajna US85 changes  ends (For same ES and LS)
                }

                //if (!string.IsNullOrEmpty(Session["PartToolTempAddr"] as string))
                //    Session["PartToolTempAddr"] = null;
                if (Session["PartToolTempAddr"] != null)
                    Session["PartToolTempAddr"] = null;
                Session["PartToolTempAddr"] = resindextoreq.addressArray;



                //to maintain the same values when navigated between pages.

                if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                    Session["SDTHomeDuration"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeSSOFse1"] as string))
                    Session["SDTHomeSSOFse1"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                    Session["SDTHomeTaskNotes"] = null;

                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                if (!string.IsNullOrEmpty(Session["SDTHomeSRDesc"] as string))
                    Session["SDTHomeSRDesc"] = null;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                    Session["SDTHomeProfile"] = null;

                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                    Session["SDTHomeFseSkill"] = null;

                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                    Session["SDTHomeIsRequiredfse"] = null;
                if (Session["SDTHomeDuration"] != null)
                {
                    Session["SDTHomeDuration"] = null;
                }
                if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                    Session["SDTHomeDuration"] = null;
                Session["SDTHomeDuration"] = Duration;
                Session["SDTHomeSSOFse1"] = SSOFse1;
                Session["SDTHomeTaskNotes"] = TaskNotes;
                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                    Session["SDTHomeTaskNotes"] = Session["SDTHomeTaskNotes"].ToString().Replace("\n", "\\n");

                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                Session["SDTHomeSRDesc"] = SRDesc;
                if (!string.IsNullOrEmpty(Session["SDTHomeSRDesc"] as string))
                    Session["SDTHomeSRDesc"] = Session["SDTHomeSRDesc"].ToString().Replace("\n", "\\n");
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                Session["SDTHomeProfile"] = Profile;
                Session["SDTHomeFseSkill"] = fseSkill;
                Session["SDTHomeIsRequiredfse"] = IsRequiredfse;



            }
            catch (Exception ex)
            {
                logger.Error("In RequestAppointmentBookingController RequestAppointment method | Exception: " + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                //Logger.Error("Fault Exception thrown", ex);
                return Json(new { HasError = true, Message = "Failure" });
            }
            logger.Debug("In RequestAppointmentBookingController RequestAppointment method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            return Json(new { HasError = false, Message = "Success" });
        }

        /// <summary>
        /// Retrieves extended slots.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RequestExtendedSlotsTask()
        {
            //logger.Debug("RequestAppointmentBookingController;RequestExtenderSlotTsk; Start - Parameters Date: " + date + " AppStart: " + appStart + " AppFinish: " + appFinish + " RequiredFse:" + requiredFse);
            //deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code was used to initialize the 'isBump' flag)
            //bool isBump = false;
            DateTime startTime = DateTime.Now;
            // string inSLAColumn = string.Empty;
            # region deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code was used assign session value to the 'isBump' flag)
            //if (Session["IsBump"] != null)
            //{
            //    isBump = Convert.ToBoolean(Session["IsBump"]);
            //}
            # endregion deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code was used assign session value to the 'isBump' flag)
            
            IndextoRequest resindextoreq = new IndextoRequest();
            if (Session["IndextoRequest"] != null)
            {
                resindextoreq = (IndextoRequest)Session["IndextoRequest"];
            }
            else
                logger.Fatal("In RequestAppointmentBookingController RequestExtenderSlotTask method | Session IndextoRequest is null" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

            objCallClickService = new CallClickSerrvice();
            IEnumerable<AppointmentSlots> updatedSlots = null;
            IEnumerable<AppointmentSlots> resExtendedSlots = null;
            try
            {

                inSLASlotsLists = (IEnumerable<AppointmentSlots>)Session["RequesAppointmentNormalSlots"];
                IsExtendedSlots = true;

                object ESLS = Session["EsAndLS"];
                #region Logic for Extended Slots with and without Bump Task - Rajesh
                //if (isBump)
                //    Session["IsBump"] = isBump;
                if (resindextoreq.earlyStart != string.Empty && resindextoreq.lateStart != string.Empty)
                {
                   
                    DateTime startTimeTx8 = DateTime.Now;
                    resExtendedSlots = await objCallClickService.ClickCallByOperation(resindextoreq.earlyStart, resindextoreq.lateStart, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, SDTEnum.ExtendedSlotsType.ExtendedSlots.ToString());
                    var resExtendedSlotsWithOneHour = await objCallClickService.ClickCallByOperation(resindextoreq.earlyStart, resindextoreq.lateStart, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, SDTEnum.ExtendedSlotsType.ExtendedSlotsWithOneHour.ToString());
                    var resExtendedSlotsWithTwoHours = await objCallClickService.ClickCallByOperation(resindextoreq.earlyStart, resindextoreq.lateStart, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, SDTEnum.ExtendedSlotsType.ExtendedSlotsWithTwoHours.ToString());
                    var res1 = resExtendedSlotsWithOneHour.Concat(resExtendedSlots).GroupBy(x => x.EarlyStart + "|" + x.LateStart).Select(x => x.First());
                    var res2 = resExtendedSlotsWithTwoHours.Concat(res1).GroupBy(x => x.EarlyStart + "|" + x.LateStart).Select(x => x.First());
                    resExtendedSlots = res2.ToList();
                    DateTime endTimeTx8 = DateTime.Now;
                    TimeSpan Tx8 = endTimeTx8.Subtract(startTimeTx8);
                    logger.Debug("In RequestAppointmentBooking controller RequestExtendedSlots Method | To retrieve extended slots with 1 and 2 hours (3 calls) | Call to ClickCallService.cs (CallClickSerrvice.ClickCallByOperation) ~ " + IP + " ~ Request: " + resindextoreq.earlyStart + ", " + resindextoreq.lateStart + ", " + resindextoreq.Duration + ", " + resindextoreq.profile + ", " + resindextoreq.PreferredFSEs + ", " + resindextoreq.FseSkillLevel + ", " + IsExtendedSlots + " ~ Response : Extended Slots list ~ Call duration: " + Tx8.TotalMilliseconds);
                }
                if (inSLASlotsLists != null)
                {
                    //inSLAColumn = inSLASlotsLists.Select(x => x.SLA).FirstOrDefault();
                    // inSLASlotsLists.ToList().ForEach(x => x.SLA = null);
                    if (resExtendedSlots != null && resExtendedSlots.Count() > 0)
                    {
                        resExtendedSlots = (from lst1 in resExtendedSlots
                                            where !inSLASlotsLists.Any(
                                                                        x => x.EarlyStart == lst1.EarlyStart &&
                                                                             x.LateStart == lst1.LateStart)
                                            select lst1).ToList();

                    }
                }
                //before SLA
                if (ESLS != null)
                {
                    if ((((System.Tuple<string, string>)(ESLS)).Item1) != string.Empty && (((System.Tuple<string, string>)(ESLS)).Item2) != string.Empty)
                    {
                        if (resExtendedSlots != null && resExtendedSlots.Count() > 0)
                        {
                            var beforeSLA = resExtendedSlots.Where(date => Convert.ToDateTime(date.EarlyStart)
                             < Convert.ToDateTime(((System.Tuple<string, string>)(ESLS)).Item1));
                            //after SLA
                            var afterSLA = resExtendedSlots.Where(date => Convert.ToDateTime(date.LateStart)
                            > Convert.ToDateTime(((System.Tuple<string, string>)(ESLS)).Item2));

                            afterSLA = (from lstafterSLA in afterSLA
                                        where !beforeSLA.Any(
                                                                  x => x.EarlyStart == lstafterSLA.EarlyStart &&
                                                                       x.LateStart == lstafterSLA.LateStart)
                                        select lstafterSLA).ToList();



                            #region This logic is implemented when extended slots are getting between 7-8 pm  in Korea region

                            var eStartUI = "";
                            var lStartUI = "";
                            if (Session["objSiebelResponseforType"].ToString() == "Installation")
                            {
                                eStartUI = Convert.ToDateTime(resindextoreq.earlyStart).ToShortDateString();
                                lStartUI = Convert.ToDateTime(resindextoreq.lateStart).ToString();
                            }
                            else
                            {
                                eStartUI = DateTime.Parse(resindextoreq.earlyStart, CultureInfo.GetCultureInfo("en-gb")).ToShortDateString();
                                lStartUI = DateTime.Parse(resindextoreq.lateStart, CultureInfo.GetCultureInfo("en-gb")).ToString();
                            }


                            var ExtendedWithinESLSUI = resExtendedSlots.Where(date =>
                                Convert.ToDateTime(date.EarlyStart) >= Convert.ToDateTime(eStartUI)
                                && Convert.ToDateTime(date.LateStart) <= Convert.ToDateTime(lStartUI));


                            if (ExtendedWithinESLSUI != null && ExtendedWithinESLSUI.Count() > 0)
                            {
                                ExtendedWithinESLSUI = (from lst1 in ExtendedWithinESLSUI
                                                        where !beforeSLA.Any(
                                                                                    x => x.EarlyStart == lst1.EarlyStart &&
                                                                                         x.LateStart == lst1.LateStart)
                                                        select lst1).ToList();

                            }
                            if (ExtendedWithinESLSUI != null && ExtendedWithinESLSUI.Count() > 0)
                            {
                                ExtendedWithinESLSUI = (from lst1 in ExtendedWithinESLSUI
                                                        where !afterSLA.Any(
                                                                                    x => x.EarlyStart == lst1.EarlyStart &&
                                                                                         x.LateStart == lst1.LateStart)
                                                        select lst1).ToList();

                            }

                            #endregion


                            beforeSLA.ToList().ForEach(cc => cc.SLA = "Before SLA");
                            afterSLA.ToList().ForEach(cc => cc.SLA = "After SLA");
                            // inSLASlotsLists.ToList().ForEach(x => x.SLA = "IN SLA");
                            //Code Start Prajna - 04/17/2017 - US177/TA1302
                            //SLA column was showing blank when Extended slot SLA is "IN SLA", Added IN SLA to the list
                            if (ExtendedWithinESLSUI != null && ExtendedWithinESLSUI.Count() > 0)
                            {
                                ExtendedWithinESLSUI.ToList().ForEach(x => x.SLA = "IN SLA");
                            }
                            ////Code End Prajna - 04/17/2017 - US177/TA1302
                            inSLASlotsLists.ToList().ForEach(x => x.Working = "Normal");
                            beforeSLA.ToList().ForEach(x => x.Working = "Extended");
                            afterSLA.ToList().ForEach(x => x.Working = "Extended");
                            ExtendedWithinESLSUI.ToList().ForEach(x => x.Working = "Extended");

                            # region deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code in foreach loop was iteratively calling the BumpCheck function in ClickCallService.cs for every slot retrieved from SDT Schedule)
                            //foreach (var i in resExtendedSlots)
                            //{
                            //    logger.Debug("In RequestAppointmentBooking controller RequestExtendedSlots Method for Bump Check | Call to ClickCallService.cs (CallClickSerrvice.BumpCheck) with params: " + i.EarlyStart + " " + i.LateStart + " " + i.PreferredFSE + " " + i.SSOID.ToString());
                            //    DateTime startTimeTx9 = DateTime.Now;
                            //    var result = objCallClickService.BumpCheck(i.EarlyStart, i.LateStart, true, i.PreferredFSE, i.SSOID.ToString());
                            //    DateTime endTimeTx9 = DateTime.Now;
                            //    TimeSpan Tx9 = endTimeTx9.Subtract(startTimeTx9);                                
                            //    if (result.Count > 0 && isBump == true && (result[0].Task.Status.DisplayString == "Tentative" || result[0].Task.Status.DisplayString == "Assigned") && (result[0].Task.SystemStatus.DisplayString == "3 (System up and running)" || result[0].Task.SystemStatus.DisplayString == "3(System up and running)"))
                            //    {
                            //        logger.Debug("In RequestAppointmentBooking controller RequestExtendedSlots Method for Bump Check | Call to ClickCallService.cs (CallClickSerrvice.BumpCheck) | Call duration: " + Tx9.TotalMilliseconds + " | Response: " + Convert.ToString(result));
                            //        i.Bump = "Yes";
                            //        i.TaskStatus = result[0].Task.Status.DisplayString;
                            //        //afterSLA.ToList().ForEach(x => x.Bump = "Yes");
                            //        //beforeSLA.ToList().ForEach(x => x.Bump = "Yes");
                            //    }
                            //    else
                            //    {
                            //        i.Bump = "";
                            //        //afterSLA.ToList().ForEach(x => x.Bump = "");
                            //        //beforeSLA.ToList().ForEach(x => x.Bump = "");
                            //    }

                            //}
                            # endregion
                            //if (isBump == true)
                            //{
                            //    afterSLA.ToList().ForEach(x => x.Bump = "Yes");
                            //    beforeSLA.ToList().ForEach(x => x.Bump = "Yes");
                            //}
                            //else
                            //{
                            //    afterSLA.ToList().ForEach(x => x.Bump = "");
                            //    beforeSLA.ToList().ForEach(x => x.Bump = "");
                            //}

                            //var updatedSlots = inSLASlotsLists.Concat(beforeSLA).Concat(afterSLA);
                            if (inSLASlotsLists != null)
                            {
                                #region ExtendedWithinESLSUI  Concat logic is implemented when extended slots are getting between 7-8 pm  in Korea region

                                updatedSlots = inSLASlotsLists.Concat(afterSLA).Concat(beforeSLA).Concat(ExtendedWithinESLSUI);

                                #endregion
                            }
                            else
                                updatedSlots = beforeSLA.Concat(afterSLA).Concat(ExtendedWithinESLSUI);

                        }
                        else
                            if (inSLASlotsLists != null)
                            {
                                //  inSLASlotsLists.ToList().ForEach(x => x.SLA = inSLAColumn.ToString());
                                inSLASlotsLists.ToList().ForEach(x => x.Working = "Normal");
                                updatedSlots = inSLASlotsLists.ToList();
                            }
                    }
                }
                else
                {
                    if (inSLASlotsLists != null)
                    {
                        //  inSLASlotsLists.ToList().ForEach(x => x.SLA = inSLAColumn.ToString());
                        inSLASlotsLists.ToList().ForEach(x => x.Working = "Normal");
                        updatedSlots = inSLASlotsLists.ToList();
                    }
                }
                //    }
                //}

                if (resindextoreq.desiredDate == null) resindextoreq.desiredDate = string.Empty;
                if (!String.IsNullOrEmpty(resindextoreq.desiredDate))
                {
                    var desiredDt = DateTime.ParseExact(resindextoreq.desiredDate.TrimEnd(), "dd/MM/yyyy HH:mm", null);
                    var desiredDateSlots = updatedSlots.Where(x => (Convert.ToDateTime(x.EarlyStart) >= Convert.ToDateTime(desiredDt)) &&
          Convert.ToDateTime(x.LateStart) <= Convert.ToDateTime(desiredDt));

                    var nonDesiredDateSlots = (from lst1 in updatedSlots
                                               where !desiredDateSlots.Any(
                                                                         x => x.EarlyStart == lst1.EarlyStart &&
                                                                              x.LateStart == lst1.LateStart)
                                               select lst1).ToList();
                    updatedSlots = desiredDateSlots.Concat(nonDesiredDateSlots);
                }
                if (!string.IsNullOrEmpty(Session["objSiebelResponseforType"] as string))
                {
                    var eStart = "";
                    var lStart = "";

                    if (!string.IsNullOrEmpty(resindextoreq.earlyStart) && !string.IsNullOrEmpty(resindextoreq.lateStart))
                    {
                        if (Session["objSiebelResponseforType"].ToString() == "Installation")
                        {
                            eStart = Convert.ToDateTime(resindextoreq.earlyStart).ToShortDateString();
                            lStart = Convert.ToDateTime(resindextoreq.lateStart).ToString();
                        }
                        else
                        {
                            eStart = DateTime.Parse(resindextoreq.earlyStart, CultureInfo.GetCultureInfo("en-gb")).ToShortDateString();
                            lStart = DateTime.Parse(resindextoreq.lateStart, CultureInfo.GetCultureInfo("en-gb")).ToString();
                        }

                        if (resindextoreq.profile == "TWO HOURS")
                        {
                            updatedSlots = (from esList in updatedSlots
                                            where (Convert.ToDateTime(esList.EarlyStart) >= Convert.ToDateTime(eStart).AddHours(-1).AddMinutes(-59) &&
                                                Convert.ToDateTime(esList.LateStart) <= Convert.ToDateTime(lStart).AddHours(2))
                                            select esList
                                );
                        }
                        else if (resindextoreq.profile == "ONE HOUR")
                        {
                            updatedSlots = (from esList in updatedSlots
                                            where (Convert.ToDateTime(esList.EarlyStart) >= Convert.ToDateTime(eStart).AddMinutes(-59) &&
                                                Convert.ToDateTime(esList.LateStart) <= Convert.ToDateTime(lStart).AddHours(1))
                                            select esList
                                    );
                        }
                        else
                        {
                            if (Session["objSiebelResponseforType"].ToString() == "Installation")
                            {
                                eStart = Convert.ToDateTime(resindextoreq.earlyStart).ToShortDateString();
                            }
                            else
                            {
                                eStart = DateTime.Parse(resindextoreq.earlyStart, CultureInfo.GetCultureInfo("en-gb")).ToShortDateString();
                            }
                            updatedSlots = (from esList in updatedSlots
                                            where (Convert.ToDateTime(esList.EarlyStart) >= Convert.ToDateTime(eStart) &&
                                                Convert.ToDateTime(esList.LateStart) <= Convert.ToDateTime(lStart))
                                            select esList
                                    );
                        }
                      

                    }

                }

                var updatedlist = updatedSlots.Select((r, i) => new
                {
                    id = i,
                    score = Convert.ToInt32(r.Grade),//"900",I 
                    date = DateTime.Parse(r.EarlyStart, CultureInfo.GetCultureInfo("en-US")).ToString(),
                    AppStart = DateTime.Parse(r.EarlyStart, CultureInfo.GetCultureInfo("en-US")).ToShortTimeString(),
                    AppFinish = DateTime.Parse(r.LateStart, CultureInfo.GetCultureInfo("en-US")).ToShortTimeString(),
                    TentativeFse = r.PreferredFSE,
                    RequiredFse = "false",
                    SLA = r.SLA,
                    Working = r.Working,
                    //deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code was used to accomodate the Bump parameter for updating the Extended slots list)
                    //Bump = r.Bump,
                    SSOID = r.SSOID,
                    TaskStatus = r.TaskStatus,
                    SchedulingpolicyExtended = r.SchedulingpolicyExtendedSlots

                });
                if (!string.IsNullOrEmpty(Session["objSiebelResponseforType"] as string))
                {
                    //if (Session["objSiebelResponseforType"].ToString() != "PM")
                    //{
                    //    updatedlist = updatedlist.OrderBy(j => j.score);

                    //}
                    //Changed to order by asc with date
                    if (Session["objSiebelResponseforType"].ToString() != "PM")
                    {
                        updatedlist.OrderBy(d => DateTime.Parse(d.date));

                    }
                }

                foreach (var item in updatedlist)
                {
                    logger.Debug("In RequestAppointmentBookingControllerRequest Extended Slots | Updated List Logs " + item.date + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                }

                #endregion
                updatedlist = updatedlist.GroupBy(p => new { p.date, p.AppStart, p.AppFinish }).Select(g => g.First()).ToList();

                var jsonData = new
                {
                    total = 1,
                    page = 1,
                    records = 10,
                    rows = new[] { updatedlist },
                };
                logger.Debug("In RequestAppointmentBookingController RequestExtendedSlotsTask method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                //DateTime endTime = DateTime.Now;

                //TimeSpan span = endTime.Subtract(startTime);
                //ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                //Session["ReqTimeSpan"] = span.Minutes * 60 + span.Seconds;
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("In RequestAppointmentBookingController RequestExtendedSlotsTask method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                Session["ReqTimeSpan"] = span.Minutes * 60 + span.Seconds;
                ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;

                return Json(new { status = SDTEnum.OperationStatus.NotProcessed.ToString(), message = ex.Message.ToString() });
            }
        }

        /// <summary>
        /// This method creates a task in click for the selected appointment slot.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="appStart"></param>
        /// <param name="appFinish"></param>
        /// <param name="requiredFse"></param>
        /// <param name="isBumpTask"></param>
        /// <param name="Schedulingpolicy"></param>
        /// <returns></returns>
        [HttpPost]
        //deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This function is used to process the task creation, the isBumpTask is removed from the method params)
        //public async Task<ActionResult> ProcessTsk(string date, string appStart, string appFinish, bool requiredFse, string isBumpTask, string Schedulingpolicy)
        public async Task<ActionResult> ProcessTsk(string date, string appStart, string appFinish, bool requiredFse, string Schedulingpolicy)
        {        
            Session["Schedulingpolicy"] = Schedulingpolicy;           
            //deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code was used to assign the 'isBumpTask' flag to the session variable)
            //Session["IsBumpTask"] = isBumpTask;

            int taskStatus = 0;
            logger.Debug("In RequestAppointmentBookingController ProcessTsk method | Start - Parameters Date: " + date + " , AppStart: " + appStart + " , AppFinish: " + appFinish + " , RequiredFse: " + requiredFse + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            if (Session["objClickTasksResponse"] != null)
            {
                ClickTasksResSystemSite objClickTasksResponse = new ClickTasksResSystemSite();
                objClickTasksResponse = (ClickTasksResSystemSite)Session["objClickTasksResponse"];
                if (objClickTasksResponse.TaskExists)
                    taskStatus = 1;
            }
            IndextoRequest objIndexToRequest = new IndextoRequest();
            if (Session["IndextoRequest"] != null)
            {
                objIndexToRequest = (IndextoRequest)Session["IndextoRequest"];
            }
            else
                logger.Fatal("In RequestAppointmentBookingController ProcessTsk method | Session IndextoRequest is null" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

            string aStart = date + ' ' + appStart;
            string aFinish = date + ' ' + appFinish;
            string taskID = string.Empty;
            if (Session["SiebelData"] != null)
            {
                var res = (SiebelJsonToEntity)Session["SiebelData"];
                taskID = res.serviceRequest.activityDetailList[0].activityId;
            }
            else
            {
                ActionExecutingContext filterContext = new ActionExecutingContext();
                filterContext.Result = new RedirectResult("~/Shared/Error");
                base.OnActionExecuting(filterContext);
            }
            objCallClickService = new CallClickSerrvice();
            DateTime startTime = DateTime.Now;
            try
            {
                //if (!string.IsNullOrEmpty(objIndexToRequest.addressArray) && objIndexToRequest.addressArray.Length > 1)

                if (objIndexToRequest.addressArray != null)
                {
                    //var addressArray = objIndexToRequest.addressArray;
                    // addressArray = addressArray.Remove(addressArray.Length - 1);
                    //addressArray = addressArray.Replace("\"", "");
                    if (objIndexToRequest.addressArray[0].Count > 0)
                    {
                        
                        DateTime startTimeTx10 = DateTime.Now;
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                        var resPartPickUp = objCallClickService.PartPickDependency(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, objIndexToRequest.addressArray, objIndexToRequest.PartComments, objIndexToRequest.PreferredFSEs, objIndexToRequest.SRDescription, DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                                          DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"));
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                        DateTime endTimeTx10 = DateTime.Now;
                        TimeSpan Tx10 = endTimeTx10.Subtract(startTimeTx10);
                        logger.Debug("In RequestAppointmentBooking controller ProcessTsk Method | For Creating Part Pick up Dependency | Call to ClickCallService.cs (CallClickSerrvice.PartPickDependency) ~ " + IP + " ~ Request: with params: " + objIndexToRequest.earlyStart + " , " + objIndexToRequest.lateStart + " , " + objIndexToRequest.Duration + " , " + objIndexToRequest.TaskNotes + " , " + objIndexToRequest.FseSkillLevel + " , " + objIndexToRequest.addressArray + " , " + objIndexToRequest.PartComments + " , " + objIndexToRequest.PreferredFSEs + " , " + DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " , " + DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " ~ Response: " + taskID + " ~ Call duration: " + Tx10.TotalMilliseconds);
                        Session["PartToolReturn"] = "false";
                        DateTime endTime = DateTime.Now;

                        TimeSpan span = endTime.Subtract(startTime);
                        ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                        return Json(new { message = "Success", JobType = "Part", TaskID = taskID });
                        //return Json(new { status = resProcessTask.FirstOrDefault().Status, TaskID = resProcessTask.FirstOrDefault().TaskID, message = "Success", taskStatus = taskStatus.ToString() });
                    }
                    else
                    {
                        logger.Debug("In RequestAppointmentBookingController ProcessTsk method | API Call: ClickCallToCreateTask Parameters: EarlyStart: " + objIndexToRequest.earlyStart + ", LateStart :" + objIndexToRequest.lateStart +
                        "Duration :" + Convert.ToInt32(objIndexToRequest.Duration) + "Start :" + DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " Finish: " + DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");                        
                        DateTime startTimeTx11 = DateTime.Now;
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                        var resProcessTask = await objCallClickService.ClickCallToCreateTask(objIndexToRequest.earlyStart,
                                                                         objIndexToRequest.lateStart,
                                                                         Convert.ToInt32(objIndexToRequest.Duration),
                                                                         objIndexToRequest.TaskNotes,
                                                                         objIndexToRequest.FseSkillLevel,
                                                                         objIndexToRequest.PreferredFSEs,                                                                     
                                                                         objIndexToRequest.SRDescription,
                                                                         DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                         DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                         Convert.ToBoolean(Session["IsRequiredFse"].ToString()));
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                        DateTime endTimeTx11 = DateTime.Now;
                        TimeSpan Tx11 = endTimeTx11.Subtract(startTimeTx11);
                        logger.Debug("In RequestAppointmentBooking controller ProcessTsk Method | For Creating Task | Call to ClickCallService.cs (CallClickSerrvice.ClickCallToCreateTask) ~ " + IP + " ~ Request: with params: " + Convert.ToString(objIndexToRequest.earlyStart) + " , " + Convert.ToString(objIndexToRequest.lateStart) + " , " + Convert.ToInt32(objIndexToRequest.Duration) + " , " + Convert.ToString(objIndexToRequest.TaskNotes) + " , " + Convert.ToString(objIndexToRequest.FseSkillLevel) + " " + Convert.ToString(objIndexToRequest.PreferredFSEs) + " , " + DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " , " + DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " , " + Convert.ToBoolean(Session["IsRequiredFse"].ToString()) + " ~ Response: " + Convert.ToString(resProcessTask.FirstOrDefault().Status) + " , " + Convert.ToString(resProcessTask.FirstOrDefault().TaskID) + " , " + taskStatus.ToString() + " ~ Call duration: " + Tx11.TotalMilliseconds);
                        if (resProcessTask == null)
                            logger.Debug("In RequestAppointmentBookingController ProcessTsk method | API Response: Process Task is NULL ");

                        DateTime endTime = DateTime.Now;

                        TimeSpan span = endTime.Subtract(startTime);
                        ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                        logger.Debug("In RequestAppointmentBookingController ProcessTsk method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                        return Json(new { status = resProcessTask.FirstOrDefault().Status, TaskID = resProcessTask.FirstOrDefault().TaskID, message = "Success", taskStatus = taskStatus.ToString() });

                    }
                }
                else
                {
                    logger.Debug("In RequestAppointmentBookingController ProcessTs method | API Call: ClickCallToCreateTask Parameters: " + "EarlyStart : " + objIndexToRequest.earlyStart + ", LateStart :" + objIndexToRequest.lateStart +
                        ", Duration: " + Convert.ToInt32(objIndexToRequest.Duration) + ", Start: " + DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + ", Finish: " + DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                    
                    DateTime startTimeTx12 = DateTime.Now;

                    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                    var resProcessTask = await objCallClickService.ClickCallToCreateTask(objIndexToRequest.earlyStart,
                                                                     objIndexToRequest.lateStart,
                                                                     Convert.ToInt32(objIndexToRequest.Duration),
                                                                     objIndexToRequest.TaskNotes,
                                                                     objIndexToRequest.FseSkillLevel,
                                                                     objIndexToRequest.PreferredFSEs,
                                                                     objIndexToRequest.SRDescription,
                                                                     DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                     DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                     Convert.ToBoolean(Session["IsRequiredFse"].ToString()));

                    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                    DateTime endTimeTx12 = DateTime.Now;
                    TimeSpan Tx12 = endTimeTx12.Subtract(startTimeTx12);
                    logger.Debug("In RequestAppointmentBooking controller ProcessTsk Method | For Creating Task | Call to ClickCallService.cs (CallClickSerrvice.ClickCallToCreateTask) ~ " + IP + " ~ Request: with params: " + Convert.ToString(objIndexToRequest.earlyStart) + " , " + Convert.ToString(objIndexToRequest.lateStart) + " , " + Convert.ToInt32(objIndexToRequest.Duration) + " , " + Convert.ToString(objIndexToRequest.TaskNotes) + " , " + Convert.ToString(objIndexToRequest.FseSkillLevel) + " " + Convert.ToString(objIndexToRequest.PreferredFSEs) + " , " + DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " , " + DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " , " + Convert.ToBoolean(Session["IsRequiredFse"].ToString()) + " ~ Response: " + Convert.ToString(resProcessTask.FirstOrDefault().Status) + " , " + Convert.ToString(resProcessTask.FirstOrDefault().TaskID) + " , " + taskStatus.ToString() + " ~ Call duration: " + Tx12.TotalMilliseconds);
                    if (resProcessTask == null)
                        logger.Debug("In RequestAppointmentBookingController ProcessTsk method| API Response: Process Task is NULL " + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                    DateTime endTime = DateTime.Now;

                    TimeSpan span = endTime.Subtract(startTime);
                    ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                    logger.Debug("In RequestAppointmentBookingController ProcessTsk method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                    return Json(new { status = resProcessTask.FirstOrDefault().Status, TaskID = resProcessTask.FirstOrDefault().TaskID, message = "Success", taskStatus = taskStatus.ToString() });
                }
            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("In RequestAppointmentBookingController ProcessTsk method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                return Json(new { status = SDTEnum.OperationStatus.NotProcessed.ToString(), message = ex.Message.ToString() });
            }
        }

        /// <summary>
        /// This method creates a task in click without searching for any available slots.
        /// </summary>
        /// <param name="EStart"></param>
        /// <param name="LStart"></param>
        /// <param name="Duration"></param>
        /// <param name="TaskNotes"></param>
        /// <param name="addressArray"></param>
        /// <param name="fseSkill"></param>
        /// <param name="PreferredFSEs"></param>
        /// <param name="partcomments"></param>
        /// <param name="taskOpenDate"></param>
        /// <param name="jobType"></param>
        /// <param name="IsRequiredfse"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ProcessTaskwithoutAppointment(string EStart, string LStart, int Duration, string TaskNotes, List<List<Array>> addressArray, string fseSkill, string PreferredFSEs, string partcomments, string taskOpenDate, string jobType, string IsRequiredfse, string SRDesc)
        {
            Session["IsRequiredFse"] = IsRequiredfse;
            IEnumerable<AppointmentSlots> taskappointmentsSolt = null;
            IndextoRequest objIndexToRequest = new IndextoRequest();
            string taskID = string.Empty;
            //addressArray = addressArray.Remove(addressArray.Length - 1);
            //if (addressArray.Length > 1)
            //{

            //    string[] arrayAddress = addressArray.Split('=');
            //    for (int i = 0; i < arrayAddress.Length; i++)
            //    {
            //        string[] addres = arrayAddress[i].Split(',');
            //    }
            //}
            logger.Debug("In RequestAppointmentBookingController ProcessTaskwithoutAppointment method | Start - Parameters EStart: " + EStart + ", LStart: " + LStart + ", Duration: " + Duration + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            if (Session["SiebelData"] != null)
            {
                var res = (SiebelJsonToEntity)Session["SiebelData"];
                taskID = res.serviceRequest.activityDetailList[0].activityId;
            }

            if (Session["IndextoRequest"] != null)
            {
                objIndexToRequest = (IndextoRequest)Session["IndextoRequest"];
            }
            else
            {

                if (jobType == "Install")
                {
                    EStart = taskOpenDate;
                    EStart = (Convert.ToDateTime(DateTime.Parse(EStart, CultureInfo.GetCultureInfo("en-gb")).ToString())).ToString();
                    LStart = (Convert.ToDateTime(EStart).AddDays(3)).ToString();
                    Duration = Duration * 60;

                    //EStart = (Convert.ToDateTime(DateTime.Parse(EStart).ToString())).ToString("dd/MM/yyyy hh:mm:ss tt");
                    //LStart = (Convert.ToDateTime(EStart).AddDays(3).ToString("MM/dd/yyyy hh:mm:ss tt"));
                    //EStart = (Convert.ToDateTime(EStart).ToString("dd/MM/yyyy hh:mm:ss tt"));
                    //LStart = (Convert.ToDateTime(LStart).ToString("dd/MM/yyyy hh:mm:ss tt"));

                }
                objIndexToRequest.earlyStart = EStart;
                objIndexToRequest.lateStart = LStart;
                objIndexToRequest.Duration = Duration;
                objIndexToRequest.TaskNotes = TaskNotes;
                objIndexToRequest.PartComments = partcomments;
                
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
				objIndexToRequest.SRDescription= SRDesc;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                if (!string.IsNullOrEmpty(PreferredFSEs))
                {
                    objIndexToRequest.PreferredFSEs = PreferredFSEs;
                }
                if (!string.IsNullOrEmpty(fseSkill))
                {
                    string[] fseSkillLevel = fseSkill.Split(',');
                    if (fseSkillLevel.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(fseSkillLevel[0]) && fseSkillLevel[0].ToUpper() != "NULL")
                        {
                            objIndexToRequest.FseSkillLevel = fseSkillLevel[0];
                        }
                    }
                }
                // objIndexToRequest.profile = Request.QueryString["profile"]; 
            }
            objCallClickService = new CallClickSerrvice();
            try
            {
                if (addressArray != null)
                {
                    if (addressArray[0].Count > 0)
                    {
                       
                        DateTime startTimeTx13 = DateTime.Now;
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                        var resPartPickUp = objCallClickService.PartPickDependency(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, addressArray, objIndexToRequest.PartComments, objIndexToRequest.PreferredFSEs, objIndexToRequest.SRDescription);
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                        DateTime endTimeTx13 = DateTime.Now;
                        TimeSpan Tx13 = endTimeTx13.Subtract(startTimeTx13);
                        logger.Debug("In RequestAppointmentBooking controller ProcessTaskwithoutAppointment Method | For Creating Task | Call to ClickCallService.cs (CallClickSerrvice.PartPickDependency) ~ " + IP + " ~ Request: With params: " + objIndexToRequest.earlyStart + " , " + objIndexToRequest.lateStart + " , " + objIndexToRequest.Duration + " , " + objIndexToRequest.TaskNotes + " , " + objIndexToRequest.FseSkillLevel + " , " + objIndexToRequest.PreferredFSEs + " ~ Response: Unused variable ~ Call duration: " + Tx13.TotalMilliseconds);
                        return Json(new { message = "Success", JobType = "Part", TaskID = taskID });
                    }
                    else
                    {
                        logger.Debug("In RequestAppointmentBookingController ProcessTaskwithoutAppointment method | API Call: ClickCallToCreateTask Parameters: EarlyStart: " + objIndexToRequest.earlyStart + ", LateStart: " + objIndexToRequest.lateStart + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                        DateTime startTimeTx14 = DateTime.Now;
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                        taskappointmentsSolt = await objCallClickService.ClickCallToCreateTask(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, objIndexToRequest.PreferredFSEs, objIndexToRequest.SRDescription);
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                        DateTime endTimeTx14 = DateTime.Now;
                        TimeSpan Tx14 = endTimeTx14.Subtract(startTimeTx14);
                        logger.Debug("In RequestAppointmentBooking controller ProcessTaskwithoutAppointment Method | For Creating Task | Call to ClickCallService.cs (CallClickSerrvice.PartPickDependency) ~ " + IP + " ~ Request: With params: " + objIndexToRequest.earlyStart + " , " + objIndexToRequest.lateStart + " , " + objIndexToRequest.Duration + " , " + objIndexToRequest.TaskNotes + " , " + objIndexToRequest.FseSkillLevel + " , " + objIndexToRequest.PreferredFSEs + " ~ Response: " + taskappointmentsSolt.FirstOrDefault().Status + " , " + taskappointmentsSolt.FirstOrDefault().TaskID + " ~ Call duration: " + Tx14.TotalMilliseconds);
                        logger.Debug("In RequestAppointmentBookingController ProcessTaskwithoutAppointment Method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                    }
                }
                else
                {
                    logger.Debug("In RequestAppointmentBookingController ProcessTaskwithoutAppointment method | API Call: ClickCallToCreateTask Parameters: EarlyStart: " + objIndexToRequest.earlyStart + ", LateStart: " + objIndexToRequest.lateStart + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                    DateTime startTimeTx15 = DateTime.Now;
                    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                    taskappointmentsSolt = await objCallClickService.ClickCallToCreateTask(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, objIndexToRequest.PreferredFSEs, objIndexToRequest.SRDescription);
                    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                    DateTime endTimeTx15 = DateTime.Now;
                    TimeSpan Tx15 = endTimeTx15.Subtract(startTimeTx15);
                    logger.Debug("In RequestAppointmentBooking controller ProcessTaskwithoutAppointment Method | For Creating Task | Call to ClickCallService.cs (CallClickSerrvice.PartPickDependency) ~ " + IP + " ~ Request: With params: " + objIndexToRequest.earlyStart + " , " + objIndexToRequest.lateStart + " , " + objIndexToRequest.Duration + " , " + objIndexToRequest.TaskNotes + " , " + objIndexToRequest.FseSkillLevel + " , " + objIndexToRequest.PreferredFSEs + " ~ Response: " + taskappointmentsSolt.FirstOrDefault().Status + " , " + taskappointmentsSolt.FirstOrDefault().TaskID + " ~ Call duration: " + Tx15.TotalMilliseconds);
                    logger.Debug("In RequestAppointmentBookingController ProcessTaskwithoutAppointment method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                }
                return Json(new { status = taskappointmentsSolt.FirstOrDefault().Status, TaskID = taskappointmentsSolt.FirstOrDefault().TaskID, message = "Success" });

            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("In RequestAppointmentBookingController ProcessTaskwithoutAppointment method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                return Json(new { status = SDTEnum.OperationStatus.NotProcessed.ToString(), message = ex.Message.ToString() });
            }
        }

        /// <summary>
        /// Method for cancelling task.
        /// </summary>
        /// <param name="CancelReason"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelTask(string CancelReason)
        {
            string cancelStatus = string.Empty;
            try
            {
                logger.Debug("In RequestAppointmentBookingController CancelTask | Start" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                objCallClickService = new CallClickSerrvice();
                GetTasksResponse objGetTaskResponse = new GetTasksResponse();
                var res = (HTTPPostParams)Session["SiebelHttpPostParams"];
               
                DateTime startTimeTx16 = DateTime.Now;
                if (objCallClickService.CancelTask(CancelReason))
                {
                 cancelStatus = "Cancelled";
                }
                else
                {
                 cancelStatus = "notcancelled";
                }
                DateTime endTimeTx16 = DateTime.Now;
                TimeSpan Tx16 = endTimeTx16.Subtract(startTimeTx16);
                logger.Debug("In RequestAppointmentBookingController CancelTask Method | For Cancelling Task | Call to ClickCallService.cs (CallClickSerrvice.PartPickDependency) ~ " + IP + " ~ Request: with cancel reason: " + CancelReason + " ~ Response: " + cancelStatus + " ~ Call duration: " + Tx16.TotalMilliseconds);
                logger.Debug("In RequestAppointmentBookingController CancelTask method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                return Json(new { message = cancelStatus });
            }
            catch (Exception ex)
            {
                logger.Error("In RequestAppointmentBookingController CancelTask method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                //throw;
                //Added By phani kanth
                TempData["Errormsg"] = ex.Message;
                //TempData["Errormsg"] = "Unable to complete your request to Cancel.<n/> Please contact the server administrator, webmaster@ge.com and inform them of the time the error occurred.";
                // return RedirectToAction("Errormsg", "ErrorPage");
                return Json(new { message = ex.Message.ToString() });
            }


        }

        /// <summary>
        /// Method for modifying the visit/task.
        /// </summary>
        /// <param name="earlyStart"></param>
        /// <param name="lateStart"></param>
        /// <param name="duration"></param>
        /// <param name="addressArray"></param>
        /// <param name="TaskNotes"></param>
        /// <param name="fseSkill"></param>
        /// <param name="PreferredFSEs"></param>
        /// <param name="JobType"></param>
        /// <param name="partcomments"></param>
        /// <param name="IsRequiredfse"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyVisit(string earlyStart, string lateStart, int duration, List<List<Array>> addressArray, string TaskNotes, string fseSkill, string PreferredFSEs, string JobType, string partcomments, string IsRequiredfse, string SRDesc)
        {

            Session["IsRequiredFse"] = IsRequiredfse;
            // string[] arrayAddress = addressArray.Split('=');
            string fseSkills = string.Empty;


            //foreach (int i in list)
            //{
            //    Address.Add(i.ToString());
            //}
            logger.Debug("In RequestAppointmentBookingController ModifyVisit method | Start" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

            string modifyStatus = string.Empty;
            try
            {
                objCallClickService = new CallClickSerrvice();
                if (!string.IsNullOrEmpty(fseSkill))
                {
                    string[] fseSkillLevel = fseSkill.Split(',');
                    if (fseSkillLevel.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(fseSkillLevel[0]) && fseSkillLevel[0].ToUpper() != "NULL")
                        {
                            fseSkills = fseSkillLevel[0];
                        }
                    }
                }
                if (JobType == "Install")
                {
                    //earlyStart = (Convert.ToDateTime(DateTime.Parse(earlyStart, CultureInfo.InvariantCulture).ToShortDateString())).ToString();
                    //lateStart = (Convert.ToDateTime(earlyStart)).AddDays(3).ToString();
                    //lateStart = (Convert.ToDateTime(DateTime.Parse(lateStart, CultureInfo.InvariantCulture).ToShortDateString())).ToString();
                    duration = duration * 60;
                    earlyStart = (Convert.ToDateTime(DateTime.Parse(earlyStart, CultureInfo.GetCultureInfo("en-gb")).ToString())).ToString();
                    lateStart = (Convert.ToDateTime(earlyStart).AddDays(3)).ToString();
                }

                string taskID = string.Empty;
                if (Session["SiebelData"] != null)
                {
                    var res = (SiebelJsonToEntity)Session["SiebelData"];
                    taskID = res.serviceRequest.activityDetailList[0].activityId;
                }

                IndextoRequest objIndexToRequest = new IndextoRequest();
                objIndexToRequest.earlyStart = earlyStart;
                objIndexToRequest.lateStart = lateStart;
                objIndexToRequest.Duration = duration;
                objIndexToRequest.TaskNotes = TaskNotes;
                objIndexToRequest.FseSkillLevel = fseSkills;
                objIndexToRequest.addressArray = addressArray; //added this part pick up
                objIndexToRequest.PartComments = partcomments;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                objIndexToRequest.SRDescription = SRDesc;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                //if (Session["IndextoRequest"] != null)
                //{
                //    objIndexToRequest = (IndextoRequest)Session["IndextoRequest"];
                //}
                if (addressArray != null)
                {
                    //var addrArray = objIndexToRequest.addressArray;
                    //addrArray = addrArray.Remove(addrArray.Length - 1);
                    //if (addrArray.EndsWith(":") == true)
                    //{
                    //    addrArray = addrArray.Remove(addrArray.Length - 1);
                    //}
                    //addressArray = addressArray.Replace("\"", "");
                    if (addressArray[0].Count > 0)
                    {
                       
                        DateTime startTimeTx17 = DateTime.Now;
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                        var resPartPickUp = objCallClickService.PartPickDependency(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, addressArray, objIndexToRequest.PartComments, PreferredFSEs, objIndexToRequest.SRDescription);
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                        DateTime endTimeTx17 = DateTime.Now;
                        TimeSpan Tx17 = endTimeTx17.Subtract(startTimeTx17);
                        logger.Debug("In RequestAppointmentBooking controller ModifyVisit Method | For Modifying Task | Call to ClickCallService.cs (CallClickSerrvice.PartPickDependency) ~ " + IP + " ~ Request: with params: " + objIndexToRequest.earlyStart + " , " + objIndexToRequest.lateStart + " , " + objIndexToRequest.Duration + " , " + objIndexToRequest.TaskNotes + " , " + objIndexToRequest.FseSkillLevel + " , " + addressArray + " , " + objIndexToRequest.PartComments + " , " + PreferredFSEs + " ~ Response : " + resPartPickUp + " ~ Call duration: " + Tx17.TotalMilliseconds);
                        //
                        if (resPartPickUp == true)
                        {
                            //modifyStatus = "Modified";
                            return Json(new { message = "Success", JobType = "Part", TaskID = taskID });
                        }
                        else
                            modifyStatus = "Not Modified";

                        return Json(new { HasError = false, Message = modifyStatus });
                        //return Json(new { status = resProcessTask.FirstOrDefault().Status, TaskID = resProcessTask.FirstOrDefault().TaskID, message = "Success", taskStatus = taskStatus.ToString() });
                    }
                    else
                    {
                        //Jayesh Soni - US82 - 18/04/2017 - SR Description
                        if (objCallClickService.ModifyVisit(earlyStart, lateStart, duration, 1, TaskNotes, fseSkills, PreferredFSEs,SRDesc))
                        {
                            modifyStatus = "Modified";
                        }
                        else
                        {
                            modifyStatus = "Not Modified";
                        }

                        if (JobType == "Install")
                        {
                            Session["TaskDurationInstall"] = duration;
                        }

                    }
                }
                else
                {
                    //Jayesh Soni - US82 - 18/04/2017 - SR Description
                    if (objCallClickService.ModifyVisit(earlyStart, lateStart, duration, 1, TaskNotes, fseSkills, PreferredFSEs, SRDesc))
                    {
                        modifyStatus = "Modified";
                    }
                    else
                    {
                        modifyStatus = "Not Modified";
                    }

                    if (JobType == "Install")
                    {
                        Session["TaskDurationInstall"] = duration;
                    }


                }
                logger.Debug("In RequestAppointmentBookingController ModifyVisit method | End" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                return Json(new { HasError = false, Message = modifyStatus });
            }
            catch (Exception ex)
            {
                logger.Error("In RequestAppointmentBookingController ModifyVisit method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                //Logger.Error("Fault Exception thrown", ex);
                return Json(new { HasError = true, Message = ex.Message.ToString() });
                //Added By phani kanth
                //TempData["Errormsg"] = ex.Message;
                // TempData["Errormsg"] = "Unable to complete your request to modify the visit.<n/> Please contact the server administrator, webmaster@ge.com and inform them of the time the error occurred.";
                // return RedirectToAction("Errormsg", "ErrorPage");
            }
        }

        public ActionResult CreateVisitWithDependency()
        {
            return View();
        }

        /// <summary>
        /// Method to get slots for the requested appointment.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="appStart"></param>
        /// <param name="appFinish"></param>
        /// <param name="requiredFse"></param>
        /// <param name="TentativeFse"></param>
        /// <param name="SSOID"></param>
        /// <returns></returns>
        public ActionResult GetSlotDetails(string date, string appStart, string appFinish, bool requiredFse, string TentativeFse, string SSOID)
        {
            objCallClickService = new CallClickSerrvice();
            try
            {
                
                DateTime startTimeTx18 = DateTime.Now;
                var result = objCallClickService.GetTasksResponse(date, appStart, appFinish, requiredFse, TentativeFse, SSOID);
                DateTime endTimeTx18 = DateTime.Now;
                TimeSpan Tx18 = endTimeTx18.Subtract(startTimeTx18);
                logger.Debug("In RequestAppointmentBooking controller GetSlotDetails Method | To get task slots | Call to ClickCallService.cs (CallClickSerrvice.GetTasksResponse) ~ " + IP + " ~ Request: With params: " + date + " , " + appStart + " , " + appFinish + " , " + requiredFse + " , " + TentativeFse + " , " + SSOID + " ~ Response: List of Engineer Appointment Schedules ~ Call duration: " + Tx18.TotalMilliseconds);

                return Json(new { result, message = "Success" });
            }
            catch (Exception ex)
            {
                logger.Error("In RequestAppointmentBookingController GetSlotDetails Method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                //throw;
                return Json(new { message = "error" });
            }

        }
        [HttpGet]
        public JsonResult SessionClear()
        {
            Session.Abandon();
            return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

    }
}