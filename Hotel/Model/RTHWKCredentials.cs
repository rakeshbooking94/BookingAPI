using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravillioXMLOutService.Hotel.Model
{
    public class RTHWKCredentials : CredentialModel
    {


        public string IpAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }


        public RTHWKCredentials()
        {
            this.BaseUrl = "https://api.worldota.net/api/b2b/v3/";
            this.ClientId = "9034";
            this.SecretKey = "a64cb869-93fc-44a0-8343-102334608f7d";
            this.CustomerId = "111025";
            this.SupplierId = 24;
            this.Currency = "EUR";
            this.Culture = "en";
            this.Email = "s.hijazin@bookingexpress.co";
            this.CustomerName = "Saleem Hijazin";
            this.Phone = "+962795674029";
        }
    }
}
