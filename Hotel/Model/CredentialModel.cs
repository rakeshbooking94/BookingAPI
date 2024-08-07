using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravillioXMLOutService.Hotel.Model
{
    public abstract class CredentialModel
    {
        public string BaseUrl
        {
            get;
            set;
        }
        public string ClientId
        {
            get;
            set;
        }
        public string SecretKey
        {
            get;
            set;
        }
        public string CustomerId
        {
            get;
            set;
        }
        public int SupplierId
        {
            get;
            set;
        }
        public string Currency
        {
            get;
            set;
        }
        public string Culture
        {
            get;
            set;
        }
        public string Service
        {
            get;
            set;
        }
    

    }
}
