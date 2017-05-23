using NewSDTApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using NewSDTApplication.ScheduleServiceDev1;
using SDTLogger;
using System.Threading;
using NewSDTApplication.ServiceOptimizationService;
using System.Collections;


namespace NewSDTApplication.Utilities
{
    public class CallClickSerrvice
    {
        CountriesTimeZoneConversion objCountriesTimeZoneConversion;
        private ScheduleServiceClient ScheduleService;
        private ServiceOptimizationServiceClient ServiceOptimizationService;
        private ScheduleServiceDev1.Task task;
        private ScheduleServiceDev1.OptionalParametersItem[] optparam;
        ScheduleServiceDev1.OptionalParameters opt;
        private const string ExtendedWithOneHour = "Alternative 1";
        private const string ExtendedWithTwoHours = "Alternative 2";
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<GetTasksResponse> GetTasksRequestByPropertyName(string propValue, string propertyName)
        {
            logger.Debug("CallClickSerrvice.cs;GetTasksRequestByPropertyName; Start");

            OpenConnectionToSDT();
            GetTasksRequest getTasksRequest = new GetTasksRequest();
            getTasksRequest.OptionalParameters = new NewSDTApplication.ScheduleServiceDev1.OptionalParameters() { CallerIdentity = "", ErrorOnNonExistingDictionaries = true };
            getTasksRequest.RequestedProperties = new string[] { "CallID",
                                                                "TaskSystemName",
                                                                "Number",
                                                                "MUSTJobNumber",
                                                                "TaskType",
                                                                "SystemID",
                                                                "TaskSiteID",
                                                                "TaskSiteName",
                                                                "Status",
                                                                "SkillLevel",
                                                                "TimeDependencies",
                                                                "TaskSystemModality",
                                                                "TaskSystemProductName",
                                                                "Duration",
                                                                "DurationSpecified",
                                                                "TaskSystemID",
                                                                "LateStart",
                                                                "EarlyStart",
                                                                "Key",
                                                                "IsMST",
                                                                "JeopardyState",
                                                                "KeySpecified",
                                                                "Priority",
                                                                "PrioritySpecified",
                                                                "TaskSystemProductID",
                                                                "AppointmentFinish",
                                                                "AppointmentStart",
                                                                "TaskID",
                                                                "RequiredFSEs",
                                                                "PartDeliveryType",
                                                                "PartEstimatedDeliveryDate",
                                                                "PartComment",
                                                                "Street",
                                                                "City",
                                                                "Postcode",
                                                                "CountryID",
                                                                "Notes",
                                                                "PreferredFSEs",
                                                                "Latitude",
                                                                "Longitude"

                                                               };

            getTasksRequest.AssignmentRequestedProperties = new string[] { "Start",
                                                                          "Finish",
                                                                          "Task",
                                                                          "AssignedEngineers",
                                                                          "Engineers"
                                            };
            string PropertyId = "";
            switch (propertyName)
            {
                case "TaskSystemID":
                    PropertyId = "133";
                    break;
                case "TaskSiteID":
                    PropertyId = "135";
                    break;
                case "TaskRfsNumber":
                    PropertyId = "87";
                    break;
                case "TaskActivityID":
                    PropertyId = "003";
                    break;
                case "MUSTJobNumber":
                    PropertyId = "087";
                    break;
                case "TaskID":
                    PropertyId = "086";
                    break;
            }
            getTasksRequest.GetAssignments = true;
            getTasksRequest.EnableGroupOnTheFly = true;
            //getTasksRequest.GetAssignments = true;
            //getTasksRequest.AssignmentRequestedProperties = AssignmentRequestedProperties;
            getTasksRequest.Group = new NewSDTApplication.ScheduleServiceDev1.Group();
            getTasksRequest.Group.GroupParameters = new string[] { propValue };
            getTasksRequest.Group.GroupType = 2;
            getTasksRequest.Group.GroupTypeSpecified = true;
            getTasksRequest.Group.Body = @"<CONDITION TYPE=""101"" TYPE_TEXT=""Arithmetic Condition""><VALUE TYPE=""200"" TYPE_TEXT=""Property Value""><ID>" + PropertyId + @"</ID></VALUE><OPERATION TYPE=""600"" TYPE_TEXT=""="" /><VALUE TYPE=""201"" TYPE_TEXT=""Const Value"">%1</VALUE></CONDITION>";
            // Response from Click on TaskList for particular site
            GetTasksResponse getTasksResponse = new GetTasksResponse();
            // getTasksResponse.Tasks = ScheduleService.GetTaskAsync(getTasksRequest);
            //var result = await ScheduleService.GetTaskAsync(getTasksRequest);
            var objGetTasksResponse = await ScheduleService.GetTasksAsync(getTasksRequest);
            //  objGetTasksResponse.Tasks = objGetTasksResponse.Tasks.Where(x => x.Number == 1).ToArray();
            //commented by Rajesh
            //getTasksResponse.Tasks = ScheduleService.GetTasks(getTasksRequest.OptionalParameters,
            //    getTasksRequest.Indexes, getTasksRequest.KeySet, getTasksRequest.Group,
            //     getTasksRequest.EnableGroupOnTheFly, getTasksRequest.RequestedProperties,
            //     getTasksRequest.GetAssignments, getTasksRequest.AssignmentRequestedProperties,
            //     out getTasksResponse.Assignments);
            logger.Debug("CallClickSerrvice.cs;GetTasksRequestByPropertyName; End");

            return objGetTasksResponse;
        }

        public Tuple<string, string> GetESLSByStandardOperation(int duration)
        {

            Tuple<string, string> dates = null;
            logger.Debug("CallClickSerrvice.cs;GetESLSByPropertyName; Start");
            try
            {
                OpenConnectionToSDT();
                ServiceOptimizationService.ExecuteOperationRequest SingleOperation = new ServiceOptimizationService.ExecuteOperationRequest();
                SingleOperation.OptionalParameters = new ServiceOptimizationService.OptionalParameters();
                SingleOperation.OptionalParameters.CallerIdentity = "MUST";
                SingleOperation.OptionalParameters.ErrorOnNonExistingDictionaries = true;
                NewSDTApplication.ServiceOptimizationService.Task objSiebelTask = new NewSDTApplication.ServiceOptimizationService.Task();

                NewSDTApplication.ServiceOptimizationService.StandardOperation objStandardOperation = null;
                objStandardOperation = new StandardOperation();

                SingleOperation.RequestedProperties = new string[] { "EarlyStart", "LateStart" };
                objStandardOperation.Action = "Preview";
                objStandardOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                objStandardOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)objSiebelTask;

                //objSiebelTask.Duration = 
                //objSiebelTask.Priority=            
                //objSiebelTask.SkillLevel =

                var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];
                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                objSiebelTask.CallID = "APAC" + "-" + res.serviceRequest.activityDetailList[0].activityId + "-" + "1";
                objSiebelTask.Number = 1;
                objSiebelTask.NumberSpecified = true;
                objSiebelTask.OpenDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), CultureInfo.GetCultureInfo("en-gb")));

                //objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                objSiebelTask.OpenDateSpecified = true;
                objSiebelTask.Duration = duration * 60;// 14400;
                objSiebelTask.DurationSpecified = true;
                objSiebelTask.TaskID = res.serviceRequest.activityDetailList[0].activityId;
                objSiebelTask.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference CRMReference = new NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                objSiebelTask.CRMSystemName = CRMReference;

                NewSDTApplication.ServiceOptimizationService.GEHCSystemReference SystemReference = new NewSDTApplication.ServiceOptimizationService.GEHCSystemReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).SystemID };
                objSiebelTask.SystemID = SystemReference;
                TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
                if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                {
                    string taskType = string.Empty;
                    taskType = objSiebelToClick.GetTaskType(res.serviceRequest.srType);
                    if (!string.IsNullOrEmpty(taskType))
                    //task.TaskType = new ScheduleServiceDev1.TaskTypeReference() { Name = taskType };
                    {
                        NewSDTApplication.ServiceOptimizationService.TaskTypeReference TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference() { Name = taskType };
                        objSiebelTask.TaskType = TaskType;
                    }
                }

                //NewSDTApplication.ServiceOptimizationService.TaskTypeReference TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference() { Name = "PM" };
                //objSiebelTask.TaskType = TaskType;

                objSiebelTask.CustomerExpectation = "B";
                DateTime startTime = DateTime.Now;
                var results = ServiceOptimizationService.ExecuteOperation(SingleOperation.OptionalParameters, objStandardOperation, SingleOperation.RequestedProperties.ToArray());

                var task = (NewSDTApplication.ServiceOptimizationService.Task)results;

                dates = new Tuple<string, string>(task.EarlyStart.ToString(), task.LateStart.ToString());
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                logger.Debug("CallClickSerrvice.cs;GetESLSByStartdardOperation;ExecuteOperation TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);
                CloseConnectionSDT();
                return dates;
            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;GetESLSByPropertyName; Exception: " + ex.Message);
                return null;
            }
        }

        public bool OpenConnectionToSDT()
        {
            try
            {
                logger.Debug("CallClickSerrvice.cs;OpenConnectionToSDT; Start");

                //ScheduleService = new ScheduleServiceClient();
                if (ScheduleService == null || ScheduleService.State == CommunicationState.Closed || ScheduleService.State == CommunicationState.Faulted)
                {

                    BasicHttpBinding basicHttpBindingscheduleService = new BasicHttpBinding(new BasicHttpSecurityMode());
                    EndpointAddress scheduleServiceClientEndpointAddress = new EndpointAddress(ConfigurationManager.AppSettings["ClickServicesURL"].ToString());
                    basicHttpBindingscheduleService.MaxReceivedMessageSize = 2147483647;
                    basicHttpBindingscheduleService.MaxBufferSize = 2147483647;
                    basicHttpBindingscheduleService.SendTimeout = new TimeSpan(0, 10, 0);
                    basicHttpBindingscheduleService.OpenTimeout = new TimeSpan(0, 20, 0);
                    basicHttpBindingscheduleService.CloseTimeout = new TimeSpan(0, 20, 0);
                    basicHttpBindingscheduleService.ReceiveTimeout = new TimeSpan(0, 20, 0);
                    basicHttpBindingscheduleService.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                    basicHttpBindingscheduleService.Security.Mode = BasicHttpSecurityMode.Transport;
                    basicHttpBindingscheduleService.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                    basicHttpBindingscheduleService.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                    ScheduleService = new ScheduleServiceDev1.ScheduleServiceClient(basicHttpBindingscheduleService, scheduleServiceClientEndpointAddress);
                    ScheduleService.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["ClickUserName"].ToString();
                    ScheduleService.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["ClickPassword"].ToString();
                    var proxy = ScheduleService.Endpoint.Binding as System.ServiceModel.BasicHttpBinding;
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClickProxyURL"].ToString()))
                    {
                        proxy.ProxyAddress = new Uri(ConfigurationManager.AppSettings["ClickProxyURL"].ToString());
                        proxy.BypassProxyOnLocal = false;
                        proxy.UseDefaultWebProxy = false;
                        ScheduleService.Endpoint.Binding = proxy;
                    }
                    ScheduleService.Open();
                }
                else
                {
                    if (ScheduleService.State != CommunicationState.Opened && ScheduleService.State != CommunicationState.Opening)
                    {
                        ScheduleService.Open();
                    }
                }

                if ((ServiceOptimizationService == null) || (ServiceOptimizationService.State == CommunicationState.Faulted) || (ServiceOptimizationService.State == CommunicationState.Closed))
                {
                    BasicHttpBinding basicHttpBindingserviceOptimizationService = new BasicHttpBinding(new BasicHttpSecurityMode());
                    EndpointAddress serviceOptimizationServiceEndpointAddress = new EndpointAddress(ConfigurationManager.AppSettings["ClickServicesOptimizationURL"].ToString());
                    ServiceOptimizationService = new ServiceOptimizationService.ServiceOptimizationServiceClient(basicHttpBindingserviceOptimizationService, serviceOptimizationServiceEndpointAddress);
                    ServiceOptimizationService.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["ClickUserName"].ToString();
                    ServiceOptimizationService.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["ClickPassword"].ToString();

                    basicHttpBindingserviceOptimizationService.MaxReceivedMessageSize = 2147483647;
                    basicHttpBindingserviceOptimizationService.MaxBufferSize = 2147483647;
                    // basicHttpBindingserviceOptimizationService.SendTimeout = new TimeSpan(0, 2, 0);
                    basicHttpBindingserviceOptimizationService.SendTimeout = new TimeSpan(0, 10, 0);
                    basicHttpBindingserviceOptimizationService.OpenTimeout = new TimeSpan(0, 10, 0);
                    basicHttpBindingserviceOptimizationService.CloseTimeout = new TimeSpan(0, 10, 0);
                    basicHttpBindingserviceOptimizationService.ReceiveTimeout = new TimeSpan(0, 10, 0);

                    basicHttpBindingserviceOptimizationService.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                    basicHttpBindingserviceOptimizationService.Security.Mode = BasicHttpSecurityMode.Transport;
                    basicHttpBindingserviceOptimizationService.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                    basicHttpBindingserviceOptimizationService.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                    var proxy = ServiceOptimizationService.Endpoint.Binding as System.ServiceModel.BasicHttpBinding;
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClickProxyURL"].ToString()))
                    {
                        proxy.ProxyAddress = new Uri(ConfigurationManager.AppSettings["ClickProxyURL"].ToString());
                        proxy.BypassProxyOnLocal = false;
                        proxy.UseDefaultWebProxy = false;
                        ScheduleService.Endpoint.Binding = proxy;
                    }
                    ServiceOptimizationService.Open();
                }
                else
                {
                    if (ServiceOptimizationService.State != CommunicationState.Opened && ServiceOptimizationService.State != CommunicationState.Opening)
                    {
                        ServiceOptimizationService.Open();
                    }
                }
                logger.Debug("CallClickSerrvice.cs;OpenConnectionToSDT; End");

                return true;
            }

            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;OpenConnectionToSDT; Exception: " + ex.Message);

                CloseConnectionSDT();
                // Logger.Error(ex.Message);
                return false;
            }
        }

        public void CloseConnectionSDT()
        {
            try
            {
                if (ScheduleService.State == CommunicationState.Opened || ScheduleService.State == CommunicationState.Opening)
                {
                    ScheduleService.Abort();
                    ScheduleService.Close();
                }
                if (ServiceOptimizationService.State == CommunicationState.Opened || ServiceOptimizationService.State == CommunicationState.Opening)
                {
                    ServiceOptimizationService.Abort();
                    ServiceOptimizationService.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;CloseConnectionSDT; Exception: " + ex.Message);

                //Logger.Error("Error while closing the service connection to Click Server" + ex.Message);
            }

        }

        public long GetTravelTimeWithKeys(int SourceKey, int DestinationKey)
        {
            long returnValue = -1;
            try
            {
                OpenConnectionToSDT();
                ServiceOptimizationService.OptionalParameters _optionalParameters = new ServiceOptimizationService.OptionalParameters() { CallerIdentity = "MUST", ErrorOnNonExistingDictionaries = true };

                //   RouteResult obj = ServiceOptimizationService.QueryRouteWithKeys(_optionalParameters, 118509599, 184191666);
                RouteResult obj = ServiceOptimizationService.QueryRouteWithKeys(_optionalParameters, SourceKey, DestinationKey);

                //ServiceOptimizationService.RouteResult obj =ServiceOptimizationService.qu .QueryRouteWithKeys(_optionalParameters, SourceKey, DestinationKey);
                returnValue = obj != null ? obj.TravelTime : -1;
                //  CloseConnectionSDT();
                return returnValue;
            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;GetTravelTimeWithKeys; Exception: " + ex.Message);
                CloseConnectionSDT();
                return returnValue;
            }
        }

        public async Task<IEnumerable<AppointmentSlots>> ClickCallToCreateTask(string earlystart, string latestart, int duration, string TaskNotes, string fseSkill, string PreferredFSEs, string AppStart = "", string AppFinish = "", bool RequiredFSE = false)
        {

            logger.Debug("CallClickSerrvice.cs;ClickCallToCreateTask; SessionTest: Task StartDate" + earlystart + "Duration" + duration);
            IEnumerable<AppointmentSlots> objAppSlotsList;
            AppointmentSlots objAppointmentSlots;
            DateTime startTime = DateTime.Now;
            try
            {
                OpenConnectionToSDT();
                task = new ScheduleServiceDev1.Task();
                ScheduleServiceDev1.TimeInterval period = new ScheduleServiceDev1.TimeInterval();
                optparam = new ScheduleServiceDev1.OptionalParametersItem[2];
                opt = new ScheduleServiceDev1.OptionalParameters() { CallerIdentity = "MUST", ExtraData = optparam, ErrorOnNonExistingDictionaries = true };
                var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];
                TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
                //ClickTranslator clicktranslator = new ClickTranslator();
                //clicktranslator = objSiebelToClick.TransformValues(res);
                var SiebelHttpPostParams = (HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"];
                ScheduleServiceDev1.ProcessTaskExRequest objProcessTaskExRequest = new ScheduleServiceDev1.ProcessTaskExRequest();
                if ((res != null) && (res.serviceRequest != null) && (!string.IsNullOrEmpty(res.serviceRequest.gEHCSafetyConcern)))
                {
                    task.IsSafety = objSiebelToClick.GetSafetyConcernValue();
                    task.IsSafetySpecified = true;
                }
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["taskStatus"] as string))
                {

                    if (HttpContext.Current.Session["taskStatus"] == "New")
                    {
                        task.CallID = "APAC" + "-" + res.serviceRequest.activityDetailList[0].activityId + "-" + "1";
                        task.Number = 1;
                    }
                    else
                    {
                        GetTasksResponse taskResponse = new GetTasksResponse();

                        task = new ScheduleServiceDev1.Task();
                        taskResponse = (ScheduleServiceDev1.GetTasksResponse)HttpContext.Current.Session["TaskResponseByTask"];
                        task = taskResponse.Tasks[0];
                        task.CallID = task.CallID;
                        task.Number = task.Number;
                    }
                }

                //task.OwnerName = Session["objPreferredSFESiebel"];
                task.NumberSpecified = true;
                //task.Priority = 10;
                task.PrioritySpecified = true;
                task.MUSTJobNumber = res.serviceRequest.srNumber;

                //IsBump Task added by Phani Kanth P.
                if (HttpContext.Current.Session["IsBumpTask"] != null)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["IsBumpTask"] as string))
                    {
                        if (HttpContext.Current.Session["IsBumpTask"].ToString() == "Yes")
                        {
                            task.SuperPowerTaskFlag = true;
                            task.SuperPowerTaskFlagSpecified = true;
                        }
                    }
                }

                // task.EarlyStart=Convert.ToDateTime(DateTime.)
                if (res.serviceRequest.srType == "Corrective Repair" && res.serviceRequest.gEMSEntitlementFlag == "Y" &&
                    string.IsNullOrEmpty(earlystart) && string.IsNullOrEmpty(latestart))
                {
                    task.EarlyStartSpecified = false;
                    task.LateStartSpecified = false;
                }
                else
                {

                    if (res.serviceRequest.srType == "Installation")
                    {
                        task.EarlyStart = Convert.ToDateTime(earlystart);
                        task.LateStart = Convert.ToDateTime(latestart);

                        task.IsMDT = true;
                        task.IsMDTSpecified = true;
                        task.EarlyStartSpecified = true;
                        task.LateStartSpecified = true;
                        // task.CustomerExpectation = "B";

                    }
                    else
                    {
                        if (res.serviceRequest.srType == "Corrective Repair" && res.serviceRequest.gEMSEntitlementFlag == "Y" &&
                         string.IsNullOrEmpty(earlystart))
                        {
                            task.LateStart = Convert.ToDateTime(DateTime.Parse(latestart, CultureInfo.GetCultureInfo("en-gb")));
                            task.LateStartSpecified = true;
                        }
                        else if (res.serviceRequest.srType == "Corrective Repair" && res.serviceRequest.gEMSEntitlementFlag == "Y" &&
                         string.IsNullOrEmpty(latestart))
                        {
                            task.EarlyStart = Convert.ToDateTime(DateTime.Parse(earlystart, CultureInfo.GetCultureInfo("en-gb")));
                            task.EarlyStartSpecified = true;
                        }
                        else
                        {
                            task.EarlyStart = Convert.ToDateTime(DateTime.Parse(earlystart, CultureInfo.GetCultureInfo("en-gb")));
                            task.LateStart = Convert.ToDateTime(DateTime.Parse(latestart, CultureInfo.GetCultureInfo("en-gb")));
                            task.EarlyStartSpecified = true;
                            task.LateStartSpecified = true;
                        }
                    }

                }
                //CustomerExpectation Added by phani kanth p
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Schedulingpolicy"] as string))
                {
                    task.SchedulingPolicy = Convert.ToString(HttpContext.Current.Session["Schedulingpolicy"]);
                }

                task.CustomerExpectation = "B";
                task.ContactName = res.serviceRequest.contactFirstName + " " + res.serviceRequest.contactLastName;
                task.ContactPhoneNumber = res.serviceRequest.gEHCWorkPhone;
                task.CustomerEmail = res.serviceRequest.contactEmail;
                // From here we will get the system status

                //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]
                //Checking null condtion in Systemstatus.
                string systemstatus = objSiebelToClick.GetSystemStatus(res.serviceRequest.currentEquiptmentStatus);
                if (!string.IsNullOrEmpty(systemstatus))
                {
                    var arr = systemstatus.Split('(');


                    if (string.IsNullOrEmpty(HttpContext.Current.Session["systemstatus"] as string))
                    {
                        if (arr[0].Trim() == "3")
                            HttpContext.Current.Session["systemstatus"] = "up";
                        else if (arr[0].Trim() == "2")
                            HttpContext.Current.Session["systemstatus"] = "partial";
                        else if (arr[0].Trim() == "1")
                            HttpContext.Current.Session["systemstatus"] = "down";
                    }
                }
                else
                { HttpContext.Current.Session["systemstatus"] = ""; }


                //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                if (!string.IsNullOrEmpty(res.serviceRequest.currentEquiptmentStatus))
                    task.SystemStatus = new ScheduleServiceDev1.GEHCSystemStatusReference { Name = systemstatus, DisplayString = systemstatus };
                task.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;
                task.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;

                if (!string.IsNullOrEmpty(fseSkill))
                {
                    task.SkillLevel = Convert.ToInt32(fseSkill);
                    task.SkillLevelSpecified = true;
                }

                if (!string.IsNullOrEmpty(PreferredFSEs))
                {
                    task.PreferredFSEs = PreferredFSEs;
                    if (HttpContext.Current.Session["IsRequiredFse"].ToString() != null)
                    {
                        if (HttpContext.Current.Session["IsRequiredFse"].ToString() == "true") task.RequiredFSEs = PreferredFSEs;
                        else task.RequiredFSEs = "";
                    }
                }
                else
                {
                    task.RequiredFSEs = "";
                    task.PreferredFSEs = "";
                }
                task.Priority = GetPriority();


                HttpContext.Current.Session["systemstatus"] = null;

                task.Duration = duration * 60;
                task.DurationSpecified = true;
                task.PrioritySpecified = true;
                ScheduleServiceDev1.GEHCCRMSystemReference CRMReference = new ScheduleServiceDev1.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                task.CRMSystemName = CRMReference;

                ScheduleServiceDev1.GEHCSystemReference SystemReference = new ScheduleServiceDev1.GEHCSystemReference() { ID = SiebelHttpPostParams.SystemID };
                task.SystemID = SystemReference;

                ScheduleServiceDev1.GEHCSiteReference SiteReference = new ScheduleServiceDev1.GEHCSiteReference() { ID = SiebelHttpPostParams.ShipToSite };
                //SiteID comment by Phani Kanth
                // task.SiteID = SiteReference;

                //task.TaskType = new ScheduleServiceDev1.TaskTypeReference() { Name = objSiebelToClick.GetTaskType(res.serviceRequest.srType) };
                //task.TaskSubType = new ScheduleServiceDev1.GEHCTaskSubTypeReference() { Name = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType) };
                if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                {
                    string taskType = string.Empty;
                    taskType = objSiebelToClick.GetTaskType(res.serviceRequest.srType);
                    if (!string.IsNullOrEmpty(taskType))
                        task.TaskType = new ScheduleServiceDev1.TaskTypeReference() { Name = taskType };
                }
                if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))
                {
                    string taskSubType = string.Empty;
                    taskSubType = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType);
                    if (!string.IsNullOrEmpty(taskSubType))
                        task.TaskSubType = new ScheduleServiceDev1.GEHCTaskSubTypeReference() { Name = taskSubType };
                }
                if (res.serviceRequest.srType == "FMI")
                {
                    if (!string.IsNullOrEmpty(res.serviceRequest.fmiDueDate))
                    {
                        task.FMIDueDate = Convert.ToDateTime(DateTime.Parse(res.serviceRequest.fmiDueDate, CultureInfo.GetCultureInfo("en-gb")));
                        task.FMIDueDateSpecified = true;
                    }
                    task.FMINumber = res.serviceRequest.fMINumber;

                }
                //task.DueDate = System.DateTime.Parse("2014-09-30T09:00:00");
                // Created date is missing in the API, we need to change once the API sends that value
                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                //if (!string.IsNullOrEmpty(res.serviceRequest.created))
                //{
                //    // res.serviceRequest.created = "03/11/2016 19:59:56";
                task.OpenDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), CultureInfo.GetCultureInfo("en-gb")));

                //objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                task.OpenDateSpecified = true;
                //}
                task.NumberOfRequiredEngineers = 1;
                task.NumberOfRequiredEngineersSpecified = true;

                task.Notes = TaskNotes;
                //task.JobComments = new ScheduleServiceDev1.GEHCJobCommentsReference() { DisplayString = TaskNotes };

                if (!String.IsNullOrEmpty(AppStart))
                {
                    task.AppointmentStart = Convert.ToDateTime(DateTime.Parse(AppStart, CultureInfo.GetCultureInfo("en-gb")));
                    task.AppointmentStartSpecified = true;
                }
                if (!String.IsNullOrEmpty(AppFinish))
                {
                    task.AppointmentFinish = Convert.ToDateTime(DateTime.Parse(AppFinish, CultureInfo.GetCultureInfo("en-gb")));
                    task.AppointmentFinishSpecified = true;
                }
                //task.RequiredFSEs = Convert.ToString(RequiredFSE);

                //task.TaskID = res.serviceRequest.srNumber + "_" + res.serviceRequest.activityDetailList[0].activityId;
                task.TaskID = res.serviceRequest.activityDetailList[0].activityId;
                objProcessTaskExRequest.Task = task;
                objProcessTaskExRequest.ReturnSchedulingError = true;
                // DateTime startTime = DateTime.Now;
                var result = await ScheduleService.ProcessTaskExAsync(objProcessTaskExRequest);
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                logger.Debug("CallClickSerrvice.cs;ClickCalltoCreateTask(Create Task):ProcessTaskExAsync TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);
                CloseConnectionSDT();
                if (result.Task.Key > 0)
                {
                    objAppointmentSlots = new AppointmentSlots();
                    objAppointmentSlots.Status = Convert.ToInt32(SDTEnum.OperationStatus.Processed);
                    objAppointmentSlots.TaskID = res.serviceRequest.activityDetailList[0].activityId;
                    List<AppointmentSlots> objTaskList = new List<AppointmentSlots>();
                    objTaskList.Add(objAppointmentSlots);
                    return objTaskList;
                }
                else
                {
                    return objAppSlotsList = new List<AppointmentSlots>();
                }

            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs; ClickCallToCreateTask; Exception: " + ex.Message);
                //DateTime endTime = DateTime.Now;

                //TimeSpan span = endTime.Subtract(startTime);
                //logger.Debug("CallClickSerrvice.cs;ClickCalltoCreateTask(Create Task):ProcessTaskExAsync TimeSpan in Sec ;" + span.Seconds);

                //objAppSlotsList = new List<AppointmentSlots>();
                //return objAppSlotsList;
                throw;
            }
        }
        public async Task<IEnumerable<AppointmentSlots>> ClickCallByOperation(string earlystart, string latestart, int duration, string profile, string preferredFSEs, string fseSkillLevel, bool IsExtendedSlots, string ExtendedSlotsType)
        {
            IEnumerable<AppointmentSlots> objAppSlotsList;
            // AppointmentSlots objAppointmentSlots;
            ExtendedGetAppointmentsEx2Request extendedGetAppointmentsEx2Request = new ExtendedGetAppointmentsEx2Request();
            DateTime startTime = DateTime.Now;
            var SchedulingpolicySlot = "";
            try
            {
                OpenConnectionToSDT();
                task = new ScheduleServiceDev1.Task();
                ScheduleServiceDev1.TimeInterval period = new ScheduleServiceDev1.TimeInterval();
                optparam = new ScheduleServiceDev1.OptionalParametersItem[2];
                opt = new ScheduleServiceDev1.OptionalParameters() { CallerIdentity = "MUST", ExtraData = optparam, ErrorOnNonExistingDictionaries = true };
                DateTime edt;
                DateTime ldt;
                // period.Start=
                var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];
                //if (res.serviceRequest.srType != "Corrective Repair" && res.serviceRequest.gEMSEntitlementFlag != "Y")
                //{
                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                if (string.IsNullOrEmpty(earlystart))
                {
                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                    period.Start = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                }
                else
                {
                    if (res.serviceRequest.srType == "Installation")
                        edt = Convert.ToDateTime(earlystart);
                    else
                        edt = Convert.ToDateTime(DateTime.Parse(earlystart, CultureInfo.GetCultureInfo("en-gb")));
                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                    //period.Start = edt < objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress) ? objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.shipToAddress, res.serviceRequest.shipToAddress) : edt;
                    period.Start = edt < objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress) ? objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress) : edt;

                }
                if (string.IsNullOrEmpty(latestart))
                {
                    period.Finish = period.Start.AddDays(3);
                }
                else
                {
                    if (res.serviceRequest.srType == "Installation")
                        ldt = Convert.ToDateTime(latestart);
                    else
                        ldt = Convert.ToDateTime(DateTime.Parse(latestart, CultureInfo.GetCultureInfo("en-gb")));
                    objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                    period.Finish = ldt < objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress) ? period.Start.AddDays(3) : ldt;
                }
                //}

                TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
                //ClickTranslator clicktranslator = new ClickTranslator();
                //clicktranslator = objSiebelToClick.TransformValues(res);
                var SiebelHttpPostParams = (HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"];
                //task.CallID =  res.serviceRequest.activityDetailList[0].activityId;
                task.CallID = "APAC" + "-" + res.serviceRequest.activityDetailList[0].activityId + "-" + "1";
                //task.CallID = "IE-46723225-1";
                task.Number = 1;// Correct property needs to be mapped.
                task.NumberSpecified = true;
                //Added the below two lines as it was missed during Task Creation
                task.TaskID = res.serviceRequest.activityDetailList[0].activityId;
                //Commented by Rajesh for setting taskid as actvityid as per Joy.
                //task.TaskID = res.serviceRequest.activityDetailList[0].activityId + res.serviceRequest.srNumber;
                // task.JobComments = " ";
                // Need to understand what should be linked for Job comments in Siebel



                task.OpenDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), CultureInfo.GetCultureInfo("en-gb")));

                //objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                task.OpenDateSpecified = true;
                task.Duration = Convert.ToInt32(duration) * 60;
                task.DurationSpecified = true;
                task.SystemID = new ScheduleServiceDev1.GEHCSystemReference { ID = SiebelHttpPostParams.SystemID };//SystemID is mapped with gEHCSystemId
                //SiteID comment by Phani Kanth
                //  task.SiteID = new ScheduleServiceDev1.GEHCSiteReference { ID = SiebelHttpPostParams.ShipToSite };
                if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                {
                    string taskType = string.Empty;
                    taskType = objSiebelToClick.GetTaskType(res.serviceRequest.srType);
                    if (!string.IsNullOrEmpty(taskType))
                        task.TaskType = new ScheduleServiceDev1.TaskTypeReference() { Name = taskType };
                }

                //task.TaskType = new ScheduleServiceDev1.TaskTypeReference() { Name = objSiebelToClick.GetTaskType(res.serviceRequest.srType) };
                // Take the values from the index page radio button
                //string Profile = "ONE HOUR";
                string Schedulingpolicy = "Appointment Booking - No overtime";
                //if (res.serviceRequest.srType == "Corrective Repair" && res.serviceRequest.gEMSEntitlementFlag == "Y" &&
                //    string.IsNullOrEmpty(earlystart) && string.IsNullOrEmpty(latestart))
                if (string.IsNullOrEmpty(earlystart) && string.IsNullOrEmpty(latestart))
                {
                    task.EarlyStartSpecified = false;
                    task.LateStartSpecified = false;
                }
                else
                {
                    if (res.serviceRequest.srType == "Installation")
                    {
                        task.EarlyStart = Convert.ToDateTime(earlystart);
                        task.LateStart = Convert.ToDateTime(latestart);
                        task.IsMDT = true;
                        task.IsMDTSpecified = true;
                        task.EarlyStartSpecified = true;
                        task.LateStartSpecified = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(earlystart))
                        {
                            task.EarlyStart = Convert.ToDateTime(DateTime.Parse(earlystart, CultureInfo.GetCultureInfo("en-gb")));//.ToShortDateString());
                            task.EarlyStartSpecified = true;
                        }

                        if (!string.IsNullOrEmpty(latestart))
                        {
                            task.LateStart = Convert.ToDateTime(DateTime.Parse(latestart, CultureInfo.GetCultureInfo("en-gb")));//.ToShortDateString());
                            task.LateStartSpecified = true;
                        }
                    }

                }


                if (!string.IsNullOrEmpty(preferredFSEs))
                {
                    //task.PreferredFSEs = preferredFSEs;
                    if (HttpContext.Current.Session["IsRequiredFse"].ToString() != null)
                    {
                        if (HttpContext.Current.Session["IsRequiredFse"].ToString() == "true") task.RequiredFSEs = preferredFSEs;
                        else task.RequiredFSEs = "";
                    }
                }

                if (!string.IsNullOrEmpty(fseSkillLevel))
                {
                    task.SkillLevel = Convert.ToInt32(fseSkillLevel);
                    task.SkillLevelSpecified = true;
                }
                //// Below code added to get only 8 slots per appointment request in TPL request
                // Needs to figure out a way to make this all configuration like European Solution
                int NbSlots = Convert.ToInt16(ConfigurationManager.AppSettings["NumberOfSlots"]);
                //Changed to 24 slots since all the slots were not getting returned.
                extendedGetAppointmentsEx2Request.PerformanceParameters = new PerformanceParameters() { NumberOfSlotsToReturn = NbSlots, UseAerialDistance = false };
                extendedGetAppointmentsEx2Request.TimeOut = 180;
                extendedGetAppointmentsEx2Request.SuggestCandidateResources = true;
                extendedGetAppointmentsEx2Request.ExcludeCurrentAppointment = false;
                extendedGetAppointmentsEx2Request.ParallelFactor = 0;
                //extendedGetAppointmentsEx2Request.UseSLRCache = true;
                extendedGetAppointmentsEx2Request.UsePartitionControl = true;
                extendedGetAppointmentsEx2Request.TimeIntervals = null;
                extendedGetAppointmentsEx2Request.GradeAppointments = Convert.ToBoolean(ConfigurationManager.AppSettings["GradeAppointmentsFlag"]);

                // below code has to be validated by Joy ... Scenario of doule booking is not clear
                //extendedGetAppointmentsEx2Request.DoubleBookingTasks = new NewSDTApplication.ScheduleServiceDev1.GroupReference() { Name = "Allow Unschedule Tasks" };
                //extendedGetAppointmentsEx2Request.UnmovableTasks = new NewSDTApplication.ScheduleServiceDev1.GroupReference(){ Name=""}
                if (task.EarlyStartSpecified == false && task.LateStartSpecified == false)
                {
                    var result = await
                          ScheduleService.ExtendedGetAppointmentsEx2Async(opt, task, Schedulingpolicy, null, profile, period, extendedGetAppointmentsEx2Request.ExcludeCurrentAppointment, extendedGetAppointmentsEx2Request.TimeOut,
                                                                          extendedGetAppointmentsEx2Request.GradeAppointments, null, null, null, null,
                                                                         null, "", true, null, true, extendedGetAppointmentsEx2Request.PerformanceParameters,
                                                                         extendedGetAppointmentsEx2Request.ParallelFactor, extendedGetAppointmentsEx2Request.UseSLRCache, extendedGetAppointmentsEx2Request.UsePartitionControl);

                    objAppSlotsList = from res1 in result.OptionalAppointments
                                      select new AppointmentSlots
                                      {
                                          Grade = res1.Grade.ToString(),
                                          EarlyStart = res1.Start.ToString(),
                                          LateStart = res1.Finish.ToString(),
                                          PreferredFSE = res1.SuggestedArrangement[0].Resource.DisplayString,
                                          SSOID = res1.SuggestedArrangement[0].Resource.Key,
                                          SchedulingpolicyExtendedSlots = SchedulingpolicySlot
                                          //travelTime = GetTravelTimeWithKeys(res1.SuggestedArrangement[0].Resource.Key, res1.SuggestedArrangement[0].Task.Key)
                                      };

                    DateTime endTime = DateTime.Now;

                    TimeSpan span = endTime.Subtract(startTime);
                    logger.Debug("CallClickSerrvice.cs;ClcikcallByOperation(Request Appointment):ExtendedGetAppointmentsEx2Async TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);

                    CloseConnectionSDT();
                    return objAppSlotsList;
                }
                else
                {


                    int split = (task.LateStart - period.Start).Days / 3;
                    List<System.Threading.Tasks.Task<ExtendedGetAppointmentsEx2Response>> tasks = new List<System.Threading.Tasks.Task<ExtendedGetAppointmentsEx2Response>>();
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    CancellationToken token = cancellationTokenSource.Token;
                    List<AppointmentSlots> appointmentSlots = new List<AppointmentSlots>();
                    if (split > 9) split = 9;
                    DateTime splitperiodstart;
                    splitperiodstart = period.Start;
                    //int reccount = 0;
                    for (int i = 0; i <= split; i++)
                    {
                        if (splitperiodstart == task.LateStart)
                            break;
                        NewSDTApplication.ScheduleServiceDev1.TimeInterval periodSplit = new NewSDTApplication.ScheduleServiceDev1.TimeInterval();
                        periodSplit.Start = splitperiodstart;
                        if (splitperiodstart.AddDays(3) <= task.LateStart)
                            periodSplit.Finish = splitperiodstart.AddDays(3);
                        else
                            periodSplit.Finish = task.LateStart;

                        NewSDTApplication.ScheduleServiceDev1.GroupReference GroupReference1 = new NewSDTApplication.ScheduleServiceDev1.GroupReference();
                        if (IsExtendedSlots)
                        {
                            if (HttpContext.Current.Session["IsBump"] != null && Convert.ToBoolean(HttpContext.Current.Session["IsBump"]) == true)
                            {

                                GroupReference1.Name = "Allow Unschedule Tasks APAC";
                                //extendedGetAppointmentsEx2Request.DoubleBookingTasks = new NewSDTApplication.ScheduleServiceDev1.GroupReference() { Name = "Allow Unschedule Tasks APAC" };
                                //extendedGetAppointmentsEx2Request.DoubleBookingTasks.KeySpecified = true;

                            }

                            Schedulingpolicy = "Appointment Booking";
                            //profile = "ONE HOUR";
                            //task.UseDistrictCalendar = true;
                            //task.UseDistrictCalendarSpecified = true;
                            if (ExtendedSlotsType == SDTEnum.ExtendedSlotsType.ExtendedSlotsWithOneHour.ToString())
                            {
                                task.SchedulingPolicy = ExtendedWithOneHour;

                                SchedulingpolicySlot = ExtendedWithOneHour;
                            }
                            else if (ExtendedSlotsType == SDTEnum.ExtendedSlotsType.ExtendedSlotsWithTwoHours.ToString())
                            {
                                task.SchedulingPolicy = ExtendedWithTwoHours;
                                SchedulingpolicySlot = ExtendedWithTwoHours;
                            }
                        }
                        if (HttpContext.Current.Session["IsBump"] != null && Convert.ToBoolean(HttpContext.Current.Session["IsBump"]) == true && IsExtendedSlots == true)
                        {
                            var taskRun =
                             ScheduleService.ExtendedGetAppointmentsEx2Async(opt, task, Schedulingpolicy, null, profile, periodSplit, extendedGetAppointmentsEx2Request.ExcludeCurrentAppointment, extendedGetAppointmentsEx2Request.TimeOut,
                                                                             extendedGetAppointmentsEx2Request.GradeAppointments, null, GroupReference1, null, null,
                                                                            null, "", true, null, true, extendedGetAppointmentsEx2Request.PerformanceParameters,
                                                                            extendedGetAppointmentsEx2Request.ParallelFactor, extendedGetAppointmentsEx2Request.UseSLRCache, extendedGetAppointmentsEx2Request.UsePartitionControl);
                            tasks.Add(taskRun);

                        }
                        else
                        {
                            var taskRun =
                            ScheduleService.ExtendedGetAppointmentsEx2Async(opt, task, Schedulingpolicy, null, profile, periodSplit, extendedGetAppointmentsEx2Request.ExcludeCurrentAppointment, extendedGetAppointmentsEx2Request.TimeOut,
                                                                            extendedGetAppointmentsEx2Request.GradeAppointments, null, null, null, null,
                                                                           null, "", true, null, true, extendedGetAppointmentsEx2Request.PerformanceParameters,
                                                                           extendedGetAppointmentsEx2Request.ParallelFactor, extendedGetAppointmentsEx2Request.UseSLRCache, extendedGetAppointmentsEx2Request.UsePartitionControl);
                            tasks.Add(taskRun);
                        }




                        splitperiodstart = periodSplit.Finish;
                        DateTime endTime = DateTime.Now;

                        TimeSpan span = endTime.Subtract(startTime);
                        logger.Debug("CallClickSerrvice.cs;ClcikcallByOperation(Request Appointment):ExtendedGetAppointmentsEx2Async TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);

                    }
                    System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                    foreach (var t in tasks)
                    {


                        appointmentSlots.AddRange(from res1 in t.Result.OptionalAppointments
                                                  select new AppointmentSlots
                                                  {
                                                      Grade = res1.Grade.ToString(),
                                                      EarlyStart = res1.Start.ToString(),
                                                      LateStart = res1.Finish.ToString(),
                                                      PreferredFSE = res1.SuggestedArrangement[0].Resource.DisplayString,
                                                      SSOID = res1.SuggestedArrangement[0].Resource.Key,
                                                      SchedulingpolicyExtendedSlots = SchedulingpolicySlot
                                                      //travelTime = GetTravelTimeWithKeys(res1.SuggestedArrangement[0].Resource.Key, res1.SuggestedArrangement[0].Task.Key)
                                                  });
                    }
                    objAppSlotsList = appointmentSlots;
                    //  long traveltime = GetTravelTimeWithKeys(1, 2);

                    //End of Joy Code
                    CloseConnectionSDT();
                    return objAppSlotsList;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;ClickCallByOperation; Exception: " + ex.Message);
                //DateTime endTime = DateTime.Now;

                //TimeSpan span = endTime.Subtract(startTime);
                //logger.Debug("CallClickSerrvice.cs;ClcikcallByOperation(Request Appointment):ExtendedGetAppointmentsEx2Async TimeSpan in Sec ;" + span.Seconds);

                objAppSlotsList = new List<AppointmentSlots>();
                return objAppSlotsList;
            }
        }

        public bool ModifyVisit(string EarlyStart, string LateStart, int Duration, int Number, string TaskNotes, string fseSkills, string PreferredFSEs)
        {
            logger.Debug("CallClickSerrvice.cs;ModifyVisit; SessionTest: Task StartDate" + EarlyStart + "Duration" + Duration);

            TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
            DateTime startTime = DateTime.Now;
            try
            {
                if (HttpContext.Current.Session["TaskResponseByTask"] != null)
                {
                    OpenConnectionToSDT();
                    ProcessTaskExRequest processTaskExRequest = new ProcessTaskExRequest();
                    ProcessTaskExResponse processTaskExResponse = new ProcessTaskExResponse();
                    processTaskExRequest.OptionalParameters = new ScheduleServiceDev1.OptionalParameters() { CallerIdentity = "MUST", ErrorOnNonExistingDictionaries = true };
                    //processTaskExRequest.OptionalParameters = new SDT.Schedule.Web.Services.ScheduleService.OptionalParameters() { CallerIdentity = "MUST" };
                    processTaskExRequest.TaskRequestedProperties = new string[]
                    {
                        "Key",
                        "KeySpecified",
                        "Priority",
                        "PrioritySpecified",
                        "Duration",
                        "DurationSpecified"
                    };

                    GetTasksResponse taskResponse = new GetTasksResponse();

                    task = new ScheduleServiceDev1.Task();
                    taskResponse = (ScheduleServiceDev1.GetTasksResponse)HttpContext.Current.Session["TaskResponseByTask"];
                    task = taskResponse.Tasks[0];

                    //task = new ScheduleServiceDev1.Task();
                    processTaskExRequest.Task = new ScheduleServiceDev1.Task();
                    processTaskExRequest.Task.Key = task.Key;
                    processTaskExRequest.Task.KeySpecified = task.KeySpecified;
                    //processTaskExRequest.Task.Number = Number;
                    //processTaskExRequest.Task.Priority = task.Priority;
                    //processTaskExRequest.Task.PrioritySpecified = task.PrioritySpecified;
                    //processTaskExRequest.Task.Duration = task.Duration;
                    //processTaskExRequest.Task.DurationSpecified = task.DurationSpecified;
                    var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];
                    if (res.serviceRequest.srType == "Corrective Repair" && res.serviceRequest.gEMSEntitlementFlag == "Y"
                        && string.IsNullOrEmpty(EarlyStart) && string.IsNullOrEmpty(LateStart))
                    {
                        processTaskExRequest.Task.EarlyStartSpecified = false;
                        processTaskExRequest.Task.LateStartSpecified = false;
                    }
                    else
                    {
                        if (res.serviceRequest.srType == "Installation")
                        {
                            processTaskExRequest.Task.EarlyStart = Convert.ToDateTime(EarlyStart);//.ToShortDateString());
                            processTaskExRequest.Task.LateStart = Convert.ToDateTime(LateStart);//.ToShortDateString());
                            processTaskExRequest.Task.EarlyStartSpecified = true;
                            processTaskExRequest.Task.LateStartSpecified = true;
                            //Commented by Phani Kanth P
                            //processTaskExRequest.Task.CustomerExpectation = "B";

                        }

                        else
                        {
                            processTaskExRequest.Task.EarlyStart = Convert.ToDateTime(DateTime.Parse(EarlyStart, CultureInfo.GetCultureInfo("en-gb")));//.ToShortDateString());
                            processTaskExRequest.Task.EarlyStartSpecified = true;
                            processTaskExRequest.Task.LateStart = Convert.ToDateTime(DateTime.Parse(LateStart, CultureInfo.GetCultureInfo("en-gb")));//.ToShortDateString());
                            processTaskExRequest.Task.LateStartSpecified = true;

                        }
                    }
                    //CustomerExpectation   "B" added by phani kanth p
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["Schedulingpolicy"] as string))
                    {
                        processTaskExRequest.Task.SchedulingPolicy = Convert.ToString(HttpContext.Current.Session["Schedulingpolicy"]);
                    }


                    processTaskExRequest.Task.CustomerExpectation = "B";
                    task.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;
                    task.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;

                    // ContactName,ContactPhoneNumber,CustomerEmail added by phani [9/9/2016]

                    processTaskExRequest.Task.ContactName = res.serviceRequest.contactFirstName + " " + res.serviceRequest.contactLastName;
                    processTaskExRequest.Task.ContactPhoneNumber = res.serviceRequest.gEHCWorkPhone;
                    processTaskExRequest.Task.CustomerEmail = res.serviceRequest.contactEmail;


                    processTaskExRequest.Task.OwnerName = task.OwnerName;
                    processTaskExRequest.Task.OwnerSSO = task.OwnerSSO;
                    if (res.serviceRequest.srType == "FMI")
                    {
                        if (!string.IsNullOrEmpty(res.serviceRequest.fmiDueDate))
                        {
                            processTaskExRequest.Task.FMIDueDate = Convert.ToDateTime(DateTime.Parse(res.serviceRequest.fmiDueDate, CultureInfo.GetCultureInfo("en-gb")));
                            processTaskExRequest.Task.FMIDueDateSpecified = true;
                        }
                        processTaskExRequest.Task.FMINumber = res.serviceRequest.fMINumber;

                    }


                    processTaskExRequest.Task.Priority = GetPriority();
                    processTaskExRequest.Task.PrioritySpecified = true;
                    HttpContext.Current.Session["systemstatus"] = null;
                    processTaskExRequest.Task.Duration = Duration * 60;
                    processTaskExRequest.Task.DurationSpecified = true;
                    processTaskExRequest.Task.Notes = TaskNotes;
                    if (!string.IsNullOrEmpty(fseSkills))
                    {
                        processTaskExRequest.Task.SkillLevel = Convert.ToInt32(fseSkills);
                        processTaskExRequest.Task.SkillLevelSpecified = true;
                    }

                    //if (!string.IsNullOrEmpty(PreferredFSEs))
                    //{
                    //    //if (HttpContext.Current.Session["IsRequiredFse"].ToString() != null)
                    //    //{
                    //    //    if (HttpContext.Current.Session["IsRequiredFse"].ToString() == "true")
                    //    //        processTaskExRequest.Task.RequiredFSEs = PreferredFSEs;
                    //    //    else processTaskExRequest.Task.PreferredFSEs = PreferredFSEs;
                    //    //}
                    //    processTaskExRequest.Task.PreferredFSEs = PreferredFSEs;
                    //}

                    //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                    string systemstatus = objSiebelToClick.GetSystemStatus(res.serviceRequest.currentEquiptmentStatus);

                    if (!string.IsNullOrEmpty(res.serviceRequest.currentEquiptmentStatus))
                        processTaskExRequest.Task.SystemStatus = new ScheduleServiceDev1.GEHCSystemStatusReference { Name = systemstatus, DisplayString = systemstatus };


                    if (!string.IsNullOrEmpty(PreferredFSEs))
                    {
                        processTaskExRequest.Task.PreferredFSEs = PreferredFSEs;
                    }
                    else
                    {
                        processTaskExRequest.Task.PreferredFSEs = "";
                    }

                    if (!string.IsNullOrEmpty(PreferredFSEs))
                    {
                        if (HttpContext.Current.Session["IsRequiredFse"].ToString() != null)
                        {
                            if (HttpContext.Current.Session["IsRequiredFse"].ToString() == "true")
                                processTaskExRequest.Task.RequiredFSEs = PreferredFSEs;
                            else processTaskExRequest.Task.RequiredFSEs = "";
                        }
                        //processTaskExRequest.Task.PreferredFSEs = PreferredFSEs;
                    }
                    else
                    {
                        processTaskExRequest.Task.RequiredFSEs = "";
                        processTaskExRequest.Task.PreferredFSEs = "";
                    }
                    //else{
                    //    if (HttpContext.Current.Session["IsRequiredFse"].ToString() == "true")
                    //        processTaskExRequest.Task.RequiredFSEs = "";
                    //    else processTaskExRequest.Task.PreferredFSEs = "";
                    //}
                    //processTaskExRequest.Task.JobComments = new ScheduleServiceDev1.GEHCJobCommentsReference() { DisplayString = TaskNotes };

                    if ((res != null) && (res.serviceRequest != null) && (!string.IsNullOrEmpty(res.serviceRequest.gEHCSafetyConcern)))
                    {
                        processTaskExRequest.Task.IsSafety = objSiebelToClick.GetSafetyConcernValue();
                        processTaskExRequest.Task.IsSafetySpecified = true;

                    }
                    ScheduleService.ProcessTaskEx(processTaskExRequest.OptionalParameters,
                    ref processTaskExRequest.Task, ref processTaskExRequest.Assignment, processTaskExRequest.ReturnAssignment,
                    processTaskExRequest.RelatedTasks, processTaskExRequest.SchedulingWorkflow, processTaskExRequest.LogicDomain,
                    processTaskExRequest.SchedulingHorizon, processTaskExRequest.TaskRequestedProperties, processTaskExRequest.AssignmentRequestedProperties,
                    processTaskExRequest.ReturnSchedulingError, out processTaskExResponse.LinkedTasks, out processTaskExResponse.SchedulingError);
                    DateTime endTime = DateTime.Now;

                    TimeSpan span = endTime.Subtract(startTime);
                    logger.Debug("CallClickSerrvice.cs;CreateTaskWithDependencies:ExecuteMultipleOperations TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                //DateTime endTime = DateTime.Now;

                //TimeSpan span = endTime.Subtract(startTime);
                //logger.Debug("CallClickSerrvice.cs;CreateTaskWithDependencies:ExecuteMultipleOperations TimeSpan in Sec ;" + span.Seconds);

                logger.Error("CallClickSerrvice.cs;ModifyVisit; Exception: " + ex.Message);
                CloseConnectionSDT();
                throw;
            }
        }


        public bool CancelTask(string CancelReason)
        {
            string taskStatus = string.Empty;
            string taskStatusvalue = string.Empty;

            try
            {
                if (HttpContext.Current.Session["TaskResponseByTask"] != null)
                {
                    if (HttpContext.Current.Session["taskStatusvalue"] != null)
                        taskStatusvalue = HttpContext.Current.Session["taskStatusvalue"].ToString();

                    if (taskStatusvalue == SDTEnum.TaskStatusValue.New.ToString() || taskStatusvalue == SDTEnum.TaskStatusValue.Rejected.ToString() || taskStatusvalue == "Rejected by FSE" || taskStatusvalue == SDTEnum.TaskStatusValue.Tentative.ToString())
                        taskStatus = "Cancelled";
                    else if (taskStatusvalue == SDTEnum.TaskStatusValue.Acknowledged.ToString() || taskStatusvalue == SDTEnum.TaskStatusValue.Assigned.ToString())
                        taskStatus = "Rejected";

                    OpenConnectionToSDT();
                    GetTasksResponse taskResponse = new GetTasksResponse();
                    ProcessTaskExRequest processTaskExRequest = new ProcessTaskExRequest();
                    ProcessTaskExResponse processTaskExResponse = new ProcessTaskExResponse();
                    processTaskExRequest.OptionalParameters = new ScheduleServiceDev1.OptionalParameters() { CallerIdentity = "MUST" };
                    processTaskExRequest.TaskRequestedProperties = new string[] { "Key", "KeySpecified", "CallID", "Number", "TaskID", "MUSTJobNumber", "TimeDependencies", "EngineerDependencies", "NumberSpecified", "OwnerName", "OwnerSSO", "Status", "CancellationReason" };
                    task = new ScheduleServiceDev1.Task();
                    taskResponse = (ScheduleServiceDev1.GetTasksResponse)HttpContext.Current.Session["TaskResponseByTask"];
                    task = taskResponse.Tasks[0];
                    processTaskExRequest.Task = new ScheduleServiceDev1.Task();
                    processTaskExRequest.Task.Key = task.Key;
                    processTaskExRequest.Task.KeySpecified = true;
                    //Removed as per the discussion with Joy on 06/04/2016- Rajesh
                    //processTaskExRequest.Task.CallID = task.CallID;
                    //processTaskExRequest.Task.TimeDependencies = new List<NewSDTApplication.ScheduleServiceDev1.TaskTaskTimeDependency>() { }.ToArray();
                    //processTaskExRequest.Task.EngineerDependencies = new List<NewSDTApplication.ScheduleServiceDev1.TaskTaskEngineerDependency>() { }.ToArray();
                    // End of Comment--Removed as per the discussion with Joy on 06/04/2016- Rajesh
                    // processTaskExRequest.Task.Number = task.Number;
                    // processTaskExRequest.Task.NumberSpecified = true;
                    processTaskExRequest.Task.OwnerName = task.OwnerName;
                    processTaskExRequest.Task.OwnerSSO = task.OwnerSSO;
                    if (((task.Status.Name != null ? task.Status.Name : task.Status.DisplayString != null ? task.Status.DisplayString : "New") != "New") && ((task.Status.Name != null ? task.Status.Name : task.Status.DisplayString != null ? task.Status.DisplayString : "Tentative") != "Tentative"))
                    { }
                    ScheduleServiceDev1.GEHCCancellationReasonReference objCancellation = new NewSDTApplication.ScheduleServiceDev1.GEHCCancellationReasonReference() { Name = CancelReason };


                    ScheduleServiceDev1.GEHCRejectionReasonReference objRejectionReason = new NewSDTApplication.ScheduleServiceDev1.GEHCRejectionReasonReference() { Name = "Other" };

                    ScheduleServiceDev1.GEHCIncompleteReasonReference objIncompleteReason = new NewSDTApplication.ScheduleServiceDev1.GEHCIncompleteReasonReference() { Name = CancelReason };

                    processTaskExRequest.Task.Status = new ScheduleServiceDev1.TaskStatusReference() { Name = taskStatus };

                    bool status = false;
                    //string cancelStatus = string.Empty;
                    if (taskStatusvalue == "Acknowledged" || taskStatusvalue == "Assigned")
                    {
                        processTaskExRequest.Task.RejectionReason = objRejectionReason;
                        processTaskExRequest.Task.RejectionComment = CancelReason;
                        DateTime startTime = DateTime.Now;
                        var cancelStatus = ScheduleService.ProcessTaskEx(processTaskExRequest.OptionalParameters, ref processTaskExRequest.Task, ref processTaskExRequest.Assignment, processTaskExRequest.ReturnAssignment,
                        processTaskExRequest.RelatedTasks, processTaskExRequest.SchedulingWorkflow, processTaskExRequest.LogicDomain,
                        processTaskExRequest.SchedulingHorizon, processTaskExRequest.TaskRequestedProperties, processTaskExRequest.AssignmentRequestedProperties,
                        processTaskExRequest.ReturnSchedulingError, out processTaskExResponse.LinkedTasks, out processTaskExResponse.SchedulingError);
                        status = true;
                        DateTime endTime = DateTime.Now;

                        TimeSpan span = endTime.Subtract(startTime);
                        logger.Debug("CallClickSerrvice.cs;CancelTask:ProcessTaskEx TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);
                        logger.Debug("CallClickSerrvice.cs;CancelTask; Rejected Status set Successfully");
                        processTaskExRequest.Task.Status = null;
                        processTaskExRequest.Task.RejectionReason = null;
                        processTaskExRequest.Task.RejectionComment = string.Empty;
                        processTaskExRequest.Task.CancellationReason = objCancellation;
                        processTaskExRequest.Task.Status = new ScheduleServiceDev1.TaskStatusReference() { Name = "Cancelled" };
                        DateTime startTime1 = DateTime.Now;
                        ScheduleService.ProcessTaskEx(processTaskExRequest.OptionalParameters, ref processTaskExRequest.Task, ref processTaskExRequest.Assignment, processTaskExRequest.ReturnAssignment,
                    processTaskExRequest.RelatedTasks, processTaskExRequest.SchedulingWorkflow, processTaskExRequest.LogicDomain,
                    processTaskExRequest.SchedulingHorizon, processTaskExRequest.TaskRequestedProperties, processTaskExRequest.AssignmentRequestedProperties,
                    processTaskExRequest.ReturnSchedulingError, out processTaskExResponse.LinkedTasks, out processTaskExResponse.SchedulingError);
                        CloseConnectionSDT();
                        DateTime endTime1 = DateTime.Now;

                        TimeSpan span1 = endTime1.Subtract(startTime1);
                        logger.Debug("CallClickSerrvice.cs;CancelTask:ProcessTaskEx TimeSpan in Sec ;" + span1.Minutes * 60 + span1.Seconds);
                        logger.Debug("CallClickSerrvice.cs;Cancel status set successfully");

                        return true;
                    }
                    else
                    {
                        DateTime startTime2 = DateTime.Now;
                        processTaskExRequest.Task.CancellationReason = objCancellation;

                        ScheduleService.ProcessTaskEx(processTaskExRequest.OptionalParameters, ref processTaskExRequest.Task, ref processTaskExRequest.Assignment, processTaskExRequest.ReturnAssignment,
                    processTaskExRequest.RelatedTasks, processTaskExRequest.SchedulingWorkflow, processTaskExRequest.LogicDomain,
                    processTaskExRequest.SchedulingHorizon, processTaskExRequest.TaskRequestedProperties, processTaskExRequest.AssignmentRequestedProperties,
                    processTaskExRequest.ReturnSchedulingError, out processTaskExResponse.LinkedTasks, out processTaskExResponse.SchedulingError);
                        CloseConnectionSDT();
                        DateTime endTime2 = DateTime.Now;

                        TimeSpan span2 = endTime2.Subtract(startTime2);
                        logger.Debug("CallClickSerrvice.cs;CancelTask:ProcessTaskEx TimeSpan in Sec ;" + span2.Minutes * 60 + span2.Seconds);
                        logger.Debug("CallClickSerrvice.cs;CancelTask; End - True");

                        return true;
                    }


                    //processTaskExRequest=processTaskRequest;
                    // await ScheduleService.ProcessTaskAsync(processTaskExRequest);
                    //ScheduleService.ProcessTaskEx(processTaskExRequest.OptionalParameters, ref processTaskExRequest.Task, ref processTaskExRequest.Assignment, processTaskExRequest.ReturnAssignment,
                    //processTaskExRequest.RelatedTasks, processTaskExRequest.SchedulingWorkflow, processTaskExRequest.LogicDomain,
                    //processTaskExRequest.SchedulingHorizon, processTaskExRequest.TaskRequestedProperties, processTaskExRequest.AssignmentRequestedProperties,
                    //processTaskExRequest.ReturnSchedulingError, out processTaskExResponse.LinkedTasks, out processTaskExResponse.SchedulingError);
                    //CloseConnectionSDT();
                    //logger.Debug("CallClickSerrvice.cs;CancelTask; End - True");

                    //return true;


                }
                else
                {
                    logger.Debug("CallClickSerrvice.cs;CancelTask; End - True");

                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;CancelTask; Exception: " + ex.Message);

                //Logger.Error(ex.Message);
                //SDT.Booking.Data.Library.Utility.ShowMessageBox(SDT.Booking.Data.Library.Utility.Translate("BKG016"), SDT.Booking.Data.Library.Utility.Translate("Attention"), SDT.Booking.Data.Library.Utility.Translate("ReturnedMessageFromClick") + ex.Message, SDT.Booking.Data.Library.Utility.Translate("TryToRepeateLaterPleaseTryLater"), TaskDialogIcon.Warning, TaskDialogButton.Ok, TaskDialogResult.Ok);
                //SDT.Booking.Data.Library.Utility.Debug(Log.Error, "", LogStatus.Step, ex);
                CloseConnectionSDT();
                throw;
            }
        }


        public bool IsTaskCancellable(ScheduleServiceDev1.GetTasksResponse objResponse)
        {
            logger.Debug("CallClickSerrvice.cs;IsTaskCancellable; Start");

            optparam = new ScheduleServiceDev1.OptionalParametersItem[2];
            opt = new ScheduleServiceDev1.OptionalParameters() { CallerIdentity = "MUST", ExtraData = optparam };
            NewSDTApplication.ScheduleServiceDev1.ObjectReference Status = new NewSDTApplication.ScheduleServiceDev1.ObjectReference();
            Status.Key = objResponse.Tasks[0].Status.Key;
            Status.KeySpecified = true;
            bool lockonStart = false;
            bool lockonfinish = false;
            bool IsScheduled = objResponse.Tasks[0].IsScheduled;
            string user = "ALL USERS";
            OpenConnectionToSDT();
            NewSDTApplication.ScheduleServiceDev1.ObjectReference[] objReference = ScheduleService.GetNextStatusTransition(opt, ref user, eMobileType.SCHEDULER_CLIENT, Status, IsScheduled, out lockonStart, out lockonfinish);
            logger.Debug("CallClickSerrvice.cs;IsTaskCancellable; End");

            return true;
        }
        public ServiceOptimizationService.GEHCSystem GetSystemDetails(string SystemId)
        {
            //SystemId = "0910553087AW1";
            OpenConnectionToSDT();
            try
            {
                if (string.IsNullOrEmpty(SystemId)) return null;
                NewSDTApplication.ServiceOptimizationService.GetResourcesRequest getResourcesRequest = new NewSDTApplication.ServiceOptimizationService.GetResourcesRequest();
                getResourcesRequest.OptionalParameters = new NewSDTApplication.ServiceOptimizationService.OptionalParameters() { CallerIdentity = "MUST", ErrorOnNonExistingDictionaries = true };
                List<NewSDTApplication.ServiceOptimizationService.IndexesIndex> indexesIndexs = new List<NewSDTApplication.ServiceOptimizationService.IndexesIndex>();
                NewSDTApplication.ServiceOptimizationService.IndexesIndex indexesIndex = new NewSDTApplication.ServiceOptimizationService.IndexesIndex();
                indexesIndex.LowBound = new NewSDTApplication.ServiceOptimizationService.Property[1] { new NewSDTApplication.ServiceOptimizationService.Property() { Name = "ID", Value = new NewSDTApplication.ServiceOptimizationService.KeyValue() { Value = SystemId } } };
                indexesIndex.HighBound = indexesIndex.LowBound;
                indexesIndexs.Add(indexesIndex);
                getResourcesRequest.Indexes = new NewSDTApplication.ServiceOptimizationService.Indexes() { Index = indexesIndexs.ToArray() };
                NewSDTApplication.ServiceOptimizationService.BaseObject[] obj = new NewSDTApplication.ServiceOptimizationService.BaseObject[] { };
                obj = ServiceOptimizationService.GetObjects(getResourcesRequest.OptionalParameters, "GEHCSystem", getResourcesRequest.Indexes, getResourcesRequest.Group, getResourcesRequest.EnableGroupOnTheFly, getResourcesRequest.KeySet, getResourcesRequest.RequestedProperties);
                if (obj.Count() > 0)
                {
                    //CloseConnectionSDT();
                    return ((NewSDTApplication.ServiceOptimizationService.GEHCSystem)(obj.First()));
                }
                // CloseConnectionSDT();
                return new ServiceOptimizationService.GEHCSystem();
            }
            catch (FaultException ex)
            {
                // Implement Logger correctly
                logger.Error("CallClickSerrvice.cs;GetSystemDetails; Exception: " + ex.Message);
                // CloseConnectionSDT();
                return null;
            }
            catch (Exception ex)
            {
                // Implement Logger correctly
                logger.Error("CallClickSerrvice.cs;GetSystemDetails; Exception: " + ex.Message);
                //CloseConnectionSDT();
                return null;
            }
        }
        public ServiceOptimizationService.Engineer[] GetResources(string name, string value)
        {

            // OpenConnectionToSDT();
            ServiceOptimizationService.Engineer[] engineers = new ServiceOptimizationService.Engineer[] { };
            try
            {
                ServiceOptimizationService.GetResourcesRequest getResourcesRequest = new ServiceOptimizationService.GetResourcesRequest();
                getResourcesRequest.OptionalParameters = new ServiceOptimizationService.OptionalParameters() { CallerIdentity = "MUST", ErrorOnNonExistingDictionaries = true };
                List<ServiceOptimizationService.IndexesIndex> indexesIndexs = new List<ServiceOptimizationService.IndexesIndex>();
                ServiceOptimizationService.IndexesIndex indexesIndex = new ServiceOptimizationService.IndexesIndex();
                indexesIndex.LowBound = new ServiceOptimizationService.Property[1] { new ServiceOptimizationService.Property() { Name = name, Value = new ServiceOptimizationService.KeyValue() { Value = value } } };

                indexesIndex.HighBound = indexesIndex.LowBound;
                indexesIndexs.Add(indexesIndex);
                getResourcesRequest.Indexes = new ServiceOptimizationService.Indexes() { Index = indexesIndexs.ToArray() };
                engineers = ServiceOptimizationService.GetResources(getResourcesRequest.OptionalParameters, getResourcesRequest.Indexes, getResourcesRequest.Group, getResourcesRequest.EnableGroupOnTheFly, getResourcesRequest.KeySet, getResourcesRequest.RequestedProperties);
                //CloseConnectionSDT();

                return engineers;
            }
            catch (FaultException ex)
            {
                logger.Error("CallClickSerrvice.cs;GetResources; Exception: " + ex.Message);
                return new ServiceOptimizationService.Engineer[] { };

            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;GetResources; Exception: " + ex.Message);
                return new ServiceOptimizationService.Engineer[] { };

            }
            finally
            {
                // CloseConnectionSDT();
            }
        }
        public ServiceOptimizationService.GEHCSite GetSiteDetails(string SiteID)
        {
            //SystemId = "0910553087AW1";
            //SiteID = "GB-00107";
            OpenConnectionToSDT();
            try
            {
                if (string.IsNullOrEmpty(SiteID)) return null;
                NewSDTApplication.ServiceOptimizationService.GetResourcesRequest getResourcesRequest = new NewSDTApplication.ServiceOptimizationService.GetResourcesRequest();
                getResourcesRequest.OptionalParameters = new NewSDTApplication.ServiceOptimizationService.OptionalParameters() { CallerIdentity = "MUST", ErrorOnNonExistingDictionaries = true };
                List<NewSDTApplication.ServiceOptimizationService.IndexesIndex> indexesIndexs = new List<NewSDTApplication.ServiceOptimizationService.IndexesIndex>();
                NewSDTApplication.ServiceOptimizationService.IndexesIndex indexesIndex = new NewSDTApplication.ServiceOptimizationService.IndexesIndex();
                indexesIndex.LowBound = new NewSDTApplication.ServiceOptimizationService.Property[1] { new NewSDTApplication.ServiceOptimizationService.Property() { Name = "ID", Value = new NewSDTApplication.ServiceOptimizationService.KeyValue() { Value = SiteID } } };
                indexesIndex.HighBound = indexesIndex.LowBound;
                indexesIndexs.Add(indexesIndex);
                getResourcesRequest.Indexes = new NewSDTApplication.ServiceOptimizationService.Indexes() { Index = indexesIndexs.ToArray() };
                NewSDTApplication.ServiceOptimizationService.BaseObject[] obj = new NewSDTApplication.ServiceOptimizationService.BaseObject[] { };
                obj = ServiceOptimizationService.GetObjects(getResourcesRequest.OptionalParameters, "GEHCSite", getResourcesRequest.Indexes, getResourcesRequest.Group, getResourcesRequest.EnableGroupOnTheFly, getResourcesRequest.KeySet, getResourcesRequest.RequestedProperties);
                if (obj.Count() > 0)
                {
                    //CloseConnectionSDT();
                    return ((NewSDTApplication.ServiceOptimizationService.GEHCSite)(obj.First()));
                }
                // CloseConnectionSDT();
                return new ServiceOptimizationService.GEHCSite();
            }
            catch (FaultException ex)
            {
                // Implement Logger correctly
                logger.Error("CallClickSerrvice.cs;GetSiteDetails; Exception: " + ex.Message);
                // CloseConnectionSDT();
                throw ex;
                return null;
            }
            catch (Exception ex)
            {
                // Implement Logger correctly
                logger.Error("CallClickSerrvice.cs;GetSiteDetails; Exception: " + ex.Message);
                // CloseConnectionSDT();
                throw ex;
                return null;
            }
        }

        public ServiceOptimizationService.GEHCSystem GetDateDetails(string SystemID)
        {
            //SystemId = "0910553087AW1";
            //SiteID = "GB-00107";
            OpenConnectionToSDT();
            try
            {
                if (string.IsNullOrEmpty(SystemID)) return null;
                NewSDTApplication.ServiceOptimizationService.GetResourcesRequest getResourcesRequest = new NewSDTApplication.ServiceOptimizationService.GetResourcesRequest();
                getResourcesRequest.OptionalParameters = new NewSDTApplication.ServiceOptimizationService.OptionalParameters() { CallerIdentity = "MUST", ErrorOnNonExistingDictionaries = true };
                List<NewSDTApplication.ServiceOptimizationService.IndexesIndex> indexesIndexs = new List<NewSDTApplication.ServiceOptimizationService.IndexesIndex>();
                NewSDTApplication.ServiceOptimizationService.IndexesIndex indexesIndex = new NewSDTApplication.ServiceOptimizationService.IndexesIndex();
                indexesIndex.LowBound = new NewSDTApplication.ServiceOptimizationService.Property[1] { new NewSDTApplication.ServiceOptimizationService.Property() { Name = "ID", Value = new NewSDTApplication.ServiceOptimizationService.KeyValue() { Value = SystemID } } };
                indexesIndex.HighBound = indexesIndex.LowBound;
                indexesIndexs.Add(indexesIndex);
                getResourcesRequest.Indexes = new NewSDTApplication.ServiceOptimizationService.Indexes() { Index = indexesIndexs.ToArray() };
                NewSDTApplication.ServiceOptimizationService.BaseObject[] obj = new NewSDTApplication.ServiceOptimizationService.BaseObject[] { };
                obj = ServiceOptimizationService.GetObjects(getResourcesRequest.OptionalParameters, "GEHCSystem", getResourcesRequest.Indexes, getResourcesRequest.Group, getResourcesRequest.EnableGroupOnTheFly, getResourcesRequest.KeySet, getResourcesRequest.RequestedProperties);
                if (obj.Count() > 0)
                {
                    //CloseConnectionSDT();
                    return ((NewSDTApplication.ServiceOptimizationService.GEHCSystem)(obj.First()));
                }
                // CloseConnectionSDT();
                return new ServiceOptimizationService.GEHCSystem();
            }
            catch (FaultException ex)
            {
                // Implement Logger correctly
                logger.Error("CallClickSerrvice.cs;GetSiteDetails; Exception: " + ex.Message);
                // CloseConnectionSDT();
                return null;
            }
            catch (Exception ex)
            {
                // Implement Logger correctly
                logger.Error("CallClickSerrvice.cs;GetSiteDetails; Exception: " + ex.Message);
                // CloseConnectionSDT();
                return null;
            }
        }

        public async Task<int> getSiebelTaskNumber(string CallId)
        {
            CallClickSerrvice objClickCallService = new CallClickSerrvice();
            var TaskNumber = 0;
            var resTasksNumber = await objClickCallService.GetTasksRequestByPropertyName(CallId, "TaskActivityID");
            var DependencyTaskNumber = resTasksNumber.Tasks.Where(x => x.TaskType.DisplayString != "Parts Pickup").OrderByDescending(a => a.Number).FirstOrDefault();
            TaskNumber = DependencyTaskNumber.Number + 1;
            return TaskNumber;
        }

        //public string CreateTaskWithDependencies(List<CustomTasksList> TaskData)
        //Change String to async added by Phani Kanth P.
        public async Task<string> CreateTaskWithDependencies(List<CustomTasksList> TaskData)
        {
            TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
            string CallID = string.Empty;
            int TaskNumber = 0;
            int Duration = 0;
            DateTime startTime = DateTime.Now;
            string MustJobNumber = string.Empty;
            if (TaskData.Count > 0)
            {
                CallID = TaskData[0].CallID;
                TaskNumber = Convert.ToInt32(TaskData[0].TaskNumber);
                Duration = Convert.ToInt32(TaskData[0].Duration);
                MustJobNumber = TaskData[0].MUSTJobNumber;
            }


            //int requestCount = FinalRes.Count();
            OpenConnectionToSDT();
            ServiceOptimizationService.ExecuteMultipleOperationsRequest multipleOPeration = new ServiceOptimizationService.ExecuteMultipleOperationsRequest();
            NewSDTApplication.ServiceOptimizationService.GEHCJobComments jobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobComments();
            try
            {
                multipleOPeration.ContinueOnError = true;
                multipleOPeration.OneTransaction = true;
                multipleOPeration.OptionalParameters = new ServiceOptimizationService.OptionalParameters();
                multipleOPeration.OptionalParameters.CallerIdentity = "MUST";
                multipleOPeration.OptionalParameters.ErrorOnNonExistingDictionaries = true;
                List<NewSDTApplication.ServiceOptimizationService.StandardOperation> standardOperations = new List<NewSDTApplication.ServiceOptimizationService.StandardOperation>();

                //NewSDTApplication.ServiceOptimizationService.StandardOperation[] standardOperations1 = new StandardOperation[2];


                //  StandardOperation[] standardOperations = new StandardOperation[];
                NewSDTApplication.ServiceOptimizationService.Task ObjDependencyTask = new NewSDTApplication.ServiceOptimizationService.Task();
                NewSDTApplication.ServiceOptimizationService.Task objSiebelTask = new NewSDTApplication.ServiceOptimizationService.Task();
                //Job Comment Operation//



                //NewSDTApplication.ServiceOptimizationService.StandardOperation JobComentOperation = null;
                //JobComentOperation = new StandardOperation();
                //JobComentOperation.Action = "UpdateOrCreate";
                //JobComentOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                //JobComentOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)jobComments;
                //jobComments.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                //jobComments.Text = "Hello World";
                //standardOperations.Add(JobComentOperation);

                //Siebel Task Operation//
                NewSDTApplication.ServiceOptimizationService.StandardOperation SiebelOperation = null;
                SiebelOperation = new StandardOperation();
                SiebelOperation.Action = "UpdateOrCreate";
                SiebelOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                SiebelOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)objSiebelTask;
                var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];

                if (HttpContext.Current.Session["taskStatus"] == "Exists")
                {
                    GetTasksResponse taskResponse = new GetTasksResponse();
                    task = new ScheduleServiceDev1.Task();
                    taskResponse = (ScheduleServiceDev1.GetTasksResponse)HttpContext.Current.Session["TaskResponseByTask"];
                    task = taskResponse.Tasks[0];

                    objSiebelTask.Key = task.Key;
                    objSiebelTask.KeySpecified = true;
                }
                //TAke Call ID from parameter Passed// (Selected Site Parameter from Site DEpendency Grid)


                if (!string.IsNullOrEmpty(HttpContext.Current.Session["SDTHomeSSOFse1"] as string))
                {
                    objSiebelTask.PreferredFSEs = HttpContext.Current.Session["SDTHomeSSOFse1"].ToString();
                }
                else
                {
                    objSiebelTask.PreferredFSEs = "";
                }
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["SDTHomeSSOFse1"] as string))
                {
                    if (HttpContext.Current.Session["SDTHomeIsRequiredfse"].ToString() != null)
                    {
                        if (HttpContext.Current.Session["SDTHomeIsRequiredfse"].ToString() == "true")
                            objSiebelTask.RequiredFSEs = HttpContext.Current.Session["SDTHomeSSOFse1"].ToString();
                        else objSiebelTask.RequiredFSEs = "";
                    }

                }
                else
                {
                    objSiebelTask.RequiredFSEs = "";
                    objSiebelTask.PreferredFSEs = "";
                }
                objSiebelTask.CallID = CallID;
                //TAke Number from parameter Passed//
                // objSiebelTask.Number = TaskNumber + 1; // pass the parameter Number from method parameter  + 1;

                //Added by Phani Kanth P.

                objSiebelTask.Number = await getSiebelTaskNumber(CallID); // pass the parameter Number from method parameter  + 1;


                objSiebelTask.NumberSpecified = true;
                objSiebelTask.PrioritySpecified = true;
                //This is critical because we have to create successful dependency otherwise it will be not assigned on Gantt//
                objSiebelTask.Critical = true;
                objSiebelTask.CriticalSpecified = true;
                objSiebelTask.MUSTJobNumber = res.serviceRequest.srNumber;
                objSiebelTask.IsMST = true;
                objSiebelTask.IsMSTSpecified = true;
                objSiebelTask.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;
                objSiebelTask.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;
                //CustomerExpectation "B" added by phani kanth p
                objSiebelTask.CustomerExpectation = "B";
                if (res.serviceRequest.srType == "Installation")
                {
                    //processTaskExRequest.Task.EarlyStart = Convert.ToDateTime(EarlyStart);//.ToShortDateString());
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart))
                    {
                        objSiebelTask.EarlyStart = Convert.ToDateTime(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart);
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.EarlyStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart))
                    {
                        objSiebelTask.LateStart = Convert.ToDateTime(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart);
                        objSiebelTask.LateStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.LateStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress).AddDays(10);
                        objSiebelTask.LateStartSpecified = true;
                    }


                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel))
                    {
                        objSiebelTask.SkillLevel = Convert.ToInt32(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel);
                        objSiebelTask.SkillLevelSpecified = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel))
                    {
                        objSiebelTask.SkillLevel = Convert.ToInt32(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel);
                        objSiebelTask.SkillLevelSpecified = true;
                    }
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart))
                    {
                        objSiebelTask.EarlyStart = Convert.ToDateTime(DateTime.Parse(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart, CultureInfo.GetCultureInfo("en-gb")));
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.EarlyStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart))
                    {
                        objSiebelTask.LateStart = Convert.ToDateTime(DateTime.Parse(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart, CultureInfo.GetCultureInfo("en-gb")));
                        objSiebelTask.LateStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.LateStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress).AddDays(10);
                        objSiebelTask.LateStartSpecified = true;
                    }
                }

                //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                string systemstatus = objSiebelToClick.GetSystemStatus(res.serviceRequest.currentEquiptmentStatus);

                if (!string.IsNullOrEmpty(res.serviceRequest.currentEquiptmentStatus))
                    objSiebelTask.SystemStatus = new NewSDTApplication.ServiceOptimizationService.GEHCSystemStatusReference() { Name = systemstatus, DisplayString = systemstatus };


                objSiebelTask.Priority = GetPriority();

                // ContactName,ContactPhoneNumber,CustomerEmail added by phani [9/9/2016]
                objSiebelTask.ContactName = res.serviceRequest.contactFirstName + " " + res.serviceRequest.contactLastName;
                objSiebelTask.ContactPhoneNumber = res.serviceRequest.gEHCWorkPhone;
                objSiebelTask.CustomerEmail = res.serviceRequest.contactEmail;

                HttpContext.Current.Session["systemstatus"] = null;
                objSiebelTask.Duration = (((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).Duration) * (60);
                objSiebelTask.DurationSpecified = true;
                NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference CRMReference = new NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                objSiebelTask.CRMSystemName = CRMReference;
                NewSDTApplication.ServiceOptimizationService.GEHCSystemReference SystemReference = new NewSDTApplication.ServiceOptimizationService.GEHCSystemReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).SystemID };
                objSiebelTask.SystemID = SystemReference;
                NewSDTApplication.ServiceOptimizationService.GEHCSiteReference SiteReference = new NewSDTApplication.ServiceOptimizationService.GEHCSiteReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ShipToSite };
                //SiteID comment by Phani Kanth
                // objSiebelTask.SiteID = SiteReference;
                //TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();

                if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                {
                    string taskType = string.Empty;
                    taskType = objSiebelToClick.GetTaskType(res.serviceRequest.srType);
                    if (!string.IsNullOrEmpty(taskType))
                        objSiebelTask.TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference() { Name = taskType };
                }
                if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))
                {
                    string taskSubType = string.Empty;
                    taskSubType = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType);
                    if (!string.IsNullOrEmpty(taskSubType))
                        objSiebelTask.TaskSubType = new NewSDTApplication.ServiceOptimizationService.GEHCTaskSubTypeReference() { Name = taskSubType };
                }
                if ((res != null) && (res.serviceRequest != null) && (!string.IsNullOrEmpty(res.serviceRequest.gEHCSafetyConcern)))
                {
                    objSiebelTask.IsSafety = objSiebelToClick.GetSafetyConcernValue();
                    objSiebelTask.IsSafetySpecified = true;
                }
                //if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                //    objSiebelTask.TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference { Name = objSiebelToClick.GetTaskType(res.serviceRequest.srType) };
                //if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))
                //    objSiebelTask.TaskSubType = new NewSDTApplication.ServiceOptimizationService.GEHCTaskSubTypeReference() { Name = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType) };
                //task.DueDate = System.DateTime.Parse("2014-09-30T09:00:00");
                // Created date is missing in the API, we need to change once the API sends that value
                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                objSiebelTask.OpenDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), CultureInfo.GetCultureInfo("en-gb")));
                //objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                objSiebelTask.OpenDateSpecified = true;
                objSiebelTask.NumberOfRequiredEngineers = 1;
                objSiebelTask.NumberOfRequiredEngineersSpecified = true;

                objSiebelTask.Notes = ((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).TaskNotes;


                objSiebelTask.TaskID = res.serviceRequest.activityDetailList[0].activityId;

                objSiebelTask.JobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobCommentsReference() { MUSTJobNumber = "GEHC APAC Global Comment" };


                standardOperations.Add(SiebelOperation);
                //Dependency Operation for existing Site Task in ClickSoftware//
                NewSDTApplication.ServiceOptimizationService.StandardOperation DependenciesOperation = null;
                DependenciesOperation = new StandardOperation();
                DependenciesOperation.Action = "UpdateOrCreate";
                DependenciesOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                DependenciesOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)ObjDependencyTask;
                NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency[] Timedependency = new ServiceOptimizationService.TaskTaskTimeDependency[2];
                NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[] Engineerdependency = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[1];
                //Take the callID passed in method parameter//
                ObjDependencyTask.CallID = CallID;
                //Take The Number Passed in Parameter//
                ObjDependencyTask.Number = 1;
                ObjDependencyTask.NumberSpecified = true;
                ObjDependencyTask.Critical = true;
                ObjDependencyTask.CriticalSpecified = true;
                ObjDependencyTask.IsMST = true;
                ObjDependencyTask.IsMSTSpecified = true;
                ObjDependencyTask.Priority = objSiebelTask.Priority;
                ObjDependencyTask.PrioritySpecified = true;
                ObjDependencyTask.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;
                ObjDependencyTask.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;



                //ObjDependencyTask.IsMST = true;
                //ObjDependencyTask.IsMSTSpecified = true;

                //if ((res != null) && (res.serviceRequest != null) && (!string.IsNullOrEmpty(res.serviceRequest.gEHCSafetyConcern)))
                //{
                //    ObjDependencyTask.IsSafety = objSiebelToClick.GetSafetyConcernValue();
                //    ObjDependencyTask.IsSafetySpecified = true;
                //}
                // Setting dependencies

                // NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency

                Timedependency[0] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                Timedependency[1] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                Engineerdependency[0] = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency();

                Timedependency[0].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = objSiebelTask.Number, NumberSpecified = true };
                //Timedependency[0].TaskKey.Number = objSiebelTask.Number;
                Timedependency[0].UpperBound = 0;
                Timedependency[0].UpperBoundSpecified = true;
                Timedependency[0].LowerBound = 0;
                Timedependency[0].LowerBoundSpecified = true;
                Timedependency[0].RelationType = 0;
                Timedependency[0].RelationTypeSpecified = true;
                Timedependency[0].RelationOperator = 2;
                Timedependency[0].RelationOperatorSpecified = true;
                Timedependency[0].Critical = false;
                Timedependency[0].CriticalSpecified = true;

                Timedependency[1].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = objSiebelTask.Number, NumberSpecified = true };
                //Timedependency[0].TaskKey.Number = objSiebelTask.Number;
                Timedependency[1].UpperBound = 0;
                Timedependency[1].UpperBoundSpecified = true;
                Timedependency[1].LowerBound = 0;
                Timedependency[1].LowerBoundSpecified = true;
                Timedependency[1].RelationType = 4;
                Timedependency[1].RelationTypeSpecified = true;
                Timedependency[1].RelationOperator = 2;
                Timedependency[1].RelationOperatorSpecified = true;
                Timedependency[1].Critical = false;
                Timedependency[1].CriticalSpecified = true;

                Engineerdependency[0].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = objSiebelTask.Number, NumberSpecified = true };
                //Engineerdependency[0].TaskKey.Number = objSiebelTask.Number;
                Engineerdependency[0].TaskKey.NumberSpecified = true;
                Engineerdependency[0].RelationType = 1;
                Engineerdependency[0].RelationTypeSpecified = true;

                ObjDependencyTask.TimeDependencies = Timedependency;
                ObjDependencyTask.EngineerDependencies = Engineerdependency;


                NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference CRMReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                ObjDependencyTask.CRMSystemName = CRMReference1;
                // System ID from Grid to be passed (Existing Task System reference)
                NewSDTApplication.ServiceOptimizationService.GEHCSystemReference SystemReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCSystemReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).SystemID };
                // ObjDependencyTask.SystemID = SystemReference1;
                // Site ID will be passed from siebel Object or from Grid (both will be same for Site Dependency case)
                NewSDTApplication.ServiceOptimizationService.GEHCSiteReference SiteReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCSiteReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ShipToSite };
                //SiteID comment by Phani Kanth
                // ObjDependencyTask.SiteID = SiteReference1;
                //Added by Phani Kanth P
                //  ObjDependencyTask.Duration = (15) * (60);
                // ObjDependencyTask.DurationSpecified = true;
                // if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).Duration.ToString()))
                //{
                ObjDependencyTask.Duration = (Duration) * (60); // ((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).Duration;
                ObjDependencyTask.DurationSpecified = true;
                // }
                // ObjDependencyTask.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                ObjDependencyTask.JobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobCommentsReference() { MUSTJobNumber = "GEHC APAC Global Comment" };
                standardOperations.Add(DependenciesOperation);
                //  multipleOPeration.Operations = standardOperations.ToArray() ;

                var results = ServiceOptimizationService.ExecuteMultipleOperations(multipleOPeration.OptionalParameters, standardOperations.ToArray(), true, false);

                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                logger.Debug("CallClickSerrvice.cs;CreateTaskWithDependencies:ExecuteMultipleOperations TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);

                HttpContext.Current.Session["CallIDDependent"] = TaskData[0].CallID;

                //}              
                //return true;
                return "Success";


            }

            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;CreateTaskWithDependencies; Exception: " + ex.Message);
                //return false;
                //DateTime endTime = DateTime.Now;

                //TimeSpan span = endTime.Subtract(startTime);
                //logger.Debug("CallClickSerrvice.cs;CreateTaskWithDependencies:ExecuteMultipleOperations TimeSpan in Sec ;" + span.Seconds);

                return ex.Message.ToString();
                //throw;
            }
            throw new NotImplementedException();
        }

        //public bool CreateTaskWithSystemDependencies(List<CustomTasksList> TaskData)
        //Change String to async added by Phani Kanth P.
        public async Task<string> CreateTaskWithSystemDependencies(List<CustomTasksList> TaskData)
        {
            TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
            string CallID = string.Empty;
            int TaskNumber = 0;
            int Duration = 0;
            string MustJobNumber = string.Empty;
            if (TaskData.Count > 0)
            {
                CallID = TaskData[0].CallID;
                TaskNumber = Convert.ToInt32(TaskData[0].TaskNumber);
                Duration = Convert.ToInt32(TaskData[0].Duration);
                MustJobNumber = TaskData[0].MUSTJobNumber;
            }
            DateTime startTime = DateTime.Now;
            //int requestCount = FinalRes.Count();
            OpenConnectionToSDT();
            ServiceOptimizationService.ExecuteMultipleOperationsRequest multipleOPeration = new ServiceOptimizationService.ExecuteMultipleOperationsRequest();
            NewSDTApplication.ServiceOptimizationService.GEHCJobComments jobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobComments();
            try
            {
                multipleOPeration.ContinueOnError = true;
                multipleOPeration.OneTransaction = true;
                multipleOPeration.OptionalParameters = new ServiceOptimizationService.OptionalParameters();
                multipleOPeration.OptionalParameters.CallerIdentity = "MUST";
                multipleOPeration.OptionalParameters.ErrorOnNonExistingDictionaries = true;
                List<NewSDTApplication.ServiceOptimizationService.StandardOperation> standardOperations = new List<NewSDTApplication.ServiceOptimizationService.StandardOperation>();
                NewSDTApplication.ServiceOptimizationService.Task ObjDependencyTask = new NewSDTApplication.ServiceOptimizationService.Task();
                NewSDTApplication.ServiceOptimizationService.Task objSiebelTask = new NewSDTApplication.ServiceOptimizationService.Task();
                //Job Comment Operation//
                NewSDTApplication.ServiceOptimizationService.StandardOperation JobComentOperation = null;
                JobComentOperation = new StandardOperation();
                JobComentOperation.Action = "UpdateOrCreate";
                JobComentOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                JobComentOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)jobComments;
                jobComments.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                jobComments.Text = "Hello World";
                standardOperations.Add(JobComentOperation);
                //Siebel Task Operation//
                NewSDTApplication.ServiceOptimizationService.StandardOperation SiebelOperation = null;
                SiebelOperation = new StandardOperation();
                SiebelOperation.Action = "UpdateOrCreate";
                SiebelOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                SiebelOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)objSiebelTask;
                var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];
                //TAke Call ID from parameter Passed// (Selected Site Parameter from Site DEpendency Grid)

                //Key specified attribute Added by Phani Kanth
                if (HttpContext.Current.Session["taskStatus"] == "Exists")
                {
                    GetTasksResponse taskResponse = new GetTasksResponse();
                    task = new ScheduleServiceDev1.Task();
                    taskResponse = (ScheduleServiceDev1.GetTasksResponse)HttpContext.Current.Session["TaskResponseByTask"];
                    task = taskResponse.Tasks[0];

                    objSiebelTask.Key = task.Key;
                    objSiebelTask.KeySpecified = true;
                }

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["SDTHomeSSOFse1"] as string))
                {
                    objSiebelTask.PreferredFSEs = HttpContext.Current.Session["SDTHomeSSOFse1"].ToString();
                }
                else
                {
                    objSiebelTask.PreferredFSEs = "";
                }
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["SDTHomeSSOFse1"] as string))
                {
                    if (HttpContext.Current.Session["SDTHomeIsRequiredfse"].ToString() != null)
                    {
                        if (HttpContext.Current.Session["SDTHomeIsRequiredfse"].ToString() == "true")
                            objSiebelTask.RequiredFSEs = HttpContext.Current.Session["SDTHomeSSOFse1"].ToString();
                        else objSiebelTask.RequiredFSEs = "";
                    }

                }
                else
                {
                    objSiebelTask.RequiredFSEs = "";
                    objSiebelTask.PreferredFSEs = "";
                }
                objSiebelTask.CallID = CallID;
                //TAke Number from parameter Passed//
                // objSiebelTask.Number = TaskNumber + 1; // pass the parameter Number from method parameter  + 1;
                //Added by Phani Kanth P.

                objSiebelTask.Number = await getSiebelTaskNumber(CallID); // pass the parameter Number from method parameter  + 1;
                //CustomerExpectation "B" added by phani kanth p
                objSiebelTask.CustomerExpectation = "B";

                objSiebelTask.NumberSpecified = true;
                objSiebelTask.PrioritySpecified = true;
                //This is critical because we have to create successful dependency otherwise it will be not assigned on Gantt//
                objSiebelTask.Critical = true;
                objSiebelTask.CriticalSpecified = true;
                objSiebelTask.MUSTJobNumber = res.serviceRequest.srNumber;
                objSiebelTask.IsMST = true;
                objSiebelTask.IsMSTSpecified = true;

                // ContactName,ContactPhoneNumber,CustomerEmail added by phani [9/9/2016]
                objSiebelTask.ContactName = res.serviceRequest.contactFirstName + " " + res.serviceRequest.contactLastName;
                objSiebelTask.ContactPhoneNumber = res.serviceRequest.gEHCWorkPhone;
                objSiebelTask.CustomerEmail = res.serviceRequest.contactEmail;

                if (res.serviceRequest.srType == "Installation")
                {
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel))
                    {
                        objSiebelTask.SkillLevel = Convert.ToInt32(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel);
                        objSiebelTask.SkillLevelSpecified = true;
                    }
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart))
                    {


                        objSiebelTask.EarlyStart = Convert.ToDateTime(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart);
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.EarlyStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart))
                    {
                        objSiebelTask.LateStart = Convert.ToDateTime(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart);
                        objSiebelTask.LateStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.LateStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress).AddDays(10);
                        objSiebelTask.LateStartSpecified = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel))
                    {
                        objSiebelTask.SkillLevel = Convert.ToInt32(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).FseSkillLevel);
                        objSiebelTask.SkillLevelSpecified = true;
                    }
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart))
                    {
                        objSiebelTask.EarlyStart = Convert.ToDateTime(DateTime.Parse(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).earlyStart, CultureInfo.GetCultureInfo("en-gb")));
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.EarlyStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                        objSiebelTask.EarlyStartSpecified = true;
                    }
                    if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart))
                    {
                        objSiebelTask.LateStart = Convert.ToDateTime(DateTime.Parse(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).lateStart, CultureInfo.GetCultureInfo("en-gb")));
                        objSiebelTask.LateStartSpecified = true;
                    }
                    else
                    {
                        objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                        objSiebelTask.LateStart = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress).AddDays(10);
                        objSiebelTask.LateStartSpecified = true;
                    }
                }

                //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                string systemstatus = objSiebelToClick.GetSystemStatus(res.serviceRequest.currentEquiptmentStatus);

                if (!string.IsNullOrEmpty(res.serviceRequest.currentEquiptmentStatus))
                    objSiebelTask.SystemStatus = new NewSDTApplication.ServiceOptimizationService.GEHCSystemStatusReference() { Name = systemstatus, DisplayString = systemstatus };


                objSiebelTask.Priority = GetPriority();


                HttpContext.Current.Session["systemstatus"] = null;
                objSiebelTask.Duration = (((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).Duration) * (60);
                objSiebelTask.DurationSpecified = true;
                NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference CRMReference = new NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                objSiebelTask.CRMSystemName = CRMReference;
                NewSDTApplication.ServiceOptimizationService.GEHCSystemReference SystemReference = new NewSDTApplication.ServiceOptimizationService.GEHCSystemReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).SystemID };
                objSiebelTask.SystemID = SystemReference;
                NewSDTApplication.ServiceOptimizationService.GEHCSiteReference SiteReference = new NewSDTApplication.ServiceOptimizationService.GEHCSiteReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ShipToSite };
                //SiteID comment by Phani Kanth
                //  objSiebelTask.SiteID = SiteReference;

                objSiebelTask.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;
                objSiebelTask.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;

                if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                {
                    string taskType = string.Empty;
                    taskType = objSiebelToClick.GetTaskType(res.serviceRequest.srType);
                    if (!string.IsNullOrEmpty(taskType))
                        objSiebelTask.TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference() { Name = taskType };
                }
                if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))
                {
                    string taskSubType = string.Empty;
                    taskSubType = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType);
                    if (!string.IsNullOrEmpty(taskSubType))
                        objSiebelTask.TaskSubType = new NewSDTApplication.ServiceOptimizationService.GEHCTaskSubTypeReference() { Name = taskSubType };
                }

                if ((res != null) && (res.serviceRequest != null) && (!string.IsNullOrEmpty(res.serviceRequest.gEHCSafetyConcern)))
                {
                    objSiebelTask.IsSafety = objSiebelToClick.GetSafetyConcernValue();
                    objSiebelTask.IsSafetySpecified = true;
                }

                //if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                //{ 
                //    objSiebelTask.TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference { Name = objSiebelToClick.GetTaskType(res.serviceRequest.srType) };
                //}
                //if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))
                //    objSiebelTask.TaskSubType = new NewSDTApplication.ServiceOptimizationService.GEHCTaskSubTypeReference() { Name = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType) };
                //task.DueDate = System.DateTime.Parse("2014-09-30T09:00:00");
                // Created date is missing in the API, we need to change once the API sends that value
                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();

                objSiebelTask.OpenDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), CultureInfo.GetCultureInfo("en-gb")));
                //objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                objSiebelTask.OpenDateSpecified = true;
                objSiebelTask.NumberOfRequiredEngineers = 1;
                objSiebelTask.NumberOfRequiredEngineersSpecified = true;
                objSiebelTask.Notes = ((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).TaskNotes;
                objSiebelTask.TaskID = res.serviceRequest.activityDetailList[0].activityId;
                objSiebelTask.JobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobCommentsReference() { MUSTJobNumber = objSiebelTask.MUSTJobNumber };
                standardOperations.Add(SiebelOperation);
                //Dependency Operation for existing Site Task in ClickSoftware//
                NewSDTApplication.ServiceOptimizationService.StandardOperation DependenciesOperation = null;
                DependenciesOperation = new StandardOperation();
                DependenciesOperation.Action = "UpdateOrCreate";
                DependenciesOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                DependenciesOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)ObjDependencyTask;
                NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency[] Timedependency = new ServiceOptimizationService.TaskTaskTimeDependency[1];
                NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[] Engineerdependency = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[1];
                //Take the callID passed in method parameter//
                ObjDependencyTask.CallID = CallID;
                //Take The Number Passed in Parameter//
                ObjDependencyTask.Number = 1;
                ObjDependencyTask.NumberSpecified = true;
                ObjDependencyTask.Critical = true;
                ObjDependencyTask.CriticalSpecified = true;
                ObjDependencyTask.IsMST = true;
                ObjDependencyTask.IsMSTSpecified = true;
                ObjDependencyTask.Priority = objSiebelTask.Priority;
                ObjDependencyTask.PrioritySpecified = true;
                ObjDependencyTask.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;
                ObjDependencyTask.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;

                //if ((res != null) && (res.serviceRequest != null) && (!string.IsNullOrEmpty(res.serviceRequest.gEHCSafetyConcern)))
                //{
                //    ObjDependencyTask.IsSafety = objSiebelToClick.GetSafetyConcernValue();
                //    ObjDependencyTask.IsSafetySpecified = true;
                //}
                // Setting dependencies

                // NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency

                Timedependency[0] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                //  Timedependency[1] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                Engineerdependency[0] = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency();

                Timedependency[0].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = objSiebelTask.Number, NumberSpecified = true };
                //Timedependency[0].TaskKey.Number = objSiebelTask.Number;
                Timedependency[0].UpperBound = 0;
                Timedependency[0].UpperBoundSpecified = true;
                Timedependency[0].LowerBound = 0;
                Timedependency[0].LowerBoundSpecified = true;
                Timedependency[0].RelationType = 4;
                Timedependency[0].RelationTypeSpecified = true;
                Timedependency[0].RelationOperator = 2;
                Timedependency[0].RelationOperatorSpecified = true;
                Timedependency[0].Critical = false;
                Timedependency[0].CriticalSpecified = true;

                //Timedependency[1].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = objSiebelTask.Number, NumberSpecified = true };
                ////Timedependency[0].TaskKey.Number = objSiebelTask.Number;
                //Timedependency[1].UpperBound = 0;
                //Timedependency[1].UpperBoundSpecified = true;
                //Timedependency[1].LowerBound = 0;
                //Timedependency[1].LowerBoundSpecified = true;
                //Timedependency[1].RelationType = 4;
                //Timedependency[1].RelationTypeSpecified = true;
                //Timedependency[1].RelationOperator = 2;
                //Timedependency[1].RelationOperatorSpecified = true;
                //Timedependency[1].Critical = false;
                //Timedependency[1].CriticalSpecified = true;

                Engineerdependency[0].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = objSiebelTask.Number, NumberSpecified = true };
                //Engineerdependency[0].TaskKey.Number = objSiebelTask.Number;
                Engineerdependency[0].TaskKey.NumberSpecified = true;
                Engineerdependency[0].RelationType = 1;
                Engineerdependency[0].RelationTypeSpecified = true;

                ObjDependencyTask.TimeDependencies = Timedependency;
                ObjDependencyTask.EngineerDependencies = Engineerdependency;
                NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference CRMReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                ObjDependencyTask.CRMSystemName = CRMReference1;
                // System ID from Grid to be passed (Existing Task System reference)
                NewSDTApplication.ServiceOptimizationService.GEHCSystemReference SystemReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCSystemReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).SystemID };
                // ObjDependencyTask.SystemID = SystemReference1;
                // Site ID will be passed from siebel Object or from Grid (both will be same for Site Dependency case)
                NewSDTApplication.ServiceOptimizationService.GEHCSiteReference SiteReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCSiteReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ShipToSite };
                //SiteID comment by Phani Kanth
                //  ObjDependencyTask.SiteID = SiteReference1;
                // ObjDependencyTask.Duration = (15) * (60);
                //if (!string.IsNullOrEmpty(((IndextoRequest)HttpContext.Current.Session["IndextoRequest"]).Duration.ToString()))
                // {
                //Duration added by phani kanth P
                ObjDependencyTask.Duration = (Duration) * (60);
                ObjDependencyTask.DurationSpecified = true;
                // }
                // ObjDependencyTask.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                ObjDependencyTask.JobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobCommentsReference() { MUSTJobNumber = "GEHC APAC Global Comment" };
                standardOperations.Add(DependenciesOperation);
                var results = ServiceOptimizationService.ExecuteMultipleOperations(multipleOPeration.OptionalParameters, standardOperations.ToArray(), true, false);
                var res1 = results.Items.Where(x => x.Action == "Create");
                if (res1.Count() > 0)
                {
                    HttpContext.Current.Session["CallIDDependent"] = TaskData[0].CallID;

                }
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                logger.Debug("CallClickSerrvice.cs;CreateTaskWithSystemDependencies;ExecuteMultipleOperations TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);
                return "Success";
            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;CreateTaskWithSystemDependencies; Exception: " + ex.Message);
                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                logger.Debug("CallClickSerrvice.cs;CreateTaskWithSystemDependencies;ExecuteMultipleOperations TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);

                throw;

            }
            throw new NotImplementedException();
        }

        public int GetPriority()
        {

            TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
            var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];
            NewSDTApplication.ServiceOptimizationService.Task objSiebelTask = new NewSDTApplication.ServiceOptimizationService.Task();

            if (string.IsNullOrEmpty(HttpContext.Current.Session["systemstatus"] as string))
            {
                //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                string systemstatus = objSiebelToClick.GetSystemStatus(res.serviceRequest.currentEquiptmentStatus);
                if (!string.IsNullOrEmpty(systemstatus))
                {
                    var arr = systemstatus.Split('(');

                    if (arr[0].Trim() == "3")
                        HttpContext.Current.Session["systemstatus"] = "up";
                    else if (arr[0].Trim() == "2")
                        HttpContext.Current.Session["systemstatus"] = "partial";
                    else if (arr[0].Trim() == "1")
                        HttpContext.Current.Session["systemstatus"] = "down";
                }
                else
                    HttpContext.Current.Session["systemstatus"] = "";
            }

            switch (res.serviceRequest.srType)
            {
                default:
                    objSiebelTask.Priority = 200;
                    break;
                case "PM":
                    if (res.serviceRequest.gEMSEntitlementFlag == "Y")
                        objSiebelTask.Priority = 650;
                    else
                        objSiebelTask.Priority = 600;
                    break;
                case "Corrective Repair":
                    if (res.serviceRequest.gEMSEntitlementFlag == "Y")
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["systemstatus"] as string))
                        {
                            if ((res.serviceRequest.gEHCSafetyConcern == "Actual" || res.serviceRequest.gEHCSafetyConcern == "Potential") && (HttpContext.Current.Session["systemstatus"].ToString() == "down"))//3(System up and running)
                                objSiebelTask.Priority = 800;
                            else if ((res.serviceRequest.gEHCSafetyConcern == "No") && (HttpContext.Current.Session["systemstatus"].ToString() == "partial" || HttpContext.Current.Session["systemstatus"].ToString() == "up"))
                                objSiebelTask.Priority = 750;
                            else
                                objSiebelTask.Priority = 200;
                        }
                        else
                            objSiebelTask.Priority = 200;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["systemstatus"] as string))
                        {
                            if ((res.serviceRequest.gEHCSafetyConcern == "Actual" || res.serviceRequest.gEHCSafetyConcern == "Potential") && (HttpContext.Current.Session["systemstatus"].ToString() == "down"))// Here in the system statum we are getting numbering(3(System up and running))
                                objSiebelTask.Priority = 700;
                            else if ((res.serviceRequest.gEHCSafetyConcern == "No") && (HttpContext.Current.Session["systemstatus"].ToString() == "partial" || HttpContext.Current.Session["systemstatus"].ToString() == "up"))
                                objSiebelTask.Priority = 400;
                            else
                                objSiebelTask.Priority = 200;
                        }
                        else
                            objSiebelTask.Priority = 200;
                    }
                    break;
                case "Installation":
                    objSiebelTask.Priority = 300;
                    break;
                case "FMI":
                    switch (res.serviceRequest.srSubType)
                    {
                        case "Safety":
                            objSiebelTask.Priority = 580;
                            break;
                        case "Mandatory":
                            objSiebelTask.Priority = 590;
                            break;
                        case "On Request":
                            objSiebelTask.Priority = 570;
                            break;
                    }
                    break;
            }

            return objSiebelTask.Priority;
        }
        //var resPartPickUp = objCallClickService.PartPickDependency(objIndexToRequest.earlyStart, objIndexToRequest.lateStart, objIndexToRequest.Duration, objIndexToRequest.TaskNotes, objIndexToRequest.FseSkillLevel, addressArray, objIndexToRequest.PreferredFSEs);
        public bool PartPickDependency(string earlystart, string latestart, int duration, string TaskNotes, string fseSkill, List<List<Array>> AddressArray, string partComments, string PreferredFSEs, string AppStart = "", string AppFinish = "")
        {

            TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();

            logger.Debug("CallClickSerrvice.cs;PartPickDependency; Start");
            logger.Debug("CallClickSerrvice.cs;PartPickDependency; SessionTest Task StartDate" + earlystart + "Duration" + duration);

            //Changes done by Raju
            //Removed  to allow the all special char
            ArrayList list = new ArrayList();
            for (int iList = 0; iList < AddressArray[0].Count; iList++)
            {
                list.Add(AddressArray[0][iList]);

            }

            //for (int j = 0; j < list.Count; j++)
            //{

            //    string[] addres = list[j] as string[];
            //}
            ProcessTaskExRequest processTaskExRequest = new ProcessTaskExRequest();
            ProcessTaskExResponse processTaskExResponse = new ProcessTaskExResponse();

            processTaskExRequest.RelatedTasks = new ScheduleServiceDev1.Task[AddressArray[0].Count];
            processTaskExRequest.OptionalParameters = new ScheduleServiceDev1.OptionalParameters() { CallerIdentity = "MUST", ErrorOnNonExistingDictionaries = true };

            //processTaskExRequest.OptionalParameters = new SDT.Schedule.Web.Services.ScheduleService.OptionalParameters() { CallerIdentity = "MUST" };
            processTaskExRequest.TaskRequestedProperties = new string[]
                    {
                        "Key",
                        "KeySpecified",
                        "Priority",
                        "PrioritySpecified",
                        "Duration",
                        "DurationSpecified"
                    };


            //task = new ScheduleServiceDev1.Task();

            logger.Debug("CallClickSerrvice.cs;PartPickDependency; arrayAddressCount:" + AddressArray[0].Count);

            ArrayList Address = new ArrayList();
            var res = (SiebelJsonToEntity)HttpContext.Current.Session["SiebelData"];
            //Changes done by Raju
            //Removed Split to allow the all special char
            //for (int i = 0; i < arrayAddress.Length; i++)
            //{
            //    string[] addres = arrayAddress[i].Split(',');
            //    Address.Add(addres);

            //    logger.Debug("CallClickSerrvice.cs;PartPickDependency; arrayAddressCount:" + arrayAddress[i].ToString());

            //}
            objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();

            string CallID = "APAC" + "-" + res.serviceRequest.activityDetailList[0].activityId + "-" + "1";

            //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

            string systemstatus = objSiebelToClick.GetSystemStatus(res.serviceRequest.currentEquiptmentStatus);

            logger.Debug("CallClickSerrvice.cs;PartPickDependency; CallID:" + CallID);
            OpenConnectionToSDT();
            ServiceOptimizationService.ExecuteMultipleOperationsRequest multipleOPeration = new ServiceOptimizationService.ExecuteMultipleOperationsRequest();
            NewSDTApplication.ServiceOptimizationService.GEHCJobComments jobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobComments();
            try
            {
                multipleOPeration.ContinueOnError = true;
                multipleOPeration.OneTransaction = true;
                multipleOPeration.OptionalParameters = new ServiceOptimizationService.OptionalParameters();
                multipleOPeration.OptionalParameters.CallerIdentity = "MUST";
                multipleOPeration.OptionalParameters.ErrorOnNonExistingDictionaries = true;
                List<NewSDTApplication.ServiceOptimizationService.StandardOperation> standardOperations = new List<NewSDTApplication.ServiceOptimizationService.StandardOperation>();

                NewSDTApplication.ServiceOptimizationService.Task objSiebelTask = new NewSDTApplication.ServiceOptimizationService.Task();
                //Job Comment Operation//
                NewSDTApplication.ServiceOptimizationService.StandardOperation JobComentOperation = null;
                JobComentOperation = new StandardOperation();
                JobComentOperation.Action = "UpdateOrCreate";
                JobComentOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                JobComentOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)jobComments;
                jobComments.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                jobComments.Text = "Hello World";
                standardOperations.Add(JobComentOperation);
                //Changes done by Raju
                //Removed Split to allow the all special char
                //for (int j = 0; j < list.Count; j++)
                //{

                //    string[] addres = list[j] as string[];
                //}

                //for (int i = 0; i < arrayAddress.Length; i++)
                //{
                //    string[] addressArray = Address[i] as string[];
                for (int j = 0; j < list.Count; j++)
                {

                    string[] addressArray = list[j] as string[];
                    // Added by phani Kanth P.

                    processTaskExRequest.RelatedTasks[j] = new ScheduleServiceDev1.Task();
                    processTaskExRequest.RelatedTasks[j].CallID = CallID;
                    processTaskExRequest.RelatedTasks[j].MacroVersion = "1";// Convert.ToString(objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress));
                    processTaskExRequest.RelatedTasks[j].Number = j + 2;
                    processTaskExRequest.RelatedTasks[j].NumberSpecified = true;


                    NewSDTApplication.ServiceOptimizationService.Task ObjDependencyTask = new NewSDTApplication.ServiceOptimizationService.Task();
                    NewSDTApplication.ServiceOptimizationService.StandardOperation DependenciesOperation = null;
                    DependenciesOperation = new StandardOperation();
                    DependenciesOperation.Action = "UpdateOrCreate";
                    DependenciesOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                    DependenciesOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)ObjDependencyTask;
                    NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency[] Timedependency = new ServiceOptimizationService.TaskTaskTimeDependency[list.Count];
                    NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[] Engineerdependency = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[list.Count];
                    //Take the callID passed in method parameter//
                    ObjDependencyTask.CallID = CallID;
                    //Take The Number Passed in Parameter//
                    //ObjDependencyTask.Number = i + 2;
                    //ObjDependencyTask.NumberSpecified = true;
                    ObjDependencyTask.Critical = true;
                    ObjDependencyTask.CriticalSpecified = true;
                    ObjDependencyTask.Priority = GetPriority();
                    ObjDependencyTask.PrioritySpecified = true;
                    ObjDependencyTask.IsMST = true;
                    ObjDependencyTask.IsMSTSpecified = true;

                    //if (!string.IsNullOrEmpty(earlystart))
                    //{
                    //    if (res.serviceRequest.srType == "Installation")
                    //    {
                    //        ObjDependencyTask.EarlyStart = Convert.ToDateTime(earlystart);
                    //        ObjDependencyTask.EarlyStartSpecified = true;
                    //    }
                    //    else
                    //    {
                    //        ObjDependencyTask.EarlyStart = Convert.ToDateTime(DateTime.Parse(earlystart, CultureInfo.GetCultureInfo("en-gb")));
                    //        ObjDependencyTask.EarlyStartSpecified = true;
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(latestart))
                    {
                        if (res.serviceRequest.srType == "Installation")
                        {
                            ObjDependencyTask.LateStart = Convert.ToDateTime(latestart);
                            ObjDependencyTask.LateStartSpecified = true;
                            ObjDependencyTask.IsMDT = true;
                            ObjDependencyTask.IsMDTSpecified = true;
                        }
                        else
                        {
                            ObjDependencyTask.LateStart = Convert.ToDateTime(DateTime.Parse(latestart, CultureInfo.GetCultureInfo("en-gb")));
                            ObjDependencyTask.LateStartSpecified = true;
                        }
                    }

                    var CountryIDKO = ConfigurationManager.AppSettings["CountryIDKO"];
                    if (addressArray[4] == "KOREA REPUBLIC OF")
                    {
                        NewSDTApplication.ServiceOptimizationService.CountryReference CountryRefernce = new NewSDTApplication.ServiceOptimizationService.CountryReference() { Name = CountryIDKO };
                        ObjDependencyTask.CountryID = CountryRefernce;
                        logger.Debug("CallClickSerrvice.cs;PartPickDependency; CountryID:" + CountryIDKO);
                    }
                    else
                    {
                        NewSDTApplication.ServiceOptimizationService.CountryReference CountryRefernce = new NewSDTApplication.ServiceOptimizationService.CountryReference() { Name = addressArray[4] };
                        ObjDependencyTask.CountryID = CountryRefernce;
                        logger.Debug("CallClickSerrvice.cs;PartPickDependency; CountryID:" + addressArray[4]);
                    }

                    NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference CRMReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                    ObjDependencyTask.CRMSystemName = CRMReference1;
                    // System ID from Grid to be passed (Existing Task System reference)
                    NewSDTApplication.ServiceOptimizationService.GEHCSystemReference SystemReference1 = new NewSDTApplication.ServiceOptimizationService.GEHCSystemReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).SystemID };
                    ObjDependencyTask.SystemID = SystemReference1;


                    logger.Debug("CallClickSerrvice.cs;PartPickDependency; SystemID:" + SystemReference1.ID);

                    // Site ID will be passed from siebel Object or from Grid (both will be same for Site Dependency case)
                    NewSDTApplication.ServiceOptimizationService.TaskTypeReference TaskType1 = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference() { Name = "Parts Pickup" };
                    ObjDependencyTask.TaskType = TaskType1;
                    ObjDependencyTask.Duration = 900;
                    ObjDependencyTask.DurationSpecified = true;


                    //if (!string.IsNullOrEmpty(partComments))
                    //    ObjDependencyTask.PartComment = partComments;

                    ObjDependencyTask.PartDeliveryType = addressArray[0];
                    //if (addressArray[0] != "\"Customer Site")
                    //{
                    if (!string.IsNullOrEmpty(addressArray[1].Trim()))
                    {
                        ObjDependencyTask.Street = addressArray[1];
                    }
                    else
                    {
                        ObjDependencyTask.Street = "";
                    }

                    if (!string.IsNullOrEmpty(addressArray[2].Trim()))
                    {
                        ObjDependencyTask.City = addressArray[2];
                    }
                    else
                    {
                        ObjDependencyTask.City = "";
                    }


                    if (!string.IsNullOrEmpty(addressArray[3].Trim()))
                    {
                        ObjDependencyTask.Postcode = addressArray[3];
                    }
                    else
                    {
                        ObjDependencyTask.Postcode = "";
                    }



                    if (!string.IsNullOrEmpty(addressArray[5].Trim()))
                    {
                        ObjDependencyTask.PartComment = addressArray[5];
                    }
                    else
                    {
                        ObjDependencyTask.PartComment = "";
                    }

                    if (!string.IsNullOrEmpty(addressArray[6].Trim()))
                    {



                        //if (res.serviceRequest.srType == "Installation")
                        //{
                        //    ObjDependencyTask.PartEstimatedDeliveryDate = Convert.ToDateTime(DateTime.Parse(addressArray[6].Trim()));
                        //    ObjDependencyTask.EarlyStart = Convert.ToDateTime(addressArray[6].Trim());

                        //}
                        //else
                        //{
                        ObjDependencyTask.PartEstimatedDeliveryDate = (Convert.ToDateTime(DateTime.Parse(addressArray[6].Trim(), CultureInfo.GetCultureInfo("en-gb"))));
                        ObjDependencyTask.EarlyStart = Convert.ToDateTime(DateTime.Parse(addressArray[6].Trim(), CultureInfo.GetCultureInfo("en-gb")));

                        //}
                        ObjDependencyTask.EarlyStartSpecified = true;
                        ObjDependencyTask.PartEstimatedDeliveryDateSpecified = true;
                    }


                    if (!string.IsNullOrEmpty(addressArray[7].Trim()))
                    {

                        ObjDependencyTask.Latitude = Convert.ToInt32(addressArray[7].Trim());
                        ObjDependencyTask.LatitudeSpecified = true;
                    }
                    else
                    {
                        ObjDependencyTask.Latitude = 0;
                        ObjDependencyTask.LatitudeSpecified = false;
                    }

                    // addressArray[8] = addressArray[8].Trim().Replace("\"", "");
                    if (!string.IsNullOrEmpty(addressArray[8]))
                    {
                        // If Length is 1 it is getting subtracted to 0 .it is thrown exception in this case.

                        if (addressArray[8].Length > 1)
                        {
                            if (!string.IsNullOrEmpty(addressArray[8].Trim()))
                            {
                                ObjDependencyTask.Longitude = Convert.ToInt32(addressArray[8].Trim());
                                ObjDependencyTask.LongitudeSpecified = true;
                            }
                            else
                            {
                                ObjDependencyTask.Longitude = 0;
                                ObjDependencyTask.LongitudeSpecified = true;
                            }
                        }
                        else
                        {
                            ObjDependencyTask.Longitude = 1;
                            ObjDependencyTask.LongitudeSpecified = true;
                        }
                    }
                    else
                    {
                        ObjDependencyTask.Longitude = 0;
                        ObjDependencyTask.LongitudeSpecified = false;
                    }

                    if (addressArray.Length > 9)
                    {
                        if (!string.IsNullOrEmpty(addressArray[9].Trim()))
                        {
                            addressArray[9] = addressArray[9].Trim().Replace("\"", "");

                            ObjDependencyTask.Number = Convert.ToInt32(addressArray[9]);
                            ObjDependencyTask.NumberSpecified = true;


                        }
                        else
                        {
                            ObjDependencyTask.Number = j + 2; ;
                            ObjDependencyTask.NumberSpecified = true;
                        }
                    }
                    else
                    {
                        ObjDependencyTask.Number = j + 2; ;
                        ObjDependencyTask.NumberSpecified = true;
                    }
                    if (addressArray.Length > 10)
                    {
                        if (!string.IsNullOrEmpty(addressArray[10].Trim()))
                        {
                            addressArray[10] = addressArray[10].Trim().Replace("\"", "");

                            //ProcessTaskExRequest processTaskExRequest1 = new ProcessTaskExRequest();

                            //processTaskExRequest1.Task.Status =
                            if (addressArray[10] == "Cancelled")
                            {
                                ObjDependencyTask.Status = new NewSDTApplication.ServiceOptimizationService.TaskStatusReference() { Name = addressArray[10] };

                                // objSiebelTask.TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference() { Name = taskType };

                                ObjDependencyTask.CancellationReason = new NewSDTApplication.ServiceOptimizationService.GEHCCancellationReasonReference() { Name = "Other" };

                            }

                            //else
                            //{
                            //    ObjDependencyTask.Status = new NewSDTApplication.ServiceOptimizationService.TaskStatusReference() { Name = addressArray[10] };

                            //}

                        }
                    }
                    if (addressArray.Length > 11)
                    {
                        if (!string.IsNullOrEmpty(addressArray[11].Trim()))
                        {
                            addressArray[11] = addressArray[11].Trim().Replace("\"", "");

                            //ProcessTaskExRequest processTaskExRequest1 = new ProcessTaskExRequest();

                            //processTaskExRequest1.Task.Status =
                            if (addressArray[11] == "false")
                            {
                                ObjDependencyTask.IsMST = false;
                                ObjDependencyTask.IsMSTSpecified = false;

                            }
                            else
                            {
                                ObjDependencyTask.IsMST = true;
                                ObjDependencyTask.IsMSTSpecified = true;

                            }

                        }
                    }
                    if (addressArray.Length > 12)
                    {
                        addressArray[12] = addressArray[12].Trim().Replace("\"", "");

                        if (!string.IsNullOrEmpty(addressArray[12].Trim()))
                        {

                            //ProcessTaskExRequest processTaskExRequest1 = new ProcessTaskExRequest();

                            //processTaskExRequest1.Task.Status =
                            if (addressArray[12] == "false")
                            {
                                ObjDependencyTask.Critical = false;
                                ObjDependencyTask.CriticalSpecified = false;

                            }
                            else
                            {
                                ObjDependencyTask.Critical = true;
                                ObjDependencyTask.CriticalSpecified = true;

                            }

                        }
                    }

                    //ObjDependencyTask.LatitudeSpecified = true;
                    //ObjDependencyTask.Longitude = Convert.ToInt32(addressArray[6].TrimEnd(addressArray[6][addressArray[6].Length - 1]));
                    //ObjDependencyTask.LongitudeSpecified = true;

                    if (!string.IsNullOrEmpty(fseSkill))
                    {
                        ObjDependencyTask.SkillLevel = Convert.ToInt32(fseSkill);
                        ObjDependencyTask.SkillLevelSpecified = true;
                    }
                    ObjDependencyTask.TaskID = res.serviceRequest.activityDetailList[0].activityId;


                    //ObjDependencyTask.IsMST = true;
                    //ObjDependencyTask.IsMSTSpecified = true;

                    //}
                    //else
                    //{
                    //    // NewSDTApplication.ServiceOptimizationService.GEHCSiteReference SiteReferenceDependency = new NewSDTApplication.ServiceOptimizationService.GEHCSiteReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ShipToSite };
                    //    objSiebelTask.SystemID = SystemReference1;
                    //    ObjDependencyTask.IsMST = true;
                    //    ObjDependencyTask.IsMSTSpecified = true;

                    //}
                    if (addressArray[0] == "\"Customer Site")
                    {
                        objSiebelTask.SystemID = SystemReference1;
                        //ObjDependencyTask.IsMST = true;
                        //ObjDependencyTask.IsMSTSpecified = true;

                    }
                    //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                    if (!string.IsNullOrEmpty(res.serviceRequest.currentEquiptmentStatus))
                        ObjDependencyTask.SystemStatus = new NewSDTApplication.ServiceOptimizationService.GEHCSystemStatusReference() { Name = systemstatus, DisplayString = systemstatus };


                    ObjDependencyTask.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;
                    ObjDependencyTask.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;
                    ObjDependencyTask.UseDistrictCalendar = true;
                    ObjDependencyTask.UseDistrictCalendarSpecified = true;
                    ObjDependencyTask.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                    // ObjDependencyTask.Priority = objSiebelTask.Priority;
                    ObjDependencyTask.MUSTJobNumber = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ServiceRequestNumber;
                    ObjDependencyTask.JobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobCommentsReference() { MUSTJobNumber = ObjDependencyTask.MUSTJobNumber };
                    standardOperations.Add(DependenciesOperation);
                }


                NewSDTApplication.ServiceOptimizationService.StandardOperation SiebelOperation = null;
                SiebelOperation = new StandardOperation();
                SiebelOperation.Action = "UpdateOrCreate";
                SiebelOperation.Object = new NewSDTApplication.ServiceOptimizationService.BaseObjectWrapper();
                SiebelOperation.Object.Object = (NewSDTApplication.ServiceOptimizationService.BaseObject)objSiebelTask;

                //TAke Call ID from parameter Passed// (Selected Site Parameter from Site DEpendency Grid)
                objSiebelTask.CallID = CallID;
                //Bump Task Super Power Task flag
                if (HttpContext.Current.Session["IsBumpTask"] != null)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["IsBumpTask"] as string))
                    {
                        if (HttpContext.Current.Session["IsBumpTask"].ToString() == "Yes")
                        {
                            objSiebelTask.SuperPowerTaskFlag = true;
                            objSiebelTask.SuperPowerTaskFlagSpecified = true;
                        }
                    }
                }
                //TAke Number from parameter Passed//
                objSiebelTask.Number = 1; // pass the parameter Number from method parameter  + 1;
                objSiebelTask.NumberSpecified = true;
                objSiebelTask.PrioritySpecified = true;
                //This is critical because we have to create successful dependency otherwise it will be not assigned on Gantt//
                objSiebelTask.Critical = true;
                objSiebelTask.CriticalSpecified = true;
                objSiebelTask.MUSTJobNumber = res.serviceRequest.srNumber;
                objSiebelTask.IsMST = true;
                objSiebelTask.IsMSTSpecified = true;


                if ((res != null) && (res.serviceRequest != null) && !string.IsNullOrEmpty(res.serviceRequest.gEHCSafetyConcern))
                {
                    objSiebelTask.IsSafety = objSiebelToClick.GetSafetyConcernValue();
                    objSiebelTask.IsSafetySpecified = true;
                }
                //CustomerExpectation Added by phani kanth p
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Schedulingpolicy"] as string))
                {
                    objSiebelTask.SchedulingPolicy = Convert.ToString(HttpContext.Current.Session["Schedulingpolicy"]);
                }
                objSiebelTask.CustomerExpectation = "B";
                if (!string.IsNullOrEmpty(earlystart))
                {

                    if (res.serviceRequest.srType == "Installation")
                    {
                        objSiebelTask.EarlyStart = Convert.ToDateTime(earlystart);
                        objSiebelTask.IsMDT = true;
                        objSiebelTask.IsMDTSpecified = true;
                        //  objSiebelTask.CustomerExpectation = "B";

                    }
                    else
                        objSiebelTask.EarlyStart = Convert.ToDateTime(DateTime.Parse(earlystart, CultureInfo.GetCultureInfo("en-gb")));
                    objSiebelTask.EarlyStartSpecified = true;
                }
                else
                {
                    //objSiebelTask.EarlyStart = DateTime.Now;
                    objSiebelTask.EarlyStartSpecified = false;
                }
                if (!string.IsNullOrEmpty(latestart))
                {
                    if (res.serviceRequest.srType == "Installation")
                        objSiebelTask.LateStart = Convert.ToDateTime(latestart);
                    else
                        objSiebelTask.LateStart = Convert.ToDateTime(DateTime.Parse(latestart, CultureInfo.GetCultureInfo("en-gb")));
                    objSiebelTask.LateStartSpecified = true;
                }
                else
                {
                    //objSiebelTask.LateStart = DateTime.Now.AddDays(10);
                    objSiebelTask.LateStartSpecified = false;
                }
                //if (!string.IsNullOrEmpty(fseSkill))
                //{
                //    objSiebelTask.SkillLevel = Convert.ToInt32(fseSkill);
                //    objSiebelTask.SkillLevelSpecified = true;
                //}
                if (!string.IsNullOrEmpty(fseSkill))
                {
                    objSiebelTask.SkillLevel = Convert.ToInt32(fseSkill);
                    objSiebelTask.SkillLevelSpecified = true;
                }

                if (!string.IsNullOrEmpty(PreferredFSEs))
                {
                    objSiebelTask.PreferredFSEs = PreferredFSEs;
                }
                else
                {
                    objSiebelTask.PreferredFSEs = "";
                }
                if (!string.IsNullOrEmpty(PreferredFSEs))
                {
                    if (HttpContext.Current.Session["IsRequiredFse"].ToString() != null)
                    {
                        if (HttpContext.Current.Session["IsRequiredFse"].ToString() == "true") objSiebelTask.RequiredFSEs = PreferredFSEs;
                        else objSiebelTask.RequiredFSEs = "";
                    }
                    //processTaskExRequest.Task.PreferredFSEs = PreferredFSEs;
                }
                else
                {
                    objSiebelTask.RequiredFSEs = "";
                    objSiebelTask.PreferredFSEs = "";
                }

                //  string systemstatus = objSiebelToClick.GetSystemStatus(res.serviceRequest.currentEquiptmentStatus);
                //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]

                if (!string.IsNullOrEmpty(res.serviceRequest.currentEquiptmentStatus))
                    objSiebelTask.SystemStatus = new NewSDTApplication.ServiceOptimizationService.GEHCSystemStatusReference() { Name = systemstatus, DisplayString = systemstatus };
                objSiebelTask.Priority = GetPriority();

                HttpContext.Current.Session["systemstatus"] = null;

                objSiebelTask.Duration = duration * 60;

                objSiebelTask.DurationSpecified = true;
                NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference CRMReference = new NewSDTApplication.ServiceOptimizationService.GEHCCRMSystemReference() { Name = "SIEBEL-INTL-APAC" };
                objSiebelTask.CRMSystemName = CRMReference;
                NewSDTApplication.ServiceOptimizationService.GEHCSystemReference SystemReference = new NewSDTApplication.ServiceOptimizationService.GEHCSystemReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).SystemID };
                objSiebelTask.SystemID = SystemReference;
                NewSDTApplication.ServiceOptimizationService.GEHCSiteReference SiteReference = new NewSDTApplication.ServiceOptimizationService.GEHCSiteReference() { ID = ((HTTPPostParams)HttpContext.Current.Session["SiebelHttpPostParams"]).ShipToSite };
                //SiteID comment by Phani Kanth
                //objSiebelTask.SiteID = SiteReference;
                // TrasformSiebelToClick objSiebelToClick = new TrasformSiebelToClick();
                if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                {
                    string taskType = string.Empty;
                    taskType = objSiebelToClick.GetTaskType(res.serviceRequest.srType);
                    if (!string.IsNullOrEmpty(taskType))
                        objSiebelTask.TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference() { Name = taskType };
                }

                if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))
                {
                    string taskSubType = string.Empty;
                    taskSubType = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType);
                    if (!string.IsNullOrEmpty(taskSubType))
                        objSiebelTask.TaskSubType = new NewSDTApplication.ServiceOptimizationService.GEHCTaskSubTypeReference() { Name = taskSubType };
                }
                //if (!string.IsNullOrEmpty(res.serviceRequest.srType))
                //    objSiebelTask.TaskType = new NewSDTApplication.ServiceOptimizationService.TaskTypeReference { Name = objSiebelToClick.GetTaskType(res.serviceRequest.srType) };
                //if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))
                //    objSiebelTask.TaskSubType = new NewSDTApplication.ServiceOptimizationService.GEHCTaskSubTypeReference() { Name = objSiebelToClick.GetTaskSubType(res.serviceRequest.srSubType) };
                //task.DueDate = System.DateTime.Parse("2014-09-30T09:00:00");
                // Created date is missing in the API, we need to change once the API sends that value
                objCountriesTimeZoneConversion = new CountriesTimeZoneConversion();
                objSiebelTask.OpenDate = Convert.ToDateTime(DateTime.Parse(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), CultureInfo.GetCultureInfo("en-gb")));
                //objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);
                objSiebelTask.OpenDateSpecified = true;
                //objSiebelTask.OpenDate = objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress);//will change after conferming from rajesh
                //objSiebelTask.OpenDateSpecified = true;
                //Commented as per the discussion with Manjula on 12-04-2016- Rajesh
                //objSiebelTask.NumberOfRequiredEngineers = 1;
                //objSiebelTask.NumberOfRequiredEngineersSpecified = true;
                objSiebelTask.OwnerName = res.serviceRequest.activityDetailList[0].ownerFirstName + " " + res.serviceRequest.activityDetailList[0].ownerLastName;
                objSiebelTask.OwnerSSO = res.serviceRequest.activityDetailList[0].primaryOwnedBy;
                if (res.serviceRequest.srType == "FMI")
                {
                    if (!string.IsNullOrEmpty(res.serviceRequest.fmiDueDate))
                    {
                        objSiebelTask.FMIDueDate = Convert.ToDateTime(DateTime.Parse(res.serviceRequest.fmiDueDate, CultureInfo.GetCultureInfo("en-gb")));
                        objSiebelTask.FMIDueDateSpecified = true;
                    }
                    objSiebelTask.FMINumber = res.serviceRequest.fMINumber;

                }
                objSiebelTask.Notes = TaskNotes;
                objSiebelTask.TaskID = res.serviceRequest.activityDetailList[0].activityId;
                objSiebelTask.JobComments = new NewSDTApplication.ServiceOptimizationService.GEHCJobCommentsReference() { MUSTJobNumber = objSiebelTask.MUSTJobNumber };

                NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency[] TimedependencyPartpickup = new ServiceOptimizationService.TaskTaskTimeDependency[list.Count];
                NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[] EngineerdependencyPartPickUp = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency[list.Count];
                NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency[] TimedependencyPartpickup1 = new ServiceOptimizationService.TaskTaskTimeDependency[list.Count];
                NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency[] TimedependencyPartpickup2 = new ServiceOptimizationService.TaskTaskTimeDependency[list.Count];
                //Changes done by Raju
                //Removed Split to allow the all special char
                for (int i = 0; i < list.Count; i++)
                {
                    int number = 0;
                    // string[] addressArray = Address[i] as string[];
                    string[] addressArray = list[i] as string[];
                    if (addressArray.Length > 9)
                    {
                        if (!string.IsNullOrEmpty(addressArray[9].Trim()))
                        {
                            addressArray[9] = addressArray[9].Trim().Replace("\"", "");

                            number = Convert.ToInt32(addressArray[9]);


                        }
                        else
                        {
                            number = i + 2;

                        }
                    }
                    else
                    {
                        number = i + 2;

                    }
                    if (addressArray.Length > 11)
                    {
                        if (!string.IsNullOrEmpty(addressArray[11].Trim()))
                        {
                            addressArray[11] = addressArray[11].Trim().Replace("\"", "");


                            //ProcessTaskExRequest processTaskExRequest1 = new ProcessTaskExRequest();

                            //processTaskExRequest1.Task.Status =
                            if (addressArray[11] == "true")
                            {
                                TimedependencyPartpickup1[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                                TimedependencyPartpickup1[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                                // Changed critical to true as per the discussion with click team on 04/05/2016
                                TimedependencyPartpickup1[i].Critical = true;
                                TimedependencyPartpickup1[i].CriticalSpecified = true;
                                //changed to relationType 4 to 0 .Changes done by Phani
                                TimedependencyPartpickup1[i].RelationType = 0;
                                TimedependencyPartpickup1[i].RelationTypeSpecified = true;

                                // TimedependencyPartpickup = TimedependencyPartpickup1;


                                TimedependencyPartpickup2[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                                TimedependencyPartpickup2[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                                // Changed critical to true as per the discussion with click team on 04/05/2016
                                TimedependencyPartpickup2[i].Critical = true;
                                TimedependencyPartpickup2[i].CriticalSpecified = true;
                                //changed to relationType 4 to 0 .Changes done by Phani
                                TimedependencyPartpickup2[i].RelationType = 4;
                                TimedependencyPartpickup2[i].RelationTypeSpecified = true;
                                //  TimedependencyPartpickup[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();


                                //TimedependencyPartpickup[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                                EngineerdependencyPartPickUp[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency();
                                //  TimedependencyPartpickup[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = i + 2, NumberSpecified = true };
                                // Changed critical to true as per the discussion with click team on 04/05/2016
                                //TimedependencyPartpickup[i].Critical = true;
                                //  TimedependencyPartpickup[i].CriticalSpecified = true;
                                //changed to relationType 4 to 0 .Changes done by Phani
                                //  TimedependencyPartpickup[i].RelationType = 0;
                                //  TimedependencyPartpickup[i].RelationTypeSpecified = true;
                                EngineerdependencyPartPickUp[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                                EngineerdependencyPartPickUp[i].TaskKey.NumberSpecified = true;
                                EngineerdependencyPartPickUp[i].RelationType = 1;
                                EngineerdependencyPartPickUp[i].RelationTypeSpecified = true;

                            }


                        }
                        else
                        {

                            TimedependencyPartpickup1[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                            TimedependencyPartpickup1[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                            // Changed critical to true as per the discussion with click team on 04/05/2016
                            TimedependencyPartpickup1[i].Critical = true;
                            TimedependencyPartpickup1[i].CriticalSpecified = true;
                            //changed to relationType 4 to 0 .Changes done by Phani
                            TimedependencyPartpickup1[i].RelationType = 0;
                            TimedependencyPartpickup1[i].RelationTypeSpecified = true;

                            // TimedependencyPartpickup = TimedependencyPartpickup1;


                            TimedependencyPartpickup2[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                            TimedependencyPartpickup2[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                            // Changed critical to true as per the discussion with click team on 04/05/2016
                            TimedependencyPartpickup2[i].Critical = true;
                            TimedependencyPartpickup2[i].CriticalSpecified = true;
                            //changed to relationType 4 to 0 .Changes done by Phani
                            TimedependencyPartpickup2[i].RelationType = 4;
                            TimedependencyPartpickup2[i].RelationTypeSpecified = true;
                            //  TimedependencyPartpickup[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();


                            //TimedependencyPartpickup[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                            EngineerdependencyPartPickUp[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency();
                            //  TimedependencyPartpickup[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = i + 2, NumberSpecified = true };
                            // Changed critical to true as per the discussion with click team on 04/05/2016
                            //TimedependencyPartpickup[i].Critical = true;
                            //  TimedependencyPartpickup[i].CriticalSpecified = true;
                            //changed to relationType 4 to 0 .Changes done by Phani
                            //  TimedependencyPartpickup[i].RelationType = 0;
                            //  TimedependencyPartpickup[i].RelationTypeSpecified = true;
                            EngineerdependencyPartPickUp[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                            EngineerdependencyPartPickUp[i].TaskKey.NumberSpecified = true;
                            EngineerdependencyPartPickUp[i].RelationType = 1;
                            EngineerdependencyPartPickUp[i].RelationTypeSpecified = true;
                        }


                    }
                    else
                    {

                        TimedependencyPartpickup1[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                        TimedependencyPartpickup1[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                        // Changed critical to true as per the discussion with click team on 04/05/2016
                        TimedependencyPartpickup1[i].Critical = true;
                        TimedependencyPartpickup1[i].CriticalSpecified = true;
                        //changed to relationType 4 to 0 .Changes done by Phani
                        TimedependencyPartpickup1[i].RelationType = 0;
                        TimedependencyPartpickup1[i].RelationTypeSpecified = true;

                        // TimedependencyPartpickup = TimedependencyPartpickup1;


                        TimedependencyPartpickup2[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                        TimedependencyPartpickup2[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                        // Changed critical to true as per the discussion with click team on 04/05/2016
                        TimedependencyPartpickup2[i].Critical = true;
                        TimedependencyPartpickup2[i].CriticalSpecified = true;
                        //changed to relationType 4 to 0 .Changes done by Phani
                        TimedependencyPartpickup2[i].RelationType = 4;
                        TimedependencyPartpickup2[i].RelationTypeSpecified = true;
                        //  TimedependencyPartpickup[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();


                        //TimedependencyPartpickup[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskTimeDependency();
                        EngineerdependencyPartPickUp[i] = new NewSDTApplication.ServiceOptimizationService.TaskTaskEngineerDependency();
                        //  TimedependencyPartpickup[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = i + 2, NumberSpecified = true };
                        // Changed critical to true as per the discussion with click team on 04/05/2016
                        //TimedependencyPartpickup[i].Critical = true;
                        //  TimedependencyPartpickup[i].CriticalSpecified = true;
                        //changed to relationType 4 to 0 .Changes done by Phani
                        //  TimedependencyPartpickup[i].RelationType = 0;
                        //  TimedependencyPartpickup[i].RelationTypeSpecified = true;
                        EngineerdependencyPartPickUp[i].TaskKey = new NewSDTApplication.ServiceOptimizationService.TaskReference() { CallID = CallID, Number = number, NumberSpecified = true };
                        EngineerdependencyPartPickUp[i].TaskKey.NumberSpecified = true;
                        EngineerdependencyPartPickUp[i].RelationType = 1;
                        EngineerdependencyPartPickUp[i].RelationTypeSpecified = true;
                    }


                }
                TimedependencyPartpickup = TimedependencyPartpickup1.Concat(TimedependencyPartpickup2).ToArray();
                objSiebelTask.TimeDependencies = TimedependencyPartpickup;
                objSiebelTask.EngineerDependencies = EngineerdependencyPartPickUp;
                objSiebelTask.ContactName = res.serviceRequest.contactFirstName + " " + res.serviceRequest.contactLastName;
                objSiebelTask.ContactPhoneNumber = res.serviceRequest.gEHCWorkPhone;
                objSiebelTask.CustomerEmail = res.serviceRequest.contactEmail;
                if (!String.IsNullOrEmpty(AppStart))
                {
                    objSiebelTask.AppointmentStart = Convert.ToDateTime(DateTime.Parse(AppStart, CultureInfo.GetCultureInfo("en-gb")));
                    objSiebelTask.AppointmentStartSpecified = true;
                }
                if (!String.IsNullOrEmpty(AppFinish))
                {
                    objSiebelTask.AppointmentFinish = Convert.ToDateTime(DateTime.Parse(AppFinish, CultureInfo.GetCultureInfo("en-gb")));
                    objSiebelTask.AppointmentFinishSpecified = true;
                }
                //objSiebelTask.AppointmentStart
                standardOperations.Add(SiebelOperation);

                DateTime startTime = DateTime.Now;
                var results = ServiceOptimizationService.ExecuteMultipleOperations(multipleOPeration.OptionalParameters, standardOperations.ToArray(), true, false);

                DateTime endTime = DateTime.Now;

                TimeSpan span = endTime.Subtract(startTime);
                logger.Debug("CallClickSerrvice.cs;PartpickDependency;ExecuteMultipleOperations TimeSpan in Sec ;" + span.Minutes * 60 + span.Seconds);
                //ProcessTaskExAsync method added by Phani Kanth



                processTaskExRequest.Task = new ScheduleServiceDev1.Task();
                processTaskExRequest.Task.CallID = CallID;
                processTaskExRequest.Task.MacroVersion = "1";//Convert.ToString(objCountriesTimeZoneConversion.GetTimeZone(res.serviceRequest.countryCode, res.serviceRequest.shipToAddress));
                processTaskExRequest.Task.Number = 1;
                processTaskExRequest.Task.NumberSpecified = true;

                processTaskExRequest.SchedulingWorkflow = "Schedule Workflow";
                DateTime startTime1 = DateTime.Now;

                ScheduleService.ProcessTaskExAsync(processTaskExRequest);

                DateTime endTime1 = DateTime.Now;

                TimeSpan span1 = endTime1.Subtract(startTime1);
                logger.Debug("CallClickSerrvice.cs;PartpickDependency;ProcessTaskExAsync TimeSpan in Sec ;" + span1.Minutes * 60 + span1.Seconds);
                logger.Debug("CallClickSerrvice.cs;PartPickDependency; results: Success");
                return true;

            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;PartPickDependency; Exception: " + ex.Message);
                throw;
            }


        }

        public List<EngineerScheduleAppointment> GetTasksResponse(string date, string appStart, string appFinish, bool requiredFse, string TentativeFse, string SSOID)
        {
            List<EngineerScheduleAppointment> ObjEngineerList = new List<EngineerScheduleAppointment>();
            OpenConnectionToSDT();
            try
            {


                NewSDTApplication.ScheduleServiceDev1.GetResourceScheduleRequest ResourseDetails = new NewSDTApplication.ScheduleServiceDev1.GetResourceScheduleRequest();
                NewSDTApplication.ScheduleServiceDev1.OptionalParameters Opetionalparam = new NewSDTApplication.ScheduleServiceDev1.OptionalParameters();
                NewSDTApplication.ScheduleServiceDev1.EngineerReference Engineer = new NewSDTApplication.ScheduleServiceDev1.EngineerReference();
                NewSDTApplication.ScheduleServiceDev1.TimeInterval ResourseScheduleTime = new NewSDTApplication.ScheduleServiceDev1.TimeInterval();
                ResourseDetails.TaskRequestedProperties = new string[] { "CallID",
                                                                "TaskSystemName",
                                                                "Number",
                                                                "MUSTJobNumber",
                                                                "TaskType",
                                                                "SystemID",
                                                                "TaskSiteID",
                                                                "TaskSiteName",
                                                                "Status",
                                                                "SkillLevel",
                                                                "TimeDependencies",
                                                                "TaskSystemModality",
                                                                "TaskSystemProductName",
                                                                "Duration",
                                                                "DurationSpecified",
                                                                "TaskSystemID",
                                                                "LateStart",
                                                                "EarlyStart",
                                                                "Key",
                                                                "IsMST",
                                                                "JeopardyState",
                                                                "KeySpecified",
                                                                "Priority",
                                                                "PrioritySpecified",
                                                                "TaskSystemProductID",
                                                                "AppointmentFinish",
                                                                "AppointmentStart",
                                                                "TaskID",
                                                                "RequiredFSEs",
                                                                "PartDeliveryType",
                                                                "PartEstimatedDeliveryDate",
                                                                "PartComment",
                                                                "Street",
                                                                "City",
                                                                "Postcode",
                                                                "CountryID",
                                                                "Notes",
                                                                "PreferredFSEs",
                                                                "Latitude",
                                                                "Longitude",
                                                                "SystemStatus"

                                                               };

                ResourseDetails.AssignmentRequestedProperties = new string[] { "Start",
                                                                          "Finish",
                                                                          "Task",
                                                                          "AssignedEngineers",
                                                                          "Engineers"
                                            };
                Opetionalparam.CallerIdentity = "Must";
                Opetionalparam.ErrorOnNonExistingDictionaries = true;
                Engineer.Key = Convert.ToInt32(SSOID);
                Engineer.KeySpecified = true;
                ResourseScheduleTime.Start = Convert.ToDateTime(date + " " + appStart);
                ResourseScheduleTime.Finish = Convert.ToDateTime(date + " " + appFinish);
                var getTasksResponse = ScheduleService.GetResourceSchedule(Opetionalparam, Engineer, ResourseScheduleTime, true, ResourseDetails.TaskRequestedProperties, ResourseDetails.AssignmentRequestedProperties);

                return getTasksResponse.ToList();

            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;GetResourseDetails; Exception: " + ex.Message);
                return ObjEngineerList;
            }


        }
        public List<EngineerScheduleAppointment> BumpCheck(string appStart, string appFinish, bool requiredFse, string TentativeFse, string SSOID)
        {
            List<EngineerScheduleAppointment> ObjEngineerList = new List<EngineerScheduleAppointment>();
            OpenConnectionToSDT();
            try
            {


                NewSDTApplication.ScheduleServiceDev1.GetResourceScheduleRequest ResourseDetails = new NewSDTApplication.ScheduleServiceDev1.GetResourceScheduleRequest();
                NewSDTApplication.ScheduleServiceDev1.OptionalParameters Opetionalparam = new NewSDTApplication.ScheduleServiceDev1.OptionalParameters();
                NewSDTApplication.ScheduleServiceDev1.EngineerReference Engineer = new NewSDTApplication.ScheduleServiceDev1.EngineerReference();
                NewSDTApplication.ScheduleServiceDev1.TimeInterval ResourseScheduleTime = new NewSDTApplication.ScheduleServiceDev1.TimeInterval();
                ResourseDetails.TaskRequestedProperties = new string[] { "CallID",
                                                                "TaskSystemName",
                                                                "Number",
                                                                "MUSTJobNumber",
                                                                "TaskType",
                                                                "SystemID",
                                                                "TaskSiteID",
                                                                "TaskSiteName",
                                                                "Status",
                                                                "SkillLevel",
                                                                "TimeDependencies",
                                                                "TaskSystemModality",
                                                                "TaskSystemProductName",
                                                                "Duration",
                                                                "DurationSpecified",
                                                                "TaskSystemID",
                                                                "LateStart",
                                                                "EarlyStart",
                                                                "Key",
                                                                "IsMST",
                                                                "JeopardyState",
                                                                "KeySpecified",
                                                                "Priority",
                                                                "PrioritySpecified",
                                                                "TaskSystemProductID",
                                                                "AppointmentFinish",
                                                                "AppointmentStart",
                                                                "TaskID",
                                                                "RequiredFSEs",
                                                                "PartDeliveryType",
                                                                "PartEstimatedDeliveryDate",
                                                                "PartComment",
                                                                "Street",
                                                                "City",
                                                                "Postcode",
                                                                "CountryID",
                                                                "Notes",
                                                                "PreferredFSEs",
                                                                "Latitude",
                                                                "Longitude",
                                                                "SystemStatus"

                                                               };

                ResourseDetails.AssignmentRequestedProperties = new string[] { "Start",
                                                                          "Finish",
                                                                          "Task",
                                                                          "AssignedEngineers",
                                                                          "Engineers"
                                            };
                Opetionalparam.CallerIdentity = "Must";
                Opetionalparam.ErrorOnNonExistingDictionaries = true;
                Engineer.Key = Convert.ToInt32(SSOID);
                Engineer.KeySpecified = true;
                ResourseScheduleTime.Start = Convert.ToDateTime(appStart);
                ResourseScheduleTime.Finish = Convert.ToDateTime(appFinish);
                var getTasksResponse = ScheduleService.GetResourceSchedule(Opetionalparam, Engineer, ResourseScheduleTime, true, ResourseDetails.TaskRequestedProperties, ResourseDetails.AssignmentRequestedProperties);
                return getTasksResponse.ToList();

            }
            catch (Exception ex)
            {
                logger.Error("CallClickSerrvice.cs;GetResourseDetails; Exception: " + ex.Message);
                return ObjEngineerList;
            }


        }
    }
}