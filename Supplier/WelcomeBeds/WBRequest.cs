using System;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Models.EBookingCenter;
using TravillioXMLOutService.Supplier.Welcomebeds;
using TravillioXMLOutService.Supplier.EBookingCenter;

namespace TravillioXMLOutService.Supplier.Welcomebeds
{
    public class WBRequest
    {

        public XElement ServerRequestHotelSearch(string req, string url, LogModel logmodel, int timeOut)
        {
            DateTime startime = DateTime.Now;
            XElement response = null;
            try
            {
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "POST";
                myhttprequest.Headers.Add(@"SOAPAction: ");
                myhttprequest.ContentType = "text/xml; charset=UTF-8";
                byte[] data = Encoding.ASCII.GetBytes(req);
                myhttprequest.Timeout = timeOut;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();

                Stream responseStream = myhttpresponse.GetResponseStream();
                StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                string pageContent = myReader.ReadToEnd();
                myReader.Close();
                responseStream.Close();
                myhttpresponse.Close();
                response = XElement.Parse(pageContent).RemoveXmlns();
              
            }
            catch (WebException ex)
            {
                string err="";
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = ex.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        err+=sr.ReadToEnd();
                    }
                }
                response.Add(new XElement("Data", new XElement("Exception", ex.Message)));
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "ServerRequestHotelSearch";
                custEx.PageName = "WBRequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                #region Save Logs
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
                    if (logmodel.LogTypeID == 1 && logmodel.LogType == "Search")
                    {
                        SaveAPILog apilog = new SaveAPILog();
                        apilog.SaveAPILogs_search(log);
                    }
                    else
                    {
                        SaveAPILog apilog = new SaveAPILog();
                        apilog.SaveAPILogs(log);
                    }

                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequestHotelSearch";
                    custEx.PageName = "WBRequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                }
                #endregion
            }
            return response;
        }

        public XElement ServerRequest(string req, string url, LogModel logmodel)
        {
            DateTime startime = DateTime.Now;
            XElement response = null;
            try
            {
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "POST";
                myhttprequest.Headers.Add(@"SOAPAction: ");
                myhttprequest.ContentType = "text/xml; charset=UTF-8";
                byte[] data = Encoding.ASCII.GetBytes(req);
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();

                Stream responseStream = myhttpresponse.GetResponseStream();
                StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                string pageContent = myReader.ReadToEnd();
                myReader.Close();
                responseStream.Close();
                myhttpresponse.Close();
                response = XElement.Parse(pageContent).RemoveXmlns();

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
                response.Add(new XElement("Data", new XElement("Exception", ex.Message)));
                CustomException custEx = new CustomException(ex);
                custEx.MethodName = "ServerRequest";
                custEx.PageName = "WBRequest";
                custEx.CustomerID = logmodel.CustomerID.ToString();
                custEx.TranID = logmodel.TrackNo;
                SaveAPILog apilog = new SaveAPILog();
                apilog.SendCustomExcepToDB(custEx);
            }
            finally
            {
                #region Save Logs
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
                    //SaveAPILog apilog = new SaveAPILog();
                    //apilog.SaveAPILogs(log);
                    if (logmodel.LogTypeID == 2 && logmodel.LogType == "RoomAvail")
                    {
                        SaveAPILog apilog = new SaveAPILog();
                        apilog.SaveAPILogs_room(log);
                    }
                    else
                    {
                        SaveAPILog apilog = new SaveAPILog();
                        apilog.SaveAPILogs(log);
                    }

                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = "ServerRequest";
                    custEx.PageName = "WBRequest";
                    custEx.CustomerID = logmodel.CustomerID.ToString();
                    custEx.TranID = logmodel.TrackNo;
                    SaveAPILog apilog = new SaveAPILog();
                    apilog.SendCustomExcepToDB(custEx);
                }
                #endregion
            }
            return response;
        }
    }
}