using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewSDTApplication.Models;
using NewSDTApplication.Utilities;
using System.Configuration;
using System.Threading.Tasks;

namespace NewSDTApplication.Utilities
{

    public class SSOLogic
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        List<SelectListItem> items = new List<SelectListItem>();
        CallClickSerrvice objClickCallService = new CallClickSerrvice();
        List<SelectListItem> EngineerList = new List<SelectListItem>();
        public async Task<List<SelectListItem>> GetSSODetails(string StrSystemId)
        {


            try
            {

                //--Calling 1st Click service method to get FSE values
                logger.Debug("In SSOLogic.cs GetSSODetails Method | Call to ClickCallService.cs (objClickCallService.GetSystemDetails) with SystemID: " + StrSystemId);
                DateTime startTimeTx1 = DateTime.Now;
                var ClickResponse = objClickCallService.GetSystemDetails(StrSystemId);
                DateTime endTimeTx1 = DateTime.Now;
                TimeSpan Tx1 = endTimeTx1.Subtract(startTimeTx1);
                logger.Debug("In SSOLogic.cs GetSSODetails Method | Call to ClickCallService.cs (objClickCallService.GetSystemDetails) with SystemID: " + StrSystemId + " | Call duration: " + Tx1.Milliseconds);
                HttpContext.Current.Session["Systemrating"] = ClickResponse;
                if (!string.IsNullOrEmpty(ClickResponse.PreferredFSEs))
                {
                    var PreferredFSEvalues = new List<string>((ClickResponse.PreferredFSEs).Split(','));
                    if (!string.IsNullOrEmpty(PreferredFSEvalues[0]))
                    {
                        logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of SSOs| Call to ClickCallService.cs (objClickCallService.GetResources)");
                        DateTime startTimeTx2 = DateTime.Now;
                        foreach (var t in PreferredFSEvalues)
                        {
                            if (!string.IsNullOrEmpty(t.ToString()))
                            {
                                //--Calling 2nd Click service by Passing FSE value to get specific Engineer name
                                var GetEngg = objClickCallService.GetResources("ID", t.ToString());
                                for (int i = 0; i < GetEngg.Length; i++)
                                {
                                    SelectListItem Eng = new SelectListItem();
                                    Eng.Value = GetEngg[i].ID;//t.ToString();//FSE value
                                    Eng.Text = GetEngg[i].Name; // Engineer name
                                    EngineerList.Add(Eng);
                                }

                            }
                        }
                        DateTime endTimeTx2 = DateTime.Now;
                        TimeSpan Tx2 = endTimeTx2.Subtract(startTimeTx2);
                        logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of SSOs| Call to ClickCallService.cs (objClickCallService.GetResources) | Call duration: " + Tx2.Milliseconds);
                    }
                }

                if (!string.IsNullOrEmpty(ClickResponse.RequiredFSEs))
                {
                    var RequiredFSEsvalues = new List<string>((ClickResponse.RequiredFSEs).Split(','));

                    logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of Engineer Names and SSOs| Call to ClickCallService.cs (objClickCallService.GetResources)");
                    DateTime startTimeTx3 = DateTime.Now;
                    if (!string.IsNullOrEmpty(RequiredFSEsvalues[0]))
                    {
                        foreach (var t1 in RequiredFSEsvalues)
                        {
                            if (!string.IsNullOrEmpty(t1.ToString()))
                            {
                                //--Calling 2nd Click service by Passing FSE value to get specific Engineer name
                                var GetEngg1 = objClickCallService.GetResources("ID", t1.ToString());
                                for (int i = 0; i < GetEngg1.Length; i++)
                                {
                                    SelectListItem Eng1 = new SelectListItem();
                                    Eng1.Value = GetEngg1[i].ID;//t.ToString();//FSE value
                                    Eng1.Text = GetEngg1[i].Name; // Engineer name
                                    EngineerList.Add(Eng1);
                                }
                            }
                        }
                        DateTime endTimeTx3 = DateTime.Now;
                        TimeSpan Tx3 = endTimeTx3.Subtract(startTimeTx3);
                        logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of Engineer Names and SSOs| Call to ClickCallService.cs (objClickCallService.GetResources) | Call duration: " + Tx3.Milliseconds);
                    }
                }
                if (HttpContext.Current.Session["taskStatus"] == "Exists")
                {
                    #region  Incomplete in clicksoftware, populate all those SSO ID's and bind it in drop down
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["MUSTJobNumber"] as string))
                    {
                        logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of taks by must tasks | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName)");
                        DateTime startTimeTx4 = DateTime.Now;
                        var resTasksResponseByMustJobTask = await objClickCallService.GetTasksRequestByPropertyName(HttpContext.Current.Session["MUSTJobNumber"].ToString(), "MUSTJobNumber");
                        DateTime endTimeTx4 = DateTime.Now;
                        TimeSpan Tx4 = endTimeTx4.Subtract(startTimeTx4);
                        logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of taks by must tasks | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) | Call duration: " + Tx4.Milliseconds);
                        var res = resTasksResponseByMustJobTask.Tasks.Where(x => x.Status.DisplayString == "Incomplete");
                        if (res.Count() > 0)
                        {

                            logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of Engineer Names and SSOs| Call to ClickCallService.cs (objClickCallService.GetResources)");
                            DateTime startTimeTx5 = DateTime.Now;
                            foreach (var item in res)
                            {
                                foreach (var item1 in resTasksResponseByMustJobTask.Assignments)
                                {
                                    if (item.CallID == item1.Task.CallID)
                                    {
                                        var IncompletedSsoid = item1.Engineers[0].Text[0].ToString();// item1.Engineers[0].Text.ToString();
                                        if (!string.IsNullOrEmpty(IncompletedSsoid))
                                        {
                                            var resEngineers = objClickCallService.GetResources("ID", IncompletedSsoid);
                                            SelectListItem Eng4 = new SelectListItem();
                                            Eng4.Value = resEngineers[0].ID;//t.ToString();//FSE value
                                            Eng4.Text = resEngineers[0].Name; // Engineer name
                                            EngineerList.Add(Eng4);
                                            break;
                                        }
                                    }
                                }


                                var temp = item.PreferredFSEs.Split(',');

                                if (!string.IsNullOrEmpty(temp[0]))
                                {
                                    //  if (temp.Length > 0)
                                    // {
                                    foreach (var listitem in temp)
                                    {
                                        if (!string.IsNullOrEmpty(listitem))
                                        {
                                            var resEngineers = objClickCallService.GetResources("ID", listitem);
                                            SelectListItem Eng2 = new SelectListItem();
                                            Eng2.Value = resEngineers[0].ID;//t.ToString();//FSE value
                                            Eng2.Text = resEngineers[0].Name; // Engineer name
                                            EngineerList.Add(Eng2);
                                        }
                                    }
                                }

                                var tempRequiredFSEs = item.RequiredFSEs.Split(',');
                                if (!string.IsNullOrEmpty(tempRequiredFSEs[0]))
                                {
                                    foreach (var listitemRequiredFSE in tempRequiredFSEs)
                                    {
                                        if (!string.IsNullOrEmpty(listitemRequiredFSE))
                                        {
                                            var resEngineersRequiredFSE = objClickCallService.GetResources("ID", listitemRequiredFSE);
                                            SelectListItem Eng3 = new SelectListItem();
                                            Eng3.Value = resEngineersRequiredFSE[0].ID;//t.ToString();//FSE value
                                            Eng3.Text = resEngineersRequiredFSE[0].Name; // Engineer name
                                            EngineerList.Add(Eng3);
                                        }
                                    }
                                }
                            }
                            DateTime endTimeTx5 = DateTime.Now;
                            TimeSpan Tx5 = endTimeTx5.Subtract(startTimeTx5);
                            logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of Engineer Names and SSOs| Call to ClickCallService.cs (objClickCallService.GetResources) | Call duration: " + Tx5.Milliseconds);
                        }
                    }

                    #endregion
                }
                else
                {
                    #region  Incomplete in clicksoftware, populate all those SSO ID's and bind it in drop down
                    if (!string.IsNullOrEmpty(((NewSDTApplication.Models.HTTPPostParams)(HttpContext.Current.Session["SiebelHttpPostParams"])).ServiceRequestNumber as string))
                    {
                        logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of taks by must tasks | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName)");
                        DateTime startTimeTx6 = DateTime.Now;
                        var resTasksResponseByMustJobTask = await objClickCallService.GetTasksRequestByPropertyName(((NewSDTApplication.Models.HTTPPostParams)(HttpContext.Current.Session["SiebelHttpPostParams"])).ServiceRequestNumber.ToString(), "MUSTJobNumber");
                        DateTime endTimeTx6 = DateTime.Now;
                        TimeSpan Tx6 = endTimeTx6.Subtract(startTimeTx6);
                        logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of taks by must tasks | Call to ClickCallService.cs (objClickCallService.GetTasksRequestByPropertyName) | Call duration: " + Tx6.Milliseconds);
                        var res = resTasksResponseByMustJobTask.Tasks.Where(x => x.Status.DisplayString == "Incomplete");
                        if (res.Count() > 0)
                        {
                            logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of Engineer Names and SSOs | Call to ClickCallService.cs (objClickCallService.GetResources)");
                            DateTime startTimeTx7 = DateTime.Now;
                            foreach (var item in res)
                            {
                                foreach (var item1 in resTasksResponseByMustJobTask.Assignments)
                                {
                                    if (item.CallID == item1.Task.CallID)
                                    {
                                        var IncompletedSsoid = item1.Engineers[0].Text[0].ToString();// item1.Engineers[0].Text.ToString();
                                        if (!string.IsNullOrEmpty(IncompletedSsoid))
                                        {
                                            var resEngineers = objClickCallService.GetResources("ID", IncompletedSsoid);
                                            SelectListItem Eng4 = new SelectListItem();
                                            Eng4.Value = resEngineers[0].ID;//t.ToString();//FSE value
                                            Eng4.Text = resEngineers[0].Name; // Engineer name
                                            EngineerList.Add(Eng4);
                                            break;
                                        }
                                    }
                                }


                                var temp = item.PreferredFSEs.Split(',');

                                if (!string.IsNullOrEmpty(temp[0]))
                                {
                                    //  if (temp.Length > 0)
                                    // {
                                    foreach (var listitem in temp)
                                    {
                                        if (!string.IsNullOrEmpty(listitem))
                                        {
                                            var resEngineers = objClickCallService.GetResources("ID", listitem);
                                            SelectListItem Eng2 = new SelectListItem();
                                            Eng2.Value = resEngineers[0].ID;//t.ToString();//FSE value
                                            Eng2.Text = resEngineers[0].Name; // Engineer name
                                            EngineerList.Add(Eng2);
                                        }
                                    }
                                }

                                var tempRequiredFSEs = item.RequiredFSEs.Split(',');
                                if (!string.IsNullOrEmpty(tempRequiredFSEs[0]))
                                {
                                    foreach (var listitemRequiredFSE in tempRequiredFSEs)
                                    {
                                        if (!string.IsNullOrEmpty(listitemRequiredFSE))
                                        {
                                            var resEngineersRequiredFSE = objClickCallService.GetResources("ID", listitemRequiredFSE);
                                            SelectListItem Eng3 = new SelectListItem();
                                            Eng3.Value = resEngineersRequiredFSE[0].ID;//t.ToString();//FSE value
                                            Eng3.Text = resEngineersRequiredFSE[0].Name; // Engineer name
                                            EngineerList.Add(Eng3);
                                        }
                                    }
                                }
                            }
                            DateTime endTimeTx7 = DateTime.Now;
                            TimeSpan Tx7 = endTimeTx7.Subtract(startTimeTx7);
                            logger.Debug("In SSOLogic.cs GetSSODetails Method to return a list of Engineer Names and SSOs | Call to ClickCallService.cs (objClickCallService.GetResources) | Call duration: " + Tx7.Milliseconds);
                        }
                    }

                    #endregion

                }
                var result = new SelectList(items, "Text", "Value"); //--FSE List 
                var SelectedEngineerList = new SelectList(EngineerList.Distinct(), "Value", "Text"); //--Engineer List


                objClickCallService.CloseConnectionSDT();
            }
            catch (Exception ex)
            {
                logger.Error("SSOLogic;Index; Exception occured while fetching SSO details from ClickSoftware Service call :" + ex.Message);
                EngineerList = null;

            }
            var resultssoid = EngineerList;
            return resultssoid;
        }


        public string GetValidFSESSOID(string strSSOID, string StrSystemId)
        {
            List<SelectListItem> EngineerList1 = new List<SelectListItem>();
            logger.Debug("In SSOLogic.cs GetValidFSESSOID Method | Call to ClickCallService.cs (objClickCallService.GetSystemDetails) with SystemID: " + StrSystemId);
            DateTime startTimeTx8 = DateTime.Now;
            var ClickResponse = objClickCallService.GetSystemDetails(StrSystemId);
            DateTime endTimeTx8 = DateTime.Now;
            TimeSpan Tx8 = endTimeTx8.Subtract(startTimeTx8);
            logger.Debug("In SSOLogic.cs GetValidFSESSOID Method | Call to ClickCallService.cs (objClickCallService.GetSystemDetails) with SystemID: " + StrSystemId + " | Call duration: " + Tx8.Milliseconds);
            var Searchssoid = "";
            try
            {

                if (!string.IsNullOrEmpty(strSSOID.Trim()))
                //--Calling 1st Click service method to get FSE values
                {
                    logger.Debug("In SSOLogic.cs GetValidFSESSOID Method | Call to ClickCallService.cs (objClickCallService.GetResources) with SSOID: " + strSSOID.Trim());
                    DateTime startTimeTx9 = DateTime.Now;
                    var ValidSSOID = objClickCallService.GetResources("ID", strSSOID.Trim());
                    DateTime endTimeTx9 = DateTime.Now;
                    TimeSpan Tx9 = endTimeTx9.Subtract(startTimeTx9);
                    logger.Debug("In SSOLogic.cs GetValidFSESSOID Method | Call to ClickCallService.cs (objClickCallService.GetResources) with SSOID: " + strSSOID.Trim() + " | Call duration: " + Tx9.Milliseconds);
                    if (ValidSSOID.Length > 0)
                    {
                        Searchssoid = ValidSSOID[0].ID + " || " + ValidSSOID[0].Name + " || " + "SDT Schedule";
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("GetValidFSESSOID;Index; Exception occured while fetching SSO details from ClickSoftware Service call :" + ex.Message);
                EngineerList = null;

            }
            return Searchssoid;
        }


        public async Task<List<SelectListItem>> GetValidSearchSSOID(string strSSOID, string StrSystemId)
        {
            //--Calling 1st Click service method to get FSE values
            CallClickSerrvice objClickCallService1 = new CallClickSerrvice();
            logger.Debug("In SSOLogic.cs GetValidSearchSSOID Method | Call to ClickCallService.cs (objClickCallService.GetSystemDetails) with SystemID: " + StrSystemId);
            DateTime startTimeTx10 = DateTime.Now;
            var ClickResponse = objClickCallService1.GetSystemDetails(StrSystemId);
            DateTime endTimeTx10 = DateTime.Now;
            TimeSpan Tx10 = endTimeTx10.Subtract(startTimeTx10);
            logger.Debug("In SSOLogic.cs GetValidSearchSSOID Method | Call to ClickCallService.cs (objClickCallService.GetSystemDetails) with SystemID: " + StrSystemId + " | Call duration: " + Tx10.Milliseconds);
            List<SelectListItem> EngineerList1 = new List<SelectListItem>();
            try
            {
                logger.Debug("In SSOLogic.cs GetValidSearchSSOID Method | Call to ClickCallService.cs (objClickCallService.GetResources) with SSOID: " + strSSOID);
                DateTime startTimeTx11 = DateTime.Now;
                if (!string.IsNullOrEmpty(strSSOID))
                {
                    var strSSOIDValues = new List<string>((strSSOID).Split(','));
                    foreach (var t1 in strSSOIDValues)
                    {
                        //--Calling 2nd Click service by Passing FSE value to get specific Engineer name
                        if (!string.IsNullOrEmpty(t1.ToString()))
                        {
                            var GetEngg1 = objClickCallService1.GetResources("ID", t1.ToString());

                            for (int i = 0; i < GetEngg1.Length; i++)
                            {
                                SelectListItem Eng1 = new SelectListItem();
                                Eng1.Value = GetEngg1[i].ID;//t.ToString();//FSE value
                                Eng1.Text = GetEngg1[i].Name; // Engineer name
                                EngineerList1.Add(Eng1);
                            }
                        }
                    }

                }
                DateTime endTimeTx11 = DateTime.Now;
                TimeSpan Tx11 = endTimeTx11.Subtract(startTimeTx11);
                logger.Debug("In SSOLogic.cs GetValidSearchSSOID Method | Call to ClickCallService.cs (objClickCallService.GetResources) with SSOID: " + strSSOID + " | Call duration: " + Tx11.Milliseconds);
            }
            catch (Exception ex)
            {
                logger.Error("GetValidSearchSSOID;Index; Exception occured while fetching SSO details from ClickSoftware Service call :" + ex.Message);
                EngineerList = null;

            }
            return EngineerList1;
        }

    }
}
