using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class wte_request_bookdetail
    {
        public string Token { get; set; }
        public wteRequest Request { get; set; }
        public wteAdvancedOptions AdvancedOptions { get; set; }
        public class wteRequest
        {
            public wteBookingDetailRQ BookingDetailRQ { get; set; }
        }
        public class wteBookingDetailRQ
        {
            public string InternalReference { get; set; }
        }
        public class wteAdvancedOptions
        {
            public string Currency { get; set; }
        }
    }
}