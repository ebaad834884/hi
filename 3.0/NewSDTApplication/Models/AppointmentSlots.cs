using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Models
{
    public static class DefaultClickDates
    {
        public const string DefaultClickDate = "12/30/1899 12:00:00 AM";
    }
    public class AppointmentSlots
    {
        public string EarlyStart { get; set; }
        public string LateStart { get; set; }
        public string PreferredFSE { get; set; }
        public string Grade { get; set; }
        public string TaskID { get; set; }
        //public long travelTime { get; set; } // Farhan US87/TA899 21/2/17
        //public int sourceKey { get; set; }
        //public int destinationKey { get; set; }
        public int Status { get; set; }
        public string Working { get; set; }
        public string SLA { get; set; }
        //deprecation of Bump Check mechanism - US197 - 7/4/2017 by Ebaad (This is the get set property for the 'Bump' variable)
        //public string Bump { get; set; } 
        public int SSOID { get; set; }
        public string TaskStatus { get; set; }
        public string SchedulingpolicyExtendedSlots { get; set; }
    }
}