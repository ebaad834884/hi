using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewSDTApplication.Models
{
    public class HTTPPostParams
    {
        public string SystemID { get; set; }
        public string SerialNumber { get; set; }
        public string ServiceRequestNumber { get; set; }
        public string ActivityNo { get; set; }
        public string ServiceRequestID { get; set; }
        public string ShipToSite { get; set; }
        
    }
}