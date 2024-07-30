using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{

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

    public class RTHWKLookUpRateResponse
    {
        public RTHWKLookUpRateData data { get; set; }
        public RTHWKLookUpRateDebug debug { get; set; }
        public string status { get; set; }
        public object error { get; set; }
    }

    public class RTHWKLookUpRateData
    {
        public List<Hotel> hotels { get; set; }
        public OriginalRequestParams original_request_params { get; set; }
    }
    public class RTHWKLookUpRateDebug
    {
        public RTHWKLookUpRateRequest request { get; set; }
        public int key_id { get; set; }
        public object validation_error { get; set; }
    }











}