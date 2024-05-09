using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class wte_request_cancel
    {
        public string Token { get; set; }
        public wteRequest Request { get; set; }
        public wteAdvancedOptions AdvancedOptions { get; set; }
        public class wteRequest
        {
            public wteCancelRQ CancelRQ { get; set; }
        }
        public class wteCancelRQ
        {
            public int BookingId { get; set; }
            public int BookingDetailId { get; set; }
            public string CancelCode { get; set; }
            public int CancelAll { get; set; }
            public string Reason { get; set; }
        }
        public class wteAdvancedOptions
        {
            public string Currency { get; set; }
        }
    }
}