using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

using TravillioXMLOutService.Hotel.Model;
using TravillioXMLOutService.Hotel.Repository.Interfaces;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Transfer.Helpers;
using TravillioXMLOutService.Transfer.Models.HB;

namespace TravillioXMLOutService.Hotel.Repository
{
    internal class RTHWKRepository : IRTHWKRepository, IDisposable
    {
        RTHWKCredentials model;
        private static readonly HttpClient _httpClient = new HttpClient();
        public RTHWKRepository(RTHWKCredentials _model)
        {
            model = _model;
        }

        HttpClient CreateClient()
        {
            HttpClient _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(model.BaseUrl);
            _httpClient.Timeout = new TimeSpan(0, 1, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var authenticationString = $"{model.ClientId}:{model.SecretKey}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            return _httpClient;
        }

        public async Task<string> HotelSearchAsync(RequestModel reqModel)
        {
            var startTime = DateTime.Now;
            APILogDetail log = new APILogDetail();
            string response = string.Empty;

            try
            {

                var _httpClient = this.CreateClient();
                _httpClient.Timeout = TimeSpan.FromTicks(reqModel.TimeOut);

                using (var request = new HttpRequestMessage(HttpMethod.Post, "search/serp/hotels/"))
                {
                    request.Content = new StringContent(reqModel.RequestStr);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = await _httpClient.SendAsync(request);
                    if (result.IsSuccessStatusCode)
                    {
                        response = await result.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new HttpRequestException(result.ReasonPhrase);
                    }
                }


            }
            catch (Exception ex)
            {
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "ServerRequestSearch Issue while reading response";
                custEx.PageName = "RTHWKRepository";
                custEx.CustomerID = reqModel.Customer.ToString();
                custEx.TranID = reqModel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
                log.logresponseXML = response;

            }
            finally
            {

                log.customerID = reqModel.Customer;
                log.LogTypeID = reqModel.ActionId;
                log.LogType = reqModel.Action;
                log.SupplierID = model.SupplierId;
                log.TrackNumber = reqModel.TrackNo;
                log.logrequestXML = reqModel.RequestStr;

                log.StartTime = startTime;
                log.EndTime = DateTime.Now;
                SaveAPILog savelog = new SaveAPILog();
                try
                {
                    if (log.LogTypeID == 1 && log.LogType == "Search")
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
                    custEx.PageName = "RTHWKRepository";
                    custEx.CustomerID = log.customerID.ToString();
                    custEx.TranID = log.TrackNumber;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = response;
                    savelog.SaveAPILogwithResponseError(log);
                }
            }

            return response;
        }

        public async Task<string> RoomSearchAsync(RequestModel reqModel)
        {
            var startTime = DateTime.Now;
            APILogDetail log = new APILogDetail();
            string response = string.Empty;

            try
            {

                var _httpClient = this.CreateClient();
                _httpClient.Timeout = TimeSpan.FromTicks(reqModel.TimeOut);

                using (var request = new HttpRequestMessage(HttpMethod.Post, "search/hp/"))
                {
                    request.Content = new StringContent(reqModel.RequestStr);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = await _httpClient.SendAsync(request);
                    if (result.IsSuccessStatusCode)
                    {
                        response = await result.Content.ReadAsStringAsync();

                    }
                    else
                    {
                        throw new HttpRequestException(result.ReasonPhrase);
                    }
                }


            }
            catch (Exception ex)
            {
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "RoomSearchAsync Issue while reading response";
                custEx.PageName = "RTHWKRepository";
                custEx.CustomerID = reqModel.Customer.ToString();
                custEx.TranID = reqModel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
                log.logresponseXML = response;

            }
            finally
            {

                log.customerID = reqModel.Customer;
                log.LogTypeID = reqModel.ActionId;
                log.LogType = reqModel.Action;
                log.SupplierID = model.SupplierId;
                log.TrackNumber = reqModel.TrackNo;
                log.logrequestXML = reqModel.RequestStr;

                log.StartTime = startTime;
                log.EndTime = DateTime.Now;
                SaveAPILog savelog = new SaveAPILog();
                try
                {
                    if (log.LogTypeID == 1 && log.LogType == "Search")
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
                    custEx.MethodName = "RoomSearchAsync Error on saving apilog";
                    custEx.PageName = "RTHWKRepository";
                    custEx.CustomerID = log.customerID.ToString();
                    custEx.TranID = log.TrackNumber;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = response;
                    savelog.SaveAPILogwithResponseError(log);
                }
            }

            return response;
        }












        //Task<string> CancellationPolicyAsync(XElement req);
        //Task<string> PreBookingAsync(XElement req);
        //Task<string> HotelBookingAsync(XElement req);
        //Task<string> CancelBookingAsync(XElement req);


        public async Task<string> DownlaodHotelAsync()
        {
            try
            {
                string responseBody = string.Empty;
                using (var request = new HttpRequestMessage(HttpMethod.Post, "hotel/info/dump/"))
                {
                    request.Content = new StringContent("{\n    \"inventory\": \"all\",\n    \"language\": \"en\"\n}");
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();

                        //if (!string.IsNullOrEmpty(responseBody))
                        //{
                        //    File.WriteAllText(basePath + string.Format("HBSRESP-{0}.json", DateTime.Now.Ticks), responseBody);
                        //    result = JsonConvert.DeserializeObject<SearchResponseModel>(responseBody);
                        //}

                        return responseBody;

                    }
                    else
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                var _exception = new XElement("SearchException",
                    new XElement("Message", ex.Message),
                    new XElement("Source", ex.StackTrace),
                    new XElement("HelpLink", ex.HelpLink));
                throw ex;
            }

        }

        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                model = null;
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        ~RTHWKRepository()
        {
            Dispose(false);
        }
        #endregion
    }
}


