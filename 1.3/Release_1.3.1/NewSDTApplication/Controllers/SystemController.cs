using NewSDTApplication.Models;
using NewSDTApplication.ScheduleServiceDev1;
using NewSDTApplication.Utilities;
using Newtonsoft.Json;
using SDTLogger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NewSDTApplication.Controllers
{
    [SessionExpire]
    public class SystemController : Controller
    {
        private ScheduleServiceDev1.Task task;

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult SystemDependencies()
        {
            IndextoRequest resindextoreq1 = new IndextoRequest();
            resindextoreq1 = (IndextoRequest)Session["IndextoRequest"];
            if (resindextoreq1.DependencyType == "PotentialDependency")
            {
                ViewBag.SystemPotentialDependency = "Potential Same System Dependencies";
            }
            else
            {
                ViewBag.SystemPotentialDependency = "Linked Same System Dependencies";
            }
            return View();
        }

        [HttpPost]
        public JsonResult SystemDependencies(string TaskSystemID, string SystemCountActual, string EStart, string LStart, string Duration, string TaskNotes, string DependencyType,
           List<List<Array>> addressArray, string Profile, string SSOFse1, string fseSkill, string IsRequiredfse, string JobType)
        {
            try
            {

                if (JobType == "Installation")
                {
                    EStart = (Convert.ToDateTime(DateTime.Parse(EStart, CultureInfo.GetCultureInfo("en-gb")).ToString())).ToString();
                    LStart = (Convert.ToDateTime(EStart).AddDays(3)).ToString();


                    // For Installlation to maintain values when navigated between pages.

                    if (!string.IsNullOrEmpty(Session["SDTHomeEarlyStart"] as string))
                        Session["SDTHomeEarlyStart"] = null;
                    if (!string.IsNullOrEmpty(Session["SDTHomeLateStart"] as string))
                        Session["SDTHomeLateStart"] = null;
                    if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                        Session["SDTHomeDuration"] = null;

                    Session["SDTHomeEarlyStart"] = EStart;
                    Session["SDTHomeLateStart"] = LStart;
                    Session["SDTHomeDuration"] = Duration;
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
                    if (!string.IsNullOrEmpty(Session["SDTHomeDuration"] as string))
                        Session["SDTHomeDuration"] = null;

                    Session["SDTHomeEarlyStart"] = EStart;
                    Session["SDTHomeLateStart"] = LStart;
                    Session["SDTHomeDuration"] = Duration;

                    logger.Debug("SystemController;SystemDependencies -2 ;Start - Parameters EStart:" + EStart + " LStart:" + LStart + " Duration:" + Duration + " Profile:" + Profile + " TaskNotes:" + TaskNotes);
                }
                ViewBag.SystemActualCount = SystemCountActual;
                Session["SystemCountActual"] = SystemCountActual;
                IndextoRequest resindextoreq = new IndextoRequest();
                resindextoreq.TaskSystemID = TaskSystemID;
                resindextoreq.earlyStart = EStart;
                resindextoreq.lateStart = LStart;
                resindextoreq.TaskNotes = TaskNotes;
                if (!string.IsNullOrEmpty(Duration as string))
                {
                    TempData["duration"] = resindextoreq.Duration = Convert.ToInt32(Duration);

                }
                resindextoreq.FseSkillLevel = fseSkill;
                resindextoreq.DependencyType = DependencyType;

                //barnali added this part pick up

                //, string desiredDate, string partcomments

                resindextoreq.addressArray = addressArray;

                //if (!string.IsNullOrEmpty(desiredDate))
                //resindextoreq.desiredDate = desiredDate;

                //if (!string.IsNullOrEmpty(partcomments))
                //    resindextoreq.PartComments = partcomments;


                //if (!string.IsNullOrEmpty(Session["PartToolTempAddr"] as string))
                //    Session["PartToolTempAddr"] = null;
                Session["PartToolReturn"] = "true";
                if (Session["PartToolTempAddr"] != null)
                    Session["PartToolTempAddr"] = null;
                Session["PartToolTempAddr"] = resindextoreq.addressArray;


                if (!string.IsNullOrEmpty(Session["SystemDependencies"] as string))
                    Session["SystemDependencies"] = null;

                Session["SystemDependencies"] = resindextoreq.DependencyType;

                Session["IndextoRequest"] = resindextoreq;

                //to maintain the same values when navigated between pages.

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


                Session["SDTHomeSSOFse1"] = SSOFse1;
                Session["SDTHomeTaskNotes"] = TaskNotes;

                Session["SDTHomeProfile"] = Profile;
                Session["SDTHomeFseSkill"] = fseSkill;
                Session["SDTHomeIsRequiredfse"] = IsRequiredfse;
            }
            catch (Exception ex)
            {
                //Logger.Error("Fault Exception thrown", ex);
                logger.Error("SystemController;SystemDependencies; Exception:" + ex.Message);

                return Json(new { HasError = true, Message = "Failure" });
            }
            return Json(new { HasError = false, Message = "Success" });
        }

        [HttpGet]
        public async Task<JsonResult> GetSystemDependencies()
        {
            #region Site Dependency code changes done by phani Kanth P.
            try
            {
                List<ScheduleServiceDev1.Task> Tasklist = new List<ScheduleServiceDev1.Task>();
                List<CustomTasksList> objCustomtasksList = new List<CustomTasksList>();
                List<ScheduleServiceDev1.Assignment> Assignmentlist = new List<ScheduleServiceDev1.Assignment>();

                IndextoRequest resindextoreq = new IndextoRequest();
                resindextoreq = (IndextoRequest)Session["IndextoRequest"];
                CallClickSerrvice objClickCallService = new CallClickSerrvice();
                ScheduleServiceDev1.GetTasksResponse objtaskResponse = new ScheduleServiceDev1.GetTasksResponse();

                //Rating Logic
                var TaskSystemModality = "";
                var TaskSystemProductName = "";
                var SkillLevel = "";
                int level = 0;

                string callID = string.Empty;
                objtaskResponse = (ScheduleServiceDev1.GetTasksResponse)Session["TaskResponseByTask"];
                if (objtaskResponse != null && objtaskResponse.Tasks.Count() > 0)
                {
                    callID = objtaskResponse.Tasks[0].CallID;
                }
                logger.Debug("In Site controller GetSiteDependencies Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) with SystemID: " + resindextoreq.TaskSystemID);
                DateTime startTimeTx1 = DateTime.Now;
                var lst = await objClickCallService.GetTasksRequestByPropertyName(resindextoreq.TaskSystemID, "TaskSystemID");
                DateTime endTimeTx1 = DateTime.Now;
                TimeSpan Tx1 = endTimeTx1.Subtract(startTimeTx1);
                logger.Debug("In Site controller GetSiteDependencies Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) | Call duration: " + Tx1.Milliseconds);
                Tasklist = lst.Tasks.Where(x => x.IsMST == false && x.TaskType.DisplayString != "Parts Pickup" && (x.Status.DisplayString == "New" || x.Status.DisplayString == "Tentative") && x.TaskID != ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).ActivityNo).ToList();
                if (Tasklist.Count > 0)
                {
                    foreach (var item in Tasklist)
                    {
                        CustomTasksList objCustomTask = new CustomTasksList();
                        objCustomTask.Sitename = item.TaskSiteName;
                        objCustomTask.Systemname = item.TaskSystemName;
                        objCustomTask.SystemID = item.TaskSystemID; //Added by AJaySara
                        objCustomTask.TaskID = item.TaskID; //Added by AJaySara
                        objCustomTask.EarlyStart = Convert.ToDateTime(item.EarlyStart).ToString();

                        //Added by Phani Kanth P.
                        if (item.AppointmentStart.ToString() == DefaultClickDates.DefaultClickDate)
                        {
                            objCustomTask.AppointmentStart = "";
                        }
                        else
                        {
                            objCustomTask.AppointmentStart = Convert.ToDateTime(item.AppointmentStart).ToString();
                        }
                        if (item.AppointmentFinish.ToString() == DefaultClickDates.DefaultClickDate)
                        {
                            objCustomTask.AppointmentFinish = "";
                        }
                        else
                        {
                            objCustomTask.AppointmentFinish = Convert.ToDateTime(item.AppointmentFinish).ToString();
                        }

                        Assignmentlist = lst.Assignments.Where(x => x.Task.CallID == item.CallID).ToList();
                        if (Assignmentlist.Count > 0)
                        {
                            objCustomTask.AssignedFSE = Assignmentlist[0].AssignedEngineers;
                            objCustomTask.AssignmentStart = Convert.ToDateTime(Assignmentlist[0].Start).ToString();
                            objCustomTask.AssignmentFinish = Convert.ToDateTime(Assignmentlist[0].Finish).ToString();
                        }
                        objCustomTask.Tasktype = item.TaskType.DisplayString;
                        objCustomTask.Duration = item.Duration.ToString();
                        objCustomTask.Level = item.SkillLevel.ToString();
                        objCustomTask.ProductID = item.TaskSystemProductID.DisplayString;
                        objCustomTask.RFS = item.RequiredFSEs;
                        objCustomTask.Status = item.Status.DisplayString;
                        objCustomTask.CallID = item.CallID;
                        objCustomTask.TaskNumber = item.Number.ToString();
                        objCustomTask.SRnumber = item.MUSTJobNumber;
                        objCustomTask.IsMst = item.IsMST;
                        objCustomTask.Dependency = "Yes";
                        #region Rating logic implemented by Phani Kanth P.

                        GetTasksResponse taskResponse = new GetTasksResponse();
                        task = new ScheduleServiceDev1.Task();
                        taskResponse = (ScheduleServiceDev1.GetTasksResponse)Session["TaskResponseByTask"];
                        if (Session["taskStatus"] != "New")
                        {

                            task = taskResponse.Tasks[0];
                            TaskSystemModality = task.TaskSystemModality;
                            TaskSystemProductName = task.TaskSystemProductName;
                            SkillLevel = Convert.ToString(task.SkillLevel);
                        }
                        else
                        {
                            if (Session["Systemrating"] != null)
                            {
                                TaskSystemModality = ((NewSDTApplication.ServiceOptimizationService.GEHCSystem)(Session["Systemrating"])).Modality;
                                TaskSystemProductName = ((NewSDTApplication.ServiceOptimizationService.GEHCSystem)(Session["Systemrating"])).ProductName;
                                // SkillLevel = ((NewSDTApplication.Models.SiebelJsonToEntity)(Session["SiebelData"])).serviceRequest.gehcFse1;
                                SkillLevel = ((IndextoRequest)Session["IndextoRequest"]).FseSkillLevel;

                                if (!string.IsNullOrEmpty(SkillLevel))
                                {
                                    var sLevel = SkillLevel.Replace("Level", "").Trim();
                                    if (sLevel == "None")
                                    {
                                        SkillLevel = "0";
                                    }
                                    else if (sLevel == "Any FE")
                                    {
                                        SkillLevel = "0";
                                    }
                                    else
                                    {
                                        SkillLevel = sLevel;

                                    }
                                }
                                else
                                {
                                    SkillLevel = "0";
                                }
                            }

                        }
                        objCustomTask.Rating = "*";
                        if (item.TaskSystemModality == TaskSystemModality)
                        {
                            objCustomTask.Rating = "**";
                        }
                        if (item.TaskSystemModality == TaskSystemModality && item.TaskSystemProductName == TaskSystemProductName)
                        {
                            objCustomTask.Rating = "***";
                        }

                        if (item.TaskSystemModality == TaskSystemModality && item.TaskSystemProductName == TaskSystemProductName && item.SkillLevel == Convert.ToInt32(SkillLevel))
                        {
                            objCustomTask.Rating = "****";
                        }
                        #endregion

                        objCustomtasksList.Add(objCustomTask);

                    }
                    var jsonData = new
                    {
                        total = 1,
                        page = 1,
                        records = 10,
                        rows = objCustomtasksList,
                    };

                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("SystemController;GetSystemDependencies; Exception:" + ex.Message);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            #endregion
        }



        [HttpGet]
        public async Task<JsonResult> GetActualSystemDependencies()
        {
            try
            {
                List<CustomTasksList> objCustomtasksList = new List<CustomTasksList>();
                List<ScheduleServiceDev1.Task> Tasklist = new List<ScheduleServiceDev1.Task>();
                List<ScheduleServiceDev1.Assignment> Assignmentlist = new List<ScheduleServiceDev1.Assignment>();

                IndextoRequest resindextoreq = new IndextoRequest();
                resindextoreq = (IndextoRequest)Session["IndextoRequest"];
                CallClickSerrvice objClickCallService = new CallClickSerrvice();
                ScheduleServiceDev1.GetTasksResponse objtaskResponse = new ScheduleServiceDev1.GetTasksResponse();
                string callID = string.Empty;

                //Rating Logic
                var TaskSystemModality = "";
                var TaskSystemProductName = "";
                var SkillLevel = "";
                int level = 0;

                objtaskResponse = (ScheduleServiceDev1.GetTasksResponse)Session["TaskResponseByTask"];
                if (objtaskResponse != null && objtaskResponse.Tasks.Count() > 0)
                {
                    callID = objtaskResponse.Tasks[0].CallID;
                }
                                
                logger.Debug("In Site controller GetSiteDependencies Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) with SystemID: " + resindextoreq.TaskSystemID);
                DateTime startTimeTx2 = DateTime.Now;
                var lst = await objClickCallService.GetTasksRequestByPropertyName(resindextoreq.TaskSystemID, "TaskSystemID");
                DateTime endTimeTx2 = DateTime.Now;
                TimeSpan Tx2 = endTimeTx2.Subtract(startTimeTx2);
                logger.Debug("In Site controller GetSiteDependencies Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) | Call duration: " + Tx2.Milliseconds);
                Tasklist = lst.Tasks.Where(x => x.IsMST == true && x.TaskType.DisplayString != "Parts Pickup" && (x.Status.DisplayString == "New" || x.Status.DisplayString == "Tentative") && x.CallID == callID && x.TaskID != ((NewSDTApplication.Models.HTTPPostParams)(Session["SiebelHttpPostParams"])).ActivityNo).ToList();
                if (Tasklist.Count > 0)
                {
                    foreach (var item in Tasklist)
                    {
                        CustomTasksList objCustomTask = new CustomTasksList();
                        objCustomTask.Sitename = item.TaskSiteName;
                        objCustomTask.Systemname = item.TaskSystemName;
                        objCustomTask.SystemID = item.TaskSystemID; //Added by AJaySara
                        objCustomTask.TaskID = item.TaskID; //Added by AJaySara
                        objCustomTask.EarlyStart = Convert.ToDateTime(item.EarlyStart).ToString();

                        if (item.AppointmentStart.ToString() == DefaultClickDates.DefaultClickDate)
                        {
                            objCustomTask.AppointmentStart = "";
                        }
                        else
                        {
                            objCustomTask.AppointmentStart = Convert.ToDateTime(item.AppointmentStart).ToString();
                        }
                        if (item.AppointmentFinish.ToString() == DefaultClickDates.DefaultClickDate)
                        {
                            objCustomTask.AppointmentFinish = "";
                        }
                        else
                        {
                            objCustomTask.AppointmentFinish = Convert.ToDateTime(item.AppointmentFinish).ToString();
                        }

                        Assignmentlist = lst.Assignments.Where(x => x.Task.CallID == item.CallID).ToList();

                        if (Assignmentlist.Count > 0)
                        {
                            objCustomTask.AssignedFSE = Assignmentlist[0].AssignedEngineers;
                            objCustomTask.AssignmentStart = Convert.ToDateTime(Assignmentlist[0].Start).ToString();
                            objCustomTask.AssignmentFinish = Convert.ToDateTime(Assignmentlist[0].Finish).ToString();
                        }


                        objCustomTask.Tasktype = item.TaskType.DisplayString;
                        objCustomTask.Duration = item.Duration.ToString();
                        objCustomTask.Level = item.SkillLevel.ToString();
                        objCustomTask.ProductID = item.TaskSystemProductID.DisplayString;
                        objCustomTask.RFS = item.RequiredFSEs;
                        objCustomTask.Status = item.Status.DisplayString;
                        objCustomTask.CallID = item.CallID;
                        objCustomTask.TaskNumber = item.Number.ToString();
                        objCustomTask.SRnumber = item.MUSTJobNumber;
                        objCustomTask.IsMst = item.IsMST;

                        #region Rating logic implemented by Phani Kanth P.

                        GetTasksResponse taskResponse = new GetTasksResponse();
                        task = new ScheduleServiceDev1.Task();
                        taskResponse = (ScheduleServiceDev1.GetTasksResponse)Session["TaskResponseByTask"];
                        if (Session["taskStatus"] != "New")
                        {

                            task = taskResponse.Tasks[0];
                            TaskSystemModality = task.TaskSystemModality;
                            TaskSystemProductName = task.TaskSystemProductName;
                            SkillLevel = Convert.ToString(task.SkillLevel);
                        }
                        else
                        {
                            if (Session["Systemrating"] != null)
                            {
                                TaskSystemModality = ((NewSDTApplication.ServiceOptimizationService.GEHCSystem)(Session["Systemrating"])).Modality;
                                TaskSystemProductName = ((NewSDTApplication.ServiceOptimizationService.GEHCSystem)(Session["Systemrating"])).ProductName;
                                // SkillLevel = ((NewSDTApplication.Models.SiebelJsonToEntity)(Session["SiebelData"])).serviceRequest.gehcFse1;
                                SkillLevel = ((IndextoRequest)Session["IndextoRequest"]).FseSkillLevel;

                                if (!string.IsNullOrEmpty(SkillLevel))
                                {
                                    var sLevel = SkillLevel.Replace("Level", "").Trim();
                                    if (sLevel == "None")
                                    {
                                        SkillLevel = "0";
                                    }
                                    else if (sLevel == "Any FE")
                                    {
                                        SkillLevel = "0";
                                    }
                                    else
                                    {
                                        SkillLevel = sLevel;

                                    }
                                }
                                else
                                {
                                    SkillLevel = "0";
                                }
                            }

                        }
                        objCustomTask.Rating = "*";
                        if (item.TaskSystemModality == TaskSystemModality)
                        {
                            objCustomTask.Rating = "**";
                        }
                        if (item.TaskSystemModality == TaskSystemModality && item.TaskSystemProductName == TaskSystemProductName)
                        {
                            objCustomTask.Rating = "***";
                        }

                        if (item.TaskSystemModality == TaskSystemModality && item.TaskSystemProductName == TaskSystemProductName && item.SkillLevel == Convert.ToInt32(SkillLevel))
                        {
                            objCustomTask.Rating = "****";
                        }

                        #endregion
                        objCustomtasksList.Add(objCustomTask);

                    }
                    var jsonData = new
                    {
                        total = 1,
                        page = 1,
                        records = 10,
                        rows = objCustomtasksList,
                    };

                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("SystemController;GetSystemDependencies; Exception:" + ex.Message);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateVisitWithDependencies(string FinalRes)
        {
            try
            {
                FinalRes = FinalRes.Remove(FinalRes.Length - 1);
                FinalRes = "[" + FinalRes + "]";
                var objres = JsonConvert.DeserializeObject<List<CustomTasksList>>(FinalRes);
                CallClickSerrvice objCallClickService = new CallClickSerrvice();
                //Added by phani kanth p.                
                logger.Debug("In Site controller GetSiteDependencies Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName)");
                DateTime startTimeTx3 = DateTime.Now;
                string createStatus = await objCallClickService.CreateTaskWithSystemDependencies(objres);
                DateTime endTimeTx3 = DateTime.Now;
                TimeSpan Tx3 = endTimeTx3.Subtract(startTimeTx3);
                logger.Debug("In Site controller GetSiteDependencies Method | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) | Call duration: " + Tx3.Milliseconds);
                if (createStatus == "Success")                
                {
                    return Json(new { CallID = objres[0].CallID, message = "Success" });
                }
                else if (createStatus.Contains("TaskSubType"))
                {
                    return Json(new { CallID = "0", message = "TaskSubTypeEmpty" });
                }
                else
                {
                    return Json(new { HasError = true, message = createStatus });
                }

                //if (objCallClickService.CreateTaskWithSystemDependencies(objres))

                //    return Json(new { CallID = objres[0].CallID, message = "Success" });
                //else
                //    return Json(new { HasError = true, message = "Failure" });

                // Write a method in CallClickService for ExecuteMultipleRequests by passing the objres to the method. Just implement that method and save the details on click server.
                // I will implement the logic for binding the result and showing a message to the user. Joy Please corordiante with Ankur and implement this task.


                // return View();
            }
            catch (Exception ex)
            {
                logger.Error("SiteController;CreateVisitWithDependencies; Exception:" + ex.Message);
                return Json(new { HasError = true, message = ex.Message.ToString() });
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
