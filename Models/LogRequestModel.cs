using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Models
{
    public class LogRequestModel
    {
        public string TrackNumber
        {
            get;
            set;
        }
        public string IpAddress
        {
            get;
            set;
        }

        public long CustomerId
        {
            get;
            set;
        }
        public int LogTypeId
        {
            get;
            set;
        }
 
        public long SupplierId
        {
            get;
            set;
        }

        public string  LogResponse
        {
            get;
            set;
        }

        public bool IsResult
        {
            get;
            set;
        }
    }
}