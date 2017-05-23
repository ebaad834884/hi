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
        //Code changes by Ebaad on 22/3/2017 for the US134
        DateTime tzConversion;
        //Addition of IP address in logs - US185 - 18/4/2017 by Ebaad (This line will provide us with Client IP Address)
        string IP = "IP: " + (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
        public async Task<ActionResult> Index()
        {
            CountriesTimeZoneConversion objCountriesTimeZoneConversion;
            logger.Debug("-----------------------------------------------------------------");
            logger.Debug("In HomeController Index method | Start" + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            HTTPPostParams SiebelHttpPostParams = new HTTPPostParams();
            var completeURL = ConfigurationManager.AppSettings["SiebelURL"].ToString();
            DateTime startTime = DateTime.Now;

            logger.Debug("In Home controller Index Method | Request form Activity ID: " + Request.Form["ACTIVITY_ID"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            #region Set Values from HTTP Post
            if (Session["SiebelHttpPostParams"] != null && string.IsNullOrEmpty(Request.Form["SERVICE_REQUEST_NUMBER"]))
            {
                SiebelHttpPostParams = (HTTPPostParams)Session["SiebelHttpPostParams"];
            }
            else
            {
                Session.Clear();
                if (!string.IsNullOrEmpty(Request.Form["ACTIVITY_ID"]))
                {
                    logger.Debug("In Home controller Index Method | Request form (in 1st IF condition) System ID: " + Request.Form["SYSTEM_ID"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                    if (!string.IsNullOrEmpty(Request.Form["SYSTEM_ID"]))
                    {
                        logger.Debug("In Home controller Index Method | Request form (in 2nd IF condition) System ID: " + Request.Form["SYSTEM_ID"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                        SiebelHttpPostParams.SystemID = Request.Form["SYSTEM_ID"];
                    }
                    else
                    {
                        logger.Debug("In Home controller Index Method | Request form (in else condition) System ID: " + Request.Form["SYSTEM_ID"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

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
                    logger.Debug("In Home controller Index Method | When Activity ID is null : " + Request.Form["ACTIVITY_ID"] + " Service Request Number : " + Request.Form["SERVICE_REQUEST_NUMBER"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                    ///////////Session["SiebelHttpPostParams"] = SiebelHttpPostParams;  //This line is not in use at all. Need to remove this in future                  
                    return RedirectToAction("NotAuthorizedPage", "ErrorPage");
                }

                //SiebelHttpPostParams.ActivityNo = string.IsNullOrEmpty(SiebelHttpPostParams.ActivityNo) ? "1-1GHBFA8" : SiebelHttpPostParams.ActivityNo;
                //SiebelHttpPostParams.ServiceRequestNumber = string.IsNullOrEmpty(SiebelHttpPostParams.ServiceRequestNumber) ? "1-3173858010" : SiebelHttpPostParams.ServiceRequestNumber;
                //SiebelHttpPostParams.ShipToSite = string.IsNullOrEmpty(SiebelHttpPostParams.ShipToSite) ? "852126" : SiebelHttpPostParams.ShipToSite;
                //SiebelHttpPostParams.SystemID = string.IsNullOrEmpty(SiebelHttpPostParams.SystemID) ? "0850240752" : SiebelHttpPostParams.SystemID;


                
                Session["SiebelHttpPostParams"] = SiebelHttpPostParams;
                Session["SiebelHttpPostParamsActivityNo"] = SiebelHttpPostParams.ActivityNo;
                logger.Debug("In HomeController Index method | (For hardcoded data) ActivityNo: " + SiebelHttpPostParams.ActivityNo + " SerialNumber: " + SiebelHttpPostParams.SerialNumber + " ServiceRequestID: " + SiebelHttpPostParams.ServiceRequestID + " ServiceRequestNumber: " + SiebelHttpPostParams.ServiceRequestNumber + " ShipToSite: " + SiebelHttpPostParams.ShipToSite + " SystemID: " + SiebelHttpPostParams.SystemID + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
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


                DateTime startTimeTx1 = DateTime.Now;
                var ClickResponseSystemId = objClickCallService.GetDateDetails(SiebelHttpPostParams.SystemID);
                DateTime endTimeTx1 = DateTime.Now;
                TimeSpan Tx1 = endTimeTx1.Subtract(startTimeTx1);
                logger.Debug("In Home controller Index Method | Call to ClickCallService.cs (objClickCallService.GetDateDetails) ~ " + IP + " ~ Request SystemID: " + Convert.ToString(SiebelHttpPostParams.SystemID) + " ~ Response: " + Convert.ToString(ClickResponseSystemId) + " ~ Call duration: " + Tx1.TotalMilliseconds);
                if (ClickResponseSystemId != null)
                {
                    if (string.IsNullOrEmpty(ClickResponseSystemId.ID))
                    {
                        ViewBag.ValidateSystemId = "SystemIDNotExistInClick";
                    }
                }
                else
                {
                    ViewBag.ValidateSystemId = "SystemIDNotExistInSiebel";
                }

                DateTime startTimeTx2 = DateTime.Now;
                var ClickResponse = objClickCallService.GetSiteDetails(SiebelHttpPostParams.ShipToSite);
                DateTime endTimeTx2 = DateTime.Now;
                TimeSpan Tx2 = endTimeTx2.Subtract(startTimeTx2);

                if (ClickResponse != null)
                {
                   // logger.Debug("In Home controller Index Method | Call to ClickCallService.cs (objClickCallService.GetSiteDetails) ~ " + IP + " ~  Request ShipToSite ID: " + SiebelHttpPostParams.ShipToSite + " ~ Response: " + ClickResponse.Street + " , " + ClickResponse.City + " , " + ClickResponse.Postcode + " , " + ClickResponse.CountryID.Name + " , " + ClickResponse.Latitude + " , " + ClickResponse.Longitude + " ~ Call duration: " + Tx2.TotalMilliseconds);
                    if (!string.IsNullOrEmpty(ClickResponse.Street))
                    {
                        ViewBag.street = ClickResponse.Street.Replace(",", " ") == null ? "" : ClickResponse.Street.Replace(",", " ");
                    }
                    else
                    {

                        ViewBag.ValidateShipToSite = "SiteIDNotExistInClick";
                    }
                }
                else
                {
                    ViewBag.ValidateShipToSite = "SiteIDNotExistInSiebel";
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

                DateTime startTimeTx3 = DateTime.Now;
                var resTasksResponseByTask = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.ActivityNo, "TaskID");
                DateTime endTimeTx3 = DateTime.Now;
                TimeSpan Tx3 = endTimeTx3.Subtract(startTimeTx3);
                logger.Debug("In Home controller Index Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) ~ " + IP + " ~  Request: Activity ID: " + SiebelHttpPostParams.ActivityNo + " ~ Response : Tasks List" + " ~ Call duration: " + Tx3.TotalMilliseconds);

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
                    if (AddressArray != null)
                    {
                        if (AddressArray[0].Count > 0)
                        {


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
                    }
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
                            //Jayesh Soni - US82 - 13/04/2017 - SR Description from CLick Response
                            //srDescription = i.SRDescription
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
                            //Jayesh Soni - US82 - 13/04/2017 - SR Description from CLick Response
                            //srDescription = i.SRDescription
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
                    //Jayesh Soni - US82 - 13/04/2017 - SR Description assigned to objClickTaskResponse                                                                   
                    //if(resTasksResponseByTask.Tasks.Count>1)
                    //objClickTasksResponse.SRDescription = resTasksResponseByTask.Tasks[0].SRDescription;
                    //Ends SR Description
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
                    //Jayesh Soni - US82 - 18/04/2017 - SR Description -Begin
                    Session["SRDesc"] = resTasksResponseByTask.Tasks[0].SRDescription == null ? string.Empty : resTasksResponseByTask.Tasks[0].SRDescription;
                    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                    Session["PostCode"] = resTasksResponseByTask.Tasks[0].Postcode;
                    Session["Address"] = resTasksResponseByTask.Tasks[0].Street;
                    Session["City"] = resTasksResponseByTask.Tasks[0].City;
                    Session["CountryID"] = resTasksResponseByTask.Tasks[0].CountryID.DisplayString;
                    ViewBag.calltestiddata = resTasksResponseByTask.Tasks[0].CallID.ToString();
                    ViewBag.checktaskids = resTasksResponseByTask.Tasks[0].TaskID == null ? "" : resTasksResponseByTask.Tasks[0].TaskID.ToString();
                    Session["checktaskids"] = ViewBag.checktaskids;
                    ViewBag.checkStatusVal = resTasksResponseByTask.Tasks[0].Status.Text[0].ToString();
                    logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " Task Duration: " + Session["ModifiedDuration"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                    logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " Task Duration: " + resTasksResponseByTask.Tasks[0].Duration + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");


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

                logger.Error("In HomeController Index method | (Code: 500) Exception from Click call: " + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                TempData["ErrorCode"] = "500";

                //Added By phani kanth
                //US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message added when any of the click calls fail 
                TempData["Errormsg"] = "Unable to process the request due to system error. Please reach out to Application helpdesk for support. Techincal Message: " + ex.Message;

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
                    logger.Error("In HomeController Index method | (Oauth Cookie) Exception:" + ex.Message + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

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
                            # region deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This code had three if conditions but it was only setting the 'IsBump' session variable to true)
                            //if (objSiebelResponse != null && objSiebelResponse.serviceRequest != null)
                            //{
                            //    //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                            //    if (objSiebelResponse.serviceRequest.activityDetailList.Count() > 0 && !string.IsNullOrEmpty(objSiebelResponse.serviceRequest.currentEquiptmentStatus))
                            //    {
                            //        if ((objSiebelResponse.serviceRequest.currentEquiptmentStatus.Contains('1') || objSiebelResponse.serviceRequest.currentEquiptmentStatus.Contains('2') || objSiebelResponse.serviceRequest.gEHCSafetyConcern.ToLower() == "yes" || objSiebelResponse.serviceRequest.gEHCSafetyConcern.ToLower() == "actual" || objSiebelResponse.serviceRequest.gEHCSafetyConcern.ToLower() == "potential") && objSiebelResponse.serviceRequest.srType == "Corrective Repair")
                            //        {
                            //            Session["IsBump"] = true;
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    logger.Error("HomeController;Index; No Record available at Siebel");
                            //    TempData["ErrorCode"] = "204";
                            //    TempData["Errormsg"] = "Activity Details couldn't  be found, Please try with a valid activity";
                            //    return RedirectToAction("Errormsg", "ErrorPage");
                            //}
                            # endregion
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
                                //Code changes by Ebaad on 22/3/2017 for US134
                                DateTime startTimeTZ = DateTime.Now;
                                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                                tzConversion = objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);
                                DateTime endTimeTZ = DateTime.Now;
                                TimeSpan TZ = endTimeTZ.Subtract(startTimeTZ);
                                logger.Debug("In Home controller Index Method | Call to ClickCallService.cs (objCountriesTimeZoneConversion.GetTimeZone) ~ " + IP + " ~ Request: Country code and Ship to site address respectively: " + objSiebelResponse.serviceRequest.countryCode + " " + objSiebelResponse.serviceRequest.shipToAddress + " ~ Response: " + tzConversion + " ~ Call duration: " + TZ.TotalMilliseconds);

                                //Code changes by Ebaad on 22/3/2017 for US134
                                var newDDate = tzConversion;
                                ViewBag.DefaultDate = Convert.ToDateTime(newDDate).ToString("dd/MM/yyyy HH:mm");

                                if (objSiebelResponse.serviceRequest == null)
                                    logger.Debug("In HomeController Index Method | Siebel Response is NULL" + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                                else
                                    logger.Debug("In HomeController Index Method | Siebel Response is Arriving" + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                                if (Session["SiebelData"] != null)
                                {
                                    Session["SiebelData"] = null;
                                }
                                Session["SiebelData"] = objSiebelResponse;
                                logger.Debug("In HomeController Index method | Session logger:" + objSiebelResponse.serviceRequest.activityDetailList[0].activityUID + " Task Duration: " + Session["ModifiedDuration"] + " Request Duration: " + Session["SDTHomeDuration"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                                if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.srType))
                                    ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;

                                //Jayesh Soni - US82 - 13/04/2017
                                //string desc = Convert.ToString(ViewModel.SiebelResponseAttributes.serviceRequest.description);
                                //Jayesh Soni - US82 - 13/04/2017 - Condition Check for SR desc from Siebel and Click

                                //var originalSRDesc = Convert.ToString(ViewModel.SiebelResponseAttributes.serviceRequest.description);
                                //desc = desc.Replace("<", string.Empty);
                               // desc = desc.Replace(">", string.Empty);
                               // ViewBag.CompleteSRDesc = originalSRDesc;
                                //if (objClickTasksResponse.TaskExists == false)
                                //{
                                 //   if (desc.Length > 256)
                                 //   {
                                 //       ViewBag.isValid = "true";
                                //        desc = desc.Substring(0, 256);
                                 //   }
                                //    ViewBag.SRDescription = desc;
                               // }
                               // else
                               // {
                                //    ViewBag.SRDescription = Convert.ToString(ViewModel.ClickTaskResponseAttributes.SRDescription);
                               // }
                                //ends for SR Description
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
                                    logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " View Bag Task Duration: " + ViewBag.TaskDuration + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");


                                }
                                else
                                {
                                    ViewBag.TaskDurationInstall = 0;
                                    ViewBag.TaskDuration = 0;

                                    logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " View Bag Task Duration: " + ViewBag.TaskDuration + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

                                }
                                if (Session["SelectedFSE"] != null)
                                {
                                    // if (!string.IsNullOrEmpty(Session["SelectedFSE"] as string))
                                    ViewModel.SiebelResponseAttributes.serviceRequest.gehcFse1 = Convert.ToString(Session["SelectedFSE"]);
                                }



                                if (Session["SDTHomeDuration"] != null)
                                {
                                    if (Session["SDTHomeDuration"] != "")
                                    {


                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            ViewBag.TaskDurationInstall = Convert.ToInt32(Session["SDTHomeDuration"]);
                                            ViewBag.TaskDuration = 0;
                                        }
                                        else
                                        {
                                            ViewBag.TaskDuration = Convert.ToInt32(Session["SDTHomeDuration"]);
                                            ViewBag.TaskDurationInstall = 0;
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(Session["SDTHomeTaskNotes"] as string))
                                {


                                    if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                        ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment = Session["SDTHomeTaskNotes"].ToString().Replace("\\n", "\n");

                                }

                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begin

                                if (!string.IsNullOrEmpty(Session["SDTHomeSRDesc"] as string))
                                {
                                    if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                        ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].description = Session["SDTHomeSRDesc"].ToString().Replace("\\n", "\n");
                                }
                                else
                                {
                                    if (objClickTasksResponse.TaskExists == false)
                                    {
                                        var desc = objSiebelResponse.serviceRequest.description;
                                        var originalSRDesc = objSiebelResponse.serviceRequest.description;
                                        ViewBag.CompleteSRDesc = originalSRDesc;

                                        desc = desc.Replace("<", string.Empty);
                                        desc = desc.Replace(">", string.Empty);

                                        if (desc.Length > 256)
                                        {
                                            ViewBag.isValid = "true";
                                            desc = desc.Substring(0, 256);
                                        }
                                        ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].description = desc;
                                    }
                                    else
                                    {
                                        ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].description = objSiebelResponse.serviceRequest.description;
                                    }
                                }
                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

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

                                        //Code Start - Tejashree - 07/04/2017 - Scope: US210 Contract Popup Issue
                                        if (!string.IsNullOrEmpty(objClickTasksResponse.SystemId) && !string.IsNullOrEmpty(objSiebelResponse.serviceRequest.openedDate))
                                        {

                                            var contractData = objClickCallService.GetContractDetails(objClickTasksResponse.SystemId, objSiebelResponse.serviceRequest.openedDate);
                                            if (contractData == null)
                                            {
                                                Session["clickContractAvailabilityFlag"] = "N";
                                                //Code Start-Prajna - 18/04/2017 - US187/TA1417
                                                //When contract is not present in click "*From contract" Label is not displayed in Home page
                                                ViewBag.lEarlyStart = "";
                                                ViewBag.lLateStart = "";
                                            }
                                            else
                                            {
                                                ViewBag.lEarlyStart = "* From contract";
                                                ViewBag.lLateStart = "* From contract";
                                            }
                                            //Code End-Prajna - 18/04/2017 - US187/TA1417
                                        }
                                        //Code End - Tejashree - 07/04/2017 - Scope: US210 Contract Popup Issue
                                        //Code Comment starts by Prajna for US187/TA1417-18/04/2017 
                                        //ViewBag.lEarlyStart = "* From contract";
                                        //ViewBag.lLateStart = "* From contract";
                                        //Code Comment Ends by Prajna for US187/TA1417-18/04/2017
                                        ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                        ViewBag.CREntitlementFlag = "Y";
                                        ViewBag.CRcontractFlag = "No";

                                    }
                                    Session["gTaskNotes"] = null;
                                    Session["gTaskNotes"] = ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment;
                                    Session["Installation"] = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                    Session["gTaskNotes"] = Session["gTaskNotes"].ToString().Replace("\n", "\\n");

                                    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begin
                                    Session["gSRDesc"] = null;
                                    Session["gSRDesc"] = ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].description;
                                    Session["gSRDesc"] = Session["gSRDesc"].ToString().Replace("\n", "\\n");
                                    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                                }
                                else
                                {
                                    if (Session["taskStatus"] == "New")
                                    {
                                        //Early Start and Late Start logic based on srType
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "FMI")
                                        {
                                            //FMI ES & LS Logic Start                                            

                                            //Code changes by Ebaad on 22/3/2017 for US134                                                                          
                                            DateTime esFMI = (ViewModel.SiebelResponseAttributes.serviceRequest.created != "" ?
                                                Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.created) : tzConversion);

                                            //Code changes by Ebaad on 22/3/2017 for US134
                                            DateTime esLFMI = (ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate != "" ?
											//Deepak – 23/03/2017 – US149/TaskNo#1283 - Replacing FMI days from 7 to 14 and Time-23:59
                                                Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate).AddDays(-14) : tzConversion);
												//US149 - Code change ends here
                                            //Code changes by Ebaad on 22/3/2017 for US134
                                            DateTime esMFMI = (ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate != "" ?
												//Deepak – 23/03/2017 – US149/TaskNo#1283 - Replacing FMI days from 7 to 14 and Time-23:59
                                               Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate).AddDays(14) : tzConversion);
												//US149 - Code change ends here
                                            //FMI due date is more than 7 days away from today’s date.

                                            if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate))
                                            {
                                                //Code changes by Ebaad on 22/3/2017 for US134
												//Deepak – 23/03/2017 – US149/TaskNo#1283 - Replacing FMI days from 7 to 14 and Time-23:59
                                                if (tzConversion.AddDays(14) < Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate))
                                                {
                                                    //Code changes by Ebaad on 22/3/2017 for US134
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (tzConversion).AddMinutes(10);
                                                   
												   ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = CalculateLSForFMI(Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate), Convert.ToInt32(-14), Convert.ToInt32(23), Convert.ToInt32(59));

                                                }
												//US149 - code change ends here
                                                //FMI Due date is less than or equal to 7 days from today's date 
                                                //Code changes by Ebaad on 22/3/2017 for US134
												//Deepak – 23/03/2017 – US149/TaskNo#1283 - Replacing FMI days from 7 to 14 and Time-23:59
                                               
                                            	//US149 - code change ends here
                                                else
                                                {
                                                    //Code changes by Ebaad on 22/3/2017 for US134
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (tzConversion).AddMinutes(10);
                                                   
													ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = CalculateLSForFMI(Convert.ToDateTime(tzConversion), Convert.ToInt32(+14), Convert.ToInt32(23), Convert.ToInt32(59));
                                                }
                                            }
                                            else
                                            {
                                                //Code changes by Ebaad on 22/3/2017 for US134
                                                //Deepak – 23/03/2017 – US149/TaskNo#1283 - Replacing FMI days from 7 to 14 and Time-23:59
                                                ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = (tzConversion).AddMinutes(10);
                                                
                                                ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = CalculateLSForFMI(Convert.ToDateTime(tzConversion), Convert.ToInt32(+14), Convert.ToInt32(23), Convert.ToInt32(59));
                                            }


                                        }
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "PM")
                                        {
                                            //FMI ES & LS Logic Start for PM Job
                                            //If Desired Date exists & is beyond today's date

                                            DateTime startTimeTx9 = DateTime.Now;
                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0 && !string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime))
                                            {
                                                //Code changes by Ebaad on 22/3/2017 for US134
                                                // ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime = (objCountriesTimeZoneConversion.ConvertDesiredDate(Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime), objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).ToString();
                                                DateTime gEHCDesireDate = Convert.ToDateTime((objCountriesTimeZoneConversion.ConvertDesiredDate(Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime), objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress)).ToString());
                                                Session["gEHCDesireDate"] = gEHCDesireDate;
                                                //Code changes by Ebaad on 22/3/2017 for US134
                                                if (tzConversion < gEHCDesireDate)
                                                {
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = gEHCDesireDate;
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = gEHCDesireDate.AddDays(15);
                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartPM = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartPM;
                                                }
                                                else
                                                {
                                                    //Code changes by Ebaad on 22/3/2017 for US134
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = tzConversion;
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = tzConversion.AddDays(15);
                                                }
                                            }
                                            else
                                            {
                                                DateTime esPM = (ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate != "" ?

                                                 Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate).AddDays(-15) : tzConversion);
                                                DateTime lsPM = (ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate != "" ?

                                                Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate).AddDays(15) : tzConversion);
                                                //If no desired date exists & Schedule Date-15 is beyond today's date
                                                //Code changes by Ebaad on 22/3/2017 for US134
                                                if (tzConversion < esPM)
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
                                                //Code changes by Ebaad on 22/3/2017 for US134
                                                if (tzConversion >= lsPM || tzConversion.AddDays(-15) > lsPM)
                                                {
                                                    //Code changes by Ebaad on 22/3/2017 for US134
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = tzConversion.AddMinutes(10);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = tzConversion.AddDays(15);

                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartPM = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartPM;

                                                }
                                                //If no desired date exists &Schedule Date-15 is today's date or a date in the past 
                                                //Code changes by Ebaad on 22/3/2017 for US134
                                                else if (tzConversion >= esPM)
                                                {
                                                    //Code changes by Ebaad on 22/3/2017 for US134
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = tzConversion.AddMinutes(10);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lsPM;
                                                    string strLatestr = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.lateStart).ToShortDateString();
                                                    DateTime lateStartPM = Convert.ToDateTime(strLatestr).AddHours(9);
                                                    ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartPM;
                                                }
                                            }
                                            DateTime endTimeTx9 = DateTime.Now;
                                            TimeSpan Tx9 = endTimeTx9.Subtract(startTimeTx9);

                                            logger.Debug("In Home controller Index Method for PM | Call to CountriesTimeZoneConversion.cs (objCountriesTimeZoneConversion.ConvertDesiredDate) ~ Request: " + ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime + " , " + objSiebelResponse.serviceRequest.shipToAddress + " ~ Response: Early Start: " + ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart + " Late Start: " + ViewModel.SiebelResponseAttributes.serviceRequest.lateStart + " ~ Call duration: " + Tx9.TotalMilliseconds);

                                        }

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            //Code changes by Ebaad on 22/3/2017 for US134
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = tzConversion.AddMinutes(10);
                                            var newDate = tzConversion;

                                            ViewBag.getCurrentDate = Convert.ToDateTime(newDate).ToString("dd/MM/yyyy HH:mm");
                                        }

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.gEMSEntitlementFlag == "Y"
                                            && ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Corrective Repair")
                                        {
                                            //Code Start - Tejashree - 07/04/2017 - Scope: US210 Contract Popup Issue
                                            if (!string.IsNullOrEmpty(objClickTasksResponse.SystemId) && !string.IsNullOrEmpty(objSiebelResponse.serviceRequest.openedDate))
                                            {

                                                var contractData = objClickCallService.GetContractDetails(objClickTasksResponse.SystemId, objSiebelResponse.serviceRequest.openedDate);
                                                if (contractData == null)
                                                {
                                                    Session["clickContractAvailabilityFlag"] = "N";
                                                    //Code Start-Prajna - 18/04/2017 - US187/TA1417
                                                    //When contract is not present in click "*From contract" Label is not displayed in Home page
                                                    ViewBag.lEarlyStart = "";
                                                    ViewBag.lLateStart = "";
                                                }
                                                else
                                                {
                                                    ViewBag.lEarlyStart = "* From contract";
                                                    ViewBag.lLateStart = "* From contract";
                                                }
                                                //Code End-Prajna - 18/04/2017 - US187/TA1417
                                            }
                                            //Code End - Tejashree - 07/04/2017 - Scope: US210 Contract Popup Issue

                                            //Code changes by Ebaad on 22/3/2017 for US134                                            
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = tzConversion;
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = tzConversion.AddDays(1).AddHours(0.1);

                                            //Code Comment starts by Prajna for US187/TA1417-18/04/2017
                                            //ViewBag.lEarlyStart = "* From contract"; 
                                            //ViewBag.lLateStart = "* From contract";  
                                            //Code Comment ends by Prajna for US187/TA1417-18/04/2017
                                            ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                            ViewBag.CREntitlementFlag = "Y";
                                            ViewBag.CRcontractFlag = "Yes";
                                        }

                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.gEMSEntitlementFlag == "N"
                                          && ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Corrective Repair")
                                        {
                                            //Code changes by Ebaad on 22/3/2017 for US134                                            
                                            string strLatestr = Convert.ToDateTime(tzConversion.AddDays(5)).ToShortDateString();

                                            DateTime lateStartCR = Convert.ToDateTime(strLatestr).AddHours(9);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = tzConversion.AddMinutes(10);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = lateStartCR;
                                        }
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType != "Corrective Repair" && ViewModel.SiebelResponseAttributes.serviceRequest.srType != "Installation" && ViewModel.SiebelResponseAttributes.serviceRequest.srType != "PM" && ViewModel.SiebelResponseAttributes.serviceRequest.srType != "FMI")
                                        {
                                            //Code changes by Ebaad on 22/3/2017 for US134
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = tzConversion.AddMinutes(10);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.lateStart = tzConversion.AddDays(7);
                                        }


                                    }
                                    else
                                    {
                                        if (ViewModel.SiebelResponseAttributes.serviceRequest.srType == "Installation")
                                        {
                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                                ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].comment = Convert.ToString(Session["TaskNotes"]);

                                            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begin
                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                                ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].description = Convert.ToString(Session["SRDesc"]);
                                            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

                                            ViewModel.SiebelResponseAttributes.serviceRequest.postcode = Convert.ToString(Session["PostCode"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.address = Convert.ToString(Session["Address"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.city = Convert.ToString(Session["City"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.countryID = Convert.ToString(Session["CountryID"]);
                                            ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart = Convert.ToDateTime(Session["ModifiedES"]);
                                            //ViewModel.SiebelResponseAttributes.serviceRequest.earlyStart=objCountriesTimeZoneConversion.GetTimeZone(objSiebelResponse.serviceRequest.countryCode, objSiebelResponse.serviceRequest.shipToAddress);

                                            ViewBag.TaskDurationInstall = Convert.ToInt32(Session["ModifiedDuration"]);
                                            ViewBag.SRType = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                            //Code changes by Ebaad on 22/3/2017 for US134
                                            var newDate = tzConversion;
                                            ViewBag.getCurrentDate = Convert.ToDateTime(newDate).ToString("dd/MM/yyyy HH:mm");
                                            logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " Task Duration: " + Session["ModifiedDuration"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                                            logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " View Bag Task Duration: " + ViewBag.TaskDurationInstall + " Request Duration: " + Session["SDTHomeDuration"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                                        }
                                        else
                                        {
                                            //Code changes by Ebaad on 22/3/2017 for US134
                                            var currentDateTime = tzConversion;
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

                                            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                                            if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0)
                                                ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].description = Convert.ToString(Session["SRDesc"]);
                                            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                                            logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " Task Duration: " + Session["ModifiedDuration"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                                            logger.Debug("In HomeController Index method | Session logger: " + SiebelHttpPostParams.ActivityNo + " View Bag Task Duration: " + ViewBag.TaskDuration + " Request Duration: " + Session["SDTHomeDuration"] + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

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

                                        //Jayesh Soni - US82 - 18/04/2017 - SR Description- Begins

                                        Session["gSRDesc"] = null;
                                        Session["gSRDesc"] = ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].description;
                                        //Session["Installation"] = ViewModel.SiebelResponseAttributes.serviceRequest.srType;
                                        Session["gSRDesc"] = Session["gSRDesc"].ToString().Replace("\n", "\\n");
                                        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

                                    }
                                }

                                //------------
                                if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate))
                                {
                                    //Deepak – 23/03/2017 – US149/TaskNo#1283 - Replacing FMI days from 7 to 14 and Time-23:59
                                    DateTime dt = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate);
                                    
                                    ViewModel.SiebelResponseAttributes.serviceRequest.fmiDueDate = dt.ToString("dd/MM/yyyy");

                                }
                                if (!string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate))
                                {

                                    DateTime dt = Convert.ToDateTime(ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate);
                                    ViewModel.SiebelResponseAttributes.serviceRequest.gEHCSchedDate = dt.ToString("dd/MM/yyyy");

                                }

                                if (ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList.Count() > 0 && !string.IsNullOrEmpty(ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime))
                                {

                                    DateTime startTimeTx16 = DateTime.Now;
                                    //changes For Desired Date                                   
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
                                    DateTime endTimeTx16 = DateTime.Now;
                                    TimeSpan Tx16 = endTimeTx16.Subtract(startTimeTx16);
                                    logger.Debug("In Home controller Index Method | Call to CountriesTimeZoneConversion.cs For changes in desired date (objCountriesTimeZoneConversion.ConvertDesiredDate) ~ " + IP + " ~ Request: " + ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime + " , " + objSiebelResponse.serviceRequest.shipToAddress + " ~ Response: " + ViewModel.SiebelResponseAttributes.serviceRequest.activityDetailList[0].gEHCDesiredDateTime + " ~ Call duration: " + Tx16.TotalMilliseconds);
                                }

                                logger.Debug("In HomeController Index method | End of Index Method" + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");

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


                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                                if (!string.IsNullOrEmpty(Session["SRDesc"] as string))
                                    Session["SRDesc"] = null;
                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

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

                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                                if (!string.IsNullOrEmpty(Session["SDTHomeSRDesc"] as string))
                                    Session["SDTHomeSRDesc"] = null;
                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

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

                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                                if (!string.IsNullOrEmpty(Session["SRDesc"] as string))
                                    Session["SRDesc"] = null;
                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

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

                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins

                                if (!string.IsNullOrEmpty(Session["SDTHomeSRDesc"] as string))
                                    Session["SDTHomeSRDesc"] = null;
                                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                                
                                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                                    Session["SDTHomeIsRequiredfse"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                                    Session["SDTHomeProfile"] = null;

                                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                                    Session["SDTHomeFseSkill"] = null;
                                logger.Error("In HomeController Index method | (Error code 204) No Record available at Siebel" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                                TempData["ErrorCode"] = "204";
                                //US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message added when the Activity ID is not found  
                                TempData["Errormsg"] = "Activity Id Not Found. Please try relaunch SDT from Siebel. If the Issue persists, please contact Application helpdesk for support."; 
                                return RedirectToAction("Errormsg", "ErrorPage");
                            }

                        }
                        logger.Error("In HomeController Index method | Siebel Response stream Null" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                    }
                    logger.Debug("In HomeController Index method | Siebel Response Null" + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
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


                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins

                if (!string.IsNullOrEmpty(Session["SRDesc"] as string))
                    Session["SRDesc"] = null;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
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

                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins

                if (!string.IsNullOrEmpty(Session["SDTHomeSRDesc"] as string))
                    Session["SDTHomeSRDesc"] = null;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
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

                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                if (!string.IsNullOrEmpty(Session["SRDesc"] as string))
                    Session["SRDesc"] = null;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
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

                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                if (!string.IsNullOrEmpty(Session["SDTHomeSRDesc"] as string))
                    Session["SDTHomeSRDesc"] = null;
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                if (!string.IsNullOrEmpty(Session["SDTHomeIsRequiredfse"] as string))
                    Session["SDTHomeIsRequiredfse"] = null;

                if (!string.IsNullOrEmpty(Session["SDTHomeProfile"] as string))
                    Session["SDTHomeProfile"] = null;
                if (!string.IsNullOrEmpty(Session["SDTHomeFseSkill"] as string))
                    Session["SDTHomeFseSkill"] = null;
                //Logger.Error("Exception thrown at Home Controller Index Action", ex);
                logger.Error("In HomeController Index method | Exception from Siebel call: " + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                TempData["ErrorCode"] = "";

                //TempData["Errormsg"] = "Unable to connect to Siebel";

                // Error Message is taken from US06_AC06.
                // TempData["Errormsg"] = "SDT Booking cannot be launched due to technical issues. Please contact your administrator.";

                //Added By phani kanth
                //US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message added when the whole Index method fails 
                TempData["Errormsg"] = "Unable to process the request due to system error. Please reach out to Application helpdesk for support. Technical Message: " + ex.Message;

                return RedirectToAction("Errormsg", "ErrorPage");

                //return View();
            }
            #endregion
        }
        /// <summary>
        //This Function will calculate LS when WO type is FMI
        //Created By: Deepak Vishwakarma - 8/05/2017
        //Scope     : US149/TA1283
        /// </summary>
        /// <returns></returns>
        //Deepak – 23/03/2017 – US149/TaskNo#1283 - Replacing FMI days from 7 to 14 and Time-23:59        
        public static DateTime CalculateLSForFMI(DateTime actualDate, int daysToBeAdded, int hoursToBeAdded, int minToBeAdded)
        {
            DateTime calculatedLS;
            string shortDatetime = actualDate.ToShortDateString();
            DateTime newDate = Convert.ToDateTime(shortDatetime).AddDays(daysToBeAdded);
            calculatedLS = newDate.AddHours(hoursToBeAdded);
            calculatedLS = calculatedLS.AddMinutes(minToBeAdded);
            return calculatedLS;
        }
        //Code Changes end here - TA1283
        /// <summary>
        /// This method gives the site and system count from click service call
        /// </summary>
        /// <returns></returns>
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

                DateTime startTimeTx4 = DateTime.Now;
                // objtaskResponse = (ScheduleServiceDev1.GetTasksResponse)Session["TaskResponseByTask"];
                var resTasksResponseByTask = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.ActivityNo, "TaskID");
                DateTime endTimeTx4 = DateTime.Now;
                TimeSpan Tx4 = endTimeTx4.Subtract(startTimeTx4);
                logger.Debug("In Home controller GetSiteSytemClickTaskCount Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName)  ~ " + IP + " ~ Request: ActivityNo: " + SiebelHttpPostParams.ActivityNo + " ~ Response : Task List ~ Call duration: " + Tx4.TotalMilliseconds);

                //below condition added by phani kanth p.

                resTasksResponseByTask.Tasks = resTasksResponseByTask.Tasks.Where(x => x.TaskType.DisplayString != "Parts Pickup").ToArray();

                objtaskResponse = (ScheduleServiceDev1.GetTasksResponse)resTasksResponseByTask;

                if (objtaskResponse != null && objtaskResponse.Tasks.Count() > 0)
                {
                    callID = objtaskResponse.Tasks[0].CallID;
                }


                DateTime startTimeTx5 = DateTime.Now;
                var resTasksResponseBySite = await objClickCallService.GetTasksRequestByPropertyName(SiebelHttpPostParams.ShipToSite, "TaskSiteID");
                DateTime endTimeTx5 = DateTime.Now;
                TimeSpan Tx5 = endTimeTx5.Subtract(startTimeTx5);
                logger.Debug("In Home controller GetSiteSytemClickTaskCount Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName)  ~ " + IP + " ~ Request : " + SiebelHttpPostParams.ShipToSite + " ~ Response : Task List ~ Call duration: " + Tx5.TotalMilliseconds);

                DateTime startTimeTx6 = DateTime.Now;
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

                DateTime endTimeTx6 = DateTime.Now;
                TimeSpan Tx6 = endTimeTx6.Subtract(startTimeTx6);
                logger.Debug("In Home controller GetSiteSytemClickTaskCount Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName)  ~ " + IP + " ~ Request : SystemID: " + SiebelHttpPostParams.SystemID + "~ Response : Site Count: " + SiteCountPotential.Count() + " System Count: " + SystemCountPotential.Count() + " ~ Call duration: " + Tx6.TotalMilliseconds);

                DateTime endTime = DateTime.Now;
                TimeSpan span = endTime.Subtract(sstartTime);
                lstCounts.receiveTime = span.Minutes * 60 + span.Seconds + Convert.ToInt32(Session["FSEreceiveTime"]);
                return Json(lstCounts, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error("In HomeController GetSiteSytemClickTaskCount method | Exception occured while fetching site and system count from Click Service call: " + ex.Message + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Fetches FSE value/sso from Click
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetFSE1ddlItems()
        {
            DateTime fseStartTime = DateTime.Now;

            try
            {
                SSOLogic objsso = new SSOLogic();
                List<SelectListItem> EngineerList = new List<SelectListItem>();

                DateTime startTimeTx17 = DateTime.Now;
                var result = await objsso.GetSSODetails(((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);
                DateTime endTimeTx17 = DateTime.Now;
                TimeSpan Tx17 = endTimeTx17.Subtract(startTimeTx17);

                // FOR SIEBEL ID START             
                if (result != null)
                {
                    logger.Debug("In Home controller GetFSE1ddlItems Method | Call to ClickCallService.cs (SSOLogic.GetSSODetails) ~ " + IP + " ~ Request : SystemID: " + ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID + " ~ Response : Count of Engg. list: " + result.Count + " ~ Call duration: " + Tx17.TotalMilliseconds);
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

                    DateTime startTimeTx19 = DateTime.Now;
                    var searchssoidTextbox = await objsso.GetValidSearchSSOID(Session["PreferredFSEs"].ToString(), ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);
                    DateTime endTimeTx19 = DateTime.Now;
                    TimeSpan Tx19 = endTimeTx19.Subtract(startTimeTx19);
                    logger.Debug("In Home controller GetFSE1ddlItems Method | Call to SSOLogic.cs for preferred FSE (SSOLogic.GetSSODetails) ~ " + IP + " ~ Request : SystemID : " + ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID + " ~ Response : SSO of Engg.: " + result + " ~ Call duration: " + Tx19.TotalMilliseconds);
                    EngineerList2 = searchssoidTextbox.ToList<SelectListItem>();
                    EngineerList.AddRange(EngineerList2);
                }

                //Required FSE added by Phani Kanth
                if (!string.IsNullOrEmpty(Session["RreferredFSEs"] as string))
                {
                    List<SelectListItem> RreferredFSEsEngineerList = new List<SelectListItem>();

                    DateTime startTimeTx20 = DateTime.Now;
                    var searchssoidTextbox = await objsso.GetValidSearchSSOID(Session["RreferredFSEs"].ToString(), ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);
                    DateTime endTimeTx20 = DateTime.Now;
                    TimeSpan Tx20 = endTimeTx20.Subtract(startTimeTx20);
                    logger.Debug("In Home controller GetFSE1ddlItems Method | Call to SSOLogic.cs for Required FSE (SSOLogic.GetSSODetails) ~ " + IP + " ~ Request : SystemID : " + ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID + " , " + " SSO ID: " + Session["PreferredFSEs"].ToString() + " ~ Response : SSO of Engg.: " + result + " ~ Call duration: " + Tx20.TotalMilliseconds);
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
                logger.Error("In HomeController GetFSE1ddlItems method | Exception occured while fetching SSO details from ClickSoftware Service call:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method fetches SSO details from Clicksoftware Service Call
        /// </summary>
        /// <param name="SSOID"></param>
        /// <returns></returns>
        public ActionResult GetValidFSESSOID(string SSOID)
        {

            try
            {
                SSOLogic objsso = new SSOLogic();

                DateTime startTimeTx18 = DateTime.Now;
                var result = objsso.GetValidFSESSOID(SSOID, ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID);
                DateTime endTimeTx18 = DateTime.Now;
                TimeSpan Tx18 = endTimeTx18.Subtract(startTimeTx18);
                logger.Debug("In Home controller GetValidFSESSOID Method | Call to ClickCallService.cs (SSOLogic.GetValidFSESSOID) ~ " + IP + " ~ Request : SystemID: " + ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).SystemID + " ~ Response : SSO of Engg.: " + result + " ~ Call duration: " + Tx18.TotalMilliseconds);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("In HomeController GetFSE1ddlItems method | Exception occured while fetching SSO details from ClickSoftware Service call: " + ex.Message + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Clears the Session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SessionClear()
        {
            Session.Abandon();

            return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

    }
}
