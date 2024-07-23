﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TravillioXMLOutService.Hotel.Model
{



    public class RTHWKHotelSearchResponse
    {
        public Data data { get; set; }
        public Debug debug { get; set; }
        public string status { get; set; }
        public object error { get; set; }
    }













    public class CancellationPenalties
    {
        public List<Policy> policies { get; set; }
        public DateTime? free_cancellation_before { get; set; }
    }

    public class Amount
    {
        public string amount_gross { get; set; }
        public string amount_net { get; set; }
        public string amount_commission { get; set; }
    }

    public class CommissionInfo
    {
        public Amount show { get; set; }
        public Amount charge { get; set; }
    }

    public class Data
    {
        public List<Hotel> hotels { get; set; }
        public int total_hotels { get; set; }
    }

    public class Debug
    {
        public RTHWKHotelSearchRequest request { get; set; }
        public int key_id { get; set; }
        public object validation_error { get; set; }
    }



    public class Hotel
    {
        public string id { get; set; }
        public List<Rate> rates { get; set; }
        public object bar_price_data { get; set; }
    }

    public class PaymentOptions
    {
        public List<PaymentType> payment_types { get; set; }
    }

    public class PaymentType
    {
        public string amount { get; set; }
        public string show_amount { get; set; }
        public string currency_code { get; set; }
        public string show_currency_code { get; set; }
        public object by { get; set; }
        public bool is_need_credit_card_data { get; set; }
        public bool is_need_cvc { get; set; }
        public string type { get; set; }
        public VatData vat_data { get; set; }
        public TaxData tax_data { get; set; }
        public Perks perks { get; set; }
        public CommissionInfo commission_info { get; set; }
        public CancellationPenalties cancellation_penalties { get; set; }
        public object recommended_price { get; set; }
    }

    public class Perks
    {
    }

    public class Policy
    {
        public DateTime? start_at { get; set; }
        public DateTime? end_at { get; set; }
        public string amount_charge { get; set; }
        public string amount_show { get; set; }
        public CommissionInfo commission_info { get; set; }
    }

    public class Rate
    {

        public Rate()
        {
            this.totalPrice = daily_prices.Sum();

        }
        public double totalPrice { get; set; }
        public string match_hash { get; set; }
        public List<double> daily_prices { get; set; }
        public string meal { get; set; }
        public PaymentOptions payment_options { get; set; }
        public object bar_rate_price_data { get; set; }
        public RgExt rg_ext { get; set; }
        public string room_name { get; set; }
        public object room_name_info { get; set; }
        public List<string> serp_filters { get; set; }
        public object sell_price_limits { get; set; }
        public int allotment { get; set; }
        public List<string> amenities_data { get; set; }
        public bool any_residency { get; set; }
        public object deposit { get; set; }
        public object no_show { get; set; }
        public RoomDataTrans room_data_trans { get; set; }
    }



    public class RgExt
    {
        public int @class { get; set; }
        public int quality { get; set; }
        public int sex { get; set; }
        public int bathroom { get; set; }
        public int bedding { get; set; }
        public int family { get; set; }
        public int capacity { get; set; }
        public int club { get; set; }
        public int bedrooms { get; set; }
        public int balcony { get; set; }
        public int view { get; set; }
        public int floor { get; set; }
    }

    public class RoomDataTrans
    {
        public string main_room_type { get; set; }
        public string main_name { get; set; }
        public object bathroom { get; set; }
        public string bedding_type { get; set; }
        public string misc_room_type { get; set; }
    }



    public class Show
    {
        public string amount_gross { get; set; }
        public string amount_net { get; set; }
        public string amount_commission { get; set; }
    }

    public class TaxData
    {
        public List<Tax> taxes { get; set; }
    }

    public class VatData
    {
        public bool included { get; set; }
        public bool applied { get; set; }
        public string amount { get; set; }
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Tax
    {
        public string name { get; set; }
        public bool included_by_supplier { get; set; }     
        public double amount { get; set; }
        public string currency_code { get; set; }
      
    }


}