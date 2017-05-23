using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewSDTApplication.Utilities
{
    public static class ClsDropDownHelper
    {


        public enum DropDownTypes
	    {
            CancelTaskValuesInSDType
     
	    }

        public static SelectList CustomDropDownHelper(DropDownTypes dropDownType  )
        {
            switch (dropDownType)
            {
                case DropDownTypes.CancelTaskValuesInSDType:
                    return new SelectList(ClsWebConfigHelper.GetCancelTaskValuesInSDT());
                
            }
            return new SelectList(null);
        }
    
    }
}