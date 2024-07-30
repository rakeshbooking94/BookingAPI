using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{
    public class RTHWKPreBookRequest
    {
        public string hash { get; set; }
    }

    public class RTHWKPreBookResponse
    {
        public PreBookData data { get; set; }
        public PreBookDebug debug { get; set; }
        public string status { get; set; }
        public object error { get; set; }
    }


    public class PreBookData
    {
        public List<Hotel> hotels { get; set; }
        public Changes changes { get; set; }
    }
    public class Changes
    {
        public bool price_changed { get; set; }
    }

    public class PreBookDebug
    {
        public RTHWKPreBookRequest request { get; set; }
        public int key_id { get; set; }
        public object validation_error { get; set; }
    }
}