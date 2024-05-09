using System;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;
using TravillioXMLOutService.Supplier.Expedia;
using TravillioXMLOutService.Models.Expedia;
using TravillioXMLOutService.Models;
using RestSharp;
using System.Text.RegularExpressions;


namespace TravillioXMLOutService.Supplier.Expedia
{
    public class ExpediaRequest
    {

        string ip = "82.212.125.250";
        public string ServerRequestSearch(string req, string ApiKey, string sharedSecret, EpsLogModel logmodel, int sup_timeout)
        {
            DateTime startime = DateTime.Now;
            string response = string.Empty;
          
            try
            {
                string authHeader = EpsHelper.getAuthHeader(ApiKey, sharedSecret);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(req);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "GET";
                myhttprequest.PreAuthenticate = true;
                myhttprequest.Timeout = sup_timeout;
                myhttprequest.Headers.Add("Authorization", authHeader); 
                myhttprequest.Headers.Add("Customer-Session-Id", logmodel.TrackNo);
                myhttprequest.Headers.Add("Customer-Ip", ip);
                myhttprequest.ContentType = "application/json";
                myhttprequest.Accept = "application/json";
                myhttprequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
                req += "  Authorization: " + authHeader;
                using (HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(myhttpresponse.GetResponseStream()))
                    {

                        response = streamReader.ReadToEnd();
                        response=Regex.Replace(response, @"\p{C}+", string.Empty); //remove non printable characters
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
                custEx.MethodName = "ServerRequestSearch Issue while reading response";
                custEx.PageName = "ExpediaRequest";
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
                    else
                    {
                        log.logresponseXML = response == null ? null : response.ToString();
                        savelog.SaveAPILogs(log);
                    }
                    
                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequestSearch Error on saving apilog";
                    custEx.PageName = "ExpediaRequest";
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
        public string ServerRequest_Room(string req, string ApiKey, string sharedSecret, EpsLogModel logmodel)
        {
            DateTime startime = DateTime.Now;
            string response = "";
            try
            {
                string authHeader = EpsHelper.getAuthHeader(ApiKey, sharedSecret);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(req);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "GET";
                myhttprequest.PreAuthenticate = true;
                myhttprequest.Timeout = 2300;
                myhttprequest.Headers.Add("Authorization", authHeader);
                myhttprequest.Headers.Add("Customer-Session-Id", logmodel.TrackNo);
                myhttprequest.Headers.Add("Customer-Ip", ip);
                myhttprequest.ContentType = "application/json";
                myhttprequest.Accept = "application/json";
                myhttprequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";

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
                custEx.PageName = "ExpediaRequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                try
                {
                    APILogDetail log = new APILogDetail();
                    log.customerID = logmodel.CustomerID;
                    log.LogTypeID = logmodel.LogTypeID;
                    log.LogType = logmodel.LogType;
                    log.SupplierID = logmodel.SuplId;
                    log.TrackNumber = logmodel.TrackNo;
                    log.logrequestXML = req;
                    log.logresponseXML = response == null ? null : response.ToString();
                    log.StartTime = startime;
                    log.EndTime = DateTime.Now;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SaveAPILogs_room(log);
                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequest";
                    custEx.PageName = "ExpediaRequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                }
            }
            return response;
        }
        public string ServerRequest(string req, string ApiKey, string sharedSecret, EpsLogModel logmodel)
        {
            DateTime startime = DateTime.Now;
            string response = "";
            try
            {
                string authHeader = EpsHelper.getAuthHeader(ApiKey, sharedSecret);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(req);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "GET";
                myhttprequest.PreAuthenticate = true;
                myhttprequest.Timeout = 2300;
                myhttprequest.Headers.Add("Authorization", authHeader); 
                myhttprequest.Headers.Add("Customer-Session-Id", logmodel.TrackNo);
                myhttprequest.Headers.Add("Customer-Ip", ip);
                myhttprequest.ContentType = "application/json";
                myhttprequest.Accept = "application/json";
                myhttprequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
               
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
                custEx.PageName = "ExpediaRequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                try
                {
                    APILogDetail log = new APILogDetail();
                    log.customerID = logmodel.CustomerID;
                    log.LogTypeID = logmodel.LogTypeID;
                    log.LogType = logmodel.LogType;
                    log.SupplierID = logmodel.SuplId;
                    log.TrackNumber = logmodel.TrackNo;
                    log.logrequestXML = req;
                    log.logresponseXML = response == null ? null : response.ToString();
                    log.StartTime = startime;
                    log.EndTime = DateTime.Now;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SaveAPILogs(log);
                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequest";
                    custEx.PageName = "ExpediaRequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                }
            }
            return response;
        }
 

        public string ServerBookRequest(string req, string ApiKey, string sharedSecret, EpsLogModel logmodel, string content, string requestType)
        {
            DateTime startime = DateTime.Now;
            string response = "";
            try
            {
                string authHeader = EpsHelper.getAuthHeader(ApiKey, sharedSecret);
                var client = new RestClient(req);
                client.Timeout = -1;
                RestRequest request;
                if (requestType == "POST")
                {
                    request = new RestRequest(Method.POST);
                }
                else
                {
                    request = new RestRequest(Method.DELETE);
                }
                request.AddHeader("Authorization", authHeader); 
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.129 Safari/537.36");
                request.AddHeader("Customer-Session-Id", logmodel.TrackNo);
                request.AddHeader("Customer-Ip", ip);
                //request.AddHeader("Test", "standard");
                if (!string.IsNullOrEmpty(content))
                {
                    request.AddParameter("application/json", content, ParameterType.RequestBody);
                }

                IRestResponse resp = client.Execute(request);
                response = resp.Content;
                
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
                custEx.MethodName = "ServerBookRequest";
                custEx.PageName = "ExpediaRequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                try
                {
                    APILogDetail log = new APILogDetail();
                    log.customerID = logmodel.CustomerID;
                    log.LogTypeID = logmodel.LogTypeID;
                    log.LogType = logmodel.LogType;
                    log.SupplierID = logmodel.SuplId;
                    log.TrackNumber = logmodel.TrackNo;
                    log.logrequestXML = req + "#passengerdetail=" + content;
                    log.logresponseXML = response == null ? null : response.ToString();
                    log.StartTime = startime;
                    log.EndTime = DateTime.Now;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SaveAPILogs(log);
                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerBookRequest";
                    custEx.PageName = "ExpediaRequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                }
            }
            return response;
        }
          
    }
}