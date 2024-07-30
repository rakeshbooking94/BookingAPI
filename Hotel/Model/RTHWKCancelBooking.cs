using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{
    public class RTHWKCancelRequest
    {
        public string partner_order_id { get; set; }
    }


    public class RTHWKCancelResponse
    {
        public RTHWKCancelData data { get; set; }
        public string error { get; set; }
        public string status { get; set; }
    }
    public class AmountInfo
    {
        public double amount_commission { get; set; }
        public double amount_gross { get; set; }
        public double amount_net { get; set; }
    }

    public class AmountPayable
    {
        public double amount { get; set; }
        public AmountInfo amount_info { get; set; }
        public string currency_code { get; set; }
    }

    public class AmountRefunded
    {
        public double amount { get; set; }
        public AmountInfo amount_info { get; set; }
        public string currency_code { get; set; }
    }

    public class AmountSell
    {
        public double amount { get; set; }
        public AmountInfo amount_info { get; set; }
        public string currency_code { get; set; }
    }

    public class RTHWKCancelData
    {
        public AmountPayable amount_payable { get; set; }
        public AmountRefunded amount_refunded { get; set; }
        public AmountSell amount_sell { get; set; }
    }

  


}