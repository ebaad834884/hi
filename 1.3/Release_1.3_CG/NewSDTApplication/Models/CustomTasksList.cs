using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Models
{
    public class CustomTasksList
    {
        public string Rating { get; set; }
        public string Sitename { get; set; }
        public string Systemname { get; set; }
        public string SystemID { get; set; } //Added by AJaySara
        public string TaskID { get; set; } //Added by AJaySara
        public string EarlyStart { get; set; }

        //Added by Phani Kanth P.
        public string AppointmentStart { get; set; }
        public string AppointmentFinish { get; set; }

        public string AssignmentStart { get; set; }
        public string AssignmentFinish { get; set; }

        public string AssignedFSE { get; set; }

        public string Tasktype { get; set; }
        public string Duration { get; set; }
        public string Level { get; set; }
        public string ProductID { get; set; }
        public string RFS { get; set; }
        public string Status { get; set; }
        public string Dependency { get; set; }
        public string RatingColor { get; set; }
        public string CallID { get; set; }
        public string TaskNumber { get; set; }
        public string MUSTJobNumber { get; set; }
        public bool IsMst { get; set; }
        public string SiteCountActual { get; set; }
        public string SystemCountActual { get; set; }
        public string SRnumber { get; set; }
    }

    public class DependencyTasksList
    {
        //public List<string> SSCount { get; set; }
        public int SiteCountActual { get; set; }
        public int SystemCountActual { get; set; }
        public string SiteStatusActual { get; set; }
        public string SystemStatusActual { get; set; }

        public int SiteCountPotential { get; set; }
        public int SystemCountPotential { get; set; }
        public string SiteStatusPotential { get; set; }
        public string SystemStatusPotential { get; set; }

        public string SiteStatus { get; set; }
        public string SystemStatus { get; set; }
        public string taskiddep { get; set; }        
        public int  receiveTime { get; set; }
    }
}