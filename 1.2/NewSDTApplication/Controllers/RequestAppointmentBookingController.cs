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
        [HttpGet]
        public async Task<JsonResult> GetAppointments()
        {
            logger.Debug("RequestAppointmentBookingController;GetAppointments; Start");
            DateTime startTime = DateTime.Now;
            IndextoRequest resindextoreq = new IndextoRequest();
            resindextoreq = (IndextoRequest)Session["IndextoRequest"];
            var objser = new NewSDTApplication.Utilities.CallClickSerrvice();
            var objserWithESLS = new NewSDTApplication.Utilities.CallClickSerrvice();
            IEnumerable<AppointmentSlots> updatedSlots = null;
            try
            {
                IsExtendedSlots = false;
                logger.Debug("RequestAppointmentBookingController;API Call: ClickCallByOperation Parameters: " + "EarlyStart :" + resindextoreq.earlyStart + "LateStart :" + resindextoreq.lateStart + "Duration :" + resindextoreq.Duration + "Profile :" + resindextoreq.profile);

                objCallClickService = new CallClickSerrvice();

                //GetESLSByStandardOperation will give ES and LS dates for fetching INSLA
                var EsAndLS = objCallClickService.GetESLSByStandardOperation(resindextoreq.Duration);
                Session["EsAndLS"] = EsAndLS;
                if (EsAndLS != null)
                {
                    if ((EsAndLS.Item1 != string.Empty && EsAndLS.Item2 != string.Empty) && (EsAndLS.Item1 != EsAndLS.Item2))
                    {

                        if (Session["SelectJobType"].ToString() != "Install")
                        {
                            inSLASlotsLists = await objser.ClickCallByOperation(Convert.ToDateTime(EsAndLS.Item1).ToString("dd/MM/yyyy HH:mm"), Convert.ToDateTime(EsAndLS.Item2).ToString("dd/MM/yyyy HH:mm"),
                               resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);
                            inSLASlotsLists.ToList().ForEach(cc => cc.SLA = "IN SLA");
                        }
                        else
                        {
                            inSLASlotsLists = await objser.ClickCallByOperation(Convert.ToDateTime(EsAndLS.Item1).ToString(), Convert.ToDateTime(EsAndLS.Item2).ToString(),
                                                       resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);
                            inSLASlotsLists.ToList().ForEach(cc => cc.SLA = "IN SLA");

                        }
                    }
                }

                if (resindextoreq.earlyStart != string.Empty && resindextoreq.lateStart != string.Empty)
                {
                    if (Session["SelectJobType"].ToString() == "Install")
                    {
                        var appStart = Convert.ToDateTime(resindextoreq.earlyStart).ToString();
                        var appFinish = Convert.ToDateTime(resindextoreq.lateStart).ToString();
                        objAppSlotsWithESLS = await objser.ClickCallByOperation(appStart, appFinish, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);

                    }
                    else
                    {
                        var appStart = DateTime.ParseExact(resindextoreq.earlyStart.TrimEnd(), "dd/MM/yyyy HH:mm", null);
                        var appFinish = DateTime.ParseExact(resindextoreq.lateStart.TrimEnd(), "dd/MM/yyyy HH:mm", null);
                        objAppSlotsWithESLS = await objser.ClickCallByOperation(appStart.ToString("dd/MM/yyyy HH:mm"), appFinish.ToString("dd/MM/yyyy HH:mm"), resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, string.Empty);
                        //objAppSlotsWithESLS = objAppSlotsWithESLS.Distinct().ToList();
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
                    Bump = string.Empty,
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
                    logger.Debug("RequestAppointmentBookingController;GetAppointments; Updatedlist Logs" + item.date);

                }

                updatedlist = updatedlist.GroupBy(p => new { p.date, p.AppStart, p.AppFinish }).Select(g => g.First()).ToList();
                var jsonData = new
                {
                    total = 1,
                    page = 1,
                    records = 10,
                    rows = updatedlist,
                };
                logger.Debug("RequestAppointmentBookingController;GetAppointments; End");
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("RequestAppointmentBookingController;GetAppointments; Exception:" + ex.Message);

                inSLASlotsLists = new List<AppointmentSlots>();
                return Json(inSLASlotsLists, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RequestAppointment()
        {
            logger.Debug("RequestAppointmentBookingController;RequestAppointment -1; Start");
            logger.Debug("RequestAppointmentBookingController;RequestAppointment -1; End");
            ViewBag.PartAddresses = Session["PartCount"];
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            //Response.Cache.SetNoStore();
            return View();
        }
        [HttpPost]
        public JsonResult RequestAppointment(string EStart, string LStart, int Duration, string Profile, string TaskNotes, string SSOFse1, string SSOFse2, string SSOFse3, string fseSkill, string taskStatus, List<List<Array>> AddressArray, string desiredDate, string partcomments, string JobType, string IsRequiredfse)
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

                logger.Debug("RequestAppointmentBookingController;RequestAppointment -2 ;Start - Parameters EStart:" + EStart + " LStart:" + LStart + " Duration:" + Duration + " Profile:" + Profile + " TaskNotes:" + TaskNotes);
            }
            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
            DateTime StartTime = objCountriesTimeZoneConversion.GetTimeZone(Conversion.serviceRequest.countryCode, Conversion.serviceRequest.shipToAddress);
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
                TempData["taskStatus"] = resindextoreq.TaskStatus = taskStatus;
                resindextoreq.PreferredFSEs = preferredSSOIDs;
                resindextoreq.addressArray = AddressArray; //added this part pick up
                resindextoreq.desiredDate = desiredDate;
                if (!string.IsNullOrEmpty(partcomments))
                    resindextoreq.PartComments = partcomments;
                Session["IndextoRequest"] = resindextoreq;

                //GetESLSByStandardOperation will give ES and LS dates for fetching INSLA
                var EsAndLS = objCallClickService.GetESLSByStandardOperation(Duration);
                //Changes done by Raju Babu
                if (EsAndLS != null)
                {
                    if (EStart == "")
                    {
                        Session["SelectEarlyStart"] = EsAndLS.Item1;
                    }
                    if (LStart == "")
                    {
                        Session["SelectLateStart"] = EsAndLS.Item2;
                    }
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
                Session["SDTHomeProfile"] = Profile;
                Session["SDTHomeFseSkill"] = fseSkill;
                Session["SDTHomeIsRequiredfse"] = IsRequiredfse;



            }
            catch (Exception ex)
            {
                logger.Error("RequestAppointmentBookingController;RequestAppointment -2; Exception:" + ex.Message);

                //Logger.Error("Fault Exception thrown", ex);
                return Json(new { HasError = true, Message = "Failure" });
            }
            logger.Debug("RequestAppointmentBookingController;RequestAppointment -2; End");
            return Json(new { HasError = false, Message = "Success" });
        }

        [HttpPost]
        public async Task<ActionResult> RequestExtendedSlotsTask()
        {
            //logger.Debug("RequestAppointmentBookingController;RequestExtenderSlotTsk; Start - Parameters Date: " + date + " AppStart: " + appStart + " AppFinish: " + appFinish + " RequiredFse:" + requiredFse);
            bool isBump = false;
            DateTime startTime = DateTime.Now;
            // string inSLAColumn = string.Empty;
            if (Session["IsBump"] != null)
            {
                isBump = Convert.ToBoolean(Session["IsBump"]);
            }
            IndextoRequest resindextoreq = new IndextoRequest();
            if (Session["IndextoRequest"] != null)
            {
                resindextoreq = (IndextoRequest)Session["IndextoRequest"];
            }
            else
                logger.Fatal("RequestAppointmentBookingController;RequestExtenderSlotTsk; Session IndextoRequest is null");

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
                    resExtendedSlots = await objCallClickService.ClickCallByOperation(resindextoreq.earlyStart, resindextoreq.lateStart, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, SDTEnum.ExtendedSlotsType.ExtendedSlots.ToString());
                    var resExtendedSlotsWithOneHour = await objCallClickService.ClickCallByOperation(resindextoreq.earlyStart, resindextoreq.lateStart, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, SDTEnum.ExtendedSlotsType.ExtendedSlotsWithOneHour.ToString());
                    var resExtendedSlotsWithTwoHours = await objCallClickService.ClickCallByOperation(resindextoreq.earlyStart, resindextoreq.lateStart, resindextoreq.Duration, resindextoreq.profile, resindextoreq.PreferredFSEs, resindextoreq.FseSkillLevel, IsExtendedSlots, SDTEnum.ExtendedSlotsType.ExtendedSlotsWithTwoHours.ToString());
                    var res1 = resExtendedSlotsWithOneHour.Concat(resExtendedSlots).GroupBy(x => x.EarlyStart + "|" + x.LateStart).Select(x => x.First());
                    var res2 = resExtendedSlotsWithTwoHours.Concat(res1).GroupBy(x => x.EarlyStart + "|" + x.LateStart).Select(x => x.First());
                    resExtendedSlots = res2.ToList();
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

                            inSLASlotsLists.ToList().ForEach(x => x.Working = "Normal");
                            beforeSLA.ToList().ForEach(x => x.Working = "Extended");
                            afterSLA.ToList().ForEach(x => x.Working = "Extended");
                            ExtendedWithinESLSUI.ToList().ForEach(x => x.Working = "Extended");

                            foreach (var i in resExtendedSlots)
                            {

                                var result = objCallClickService.BumpCheck(i.EarlyStart, i.LateStart, true, i.PreferredFSE, i.SSOID.ToString());
                                if (result.Count > 0 && isBump == true && (result[0].Task.Status.DisplayString == "Tentative" || result[0].Task.Status.DisplayString == "Assigned") && (result[0].Task.SystemStatus.DisplayString == "3 (System up and running)" || result[0].Task.SystemStatus.DisplayString == "3(System up and running)"))
                                {
                                    i.Bump = "Yes";
                                    i.TaskStatus = result[0].Task.Status.DisplayString;
                                    //afterSLA.ToList().ForEach(x => x.Bump = "Yes");
                                    //beforeSLA.ToList().ForEach(x => x.Bump = "Yes");
                                }
                                else
                                {
                                    i.Bump = "";
                                    //afterSLA.ToList().ForEach(x => x.Bump = "");
                                    //beforeSLA.ToList().ForEach(x => x.Bump = "");
                                }

                            }
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

                        //updatedSlots = (from esList in updatedSlots
                        //                where (Convert.ToDateTime(esList.EarlyStart) >= Convert.ToDateTime(eStart)) &&
                        //                    Convert.ToDateTime(esList.LateStart) <= Convert.ToDateTime(lStart)
                        //                select esList
                        //            );

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
                    Bump = r.Bump,
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
                    logger.Debug("RequestAppointmentBookingController;Request Extended Slots; Updatedlist Logs " + item.date);

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
                logger.Debug("RequestAppointmentBookingController;RequestExtendedSlotsTask; End");
                //DateTime endTime = DateTime.Now;

                //TimeSpan span = endTime.Subtract(startTime);
                //ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                //Session["ReqTimeSpan"] = span.Minutes * 60 + span.Seconds;
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("RequestAppointmentBookingController;RequestExtendedSlotsTask; Exception:" + ex.Message);
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                Session["ReqTimeSpan"] = span.Minutes * 60 + span.Seconds;
                ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;

                return Json(new { status = SDTEnum.OperationStatus.NotProcessed.ToString(), message = ex.Message.ToString() });
            }
        }
        [HttpPost]
        public async Task<ActionResult> ProcessTsk(string date, string appStart, string appFinish, bool requiredFse, string isBumpTask, string Schedulingpolicy)
        {
            //Session["IsRequiredFse"] = requiredFse;
            Session["Schedulingpolicy"] = Schedulingpolicy;
            //IsBump Task added by Phani Kanth P.
            Session["IsBumpTask"] = isBumpTask;

            int taskStatus = 0;
            logger.Debug("RequestAppointmentBookingController;ProcessTsk; Start - Parameters Date: " + date + " AppStart: " + appStart + " AppFinish: " + appFinish + " RequiredFse:" + requiredFse);
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
                logger.Fatal("RequestAppointmentBookingController;ProcessTsk; Session IndextoRequest is null");

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

                    var resPartPickUp = objCallClickService.PartPickDependency(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, objIndexToRequest.addressArray, objIndexToRequest.PartComments, objIndexToRequest.PreferredFSEs, DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                                      DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"));
                    Session["PartToolReturn"] = "false";
                    DateTime endTime = DateTime.Now;

                    TimeSpan span = endTime.Subtract(startTime);
                    ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                    return Json(new { message = "Success", JobType = "Part", TaskID = taskID });
                    //return Json(new { status = resProcessTask.FirstOrDefault().Status, TaskID = resProcessTask.FirstOrDefault().TaskID, message = "Success", taskStatus = taskStatus.ToString() });

                }
                else
                {
                    logger.Debug("RequestAppointmentBookingController;ProcessTsk; API Call: ClickCallToCreateTask Parameters: " + "EarlyStart :" + objIndexToRequest.earlyStart + "LateStart :" + objIndexToRequest.lateStart +
                        "Duration :" + Convert.ToInt32(objIndexToRequest.Duration) + "Start :" + DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm") + " Finish: " + DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"));


                    var resProcessTask = await objCallClickService.ClickCallToCreateTask(objIndexToRequest.earlyStart,
                                                                     objIndexToRequest.lateStart,
                                                                     Convert.ToInt32(objIndexToRequest.Duration),
                                                                     objIndexToRequest.TaskNotes,
                                                                     objIndexToRequest.FseSkillLevel,
                                                                     objIndexToRequest.PreferredFSEs,
                                                                     DateTime.Parse(aStart, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                     DateTime.Parse(aFinish, CultureInfo.GetCultureInfo("en-US")).ToString("dd/MM/yyyy HH:mm"),
                                                                     Convert.ToBoolean(Session["IsRequiredFse"].ToString()));

                    if (resProcessTask == null)
                        logger.Debug("RequestAppointmentBookingController;ProcessTsk;API Response: Process Task is NULL ");

                    DateTime endTime = DateTime.Now;

                    TimeSpan span = endTime.Subtract(startTime);
                    ViewBag.ReqTimeSpan = span.Minutes * 60 + span.Seconds;
                    logger.Debug("RequestAppointmentBookingController;ProcessTsk; End");

                    return Json(new { status = resProcessTask.FirstOrDefault().Status, TaskID = resProcessTask.FirstOrDefault().TaskID, message = "Success", taskStatus = taskStatus.ToString() });
                }
            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("RequestAppointmentBookingController;ProcessTsk; Exception:" + ex.Message);

                return Json(new { status = SDTEnum.OperationStatus.NotProcessed.ToString(), message = ex.Message.ToString() });
            }
        }


        [HttpPost]
        public async Task<ActionResult> ProcessTaskwithoutAppointment(string EStart, string LStart, int Duration, string TaskNotes, List<List<Array>> addressArray, string fseSkill, string PreferredFSEs, string partcomments, string taskOpenDate, string jobType, string IsRequiredfse)
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
            logger.Debug("RequestAppointmentBookingController;ProcessTaskwithoutAppointment; Start - Parameters EStart: " + EStart + " LStart: " + LStart + " Duration: " + Duration);
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
                    var resPartPickUp = objCallClickService.PartPickDependency(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, addressArray, objIndexToRequest.PartComments, objIndexToRequest.PreferredFSEs);
                    return Json(new { message = "Success", JobType = "Part", TaskID = taskID });
                }
                else
                {
                    logger.Debug("RequestAppointmentBookingController;ProcessTaskwithoutAppointment; API Call: ClickCallToCreateTask Parameters: " + "EarlyStart :" + objIndexToRequest.earlyStart + "LateStart :" + objIndexToRequest.lateStart);
                    //var resProcessTask = await objCallClickService.ClickCallToCreateTask(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes);
                    taskappointmentsSolt = await objCallClickService.ClickCallToCreateTask(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, objIndexToRequest.PreferredFSEs);
                    logger.Debug("RequestAppointmentBookingController;ProcessTaskwithoutAppointment; End");

                }
                return Json(new { status = taskappointmentsSolt.FirstOrDefault().Status, TaskID = taskappointmentsSolt.FirstOrDefault().TaskID, message = "Success" });

            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("RequestAppointmentBookingController;ProcessTaskwithoutAppointment; Exception:" + ex.Message);

                return Json(new { status = SDTEnum.OperationStatus.NotProcessed.ToString(), message = ex.Message.ToString() });
            }
        }

        [HttpPost]
        public ActionResult CancelTask(string CancelReason)
        {
            string cancelStatus = string.Empty;
            try
            {
                logger.Debug("RequestAppointmentBookingController;CancelTask; Start");

                objCallClickService = new CallClickSerrvice();
                GetTasksResponse objGetTaskResponse = new GetTasksResponse();
                var res = (HTTPPostParams)Session["SiebelHttpPostParams"];
                if (objCallClickService.CancelTask(CancelReason))
                    cancelStatus = "Cancelled";
                else
                    cancelStatus = "notcancelled";

                logger.Debug("RequestAppointmentBookingController;CancelTask; End");

                return Json(new { message = cancelStatus });
            }
            catch (Exception ex)
            {
                logger.Error("RequestAppointmentBookingController;CancelTask; Exception:" + ex.Message);

                //throw;
                //Added By phani kanth
                TempData["Errormsg"] = ex.Message;
                //TempData["Errormsg"] = "Unable to complete your request to Cancel.<n/> Please contact the server administrator, webmaster@ge.com and inform them of the time the error occurred.";
                // return RedirectToAction("Errormsg", "ErrorPage");
                return Json(new { message = ex.Message.ToString() });
            }


        }

        [HttpPost]
        public ActionResult ModifyVisit(string earlyStart, string lateStart, int duration, List<List<Array>> addressArray, string TaskNotes, string fseSkill, string PreferredFSEs, string JobType, string partcomments, string IsRequiredfse)
        {

            Session["IsRequiredFse"] = IsRequiredfse;
            // string[] arrayAddress = addressArray.Split('=');
            string fseSkills = string.Empty;


            //foreach (int i in list)
            //{
            //    Address.Add(i.ToString());
            //}
            logger.Debug("RequestAppointmentBookingController;ModifyVisit; Start");

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
                    var resPartPickUp = objCallClickService.PartPickDependency(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, addressArray, objIndexToRequest.PartComments, PreferredFSEs);
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
                    if (objCallClickService.ModifyVisit(earlyStart, lateStart, duration, 1, TaskNotes, fseSkills, PreferredFSEs))
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
                logger.Debug("RequestAppointmentBookingController;ModifyVisit; End");

                return Json(new { HasError = false, Message = modifyStatus });
            }
            catch (Exception ex)
            {
                logger.Error("RequestAppointmentBookingController;ModifyVisit; Exception:" + ex.Message);
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
        public ActionResult GetSlotDetails(string date, string appStart, string appFinish, bool requiredFse, string TentativeFse, string SSOID)
        {
            objCallClickService = new CallClickSerrvice();
            try
            {
                var result = objCallClickService.GetTasksResponse(date, appStart, appFinish, requiredFse, TentativeFse, SSOID);


                return Json(new { result, message = "Success" });
            }
            catch (Exception ex)
            {
                logger.Error("RequestAppointmentBookingController;GetSlotDetails; Exception:" + ex.Message);
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