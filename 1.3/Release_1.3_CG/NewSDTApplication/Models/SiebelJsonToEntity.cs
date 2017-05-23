using System;
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
        public string subType { get; set; }
        public string equipmentStatus { get; set; }
        public string description { get; set; }
        public string testProcedure { get; set; }
        public string problem { get; set; }
        public string primaryOwnerId { get; set; }
        public string Id { get; set; }
        public string status { get; set; }
        public string activityUID { get; set; }
        public string activityId { get; set; }
        public string planned { get; set; }
        public string plannedCompletion { get; set; }
        public string gehcSequenceNumber { get; set; }
        public string gehcPMLevelofService { get; set; }
        public List<object> timeTracker { get; set; }
        public List<object> partTracker { get; set; }
        public List<object> expenseTracker { get; set; }
        public List<object> recommendedPart { get; set; }
        public List<object> assessmentDetails { get; set; }
        public List<object> toolTracker { get; set; }
        public List<object> salesOrderTracker { get; set; }
        public List<object> installTracker { get; set; }
        public List<object> missingAssetsTracker { get; set; }
        public List<object> activityAttachmentTracker { get; set; }
        public List<object> activityNotesTracker { get; set; }
        public List<object> UdiTracker { get; set; }
        public string createdBy { get; set; }
        public string gEHCDesiredDateTime { get; set; }
        public string gEHCOwnerName { get; set; }
        public string gEHCPatConsequence { get; set; }
        public string gEHCProblemFound { get; set; }
        public string gEHCScheduleDate { get; set; }
        public string serviceRegion { get; set; }
        public string serviceRegionId { get; set; }
        public string subStatus { get; set; }
        public string earliestStart { get; set; }
        public string latestStart { get; set; }
        
        
    }
    public class SrNotesList
    {
        public string gEHCFFAURLLink { get; set; }
        public string created { get; set; }
        public string createdByFirstName { get; set; }
        public string createdByLastName { get; set; }
        public string Id { get; set; }
        public string note { get; set; }
        public string noteType { get; set; }
        public string srPrivate { get; set; }
        public string serviceRequestId { get; set; }
        public string gehcSRNumber { get; set; }
        public string gehcSRId { get; set; }
        public string gEHCFFAURL { get; set; }
    }
    public class ServiceRequest
    {
        //DV
        public string stvCancelTaskValuesInSDT { get; set; }
        //DV

        public string customerPONumber { get; set; }
        public string assetId { get; set; }
        public string openedDate { get; set; }
        public string description { get; set; }
        public string productDescription { get; set; }
        public string owner { get; set; }
        public string srNumber { get; set; }
        public string srSubType { get; set; }
        public string serialNumber { get; set; }
        public string shipToAddress { get; set; }
        public string ownerLastName { get; set; }
        public string srType { get; set; }
        public string countryCode { get; set; }
        public string ownerFirstName { get; set; }
        public string closedDate { get; set; }
        public string priority { get; set; }
        public string fmiDueDate { get; set; }
        public string source { get; set; }
        public string subStatus { get; set; }
        public string contactFirstName { get; set; }
        public string contactLastName { get; set; }
        public string status { get; set; }
        public List<ActivityDetailList> activityDetailList { get; set; }
        public string created { get; set; }
        public string createdBy { get; set; }
        public string Id { get; set; }
        public List<SrNotesList> srNotesList { get; set; }
        public string gEMSEntitlementFlag { get; set; }
        public string contactEmail { get; set; }
        public string fMINumber { get; set; }
        public string gEHCPatientUse { get; set; }
        public string gEHCRoomDepartment { get; set; }
        public string gEHCRuleID { get; set; }
        public string gEHCSROrganization { get; set; }
        public string gEHCSafetyConcern { get; set; }
        public string gEHCSchedDate { get; set; }
        public string gEHCSourceSubType { get; set; }
        public string gEHCSystemId { get; set; }
        public string gEHCWorkPhone { get; set; }
        public string operatingUnit { get; set; }
        public string shippingAccountSite { get; set; }
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