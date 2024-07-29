using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


using TravillioXMLOutService.Hotel.Helper;
using TravillioXMLOutService.Hotel.Model;
using TravillioXMLOutService.Hotel.Repository;
using TravillioXMLOutService.Hotel.Repository.Interfaces;
using TravillioXMLOutService.Models;
using Newtonsoft.Json;
using TravillioXMLOutService.Transfer.Models.HB;
using TravillioXMLOutService.Supplier.Expedia;
using System.Threading;
using System.Web.UI.WebControls.Expressions;
using Newtonsoft.Json.Linq;
using System.Reflection;
using TravillioXMLOutService.Air.Models.TBO;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Sockets;
using System.Net;
using TravillioXMLOutService.Models.Expedia;
using System.Security.Policy;


namespace TravillioXMLOutService.Hotel.Service
{
    internal class RTHWKServices
    {

        #region Global vars
        RTHWKCredentials model;
        IRTHWKRepository repo;
        HotelRepository htlRepo;
        string customerid = string.Empty;
        string dmc = string.Empty;
        const int supplierId = 24;
        int chunksize = 250;
        int sup_cutime = 20, threadCount = 2;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        ExpediaRequest epsRequest;
        string sales_environment = "hotel_package";
        #endregion
        public RTHWKServices()
        {
            model = new RTHWKCredentials();
            repo = new RTHWKRepository(model);
            htlRepo = new HotelRepository();
        }
        public RTHWKServices(string customerId)
        {
            model = customerId.ReadCredential(supplierId);
            repo = new RTHWKRepository(model);
            htlRepo = new HotelRepository();
        }

        #region HotelSearch

        RTHWKHotelSearchRequest BindSearchRequest(XElement req)
        {
            req = req.Element("searchRequest");
            RTHWKHotelSearchRequest model = new RTHWKHotelSearchRequest()
            {
                checkin = req.Element("FromDate").Value.RTHWKDate(),
                checkout = req.Element("ToDate").Value.RTHWKDate(),
                residency = req.Element("PaxNationality_CountryCode").Value.RTHWKResidenc(),
                language = RTHWKHelper.RTHWKlanguage(),
                currency = req.Element("DesiredCurrencyCode").Value.RTHWKCurrency(),
            };
            model.guests = req.Element("Rooms").Descendants("RoomPax").Select(x => new Guest
            {
                adults = x.Attribute("Adult").ToINT(),
                children = x.Attribute("ChildAge").Children()
            }).ToList();
            return model;
        }
        public List<XElement> HotelAvailability(XElement req, string custID, string xtype)
        {
            List<XElement> HotelsList = new List<XElement>();
            var _hreq = new HotelSearch()
            {
                SupplierId = supplierId,
                CityCode = req.Descendants("CityID").FirstOrDefault().Value,
                CountryCode = req.Descendants("CountryID").FirstOrDefault().Value,
                HotelId = req.Descendants("HotelID").FirstOrDefault().Value,
                HotelName = req.Descendants("HotelName").FirstOrDefault().Value,
                MinRating = req.Descendants("MinStarRating").FirstOrDefault().GetValueOrDefault(0),
                MaxRating = req.Descendants("MaxStarRating").FirstOrDefault().GetValueOrDefault(0),
            };
            try
            {
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

                var hotelData = htlRepo.GetAllHotelList(_hreq);
                if (hotelData == null || hotelData.Count == 0)
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
                var statiProperties = hotelData.Select(r => r.HotelId).ToList();
                List<List<string>> splitList = statiProperties.SplitHotelList(chunksize);

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
                                    new Thread(async()=> thr1 = await GetSearchAsync(req, splitList[i], hotelData,timeOut,sales_environment)),

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
                                   new Thread(async()=> thr1 = await GetSearchAsync(req, splitList[i], hotelData,timeOut,sales_environment)),
                                  new Thread(async()=> thr2 =  await GetSearchAsync(req, splitList[i+1], hotelData,timeOut,sales_environment)),

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
                                    new Thread(async ()=> thr1 = await GetSearchAsync(req,splitList[i], hotelData,timeOut ,sales_environment)),
                                    new Thread(async()=> thr2 =await  GetSearchAsync(req, splitList[i+1], hotelData,timeOut,sales_environment)),
                                    new Thread(async()=> thr3 =await GetSearchAsync(req,splitList[i+2], hotelData,timeOut,sales_environment)),

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
                                    new Thread(async()=> thr1 =await GetSearchAsync(req, splitList[i], hotelData,timeOut ,sales_environment)),
                                    new Thread(async()=> thr2 =await GetSearchAsync(req, splitList[i+1], hotelData,timeOut,sales_environment)),
                                    new Thread(async()=> thr3 =await GetSearchAsync(req, splitList[i+2], hotelData,timeOut,sales_environment)),
                                     new Thread(async()=> thr4 =await GetSearchAsync(req, splitList[i+2], hotelData,timeOut,sales_environment)),
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
                return HotelsList;
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelAvailability";
                ex1.PageName = "RTHWKServices";
                ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                ex1.TranID = req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                #endregion

                return null;
            }
        }

        public async Task<List<XElement>> GetSearchAsync(XElement _travyoReq, List<string> htlCodes, List<HotelModel> hotelData, int timeout, string sales_environment = "")
        {
            List<XElement> htList = new List<XElement>();
            var reqObj = new RequestModel();
            reqObj.TimeOut = timeout;
            reqObj.StartTime = DateTime.Now;
            reqObj.Customer = Convert.ToInt64(_travyoReq.Attribute("customerId").Value);
            reqObj.TrackNo = _travyoReq.Attribute("transId").Value;
            reqObj.ActionId = (int)_travyoReq.Name.LocalName.GetAction();
            reqObj.Action = _travyoReq.Name.LocalName.GetAction().ToString();
            var _req = BindSearchRequest(_travyoReq);
            _req.ids = htlCodes;
            reqObj.RequestStr = JsonConvert.SerializeObject(_req);
            reqObj.ResponseStr = await repo.HotelSearchAsync(reqObj);
            var response = JsonConvert.DeserializeObject<RTHWKHotelSearchResponse>(reqObj.ResponseStr);
            if (response.status == "ok")
            {
                if (response.data.total_hotels > 0)
                {
                    var hotelResult = from htl in response.data.hotels
                                      join htlD in hotelData
                                      on htl.id equals htlD.HotelId
                                      select new XElement("Hotel", new XElement("HotelID", htl.id),
                                    new XElement("HotelName", htlD.HotelName),
                                    new XElement("PropertyTypeName", ""),
                                    new XElement("CountryID", ""),
                                    new XElement("CountryName", _travyoReq.Descendants("CountryName").FirstOrDefault().Value),
                                    new XElement("CountryCode", htlD.CountryCode),
                                    new XElement("CityId"),
                                    new XElement("CityCode", _travyoReq.Descendants("CityCode").FirstOrDefault().Value),
                                    new XElement("CityName", htlD.CityName),
                                    new XElement("AreaId"),
                                    new XElement("AreaName", ""),
                                    new XElement("RequestID", ""),
                                    new XElement("Address", htlD.HotelAddress),
                                    new XElement("Location", htlD.HotelAddress),
                                    new XElement("Description"),
                                    new XElement("StarRating", htlD.Rating),
                                    new XElement("MinRate", htl.rates.Min(x => x.totalPrice)),
                                    new XElement("HotelImgSmall", htlD.HotelImage),
                                    new XElement("HotelImgLarge", htlD.HotelImage),
                                    new XElement("MapLink"),
                                    new XElement("Longitude", htlD.Longitude),
                                    new XElement("Latitude", htlD.Latitude),
                                    new XElement("xmloutcustid", customerid),
                                    new XElement("xmlouttype", false),
                                    new XElement("DMC", dmc), new XElement("SupplierID", supplierId),
                                    new XElement("Currency", _req.currency),
                                    new XElement("Offers", ""), new XElement("Facilities", null),
                                    new XElement("Rooms", ""),
                                    new XElement("searchType", sales_environment)
                                    );
                    htList = hotelResult.ToList();
                }
            }
            return htList;
        }

        #endregion

        #region Common
        public XElement BindSuplements(TaxData tax_data)
        {
            XElement supplements = new XElement("Supplements");
            if (tax_data != null)
            {
                var _suplements = tax_data.taxes.Select(tax =>
                new XElement("Supplement",
                new XAttribute("suppId", ""),
                new XAttribute("supptType", ""),
                new XAttribute("suppType", "PerRoomSupplement"),
                new XAttribute("suppCurrency", tax.currency_code),
                new XAttribute("suppName", tax.name),
                new XAttribute("suppIsMandatory", !tax.included_by_supplier),
                new XAttribute("suppChargeType", "AtProperty"),
                new XAttribute("suppPrice", tax.amount)));
                supplements.Add(_suplements);
            }
            return supplements;
        }

        public XElement BindAmenity(List<string> amenities_data)
        {
            XElement AmtyItem;
            if (!amenities_data.IsNullOrEmpty())
            {
                var result = from amty in amenities_data
                             select new XElement("Amenity", amty);
                AmtyItem = new XElement("Amenities", result);
            }
            else
            {
                AmtyItem = new XElement("Amenities", new XElement("Amenity", null));
            }
            return AmtyItem;
        }

        #endregion
        #region RoomSearch
        RTHWKRoomSearchRequest BindRoomRequest(XElement req)
        {
            req = req.Element("searchRequest");
            var htl = req.Element("GiataList").Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == supplierId.ToString()).FirstOrDefault();
            RTHWKRoomSearchRequest model = new RTHWKRoomSearchRequest()
            {
                checkin = req.Element("FromDate").Value.RTHWKDate(),
                checkout = req.Element("ToDate").Value.RTHWKDate(),
                residency = req.Element("PaxNationality_CountryCode").Value.RTHWKResidenc(),
                language = RTHWKHelper.RTHWKlanguage(),
                currency = req.Element("DesiredCurrencyCode").Value.RTHWKCurrency(),
                id = htl.Attribute("GHtlID").Value
            };
            model.guests = req.Element("Rooms").Descendants("RoomPax").Select(x => new Guest
            {
                adults = x.Attribute("Adult").ToINT(),
                children = x.Attribute("ChildAge").Children()
            }).ToList();
            return model;
        }

        public async Task<XElement> GetRoomAvailabilityAsync(XElement roomReq, int timeout, string xtype, string htlid, string custid)
        {
            XElement searchReq = roomReq.Descendants("searchRequest").FirstOrDefault();
            XElement RoomDetails = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement("Authentication", new XElement("AgentID", roomReq.Descendants("AgentID").FirstOrDefault().Value),
                new XElement("UserName", roomReq.Descendants("UserName").FirstOrDefault().Value),
                new XElement("Password", roomReq.Descendants("Password").FirstOrDefault().Value),
                new XElement("ServiceType", roomReq.Descendants("ServiceType").FirstOrDefault().Value),
                new XElement("ServiceVersion", roomReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                var _req = BindRoomRequest(roomReq);
                var hotelObj = htlRepo.GetHotelDetail(_req.id);
                var reqObj = new RequestModel();
                reqObj.TimeOut = timeout;
                reqObj.StartTime = DateTime.Now;
                reqObj.Customer = Convert.ToInt64(roomReq.Attribute("customerId").Value);
                reqObj.TrackNo = roomReq.Attribute("transId").Value;
                reqObj.ActionId = (int)roomReq.Name.LocalName.GetAction();
                reqObj.Action = roomReq.Name.LocalName.GetAction().ToString();
                reqObj.RequestStr = JsonConvert.SerializeObject(_req);
                reqObj.ResponseStr = await repo.RoomSearchAsync(reqObj);

                var response = JsonConvert.DeserializeObject<RTHWKHotelSearchResponse>(reqObj.ResponseStr);
                if (response.status == "ok")
                {
                    int counter = 0;
                    var roomsResult = from rate in response.data.hotels[0].rates
                                      join roms in hotelObj.room_groups on rate.rg_ext equals roms.rg_ext
                                      let nightPrice = rate.totalPrice / rate.daily_prices.Count
                                      let roomPrice = nightPrice / searchReq.Descendants("RoomPax").Count()
                                      select new XElement("RoomTypes",
                                             new XAttribute("Index", counter++), new XAttribute("HtlCode", htlid),
                                             new XAttribute("CrncyCode", _req.currency), new XAttribute("DMCType", dmc),
                                             new XAttribute("CUID", customerid), new XAttribute("TotalRate", rate.totalPrice),
                                             searchReq.Descendants("RoomPax").Select((y, i) =>
                                             new XElement("Room",
                                                 new XAttribute("ID", rate.book_hash),
                                                 new XElement("RequestID", rate.book_hash),
                                                 new XAttribute("SuppliersID", supplierId),
                                                 new XAttribute("RoomSeq", i),
                                                 new XAttribute("SessionID", rate.match_hash),
                                                 new XAttribute("RoomType", rate.room_name),
                                                 new XAttribute("OccupancyID", string.Empty),
                                                 new XAttribute("OccupancyName", rate.room_data_trans.bedding_type),
                                                 new XAttribute("MealPlanID", ""),
                                                 new XAttribute("MealPlanName", ""),
                                                 new XAttribute("MealPlanCode", ""),
                                                 new XAttribute("MealPlanPrice", ""),
                                                 new XAttribute("PerNightRoomRate", nightPrice),
                                                 new XAttribute("TotalRoomRate", roomPrice),
                                                 new XAttribute("CancellationDate", ""),
                                                 new XAttribute("CancellationAmount", ""),
                                                 new XAttribute("isAvailable", true),
                                                 new XElement("Offers", ""),
                                                 BindAmenity(rate.amenities_data),
                                                 BindSuplements(rate.payment_options.payment_types.First().tax_data),
                                                 new XElement("AdultNum", y.Element("Adult").Value),
                                                 new XElement("ChildNum", y.Element("Child").Value)))
                                             );
                    XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID"), new XElement("HotelName"), new XElement("PropertyTypeName"),
                                       new XElement("CountryID"), new XElement("CountryName"), new XElement("CityCode"), new XElement("CityName"),
                                       new XElement("AreaId"), new XElement("AreaName"), new XElement("RequestID"), new XElement("Address"), new XElement("Location"),
                                       new XElement("Description"), new XElement("StarRating"), new XElement("MinRate"), new XElement("HotelImgSmall"),
                                       new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("Longitude"), new XElement("Latitude"), new XElement("DMC"),
                                       new XElement("SupplierID"), new XElement("Currency", _req.currency), new XElement("Offers"),
                                       new XElement("Rooms", roomsResult)));

                    RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", hoteldata)));
                }
                else
                {
                    RoomDetails.Add(new XElement(soapenv + "Body", searchReq,
                        new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"))));
                }
                return null;
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


        #endregion



        #region Hotel Details
        public XElement HotelDetails(XElement req)
        {
            XElement hotelDesc = new XElement("Hotels");
            XElement HotelDescReq = req.Descendants("hoteldescRequest").FirstOrDefault();
            XElement hotelDescResdoc = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement("Authentication", new XElement("AgentID", req.Descendants("AgentID").Single().Value),
                new XElement("UserName", req.Descendants("UserName").Single().Value),
                new XElement("Password", req.Descendants("Password").Single().Value),
                new XElement("ServiceType", req.Descendants("ServiceType").Single().Value),
                new XElement("ServiceVersion", req.Descendants("ServiceVersion").Single().Value))));

            try
            {
                var hotelObj = htlRepo.GetHotelDetail(req.Descendants("HotelID").FirstOrDefault().Value);
                if (hotelObj != null)
                {
                    StringBuilder sb = new StringBuilder("<h5>" + hotelObj.name + "</h6>");
                    sb.Append("<p>" + hotelObj.address + "</p>");
                    foreach (var item in hotelObj.description_struct)
                    {
                        sb.Append("<h6><b>" + item.title + "</b></h6>");
                        foreach (var text in item.paragraphs)
                        {
                            sb.Append("<p>" + text + "</p>");
                        }
                    }
                    hotelDescResdoc.Add(new XElement(soapenv + "Body", HotelDescReq, new XElement("hoteldescResponse",
                        new XElement("Hotels", new XElement("Hotel",
                        new XElement("HotelID", req.Descendants("HotelID").FirstOrDefault().Value),
                                        new XElement("Description", sb.ToString()),
                                        HotelImageTag(hotelObj.images),
                                        new XElement("ContactDetails", new XElement("Phone", hotelObj.phone),
                                        new XElement("Fax", "")),
                                        new XElement("CheckinTime", hotelObj.check_in_time), new XElement("CheckoutTime", hotelObj.check_out_time)
                                        )))));
                }
                return hotelDescResdoc;
            }
            catch (Exception ex)
            {
                return hotelDescResdoc;
            }
        }
        public XElement HotelImageTag(List<string> imgList)
        {
            XElement mgItem;
            if (!imgList.IsNullOrEmpty())
            {

                var result = from itm in imgList
                             select new XElement("Image", new XAttribute("Path", itm));
                mgItem = new XElement("Images", result);
            }
            else
            {
                mgItem = new XElement("Images", null);
            }
            return mgItem;

        }
        #endregion


        public XElement RoomCxlPolicy(CancellationPenalties cancellation_penalties, DateTime checkIn, double sumPrice)
        {

            XElement cxlItem = new XElement("CancellationPolicies");
            List<cxlPolicyModel> cxlList = new List<cxlPolicyModel>();
            cxlList = cancellation_penalties.policies.Select(x =>
               new cxlPolicyModel()
               {
                   start = x.start_at.HasValue ? x.start_at.Value : DateTime.Now,
                   end = x.end_at.HasValue ? x.end_at.Value : checkIn,
                   amount = x.amount_show
               }).ToList();

            if (cancellation_penalties.free_cancellation_before != null)
            {
                var lastDate = cancellation_penalties.free_cancellation_before.Value.AddDays(-1);
                var _item = new XElement("CancellationPolicy",
                                       new XAttribute("LastCancellationDate", lastDate.ToString("yyyy-MM-dd")),
                                       new XAttribute("ApplicableAmount", 0),
                                       new XAttribute("NoShowPolicy", 0));
                cxlItem.AddAfterSelf(_item);

                cxlList = cxlList.Where(x => x.amount > 0).OrderBy(x => x.start).Distinct().ToList();
                foreach (var item in cxlList)
                {
                    if (item.start > lastDate && item.start < checkIn)
                    {
                        var cItem = new XElement("CancellationPolicy",
                                                              new XAttribute("LastCancellationDate", item.start.Value.ToString("yyyy-MM-dd")),
                                                              new XAttribute("ApplicableAmount", item.amount),
                                                              new XAttribute("NoShowPolicy", 0));
                        cxlItem.AddAfterSelf(cItem);
                    }
                    else
                    {
                        var cItem = new XElement("CancellationPolicy",
                                                              new XAttribute("LastCancellationDate", item.start.Value.ToString("yyyy-MM-dd")),
                                                              new XAttribute("ApplicableAmount", sumPrice),
                                                              new XAttribute("NoShowPolicy", 0));
                        cxlItem.AddAfterSelf(cItem);
                    }
                }
            }
            else
            {
                cxlItem = new XElement("CancellationPolicies", new XElement("CancellationPolicy",
                              new XAttribute("LastCancellationDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                              new XAttribute("ApplicableAmount", sumPrice),
                              new XAttribute("NoShowPolicy", 1)));


            }
            return cxlItem;
        }




        #region PreBooking
        RTHWKPreBookRequest BindPreBookRequest(XElement req)
        {
            req = req.Element("HotelPreBookingRequest");
            RTHWKPreBookRequest model = new RTHWKPreBookRequest()
            {
                hash = req.Descendants("RequestID").First().Value.ToString()
            };
            return model;
        }

        public async Task<XElement> PreBooking(XElement preBookReq, string xmlout)
        {
            dmc = xmlout;
            XElement preBookReqest = preBookReq.Descendants("HotelPreBookingRequest").FirstOrDefault();
            XElement PreBookResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", preBookReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", preBookReq.Descendants("UserName").FirstOrDefault().Value),
                                       new XElement("Password", preBookReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", preBookReq.Descendants("ServiceType").FirstOrDefault().Value),
                                       new XElement("ServiceVersion", preBookReq.Descendants("ServiceVersion").FirstOrDefault().Value))));
            var checkIn = preBookReq.Element("FromDate").Value.TravayooDateTime();
            try
            {
                var _req = BindPreBookRequest(preBookReq);
                var reqObj = new RequestModel();
                reqObj.StartTime = DateTime.Now;
                reqObj.Customer = Convert.ToInt64(preBookReqest.Element("CustomerID").Value);
                reqObj.TrackNo = preBookReqest.Element("TransID").Value;
                reqObj.ActionId = (int)preBookReqest.Name.LocalName.GetAction();
                reqObj.Action = preBookReqest.Name.LocalName.GetAction().ToString();
                reqObj.RequestStr = JsonConvert.SerializeObject(_req);
                reqObj.ResponseStr = await repo.PreBookingAsync(reqObj);

                var response = JsonConvert.DeserializeObject<RTHWKPreBookResponse>(reqObj.ResponseStr);
                if (response.status == "ok")
                {
                    int counter = 0;
                    var roomsResult = (from rate in response.data.hotels[0].rates
                                       let nightPrice = rate.totalPrice / rate.daily_prices.Count
                                       let roomPrice = nightPrice / preBookReq.Descendants("RoomPax").Count()
                                       select new XElement("RoomTypes",
                                              new XAttribute("Index", counter++),
                                              new XAttribute("DMCType", dmc),
                                              new XAttribute("CUID", customerid), new XAttribute("TotalRate", rate.totalPrice),
                                              preBookReq.Descendants("RoomPax").Select((y, i) =>
                                              new XElement("Room",
                                                  new XAttribute("ID", rate.book_hash),
                                                  new XElement("RequestID", rate.book_hash),
                                                  new XAttribute("SuppliersID", supplierId),
                                                  new XAttribute("RoomSeq", i),
                                                  new XAttribute("SessionID", rate.match_hash),
                                                  new XAttribute("RoomType", rate.room_name),
                                                  new XAttribute("OccupancyID", string.Empty),
                                                  new XAttribute("OccupancyName", rate.room_data_trans.bedding_type),
                                                  new XAttribute("MealPlanID", ""),
                                                  new XAttribute("MealPlanName", rate.meal),
                                                  new XAttribute("MealPlanCode", ""),
                                                  new XAttribute("MealPlanPrice", ""),
                                                  new XAttribute("PerNightRoomRate", nightPrice),
                                                  new XAttribute("TotalRoomRate", roomPrice),
                                                  new XAttribute("CancellationDate", ""),
                                                  new XAttribute("CancellationAmount", ""),
                                                  new XAttribute("isAvailable", true),
                                                  new XElement("Offers", ""),
                                                  BindAmenity(rate.amenities_data),
                                                  BindSuplements(rate.payment_options.payment_types.First().tax_data),
                                                  new XElement("AdultNum", y.Element("Adult").Value),
                                                  new XElement("ChildNum", y.Element("Child").Value))),
                                              RoomCxlPolicy(rate.payment_options.payment_types[0].cancellation_penalties, checkIn, rate.totalPrice)
                                              )).First();

                    string termsCondition = "";
                    XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", preBookReq.Descendants("HotelID").FirstOrDefault().Value),
                                           new XElement("HotelName", preBookReq.Descendants("HotelName").FirstOrDefault().Value), new XElement("Status", true),
                                           new XElement("TermCondition", termsCondition), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"),
                                           new XElement("MapLink"), new XElement("DMC", dmc), new XElement("Currency", ""),
                                           new XElement("Offers"), new XElement("Rooms", roomsResult)));

                    if (response.data.changes.price_changed)
                    {
                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest,
                            new XElement("HotelPreBookingResponse",
                            new XElement("NewPrice", response.data.hotels[0].rates[0].totalPrice), hoteldata)));

                    }
                    else
                    {
                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest,
                        new XElement("HotelPreBookingResponse",
                        new XElement("NewPrice", ""), hoteldata)));
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





        #region Cancellation Policy
        public async  Task<XElement> CancellationPolicyAsync(XElement cxlPolicyReq)
        {
            XElement CxlPolicyReqest = cxlPolicyReq.Descendants("hotelcancelpolicyrequest").FirstOrDefault();
            XElement CxlPolicyResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", cxlPolicyReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cxlPolicyReq.Descendants("UserName").FirstOrDefault().Value),
                                       new XElement("Password", cxlPolicyReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cxlPolicyReq.Descendants("ServiceType").FirstOrDefault().Value),
                                       new XElement("ServiceVersion", cxlPolicyReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                var _req = new
                {
                    book_hash = CxlPolicyReqest.Descendants("RequestID").First().Value.ToString(),
                    language = "en"
                };
                var reqObj = new RequestModel();
                reqObj.StartTime = DateTime.Now;
                reqObj.Customer = Convert.ToInt64(CxlPolicyReqest.Element("CustomerID").Value);
                reqObj.TrackNo = CxlPolicyReqest.Element("TransID").Value;
                reqObj.ActionId = (int)CxlPolicyReqest.Name.LocalName.GetAction();
                reqObj.Action = CxlPolicyReqest.Name.LocalName.GetAction().ToString();
                reqObj.RequestStr = JsonConvert.SerializeObject(_req);
                reqObj.ResponseStr = await repo.CancellationPolicyAsync(reqObj);

                return null;
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
        #endregion


        
        #region Confirm Booking
        Task<XElement> HotelBookingAsync(XElement BookingReq)
        {
            XElement BookReq = BookingReq.Descendants("HotelBookingRequest").FirstOrDefault();
            XElement HotelBookingRes = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", BookingReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", BookingReq.Descendants("UserName").FirstOrDefault().Value), new XElement("Password", BookingReq.Descendants("Password").FirstOrDefault().Value),
                                       new XElement("ServiceType", BookingReq.Descendants("ServiceType").FirstOrDefault().Value), new XElement("ServiceVersion", BookingReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Cancel Booking
        Task<XElement> CancelBookingAsync(XElement cancelReq)
        {
            XElement CxlReq = cancelReq.Descendants("HotelCancellationRequest").FirstOrDefault();
            XElement BookCXlRes = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                  new XElement("Authentication", new XElement("AgentID", cancelReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cancelReq.Descendants("UserName").FirstOrDefault().Value),
                                  new XElement("Password", cancelReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cancelReq.Descendants("ServiceType").FirstOrDefault().Value),
                                  new XElement("ServiceVersion", cancelReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                return null;
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









        #region Hotel Static
        public async Task<string> DownlaodHotelAsync()
        {
            try
            {
                return await repo.DownlaodHotelAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> HotelDetail()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.worldota.net/api/b2b/v3/hotel/info/");

                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("9034:a64cb869-93fc-44a0-8343-102334608f7d")));


                var content = new StringContent("{\n    \"id\": \"crowne_plaza_berlin_city_centre\",\n    \"language\": \"en\"\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<string> HotelSearch()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.worldota.net/api/b2b/v3/search/serp/hotels/");
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("9034:a64cb869-93fc-44a0-8343-102334608f7d")));

                var content = new StringContent("{\n    \"checkin\": \"2024-11-25\",\n    \"checkout\": \"202480-11-26\",\n    \"residency\": \"gb\",\n    \"language\": \"en\",\n    \"guests\": [\n        {\n            \"adults\": 2,\n            \"children\": []\n        }\n    ],\n    \"ids\": [\n        \"access_international_hotel_annex\",\n        \"rila_muam_castle_hotel\",\n        \"alama_hotel_multipurpose\",\n        \"prestige_hotel_limited\",\n        \"chimcherry_hotel_limited\",\n        \"green_suites_villa\",\n        \"kenfeli_international_palmbeach_hotel\"\n    ],\n    \"currency\": \"EUR\"\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> PreBooking()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.worldota.net/api/b2b/v3/hotel/prebook");
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("9034:a64cb869-93fc-44a0-8343-102334608f7d")));

                var content = new StringContent("{\n    \"hash\": \"h-b91ec066-8cb3-57bd-9a0f-2bf9cb12c132\",\n    \"price_increase_percent\": 20\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion


    }
}
