using NewSDTApplication.Models;
using NewSDTApplication.Utilities;
using NewSDTApplication.ViewModels;
using Newtonsoft.Json;
using SDTLogger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NewSDTApplication.Controllers
{
    [SessionExpire]
    public class HomeController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<ActionResult> Index()
        {
            CountriesTimeZoneConversion objCountriesTimeZoneConversion;
            logger.Debug("----------------------------------------------------------------- ");
            logger.Debug("HomeController;Index; Start ");
            HTTPPostParams SiebelHttpPostParams = new HTTPPostParams();
            var completeURL = ConfigurationManager.AppSettings["SiebelURL"].ToString();
            DateTime startTime = DateTime.Now;

            logger.Debug("In Home controller Index Method:Activity" + Request.Form["ACTIVITY_ID"]);
            #region Set Values from HTTP Post
            if (Session["SiebelHttpPostParams"] != null && string.IsNullOrEmpty(Request.Form["SERVICE_REQUEST_NUMBER"]))
            {

                SiebelHttpPostParams = (HTTPPostParams)Session["SiebelHttpPostParams"];
            }
            else
            {
                // Clear cache
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                //// Session.Abandon();
                //Response.Cache.SetNoStore();
                Session.Clear();

                if (!string.IsNullOrEmpty(Request.Form["ACTIVITY_ID"]))
                {
                    logger.Debug("In Home controller Index Method:SYSTEM ID Start" + Request.Form["SYSTEM_ID"]);

                    if (!string.IsNullOrEmpty(Request.Form["SYSTEM_ID"]))
                    {
                        logger.Debug("In Home controller Index Method:SYSTEM ID IF" + Request.Form["SYSTEM_ID"]);

                        SiebelHttpPostParams.SystemID = Request.Form["SYSTEM_ID"];
                    }
                    else
                    {
                        logger.Debug("In Home controller Index Method:SYSTEM ID End" + Request.Form["SYSTEM_ID"]);

                        SiebelHttpPostParams.SystemID = "";
                    }
                    SiebelHttpPostParams.SerialNumber = Request.Form["SERIAL_NUMBER"];
                    SiebelHttpPostParams.ServiceRequestNumber = Request.Form["SERVICE_REQUEST_NUMBER"];
                    SiebelHttpPostParams.ActivityNo = Request.Form["ACTIVITY_ID"];
                    SiebelHttpPostParams.ServiceRequestID = Request.Form["SERVICE_REQUEST_ID"];
                    SiebelHttpPostParams.ShipToSite = Request.Form["SHIP_TO_SITE"];
                }
                else
                {
                    logger.Debug("In Home controller Index Method Activity is null" + Request.Form["ACTIVITY_ID"] + "Service Request Number:" + Request.Form["SERVICE_REQUEST_NUMBER"]);
                    // Session["SiebelHttpPostParams"] = SiebelHttpPostParams;
                      //return RedirectToAction("NotAuthorizedPage", "ErrorPage");
                }

                SiebelHttpPostParams.ActivityNo = string.IsNullOrEmpty(SiebelHttpPostParams.ActivityNo) ? "1-24KVRC4" : SiebelHttpPostParams.ActivityNo;
                SiebelHttpPostParams.ServiceRequestNumber = string.IsNullOrEmpty(SiebelHttpPostParams.ServiceRequestNumber) ? "1-4630503223" : SiebelHttpPostParams.ServiceRequestNumber;
                SiebelHttpPostParams.ShipToSite = string.IsNullOrEmpty(SiebelHttpPostParams.ShipToSite) ? "4347545" : SiebelHttpPostParams.ShipToSite;
                SiebelHttpPostParams.SystemID = string.IsNullOrEmpty(SiebelHttpPostParams.SystemID) ? "0835270013" : SiebelHttpPostParams.SystemID;


                Session["SiebelHttpPostParams"] = SiebelHttpPostParams;

                Session["SiebelHttpPostParamsActivityNo"] = SiebelHttpPostParams.ActivityNo;

                logger.Debug("HomeController;Index; ActivityNo:" + SiebelHttpPostParams.ActivityNo + " SerialNumber: " + SiebelHttpPostParams.SerialNumber + " ServiceRequestID: " + SiebelHttpPostParams.ServiceRequestID + " ServiceRequestNumber: " + SiebelHttpPostParams.ServiceRequestNumber + " ShipToSite: " + SiebelHttpPostParams.ShipToSite + " SystemID: " + SiebelHttpPostParams.SystemID);
            }
            SessionHanlder.IsSessionExpired = true;
            completeURL += "serviceRequestId=" + SiebelHttpPostParams.ServiceRequestID + "&activityId=" + SiebelHttpPostParams.ActivityNo + "&serviceRequestNumber=" + SiebelHttpPostParams.ServiceRequestNumber;
            //completeURL += "SYSTEM_ID=" + SystemID + "&SERIAL_NUMBER=" + SerialNumber + "&SERVICE_REQUEST_NUMBER=" + ServiceRequestNumber + "&ACTIVITY_ID=" + ActivityNo + "&SERVICE_REQUEST_ID=" + ServiceRequestID + "&SHIP_TO_SITE=" + ShipToSite;
            #endregion

            #region Call To Click to fetch Task Details.

            CallClickSerrvice objClickCallService = new CallClickSerrvice();
            ViewBag.TaskSiteID = SiebelHttpPostParams.ShipToSite;
            ViewBag.TaskSystemID = SiebelHttpPostParams.SystemID;
            ClickTasksResSystemSite objClickTasksResponse = new ClickTasksResSystemSite();

            try
            {
                //objClickCallService.getSiebelTaskNumber();

                var ClickResponseSystemId = objClickCallService.GetDateDetails(SiebelHttpPostParams.SystemID);
                if (ClickResponseSystemId != null)
                {
                    if (string.IsNullOrEmpty(ClickResponseSystemId.ID))
                    {
                        ViewBag.ValidateSystemId = "SystemIDNotExit";
                    }
                }
                else
                {
                    ViewBag.ValidateSystemId = "SystemIDNotExit";
                }

                var ClickResponse = objClickCallService.GetSiteDetails(SiebelHttpPostParams.ShipToSite);
                if (ClickResponse != null)
                {
                    if (!string.IsNullOrEmpty(ClickResponse.Street))
                    {
                        ViewBag.street = ClickResponse.Street.Replace(",", " ") == null ? "" : ClickResponse.Street.Replace(",", " ");
                    }
                    else
                    {

                        ViewBag.ValidateShipToSite = "SiteIDNotExit";
                    }
                }
                else
                {
                    ViewBag.ValidateShipToSite = "SiteIDNotExit";
                }
                if (ClickResponse != null)
                {
                    if (!string.IsNullOrEmpty(ClickResponse.City))
                        ViewBag.city = ClickResponse.City.Replace(",", " ") == null ? "" : ClickResponse.City.Replace(",", " ");
                    ViewBag.postcode = ClickResponse.Postcode == null ? "" : ClickResponse.Postcode;
                    ViewBag.country = ClickResponse.CountryID == null ? "" : ClickResponse.CountryID.DisplayString;
                    ViewBag.latitude = ClickResponse.Latitude;
                    ViewBag.longitude = ClickResponse.Longitude;
                }
                //var resTasksResponseByTask = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.ActivityNo, "TaskActivityID");
                // var resTasksResponseByTask = await objClickCallService.GetTasksRequestByPropertyName("APAC" + "-" + SiebelHttpPostParams.ActivityNo + "-" + "1", "TaskActivityID");
                var resTasksResponseByTask = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.ActivityNo, "TaskID");

                Session["resTasksResponseByTask"] = resTasksResponseByTask.Tasks.Where(x => x.TaskType.DisplayString != "Parts Pickup");


                // Save the part pick up addresses in memory and load those values when returned back to the landing page 
                // Code added by sudhir

                System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
new System.Web.Script.Serialization.JavaScriptSerializer();


                string tempAddArray = string.Empty;
                string sJSON = "";

                if (Session["PartToolTempAddr"] != null && Session["PartToolReturn"] == "true")
                {
                    //Changes done by Raju
                    //removed Splitting.. Special char related defect fixes
                    List<List<Array>> AddressArray = (List<List<Array>>)Session["PartToolTempAddr"];
                    ArrayList list = new ArrayList();
                    for (int i = 0; i < AddressArray[0].Count; i++)
                    {
                        list.Add(AddressArray[0][i]);

                    }

                    List<PartToolAddress> lstServiceRequest = new List<PartToolAddress>();
                    //  tempAddArray = Session["PartToolTempAddr"].ToString();
                    // List<List<Array>> arrayAddress =  Session["PartToolTempAddr"];
                    // string[] arrayAddress = tempAddArray.Split('=');


                    int number = 1;
                    for (int j = 0; j < list.Count; j++)
                    {

                        string[] addres = list[j] as string[];

                        //for (int i = 0; i < arrayAddress.Length; i++)
                        //{
                        //if (arrayAddress[i].Contains("KOREA, REPUBLIC OF"))
                        //{
                        //    arrayAddress[i] = arrayAddress.Select(x => x.Replace("KOREA, REPUBLIC OF", "KOREA REPUBLIC OF")).ToArray();

                        //    //arrayAddress[i].Replace("KOREA, REPUBLIC OF", "KOREA REPUBLIC OF");
                        //}

                        // string[] addres = arrayAddress[i].Split(',');
                        //addres = addres.Select(x => x.Replace("KOREA, REPUBLIC OF", "KOREA REPUBLIC OF")).ToArray();
                        PartToolAddress objServiceRequest = new PartToolAddress();
                        objServiceRequest.PartDeliveryType = addres[0].Replace("\"", "");
                        objServiceRequest.Street = addres[1].Replace("\"", "").Replace("\n", "\\n");
                        objServiceRequest.City = addres[2].Replace("\"", "");
                        objServiceRequest.Postcode = addres[3].Replace("\"", "");
                        objServiceRequest.CountryID = addres[4].Replace("\"", "");
                        objServiceRequest.PartComment = addres[5].Replace("\"", "").Replace("\n", "\\n");
                        objServiceRequest.deliveryDate = addres[6].Replace("\"", "");
                        objServiceRequest.Latitude = addres[7].Replace("\"", "");
                        objServiceRequest.Longitude = addres[8].Replace("\"", "");
                        if (addres.Length > 9)
                        {
                            if (!string.IsNullOrEmpty(addres[9]))
                            {
                                objServiceRequest.Number = addres[9];

                            }
                            //else
                            //{
                            //    objServiceRequest.Number = i + 2;
                            //}
                        }
                        if (addres.Length > 10)
                        {
                            objServiceRequest.Status = addres[10].Replace("\"", "");

                        }
                        if (addres.Length > 11)
                        {
                            objServiceRequest.IsMST = addres[11].Replace("\"", "");
                            objServiceRequest.IsCritical = addres[12].Replace("\"", "");
                        }
                        else
                        {
                            objServiceRequest.IsMST = "";
                            objServiceRequest.IsCritical = "";
                        }
                        lstServiceRequest.Add(objServiceRequest);
                        number++;
                    }

                    sJSON = oSerializer.Serialize(lstServiceRequest);
                    Session["PartCount"] = sJSON.Replace(@"\""", "");
                    ViewBag.PartAddresses = sJSON;
                }
                else
                {

                    //In part tools status [Status] column added by Phani Kanth p
                    var partAddrCutomerSite = resTasksResponseByTask.Tasks.
                        Where(x => x.PartDeliveryType == "\"Customer Site" &&
                        x.TaskType.DisplayString == "Parts Pickup")
                        .Select(i => new
                        {
                            Street = ClickResponse.Street.Replace(",", " ") == null ? "" : ClickResponse.Street.Replace(",", " ").Replace("\n", "\\n"),
                            City = ClickResponse.City.Replace(",", " ") == null ? "" : ClickResponse.City.Replace(",", " "),
                            Postcode = ClickResponse.Postcode == null ? "" : ClickResponse.Postcode,
                            PartDeliveryType = i.PartDeliveryType,
                            CountryID = ClickResponse.CountryID.DisplayString,
                            PartComment = i.PartComment.Replace("\n", "\\n"),
                            deliveryDate = i.PartEstimatedDeliveryDate.ToString("dd/MM/yyyy HH:mm"),
                            Latitude = ClickResponse.Latitude,
                            Longitude = ClickResponse.Longitude,
                            Number = i.Number,
                            Status = i.Status.DisplayString,
                            IsMST = i.IsMST,
                            IsCritical = i.Critical
                        }).ToList();

                    var items = resTasksResponseByTask.Tasks.
                        Where(x => x.Number != 1 && x.TaskType.DisplayString == "Parts Pickup" && x.PartDeliveryType != "\"Customer Site" && x.PartDeliveryType != string.Empty)
                        .Select(i => new
                        {
                            Street = i.Street.Replace("\n", "\\n"),
                            City = i.City,
                            Postcode = i.Postcode,
                            PartDeliveryType = i.PartDeliveryType,
                            CountryID = i.CountryID.DisplayString,
                            PartComment = i.PartComment.Replace("\n", "\\n"),
                            deliveryDate = i.PartEstimatedDeliveryDate.ToString("dd/MM/yyyy HH:mm"),
                            Latitude = i.Latitude,
                            Longitude = i.Longitude,
                            Number = i.Number,
                            Status = i.Status.DisplayString,
                            IsMST = i.IsMST,
                            IsCritical = i.Critical
                        }).ToList();



                    var partAddress = items.Concat(partAddrCutomerSite).OrderBy(i => i.Number);

                    sJSON = oSerializer.Serialize(partAddress);
                    Session["PartCount"] = sJSON.Replace(@"\""", "");
                    ViewBag.PartAddresses = sJSON.Replace(@"\""", "");
                }


                Session["PartaddressArray"] = null;
                Session["PartToolReturn"] = null;
                //resTasksResponseByTask.Tasks = resTasksResponseByTask.Tasks.Where(x => x.Number == 1).ToArray();
                resTasksResponseByTask.Tasks = resTasksResponseByTask.Tasks.Where(x => x.TaskType.DisplayString != "Parts Pickup").ToArray();

                if (Session["TaskResponseByTask"] != null)
                {
                    Session["TaskResponseByTask"] = null;
                }

                Session["TaskResponseByTask"] = resTasksResponseByTask;

                #region Check for Task Cancellation

                if (resTasksResponseByTask.Tasks.Length > 0)
                {
                    objClickTasksResponse.AppointmentFinish = resTasksResponseByTask.Tasks[0].AppointmentFinish;
                    objClickTasksResponse.AppointmentStart = resTasksResponseByTask.Tasks[0].AppointmentStart;
                    objClickTasksResponse.TaskExists = true;
                    string taskStatus = string.Empty;
                    taskStatus = resTasksResponseByTask.Tasks[0].Status.Text[0].ToString();
                    if (Session["taskStatus"] != null)
                    {
                        Session["taskStatus"] = null;
                    }
                    ViewBag.taskExistsStatus = "Exists";
                    Session["taskStatus"] = "Exists";
                    //if (!string.IsNullOrEmpty(taskStatus))
                    //{
                    //    if (taskStatus == "New" || taskStatus == "Rejected" || taskStatus == "Cancelled" || taskStatus == "Tentative" || taskStatus == "Acknowledged" || taskStatus == "Rejected by FSE" || taskStatus == "Assigned")
                    //        objClickTasksResponse.TaskCancelAllowed = true;

                    //    else
                    //        objClickTasksResponse.TaskCancelAllowed = false;
                    //}

                    if (!string.IsNullOrEmpty(taskStatus))
                    {
                        if (Session["taskStatusvalue"] != null)
                        {
                            Session["taskStatusvalue"] = null;
                        }
                        Session["taskStatusvalue"] = taskStatus;
                        if (taskStatus == SDTEnum.TaskStatusValue.New.ToString() || taskStatus == SDTEnum.TaskStatusValue.Tentative.ToString() || taskStatus == SDTEnum.TaskStatusValue.Assigned.ToString() || taskStatus == SDTEnum.TaskStatusValue.Acknowledged.ToString() || taskStatus == SDTEnum.TaskStatusValue.Rejected.ToString() || taskStatus == "Rejected by FSE")
                            objClickTasksResponse.TaskCancelAllowed = true;

                        else
                            objClickTasksResponse.TaskCancelAllowed = false;
                    }
                    if (Session["MUSTJobNumber"] != null)
                    {
                        Session["MUSTJobNumber"] = null;
                    }
                    Session["MUSTJobNumber"] = resTasksResponseByTask.Tasks[0].MUSTJobNumber;

                    if (!string.IsNullOrEmpty(Session["PreferredFSEs"] as string))
                        Session["PreferredFSEs"] = null;

                    Session["PreferredFSEs"] = resTasksResponseByTask.Tasks[0].PreferredFSEs;

                    if (!string.IsNullOrEmpty(Session["RreferredFSEs"] as string))
                        Session["RreferredFSEs"] = null;

                    Session["RreferredFSEs"] = resTasksResponseByTask.Tasks[0].RequiredFSEs;

                    //Session["PreferredFSEs"] = resTasksResponseByTask.Tasks[0].PreferredFSEs;
                    //Session["RreferredFSEs"] = resTasksResponseByTask.Tasks[0].RequiredFSEs;
                    Session["SelectedFSE"] = resTasksResponseByTask.Tasks[0].SkillLevel;

                    Session["fseskills"] = resTasksResponseByTask.Tasks[0].SkillLevel;

                    Session["ModifiedES"] = resTasksResponseByTask.Tasks[0].EarlyStart;

                    Session["ModifiedLS"] = resTasksResponseByTask.Tasks[0].LateStart;
                    Session["ModifiedDuration"] = resTasksResponseByTask.Tasks[0].Duration == null ? 0 : resTasksResponseByTask.Tasks[0].Duration / 60;
                    // Session["ModifiedInstallDuration"] = resTasksResponseByTask.Tasks[0].Duration == null ? 0 : resTasksResponseByTask.Tasks[0].Duration / 60;
                    Session["TaskNotes"] = resTasksResponseByTask.Tasks[0].Notes == null ? string.Empty : resTasksResponseByTask.Tasks[0].Notes;
                    Session["PostCode"] = resTasksResponseByTask.Tasks[0].Postcode;
                    Session["Address"] = resTasksResponseByTask.Tasks[0].Street;
                    Session["City"] = resTasksResponseByTask.Tasks[0].City;
                    Session["CountryID"] = resTasksResponseByTask.Tasks[0].CountryID.DisplayString;
                    ViewBag.calltestiddata = resTasksResponseByTask.Tasks[0].CallID.ToString();
                    ViewBag.checktaskids = resTasksResponseByTask.Tasks[0].TaskID == null ? "" : resTasksResponseByTask.Tasks[0].TaskID.ToString();
                    Session["checktaskids"] = ViewBag.checktaskids;
                    ViewBag.checkStatusVal = resTasksResponseByTask.Tasks[0].Status.Text[0].ToString();
                    logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " Task Duartion: " + Session["ModifiedDuration"]);
                    logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " SessionTest: Task Duartion: " + resTasksResponseByTask.Tasks[0].Duration);


                    // ViewBag.checkStatusVal = "Tentative";
                    //ViewBag.checkfses = resTasksResponseByTask.Tasks[0].RequiredFSEs == null ? "" : resTasksResponseByTask.Tasks[0].RequiredFSEs.ToString();                    
                    if (Session["RreferredFSEs"].ToString() != "")
                    {
                        ViewBag.ReqPrefFlag = "R";
                    }
                    else { ViewBag.ReqPrefFlag = "P"; }
                    if (resTasksResponseByTask.Assignments.Length > 0)
                    {
                        ViewBag.checkfses = resTasksResponseByTask.Assignments[0].AssignedEngineers == null ? string.Empty : resTasksResponseByTask.Assignments[0].AssignedEngineers.ToString();
                    }
                    else
                    {
                        ViewBag.checkfses = string.Empty;
                    }
                    ViewBag.appointmentStart = resTasksResponseByTask.Tasks[0].AppointmentStart == Convert.ToDateTime("12/30/1899 12:00:00 AM") ? "" : resTasksResponseByTask.Tasks[0].AppointmentStart.ToString("dd/MM/yyyy HH:mm");
                    ViewBag.appointmentFinish = resTasksResponseByTask.Tasks[0].AppointmentFinish == Convert.ToDateTime("12/30/1899 12:00:00 AM") ? "" : resTasksResponseByTask.Tasks[0].AppointmentFinish.ToString("dd/MM/yyyy HH:mm");

                }
                else
                {
                    if (Session["taskStatus"] != null)
                    {
                        Session["taskStatus"] = null;
                    }
                    Session["taskStatus"] = "New";
                }

                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                if (Session["clickReceiveTime"] != null)
                {
                    Session["clickReceiveTime"] = 0;
                }
                Session["clickReceiveTime"] = span.Minutes * 60 + span.Seconds;
                #endregion
            }
            catch (Exception ex)
            {
                //logger.Error("HomeController;Index; The server encountered an network issue, please check network connection or reboot the system");

                logger.Error("HomeController;Index; Exception from Click call:" + ex.Message);
                TempData["ErrorCode"] = "500";

                //Added By phani kanth
                TempData["Errormsg"] = ex.Message;

                // TempData["Errormsg"] = "The click server encountered an network error and was unable to complete your request.";
                //TempData["Errormsg"] = "The server encountered an network error and was unable to complete your request.<n/> Please contact the server administrator, webmaster@ge.com and inform them of the time the error occured.";
                return RedirectToAction("Errormsg", "ErrorPage");
            }

            // ViewBag.clickReceiveTime = DateTime.Now;
            objClickTasksResponse.SiteId = SiebelHttpPostParams.ShipToSite;
            objClickTasksResponse.SystemId = SiebelHttpPostParams.SystemID;

            #endregion
            #region OAuth Cookie implementation
            NewSDTApplication.Program.AccessToken OAuthAccessToken;
            string headerValue = string.Empty;
            DateTime seiblestartTime = DateTime.Now;

            if (HttpContext.Request.Cookies["AccessTokenObject"].HasKeys == false)
            {
                try
                {
                    string clientID = ConfigurationManager.AppSettings["ClientID"];
                    string clientSecret = ConfigurationManager.AppSettings["Clientsecret"];
                    string scope = ConfigurationManager.AppSettings["Scope"];
                    NewSDTApplication.Program.AccessToken.OAuthAuthentication OAuth = new NewSDTApplication.Program.AccessToken.OAuthAuthentication(clientID, clientSecret, scope);
                    OAuthAccessToken = OAuth.GetAccessToken();
                    // Create a header with the access_token property of the returned token
                    headerValue = "Bearer " + OAuthAccessToken.access_token;
                    // Program.DetectMethod(headerValue);
                }

                catch (Exception ex)
                {
                    logger.Error("HomeController;Index; Exception:" + ex.Message);

                    //Logger.Error("Exception thrown at Home Controller Index Action", ex);
                    //  return View();
                }

            }

            HttpCookie aCookie = HttpContext.Request.Cookies["AccessTokenObject"];

            if (string.IsNullOrEmpty(headerValue))
            {
                headerValue = "Bearer " + aCookie.Values["Token"].ToString();
            }
            #endregion
            #region Call to Sibel and Sending the response to the View

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(completeURL);
            request.Method = "GET";
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "application/json";
            // request.Headers.Add("X-SSO_ID:305017059");  - No header to be passed in Dev Api
            // request.Headers.Add("X-SSO_ID:501314520");  // For Stage API SSO
            request.Headers.Add("Authorization", headerValue);
            request.Accept = "application/json";
            try
            {

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {

                        using (StreamReader objres = new StreamReader(stream))
                        {

                            string str = objres.ReadToEnd();

                            Models.SiebelJsonToEntity objSiebelResponse = new SiebelJsonToEntity();
                            objSiebelResponse = JsonConvert.DeserializeObject<SiebelJsonToEntity>(str);
                            if (objSiebelResponse != null && objSiebelResponse.serviceRequest != null)
                            {
                                //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                                if (objSiebelResponse.serviceRequest.activityDetailList.Count() > 0 && !string.IsNullOrEmpty(objSiebelResponse.serviceRequest.currentEquiptmentStatus))
                                {
                                    if ((objSiebelResponse.serviceRequest.currentEquiptmentStatus.Contains('1') || objSiebelResponse.serviceRequest.currentEquiptmentStatus.Contains('2') || objSiebelResponse.serviceRequest.gEHCSafetyConcern.ToLower() == "yes" || objSiebelResponse.serviceRequest.gEHCSafetyConcern.ToLower() == "actual" || objSiebelResponse.serviceRequest.gEHCSafetyConcern.ToLower() == "potential") && objSiebelResponse.serviceRequest.srType == "Corrective Repair")
                                    {
                                        Session["IsBump"] = true;
                                    }
                                }
                            }
                            else
                            {
                                logger.Error("HomeController;Index; No Record available at Siebel");
                                TempData["ErrorCode"] = "204";
                                TempData["Errormsg"] = "Activity Details couldn't  be found, Please try with a valid activity";
                                return RedirectToAction("Errormsg", "ErrorPage");
                            }
                            if (Session["objClickTasksResponse"] != null)
                            {
                                Session["objClickTasksResponse"] = null;
                            }
                            Session["objClickTasksResponse"] = objClickTasksResponse;
                            if (Session["IsTaskExistsForDependencies"] != null)
                            {
                                Session["IsTaskExistsForDependencies"] = null;
                            }
                            Session["IsTaskExistsForDependencies"] = objClickTasksResponse.TaskExists;

                            if (objSiebelResponse.serviceRequest.owner != "")
                            {
                                if (Session["objPreferredSFESiebel"] != null)
                                {
                                    Session["objPreferredSFESiebel"] = null;
                                }
                                Session["objPreferredSFESiebel"] = objSiebelResponse.serviceRequest.owner;

                            }
                            //objSiebelResponse.serviceRequest.gEHCSchedDate.to
                            var ViewModel = new SiebelTaskResponseViewModel { ClickTaskResponseAttributes = objClickTasksResponse, SiebelResponseAttributes = objSiebelResponse };

                            if (!string.IsNullOrEmpty(Session["objSiebelResponseforType"] as string))
                                Session["objSiebelResponseforType"] = null;

                            Session["objSiebelResponseforType"] = ViewModel.SiebelResponseAttributes.serviceRequest.srType;

                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count > 0)
                            {
                                //  ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment = (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment).Replace("\n", "\\n");

                                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                var newDDate = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);
                                ViewBag.DefaultDate = Convert.ToDateTime(newDDate).ToString("dd/MM/yyyy HH:mm");

                                if (objSiebelResponse.serviceRequest == null)
                                    logger.Debug("HomeController;Index; Siebel Response is NULL");
                                else
                                    logger.Debug("HomeController;Index; Siebel Response is Coming");
                                if (Session["SiebelData"] != null)
                                {
                                    Session["SiebelData"] = null;
                                }
                                Session["SiebelData"] = objSiebelResponse;
                                logger.Debug("HomeController;Index; Session logger:" + objSiebelResponse.serviceRequest.activityDetailList[0].activityUID + " Task Duartion: " + Session["ModifiedDuration"] + "Req Duration:" + Session["SDTHomeDuration"]);

                                if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.srType))
                                    ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;

                                if (ViewModel.SiebelResponseAttributes.serviceRequest.gehcHours != "" || ViewModel.SiebelResponseAttributes.serviceRequest.gehcMinutes != "")
                                {
                                    int hrs = ViewModel.SiebelResponseAttributes.serviceRequest.gehcHours != "" ? (Convert.ToInt32(ViewModel.SiebelResponseAttributes.serviceRequest.gehcHours) * 60) : 0;
                                    int mins = ViewModel.SiebelResponseAttributes.serviceRequest.gehcMinutes != "" ? Convert.ToInt32(ViewModel.SiebelResponseAttributes.serviceRequest.gehcMinutes) : 0;

                                    if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                    {
                                        ViewBag.TaskDurationInstall = hrs;
                                        ViewBag.TaskDuration = 0;
                                    }
                                    else
                                    {
                                        ViewBag.TaskDuration = hrs + mins;
                                        ViewBag.TaskDurationInstall = 0;

                                    }
                                    logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " View Bag Task Duartion: " + ViewBag.TaskDuration);


                                }
                                else
                                {
                                    ViewBag.TaskDurationInstall = 0;
                                    ViewBag.TaskDuration = 0;

                                    logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " View Bag Task Duartion: " + ViewBag.TaskDuration);

                                }
                                if (Session["SelectedFSE"] != null)
                                {
                                    // if (!string.IsNullOrEmpty(Session["SelectedFSE"] as string))
                                    ViewModel.SiebelResponseAttributes.serviceRequest.gehcFse1 = Convert.ToString(Session["SelectedFSE"]);
                                }



                                if (TempData["duration"] != null)
                                {
                                    if (TempData["duration"] != "")
                                    {


                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            ViewBag.TaskDurationInstall = Convert.ToInt32(TempData["duration"]);
                                            ViewBag.TaskDuration = 0;
                                        }
                                        else
                                        {
                                            ViewBag.TaskDuration = Convert.ToInt32(TempData["duration"]);
                                            ViewBag.TaskDurationInstall = 0;
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                                {


                                    if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                        ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment = Session["SDTHomeTaskNotes"].ToString().Replace("\\n", "\n");

                                }


                                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                                {
                                    ViewBag.chkfse1 = Session["SDTHomeIsRequiredfse"].ToString();
                                    if (Session["SDTHomeIsRequiredfse"].ToString() == "true")
                                    {

                                        ViewBag.ReqPrefFlag = "R";
                                    }
                                    else { ViewBag.ReqPrefFlag = "P"; }

                                }
                                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                                    ViewBag.Profile = Session["SDTHomeProfile"].ToString();

                                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                                    ViewModel.SiebelResponseAttributes.serviceRequest.gehcFse1 = Convert.ToString(Session["SDTHomeFseSkill"]);
                                //}
                                //else
                                //{

                                if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string) || (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string)))
                                {


                                    if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                                    {

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = Convert.ToDateTime(Session["SDTHomeEarlyStart"]);
                                        }
                                        else
                                        {
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (Convert.ToDateTime(DateTime.Parse(Session["SDTHomeEarlyStart"].ToString(), CultureInfo.GetCultureInfo("en-gb")).ToString()));
                                        }

                                    }
                                    else
                                    {
                                        ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (Convert.ToDateTime("1/1/0001 12:00:00 AM"));
                                        ViewBag.EstartEmptyvalue = "1/1/0001 12:00:00 AM";

                                    }
                                    if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                                    {

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = Convert.ToDateTime(Session["SDTHomeLateStart"]);
                                        }
                                        else
                                        {
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = (Convert.ToDateTime(DateTime.Parse(Session["SDTHomeLateStart"].ToString(), CultureInfo.GetCultureInfo("en-gb")).ToString()));
                                        }
                                    }
                                    else
                                    {
                                        ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = (Convert.ToDateTime("1/1/0001 12:00:00 AM"));
                                        ViewBag.LstartEmptyvalue = "1/1/0001 12:00:00 AM";
                                    }
                                    //}
                                    //}
                                    if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                    {
                                        Session["OldModifiedES"] = null;
                                        Session["OldModifiedES"] = ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart;

                                        Session["OldModifiedLS"] = null;
                                        Session["OldTaskgduration"] = null;
                                        Session["OldTaskgduration"] = ViewBag.TaskDurationInstall;

                                    }
                                    else
                                    {
                                        Session["OldModifiedES"] = null;
                                        Session["OldModifiedES"] = ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart.ToString("dd/MM/yyyy HH:mm");

                                        Session["OldModifiedLS"] = null;
                                        Session["OldModifiedLS"] = ViewModel.SiebelResponseAttributes.serviceRequest.lateStart.ToString("dd/MM/yyyy HH:mm");

                                        Session["OldTaskgduration"] = null;
                                        Session["OldTaskgduration"] = ViewBag.TaskDuration;
                                    }

                                    if (ViewModel.SiebelResponseAttributes.serviceRequest.gEMSEntitlementFlag == "Y"
                                           && ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Corrective Repair" && Session["taskStatus"] == "New")
                                    {
                                        ViewBag.lEarlyStart = "* From contract";
                                        ViewBag.lLateStart = "* From contract";
                                        ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                        ViewBag.CREntitlementFlag = "Y";
                                        ViewBag.CRcontractFlag = "No";

                                    }
                                    Session["gTaskNotes"] = null;
                                    Session["gTaskNotes"] = ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment;
                                    Session["Installation"] = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                    Session["gTaskNotes"] = Session["gTaskNotes"].ToString().Replace("\n", "\\n");

                                }
                                else
                                {
                                    //if (Session["SDTHomeEarlyStart"] != null)
                                    //    Session["SDTHomeEarlyStart"] = null;
                                    //if (Session["SDTHomeLateStart"] != null)
                                    //    Session["SDTHomeLateStart"] = null;
                                    //if (Session["SDTHomeDuration"] != null)
                                    //    Session["SDTHomeDuration"] = null;
                                    if (Session["taskStatus"] == "New")
                                    {
                                        //Early Start and Late Start logic based on srType
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "FMI")
                                        {
                                            //FMI ES & LS Logic Start

                                            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                            DateTime esFMI = (ViewModel.SiebelResponseAttributes.serviceRequest.created != "" ?
                                                Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.created) : objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress));

                                            DateTime esLFMI = (ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate != "" ?
                                                Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate).AddDays(-7) : objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress));

                                            DateTime esMFMI = (ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate != "" ?
                                               Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate).AddDays(7) : objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress));
                                            //FMI due date is more than 7 days away from today’s date.

                                            if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate))
                                            {

                                                if (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(7) < Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate))
                                                {
                                                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).AddMinutes(10);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate).AddDays(-7);
                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartFMI = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartFMI;


                                                }
                                                //FMI Due date is less than or equal to 7 days from today's date 
                                                else if (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(-7) >= Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate))
                                                {
                                                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).AddMinutes(10);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(+7);
                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartFMI = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartFMI;
                                                }
                                                else
                                                {
                                                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).AddMinutes(10);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddMinutes(30);

                                                }
                                            }
                                            else
                                            {
                                                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).AddMinutes(10);
                                                ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddMinutes(30);

                                            }
                                        }
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "PM")
                                        {
                                            //FMI ES & LS Logic Start for PM Job

                                            //If Desired Date exists & is beyond today's date

                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0 && !string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime))
                                            {
                                                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                // ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime = (objCountriesTimeZoneConversion.ConvertDesiredDate(Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime), objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).ToString();
                                                DateTime gEHCDesireDate = Convert.ToDateTime((objCountriesTimeZoneConversion.ConvertDesiredDate(Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime), objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).ToString());
                                                Session["gEHCDesireDate"] = gEHCDesireDate;
                                                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                if (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress) < gEHCDesireDate)
                                                {
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = gEHCDesireDate;
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = gEHCDesireDate.AddDays(15);
                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartPM = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartPM;
                                                }
                                                else
                                                {
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(15);

                                                }
                                            }
                                            else
                                            {
                                                DateTime esPM = (ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate != "" ?

                                                   Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate).AddDays(-15) : objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress));
                                                DateTime lsPM = (ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate != "" ?

                                                    Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate).AddDays(15) : objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress));
                                                //If no desired date exists & Schedule Date-15 is beyond today's date
                                                if (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress) < esPM)
                                                {

                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = esPM;
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lsPM;
                                                    string strEarlstr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart).ToShortDateString();
                                                    DateTime EarlStartPM = Convert.ToDateTime(strEarlstr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = EarlStartPM;
                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartPM = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartPM;

                                                }
                                                //If no desired date exists &Schedule Date+15 is today's date or a date in the past
                                                //If no desired date exists &Schedule Date+15 is less than 15 days from today's date 

                                                if (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress) >= lsPM || objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(-15) > lsPM)
                                                {
                                                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddMinutes(10);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(15);

                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartPM = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartPM;

                                                }
                                                //If no desired date exists &Schedule Date-15 is today's date or a date in the past 
                                                else if (objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress) >= esPM)
                                                {
                                                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddMinutes(10);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lsPM;
                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartPM = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartPM;

                                                }


                                            }

                                        }

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddMinutes(10);
                                            var newDate = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);
                                            ViewBag.getCurrentDate = Convert.ToDateTime(newDate).ToString("dd/MM/yyyy HH:mm");

                                        }

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.gEMSEntitlementFlag == "Y"
                                            && ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Corrective Repair")
                                        {
                                            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(1).AddHours(0.1);
                                            ViewBag.lEarlyStart = "* From contract";
                                            ViewBag.lLateStart = "* From contract";
                                            ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                            ViewBag.CREntitlementFlag = "Y";
                                            ViewBag.CRcontractFlag = "Yes";
                                        }

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.gEMSEntitlementFlag == "N"
                                          && ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Corrective Repair")
                                        {
                                            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                            string strLatestr = Convert.ToDateTime(objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(5)).ToShortDateString();
                                            DateTime lateStartCR = Convert.ToDateTime(strLatestr).AddHours(9);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddMinutes(10);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartCR;
                                        }
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType != "Corrective Repair" && ViewModel.SiebelResponseAttributes.serviceRequest.srType != "Installation" && ViewModel.SiebelResponseAttributes.serviceRequest.srType != "PM" && ViewModel.SiebelResponseAttributes.serviceRequest.srType != "FMI")
                                        {
                                            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddMinutes(10);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress).AddDays(7);
                                        }


                                    }
                                    else
                                    {
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                            //Session["ModifiedDuration"] = "4days(32 Hours)";// 1920;
                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                                ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment = Convert.ToString(Session["TaskNotes"]);

                                            ViewModel.SiebelResponseAttributes.serviceRequest.postcode = Convert.ToString(Session["PostCode"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.address = Convert.ToString(Session["Address"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.city = Convert.ToString(Session["City"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.countryID = Convert.ToString(Session["CountryID"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = Convert.ToDateTime(Session["ModifiedES"]);
                                            //ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart=objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);

                                            ViewBag.TaskDurationInstall = Convert.ToInt32(Session["ModifiedDuration"]);
                                            ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                            var newDate = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);
                                            ViewBag.getCurrentDate = Convert.ToDateTime(newDate).ToString("dd/MM/yyyy HH:mm");

                                            logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " Task Duartion: " + Session["ModifiedDuration"]);
                                            logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " View Bag Task Duartion: " + ViewBag.TaskDurationInstall + "Req Duration:" + Session["SDTHomeDuration"]);

                                        }
                                        else
                                        {
                                            var currentDateTime = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);
                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "PM" || ViewModel.SiebelResponseAttributes.serviceRequest.srType == "FMI")
                                            {
                                                if (Convert.ToDateTime(Session["ModifiedES"]) < currentDateTime)
                                                {
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = currentDateTime.AddMinutes(10);


                                                }
                                                else
                                                {
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = Convert.ToDateTime(Session["ModifiedES"]);

                                                }
                                                if (Convert.ToDateTime(Session["ModifiedLS"]) < currentDateTime)
                                                {
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = currentDateTime.AddDays(15);

                                                }
                                                else
                                                {
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = Convert.ToDateTime(Session["ModifiedLS"]);

                                                }
                                            }
                                            else
                                            {
                                                ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = Convert.ToDateTime(Session["ModifiedES"]);

                                                ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = Convert.ToDateTime(Session["ModifiedLS"]);

                                            }

                                            ViewBag.TaskDuration = Convert.ToInt32(Session["ModifiedDuration"]);
                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                                ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment = Convert.ToString(Session["TaskNotes"]);
                                            logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " Task Duartion: " + Session["ModifiedDuration"]);
                                            logger.Debug("HomeController;Index; Session logger:" + SiebelHttpPostParams.ActivityNo + " View Bag Task Duartion: " + ViewBag.TaskDuration + "Req Duration:" + Session["SDTHomeDuration"]);



                                        }
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            Session["OldModifiedES"] = null;
                                            Session["OldModifiedES"] = ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart;

                                            Session["OldModifiedLS"] = null;
                                            Session["OldTaskgduration"] = null;
                                            Session["OldTaskgduration"] = ViewBag.TaskDurationInstall;

                                        }
                                        else
                                        {
                                            Session["OldModifiedES"] = null;
                                            Session["OldModifiedES"] = ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart.ToString("dd/MM/yyyy HH:mm");

                                            Session["OldModifiedLS"] = null;
                                            Session["OldModifiedLS"] = ViewModel.SiebelResponseAttributes.serviceRequest.lateStart.ToString("dd/MM/yyyy HH:mm");

                                            Session["OldTaskgduration"] = null;
                                            Session["OldTaskgduration"] = ViewBag.TaskDuration;
                                        }


                                        Session["gTaskNotes"] = null;
                                        Session["gTaskNotes"] = ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment;
                                        Session["Installation"] = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                        Session["gTaskNotes"] = Session["gTaskNotes"].ToString().Replace("\n", "\\n");

                                    }
                                }

                                //------------
                                if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate))
                                {

                                    string strDate = Convert.ToDateTime(DateTime.Parse(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate)).ToShortTimeString();
                                    DateTime dt = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate);
                                    if (string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string) && Session["taskStatus"] == "New")
                                    {

                                        if (strDate.Contains("12:00 AM"))
                                        {
                                            string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                            DateTime addLateStartHr = Convert.ToDateTime(strLatestr).AddHours(9);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = addLateStartHr;
                                        }
                                    }
                                    ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate = dt.ToString("dd/MM/yyyy");

                                }
                                if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate))
                                {

                                    DateTime dt = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate);
                                    ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate = dt.ToString("dd/MM/yyyy");

                                }

                                if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0 && !string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime))
                                {
                                    //changes For Desired Date 
                                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                    ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime = (objCountriesTimeZoneConversion.ConvertDesiredDate(Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime), objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).ToString();
                                    DateTime gEHCDesireDate = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime);

                                    DateTime dt = Convert.ToDateTime(Session["gEHCDesireDate"]);
                                    ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime = gEHCDesireDate.ToString("dd/MM/yyyy HH:mm");

                                    // append 09:00


                                    string defaultDesiredDateforPM = ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime;
                                    if (defaultDesiredDateforPM.Contains("00:00"))//00:00
                                    {
                                        // show time as  09:00 
                                        if (string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string) && Session["taskStatus"] == "New")
                                        {
                                            string strLateSrtYear = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();

                                            DateTime dt1 = Convert.ToDateTime(strLateSrtYear).AddHours(9);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = dt1;//ToString("dd/MM/yyyy HH:mm");
                                        }
                                    }
                                }

                                logger.Debug("HomeController;Index; End");

                                // Making session values to null 
                                if (Session["SelectedFSE"] != null)
                                    Session["SelectedFSE"] = null;
                                if (Session["ModifiedES"] != null)
                                    Session["ModifiedES"] = null;
                                if (Session["ModifiedLS"] != null)
                                    Session["ModifiedLS"] = null;
                                if (!string.IsNullOrEmpty(Session["ModifiedDuration"] as string))
                                    Session["ModifiedDuration"] = null;

                                if (Session["ModifiedDuration"] != null)
                                    Session["ModifiedDuration"] = null;
                                if (!string.IsNullOrEmpty(Session["TaskNotes"] as string))
                                    Session["TaskNotes"] = null;


                                // which were created to maintain values when navigated between pages without saving 

                                //if (!string.IsNullOrEmpty(Session["PartToolTempAddr"] as string))
                                //    Session["PartToolTempAddr"] = null;
                                if (Session["PartToolTempAddr"] != null)
                                    Session["PartToolTempAddr"] = null;
                                if (Session["PartToolReturn"] != null)
                                    Session["PartToolReturn"] = null;
                                //to maintain the same values when navigated between pages.
                                if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                                    Session["SDTHomeEarlyStart"] = null;
                                if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                                    Session["SDTHomeLateStart"] = null;
                                if (Session["SDTHomeEarlyStart"] != null)
                                    Session["SDTHomeEarlyStart"] = null;
                                if (Session["SDTHomeLateStart"] != null)
                                    Session["SDTHomeLateStart"] = null;
                                if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                                    Session["SDTHomeDuration"] = null;
                                if (Session["SDTHomeDuration"] != null)
                                    Session["SDTHomeDuration"] = null;
                                //if (!string.IsNullOrEmpty(Session["SDTHomeSSOFse1"] as string))
                                //    Session["SDTHomeSSOFse1"] = null;
                                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                                    Session["SDTHomeTaskNotes"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                                    Session["SDTHomeIsRequiredfse"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                                    Session["SDTHomeProfile"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                                    Session["SDTHomeFseSkill"] = null;
                                DateTime endTime = DateTime.Now;

                                TimeSpan span = endTime.Subtract(seiblestartTime);
                                ViewBag.siebleReceiveTime = span.Minutes * 60 + span.Seconds;
                                return View(ViewModel);
                            }
                            else
                            {
                                if (Session["SelectedFSE"] != null)
                                    Session["SelectedFSE"] = null;
                                if (Session["ModifiedES"] != null)
                                    Session["ModifiedES"] = null;
                                if (Session["ModifiedLS"] != null)
                                    Session["ModifiedLS"] = null;
                                if (!string.IsNullOrEmpty(Session["ModifiedDuration"] as string))
                                    Session["ModifiedDuration"] = null;

                                if (Session["ModifiedDuration"] != null)
                                    Session["ModifiedDuration"] = null;
                                if (!string.IsNullOrEmpty(Session["TaskNotes"] as string))
                                    Session["TaskNotes"] = null;


                                // which were created to maintain values when navigated between pages without saving 

                                //if (!string.IsNullOrEmpty(Session["PartToolTempAddr"] as string))
                                //    Session["PartToolTempAddr"] = null;
                                if (Session["PartToolTempAddr"] != null)
                                    Session["PartToolTempAddr"] = null;
                                if (Session["PartToolReturn"] != null)
                                    Session["PartToolReturn"] = null;

                                //to maintain the same values when navigated between pages.
                                if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                                    Session["SDTHomeEarlyStart"] = null;
                                if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                                    Session["SDTHomeLateStart"] = null;
                                if (Session["SDTHomeEarlyStart"] != null)
                                    Session["SDTHomeEarlyStart"] = null;
                                if (Session["SDTHomeLateStart"] != null)
                                    Session["SDTHomeLateStart"] = null;
                                if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                                    Session["SDTHomeDuration"] = null;
                                if (Session["SDTHomeDuration"] != null)
                                    Session["SDTHomeDuration"] = null;
                                //if (!string.IsNullOrEmpty(Session["SDTHomeSSOFse1"] as string))
                                //    Session["SDTHomeSSOFse1"] = null;
                                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                                    Session["SDTHomeTaskNotes"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                                    Session["SDTHomeIsRequiredfse"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                                    Session["SDTHomeProfile"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                                    Session["SDTHomeFseSkill"] = null;
                                logger.Error("HomeController;Index; No Record available at Siebel");
                                TempData["ErrorCode"] = "204";
                                TempData["Errormsg"] = "Activity Details couldn't  be found, Please try with a valid activity";
                                return RedirectToAction("Errormsg", "ErrorPage");
                            }

                        }
                        logger.Debug("HomeController;Index; Siebel Response stream Null");
                    }
                    logger.Debug("HomeController;Index; Siebel Response Null");
                    // Making session values to null 

                }
                // Making session values to null 
                if (Session["SelectedFSE"] != null)
                    Session["SelectedFSE"] = null;
                if (Session["ModifiedES"] != null)
                    Session["ModifiedES"] = null;
                if (Session["ModifiedLS"] != null)
                    Session["ModifiedLS"] = null;
                if (!string.IsNullOrEmpty(Session["ModifiedDuration"] as string))
                    Session["ModifiedDuration"] = null;

                if (Session["ModifiedDuration"] != null)
                    Session["ModifiedDuration"] = null;
                if (!string.IsNullOrEmpty(Session["TaskNotes"] as string))
                    Session["TaskNotes"] = null;
                // which were created to maintain values when navigated between pages without saving 

                //if (!string.IsNullOrEmpty(Session["PartToolTempAddr"] as string))
                //    Session["PartToolTempAddr"] = null;
                if (Session["PartToolTempAddr"] != null)
                    Session["PartToolTempAddr"] = null;
                if (Session["PartToolReturn"] != null)
                    Session["PartToolReturn"] = null;

                //to maintain the same values when navigated between pages.
                if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                    Session["SDTHomeEarlyStart"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                    Session["SDTHomeLateStart"] = null;
                if (Session["SDTHomeEarlyStart"] != null)
                    Session["SDTHomeEarlyStart"] = null;
                if (Session["SDTHomeLateStart"] != null)
                    Session["SDTHomeLateStart"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                    Session["SDTHomeDuration"] = null;
                if (Session["SDTHomeDuration"] != null)
                    Session["SDTHomeDuration"] = null;
                //if (!string.IsNullOrEmpty(Session["SDTHomeSSOFse1"] as string))
                //    Session["SDTHomeSSOFse1"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                    Session["SDTHomeTaskNotes"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                    Session["SDTHomeIsRequiredfse"] = null;

                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                    Session["SDTHomeProfile"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                    Session["SDTHomeFseSkill"] = null;
            }
            catch (Exception ex)
            {

                // Making session values to null 
                if (Session["SelectedFSE"] != null)
                    Session["SelectedFSE"] = null;
                if (Session["ModifiedES"] != null)
                    Session["ModifiedES"] = null;
                if (Session["ModifiedLS"] != null)
                    Session["ModifiedLS"] = null;
                if (!string.IsNullOrEmpty(Session["ModifiedDuration"] as string))
                    Session["ModifiedDuration"] = null;

                if (Session["ModifiedDuration"] != null)
                    Session["ModifiedDuration"] = null;
                if (!string.IsNullOrEmpty(Session["TaskNotes"] as string))
                    Session["TaskNotes"] = null;
                // which were created to maintain values when navigated between pages without saving 

                //if (!string.IsNullOrEmpty(Session["PartToolTempAddr"] as string))
                //    Session["PartToolTempAddr"] = null;
                if (Session["PartToolTempAddr"] != null)
                    Session["PartToolTempAddr"] = null;
                if (Session["PartToolReturn"] != null)
                    Session["PartToolReturn"] = null;

                //to maintain the same values when navigated between pages.
                if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                    Session["SDTHomeEarlyStart"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                    Session["SDTHomeLateStart"] = null;
                if (Session["SDTHomeEarlyStart"] != null)
                    Session["SDTHomeEarlyStart"] = null;
                if (Session["SDTHomeLateStart"] != null)
                    Session["SDTHomeLateStart"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                    Session["SDTHomeDuration"] = null;
                if (Session["SDTHomeDuration"] != null)
                    Session["SDTHomeDuration"] = null;
                //if (!string.IsNullOrEmpty(Session["SDTHomeSSOFse1"] as string))
                //    Session["SDTHomeSSOFse1"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                    Session["SDTHomeTaskNotes"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                    Session["SDTHomeIsRequiredfse"] = null;

                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                    Session["SDTHomeProfile"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                    Session["SDTHomeFseSkill"] = null;
                //Logger.Error("Exception thrown at Home Controller Index Action", ex);
                logger.Error("HomeController;Index; Exception from Siebel call:" + ex.Message);
                TempData["ErrorCode"] = "";

                //TempData["Errormsg"] = "Unable to connect to Siebel";

                // Error Message is taken from US06_AC06.
                // TempData["Errormsg"] = "SDT Booking cannot be launched due to technical issues. Please contact your administrator.";

                //Added By phani kanth
                TempData["Errormsg"] = ex.Message;

                return RedirectToAction("Errormsg", "ErrorPage");

                //return View();
            }
            #endregion
        }

        [HttpPost]
        public async Task<JsonResult> GetSiteSytemClickTaskCount()
        {
            DateTime sstartTime = DateTime.Now;
            CallClickSerrvice objClickCallService = new CallClickSerrvice();
            DependencyTasksList lstCounts = new DependencyTasksList();
            try
            {
                HTTPPostParams SiebelHttpPostParams = new HTTPPostParams();
                SiebelHttpPostParams = (HTTPPostParams)Session["SiebelHttpPostParams"];
                ScheduleServiceDev1.GetTasksResponse objtaskResponse = new ScheduleServiceDev1.GetTasksResponse();
                string callID = string.Empty;
                // objtaskResponse = (ScheduleServiceDev1.GetTasksResponse)Session["TaskResponseByTask"];
                var resTasksResponseByTask = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.ActivityNo, "TaskID");

                //below condition added by phani kanth p.

                resTasksResponseByTask.Tasks = resTasksResponseByTask.Tasks.Where(x => x.TaskType.DisplayString != "Parts Pickup").ToArray();

                objtaskResponse = (ScheduleServiceDev1.GetTasksResponse)resTasksResponseByTask;

                if (objtaskResponse != null && objtaskResponse.Tasks.Count() > 0)
                {
                    callID = objtaskResponse.Tasks[0].CallID;
                }

                var resTasksResponseBySite = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.ShipToSite, "TaskSiteID");
                var resTasksResponseBySystem = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.SystemID, "TaskSystemID");

                //Potential Dependencies

                var SiteCountPotential = resTasksResponseBySite.Tasks.Where(x => (x.Status.DisplayString == "New" || x.Status.DisplayString == "Tentative") && x.TaskID != SiebelHttpPostParams.ActivityNo && x.IsMST == false && x.TaskType.DisplayString != "Parts Pickup").ToList();
                lstCounts.SiteCountPotential = SiteCountPotential.Count();

                var SystemCountPotential = resTasksResponseBySystem.Tasks.Where(x => (x.Status.DisplayString == "New" || x.Status.DisplayString == "Tentative") && x.TaskID != SiebelHttpPostParams.ActivityNo && x.IsMST == false && x.TaskType.DisplayString != "Parts Pickup").ToList();
                lstCounts.SystemCountPotential = SystemCountPotential.Count();

                //Actual Dependencies
                lstCounts.SiteCountActual = resTasksResponseBySite.Tasks.Where(x => x.IsMST == true && x.CallID == callID && x.TaskID != SiebelHttpPostParams.ActivityNo && x.TaskType.DisplayString != "Parts Pickup").Count();
                ViewBag.SiteCountActual = lstCounts.SiteCountActual;//ignore
                Session["SiteCountActual"] = lstCounts.SiteCountActual;

                lstCounts.SystemCountActual = resTasksResponseBySystem.Tasks.Where(x => x.IsMST == true && x.CallID == callID && x.TaskID != SiebelHttpPostParams.ActivityNo && x.TaskType.DisplayString != "Parts Pickup").Count();
                ViewBag.SystemCountActual = lstCounts.SystemCountActual;//ignore
                Session["SystemCountActual"] = lstCounts.SystemCountActual;

                DateTime endTime = DateTime.Now;
                TimeSpan span = endTime.Subtract(sstartTime);
                lstCounts.receiveTime = span.Minutes * 60 + span.Seconds + Convert.ToInt32(Session["FSEreceiveTime"]);
                return Json(lstCounts, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error("HomeController;GetSiteSytemClickTaskCount; Exception occured while fetching site and system count from Click Service call:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetFSE1ddlItems()
        {
            DateTime fseStartTime = DateTime.Now;

            try
            {
                SSOLogic objsso = new SSOLogic();
                List<SelectListItem> EngineerList = new List<SelectListItem>();

                var result = await objsso.GetSSODetails(((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);


                // FOR SIEBEL ID START             
                if (result != null)
                {
                    if (result.Count > 0)
                        EngineerList = result.ToList<SelectListItem>();
                }

                //SelectListItem Eng1 = new SelectListItem();

                //if (!string.IsNullOrEmpty(Session["objPreferredSFESiebel"] as string))
                //{
                //    Eng1.Value = Session["objPreferredSFESiebel"].ToString();
                //    Eng1.Text = ((NewSDTApplication.Models.SiebelJsonToEntity)(Session["SiebelData"])).serviceRequest.ownerFirstName + " " + ((NewSDTApplication.Models.SiebelJsonToEntity)(Session["SiebelData"])).serviceRequest.ownerLastName;
                //    EngineerList.Add(Eng1);
                //}

                // FOR SIEBEL END
                if (!string.IsNullOrEmpty(Session["PreferredFSEs"] as string))
                {
                    List<SelectListItem> EngineerList2 = new List<SelectListItem>();

                    var searchssoidTextbox = await objsso.GetValidSearchSSOID(Session["PreferredFSEs"].ToString(), ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);
                    EngineerList2 = searchssoidTextbox.ToList<SelectListItem>();
                    EngineerList.AddRange(EngineerList2);
                }

                //Required FSE added by Phani Kanth
                if (!string.IsNullOrEmpty(Session["RreferredFSEs"] as string))
                {
                    List<SelectListItem> RreferredFSEsEngineerList = new List<SelectListItem>();

                    var searchssoidTextbox = await objsso.GetValidSearchSSOID(Session["RreferredFSEs"].ToString(), ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);
                    RreferredFSEsEngineerList = searchssoidTextbox.ToList<SelectListItem>();
                    EngineerList.AddRange(RreferredFSEsEngineerList);
                }


                // Code added to maintain dropdown values when navaigated between pages without saving.
                if (!string.IsNullOrEmpty(Session["SDTHomeSSOFse1"] as string))
                {
                    string value = Session["SDTHomeSSOFse1"].ToString();

                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] values = value.Split(',');

                        for (int i = 0; i < values.Length; i++)
                        {
                            for (int j = 0; j < EngineerList.Count; j++)
                            {
                                if (values[i] != "")
                                {
                                    if (values[i] == EngineerList[j].Value)
                                    {
                                        EngineerList[j].Selected = true;
                                    }
                                }
                            }
                        }
                    }
                }

                else
                {
                    if (!string.IsNullOrEmpty(Session["PreferredFSEs"] as string))
                    {
                        string value = Session["PreferredFSEs"].ToString();

                        if (!string.IsNullOrEmpty(value))
                        {
                            string[] values = value.Split(',');

                            for (int i = 0; i < values.Length; i++)
                            {
                                for (int j = 0; j < EngineerList.Count; j++)
                                {
                                    if (values[i] != "")
                                    {
                                        if (values[i] == EngineerList[j].Value)
                                        {
                                            EngineerList[j].Selected = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Session["RreferredFSEs"] as string))
                    {
                        string value = Session["RreferredFSEs"].ToString();

                        if (!string.IsNullOrEmpty(value))
                        {
                            string[] values = value.Split(',');

                            for (int i = 0; i < values.Length; i++)
                            {
                                for (int j = 0; j < EngineerList.Count; j++)
                                {
                                    if (values[i] != "")
                                    {
                                        if (values[i] == EngineerList[j].Value)
                                        {
                                            EngineerList[j].Selected = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Session["SDTHomeSSOFse1"] as string))
                {
                    Session["SDTHomeSSOFse1"] = null;
                }
                if (!string.IsNullOrEmpty(Session["PreferredFSEs"] as string))

                    Session["PreferredFSEs"] = null;

                if (!string.IsNullOrEmpty(Session["RreferredFSEs"] as string))
                    Session["RreferredFSEs"] = null;

                if (Session["FSEreceiveTime"] != null)
                {
                    Session["FSEreceiveTime"] = 0;
                }
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(fseStartTime);
                Session["FSEreceiveTime"] = span.Minutes * 60 + span.Seconds + Convert.ToInt32(Session["clickReceiveTime"]);

                return Json(EngineerList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("HomeController;GetFSE1ddlItems; Exception occured while fetching SSO details from ClickSoftware Service call:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetValidFSESSOID(string SSOID)
        {

            try
            {
                SSOLogic objsso = new SSOLogic();
                var result = objsso.GetValidFSESSOID(SSOID, ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("HomeController;GetFSE1ddlItems; Exception occured while fetching SSO details from ClickSoftware Service call:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
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
