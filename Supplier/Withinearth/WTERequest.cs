using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TravillioXMLOutService.Models;

namespace TravillioXMLOutService.Supplier.Withinearth
{
    public class WTERequest : IDisposable
    {
        #region Request
        public string ServerRequestSearch(string req, string URL, WTELogModel logmodel, int sup_timeout)
        {
            DateTime startime = DateTime.Now;
            string response = string.Empty;

            try
            {
                
                //string reqq = "{\n    \"Token\": \"fxHCAt48fQ0dIE9EdJBhMU5ov89hUkTX4vEMHkgojdim4OIDXZqRBQ5vc374jrHlMwTgyqB5+vYcFqP0fT4Uew==\",\n    \"Request\": {\n        \"Rooms\": [\n            {\n                \"RoomNo\": 1,\n                \"NoofAdults\": 2,\n                \"NoOfChild\": 0,\n                \"ChildAge\": []\n            },\n             {\n                \"RoomNo\": 2,\n                \"NoofAdults\": 1,\n                \"NoOfChild\": 0,\n                \"ChildAge\": []\n            }\n        ],\n        \"CityID\": \"13668\",\n        \"CheckInDate\": \"10-15-2023\",\n        \"CheckOutDate\": \"10-16-2023\",\n        \"NoofNights\": \"1\",\n        \"Nationality\": \"india\",\n        \"Filters\": {\n            \"IsRecommendedOnly\": \"0\",\n            \"IsShowRooms\": \"1\",\n            \"IsOnlyAvailable\": \"1\",\n            \"StarRating\": {\n                \"Min\": 4,\n                \"Max\": 5\n            },\n            \"HotelIds\": \"10,31,33,41,43,44,46\"\n        }\n    },\n    \"AdvancedOptions\": {\n        \"Currency\": \"USD\",\n        \"CustomerIpAddress\":\"123.45.67.89\"\n    }\n}";

                //string jjjkk = "{\"Token\":\"fxHCAt48fQ0dIE9EdJBhMU5ov89hUkTX4vEMHkgojdim4OIDXZqRBQ5vc374jrHlMwTgyqB5+vYcFqP0fT4Uew==\",\"Request\":{\"Rooms\":[{\"RoomNo\":1,\"NoofAdults\":1,\"NoOfChild\":0,\"ChildAge\":[]}],\"CityID\":\"13668\",\"CheckInDate\":\"11-24-2023\",\"CheckOutDate\":\"11-25-2023\",\"NoofNights\":\"1\",\"Nationality\":\"Jordan\",\"Filters\":{\"IsRecommendedOnly\":\"0\",\"IsShowRooms\":\"1\",\"IsOnlyAvailable\":\"1\",\"StarRating\":{\"Min\":3,\"Max\":5},\"HotelIds\":\"10,31,33,41,43,44,46\"}},\"AdvancedOption\":{\"Currency\":\"USD\",\"CustomerIpAddress\":\"192.168.100.5\"}}";
                
                //string reqqt = "{\"Token\":\"fxHCAt48fQ0dIE9EdJBhMU5ov89hUkTX4vEMHkgojdim4OIDXZqRBQ5vc374jrHlMwTgyqB5+vYcFqP0fT4Uew==\",\"Request\":{\"Rooms\":[{\"RoomNo\":1,\"NoofAdults\":2,\"NoOfChild\":0,\"ChildAge\":[]}],\"CityID\":\"13668\",\"CheckInDate\":\"10-15-2023\",\"CheckOutDate\":\"10-16-2023\",\"NoofNights\":\"1\",\"Nationality\":\"india\",\"Filters\":{\"IsRecommendedOnly\":\"0\",\"IsShowRooms\":\"1\",\"IsOnlyAvailable\":\"1\",\"StarRating\":{\"Min\":4,\"Max\":5},\"HotelIds\":\"10,31,33,41,43,44,46\"}},\"AdvancedOptions\":{\"Currency\":\"USD\",\"CustomerIpAddress\":\"123.45.67.89\"}}";
                //string jjkkk = "{\"Token\":\"fxHCAt48fQ0dIE9EdJBhMU5ov89hUkTX4vEMHkgojdim4OIDXZqRBQ5vc374jrHlMwTgyqB5+vYcFqP0fT4Uew==\",\"Request\":{\"Rooms\":[{\"RoomNo\":1,\"NoofAdults\":1,\"NoOfChild\":0,\"ChildAge\":[]}],\"CityID\":\"13668\",\"CheckInDate\":\"11-24-2023\",\"CheckOutDate\":\"11-25-2023\",\"NoofNights\":\"1\",\"Nationality\":\"india\",\"Filters\":{\"IsRecommendedOnly\":\"0\",\"IsShowRooms\":\"1\",\"IsOnlyAvailable\":\"1\",\"StarRating\":{\"Min\":3,\"Max\":5},\"HotelIds\":\"10,31,33,41,43,44,46\"}},\"AdvancedOptions\":{\"Currency\":\"USD\",\"CustomerIpAddress\":\"192.168.100.5\"}}";

                
                ////{\"RoomNo\":2,\"NoofAdults\": 1,\"NoOfChild\": 0,\"ChildAge\": []}],

                //string jj = "{\"Token\":\"fxHCAt48fQ0dIE9EdJBhMU5ov89hUkTX4vEMHkgojdim4OIDXZqRBQ5vc374jrHlMwTgyqB5+vYcFqP0fT4Uew==\",\"Request\":{\"Rooms\":[{\"RoomNo\":1,\"NoofAdults\":1,\"NoOfChild\":0,\"ChildAge\":[]}],\"CityID\":\"13668\",\"CheckInDate\":\"11-24-2023\",\"CheckOutDate\":\"11-25-2023\",\"NoofNights\":\"1\",\"Nationality\":\"Jordan\",\"Filters\":{\"IsRecommendedOnly\":\"0\",\"IsShowRooms\":\"1\",\"IsOnlyAvailable\":\"1\",\"StarRating\":{\"Min\":3,\"Max\":5},\"HotelIds\":\"10,31,33\"}},\"AdvancedOptions\":{\"Currency\":\"USD\",\"CustomerIpAddress\":\"192.168.100.5\"}}";
                
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(URL);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "POST";
                //myhttprequest.PreAuthenticate = true;
                myhttprequest.Timeout = sup_timeout;
                myhttprequest.ContentType = "application/json";
                byte[] data = Encoding.ASCII.GetBytes(req.ToString());
                myhttprequest.ContentLength = data.Length;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                using (HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(myhttpresponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }

            }
            catch (WebException ex)
            {
                string err = "";
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = ex.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        err += sr.ReadToEnd();
                    }
                }
                //response.Add(new XElement("Data", new XElement("Exception", ex.Message)));
                response = ex.Message;
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "ServerRequestSearch";
                custEx.PageName = "WTERequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                APILogDetail log = new APILogDetail();
                log.customerID = logmodel.CustomerID;
                log.LogTypeID = logmodel.LogTypeID;
                log.LogType = logmodel.LogType;
                log.SupplierID = logmodel.SuplId;
                log.TrackNumber = logmodel.TrackNo;
                log.logrequestXML = req;

                log.StartTime = startime;
                log.EndTime = DateTime.Now;
                SaveAPILog savelog = new SaveAPILog();
                try
                {
                    if (logmodel.LogTypeID == 1 && logmodel.LogType == "Search")
                    {
                        log.logresponseXML = response == null ? null : response.ToString();
                        savelog.SaveAPILogs_search(log);
                    }
                    else if (logmodel.LogTypeID == 2 && logmodel.LogType == "RoomAvail")
                    {
                        log.logresponseXML = response == null ? null : response.ToString();
                        savelog.SaveAPILogs_room(log);
                    }
                    else
                    {
                        log.logresponseXML = response == null ? null : response.ToString();
                        savelog.SaveAPILogs(log);
                    }

                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequestSearch";
                    custEx.PageName = "WTERequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = response;
                    savelog.SaveAPILogwithResponseError(log);
                }
            }
            return response;
        }
        public string ServerRequestRoom(string req, string URL, WTELogModel logmodel, string hotelid)
        {
            DateTime startime = DateTime.Now;
            string response = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(URL);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "POST";
                myhttprequest.ContentType = "application/json";
                byte[] data = Encoding.ASCII.GetBytes(req.ToString());
                myhttprequest.ContentLength = data.Length;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                using (HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(myhttpresponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }

            }
            catch (WebException ex)
            {
                string err = "";
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = ex.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        err += sr.ReadToEnd();
                    }
                }
                //response.Add(new XElement("Data", new XElement("Exception", ex.Message)));
                response = ex.Message;
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "ServerRequestSearch";
                custEx.PageName = "WTERequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                APILogDetail log = new APILogDetail();
                log.customerID = logmodel.CustomerID;
                log.LogTypeID = logmodel.LogTypeID;
                log.LogType = logmodel.LogType;
                log.SupplierID = logmodel.SuplId;
                log.TrackNumber = logmodel.TrackNo;
                log.logrequestXML = req;
                log.HotelId = hotelid;
                log.StartTime = startime;
                log.EndTime = DateTime.Now;
                SaveAPILog savelog = new SaveAPILog();
                try
                {
                    if (logmodel.LogTypeID == 2 && logmodel.LogType == "RoomAvail")
                    {
                        log.logresponseXML = response == null ? null : HttpUtility.UrlEncode(response.ToString());
                        savelog.SaveAPILogs_room(log);
                    }
                    else
                    {
                        log.logresponseXML = response == null ? null : HttpUtility.UrlEncode(response.ToString());
                        savelog.SaveAPILogs(log);
                    }

                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequestRoom";
                    custEx.PageName = "WTERequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = response;
                    savelog.SaveAPILogwithResponseError(log);
                }
            }
            return response;
        }
        public string ServerRequestprecheck(string req, string URL, WTELogModel logmodel)
        {
            DateTime startime = DateTime.Now;
            string response = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(URL);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "POST";
                myhttprequest.ContentType = "application/json";
                byte[] data = Encoding.ASCII.GetBytes(req.ToString());
                myhttprequest.ContentLength = data.Length;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                using (HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(myhttpresponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }

            }
            catch (WebException ex)
            {
                string err = "";
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = ex.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        err += sr.ReadToEnd();
                    }
                }
                response = ex.Message;
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "ServerRequest";
                custEx.PageName = "WTERequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                APILogDetail log = new APILogDetail();
                log.customerID = logmodel.CustomerID;
                log.LogTypeID = logmodel.LogTypeID;
                log.LogType = logmodel.LogType;
                log.SupplierID = logmodel.SuplId;
                log.TrackNumber = logmodel.TrackNo;
                log.logrequestXML = req;
                log.StartTime = startime;
                log.EndTime = DateTime.Now;
                SaveAPILog savelog = new SaveAPILog();
                try
                {
                    log.logresponseXML = response == null ? null : HttpUtility.UrlEncode(response.ToString());
                    savelog.SaveAPILogs(log);
                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequest";
                    custEx.PageName = "WTERequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = response;
                    savelog.SaveAPILogwithResponseError(log);
                }
            }
            return response;
        }
        public string ServerRequest(string req, string URL, WTELogModel logmodel)
        {
            DateTime startime = DateTime.Now;
            string response = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(URL);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "POST";
                myhttprequest.ContentType = "application/json";
                byte[] data = Encoding.ASCII.GetBytes(req.ToString());
                myhttprequest.ContentLength = data.Length;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                using (HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(myhttpresponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }

            }
            catch (WebException ex)
            {
                string err = "";
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = ex.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        err += sr.ReadToEnd();
                    }
                }
                response = ex.Message;
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "ServerRequest";
                custEx.PageName = "WTERequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                APILogDetail log = new APILogDetail();
                log.customerID = logmodel.CustomerID;
                log.LogTypeID = logmodel.LogTypeID;
                log.LogType = logmodel.LogType;
                log.SupplierID = logmodel.SuplId;
                log.TrackNumber = logmodel.TrackNo;
                log.logrequestXML = req;
                log.StartTime = startime;
                log.EndTime = DateTime.Now;
                SaveAPILog savelog = new SaveAPILog();
                try
                {
                    log.logresponseXML = response == null ? null : response.ToString();
                    savelog.SaveAPILogs(log);
                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequest";
                    custEx.PageName = "WTERequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = response;
                    savelog.SaveAPILogwithResponseError(log);
                }
            }
            return response;
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