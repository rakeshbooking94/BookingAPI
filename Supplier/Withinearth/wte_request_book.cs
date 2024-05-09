using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class wte_request_book
    {
        public string Token { get; set; }
        public wteRequest Request { get; set; }
        public wteAdvancedOptions AdvancedOptions { get; set; }

        public class wteRequest
        {
            public string SearchKey { get; set; }
            public wteBookRQ BookRQ { get; set; }
        }
        public class wteBookRQ
        {
            public string BookingToken { get; set; }
            public double TotalPrice { get; set; }
            public string InternalReference { get; set; }
            public List<wteHotelRoom> HotelRooms { get; set; }
        }
        public class wteHotelRoom
        {
            public int UniqueId { get; set; }
            public string RoomNo { get; set; }
            public string IsLead { get; set; }
            public string PaxType { get; set; }
            public string Prefix { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ChildAge { get; set; }
        }
        public class wteAdvancedOptions
        {
            public string Currency { get; set; }
            public string CustomerIpAddress { get; set; }

        }
    }
}