using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Models
{
    public class TaskAssignmentRequestedProperties
    {
               
    }
    public class TaskRequestedProperties
    {
        public string  CallID { get; set; }
        public string TaskSystemName { get; set; }
        public string Number { get; set; }
        public string MUSTJobNumber { get; set; }
        public string TaskType { get; set; }
        public string SystemID { get; set; }
        public string TaskSiteID { get; set; }
        public string  TaskSiteName { get; set; }
        public string Status { get; set; }
        public string SkillLevel { get; set; }
        public string TimeDependencies { get; set; }
        public string TaskSystemModality { get; set; }
        public string TaskSystemProductName { get; set; }
        public string  Duration { get; set; }
        public string DurationSpecified { get; set; }
        public string TaskSystemID { get; set; }
        public string  LateStart { get; set; }
        public string  EarlyStart { get; set; }
        public string Key { get; set; }
        public string IsMST { get; set; }
        public string JeopardyState { get; set; }
        public string KeySpecified{get;set;}
        public string Priority { get; set; }
        public string PrioritySpecified { get; set; }
        public string TaskSystemProductID{get;set;}
       // public AssignmentRequestedProperties assignmentrequestedproperties { get; set; }
        
    }
    public class AssignmentRequestedProperties
    {
        public string Start { get; set; }
        public string Finish { get; set; }
        public TaskRequestedProperties task { get; set; }
        public string  AssignedEngineers { get; set; }   
        public string Engineers { get; set; }                                                                  
    }
}