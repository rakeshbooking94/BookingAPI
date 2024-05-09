using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Models;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class WTEService : IDisposable
    {
        #region Credentails
        string wteToken = string.Empty;
        string wteCurrency = string.Empty;
        string wtesearch = string.Empty;
        string wteroom = string.Empty;
        string wterecheck = string.Empty;
        string wteprebook = string.Empty;
        string wtebook = string.Empty;
        string wtebookdetail = string.Empty;
        string wtecancelcharge = string.Empty;
        string wtecancel = string.Empty;
        string wteipaddress = string.Empty;
        #endregion
        #region Global vars
        string customerid = string.Empty;
        string dmc = string.Empty;
        const int supplierid = 48;
        int chunksize = 200;
        int sup_cutime = 20, threadCount = 2;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        WTERequest wteRequest;
        XNamespace xmlns = "http://schemas.xmlsoap.org/soap/envelope/";
        #endregion
        public WTEService(string _customerid)
        {
            XElement suppliercred = supplier_Cred.getsupplier_credentials(_customerid, "48");
            try
            {
                wteToken = suppliercred.Descendants("Token").FirstOrDefault().Value;
                wteCurrency = suppliercred.Descendants("Currency").FirstOrDefault().Value;
                wtesearch = suppliercred.Descendants("search").FirstOrDefault().Value;
                wteroom = suppliercred.Descendants("room").FirstOrDefault().Value;
                wterecheck = suppliercred.Descendants("recheck").FirstOrDefault().Value;
                wteprebook = suppliercred.Descendants("prebook").FirstOrDefault().Value;
                wtebook = suppliercred.Descendants("book").FirstOrDefault().Value;
                wtebookdetail = suppliercred.Descendants("bookdetail").FirstOrDefault().Value;
                wtecancelcharge = suppliercred.Descendants("cancelcharge").FirstOrDefault().Value;
                wtecancel = suppliercred.Descendants("cancel").FirstOrDefault().Value;
                wteipaddress = suppliercred.Descendants("ipaddress").FirstOrDefault().Value;
            }
            catch { }
        }
        public WTEService()
        {
             
        }
        #region Hotel Availability
        public List<XElement> HotelAvailability(XElement req, string xtype, string custID)
        {
            dmc = xtype;
            customerid = custID;
            string htlCode = string.Empty;
            List<XElement> HotelsList = new List<XElement>();
            try
            {
                WTEStatic wteStatic = new WTEStatic();
                #region get cut off time
                try
                {
                    sup_cutime = supplier_Cred.secondcutoff_time(req.Descendants("HotelID").FirstOrDefault().Value);
                }
                catch { }

                int timeOut = sup_cutime;
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                #endregion
                DataTable wteCity = wteStatic.GetCityCode(req.Descendants("CityID").FirstOrDefault().Value, req.Descendants("CountryID").FirstOrDefault().Value, 48);
                bool isCityMapped = true;
                string destinationcity = string.Empty;
                string countryCode = string.Empty;
                if (wteCity != null)
                {
                    if (wteCity.Rows.Count != 0)
                    {
                        destinationcity = wteCity.Rows[0]["CityID"].ToString();
                        countryCode = wteCity.Rows[0]["CountryCode"].ToString();
                    }
                    else
                    {
                        isCityMapped = false;
                    }
                }
                DataTable wteCountry = wteStatic.GetCountryCode(req.Descendants("PaxNationality_CountryID").FirstOrDefault().Value, 48);
                string paxnationality = string.Empty;
                if (wteCountry != null)
                {
                    if (wteCountry.Rows.Count != 0)
                    {
                        paxnationality = wteCountry.Rows[0]["PaxNationality"].ToString();
                    }
                }
                DataTable wteHotels = wteStatic.GetStaticHotels(req.Descendants("HotelID").FirstOrDefault().Value, req.Descendants("HotelName").FirstOrDefault().Value, destinationcity, countryCode, req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt(), req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt());

                if (wteHotels == null || wteHotels.Rows.Count == 0)
                {
                    #region No hotel found exception
                    CustomException ex1 = new CustomException("There is no hotel available in database");
                    ex1.MethodName = "HotelAvailability";
                    ex1.PageName = "WTEService";
                    ex1.CustomerID = customerid.ToString();
                    ex1.TranID = req.Descendants("TransID").First().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    #endregion
                    return null;
                }
                var statiProperties = wteHotels.AsEnumerable().Select(r => r.Field<string>("hotelcode")).ToList();
                List<List<string>> splitList = statiProperties.SplitPropertyList(chunksize);
                try
                {
                    #region Thread Initialize
                    List<XElement> thr1 = new List<XElement>();
                    List<XElement> thr2 = new List<XElement>();
                    List<XElement> thr3 = new List<XElement>();
                    List<XElement> thr4 = new List<XElement>();
                    List<XElement> thr5 = new List<XElement>();
                    List<XElement> thr6 = new List<XElement>();

                    List<Thread> threadlist;

                    for (int i = 0; i < splitList.Count(); i += 2)
                    {
                        if (timeOut >= 1)
                        {
                            int rangecount = threadCount;
                            if (splitList.Count - i < threadCount)
                                rangecount = splitList.Count - i;

                            #region rangecount equals 1
                            if (rangecount == 1)
                            {
                                threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], wteHotels,timeOut,paxnationality)),
                                   
                                };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOut));
                                threadlist.ForEach(t => t.Abort());
                                HotelsList.AddRange(thr1);
                            }
                            #endregion
                            #region rangecount equals 2
                            else if (rangecount == 2)
                            {
                                threadlist = new List<Thread>
                                {   
                                   new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], wteHotels,timeOut,paxnationality)),
                                   new Thread(()=> thr2 =  SearchHotel(req,countryCode, splitList[i+1], wteHotels,timeOut,paxnationality)),
                                  
                                };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOut));
                                threadlist.ForEach(t => t.Abort());
                                HotelsList.AddRange(thr1.Concat(thr2));

                            }
                            #endregion
                            #region rangecount equals 3
                            else if (rangecount == 3)
                            {
                                threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], wteHotels,timeOut,paxnationality )),
                                    new Thread(()=> thr2 = SearchHotel(req,countryCode, splitList[i+1], wteHotels,timeOut,paxnationality)),
                                    new Thread(()=> thr3 = SearchHotel(req,countryCode, splitList[i+2], wteHotels,timeOut,paxnationality)),
                                      
                                };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOut));
                                threadlist.ForEach(t => t.Abort());
                                HotelsList.AddRange(thr1.Concat(thr2).Concat(thr3));
                            }
                            #endregion
                            #region rangecount equals 4
                            else if (rangecount == 4)
                            {
                                threadlist = new List<Thread>
                                {   
                                    new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], wteHotels,timeOut,paxnationality )),
                                    new Thread(()=> thr2 = SearchHotel(req,countryCode, splitList[i+1], wteHotels,timeOut,paxnationality)),
                                    new Thread(()=> thr3 = SearchHotel(req,countryCode, splitList[i+2], wteHotels,timeOut,paxnationality)),
                                     new Thread(()=> thr4 = SearchHotel(req,countryCode, splitList[i+2], wteHotels,timeOut,paxnationality)),
                                      
                                };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOut));
                                threadlist.ForEach(t => t.Abort());
                                HotelsList.AddRange(thr1.Concat(thr2).Concat(thr3).Concat(thr4));
                            }
                            #endregion
                            HotelsList = HotelsList.OrderBy(x => x.Descendants("MinRate").FirstOrDefault().Value).ToList();
                            timeOut = timeOut - Convert.ToInt32(timer.ElapsedMilliseconds);
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    #region Exception
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "HotelAvailability";
                    ex1.PageName = "WTEService";
                    ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                    ex1.TranID = req.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    #endregion
                }

                return HotelsList;

            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelAvailability";
                ex1.PageName = "WTEService";
                ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                ex1.TranID = req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion
                return null;
            }
        }
        #endregion
        #region thread search result
        private List<XElement> SearchHotel(XElement req, string countryCode, List<string> propertyChunk, DataTable wteStaticHotels, int timeout,string paxnationality)
        {
            List<XElement> ThHotelsList = new List<XElement>();
            try
            {
                wteRequest = new WTERequest();
                var result_hotelids = string.Join(",", propertyChunk);
                DateTime fromDate = DateTime.ParseExact(req.Descendants("FromDate").Single().Value, "dd/MM/yyyy", null);
                DateTime toDate = DateTime.ParseExact(req.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                int nights = (int)(toDate - fromDate).TotalDays;
                wte_request_search wterequs = new wte_request_search();
                wterequs.Token = wteToken;
                wte_request_search.wteAdvancedOptions adreq = new wte_request_search.wteAdvancedOptions();
                adreq.Currency = wteCurrency;
                adreq.CustomerIpAddress = wteipaddress;
                wterequs.AdvancedOptions = adreq;
                wte_request_search.wteRequest wteReqobj = new wte_request_search.wteRequest();
                wteReqobj.CountryID = countryCode; // 13668
                wteReqobj.CheckInDate = Convert.ToString(fromDate.ToString("MM-dd-yyyy"));
                wteReqobj.CheckOutDate = Convert.ToString(toDate.ToString("MM-dd-yyyy"));
                wteReqobj.NoofNights = Convert.ToString(nights);
                wteReqobj.Nationality = paxnationality;

                wte_request_search.wteFilters objflt = new wte_request_search.wteFilters();
                objflt.IsRecommendedOnly = "0";
                objflt.IsShowRooms = "0";
                objflt.IsOnlyAvailable = "1";
                objflt.HotelIds = result_hotelids;
                wte_request_search.wteStarRating objstar = new wte_request_search.wteStarRating();
                objstar.Min = req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt();
                objstar.Max = req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt();
                objflt.StarRating = objstar;
                wteReqobj.Filters = objflt;

                List<wte_request_search.wteRooms> objrm_main = new List<wte_request_search.wteRooms>();
                List<XElement> roompax = req.Descendants("RoomPax").ToList();
                for (int i = 0; i < roompax.Count(); i++)
                {
                    wte_request_search.wteRooms objrm = new wte_request_search.wteRooms();
                    objrm.RoomNo = i + 1;
                    objrm.NoofAdults = Convert.ToInt32(roompax[i].Descendants("Adult").FirstOrDefault().Value);
                    objrm.NoOfChild = Convert.ToInt32(roompax[i].Descendants("Child").FirstOrDefault().Value);
                    List<string> childpaxdetails = new List<string>();
                    if (Convert.ToInt32(roompax[i].Descendants("Child").FirstOrDefault().Value) > 0)
                    {
                        List<XElement> childcount = roompax[i].Descendants("ChildAge").ToList();
                        for (int j = 0; j < childcount.Count(); j++)
                        {
                            childpaxdetails.Add(childcount[j].Value);
                        }
                        objrm.ChildAge = childpaxdetails;
                    }
                    else
                    {
                        objrm.ChildAge = childpaxdetails;
                    }
                    objrm_main.Add(objrm);
                }
                wteReqobj.Rooms = objrm_main;
                wterequs.Request = wteReqobj;

                WTERequest wtepost = new WTERequest();
                WTELogModel logmodel = new WTELogModel()
                {
                    TrackNo = req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = customerid.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Search",
                    LogTypeID = 1
                };
                string resp = wteRequest.ServerRequestSearch(JsonConvert.SerializeObject(wterequs), wtesearch, logmodel, timeout);

                if (string.IsNullOrEmpty(resp) || resp == "[]" || resp == "The remote server returned an error: (403) Forbidden." || resp == "The remote server returned an error: (400) Bad Request.")
                {
                    return null;
                }
                else
                {
                    int rumcount = req.Descendants("RoomPax").Count();
                    string xmlouttype = string.Empty;
                    try
                    {
                        if (dmc == "Withinearth")
                        {
                            xmlouttype = "false";
                        }
                        else
                        { xmlouttype = "true"; }
                    }
                    catch { }
                    dynamic hotelList = JsonConvert.DeserializeObject(resp);
                    foreach (var hotel in hotelList.AvailabilityRS.HotelResult)
                    {
                        try
                        {
                            var htlcode = hotel.HotelId.Value;
                            DataRow[] htlSt = wteStaticHotels.Select("[hotelcode] = '" + htlcode + "'");
                            if (htlSt.Length > 0)
                            {
                                var HotelData = htlSt[0];
                                #region get minimum rate
                                decimal minrate = Convert.ToDecimal(hotel.StartPrice.Value) * rumcount;
                                #endregion

                                XElement hoteldata = new XElement("Hotel", new XElement("HotelID", HotelData["hotelcode"].ToString()),
                                                new XElement("HotelName", HotelData["hotelname"].ToString()),
                                                new XElement("PropertyTypeName", ""),
                                                new XElement("CountryID", req.Descendants("CountryID").FirstOrDefault().Value),
                                                new XElement("CountryName", req.Descendants("CountryName").FirstOrDefault().Value),
                                                new XElement("CountryCode", req.Descendants("CountryCode").FirstOrDefault().Value),
                                                new XElement("CityId", req.Descendants("CityID").FirstOrDefault().Value),
                                                new XElement("CityCode", req.Descendants("CityCode").FirstOrDefault().Value),
                                                new XElement("CityName", HotelData["CityName"].ToString()),
                                                new XElement("AreaId"),
                                                new XElement("AreaName", ""),
                                                new XElement("RequestID", ""),
                                                new XElement("Address", HotelData["address"].ToString()),
                                                new XElement("Location", HotelData["address"].ToString()),
                                                new XElement("Description"),
                                                new XElement("StarRating", HotelData["rating"].ToString().ModifyToStar()),
                                                new XElement("MinRate", minrate),
                                                new XElement("HotelImgSmall", HotelData["HotelFrontImage"].ToString()),
                                                new XElement("HotelImgLarge", HotelData["HotelFrontImage"].ToString()),
                                                new XElement("MapLink"),
                                                new XElement("Longitude", HotelData["longitude"].ToString()),
                                                new XElement("Latitude", HotelData["latitude"].ToString()),
                                                new XElement("xmloutcustid", customerid),
                                                new XElement("xmlouttype", xmlouttype),
                                                new XElement("DMC", dmc), new XElement("SupplierID", supplierid),
                                                new XElement("Currency", wteCurrency),
                                                new XElement("Offers", ""), new XElement("Facilities", null),
                                                new XElement("Rooms", "")
                                                );

                                ThHotelsList.Add(hoteldata);


                            }
                        }
                        catch { }
                    }

                }
            }
            catch(Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "SearchHotel";
                ex1.PageName = "WTEService";
                ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                ex1.TranID = req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion
            }
            return ThHotelsList;


        }
        #endregion
        #region Room Availability
        public XElement GetRoomAvail_wteOUT(XElement req, XElement wteRoomsMealPlansfile, int supID)
        {
            List<XElement> roomavailabilityresponse = new List<XElement>();
            XElement getrm = null;
            try
            {
                #region changed
                string dmc = string.Empty;
                List<XElement> htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == Convert.ToString(supID)).ToList();
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
                        if (supID == 48)
                        {
                            dmc = "Withinearth";
                        }
                    }
                    roomavailabilityresponse.Add(RoomAvailability(req, dmc, htlid, wteRoomsMealPlansfile, customerid));
                    if (roomavailabilityresponse.Descendants("Room").ToList().Count == 0)
                    {
                        roomavailabilityresponse.Add(RoomAvailability(req, dmc, htlid, wteRoomsMealPlansfile, customerid));
                    }
                }
                #endregion
                getrm = new XElement("TotalRooms", roomavailabilityresponse);
                return getrm;
            }
            catch { return null; }
        }
        public XElement RoomAvailability(XElement req, string SupplierType, string htlID, XElement wteRoomsMealPlansfile, string custIDs)
        {
            customerid = custIDs;            
            dmc = SupplierType;            
            XElement RoomResponse = new XElement("searchResponse");
            XElement AvailablilityResponse = null;
            try
            {
                #region Credentials
                string username = req.Descendants("UserName").Single().Value;
                string password = req.Descendants("Password").Single().Value;
                string AgentID = req.Descendants("AgentID").Single().Value;
                string ServiceType = req.Descendants("ServiceType").Single().Value;
                string ServiceVersion = req.Descendants("ServiceVersion").Single().Value;
                #endregion
                WTEStatic wteStatic = new WTEStatic();
                DataTable wteCity = wteStatic.GetCityCode(req.Descendants("CityID").FirstOrDefault().Value, req.Descendants("CountryID").FirstOrDefault().Value, 48);
                string countryCode = string.Empty;
                if (wteCity != null)
                {
                    if (wteCity.Rows.Count != 0)
                    {
                        countryCode = wteCity.Rows[0]["CountryCode"].ToString();
                    }
                }
                DataTable wteCountry = wteStatic.GetCountryCode(req.Descendants("PaxNationality_CountryID").FirstOrDefault().Value, 48);
                string paxnationality = string.Empty;
                if (wteCountry != null)
                {
                    if (wteCountry.Rows.Count != 0)
                    {
                        paxnationality = wteCountry.Rows[0]["PaxNationality"].ToString();
                    }
                }
                wteRequest = new WTERequest();
                DateTime fromDate = DateTime.ParseExact(req.Descendants("FromDate").Single().Value, "dd/MM/yyyy", null);
                DateTime toDate = DateTime.ParseExact(req.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                int nights = (int)(toDate - fromDate).TotalDays;
                wte_request_search wterequs = new wte_request_search();
                wterequs.Token = wteToken;
                wte_request_search.wteAdvancedOptions adreq = new wte_request_search.wteAdvancedOptions();
                adreq.Currency = wteCurrency;
                adreq.CustomerIpAddress = wteipaddress;
                wterequs.AdvancedOptions = adreq;
                wte_request_search.wteRequest wteReqobj = new wte_request_search.wteRequest();
                wteReqobj.CountryID = countryCode; // 13063
                wteReqobj.CheckInDate = Convert.ToString(fromDate.ToString("MM-dd-yyyy"));
                wteReqobj.CheckOutDate = Convert.ToString(toDate.ToString("MM-dd-yyyy"));
                wteReqobj.NoofNights = Convert.ToString(nights);
                wteReqobj.Nationality = paxnationality;

                wte_request_search.wteFilters objflt = new wte_request_search.wteFilters();
                objflt.IsRecommendedOnly = "0";
                objflt.IsShowRooms = "1";
                objflt.IsOnlyAvailable = "1";
                objflt.HotelIds = htlID;
                wte_request_search.wteStarRating objstar = new wte_request_search.wteStarRating();
                objstar.Min = req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt();
                objstar.Max = req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt();
                objflt.StarRating = objstar;
                wteReqobj.Filters = objflt;

                List<wte_request_search.wteRooms> objrm_main = new List<wte_request_search.wteRooms>();
                List<XElement> roompax = req.Descendants("RoomPax").ToList();
                for (int i = 0; i < roompax.Count(); i++)
                {
                    wte_request_search.wteRooms objrm = new wte_request_search.wteRooms();
                    objrm.RoomNo = i + 1;
                    objrm.NoofAdults = Convert.ToInt32(roompax[i].Descendants("Adult").FirstOrDefault().Value);
                    objrm.NoOfChild = Convert.ToInt32(roompax[i].Descendants("Child").FirstOrDefault().Value);
                    List<string> childpaxdetails = new List<string>();
                    if (Convert.ToInt32(roompax[i].Descendants("Child").FirstOrDefault().Value) > 0)
                    {
                        List<XElement> childcount = roompax[i].Descendants("ChildAge").ToList();
                        for (int j = 0; j < childcount.Count(); j++)
                        {
                            childpaxdetails.Add(childcount[j].Value);
                        }
                        objrm.ChildAge = childpaxdetails;
                    }
                    else
                    {
                        objrm.ChildAge = childpaxdetails;
                    }
                    objrm_main.Add(objrm);
                }
                wteReqobj.Rooms = objrm_main;
                wterequs.Request = wteReqobj;

                WTERequest wtepost = new WTERequest();
                WTELogModel logmodel = new WTELogModel()
                {
                    TrackNo = req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = custIDs.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "RoomAvail",
                    LogTypeID = 2
                };
                string resp = wteRequest.ServerRequestRoom(JsonConvert.SerializeObject(wterequs), wteroom, logmodel, htlID);

                if (string.IsNullOrEmpty(resp) || resp == "[]" || resp == "The remote server returned an error: (403) Forbidden." || resp == "The remote server returned an error: (400) Bad Request.")
                {
                    return null;
                }
                else
                {
                    List<XElement> strgrp = new List<XElement>();
                    dynamic hotelList = JsonConvert.DeserializeObject(resp);
                    int totrum = roompax.Count;
                    int rumcomindx = 1;
                    string searchkey = hotelList.AvailabilityRS.SearchKey;
                    foreach (var hotel in hotelList.AvailabilityRS.HotelResult[0].HotelOption)
                    {
                        string hotelid = Convert.ToString(hotelList.AvailabilityRS.HotelResult[0].HotelId.Value);
                        try
                        {
                            if (hotel.IsCombineRoom.Value.ToString().ToUpper() == "TRUE")
                            {
                                foreach (var rooms in hotel.HotelRooms)
                                {
                                    List<XElement> str = new List<XElement>();
                                    double totrumprc = 0;
                                    int rmct = 0;
                                    foreach (var room in rooms)
                                    {
                                        for (int i = 0; i < totrum; i++)
                                        {
                                            totrumprc = Convert.ToDouble(room.Price.Value);
                                            if (room.BookingStatus.Value.ToString().ToUpper() == "AVAILABLE")
                                            {
                                                IEnumerable<XElement> totroomprc = null;
                                                totroomprc = GetequalBreakuphWTE(Convert.ToString(room.Price.Value), nights);
                                                string mealcode = string.Empty;
                                                try
                                                {
                                                    XElement mealel = wteRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.MappedMealName.Value).FirstOrDefault();
                                                    mealcode = mealel.Attribute("B2BCode").Value;
                                                }
                                                catch { }
                                                str.Add(new XElement("Room",
                                                         new XAttribute("ID", Convert.ToString(hotel.IsCombineRoom.Value.ToString())),
                                                         new XAttribute("SuppliersID", "48"),
                                                         new XAttribute("RoomSeq", rmct+1),
                                                         new XAttribute("SessionID", Convert.ToString(room.RoomToken.Value)),
                                                         new XAttribute("RoomType", room.RoomTypeName.Value),
                                                         new XAttribute("OccupancyID", Convert.ToString(searchkey)),
                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                         new XAttribute("MealPlanID", Convert.ToString("")),
                                                         new XAttribute("MealPlanName", Convert.ToString(room.MappedMealName.Value)),
                                                         new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                                         new XAttribute("MealPlanPrice", ""),
                                                         new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                         new XAttribute("TotalRoomRate", Convert.ToString(Convert.ToDouble(room.Price.Value) / totrum)),
                                                         new XAttribute("CancellationDate", ""),
                                                         new XAttribute("CancellationAmount", ""),
                                                         new XAttribute("isAvailable", "true"),
                                                         new XElement("RequestID", Convert.ToString(hotel.HotelOptionId.Value.ToString())),
                                                         new XElement("Offers", ""),
                                                         new XElement("PromotionList", ""),
                                                         new XElement("CancellationPolicy", ""),
                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                         new XElement("Supplements", ""),
                                                             new XElement("PriceBreakups", totroomprc),
                                                             new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                                             new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                                         ));
                                            }
                                            rmct++;
                                        }

                                    }
                                    if (totrum == str.ToList().Count())
                                    {
                                        strgrp.Add(new XElement("RoomTypes", new XAttribute("Index", rumcomindx), new XAttribute("TotalRate", Convert.ToString(totrumprc)), new XAttribute("HtlCode", hotelid), new XAttribute("CrncyCode", wteCurrency), new XAttribute("DMCType", dmc), new XAttribute("CUID", customerid),
                                                     str));
                                        rumcomindx++;
                                    }
                                }
                            }
                            else if (hotel.IsCombineRoom.Value.ToString().ToUpper() == "FALSE")
                            {
                                
                                foreach (var rooms in hotel.HotelRooms)
                                {
                                    List<XElement> str = new List<XElement>();
                                    double totrumprc = 0;
                                    int rmct = 0;
                                    foreach (var room in rooms)
                                    {
                                        totrumprc += Convert.ToDouble(room.Price.Value);
                                        if (room.BookingStatus.Value.ToString().ToUpper() == "AVAILABLE")
                                        {
                                            IEnumerable<XElement> totroomprc = null;
                                            totroomprc = GetequalBreakuphWTE(Convert.ToString(room.Price.Value), nights);
                                            string mealcode = string.Empty;
                                            try
                                            {
                                                XElement mealel = wteRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.MappedMealName.Value).FirstOrDefault();
                                                mealcode = mealel.Attribute("B2BCode").Value;
                                            }
                                            catch { }
                                            str.Add(new XElement("Room",
                                                     new XAttribute("ID", Convert.ToString(hotel.IsCombineRoom.Value.ToString())),
                                                     new XAttribute("SuppliersID", "48"),
                                                     new XAttribute("RoomSeq", room.RoomNo.Value),
                                                     new XAttribute("SessionID", Convert.ToString(room.RoomToken.Value)),
                                                     new XAttribute("RoomType", room.RoomTypeName.Value),
                                                     new XAttribute("OccupancyID", Convert.ToString(searchkey)),
                                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                                     new XAttribute("MealPlanID", Convert.ToString("")),
                                                     new XAttribute("MealPlanName", Convert.ToString(room.MappedMealName.Value)),
                                                     new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                                     new XAttribute("MealPlanPrice", ""),
                                                     new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                     new XAttribute("TotalRoomRate", Convert.ToString(room.Price.Value)),
                                                     new XAttribute("CancellationDate", ""),
                                                     new XAttribute("CancellationAmount", ""),
                                                     new XAttribute("isAvailable", "true"),
                                                     new XElement("RequestID", Convert.ToString(hotel.HotelOptionId.Value.ToString())),
                                                     new XElement("Offers", ""),
                                                     new XElement("PromotionList", ""),
                                                     new XElement("CancellationPolicy", ""),
                                                     new XElement("Amenities", new XElement("Amenity", "")),
                                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                     new XElement("Supplements", ""),
                                                         new XElement("PriceBreakups", totroomprc),
                                                         new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                                         new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                                     ));
                                        }
                                        rmct++;
                                    }
                                    if (totrum == str.ToList().Count())
                                    {
                                        strgrp.Add(new XElement("RoomTypes", new XAttribute("Index", rumcomindx), new XAttribute("TotalRate", Convert.ToString(totrumprc)), new XAttribute("HtlCode", hotelid), new XAttribute("CrncyCode", wteCurrency), new XAttribute("DMCType", dmc), new XAttribute("CUID", customerid),
                                                     str));
                                        rumcomindx++;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    AvailablilityResponse = new XElement("Rooms", strgrp);
                }
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "RoomAvailability";
                ex1.PageName = "WTEService";
                ex1.CustomerID = req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = req.Descendants("TransID").FirstOrDefault().Value;
                APILog.SendCustomExcepToDB(ex1);
                #endregion
            }
                
            return AvailablilityResponse;

        }
        private IEnumerable<XElement> GetequalBreakuphWTE(string totalprice, int totalnight)
        {
            #region HotelsPro Room's Price Breakups
            List<XElement> str = new List<XElement>();
            try
            {
                decimal prctotal = Convert.ToDecimal(totalprice);
                decimal nightprc = prctotal / totalnight;
                for (int i = 0; i < totalnight; i++)
                {
                    str.Add(new XElement("Price",
                          new XAttribute("Night", Convert.ToString(Convert.ToInt32(i + 1))),
                          new XAttribute("PriceValue", Convert.ToString(nightprc)))
                   );
                }
            }
            catch { }
            return str;
            #endregion
        }
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
                WTEStatic wteStatic = new WTEStatic();
                int nights = (int)(cxlPolicyReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - cxlPolicyReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;

                DataTable wtePolicy = wteStatic.GetCXLPolicy(cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value, cxlPolicyReq.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "48").FirstOrDefault().Attribute("GHtlID").Value);
                string response = string.Empty;
                string countryCode = string.Empty;
                int iscxl = 0;
                if (wtePolicy != null)
                {
                    if (wtePolicy.Rows.Count != 0)
                    {
                        response = wtePolicy.Rows[0]["logresponseXML"].ToString();
                        response = HttpUtility.UrlDecode(response.ToString());
                        iscxl = 1;
                    }
                }
                if (iscxl == 0)
                {
                    wteRequest = new WTERequest();
                    wte_request_recheck wterequs = new wte_request_recheck();
                    wterequs.Token = wteToken;
                    wte_request_recheck.wteAdvancedOptions adreq = new wte_request_recheck.wteAdvancedOptions();
                    adreq.Currency = wteCurrency;
                    adreq.CustomerIpAddress = wteipaddress;
                    wterequs.AdvancedOptions = adreq;
                    wte_request_recheck.wteRequest wteReqobj = new wte_request_recheck.wteRequest();
                    wteReqobj.SearchKey = cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("OccupancyID").Value;
                    wte_request_recheck.wteHotelOption objrm_main = new wte_request_recheck.wteHotelOption();
                    objrm_main.HotelOptionId = cxlPolicyReq.Descendants("Room").FirstOrDefault().Element("RequestID").Value;
                    List<wte_request_recheck.wteHotelRooms>
                        objrm = new List<wte_request_recheck.wteHotelRooms>();
                    int rumind = 1;
                    if (cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("ID").Value.ToUpper() == "TRUE")
                    {
                        wte_request_recheck.wteHotelRooms objrm1 = new wte_request_recheck.wteHotelRooms();
                        objrm1.RoomNo = Convert.ToString(cxlPolicyReq.Descendants("Room").Count());
                        objrm1.RoomToken = cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("SessionID").Value;
                        objrm.Add(objrm1);
                    }
                    else
                    {
                        foreach (XElement room in cxlPolicyReq.Descendants("Room"))
                        {
                            wte_request_recheck.wteHotelRooms objrm1 = new wte_request_recheck.wteHotelRooms();
                            objrm1.RoomNo = Convert.ToString(rumind);
                            objrm1.RoomToken = room.Attribute("SessionID").Value;
                            objrm.Add(objrm1);
                            rumind++;
                        }
                    }
                    objrm_main.HotelRooms = objrm;
                    wteReqobj.HotelOption = objrm_main;
                    wterequs.Request = wteReqobj;
                    WTELogModel logmodel = new WTELogModel()
                    {
                        TrackNo = cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value,
                        CustomerID = cxlPolicyReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                        SuplId = supplierid,
                        LogType = "ReCheck",
                        LogTypeID = 4
                    };
                    string resp = wteRequest.ServerRequestprecheck(JsonConvert.SerializeObject(wterequs), wterecheck, logmodel);
                    XDocument rechkresdoc = (XDocument)JsonConvert.DeserializeXNode(resp, "recheckhoteldetail");
                    List<XElement> cxlist = new List<XElement>();
                    XElement cp = new XElement("CancellationPolicies");
                    decimal bookingcost = 0;
                    cxlist = rechkresdoc.Descendants("CancellationPolicy").ToList();
                    foreach (XElement policy in cxlist)
                    {
                        if (Convert.ToDecimal(policy.Element("CancellationPrice").Value) != 0)
                        {
                            DateTime toDate = DateTime.ParseExact(policy.Element("FromDate").Value, "MM-dd-yyyy", null);
                            string dt = toDate.ToString("dd/MM/yyyy");
                            decimal cxlprc = Convert.ToDecimal(policy.Element("CancellationPrice").Value);
                            bookingcost = bookingcost + cxlprc;

                            cp.Add(new XElement("CancellationPolicy",
                                                new XAttribute("LastCancellationDate", dt),
                                                new XAttribute("ApplicableAmount", Convert.ToString(Convert.ToDecimal(policy.Element("CancellationPrice").Value))),
                                                new XAttribute("NoShowPolicy", "0")));
                        }
                    }
                    List<XElement> mergeinput = new List<XElement>();
                    mergeinput.Add(cp);
                    XElement finalcp = MergCxlPolicy(mergeinput, bookingcost);

                    XElement _travyoResp;
                    _travyoResp = new XElement("HotelDetailwithcancellationResponse",
                        new XElement("Hotels",
                            new XElement("Hotel",
                                new XElement("HotelID", cxlPolicyReq.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "48").FirstOrDefault().Attribute("GHtlID").Value),
                                new XElement("HotelName", cxlPolicyReq.Descendants("HotelName").First().Value),
                                new XElement("HotelImgSmall", null),
                                new XElement("HotelImgLarge", null),
                                new XElement("MapLink", null),
                                new XElement("DMC", "Withinearth"),
                                new XElement("Currency"),
                                new XElement("Offers"),
                                new XElement("Rooms",
                                    new XElement("Room",
                                    new XAttribute("ID", ""),
                                    new XAttribute("RoomType", ""),
                                    new XAttribute("MealPlanPrice", ""),
                                    new XAttribute("PerNightRoomRate", ""),
                                    new XAttribute("TotalRoomRate", ""),
                                    new XAttribute("CancellationDate", ""), finalcp)))));


                    XElement SearReq = cxlPolicyReq.Descendants("hotelcancelpolicyrequest").FirstOrDefault();
                    SearReq.AddAfterSelf(_travyoResp);

                    return cxlPolicyReq;
                }
                else
                {
                    XDocument resdoc = (XDocument)JsonConvert.DeserializeXNode(response, "hotellist");

                    XElement resp = resdoc.Descendants("HotelOption").Where(x => x.Element("HotelOptionId").Value == cxlPolicyReq.Descendants("Room").FirstOrDefault().Descendants("RequestID").FirstOrDefault().Value).FirstOrDefault();

                    List<XElement> cxlist = new List<XElement>();
                    string IsCombineRoom = string.Empty;
                    IsCombineRoom = resp.Element("IsCombineRoom").Value;
                    int totrum = cxlPolicyReq.Descendants("Room").Count();
                    foreach (XElement room in cxlPolicyReq.Descendants("Room"))
                    {
                        List<XElement> cpo = resp.Descendants("CancellationPolicy").Where(x => x.Parent.Element("RoomToken").Value == room.Attribute("SessionID").Value).ToList();
                        cxlist.AddRange(cpo);
                    }
                    XElement cp = new XElement("CancellationPolicies");
                    decimal bookingcost = 0;
                    foreach (XElement policy in cxlist)
                    {
                        if (Convert.ToDecimal(policy.Element("CancellationPrice").Value) != 0)
                        {
                            DateTime toDate = DateTime.ParseExact(policy.Element("FromDate").Value, "MM-dd-yyyy", null);
                            string dt = toDate.ToString("dd/MM/yyyy");
                            decimal cxlprc = Convert.ToDecimal(policy.Element("CancellationPrice").Value);
                            if (IsCombineRoom.ToUpper() == "TRUE")
                            {
                                cxlprc = cxlprc / totrum;
                                bookingcost = bookingcost + cxlprc;
                            }
                            else
                            {
                                bookingcost = bookingcost + cxlprc;
                            }

                            cp.Add(new XElement("CancellationPolicy",
                                                new XAttribute("LastCancellationDate", dt),
                                                new XAttribute("ApplicableAmount", Convert.ToString(Convert.ToDecimal(policy.Element("CancellationPrice").Value))),
                                                new XAttribute("NoShowPolicy", "0")));
                        }
                    }
                    List<XElement> mergeinput = new List<XElement>();
                    mergeinput.Add(cp);
                    XElement finalcp = MergCxlPolicy(mergeinput, bookingcost);

                    XElement _travyoResp;
                    _travyoResp = new XElement("HotelDetailwithcancellationResponse",
                        new XElement("Hotels",
                            new XElement("Hotel",
                                new XElement("HotelID", cxlPolicyReq.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "48").FirstOrDefault().Attribute("GHtlID").Value),
                                new XElement("HotelName", cxlPolicyReq.Descendants("HotelName").First().Value),
                                new XElement("HotelImgSmall", null),
                                new XElement("HotelImgLarge", null),
                                new XElement("MapLink", null),
                                new XElement("DMC", "Withinearth"),
                                new XElement("Currency"),
                                new XElement("Offers"),
                                new XElement("Rooms",
                                    new XElement("Room",
                                    new XAttribute("ID", ""),
                                    new XAttribute("RoomType", ""),
                                    new XAttribute("MealPlanPrice", ""),
                                    new XAttribute("PerNightRoomRate", ""),
                                    new XAttribute("TotalRoomRate", ""),
                                    new XAttribute("CancellationDate", ""), finalcp)))));


                    XElement SearReq = cxlPolicyReq.Descendants("hotelcancelpolicyrequest").FirstOrDefault();
                    SearReq.AddAfterSelf(_travyoResp);

                    return cxlPolicyReq;
                }
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "CancellationPolicy";
                ex1.PageName = "WTEService";
                ex1.CustomerID = cxlPolicyReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                CxlPolicyResponse.Add(new XElement(soapenv + "Body", CxlPolicyReqest, new XElement("HotelDetailwithcancellationResponse", new XElement("ErrorTxt", "No cancellation policy found"))));
                #endregion
                return CxlPolicyResponse;

            }


        }
        public XElement MergCxlPolicy(List<XElement> rooms, decimal bookingcost)
        {
            List<XElement> cxlList = new List<XElement>();

            IEnumerable<XElement> dateLst = rooms.Descendants("CancellationPolicy").
               GroupBy(r => new { r.Attribute("LastCancellationDate").Value, noshow = r.Attribute("NoShowPolicy").Value }).Select(y => y.First()).
               OrderBy(p => DateTime.ParseExact(p.Attribute("LastCancellationDate").Value, "dd/MM/yyyy", CultureInfo.InvariantCulture));
               //OrderBy(p => p.Attribute("LastCancellationDate").Value);

            if (dateLst.Count() > 0)
            {

                foreach (var item in dateLst)
                {
                    string date = item.Attribute("LastCancellationDate").Value;
                    string noShow = item.Attribute("NoShowPolicy").Value;
                    decimal datePrice = 0.0m;
                    foreach (var rm in rooms.Descendants("CancellationPolicy"))
                    {

                        if (rm.Attribute("NoShowPolicy").Value == noShow && rm.Attribute("LastCancellationDate").Value == date)
                        {

                            var price = rm.Attribute("ApplicableAmount").Value;
                            datePrice += Convert.ToDecimal(price);
                            if (bookingcost < datePrice)
                            {
                                datePrice = bookingcost;
                            }
                        }
                        else
                        {
                            if (noShow == "1")
                            {
                                datePrice += Convert.ToDecimal(rm.Attribute("ApplicableAmount").Value);
                                if (bookingcost < datePrice)
                                {
                                    datePrice = bookingcost;
                                }
                            }
                            else
                            {


                                var lastItem = rm.Descendants("CancellationPolicy").
                                    Where(pq => (pq.Attribute("NoShowPolicy").Value == noShow && Convert.ToDateTime(pq.Attribute("LastCancellationDate").Value) < chnagetoTime(date)));

                                if (lastItem.Count() > 0)
                                {
                                    var lastDate = lastItem.Max(y => y.Attribute("LastCancellationDate").Value);
                                    var lastprice = rm.Descendants("CancellationPolicy").
                                        Where(pq => (pq.Attribute("NoShowPolicy").Value == noShow && pq.Attribute("LastCancellationDate").Value == lastDate)).
                                        FirstOrDefault().Attribute("ApplicableAmount").Value;
                                    datePrice += Convert.ToDecimal(lastprice);
                                    if (bookingcost < datePrice)
                                    {
                                        datePrice = bookingcost;
                                    }
                                }

                            }

                        }
                    }
                    XElement pItem = new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", date), new XAttribute("ApplicableAmount", datePrice), new XAttribute("NoShowPolicy", noShow));
                    cxlList.Add(pItem);

                }

                //cxlList = cxlList.GroupBy(x => new { DateTime.ParseExact(x.Attribute("LastCancellationDate").Value, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date, x.Attribute("NoShowPolicy").Value }).
                //    Select(y => new XElement("CancellationPolicy",
                //        new XAttribute("LastCancellationDate", y.Key.Date.ToString("dd/MM/yyyy")),
                //        new XAttribute("ApplicableAmount", y.Max(p => Convert.ToDecimal(p.Attribute("ApplicableAmount").Value))),
                //        new XAttribute("NoShowPolicy", y.Key.Value))).OrderBy(p => p.Attribute("LastCancellationDate").Value).ToList();

                cxlList = cxlList.GroupBy(x => new { DateTime.ParseExact(x.Attribute("LastCancellationDate").Value, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date, x.Attribute("NoShowPolicy").Value }).
                    Select(y => new XElement("CancellationPolicy",
                        new XAttribute("LastCancellationDate", y.Key.Date.ToString("dd/MM/yyyy")),
                        new XAttribute("ApplicableAmount", y.Max(p => Convert.ToDecimal(p.Attribute("ApplicableAmount").Value))),
                        new XAttribute("NoShowPolicy", y.Key.Value))).OrderBy(p => DateTime.ParseExact(p.Attribute("LastCancellationDate").Value, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();


                var fItem = cxlList.FirstOrDefault();

                if (Convert.ToDecimal(fItem.Attribute("ApplicableAmount").Value) != 0.0m)
                {
                    cxlList.Insert(0, new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", fItem.Attribute("LastCancellationDate").Value.GetDateTime("dd/MM/yyyy").AddDays(-1).Date.ToString("dd/MM/yyyy")), new XAttribute("ApplicableAmount", "0.00"), new XAttribute("NoShowPolicy", "0")));

                }
            }

            XElement cxlItem = new XElement("CancellationPolicies", cxlList);
            return cxlItem;

        }
        public DateTime chnagetoTime(string strDate)
        {
            DateTime oDate = DateTime.ParseExact(strDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return oDate;

        }
        #endregion
        #region Prebook
        public XElement PreBook(XElement Req, string xmlout)
        {
            //string json = File.ReadAllText(Server.MapPath("~/files/myfile.json"));
            //string allText = System.IO.File.ReadAllText(@"C:\IngBackup\BE_Project\BE_HotelService\BE_hotel_Service\XML_OUT_WhiteLabelAPIHotel\res.json");
            dmc = xmlout;
            XElement Response = null;
            #region Credentials
            string username = Req.Descendants("UserName").Single().Value;
            string password = Req.Descendants("Password").Single().Value;
            string AgentID = Req.Descendants("AgentID").Single().Value;
            string ServiceType = Req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = Req.Descendants("ServiceVersion").Single().Value;
            #endregion
            try
            {
                wteRequest = new WTERequest();
                wte_request_recheck wterequs = new wte_request_recheck();
                wterequs.Token = wteToken;
                wte_request_recheck.wteAdvancedOptions adreq = new wte_request_recheck.wteAdvancedOptions();
                adreq.Currency = wteCurrency;
                adreq.CustomerIpAddress = wteipaddress;
                wterequs.AdvancedOptions = adreq;
                wte_request_recheck.wteRequest wteReqobj = new wte_request_recheck.wteRequest();
                wteReqobj.SearchKey = Req.Descendants("Room").FirstOrDefault().Attribute("OccupancyID").Value;
                wte_request_recheck.wteHotelOption objrm_main = new wte_request_recheck.wteHotelOption();
                objrm_main.HotelOptionId = Req.Descendants("Room").FirstOrDefault().Element("RequestID").Value;
                List<wte_request_recheck.wteHotelRooms> objrm = new List<wte_request_recheck.wteHotelRooms>();
                int rumind = 1;
                if (Req.Descendants("Room").FirstOrDefault().Attribute("ID").Value.ToUpper() == "TRUE")
                {
                    wte_request_recheck.wteHotelRooms objrm1 = new wte_request_recheck.wteHotelRooms();
                    objrm1.RoomNo = Convert.ToString(Req.Descendants("Room").Count());
                    objrm1.RoomToken = Req.Descendants("Room").FirstOrDefault().Attribute("SessionID").Value;
                    objrm.Add(objrm1);
                }
                else
                {
                    foreach (XElement room in Req.Descendants("Room"))
                    {
                        wte_request_recheck.wteHotelRooms objrm1 = new wte_request_recheck.wteHotelRooms();
                        objrm1.RoomNo = Convert.ToString(rumind);
                        objrm1.RoomToken = room.Attribute("SessionID").Value;
                        objrm.Add(objrm1);
                        rumind++;
                    }
                }
                objrm_main.HotelRooms = objrm;
                wteReqobj.HotelOption = objrm_main;
                wterequs.Request = wteReqobj;
                WTELogModel logmodel = new WTELogModel()
                {
                    TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "ReCheck",
                    LogTypeID = 4
                };
                string resp = wteRequest.ServerRequestprecheck(JsonConvert.SerializeObject(wterequs), wterecheck, logmodel);

                if (string.IsNullOrEmpty(resp) || resp == "[]" || resp == "The remote server returned an error: (403) Forbidden." || resp == "The remote server returned an error: (400) Bad Request.")
                {
                    return null;
                }
                else
                {
                    WTELogModel nlogmodel = new WTELogModel()
                    {
                        TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                        CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                        SuplId = supplierid,
                        LogType = "PreBook",
                        LogTypeID = 4
                    };
                    string preresp = wteRequest.ServerRequest(JsonConvert.SerializeObject(wterequs), wteprebook, nlogmodel);
                    
                    if (string.IsNullOrEmpty(preresp) || preresp == "[]" || preresp == "The remote server returned an error: (403) Forbidden." || preresp == "The remote server returned an error: (400) Bad Request.")
                    {
                        return null;
                    }
                    else
                    {
                        //resp = allText;
                        XDocument rechkresdoc = (XDocument)JsonConvert.DeserializeXNode(resp, "recheckhoteldetail");                       
                        int avail = 1;
                        string tnc = string.Empty;
                        foreach (XElement room in rechkresdoc.Descendants("ReCheckRS"))
                        {
                            if (room.Element("BookingStatus").Value != "Available")
                            {
                                avail = 0;
                            }
                            tnc = tnc + room.Element("EssentialInformation").Value;
                        }
                        if (avail == 1)
                        {
                            
                            XDocument resdoc = (XDocument)JsonConvert.DeserializeXNode(preresp, "hoteldetails");
                            XElement prebookresp = resdoc.Descendants("PreBookRS").FirstOrDefault();
                            if (prebookresp.Descendants("Status").FirstOrDefault().Value == "Bookable")
                            {
                                #region CXL Policy
                                List<XElement> cxlist = new List<XElement>();
                                XElement cp = new XElement("CancellationPolicies");
                                decimal bookingcost = 0;
                                cxlist = rechkresdoc.Descendants("CancellationPolicy").ToList();
                                foreach (XElement policy in cxlist)
                                {
                                    if (Convert.ToDecimal(policy.Element("CancellationPrice").Value) != 0)
                                    {
                                        DateTime toDate = DateTime.ParseExact(policy.Element("FromDate").Value, "MM-dd-yyyy", null);
                                        string dt = toDate.ToString("dd/MM/yyyy");
                                        decimal cxlprc = Convert.ToDecimal(policy.Element("CancellationPrice").Value);
                                        bookingcost = bookingcost + cxlprc;

                                        cp.Add(new XElement("CancellationPolicy",
                                                            new XAttribute("LastCancellationDate", dt),
                                                            new XAttribute("ApplicableAmount", Convert.ToString(Convert.ToDecimal(policy.Element("CancellationPrice").Value))),
                                                            new XAttribute("NoShowPolicy", "0")));
                                    }
                                }
                                List<XElement> mergeinput = new List<XElement>();
                                mergeinput.Add(cp);
                                XElement finalcp = MergCxlPolicy(mergeinput, bookingcost);
                                #endregion

                                #region prebook response bind
                                List<XElement> roompax = Req.Descendants("RoomPax").ToList();
                                DateTime fDate = DateTime.ParseExact(Req.Descendants("FromDate").Single().Value, "dd/MM/yyyy", null);
                                DateTime tDate = DateTime.ParseExact(Req.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                                int nights = (int)(tDate - fDate).TotalDays;
                                XElement wteRoomsMealPlansfile = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/Withinearth_Meals.xml"));
                                decimal totalprice = Convert.ToDecimal(prebookresp.Descendants("TotalPrice").FirstOrDefault().Value);
                                List<XElement> roombind = new List<XElement>();
                                int rmct = 0;
                                foreach (XElement room in prebookresp.Descendants("HotelRooms").OrderBy(e => e.Element("RoomNo").Value))
                                {
                                    if (Convert.ToDecimal(room.Element("Price").Value) == 0)
                                    {
                                        string mealcode = string.Empty;
                                        try
                                        {
                                            XElement mealel = wteRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.Element("MappedMealName").Value).FirstOrDefault();
                                            mealcode = mealel.Attribute("B2BCode").Value;
                                        }
                                        catch { }
                                        decimal roomprice = totalprice / Convert.ToInt16(Req.Descendants("RoomPax").Count());
                                        IEnumerable<XElement> totroomprc = null;
                                        totroomprc = GetequalBreakuphWTE(Convert.ToString(roomprice), nights);
                                        roombind.Add(new XElement("Room",
                                                 new XAttribute("ID", Convert.ToString("")),
                                                 new XAttribute("SuppliersID", "48"),
                                                 new XAttribute("RoomSeq", room.Element("RoomNo").Value),
                                                 new XAttribute("SessionID", Convert.ToString(room.Element("UniqueId").Value)),
                                                 new XAttribute("RoomType", Convert.ToString(room.Element("RoomTypeName").Value)),
                                                 new XAttribute("OccupancyID", Convert.ToString(Req.Descendants("Room").FirstOrDefault().Attribute("OccupancyID").Value)),
                                                 new XAttribute("OccupancyName", Convert.ToString("")),
                                                 new XAttribute("MealPlanID", Convert.ToString("")),
                                                 new XAttribute("MealPlanName", Convert.ToString(room.Element("MappedMealName").Value)),
                                                 new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                                 new XAttribute("MealPlanPrice", ""),
                                                 new XAttribute("PerNightRoomRate", Convert.ToString("0")), 
                                                 new XAttribute("TotalRoomRate", Convert.ToString(roomprice)),
                                                 new XAttribute("CancellationDate", ""),
                                                 new XAttribute("CancellationAmount", ""),
                                                 new XAttribute("isAvailable", "true"),
                                                 new XElement("RequestID", Convert.ToString(prebookresp.Descendants("BookingToken").FirstOrDefault().Value)),
                                                 new XElement("Offers", ""),
                                                 new XElement("PromotionList", ""),
                                                 new XElement("CancellationPolicy", ""),
                                                 new XElement("Amenities", new XElement("Amenity", "")),
                                                 new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                 new XElement("Supplements", ""),
                                                      new XElement("PriceBreakups", totroomprc),
                                                      new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                                      new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                                 ));
                                    }
                                    else
                                    {
                                        string mealcode = string.Empty;
                                        try
                                        {
                                            XElement mealel = wteRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.Element("MappedMealName").Value).FirstOrDefault();
                                            mealcode = mealel.Attribute("B2BCode").Value;
                                        }
                                        catch { }
                                        IEnumerable<XElement> totroomprc = null;
                                        totroomprc = GetequalBreakuphWTE(Convert.ToString(room.Element("Price").Value), nights);
                                        roombind.Add(new XElement("Room",
                                                 new XAttribute("ID", Convert.ToString("")),
                                                 new XAttribute("SuppliersID", "48"),
                                                 new XAttribute("RoomSeq", room.Element("RoomNo").Value),
                                                 new XAttribute("SessionID", Convert.ToString(room.Element("UniqueId").Value)),
                                                 new XAttribute("RoomType", Convert.ToString(room.Element("RoomTypeName").Value)),
                                                 new XAttribute("OccupancyID", Convert.ToString(Req.Descendants("Room").FirstOrDefault().Attribute("OccupancyID").Value)),
                                                 new XAttribute("OccupancyName", Convert.ToString("")),
                                                 new XAttribute("MealPlanID", Convert.ToString("")),
                                                 new XAttribute("MealPlanName", Convert.ToString(room.Element("MappedMealName").Value)),
                                                 new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                                 new XAttribute("MealPlanPrice", ""),
                                                 new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                 new XAttribute("TotalRoomRate", Convert.ToString(room.Element("Price").Value)),
                                                 new XAttribute("CancellationDate", ""),
                                                 new XAttribute("CancellationAmount", ""),
                                                 new XAttribute("isAvailable", "true"),
                                                 new XElement("RequestID", Convert.ToString(prebookresp.Descendants("BookingToken").FirstOrDefault().Value)),
                                                 new XElement("Offers", ""),
                                                 new XElement("PromotionList", ""),
                                                 new XElement("CancellationPolicy", ""),
                                                 new XElement("Amenities", new XElement("Amenity", "")),
                                                 new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                 new XElement("Supplements", ""),
                                                      new XElement("PriceBreakups", totroomprc),
                                                      new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                                      new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                                 ));
                                    }
                                    rmct++;
                                }
                                roombind.Add(finalcp);
                                XElement roomgrp = new XElement("RoomTypes", new XAttribute("Index", "1"), new XAttribute("TotalRate", Convert.ToString(totalprice)),
                                    roombind);
                                XElement hotel = new XElement("Hotel",
                                                   new XElement("HotelID", Convert.ToString(Req.Descendants("HotelID").FirstOrDefault().Value)),
                                                               new XElement("HotelName", Convert.ToString(prebookresp.Descendants("HotelName").FirstOrDefault().Value)),
                                                               new XElement("Status", "true"),
                                                               new XElement("TermCondition", tnc),
                                                               new XElement("HotelImgSmall", Convert.ToString("")),
                                                               new XElement("HotelImgLarge", Convert.ToString("")),
                                                               new XElement("MapLink", ""),
                                                               new XElement("DMC", dmc),
                                                               new XElement("Currency", Convert.ToString(prebookresp.Descendants("Currency").FirstOrDefault().Value)),
                                                               new XElement("Offers", "")
                                                               , new XElement("Rooms",
                                                                        roomgrp
                                                       ));
                                #endregion

                                Response = new XElement(
                               new XElement(soapenv + "Envelope",
                                         new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                         new XElement(soapenv + "Header",
                                          new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                          new XElement("Authentication",
                                              new XElement("AgentID", AgentID),
                                              new XElement("UserName", username),
                                              new XElement("Password", password),
                                              new XElement("ServiceType", ServiceType),
                                              new XElement("ServiceVersion", ServiceVersion))),
                                                 new XElement(soapenv + "Body",
                                                     new XElement(Req.Descendants("HotelPreBookingRequest").FirstOrDefault()),
                                                        new XElement("HotelPreBookingResponse",
                                                            new XElement("NewPrice", null), // said by manisha
                                                            new XElement("Hotels",
                                                                hotel
                                               )))));
                            }
                            else
                            {
                                Response = new XElement(
                                    new XElement(soapenv + "Envelope",
                                    new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                    new XElement(soapenv + "Header",
                                     new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                     new XElement("Authentication",
                                         new XElement("AgentID", AgentID),
                                         new XElement("UserName", username),
                                         new XElement("Password", password),
                                         new XElement("ServiceType", ServiceType),
                                         new XElement("ServiceVersion", ServiceVersion))),
                                            new XElement(soapenv + "Body",
                                                new XElement(Req.Descendants("HotelPreBookingRequest").FirstOrDefault()),
                                                   new XElement("HotelPreBookingResponse"))));
                            }
                        }
                        else
                        {
                            Response = new XElement(
                                    new XElement(soapenv + "Envelope",
                                     new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                     new XElement(soapenv + "Header",
                                      new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                      new XElement("Authentication",
                                          new XElement("AgentID", AgentID),
                                          new XElement("UserName", username),
                                          new XElement("Password", password),
                                          new XElement("ServiceType", ServiceType),
                                          new XElement("ServiceVersion", ServiceVersion))),
                                             new XElement(soapenv + "Body",
                                                 new XElement(Req.Descendants("HotelPreBookingRequest").FirstOrDefault()),
                                                    new XElement("HotelPreBookingResponse"))));
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "PreBook";
                ex1.PageName = "WTEService";
                ex1.CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = Req.Descendants("TransID").FirstOrDefault().Value;
                APILog.SendCustomExcepToDB(ex1);
                IEnumerable<XElement> request = Req.Descendants("HotelPreBookingRequest").ToList();
                Response = new XElement(
                         new XElement(soapenv + "Envelope",
                                   new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                   new XElement(soapenv + "Header",
                                    new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                    new XElement("Authentication",
                                        new XElement("AgentID", AgentID),
                                        new XElement("UserName", username),
                                        new XElement("Password", password),
                                        new XElement("ServiceType", ServiceType),
                                        new XElement("ServiceVersion", ServiceVersion))),
                                           new XElement(soapenv + "Body",
                                               new XElement(request.Single()),
                                                  new XElement("HotelPreBookingResponse",
                                                       new XElement("ErrorTxt", ex.Message.ToString())))));
                #endregion
            }
            return Response;
        }
        #endregion
        #region Book
        public XElement Booking(XElement Req)
        {
            #region Credentials
            string username = Req.Descendants("UserName").Single().Value;
            string password = Req.Descendants("Password").Single().Value;
            string AgentID = Req.Descendants("AgentID").Single().Value;
            string ServiceType = Req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = Req.Descendants("ServiceVersion").Single().Value;
            #endregion
            XElement BookingResponse = new XElement("HotelBookingResponse");
            XElement SupplierResponse = null;
            try
            {
                wteRequest = new WTERequest();
                wte_request_book wtereq = new wte_request_book();
                wtereq.Token = wteToken;
                wte_request_book.wteAdvancedOptions adreq = new wte_request_book.wteAdvancedOptions();
                adreq.Currency = wteCurrency;
                adreq.CustomerIpAddress = wteipaddress;
                wtereq.AdvancedOptions = adreq;
                wte_request_book.wteRequest wteReqobj = new wte_request_book.wteRequest();
                wteReqobj.SearchKey = Req.Descendants("Room").FirstOrDefault().Attribute("OccupancyID").Value;
                wte_request_book.wteBookRQ wteReqbq = new wte_request_book.wteBookRQ();
                wteReqbq.BookingToken = Req.Descendants("Room").FirstOrDefault().Descendants("RequestID").FirstOrDefault().Value;
                wteReqbq.TotalPrice = Convert.ToDouble(Req.Descendants("TotalAmount").FirstOrDefault().Value);
                wteReqbq.InternalReference = Req.Descendants("TransID").FirstOrDefault().Value;


                List<wte_request_book.wteHotelRoom> objrm = new List<wte_request_book.wteHotelRoom>();
                int roomindx = 1;
                int leadcheck = 0;
                foreach (XElement room in Req.Descendants("Room"))
                {
                    foreach (XElement pax in room.Descendants("PaxInfo"))
                    {
                        string isleadck = "0";
                        if (leadcheck == 0)
                        {
                            isleadck = "1";
                        }
                        string paxTitle = string.Empty;
                        if (pax.Element("Title").Value.Trim().ToLower() == "miss")
                        {
                            paxTitle = "Ms.";
                        }
                        else
                        {
                            paxTitle = pax.Element("Title").Value.Trim() + ".";
                        }
                        string fullnamefst = pax.Element("FirstName").Value.Trim() + " " + pax.Element("MiddleName").Value.Trim();
                        wte_request_book.wteHotelRoom wteReqrm = new wte_request_book.wteHotelRoom();
                        wteReqrm.UniqueId = Convert.ToInt32(room.Attribute("SessionID").Value);
                        wteReqrm.RoomNo = Convert.ToString(roomindx);
                        wteReqrm.IsLead = isleadck;
                        wteReqrm.PaxType = pax.Element("GuestType").Value;
                        wteReqrm.Prefix = paxTitle;
                        wteReqrm.FirstName = fullnamefst.ToString().Trim();
                        wteReqrm.LastName = pax.Element("LastName").Value.Trim();
                        wteReqrm.ChildAge = pax.Element("Age").Value;
                        objrm.Add(wteReqrm);
                        leadcheck++;
                    }
                    roomindx++;
                }
                wteReqbq.HotelRooms = objrm;
                wteReqobj.BookRQ = wteReqbq;
                wtereq.Request = wteReqobj;
                WTELogModel logmodel = new WTELogModel()
                {
                    TrackNo = Req.Descendants("TransactionID").FirstOrDefault().Value,
                    CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Book",
                    LogTypeID = 5
                };
                string resp = wteRequest.ServerRequest(JsonConvert.SerializeObject(wtereq), wtebook, logmodel);
                try
                {
                    XDocument bookresdoc = (XDocument)JsonConvert.DeserializeXNode(resp, "booking");
                    string bookingid = bookresdoc.Descendants("BookingId").FirstOrDefault().Value;
                    string bookingref = bookresdoc.Descendants("ReferenceNo").FirstOrDefault().Value;
                    double amount = 0;
                    string bookingstatus = string.Empty;
                    try
                    {
                        wte_request_bookdetail wtereqdet = new wte_request_bookdetail();
                        wtereqdet.Token = wteToken;
                        wte_request_bookdetail.wteAdvancedOptions adreqdet = new wte_request_bookdetail.wteAdvancedOptions();
                        adreqdet.Currency = wteCurrency;
                        wtereqdet.AdvancedOptions = adreqdet;
                        wte_request_bookdetail.wteRequest wteReqobjdet = new wte_request_bookdetail.wteRequest();
                        wte_request_bookdetail.wteBookingDetailRQ wteReqobjbokdet = new wte_request_bookdetail.wteBookingDetailRQ();
                        wteReqobjbokdet.InternalReference = Req.Descendants("TransID").FirstOrDefault().Value;
                        wteReqobjdet.BookingDetailRQ = wteReqobjbokdet;
                        wtereqdet.Request = wteReqobjdet;
                        WTELogModel logmodeldet = new WTELogModel()
                        {
                            TrackNo = Req.Descendants("TransactionID").FirstOrDefault().Value,
                            CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                            SuplId = supplierid,
                            LogType = "BookDetail",
                            LogTypeID = 5
                        };
                        string respdet = wteRequest.ServerRequest(JsonConvert.SerializeObject(wtereqdet), wtebookdetail, logmodeldet);
                        XDocument bookresdetdoc = (XDocument)JsonConvert.DeserializeXNode(respdet, "bookingdetail");
                        bookingid = bookresdetdoc.Descendants("BookingId").FirstOrDefault().Value;
                        bookingref = bookresdetdoc.Descendants("ReferenceNo").FirstOrDefault().Value;
                        amount = Convert.ToDouble(bookresdetdoc.Descendants("TotalPrice").FirstOrDefault().Value);
                        foreach (XElement statusnode in bookresdetdoc.Descendants("HotelRooms"))
                        {
                            string sstt = statusnode.Descendants("BookingStatus").FirstOrDefault().Value;
                            if (sstt.ToLower().Trim() == "on request" || sstt.ToLower().Trim() == "not confirmed" || sstt.ToLower().Trim() == "completed" || sstt.ToLower().Trim() == "cancelled" || sstt.ToLower().Trim() == "cancel charge")
                            {
                                bookingstatus = "Inprocess";
                                break;
                            }
                            else if (sstt.ToLower().Trim() == "failed")
                            {
                                bookingstatus = "Failed";
                                break;
                            }
                            else if (sstt.ToLower().Trim() == "confirmed" || sstt.ToLower().Trim() == "vouchered")
                            {
                                bookingstatus = "confirmed";
                            }
                        }
                        if (bookingstatus == "Failed")
                        {
                            bookingstatus = "Failed";
                        }
                        if (bookingstatus.ToLower().Trim() == "vouchered" || bookingstatus.ToLower().Trim() == "confirmed")
                        {
                            bookingstatus = "Success";
                        }
                    }
                    catch { bookingstatus = "Inprocess"; }
                    
                    BookingResponse.Add(new XElement("Hotels",
                                                   new XElement("Hotel",
                                                       new XElement("HotelID", Req.Descendants("HotelID").FirstOrDefault().Value),
                                                       new XElement("HotelName", Req.Descendants("HotelName").FirstOrDefault().Value),
                                                       new XElement("FromDate", Req.Descendants("FromDate").FirstOrDefault().Value),
                                                       new XElement("ToDate", Req.Descendants("ToDate").FirstOrDefault().Value),
                                                       new XElement("AdultPax", Req.Descendants("Rooms").Descendants("RoomPax").Descendants("Adult").FirstOrDefault().Value),
                                                       new XElement("ChildPax", Req.Descendants("Rooms").Descendants("RoomPax").Descendants("Child").FirstOrDefault().Value),
                                                       new XElement("TotalPrice", ""),
                                                       new XElement("CurrencyID"),
                                                       new XElement("CurrencyCode", wteCurrency),
                                                       new XElement("MarketID"),
                                                       new XElement("MarketName"),
                                                       new XElement("HotelImgSmall"),
                                                       new XElement("HotelImgLarge"),
                                                       new XElement("MapLink"),
                                                       new XElement("VoucherRemark", ""),
                                                       new XElement("TransID", Req.Descendants("TransactionID").FirstOrDefault().Value),
                                                       new XElement("ConfirmationNumber", bookingref),
                                                       new XElement("Status", bookingstatus),
                                                       new XElement("PassengerDetail",
                                                       bookingRespRooms(Req.Descendants("PassengersDetail").FirstOrDefault(), amount, bookingid)))));
                }
                catch (Exception exinn)
                {
                    #region Exception
                    CustomException exi1 = new CustomException(exinn);
                    exi1.MethodName = "Booking";
                    exi1.PageName = "WTEService";
                    exi1.CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value;
                    exi1.TranID = Req.Descendants("TransactionID").FirstOrDefault().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(exi1);
                    BookingResponse.Add(new XElement("ErrorTxt", exinn.Message));
                    #endregion
                }


            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "Booking";
                ex1.PageName = "WTEService";
                ex1.CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = Req.Descendants("TransactionID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                BookingResponse.Add(new XElement("ErrorTxt", ex.Message));
                #endregion
            }
            #region Response Format
            XElement Response = new XElement(xmlns + "Envelope",
                                        new XAttribute(XNamespace.Xmlns + "soapenv", xmlns),
                                        new XElement(xmlns + "Header",
                                            new XElement("Authentication",
                                                new XElement("AgentID", AgentID),
                                                new XElement("Username", username),
                                                new XElement("Password", password),
                                                new XElement("ServiceType", ServiceType),
                                                new XElement("ServiceVersion", ServiceVersion))),
                                        new XElement(xmlns + "Body",
                                            new XElement(Req.Descendants("HotelBookingRequest").FirstOrDefault()),
                                            BookingResponse));
            #endregion
            return Response;
        }
        public XElement bookingRespRooms(XElement Rooms, double amount, string bookingref)
        {
            double perRoomRate = amount / Rooms.Descendants("Room").Count();
            XElement GuestDetails = new XElement("GuestDetails");
            foreach (XElement room in Rooms.Descendants("Room"))
            {
                GuestDetails.Add(new XElement("Room",
                                    new XAttribute("ID", room.Attribute("RoomTypeID").Value),
                                    new XAttribute("RoomType", room.Attribute("RoomType").Value),
                                    new XAttribute("ServiceID", ""),
                                    new XAttribute("RefNo", bookingref),
                                    new XAttribute("MealPlanID", room.Attribute("MealPlanID").Value),
                                    new XAttribute("MealPlanName", ""),
                                    new XAttribute("MealPlanCode", ""),
                                    new XAttribute("MealPlanPrice", room.Attribute("MealPlanPrice").Value),
                                    new XAttribute("PerNightRoomRate", ""),
                                    new XAttribute("RoomStatus", "true"),
                                    new XAttribute("TotalRoomRate", perRoomRate.ToString()),
                                    guestDetailsResp(room.Descendants("PaxInfo").ToList())));
            }
            return GuestDetails;
        }
        public List<XElement> guestDetailsResp(List<XElement> PaxInfo)
        {
            List<XElement> guests = new List<XElement>();
            try
            {
                foreach (XElement pax in PaxInfo)
                    guests.Add(new XElement("RoomGuest", pax.Elements()));
                return guests;
            }
            catch { return guests; }
        }
        #endregion
        #region Cancel
        public XElement CancelBooking(XElement Req)
        {
            #region Credentials
            string username = Req.Descendants("UserName").Single().Value;
            string password = Req.Descendants("Password").Single().Value;
            string AgentID = Req.Descendants("AgentID").Single().Value;
            string ServiceType = Req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = Req.Descendants("ServiceVersion").Single().Value;
            #endregion
            XElement Cancellation = null;
            XElement CancelResponse = new XElement("HotelCancellationResponse");
            try
            {
                #region Cancellation Charge
                Random rnd = new Random();
                int myRandomNo = rnd.Next(10000000, 99999999);
                wteRequest = new WTERequest();
                wte_request_cancelcharge wtereq = new wte_request_cancelcharge();
                wtereq.Token = wteToken;
                wte_request_cancelcharge.wteAdvancedOptions adreq = new wte_request_cancelcharge.wteAdvancedOptions();
                adreq.Currency = wteCurrency;
                wtereq.AdvancedOptions = adreq;
                wte_request_cancelcharge.wteCheckHotelCancellationChargesRQ cxlchobj = new wte_request_cancelcharge.wteCheckHotelCancellationChargesRQ();
                cxlchobj.BookingId = Convert.ToInt32(Req.Descendants("BookingCode").FirstOrDefault().Value);
                cxlchobj.InternalReference = Convert.ToString(myRandomNo);
                cxlchobj.ReferenceNo = Req.Descendants("ConfirmationNumber").FirstOrDefault().Value;
               
                wte_request_cancelcharge.wteRequest wtechobj = new wte_request_cancelcharge.wteRequest();
                wtechobj.CheckHotelCancellationChargesRQ = cxlchobj;
                wtereq.Request = wtechobj;
                
                WTELogModel logmodel = new WTELogModel()
                {
                    TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Cancel",
                    LogTypeID = 6
                };
                string resp = wteRequest.ServerRequest(JsonConvert.SerializeObject(wtereq), wtecancelcharge, logmodel);
                XDocument cxlchargeresdoc = (XDocument)JsonConvert.DeserializeXNode(resp, "cancelcharge");
                string cancelcode = cxlchargeresdoc.Descendants("CancelCode").FirstOrDefault().Value;
                string cxlamt = cxlchargeresdoc.Descendants("TotalCharge").FirstOrDefault().Value;
                string bokngId = cxlchargeresdoc.Descendants("BookingId").FirstOrDefault().Value;
                #endregion
                #region cancel
                wte_request_cancel wtecanreq = new wte_request_cancel();
                wtecanreq.Token = wteToken;
                wte_request_cancel.wteAdvancedOptions adcanreq = new wte_request_cancel.wteAdvancedOptions();
                adcanreq.Currency = wteCurrency;
                wtecanreq.AdvancedOptions = adcanreq;
                wte_request_cancel.wteCancelRQ cxlrobj = new wte_request_cancel.wteCancelRQ();
                cxlrobj.BookingId = Convert.ToInt32(bokngId);
                cxlrobj.BookingDetailId = 0;
                cxlrobj.CancelCode = cancelcode;
                cxlrobj.CancelAll = 1;
                cxlrobj.Reason = "I want to cancel the booking";
                wte_request_cancel.wteRequest wtecanobj = new wte_request_cancel.wteRequest();
                wtecanobj.CancelRQ = cxlrobj;
                wtecanreq.Request = wtecanobj;
                WTELogModel logmodel2 = new WTELogModel()
                {
                    TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Cancel",
                    LogTypeID = 6
                };
                string canresp = wteRequest.ServerRequest(JsonConvert.SerializeObject(wtecanreq), wtecancel, logmodel2);
                XDocument cancelresdoc = (XDocument)JsonConvert.DeserializeXNode(canresp, "cancel");
                string status = cancelresdoc.Descendants("CancelStatus").FirstOrDefault().Value;
                if (status == "1")
                {
                    CancelResponse.Add(new XElement("Rooms",
                                            new XElement("Room",
                                                new XElement("Cancellation",
                                                    new XElement("Amount", cxlamt.ToString()),
                                                    new XElement("Status", "Success")))));
                }
                else
                {
                    double cxAmount = Convert.ToDouble(0);
                    CancelResponse.Add(new XElement("Rooms",
                                            new XElement("Room",
                                                new XElement("Cancellation",
                                                    new XElement("Amount", cxAmount.ToString()),
                                                    new XElement("Status", "Failed")))));
                }
                #endregion
                
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "CancelBooking";
                ex1.PageName = "WTEService";
                ex1.CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = Req.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion
            }
            Cancellation = new XElement(xmlns + "Envelope",
                                           new XAttribute(XNamespace.Xmlns + "soapenv", xmlns),
                                           new XElement(xmlns + "Header",
                                               new XElement("Authentication",
                                                   new XElement("AgentID", AgentID),
                                                   new XElement("Username", username),
                                                   new XElement("Password", password),
                                                   new XElement("ServiceType", ServiceType),
                                                   new XElement("ServiceVersion", ServiceVersion))),
                                           new XElement(xmlns + "Body",
                                               new XElement(Req.Descendants("HotelCancellationRequest").FirstOrDefault()),
                                               CancelResponse));
            return Cancellation;
        }
        #endregion
        #region Hotel Detail
        public XElement HotelDetail(XElement Req, int supID)
        {
            #region Credentials
            string username = Req.Descendants("UserName").Single().Value;
            string password = Req.Descendants("Password").Single().Value;
            string AgentID = Req.Descendants("AgentID").Single().Value;
            string ServiceType = Req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = Req.Descendants("ServiceVersion").Single().Value;
            #endregion
            XElement Response = new XElement("hoteldescResponse");
            try
            {
                string hotelID = Req.Descendants("HotelID").FirstOrDefault().Value;
                WTEStatic wteStatic = new WTEStatic();
                DataTable wteHotels = wteStatic.GetSingleHotelDetail(hotelID);

                if (wteHotels != null && wteHotels.Rows.Count > 0)
                {
                    XElement Images = new XElement("Images");
                    Images.Add(new XElement("Image",
                                        new XAttribute("Path", wteHotels.Rows[0][0].ToString()),
                                        new XAttribute("Caption", "")));
                    XElement Facilities = new XElement("Facilities");  
                    Response.Add(new XElement("Hotels",
                                    new XElement("Hotel",
                                        new XElement("HotelID", Req.Descendants("HotelID").FirstOrDefault().Value),
                                        new XElement("Description", ""),
                                        Images,
                                        Facilities,
                                        new XElement("ContactDetails",
                                            new XElement("Phone", ""),
                                            new XElement("Fax", "")),
                                        new XElement("CheckinTime"),
                                        new XElement("CheckoutTime"))));
                }
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelDetail";
                ex1.PageName = "WTEService";
                ex1.CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = Req.Descendants("TransID").FirstOrDefault().Value;
                APILog.SendCustomExcepToDB(ex1);
                #endregion
            }
            XElement DetailsResponse = new XElement(xmlns + "Envelope",
                                        new XAttribute(XNamespace.Xmlns + "soapenv", xmlns),
                                        new XElement(xmlns + "Header",
                                            new XElement("Authentication",
                                            new XElement("AgentID", AgentID),
                                            new XElement("Username", username),
                                            new XElement("Password", password),
                                            new XElement("ServiceType", ServiceType),
                                            new XElement("ServiceVersion", ServiceVersion))),
                                        new XElement(xmlns + "Body",
                                            Req.Descendants("hoteldescRequest").FirstOrDefault(),
                                            Response));
            return DetailsResponse;
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