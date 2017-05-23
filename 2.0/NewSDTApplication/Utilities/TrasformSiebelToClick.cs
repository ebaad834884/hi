using NewSDTApplication.Models;
using SDTLogger;
using System;
using System.Configuration;
using System.Web;
using System.Xml;


namespace NewSDTApplication.Utilities
{
    public class TrasformSiebelToClick
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        XmlDocument xmlDoc = new XmlDocument();
        XmlNodeList nodeList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        //public ClickTranslator TransformValues(SiebelJsonToEntity res)
        //{
        //    logger.Debug("TrasformSiebelToClick.cs;TransformValues; Start");

        //    XmlDocument xmlDoc = new XmlDocument();

        //    try
        //    {
        //       // Logger.Info("Conversion of Siebel Entity Started");
        //        ClickTranslator obj = new ClickTranslator();

        //        //-Reading XML data from Local solution XML folder.
        //        string xmlpath = HttpContext.Current.Server.MapPath("/XML/TransformToClick.xml");
        //        if (!(System.IO.File.Exists(xmlpath)))
        //        {
        //            logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -1 : " + xmlpath);
        //            // Code to commented start 

        //            if (!(System.IO.File.Exists(xmlpath)))
        //            {
        //                xmlpath = HttpContext.Current.Server.MapPath("~/XML/TransformToClick.xml");
        //                if (!(System.IO.File.Exists(xmlpath)))
        //                    logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -2: " + xmlpath);
        //            }

        //            if (!(System.IO.File.Exists(xmlpath)))
        //            {
        //                xmlpath = HttpContext.Current.Server.MapPath(@"/XML/TransformToClick.xml");
        //                if (!(System.IO.File.Exists(xmlpath)))
        //                    logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -3:" + xmlpath);
        //            }
        //            if (!(System.IO.File.Exists(xmlpath)))
        //            {
        //                xmlpath = HttpContext.Current.Server.MapPath(@"//XML//TransformToClick.xml");
        //                if (!(System.IO.File.Exists(xmlpath)))
        //                    logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -4:" + xmlpath);
        //            }


        //            // Code to commented End


        //        }

        //        xmlDoc.Load(xmlpath);
        //        logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML Path: " + xmlpath);

        //        //logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML Path :" + HttpContext.Current.Server.MapPath("//XML//TransformToClick.xml"));

        //        if (!string.IsNullOrEmpty(res.serviceRequest.activityDetailList[0].serviceRegion))// values is null or not.
        //        {

        //            string Region = res.serviceRequest.activityDetailList[0].serviceRegion;
        //            string SelectedDistrict = "";
        //            string RegionName = "", District = "";

        //            try
        //            {
        //                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/serviceRequest/Regions/Region");

        //                foreach (XmlNode node in nodeList)
        //                {
        //                    RegionName = node.SelectSingleNode("Name").InnerText;
        //                    District = node.SelectSingleNode("District").InnerText;

        //                    if (Region.ToUpper() == RegionName.ToUpper())
        //                    {
        //                        SelectedDistrict = District;
        //                        break;
        //                    }
        //                }

        //                obj.District = SelectedDistrict;
        //            }
        //            catch(Exception ex)
        //            {
        //                logger.Error("TrasformSiebelToClick.cs;TransformValues; Exception -Regions: " + ex.Message);

        //            }
        //            finally
        //            {
        //                Region = null;
        //            }

        //        }
        //        if (!string.IsNullOrEmpty(res.serviceRequest.srType))// values is null or not.
        //        {

        //            string srType = res.serviceRequest.srType;
        //            string SelectedTaskType = "";
        //            string srTypeName = "", TaskType = "";

        //            try
        //            {
        //                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/serviceRequest/srTypes/srType");

        //                foreach (XmlNode node in nodeList)
        //                {
        //                    srTypeName = node.SelectSingleNode("Name").InnerText;
        //                    TaskType = node.SelectSingleNode("TaskType").InnerText;

        //                    if (srType.ToUpper() == srTypeName.ToUpper())
        //                    {
        //                        SelectedTaskType = TaskType;
        //                        break;
        //                    }
        //                }

        //                obj.TaskType = SelectedTaskType;
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error("TrasformSiebelToClick.cs;TransformValues; Exception -ServiceRequest Type: " + ex.Message);

        //            }
        //            finally
        //            {
        //                srType = null;
        //            }
        //        }


        //        if (!string.IsNullOrEmpty(res.serviceRequest.srSubType))// values is null or not.
        //        {

        //            string srSubType = res.serviceRequest.srSubType;
        //            string SelectedTaskType = "";
        //            string srSubTypeName = "", TaskType = "";

        //            try
        //            {
        //                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/serviceRequest/srSubTypes/srSubType");

        //                foreach (XmlNode node in nodeList)
        //                {
        //                    srSubTypeName = node.SelectSingleNode("Name").InnerText;
        //                    TaskType = node.SelectSingleNode("TaskSubType").InnerText;

        //                    if (srSubType.ToUpper() == srSubTypeName.ToUpper())
        //                    {
        //                        SelectedTaskType = TaskType;
        //                        break;
        //                    }
        //                }

        //                obj.TaskSubType = SelectedTaskType;
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error("TrasformSiebelToClick.cs;TransformValues; Exception -ServiceRequest SubType: " + ex.Message);
        //            }
        //            finally
        //            {
        //                srSubType = null;


        //            }
        //        }
        //        else
        //        {
        //            obj.TaskSubType = "Mandatory";
        //        }

        //        //Logger.Info("Conversion of Siebel Entity Completed");
        //        logger.Debug("TrasformSiebelToClick.cs;TransformValues; Conversion of Siebel Entity Completed");

        //        return obj;
        //    }

        //    catch (Exception ex)
        //    {
        //        logger.Error("TrasformSiebelToClick.cs;TransformValues; Exception: " + ex.Message);

        //        //Logger.Error("Exception thrown at TransformValues Class", ex);
        //        throw;
        //    }
        //    finally
        //    {
        //        xmlDoc = null;
        //    }
        //}

        /// <summary>
        /// Loads the XML document(Transform ToClick.xml) from the specified URL. 
        /// </summary>
        public void GetXMlData()
        {
            string xmlpath = HttpContext.Current.Server.MapPath("/XML/TransformToClick.xml");
            logger.Debug("TrasformSiebelToClick.cs;GetXMlData; Start");
            try
            {
                if (!(System.IO.File.Exists(xmlpath)))
                {
                    logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -1 : " + xmlpath);
                    if (!(System.IO.File.Exists(xmlpath)))
                    {
                        xmlpath = HttpContext.Current.Server.MapPath("~/XML/TransformToClick.xml");
                        if (!(System.IO.File.Exists(xmlpath)))
                            logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -2: " + xmlpath);
                    }
                    else if (!(System.IO.File.Exists(xmlpath)))
                    {
                        xmlpath = HttpContext.Current.Server.MapPath(@"/XML/TransformToClick.xml");
                        if (!(System.IO.File.Exists(xmlpath)))
                            logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -3:" + xmlpath);
                    }
                    else if (!(System.IO.File.Exists(xmlpath)))
                    {
                        xmlpath = HttpContext.Current.Server.MapPath(@"//XML//TransformToClick.xml");
                        if (!(System.IO.File.Exists(xmlpath)))
                            logger.Debug("TrasformSiebelToClick.cs;TransformValues; TransformToClick XML File doesn't Exist -4:" + xmlpath);
                    }
                }
                xmlDoc.Load(xmlpath);
                System.Web.HttpContext.Current.Session["XMLTransformToClick"] = xmlDoc;
                logger.Debug("TrasformSiebelToClick.cs;GetXMlData; TransformToClick XML Path: " + xmlpath);
                logger.Debug("TrasformSiebelToClick.cs;GetXMlData; End");
            }
            catch (Exception ex)
            {
                logger.Error("TrasformSiebelToClick.cs;TransformValues; Exception: " + ex.Message);
                throw;
            }
            finally
            {
                //xmlDoc = null;
            }
        }

        /// <summary>
        /// Selects a list of nodes matching the XPath expression(Node:Region) and returns the district of the region.
        /// </summary>
        /// <param name="serviceRegion"></param>
        /// <returns></returns>
        public string GetDistrict(string serviceRegion)
        {
            string SelectedDistrict = string.Empty;
            string RegionName = string.Empty;
            string District = string.Empty;

            try
            {

                if (System.Web.HttpContext.Current.Session["XMLTransformToClick"] == null)
                {
                    GetXMlData();
                }
                else
                {
                    xmlDoc = System.Web.HttpContext.Current.Session["XMLTransformToClick"] as System.Xml.XmlDocument;
                }

                nodeList = xmlDoc.DocumentElement.SelectNodes("/serviceRequest/Regions/Region");

                foreach (XmlNode node in nodeList)
                {
                    RegionName = node.SelectSingleNode("Name").InnerText;
                    District = node.SelectSingleNode("District").InnerText;

                    if (serviceRegion.ToUpper() == RegionName.ToUpper())
                    {
                        SelectedDistrict = District;
                        break;
                    }
                }


                return SelectedDistrict;
            }
            catch (Exception ex)
            {
                logger.Error("TrasformSiebelToClick.cs;GetDistrict; Exception -Regions: " + ex.Message);
                return SelectedDistrict = null;

            }
            finally
            {
                RegionName = null;
                District = null;
                xmlDoc = null;
            }
        }

        /// <summary>
        /// Selects a list of nodes matching the XPath expression(Node:srType) and returns the task type.
        /// </summary>
        /// <param name="AssignsrType"></param>
        /// <returns></returns>
        public string GetTaskType(string AssignsrType)
        {
            string SelectedTaskType = string.Empty;
            string srTypeName = string.Empty;
            string TaskType = string.Empty;

            try
            {
                //GetXMlData();
                if (System.Web.HttpContext.Current.Session["XMLTransformToClick"] == null)
                {
                    GetXMlData();
                }
                else
                {
                    xmlDoc = System.Web.HttpContext.Current.Session["XMLTransformToClick"] as System.Xml.XmlDocument;
                }

                nodeList = xmlDoc.DocumentElement.SelectNodes("/serviceRequest/srTypes/srType");
                foreach (XmlNode node in nodeList)
                {
                    srTypeName = node.SelectSingleNode("Name").InnerText;
                    TaskType = node.SelectSingleNode("TaskType").InnerText;

                    if (AssignsrType.ToUpper() == srTypeName.ToUpper())
                    {
                        SelectedTaskType = TaskType;
                        break;
                    }
                }


                return SelectedTaskType;
            }
            catch (Exception ex)
            {
                logger.Error("TrasformSiebelToClick.cs;GetTaskType; Exception -ServiceRequest SRType: " + ex.Message);
                return SelectedTaskType = null;
            }
            finally
            {
                srTypeName = null;
                TaskType = null;
                xmlDoc = null;
            }
        }

        /// <summary>
        /// Selects a list of nodes matching the XPath expression(Node:srSubType) and returns the task subtype.
        /// </summary>
        /// <param name="AssignsrSubType"></param>
        /// <returns></returns>
        public string GetTaskSubType(string AssignsrSubType)
        {

            string SelectedsubTaskType = string.Empty;
            string srSubTypeName = string.Empty;
            string TaskType = string.Empty;

            try
            {
                //GetXMlData();
                if (System.Web.HttpContext.Current.Session["XMLTransformToClick"] == null)
                {
                    GetXMlData();
                }
                else
                {
                    xmlDoc = System.Web.HttpContext.Current.Session["XMLTransformToClick"] as System.Xml.XmlDocument;
                }

                nodeList = xmlDoc.DocumentElement.SelectNodes("/serviceRequest/srSubTypes/srSubType");

                foreach (XmlNode node in nodeList)
                {
                    srSubTypeName = node.SelectSingleNode("Name").InnerText;
                    TaskType = node.SelectSingleNode("TaskSubType").InnerText;

                    if (AssignsrSubType.ToUpper() == srSubTypeName.ToUpper())
                    {
                        SelectedsubTaskType = TaskType;
                        break;
                    }
                }

                return SelectedsubTaskType;
            }
            catch (Exception ex)
            {
                logger.Error("TrasformSiebelToClick.cs;GetTaskSubType; Exception -srSubType: " + ex.Message);
                return SelectedsubTaskType = null;
            }
            finally
            {
                srSubTypeName = null;
                TaskType = null;
                xmlDoc = null;
            }
        }

        /// <summary>
        /// Selects a list of nodes matching the XPath expression(Node:Status) and returns the system status.
        /// </summary>
        /// <param name="AssignEquipmentStatus"></param>
        /// <returns></returns>
        public string GetSystemStatus(string AssignEquipmentStatus)
        {

            string SelectedSystemStatus = string.Empty;
            string EquipmentStatus = string.Empty;
            string SystemStatus = string.Empty;

            try
            {
                //GetXMlData();
                if (System.Web.HttpContext.Current.Session["XMLTransformToClick"] == null)
                {
                    GetXMlData();
                }
                else
                {
                    xmlDoc = System.Web.HttpContext.Current.Session["XMLTransformToClick"] as System.Xml.XmlDocument;
                }
                nodeList = xmlDoc.DocumentElement.SelectNodes("/serviceRequest/Statuses/Status");

                foreach (XmlNode node in nodeList)
                {
                    EquipmentStatus = node.SelectSingleNode("EquipmentStatus").InnerText;
                    SystemStatus = node.SelectSingleNode("SystemStatus").InnerText;

                    if (AssignEquipmentStatus.ToUpper() == EquipmentStatus.ToUpper())
                    {
                        SelectedSystemStatus = SystemStatus;
                        break;
                    }
                }

                return SelectedSystemStatus;



            }
            catch (Exception ex)
            {
                logger.Error("TrasformSiebelToClick.cs;GetSystemStatus; Exception - SystemStatus: " + ex.Message);
                return SelectedSystemStatus = null;
            }
            finally
            {
                EquipmentStatus = null;
                SystemStatus = null;
                xmlDoc = null;
            }
        }

        /// <summary>
        /// Returns boolean value which denotes the Safety concern.
        /// </summary>
        /// <returns></returns>
        public bool GetSafetyConcernValue()
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["SiebelData"] != null)
                {
                    var res = (SiebelJsonToEntity)System.Web.HttpContext.Current.Session["SiebelData"];
                    var SafetyConcern = res.serviceRequest.gEHCSafetyConcern.ToLower();
                    switch (SafetyConcern)
                    {
                        case "yes" :                            
                        case "actual":                            
                        case "potential":
                            return true;
                        case "no":
                            return false;
                        default:
                            return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                logger.Error("TrasformSiebelToClick.cs;GetSafetyConcernValue; Exception - SystemStatus: " + ex.Message);
                return false;
            }
        }

    }
}