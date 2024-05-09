using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.IOL
{
    public class IOLLogModel
    {
        public long CustomerID
        {
            get;
            set;
        }
        public string TrackNo
        {
            get;
            set;
        }
        public string LogType
        {
            get;
            set;
        }
        public long LogTypeID
        {
            get;
            set;
        }
        public long SuplId
        {
            get;
            set;
        }
    }
}