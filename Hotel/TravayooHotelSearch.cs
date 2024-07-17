using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Hotel.Service;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Transfer.Services;

namespace TravillioXMLOutService.Hotel
{

    public class TravayooHotelSearch : IDisposable
    {
        public XElement HotelAvailability(XElement req)
        {
            HeaderAuth headercheck = new HeaderAuth();
            string username = req.Descendants("UserName").Single().Value;
            string password = req.Descendants("Password").Single().Value;
            string AgentID = req.Descendants("AgentID").Single().Value;
            string ServiceType = req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = req.Descendants("ServiceVersion").Single().Value;
            #region Hotel Availability
            if (headercheck.Headervalidate(username, password, AgentID, ServiceType, ServiceVersion) == true)
            {

                try
                {
                    string customerId = req.Descendants("SearchRequest").Attributes("CustomerID").FirstOrDefault().Value;
                    RTHWKServices hbreq = new RTHWKServices(customerId);
                    XElement SearReq = req.Descendants("SearchRequest").FirstOrDefault();
                    List<XElement> hotels = hbreq.HotelAvailability(SearReq, customerId, "false");

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
                            new XElement("Hotels",
                             hotels
                                )
               ))));
                    return searchdoc;
                }
                catch (Exception ex)
                {
                    #region Exception
                    IEnumerable<XElement> request = req.Descendants("SearchRequest");
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
                           new XElement("SearchResponse",
                               new XElement("ErrorTxt", ex.Message)
                                       )
                                   )
                  ));
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "getTransferAvailability";
                    ex1.PageName = "HB_transferAvail";
                    ex1.CustomerID = req.Descendants("SearchRequest").Attributes("CustomerID").FirstOrDefault().Value;
                    ex1.TranID = req.Descendants("SearchRequest").Attributes("TransID").FirstOrDefault().Value;
                    //APILog.SendCustomExcepToDB(ex1);
                    return searchdoc;
                    #endregion
                }
            }
            else
            {
                #region Invalid Credential
                IEnumerable<XElement> request = req.Descendants("SearchRequest");
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
                       new XElement("SearchResponse",
                           new XElement("ErrorTxt", "Invalid Credentials")
                                   )
                               )
              ));
                return searchdoc;
                #endregion
            }
            #endregion
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