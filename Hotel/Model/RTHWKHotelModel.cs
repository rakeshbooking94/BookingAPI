using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Hotel.Model
{
    public class RTHWKHotelModel
    {
        public string address { get; set; }
        public List<AmenityGroup> amenity_groups { get; set; }
        public string check_in_time { get; set; }
        public string check_out_time { get; set; }
        public List<DescriptionStruct> description_struct { get; set; }
        public string id { get; set; }
        public List<string> images { get; set; }
        public string kind { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public List<PolicyStruct> policy_struct { get; set; }
        public string postal_code { get; set; }
        public List<RoomGroup> room_groups { get; set; }
        public Region region { get; set; }
        public int star_rating { get; set; }
        public string email { get; set; }
        public List<string> serp_filters { get; set; }
        public bool is_closed { get; set; }
        public bool is_gender_specification_required { get; set; }
        public MetapolicyStruct metapolicy_struct { get; set; }
        public string metapolicy_extra_info { get; set; }
        public object star_certificate { get; set; }
        public Facts facts { get; set; }
        public List<string> payment_methods { get; set; }
        public string hotel_chain { get; set; }
        public string front_desk_time_start { get; set; }
        public string front_desk_time_end { get; set; }
        public int semantic_version { get; set; }
    }
    public class AddFee
    {
        public string price { get; set; }
        public string currency { get; set; }
        public string price_unit { get; set; }
        public string fee_type { get; set; }
    }

    public class AmenityGroup
    {
        public List<string> amenities { get; set; }
        public string group_name { get; set; }
    }

    public class Child
    {
        public int age_start { get; set; }
        public int age_end { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string extra_bed { get; set; }
    }

    public class ChildrenMeal
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string meal_type { get; set; }
        public int age_start { get; set; }
        public int age_end { get; set; }
    }

    public class Cot
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string price_unit { get; set; }
        public int amount { get; set; }
    }

    public class Deposit
    {
        public string availability { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string price_unit { get; set; }
        public string pricing_method { get; set; }
        public string payment_type { get; set; }
        public string deposit_type { get; set; }
    }

    public class DescriptionStruct
    {
        public List<string> paragraphs { get; set; }
        public string title { get; set; }
    }

    public class Electricity
    {
        public List<int> frequency { get; set; }
        public List<int> voltage { get; set; }
        public List<string> sockets { get; set; }
    }

    public class ExtraBed
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string price_unit { get; set; }
        public int amount { get; set; }
    }

    public class Facts
    {
        public int floors_number { get; set; }
        public int rooms_number { get; set; }
        public int year_built { get; set; }
        public int year_renovated { get; set; }
        public Electricity electricity { get; set; }
    }

    public class Internet
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string price_unit { get; set; }
        public string internet_type { get; set; }
        public string work_area { get; set; }
    }

    public class Meal
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string meal_type { get; set; }
    }

    public class MetapolicyStruct
    {
        public List<Internet> internet { get; set; }
        public List<Meal> meal { get; set; }
        public List<ChildrenMeal> children_meal { get; set; }
        public List<ExtraBed> extra_bed { get; set; }
        public List<Cot> cot { get; set; }
        public List<Pet> pets { get; set; }
        public List<Shuttle> shuttle { get; set; }
        public List<Parking> parking { get; set; }
        public List<Child> children { get; set; }
        public Visa visa { get; set; }
        public List<Deposit> deposit { get; set; }
        public NoShow no_show { get; set; }
        public List<AddFee> add_fee { get; set; }
        public List<object> check_in_check_out { get; set; }
    }

    public class NameStruct
    {
        public string bathroom { get; set; }
        public string bedding_type { get; set; }
        public string main_name { get; set; }
    }

    public class NoShow
    {
        public string availability { get; set; }
        public string time { get; set; }
        public string day_period { get; set; }
    }

    public class Parking
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string price_unit { get; set; }
        public string territory_type { get; set; }
    }

    public class Pet
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string price_unit { get; set; }
        public string pets_type { get; set; }
    }

    public class PolicyStruct
    {
        public List<string> paragraphs { get; set; }
        public string title { get; set; }
    }

    public class Region
    {
        public int id { get; set; }
        public string country_code { get; set; }
        public string iata { get; set; }
        public string name { get; set; }
        public string type { get; set; }
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
        public int floor { get; set; }
        public int view { get; set; }
    }

    public class RoomGroup
    {
        public int room_group_id { get; set; }
        public List<string> images { get; set; }
        public string name { get; set; }
        public List<string> room_amenities { get; set; }
        public RgExt rg_ext { get; set; }
        public NameStruct name_struct { get; set; }
    }



    public class Shuttle
    {
        public string inclusion { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public string shuttle_type { get; set; }
        public string destination_type { get; set; }
    }

    public class Visa
    {
        public string visa_support { get; set; }
    }


}