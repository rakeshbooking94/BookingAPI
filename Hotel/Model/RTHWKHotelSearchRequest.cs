
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TravillioXMLOutService.Hotel.Model
{

    public class Guest
    {
        public int adults { get; set; }
        public List<int> children { get; set; }
    }

    public class RTHWKHotelSearchRequest
    {
        public string checkin { get; set; }
        public string checkout { get; set; }
        public string residency { get; set; }
        public string language { get; set; }
        public List<Guest> guests { get; set; }
        public List<string> ids { get; set; }
        public string currency { get; set; }
    }
    public class RTHWKRoomSearchRequest
    {
        public string checkin { get; set; }
        public string checkout { get; set; }
        public string residency { get; set; }
        public string language { get; set; }
        public List<Guest> guests { get; set; }
        public string id { get; set; }
        public string currency { get; set; }
    }

    public class RTHWKPreBookRequest
    {
        public string hash { get; set; }     
    }
}