using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{

    public class HotelSearch
    {
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        public int MinRating { get; set; }
        public int MaxRating { get; set; }
        public int SupplierId { get; set; }
        public string CityCode { get; set; }
        public string CountryCode { get; set; }

    }
    public class HotelModel
    {
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        public string Rating { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string HotelAddress { get; set; }
        public string PostalCode { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public string HotelImage { get; set; }
    }
}