using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using transferAPI.model;
using transferAPI.Repository.Interfaces;
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
            _httpClient.BaseAddress = new Uri(model.BaseUrl);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var authenticationString = $"{model.ClientId}:{model.SecretKey}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);


        }





        public async Task<string> HotelSearchAsync()
        {
            DateTime startime = DateTime.Now;
            string response = string.Empty;

            try
            {


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

        Task<string> HotelSearchAsync(XElement req);
        Task<string> RoomSearchAsync(XElement req);
        Task<string> CancellationPolicyAsync(XElement req);
        Task<string> PreBookingAsync(XElement req);
        Task<string> HotelBookingAsync(XElement req);
        Task<string> CancelBookingAsync(XElement req);


















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


