using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Transfer.Helpers;
using TravillioXMLOutService.Transfer.Models;
using TravillioXMLOutService.Transfer.Models.HB;

namespace TravillioXMLOutService.Repository.Transfer
{
    public class HotelBedRepository : IDisposable
    {

        HBCredentials model;
        public HotelBedRepository(HBCredentials _model)
        {
            model = _model;
        }

        HttpClient CreateClient()
        {
            HttpClient _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(model.ServiceHost);
            _httpClient.Timeout = new TimeSpan(0, 10, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Api-key", model.UserName);
            _httpClient.DefaultRequestHeaders.Add("X-Signature", model.Password);
            return _httpClient;
        }

        public async Task<SearchResponseModel> GetSearchAsync(RequestModel reqModel)
        {
            var startTime = DateTime.Now;
            string stringResponse;
            SearchResponseModel result = null;
            string soapResult = string.Empty;
            try
            {
                var _httpClient = this.CreateClient();

                using (var request = new HttpRequestMessage(HttpMethod.Get, reqModel.RequestStr))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        stringResponse = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(stringResponse))
                        {
                            result = JsonConvert.DeserializeObject<SearchResponseModel>(stringResponse);
                        }
                        else
                        {
                            result = null;
                        }

                    }
                    else
                    {
                        stringResponse = response.ReasonPhrase;
                    }
                    reqModel.ResponseStr = stringResponse.cleanFormJSON();
                    reqModel.EndTime = DateTime.Now;
                    SaveLog(reqModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                var _exception = new XElement("SearchException",
                    new XElement("Message", ex.Message),
                    new XElement("Source", ex.StackTrace),
                    new XElement("HelpLink", ex.HelpLink));
                reqModel.ResponseStr = _exception.ToString();
                reqModel.EndTime = DateTime.Now;
                SaveLog(reqModel);
                throw ex;
            }


        }
               
        public async Task<SearchResponseModel> GetPreBookSearchAsync(LogRequestModel reqModel)
        {
            string stringResponse;
            SearchResponseModel result = null;
            try
            {
                using (var dbConn = new SaveAPILog())
                {
                    stringResponse = await dbConn.GetLogResponseAsync(reqModel);
                    if (reqModel.IsResult)
                    {
                        result = JsonConvert.DeserializeObject<SearchResponseModel>(stringResponse);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                var _exception = new XElement("PrebookException",
                    new XElement("Message", ex.Message),
                    new XElement("Source", ex.StackTrace),
                    new XElement("HelpLink", ex.HelpLink));
                throw ex;
            }
        }
                          
        public async Task<ConfirmResponseModel> GetConfirmAsync(RequestModel reqModel, ConfirmReqModel _req)
        {
            var startTime = DateTime.Now;
            string stringResponse;
            ConfirmResponseModel result = null;
            try
            {
                var _httpClient = this.CreateClient();

                using (var request = new HttpRequestMessage(HttpMethod.Post, reqModel.RequestStr))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = new StringContent(JsonConvert.SerializeObject(_req));

                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        stringResponse = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(stringResponse))
                        {
                            result = JsonConvert.DeserializeObject<ConfirmResponseModel>(stringResponse);
                        }
                        else
                        {
                            result = null;
                        }

                    }
                    else
                    {
                        stringResponse = response.ReasonPhrase;
                    }
                    reqModel.ResponseStr = stringResponse.cleanFormJSON();
                    reqModel.EndTime = DateTime.Now;
                    SaveLog(reqModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                var _exception = new XElement("SearchException",
                    new XElement("Message", ex.Message),
                    new XElement("Source", ex.StackTrace),
                    new XElement("HelpLink", ex.HelpLink));
                reqModel.ResponseStr = _exception.ToString();
                reqModel.EndTime = DateTime.Now;
                SaveLog(reqModel);
                throw ex;
            }


        }
                          
        public async Task<ConfirmResponseModel> CancelBookingAsync(RequestModel reqModel)
        {
            var startTime = DateTime.Now;
            string stringResponse;
            ConfirmResponseModel result = null;
            string soapResult = string.Empty;
            try
            {
                var _httpClient = this.CreateClient();
                using (var request = new HttpRequestMessage(HttpMethod.Delete, reqModel.RequestStr))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        stringResponse = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(stringResponse))
                        {
                            result = JsonConvert.DeserializeObject<ConfirmResponseModel>(stringResponse);
                        }
                        else
                        {
                            result = null;
                        }

                    }
                    else
                    {
                        stringResponse = response.ReasonPhrase;
                    }
                    reqModel.ResponseStr = stringResponse.cleanFormJSON();
                    reqModel.EndTime = DateTime.Now;
                    SaveLog(reqModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                var _exception = new XElement("CancelException",
                    new XElement("Message", ex.Message),
                    new XElement("Source", ex.StackTrace),
                    new XElement("HelpLink", ex.HelpLink));
                reqModel.ResponseStr = _exception.ToString();
                reqModel.EndTime = DateTime.Now;
                SaveLog(reqModel);
                throw ex;
            }

        }
                                    

        public void SaveLog(RequestModel _req)
        {
            APILogDetail log = new APILogDetail();
            log.customerID = _req.Customer;
            log.LogTypeID = _req.ActionId;
            log.LogType = _req.Action;
            log.TrackNumber = _req.TrackNo;
            log.SupplierID = model.SupplierId;
            log.logrequestXML = _req.RequestStr;
            log.logresponseXML = _req.ResponseStr;
            log.StartTime = _req.StartTime;
            log.EndTime = _req.EndTime;
            SaveAPILog savelog = new SaveAPILog();
            savelog.SaveAPILogs_Transfer(log);
        }

        public void SaveException(RequestModel _req, Exception ex)
        {
            CustomException custEx = new CustomException(ex);
            custEx.MethodName = _req.Action;
            custEx.PageName = "Repository";
            custEx.CustomerID = _req.Customer.ToString();
            custEx.TranID = _req.TrackNo;
            SaveAPILog saveex = new SaveAPILog();
            saveex.SendCustomExcepToDB(custEx);
        }



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
