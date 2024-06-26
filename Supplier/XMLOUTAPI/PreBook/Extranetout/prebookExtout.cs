﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Common.DotW;


namespace TravillioXMLOutService.Supplier.XMLOUTAPI.PreBook.Extranetout
{
    public class prebookExtout : IDisposable
    {
        #region GlobalVars
        XElement reqTravayoo = null;
        string dmc = string.Empty;
        string customerid = string.Empty;
        string suppID = string.Empty;
        #endregion
        #region Extranet Hotel's Pre Booking Response
        public XElement GetprebookExtranetout(XElement req, XElement supresponse)
        {
            reqTravayoo = req;
            XElement htlresponse = null;
            try
            {
                string tnc = string.Empty;
                string dmc = string.Empty;
                suppID = req.Descendants("GiataHotelList").FirstOrDefault().Attribute("GSupID").Value;
                dmc = req.Descendants("GiataHotelList").FirstOrDefault().Attribute("custName").Value;
                tnc = supresponse.Descendants("rate").FirstOrDefault().Attribute("termCondition").Value;
                htlresponse = new XElement("Hotel",
                                           new XElement("HotelID", Convert.ToString(supresponse.Attribute("code").Value)),
                                                       new XElement("HotelName", Convert.ToString(supresponse.Attribute("name").Value)),
                                                       new XElement("Status", "true"),
                                                       new XElement("TermCondition", tnc),
                                                       new XElement("HotelImgSmall", Convert.ToString("")),
                                                       new XElement("HotelImgLarge", Convert.ToString("")),
                                                       new XElement("MapLink", ""),
                                                       new XElement("DMC", dmc),
                                                       new XElement("sourcekey", supresponse.Attribute("sourcekey").Value),
                                                       new XElement("Currency", Convert.ToString(supresponse.Attribute("currency").Value)),
                                                       new XElement("Offers", "")
                                                       , new XElement("Rooms",
                                                getroomextOut(supresponse.Descendants("room").ToList(), supresponse.Descendants("cancellationPolicy").ToList())
                                               )
                    );
            }
            catch(Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GetprebookExtranetout";
                ex1.PageName = "prebookExtout";
                ex1.CustomerID = req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = req.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                return null;
            }
            return htlresponse;
        }
        #endregion
        #region grouping Class
        private class roomgroup
        {
            public string Key { get; set; }
            public int Count { get; set; }

            public int allocation { get; set; }

            public string OR { get; set; }

            public List<string> rno { get; set; }
        }
        private class roomsss
        {
            public int rno { get; set; }
            public string rid { get; set; }
            public string risavail { get; set; }
        }
        #endregion
        #region Extranet Hotel's Room Listing
        private IEnumerable<XElement> getroomextOut(List<XElement> roomlist, List<XElement> cxllist)
        {
            List<XElement> str = new List<XElement>();
            List<XElement> roomList1 = new List<XElement>();
            List<XElement> roomList2 = new List<XElement>();
            List<XElement> roomList3 = new List<XElement>();
            List<XElement> roomList4 = new List<XElement>();
            List<XElement> roomList5 = new List<XElement>();
            List<XElement> ttlroom = reqTravayoo.Descendants("RoomPax").ToList();
            int totalroom = Convert.ToInt32(reqTravayoo.Descendants("RoomPax").Count());
            #region Room Count 1
            if (totalroom == 1)
            {
                #region Get Combination (Room 1)
                roomList1 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "1").ToList();
                #endregion
                for (int m = 0; m < roomList1.Count(); m++)
                {
                    List<XElement> pricebrkups = roomList1[m].Descendants("dailyRate").ToList();
                    List<XElement> promotions1 = roomList1[m].Descendants("offer").ToList();
                    int group = 0;
                    string isavailable = "false";
                    if (roomList1[m].Attribute("onRequest").Value == "false" && Convert.ToInt32(roomList1[m].Attribute("allotment").Value) > 0)
                    {
                        isavailable = "true";
                    }
                    else if (roomList1[m].Attribute("onRequest").Value == "false" && Convert.ToInt32(roomList1[m].Attribute("allotment").Value) == 0)
                    {
                        group = 1;
                    }
                    else if (roomList1[m].Attribute("onRequest").Value == "true" && Convert.ToInt32(roomList1[m].Attribute("allotment").Value) > 0)
                    {
                        isavailable = "true";
                    }
                    if (group == 0)
                    {
                        try
                        {
                            #region With Board Bases
                            str.Add(new XElement("RoomTypes", new XAttribute("TotalRate", roomList1[m].Attribute("net").Value), new XAttribute("Index", m + 1),
                                new XElement("Room",
                                     new XAttribute("ID", Convert.ToString(roomList1[m].Parent.Parent.Attribute("code").Value)),
                                     new XAttribute("SuppliersID", suppID),
                                     new XAttribute("RoomSeq", "1"),
                                     new XAttribute("SessionID", Convert.ToString(roomList1[m].Attribute("rateKey").Value)),
                                     new XAttribute("RoomType", Convert.ToString(roomList1[m].Parent.Parent.Attribute("name").Value)),
                                     new XAttribute("OccupancyID", Convert.ToString("")),
                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                     new XAttribute("MealPlanID", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                     new XAttribute("MealPlanName", Convert.ToString(roomList1[m].Attribute("boardName").Value)),
                                     new XAttribute("MealPlanCode", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                     new XAttribute("MealPlanPrice", ""),
                                     new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                     new XAttribute("TotalRoomRate", Convert.ToString(roomList1[m].Attribute("net").Value)),
                                     new XAttribute("CancellationDate", ""),
                                     new XAttribute("CancellationAmount", ""),
                                     new XAttribute("sourcekey", roomList1[m].Attribute("sourcekey").Value),
                                     new XAttribute("isAvailable", isavailable),
                                     new XElement("RequestID", Convert.ToString("")),
                                     new XElement("Offers", ""),
                                     new XElement("PromotionList", GetHotelpromotionsExtranet(promotions1)),
                                     new XElement("CancellationPolicy", ""),
                                     new XElement("Amenities", new XElement("Amenity", "")),
                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                     new XElement("Supplements", Getsupplementsextranet(roomList1[m].Descendants("supplement").ToList())),
                                         new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups)),
                                         new XElement("AdultNum", Convert.ToString(ttlroom[0].Descendants("Adult").FirstOrDefault().Value)),
                                         new XElement("ChildNum", Convert.ToString(ttlroom[0].Descendants("Child").FirstOrDefault().Value))
                                     ),
                                     new XElement("CancellationPolicies",
                                 GetRoomCancellationPolicyExtranet(cxllist))
                                 ));
                            #endregion
                        }
                        catch { }
                    }
                }
                return str;
            }
            #endregion
            #region Room Count 2
            if (totalroom == 2)
            {
                List<roomsss> roomssss = new List<roomsss>() { new roomsss() { rno = 1, risavail = "false" }, new roomsss() { rno = 2, risavail = "false" } };
                #region Get Combination (Room 1)
                roomList1 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "1").ToList();
                #endregion
                #region Get Combination (Room 2)
                roomList2 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "2").ToList();
                #endregion
                int group = 0;
                for (int m = 0; m < roomList1.Count(); m++)
                {
                    for (int n = 0; n < roomList2.Count(); n++)
                    {
                        string bb1 = roomList1[m].Attribute("boardCode").Value;
                        string bb2 = roomList2[n].Attribute("boardCode").Value;
                        if (bb1 == bb2)
                        {
                            try
                            {
                                String condition = "";
                                List<string> ratekeylist = new List<string>();
                                ratekeylist.Add(roomList1[m].Parent.Parent.Attribute("code").Value);
                                ratekeylist.Add(roomList2[n].Parent.Parent.Attribute("code").Value);
                                int iii = 0;
                                foreach (var r in ratekeylist)
                                {
                                    roomssss[iii].rid = r;
                                    iii++;
                                }
                                var grouped = ratekeylist.GroupBy(ss => ss).Select(ax => new roomgroup
                                {
                                    Key = ax.Key,
                                    Count = ax.Count(),
                                    allocation = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value)),
                                    OR = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("onRequest").FirstOrDefault().Value

                                }).ToList();
                                int k = 0;
                                foreach (var item in grouped)
                                {
                                    var rtkey = item.Key;
                                    var count = item.Count;
                                    int totalt = roomlist.Where(x => x.Attribute("code").Value == rtkey).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value));
                                    item.allocation = totalt;
                                    if (k == grouped.Count() - 1)
                                    {
                                        condition = condition + totalt + " >= " + count;
                                    }
                                    else
                                    {
                                        condition = condition + totalt + " >= " + count + " and ";
                                    }
                                    k++;
                                }
                                System.Data.DataTable table = new System.Data.DataTable();
                                table.Columns.Add("", typeof(Boolean));
                                table.Columns[0].Expression = condition;
                                System.Data.DataRow ckr = table.NewRow();
                                table.Rows.Add(ckr);
                                bool _condition = (Boolean)ckr[0];
                                string r1isavail = "false";
                                int noset = 0;
                                if (_condition)
                                {
                                    roomssss[0].risavail = "true";
                                    roomssss[1].risavail = "true";
                                }
                                else
                                {
                                    #region On Request
                                    int _totalt = 0;
                                    List<roomsss> new_roomssss = new List<roomsss>();
                                    foreach (var item in roomssss)
                                    {
                                        var r = item.rid;
                                        var items = grouped.Where(a => a.Key == r).FirstOrDefault();
                                        _totalt = items.allocation;
                                        if (_totalt <= 0)
                                        {
                                            string or = items.OR; ;
                                            if (or == "true")
                                                r1isavail = "false";
                                            else
                                                noset = 1;
                                        }
                                        else
                                        {
                                            items.allocation--;
                                            r1isavail = "true";
                                        }
                                        item.risavail = r1isavail;
                                    }
                                    #endregion
                                }
                                if (noset == 0)
                                {
                                    //if (avail1 == avail2)
                                    {
                                        List<XElement> pricebrkups1 = roomList1[m].Descendants("dailyRate").ToList();
                                        List<XElement> pricebrkups2 = roomList2[n].Descendants("dailyRate").ToList();
                                        List<XElement> promotions1 = roomList1[m].Descendants("offer").ToList();
                                        List<XElement> promotions2 = roomList2[n].Descendants("offer").ToList();
                                        #region Board Bases >0
                                        group++;
                                        decimal totalrate = Convert.ToDecimal(roomList1[m].Attribute("net").Value) + Convert.ToDecimal(roomList2[n].Attribute("net").Value);
                                        str.Add(new XElement("RoomTypes", new XAttribute("TotalRate", totalrate), new XAttribute("Index", group),
                                        new XElement("Room",
                                         new XAttribute("ID", Convert.ToString(roomList1[m].Parent.Parent.Attribute("code").Value)),
                                         new XAttribute("SuppliersID", suppID),
                                         new XAttribute("RoomSeq", "1"),
                                         new XAttribute("SessionID", Convert.ToString(roomList1[m].Attribute("rateKey").Value)),
                                         new XAttribute("RoomType", Convert.ToString(roomList1[m].Parent.Parent.Attribute("name").Value)),
                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                         new XAttribute("MealPlanID", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                         new XAttribute("MealPlanName", Convert.ToString(roomList1[m].Attribute("boardName").Value)),
                                         new XAttribute("MealPlanCode", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                         new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList1[m].Attribute("net").Value)),
                                         new XAttribute("CancellationDate", ""),
                                         new XAttribute("CancellationAmount", ""),
                                         new XAttribute("sourcekey", roomList1[m].Attribute("sourcekey").Value),
                                          new XAttribute("isAvailable", roomssss[0].risavail.ToString()),
                                         new XElement("RequestID", Convert.ToString("")),
                                         new XElement("Offers", ""),
                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions1)),
                                         new XElement("CancellationPolicy", ""),
                                         new XElement("Amenities", new XElement("Amenity", "")),
                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                         new XElement("Supplements", Getsupplementsextranet(roomList1[m].Descendants("supplement").ToList())
                                             ),
                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups1)),
                                             new XElement("AdultNum", Convert.ToString(ttlroom[0].Descendants("Adult").FirstOrDefault().Value)),
                                             new XElement("ChildNum", Convert.ToString(ttlroom[0].Descendants("Child").FirstOrDefault().Value))
                                         ),
                                        new XElement("Room",
                                         new XAttribute("ID", Convert.ToString(roomList2[n].Parent.Parent.Attribute("code").Value)),
                                         new XAttribute("SuppliersID", suppID),
                                         new XAttribute("RoomSeq", "2"),
                                         new XAttribute("SessionID", Convert.ToString(roomList2[n].Attribute("rateKey").Value)),
                                         new XAttribute("RoomType", Convert.ToString(roomList2[n].Parent.Parent.Attribute("name").Value)),
                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                         new XAttribute("MealPlanID", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                         new XAttribute("MealPlanName", Convert.ToString(roomList2[n].Attribute("boardName").Value)),
                                         new XAttribute("MealPlanCode", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                         new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList2[n].Attribute("net").Value)),
                                         new XAttribute("CancellationDate", ""),
                                         new XAttribute("CancellationAmount", ""),
                                         new XAttribute("sourcekey", roomList2[n].Attribute("sourcekey").Value),
                                          new XAttribute("isAvailable", roomssss[1].risavail.ToString()),
                                         new XElement("RequestID", Convert.ToString("")),
                                         new XElement("Offers", ""),
                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions2)),
                                         new XElement("CancellationPolicy", ""),
                                         new XElement("Amenities", new XElement("Amenity", "")),
                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                         new XElement("Supplements", Getsupplementsextranet(roomList2[n].Descendants("supplement").ToList())
                                             ),
                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups2)),
                                             new XElement("AdultNum", Convert.ToString(ttlroom[1].Descendants("Adult").FirstOrDefault().Value)),
                                             new XElement("ChildNum", Convert.ToString(ttlroom[1].Descendants("Child").FirstOrDefault().Value))
                                         ),
                                                 new XElement("CancellationPolicies",
                                             GetRoomCancellationPolicyExtranet(cxllist))
                                             ));
                                        #endregion
                                    }
                                }
                            }
                            catch { }
                        }
                    };
                };
                return str;
            }
            #endregion
            #region Room Count 3
            if (totalroom == 3)
            {
                List<roomsss> roomssss = new List<roomsss>() { new roomsss() { rno = 1, risavail = "false" }, new roomsss() { rno = 2, risavail = "false" }, new roomsss() { rno = 3, risavail = "false" } };
                #region Get Combination (Room 1)
                roomList1 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "1").ToList();
                #endregion
                #region Get Combination (Room 2)
                roomList2 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "2").ToList();
                #endregion
                #region Get Combination (Room 3)
                roomList3 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "3").ToList();
                #endregion
                int group = 0;
                #region Room 3
                for (int m = 0; m < roomList1.Count(); m++)
                {
                    for (int n = 0; n < roomList2.Count(); n++)
                    {
                        for (int o = 0; o < roomList3.Count(); o++)
                        {
                            string bb1 = roomList1[m].Attribute("boardCode").Value;
                            string bb2 = roomList2[n].Attribute("boardCode").Value;
                            string bb3 = roomList3[o].Attribute("boardCode").Value;
                            if (bb1 == bb2 && bb2 == bb3 && bb1 == bb3)
                            {
                                try
                                {
                                    String condition = "";
                                    List<string> ratekeylist = new List<string>();
                                    ratekeylist.Add(roomList1[m].Parent.Parent.Attribute("code").Value);
                                    ratekeylist.Add(roomList2[n].Parent.Parent.Attribute("code").Value);
                                    ratekeylist.Add(roomList3[o].Parent.Parent.Attribute("code").Value);
                                    int iii = 0;
                                    foreach (var r in ratekeylist)
                                    {
                                        roomssss[iii].rid = r;
                                        iii++;
                                    }
                                    var grouped = ratekeylist.GroupBy(ss => ss).Select(ax => new roomgroup
                                    {
                                        Key = ax.Key,
                                        Count = ax.Count(),
                                        allocation = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value)),
                                        OR = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("onRequest").FirstOrDefault().Value

                                    }).ToList();
                                    int k = 0;
                                    foreach (var item in grouped)
                                    {
                                        var rtkey = item.Key;
                                        var count = item.Count;
                                        int totalt = roomlist.Where(x => x.Attribute("code").Value == rtkey).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value));
                                        item.allocation = totalt;
                                        if (k == grouped.Count() - 1)
                                        {
                                            condition = condition + totalt + " >= " + count;
                                        }
                                        else
                                        {
                                            condition = condition + totalt + " >= " + count + " and ";
                                        }
                                        k++;
                                    }
                                    System.Data.DataTable table = new System.Data.DataTable();
                                    table.Columns.Add("", typeof(Boolean));
                                    table.Columns[0].Expression = condition;
                                    System.Data.DataRow ckr = table.NewRow();
                                    table.Rows.Add(ckr);
                                    bool _condition = (Boolean)ckr[0];
                                    string r1isavail = "false";
                                    int noset = 0;
                                    if (_condition)
                                    {
                                        roomssss[0].risavail = "true";
                                        roomssss[1].risavail = "true";
                                        roomssss[2].risavail = "true";
                                    }
                                    else
                                    {
                                        #region On Request
                                        int _totalt = 0;
                                        List<roomsss> new_roomssss = new List<roomsss>();
                                        foreach (var item in roomssss)
                                        {
                                            var r = item.rid;
                                            var items = grouped.Where(a => a.Key == r).FirstOrDefault();
                                            _totalt = items.allocation;
                                            if (_totalt <= 0)
                                            {
                                                string or = items.OR; ;
                                                if (or == "true")
                                                    r1isavail = "false";
                                                else
                                                    noset = 1;
                                            }
                                            else
                                            {
                                                items.allocation--;
                                                r1isavail = "true";
                                            }
                                            item.risavail = r1isavail;
                                        }
                                        #endregion
                                    }
                                    if (noset == 0)
                                    {
                                        //if (avail1 == avail2 && avail2 == avail3 && avail1 == avail3)
                                        {
                                            #region room's group
                                            List<XElement> pricebrkups1 = roomList1[m].Descendants("dailyRate").ToList();
                                            List<XElement> pricebrkups2 = roomList2[n].Descendants("dailyRate").ToList();
                                            List<XElement> pricebrkups3 = roomList3[o].Descendants("dailyRate").ToList();
                                            List<XElement> promotions1 = roomList1[m].Descendants("offer").ToList();
                                            List<XElement> promotions2 = roomList2[n].Descendants("offer").ToList();
                                            List<XElement> promotions3 = roomList3[o].Descendants("offer").ToList();
                                            group++;
                                            decimal totalrate = Convert.ToDecimal(roomList1[m].Attribute("net").Value) + Convert.ToDecimal(roomList2[n].Attribute("net").Value) + Convert.ToDecimal(roomList3[o].Attribute("net").Value);

                                            str.Add(new XElement("RoomTypes", new XAttribute("TotalRate", totalrate), new XAttribute("Index", group),

                                            new XElement("Room",
                                             new XAttribute("ID", Convert.ToString(roomList1[m].Parent.Parent.Attribute("code").Value)),
                                             new XAttribute("SuppliersID", suppID),
                                             new XAttribute("RoomSeq", "1"),
                                             new XAttribute("SessionID", Convert.ToString(roomList1[m].Attribute("rateKey").Value)),
                                             new XAttribute("RoomType", Convert.ToString(roomList1[m].Parent.Parent.Attribute("name").Value)),
                                             new XAttribute("OccupancyID", Convert.ToString("")),
                                             new XAttribute("OccupancyName", Convert.ToString("")),
                                             new XAttribute("MealPlanID", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                             new XAttribute("MealPlanName", Convert.ToString(roomList1[m].Attribute("boardName").Value)),
                                             new XAttribute("MealPlanCode", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                             new XAttribute("MealPlanPrice", Convert.ToString("")),
                                             new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                             new XAttribute("TotalRoomRate", Convert.ToString(roomList1[m].Attribute("net").Value)),
                                             new XAttribute("CancellationDate", ""),
                                             new XAttribute("CancellationAmount", ""),
                                             new XAttribute("sourcekey", roomList1[m].Attribute("sourcekey").Value),
                                              new XAttribute("isAvailable", roomssss[0].risavail.ToString()),
                                             new XElement("RequestID", Convert.ToString("")),
                                             new XElement("Offers", ""),
                                              new XElement("PromotionList", GetHotelpromotionsExtranet(promotions1)),
                                             new XElement("CancellationPolicy", ""),
                                             new XElement("Amenities", new XElement("Amenity", "")),
                                             new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                             new XElement("Supplements", Getsupplementsextranet(roomList1[m].Descendants("supplement").ToList())
                                                 ),
                                                 new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups1)),
                                                 new XElement("AdultNum", Convert.ToString(ttlroom[0].Descendants("Adult").FirstOrDefault().Value)),
                                                 new XElement("ChildNum", Convert.ToString(ttlroom[0].Descendants("Child").FirstOrDefault().Value))
                                             ),

                                            new XElement("Room",
                                             new XAttribute("ID", Convert.ToString(roomList2[n].Parent.Parent.Attribute("code").Value)),
                                             new XAttribute("SuppliersID", suppID),
                                             new XAttribute("RoomSeq", "2"),
                                             new XAttribute("SessionID", Convert.ToString(roomList2[n].Attribute("rateKey").Value)),
                                             new XAttribute("RoomType", Convert.ToString(roomList2[n].Parent.Parent.Attribute("name").Value)),
                                             new XAttribute("OccupancyID", Convert.ToString("")),
                                             new XAttribute("OccupancyName", Convert.ToString("")),
                                             new XAttribute("MealPlanID", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                             new XAttribute("MealPlanName", Convert.ToString(roomList2[n].Attribute("boardName").Value)),
                                             new XAttribute("MealPlanCode", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                             new XAttribute("MealPlanPrice", Convert.ToString("")),
                                             new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                             new XAttribute("TotalRoomRate", Convert.ToString(roomList2[n].Attribute("net").Value)),
                                             new XAttribute("CancellationDate", ""),
                                             new XAttribute("CancellationAmount", ""),
                                             new XAttribute("sourcekey", roomList2[n].Attribute("sourcekey").Value),
                                              new XAttribute("isAvailable", roomssss[1].risavail.ToString()),
                                             new XElement("RequestID", Convert.ToString("")),
                                             new XElement("Offers", ""),
                                              new XElement("PromotionList", GetHotelpromotionsExtranet(promotions2)),
                                             new XElement("CancellationPolicy", ""),
                                             new XElement("Amenities", new XElement("Amenity", "")),
                                             new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                             new XElement("Supplements", Getsupplementsextranet(roomList2[n].Descendants("supplement").ToList())
                                                 ),
                                                 new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups2)),
                                                 new XElement("AdultNum", Convert.ToString(ttlroom[1].Descendants("Adult").FirstOrDefault().Value)),
                                                 new XElement("ChildNum", Convert.ToString(ttlroom[1].Descendants("Child").FirstOrDefault().Value))
                                             ),

                                            new XElement("Room",
                                             new XAttribute("ID", Convert.ToString(roomList3[o].Parent.Parent.Attribute("code").Value)),
                                             new XAttribute("SuppliersID", suppID),
                                             new XAttribute("RoomSeq", "3"),
                                             new XAttribute("SessionID", Convert.ToString(roomList3[o].Attribute("rateKey").Value)),
                                             new XAttribute("RoomType", Convert.ToString(roomList3[o].Parent.Parent.Attribute("name").Value)),
                                             new XAttribute("OccupancyID", Convert.ToString("")),
                                             new XAttribute("OccupancyName", Convert.ToString("")),
                                             new XAttribute("MealPlanID", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                             new XAttribute("MealPlanName", Convert.ToString(roomList3[o].Attribute("boardName").Value)),
                                             new XAttribute("MealPlanCode", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                             new XAttribute("MealPlanPrice", Convert.ToString("")),
                                             new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                             new XAttribute("TotalRoomRate", Convert.ToString(roomList3[o].Attribute("net").Value)),
                                             new XAttribute("CancellationDate", ""),
                                             new XAttribute("CancellationAmount", ""),
                                             new XAttribute("sourcekey", roomList3[o].Attribute("sourcekey").Value),
                                              new XAttribute("isAvailable", roomssss[2].risavail.ToString()),
                                             new XElement("RequestID", Convert.ToString("")),
                                             new XElement("Offers", ""),
                                              new XElement("PromotionList", GetHotelpromotionsExtranet(promotions3)),
                                             new XElement("CancellationPolicy", ""),
                                             new XElement("Amenities", new XElement("Amenity", "")),
                                             new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                             new XElement("Supplements", Getsupplementsextranet(roomList3[o].Descendants("supplement").ToList())
                                                 ),
                                                 new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups3)),
                                                 new XElement("AdultNum", Convert.ToString(ttlroom[2].Descendants("Adult").FirstOrDefault().Value)),
                                                 new XElement("ChildNum", Convert.ToString(ttlroom[2].Descendants("Child").FirstOrDefault().Value))
                                             ),
                                                 new XElement("CancellationPolicies",
                                             GetRoomCancellationPolicyExtranet(cxllist))
                                             ));
                                            #endregion
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                return str;
                #endregion
            }
            #endregion
            #region Room Count 4
            if (totalroom == 4)
            {
                List<roomsss> roomssss = new List<roomsss>() { new roomsss() { rno = 1, risavail = "false" }, new roomsss() { rno = 2, risavail = "false" }, new roomsss() { rno = 3, risavail = "false" }, new roomsss() { rno = 4, risavail = "false" } };
                #region Get Combination (Room 1)
                roomList1 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "1").ToList();
                #endregion
                #region Get Combination (Room 2)
                roomList2 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "2").ToList();
                #endregion
                #region Get Combination (Room 3)
                roomList3 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "3").ToList();
                #endregion
                #region Get Combination (Room 4)
                roomList4 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "4").ToList();
                #endregion
                int group = 0;
                #region Room 4
                for (int m = 0; m < roomList1.Count(); m++)
                {
                    for (int n = 0; n < roomList2.Count(); n++)
                    {
                        for (int o = 0; o < roomList3.Count(); o++)
                        {
                            for (int p = 0; p < roomList4.Count(); p++)
                            {
                                // add room 1, 2, 3,4

                                string bb1 = roomList1[m].Attribute("boardCode").Value;
                                string bb2 = roomList2[n].Attribute("boardCode").Value;
                                string bb3 = roomList3[o].Attribute("boardCode").Value;
                                string bb4 = roomList4[p].Attribute("boardCode").Value;
                                if (bb1 == bb2 && bb2 == bb3 && bb1 == bb3 && bb1 == bb4 && bb2 == bb4 && bb3 == bb4)
                                {
                                    try
                                    {
                                        String condition = "";
                                        List<string> ratekeylist = new List<string>();
                                        ratekeylist.Add(roomList1[m].Parent.Parent.Attribute("code").Value);
                                        ratekeylist.Add(roomList2[n].Parent.Parent.Attribute("code").Value);
                                        ratekeylist.Add(roomList3[o].Parent.Parent.Attribute("code").Value);
                                        ratekeylist.Add(roomList4[p].Parent.Parent.Attribute("code").Value);
                                        int iii = 0;
                                        foreach (var r in ratekeylist)
                                        {
                                            roomssss[iii].rid = r;
                                            iii++;
                                        }
                                        var grouped = ratekeylist.GroupBy(ss => ss).Select(ax => new roomgroup
                                        {
                                            Key = ax.Key,
                                            Count = ax.Count(),
                                            allocation = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value)),
                                            OR = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("onRequest").FirstOrDefault().Value

                                        }).ToList();
                                        int k = 0;
                                        foreach (var item in grouped)
                                        {
                                            var rtkey = item.Key;
                                            var count = item.Count;
                                            int totalt = roomlist.Where(x => x.Attribute("code").Value == rtkey).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value));
                                            item.allocation = totalt;
                                            if (k == grouped.Count() - 1)
                                            {
                                                condition = condition + totalt + " >= " + count;
                                            }
                                            else
                                            {
                                                condition = condition + totalt + " >= " + count + " and ";
                                            }
                                            k++;
                                        }
                                        System.Data.DataTable table = new System.Data.DataTable();
                                        table.Columns.Add("", typeof(Boolean));
                                        table.Columns[0].Expression = condition;
                                        System.Data.DataRow ckr = table.NewRow();
                                        table.Rows.Add(ckr);
                                        bool _condition = (Boolean)ckr[0];
                                        string r1isavail = "false";
                                        int noset = 0;
                                        if (_condition)
                                        {
                                            roomssss[0].risavail = "true";
                                            roomssss[1].risavail = "true";
                                            roomssss[2].risavail = "true";
                                            roomssss[3].risavail = "true";
                                        }
                                        else
                                        {
                                            #region On Request
                                            int _totalt = 0;
                                            List<roomsss> new_roomssss = new List<roomsss>();
                                            foreach (var item in roomssss)
                                            {
                                                var r = item.rid;
                                                var items = grouped.Where(a => a.Key == r).FirstOrDefault();
                                                _totalt = items.allocation;
                                                if (_totalt <= 0)
                                                {
                                                    string or = items.OR; ;
                                                    if (or == "true")
                                                        r1isavail = "false";
                                                    else
                                                        noset = 1;
                                                }
                                                else
                                                {
                                                    items.allocation--;
                                                    r1isavail = "true";
                                                }
                                                item.risavail = r1isavail;
                                            }
                                            #endregion
                                        }
                                        if (noset == 0)
                                        {
                                            //if (avail1 == avail2 && avail2 == avail3 && avail1 == avail3 && avail1 == avail4 && avail2 == avail4 && avail3 == avail4)
                                            {
                                                #region room's group
                                                List<XElement> pricebrkups1 = roomList1[m].Descendants("dailyRate").ToList();
                                                List<XElement> pricebrkups2 = roomList2[n].Descendants("dailyRate").ToList();
                                                List<XElement> pricebrkups3 = roomList3[o].Descendants("dailyRate").ToList();
                                                List<XElement> pricebrkups4 = roomList4[p].Descendants("dailyRate").ToList();
                                                List<XElement> promotions1 = roomList1[m].Descendants("offer").ToList();
                                                List<XElement> promotions2 = roomList2[n].Descendants("offer").ToList();
                                                List<XElement> promotions3 = roomList3[o].Descendants("offer").ToList();
                                                List<XElement> promotions4 = roomList4[p].Descendants("offer").ToList();
                                                group++;
                                                decimal totalrate = Convert.ToDecimal(roomList1[m].Attribute("net").Value) + Convert.ToDecimal(roomList2[n].Attribute("net").Value) + Convert.ToDecimal(roomList3[o].Attribute("net").Value) + Convert.ToDecimal(roomList4[p].Attribute("net").Value);

                                                str.Add(new XElement("RoomTypes", new XAttribute("TotalRate", totalrate), new XAttribute("Index", group),

                                                new XElement("Room",
                                                 new XAttribute("ID", Convert.ToString(roomList1[m].Parent.Parent.Attribute("code").Value)),
                                                 new XAttribute("SuppliersID", suppID),
                                                 new XAttribute("RoomSeq", "1"),
                                                 new XAttribute("SessionID", Convert.ToString(roomList1[m].Attribute("rateKey").Value)),
                                                 new XAttribute("RoomType", Convert.ToString(roomList1[m].Parent.Parent.Attribute("name").Value)),
                                                 new XAttribute("OccupancyID", Convert.ToString("")),
                                                 new XAttribute("OccupancyName", Convert.ToString("")),
                                                 new XAttribute("MealPlanID", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanName", Convert.ToString(roomList1[m].Attribute("boardName").Value)),
                                                 new XAttribute("MealPlanCode", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                 new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                 new XAttribute("TotalRoomRate", Convert.ToString(roomList1[m].Attribute("net").Value)),
                                                 new XAttribute("CancellationDate", ""),
                                                 new XAttribute("CancellationAmount", ""),
                                                 new XAttribute("sourcekey", roomList1[m].Attribute("sourcekey").Value),
                                                  new XAttribute("isAvailable", roomssss[0].risavail.ToString()),
                                                 new XElement("RequestID", Convert.ToString("")),
                                                 new XElement("Offers", ""),
                                                  new XElement("PromotionList", GetHotelpromotionsExtranet(promotions1)),
                                                 new XElement("CancellationPolicy", ""),
                                                 new XElement("Amenities", new XElement("Amenity", "")),
                                                 new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                 new XElement("Supplements", Getsupplementsextranet(roomList1[m].Descendants("supplement").ToList())
                                                     ),
                                                     new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups1)),
                                                     new XElement("AdultNum", Convert.ToString(ttlroom[0].Descendants("Adult").FirstOrDefault().Value)),
                                                     new XElement("ChildNum", Convert.ToString(ttlroom[0].Descendants("Child").FirstOrDefault().Value))
                                                 ),

                                                new XElement("Room",
                                                 new XAttribute("ID", Convert.ToString(roomList2[n].Parent.Parent.Attribute("code").Value)),
                                                 new XAttribute("SuppliersID", suppID),
                                                 new XAttribute("RoomSeq", "2"),
                                                 new XAttribute("SessionID", Convert.ToString(roomList2[n].Attribute("rateKey").Value)),
                                                 new XAttribute("RoomType", Convert.ToString(roomList2[n].Parent.Parent.Attribute("name").Value)),
                                                 new XAttribute("OccupancyID", Convert.ToString("")),
                                                 new XAttribute("OccupancyName", Convert.ToString("")),
                                                 new XAttribute("MealPlanID", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanName", Convert.ToString(roomList2[n].Attribute("boardName").Value)),
                                                 new XAttribute("MealPlanCode", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                 new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                 new XAttribute("TotalRoomRate", Convert.ToString(roomList2[n].Attribute("net").Value)),
                                                 new XAttribute("CancellationDate", ""),
                                                 new XAttribute("CancellationAmount", ""),
                                                 new XAttribute("sourcekey", roomList2[n].Attribute("sourcekey").Value),
                                                  new XAttribute("isAvailable", roomssss[1].risavail.ToString()),
                                                 new XElement("RequestID", Convert.ToString("")),
                                                 new XElement("Offers", ""),
                                                  new XElement("PromotionList", GetHotelpromotionsExtranet(promotions2)),
                                                 new XElement("CancellationPolicy", ""),
                                                 new XElement("Amenities", new XElement("Amenity", "")),
                                                 new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                 new XElement("Supplements", Getsupplementsextranet(roomList2[n].Descendants("supplement").ToList())
                                                     ),
                                                     new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups2)),
                                                     new XElement("AdultNum", Convert.ToString(ttlroom[1].Descendants("Adult").FirstOrDefault().Value)),
                                                     new XElement("ChildNum", Convert.ToString(ttlroom[1].Descendants("Child").FirstOrDefault().Value))
                                                 ),

                                                new XElement("Room",
                                                 new XAttribute("ID", Convert.ToString(roomList3[o].Parent.Parent.Attribute("code").Value)),
                                                 new XAttribute("SuppliersID", suppID),
                                                 new XAttribute("RoomSeq", "3"),
                                                 new XAttribute("SessionID", Convert.ToString(roomList3[o].Attribute("rateKey").Value)),
                                                 new XAttribute("RoomType", Convert.ToString(roomList3[o].Parent.Parent.Attribute("name").Value)),
                                                 new XAttribute("OccupancyID", Convert.ToString("")),
                                                 new XAttribute("OccupancyName", Convert.ToString("")),
                                                 new XAttribute("MealPlanID", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanName", Convert.ToString(roomList3[o].Attribute("boardName").Value)),
                                                 new XAttribute("MealPlanCode", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                 new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                 new XAttribute("TotalRoomRate", Convert.ToString(roomList3[o].Attribute("net").Value)),
                                                 new XAttribute("CancellationDate", ""),
                                                 new XAttribute("CancellationAmount", ""),
                                                 new XAttribute("sourcekey", roomList3[o].Attribute("sourcekey").Value),
                                                  new XAttribute("isAvailable", roomssss[2].risavail.ToString()),
                                                 new XElement("RequestID", Convert.ToString("")),
                                                 new XElement("Offers", ""),
                                                  new XElement("PromotionList", GetHotelpromotionsExtranet(promotions3)),
                                                 new XElement("CancellationPolicy", ""),
                                                 new XElement("Amenities", new XElement("Amenity", "")),
                                                 new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                 new XElement("Supplements", Getsupplementsextranet(roomList3[o].Descendants("supplement").ToList())
                                                     ),
                                                     new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups3)),
                                                     new XElement("AdultNum", Convert.ToString(ttlroom[2].Descendants("Adult").FirstOrDefault().Value)),
                                                     new XElement("ChildNum", Convert.ToString(ttlroom[2].Descendants("Child").FirstOrDefault().Value))
                                                 ),

                                                new XElement("Room",
                                                 new XAttribute("ID", Convert.ToString(roomList4[p].Parent.Parent.Attribute("code").Value)),
                                                 new XAttribute("SuppliersID", suppID),
                                                 new XAttribute("RoomSeq", "4"),
                                                 new XAttribute("SessionID", Convert.ToString(roomList4[p].Attribute("rateKey").Value)),
                                                 new XAttribute("RoomType", Convert.ToString(roomList4[p].Parent.Parent.Attribute("name").Value)),
                                                 new XAttribute("OccupancyID", Convert.ToString("")),
                                                 new XAttribute("OccupancyName", Convert.ToString("")),
                                                 new XAttribute("MealPlanID", Convert.ToString(roomList4[p].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanName", Convert.ToString(roomList4[p].Attribute("boardName").Value)),
                                                 new XAttribute("MealPlanCode", Convert.ToString(roomList4[p].Attribute("boardCode").Value)),
                                                 new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                 new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                 new XAttribute("TotalRoomRate", Convert.ToString(roomList4[p].Attribute("net").Value)),
                                                 new XAttribute("CancellationDate", ""),
                                                 new XAttribute("CancellationAmount", ""),
                                                 new XAttribute("sourcekey", roomList4[p].Attribute("sourcekey").Value),
                                                  new XAttribute("isAvailable", roomssss[3].risavail.ToString()),
                                                 new XElement("RequestID", Convert.ToString("")),
                                                 new XElement("Offers", ""),
                                                  new XElement("PromotionList", GetHotelpromotionsExtranet(promotions4)),
                                                 new XElement("CancellationPolicy", ""),
                                                 new XElement("Amenities", new XElement("Amenity", "")),
                                                 new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                 new XElement("Supplements", Getsupplementsextranet(roomList4[p].Descendants("supplement").ToList())
                                                     ),
                                                     new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups4)),
                                                     new XElement("AdultNum", Convert.ToString(ttlroom[3].Descendants("Adult").FirstOrDefault().Value)),
                                                     new XElement("ChildNum", Convert.ToString(ttlroom[3].Descendants("Child").FirstOrDefault().Value))
                                                 ),
                                                     new XElement("CancellationPolicies",
                                                 GetRoomCancellationPolicyExtranet(cxllist))
                                                 ));
                                                #endregion
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                return str;
                #endregion
            }
            #endregion
            #region Room Count 5
            if (totalroom == 5)
            {
                List<roomsss> roomssss = new List<roomsss>() { new roomsss() { rno = 1, risavail = "false" }, new roomsss() { rno = 2, risavail = "false" }, new roomsss() { rno = 3, risavail = "false" }, new roomsss() { rno = 4, risavail = "false" }, new roomsss() { rno = 5, risavail = "false" } };
                #region Get Combination (Room 1)
                roomList1 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "1").ToList();
                #endregion
                #region Get Combination (Room 2)
                roomList2 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "2").ToList();
                #endregion
                #region Get Combination (Room 3)
                roomList3 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "3").ToList();
                #endregion
                #region Get Combination (Room 4)
                roomList4 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "4").ToList();
                #endregion
                #region Get Combination (Room 5)
                roomList5 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "5").ToList();
                #endregion
                int group = 0;
                #region Room 5
                for (int m = 0; m < roomList1.Count(); m++)
                {
                    for (int n = 0; n < roomList2.Count(); n++)
                    {
                        for (int o = 0; o < roomList3.Count(); o++)
                        {
                            for (int p = 0; p < roomList4.Count(); p++)
                            {
                                for (int q = 0; q < roomList5.Count(); q++)
                                {
                                    // add room 1, 2, 3, 4, 5

                                    string bb1 = roomList1[m].Attribute("boardCode").Value;
                                    string bb2 = roomList2[n].Attribute("boardCode").Value;
                                    string bb3 = roomList3[o].Attribute("boardCode").Value;
                                    string bb4 = roomList4[p].Attribute("boardCode").Value;
                                    string bb5 = roomList5[q].Attribute("boardCode").Value;
                                    if (bb1 == bb2 && bb2 == bb3 && bb1 == bb3 && bb1 == bb4 && bb2 == bb4 && bb3 == bb4
                                        && bb1 == bb5 && bb2 == bb5 && bb3 == bb5 && bb4 == bb5)
                                    {
                                        try
                                        {
                                            String condition = "";
                                            List<string> ratekeylist = new List<string>();
                                            ratekeylist.Add(roomList1[m].Parent.Parent.Attribute("code").Value);
                                            ratekeylist.Add(roomList2[n].Parent.Parent.Attribute("code").Value);
                                            ratekeylist.Add(roomList3[o].Parent.Parent.Attribute("code").Value);
                                            ratekeylist.Add(roomList4[p].Parent.Parent.Attribute("code").Value);
                                            ratekeylist.Add(roomList5[q].Parent.Parent.Attribute("code").Value);
                                            int iii = 0;
                                            foreach (var r in ratekeylist)
                                            {
                                                roomssss[iii].rid = r;
                                                iii++;
                                            }
                                            var grouped = ratekeylist.GroupBy(ss => ss).Select(ax => new roomgroup
                                            {
                                                Key = ax.Key,
                                                Count = ax.Count(),
                                                allocation = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value)),
                                                OR = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("onRequest").FirstOrDefault().Value

                                            }).ToList();
                                            int k = 0;
                                            foreach (var item in grouped)
                                            {
                                                var rtkey = item.Key;
                                                var count = item.Count;
                                                int totalt = roomlist.Where(x => x.Attribute("code").Value == rtkey).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value));
                                                item.allocation = totalt;
                                                if (k == grouped.Count() - 1)
                                                {
                                                    condition = condition + totalt + " >= " + count;
                                                }
                                                else
                                                {
                                                    condition = condition + totalt + " >= " + count + " and ";
                                                }
                                                k++;
                                            }
                                            System.Data.DataTable table = new System.Data.DataTable();
                                            table.Columns.Add("", typeof(Boolean));
                                            table.Columns[0].Expression = condition;
                                            System.Data.DataRow ckr = table.NewRow();
                                            table.Rows.Add(ckr);
                                            bool _condition = (Boolean)ckr[0];
                                            string r1isavail = "false";
                                            int noset = 0;
                                            if (_condition)
                                            {
                                                roomssss[0].risavail = "true";
                                                roomssss[1].risavail = "true";
                                                roomssss[2].risavail = "true";
                                                roomssss[3].risavail = "true";
                                                roomssss[4].risavail = "true";
                                            }
                                            else
                                            {
                                                #region On Request
                                                int _totalt = 0;
                                                List<roomsss> new_roomssss = new List<roomsss>();
                                                foreach (var item in roomssss)
                                                {
                                                    var r = item.rid;
                                                    var items = grouped.Where(a => a.Key == r).FirstOrDefault();
                                                    _totalt = items.allocation;
                                                    if (_totalt <= 0)
                                                    {
                                                        string or = items.OR; ;
                                                        if (or == "true")
                                                            r1isavail = "false";
                                                        else
                                                            noset = 1;
                                                    }
                                                    else
                                                    {
                                                        items.allocation--;
                                                        r1isavail = "true";
                                                    }
                                                    item.risavail = r1isavail;
                                                }
                                                #endregion
                                            }
                                            if (noset == 0)
                                            {
                                                //if (avail1 == avail2 && avail2 == avail3 && avail1 == avail3 && avail1 == avail4 && avail2 == avail4 && avail3 == avail4)
                                                {
                                                    #region room's group
                                                    List<XElement> pricebrkups1 = roomList1[m].Descendants("dailyRate").ToList();
                                                    List<XElement> pricebrkups2 = roomList2[n].Descendants("dailyRate").ToList();
                                                    List<XElement> pricebrkups3 = roomList3[o].Descendants("dailyRate").ToList();
                                                    List<XElement> pricebrkups4 = roomList4[p].Descendants("dailyRate").ToList();
                                                    List<XElement> pricebrkups5 = roomList5[q].Descendants("dailyRate").ToList();
                                                    List<XElement> promotions1 = roomList1[m].Descendants("offer").ToList();
                                                    List<XElement> promotions2 = roomList2[n].Descendants("offer").ToList();
                                                    List<XElement> promotions3 = roomList3[o].Descendants("offer").ToList();
                                                    List<XElement> promotions4 = roomList4[p].Descendants("offer").ToList();
                                                    List<XElement> promotions5 = roomList5[q].Descendants("offer").ToList();
                                                    group++;
                                                    decimal totalrate = Convert.ToDecimal(roomList1[m].Attribute("net").Value) + Convert.ToDecimal(roomList2[n].Attribute("net").Value) + Convert.ToDecimal(roomList3[o].Attribute("net").Value) + Convert.ToDecimal(roomList4[p].Attribute("net").Value) + Convert.ToDecimal(roomList5[q].Attribute("net").Value);

                                                    str.Add(new XElement("RoomTypes", new XAttribute("TotalRate", totalrate), new XAttribute("Index", group),

                                                    new XElement("Room",
                                                     new XAttribute("ID", Convert.ToString(roomList1[m].Parent.Parent.Attribute("code").Value)),
                                                     new XAttribute("SuppliersID", suppID),
                                                     new XAttribute("RoomSeq", "1"),
                                                     new XAttribute("SessionID", Convert.ToString(roomList1[m].Attribute("rateKey").Value)),
                                                     new XAttribute("RoomType", Convert.ToString(roomList1[m].Parent.Parent.Attribute("name").Value)),
                                                     new XAttribute("OccupancyID", Convert.ToString("")),
                                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                                     new XAttribute("MealPlanID", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanName", Convert.ToString(roomList1[m].Attribute("boardName").Value)),
                                                     new XAttribute("MealPlanCode", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                     new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                     new XAttribute("TotalRoomRate", Convert.ToString(roomList1[m].Attribute("net").Value)),
                                                     new XAttribute("CancellationDate", ""),
                                                     new XAttribute("CancellationAmount", ""),
                                                     new XAttribute("sourcekey", roomList1[m].Attribute("sourcekey").Value),
                                                      new XAttribute("isAvailable", roomssss[0].risavail.ToString()),
                                                     new XElement("RequestID", Convert.ToString("")),
                                                     new XElement("Offers", ""),
                                                      new XElement("PromotionList", GetHotelpromotionsExtranet(promotions1)),
                                                     new XElement("CancellationPolicy", ""),
                                                     new XElement("Amenities", new XElement("Amenity", "")),
                                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                     new XElement("Supplements", Getsupplementsextranet(roomList1[m].Descendants("supplement").ToList())
                                                         ),
                                                         new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups1)),
                                                         new XElement("AdultNum", Convert.ToString(ttlroom[0].Descendants("Adult").FirstOrDefault().Value)),
                                                         new XElement("ChildNum", Convert.ToString(ttlroom[0].Descendants("Child").FirstOrDefault().Value))
                                                     ),

                                                    new XElement("Room",
                                                     new XAttribute("ID", Convert.ToString(roomList2[n].Parent.Parent.Attribute("code").Value)),
                                                     new XAttribute("SuppliersID", suppID),
                                                     new XAttribute("RoomSeq", "2"),
                                                     new XAttribute("SessionID", Convert.ToString(roomList2[n].Attribute("rateKey").Value)),
                                                     new XAttribute("RoomType", Convert.ToString(roomList2[n].Parent.Parent.Attribute("name").Value)),
                                                     new XAttribute("OccupancyID", Convert.ToString("")),
                                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                                     new XAttribute("MealPlanID", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanName", Convert.ToString(roomList2[n].Attribute("boardName").Value)),
                                                     new XAttribute("MealPlanCode", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                     new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                     new XAttribute("TotalRoomRate", Convert.ToString(roomList2[n].Attribute("net").Value)),
                                                     new XAttribute("CancellationDate", ""),
                                                     new XAttribute("CancellationAmount", ""),
                                                     new XAttribute("sourcekey", roomList2[n].Attribute("sourcekey").Value),
                                                      new XAttribute("isAvailable", roomssss[1].risavail.ToString()),
                                                     new XElement("RequestID", Convert.ToString("")),
                                                     new XElement("Offers", ""),
                                                      new XElement("PromotionList", GetHotelpromotionsExtranet(promotions2)),
                                                     new XElement("CancellationPolicy", ""),
                                                     new XElement("Amenities", new XElement("Amenity", "")),
                                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                     new XElement("Supplements", Getsupplementsextranet(roomList2[n].Descendants("supplement").ToList())
                                                         ),
                                                         new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups2)),
                                                         new XElement("AdultNum", Convert.ToString(ttlroom[1].Descendants("Adult").FirstOrDefault().Value)),
                                                         new XElement("ChildNum", Convert.ToString(ttlroom[1].Descendants("Child").FirstOrDefault().Value))
                                                     ),

                                                    new XElement("Room",
                                                     new XAttribute("ID", Convert.ToString(roomList3[o].Parent.Parent.Attribute("code").Value)),
                                                     new XAttribute("SuppliersID", suppID),
                                                     new XAttribute("RoomSeq", "3"),
                                                     new XAttribute("SessionID", Convert.ToString(roomList3[o].Attribute("rateKey").Value)),
                                                     new XAttribute("RoomType", Convert.ToString(roomList3[o].Parent.Parent.Attribute("name").Value)),
                                                     new XAttribute("OccupancyID", Convert.ToString("")),
                                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                                     new XAttribute("MealPlanID", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanName", Convert.ToString(roomList3[o].Attribute("boardName").Value)),
                                                     new XAttribute("MealPlanCode", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                     new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                     new XAttribute("TotalRoomRate", Convert.ToString(roomList3[o].Attribute("net").Value)),
                                                     new XAttribute("CancellationDate", ""),
                                                     new XAttribute("CancellationAmount", ""),
                                                     new XAttribute("sourcekey", roomList3[o].Attribute("sourcekey").Value),
                                                      new XAttribute("isAvailable", roomssss[2].risavail.ToString()),
                                                     new XElement("RequestID", Convert.ToString("")),
                                                     new XElement("Offers", ""),
                                                      new XElement("PromotionList", GetHotelpromotionsExtranet(promotions3)),
                                                     new XElement("CancellationPolicy", ""),
                                                     new XElement("Amenities", new XElement("Amenity", "")),
                                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                     new XElement("Supplements", Getsupplementsextranet(roomList3[o].Descendants("supplement").ToList())
                                                         ),
                                                         new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups3)),
                                                         new XElement("AdultNum", Convert.ToString(ttlroom[2].Descendants("Adult").FirstOrDefault().Value)),
                                                         new XElement("ChildNum", Convert.ToString(ttlroom[2].Descendants("Child").FirstOrDefault().Value))
                                                     ),

                                                    new XElement("Room",
                                                     new XAttribute("ID", Convert.ToString(roomList4[p].Parent.Parent.Attribute("code").Value)),
                                                     new XAttribute("SuppliersID", suppID),
                                                     new XAttribute("RoomSeq", "4"),
                                                     new XAttribute("SessionID", Convert.ToString(roomList4[p].Attribute("rateKey").Value)),
                                                     new XAttribute("RoomType", Convert.ToString(roomList4[p].Parent.Parent.Attribute("name").Value)),
                                                     new XAttribute("OccupancyID", Convert.ToString("")),
                                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                                     new XAttribute("MealPlanID", Convert.ToString(roomList4[p].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanName", Convert.ToString(roomList4[p].Attribute("boardName").Value)),
                                                     new XAttribute("MealPlanCode", Convert.ToString(roomList4[p].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                     new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                     new XAttribute("TotalRoomRate", Convert.ToString(roomList4[p].Attribute("net").Value)),
                                                     new XAttribute("CancellationDate", ""),
                                                     new XAttribute("CancellationAmount", ""),
                                                     new XAttribute("sourcekey", roomList4[p].Attribute("sourcekey").Value),
                                                      new XAttribute("isAvailable", roomssss[3].risavail.ToString()),
                                                     new XElement("RequestID", Convert.ToString("")),
                                                     new XElement("Offers", ""),
                                                      new XElement("PromotionList", GetHotelpromotionsExtranet(promotions4)),
                                                     new XElement("CancellationPolicy", ""),
                                                     new XElement("Amenities", new XElement("Amenity", "")),
                                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                     new XElement("Supplements", Getsupplementsextranet(roomList4[p].Descendants("supplement").ToList())
                                                         ),
                                                         new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups4)),
                                                         new XElement("AdultNum", Convert.ToString(ttlroom[3].Descendants("Adult").FirstOrDefault().Value)),
                                                         new XElement("ChildNum", Convert.ToString(ttlroom[3].Descendants("Child").FirstOrDefault().Value))
                                                     ),

                                                    new XElement("Room",
                                                     new XAttribute("ID", Convert.ToString(roomList5[q].Parent.Parent.Attribute("code").Value)),
                                                     new XAttribute("SuppliersID", suppID),
                                                     new XAttribute("RoomSeq", "5"),
                                                     new XAttribute("SessionID", Convert.ToString(roomList5[q].Attribute("rateKey").Value)),
                                                     new XAttribute("RoomType", Convert.ToString(roomList5[q].Parent.Parent.Attribute("name").Value)),
                                                     new XAttribute("OccupancyID", Convert.ToString("")),
                                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                                     new XAttribute("MealPlanID", Convert.ToString(roomList5[q].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanName", Convert.ToString(roomList5[q].Attribute("boardName").Value)),
                                                     new XAttribute("MealPlanCode", Convert.ToString(roomList5[q].Attribute("boardCode").Value)),
                                                     new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                     new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                     new XAttribute("TotalRoomRate", Convert.ToString(roomList5[q].Attribute("net").Value)),
                                                     new XAttribute("CancellationDate", ""),
                                                     new XAttribute("CancellationAmount", ""),
                                                     new XAttribute("sourcekey", roomList5[q].Attribute("sourcekey").Value),
                                                      new XAttribute("isAvailable", roomssss[4].risavail.ToString()),
                                                     new XElement("RequestID", Convert.ToString("")),
                                                     new XElement("Offers", ""),
                                                      new XElement("PromotionList", GetHotelpromotionsExtranet(promotions5)),
                                                     new XElement("CancellationPolicy", ""),
                                                     new XElement("Amenities", new XElement("Amenity", "")),
                                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                     new XElement("Supplements", Getsupplementsextranet(roomList5[q].Descendants("supplement").ToList())
                                                         ),
                                                         new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups5)),
                                                         new XElement("AdultNum", Convert.ToString(ttlroom[4].Descendants("Adult").FirstOrDefault().Value)),
                                                         new XElement("ChildNum", Convert.ToString(ttlroom[4].Descendants("Child").FirstOrDefault().Value))
                                                     ),
                                                         new XElement("CancellationPolicies",
                                                     GetRoomCancellationPolicyExtranet(cxllist))
                                                     ));
                                                    #endregion
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }
                return str;
                #endregion
            }
            #endregion
            #region Room Count 6
            if (totalroom == 6)
            {
                str = room6grp(roomlist, cxllist);
            }
            #endregion
            return str;
        }
        #endregion
        #region Room's Grouping (6)
        private List<XElement> room6grp(List<XElement> roomlist, List<XElement> cxllist)
        {
            List<XElement> str = new List<XElement>();
            List<XElement> roomList1 = new List<XElement>();
            List<XElement> roomList2 = new List<XElement>();
            List<XElement> roomList3 = new List<XElement>();
            List<XElement> roomList4 = new List<XElement>();
            List<XElement> roomList5 = new List<XElement>();
            List<XElement> roomList6 = new List<XElement>();
            List<XElement> ttlroom = reqTravayoo.Descendants("RoomPax").ToList();
            try
            {
                List<roomsss> roomssss = new List<roomsss>() { new roomsss() { rno = 1, risavail = "false" }, new roomsss() { rno = 2, risavail = "false" }, new roomsss() { rno = 3, risavail = "false" }, new roomsss() { rno = 4, risavail = "false" }, new roomsss() { rno = 5, risavail = "false" }, new roomsss() { rno = 6, risavail = "false" } };
                #region Get Combination (Room 1)
                roomList1 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "1").ToList();
                #endregion
                #region Get Combination (Room 2)
                roomList2 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "2").ToList();
                #endregion
                #region Get Combination (Room 3)
                roomList3 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "3").ToList();
                #endregion
                #region Get Combination (Room 4)
                roomList4 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "4").ToList();
                #endregion
                #region Get Combination (Room 5)
                roomList5 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "5").ToList();
                #endregion
                #region Get Combination (Room 6)
                roomList6 = roomlist.Descendants("rate").Where(el => el.Attribute("roomNo").Value == "6").ToList();
                #endregion
                int group = 0;
                #region Room 6
                for (int m = 0; m < roomList1.Count(); m++)
                {
                    for (int n = 0; n < roomList2.Count(); n++)
                    {
                        for (int o = 0; o < roomList3.Count(); o++)
                        {
                            for (int p = 0; p < roomList4.Count(); p++)
                            {
                                for (int q = 0; q < roomList5.Count(); q++)
                                {
                                    for (int r = 0; r < roomList6.Count(); r++)
                                    {
                                        // add room 1, 2, 3, 4, 5, 6

                                        string bb1 = roomList1[m].Attribute("boardCode").Value;
                                        string bb2 = roomList2[n].Attribute("boardCode").Value;
                                        string bb3 = roomList3[o].Attribute("boardCode").Value;
                                        string bb4 = roomList4[p].Attribute("boardCode").Value;
                                        string bb5 = roomList5[q].Attribute("boardCode").Value;
                                        string bb6 = roomList6[r].Attribute("boardCode").Value;
                                        if (bb1 == bb2 && bb2 == bb3 && bb1 == bb3 && bb1 == bb4 && bb2 == bb4 && bb3 == bb4
                                            && bb1 == bb5 && bb2 == bb5 && bb3 == bb5 && bb4 == bb5
                                            && bb1 == bb6 && bb2 == bb6 && bb3 == bb6 && bb4 == bb6)
                                        {
                                            try
                                            {
                                                String condition = "";
                                                List<string> ratekeylist = new List<string>();
                                                ratekeylist.Add(roomList1[m].Parent.Parent.Attribute("code").Value);
                                                ratekeylist.Add(roomList2[n].Parent.Parent.Attribute("code").Value);
                                                ratekeylist.Add(roomList3[o].Parent.Parent.Attribute("code").Value);
                                                ratekeylist.Add(roomList4[p].Parent.Parent.Attribute("code").Value);
                                                ratekeylist.Add(roomList5[q].Parent.Parent.Attribute("code").Value);
                                                ratekeylist.Add(roomList6[r].Parent.Parent.Attribute("code").Value);
                                                int iii = 0;
                                                foreach (var rr in ratekeylist)
                                                {
                                                    roomssss[iii].rid = rr;
                                                    iii++;
                                                }
                                                var grouped = ratekeylist.GroupBy(ss => ss).Select(ax => new roomgroup
                                                {
                                                    Key = ax.Key,
                                                    Count = ax.Count(),
                                                    allocation = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value)),
                                                    OR = roomlist.Where(x => x.Attribute("code").Value == ax.Key).Descendants("rate").FirstOrDefault().Attributes("onRequest").FirstOrDefault().Value

                                                }).ToList();
                                                int k = 0;
                                                foreach (var item in grouped)
                                                {
                                                    var rtkey = item.Key;
                                                    var count = item.Count;
                                                    int totalt = roomlist.Where(x => x.Attribute("code").Value == rtkey).Descendants("rate").FirstOrDefault().Attributes("allotment").Sum(e => int.Parse(e.Value));
                                                    item.allocation = totalt;
                                                    if (k == grouped.Count() - 1)
                                                    {
                                                        condition = condition + totalt + " >= " + count;
                                                    }
                                                    else
                                                    {
                                                        condition = condition + totalt + " >= " + count + " and ";
                                                    }
                                                    k++;
                                                }
                                                System.Data.DataTable table = new System.Data.DataTable();
                                                table.Columns.Add("", typeof(Boolean));
                                                table.Columns[0].Expression = condition;
                                                System.Data.DataRow ckr = table.NewRow();
                                                table.Rows.Add(ckr);
                                                bool _condition = (Boolean)ckr[0];
                                                string r1isavail = "false";
                                                int noset = 0;
                                                if (_condition)
                                                {
                                                    roomssss[0].risavail = "true";
                                                    roomssss[1].risavail = "true";
                                                    roomssss[2].risavail = "true";
                                                    roomssss[3].risavail = "true";
                                                    roomssss[4].risavail = "true";
                                                    roomssss[5].risavail = "true";
                                                }
                                                else
                                                {
                                                    #region On Request
                                                    int _totalt = 0;
                                                    List<roomsss> new_roomssss = new List<roomsss>();
                                                    foreach (var item in roomssss)
                                                    {
                                                        var rr = item.rid;
                                                        var items = grouped.Where(a => a.Key == rr).FirstOrDefault();
                                                        _totalt = items.allocation;
                                                        if (_totalt <= 0)
                                                        {
                                                            string or = items.OR; ;
                                                            if (or == "true")
                                                                r1isavail = "false";
                                                            else
                                                                noset = 1;
                                                        }
                                                        else
                                                        {
                                                            items.allocation--;
                                                            r1isavail = "true";
                                                        }
                                                        item.risavail = r1isavail;
                                                    }
                                                    #endregion
                                                }
                                                if (noset == 0)
                                                {
                                                    //if (avail1 == avail2 && avail2 == avail3 && avail1 == avail3 && avail1 == avail4 && avail2 == avail4 && avail3 == avail4)
                                                    {
                                                        #region room's group
                                                        List<XElement> pricebrkups1 = roomList1[m].Descendants("dailyRate").ToList();
                                                        List<XElement> pricebrkups2 = roomList2[n].Descendants("dailyRate").ToList();
                                                        List<XElement> pricebrkups3 = roomList3[o].Descendants("dailyRate").ToList();
                                                        List<XElement> pricebrkups4 = roomList4[p].Descendants("dailyRate").ToList();
                                                        List<XElement> pricebrkups5 = roomList5[q].Descendants("dailyRate").ToList();
                                                        List<XElement> pricebrkups6 = roomList6[r].Descendants("dailyRate").ToList();
                                                        List<XElement> promotions1 = roomList1[m].Descendants("offer").ToList();
                                                        List<XElement> promotions2 = roomList2[n].Descendants("offer").ToList();
                                                        List<XElement> promotions3 = roomList3[o].Descendants("offer").ToList();
                                                        List<XElement> promotions4 = roomList4[p].Descendants("offer").ToList();
                                                        List<XElement> promotions5 = roomList5[q].Descendants("offer").ToList();
                                                        List<XElement> promotions6 = roomList6[r].Descendants("offer").ToList();
                                                        group++;
                                                        decimal totalrate = Convert.ToDecimal(roomList1[m].Attribute("net").Value) + Convert.ToDecimal(roomList2[n].Attribute("net").Value) + Convert.ToDecimal(roomList3[o].Attribute("net").Value) + Convert.ToDecimal(roomList4[p].Attribute("net").Value) + Convert.ToDecimal(roomList5[q].Attribute("net").Value) + Convert.ToDecimal(roomList6[r].Attribute("net").Value);

                                                        str.Add(new XElement("RoomTypes", new XAttribute("TotalRate", totalrate), new XAttribute("Index", group),
                                                        new XElement("Room",
                                                         new XAttribute("ID", Convert.ToString(roomList1[m].Parent.Parent.Attribute("code").Value)),
                                                         new XAttribute("SuppliersID", suppID),
                                                         new XAttribute("RoomSeq", "1"),
                                                         new XAttribute("SessionID", Convert.ToString(roomList1[m].Attribute("rateKey").Value)),
                                                         new XAttribute("RoomType", Convert.ToString(roomList1[m].Parent.Parent.Attribute("name").Value)),
                                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                         new XAttribute("MealPlanID", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanName", Convert.ToString(roomList1[m].Attribute("boardName").Value)),
                                                         new XAttribute("MealPlanCode", Convert.ToString(roomList1[m].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                         new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList1[m].Attribute("net").Value)),
                                                         new XAttribute("CancellationDate", ""),
                                                         new XAttribute("CancellationAmount", ""),
                                                         new XAttribute("sourcekey", roomList1[m].Attribute("sourcekey").Value),
                                                          new XAttribute("isAvailable", roomssss[0].risavail.ToString()),
                                                         new XElement("RequestID", Convert.ToString("")),
                                                         new XElement("Offers", ""),
                                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions1)),
                                                         new XElement("CancellationPolicy", ""),
                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                         new XElement("Supplements", Getsupplementsextranet(roomList1[m].Descendants("supplement").ToList())
                                                             ),
                                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups1)),
                                                             new XElement("AdultNum", Convert.ToString(ttlroom[0].Descendants("Adult").FirstOrDefault().Value)),
                                                             new XElement("ChildNum", Convert.ToString(ttlroom[0].Descendants("Child").FirstOrDefault().Value))
                                                         ),
                                                        new XElement("Room",
                                                         new XAttribute("ID", Convert.ToString(roomList2[n].Parent.Parent.Attribute("code").Value)),
                                                         new XAttribute("SuppliersID", suppID),
                                                         new XAttribute("RoomSeq", "2"),
                                                         new XAttribute("SessionID", Convert.ToString(roomList2[n].Attribute("rateKey").Value)),
                                                         new XAttribute("RoomType", Convert.ToString(roomList2[n].Parent.Parent.Attribute("name").Value)),
                                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                         new XAttribute("MealPlanID", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanName", Convert.ToString(roomList2[n].Attribute("boardName").Value)),
                                                         new XAttribute("MealPlanCode", Convert.ToString(roomList2[n].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                         new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList2[n].Attribute("net").Value)),
                                                         new XAttribute("CancellationDate", ""),
                                                         new XAttribute("CancellationAmount", ""),
                                                         new XAttribute("sourcekey", roomList2[n].Attribute("sourcekey").Value),
                                                          new XAttribute("isAvailable", roomssss[1].risavail.ToString()),
                                                         new XElement("RequestID", Convert.ToString("")),
                                                         new XElement("Offers", ""),
                                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions2)),
                                                         new XElement("CancellationPolicy", ""),
                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                         new XElement("Supplements", Getsupplementsextranet(roomList2[n].Descendants("supplement").ToList())
                                                             ),
                                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups2)),
                                                             new XElement("AdultNum", Convert.ToString(ttlroom[1].Descendants("Adult").FirstOrDefault().Value)),
                                                             new XElement("ChildNum", Convert.ToString(ttlroom[1].Descendants("Child").FirstOrDefault().Value))
                                                         ),
                                                        new XElement("Room",
                                                         new XAttribute("ID", Convert.ToString(roomList3[o].Parent.Parent.Attribute("code").Value)),
                                                         new XAttribute("SuppliersID", suppID),
                                                         new XAttribute("RoomSeq", "3"),
                                                         new XAttribute("SessionID", Convert.ToString(roomList3[o].Attribute("rateKey").Value)),
                                                         new XAttribute("RoomType", Convert.ToString(roomList3[o].Parent.Parent.Attribute("name").Value)),
                                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                         new XAttribute("MealPlanID", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanName", Convert.ToString(roomList3[o].Attribute("boardName").Value)),
                                                         new XAttribute("MealPlanCode", Convert.ToString(roomList3[o].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                         new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList3[o].Attribute("net").Value)),
                                                         new XAttribute("CancellationDate", ""),
                                                         new XAttribute("CancellationAmount", ""),
                                                         new XAttribute("sourcekey", roomList3[o].Attribute("sourcekey").Value),
                                                          new XAttribute("isAvailable", roomssss[2].risavail.ToString()),
                                                         new XElement("RequestID", Convert.ToString("")),
                                                         new XElement("Offers", ""),
                                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions3)),
                                                         new XElement("CancellationPolicy", ""),
                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                         new XElement("Supplements", Getsupplementsextranet(roomList3[o].Descendants("supplement").ToList())
                                                             ),
                                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups3)),
                                                             new XElement("AdultNum", Convert.ToString(ttlroom[2].Descendants("Adult").FirstOrDefault().Value)),
                                                             new XElement("ChildNum", Convert.ToString(ttlroom[2].Descendants("Child").FirstOrDefault().Value))
                                                         ),
                                                        new XElement("Room",
                                                         new XAttribute("ID", Convert.ToString(roomList4[p].Parent.Parent.Attribute("code").Value)),
                                                         new XAttribute("SuppliersID", suppID),
                                                         new XAttribute("RoomSeq", "4"),
                                                         new XAttribute("SessionID", Convert.ToString(roomList4[p].Attribute("rateKey").Value)),
                                                         new XAttribute("RoomType", Convert.ToString(roomList4[p].Parent.Parent.Attribute("name").Value)),
                                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                         new XAttribute("MealPlanID", Convert.ToString(roomList4[p].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanName", Convert.ToString(roomList4[p].Attribute("boardName").Value)),
                                                         new XAttribute("MealPlanCode", Convert.ToString(roomList4[p].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                         new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList4[p].Attribute("net").Value)),
                                                         new XAttribute("CancellationDate", ""),
                                                         new XAttribute("CancellationAmount", ""),
                                                         new XAttribute("sourcekey", roomList4[p].Attribute("sourcekey").Value),
                                                          new XAttribute("isAvailable", roomssss[3].risavail.ToString()),
                                                         new XElement("RequestID", Convert.ToString("")),
                                                         new XElement("Offers", ""),
                                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions4)),
                                                         new XElement("CancellationPolicy", ""),
                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                         new XElement("Supplements", Getsupplementsextranet(roomList4[p].Descendants("supplement").ToList())
                                                             ),
                                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups4)),
                                                             new XElement("AdultNum", Convert.ToString(ttlroom[3].Descendants("Adult").FirstOrDefault().Value)),
                                                             new XElement("ChildNum", Convert.ToString(ttlroom[3].Descendants("Child").FirstOrDefault().Value))
                                                         ),
                                                        new XElement("Room",
                                                         new XAttribute("ID", Convert.ToString(roomList5[q].Parent.Parent.Attribute("RoomTypeId").Value)),
                                                         new XAttribute("SuppliersID", suppID),
                                                         new XAttribute("RoomSeq", "5"),
                                                         new XAttribute("SessionID", Convert.ToString(roomList5[q].Attribute("rateKey").Value)),
                                                         new XAttribute("RoomType", Convert.ToString(roomList5[q].Parent.Parent.Attribute("name").Value)),
                                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                         new XAttribute("MealPlanID", Convert.ToString(roomList5[q].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanName", Convert.ToString(roomList5[q].Attribute("boardName").Value)),
                                                         new XAttribute("MealPlanCode", Convert.ToString(roomList5[q].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                         new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList5[q].Attribute("net").Value)),
                                                         new XAttribute("CancellationDate", ""),
                                                         new XAttribute("CancellationAmount", ""),
                                                         new XAttribute("sourcekey", roomList5[q].Attribute("sourcekey").Value),
                                                          new XAttribute("isAvailable", roomssss[4].risavail.ToString()),
                                                         new XElement("RequestID", Convert.ToString("")),
                                                         new XElement("Offers", ""),
                                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions5)),
                                                         new XElement("CancellationPolicy", ""),
                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                         new XElement("Supplements", Getsupplementsextranet(roomList5[q].Descendants("supplement").ToList())
                                                             ),
                                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups5)),
                                                             new XElement("AdultNum", Convert.ToString(ttlroom[4].Descendants("Adult").FirstOrDefault().Value)),
                                                             new XElement("ChildNum", Convert.ToString(ttlroom[4].Descendants("Child").FirstOrDefault().Value))
                                                         ),
                                                        new XElement("Room",
                                                         new XAttribute("ID", Convert.ToString(roomList6[r].Parent.Parent.Attribute("code").Value)),
                                                         new XAttribute("SuppliersID", suppID),
                                                         new XAttribute("RoomSeq", "6"),
                                                         new XAttribute("SessionID", Convert.ToString(roomList6[r].Attribute("rateKey").Value)),
                                                         new XAttribute("RoomType", Convert.ToString(roomList6[r].Parent.Parent.Attribute("name").Value)),
                                                         new XAttribute("OccupancyID", Convert.ToString("")),
                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                         new XAttribute("MealPlanID", Convert.ToString(roomList6[r].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanName", Convert.ToString(roomList6[r].Attribute("boardName").Value)),
                                                         new XAttribute("MealPlanCode", Convert.ToString(roomList6[r].Attribute("boardCode").Value)),
                                                         new XAttribute("MealPlanPrice", Convert.ToString("")),
                                                         new XAttribute("PerNightRoomRate", Convert.ToString(0)),
                                                         new XAttribute("TotalRoomRate", Convert.ToString(roomList6[r].Attribute("net").Value)),
                                                         new XAttribute("CancellationDate", ""),
                                                         new XAttribute("CancellationAmount", ""),
                                                         new XAttribute("sourcekey", roomList6[r].Attribute("sourcekey").Value),
                                                          new XAttribute("isAvailable", roomssss[5].risavail.ToString()),
                                                         new XElement("RequestID", Convert.ToString("")),
                                                         new XElement("Offers", ""),
                                                          new XElement("PromotionList", GetHotelpromotionsExtranet(promotions6)),
                                                         new XElement("CancellationPolicy", ""),
                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                         new XElement("Supplements", Getsupplementsextranet(roomList6[r].Descendants("supplement").ToList())
                                                             ),
                                                             new XElement("PriceBreakups", GetRoomsPriceBreakupExtranet(pricebrkups6)),
                                                             new XElement("AdultNum", Convert.ToString(ttlroom[5].Descendants("Adult").FirstOrDefault().Value)),
                                                             new XElement("ChildNum", Convert.ToString(ttlroom[5].Descendants("Child").FirstOrDefault().Value))
                                                         ),
                                                             new XElement("CancellationPolicies",
                                                         GetRoomCancellationPolicyExtranet(cxllist))
                                                         ));
                                                        #endregion
                                                    }
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return str;
                #endregion
            }
            catch { return null; }
        }
        #endregion
        #region Hotel Facilities Extranet
        private IEnumerable<XElement> hotelfacilitiesExtranet(List<XElement> facility)
        {
            Int32 length = 0;
            if (facility != null)
            {
                length = facility.Count();
            }
            List<XElement> fac = new List<XElement>();
            if (length == 0)
            {
                try
                {
                    fac.Add(new XElement("Facility", "No Facility Available"));
                }
                catch { }
            }
            else
            {
                for (int i = 0; i < facility.Count(); i++)
                {
                    try
                    {
                        fac.Add(new XElement("Facility", Convert.ToString(facility[i].Value)));
                    }
                    catch { }
                }
            }
            return fac;
        }
        #endregion
        #region Extranet's Room's Promotion
        private IEnumerable<XElement> GetHotelpromotionsExtranet(List<XElement> roompromotions)
        {
            Int32 length = roompromotions.Count();
            List<XElement> promotion = new List<XElement>();

            if (length == 0)
            {
                promotion.Add(new XElement("Promotions", ""));
            }
            else
            {
                for (int i = 0; i < roompromotions.Count(); i++)
                {
                    promotion.Add(new XElement("Promotions", Convert.ToString(roompromotions[i].Attribute("name").Value)));
                }
            }
            return promotion;
        }
        #endregion
        #region Extranet Room's Price Breakups
        private IEnumerable<XElement> GetRoomsPriceBreakupExtranet(List<XElement> pricebreakups)
        {
            #region Extranet Room's Price Breakups
            List<XElement> str = new List<XElement>();
            try
            {
                for (int i = 0; i < pricebreakups.Count(); i++)
                {
                    str.Add(new XElement("Price",
                           new XAttribute("Night", Convert.ToString(Convert.ToInt32(i + 1))),
                           new XAttribute("PriceValue", Convert.ToString(pricebreakups[i].Attribute("dailyNet").Value)),
                           new XAttribute("sourcekey", pricebreakups[i].Attribute("sourcekey") == null ? "" : pricebreakups[i].Attribute("sourcekey").Value),
                           new XAttribute("MarkUp", pricebreakups[i].Attribute("MarkUp") == null ? "" : Convert.ToString(pricebreakups[i].Attribute("MarkUp").Value)),
                           new XAttribute("MarkUpType", pricebreakups[i].Attribute("MarkUpType") == null ? "" : Convert.ToString(pricebreakups[i].Attribute("MarkUpType").Value)),
                           new XAttribute("MarkUpValue", pricebreakups[i].Attribute("MarkUpValue") == null ? "" : Convert.ToString(pricebreakups[i].Attribute("MarkUpValue").Value)),
                           new XAttribute("DescSupply", pricebreakups[i].Attribute("DescSupply") == null ? "" : Convert.ToString(pricebreakups[i].Attribute("DescSupply").Value)),
                           new XAttribute("DescDiscount", pricebreakups[i].Attribute("DescDiscount") == null ? "" : Convert.ToString(pricebreakups[i].Attribute("DescDiscount").Value)),
                           new XAttribute("DescPrice", pricebreakups[i].Attribute("DescPrice") == null ? "" : Convert.ToString(pricebreakups[i].Attribute("DescPrice").Value)))
                    );
                }
                return str.OrderBy(x => (int)x.Attribute("Night")).ToList();
            }
            catch { return null; }
            #endregion
        }
        #endregion
        #region Extranet's Supplements
        private IEnumerable<XElement> Getsupplementsextranet(List<XElement> supplements)
        {
            Int32 length = supplements.Count();
            List<XElement> supplementlst = new List<XElement>();
            if (length == 0)
            {                
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    supplementlst.Add(new XElement("Supplement",
                             new XAttribute("suppId", Convert.ToString("0")),
                             new XAttribute("suppName", Convert.ToString(supplements[i].Attribute("name").Value)),
                             new XAttribute("supptType", Convert.ToString("0")),
                             new XAttribute("suppIsMandatory", Convert.ToString(supplements[i].Attribute("mandatory").Value)),
                             new XAttribute("suppChargeType", Convert.ToString(supplements[i].Attribute("type").Value)),
                             new XAttribute("suppPrice", Convert.ToString(supplements[i].Attribute("price").Value)),
                             new XAttribute("suppType", Convert.ToString("PerRoomSupplement")))
                          );
                }
            }
            return supplementlst;
        }
        #endregion
        #region Room's Cancellation Policies from Extranet
        private IEnumerable<XElement> GetRoomCancellationPolicyExtranet(List<XElement> cancellationpolicy)
        {
            #region Room's Cancellation Policies from Extranet
            List<XElement> htrm = new List<XElement>();
            List<XElement> pc = new List<XElement>();
            for (int i = 0; i < cancellationpolicy.Count(); i++)
            {
                string currencycode = string.Empty;
                try
                { currencycode = cancellationpolicy[i].Attribute("Currency").Value; }
                catch { }
                htrm.Add(new XElement("CancellationPolicy", "Cancellation done on after " + cancellationpolicy[i].Attribute("date").Value + "  will apply " + currencycode + " " + cancellationpolicy[i].Attribute("amount").Value + "  Cancellation fee"
                    , new XAttribute("LastCancellationDate", Convert.ToString(cancellationpolicy[i].Attribute("date").Value))
                    , new XAttribute("ApplicableAmount", cancellationpolicy[i].Attribute("amount").Value)
                     , new XAttribute("RefundValue", cancellationpolicy[i].Attribute("RefundValue") == null ? "" : cancellationpolicy[i].Attribute("RefundValue").Value)
                      , new XAttribute("MarkUpInBreakUps", cancellationpolicy[i].Attribute("MarkUpInBreakUps") == null ? "" : cancellationpolicy[i].Attribute("MarkUpInBreakUps").Value)
                       , new XAttribute("MarkUpApplied", cancellationpolicy[i].Attribute("MarkUpApplied") == null ? "" : cancellationpolicy[i].Attribute("MarkUpApplied").Value)
                    , new XAttribute("NoShowPolicy", "0")));
            };
            pc.Add(new XElement("Room", htrm));
            XElement cxlfinal = MergCxlPolicy(pc);
            List<XElement> cxlfinalres = cxlfinal.Descendants("CancellationPolicy").ToList();
            return cxlfinalres;
            #endregion
        }
        public XElement MergCxlPolicy(List<XElement> rooms)
        {
            List<XElement> cxlList = new List<XElement>();

            IEnumerable<XElement> dateLst = rooms.Descendants("CancellationPolicy").
               GroupBy(r => new { r.Attribute("LastCancellationDate").Value, noshow = r.Attribute("NoShowPolicy").Value }).Select(y => y.First()).
               OrderBy(p => p.Attribute("LastCancellationDate").Value);
            if (dateLst.Count() > 0)
            {

                foreach (var item in dateLst)
                {
                    string date = item.Attribute("LastCancellationDate").Value;
                    string noShow = item.Attribute("NoShowPolicy").Value;
                    decimal datePrice = 0.0m;
                    foreach (var rm in rooms)
                    {
                        var prItem = rm.Descendants("CancellationPolicy").
                       Where(pq => (pq.Attribute("NoShowPolicy").Value == noShow && pq.Attribute("LastCancellationDate").Value == date)).
                       FirstOrDefault();
                        if (prItem != null)
                        {
                            var price = prItem.Attribute("ApplicableAmount").Value;
                            datePrice += Convert.ToDecimal(price);
                        }
                        else
                        {
                            if (noShow == "1")
                            {
                                datePrice += Convert.ToDecimal(rm.Attribute("TotalRoomRate").Value);
                            }
                            else
                            {


                                var lastItem = rm.Descendants("CancellationPolicy").
                                    Where(pq => (pq.Attribute("NoShowPolicy").Value == noShow && Convert.ToDateTime(pq.Attribute("LastCancellationDate").Value) < date.chnagetoTime()));

                                if (lastItem.Count() > 0)
                                {
                                    var lastDate = lastItem.Max(y => y.Attribute("LastCancellationDate").Value);
                                    var lastprice = rm.Descendants("CancellationPolicy").
                                        Where(pq => (pq.Attribute("NoShowPolicy").Value == noShow && pq.Attribute("LastCancellationDate").Value == lastDate)).
                                        FirstOrDefault().Attribute("ApplicableAmount").Value;
                                    datePrice += Convert.ToDecimal(lastprice);
                                }

                            }
                        }
                    }
                    XElement pItem = new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", date), new XAttribute("ApplicableAmount", datePrice), new XAttribute("NoShowPolicy", noShow));
                    cxlList.Add(pItem);

                }

                cxlList = cxlList.GroupBy(x => new { DateTime.ParseExact(x.Attribute("LastCancellationDate").Value, "dd/MM/yyyy", null).Date, x.Attribute("NoShowPolicy").Value }).
                    Select(y => new XElement("CancellationPolicy",
                        new XAttribute("LastCancellationDate", y.Key.Date.ToString("dd/MM/yyyy")),
                        new XAttribute("ApplicableAmount", y.Max(p => Convert.ToDecimal(p.Attribute("ApplicableAmount").Value))),
                        new XAttribute("NoShowPolicy", y.Key.Value))).OrderBy(p => p.Attribute("LastCancellationDate").Value).ToList();

                var fItem = cxlList.FirstOrDefault();

                if (Convert.ToDecimal(fItem.Attribute("ApplicableAmount").Value) != 0.0m)
                {
                    cxlList.Insert(0, new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", DateTime.ParseExact(fItem.Attribute("LastCancellationDate").Value, "dd/MM/yyyy", null).AddDays(-1).Date.ToString("dd/MM/yyyy")), new XAttribute("ApplicableAmount", "0.00"), new XAttribute("NoShowPolicy", "0")));

                }
            }
            XElement cxlItem = new XElement("CancellationPolicies", cxlList);
            return cxlItem;

        }  
        #endregion
        #region Remove Namespaces
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            XElement xmlDocumentWithoutNs = removeAllNamespaces(xmlDocument);
            return xmlDocumentWithoutNs;
        }

        private static XElement removeAllNamespaces(XElement xmlDocument)
        {
            var stripped = new XElement(xmlDocument.Name.LocalName);
            foreach (var attribute in
                    xmlDocument.Attributes().Where(
                    attribute =>
                        !attribute.IsNamespaceDeclaration &&
                        String.IsNullOrEmpty(attribute.Name.NamespaceName)))
            {
                stripped.Add(new XAttribute(attribute.Name.LocalName, attribute.Value));
            }
            if (!xmlDocument.HasElements)
            {
                stripped.Value = xmlDocument.Value;
                return stripped;
            }
            stripped.Add(xmlDocument.Elements().Select(
                el =>
                    RemoveAllNamespaces(el)));
            return stripped;
        }
        #endregion
        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}