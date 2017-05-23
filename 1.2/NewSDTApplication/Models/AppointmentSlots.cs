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
        public long travelTime { get; set; }
        //public int sourceKey { get; set; }
        //public int destinationKey { get; set; }
        public int Status { get; set; }
        public string Working { get; set; }
        public string SLA { get; set; }
        public string Bump { get; set; }
        public int SSOID { get; set; }
        public string TaskStatus { get; set; }
        public string SchedulingpolicyExtendedSlots { get; set; }
    }
}