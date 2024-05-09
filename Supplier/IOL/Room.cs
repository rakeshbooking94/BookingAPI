using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TravillioXMLOutService.Supplier.IOL
{
    public class Room
    {
        public string RoomNo { get; set; }
        public string RoomType { get; set; }
        public string MealPlan { get; set; }
        public string MealPlanCode { get; set; }
        public List<string> RoomNumbers { get; set; }
        public Room() 
        {
            
        }
        public Room(XElement element)
        {
            RoomNo = element.Element("RoomNo").Value;
            RoomType = element.Element("RoomType").Value;
            MealPlan = element.Element("MealPlan").Value;
            MealPlanCode = element.Element("MealPlanCode").Value;
            RoomNumbers = new List<string>();
        }
    }
}