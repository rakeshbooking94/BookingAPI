using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{

    public class RTHWKOrderFormRequest
    {
        public string partner_order_id { get; set; }
        public string book_hash { get; set; }
        public string language { get; set; }
        public string user_ip { get; set; }
    }


    public class RTHWKOrderFormReponse
    {
        public RTHWKOrderFormData data { get; set; }
        public object debug { get; set; }
        public object error { get; set; }
        public string status { get; set; }
    }

    public class RTHWKOrderFormData
    {
        public int item_id { get; set; }
        public int order_id { get; set; }
        public string partner_order_id { get; set; }
        public List<PaymentType> payment_types { get; set; }
        public List<UpsellDatum> upsell_data { get; set; }
        public string checkout_time { get; set; }
        public string checkin_time { get; set; }
    }
    public class ChargePrice
    {
        public string amount { get; set; }
        public string currency_code { get; set; }
    }

    public class RecommendedPrice
    {
        public string amount { get; set; }
        public string currency_code { get; set; }
        public string show_amount { get; set; }
        public string show_currency_code { get; set; }
    }


    public class UpsellDatum
    {
        public ChargePrice charge_price { get; set; }
        public Data data { get; set; }
        public string name { get; set; }
        public int rule_id { get; set; }
        public string uid { get; set; }
    }


}