﻿using System;
using System.Collections.Generic;

namespace NewSDTApplication.Models
{
    public class SiebelJsonToEntity
    {
        public ServiceRequest serviceRequest { get; set; }
    }

    public class ActivityDetailList
    {
        public string priority { get; set; }
        public string type { get; set; }
        public string ownerFirstName { get; set; }
        public string ownerLastName { get; set; }
        public string comment { get; set; }
        public string primaryOwnedBy { get; set; }
        //public string subType { get; set; } // Farhan US87/TA899 21/2/17
        public string equipmentStatus { get; set; }
        public string description { get; set; }
        //public string testProcedure { get; set; } // Farhan US87/TA899 21/2/17
        //public string problem { get; set; } // Farhan US87/TA899 21/2/17
        //public string primaryOwnerId { get; set; } // Farhan US87/TA899 21/2/17
        public string Id { get; set; }
        public string status { get; set; }
        public string activityUID { get; set; }
        public string activityId { get; set; }
        //public string planned { get; set; } // Farhan US87/TA899 21/2/17
        //public string plannedCompletion { get; set; } // Farhan US87/TA899 21/2/17
        public string gehcSequenceNumber { get; set; }
        public string gehcPMLevelofService { get; set; }
        //public List<object> timeTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> partTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> expenseTracker { get; set; }  // Farhan US87/TA899 21/2/17
        //public List<object> recommendedPart { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> assessmentDetails { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> toolTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> salesOrderTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> installTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> missingAssetsTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> activityAttachmentTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> activityNotesTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public List<object> UdiTracker { get; set; } // Farhan US87/TA899 21/2/17
        //public string createdBy { get; set; } // Farhan US87/TA899 21/2/17
        public string gEHCDesiredDateTime { get; set; }
        //public string gEHCOwnerName { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCPatConsequence { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCProblemFound { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCScheduleDate { get; set; } // Farhan US87/TA899 21/2/17
        public string serviceRegion { get; set; }
        //public string serviceRegionId { get; set; }  // Farhan US87/TA899 21/2/17
        //public string subStatus { get; set; } // Farhan US87/TA899 21/2/17
        //public string earliestStart { get; set; } // Farhan US87/TA899 21/2/17
        //public string latestStart { get; set; } // Farhan US87/TA899 21/2/17
        
        
    }
    public class SrNotesList
    {
        //public string gEHCFFAURLLink { get; set; } // Farhan US87/TA899 21/2/17
        //public string created { get; set; } // Farhan US87/TA899 21/2/17
        //public string createdByFirstName { get; set; } // Farhan US87/TA899 21/2/17
        //public string createdByLastName { get; set; } // Farhan US87/TA899 21/2/17
        public string Id { get; set; }
        public string note { get; set; }
        //public string noteType { get; set; } // Farhan US87/TA899 21/2/17
        //public string srPrivate { get; set; } // Farhan US87/TA899 21/2/17
        //public string serviceRequestId { get; set; } // Farhan US87/TA899 21/2/17
        //public string gehcSRNumber { get; set; } // Farhan US87/TA899 21/2/17
        //public string gehcSRId { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCFFAURL { get; set; }
    }
    public class ServiceRequest
    {
        //DV
        //public string stvCancelTaskValuesInSDT { get; set; } // Farhan US87/TA899 21/2/17
        //DV

        // public string customerPONumber { get; set; } // Farhan US87/TA899 21/2/17
        //public string assetId { get; set; } // Farhan US87/TA899 21/2/17
        
        //Tejashree - 07/04/2017 - Scope: US210 Contract Popup Issue
        // Farhan US87/TA899 21/2/17
        //Below property was commented before for US87. Uncommented for US210  
        public string openedDate { get; set; } 
        
        public string description { get; set; }
        //public string productDescription { get; set; } // Farhan US87/TA899 21/2/17
        public string owner { get; set; }
        public string srNumber { get; set; }
        public string srSubType { get; set; }
        //public string serialNumber { get; set; } // Farhan US87/TA899 21/2/17
        public string shipToAddress { get; set; }
        //public string ownerLastName { get; set; } // Farhan US87/TA899 21/2/17
        public string srType { get; set; }
        public string countryCode { get; set; }
        //public string ownerFirstName { get; set; } // Farhan US87/TA899 21/2/17
        //public string closedDate { get; set; } // Farhan US87/TA899 21/2/17
        public string priority { get; set; }
        public string fmiDueDate { get; set; }
        public string source { get; set; }
        //public string subStatus { get; set; } // Farhan US87/TA899 21/2/17
        public string contactFirstName { get; set; }
        public string contactLastName { get; set; }
        public string status { get; set; }
        public List<ActivityDetailList> activityDetailList { get; set; }
        public string created { get; set; }
        //public string createdBy { get; set; } // Farhan US87/TA899 21/2/17
        public string Id { get; set; }
        //public List<SrNotesList> srNotesList { get; set; } // Farhan US87/TA899 21/2/17
        public string gEMSEntitlementFlag { get; set; }
        public string contactEmail { get; set; }
        public string fMINumber { get; set; }
        //public string gEHCPatientUse { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCRoomDepartment { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCRuleID { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCSROrganization { get; set; } // Farhan US87/TA899 21/2/17
        public string gEHCSafetyConcern { get; set; }
        public string gEHCSchedDate { get; set; }
        //public string gEHCSourceSubType { get; set; } // Farhan US87/TA899 21/2/17
        //public string gEHCSystemId { get; set; } // Farhan US87/TA899 21/2/17
        public string gEHCWorkPhone { get; set; }
        //public string operatingUnit { get; set; } // Farhan US87/TA899 21/2/17
        //public string shippingAccountSite { get; set; } // Farhan US87/TA899 21/2/17
        public string gehcFse1 { get; set; }
        public string gehcFse2 { get; set; }
        public string gehcFse3 { get; set; }
        public string gehcHours { get; set; }
        public string gehcMinutes { get; set; }
        //public int gehcHours { get; set; }
        //public int gehcMinutes { get; set; }
        public DateTime earlyStart { get; set; }
        public DateTime lateStart { get; set; }

        public string city { get; set; }
        public string address { get; set; }
        public string countryID { get; set; }
        public string postcode { get; set; }

        //EquipmentStatus changed to currentEquiptmentStatus.Done by Phani Kanth P [10/18/2016]
        public string currentEquiptmentStatus { get; set; }
    }


    public class PartToolAddress
    {

        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string PartDeliveryType { get; set; }
        public string CountryID { get; set; }
        public string PartComment { get; set; }
        public string deliveryDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string IsMST { get; set; }
        public string IsCritical { get; set; }

    }

}