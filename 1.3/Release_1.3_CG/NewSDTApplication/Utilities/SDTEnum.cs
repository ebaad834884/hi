using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Utilities
{
    public class SDTEnum
    {
        public enum Operation
        {
            RequestAppointment = 1,
            ProcessTask = 2,
            CancelTask = 3,
            UpdateTask = 4,
        }

        public enum OperationStatus
        {
            Processed = 1,
            Updated = 2,
            Cancelled = 3,
            Deleted = 4,
            NotProcessed = 5
        }

        public enum TaskStatus
        {
            Cancel = 1,
            New = 2,
            Tentative = 3,
            Completed = 4,
            Incompleted = 5
        }

        public enum TaskStatusValue
        {
            New,
            Tentative,
            Assigned,
            Acknowledged,
            Rejected,
            Cancelled
        }

        public enum ExtendedSlotsType
        {
            ExtendedSlots,
            ExtendedSlotsWithOneHour,
            ExtendedSlotsWithTwoHours
        }
        
        //Rejected by FSE         
    }

}
