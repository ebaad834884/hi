using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Models
{
    public class ClickTasksResSystemSite
    {

        public int TaskResponseBySite { get; set; }
        public int TaskResponseBySystem { get; set; }
        public bool TaskExists { get; set; }
        public bool TaskCancelAllowed { get; set; }
        public DateTime AppointmentStart { get; set; }
        public DateTime AppointmentFinish { get; set; }
        public string SiteId { get; set; }
        public string SystemId { get; set; }

        public DateTime EarlyStart { get; set; }
        public DateTime LateStart { get; set; }
    }
}