using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{

    public class RTHOrderFinishRequest
    {
        public User user { get; set; }
        public SupplierData supplier_data { get; set; }
        public Partner partner { get; set; }
        public string language { get; set; }
        public List<Room> rooms { get; set; }
        public OrderFinishPaymentType payment_type { get; set; }
    }


    public class OrderFinishPaymentType
    {
        public string type { get; set; }
        public string amount { get; set; }
        public string currency_code { get; set; }


    }


    public class Room
    {
        public List<GuestName> guests { get; set; }
    }

    public class GuestName
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class Partner
    {
        public string partner_order_id { get; set; }
        public string comment { get; set; }

    }





    public class SupplierData
    {
        public string first_name_original { get; set; }
        public string last_name_original { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }


    public class User
    {
        public string email { get; set; }
        public string comment { get; set; }
        public string phone { get; set; }
    }


}