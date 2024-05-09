using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class wte_request_search
    {
        public string Token { get; set; }
        public wteRequest Request { get; set; }
        public wteAdvancedOptions AdvancedOptions { get; set; }


        public class wteRooms
        {
            public int RoomNo { get; set; }
            public int NoofAdults { get; set; }
            public int NoOfChild { get; set; }
            public IList<string> ChildAge { get; set; }

        }
        public class wteStarRating
        {
            public int Min { get; set; }
            public int Max { get; set; }

        }
        public class wteFilters
        {
            public string IsRecommendedOnly { get; set; }
            public string IsShowRooms { get; set; }
            public string IsOnlyAvailable { get; set; }
            public wteStarRating StarRating { get; set; }
            public string HotelIds { get; set; }

        }
        public class wteRequest
        {
            public IList<wteRooms> Rooms { get; set; }
            //public string CityID { get; set; }
            public string CountryID { get; set; }
            public string CheckInDate { get; set; }
            public string CheckOutDate { get; set; }
            public string NoofNights { get; set; }
            public string Nationality { get; set; }
            public wteFilters Filters { get; set; }

        }
        public class wteAdvancedOptions
        {
            public string Currency { get; set; }
            public string CustomerIpAddress { get; set; }

        }

    }
}