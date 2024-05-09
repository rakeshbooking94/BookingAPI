using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class wte_request_recheck
    {
        public string Token { get; set; }
        public wteRequest Request { get; set; }
        public wteAdvancedOptions AdvancedOptions { get; set; }
        public class wteRequest
        {
            public string SearchKey { get; set; }
            public wteHotelOption HotelOption { get; set; }
        }
        public class wteHotelOption
        {
            public string HotelOptionId { get; set; }
            public List<wteHotelRooms> HotelRooms { get; set; }
        }
        public class wteHotelRooms
        {
            public string RoomNo { get; set; }
            public string RoomToken { get; set; }
        }
        public class wteAdvancedOptions
        {
            public string Currency { get; set; }
            public string CustomerIpAddress { get; set; }

        }
    }
}