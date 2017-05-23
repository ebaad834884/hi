using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewSDTApplication.Models;

namespace NewSDTApplication.ViewModels
{
    public class SiebelTaskResponseViewModel
    {

        public SiebelJsonToEntity SiebelResponseAttributes { get; set; }
        public ClickTasksResSystemSite ClickTaskResponseAttributes { get; set; }
    }
}