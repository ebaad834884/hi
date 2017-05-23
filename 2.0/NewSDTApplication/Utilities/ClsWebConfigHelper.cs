using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Utilities
{
    public static class ClsWebConfigHelper
    {
        public static List<string> GetCancelTaskValuesInSDT()
        {
            List<string> lstCancelTaskValuesInSDT = System.Configuration.ConfigurationManager.AppSettings["CancelTaskValuesInSDT"].Split(';').ToList();
            return lstCancelTaskValuesInSDT;

        }
    }
}