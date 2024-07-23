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



        #region RoomSearch
        RTHWKRoomSearchRequest BindRoomRequest(XElement req)
        {
            req = req.Element("searchRequest");

            var htl = req.Element("GiataList").Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "24").FirstOrDefault();

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
                List<XElement> htList = new List<XElement>();
                var reqObj = new RequestModel();
                reqObj.TimeOut = timeout;
                reqObj.StartTime = DateTime.Now;
                reqObj.Customer = Convert.ToInt64(roomReq.Attribute("customerId").Value);
                reqObj.TrackNo = roomReq.Attribute("transId").Value;
                reqObj.ActionId = (int)roomReq.Name.LocalName.GetAction();
                reqObj.Action = roomReq.Name.LocalName.GetAction().ToString();
                var _req = BindRoomRequest(roomReq);
                reqObj.RequestStr = JsonConvert.SerializeObject(_req);
                reqObj.ResponseStr = await repo.RoomSearchAsync(reqObj);
                var response = JsonConvert.DeserializeObject<RTHWKHotelSearchResponse>(reqObj.ResponseStr);
                if (response.status == "ok")
                {
                    var hotelResult = from htl in response.data.hotels
                                      join htlD in hotelData
                                      on htl.id equals htlD.HotelId
                                      select new XElement("Room", new XAttribute("ID", roomid), new XAttribute("SuppliersID", supplierid),
                                      new XAttribute("RoomSeq", roomSeq),
                                      new XAttribute("SessionID", cxlPolicy), new XAttribute("RoomType", roomName), new XAttribute("OccupancyID", bedgroupid),
                                                          new XAttribute("OccupancyName", bedname), new XAttribute("MealPlanID", promotion),
                                                          new XAttribute("MealPlanName", BoardName), new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""),
                                                          new XAttribute("PerNightRoomRate", roomPrice / nights),
                                                          new XAttribute("TotalRoomRate", roomPrice), new XAttribute("CancellationDate", ""),
                                                          new XAttribute("CancellationAmount", nonRefundableDate),
                                                          new XAttribute("isAvailable", true),
                                                          new XAttribute("searchType", searchtype),
                                                          new XElement("RequestID", pricechecklink), new XElement("Offers", ""), new XElement("PromotionList", new XElement("Promotions", promotion)),
                                                          new XElement("CancellationPolicy"), new XElement("Amenities", new XElement("Amenity")),
                                                          new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                                                          new XElement("Supplements"),
                                                          new XElement(getPriceBreakup(nights, roomPrice)),
                                                          new XElement("AdultNum", roompax.Descendants("Adult").FirstOrDefault().Value),
                                                          new XElement("ChildNum", roompax.Descendants("Child").FirstOrDefault().Value))










                    //XElement hoteldata = new XElement("Hotels", new XElement("Hotel",
                    //    new XElement("HotelID"), new XElement("HotelName"), new XElement("PropertyTypeName"),
                    //    new XElement("CountryID"), new XElement("CountryName"), new XElement("CityCode"), new XElement("CityName"),
                    //    new XElement("AreaId"), new XElement("AreaName"), new XElement("RequestID"), new XElement("Address"), new XElement("Location"),
                    //    new XElement("Description"), new XElement("StarRating"), new XElement("MinRate"), new XElement("HotelImgSmall"),
                    //    new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("Longitude"), new XElement("Latitude"), new XElement("DMC"),
                    //    new XElement("SupplierID"), new XElement("Currency", currency), new XElement("Offers"),
                    //    new XElement(groupDetails)));
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

        #endregion








        //#region Cancellation Policy
        //Task<XElement> CancellationPolicyAsync(XElement cxlPolicyReq)
        //{
        //    XElement CxlPolicyReqest = cxlPolicyReq.Descendants("hotelcancelpolicyrequest").FirstOrDefault();
        //    XElement CxlPolicyResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
        //                               new XElement("Authentication", new XElement("AgentID", cxlPolicyReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cxlPolicyReq.Descendants("UserName").FirstOrDefault().Value),
        //                               new XElement("Password", cxlPolicyReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cxlPolicyReq.Descendants("ServiceType").FirstOrDefault().Value),
        //                               new XElement("ServiceVersion", cxlPolicyReq.Descendants("ServiceVersion").FirstOrDefault().Value))));


        //    try
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Exception
        //        CustomException ex1 = new CustomException(ex);
        //        ex1.MethodName = "CancellationPolicy";
        //        ex1.PageName = "ExpediaService";
        //        ex1.CustomerID = cxlPolicyReq.Descendants("CustomerID").FirstOrDefault().Value;
        //        ex1.TranID = cxlPolicyReq.Descendants("TransID").FirstOrDefault().Value;
        //        SaveAPILog saveex = new SaveAPILog();
        //        saveex.SendCustomExcepToDB(ex1);
        //        CxlPolicyResponse.Add(new XElement(soapenv + "Body", CxlPolicyReqest, new XElement("HotelDetailwithcancellationResponse", new XElement("ErrorTxt", "No cancellation policy found"))));
        //        #endregion
        //        return CxlPolicyResponse;

        //    }
        //}
        //#endregion
        //#region Pre Booking
        //Task<XElement> PreBookingAsync(XElement preBookReq)
        //{
        //    XElement preBookReqest = preBookReq.Descendants("HotelPreBookingRequest").FirstOrDefault();
        //    XElement PreBookResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
        //                               new XElement("Authentication", new XElement("AgentID", preBookReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", preBookReq.Descendants("UserName").FirstOrDefault().Value),
        //                               new XElement("Password", preBookReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", preBookReq.Descendants("ServiceType").FirstOrDefault().Value),
        //                               new XElement("ServiceVersion", preBookReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

        //    try
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Exception
        //        CustomException ex1 = new CustomException(ex);
        //        ex1.MethodName = "PreBooking";
        //        ex1.PageName = "ExpediaService";
        //        ex1.CustomerID = preBookReq.Descendants("CustomerID").FirstOrDefault().Value;
        //        ex1.TranID = preBookReq.Descendants("TransID").FirstOrDefault().Value;
        //        SaveAPILog saveex = new SaveAPILog();
        //        saveex.SendCustomExcepToDB(ex1);
        //        #endregion
        //        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReqest, new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"))));

        //        return PreBookResponse;
        //    }
        //}
        //#endregion
        //#region Confirm Booking
        //Task<XElement> HotelBookingAsync(XElement BookingReq)
        //{
        //    XElement BookReq = BookingReq.Descendants("HotelBookingRequest").FirstOrDefault();
        //    XElement HotelBookingRes = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
        //                               new XElement("Authentication", new XElement("AgentID", BookingReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", BookingReq.Descendants("UserName").FirstOrDefault().Value), new XElement("Password", BookingReq.Descendants("Password").FirstOrDefault().Value),
        //                               new XElement("ServiceType", BookingReq.Descendants("ServiceType").FirstOrDefault().Value), new XElement("ServiceVersion", BookingReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

        //    try
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //#endregion
        //#region Cancel Booking
        //Task<XElement> CancelBookingAsync(XElement cancelReq)
        //{
        //    XElement CxlReq = cancelReq.Descendants("HotelCancellationRequest").FirstOrDefault();
        //    XElement BookCXlRes = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
        //                          new XElement("Authentication", new XElement("AgentID", cancelReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cancelReq.Descendants("UserName").FirstOrDefault().Value),
        //                          new XElement("Password", cancelReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cancelReq.Descendants("ServiceType").FirstOrDefault().Value),
        //                          new XElement("ServiceVersion", cancelReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

        //    try
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        CustomException ex1 = new CustomException(ex);
        //        ex1.MethodName = "BookingCancellation";
        //        ex1.PageName = "Expedia";
        //        ex1.CustomerID = cancelReq.Descendants("CustomerID").FirstOrDefault().Value;
        //        ex1.TranID = cancelReq.Descendants("TransID").FirstOrDefault().Value;
        //        SaveAPILog saveex = new SaveAPILog();
        //        saveex.SendCustomExcepToDB(ex1);
        //        BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", "There is some technical error"))));
        //        return BookCXlRes;
        //    }
        //}
        //#endregion









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
