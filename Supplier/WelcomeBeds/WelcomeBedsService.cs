
using System.Net;
using TravillioXMLOutService.App_Code;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Supplier.Welcomebeds;
using TravillioXMLOutService.Models.EBookingCenter;
using TravillioXMLOutService.Models.Common;
using TravillioXMLOutService.Common;
using TravillioXMLOutService.Supplier.EBookingCenter;
using System.IO;
using System.Configuration;



namespace TravillioXMLOutService.Supplier.Welcomebeds
{
    public class WelcomebedsService
    {
        #region Credentails
        string AccountCode = string.Empty;
        string Password  = string.Empty;
        string System = string.Empty;
        string SalesChannel = string.Empty;
        string Language = string.Empty;
        string ConnectionString = string.Empty;
        string customerid = string.Empty;
        string Url = string.Empty;
        string AgentName = string.Empty;

        #endregion
        #region Global vars
        string dmc = string.Empty;
        const int SupplierId = 44;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        XNamespace ns = XNamespace.Get("http://www.opentravel.org/OTA/2003/05");
        WBRequest srvRequest;
        int timeOut = 27000;
        #endregion
        public WelcomebedsService(string _customerid)
        {
            XElement suppliercred = supplier_Cred.getsupplier_credentials(_customerid, "44");
            try
            {
                AccountCode = suppliercred.Descendants("AccountCode").FirstOrDefault().Value;
                Password = suppliercred.Descendants("Password").FirstOrDefault().Value;
                System = suppliercred.Descendants("System").FirstOrDefault().Value;
                SalesChannel = suppliercred.Descendants("SalesChannel").FirstOrDefault().Value;
                Language = suppliercred.Descendants("Language").FirstOrDefault().Value; ;
                ConnectionString = suppliercred.Descendants("ConnectionString").FirstOrDefault().Value;
                Url = suppliercred.Descendants("Url").FirstOrDefault().Value;
                AgentName = suppliercred.Descendants("TravelAgentName").FirstOrDefault().Value; 
            }
            catch { }
        }
        public WelcomebedsService()
        {

        }

        #region Hotel Availability
        public List<XElement> HotelAvailability(XElement req,string custID, string xtype)
        {
            #region get cut off time
            int timeOutsup = 0;
            try
            {
                timeOutsup = supplier_Cred.secondcutoff_time(req.Descendants("HotelID").FirstOrDefault().Value);
            }
            catch { }
            #endregion
            dmc = xtype;
            customerid = custID;
            string soapResult = string.Empty;
            string htlCode = string.Empty;
            int chunksize = Convert.ToInt32(ConfigurationManager.AppSettings["chunksize"]);
            List<XElement> HotelsList = new List<XElement>();
            try
            {
                WBStaticData wbStatic = new WBStaticData();
                DataTable wbCity = wbStatic.GetWBCityCode(req.Descendants("CityID").FirstOrDefault().Value, 44);
                DataTable wbHotelCode = wbStatic.GetWBStaticHotels(wbCity.Rows[0]["CityCode"].ToString(),wbCity.Rows[0]["CountryCode"].ToString(),null,0,5);
                DataTable wbHotels = wbStatic.GetWBStaticHotels(wbCity.Rows[0]["CityCode"].ToString(), wbCity.Rows[0]["CountryCode"].ToString(), null, req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt(), req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt());
                var statiProperties = wbHotelCode.AsEnumerable().Select(r => r.Field<string>("HotelId")).ToList();
                List<List<string>> splitList = statiProperties.SplitPropertyList(chunksize);

                srvRequest = new WBRequest();
                XmlDocument SoapReq = new XmlDocument();
                string nationality = req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value;
                #region hotelname

                string HtId = req.Descendants("HotelID").FirstOrDefault().Value;
                XElement htlItem = null;

                if (!string.IsNullOrEmpty(req.Descendants("HotelID").FirstOrDefault().Value) || !string.IsNullOrEmpty(req.Descendants("HotelName").FirstOrDefault().Value))
                {

                    string CityId = wbCity.Rows[0]["CityCode"].ToString();
                    var model = new SqlModel()
                    {
                        flag = 2,
                        columnList = "ID_HOTEL as HotelID,NOMBRE_HOTEL as HotelName, ID_CATEGORY as StarRating",
                        table = "tblWelcomeBedsHotelList",
                        filter = "ID_PROVINCE='" + CityId + "' AND NOMBRE_HOTEL LIKE '%" + req.Descendants("HotelName").FirstOrDefault().Value + "%'",
                        SupplierId = SupplierId
                    };
                    if (!string.IsNullOrEmpty(req.Descendants("HotelID").FirstOrDefault().Value))
                    {
                        model.HotelCode = req.Descendants("HotelID").FirstOrDefault().Value;
                        timeOut = 10000;
                    }
                    DataTable htlList = TravayooRepository.GetData(model);

                    if (htlList.Rows.Count > 0)
                    {
                        var result = htlList.AsEnumerable().Select(y => new XElement("productCode", y.Field<string>("HotelID")));
                        htlItem = new XElement("productCodes", result);
                    }
                    else
                    {
                        //throw new Exception("There is no hotel available in database");
                        return null;
                    }
                }
                #endregion hotelname
                XElement PaxProfile = null;
                int sup_cutime = 100000, threadCount = 2;
                List<XElement> OccupancyList = new List<XElement>();
                GetSearchPaxProfile(req.Descendants("RoomPax").ToList(), nationality, out PaxProfile, out OccupancyList);
                int rmCount = -1;
                List<XElement> thr1 = new List<XElement>();
                List<XElement> thr2 = new List<XElement>();
                List<XElement> thr3 = new List<XElement>();
                List<XElement> thr4 = new List<XElement>();
                List<XElement> thr5 = new List<XElement>();
                List<XElement> thr6 = new List<XElement>();

                List<Thread> threadlist;
                #region Commented
                ////for (int i = 0; i < splitList.Count(); i++)
                // {
                //     if (splitList.Count() ==  1)
                //     {
                //          List<XElement>  hotelCodeElement = callHotelElement(splitList[0] , "5");
                //         threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic)),
                //                };
                //         threadlist.ForEach(t => t.Start());
                //         threadlist.ForEach(t => t.Join(timeOut));
                //         threadlist.ForEach(t => t.Abort());
                //         HotelsList.AddRange(thr1);
                //     }


                //     if (splitList.Count() == 2 )
                //     {
                //         List<XElement> hotelCodeElement = callHotelElement(splitList[0], "2");
                //         List<XElement> hotelCodeElement2 = callHotelElement(splitList[1], "2");
                //         threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic)),
                //                     new Thread(()=> thr2 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement2,wbCity , wbStatic)),
                //                };
                //         threadlist.ForEach(t => t.Start());
                //         threadlist.ForEach(t => t.Join(timeOut));
                //         threadlist.ForEach(t => t.Abort());
                //         //if (thr1.Count > 0 && thr2.Count > 0)
                //         //{
                //         //    HotelsList.AddRange(thr1.Concat(thr2));
                //         //}
                //         //else
                //         //{
                //             if (thr1.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr1);
                //             }

                //             if (thr2.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr2);
                //             }
                //         //}
                         
                //     }

                //     if (splitList.Count() == 3)
                //     {
                //         List<XElement> hotelCodeElement = callHotelElement(splitList[0], "2");
                //         List<XElement> hotelCodeElement2 = callHotelElement(splitList[1], "2");
                //         List<XElement> hotelCodeElement3 = callHotelElement(splitList[2], "2");
                //         threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic)),
                //                     new Thread(()=> thr2 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement2,wbCity , wbStatic)),
                //                      new Thread(()=> thr3 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement3,wbCity , wbStatic)),
                //                };
                //         threadlist.ForEach(t => t.Start());
                //         threadlist.ForEach(t => t.Join(timeOut));
                //         threadlist.ForEach(t => t.Abort());

                //         //if (thr1.Count > 0 && thr2.Count > 0 && thr3.Count > 0)
                //         //{
                //         //    HotelsList.AddRange(thr1.Concat(thr2).Concat(thr3));
                //         //}
                //         //else
                //         //{
                //             if (thr1.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr1);
                //             }
                //             if (thr2.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr2);
                //             }
                //             if (thr3.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr3);
                //             }
                //         //}
                //     }


                //     if (splitList.Count() == 4)
                //     {
                //         List<XElement> hotelCodeElement = callHotelElement(splitList[0], "2");
                //         List<XElement> hotelCodeElement2 = callHotelElement(splitList[1], "2");
                //         List<XElement> hotelCodeElement3 = callHotelElement(splitList[2], "2");
                //         List<XElement> hotelCodeElement4 = callHotelElement(splitList[3], "2");
                //         threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic)),
                //                     new Thread(()=> thr2 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement2,wbCity , wbStatic)),
                //                      new Thread(()=> thr3 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement3,wbCity , wbStatic)),
                //                        new Thread(()=> thr4 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement4,wbCity , wbStatic)),
                //                };
                //         threadlist.ForEach(t => t.Start());
                //         threadlist.ForEach(t => t.Join(timeOut));
                //         threadlist.ForEach(t => t.Abort());
                //         //if (thr1.Count > 0 && thr2.Count > 0 && thr3.Count > 0 && thr4.Count > 0)
                //         //{
                //         //    HotelsList.AddRange(thr1.Concat(thr2).Concat(thr3).Concat(thr4));
                //         //}
                //         //else
                //         //{
                //             if (thr1.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr1);
                //             }
                //             if (thr2.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr2);
                //             }
                //             if (thr3.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr3);
                //             }
                //             if (thr4.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr4);
                //             }
                //         //}
                        
                //     }

                //     if (splitList.Count() == 5)
                //     {
                //         List<XElement> hotelCodeElement = callHotelElement(splitList[0], "2");
                //         List<XElement> hotelCodeElement2 = callHotelElement(splitList[1], "2");
                //         List<XElement> hotelCodeElement3 = callHotelElement(splitList[2], "2");
                //         List<XElement> hotelCodeElement4 = callHotelElement(splitList[3], "2");
                //         List<XElement> hotelCodeElement5 = callHotelElement(splitList[4], "2");
                //         threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic)),
                //                     new Thread(()=> thr2 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement2,wbCity , wbStatic)),
                //                      new Thread(()=> thr3 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement3,wbCity , wbStatic)),
                //                        new Thread(()=> thr4 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement4,wbCity , wbStatic)),
                //                          new Thread(()=> thr5 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement5,wbCity , wbStatic)),
                //                };
                //         threadlist.ForEach(t => t.Start());
                //         threadlist.ForEach(t => t.Join(timeOut));
                //         threadlist.ForEach(t => t.Abort());
                //         //if (thr1.Count > 0 && thr2.Count > 0 && thr3.Count > 0 && thr4.Count > 0 && thr5.Count > 0)
                //         //{
                //         //   HotelsList.AddRange(thr1.Concat(thr2).Concat(thr3).Concat(thr4).Concat(thr5));
                //         //}
                //         //else
                //         //{
                //             if (thr1.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr1);
                //             }
                //             if (thr2.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr2);
                //             }
                //             if (thr3.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr3);
                //             }
                //             if (thr4.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr4);
                //             }
                //             if (thr5.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr5);
                //             }
                //         //}
                //     }

                //     if (splitList.Count() == 6)
                //     {
                //         List<XElement> hotelCodeElement = callHotelElement(splitList[0], "2");
                //         List<XElement> hotelCodeElement2 = callHotelElement(splitList[1], "2");
                //         List<XElement> hotelCodeElement3 = callHotelElement(splitList[2], "2");
                //         List<XElement> hotelCodeElement4 = callHotelElement(splitList[3], "2");
                //         List<XElement> hotelCodeElement5 = callHotelElement(splitList[4], "2");
                //         List<XElement> hotelCodeElement6 = callHotelElement(splitList[5], "2");
                //         threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic)),
                //                     new Thread(()=> thr2 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement2,wbCity , wbStatic)),
                //                      new Thread(()=> thr3 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement3,wbCity , wbStatic)),
                //                        new Thread(()=> thr4 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement4,wbCity , wbStatic)),
                //                          new Thread(()=> thr5 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement5,wbCity , wbStatic)),
                //                           new Thread(()=> thr6 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement6,wbCity , wbStatic)),
                //                };
                //         threadlist.ForEach(t => t.Start());
                //         threadlist.ForEach(t => t.Join(timeOut));
                //         threadlist.ForEach(t => t.Abort());
                //         //if (thr1.Count > 0 && thr2.Count > 0 && thr3.Count > 0 && thr4.Count > 0 && thr5.Count > 0 && thr6.Count > 0)
                //         //{
                //         //    HotelsList.AddRange(thr1.Concat(thr2).Concat(thr3).Concat(thr4).Concat(thr5).Concat(thr6));
                //         //}
                //         //else
                //         //{
                //             if (thr1.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr1);
                //             }
                //             if (thr2.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr2);
                //             }
                //             if (thr3.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr3);
                //             }
                //             if (thr4.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr4);
                //             }
                //             if (thr5.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr5);
                //             }
                //             if (thr6.Count > 0)
                //             {
                //                 HotelsList.AddRange(thr6);
                //             }
                //         //}
                //     }
                // }
                #endregion

                 int Number = splitList.Count;
                 bool checkod = true;
                 if (Number % 2 == 0)
                 {
                     checkod = false;
                 }
                 else
                 {
                     Number = Number + 1;
                 }
                 int count = 1;
                 System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                 timer.Start();
                 if (splitList.Count() == 1)
                 {
                     List<XElement> hotelCodeElement = callHotelElement(splitList[0], "5");
                     threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic, wbHotels)),
                                };                     
                     threadlist.ForEach(t => t.Start());
                     threadlist.ForEach(t => t.Join(timeOutsup));
                     threadlist.ForEach(t => t.Abort());
                     try
                     {
                         XElement res = new XElement("Hotels", thr1);
                         foreach (XElement hotel in res.Descendants("Hotel"))
                             HotelsList.Add(hotel);
                     }
                     catch { }
                     timeOutsup = timeOutsup - Convert.ToInt32(timer.ElapsedMilliseconds);
                 }
                 else
                 {
                     for (int i = 0; i < Number / 2; i += 2)
                     {
                         if (timeOutsup >= 2)
                         {
                             if (checkod == true && i == Number - 1)
                             {
                                 List<XElement> hotelCodeElement = callHotelElement(splitList[i], "2");
                                 threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic, wbHotels)),
                                };  
                                 threadlist.ForEach(t => t.Start());
                                 threadlist.ForEach(t => t.Join(timeOutsup));
                                 threadlist.ForEach(t => t.Abort());
                                 try
                                 {
                                     XElement res = new XElement("Hotels", thr1);
                                     foreach (XElement hotel in res.Descendants("Hotel"))
                                         HotelsList.Add(hotel);
                                 }
                                 catch { }
                             }
                             else
                             {
                                 List<XElement> hotelCodeElement = callHotelElement(splitList[i], "2");
                                 List<XElement> hotelCodeElement2 = callHotelElement(splitList[i+1], "2");
                                 threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic, wbHotels)),
                                     new Thread(()=> thr2 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement2,wbCity , wbStatic, wbHotels)),
                                };
                                 threadlist.ForEach(t => t.Start());
                                 threadlist.ForEach(t => t.Join(timeOutsup));
                                 threadlist.ForEach(t => t.Abort());
                                 if (thr1 != null)
                                 {
                                     try
                                     {
                                         XElement res = new XElement("Hotels", thr1);
                                         foreach (XElement hotel in res.Descendants("Hotel"))
                                             HotelsList.Add(hotel);
                                     }
                                     catch { }
                                 }
                                 if (thr2 != null)
                                 {
                                     try
                                     {
                                         XElement res = new XElement("Hotels", thr2);
                                         foreach (XElement hotel in res.Descendants("Hotel"))
                                             HotelsList.Add(hotel);
                                     }
                                     catch { }
                                 }
                             }
                             timeOutsup = timeOutsup - Convert.ToInt32(timer.ElapsedMilliseconds);
                         }
                     }
                 }

                 return HotelsList;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public List<XElement> HotelAvailabilityNew(XElement req, string custID, string xtype)
        {
            #region get cut off time
            int timeOutsup = 0;
            try
            {
                timeOutsup = supplier_Cred.secondcutoff_time(req.Descendants("HotelID").FirstOrDefault().Value);
            }
            catch { }
            #endregion
            dmc = xtype;
            customerid = custID;
            string soapResult = string.Empty;
            string htlCode = string.Empty;
            int chunksize = Convert.ToInt32(ConfigurationManager.AppSettings["chunksize"]);
            List<XElement> HotelsList = new List<XElement>();
            try
            {
                WBStaticData wbStatic = new WBStaticData();
                DataTable wbCity = wbStatic.GetWBCityCode(req.Descendants("CityID").FirstOrDefault().Value, 44);
                DataTable wbHotelCode = wbStatic.GetWBStaticHotels(wbCity.Rows[0]["CityCode"].ToString(), wbCity.Rows[0]["CountryCode"].ToString(), null, 0, 5);
                DataTable wbHotels = wbStatic.GetWBStaticHotels(wbCity.Rows[0]["CityCode"].ToString(), wbCity.Rows[0]["CountryCode"].ToString(), null, req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt(), req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt());
                var statiProperties = wbHotelCode.AsEnumerable().Select(r => r.Field<string>("HotelId")).ToList();
                List<List<string>> splitList = statiProperties.SplitPropertyList(chunksize);

                srvRequest = new WBRequest();
                XmlDocument SoapReq = new XmlDocument();
                string nationality = req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value;
                #region hotelname

                string HtId = req.Descendants("HotelID").FirstOrDefault().Value;
                XElement htlItem = null;

                if (!string.IsNullOrEmpty(req.Descendants("HotelID").FirstOrDefault().Value) || !string.IsNullOrEmpty(req.Descendants("HotelName").FirstOrDefault().Value))
                {

                    string CityId = wbCity.Rows[0]["CityCode"].ToString();
                    var model = new SqlModel()
                    {
                        flag = 2,
                        columnList = "ID_HOTEL as HotelID,NOMBRE_HOTEL as HotelName, ID_CATEGORY as StarRating",
                        table = "tblWelcomeBedsHotelList",
                        filter = "ID_PROVINCE='" + CityId + "' AND NOMBRE_HOTEL LIKE '%" + req.Descendants("HotelName").FirstOrDefault().Value + "%'",
                        SupplierId = SupplierId
                    };
                    if (!string.IsNullOrEmpty(req.Descendants("HotelID").FirstOrDefault().Value))
                    {
                        model.HotelCode = req.Descendants("HotelID").FirstOrDefault().Value;
                        timeOut = 10000;
                    }
                    DataTable htlList = TravayooRepository.GetData(model);

                    if (htlList.Rows.Count > 0)
                    {
                        var result = htlList.AsEnumerable().Select(y => new XElement("productCode", y.Field<string>("HotelID")));
                        htlItem = new XElement("productCodes", result);
                    }
                    else
                    {
                        //throw new Exception("There is no hotel available in database");
                        return null;
                    }
                }
                #endregion hotelname
                XElement PaxProfile = null;
                int sup_cutime = 100000, threadCount = 2;
                List<XElement> OccupancyList = new List<XElement>();
                GetSearchPaxProfile(req.Descendants("RoomPax").ToList(), nationality, out PaxProfile, out OccupancyList);
                int rmCount = -1;
                List<XElement> thr1 = new List<XElement>();
                List<XElement> thr2 = new List<XElement>();
                List<XElement> thr3 = new List<XElement>();
                List<XElement> thr4 = new List<XElement>();
                List<XElement> thr5 = new List<XElement>();
                List<XElement> thr6 = new List<XElement>();

                List<Thread> threadlist;
                

                int Number = splitList.Count;
                bool checkod = true;
                if (Number % 2 == 0)
                {
                    checkod = false;
                }
                else
                {
                    Number = Number + 1;
                }
                int count = 1;
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                if (splitList.Count() == 1)
                {
                    List<XElement> hotelCodeElement = callHotelElement(splitList[0], "5");
                    threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic, wbHotels)),
                                };
                    threadlist.ForEach(t => t.Start());
                    threadlist.ForEach(t => t.Join(timeOutsup));
                    threadlist.ForEach(t => t.Abort());
                    try
                    {
                        XElement res = new XElement("Hotels", thr1);
                        foreach (XElement hotel in res.Descendants("Hotel"))
                            HotelsList.Add(hotel);
                    }
                    catch { }
                    timeOutsup = timeOutsup - Convert.ToInt32(timer.ElapsedMilliseconds);
                }
                else
                {
                    for (int i = 0; i < Number / 2; i += 2)
                    {
                        if (timeOutsup >= 2)
                        {
                            if (checkod == true && i == Number - 1)
                            {
                                List<XElement> hotelCodeElement = callHotelElement(splitList[i], "2");
                                threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic, wbHotels)),
                                };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOutsup));
                                threadlist.ForEach(t => t.Abort());
                                try
                                {
                                    XElement res = new XElement("Hotels", thr1);
                                    foreach (XElement hotel in res.Descendants("Hotel"))
                                        HotelsList.Add(hotel);
                                }
                                catch { }
                            }
                            else
                            {
                                List<XElement> hotelCodeElement = callHotelElement(splitList[i], "2");
                                List<XElement> hotelCodeElement2 = callHotelElement(splitList[i + 1], "2");
                                threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement,wbCity , wbStatic, wbHotels)),
                                     new Thread(()=> thr2 = SearchHotel(req,PaxProfile,OccupancyList ,rmCount,htlItem,hotelCodeElement2,wbCity , wbStatic, wbHotels)),
                                };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOutsup));
                                threadlist.ForEach(t => t.Abort());
                                if (thr1 != null)
                                {
                                    try
                                    {
                                        XElement res = new XElement("Hotels", thr1);
                                        foreach (XElement hotel in res.Descendants("Hotel"))
                                            HotelsList.Add(hotel);
                                    }
                                    catch { }
                                }
                                if (thr2 != null)
                                {
                                    try
                                    {
                                        XElement res = new XElement("Hotels", thr2);
                                        foreach (XElement hotel in res.Descendants("Hotel"))
                                            HotelsList.Add(hotel);
                                    }
                                    catch { }
                                }
                            }
                            timeOutsup = timeOutsup - Convert.ToInt32(timer.ElapsedMilliseconds);
                        }
                    }
                }

                return HotelsList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<XElement> SearchHotel(XElement req, XElement PaxProfile, List<XElement> OccupancyList, int rmCount, XElement htlItem, List<XElement> hotelCodeElement, DataTable wbCity, WBStaticData wbStatic, DataTable wbHotels)
        {
            List<XElement> HotelsList = new List<XElement>();
            XElement searchRequest = new XElement(soapenv + "Envelope",
                  new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                  new XAttribute(XNamespace.Xmlns + "ns0", "http://www.opentravel.org/OTA/2003/05"),
                  new XElement(soapenv + "Header"),
                  new XElement(soapenv + "Body",
                      new XElement(ns + "OTA_HotelAvailRQ", new XAttribute("Version", "1"),
                      new XElement(ns + "AvailRequestSegments",
                         new XElement(ns + "AvailRequestSegment",
                         new XElement(ns + "StayDateRange", new XAttribute("Start", req.Descendants("FromDate").FirstOrDefault().Value.WBDateString()), new XAttribute("End", req.Descendants("ToDate").FirstOrDefault().Value.WBDateString())),
                         PaxProfile,
                         new XElement(ns + "RoomStayCandidates",
                          from rpax in req.Descendants("RoomPax")
                          select new XElement(ns + "RoomStayCandidate",
                          OccupancyList[++rmCount]
                         )),
                         (htlItem != null) ?
                          new XElement(ns + "HotelSearchCriteria", new XElement(ns + "Criterion", from htl in htlItem.Descendants("productCode") select new XElement(ns + "HotelRef", new XAttribute("HotelCode", htl.Value))))
                          : new XElement(ns + "HotelSearchCriteria",
                              new XElement(ns + "Criterion", hotelCodeElement)),

                        new XElement(ns + "TPA_Extensions",
                          new XElement(ns + "Providers",
                          new XElement(ns + "Provider", new XAttribute("Provider", "GSI"),
                          new XElement(ns + "Credentials",
                              new XElement(ns + "Credential", new XAttribute("CredentialCode", AccountCode), new XAttribute("CredentialName", "AccountCode")),
                              new XElement(ns + "Credential", new XAttribute("CredentialCode", Password), new XAttribute("CredentialName", "Password")),
                              new XElement(ns + "Credential", new XAttribute("CredentialCode", System), new XAttribute("CredentialName", "System")),
                              new XElement(ns + "Credential", new XAttribute("CredentialCode", SalesChannel), new XAttribute("CredentialName", "SalesChannel")),
                              new XElement(ns + "Credential", new XAttribute("CredentialCode", Language), new XAttribute("CredentialName", "Language")),
                              new XElement(ns + "Credential", new XAttribute("CredentialCode", ConnectionString), new XAttribute("CredentialName", "ConnectionString"))),


                           new XElement(ns + "ProviderAreas",
                               new XElement(ns + "Area", new XAttribute("TypeCode", "Country"), new XAttribute("AreaCode", wbCity.Rows[0]["CountryCode"].ToString())),
                               new XElement(ns + "Area", new XAttribute("TypeCode", "Province"), new XAttribute("AreaCode", wbCity.Rows[0]["CityCode"].ToString())),
                               new XElement(ns + "Area", new XAttribute("TypeCode", "Town"), new XAttribute("AreaCode", wbCity.Rows[0]["TownCode"].ToString())))


                           )),
                           new XElement(ns + "ProviderTokens", new XElement(ns + "Token", new XAttribute("TokenName", "ResponseMode"), new XAttribute("TokenCode", "4")))
               ))))));


            LogModel logmodel = new LogModel()
            {
                TrackNo = req.Descendants("TransID").FirstOrDefault().Value,
                CustomerID = customerid.ModifyToLong(),
                SuplId = SupplierId,
                LogType = "Search",
                LogTypeID = 1
            };

            XElement respHTL = srvRequest.ServerRequestHotelSearch(searchRequest.ToString(), Url, logmodel, timeOut);

            if (respHTL.Descendants("RoomStay").Count() == 0)
            {
                return null;
            }
            else
            {
                //DataTable wbHotels = wbStatic.GetWBStaticHotels(wbCity.Rows[0]["CityCode"].ToString(), wbCity.Rows[0]["CountryCode"].ToString(), wbCity.Rows[0]["TownCode"].ToString(), req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt(), req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt());
                string xmlouttype = string.Empty;
                try
                {
                    if (dmc == "WelcomeBeds")
                    {
                        xmlouttype = "false";
                    }
                    else
                    { xmlouttype = "true"; }
                }
                catch { }
                var missingHid = "";
                foreach (var hotel in respHTL.Descendants("RoomStay").ToList())
                {
                    try
                    {
                        var htlcode = hotel.Descendants("BasicPropertyInfo").FirstOrDefault().Attribute("HotelCode").Value;
                        var HotelData = wbHotels.Select("[HotelId] = '" + hotel.Descendants("BasicPropertyInfo").FirstOrDefault().Attribute("HotelCode").Value + "'").FirstOrDefault();
                        if (HotelData != null)
                        {
                            int htlStarRating = hotel.Descendants("HotelInfo").FirstOrDefault().Descendants("CategoryCode").FirstOrDefault().Attribute("Code").Value.ModifyToInt();
                            int minRating = req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt();
                            int maxRating = req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt();
                            if (minRating <= htlStarRating && htlStarRating <= maxRating)
                            {
                                decimal minrate = 0;
                                foreach (var room in hotel.Descendants("RoomRate").Where(x => x.Attribute("InvBlockCode").Value == "1"))
                                {
                                    minrate = minrate + room.Descendants("Rate").FirstOrDefault().Descendants("Total").FirstOrDefault().Attribute("AmountAfterTax").Value.ModifyToDecimal();
                                }
                                XElement hoteldata = null;
                                try
                                {
                                    hoteldata = new XElement("Hotel", new XElement("HotelID", HotelData["HotelId"].ToString()),
                                                    new XElement("HotelName", HotelData["HotelName"].ToString()),
                                                    new XElement("PropertyTypeName"),
                                                    new XElement("CountryID"),
                                                    new XElement("CountryName", req.Descendants("CountryName").FirstOrDefault().Value),
                                                    new XElement("CountryCode", HotelData["CountryId"].ToString()),
                                                    new XElement("CityId"),
                                                    new XElement("CityCode", HotelData["CityId"].ToString()),
                                                    new XElement("CityName", req.Descendants("CityName").FirstOrDefault().Value),
                                                    new XElement("AreaId"),
                                                    new XElement("AreaName", HotelData["Area"].ToString()),
                                                    new XElement("RequestID", ""),
                                                    new XElement("Address", HotelData["Address"].ToString()),
                                                    new XElement("Location", ""),
                                                    new XElement("Description"),
                                                    new XElement("StarRating", hotel.Descendants("HotelInfo").FirstOrDefault().Descendants("CategoryCode").FirstOrDefault().Attribute("Code").Value),
                                                    new XElement("MinRate", minrate),
                                                    new XElement("HotelImgSmall", hotel.Descendants("Photo").Count() > 0 ? hotel.Descendants("Photo").FirstOrDefault().Attribute("URL").Value : HotelData["Img"]),
                                                    new XElement("HotelImgLarge", hotel.Descendants("Photo").Count() > 0 ? hotel.Descendants("Photo").FirstOrDefault().Attribute("URL").Value : HotelData["Img"]),
                                                    new XElement("MapLink"),
                                                    new XElement("Longitude", HotelData["Longitude"].ToString()),
                                                    new XElement("Latitude", HotelData["Latitude"].ToString()),
                                                    new XElement("xmloutcustid", customerid),
                                                    new XElement("xmlouttype", xmlouttype),
                                                    new XElement("DMC", dmc),
                                                    new XElement("SupplierID", SupplierId),
                                                    new XElement("Currency", hotel.Descendants("RoomRate").FirstOrDefault().Descendants("Rate").FirstOrDefault().Descendants("Total").FirstOrDefault().Attribute("CurrencyCode").Value),
                                                    new XElement("Offers"),
                                                    new XElement("Facilities"),
                                                    new XElement("Rooms")
                                                    );
                                    HotelsList.Add(hoteldata);
                                }
                                catch (Exception ex)
                                {
                                    //var TEXT = ex.Message;
                                    //WriteToLogFile("hoteldata Exception - " + htlcode + ",");
                                }




                            }

                        }
                        else
                        {
                            //  WriteToLogFile("Missing Id-" + htlcode + ",");
                            missingHid += htlcode + ",";
                        }
                    }
                    catch { }

                }
                //WriteToLogFile("WelcomeBeds missing hotel_Id-" + missingHid);
            }
            return HotelsList;
        }


        public List<XElement> callHotelElement(List<string> splitList, string minstarrating)
        {
            List<XElement> _temp = new List<XElement>();
          
            foreach(var h in splitList)
            {
                    XElement _element = new XElement(ns + "HotelRef", new XAttribute("HotelCode", h), new XAttribute("SegmentCategoryCode", minstarrating));
                    _temp.Add(_element);
            }
            return _temp;
        }


        public void GetSearchPaxProfile(List<XElement> PaxList, string nationalitycode, out XElement PaxProfile, out List<XElement> OccupancyList)
        {
            OccupancyList = new List<XElement>();
            try
            {
                List<XElement> profileList = new List<XElement>();
                int cnt = 1;
                foreach (var roompax in PaxList)
                {
                    XElement RoomOcp = new XElement(ns + "GuestCounts");
                    for (int i = 0; i < roompax.Element("Adult").Value.ModifyToInt(); i++)
                    {
                        XElement waProfile = new XElement(ns + "ProfileInfo",
                             new XElement(ns + "Profile", new XAttribute("RPH",cnt),
                             new XElement(ns+"Customer", new XElement(ns+"Address", new XElement(ns+"CountryName", new XAttribute("Code", nationalitycode))),
                             new XElement(ns + "CitizenCountryName", new XAttribute("Code", nationalitycode)))));
                        profileList.Add(waProfile);
                        RoomOcp.Add(new XElement(ns + "GuestCount", new XAttribute("Count", "1"), new XAttribute("Age", "30"), new XAttribute("ResGuestRPH",cnt)));
                        cnt++;
                    }
                    foreach (var child in roompax.Descendants("ChildAge").ToList())
                    {
                        XElement wcProfile = new XElement(ns + "ProfileInfo",
                             new XElement(ns + "Profile", new XAttribute("RPH", cnt),
                             new XElement("Customer", new XElement(ns + "Address", new XElement(ns + "CountryName", new XAttribute("Code",nationalitycode))),
                             new XElement(ns + "CitizenCountryName", new XAttribute("Code", nationalitycode)))));
                        
                        profileList.Add(wcProfile);
                        RoomOcp.Add(new XElement(ns + "GuestCount", new XAttribute("Count", "1"), new XAttribute("Age", child.Value), new XAttribute("ResGuestRPH", cnt)));
               
                        cnt++;
                    }
                    OccupancyList.Add(RoomOcp);
                }
                PaxProfile = new XElement(ns + "Profiles", profileList);
            }
            catch
            {
                PaxProfile = new XElement(ns + "Profiles");
                foreach (var roompax in PaxList)
                {
                    OccupancyList.Add(new XElement(ns + "GuestCounts"));
                }
            }


        }
       
        #endregion

        #region Hotel Description
        public XElement HotelDescription(XElement req)
        {
            XElement hotelDesc = new XElement("Hotels");
            XElement HotelDescReq = req.Descendants("hoteldescRequest").FirstOrDefault();
            XElement hotelDescResdoc = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", req.Descendants("AgentID").Single().Value), new XElement("UserName", req.Descendants("UserName").Single().Value),
                                       new XElement("Password", req.Descendants("Password").Single().Value), new XElement("ServiceType", req.Descendants("ServiceType").Single().Value), new XElement("ServiceVersion", req.Descendants("ServiceVersion").Single().Value))));

            try
            {
                WBStaticData wbStatic = new WBStaticData();
                DataTable HotelDetail = wbStatic.GetWBHotelDetails(req.Descendants("HotelID").FirstOrDefault().Value);
                if (!string.IsNullOrEmpty(HotelDetail.Rows[0]["HotelId"].ToString()))
                {
                    string hoteldetails = string.IsNullOrEmpty(HotelDetail.Rows[0]["Details"].ToString()) ? HotelDetail.Rows[0]["HotelName"].ToString().replaceAscii() : HotelDetail.Rows[0]["Details"].ToString().replaceAscii();

                    hotelDescResdoc.Add(new XElement(soapenv + "Body", HotelDescReq, new XElement("hoteldescResponse", new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", req.Descendants("HotelID").FirstOrDefault().Value),
                                        new XElement("Description", hoteldetails), HotelDetail.Rows[0]["Images"].ToString().getHotelImages(), HotelDetail.Rows[0]["Facilities"].ToString().getHotelFacilities(),
                                        new XElement("ContactDetails", new XElement("Phone", HotelDetail.Rows[0]["PHONE"].ToString()), new XElement("Fax", HotelDetail.Rows[0]["FAX"].ToString())),
                                        new XElement("CheckinTime", HotelDetail.Rows[0]["CHECKIN"].ToString()), new XElement("CheckoutTime", HotelDetail.Rows[0]["CHECKOUT"].ToString())
                                        )))));
                }
                return hotelDescResdoc;
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelDescription";
                ex1.PageName = "WelcomeBeds";
                ex1.CustomerID = req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = req.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                hotelDescResdoc.Add(new XElement(soapenv + "Body", HotelDescReq, new XElement("hoteldescResponse", new XElement("Hotels"))));
                return hotelDescResdoc;
            }
        }
       #endregion

        #region Room Availability
        public XElement GetRoomAvail_welcomebedsOUT(XElement req)
        {
            List<XElement> roomavailabilityresponse = new List<XElement>();
            XElement getrm = null;
            try
            {
               
                string dmc = string.Empty;
                string HotelID = Convert.ToString(req.Descendants("searchRequest").Elements("HotelID").FirstOrDefault().Value);
                List<XElement> htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "44" && x.Attribute("GHtlID").Value == HotelID).GroupBy(x=> x.Value).Select(x=> x.First()).ToList();
                for (int i = 0; i < htlele.Count(); i++)
                {
                    string custID = string.Empty;
                    string custName = string.Empty;
                    string htlid = htlele[i].Attribute("GHtlID").Value;
                    string xmlout = string.Empty;
                    try
                    {
                        xmlout = htlele[i].Attribute("xmlout").Value;
                    }
                    catch { xmlout = "false"; }
                    if (xmlout == "true")
                    {
                        try
                        {
                            customerid = htlele[i].Attribute("custID").Value;
                            dmc = htlele[i].Attribute("custName").Value;
                        }
                        catch { custName = "HA"; }
                    }
                    else
                    {
                        try
                        {
                            customerid = htlele[i].Attribute("custID").Value;
                        }
                        catch { }
                        dmc = "WelcomeBeds";
                    }
                    WelcomebedsService rs = new WelcomebedsService(customerid);
                    roomavailabilityresponse.Add(rs.RoomAvailability(req, dmc, htlid,customerid));
                }
                
                getrm = new XElement("TotalRooms", roomavailabilityresponse);
                return getrm;
            }
            catch { return null; }
        }

        public XElement RoomAvailability(XElement roomReq, string xtype, string htlid, string custid)
        {
            dmc = xtype;
            XElement searchReq = roomReq.Descendants("searchRequest").FirstOrDefault();
            XElement RoomDetails = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                   new XElement("Authentication", new XElement("AgentID", roomReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", roomReq.Descendants("UserName").FirstOrDefault().Value), new XElement("Password", roomReq.Descendants("Password").FirstOrDefault().Value),
                                   new XElement("ServiceType", roomReq.Descendants("ServiceType").FirstOrDefault().Value), new XElement("ServiceVersion", roomReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                srvRequest = new WBRequest();
                customerid = custid;
                XmlDocument SoapReq = new XmlDocument();
                string nationality = roomReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value;
                XElement PaxProfile = null;
                List<XElement> OccupancyList = new List<XElement>();
                GetSearchPaxProfile(roomReq.Descendants("RoomPax").ToList(), nationality, out PaxProfile, out OccupancyList);
                int rmCount = -1;


                XElement searchRequest = new XElement(soapenv + "Envelope",
                     new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                     new XAttribute(XNamespace.Xmlns + "ns0", "http://www.opentravel.org/OTA/2003/05"),
                     new XElement(soapenv + "Header"),
                     new XElement(soapenv + "Body",
                         new XElement(ns + "OTA_HotelAvailRQ", new XAttribute("Version", "1"),
                         new XElement(ns + "AvailRequestSegments",
                            new XElement(ns + "AvailRequestSegment",
                            new XElement(ns + "StayDateRange", new XAttribute("Start", roomReq.Descendants("FromDate").FirstOrDefault().Value.WBDateString()), new XAttribute("End", roomReq.Descendants("ToDate").FirstOrDefault().Value.WBDateString())),
                            PaxProfile,
                            new XElement(ns + "RoomStayCandidates",
                             from rpax in roomReq.Descendants("RoomPax")
                             select new XElement(ns + "RoomStayCandidate",
                             OccupancyList[++rmCount]
                            )),
                         new XElement(ns + "HotelSearchCriteria", new XElement(ns + "Criterion",
                             new XElement(ns + "HotelRef", new XAttribute("HotelCode", htlid)))),
                         new XElement(ns + "TPA_Extensions",
                             new XElement(ns + "Providers",
                             new XElement(ns + "Provider", new XAttribute("Provider", "GSI"),
                             new XElement(ns + "Credentials",
                                 new XElement(ns + "Credential", new XAttribute("CredentialCode", AccountCode), new XAttribute("CredentialName", "AccountCode")),
                                 new XElement(ns + "Credential", new XAttribute("CredentialCode", Password), new XAttribute("CredentialName", "Password")),
                                 new XElement(ns + "Credential", new XAttribute("CredentialCode", System), new XAttribute("CredentialName", "System")),
                                 new XElement(ns + "Credential", new XAttribute("CredentialCode", SalesChannel), new XAttribute("CredentialName", "SalesChannel")),
                                 new XElement(ns + "Credential", new XAttribute("CredentialCode", Language), new XAttribute("CredentialName", "Language")),
                                 new XElement(ns + "Credential", new XAttribute("CredentialCode", ConnectionString), new XAttribute("CredentialName", "ConnectionString"))),
                              new XElement(ns + "ProviderAreas","")
                              )),
                              new XElement(ns + "ProviderTokens", new XElement(ns + "Token", new XAttribute("TokenName", "ResponseMode"), new XAttribute("TokenCode", "4")))
                  ))))));

                LogModel logmodel = new LogModel()
                {
                    TrackNo = roomReq.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = customerid.ModifyToLong(),
                    SuplId = SupplierId,
                    LogType = "RoomAvail",
                    LogTypeID = 2
                };
                XElement respRoomAvail = srvRequest.ServerRequest(searchRequest.ToString(), Url, logmodel);
                if (respRoomAvail.Descendants("RoomStay").Count() > 0)
                {
                    foreach (var RoomAvail in respRoomAvail.Descendants("RoomStay"))
                    {
                        int totalrooms = roomReq.Descendants("RoomPax").Count();
                        XElement groupDetails = new XElement("Rooms");
                        List<XElement> roomlst = new List<XElement>();

                        int nights = (int)(roomReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - roomReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;
                        int grpIndex = 1;

                        //int tgroups = respRoomAvail.Descendants("RoomRate").LastOrDefault().Attribute("InvBlockCode").Value.ModifyToInt();
                        int tgroups = RoomAvail.Descendants("RoomRate").LastOrDefault().Attribute("InvBlockCode").Value.ModifyToInt();
                        for (int i = 1; i <= tgroups; i++)
                        {
                            //var grpRmLst = respRoomAvail.Descendants("RoomRate").Where(x => x.Attribute("InvBlockCode").Value == i.ToString()).ToList();
                            var grpRmLst = RoomAvail.Descendants("RoomRate").Where(x => x.Attribute("InvBlockCode").Value == i.ToString()).ToList();
                            string currencycode = grpRmLst.FirstOrDefault().Descendants("Total").FirstOrDefault().Attribute("CurrencyCode").Value;
                            decimal TotalRoomRate = 0;
                            int roomSeq = 1;
                            List<XElement> roomDtlList = new List<XElement>();
                            foreach (var room in grpRmLst)
                            {
                                var st = room.Descendants("Total").FirstOrDefault().Attribute("AmountAfterTax").Value;
                                decimal roomPrice = room.Descendants("Total").FirstOrDefault().Attribute("AmountAfterTax").Value.ModifyToDecimal();
                                TotalRoomRate = TotalRoomRate + roomPrice;
                                string roomName = RoomAvail.Descendants("RoomType").Where(x => x.Attribute("RoomTypeCode").Value == room.Attribute("RoomTypeCode").Value).Descendants("RoomDescription").FirstOrDefault().Attribute("Name").Value;
                                string BoardName = RoomAvail.Descendants("RatePlan").Where(x => x.Attribute("RatePlanCode").Value == room.Attribute("RatePlanCode").Value).Descendants("MealsIncluded").FirstOrDefault().Attribute("MealPlanCodes").Value.Split('-')[1];
                                string promotion = RoomAvail.Descendants("RatePlan").Where(x => x.Attribute("RatePlanCode").Value == room.Attribute("RatePlanCode").Value).FirstOrDefault().Attribute("RatePlanName") != null ? RoomAvail.Descendants("RatePlan").Where(x => x.Attribute("RatePlanCode").Value == room.Attribute("RatePlanCode").Value).FirstOrDefault().Attribute("RatePlanName").Value : "";
                                roomDtlList.Add(new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeCode").Value), new XAttribute("SuppliersID", SupplierId), new XAttribute("RoomSeq", roomSeq), new XAttribute("SessionID", room.Attribute("RatePlanCode").Value), new XAttribute("RoomType", roomName), new XAttribute("OccupancyID", room.Attribute("InvBlockCode").Value),
                                                           new XAttribute("OccupancyName", ""), new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", BoardName), new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", roomPrice / nights),
                                                           new XAttribute("TotalRoomRate", roomPrice), new XAttribute("CancellationDate", ""), new XAttribute("CancellationAmount", ""), new XAttribute("isAvailable", true),
                                                           new XElement("RequestID", room.Descendants("RoomToken").FirstOrDefault().Attribute("Token").Value), new XElement("Offers"), new XElement("PromotionList", new XElement("Promotions", promotion)),
                                                           new XElement("CancellationPolicy"), new XElement("Amenities", new XElement("Amenity")),
                                                           new XElement("Images", new XElement("Image", new XAttribute("Path", ""))), new XElement("Supplements"),
                                                           new XElement(getPriceBreakup(nights, roomPrice)),
                                                           new XElement("AdultNum", room.Descendants("GuestCount").Where(x => x.Attribute("Age").Value == "30").Count()),
                                                           new XElement("ChildNum", room.Descendants("GuestCount").Where(x => x.Attribute("Age").Value != "30").Count())));


                                roomSeq++;
                            }
                            XElement RoomType = new XElement("RoomTypes", new XAttribute("Index", grpIndex), new XAttribute("TotalRate", TotalRoomRate), new XAttribute("HtlCode", htlid), new XAttribute("CrncyCode", currencycode), new XAttribute("DMCType", dmc));
                            RoomType.Add(roomDtlList);
                            roomlst.Add(RoomType);
                            grpIndex++;
                        }

                        groupDetails.Add(roomlst);
                    

                    XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID"), new XElement("HotelName"), new XElement("PropertyTypeName"),
                                       new XElement("CountryID"), new XElement("CountryName"), new XElement("CityCode"), new XElement("CityName"),
                                       new XElement("AreaId"), new XElement("AreaName"), new XElement("RequestID"), new XElement("Address"), new XElement("Location"),
                                       new XElement("Description"), new XElement("StarRating"), new XElement("MinRate"), new XElement("HotelImgSmall"),
                                       new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("Longitude"), new XElement("Latitude"), new XElement("DMC"),
                                       new XElement("SupplierID"), new XElement("Currency"), new XElement("Offers"),
                                       new XElement(groupDetails)));
                    RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", hoteldata)));
         
                }
                }
            
                {
                    RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"))));

                }

                return RoomDetails;
            }
            catch(Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "RoomAvailability";
                ex1.PageName = "WelcomeBeds";
                ex1.CustomerID = roomReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = roomReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"))));


                return RoomDetails;
            }
        }

        #region Price Breakup
        private XElement getPriceBreakup(int nights, decimal roomPrice)
        {
            XElement pricebrk = new XElement("PriceBreakups");
            decimal nightPrice = Math.Round(roomPrice / nights, 5);
            for (int i = 1; i <= nights; i++)
            {
                pricebrk.Add(new XElement("Price", new XAttribute("Night", i), new XAttribute("PriceValue", nightPrice)));

            }
            return pricebrk;
        }
        #endregion
        #endregion
        #region Cancellation Policy
        public XElement CancellationPolicy(XElement cxlPolicyReq)
        {
            XElement CxlPolicyReqest = cxlPolicyReq.Descendants("hotelcancelpolicyrequest").FirstOrDefault();
            XElement CxlPolicyResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", cxlPolicyReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cxlPolicyReq.Descendants("UserName").FirstOrDefault().Value),
                                       new XElement("Password", cxlPolicyReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cxlPolicyReq.Descendants("ServiceType").FirstOrDefault().Value),
                                       new XElement("ServiceVersion", cxlPolicyReq.Descendants("ServiceVersion").FirstOrDefault().Value))));


            try
            {
               
                WBStaticData wbStatic = new WBStaticData();
                XElement logXMl = null;
                List<XElement> roomlist = null;
                int nights = (int)(cxlPolicyReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - cxlPolicyReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;
                decimal RoomGroupRate = cxlPolicyReq.Descendants("TotalRoomRate").FirstOrDefault().Value.ModifyToDecimal();
                string grpCode = cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("OccupancyID").Value;
                try
                {
                    string GetLog = wbStatic.GetWBLog(cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value, 2, SupplierId);
                        
                    if (!string.IsNullOrEmpty(GetLog))
                    {
                        logXMl = XElement.Parse(GetLog);
                    }
                    if (logXMl != null)
                    {
                        roomlist = logXMl.Descendants("RoomRate").Where(x => x.Attribute("InvBlockCode").Value == grpCode).ToList();
                    }
                }
                catch
                {
                    roomlist = null;

                }
                
                CxlPolicyResponse.Add(new XElement(soapenv + "Body", cxlPolicyReq, new XElement("HotelDetailwithcancellationResponse",
                        new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", cxlPolicyReq.Descendants("HotelID").FirstOrDefault().Value),
                        new XElement("HotelName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"),
                        new XElement("DMC", "WelcomeBeds"), new XElement("Currency"), new XElement("Offers"),
                        new XElement("Rooms", new XElement("Room", new XAttribute("ID", cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("ID").Value),
                        new XAttribute("RoomType", ""), new XAttribute("PerNightRoomRate", cxlPolicyReq.Descendants("PerNightRoomRate").FirstOrDefault().Value),
                        new XAttribute("TotalRoomRate", cxlPolicyReq.Descendants("TotalRoomRate").FirstOrDefault().Value),
                        new XAttribute("LastCancellationDate", ""),
                        GetRoomCxlPolicy(roomlist, cxlPolicyReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate(),RoomGroupRate)
                        )))))));
                return CxlPolicyResponse;
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "CancellationPolicy";
                ex1.PageName = "WelcomeBeds";
                ex1.CustomerID = cxlPolicyReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                CxlPolicyResponse.Add(new XElement(soapenv + "Body", CxlPolicyReqest, new XElement("HotelDetailwithcancellationResponse", new XElement("ErrorTxt", "No cancellation policy found"))));
                return CxlPolicyResponse;

            }

           
        }
        #region cxl Policy
       
        private XElement GetRoomCxlPolicy(List<XElement> roomList, DateTime CheckInDate, decimal TotalPrice)
        {

            Dictionary<DateTime, decimal> cxlPolicies = new Dictionary<DateTime, decimal>();
            DateTime lastCxldate = DateTime.MaxValue.Date;
            string policyTxt = string.Empty;
            try
            {
                if (roomList == null)
                {
                    DateTime Cxldate;
                    Cxldate = DateTime.Now.Date;
                    if (Cxldate.AddDays(-1) < lastCxldate)
                    {
                        lastCxldate = Cxldate.AddDays(-1);
                    }
                    cxlPolicies.Add(Cxldate, TotalPrice);
                }
                else
                {
                    foreach (var room in roomList)
                    {
                        decimal roomPrice = room.Descendants("Total").FirstOrDefault().Attribute("AmountAfterTax").Value.ModifyToDecimal();
                        if (room.Descendants("CancelPenalty").Where(x => x.Descendants("AmountPercent").Count() > 0).Count() == 0)
                        {
                            DateTime Cxldate;
                            Cxldate = DateTime.Now.Date;
                            if (Cxldate.AddDays(-1) < lastCxldate)
                            {
                                lastCxldate = Cxldate.AddDays(-1);
                            }
                            cxlPolicies.AddCxlPolicy(Cxldate, roomPrice);
                        }
                        else
                        {
                            List<XElement> policyList = room.Descendants("CancelPenalty").Where(x => x.Descendants("AmountPercent").Count() > 0).ToList();
                            foreach (var policy in policyList)
                            {
                                decimal cxlCharges = 0.0m;
                                DateTime Cxldate;

                                Cxldate = policy.Attribute("Start").Value.ModifyToCxlDate();
                                if (Cxldate.AddDays(-1) < lastCxldate)
                                {
                                    lastCxldate = Cxldate.AddDays(-1);
                                }
                                cxlCharges = policy.Descendants("AmountPercent").FirstOrDefault().Attribute("Amount").Value.ModifyToDecimal();
                                cxlPolicies.AddCxlPolicy(Cxldate, cxlCharges);
                            }
                        }
                    }
                }
                cxlPolicies.Add(lastCxldate, 0);
                XElement cxlplcy = new XElement("CancellationPolicies", from polc in cxlPolicies.OrderBy(k => k.Key) select new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", polc.Key.ToString("dd'/'MM'/'yyyy")), new XAttribute("ApplicableAmount", polc.Value), new XAttribute("NoShowPolicy", polc.Key == CheckInDate ? "1" : "0")));
                return cxlplcy;
            }
            catch (Exception ex)
            {
                cxlPolicies.Clear();
                DateTime Cxldate;
                Cxldate = DateTime.Now.Date;
                if (Cxldate.AddDays(-1) < lastCxldate)
                {
                    lastCxldate = Cxldate.AddDays(-1);
                }
                cxlPolicies.Add(lastCxldate, 0);
                cxlPolicies.Add(Cxldate, TotalPrice);
                XElement cxlplcy = new XElement("CancellationPolicies", from polc in cxlPolicies.OrderBy(k => k.Key) select new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", polc.Key.ToString("dd'/'MM'/'yyyy")), new XAttribute("ApplicableAmount", polc.Value), new XAttribute("NoShowPolicy", "0")));
                return cxlplcy;
            }
        }
        #endregion
        #endregion
        #region PreBooking
        public XElement PreBooking(XElement preBookReq, string xmlout)
        {
            dmc = xmlout;
            XElement preBookReqest = preBookReq.Descendants("HotelPreBookingRequest").FirstOrDefault();
            XElement PreBookResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", preBookReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", preBookReq.Descendants("UserName").FirstOrDefault().Value),
                                       new XElement("Password", preBookReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", preBookReq.Descendants("ServiceType").FirstOrDefault().Value),
                                       new XElement("ServiceVersion", preBookReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                srvRequest = new WBRequest();
                XElement groupDetails = new XElement("Rooms");
                List<XElement> roomlst = new List<XElement>();
                string termsCondition = string.Empty;
                string nationalityCode = preBookReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value;
                int totalrooms = preBookReq.Descendants("Room").Count();
                int nights = (int)(preBookReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - preBookReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;
                decimal TotalRateOld = preBookReq.Descendants("RoomTypes").FirstOrDefault().Attribute("TotalRate").Value.ModifyToDecimal();
                XElement PaxNaProfile = null;
                List<XElement> PaxTokenList = new List<XElement>();
                GetPaxNationalityProfile(preBookReq.Descendants("RoomPax").ToList(), nationalityCode,out PaxNaProfile, out PaxTokenList);
                int rmcnt = -1;
               
                XElement PreBookRequest = new XElement(soapenv + "Envelope",
                            new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                            new XAttribute(XNamespace.Xmlns + "ns0", "http://www.opentravel.org/OTA/2003/05"),
                            new XElement(soapenv + "Header"),
                            new XElement(soapenv + "Body",
                                new XElement(ns + "OTA_HotelResRQ", new XAttribute("Version", "2"), new XAttribute("ResStatus", "Initiate"),
                                new XElement(ns + "HotelReservations",
                                   new XElement(ns + "HotelReservation",
                                   new XElement(ns + "RoomStays",
                                   new XElement(ns + "RoomStay",
                                       new XElement(ns + "RoomTypes",
                                        from rpax in preBookReq.Descendants("Room")
                                        select new XElement(ns + "RoomType", new XAttribute("RoomTypeCode",rpax.Attribute("ID").Value))),
                                         new XElement(ns + "RatePlans",
                                             new XElement(ns + "RatePlan", new XAttribute("RatePlanCode", preBookReq.Descendants("Room").FirstOrDefault().Attribute("SessionID").Value))),
                                         new XElement(ns + "RoomRates",
                                          from rm in preBookReq.Descendants("Room")
                                          select new XElement(ns + "RoomRate",
                                              new XAttribute("RoomTypeCode", rm.Attribute("ID").Value), new XAttribute("RatePlanCode", rm.Attribute("SessionID").Value), new XAttribute("InvBlockCode", rm.Attribute("OccupancyID").Value), new XAttribute("AvailabilityStatus", "AvailableForSale"),
                                              new XElement(ns+"Rates",  new XElement(ns+"Rate",new XElement(ns+"Total", new XAttribute("AmountAfterTax",rm.Attribute("TotalRoomRate").Value), new XAttribute("CurrencyCode",preBookReq.Descendants("CurrencyName").FirstOrDefault().Value)),
                                              new XElement(ns+"TPA_Extensions",
                                                 PaxTokenList[++rmcnt],
                                                 new XElement(ns + "RoomToken", new XAttribute("Token", rm.Descendants("RequestID").FirstOrDefault() .Value))))),
                                              new XElement(ns+"GuestCounts",GetPreOccupnacy(rm.Attribute("Adult").Value.ModifyToInt(),rm.Attribute("ChildAge").Value )))),
                                              new XElement(ns+"TimeSpan",new XAttribute("Start", preBookReq.Descendants("FromDate").FirstOrDefault().Value.WBDateString()), new XAttribute("End", preBookReq.Descendants("ToDate").FirstOrDefault().Value.WBDateString())),
                                              new XElement(ns+"BasicPropertyInfo", new XAttribute("HotelCode",preBookReq.Descendants("RoomTypes").FirstOrDefault().Attribute("HtlCode").Value)))),
                                    PaxNaProfile,
                                    new XElement(ns + "TPA_Extensions",
                                    new XElement(ns + "Providers",
                                    new XElement(ns + "Provider", new XAttribute("Provider", "GSI"),
                                    new XElement(ns + "Credentials",
                                        new XElement(ns + "Credential", new XAttribute("CredentialCode", AccountCode), new XAttribute("CredentialName", "AccountCode")),
                                        new XElement(ns + "Credential", new XAttribute("CredentialCode", Password), new XAttribute("CredentialName", "Password")),
                                        new XElement(ns + "Credential", new XAttribute("CredentialCode", System), new XAttribute("CredentialName", "System")),
                                        new XElement(ns + "Credential", new XAttribute("CredentialCode", SalesChannel), new XAttribute("CredentialName", "SalesChannel")),
                                        new XElement(ns + "Credential", new XAttribute("CredentialCode", Language), new XAttribute("CredentialName", "Language")),
                                        new XElement(ns + "Credential", new XAttribute("CredentialCode", ConnectionString), new XAttribute("CredentialName", "ConnectionString"))),
                                        new XElement(ns + "ProviderAreas","")
                                        //new XElement(ns + "ProviderAreas",
                                        // new XElement(ns + "Area", new XAttribute("TypeCode", "Country"), new XAttribute("AreaCode", "AE")),
                                         //new XElement(ns + "Area", new XAttribute("TypeCode", "Province"), new XAttribute("AreaCode", "DXB")))
                                     )),
                                     new XElement(ns+"ProviderID", new XAttribute("Provider","GSI")),
                                     new XElement(ns + "ProviderTokens", new XElement(ns + "Token", new XAttribute("TokenName", "ExternalProvider"), new XAttribute("TokenCode", "1"))))
                              )))));

                LogModel logmodel = new LogModel()
                {
                    TrackNo = preBookReq.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = preBookReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = SupplierId,
                    LogType = "PreBook",
                    LogTypeID = 4
                };
                XElement respPreBook = srvRequest.ServerRequest(PreBookRequest.ToString(), Url, logmodel);
                if (respPreBook.Descendants("RoomStay").Count() > 0)
                {
                    string reservationID = respPreBook.Descendants("HotelReservationID").FirstOrDefault().Attribute("ResID_Value").Value;
                    string currencycode = respPreBook.Descendants("RoomRate").FirstOrDefault().Descendants("Total").FirstOrDefault().Attribute("CurrencyCode").Value;
               
                    decimal TotalRoomRate = 0;
                    int roomSeq = 1;
                   
                    List<XElement> roomDtlList = new List<XElement>();
                    foreach (var preRoom in respPreBook.Descendants("RoomRate"))
                    {
                        decimal roomPrice = preRoom.Descendants("Total").FirstOrDefault().Attribute("AmountAfterTax").Value.ModifyToDecimal();
                        TotalRoomRate = TotalRoomRate + roomPrice;
                        string roomName = respPreBook.Descendants("RoomType").Where(x => x.Attribute("RoomTypeCode").Value == preRoom.Attribute("RoomTypeCode").Value).Descendants("RoomDescription").FirstOrDefault().Attribute("Name").Value;
                        string BoardName = respPreBook.Descendants("RatePlan").Where(x => x.Attribute("RatePlanCode").Value == preRoom.Attribute("RatePlanCode").Value).Descendants("MealsIncluded").FirstOrDefault().Attribute("MealPlanCodes").Value.Split('-')[1];
                        string promotion = respPreBook.Descendants("RatePlan").Where(x => x.Attribute("RatePlanCode").Value == preRoom.Attribute("RatePlanCode").Value).FirstOrDefault().Attribute("RatePlanName")!=null?respPreBook.Descendants("RatePlan").Where(x => x.Attribute("RatePlanCode").Value == preRoom.Attribute("RatePlanCode").Value).FirstOrDefault().Attribute("RatePlanName").Value:"";
                   
                        roomDtlList.Add(new XElement("Room", new XAttribute("ID", preRoom.Attribute("RoomTypeCode").Value), new XAttribute("SuppliersID", SupplierId), new XAttribute("RoomSeq", roomSeq), new XAttribute("SessionID", preRoom.Attribute("RatePlanCode").Value), new XAttribute("RoomType", roomName), new XAttribute("OccupancyID", ""),
                                                   new XAttribute("OccupancyName", ""), new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", BoardName), new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", roomPrice / nights),
                                                   new XAttribute("TotalRoomRate", roomPrice), new XAttribute("CancellationDate", ""), new XAttribute("CancellationAmount", ""), new XAttribute("isAvailable", true),
                                                   new XElement("RequestID", reservationID), new XElement("Offers"), new XElement("PromotionList", new XElement("Promotions", promotion)),
                                                   new XElement("CancellationPolicy"), new XElement("Amenities", new XElement("Amenity")),
                                                   new XElement("Images", new XElement("Image", new XAttribute("Path", ""))), new XElement("Supplements"),
                                                   new XElement(getPriceBreakup(nights, roomPrice)),
                                                   new XElement("AdultNum", preRoom.Descendants("GuestCount").Where(x => x.Attribute("Age").Value == "30").Count()),
                                                   new XElement("ChildNum", preRoom.Descendants("GuestCount").Where(x => x.Attribute("Age").Value != "30").Count())));


                        roomSeq++;
                    }

                    XElement RoomType = new XElement("RoomTypes", new XAttribute("Index", "1"), new XAttribute("TotalRate", TotalRoomRate), roomDtlList);
                    groupDetails.Add(RoomType);
                    if (respPreBook.Descendants("RoomRateDescription").Count() > 0)
                    {
                        foreach (var rem in respPreBook.Descendants("Text").ToList())
                        {
                            if (string.IsNullOrEmpty(termsCondition))
                            {
                                termsCondition = rem.Value;
                            }
                            else
                            {
                                termsCondition += "<br/>" + rem.Value;
                            }
                        }
                    }
                    groupDetails.Descendants("Room").Last().AddAfterSelf(GetCxlPolicy(respPreBook.Descendants("RoomRate").ToList(), preBookReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate(), TotalRoomRate));
                    XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", preBookReq.Descendants("HotelID").FirstOrDefault().Value),
                                                new XElement("HotelName", preBookReq.Descendants("HotelName").FirstOrDefault().Value), new XElement("Status", true),
                                                new XElement("TermCondition", termsCondition), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"),
                                                new XElement("MapLink"), new XElement("DMC", dmc), new XElement("Currency", currencycode),
                                                new XElement("Offers"), groupDetails));
                    if (TotalRateOld == TotalRoomRate)
                    {
                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("NewPrice", ""), hoteldata)));
                    }
                    else
                    {
                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Amount has been changed"), new XElement("NewPrice", TotalRoomRate), hoteldata)));
                    }
                   
                }
                else
                {
                    PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"))));
    
                }


                return PreBookResponse;
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "PreBooking";
                ex1.PageName = "WelcomeBeds";
                ex1.CustomerID = preBookReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = preBookReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"))));
                return PreBookResponse;
            }
        }

        public List<XElement> GetPreOccupnacy(int adults, string childAge)
        {
            List<XElement> OccupancyList = new List<XElement>();
            for (int i = 0; i < adults; i++)
            {
                XElement wbAdults = new XElement(ns + "GuestCount", new XAttribute("Count", "1"), new XAttribute("Age", "30"));
                OccupancyList.Add(wbAdults);
            }
            if (!string.IsNullOrEmpty(childAge))
            {
                foreach (var child in childAge.Split(',').ToList())
                {
                    XElement wbChild = new XElement(ns + "GuestCount", new XAttribute("Count", "1"), new XAttribute("Age", child));
                    OccupancyList.Add(wbChild);
                }
            }
            return OccupancyList;
        }
        public void GetPaxNationalityProfile(List<XElement> PaxList, string nationalitycode, out XElement PaxProfile , out List<XElement> PaxToken)
        {
            PaxToken = new List<XElement>();
            try
            {
                List<XElement> profileList = new List<XElement>();
                int cnt = 1;
                foreach (var roompax in PaxList)
                {
                    XElement RoomPaxToken = new XElement(ns + "ProviderTokens");
                    for (int i = 0; i < roompax.Element("Adult").Value.ModifyToInt(); i++)
                    {
                        XElement waProfile = new XElement(ns + "ResGuest", new XAttribute("Age", "30"),
                            new XElement(ns + "Profiles", new XElement(ns + "ProfileInfo", new XElement(ns + "Profile", new XElement(ns+"Customer",
                                new XElement(ns + "CitizenCountryName", new XAttribute("Code", nationalitycode)),
                                new XElement(ns + "TPA_Extensions", new XElement(ns + "ProviderTokens",
                                    new XElement(ns + "Token", new XAttribute("TokenName", "PaxResidenceCountry"), new XAttribute("TokenCode", nationalitycode)),
                                     new XElement(ns + "Token", new XAttribute("TokenName", "PaxId"), new XAttribute("TokenCode", cnt))
                          )))))));
                        profileList.Add(waProfile);
                        RoomPaxToken.Add(new XElement(ns + "Token", new XAttribute("TokenCode", cnt), new XAttribute("TokenName", "PaxId")));
                        cnt++;
                    }

                    foreach (var child in roompax.Descendants("ChildAge").ToList())
                    {
                        XElement wcProfile = new XElement(ns + "ResGuest", new XAttribute("Age", child.Value),
                        new XElement(ns + "Profiles", new XElement(ns + "ProfileInfo", new XElement(ns + "Profile", new XElement(ns + "Customer",
                            new XElement(ns + "CitizenCountryName", new XAttribute("Code", nationalitycode)),
                            new XElement(ns + "TPA_Extensions", new XElement(ns + "ProviderTokens",
                                new XElement(ns + "Token", new XAttribute("TokenName", "PaxResidenceCountry"), new XAttribute("TokenCode", nationalitycode)),
                                 new XElement(ns + "Token", new XAttribute("TokenName", "PaxId"), new XAttribute("TokenCode", cnt))
                        )))))));
                        profileList.Add(wcProfile);
                        RoomPaxToken.Add(new XElement(ns + "Token", new XAttribute("TokenCode", cnt), new XAttribute("TokenName", "PaxId")));

                        cnt++;
                    }
                    
                    PaxToken.Add(RoomPaxToken);
                }
                PaxProfile = new XElement(ns + "ResGuests", profileList);
            }
            catch
            {
                PaxProfile= new XElement(ns + "ResGuests");
                foreach (var room in PaxList)
                {
                    PaxToken.Add(new XElement(ns + "ProviderTokens"));
                }
            }
            
        
        }
        #region Get Cancellation Policy Element
        private XElement GetCxlPolicy(List<XElement> roomList, DateTime CheckInDate,decimal TotalPrice)
        {
            
            Dictionary<DateTime, decimal> cxlPolicies = new Dictionary<DateTime, decimal>();
            DateTime lastCxldate = DateTime.MaxValue.Date;
            string policyTxt = string.Empty;
            try
            {
                foreach (var room in roomList)
                {
                    decimal roomPrice = room.Descendants("Total").FirstOrDefault().Attribute("AmountAfterTax").Value.ModifyToDecimal();
                    if (room.Descendants("CancelPenalty").Where(x => x.Descendants("AmountPercent").Count() > 0).Count() == 0)
                    {
                        DateTime Cxldate;
                        Cxldate = DateTime.Now.Date;
                        if (Cxldate.AddDays(-1) < lastCxldate)
                        {
                            lastCxldate = Cxldate.AddDays(-1);
                        }
                        cxlPolicies.AddCxlPolicy(Cxldate, roomPrice);
                    }
                    else
                    {
                        List<XElement> policyList = room.Descendants("CancelPenalty").Where(x => x.Descendants("AmountPercent").Count() > 0).ToList();
                        foreach (var policy in policyList)
                        {
                            decimal cxlCharges = 0.0m;
                            DateTime Cxldate;

                            Cxldate = policy.Attribute("Start").Value.WBCxlDate();
                            if (Cxldate.AddDays(-1) < lastCxldate)
                            {
                                lastCxldate = Cxldate.AddDays(-1);
                            }
                            cxlCharges = policy.Descendants("AmountPercent").FirstOrDefault().Attribute("Amount").Value.ModifyToDecimal();
                            cxlPolicies.AddCxlPolicy(Cxldate, cxlCharges);
                        }
                    }
                }
              
                cxlPolicies.Add(lastCxldate, 0);
                XElement cxlplcy = new XElement("CancellationPolicies", from polc in cxlPolicies.OrderBy(k => k.Key) select new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", polc.Key.ToString("dd'/'MM'/'yyyy")), new XAttribute("ApplicableAmount", polc.Value), new XAttribute("NoShowPolicy", polc.Key == CheckInDate ? "1" : "0")));
                return cxlplcy;
            }
            catch (Exception ex)
            {
                cxlPolicies.Clear();
                DateTime Cxldate;
                Cxldate = DateTime.Now.Date;
                if (Cxldate.AddDays(-1) < lastCxldate)
                {
                    lastCxldate = Cxldate.AddDays(-1);
                }
                cxlPolicies.Add(lastCxldate, 0);
                cxlPolicies.Add(Cxldate, TotalPrice);
                XElement cxlplcy = new XElement("CancellationPolicies", from polc in cxlPolicies.OrderBy(k => k.Key) select new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", polc.Key.ToString("dd'/'MM'/'yyyy")), new XAttribute("ApplicableAmount", polc.Value), new XAttribute("NoShowPolicy", "0")));
                return cxlplcy;
            }
        }
        #endregion
        #endregion
        #region HotelBooking
        public XElement HotelBooking(XElement BookingReq)
        {
            XElement BookReq = BookingReq.Descendants("HotelBookingRequest").FirstOrDefault();
            XElement HotelBookingRes = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", BookingReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", BookingReq.Descendants("UserName").FirstOrDefault().Value), new XElement("Password", BookingReq.Descendants("Password").FirstOrDefault().Value),
                                       new XElement("ServiceType", BookingReq.Descendants("ServiceType").FirstOrDefault().Value), new XElement("ServiceVersion", BookingReq.Descendants("ServiceVersion").FirstOrDefault().Value))));
            try
            {
                srvRequest = new WBRequest();
                string soapResult = string.Empty;
                List<XElement> roomlst = new List<XElement>();
                string reservationID = BookingReq.Descendants("Room").FirstOrDefault().Descendants("RequestID").FirstOrDefault().Value;
                string nationality = BookingReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value;
                XElement BookRequest = new XElement(soapenv + "Envelope",
                               new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                               new XAttribute(XNamespace.Xmlns + "ns0", "http://www.opentravel.org/OTA/2003/05"),
                               new XElement(soapenv + "Header"),
                               new XElement(soapenv + "Body",
                                   new XElement(ns + "OTA_HotelResRQ", new XAttribute("ResStatus", "Commit"),
                                   new XElement(ns + "HotelReservations",
                                      new XElement(ns + "HotelReservation",
                                      new XElement(ns + "RoomStays",
                                      new XElement(ns + "RoomStay",
                                          new XElement(ns + "Comments",
                                              new XElement(ns + "Comment", new XAttribute("CommentOriginatorCode","C1")),
                                              new XElement(ns + "Comment", new XAttribute("CommentOriginatorCode","C3"))))),
                                        new XElement(ns+"ResGuests",
                                            from pax in BookingReq.Descendants("PaxInfo")
                                            select new XElement(ns + "ResGuest", new XAttribute("Age", pax.Descendants("Age").FirstOrDefault().Value == "0" ? "30" : pax.Descendants("Age").FirstOrDefault().Value),
                                                new XElement(ns + "Profiles", new XElement(ns + "ProfileInfo", new XElement(ns + "Profile",
                                                    new XElement(ns + "Customer", new XAttribute("Gender", pax.Descendants("Title").FirstOrDefault().Value.WBGender()),
                                                        new XElement(ns + "PersonName",
                                                            new XElement(ns + "GivenName", pax.Descendants("FirstName").FirstOrDefault().Value),
                                                             new XElement(ns + "Surname", pax.Descendants("LastName").FirstOrDefault().Value)),
                                                             new XElement(ns+"CitizenCountryName",new XAttribute("Code",nationality)),
                                                             new XElement(ns + "Document",""),
                                                             new XElement(ns + "TPA_Extensions",
                                                               new XElement(ns + "ProviderTokens", new XElement(ns + "Token", new XAttribute("TokenCode", pax.Descendants("Age").FirstOrDefault().Value == "0" ? "ADULT" : "CHILD"), new XAttribute("TokenName", "PaxType")),
                                                                   new XElement(ns + "Token", new XAttribute("TokenName", "PaxResidenceCountry"), new XAttribute("TokenCode",nationality))
                                                                   )  
                                                     ))))))),
                                        new XElement(ns + "ResGlobalInfo",
                                         new XElement(ns+"Comments",
                                         new XElement(ns + "Comment", new XElement("Text", BookingReq.Descendants("SpecialRemarks").FirstOrDefault().Value))),
                                         new XElement(ns + "HotelReservationIDs",
                                            new XElement(ns + "HotelReservationID", new XAttribute("ResID_Value", reservationID)))),
                                       new XElement(ns + "TPA_Extensions",
                                       new XElement(ns + "Providers",
                                       new XElement(ns + "Provider", new XAttribute("Provider", "GSI"),
                                       new XElement(ns + "Credentials",
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", AccountCode), new XAttribute("CredentialName", "AccountCode")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", Password), new XAttribute("CredentialName", "Password")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", System), new XAttribute("CredentialName", "System")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", SalesChannel), new XAttribute("CredentialName", "SalesChannel")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", Language), new XAttribute("CredentialName", "Language")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", ConnectionString), new XAttribute("CredentialName", "ConnectionString"))))),

                                        new XElement(ns + "ProviderTokens", new XElement(ns + "Token", new XAttribute("TokenCode", BookingReq.Descendants("TransactionID").FirstOrDefault().Value), new XAttribute("TokenName", "RecordId")),
                                            new XElement(ns + "Token", new XAttribute("TokenCode", AgentName), new XAttribute("TokenName", "TravelAgentName"))),
                                            new XElement(ns + "ProviderID", new XAttribute("Provider", "GSI")))
                                 )))));

                LogModel logmodel = new LogModel()
                {
                    TrackNo = BookingReq.Descendants("TransactionID").FirstOrDefault().Value, 
                    CustomerID = BookingReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = SupplierId,
                    LogType = "Book",
                    LogTypeID = 5
                };
                XElement respBook = srvRequest.ServerRequest(BookRequest.ToString(), Url, logmodel);
                if (respBook == null)
                {
                    HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", "No response from supplier!"))));
                }
                else
                {
                    int errCnt = respBook.Descendants("Error").Count();
                    int warCnt = respBook.Descendants("Warning").Count();
                    if (errCnt > 0)
                    {
                        HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", respBook.Descendants("Error").FirstOrDefault().Attribute("Tag").Value))));
                    }
                    else
                    {
                        string bookStatus = respBook.Descendants("HotelReservation").FirstOrDefault().Attribute("ResStatus").Value;
                        if (bookStatus == "Confirmed services" || bookStatus == "Confirmed by Agency")
                        {
                            decimal amount = 0.0m;
                            try
                            {
                                amount = respBook.Descendants("RoomStay").FirstOrDefault().Element("Total").Attribute("AmountAfterTax").Value.ModifyToDecimal();
                            }
                            catch
                            {
                                amount = BookingReq.Descendants("TotalAmount").FirstOrDefault().Value.ModifyToDecimal();
                            }
                            XElement BookingRes = new XElement("HotelBookingResponse",
                                                   new XElement("Hotels", new XElement("HotelID", BookingReq.Descendants("HotelID").FirstOrDefault().Value),
                                                   new XElement("HotelName", BookingReq.Descendants("HotelName").FirstOrDefault().Value),
                                                   new XElement("FromDate", BookingReq.Descendants("FromDate").FirstOrDefault().Value),
                                                   new XElement("ToDate", BookingReq.Descendants("ToDate").FirstOrDefault().Value),
                                                   new XElement("AdultPax", BookingReq.Descendants("Rooms").Descendants("RoomPax").Descendants("Adult").FirstOrDefault().Value),
                                                   new XElement("ChildPax", BookingReq.Descendants("Rooms").Descendants("RoomPax").Descendants("Child").FirstOrDefault().Value),
                                                   new XElement("TotalPrice", amount), new XElement("CurrencyID"),
                                                   new XElement("CurrencyCode", ""),
                                                   new XElement("MarketID"), new XElement("MarketName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("VoucherRemark"),
                                                   new XElement("TransID", BookingReq.Descendants("TransID").FirstOrDefault().Value),
                                                   new XElement("ConfirmationNumber", respBook.Descendants("HotelReservationID").FirstOrDefault().Attribute("ResID_Value").Value),
                                                   new XElement("Status", "Success"),
                                                   new XElement("PassengersDetail", new XElement("GuestDetails",
                                                   from room in BookingReq.Descendants("Room")
                                                   select new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeID").Value), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("ServiceID", ""),
                                                   new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", ""),
                                                   new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", ""),
                                                   new XAttribute("RoomStatus", "true"), new XAttribute("TotalRoomRate", ""),
                                                   new XElement("RoomGuest", new XElement("GuestType", "Adult"), new XElement("Title"), new XElement("FirstName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("FirstName").FirstOrDefault().Value),
                                                   new XElement("MiddleName"), new XElement("LastName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("LastName").FirstOrDefault().Value), new XElement("IsLead", "true"), new XElement("Age")),
                                                   new XElement("Supplements"))
                                                   ))));

                            HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, BookingRes));

                        }
                        else if (bookStatus == "Pending confirmation")
                        {
                            HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", "Booking is on Pending Confirmation!"))));
                        }
                        else
                        {
                            HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", "Booking can not be generated!"))));

                        }


                    }
                }
               return HotelBookingRes;

            }
            catch (Exception ex)
            {
               
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelBooking";
                ex1.PageName = "WelcomeBeds";
                ex1.CustomerID = BookingReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = BookingReq.Descendants("TransactionID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", ex.Message))));
                return HotelBookingRes;

            }
        }

        #region Prebook Cancellation
        public bool PreBookCancellation(XElement BookReq)
        {
            bool flag = false;
            try
            {
                srvRequest = new WBRequest();
                string reservationID = BookReq.Descendants("Room").FirstOrDefault().Descendants("RequestID").FirstOrDefault().Value;

                XElement cancelRequest = new XElement(soapenv + "Envelope",
                               new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                               new XAttribute(XNamespace.Xmlns + "ns0", "http://www.opentravel.org/OTA/2003/05"),
                               new XElement(soapenv + "Header"),
                               new XElement(soapenv + "Body",
                                   new XElement(ns + "OTA_CancelRQ", new XAttribute("Version", "2"), new XAttribute("CancelType", "Ignore"),
                                   new XElement(ns + "POS", new XElement(ns + "Source", new XElement(ns + "BookingChannel", new XAttribute("Type", "TVP")))),
                                   new XElement(ns + "UniqueID", new XAttribute("ID", reservationID)),
                                       new XElement(ns + "TPA_Extensions",
                                       new XElement(ns + "Providers",
                                       new XElement(ns + "Provider", new XAttribute("Provider", "GSI"),
                                       new XElement(ns + "Credentials",
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", AccountCode), new XAttribute("CredentialName", "AccountCode")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", Password), new XAttribute("CredentialName", "Password")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", System), new XAttribute("CredentialName", "System")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", SalesChannel), new XAttribute("CredentialName", "SalesChannel")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", Language), new XAttribute("CredentialName", "Language")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", ConnectionString), new XAttribute("CredentialName", "ConnectionString"))))),
                                           new XElement(ns + "ProviderID", new XAttribute("Provider", "GSI")))
                                 )));

                LogModel logmodel = new LogModel()
                {
                    TrackNo = BookReq.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = BookReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = SupplierId,
                    LogType = "Book",
                    LogTypeID = 5
                };
                XElement resPreBokCancel = srvRequest.ServerRequest(cancelRequest.ToString(), Url, logmodel);
                int errCnt = resPreBokCancel.Descendants("Error").Count();
                if (errCnt > 0)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = true;
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "PreBookCancellation";
                ex1.PageName = "WelcomeBeds";
                ex1.CustomerID = BookReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = BookReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                
            }
            return flag;
        }
        #endregion
        #endregion

        #region Booking Cancellation
        public XElement BookingCancellation(XElement cancelReq)
        {
            XElement CxlReq = cancelReq.Descendants("HotelCancellationRequest").FirstOrDefault();
            XElement BookCXlRes = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                  new XElement("Authentication", new XElement("AgentID", cancelReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cancelReq.Descendants("UserName").FirstOrDefault().Value),
                                  new XElement("Password", cancelReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cancelReq.Descendants("ServiceType").FirstOrDefault().Value),
                                  new XElement("ServiceVersion", cancelReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                srvRequest = new WBRequest();

                XElement cancelRequest = new XElement(soapenv + "Envelope",
                               new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                               new XAttribute(XNamespace.Xmlns + "ns0", "http://www.opentravel.org/OTA/2003/05"),
                               new XElement(soapenv + "Header"),
                               new XElement(soapenv + "Body",
                                   new XElement(ns + "OTA_CancelRQ", new XAttribute("Version", "2"), new XAttribute("CancelType","Cancel"),
                                   new XElement(ns+"POS", new XElement(ns+"Source",new XElement(ns+"BookingChannel", new XAttribute("Type","TVP")))),
                                   new XElement(ns + "UniqueID", new XAttribute("ID", cancelReq.Descendants("ConfirmationNumber").FirstOrDefault().Value)),
                                       new XElement(ns + "TPA_Extensions",
                                       new XElement(ns + "Providers",
                                       new XElement(ns + "Provider", new XAttribute("Provider", "GSI"),
                                       new XElement(ns + "Credentials",
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", AccountCode), new XAttribute("CredentialName", "AccountCode")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", Password), new XAttribute("CredentialName", "Password")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", System), new XAttribute("CredentialName", "System")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", SalesChannel), new XAttribute("CredentialName", "SalesChannel")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", Language), new XAttribute("CredentialName", "Language")),
                                           new XElement(ns + "Credential", new XAttribute("CredentialCode", ConnectionString), new XAttribute("CredentialName", "ConnectionString"))))),
                                           new XElement(ns + "ProviderID", new XAttribute("Provider", "GSI")))
                                 )));

                LogModel logmodel = new LogModel()
                {
                    TrackNo = cancelReq.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = cancelReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = SupplierId,
                    LogType = "Cancel",
                    LogTypeID = 6
                };
                XElement resBokCancel = srvRequest.ServerRequest(cancelRequest.ToString(), Url, logmodel);
                int errCnt = resBokCancel.Descendants("Error").Count();
                if (errCnt > 0)
                {
                    BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", resBokCancel.Descendants("Message").FirstOrDefault().Value))));
                    return BookCXlRes;
                }
                else
                {
                    decimal amount = 0.0m;
                    try
                    {
                        amount = resBokCancel.Descendants("CancelRule").FirstOrDefault().Attribute("Amount").Value.ModifyToDecimal();
                    }
                    catch
                    {
                        amount = 0.0m;
                    }
                    BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("Rooms", new XElement("Room", new XElement("Cancellation", new XElement("Amount", amount), new XElement("Status", resBokCancel.Descendants("OTA_CancelRS").FirstOrDefault().Attribute("Status").Value == "Cancelled" ? "Success" : "Fail")))))));
                }

                return BookCXlRes;
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "BookingCancellation";
                ex1.PageName = "WelcomeBeds";
                ex1.CustomerID = cancelReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = cancelReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", "There is some technical error"))));
                return BookCXlRes;
            }
        }
        #endregion

        public void WriteToLogFile(string text)
        {
            try
            {
                string _filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string path = Path.Combine(_filePath, @"log.txt");
                //string path = Convert.ToString(HttpContext.Current.Server.MapPath(@"~\log.txt"));
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine("------------------------------------------------------------------------------");
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    //writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine("---------------------------Hotel Logs-----------------------------------------");
                    writer.Close();
                }
            }
            catch (Exception ex) { }
        }
    }
}