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
                var ClickResponse = objClickCallService.GetSystemDetails(StrSystemId);
               HttpContext.Current.Session["Systemrating"] = ClickResponse;
                if (!string.IsNullOrEmpty(ClickResponse.PreferredFSEs))
                {
                    var PreferredFSEvalues = new List<string>((ClickResponse.PreferredFSEs).Split(','));
                    if (!string.IsNullOrEmpty(PreferredFSEvalues[0]))
                    {
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
                    }
                }

                if (!string.IsNullOrEmpty(ClickResponse.RequiredFSEs))
                {
                    var RequiredFSEsvalues = new List<string>((ClickResponse.RequiredFSEs).Split(','));

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

                    }
                }
                if (HttpContext.Current.Session["taskStatus"] == "Exists")
                {
                    #region  Incomplete in clicksoftware, populate all those SSO ID's and bind it in drop down
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["MUSTJobNumber"] as string))
                    {
                        var resTasksResponseByMustJobTask = await objClickCallService.GetTasksRequestByPropertyName(HttpContext.Current.Session["MUSTJobNumber"].ToString(), "MUSTJobNumber");
                        var res = resTasksResponseByMustJobTask.Tasks.Where(x => x.Status.DisplayString == "Incomplete");
                        if (res.Count() > 0)
                        {
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
                        }
                    }

                    #endregion
                }
                else
                {
                    #region  Incomplete in clicksoftware, populate all those SSO ID's and bind it in drop down
                    if (!string.IsNullOrEmpty(((NewSDTApplication.Models.HTTPPostParams)(HttpContext.Current.Session["SiebelHttpPostParams"])).ServiceRequestNumber as string))
                    {
                        var resTasksResponseByMustJobTask = await objClickCallService.GetTasksRequestByPropertyName(((NewSDTApplication.Models.HTTPPostParams)(HttpContext.Current.Session["SiebelHttpPostParams"])).ServiceRequestNumber.ToString(), "MUSTJobNumber");
                        var res = resTasksResponseByMustJobTask.Tasks.Where(x => x.Status.DisplayString == "Incomplete");
                        if (res.Count() > 0)
                        {
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
            var ClickResponse = objClickCallService.GetSystemDetails(StrSystemId);
            // var boolvalidateSSOID = false;
            var Searchssoid = "";
            try
            {

                if (!string.IsNullOrEmpty(strSSOID.Trim()))
                //--Calling 1st Click service method to get FSE values
                {
                    var ValidSSOID = objClickCallService.GetResources("ID", strSSOID.Trim());

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
            var ClickResponse = objClickCallService1.GetSystemDetails(StrSystemId);
            List<SelectListItem> EngineerList1 = new List<SelectListItem>();
            try
            {
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
