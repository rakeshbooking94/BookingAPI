using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class wte_request_cancelcharge
    {
        public string Token { get; set; }
        public wteRequest Request { get; set; }
        public wteAdvancedOptions AdvancedOptions { get; set; }
        public class wteRequest
        {
            public wteCheckHotelCancellationChargesRQ CheckHotelCancellationChargesRQ { get; set; }
        }
        public class wteCheckHotelCancellationChargesRQ
        {
            public int BookingId { get; set; }
            public string InternalReference { get; set; }
            public string ReferenceNo { get; set; }
        }
        public class wteAdvancedOptions
        {
            public string Currency { get; set; }
        }
    }
}