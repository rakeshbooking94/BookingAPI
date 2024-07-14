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


namespace TravillioXMLOutService.Hotel.Service
{
    internal class RTHWKServices
    {

        #region Global vars
        IRTHWKRepository repo;
        RTHWKCredentials model;
        string customerid = string.Empty;
        string dmc = string.Empty;
        const int supplierid = 20;
        int chunksize = 250;
        int sup_cutime = 20, threadCount = 2;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";

        string sales_environment = "hotel_package";

        #endregion
        public RTHWKServices()
        {
            model = new RTHWKCredentials();
            repo = new RTHWKRepository(model);
        }


        #region HotelSearch

        RTHWKHotelSearchRequest BindModel(XElement req)
        {
            req = req.Element("searchRequest");
            RTHWKHotelSearchRequest model = new RTHWKHotelSearchRequest()
            {
                checkin = req.Element("FromDate").Value.RTHWKDate(),
                checkout = req.Element("ToDate").Value.RTHWKDate(),
                residency = req.Element("PaxResidenceID").Value.RTHWKDate(),
                language = RTHWKHelper.RTHWKlanguage(),
                currency = req.Element("CurrencyID").Value.RTHWKCurrency(),
            };
            model.guests = req.Element("Rooms").Descendants("RoomPax").Select(x => new Guest
            {
                adults = x.Attribute("Adult").ToINT(),
                children = x.Attribute("ChildAge").Children()
            }).ToList();
            return model;
        }
        Task<List<XElement>> HotelAvailabilityAsync(XElement req)
        {
            List<XElement> HotelsList = new List<XElement>();
            try
            {
                var reqModel = CreateReqModel(req);
                return null;
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



        RequestModel CreateReqModel(XElement _travyoReq)
        {
            var reqModel = new RequestModel();
            reqModel.StartTime = DateTime.Now;
            reqModel.Customer = Convert.ToInt64(_travyoReq.Attribute("customerId").Value);
            reqModel.TrackNo = _travyoReq.Attribute("transId").Value;
          
            return reqModel;
        }


        public async Task<XElement> GetSearchAsync(RTHWKHotelSearchRequest req, List<string> htlCodes, RequestModel reqObj)
        {
            reqObj.ActionId = (int)_travyoReq.Name.LocalName.GetAction();
            reqObj.Action = _travyoReq.Name.LocalName.GetAction().ToString();



            req.ids = htlCodes;


            reqObj.RequestStr = JsonConvert.SerializeObject(req);
            reqObj.ResponseStr = await repo.HotelSearchAsync(reqObj);
      

            //    result = JsonConvert.DeserializeObject<SearchResponseModel>(responseBody);
        }











        #endregion



        //#region RoomSearch
        //Task<XElement> GetRoomAvailabilityAsync(XElement roomReq)
        //{
        //    XElement searchReq = roomReq.Descendants("searchRequest").FirstOrDefault();
        //    XElement RoomDetails = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
        //                           new XElement("Authentication", new XElement("AgentID", roomReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", roomReq.Descendants("UserName").FirstOrDefault().Value), new XElement("Password", roomReq.Descendants("Password").FirstOrDefault().Value),
        //                           new XElement("ServiceType", roomReq.Descendants("ServiceType").FirstOrDefault().Value), new XElement("ServiceVersion", roomReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

        //    try
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Exception
        //        CustomException ex1 = new CustomException(ex);
        //        ex1.MethodName = "RoomAvailability";
        //        ex1.PageName = "ExpediaService";
        //        ex1.CustomerID = roomReq.Descendants("CustomerID").FirstOrDefault().Value;
        //        ex1.TranID = roomReq.Descendants("TransID").FirstOrDefault().Value;
        //        SaveAPILog saveex = new SaveAPILog();
        //        saveex.SendCustomExcepToDB(ex1);
        //        RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"))));
        //        #endregion
        //        return RoomDetails;
        //    }
        //}
        //#endregion
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
