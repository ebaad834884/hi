using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewSDTApplication.App_Start
{
    public class FilterConfig
    { 
        //Code Start-Hita - 12/05/2017 - US250/TA1680 
        public FilterConfig()
        {
        }
        //Code End-Hita - 12/05/2017 - US250/TA1680 

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}