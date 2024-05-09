using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Models;

namespace TravillioXMLOutService.Supplier.IOL
{
    
    public class IOLService : IDisposable
    {
        #region Credentails
        string iolCurrency = string.Empty;
        string iolOutputFormat = string.Empty;
        string iolPassword = string.Empty;
        string iolCode = string.Empty;
        string iolsearch = string.Empty;
        string iolprebook = string.Empty;
        string iolbook = string.Empty;
        string iolcancel = string.Empty;
        string iolretrieve = string.Empty;
        #endregion
        #region Global vars
        string customerid = string.Empty;
        string dmc = string.Empty;
        const int supplierid = 50;
        int chunksize = 50;
        int sup_cutime = 20, threadCount = 2;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        IOLRequest iolRequest;
        XNamespace xmlns = "http://schemas.xmlsoap.org/soap/envelope/";
        string sales_environment = "hotel_package";
        #endregion
        public IOLService(string _customerid)
        {
            XElement suppliercred = supplier_Cred.getsupplier_credentials(_customerid, "50");
            try
            {
                iolOutputFormat = suppliercred.Descendants("OutputFormat").FirstOrDefault().Value;
                iolPassword = suppliercred.Descendants("Password").FirstOrDefault().Value;
                iolCode = suppliercred.Descendants("Code").FirstOrDefault().Value;
                iolsearch = suppliercred.Descendants("search").FirstOrDefault().Value;
                iolprebook = suppliercred.Descendants("prebook").FirstOrDefault().Value;
                iolbook = suppliercred.Descendants("book").FirstOrDefault().Value;
                iolcancel = suppliercred.Descendants("cancel").FirstOrDefault().Value;
                iolretrieve = suppliercred.Descendants("retrieve").FirstOrDefault().Value;
            }
            catch { }
        }
        public IOLService()
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
                IOLStatic iolStatic = new IOLStatic();
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
                //DataTable wteCity = iolStatic.GetCityCode(req.Descendants("CityID").FirstOrDefault().Value, req.Descendants("CountryID").FirstOrDefault().Value, 50);
                //bool isCityMapped = true;
                //string destinationcity = string.Empty;
                string countryCode = string.Empty;
                //DataTable iolHotels = new DataTable();
                //if (wteCity != null)
                //{
                //    if (wteCity.Rows.Count != 0)
                //    {
                //        for (int k = 0; k < wteCity.Rows.Count;k++ )
                //        {
                //            destinationcity =wteCity.Rows[k]["CityID"].ToString();
                //            countryCode = wteCity.Rows[k]["CountryCode"].ToString();

                //            iolHotels.Merge( iolStatic.GetStaticHotels(req.Descendants("HotelID").FirstOrDefault().Value, req.Descendants("HotelName").FirstOrDefault().Value, destinationcity, countryCode, req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt(), req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt()));
                //        }
                //    }
                //    else
                //    {
                //        isCityMapped = false;
                //    }
                //}

                DataTable iolHotels = iolStatic.GetStaticHotels(req.Descendants("HotelID").FirstOrDefault().Value, req.Descendants("HotelName").FirstOrDefault().Value, req.Descendants("CityID").FirstOrDefault().Value, req.Descendants("CountryID").FirstOrDefault().Value, req.Descendants("MinStarRating").FirstOrDefault().Value.ModifyToInt(), req.Descendants("MaxStarRating").FirstOrDefault().Value.ModifyToInt());

                if (iolHotels == null || iolHotels.Rows.Count == 0)
                {
                    #region No hotel found exception
                    CustomException ex1 = new CustomException("There is no hotel available in database");
                    ex1.MethodName = "HotelAvailability";
                    ex1.PageName = "IOLService";
                    ex1.CustomerID = customerid.ToString();
                    ex1.TranID = req.Descendants("TransID").First().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    #endregion
                    return null;
                }
                var statiProperties = iolHotels.AsEnumerable().Select(r => r.Field<string>("hotelcode")).ToList();
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
                                    new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], iolHotels,timeOut)),
                                   
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
                                   new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], iolHotels,timeOut)),
                                   new Thread(()=> thr2 =  SearchHotel(req,countryCode, splitList[i+1], iolHotels,timeOut)),
                                  
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
                                    new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], iolHotels,timeOut )),
                                    new Thread(()=> thr2 = SearchHotel(req,countryCode, splitList[i+1], iolHotels,timeOut)),
                                    new Thread(()=> thr3 = SearchHotel(req,countryCode, splitList[i+2], iolHotels,timeOut)),
                                      
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
                                    new Thread(()=> thr1 = SearchHotel(req,countryCode, splitList[i], iolHotels,timeOut )),
                                    new Thread(()=> thr2 = SearchHotel(req,countryCode, splitList[i+1], iolHotels,timeOut)),
                                    new Thread(()=> thr3 = SearchHotel(req,countryCode, splitList[i+2], iolHotels,timeOut)),
                                     new Thread(()=> thr4 = SearchHotel(req,countryCode, splitList[i+2], iolHotels,timeOut)),
                                      
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
                    ex1.PageName = "IOLService";
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
                ex1.PageName = "IOLService";
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
        private List<XElement> SearchHotel(XElement req, string countryCode, List<string> propertyChunk, DataTable iolStaticHotels, int timeout)
        {
            
            List<XElement> ThHotelsList = new List<XElement>();
            try
            {
                string genguid = string.Empty;
                genguid = Guid.NewGuid().ToString();
                DateTime fdt = DateTime.ParseExact(req.Descendants("FromDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = (fdt.ToString("yyyyMMdd"));
                DateTime tdt = DateTime.ParseExact(req.Descendants("ToDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string tdate = (tdt.ToString("yyyyMMdd"));

                var result_hotelids = string.Join(",", propertyChunk);
                XElement suprequest = new XElement("HotelSearchRequest",
                    new XElement("OutputFormat", iolOutputFormat),
                    new XElement("Profile", new XElement("Password", iolPassword), new XElement("Code", iolCode), new XElement("TokenNumber", req.Descendants("TransID").FirstOrDefault().Value)),
                    new XElement("SearchCriteria",
                        new XElement("RoomConfiguration", roomGuests(req.Descendants("Rooms").FirstOrDefault())),
                        new XElement("StartDate", fdate),
                        new XElement("EndDate", tdate),
                        new XElement("Nationality", req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value),
                        new XElement("IncludeOnRequest", "N"),
                        new XElement("GroupByRooms", "Y"),
                        new XElement("CancellationPolicy", "N"),
                        new XElement("HotelCode", result_hotelids)
                        )
                    );                   
                
                IOLLogModel logmodel = new IOLLogModel()
                {
                    TrackNo = req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = customerid.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Search",
                    LogTypeID = 1
                };
                IOLRequest iolreqobj = new IOLRequest();
                XDocument Response = iolreqobj.Request(suprequest, iolsearch, genguid, logmodel);

                XElement suplResponse = removeAllNamespaces(Response.Root);
                int rumcount = req.Descendants("RoomPax").Count();
                foreach (XElement hotel in suplResponse.Descendants("Hotel"))
                {
                    try
                    {
                        DataRow[] htlSt = iolStaticHotels.Select("[hotelcode] = '" + hotel.Element("HotelCode").Value + "'");
                        if (htlSt.Length > 0)
                        {
                            var HotelData = htlSt[0];
                            iolCurrency = hotel.Descendants("Room").FirstOrDefault().Element("CurrCode").Value;
                            #region minimum rate
                            decimal minrate = 0;
                            if (rumcount == 1)
                            {
                                List<XElement> ratelst = hotel.Descendants("TotalRate").OrderBy(x => Convert.ToDecimal(x.Value)).ToList();
                                minrate = Convert.ToDecimal(ratelst.FirstOrDefault().Value);
                            }
                            else 
                            {
                                var expr = from d in hotel.Descendants("Room")
                                           group d by new
                                           {
                                               RoomTypeCode = d.Element("RoomTypeCode").Value,
                                               MealPlanCode = d.Element("MealPlanCode").Value,
                                               ContractTokenId = d.Element("ContractTokenId").Value,
                                           } into gDir
                                           select new
                                           {
                                               GroupKey = gDir.Key,
                                               Data = from z in gDir
                                                      select new { TotalRate = z.Element("TotalRate").Value }
                                           };

                                var tt = expr.Min(x => x.Data.Sum(y => Convert.ToDecimal(y.TotalRate)));
                                minrate = Convert.ToDecimal(tt);
                            }
                            #endregion

                            XElement hoteldata = new XElement("Hotel", new XElement("HotelID", HotelData["hotelcode"].ToString().ConvertHotelCodewith_underscoreIOL()),
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
                                                new XElement("RequestID", genguid),
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
                                                new XElement("xmlouttype", "false"),
                                                new XElement("DMC", dmc), new XElement("SupplierID", supplierid),
                                                new XElement("Currency", iolCurrency),
                                                new XElement("Offers", ""), new XElement("Facilities", null),
                                                new XElement("Rooms", "")
                                                );

                            ThHotelsList.Add(hoteldata);
                        }
                    }
                    catch(Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "SearchHotel";
                ex1.PageName = "IOLService";
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
        public XElement GetRoomAvail_iolOUT(XElement req, XElement iolRoomsMealPlansfile, int supID)
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
                        if (supID == 50)
                        {
                            dmc = "IOL";
                        }
                    }
                    roomavailabilityresponse.Add(RoomAvailability(req, dmc, htlid, iolRoomsMealPlansfile, customerid));
                }
                #endregion
                getrm = new XElement("TotalRooms", roomavailabilityresponse);
                return getrm;
            }
            catch { return null; }
        }
        public XElement RoomAvailability(XElement req, string SupplierType, string htlID, XElement iolRoomsMealPlansfile, string custIDs)
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
                XElement giatahtllst = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "50").FirstOrDefault();
                string genguid = string.Empty;
                genguid = Guid.NewGuid().ToString();
                string hotelid = giatahtllst.Attribute("GHtlID").Value.ConvertHotelCodewith_dashIOL();
                DateTime fdt = DateTime.ParseExact(req.Descendants("FromDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = (fdt.ToString("yyyyMMdd"));
                DateTime tdt = DateTime.ParseExact(req.Descendants("ToDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string tdate = (tdt.ToString("yyyyMMdd"));
                int totnights = (int)(tdt - fdt).TotalDays;
                XElement suprequest = new XElement("HotelSearchRequest",
                    new XElement("OutputFormat", iolOutputFormat),
                    new XElement("Profile", new XElement("Password", iolPassword), new XElement("Code", iolCode), new XElement("TokenNumber", req.Descendants("TransID").FirstOrDefault().Value)),
                    new XElement("SearchCriteria",
                        new XElement("RoomConfiguration", roomGuests(req.Descendants("Rooms").FirstOrDefault())),
                        new XElement("StartDate", fdate),
                        new XElement("EndDate", tdate),
                        new XElement("Nationality", req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value),
                        new XElement("IncludeOnRequest", "N"),
                        new XElement("IncludeRateDetails", "Y"),
                        new XElement("GroupByRooms", "Y"),
                        new XElement("CancellationPolicy", "Y"),
                        new XElement("HotelCode", hotelid)
                        )
                    );
                IOLLogModel logmodel = new IOLLogModel()
                {
                    TrackNo = req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = custIDs.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "RoomAvail",
                    LogTypeID = 2

                };

                IOLRequest iolreqobj = new IOLRequest();
                XDocument Response = iolreqobj.Request(suprequest, iolsearch, genguid, logmodel);

                XElement suplResponse = removeAllNamespaces(Response.Root);
                List<XElement> strgrp = new List<XElement>();
                List<XElement> roompax = req.Descendants("RoomPax").ToList();
                int totrum = roompax.Count();
                int rumcomindx = 1;
                foreach (XElement hotel in suplResponse.Descendants("Hotel"))
                {
                    try
                    {
                        if (totrum == 1)
                        {
                            #region one room
                            try
                            {
                                
                                foreach(XElement room in hotel.Descendants("Room"))
                                {
                                    List<XElement> str = new List<XElement>();
                                    int rmct = 0;
                                    if (room.Element("RoomStatus").Value == "OK")
                                    {
                                        string mealcode = string.Empty;
                                        string currency = string.Empty;
                                        try
                                        {
                                            currency = room.Element("CurrCode").Value;
                                            XElement mealel = iolRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.Element("MealPlan").Value).FirstOrDefault();
                                            mealcode = mealel.Attribute("B2BCode").Value;
                                        }
                                        catch { }
                                        decimal totalamt = 0;
                                        IEnumerable<XElement> totroomprc = null;
                                        try
                                        {
                                            //int isbrkup = room.Descendants("RateDetails").FirstOrDefault().Descendants("Rate").Count();
                                            //if (isbrkup > 0)
                                            //{
                                            //    totroomprc = GetRoomsPriceBreakup(room.Descendants("RateDetails").FirstOrDefault().Descendants("Rate").ToList());
                                            //    totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                            //}
                                            //else
                                            //{
                                                totroomprc = GetRoomsPriceBreakup_fromTotal(Convert.ToDecimal(room.Descendants("TotalRate").FirstOrDefault().Value), totnights);
                                                totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                            //}
                                        }
                                        catch
                                        {
                                            totroomprc = GetRoomsPriceBreakup_fromTotal(Convert.ToDecimal(room.Descendants("TotalRate").FirstOrDefault().Value), totnights);
                                            totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                        }
                                        
                                        string searchtype = string.Empty;
                                        try
                                        {
                                            if (room.Element("NonRefundable").Value == "Y")
                                            {
                                                searchtype = "hotel_package";
                                            }
                                        }
                                        catch { }
                                        str.Add(new XElement("Room",
                                                                 new XAttribute("ID", Convert.ToString(room.Element("RoomTypeCode").Value)),
                                                                 new XAttribute("SuppliersID", "50"),
                                                                 new XAttribute("RoomSeq", 1),
                                                                 new XAttribute("SessionID", Convert.ToString(room.Element("ContractTokenId").Value)),
                                                                 new XAttribute("RoomType", Convert.ToString(room.Element("RoomType").Value)),
                                                                 new XAttribute("OccupancyID", Convert.ToString(room.Element("RoomConfigurationId").Value)),
                                                                 new XAttribute("OccupancyName", Convert.ToString("")),
                                                                 new XAttribute("MealPlanID", Convert.ToString(room.Element("MealPlanCode").Value)),
                                                                 new XAttribute("MealPlanName", Convert.ToString(room.Element("MealPlan").Value)),
                                                                 new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                                                 new XAttribute("MealPlanPrice", ""),
                                                                 new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                                 new XAttribute("TotalRoomRate", Convert.ToString(totalamt)),
                                                                 new XAttribute("CancellationDate", ""),
                                                                 new XAttribute("CancellationAmount", ""),
                                                                 new XAttribute("isAvailable", "true"),
                                                                 new XAttribute("searchType", searchtype),
                                                                 new XElement("RequestID", Convert.ToString(genguid)),
                                                                 new XElement("Offers", ""),
                                                                 new XElement("PromotionList", new XElement("Promotions", room.Element("ContractLabel").Value)),
                                                                 new XElement("CancellationPolicy", ""),
                                                                 new XElement("Amenities", new XElement("Amenity", "")),
                                                                 new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                                 new XElement("Supplements", ""),
                                                                     new XElement("PriceBreakups", totroomprc),
                                                                     new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                                                     new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                                                 ));

                                        strgrp.Add(new XElement("RoomTypes", new XAttribute("Index", rumcomindx), new XAttribute("TotalRate", Convert.ToString(totalamt)), new XAttribute("HtlCode", hotelid.ConvertHotelCodewith_underscoreIOL()), new XAttribute("CrncyCode", currency), new XAttribute("DMCType", dmc), new XAttribute("CUID", customerid),
                                                     str));
                                        rumcomindx++;
                                    }

                                }
                            }
                            catch { }
                            #endregion
                        }
                        else
                        {
                            #region Multi room

                            

                            var expr = from d in hotel.Descendants("Room")
                                       group d by new
                                       {
                                           RoomTypeCode = d.Element("RoomTypeCode").Value,
                                           MealPlanCode = d.Element("MealPlanCode").Value,
                                           ContractTokenId = d.Element("ContractTokenId").Value,
                                           //RoomConfigurationId = d.Element("RoomConfigurationId").Value,
                                       }
                                      
                                       into gDir

                                       select new
                                       {
                                           GroupKey = gDir.Key,
                                           Data = from z in gDir
                                                  select new
                                                  {
                                                      RoomNo = z.Element("RoomNo").Value,
                                                      RoomType = z.Element("RoomType").Value,
                                                      RoomTypeCode = z.Element("RoomTypeCode").Value,
                                                      //MealPlanSupplierCode = z.Element("MealPlanSupplierCode").Value,
                                                      RoomStatus = z.Element("RoomStatus").Value,
                                                      CurrCode = z.Element("CurrCode").Value,
                                                      ContractTokenId = z.Element("ContractTokenId").Value,
                                                      RoomConfigurationId = z.Element("RoomConfigurationId").Value,
                                                      MealPlan = z.Element("MealPlan").Value,
                                                      MealPlanCode = z.Element("MealPlanCode").Value,
                                                      PackageYN = z.Element("PackageYN").Value,
                                                      NonRefundable = z.Element("NonRefundable").Value,
                                                      Rates = from r in z.Element("RateDetails").Descendants("Rate")
                                                              select new
                                                              {
                                                                  Rate = r.Value
                                                              },
                                                      TotalRate = z.Element("TotalRate").Value,
                                                      ContractLabel = z.Element("ContractLabel").Value,
                                                  }
                                       };


                            //var combo = from l1 in hotel.Descendants("Room").Where(x => x.Element("RoomConfigurationId").Value=="1")
                            //            from l2 in hotel.Descendants("Room").Where(x => x.Element("RoomConfigurationId").Value == "2")
                            //            select new { l1, l2 };

                           //var roomsss = hotel.Descendants("Room")
                           //                        .Select(room => new
                           //                        {
                           //                            mealPlanCode = room.Element("MealPlanCode")==null?"":room.Element("MealPlanCode").Value,
                           //                            RoomConfigurationId = room.Element("RoomConfigurationId") == null ? "" : room.Element("RoomConfigurationId").Value,
                           //                            RoomType = room.Element("RoomType") == null ? "" : room.Element("RoomType").Value,
                           //                            ContractTokenId = room.Element("ContractTokenId") == null ? "" : room.Element("ContractTokenId").Value,
                           //                            RoomTypeCode = room.Element("RoomTypeCode") == null ? "" : room.Element("RoomTypeCode").Value,
                           //                            MealPlan = room.Element("MealPlan") == null ? "" : room.Element("MealPlan").Value
                           //                        });


                           //var rooms = hotel.Descendants("Room").ToList();
                           //var groupedRooms = rooms.GroupBy(room => room.Element("MealPlanCode").Value);
                           //var rootElement = new XElement("Rooms");
                           //foreach (var group in groupedRooms)
                           //{
                           //    var combinationGroups = GetUniqueCombinations(group.ToList());
                           //    foreach (var combination in combinationGroups)
                           //    {
                           //        var combinationGroup = new XElement("CombinationGroup");
                           //        combination.OrderBy(room => int.Parse(room.Element("RoomConfigurationId").Value)).ToList().ForEach(room => combinationGroup.Add(room));
                           //        rootElement.Add(combinationGroup);
                           //    }
                           //}
                           var rooms = hotel.Descendants("Room").ToList();
                           var groupedRooms = rooms.GroupBy(room => room.Element("MealPlanCode").Value).ToList();

                           // Get maximum RoomConfigurationId for each group
                           var maxIds = groupedRooms.Select(group => group.Max(room => int.Parse(room.Element("RoomConfigurationId").Value))).ToList();

                           // Filter groups with less combinations than max RoomConfigurationId
                           groupedRooms = groupedRooms.Where((group, index) => group.Count() >= maxIds[index]).ToList();

                           // Create combination groups and add rooms
                           var rootElement = new XElement("Rooms");
                           foreach (var group in groupedRooms)
                           {
                               var combinationGroups = GenerateCombinations(group.ToList(), new List<XElement>(), 1);
                               foreach (var combination in combinationGroups)
                               {
                                   if (IsUniqueRoomTypeCombination(combination))
                                   {
                                       var combinationGroup = new XElement("CombinationGroup");
                                       // Order rooms by RoomConfigurationId before adding
                                       combination.OrderBy(room => int.Parse(room.Element("RoomConfigurationId").Value)).ToList().ForEach(room => combinationGroup.Add(room));
                                       rootElement.Add(combinationGroup);
                                   }
                               }
                           }


                            foreach (var g in expr)
                            {
                                try
                                {
                                    //List<XElement> rmlst = hotel.Descendants("Room").Where(x => x.Element("RoomTypeCode").Value== g.GroupKey.RoomTypeCode.ToString()
                                    //    && x.Element("MealPlanCode").Value == g.GroupKey.MealPlanCode.ToString()
                                    //    && x.Element("ContractTokenId").Value == g.GroupKey.ContractTokenId.ToString()).ToList();
                                    List<XElement> str = new List<XElement>();
                                    int rmct = 0;
                                    decimal totrmrate = 0;
                                    string currency = string.Empty;
                                    foreach (var room in g.Data)
                                    {
                                        try
                                        {
                                            if (room.RoomStatus == "OK")
                                            {
                                                string mealcode = string.Empty;
                                                try
                                                {
                                                    currency = room.CurrCode;
                                                    XElement mealel = iolRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.MealPlan).FirstOrDefault();
                                                    mealcode = mealel.Attribute("B2BCode").Value;
                                                }
                                                catch { }
                                                decimal totalamt = 0;
                                                IEnumerable<XElement> totroomprc = null;
                                                try
                                                {
                                                    //bool isbrkup = room.Rates.ToList().Count == 0 ? false : true;
                                                    //if (isbrkup == true)
                                                    //{
                                                    //    int ngt = 0;
                                                    //    List<XElement> strbr = new List<XElement>();
                                                    //    foreach (var br in room.Rates)
                                                    //    {
                                                    //        strbr.Add(new XElement("Price",
                                                    //               new XAttribute("Night", Convert.ToString(Convert.ToInt32(ngt + 1))),
                                                    //               new XAttribute("PriceValue", Convert.ToString(br.Rate)))
                                                    //        );
                                                    //        ngt++;
                                                    //    };
                                                    //    strbr.OrderBy(x => (int)x.Attribute("Night")).ToList();
                                                    //    totalamt = Convert.ToDecimal(strbr.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                                    //    totroomprc = strbr;
                                                    //}
                                                    //else
                                                    //{
                                                        totroomprc = GetRoomsPriceBreakup_fromTotal(Convert.ToDecimal(room.TotalRate), totnights);
                                                        totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                                    //}
                                                }
                                                catch
                                                {
                                                    //totroomprc = GetRoomsPriceBreakup_fromTotal(Convert.ToDecimal(room.TotalRate), totnights);
                                                    //totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                                }
                                                string searchtype = string.Empty;
                                                try
                                                {
                                                    if (room.NonRefundable == "Y")
                                                    {
                                                        searchtype = "hotel_package";
                                                    }
                                                }
                                                catch { }
                                                totrmrate = totrmrate + totalamt;
                                                str.Add(new XElement("Room",
                                                                         new XAttribute("ID", Convert.ToString(room.RoomTypeCode)),
                                                                         new XAttribute("SuppliersID", "50"),
                                                                         new XAttribute("RoomSeq", room.RoomNo),
                                                                         new XAttribute("SessionID", Convert.ToString(room.ContractTokenId)),
                                                                         new XAttribute("RoomType", Convert.ToString(room.RoomType)),
                                                                         new XAttribute("OccupancyID", Convert.ToString(room.RoomConfigurationId)),
                                                                         new XAttribute("OccupancyName", Convert.ToString("")),
                                                                         new XAttribute("MealPlanID", Convert.ToString(room.MealPlanCode)),
                                                                         new XAttribute("MealPlanName", Convert.ToString(room.MealPlan)),
                                                                         new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                                                         new XAttribute("MealPlanPrice", ""),
                                                                         new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                                         new XAttribute("TotalRoomRate", Convert.ToString(totalamt)),
                                                                         new XAttribute("CancellationDate", ""),
                                                                         new XAttribute("CancellationAmount", ""),
                                                                         new XAttribute("isAvailable", "true"),
                                                                         new XAttribute("searchType", searchtype),
                                                                         new XElement("RequestID", Convert.ToString(genguid)),
                                                                         new XElement("Offers", ""),
                                                                         new XElement("PromotionList", new XElement("Promotions", room.ContractLabel)),
                                                                         new XElement("CancellationPolicy", ""),
                                                                         new XElement("Amenities", new XElement("Amenity", "")),
                                                                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                                         new XElement("Supplements", ""),
                                                                             new XElement("PriceBreakups", totroomprc),
                                                                             new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                                                             new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                                                         ));
                                            }
                                        }
                                        catch { }
                                        rmct++;
                                    }
                                    if (totrum == str.ToList().Count())
                                    {
                                        strgrp.Add(new XElement("RoomTypes", new XAttribute("Index", rumcomindx), new XAttribute("TotalRate", Convert.ToString(totrmrate)), new XAttribute("HtlCode", hotelid.ConvertHotelCodewith_underscoreIOL()), new XAttribute("CrncyCode", currency), new XAttribute("DMCType", dmc), new XAttribute("CUID", customerid),
                                                     str));
                                        rumcomindx++;
                                    }
                                }
                                catch { }
                            }
                            #region combined rooms
                            try
                            {
                                XElement totlrom = XElement.Parse(rootElement.ToString());
                                foreach (XElement groom in totlrom.Descendants("CombinationGroup"))
                                {
                                    List<XElement> str = new List<XElement>();
                                    int rmct = 0;
                                    decimal totrmrate = 0;
                                    string currency = string.Empty;
                                    foreach (XElement room in groom.Descendants("Room"))
                                    {
                                        
                                        if (room.Element("RoomStatus").Value == "OK")
                                        {
                                            string mealcode = string.Empty;
                                            try
                                            {
                                                currency = room.Element("CurrCode").Value;
                                                XElement mealel = iolRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.Element("MealPlan").Value).FirstOrDefault();
                                                mealcode = mealel.Attribute("B2BCode").Value;
                                            }
                                            catch { }
                                            decimal totalamt = 0;
                                            IEnumerable<XElement> totroomprc = null;
                                            try
                                            {
                                                //int isbrkup = room.Descendants("RateDetails").FirstOrDefault().Descendants("Rate").Count();
                                                //if (isbrkup > 0)
                                                //{
                                                //    totroomprc = GetRoomsPriceBreakup(room.Descendants("RateDetails").FirstOrDefault().Descendants("Rate").ToList());
                                                //    totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                                //}
                                                //else
                                                //{
                                                    totroomprc = GetRoomsPriceBreakup_fromTotal(Convert.ToDecimal(room.Descendants("TotalRate").FirstOrDefault().Value), totnights);
                                                    totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                                //}
                                            }
                                            catch
                                            {
                                                //totroomprc = GetRoomsPriceBreakup_fromTotal(Convert.ToDecimal(room.Descendants("TotalRate").FirstOrDefault().Value), totnights);
                                                //totalamt = Convert.ToDecimal(totroomprc.Sum(x => Convert.ToDecimal(x.Attribute("PriceValue").Value)));
                                            }
                                            string searchtype = string.Empty;
                                            try
                                            {
                                                if (room.Element("NonRefundable").Value == "Y")
                                                {
                                                    searchtype = "hotel_package";
                                                }
                                            }
                                            catch { }
                                            totrmrate = totrmrate + totalamt;
                                            str.Add(new XElement("Room",
                                                                     new XAttribute("ID", Convert.ToString(room.Element("RoomTypeCode").Value)),
                                                                     new XAttribute("SuppliersID", "50"),
                                                                     new XAttribute("RoomSeq", 1),
                                                                     new XAttribute("SessionID", Convert.ToString(room.Element("ContractTokenId").Value)),
                                                                     new XAttribute("RoomType", Convert.ToString(room.Element("RoomType").Value)),
                                                                     new XAttribute("OccupancyID", Convert.ToString(room.Element("RoomConfigurationId").Value)),
                                                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                                                     new XAttribute("MealPlanID", Convert.ToString(room.Element("MealPlanCode").Value)),
                                                                     new XAttribute("MealPlanName", Convert.ToString(room.Element("MealPlan").Value)),
                                                                     new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                                                     new XAttribute("MealPlanPrice", ""),
                                                                     new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                                                     new XAttribute("TotalRoomRate", Convert.ToString(totalamt)),
                                                                     new XAttribute("CancellationDate", ""),
                                                                     new XAttribute("CancellationAmount", ""),
                                                                     new XAttribute("isAvailable", "true"),
                                                                     new XAttribute("searchType", searchtype),
                                                                     new XElement("RequestID", Convert.ToString(genguid)),
                                                                     new XElement("Offers", ""),
                                                                     new XElement("PromotionList", new XElement("Promotions", room.Element("ContractLabel").Value)),
                                                                     new XElement("CancellationPolicy", ""),
                                                                     new XElement("Amenities", new XElement("Amenity", "")),
                                                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                                     new XElement("Supplements", ""),
                                                                         new XElement("PriceBreakups", totroomprc),
                                                                         new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                                                         new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                                                     ));

                                          
                                        }
                                    }
                                    if (totrum == str.ToList().Count())
                                    {
                                        strgrp.Add(new XElement("RoomTypes", new XAttribute("Index", rumcomindx), new XAttribute("TotalRate", Convert.ToString(totrmrate)), new XAttribute("HtlCode", hotelid.ConvertHotelCodewith_underscoreIOL()), new XAttribute("CrncyCode", currency), new XAttribute("DMCType", dmc), new XAttribute("CUID", customerid),
                                                     str));
                                        rumcomindx++;
                                    }


                                }
                            }
                            catch { }
                            #endregion
                            #endregion
                        }
                    }
                    catch(Exception ex)
                    {
                    }
                }
                AvailablilityResponse = new XElement("Rooms", strgrp);

            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "RoomAvailability";
                ex1.PageName = "IOLService";
                ex1.CustomerID = req.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = req.Descendants("TransID").FirstOrDefault().Value;
                APILog.SendCustomExcepToDB(ex1);
                #endregion
            }

            return AvailablilityResponse;

        }
        private static bool IsUniqueRoomTypeCombination(List<XElement> combination)
        {
            // Group rooms by RoomConfigurationId
            var groupedRooms = combination.GroupBy(room => room.Element("RoomConfigurationId").Value);

            // Check if any group has more than one room
            return !groupedRooms.Any(group => group.Count() > 1);
        }
        private static List<List<XElement>> GenerateCombinations(List<XElement> remainingRooms, List<XElement> currentCombination, int currentDepth)
        {
            var allCombinations = new List<List<XElement>>();

            if (currentDepth == 4 || remainingRooms.Count == 0)
            {
                allCombinations.Add(new List<XElement>(currentCombination));
                return allCombinations;
            }

            for (int i = 0; i < remainingRooms.Count; i++)
            {
                if (!currentCombination.Any(room => room.Element("RoomConfigurationId").Value == remainingRooms[i].Element("RoomConfigurationId").Value))
                {
                    currentCombination.Add(remainingRooms[i]);
                    allCombinations.AddRange(GenerateCombinations(remainingRooms.Skip(i + 1).ToList(), currentCombination, currentDepth + 1));
                    currentCombination.RemoveAt(currentCombination.Count - 1); // Backtrack
                }
            }

            return allCombinations;
        }
        //private static List<List<XElement>> GetUniqueCombinations(List<XElement> rooms)
        //{
        //    var combinations = new List<List<XElement>>();

        //    for (int i = 0; i < rooms.Count; i++)
        //    {
        //        var currentCombination = new List<XElement>() { rooms[i] };
        //        GenerateCombinations(rooms.Skip(i + 1).ToList(), currentCombination, combinations, 1);
        //    }

        //    return combinations;
        //}
        //private static void GenerateCombinations(List<XElement> remainingRooms, List<XElement> currentCombination, List<List<XElement>> allCombinations, int currentDepth)
        //{
        //    if (currentDepth == 4 || remainingRooms.Count == 0)
        //    {
        //        allCombinations.Add(new List<XElement>(currentCombination));
        //        return;
        //    }

        //    for (int i = 0; i < remainingRooms.Count; i++)
        //    {
        //        if (!currentCombination.Any(room => room.Element("RoomConfigurationId").Value == remainingRooms[i].Element("RoomConfigurationId").Value))
        //        {
        //            currentCombination.Add(remainingRooms[i]);
        //            GenerateCombinations(remainingRooms.Skip(i + 1).ToList(), currentCombination, allCombinations, currentDepth + 1);
        //            currentCombination.RemoveAt(currentCombination.Count - 1); // Backtrack
        //        }
        //    }
        //}
        // generating only two rooms
        //private static List<List<XElement>> GetUniqueCombinations(List<XElement> rooms)
        //{
        //    var combinations = new List<List<XElement>>();
        //    for (int i = 0; i < rooms.Count; i++)
        //    {
        //        for (int j = i + 1; j < rooms.Count; j++)
        //        {
        //            if (rooms[i].Element("RoomConfigurationId").Value != rooms[j].Element("RoomConfigurationId").Value)
        //            {
        //                combinations.Add(new List<XElement> { rooms[i], rooms[j] });
        //            }
        //        }
        //    }
        //    return combinations;
        //}
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
                IOLStatic iolStatic = new IOLStatic();
                int nights = (int)(cxlPolicyReq.Descendants("ToDate").FirstOrDefault().Value.ConvertToDate() - cxlPolicyReq.Descendants("FromDate").FirstOrDefault().Value.ConvertToDate()).TotalDays;

                DataTable iolPolicy = iolStatic.GetCXLPolicy(cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value, cxlPolicyReq.Descendants("Room").FirstOrDefault().Descendants("RequestID").FirstOrDefault().Value);
                string response = string.Empty;
                string countryCode = string.Empty;
                int iscxl = 0;
                if (iolPolicy != null)
                {
                    if (iolPolicy.Rows.Count != 0)
                    {
                        response = iolPolicy.Rows[0]["logresponseXML"].ToString();
                        iscxl = 1;
                    }
                }

                XElement resdoc = XElement.Parse(response);
                List<XElement> cxlist = new List<XElement>();
                cxlist = resdoc.Descendants("Room").Where(x => x.Element("RoomTypeCode").Value == cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("ID").Value
                    && x.Element("MealPlanCode").Value == cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("MealPlanID").Value
                    && x.Element("ContractTokenId").Value == cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("SessionID").Value).ToList();

                
                //string IsCombineRoom = string.Empty;
               
                //int totrum = cxlPolicyReq.Descendants("Room").Count();
                //foreach (XElement room in cxlPolicyReq.Descendants("Room"))
                //{
                //    List<XElement> cpo = resp.Descendants("CancellationPolicy").Where(x => x.Parent.Element("RoomToken").Value == room.Attribute("SessionID").Value).ToList();
                //    cxlist.AddRange(cpo);
                //}
                XElement cp = new XElement("CancellationPolicies");
                decimal bookingcost = 0;

                foreach (XElement room in cxlist)
                {
                    List<XElement> Ratelst = room.Descendants("RateDetails").Descendants("Rate").ToList();
                    decimal roomcost = 0;
                    //if (Ratelst.Count() > 0)
                    //{
                    //    foreach (XElement rate in Ratelst)
                    //    {
                    //        decimal ngtcharge = Convert.ToDecimal(rate.Value);
                    //        roomcost = roomcost + ngtcharge;
                    //    }
                    //}
                    //else
                    //{
                        roomcost = Convert.ToDecimal(room.Descendants("TotalRate").FirstOrDefault().Value);
                    //}
                    bookingcost = bookingcost + roomcost;
                    foreach (XElement policy in room.Descendants("Cancellation"))
                    {
                        try
                        {
                            XElement nightcharge = policy.Element("NightToCharge");
                            XElement amtpct = policy.Element("PercentOrAmt");
                            DateTime frDate = DateTime.ParseExact(policy.Element("FromDate").Value, "yyyyMMdd", null);
                            string fdt = frDate.ToString("dd/MM/yyyy");
                            DateTime toDate = DateTime.ParseExact(policy.Element("ToDate").Value, "yyyyMMdd", null);
                            string tdt = toDate.ToString("dd/MM/yyyy");
                            if (nightcharge != null)
                            {
                                int totngt = Convert.ToInt16(policy.Element("NightToCharge").Value);
                                decimal cxlcharge = 0;
                                for (int i = 0; i < totngt; i++)
                                {
                                    //decimal ngtcharge = Convert.ToDecimal(Ratelst[i].Value);
                                    decimal ngtcharge = Convert.ToDecimal(roomcost/nights);
                                    cxlcharge = cxlcharge + ngtcharge;
                                }
                                cp.Add(new XElement("CancellationPolicy",
                                                    new XAttribute("LastCancellationDate", fdt),
                                                    new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                    new XAttribute("NoShowPolicy", "0")));
                            }
                            else if (amtpct != null)
                            {
                                if (policy.Element("PercentOrAmt").Value == "A")
                                {
                                    decimal cxlcharge = Convert.ToDecimal(policy.Element("Value").Value);
                                    cp.Add(new XElement("CancellationPolicy",
                                                    new XAttribute("LastCancellationDate", fdt),
                                                    new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                    new XAttribute("NoShowPolicy", "0")));
                                }
                                else
                                {
                                    decimal percentval = Convert.ToDecimal(policy.Element("Value").Value);
                                    decimal cxlcharge = roomcost * percentval / 100;
                                    cp.Add(new XElement("CancellationPolicy",
                                                    new XAttribute("LastCancellationDate", fdt),
                                                    new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                    new XAttribute("NoShowPolicy", "0")));
                                }

                            }
                        }
                        catch
                        {
                            cp = new XElement("CancellationPolicies");
                            break;
                        }

                    }
                }
                if (cp.Descendants("CancellationPolicy").ToList().Count() == 0)
                {
                    DateTime fdt = DateTime.ParseExact(cxlPolicyReq.Descendants("FromDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string fdate = (fdt.ToString("yyyyMMdd"));
                    DateTime tdt = DateTime.ParseExact(cxlPolicyReq.Descendants("ToDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string tdate = (tdt.ToString("yyyyMMdd"));
                    XElement suprequest = new XElement("HotelSearchRequest",
                        new XElement("OutputFormat", iolOutputFormat),
                        new XElement("Profile", new XElement("Password", iolPassword), new XElement("Code", iolCode), new XElement("TokenNumber", cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value)),
                        new XElement("SearchCriteria",
                            new XElement("RoomConfiguration", prebookroomGuests(cxlPolicyReq.Descendants("Rooms").FirstOrDefault())),
                            new XElement("StartDate", fdate),
                            new XElement("EndDate", tdate),
                            new XElement("Nationality", cxlPolicyReq.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value),
                            new XElement("HotelCode", cxlPolicyReq.Descendants("RoomTypes").FirstOrDefault().Attribute("HtlCode").Value.ConvertHotelCodewith_dashIOL()),
                            new XElement("IncludeRateDetails", "Y"),
                            new XElement("GroupByRooms", "Y"),
                            new XElement("CancellationPolicy", "Y")
                            )
                        );


                    IOLLogModel logmodel = new IOLLogModel()
                    {
                        TrackNo = cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value,
                        CustomerID = cxlPolicyReq.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                        SuplId = supplierid,
                        LogType = "CXLPolicy",
                        LogTypeID = 3
                    };
                    IOLRequest iolreqobj = new IOLRequest();
                    XDocument sup_response = iolreqobj.Request(suprequest, iolprebook, "", logmodel);

                    XElement suplResponse = removeAllNamespaces(sup_response.Root);
                    if (suplResponse.Descendants("RoomStatus").FirstOrDefault().Value == "OK")
                    {
                        XElement iolRoomsMealPlansfile = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/IOL_Meals.xml"));
                        #region CXL Policy
                        List<XElement> cxlist1 = new List<XElement>();
                        cxlist1 = suplResponse.Descendants("Room").ToList();
                        bookingcost = 0;

                        foreach (XElement room in cxlist1)
                        {
                            List<XElement> Ratelst = room.Descendants("RateDetails").Descendants("Rate").ToList();
                            decimal roomcost = 0;
                            if (Ratelst.Count() > 0)
                            {
                                foreach (XElement rate in Ratelst)
                                {
                                    decimal ngtcharge = Convert.ToDecimal(rate.Value);
                                    roomcost = roomcost + ngtcharge;
                                }
                            }
                            else
                            {
                                roomcost = Convert.ToDecimal(room.Descendants("TotalRate").FirstOrDefault().Value);
                            }
                            bookingcost = bookingcost + roomcost;
                            foreach (XElement policy in room.Descendants("Cancellation"))
                            {
                                XElement nightcharge = policy.Element("NightToCharge");
                                XElement amtpct = policy.Element("PercentOrAmt");
                                DateTime ffrDate = DateTime.ParseExact(policy.Element("FromDate").Value, "yyyyMMdd", null);
                                string ffdt = ffrDate.ToString("dd/MM/yyyy");
                                if (nightcharge != null)
                                {
                                    int totngt = Convert.ToInt16(policy.Element("NightToCharge").Value);
                                    decimal cxlcharge = 0;
                                    for (int i = 0; i < totngt; i++)
                                    {
                                        decimal ngtcharge = Convert.ToDecimal(Ratelst[i].Value);
                                        cxlcharge = cxlcharge + ngtcharge;
                                    }
                                    cp.Add(new XElement("CancellationPolicy",
                                                        new XAttribute("LastCancellationDate", ffdt),
                                                        new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                        new XAttribute("NoShowPolicy", "0")));
                                }
                                else if (amtpct != null)
                                {
                                    if (policy.Element("PercentOrAmt").Value == "A")
                                    {
                                        decimal cxlcharge = Convert.ToDecimal(policy.Element("Value").Value);
                                        cp.Add(new XElement("CancellationPolicy",
                                                        new XAttribute("LastCancellationDate", ffdt),
                                                        new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                        new XAttribute("NoShowPolicy", "0")));
                                    }
                                    else
                                    {
                                        decimal percentval = Convert.ToDecimal(policy.Element("Value").Value);
                                        decimal cxlcharge = roomcost * percentval / 100;
                                        cp.Add(new XElement("CancellationPolicy",
                                                        new XAttribute("LastCancellationDate", ffdt),
                                                        new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                        new XAttribute("NoShowPolicy", "0")));
                                    }

                                }

                            }
                        }
                        #endregion
                    }
                }
                List<XElement> mergeinput = new List<XElement>();
                mergeinput.Add(cp);
                XElement finalcp = MergCxlPolicy(mergeinput, bookingcost);

                XElement _travyoResp;
                _travyoResp = new XElement("HotelDetailwithcancellationResponse",
                    new XElement("Hotels",
                        new XElement("Hotel",
                            new XElement("HotelID", cxlPolicyReq.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "50").FirstOrDefault().Attribute("GHtlID").Value),
                            new XElement("HotelName", cxlPolicyReq.Descendants("HotelName").First().Value),
                            new XElement("HotelImgSmall", null),
                            new XElement("HotelImgLarge", null),
                            new XElement("MapLink", null),
                            new XElement("DMC", "IOL"),
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
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "CancellationPolicy";
                ex1.PageName = "IOLService";
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
                DateTime fdt = DateTime.ParseExact(Req.Descendants("FromDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = (fdt.ToString("yyyyMMdd"));
                DateTime tdt = DateTime.ParseExact(Req.Descendants("ToDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string tdate = (tdt.ToString("yyyyMMdd"));
                XElement suprequest = new XElement("HotelSearchRequest",
                    new XElement("OutputFormat", iolOutputFormat),
                    new XElement("Profile", new XElement("Password", iolPassword), new XElement("Code", iolCode), new XElement("TokenNumber", Req.Descendants("TransID").FirstOrDefault().Value)),
                    new XElement("SearchCriteria",
                        new XElement("RoomConfiguration", prebookroomGuests(Req.Descendants("Rooms").FirstOrDefault())),
                        new XElement("StartDate", fdate),
                        new XElement("EndDate", tdate),
                        new XElement("Nationality", Req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value),
                        new XElement("HotelCode", Req.Descendants("RoomTypes").FirstOrDefault().Attribute("HtlCode").Value.ConvertHotelCodewith_dashIOL()),
                        new XElement("IncludeRateDetails", "Y"),
                        new XElement("GroupByRooms", "Y"),
                        new XElement("CancellationPolicy", "Y")                        
                        )
                    );


                IOLLogModel logmodel = new IOLLogModel()
                {
                    TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "PreBook",
                    LogTypeID = 4
                };
                IOLRequest iolreqobj = new IOLRequest();
                XDocument sup_response = iolreqobj.Request(suprequest, iolprebook, "", logmodel);

                XElement suplResponse = removeAllNamespaces(sup_response.Root);
                if (suplResponse.Descendants("RoomStatus").FirstOrDefault().Value == "OK")
                {
                    XElement iolRoomsMealPlansfile = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/IOL_Meals.xml"));
                    #region CXL Policy
                    List<XElement> cxlist = new List<XElement>();
                    cxlist = suplResponse.Descendants("Room").ToList();
                    XElement cp = new XElement("CancellationPolicies");
                    decimal bookingcost = 0;

                    foreach (XElement room in cxlist)
                    {
                        List<XElement> Ratelst = room.Descendants("RateDetails").Descendants("Rate").ToList();
                        decimal roomcost = 0;
                        if (Ratelst.Count() > 0)
                        {
                            foreach (XElement rate in Ratelst)
                            {
                                decimal ngtcharge = Convert.ToDecimal(rate.Value);
                                roomcost = roomcost + ngtcharge;
                            }
                        }
                        else
                        {
                            roomcost = Convert.ToDecimal(room.Descendants("TotalRate").FirstOrDefault().Value);
                        }
                        bookingcost = bookingcost + roomcost;
                        foreach (XElement policy in room.Descendants("Cancellation"))
                        {
                            XElement nightcharge = policy.Element("NightToCharge");
                            XElement amtpct = policy.Element("PercentOrAmt");
                            DateTime ffrDate = DateTime.ParseExact(policy.Element("FromDate").Value, "yyyyMMdd", null);
                            string ffdt = ffrDate.ToString("dd/MM/yyyy");
                            if (nightcharge != null)
                            {
                                int totngt = Convert.ToInt16(policy.Element("NightToCharge").Value);
                                decimal cxlcharge = 0;
                                for (int i = 0; i < totngt; i++)
                                {
                                    decimal ngtcharge = Convert.ToDecimal(Ratelst[i].Value);
                                    cxlcharge = cxlcharge + ngtcharge;
                                }
                                cp.Add(new XElement("CancellationPolicy",
                                                    new XAttribute("LastCancellationDate", ffdt),
                                                    new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                    new XAttribute("NoShowPolicy", "0")));
                            }
                            else if (amtpct != null)
                            {
                                if (policy.Element("PercentOrAmt").Value == "A")
                                {
                                    decimal cxlcharge = Convert.ToDecimal(policy.Element("Value").Value);
                                    cp.Add(new XElement("CancellationPolicy",
                                                    new XAttribute("LastCancellationDate", ffdt),
                                                    new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                    new XAttribute("NoShowPolicy", "0")));
                                }
                                else
                                {
                                    decimal percentval = Convert.ToDecimal(policy.Element("Value").Value);
                                    decimal cxlcharge = roomcost * percentval / 100;
                                    cp.Add(new XElement("CancellationPolicy",
                                                    new XAttribute("LastCancellationDate", ffdt),
                                                    new XAttribute("ApplicableAmount", Convert.ToString(cxlcharge)),
                                                    new XAttribute("NoShowPolicy", "0")));
                                }

                            }

                        }
                    }
                    List<XElement> mergeinput = new List<XElement>();
                    mergeinput.Add(cp);
                    XElement finalcp = MergCxlPolicy(mergeinput, bookingcost);
                    #endregion

                    #region prebook response bind
                    List<XElement> roompax = Req.Descendants("RoomPax").ToList();
                    List<XElement> roombind = new List<XElement>();
                    int rmct = 0;
                    string tnc = string.Empty;
                    foreach (XElement room in cxlist)
                    {
                            string mealcode = string.Empty;
                            try
                            {
                                XElement mealel = iolRoomsMealPlansfile.Descendants("Meal").Where(X => X.Attribute("Name").Value == room.Element("MealPlan").Value).FirstOrDefault();
                                mealcode = mealel.Attribute("B2BCode").Value;
                            }
                            catch { }
                            IEnumerable<XElement> totroomprc = GetRoomsPriceBreakup(room.Descendants("RateDetails").FirstOrDefault().Descendants("Rate").ToList());
                            string roomstatus = "true";
                            if (room.Element("RoomStatus").Value != "OK")
                            {
                                roomstatus = "false";
                            }
                            List<XElement> SuppList = new List<XElement>();
                            foreach (XElement supl in room.Descendants("Message"))
                            {
                                try
                                {
                                    try
                                    {
                                        tnc += supl.Element("MessageFull") == null ? "" : supl.Element("MessageFull").Value;
                                    }
                                    catch { }
                                    SuppList.Add(new XElement("Supplement",
                                                    new XAttribute("suppId", supl.Element("MessageId") == null ? "" : supl.Element("MessageId").Value),
                                                    new XAttribute("suppName", supl.Element("MessageShort") == null ? "" : supl.Element("MessageShort").Value),
                                                    new XAttribute("supptType", "0"),
                                                    new XAttribute("suppIsMandatory", "True"),
                                                    new XAttribute("suppChargeType", "AtProperty"),
                                                    new XAttribute("suppPrice", supl.Element("Value") == null ? "" : supl.Element("Value").Value),
                                                    new XAttribute("suppType", supl.Element("MessageChargeBase") == null ? "" : supl.Element("MessageChargeBase").Value),
                                                    new XAttribute("currencyCode", supl.Element("Currency") == null ? "" : supl.Element("Currency").Value)));
                                    
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            roombind.Add(new XElement("Room",
                                     new XAttribute("ID", Convert.ToString(room.Element("RoomTypeCode").Value)),
                                     new XAttribute("SuppliersID", "50"),
                                     new XAttribute("RoomSeq", room.Element("RoomNo").Value),
                                     new XAttribute("SessionID", Convert.ToString(room.Element("ContractTokenId").Value)),
                                     new XAttribute("RoomType", Convert.ToString(room.Element("RoomType").Value)),
                                     new XAttribute("OccupancyID", Convert.ToString(room.Element("RoomConfigurationId").Value)),
                                     new XAttribute("OccupancyName", Convert.ToString("")),
                                     new XAttribute("MealPlanID", Convert.ToString(room.Element("MealPlanCode").Value)),
                                     new XAttribute("MealPlanName", Convert.ToString(room.Element("MealPlan").Value)),
                                     new XAttribute("MealPlanCode", Convert.ToString(mealcode)),
                                     new XAttribute("MealPlanPrice", ""),
                                     new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                                     new XAttribute("TotalRoomRate", Convert.ToString(room.Element("Rate").Value)),
                                     new XAttribute("CancellationDate", ""),
                                     new XAttribute("CancellationAmount", ""),
                                     new XAttribute("isAvailable", roomstatus),
                                     new XElement("RequestID", Convert.ToString("")),
                                     new XElement("Offers", ""),
                                     new XElement("PromotionList", new XElement("Promotions", room.Element("ContractLabel").Value)),
                                     new XElement("CancellationPolicy", ""),
                                     new XElement("Amenities", new XElement("Amenity", "")),
                                     new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                     new XElement("Supplements", null),
                                          new XElement("PriceBreakups", totroomprc),
                                          new XElement("AdultNum", Convert.ToString(roompax[rmct].Descendants("Adult").FirstOrDefault().Value)),
                                          new XElement("ChildNum", Convert.ToString(roompax[rmct].Descendants("Child").FirstOrDefault().Value))
                                     ));
                            rmct++;
                    }
                    roombind.Add(finalcp);
                    XElement roomgrp = new XElement("RoomTypes", new XAttribute("Index", "1"), new XAttribute("TotalRate", Convert.ToString(bookingcost)),
                        roombind);


                    XElement hotel = new XElement("Hotel",
                                       new XElement("HotelID", Convert.ToString(Req.Descendants("HotelID").FirstOrDefault().Value)),
                                                   new XElement("HotelName", Convert.ToString(suplResponse.Descendants("HotelName").FirstOrDefault().Value)),
                                                   new XElement("Status", "true"),
                                                   new XElement("TermCondition", tnc.ToString().Trim()),
                                                   new XElement("HotelImgSmall", Convert.ToString("")),
                                                   new XElement("HotelImgLarge", Convert.ToString("")),
                                                   new XElement("MapLink", ""),
                                                   new XElement("DMC", dmc),
                                                   new XElement("Currency", Convert.ToString(suplResponse.Descendants("CurrCode").FirstOrDefault().Value)),
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
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "PreBook";
                ex1.PageName = "IOLService";
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

                DateTime fdt = DateTime.ParseExact(Req.Descendants("FromDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fdate = (fdt.ToString("yyyyMMdd"));
                DateTime tdt = DateTime.ParseExact(Req.Descendants("ToDate").FirstOrDefault().Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string tdate = (tdt.ToString("yyyyMMdd"));
                XElement suprequest = new XElement("HotelBookingRequest",
                    new XElement("OutputFormat", iolOutputFormat),
                    new XElement("Profile", new XElement("Password", iolPassword), new XElement("Code", iolCode), new XElement("TokenNumber", Req.Descendants("TransID").FirstOrDefault().Value)),
                    new XElement("PassengerDetails", bindbookpassenger(Req.Descendants("PassengersDetail").FirstOrDefault(), Req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value)),
                    new XElement("HotelDetails",
                        new XElement("StartDate", fdate),
                        new XElement("EndDate", tdate),
                        new XElement("HotelCode", Req.Descendants("HotelID").FirstOrDefault().Value.ConvertHotelCodewith_dashIOL()),
                        new XElement("AgencyRef", Req.Descendants("TransID").FirstOrDefault().Value),
                        new XElement("RoomDetails", bindbookroom(Req.Descendants("PassengersDetail").FirstOrDefault()))
                        )
                    );

                IOLLogModel logmodel = new IOLLogModel()
                {
                    TrackNo = Req.Descendants("TransactionID").FirstOrDefault().Value,
                    CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                    SuplId = supplierid,
                    LogType = "Book",
                    LogTypeID = 5
                };

                IOLRequest iolreqobj = new IOLRequest();
                XDocument sup_response = iolreqobj.Request(suprequest, iolbook, "", logmodel);

                SupplierResponse = removeAllNamespaces(sup_response.Root);

                try
                {
                    string bookingstatus = string.Empty;
                    XElement bookingdetail = SupplierResponse.Descendants("BookingDetails").FirstOrDefault();
                    XElement hoteldetail = SupplierResponse.Descendants("HotelDetails").FirstOrDefault();
                    bookingstatus = bookingdetail.Element("BookingStatus").Value;
                    if (bookingstatus.ToUpper().Trim() == "CONFIRMED" || bookingstatus.ToUpper().Trim() == "PAID" || bookingstatus.ToUpper().Trim() == "BOOKED")
                    {
                        string sourceid = bookingdetail.Element("Source").Value;
                        string confirmationnum = bookingdetail.Element("BookingNumber").Value;
                        double amount = Convert.ToDouble(hoteldetail.Element("TotalRate").Value);
                        string SubResNo = hoteldetail.Descendants("Room").FirstOrDefault().Descendants("SubResNo").FirstOrDefault().Value;
                        try
                        {
                            SubResNo = SubResNo.Replace("IWTX-", "");
                        }
                        catch { }
                        iolCurrency = hoteldetail.Element("Currency").Value;
                        foreach (XElement statusnode in hoteldetail.Descendants("Room"))
                        {
                            string sstt = statusnode.Descendants("RoomStatus").FirstOrDefault().Value;
                            if (sstt.ToLower().Trim() == "requested" || sstt.ToLower().Trim() == "on request" || sstt.ToLower().Trim() == "option" || sstt.ToLower().Trim() == "cancelled")
                            {
                                bookingstatus = "Inprocess";
                                break;
                            }
                            else if (sstt.ToLower().Trim() == "failed")
                            {
                                bookingstatus = "Failed";
                                break;
                            }
                            else if (sstt.ToLower().Trim() == "confirmed" || sstt.ToLower().Trim() == "paid" || sstt.ToLower().Trim() == "booked" || sstt.ToLower().Trim() == "ok")
                            {
                                bookingstatus = "confirmed";
                            }
                            //OK – Confirmed 
                            //RQ – On Request XX –
                            //Rejected (unable to 
                            //confirm) 
                            //CX - Cancelled
                            //TEST – test booking – your client profile is set to Test mode
                        }
                        if (bookingstatus == "confirmed")
                        {
                            bookingstatus = "Success";
                        }

                        BookingResponse.Add(new XElement("Hotels",
                                                       new XElement("Hotel",
                                                           new XElement("HotelID", Req.Descendants("HotelID").FirstOrDefault().Value),
                                                           new XElement("HotelName", Req.Descendants("HotelName").FirstOrDefault().Value),
                                                           new XElement("FromDate", Req.Descendants("FromDate").FirstOrDefault().Value),
                                                           new XElement("ToDate", Req.Descendants("ToDate").FirstOrDefault().Value),
                                                           new XElement("AdultPax", Req.Descendants("Rooms").Descendants("RoomPax").Descendants("Adult").FirstOrDefault().Value),
                                                           new XElement("ChildPax", Req.Descendants("Rooms").Descendants("RoomPax").Descendants("Child").FirstOrDefault().Value),
                                                           new XElement("TotalPrice", amount),
                                                           new XElement("CurrencyID"),
                                                           new XElement("CurrencyCode", iolCurrency),
                                                           new XElement("MarketID"),
                                                           new XElement("MarketName"),
                                                           new XElement("HotelImgSmall"),
                                                           new XElement("HotelImgLarge"),
                                                           new XElement("MapLink"),
                                                           new XElement("VoucherRemark", ""),
                                                           new XElement("TransID", Req.Descendants("TransactionID").FirstOrDefault().Value),
                                                           new XElement("ConfirmationNumber", confirmationnum),
                                                           new XElement("Status", bookingstatus),
                                                           new XElement("PassengerDetail",
                                                           bookingRespRooms(Req.Descendants("PassengersDetail").FirstOrDefault(), amount, SubResNo)))));
                    }
                    else
                    {
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
                                                           new XElement("CurrencyCode", iolCurrency),
                                                           new XElement("MarketID"),
                                                           new XElement("MarketName"),
                                                           new XElement("HotelImgSmall"),
                                                           new XElement("HotelImgLarge"),
                                                           new XElement("MapLink"),
                                                           new XElement("VoucherRemark", ""),
                                                           new XElement("TransID", Req.Descendants("TransactionID").FirstOrDefault().Value),
                                                           new XElement("ConfirmationNumber", ""),
                                                           new XElement("Status", bookingstatus),
                                                           new XElement("PassengerDetail",
                                                           bookingRespRooms(Req.Descendants("PassengersDetail").FirstOrDefault(), 0, "")))));
                    }
                }
                catch (Exception exinn)
                {
                    #region Exception
                    CustomException exi1 = new CustomException(exinn);
                    exi1.MethodName = "Booking";
                    exi1.PageName = "IOLService";
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
                ex1.PageName = "IOLService";
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
        public XElement bookingRespRooms(XElement Rooms, double amount, string SubResNo)
        {
            double perRoomRate = amount / Rooms.Descendants("Room").Count();
            XElement GuestDetails = new XElement("GuestDetails");
            foreach (XElement room in Rooms.Descendants("Room"))
            {
                GuestDetails.Add(new XElement("Room",
                                    new XAttribute("ID", room.Attribute("RoomTypeID").Value),
                                    new XAttribute("RoomType", room.Attribute("RoomType").Value),
                                    new XAttribute("ServiceID", ""),
                                    new XAttribute("RefNo", SubResNo),
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
                #region cancel

                #region Get Booking Response
                string sourceid = string.Empty;
                string confirmationnum = string.Empty;
                IOLStatic iolStatic = new IOLStatic();
                DataTable bookresponse = iolStatic.GetBookingResponse(Req.Descendants("TransID").FirstOrDefault().Value);
                string bresponse = string.Empty;
                string countryCode = string.Empty;
                int isbook = 0;
                if (bookresponse != null)
                {
                    if (bookresponse.Rows.Count != 0)
                    {
                        bresponse = bookresponse.Rows[0]["logresponseXML"].ToString();
                        isbook = 1;
                    }
                }
                #endregion
                if (isbook == 1)
                {
                    XElement bresdoc = XElement.Parse(bresponse);
                    XElement bookingdetail = bresdoc.Descendants("BookingDetails").FirstOrDefault();
                    XElement hoteldetail = bresdoc.Descendants("HotelDetails").FirstOrDefault();

                    sourceid = bookingdetail.Element("Source").Value;
                    confirmationnum = bookingdetail.Element("BookingNumber").Value;
                    double amount = Convert.ToDouble(hoteldetail.Element("TotalRate").Value);
                    string SubResNo = hoteldetail.Descendants("Room").FirstOrDefault().Descendants("SubResNo").FirstOrDefault().Value;
                    iolCurrency = hoteldetail.Element("Currency").Value;
                    foreach (XElement room in hoteldetail.Descendants("Room"))
                    {
                        XElement suprequest = new XElement("CancelHotelBookingRequest",
                       new XElement("OutputFormat", iolOutputFormat),
                       new XElement("Profile", new XElement("Password", iolPassword), new XElement("Code", iolCode), new XElement("TokenNumber", Req.Descendants("TransID").FirstOrDefault().Value)),
                       new XElement("BookingDetails",
                           new XElement("Source", sourceid),
                           new XElement("BookingNumber", confirmationnum),
                           new XElement("SubResNo", room.Descendants("SubResNo").FirstOrDefault().Value)
                           )
                       );
                        IOLLogModel logmodel = new IOLLogModel()
                        {
                            TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                            CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                            SuplId = supplierid,
                            LogType = "Cancel",
                            LogTypeID = 6
                        };

                        IOLRequest iolreqobj = new IOLRequest();
                        XDocument sup_response = iolreqobj.Request(suprequest, iolcancel, "", logmodel);
                        //XElement SupplierResponse = removeAllNamespaces(sup_response.Root);
                        //string charge = SupplierResponse.Descendants("TotalAmount").FirstOrDefault().Value;

                    }

                    #region Retrieve Booking

                    XElement Rsuprequest = new XElement("RetrieveBookingRequest",
                       new XElement("OutputFormat", iolOutputFormat),
                       new XElement("Profile", new XElement("Password", iolPassword), new XElement("Code", iolCode), new XElement("TokenNumber", Req.Descendants("TransID").FirstOrDefault().Value)),
                       new XElement("BookingDetails",
                           new XElement("BookingNo", confirmationnum),
                           new XElement("Source", sourceid)                           
                           )
                       );
                    IOLLogModel Rlogmodel = new IOLLogModel()
                    {
                        TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                        CustomerID = Req.Descendants("CustomerID").FirstOrDefault().Value.ModifyToLong(),
                        SuplId = supplierid,
                        LogType = "BookingDetail",
                        LogTypeID = 6
                    };

                    IOLRequest Riolreqobj = new IOLRequest();
                    XDocument Rsup_response = Riolreqobj.Request(Rsuprequest, iolretrieve, "", Rlogmodel);

                    #endregion
                    //XElement Rhoteldetail = Rsup_response.Descendants("HotelDetails").FirstOrDefault();
                    string status = Rsup_response.Descendants("BookingStatus").FirstOrDefault().Value;

                    if (status.ToUpper().Trim() == "CANCELLED")
                    {
                        CancelResponse.Add(new XElement("Rooms",
                                                new XElement("Room",
                                                    new XElement("Cancellation",
                                                        new XElement("Amount", "0"),
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
                ex1.PageName = "IOLService";
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
                string hotelID = Req.Descendants("HotelID").FirstOrDefault().Value.ConvertHotelCodewith_dashIOL();
                IOLStatic iolStatic = new IOLStatic();
                DataTable iolHotels = iolStatic.GetSingleHotelDetail(hotelID);

                if (iolHotels != null && iolHotels.Rows.Count > 0)
                {
                    XElement Images = new XElement("Images");
                    Images.Add(new XElement("Image",
                                        new XAttribute("Path", iolHotels.Rows[0]["HotelFrontImage"].ToString()),
                                        new XAttribute("Caption", "")));
                    XElement Facilities = new XElement("Facilities");
                    Response.Add(new XElement("Hotels",
                                    new XElement("Hotel",
                                        new XElement("HotelID", Req.Descendants("HotelID").FirstOrDefault().Value),
                                        new XElement("Description", iolHotels.Rows[0]["description"].ToString()),
                                        Images,
                                        Facilities,
                                        new XElement("ContactDetails",
                                            new XElement("Phone", ""),
                                            new XElement("Fax", "")),
                                        new XElement("CheckinTime", iolHotels.Rows[0]["checkintime"].ToString()),
                                        new XElement("CheckoutTime", iolHotels.Rows[0]["checkouttime"].ToString()))));
                }
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelDetail";
                ex1.PageName = "IOLService";
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
        #region Common Methods
        #region Price Breakups
        private IEnumerable<XElement> GetRoomsPriceBreakup(List<XElement> pricebreakups)
        {
            #region Room's Price Breakups
            List<XElement> str = new List<XElement>();
            try
            {
                for (int i = 0; i < pricebreakups.Count(); i++)
                {
                    str.Add(new XElement("Price",
                           new XAttribute("Night", Convert.ToString(Convert.ToInt32(i + 1))),
                           new XAttribute("PriceValue", Convert.ToString(pricebreakups[i].Value)))
                    );
                };
                return str.OrderBy(x => (int)x.Attribute("Night")).ToList();
            }
            catch { return null; }
            #endregion
        }
        private IEnumerable<XElement> GetRoomsPriceBreakup_fromTotal(decimal totalamt,int night)
        {
            #region Room's Price Breakups
            List<XElement> str = new List<XElement>();
            try
            {
                for (int i = 0; i < night; i++)
                {
                    str.Add(new XElement("Price",
                           new XAttribute("Night", Convert.ToString(Convert.ToInt32(i + 1))),
                           new XAttribute("PriceValue", Convert.ToString(totalamt/night)))
                    );
                };
                return str.OrderBy(x => (int)x.Attribute("Night")).ToList();
            }
            catch { return null; }
            #endregion
        }
        #endregion
        #region Search Rooms
        public List<XElement> roomGuests(XElement RoomPax)
        {
            List<XElement> rooms = new List<XElement>();
            foreach (XElement room in RoomPax.Descendants("RoomPax"))
            {
                List<XElement> adultlst = new List<XElement>();
                for (int i = 0; i < Convert.ToInt16(room.Element("Adult").Value); i++)
                {
                    XElement adult = new XElement("Adult", new XElement("Age", "35"));
                    adultlst.Add(adult);
                }
                //for (int i = 0; i < room.Elements("ChildAge").ToList().Count; i++)
                //{
                //    XElement child = new XElement("Child", new XElement("Age", "35"));
                //    adultlst.Add(child);
                //}
                foreach (XElement childAge in room.Descendants("ChildAge"))
                    adultlst.Add(new XElement("Child",
                                    new XElement("Age", childAge.Value)));
                XElement roomcn = new XElement("Room", adultlst);
                rooms.Add(roomcn);
            }
            return rooms;
        }
        public List<XElement> prebookroomGuests(XElement RoomPax)
        {
            List<XElement> rooms = new List<XElement>();
            int roomindex = 0;
            List<XElement> rmlst = RoomPax.Descendants("Room").ToList();
            foreach (XElement room in RoomPax.Descendants("RoomPax"))
            {
                List<XElement> adultlst = new List<XElement>();
                for (int i = 0; i < Convert.ToInt16(room.Element("Adult").Value); i++)
                {
                    XElement adult = new XElement("Adult", new XElement("Age", "35"));
                    adultlst.Add(adult);
                }
                foreach (XElement childAge in room.Descendants("ChildAge"))
                    adultlst.Add(new XElement("Child",
                                    new XElement("Age", childAge.Value)));

                XElement rmcode = new XElement("RoomTypeCode", rmlst[roomindex].Attribute("ID").Value);
                XElement rmmealcode = new XElement("MealPlanCode", rmlst[roomindex].Attribute("MealPlanID").Value);
                XElement rmtokenid = new XElement("ContractTokenId", rmlst[roomindex].Attribute("SessionID").Value);
                XElement rmconfigid = new XElement("RoomConfigurationId", rmlst[roomindex].Attribute("OccupancyID").Value);
                XElement roomcn = new XElement("Room", adultlst, rmcode, rmmealcode, rmtokenid, rmconfigid);
                rooms.Add(roomcn);

                roomindex++;
            }
            return rooms;
        }
        #endregion
        #region Book Room bind
        public List<XElement> bindbookroom(XElement Room)
        {
            List<XElement> brooms = new List<XElement>();
            foreach (XElement broom in Room.Descendants("Room"))
            {
                XElement rmlst = new XElement("Room",
                                        new XElement("RoomTypeCode", broom.Attribute("RoomTypeID").Value),
                                        new XElement("MealPlanCode", broom.Attribute("MealPlanID").Value),
                                        new XElement("ContractTokenId", broom.Attribute("SessionID").Value),
                                        new XElement("RoomConfigurationId", broom.Attribute("OccupancyID").Value),
                                        new XElement("Rate", broom.Attribute("TotalRoomRate").Value));
                brooms.Add(rmlst);
            }
            return brooms;
        }
        public List<XElement> bindbookpassenger(XElement Room,string nationality)
        {
            List<XElement> brooms = new List<XElement>();
            int roomindx = 1;
            int paxno = 1;
            foreach (XElement broom in Room.Descendants("Room"))
            {
                foreach (XElement paxdet in broom.Descendants("PaxInfo"))
                {
                    string sexdet = "M";
                    if (paxdet.Element("Title").Value == "Mrs" || paxdet.Element("Title").Value == "Mrs." || paxdet.Element("Title").Value == "Miss" || paxdet.Element("Title").Value == "Ms"
                        || paxdet.Element("Title").Value == "Ms." || paxdet.Element("Title").Value == "Miss.")
                    {
                        sexdet = "F";
                    }
                    string fullnamefst = paxdet.Element("FirstName").Value.Trim() + " " + paxdet.Element("MiddleName").Value.Trim();
                    XElement pslst = new XElement("Passenger",
                                            new XElement("PaxNumber", paxno),
                                            new XElement("RoomNo", roomindx),
                                            new XElement("Title", paxdet.Element("Title").Value),
                                            new XElement("PassengerType", paxdet.Element("GuestType").Value == "Adult" ? "ADT" : "CHD"),
                                            new XElement("Age", paxdet.Element("GuestType").Value == "Adult" ? "35" : paxdet.Element("Age").Value),
                                            new XElement("FirstName", fullnamefst.ToString().Trim()),
                                            new XElement("LastName", paxdet.Element("LastName").Value.Trim()),
                                            new XElement("Nationality", nationality.ToString()),
                                            new XElement("Gender", sexdet));
                    brooms.Add(pslst);
                    paxno++;
                }
                roomindx++;
            }
            return brooms;
        }
        #endregion
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