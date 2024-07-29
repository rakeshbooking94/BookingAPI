using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{
    public class RTHWKLookUpRate
    {
    }
    public class RTHWKLookUpRateRequest
    {
        public string book_hash { get; set; }
        public string language { get; set; }
    }

    public class OriginalRequestParams
    {
        public string checkin { get; set; }
        public string checkout { get; set; }
        public List<Guest> guests { get; set; }
        public string residency { get; set; }
    }

    public class Data
    {
        public List<Hotel> hotels { get; set; }
        public OriginalRequestParams original_request_params { get; set; }
    }
    public class Debug
    {
        public Request request { get; set; }
        public int key_id { get; set; }
        public object validation_error { get; set; }
    }











}