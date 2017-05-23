using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Models
{
    public class IndextoRequest
    {
        public string earlyStart { get; set; }
        public string lateStart { get; set; }
        public int Duration { get; set; }
        public string TaskSystemID { get; set; }
        public string TaskSiteID { get; set; }
        public string profile { get; set; }
        public string TaskNotes { get; set; }
        public string PreferredFSEs { get; set; }
        public string FseSkillLevel { get; set; }
        public string TaskStatus { get; set; }
        public List<List<Array>> addressArray { get; set; }
        public string desiredDate { get; set; }
        public string PartComments { get; set; }
        public string DependencyType { get; set; }
        //Jayesh Soni - US82 - 13/04/2017 - SR Description added
        public string SRDescription { get; set; }
    }
}