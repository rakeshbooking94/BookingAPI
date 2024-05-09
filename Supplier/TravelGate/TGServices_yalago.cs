using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Common;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Models.Common;
using TravillioXMLOutService.Models.TravelGate;

namespace TravillioXMLOutService.Supplier.TravelGate
{
    public class TGServices_yalago : IDisposable
    {
         string dmc, hotelcode, currencyCode;
        string customerid = string.Empty;
        //int sup_cutime = 20700, timeDifference = 1800;
        int sup_cutime = 18700, timeDifference = 1200; 
        TGDataAccess tgd = new TGDataAccess();
        TGRequest tgr = new TGRequest();
        int supplierID;
        string client, testMode, context, url, apiKey, accessCode;
        XNamespace xmlns = "http://schemas.xmlsoap.org/soap/envelope/";
        XElement SmyRoomsMealPlans = null;
        public TGServices_yalago(int SuplID, string CustID)
        {
            try
            {
                supplierID = SuplID;
                TravelGateConfiguration(SuplID, CustID);
            }
            catch { }
        }
        public TGServices_yalago()
        {
        }
        #region Hotel Search
        public List<XElement> HotelSearch(XElement Req, string suplType, string custID)
        {
            dmc = suplType;
            customerid = custID;
            List<XElement> HotelList = new List<XElement>();
            DataTable StaticData = new DataTable();
            try
            {
                #region get cut off time
                try
                {
                    sup_cutime = supplier_Cred.secondcutoff_time(Req.Descendants("HotelID").FirstOrDefault().Value);
                }
                catch { }
                #endregion

                int timeOut = sup_cutime > 24700 ? 24700 : sup_cutime;
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();

                List<string> HotelListForReq = new List<string>();

                string HtId = Req.Descendants("HotelID").FirstOrDefault().Value;
                string SupCityId = TravayooRepository.SupllierCity(Convert.ToString(supplierID), Req.Descendants("CityID").FirstOrDefault().Value);
                if (!string.IsNullOrEmpty(HtId))
                {
                    #region hotel search
                    //string SupCityId = TravayooRepository.SupllierCity(Convert.ToString(supplierID), Req.Descendants("CityID").FirstOrDefault().Value);
                    int CityId = Convert.ToInt32(SupCityId);
                    var reqmodel = new SqlModel();
                    if (supplierID == 32)
                    {
                        reqmodel = new SqlModel()
                        {
                            flag = 2,
                            columnList = "HotelID, HotelName, Images, address,latitude,longitude,Star",
                            table = "yalagoHotelDetails",
                            filter = "CityID=" + CityId.ToString() + " AND HotelName LIKE '%" + Req.Descendants("HotelName").FirstOrDefault().Value + "%'",
                            SupplierId = 32
                        };
                    }
                    if (!string.IsNullOrEmpty(Req.Descendants("HotelID").FirstOrDefault().Value))
                    {
                        reqmodel.HotelCode = Req.Descendants("HotelID").FirstOrDefault().Value;
                    }
                    DataTable htlList = TravayooRepository.GetData(reqmodel);
                    StaticData = htlList;
                    if (htlList.Rows.Count > 0)
                    {
                        foreach (DataRow item in htlList.Rows)
                        {
                            HotelListForReq.Add(item["HotelID"].ToString());
                        }

                    }
                    else
                    {
                        try
                        {
                            APILogDetail log = new APILogDetail();
                            log.customerID = Convert.ToInt32(customerid);
                            log.LogTypeID = 1;
                            log.LogType = "Search";
                            log.SupplierID = supplierID;
                            log.TrackNumber = Req.Descendants("TransID").FirstOrDefault().Value;
                            log.logrequestXML = null;
                            log.logresponseXML = "There is no hotel available in database";
                            SaveAPILog savelog = new SaveAPILog();
                            savelog.SaveAPILogs_search(log);
                        }
                        catch
                        {
                        }
                        return null;
                        //throw new Exception("There is no hotel available in database");                        
                    }
                    #endregion
                }
                else
                {
                    List<string> Supl_Cities = new List<string>();
                    string CityID = Req.Descendants("CityID").FirstOrDefault().Value;
                    DataTable cityMapping = tgd.CityMapping(CityID, Convert.ToString(supplierID));
                    for (int i = 0; i < cityMapping.Rows.Count; i++)
                    {
                        Supl_Cities.Add(cityMapping.Rows[i]["SupCityId"].ToString());
                    }
                    int maxStar = Convert.ToInt32(Req.Descendants("MaxStarRating").FirstOrDefault().Value), minStar = Convert.ToInt32(Req.Descendants("MinStarRating").FirstOrDefault().Value);
                    foreach (string city in Supl_Cities)
                    {
                        //DataTable cityWiseHotels = tgd.HotelList(city, supplierID);
                        DataTable cityWiseHotels = tgd.HotelDetails_new(city, supplierID);
                        StaticData = cityWiseHotels;
                        if (cityWiseHotels != null)
                        {
                            for (int j = 0; j < cityWiseHotels.Rows.Count; j++)
                            {
                                int starRating = string.IsNullOrEmpty(cityWiseHotels.Rows[j]["Star"].ToString()) ? 0 : Convert.ToInt32(cityWiseHotels.Rows[j]["Star"].ToString());
                                if (starRating >= minStar && starRating <= maxStar)
                                    HotelListForReq.Add(cityWiseHotels.Rows[j]["HotelID"].ToString());
                            }

                        }
                    }
                }


                string hotelsString = string.Empty;


                #region junip
                int threadCount = 2;
                int _chunksize = 200;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["chunksize_tgx"]))
                {
                    _chunksize = Convert.ToInt32(ConfigurationManager.AppSettings["chunksize_tgx"]);
                }
                var chunklist = BreakIntoChunks(HotelListForReq, _chunksize);
                int Number = chunklist.Count;
                List<XElement> tr1 = new List<XElement>();
                List<XElement> tr2 = new List<XElement>();
                List<XElement> tr3 = new List<XElement>();
                List<XElement> tr4 = new List<XElement>();
                List<XElement> tr5 = new List<XElement>();
                //int timeOut = sup_cutime > 24700 ? 24700 : sup_cutime;
                //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                //timer.Start();
                for (int i = 0; i < Number; i += threadCount)
                {
                    List<Thread> threadedlist;
                    int rangecount = threadCount;
                    if (chunklist.Count - i < threadCount)
                        rangecount = chunklist.Count - i;
                    var chn = chunklist.GetRange(i, rangecount);

                    #region rangecount equals 1
                    if (rangecount == 1)
                    {
                        threadedlist = new List<Thread>
                       {   
                           new Thread(()=> tr1 = SearchHotelList(Req, chunklist.ElementAt(i),timeOut,dmc,StaticData))
                       };
                        threadedlist.ForEach(t => t.Start());
                        threadedlist.ForEach(t => t.Join(timeOut));
                        threadedlist.ForEach(t => t.Abort());
                        #region Add to list
                        if (tr1 != null && tr1.Count > 0)
                        {
                            HotelList.AddRange(tr1);
                        }
                        #endregion

                    }
                    #endregion
                    #region rangecount equals 2
                    else if (rangecount == 2)
                    {
                        threadedlist = new List<Thread>
                       {   
                           new Thread(()=> tr1 = SearchHotelList(Req, chunklist.ElementAt(i),timeOut,dmc,StaticData)),
                           new Thread(()=> tr2 = SearchHotelList(Req, chunklist.ElementAt(i+1),timeOut,dmc,StaticData))
                       };
                        threadedlist.ForEach(t => t.Start());
                        threadedlist.ForEach(t => t.Join(timeOut));
                        threadedlist.ForEach(t => t.Abort());
                        #region Add to List
                        if (tr1 != null && tr1.Count > 0)
                        {
                            HotelList.AddRange(tr1);
                        }
                        if (tr2 != null && tr2.Count > 0)
                        {
                            HotelList.AddRange(tr2);
                        }
                        #endregion
                    }
                    #endregion
                    #region rangecount equals 3
                    else if (rangecount == 3)
                    {
                        threadedlist = new List<Thread>
                       {   
                           new Thread(()=> tr1 = SearchHotelList(Req, chunklist.ElementAt(i),timeOut,dmc,StaticData)),
                           new Thread(()=> tr2 = SearchHotelList(Req, chunklist.ElementAt(i+1),timeOut,dmc,StaticData)),
                           new Thread(()=> tr3 = SearchHotelList(Req, chunklist.ElementAt(i+2),timeOut,dmc,StaticData))
                       };
                        threadedlist.ForEach(t => t.Start());
                        threadedlist.ForEach(t => t.Join(timeOut));
                        threadedlist.ForEach(t => t.Abort());
                        #region Add to List
                        if (tr1 != null && tr1.Count > 0)
                        {
                            HotelList.AddRange(tr1);
                        }
                        if (tr2 != null && tr2.Count > 0)
                        {
                            HotelList.AddRange(tr2);
                        }
                        if (tr3 != null && tr3.Count > 0)
                        {
                            HotelList.AddRange(tr3);
                        }
                        #endregion
                    }
                    #endregion
                    #region rangecount equals 4
                    else if (rangecount == 4)
                    {
                        threadedlist = new List<Thread>
                       {

                           new Thread(()=> tr1 = SearchHotelList(Req, chunklist.ElementAt(i),timeOut,dmc,StaticData)),
                           new Thread(()=> tr2 = SearchHotelList(Req, chunklist.ElementAt(i+1),timeOut,dmc,StaticData)),
                           new Thread(()=> tr3 = SearchHotelList(Req, chunklist.ElementAt(i+2),timeOut,dmc,StaticData)),
                           new Thread(()=> tr4 = SearchHotelList(Req, chunklist.ElementAt(i+3),timeOut,dmc,StaticData))

                       };
                        threadedlist.ForEach(t => t.Start());
                        threadedlist.ForEach(t => t.Join(timeOut));
                        threadedlist.ForEach(t => t.Abort());
                        #region Add to List
                        if (tr1 != null && tr1.Count > 0)
                        {
                            HotelList.AddRange(tr1);
                        }
                        if (tr2 != null && tr2.Count > 0)
                        {
                            HotelList.AddRange(tr2);
                        }
                        if (tr3 != null && tr3.Count > 0)
                        {
                            HotelList.AddRange(tr3);
                        }
                        if (tr4 != null && tr4.Count > 0)
                        {
                            HotelList.AddRange(tr4);
                        }
                        #endregion
                    }
                    #endregion
                    #region rangecount equals 5
                    else if (rangecount == 5)
                    {
                        threadedlist = new List<Thread>
                       {

                           new Thread(()=> tr1 = SearchHotelList(Req, chunklist.ElementAt(i),timeOut,dmc,StaticData)),
                           new Thread(()=> tr2 = SearchHotelList(Req, chunklist.ElementAt(i+1),timeOut,dmc,StaticData)),
                           new Thread(()=> tr3 = SearchHotelList(Req, chunklist.ElementAt(i+2),timeOut,dmc,StaticData)),
                           new Thread(()=> tr4 = SearchHotelList(Req, chunklist.ElementAt(i+3),timeOut,dmc,StaticData)),
                           new Thread(()=> tr5 = SearchHotelList(Req, chunklist.ElementAt(i+4),timeOut,dmc,StaticData))
                       };
                        threadedlist.ForEach(t => t.Start());
                        threadedlist.ForEach(t => t.Join(timeOut));
                        threadedlist.ForEach(t => t.Abort());
                        #region Add to List
                        if (tr1 != null && tr1.Count > 0)
                        {
                            HotelList.AddRange(tr1);
                        }
                        if (tr2 != null && tr2.Count > 0)
                        {
                            HotelList.AddRange(tr2);
                        }
                        if (tr3 != null && tr3.Count > 0)
                        {
                            HotelList.AddRange(tr3);
                        }
                        if (tr4 != null && tr4.Count > 0)
                        {
                            HotelList.AddRange(tr4);
                        }
                        if (tr5 != null && tr5.Count > 0)
                        {
                            HotelList.AddRange(tr5);
                        }
                        #endregion
                    }
                    #endregion
                    timeOut = timeOut - Convert.ToInt32(timer.ElapsedMilliseconds);

                }
                #endregion

            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelSearch";
                ex1.PageName = "TGServices_yalago";
                ex1.CustomerID = customerid;
                ex1.TranID = Req.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion
            }
            return HotelList;
        }
        #endregion
        #region Hotel List
        public List<XElement> SearchHotelList(XElement Req, List<string> HotelListForReq, int tgtimeout, string dmcc, DataTable StaticData)
        {
            List<XElement> HotelList = new List<XElement>();
            string currencyCode = "USD";
            try
            {
                #region Request
                #region Requset Filter Access
                var filter = new Access
                {
                    access = new Includes
                    {
                        includes = accessCode
                    },
                };
                var filterserializer = new JsonSerializer();
                var filterstringwriter = new StringWriter();
                using (var filterWriter = new JsonTextWriter(filterstringwriter))
                {
                    filterWriter.QuoteName = false;
                    filterserializer.Serialize(filterWriter, filter);
                }
                string searchFilter = filterstringwriter.ToString();
                #endregion
                #region Request Settings
                var settings = new Settings
                {
                    Client = client,
                    TestMode = Convert.ToBoolean(testMode),
                    Context = context,
                    AuditTransactions = false,
                    TimeOut = tgtimeout
                };
                var settingsserializer = new JsonSerializer();
                var settingsstringWriter = new StringWriter();
                using (var settingswriter = new JsonTextWriter(settingsstringWriter))
                {
                    settingswriter.QuoteName = false;
                    settingsserializer.Serialize(settingswriter, settings);
                }
                string searchSettings = settingsstringWriter.ToString();
                #endregion



                #region Request Criteria
                var Criteria = new Criteria
                {
                    CheckIn = reformatDate(Req.Descendants("FromDate").FirstOrDefault().Value),
                    CheckOut = reformatDate(Req.Descendants("ToDate").FirstOrDefault().Value),
                    Hotels = HotelListForReq.ToArray(),
                    Occupancies = SearchRooms(Req.Descendants("Rooms").FirstOrDefault()),
                    Currency = currencyCode,
                    Language = "EN",
                    Market = Req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value,
                    Nationality = Req.Descendants("PaxNationality_CountryCode").FirstOrDefault().Value
                };

                var serializer = new JsonSerializer();
                var stringWriter = new StringWriter();
                using (var writer = new JsonTextWriter(stringWriter))
                {
                    writer.QuoteName = false;
                    serializer.Serialize(writer, Criteria);
                }
                string searchCriteria = stringWriter.ToString();//JsonConvert.SerializeObject(criteria); 
                #endregion

                #region GraphQL Query
                string query = string.Empty;
                query += "query {\r\n  hotelX {\r\n search(criteria:" + searchCriteria + ", settings:" + searchSettings + ", filter:" + searchFilter + ") {\r\n auditData {\r\n transactions {\r\n request\r\n response\r\n}\r\n}\r\n errors {\r\n code\r\n  type\r\n description\r\n  }\r\n";
                query += "warnings {\r\n code\r\n type\r\n description\r\n}\r\n options {\r\n id\r\n accessCode\r\n supplierCode\r\n hotelCode\r\n  hotelName\r\n boardCode\r\n price {\r\n net\r\n currency\r\n }\r\n }\r\n}\r\n}\r\n}";


                #endregion
                dynamic Request = new ExpandoObject();
                Request.query = query;
                string json = JsonConvert.SerializeObject(Request);
                LogModel model = new LogModel
                {
                    TrackNo = Req.Descendants("TransID").FirstOrDefault().Value,
                    CustomerID = Convert.ToInt32(Req.Descendants("CustomerID").FirstOrDefault().Value),
                    Logtype = "Search",
                    LogtypeID = 1,
                    Supl_Id = supplierID
                };
                string response = tgr.serverRequest_search(json, model, url, apiKey, tgtimeout);
                #endregion
                #region Response
                var jsonResponse = JsonConvert.DeserializeXmlNode(response, "Response");
                XElement supplierResponse = XElement.Parse(jsonResponse.InnerXml);
                if (supplierResponse.Descendants("hotelCode").Any())
                {
                    currencyCode = supplierResponse.Descendants("options").FirstOrDefault().Descendants("price").FirstOrDefault().Element("currency").Value;

                    string requestContent = Req.Descendants("CountryCode").FirstOrDefault().Value + "_" + currencyCode;
                    var HotelGroups = from Rooms in supplierResponse.Descendants("options")//.FirstOrDefault().Elements("element")
                                      group Rooms by Rooms.Element("hotelCode").Value;



                    try
                    {
                        foreach (var Hotel in HotelGroups)
                        {
                            try
                            {
                                string hotelID = Hotel.Descendants("hotelCode").FirstOrDefault().Value;
                                var result = StaticData.AsEnumerable().Where(dt => dt.Field<string>("HotelID") == hotelID);
                                DataRow[] drow = result.ToArray();
                                if (drow.Count() > 0)
                                {
                                    List<double> Prices = Hotel.Descendants("price").Select(x => Convert.ToDouble(x.Element("net").Value)).OrderBy(x => x).ToList();
                                    double mPrice = Prices.Min();
                                    DataRow dr = drow[0];
                                    string img = string.Empty;
                                    try
                                    {
                                        img = hotelImage(dr["Images"].ToString());
                                    }
                                    catch { }
                                    string xmlouttype = string.Empty;
                                    try
                                    {
                                        if (dmcc == "YALAGO")
                                        {
                                            xmlouttype = "false";
                                        }
                                        else
                                        { xmlouttype = "true"; }
                                    }
                                    catch { }
                                    #region Response XML
                                    try
                                    {
                                        HotelList.Add(new XElement("Hotel",
                                                                        new XElement("HotelID", hotelID),
                                                                        new XElement("HotelName", dr["HotelName"].ToString()),
                                                                        new XElement("PropertyTypeName", ""),
                                                                        new XElement("CountryID", Req.Descendants("CountryID").FirstOrDefault().Value),
                                                                        new XElement("CountryName", Req.Descendants("CountryName").FirstOrDefault().Value),
                                                                        new XElement("CountryCode", Req.Descendants("CountryCode").FirstOrDefault().Value),
                                                                        new XElement("CityId", Req.Descendants("CityID").FirstOrDefault().Value),
                                                                        new XElement("CityCode", Req.Descendants("CityCode").FirstOrDefault().Value),
                                                                        new XElement("CityName", Req.Descendants("CityName").FirstOrDefault().Value),
                                                                        new XElement("AreaId"),
                                                                        new XElement("AreaName"),
                                                                        new XElement("RequestID", requestContent),
                                                                        new XElement("Address", dr["Address"] == null ? "" : dr["Address"]),
                                                                        new XElement("Location"),
                                                                        new XElement("Description"),
                                                                        new XElement("StarRating", dr["Star"]),
                                                                        new XElement("MinRate", mPrice.ToString()),
                                                                        new XElement("HotelImgSmall", img),
                                                                        new XElement("HotelImgLarge", img),
                                                                        new XElement("MapLink"),
                                                                        new XElement("Longitude", dr["Longitude"].ToString()),
                                                                        new XElement("Latitude", dr["Latitude"].ToString()),
                                                                        new XElement("xmloutcustid", customerid),
                                                                        new XElement("xmlouttype", xmlouttype),
                                                                        new XElement("DMC", dmcc),
                                                                        new XElement("SupplierID", supplierID.ToString()),
                                                                        new XElement("Currency", Hotel.Descendants("price").FirstOrDefault().Element("currency").Value),
                                                                        new XElement("Offers"),
                                                                        new XElement("Facilities", new XElement("Facility", "No Facility")),
                                                                        new XElement("Rooms")));
                                    }
                                    catch { }
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        #region Exception
                        CustomException ex1 = new CustomException(ex);
                        ex1.MethodName = "SearchHotelList2 " + dmcc;
                        ex1.PageName = "TGServices_yalago";
                        ex1.CustomerID = customerid;
                        ex1.TranID = Req.Descendants("TransID").FirstOrDefault().Value;
                        SaveAPILog saveex = new SaveAPILog();
                        saveex.SendCustomExcepToDB(ex1);
                        #endregion
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "SearchHotelList3 " + dmcc;
                ex1.PageName = "TGServices_yalago";
                ex1.CustomerID = customerid;
                ex1.TranID = Req.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion
            }
            return HotelList;
        }
        #endregion
        #region Common Functions
        public string reformatDate(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy-MM-dd");
        }
        public string hotelImage(string imgString)
        {
            string[] images = imgString.Split(new char[] { '#' });
            for (int i = 0; i < images.Length; i++)
            {
                string[] splitImageString = images[i].Split(new char[] { '_' });
                if (splitImageString[0].ToUpper().Equals("GENERAL") && !string.IsNullOrEmpty(splitImageString[1]))
                    return splitImageString[1];
            }
            return string.IsNullOrEmpty(imgString) ? string.Empty : images[0].Split(new char[] { '_' })[1];
        }
        #endregion
        #region Rooms For Hotel Search
        public Occupancy[] SearchRooms(XElement Rooms)
        {
            var occupancyList = new List<Occupancy>();
            foreach (XElement room in Rooms.Descendants("RoomPax"))
            {
                int adults = Convert.ToInt32(room.Element("Adult").Value);
                Occupancy occupancy = new Occupancy();
                int i = 0;
                for (i = 0; i < adults; i++)
                {
                    occupancy.Paxes.Add(new Pax { Age = 30 });
                }
                int[] childAges = room.Descendants("ChildAge").Select(x => Convert.ToInt32(x.Value)).ToArray();
                for (int j = 0; j < childAges.Count(); j++)
                {
                    i++;
                    occupancy.Paxes.Add(new Pax { Age = childAges[j] });
                }
                occupancyList.Add(occupancy);
            }
            return occupancyList.ToArray();
        }
        #endregion
        #region TravelGate Configuration
        private void TravelGateConfiguration(int supplierID, string CustomerID)
        {
            #region Supplier Credentials
            XElement SmyCredentials = supplier_Cred.getsupplier_credentials(CustomerID, supplierID.ToString());
            client = SmyCredentials.Element("Client").Value;
            testMode = SmyCredentials.Element("TestMode").Value;
            context = SmyCredentials.Element("Context").Value;
            url = SmyCredentials.Element("Url").Value;
            apiKey = SmyCredentials.Element("ApiKey").Value;
            accessCode = SmyCredentials.Element("AccessCode").Value;
            #endregion
        }
        #endregion
        #region Break hotel list into chunks
        public List<List<T>> BreakIntoChunks<T>(List<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }
            List<List<T>> retVal = new List<List<T>>();
            while (list.Count > 0)
            {
                int count = list.Count > chunkSize ? chunkSize : list.Count;
                retVal.Add(list.GetRange(0, count));
                list.RemoveRange(0, count);
            }

            return retVal;
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