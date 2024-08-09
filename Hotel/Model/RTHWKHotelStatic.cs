using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{
    public class RTHWKHotelStatic
    {
        public RTHWKHotelStaticData data { get; set; }
        public object debug { get; set; }
        public object error { get; set; }
        public string status { get; set; }
    }
    public class RTHWKHotelStaticData
    {
        public List<Bedding> beddings { get; set; }
        public List<MealType> meals { get; set; }
        public List<RoomAmenity> room_amenities { get; set; }
        public List<SerpFilter> serp_filters { get; set; }
        public List<SocketType> socket_types { get; set; }
        public List<Taxis> taxes { get; set; }
    }



    public class Bedding
    {
        public Locale locale { get; set; }
        public string name { get; set; }
    }



    public class Locale
    {
        public string ar { get; set; }
        public string bg { get; set; }
        public string cs { get; set; }
        public string de { get; set; }
        public string el { get; set; }
        public string en { get; set; }
        public string es { get; set; }
        public string fr { get; set; }
        public string he { get; set; }
        public string hu { get; set; }
        public string it { get; set; }
        public string ja { get; set; }
        public string kk { get; set; }
        public string ko { get; set; }
        public string nl { get; set; }
        public string pl { get; set; }
        public string pt { get; set; }
        public string pt_PT { get; set; }
        public string ro { get; set; }
        public string ru { get; set; }
        public string sq { get; set; }
        public string sr { get; set; }
        public string th { get; set; }
        public string tr { get; set; }
        public string uk { get; set; }
        public string vi { get; set; }
        public string zh_CN { get; set; }
    }


    public class MealType
    {
        public Locale locale { get; set; }
        public string name { get; set; }
    }

    public class RoomAmenity
    {
        public Locale locale { get; set; }
        public string name { get; set; }
    }



    public class SerpFilter
    {
        public Locale locale { get; set; }
        public string name { get; set; }
    }

    public class SocketType
    {
        public Locale locale { get; set; }
        public string name { get; set; }
    }

    public class Taxis
    {
        public Locale locale { get; set; }
        public string name { get; set; }
    }













}