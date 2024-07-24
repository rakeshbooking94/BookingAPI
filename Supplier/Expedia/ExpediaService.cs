using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Supplier.Expedia;
using TravillioXMLOutService.Models.Expedia;
using TravillioXMLOutService.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace TravillioXMLOutService.Supplier.Expedia
{
    public class ExpediaService
    {
        #region Credentails
        string apikey = string.Empty;
        string secret = string.Empty;
        string endpoint = string.Empty;
        string version = string.Empty;
        string point_of_sale = string.Empty;
        string payment_terms = string.Empty;
        string platform_name = string.Empty;
        string clientname = string.Empty;
        string clientaddress = string.Empty;
        string clientcity = string.Empty;
        string clientcountry = string.Empty;
        string currency = string.Empty;
        string postalcode = string.Empty;
        string rateplancount = string.Empty;
        #endregion

        #region Global vars
        string customerid = string.Empty;
        string dmc = string.Empty;
        const int supplierid = 20;
        int chunksize = 250;
        int sup_cutime = 20, threadCount=2;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        ExpediaRequest  epsRequest;
        string sales_environment = "hotel_package";
      
        #endregion

        public ExpediaService(string _customerid)
        {
            XElement suppliercred = supplier_Cred.getsupplier_credentials(_customerid, "20");
            try
            {
                apikey = suppliercred.Descendants("apikey").FirstOrDefault().Value;
                secret = suppliercred.Descendants("secret").FirstOrDefault().Value;
                endpoint = suppliercred.Descendants("endpoint").FirstOrDefault().Value;
                version = suppliercred.Descendants("version").FirstOrDefault().Value;
                point_of_sale = suppliercred.Descendants("point_of_sale").FirstOrDefault().Value;
                payment_terms = suppliercred.Descendants("payment_terms").FirstOrDefault().Value;
                platform_name = suppliercred.Descendants("platform_name").FirstOrDefault().Value;
                clientname = suppliercred.Descendants("client").FirstOrDefault().Value;
                clientaddress = suppliercred.Descendants("client").FirstOrDefault().Attribute("address").Value;
                clientcity = suppliercred.Descendants("client").FirstOrDefault().Attribute("city").Value;
                clientcountry = suppliercred.Descendants("client").FirstOrDefault().Attribute("countrycode").Value;
                postalcode = suppliercred.Descendants("client").FirstOrDefault().Attribute("postal_code").Value;
                currency = suppliercred.Descendants("currency").FirstOrDefault().Value;
                rateplancount = suppliercred.Descendants("rateplancount").FirstOrDefault().Value;

              
            }
            catch { }
        }
        public ExpediaService()
        {
             
        }

        #region Hotel Availability
        public List<XElement> HotelAvailability(XElement req, string custID, string xtype)
        {
            dmc = xtype;
            customerid = custID;
            string htlCode = string.Empty;
            List<XElement> HotelsList = new List<XElement>();
            try
            {
                ExpediaStatic epsStatic = new ExpediaStatic();

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
                string destinationcity = string.Empty;
                string countryCode = string.Empty;
                DataTable epsCity = epsStatic.GetCityCode(req.Descendants("CityID").FirstOrDefault().Value, req.Descendants("CountryID").FirstOrDefault().Value, 20);
                bool isCityMapped = true;
                //destinationcity = "Norfolk";
                //countryCode = "US";
                if (epsCity != null)
                {
                    if (epsCity.Rows.Count != 0)
                    {
                        destinationcity = epsCity.Rows[0]["City"].ToString();
                        countryCode = epsCity.Rows[0]["CountryCode"].ToString();

                    }
                    else
                    {
                        isCityMapped = false;
                    }
                }
                //if (isCityMapped)
                //{
                //    APILogDetail log = new APILogDetail();
                //    log.customerID = Convert.ToInt64(customerid);
                //    log.TrackNumber = req.Descendants("TransID").Single().Value;
                //    log.LogTypeID = 1;
                //    log.LogType = "Search";
                //    log.SupplierID = supplierid;
                //    log.logrequestXML = req.ToString();
                //    log.logresponseXML = "City mapping found";
                //    log.StartTime = DateTime.Now;
                //    log.EndTime = DateTime.Now;
                //    try
                //    {
                //        SaveAPILog savelogpro = new SaveAPILog();
                //        savelogpro.SaveAPILogs(log);
                //    }
                //    catch (Exception ex)
                //    {
                //        CustomException ex1 = new CustomException(ex);
                //        ex1.MethodName = "HotelAvailability";
                //        ex1.PageName = "ExpediaSearch";
                //        ex1.CustomerID = customerid;
                //        ex1.TranID = req.Descendants("TransID").Single().Value;
                //        SaveAPILog saveex = new SaveAPILog();
                //        saveex.SendCustomExcepToDB(ex1);
                //        return null;
                //    }
                    
                //}
              
                DataTable EpsHotels = epsStatic.GetStaticHotels(req.Descendants("HotelID").FirstOrDefault().Value, req.Descendants("HotelName").FirstOrDefault().Value, destinationcity, countryCode, req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt(), req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt());
                
                if (EpsHotels == null || EpsHotels.Rows.Count == 0)
                {
                    #region No hotel found exception
                    CustomException ex1 = new CustomException("There is no hotel available in database");
                    ex1.MethodName = "HotelAvailability";
                    ex1.PageName = "ExpediaService";
                    ex1.CustomerID = customerid.ToString();
                    ex1.TranID = req.Descendants("TransID").First().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    #endregion
                    return null;
                }
                string occupancy = req.Descendants("Rooms").FirstOrDefault().Descendants("RoomPax").ToList().epsOccupancy();
                var statiProperties = EpsHotels.AsEnumerable().Select(r => r.Field<string>("hotelcode")).ToList();
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
                                    new Thread(()=> thr1 = SearchHotel(req,occupancy, splitList[i], EpsHotels,timeOut,sales_environment)),
                                   
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
                                   new Thread(()=> thr1 = SearchHotel(req,occupancy, splitList[i], EpsHotels,timeOut,sales_environment)),
                                  new Thread(()=> thr2 =  SearchHotel(req,occupancy, splitList[i+1], EpsHotels,timeOut,sales_environment)),
                                  
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
                                    new Thread(()=> thr1 = SearchHotel(req,occupancy, splitList[i], EpsHotels,timeOut ,sales_environment)),
                                    new Thread(()=> thr2 = SearchHotel(req,occupancy, splitList[i+1], EpsHotels,timeOut,sales_environment)),
                                    new Thread(()=> thr3 = SearchHotel(req,occupancy, splitList[i+2], EpsHotels,timeOut,sales_environment)),
                                      
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
                                    new Thread(()=> thr1 = SearchHotel(req,occupancy, splitList[i], EpsHotels,timeOut ,sales_environment)),
                                    new Thread(()=> thr2 = SearchHotel(req,occupancy, splitList[i+1], EpsHotels,timeOut,sales_environment)),
                                    new Thread(()=> thr3 = SearchHotel(req,occupancy, splitList[i+2], EpsHotels,timeOut,sales_environment)),
                                     new Thread(()=> thr4 = SearchHotel(req,occupancy, splitList[i+2], EpsHotels,timeOut,sales_environment)),
                                      
                                };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOut));
                                threadlist.ForEach(t => t.Abort());
                                HotelsList.AddRange(thr1.Concat(thr2).Concat(thr3).Concat(thr4));
                            }
                            #endregion
                           // HotelsList= HotelsList.OrderBy(x => x.Descendants("MinRate").FirstOrDefault().Value).GroupBy(x=>x.Descendants("HotelID").FirstOrDefault().Value).Select(r=>r.FirstOrDefault()).ToList();
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
                    ex1.PageName = "ExpediaService";
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
                ex1.PageName = "ExpediaService";
                ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                ex1.TranID = req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion

                return null;
            }
        }

        #region thread search result
        private List<XElement> SearchHotel(XElement req, string occupancy, List<string> propertyChunk, DataTable EpsStaticHotels, int timeout, string sales_environment= "")
        {
            try
            {
                string pax_CountryCode = string.Empty;
                if (req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim() == "PS" || req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim() == "IQ" || req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim() == "LY")
                {
                    pax_CountryCode = "JO";
                }
                else
                {
                    pax_CountryCode = req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim();
                }
                List<XElement> ThHotelsList = new List<XElement>();
                epsRequest = new ExpediaRequest();
                string propertyList = propertyChunk.getEpsProperties();

                string searchRequest = endpoint + "/" + version + "/properties/availability?checkin=" + req.Descendants("FromDate").FirstOrDefault().Value.ModifyToEpsDate() + "&checkout=" + req.Descendants("ToDate").FirstOrDefault().Value.ModifyToEpsDate() + "&" + propertyList + "&currency=" + currency + "&language=en-US&country_code=" + pax_CountryCode + "&" + occupancy + "&sales_channel=agent_tool&sales_environment=" + sales_environment + "&filter=expedia_collect&rate_plan_count=1&rate_option=net_rates&partner_point_of_sale=" + point_of_sale + "&payment_terms=" + payment_terms + "";



                EpsLogModel logmodel = new EpsLogModel()
                {
                    TrackNo = req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = customerid.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Search",
                    LogTypeID = 1
                };
                string resp = epsRequest.ServerRequestSearch(searchRequest, apikey, secret, logmodel, timeout);

                if (string.IsNullOrEmpty(resp) || resp == "[]" || resp == "The remote server returned an error: (403) Forbidden." || resp == "The remote server returned an error: (400) Bad Request.")
                {
                    return null;
                }
                else
                {
                    string xmlouttype = string.Empty;
                    try
                    {
                        if (dmc == "Expedia")
                        {
                            xmlouttype = "false";
                        }
                        else
                        { xmlouttype = "true"; }
                    }
                    catch { }
                    List<string> epaxList = req.Descendants("Rooms").FirstOrDefault().Descendants("RoomPax").ToList().epsPaxList();
                    JArray hotelList = (JArray)JsonConvert.DeserializeObject(resp);
                    foreach (JObject hotel in hotelList)
                    {
                        try
                        {
                            var htlcode = (string)hotel["property_id"];
                            DataRow[] htlSt = EpsStaticHotels.Select("[hotelcode] = '" + htlcode + "'");
                            if (htlSt.Length > 0)
                            {
                                var HotelData = htlSt[0];
                                #region get minimum rate
                                decimal minrate = 0;
                                foreach (JToken room in hotel["rooms"])
                                {
                                    decimal roomRate = 0;
                                    var PriceBreakup = room["rates"][0]["occupancy_pricing"];
                                    foreach (var pax in epaxList)
                                    {
                                        decimal roomprice = (decimal)PriceBreakup[pax]["totals"]["inclusive"]["request_currency"]["value"] - (decimal)PriceBreakup[pax]["totals"]["marketing_fee"]["request_currency"]["value"];
                                        roomRate = roomRate + roomprice;
                                    }
                                    if (minrate == 0)
                                    {
                                        minrate = roomRate;
                                    }
                                    else if (roomRate < minrate)
                                    {
                                        minrate = roomRate;
                                    }
                                }
                                #endregion

                                XElement hoteldata = new XElement("Hotel", new XElement("HotelID", HotelData["hotelcode"].ToString()),
                                                new XElement("HotelName", HotelData["hotelname"].ToString()),
                                                new XElement("PropertyTypeName", ""),
                                                new XElement("CountryID", ""),
                                                new XElement("CountryName", req.Descendants("CountryName").FirstOrDefault().Value),
                                                new XElement("CountryCode", HotelData["country_code"].ToString()),
                                                new XElement("CityId"),
                                                new XElement("CityCode", req.Descendants("CityCode").FirstOrDefault().Value),
                                                new XElement("CityName", HotelData["city"].ToString()),
                                                new XElement("AreaId"),
                                                new XElement("AreaName", ""),
                                                new XElement("RequestID", ""),
                                                new XElement("Address", HotelData["address"].ToString()),
                                                new XElement("Location", HotelData["address"].ToString()),
                                                new XElement("Description"),
                                                new XElement("StarRating", HotelData["rating"].ToString().ModifyToStar()),
                                                new XElement("MinRate", minrate),
                                                new XElement("HotelImgSmall", HotelData["image"].ToString()),
                                                new XElement("HotelImgLarge", HotelData["image"].ToString()),
                                                new XElement("MapLink"),
                                                new XElement("Longitude", HotelData["longitude"].ToString()),
                                                new XElement("Latitude", HotelData["latitude"].ToString()),
                                                new XElement("xmloutcustid", customerid),
                                                new XElement("xmlouttype", xmlouttype),
                                                new XElement("DMC", dmc), new XElement("SupplierID", supplierid),
                                                new XElement("Currency", currency),
                                                new XElement("Offers", ""), new XElement("Facilities", null),
                                                new XElement("Rooms", ""),
                                                new XElement("searchType", sales_environment)
                                                );

                                ThHotelsList.Add(hoteldata);


                            }


                        }
                        catch { }
                    }
                }

                return ThHotelsList;
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelAvailability_SearchHotel";
                ex1.PageName = "ExpediaService";
                ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                ex1.TranID = req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion
                return null;
            }
        
        }
        #endregion

        #endregion

        #region Hotel Details
        public XElement HotelDetails(XElement req)
        {
            XElement hotelDesc = new XElement("Hotels");
            XElement HotelDescReq = req.Descendants("hoteldescRequest").FirstOrDefault();
            XElement hotelDescResdoc = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", req.Descendants("AgentID").Single().Value), new XElement("UserName", req.Descendants("UserName").Single().Value),
                                       new XElement("Password", req.Descendants("Password").Single().Value), new XElement("ServiceType", req.Descendants("ServiceType").Single().Value), new XElement("ServiceVersion", req.Descendants("ServiceVersion").Single().Value))));

            try
            {
                ExpediaStatic epsStatic = new ExpediaStatic();
                DataTable HotelDetail = epsStatic.GetHotelDetails(req.Descendants("HotelID").FirstOrDefault().Value);
                if (!string.IsNullOrEmpty(HotelDetail.Rows[0]["HotelId"].ToString()))
                {
                    string hoteldetails = string.IsNullOrEmpty(HotelDetail.Rows[0]["Details"].ToString()) ? HotelDetail.Rows[0]["HotelName"].ToString() : HotelDetail.Rows[0]["Details"].ToString();

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
                return hotelDescResdoc;
            }
        }


        #endregion

        #region Room Availability
        public XElement GetRoomAvail_ExpediaOUT(XElement req)
        {
            List<XElement> roomavailabilityresponse = new List<XElement>();
            XElement getrm = null;
            try
            {

                string dmc = string.Empty;
                List<XElement> htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "20").ToList();
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
                            customerid = htlele[i].Attribute("custIDs").Value;
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
                        dmc = "Expedia";
                    }
                    ExpediaService rs = new ExpediaService(customerid);
                    roomavailabilityresponse.Add(rs.RoomAvailability(req, dmc, htlid, customerid));
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
                epsRequest = new ExpediaRequest();
                customerid = custid;
                string pax_CountryCode = string.Empty;
                if (roomReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim() == "PS" || roomReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim() == "IQ" || roomReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim() == "LY")
                {
                    pax_CountryCode = "JO";
                }
                else
                {
                    pax_CountryCode = roomReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value.Trim();
                }

                string occupancy = roomReq.Descendants("Rooms").FirstOrDefault().Descendants("RoomPax").ToList().epsOccupancy();
                string propertyList = "property_id=" + htlid;
                string roomRequest = endpoint + "/" + version + "/properties/availability?checkin=" + roomReq.Descendants("FromDate").FirstOrDefault().Value.ModifyToEpsDate() +
                                       "&checkout=" + roomReq.Descendants("ToDate").FirstOrDefault().Value.ModifyToEpsDate() + "&" + propertyList + "&currency=" + currency + "&language=en-US" +
                                       "&country_code=" + pax_CountryCode + "&" + occupancy + "&sales_channel=agent_tool&sales_environment=" + sales_environment + "&filter=expedia_collect" +
                                       "&rate_plan_count="+rateplancount+"&rate_option=net_rates&partner_point_of_sale=" + point_of_sale+"&payment_terms=" + payment_terms + "";

                EpsLogModel logmodel = new EpsLogModel()
                {
                    TrackNo = roomReq.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = customerid.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "RoomAvail",
                    LogTypeID = 2
                };

                string roomResp = epsRequest.ServerRequest_Room(roomRequest, apikey, secret, logmodel);

                          
                if (!string.IsNullOrEmpty(roomResp) && roomResp != "[]")
                {
                    JArray hotelList = (JArray)JsonConvert.DeserializeObject(roomResp);
                    XElement groupDetails = new XElement("Rooms");
                   
                    int nights = (int)(roomReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - roomReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;
                    int grpIndex = 1;
                   
                        foreach (JToken room in hotelList[0]["rooms"])
                        {
                            

                            string roomid = (string)room["id"];
                            string roomName = (string)room["room_name"];
                            foreach (JToken rate in room["rates"])
                            {
                                string rateid = (string)rate["id"];
                                string BoardName = "Room Only";
                                string cxlPolicy = string.Empty;
                                string nonRefundableDate = string.Empty;
                                bool isRefundable = (bool)rate["refundable"];
                                if (isRefundable)
                                {
                                    var penalty =rate["cancel_penalties"];
                                    if (penalty != null)
                                    {
                                        cxlPolicy = penalty.ToString();
                                    }

                                }
                                var nonRefDate = rate["nonrefundable_date_ranges"];
                                if (nonRefDate != null)
                                {
                                    nonRefundableDate = nonRefDate.ToString();
                                }
                                string boardId = string.Empty;
                                var boardPlan = rate["amenities"];
                                if (boardPlan != null)
                                {
                                    foreach (JToken attribute in boardPlan)
                                    {
                                        JProperty jProperty = attribute.ToObject<JProperty>();
                                        boardId = jProperty.Name;

                                        string amenty = (string)boardPlan[boardId]["name"];
                                        if (amenty.ToLower().Contains("break") || amenty.ToLower().Contains("board") || amenty.ToLower().Contains("inclusive"))
                                        {

                                            BoardName = amenty;
                                            BoardName = (BoardName == "Free breakfast" ? "Bed & Breakfast" : BoardName);
                                        }
                                    }
                                }
                                                            
                              
                                string promotion = string.Empty;
                                var promo = rate["promotions"];
                                if (promo != null)
                                {
                                    if (promo["deal"] != null)
                                    {
                                        promotion = (string)promo["deal"]["description"];
                                    }
                                    if (promo["value_adds"] != null)
                                    {
                                        try
                                        {
                                            string pro_id = string.Empty;
                                            foreach (JToken attribute in promo["value_adds"])
                                            {
                                                JProperty jProperty = attribute.ToObject<JProperty>();
                                                pro_id = jProperty.Name;
                                            }
                                            string added_Promo = (string)promo["value_adds"][pro_id]["description"] + "( " + (string)promo["value_adds"][pro_id]["person_count"] + " Person )";
                                            promotion = promotion + " " + added_Promo;
                                        }
                                        catch { }
                                    }

                                }
                                bool isPackageRate = (bool)rate["sale_scenario"]["package"];
                                string searchtype = string.Empty;
                                if (isPackageRate)
                                {
                                    searchtype = "hotel_package";
                                }
                                var ocpPrice = rate["occupancy_pricing"];
                                var bedgroup = rate["bed_groups"];
                                foreach (JToken attribute in bedgroup)
                                {
                                    string bedgroupid = string.Empty;
                                    string bedname = string.Empty;
                                    JProperty jProperty = attribute.ToObject<JProperty>();
                                    bedgroupid = jProperty.Name;
                                    bedname = (string)bedgroup[bedgroupid]["description"];
                                    if (!isRefundable)
                                    {
                                        bedname = bedname + " - Non-refundable";
                                    }
                                    var pricechecklink = (string)bedgroup[bedgroupid]["links"]["price_check"]["href"];

                                    int roomSeq = 1;
                                    List<XElement> roomDtlList = new List<XElement>();
                                    decimal TotalRoomPrice = 0;
                                    foreach (var roompax in roomReq.Descendants("RoomPax"))
                                    {
                                        string pax = roompax.RoomOcp();
                                        decimal roomPrice = (decimal)ocpPrice[pax]["totals"]["inclusive"]["request_currency"]["value"];
                                        decimal marketingFee = (decimal)ocpPrice[pax]["totals"]["marketing_fee"]["request_currency"]["value"];
                                        roomPrice = roomPrice - marketingFee;
                           
                                        //JArray nightly = (JArray)ocpPrice[pax]["nightly"];
                                        TotalRoomPrice = TotalRoomPrice + roomPrice;
                                        roomDtlList.Add(new XElement("Room", new XAttribute("ID", roomid), new XAttribute("SuppliersID", supplierid), new XAttribute("RoomSeq", roomSeq), new XAttribute("SessionID", cxlPolicy), new XAttribute("RoomType", roomName), new XAttribute("OccupancyID", bedgroupid),
                                                              new XAttribute("OccupancyName", bedname), new XAttribute("MealPlanID", promotion), new XAttribute("MealPlanName", BoardName), new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", roomPrice / nights),
                                                              new XAttribute("TotalRoomRate", roomPrice), new XAttribute("CancellationDate", ""), new XAttribute("CancellationAmount", nonRefundableDate), new XAttribute("isAvailable", true), new XAttribute("searchType", searchtype),
                                                              new XElement("RequestID", pricechecklink), new XElement("Offers", ""), new XElement("PromotionList", new XElement("Promotions", promotion)),
                                                              new XElement("CancellationPolicy"), new XElement("Amenities", new XElement("Amenity")),
                                                              new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                              new XElement("Supplements"),
                                                              new XElement(getPriceBreakup(nights, roomPrice)),
                                                              new XElement("AdultNum", roompax.Descendants("Adult").FirstOrDefault().Value),
                                                              new XElement("ChildNum", roompax.Descendants("Child").FirstOrDefault().Value)));


                                        roomSeq++;
                                    }
                                    XElement RoomType = new XElement("RoomTypes", new XAttribute("Index", grpIndex), new XAttribute("TotalRate", TotalRoomPrice), new XAttribute("HtlCode", htlid), new XAttribute("CrncyCode", "USD"), new XAttribute("DMCType", dmc));
                                    RoomType.Add(roomDtlList);
                                    groupDetails.Add(RoomType);
                                    grpIndex++;
                                }
                            }
                        }
                           
                        

                    

                           
                
                    XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID"), new XElement("HotelName"), new XElement("PropertyTypeName"),
                                       new XElement("CountryID"), new XElement("CountryName"), new XElement("CityCode"), new XElement("CityName"),
                                       new XElement("AreaId"), new XElement("AreaName"), new XElement("RequestID"), new XElement("Address"), new XElement("Location"),
                                       new XElement("Description"), new XElement("StarRating"), new XElement("MinRate"), new XElement("HotelImgSmall"),
                                       new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("Longitude"), new XElement("Latitude"), new XElement("DMC"),
                                       new XElement("SupplierID"), new XElement("Currency",currency), new XElement("Offers"),
                                       new XElement(groupDetails)));
                    RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", hoteldata)));


                }
                else
                {
                    RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"))));
                }

                return RoomDetails;
            
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "RoomAvailability";
                ex1.PageName = "ExpediaService";
                ex1.CustomerID = roomReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = roomReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"))));
                #endregion
                return RoomDetails;
            }
        }

        #region Price Breakup
        private XElement getPriceBreakup(int nights, decimal roomPrice)
        {
            XElement pricebrk = new XElement("PriceBreakups");
            decimal stayPerNyte = Math.Round(roomPrice / nights,4);
            for (int i = 1; i <= nights; i++)
            {
                pricebrk.Add(new XElement("Price", new XAttribute("Night", i), new XAttribute("PriceValue", stayPerNyte)));
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
                ExpediaStatic epsStatic = new ExpediaStatic();

                int nights = (int)(cxlPolicyReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - cxlPolicyReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;
                decimal RoomGroupRate = cxlPolicyReq.Descendants("TotalRoomRate").FirstOrDefault().Value.ModifyToDecimal();
                string cxl_penalty = string.Empty;
                string nonrefund_DateRange = string.Empty;
                bool isNonrefundabele = false;
                string roomid = cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("ID").Value;
                cxl_penalty = cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("SessionID").Value;
                nonrefund_DateRange = cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("CancellationAmount").Value; 
                DateTime checkinDt=(DateTime)cxlPolicyReq.Descendants("FromDate").FirstOrDefault().Value.StringToDate(); 
                DateTime checkoutDt=(DateTime)cxlPolicyReq.Descendants("ToDate").FirstOrDefault().Value.StringToDate(); 
               

                try
                {
                    if (!string.IsNullOrEmpty(nonrefund_DateRange))
                    {
                        JArray NonRefDateList = (JArray)JsonConvert.DeserializeObject(nonrefund_DateRange);
                        if (NonRefDateList != null)
                        {
                            foreach (var nonrefdate in NonRefDateList)
                            {
                                var startdt = (DateTime)nonrefdate["start"];
                                var enddt = (DateTime)nonrefdate["end"];
                                if ((startdt <= checkinDt && checkinDt <= enddt) && isNonrefundabele == false)
                                {
                                    isNonrefundabele = true;
                                }
                                if ((startdt <= checkoutDt && checkoutDt <= enddt) && isNonrefundabele == false)
                                {
                                    isNonrefundabele = true;
                                }
                            }
                        }
                    }
                }
                catch { }
                cxl_penalty = isNonrefundabele ? string.Empty : cxl_penalty;
                CxlPolicyResponse.Add(new XElement(soapenv + "Body", cxlPolicyReq, new XElement("HotelDetailwithcancellationResponse",
                        new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", cxlPolicyReq.Descendants("HotelID").FirstOrDefault().Value),
                        new XElement("HotelName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"),
                        new XElement("DMC", "Expedia"), new XElement("Currency"), new XElement("Offers"),
                        new XElement("Rooms", new XElement("Room", new XAttribute("ID", cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("ID").Value),
                        new XAttribute("RoomType", ""), new XAttribute("PerNightRoomRate", cxlPolicyReq.Descendants("PerNightRoomRate").FirstOrDefault().Value),
                        new XAttribute("TotalRoomRate", cxlPolicyReq.Descendants("TotalRoomRate").FirstOrDefault().Value),
                        new XAttribute("LastCancellationDate", ""),
                        GetCxlPolicy(cxl_penalty, nights, RoomGroupRate)
                        )))))));
                return CxlPolicyResponse;
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "CancellationPolicy";
                ex1.PageName = "ExpediaService";
                ex1.CustomerID = cxlPolicyReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                CxlPolicyResponse.Add(new XElement(soapenv + "Body", CxlPolicyReqest, new XElement("HotelDetailwithcancellationResponse", new XElement("ErrorTxt", "No cancellation policy found"))));
                #endregion
                return CxlPolicyResponse;

            }


        }
        #region cxl Policy

        private XElement GetCxlPolicy(string cxl_penalty, int nights, decimal TotalPrice)
        {
            Dictionary<DateTime, decimal> cxlPolicies = new Dictionary<DateTime, decimal>();
            DateTime lastCxldate = DateTime.MaxValue.Date;
            string policyTxt = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(cxl_penalty))
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
                    JArray penaltyList = (JArray)JsonConvert.DeserializeObject(cxl_penalty);

                    foreach (var penalty in penaltyList)
                    {
                        decimal cxlCharges = 0.0m;
                        var dt = (DateTimeOffset)penalty["start"];
                        DateTime Cxldate = dt.UtcDateTime;
                        if (Cxldate.AddDays(-1) < lastCxldate)
                        {
                            lastCxldate = Cxldate.AddDays(-1);
                        }

                        if (penalty["nights"] != null)
                        {
                            cxlCharges = TotalPrice / nights * (int)penalty["nights"];
                        }
                        else if (penalty["amount"] != null)
                        {
                            cxlCharges = (decimal)penalty["amount"];
                        }
                        else if (penalty["percent"] != null)
                        {
                            string per = penalty["percent"].ToString().Replace(@"%", @"");
                            cxlCharges = TotalPrice * Decimal.Parse(per) / 100;
                        }
                        else
                        {
                            cxlCharges = TotalPrice;
                        }
                        cxlPolicies.AddCxlPolicy(Cxldate, cxlCharges);

                    }
                }
                cxlPolicies.Add(lastCxldate, 0);
                XElement cxlplcy = new XElement("CancellationPolicies", from polc in cxlPolicies.OrderBy(k => k.Key) select new XElement("CancellationPolicy", new XAttribute("LastCancellationDate", polc.Key.ToString("dd'/'MM'/'yyyy")), new XAttribute("ApplicableAmount", polc.Value), new XAttribute("NoShowPolicy", "0")));
                return cxlplcy;
            }
            catch (Exception ex)
            {
                cxlPolicies.Clear();
                DateTime Cxldate = DateTime.Now.Date;
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
                ExpediaStatic epsStatic = new ExpediaStatic();
                epsRequest = new ExpediaRequest();
                string PreBook_Request = endpoint + preBookReq.Descendants("Room").FirstOrDefault().Descendants("RequestID").FirstOrDefault().Value; 

                EpsLogModel logmodel = new EpsLogModel()
                {
                    TrackNo = preBookReq.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = preBookReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "PreBook",
                    LogTypeID = 4
                };
                string respPreBook = epsRequest.ServerRequest(PreBook_Request, apikey, secret, logmodel);

                if (!string.IsNullOrEmpty(respPreBook) && respPreBook != "[]")
                {
                    List<XElement> tvRoomList = preBookReq.Descendants("Room").ToList();
                    XElement groupDetails = new XElement("Rooms");
                    string termsCondition = string.Empty;
                    try
                    {
                        termsCondition = epsStatic.GetHotelPolicy(preBookReq.Descendants("HotelID").FirstOrDefault().Value);
                    }
                    catch { }
                    bool isNonrefundabele = false;
                    int totalrooms = preBookReq.Descendants("Room").Count();
                    int nights = (int)(preBookReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - preBookReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;
                    decimal TotalRateOld = preBookReq.Descendants("RoomTypes").FirstOrDefault().Attribute("TotalRate").Value.ModifyToDecimal();
                    string cxl_penalty = preBookReq.Descendants("Room").FirstOrDefault().Attribute("SessionID").Value;
                    string nonrefund_DateRange = preBookReq.Descendants("Room").FirstOrDefault().Attribute("CancellationAmount").Value;
                    DateTime checkinDt = (DateTime)preBookReq.Descendants("FromDate").FirstOrDefault().Value.StringToDate();
                    DateTime checkoutDt = (DateTime)preBookReq.Descendants("ToDate").FirstOrDefault().Value.StringToDate();
                    try
                    {
                        if (!string.IsNullOrEmpty(nonrefund_DateRange))
                        {
                            JArray NonRefDateList = (JArray)JsonConvert.DeserializeObject(nonrefund_DateRange);
                            if (NonRefDateList != null)
                            {
                                foreach (var nonrefdate in NonRefDateList)
                                {
                                    var startdt = (DateTime)nonrefdate["start"];
                                    var enddt = (DateTime)nonrefdate["end"];
                                    if ((startdt <= checkinDt && checkinDt <= enddt) && isNonrefundabele == false)
                                    {
                                        isNonrefundabele = true;
                                    }
                                    if ((startdt <= checkoutDt && checkoutDt <= enddt) && isNonrefundabele == false)
                                    {
                                        isNonrefundabele = true;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    cxl_penalty = isNonrefundabele ? string.Empty : cxl_penalty;

                    JObject preBook = (JObject)JsonConvert.DeserializeObject(respPreBook);
                    string status = (string)preBook["status"];
                    string bookinglink = (string)preBook["links"]["book"]["href"];
                    int roomSeq = 1;
                    if (status == "available")
                    {
                        XElement RoomType = new XElement("RoomTypes", new XAttribute("Index", "1"), new XAttribute("TotalRate", TotalRateOld));
                        foreach (var room in tvRoomList)
                        {
                            string eOcp = string.Empty;
                            if (string.IsNullOrEmpty(room.Attribute("ChildAge").Value))
                            {
                                eOcp = room.Attribute("Adult").Value;
                            }
                            else
                            {
                                eOcp = room.Attribute("Adult").Value + "-" + room.Attribute("ChildAge").Value;
                            }
                            JArray nightly = (JArray)preBook["occupancy_pricing"][eOcp]["nightly"];
                            JToken Fees = (JToken)preBook["occupancy_pricing"][eOcp]["fees"];
                            room.Element("RequestID").SetValue("");
                            var preRoom = new XElement("Room", new XAttribute("ID", room.Attribute("ID").Value), new XAttribute("SuppliersID", supplierid), new XAttribute("RoomSeq", roomSeq), new XAttribute("SessionID", ""), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("OccupancyID", ""),
                                          new XAttribute("OccupancyName", room.Attribute("OccupancyName").Value), new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", room.Attribute("MealPlanName").Value), new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", room.Attribute("PerNightRoomRate").Value),
                                          new XAttribute("TotalRoomRate", room.Attribute("TotalRoomRate").Value), new XAttribute("CancellationDate", ""), new XAttribute("CancellationAmount", ""), new XAttribute("isAvailable", true), new XAttribute("searchType", room.Element("searchType").Value),
                                          new XElement("RequestID", bookinglink),
                                          new XElement("Offers"), new XElement("PromotionList", new XElement("Promotions", room.Attribute("MealPlanID").Value)),
                                          new XElement("CancellationPolicy"), new XElement("Amenities", new XElement("Amenity")),
                                          new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                          GetSupplement(Fees),
                                          new XElement(getPriceBreakup(nights, room.Attribute("TotalRoomRate").Value.ModifyToDecimal())),
                                          new XElement("AdultNum", room.Attribute("Adult").Value),
                                          new XElement("ChildNum", string.IsNullOrEmpty(room.Attribute("ChildAge").Value) ? 0 : room.Attribute("ChildAge").Value.Split(',').Count()));

                            RoomType.Add(preRoom);
                            roomSeq++;
                        }
                        RoomType.Add(GetCxlPolicy(cxl_penalty, nights, TotalRateOld));
                        XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", preBookReq.Descendants("HotelID").FirstOrDefault().Value),
                                                new XElement("HotelName", preBookReq.Descendants("HotelName").FirstOrDefault().Value), new XElement("Status", true),
                                                new XElement("TermCondition", termsCondition), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"),
                                                new XElement("MapLink"), new XElement("DMC", dmc), new XElement("Currency", currency),
                                                new XElement("Offers"), new XElement("Rooms", RoomType)));

                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("NewPrice", ""), hoteldata)));

                    }
                    else if (status == "price_changed")
                    {
                        List<XElement> roomlst = new List<XElement>();
                        decimal totalRoomPrice = 0;
                        foreach (var room in preBookReq.Descendants("Room"))
                        {
                            room.Element("RequestID").SetValue("");

                            string eOcp = string.Empty;
                            if (string.IsNullOrEmpty(room.Attribute("ChildAge").Value))
                            {
                                eOcp = room.Attribute("Adult").Value;
                            }
                            else
                            {
                                eOcp = room.Attribute("Adult").Value + "-" + room.Attribute("ChildAge").Value;
                            }
                            
                            decimal roomprice = (decimal)preBook["occupancy_pricing"][eOcp]["totals"]["inclusive"]["request_currency"]["value"];
                            decimal marketingFee = (decimal)preBook["occupancy_pricing"][eOcp]["totals"]["marketing_fee"]["request_currency"]["value"];
                            roomprice = roomprice - marketingFee;
                            //JArray nightly = (JArray)preBook["occupancy_pricing"][eOcp]["nightly"];

                            JToken Fees = (JToken)preBook["occupancy_pricing"][eOcp]["fees"];
                            totalRoomPrice = totalRoomPrice + roomprice;

                            var preRoom = new XElement("Room", new XAttribute("ID", room.Attribute("ID").Value), new XAttribute("SuppliersID", supplierid), new XAttribute("RoomSeq", roomSeq), new XAttribute("SessionID", ""), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("OccupancyID", ""),
                                          new XAttribute("OccupancyName", room.Attribute("OccupancyName").Value), new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", room.Attribute("MealPlanName").Value), new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", Math.Round(roomprice / nights, 5)),
                                          new XAttribute("TotalRoomRate", roomprice), new XAttribute("CancellationDate", ""), new XAttribute("CancellationAmount", ""), new XAttribute("isAvailable", true), new XAttribute("searchType", room.Element("searchType").Value),
                                          new XElement("RequestID", bookinglink),
                                          new XElement("Offers"), new XElement("PromotionList", new XElement("Promotions", room.Attribute("MealPlanID").Value)),
                                          new XElement("CancellationPolicy"), new XElement("Amenities", new XElement("Amenity")),
                                          new XElement("Images", new XElement("Image", new XAttribute("Path", ""))), GetSupplement(Fees),
                                          new XElement(getPriceBreakup(nights, roomprice)),
                                          new XElement("AdultNum", room.Attribute("Adult").Value),
                                          new XElement("ChildNum", string.IsNullOrEmpty(room.Attribute("ChildAge").Value) ? 0 : room.Attribute("ChildAge").Value.Split(',').Count()));

                            roomlst.Add(preRoom);
                            roomSeq++;
                        }
                        XElement RoomType = new XElement("RoomTypes", new XAttribute("Index", "1"), new XAttribute("TotalRate", totalRoomPrice));
                        RoomType.Add(GetCxlPolicy(cxl_penalty, nights, totalRoomPrice));
                        XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", preBookReq.Descendants("HotelID").FirstOrDefault().Value),
                                                new XElement("HotelName", preBookReq.Descendants("HotelName").FirstOrDefault().Value), new XElement("Status", true),
                                                new XElement("TermCondition", termsCondition), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"),
                                                new XElement("MapLink"), new XElement("DMC", dmc), new XElement("Currency", currency),
                                                new XElement("Offers"), new XElement("Rooms", RoomType)));
                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Amount has been changed"), new XElement("NewPrice", totalRoomPrice), hoteldata)));

                    }
                    else if (status == "sold_out")
                    {
                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"))));

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
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "PreBooking";
                ex1.PageName = "ExpediaService";
                ex1.CustomerID = preBookReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = preBookReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion
                PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"))));

                return PreBookResponse;
            }
        }

        #region Supplement
        public XElement GetSupplement(JToken Fees)
        {
            decimal atPropertyFees = 0, mandatoryTax=0, mandatoryFees=0, resortFees=0;
            XElement supplements = new XElement("Supplements");
            if (Fees != null)
            {
                var mand_tax = Fees.SelectToken("mandatory_tax");
                mandatoryTax = mand_tax != null ? (decimal)Fees["mandatory_tax"]["request_currency"]["value"] : 0;
                var mand_fee = Fees.SelectToken("mandatory_fee");
                mandatoryFees = mand_fee != null ? (decimal)Fees["mandatory_fee"]["request_currency"]["value"] : 0;
                var resort_fee = Fees.SelectToken("resort_fee");
                resortFees = resort_fee != null ? (decimal)Fees["resort_fee"]["request_currency"]["value"] : 0;
                atPropertyFees = mandatoryTax + mandatoryFees + resortFees;
                if (atPropertyFees > 0)
                {
                    supplements.Add(new XElement("Supplement", new XAttribute("suppType", "PerRoomSupplement"),
                    new XAttribute("suppId", ""), new XAttribute("suppName", "Property Fee"), new XAttribute("supptType", ""),
                    new XAttribute("suppIsMandatory", "True"), new XAttribute("suppChargeType", "AtProperty"), new XAttribute("suppPrice", atPropertyFees)));
                }
            }
            return supplements;
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
                epsRequest = new ExpediaRequest();

                string Book_Request = endpoint + BookingReq.Descendants("Room").FirstOrDefault().Element("RequestID").Value;
                //string cust_email = BookingReq.Descendants("Room").FirstOrDefault().Descendants("PaxInfo").FirstOrDefault().Element("Email").Value;
                string cust_email = "s.hijazin@bookingexpress.co";
                var passangerObj = new
                {
                    affiliate_reference_id = BookingReq.Descendants("TransID").FirstOrDefault().Value,
                    hold = false,
                    email = cust_email,
                    phone = new { country_code = "1", area_code = "", number = BookingReq.Descendants("Room").FirstOrDefault().Descendants("PaxInfo").FirstOrDefault().Element("Phone").Value },
                    rooms = from room in BookingReq.Descendants("Room")
                            select new
                            {
                                given_name = room.Descendants("PaxInfo").FirstOrDefault().Element("FirstName").Value + " " + room.Descendants("PaxInfo").FirstOrDefault().Element("MiddleName").Value,
                                family_name = room.Descendants("PaxInfo").FirstOrDefault().Element("LastName").Value,
                                smoking = false,
                                special_request = BookingReq.Descendants("SpecialRemarks").FirstOrDefault().Value,
                                loyalty_id = ""
                            },

                    payments = new[] { 
                        new {
                        type="affiliate_collect",
                        billing_contact= new {
                        given_name=clientname.Split('-')[0],
                        family_name=clientname.Split('-').Length>1?clientname.Split('-')[1]:"",
                        address=new { line_1=clientaddress, line_2="",line_3="", city=clientcity, state_province_code="",postal_code=postalcode,country_code=clientcountry }
                        }, 
                        enrollment_date = DateTime.Now.ToString("yyyy-MM-dd")} 
                    },
                    affiliate_metadata = "",
                    tax_registration_number = "",
                    traveler_handling_instructions = ""
                };
               
                string passangerDetails = JsonConvert.SerializeObject(passangerObj);
                                
                EpsLogModel logmodel = new EpsLogModel()
                {
                    TrackNo = BookingReq.Descendants("TransactionID").FirstOrDefault().Value,
                    CustomerID = BookingReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Book",
                    LogTypeID = 5
                };
               
                string respBook = epsRequest.ServerBookRequest(Book_Request, apikey, secret, logmodel, passangerDetails,"POST");

                if (!string.IsNullOrEmpty(respBook) && respBook != "[]")
                {
                    if (respBook.StartsWith("{") && respBook.EndsWith("}"))
                    {
                        JObject bookObj = (JObject)JsonConvert.DeserializeObject(respBook);
                        var type = bookObj["type"];
                        if (type != null)
                        {
                            string msg = (string)bookObj["message"];
                            HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", msg))));

                        }
                        else
                        {

                            string itinerary_id = (string)bookObj["itinerary_id"];

                            string retrieveLink = (string)bookObj["links"]["retrieve"]["href"];
                            if (!string.IsNullOrEmpty(retrieveLink))
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(10));
                                string bookingDetail = epsRequest.ServerRequest(endpoint + retrieveLink, apikey, secret, logmodel);
                                if (bookingDetail.StartsWith("{") && bookingDetail.EndsWith("}"))
                                {
                                    JObject bookDtlObj = (JObject)JsonConvert.DeserializeObject(bookingDetail);

                                    if (bookDtlObj["type"] != null)
                                    {
                                        string msg = (string)bookObj["message"];
                                        HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", msg))));
                                    }
                                    else
                                    {
                                        decimal amount = 0.0m;
                                        string bokStatus = string.Empty;
                                        foreach (var room in bookDtlObj["rooms"])
                                        {
                                            bokStatus = (string)room["status"];
                                        }
                                        try
                                        {
                                            foreach (var room in bookDtlObj["rooms"])
                                            {
                                                amount = amount + (decimal)room["rate"]["pricing"]["totals"]["inclusive"]["billable_currency"]["value"];

                                            }
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
                                                       new XElement("CurrencyCode", currency),
                                                       new XElement("MarketID"), new XElement("MarketName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("VoucherRemark"),
                                                       new XElement("TransID", BookingReq.Descendants("TransID").FirstOrDefault().Value),
                                                       new XElement("ConfirmationNumber", itinerary_id),
                                                       new XElement("Status", bokStatus == "booked" ? "Success" : "Fail"),
                                                       new XElement("PassengersDetail", new XElement("GuestDetails",
                                                       from room in BookingReq.Descendants("Room")
                                                       select new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeID").Value), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("ServiceID", ""), new XAttribute("RefNo", ""),
                                                       new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", ""),
                                                       new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", ""),
                                                       new XAttribute("RoomStatus", "true"), new XAttribute("TotalRoomRate", ""),
                                                       new XElement("RoomGuest", new XElement("GuestType", "Adult"), new XElement("Title"), new XElement("FirstName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("FirstName").FirstOrDefault().Value),
                                                       new XElement("MiddleName"), new XElement("LastName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("LastName").FirstOrDefault().Value), new XElement("IsLead", "true"), new XElement("Age")),
                                                       new XElement("Supplements"))
                                                       ))));

                                        HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, BookingRes));
                                    }
                                }
                                else
                                {
                                    Thread.Sleep(TimeSpan.FromSeconds(10));
                                    bookingDetail = epsRequest.ServerRequest(endpoint + retrieveLink, apikey, secret, logmodel);
                                    if (bookingDetail.StartsWith("{") && bookingDetail.EndsWith("}"))
                                    {
                                        JObject bookDtlObj = (JObject)JsonConvert.DeserializeObject(bookingDetail);

                                        if (bookDtlObj["type"] != null)
                                        {
                                            string msg = (string)bookObj["message"];
                                            HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", msg))));
                                        }
                                        else
                                        {
                                            decimal amount = 0.0m;
                                            string bokStatus = string.Empty;
                                            foreach (var room in bookDtlObj["rooms"])
                                            {
                                                bokStatus = (string)room["status"];
                                            }
                                            try
                                            {
                                                foreach (var room in bookDtlObj["rooms"])
                                                {
                                                    amount = amount + (decimal)room["rate"]["pricing"]["totals"]["inclusive"]["billable_currency"]["value"];
                                                }
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
                                                           new XElement("CurrencyCode", currency),
                                                           new XElement("MarketID"), new XElement("MarketName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("VoucherRemark"),
                                                           new XElement("TransID", BookingReq.Descendants("TransID").FirstOrDefault().Value),
                                                           new XElement("ConfirmationNumber", itinerary_id),
                                                           new XElement("Status", bokStatus == "booked" ? "Success" : "Fail"),
                                                           new XElement("PassengersDetail", new XElement("GuestDetails",
                                                           from room in BookingReq.Descendants("Room")
                                                           select new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeID").Value), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("ServiceID", ""), new XAttribute("RefNo", ""),
                                                           new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", ""),
                                                           new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", ""),
                                                           new XAttribute("RoomStatus", "true"), new XAttribute("TotalRoomRate", ""),
                                                           new XElement("RoomGuest", new XElement("GuestType", "Adult"), new XElement("Title"), new XElement("FirstName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("FirstName").FirstOrDefault().Value),
                                                           new XElement("MiddleName"), new XElement("LastName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("LastName").FirstOrDefault().Value), new XElement("IsLead", "true"), new XElement("Age")),
                                                           new XElement("Supplements"))
                                                           ))));

                                            HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, BookingRes));
                                        }
                                    }
                                    else
                                    {

                                        // HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", bookingDetail))));

                                        Thread.Sleep(TimeSpan.FromSeconds(10));
                                        bookingDetail = epsRequest.ServerRequest(endpoint + retrieveLink, apikey, secret, logmodel);
                                        if (bookingDetail.StartsWith("{") && bookingDetail.EndsWith("}"))
                                        {
                                            JObject bookDtlObj = (JObject)JsonConvert.DeserializeObject(bookingDetail);

                                            if (bookDtlObj["type"] != null)
                                            {
                                                string msg = (string)bookObj["message"];
                                                HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", msg))));
                                            }
                                            else
                                            {
                                                decimal amount = 0.0m;
                                                string bokStatus = string.Empty;
                                                foreach (var room in bookDtlObj["rooms"])
                                                {
                                                    bokStatus = (string)room["status"];
                                                }
                                                try
                                                {
                                                    foreach (var room in bookDtlObj["rooms"])
                                                    {
                                                        amount = amount + (decimal)room["rate"]["pricing"]["totals"]["inclusive"]["billable_currency"]["value"];
                                                    }
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
                                                               new XElement("CurrencyCode", currency),
                                                               new XElement("MarketID"), new XElement("MarketName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("VoucherRemark"),
                                                               new XElement("TransID", BookingReq.Descendants("TransID").FirstOrDefault().Value),
                                                               new XElement("ConfirmationNumber", itinerary_id),
                                                               new XElement("Status", bokStatus == "booked" ? "Success" : "Fail"),
                                                               new XElement("PassengersDetail", new XElement("GuestDetails",
                                                               from room in BookingReq.Descendants("Room")
                                                               select new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeID").Value), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("ServiceID", ""), new XAttribute("RefNo", ""),
                                                               new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", ""),
                                                               new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", ""),
                                                               new XAttribute("RoomStatus", "true"), new XAttribute("TotalRoomRate", ""),
                                                               new XElement("RoomGuest", new XElement("GuestType", "Adult"), new XElement("Title"), new XElement("FirstName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("FirstName").FirstOrDefault().Value),
                                                               new XElement("MiddleName"), new XElement("LastName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("LastName").FirstOrDefault().Value), new XElement("IsLead", "true"), new XElement("Age")),
                                                               new XElement("Supplements"))
                                                               ))));

                                                HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, BookingRes));
                                            }

                                        }
                                        else
                                        {
                                            Thread.Sleep(TimeSpan.FromSeconds(10));
                                            bookingDetail = epsRequest.ServerRequest(endpoint + retrieveLink, apikey, secret, logmodel);
                                            if (bookingDetail.StartsWith("{") && bookingDetail.EndsWith("}"))
                                            {
                                                JObject bookDtlObj = (JObject)JsonConvert.DeserializeObject(bookingDetail);

                                                if (bookDtlObj["type"] != null)
                                                {
                                                    string msg = (string)bookObj["message"];
                                                    HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", msg))));
                                                }
                                                else
                                                {
                                                    decimal amount = 0.0m;
                                                    string bokStatus = string.Empty;
                                                    foreach (var room in bookDtlObj["rooms"])
                                                    {
                                                        bokStatus = (string)room["status"];
                                                    }
                                                    try
                                                    {
                                                        foreach (var room in bookDtlObj["rooms"])
                                                        {
                                                            amount = amount + (decimal)room["rate"]["pricing"]["totals"]["inclusive"]["billable_currency"]["value"];
                                                        }
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
                                                                   new XElement("CurrencyCode", currency),
                                                                   new XElement("MarketID"), new XElement("MarketName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("VoucherRemark"),
                                                                   new XElement("TransID", BookingReq.Descendants("TransID").FirstOrDefault().Value),
                                                                   new XElement("ConfirmationNumber", itinerary_id),
                                                                   new XElement("Status", bokStatus == "booked" ? "Success" : "Fail"),
                                                                   new XElement("PassengersDetail", new XElement("GuestDetails",
                                                                   from room in BookingReq.Descendants("Room")
                                                                   select new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeID").Value), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("ServiceID", ""), new XAttribute("RefNo", ""),
                                                                   new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", ""),
                                                                   new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", ""),
                                                                   new XAttribute("RoomStatus", "true"), new XAttribute("TotalRoomRate", ""),
                                                                   new XElement("RoomGuest", new XElement("GuestType", "Adult"), new XElement("Title"), new XElement("FirstName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("FirstName").FirstOrDefault().Value),
                                                                   new XElement("MiddleName"), new XElement("LastName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("LastName").FirstOrDefault().Value), new XElement("IsLead", "true"), new XElement("Age")),
                                                                   new XElement("Supplements"))
                                                                   ))));

                                                    HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, BookingRes));
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(TimeSpan.FromSeconds(10));
                                                bookingDetail = epsRequest.ServerRequest(endpoint + retrieveLink, apikey, secret, logmodel);
                                                if (bookingDetail.StartsWith("{") && bookingDetail.EndsWith("}"))
                                                {
                                                    JObject bookDtlObj = (JObject)JsonConvert.DeserializeObject(bookingDetail);

                                                    if (bookDtlObj["type"] != null)
                                                    {
                                                        string msg = (string)bookObj["message"];
                                                        HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", msg))));
                                                    }
                                                    else
                                                    {
                                                        decimal amount = 0.0m;
                                                        string bokStatus = string.Empty;
                                                        foreach (var room in bookDtlObj["rooms"])
                                                        {
                                                            bokStatus = (string)room["status"];
                                                        }
                                                        try
                                                        {
                                                            foreach (var room in bookDtlObj["rooms"])
                                                            {
                                                                amount = amount + (decimal)room["rate"]["pricing"]["totals"]["inclusive"]["billable_currency"]["value"];
                                                            }
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
                                                                       new XElement("CurrencyCode", currency),
                                                                       new XElement("MarketID"), new XElement("MarketName"), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("VoucherRemark"),
                                                                       new XElement("TransID", BookingReq.Descendants("TransID").FirstOrDefault().Value),
                                                                       new XElement("ConfirmationNumber", itinerary_id),
                                                                       new XElement("Status", bokStatus == "booked" ? "Success" : "Fail"),
                                                                       new XElement("PassengersDetail", new XElement("GuestDetails",
                                                                       from room in BookingReq.Descendants("Room")
                                                                       select new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeID").Value), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("ServiceID", ""), new XAttribute("RefNo", ""),
                                                                       new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", ""),
                                                                       new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", ""),
                                                                       new XAttribute("RoomStatus", "true"), new XAttribute("TotalRoomRate", ""),
                                                                       new XElement("RoomGuest", new XElement("GuestType", "Adult"), new XElement("Title"), new XElement("FirstName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("FirstName").FirstOrDefault().Value),
                                                                       new XElement("MiddleName"), new XElement("LastName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("LastName").FirstOrDefault().Value), new XElement("IsLead", "true"), new XElement("Age")),
                                                                       new XElement("Supplements"))
                                                                       ))));

                                                        HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, BookingRes));
                                                    }
                                                }
                                                else {
                                                     HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", bookingDetail))));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", respBook))));
                    }
                }
                else
                {
                    HotelBookingRes.Add(new XElement(soapenv + "Body", BookReq, new XElement("HotelBookingResponse", new XElement("ErrorTxt", "No response from supplier!"))));
                }
                return HotelBookingRes;
            }
            catch{
                return HotelBookingRes;
            }
        }

        
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

                epsRequest = new ExpediaRequest();
                string itinerary_id = cancelReq.Descendants("ConfirmationNumber").FirstOrDefault().Value;
                //string email = cancelReq.Descendants("BookingCode").FirstOrDefault().Value;
                string email = "s.hijazin@bookingexpress.co";

                string cxkRequest = endpoint + "/"+version+"/itineraries/" + itinerary_id + "?email=" + email;
                EpsLogModel logmodel = new EpsLogModel()
                {
                    TrackNo = cancelReq.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = cancelReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Cancel",
                    LogTypeID = 6
                };

                string bookingDetail = epsRequest.ServerRequest(cxkRequest, apikey, secret, logmodel);
                if (bookingDetail.StartsWith("{") && bookingDetail.EndsWith("}"))
                {
                    JObject bookDtlObj = (JObject)JsonConvert.DeserializeObject(bookingDetail);

                    if (bookDtlObj["type"] != null)
                    {
                        BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", (string)bookDtlObj["message"]))));
                        return BookCXlRes;

                    }
                    else
                    {
                        bool isCancel = false;
                        string err = string.Empty;
                        foreach (var roomobj in bookDtlObj["rooms"])
                        {
                            string cxlLink = endpoint + (string)roomobj["links"]["cancel"]["href"];
                            string cxLResp = epsRequest.ServerBookRequest(cxlLink, apikey, secret, logmodel, "", "DELETE");
                            if (string.IsNullOrEmpty(cxLResp))
                            {
                                isCancel = true;
                            }
                            else
                            {
                                JObject cxlError = (JObject)JsonConvert.DeserializeObject(cxLResp);
                                if (cxlError["type"] != null)
                                {
                                    err = err + (string)cxlError["message"];
                                }
                                isCancel = false;
                            }
                        }
                            if (isCancel)
                            {
                                string status = string.Empty;
                                string bokDetail = epsRequest.ServerRequest(cxkRequest, apikey, secret, logmodel);
                                if (bookingDetail.StartsWith("{") && bookingDetail.EndsWith("}"))
                                {
                                    JObject bookObj = (JObject)JsonConvert.DeserializeObject(bokDetail);
                                    foreach (var rm in bookObj["rooms"])
                                    {
                                        status = (string)rm["status"];
                                    }
                                }
                                BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("Rooms", new XElement("Room", new XElement("Cancellation", new XElement("Amount", "0.00"), new XElement("Status", status == "canceled" ? "Success" : "Fail")))))));
                            }
                            else
                            {
                                BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", err))));

                            }
                        
                    }
                }
                else
                {
                    BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", bookingDetail))));
                    return BookCXlRes;
                }
                return BookCXlRes;
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "BookingCancellation";
                ex1.PageName = "Expedia";
                ex1.CustomerID = cancelReq.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = cancelReq.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", "There is some technical error"))));
                return BookCXlRes;
            }
        }
        #endregion
 
    }
}