using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.App_Code;
using TravillioXMLOutService.Hotel;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Supplier.XMLOUT;

namespace TravillioXMLOutService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HotelService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select HotelService.svc or HotelService.svc.cs at the Solution Explorer and start debugging.
    public class HotelService : IHotelService
    {
        public object HotelAvailability(XElement req)
        {

            DateTime Reqstattime = DateTime.Now;
            try
            {
                XElement availabilityresponse = null;
                #region Time Start
                try
                {
                    APILogDetail log2 = new APILogDetail();
                    log2.customerID = Convert.ToInt64(req.Descendants("CustomerID").Single().Value);
                    log2.TrackNumber = req.Descendants("TransID").Single().Value;
                    log2.LogTypeID = 0;
                    log2.LogType = "TimeStart";
                    log2.logrequestXML = req.ToString();
                    SaveAPILog savelogt = new SaveAPILog();
                    savelogt.SaveAPILogs_search(log2);
                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "HotelAvailability";
                    ex1.PageName = "TravillioService";
                    ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                    ex1.TranID = req.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                }
                #endregion
                TravayooHotelSearch reqs = new TravayooHotelSearch();
                availabilityresponse = reqs.HotelAvailability(req);
                if (req.Descendants("Response_Type").Single().Value == "JSON")
                {
                    #region JSON Response
                    return JsonConvert.SerializeXNode(availabilityresponse);
                    #endregion
                }
                else
                {
                    #region XML Response
                    try
                    {
                        APILogDetail log = new APILogDetail();
                        log.customerID = Convert.ToInt64(req.Descendants("CustomerID").Single().Value);
                        log.TrackNumber = req.Descendants("TransID").Single().Value;
                        log.LogTypeID = 1;
                        log.LogType = "Search";
                        log.logrequestXML = req.ToString();
                        log.logresponseXML = availabilityresponse.ToString();
                        log.StartTime = Reqstattime;
                        log.EndTime = DateTime.Now;
                        SaveAPILog savelog = new SaveAPILog();
                        savelog.SaveAPILogs_search(log);
                        #region Time End
                        try
                        {
                            APILogDetail log3 = new APILogDetail();
                            log3.customerID = Convert.ToInt64(req.Descendants("CustomerID").Single().Value);
                            log3.TrackNumber = req.Descendants("TransID").Single().Value;
                            log3.LogTypeID = 0;
                            log3.LogType = "TimeEnd";
                            SaveAPILog savelog3 = new SaveAPILog();
                            savelog3.SaveAPILogs_search(log3);
                        }
                        catch (Exception ex)
                        {
                            CustomException ex1 = new CustomException(ex);
                            ex1.MethodName = "HotelAvailability";
                            ex1.PageName = "TravillioService";
                            ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                            ex1.TranID = req.Descendants("TransID").Single().Value;
                            SaveAPILog saveex = new SaveAPILog();
                            saveex.SendCustomExcepToDB(ex1);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        #region Exception
                        CustomException ex1 = new CustomException(ex);
                        ex1.MethodName = "HotelAvailability";
                        ex1.PageName = "TravillioService";
                        ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                        ex1.TranID = req.Descendants("TransID").Single().Value;
                        SaveAPILog saveex = new SaveAPILog();
                        saveex.SendCustomExcepToDB(ex1);
                        #endregion
                    }
                    SerializeXMLOut serialization = new SerializeXMLOut();
                    return serialization.Serialize(availabilityresponse);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Exception
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelAvailability";
                ex1.PageName = "TravillioService";
                ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                ex1.TranID = req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                string username = req.Descendants("UserName").Single().Value;
                string password = req.Descendants("Password").Single().Value;
                string AgentID = req.Descendants("AgentID").Single().Value;
                string ServiceType = req.Descendants("ServiceType").Single().Value;
                string ServiceVersion = req.Descendants("ServiceVersion").Single().Value;
                IEnumerable<XElement> request = req.Descendants("searchRequest");
                XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                XElement searchdoc = new XElement(
                  new XElement(soapenv + "Envelope",
                            new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                            new XElement(soapenv + "Header",
                             new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                             new XElement("Authentication",
                                 new XElement("AgentID", AgentID),
                                 new XElement("UserName", username),
                                 new XElement("Password", password),
                                 new XElement("ServiceType", ServiceType),
                                 new XElement("ServiceVersion", ServiceVersion))),
                             new XElement(soapenv + "Body",
                                 new XElement(request.Single()),
                       new XElement("searchResponse",
                           new XElement("ErrorTxt", ex.Message)
                                   )
                               )
              ));
                return searchdoc;
                #endregion
            }

        }
        public void WriteToFile(string text)
        {
            try
            {
                string path = Convert.ToString(HttpContext.Current.Server.MapPath(@"~\log.txt"));
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine("---------------------------Hotel Logs-----------------------------------------");
                    writer.Close();
                }
            }
            catch { }
        }
    }
}
