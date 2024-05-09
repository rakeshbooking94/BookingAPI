using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Models.DotW
{
    public class Client
    {
        public long Customer
        {
            get;
            set;
        }
        public string TrackNo
        {
            get;
            set;
        }
        public string Action
        {
            get;
            set;
        }
        public long ActionId
        {
            get;
            set;
        }

        public long LogTypeId
        {
            get;
            set;
        }
        // Hotel id use for when multiple tab open hotel for booking unique  identify for the selected hotel get records in DB.
        public long HotelId
        {
            get;
            set;
        }


        
       
    }
}