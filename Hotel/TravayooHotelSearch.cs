using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Hotel.Service;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Supplier.Expedia;


namespace TravillioXMLOutService.Hotel
{

    public class TravayooHotelSearch : IDisposable
    {
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        XElement reqTravillio;
        int sup_cutime = 90000;

        XElement bookingroom;
        List<XElement> hotelavailabilitylistextranet;
        #region Logs
        public void WriteToFile(string text)
        {
            try
            {
                string path = Convert.ToString(HttpContext.Current.Server.MapPath(@"~\log.txt"));
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine("---------------------------Pre Booking Response-----------------------------------------");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void WriteToFileErrorfromsupplier(string text)
        {
            try
            {
                string path = Convert.ToString(HttpContext.Current.Server.MapPath(@"~\log.txt"));
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine("---------------------------Pre Booking Response Error From supplier end-----------------------------------------");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void WriteToFilerequest(string text)
        {
            try
            {
                string path = Convert.ToString(HttpContext.Current.Server.MapPath(@"~\log.txt"));
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine("---------------------------Pre Booking Request Log -----------------------------------------");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
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
                    XElement SearReq = req.Descendants("searchRequest").FirstOrDefault();
                    string customerId = SearReq.Element("CustomerID").Value;
                    RTHWKServices hbreq = new RTHWKServices(customerId);

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
                    APILog.SendCustomExcepToDB(ex1);
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



        public XElement CreateCheckAvailability(XElement req)
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
                    reqTravillio = req;






                    int expedia = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "24").Count();


                    if (expedia > 0)
                    {
                        #region Supplier Credentials
                        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(supplier_Cred).TypeHandle);
                        #endregion
                        #region get cut off time
                        try
                        {
                            sup_cutime = supplier_Cred.rmcutoff_time();
                        }
                        catch { }
                        #endregion
                        List<XElement> hotelroomresponse = new List<XElement>();
                        List<XElement> htlst = req.Descendants("GiataHotelList").ToList();
                        List<XElement> Thresult = new List<XElement>();
                        List<XElement> hotelavailabilityresp = new List<XElement>();
                        //for (int i = 0; i < htlst.Count(); i++)  
                        {
                            XElement expediahtlavailresp = null;
                            Thread tid20 = null;
                            #region Expedia
                            if (expedia > 0)
                            {
                                try
                                {
                                    XElement SearReq = req.Descendants("searchRequest").FirstOrDefault();
                                    string customerId = SearReq.Element("CustomerID").Value;
                                    RTHWKServices hbreq = new RTHWKServices(customerId);
                                    tid20 = new Thread(new ThreadStart(() => { expediahtlavailresp = hbreq.GetRoomAvailability(req); }));
                                }
                                catch { }
                            }
                            #endregion


                            #region Thread Start
                            try
                            {

                                if (expedia > 0)
                                {
                                    tid20.Start();
                                }

                            }
                            catch (ThreadStateException te)
                            {

                            }
                            #endregion
                            #region Timer
                            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                            timer.Start();
                            #endregion
                            #region Thread Join

                            if (expedia > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid20.Join(newTime);
                                }
                                catch { }
                            }

                            #endregion
                            #region Thread Abort

                            if (tid20 != null && tid20.IsAlive)
                                tid20.Abort();

                            #endregion
                            #region Merge
                            try
                            {

                                if (expediahtlavailresp != null)
                                {
                                    XElement response = new XElement("Hotels", expediahtlavailresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }

                            }
                            catch { }
                            #endregion
                            XElement respon = new XElement("TotalSupHotels", hotelavailabilityresp);
                            if (hotelavailabilityresp != null)
                            {
                                Thresult.Add(respon);
                            }
                        }
                        hotelroomresponse = Thresult.Descendants("RoomTypes").ToList();
                        #region Bind all rooms
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
                                          new XElement("Hotel",
                                               new XElement("HotelID", Convert.ToString("")),
                                               new XElement("HotelName", Convert.ToString("")),
                                               new XElement("PropertyTypeName", Convert.ToString("")),
                                               new XElement("CountryID", Convert.ToString("")),
                                               new XElement("CountryName", Convert.ToString("")),
                                               new XElement("CountryCode", Convert.ToString("")),
                                               new XElement("CityId", Convert.ToString("")),
                                               new XElement("CityCode", Convert.ToString("")),
                                               new XElement("CityName", Convert.ToString("")),
                                               new XElement("AreaId", Convert.ToString("")),
                                               new XElement("AreaName", Convert.ToString("")),
                                               new XElement("Address", Convert.ToString("")),
                                               new XElement("Location", Convert.ToString("")),
                                               new XElement("Description", Convert.ToString("")),
                                               new XElement("StarRating", Convert.ToString("")),
                                               new XElement("MinRate", Convert.ToString("")),
                                               new XElement("HotelImgSmall", Convert.ToString("")),
                                               new XElement("HotelImgLarge", Convert.ToString("")),
                                               new XElement("MapLink", ""),
                                               new XElement("Longitude", Convert.ToString("")),
                                               new XElement("Latitude", Convert.ToString("")),
                                               new XElement("DMC", ""),
                                               new XElement("SupplierID", ""),
                                               new XElement("Currency", Convert.ToString("")),
                                               new XElement("Offers", ""),
                                               new XElement("Rooms",
                                                 hotelroomresponse
                                                   )
                        )

                                          )
                         ))));
                        #endregion
                        return searchdoc;
                    }
                    else
                    {
                        #region Supplier doesn't Exists
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
                                   new XElement("ErrorTxt", "Supplier doesn't Exists.")
                                           )
                                       )
                      ));
                        return searchdoc;
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    #region Exception
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
            else
            {
                #region Invalid Credential
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
                           new XElement("ErrorTxt", "Invalid Credentials")
                                   )
                               )
              ));
                return searchdoc;
                #endregion
            }
            #endregion
        }















        public XElement HotelPreBooking(XElement req)
        {
            #region XML OUT for Hotel PreBooking (Travayoo)
            HeaderAuth headercheck = new HeaderAuth();
            string username = req.Descendants("UserName").Single().Value;
            string password = req.Descendants("Password").Single().Value;
            string AgentID = req.Descendants("AgentID").Single().Value;
            string ServiceType = req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = req.Descendants("ServiceVersion").Single().Value;
            string supplierid = req.Descendants("SupplierID").Single().Value;
            if (headercheck.Headervalidate(username, password, AgentID, ServiceType, ServiceVersion) == true)
            {
                try
                {
                    #region Supplier Credentials
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(supplier_Cred).TypeHandle);
                    #endregion
                    IEnumerable<XElement> request = req.Descendants("HotelPreBookingRequest");
                    reqTravillio = req;
                    XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                    List<XElement> htlst = req.Descendants("GiataHotelList").ToList();
                    supplierid = htlst[0].Attribute("GSupID").Value;
                    string xmlout = string.Empty;
                    string custName = string.Empty;
                    try
                    {
                        xmlout = htlst[0].Attribute("xmlout").Value;
                        try
                        {
                            custName = htlst[0].Attribute("custName").Value;
                        }
                        catch
                        {
                            custName = "";
                        }
                    }
                    catch
                    {
                        xmlout = "false";
                    }




                    if (supplierid == "24")
                    {
                        if (custName == "")
                        {
                            custName = "Ratehawk";
                        }




                        string customerId = req.Descendants("CustomerID").Single().Value;
                        RTHWKServices hbreq = new RTHWKServices(customerId);




                        var task = Task.Run(async () => await hbreq.PreBooking(req, custName));



                        XElement prebookres = task.Result;


                        return prebookres;
                    }
                    else
                    {
                        #region No Supplier's Details Found
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
                               new XElement("HotelPreBookingResponse",
                                   new XElement("ErrorTxt", "No Supplier's Details Found")
                                           )
                                       )
                      ));
                        return searchdoc;
                        #endregion

                    }








                }
                catch (Exception ex)
                {
                    #region Exception
                    IEnumerable<XElement> request = req.Descendants("HotelPreBookingRequest");
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
                           new XElement("HotelPreBookingResponse",
                               new XElement("ErrorTxt", ex.Message)
                                       )
                                   )
                  ));

                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "HotelPreBooking";
                    ex1.PageName = "TrvHotelPreBooking";
                    ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                    ex1.TranID = req.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    return searchdoc;
                    #endregion
                }




            }
            else
            {
                #region Invalid Credential
                IEnumerable<XElement> request = req.Descendants("HotelPreBookingRequest");
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
                       new XElement("HotelPreBookingResponse",
                           new XElement("ErrorTxt", "Invalid Credentials")
                                   )
                               )
              ));
                return searchdoc;
                #endregion
            }
            #endregion
        }


        public XElement HotelCancellation(XElement req)
        {
            #region XML OUT for Cancellation
            HeaderAuth headercheck = new HeaderAuth();
            string username = req.Descendants("UserName").Single().Value;
            string password = req.Descendants("Password").Single().Value;
            string AgentID = req.Descendants("AgentID").Single().Value;
            string ServiceType = req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = req.Descendants("ServiceVersion").Single().Value;
            string supplierid = req.Descendants("SupplierID").Single().Value;
            if (headercheck.Headervalidate(username, password, AgentID, ServiceType, ServiceVersion) == true)
            {
                #region Cancellation
                try
                {
                    #region Supplier Credentials
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(supplier_Cred).TypeHandle);
                    #endregion
                    // travayooreq = req;


                    #region Expedia
                    if (supplierid == "20")
                    {
                        ExpediaService expobj = new ExpediaService(req.Descendants("CustomerID").Single().Value);
                        XElement BookingCancellation = expobj.BookingCancellation(req);
                        return BookingCancellation;
                    }
                    #endregion


                    #region No Supplier Found
                    else
                    {
                        #region No Supplier Found
                        IEnumerable<XElement> request = req.Descendants("HotelCancellationRequest");
                        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                        XElement cancellationdoc = new XElement(
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
                               new XElement("HotelCancellationResponse",
                                   new XElement("ErrorTxt", "Supplier doesn't exist")
                                           )
                                       )
                      ));
                        return cancellationdoc;
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    #region Exception
                    IEnumerable<XElement> request = req.Descendants("HotelCancellationRequest");
                    XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                    XElement cancellationdoc = new XElement(
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
                           new XElement("HotelCancellationResponse",
                               new XElement("ErrorTxt", ex.Message)
                                       )
                                   )
                  ));

                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "HotelCancellation";
                    ex1.PageName = "TrvHotelCancellation";
                    ex1.CustomerID = req.Descendants("CustomerID").Single().Value;
                    ex1.TranID = req.Descendants("TransID").Single().Value;
                    //APILog.SendCustomExcepToDB(ex1);
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    return cancellationdoc;
                    #endregion
                }
                #endregion
            }
            else
            {
                #region Invalid Credential
                IEnumerable<XElement> request = req.Descendants("HotelCancellationRequest");
                XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                XElement cancellationdoc = new XElement(
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
                       new XElement("HotelCancellationResponse",
                           new XElement("ErrorTxt", "Invalid Credentials")
                                   )
                               )
              ));
                return cancellationdoc;
                #endregion
            }
            #endregion
        }

        public XElement CreateHotelDescriptionDetail(XElement req)
        {

            string hotelproperty = string.Empty;
            string hoteldescription = string.Empty;
            string Phone = string.Empty;
            string Fax = string.Empty;
            string checkintime = string.Empty;
            string checkouttime = string.Empty;
            #region XML OUT for Hotel Details
            HeaderAuth headercheck = new HeaderAuth();
            string username = req.Descendants("UserName").Single().Value;
            string password = req.Descendants("Password").Single().Value;
            string AgentID = req.Descendants("AgentID").Single().Value;
            string ServiceType = req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = req.Descendants("ServiceVersion").Single().Value;
            string supplierid = req.Descendants("SupplierID").Single().Value;
            if (headercheck.Headervalidate(username, password, AgentID, ServiceType, ServiceVersion) == true)
            {
                try
                {
                    reqTravillio = req;
                    #region Supplier Credentials
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(supplier_Cred).TypeHandle);
                    #endregion
                    string flag = "htdescription";
                    XNamespace res = "http://travelcontrol.softexsw.us/";
                    XElement dochtd;
                    XElement responsehotels = null;
                    IEnumerable<XElement> htdetail;

                    #region Expedia
                    if (supplierid == "20")
                    {
                        ExpediaService expobj = new ExpediaService();
                        XElement hotelDetails = expobj.HotelDetails(req);
                        return hotelDetails;
                    }
                    #endregion
                    #region XML OUT

                    if (hoteldescription != null || hoteldescription != "")
                    {
                        #region Hotel Details XML OUT
                        IEnumerable<XElement> request = req.Descendants("hoteldescRequest");
                        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                        XElement hoteldescdoc = new XElement(
                          new XElement(soapenv + "Envelope",
                                    new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                    new XElement(soapenv + "Header",
                                     new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                     new XElement("Authentication", new XElement("AgentID", "TRV"), new XElement("UserName", "Travillio"), new XElement("Password", "ing@tech"), new XElement("ServiceType", "HT_001"), new XElement("ServiceVersion", "v1.0"))),
                                     new XElement(soapenv + "Body",
                                         new XElement(request.Single()),
                               new XElement("hoteldescResponse",
                                   new XElement("Hotels",
                                       new XElement("Hotel",
                                           new XElement("HotelID", Convert.ToString(req.Descendants("HotelID").Single().Value)),
                                           new XElement("Description", Convert.ToString(hoteldescription)),
                                           new XElement("ContactDetails", new XElement("Phone", Convert.ToString(Phone)), new XElement("Fax", Convert.ToString(Fax))),
                                           new XElement("CheckinTime", Convert.ToString(checkintime)),
                                           new XElement("CheckoutTime", Convert.ToString(checkouttime))
                                           ))))));
                        return hoteldescdoc;
                        #endregion
                    }
                    #endregion
                    #region Server is not responding
                    else
                    {
                        #region Server is not responding
                        IEnumerable<XElement> request = req.Descendants("hoteldescRequest");
                        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                        XElement hoteldescdoc = new XElement(
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
                               new XElement("hoteldescResponse",
                                   new XElement("ErrorTxt", "Server is not responding")
                                           )
                                       )
                      ));
                        return hoteldescdoc;
                        #endregion
                    }
                    #endregion




                }
                catch (Exception ex)
                {
                    #region Exception
                    IEnumerable<XElement> request = req.Descendants("hoteldescRequest");
                    XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                    XElement hoteldescdoc = new XElement(
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
                           new XElement("hoteldescResponse",
                               new XElement("ErrorTxt", ex.Message)
                                       )
                                   )
                  ));
                    return hoteldescdoc;
                    #endregion
                }







            }
            else
            {
                #region Invalid Credentials
                IEnumerable<XElement> request = req.Descendants("hoteldescRequest");
                XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                XElement hoteldescdoc = new XElement(
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
                       new XElement("hoteldescResponse",
                           new XElement("ErrorTxt", "Invalid Credentials")
                                   )
                               )
                    ));
                return hoteldescdoc;
                #endregion
            }
            #endregion
        }


        public XElement HotelDetailWithCancellations(XElement req)
        {
            #region XML OUT
            HeaderAuth headercheck = new HeaderAuth();
            string username = req.Descendants("UserName").Single().Value;
            string password = req.Descendants("Password").Single().Value;
            string AgentID = req.Descendants("AgentID").Single().Value;
            string ServiceType = req.Descendants("ServiceType").Single().Value;
            string ServiceVersion = req.Descendants("ServiceVersion").Single().Value;
            //string supplierid = req.Descendants("SupplierID").Single().Value;
            string supplierid = req.Descendants("Room").Attributes("SuppliersID").FirstOrDefault().Value;
            if (headercheck.Headervalidate(username, password, AgentID, ServiceType, ServiceVersion) == true)
            {
                #region XML OUT
                try
                {
                    #region Supplier Credentials
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(supplier_Cred).TypeHandle);
                    #endregion
                    IEnumerable<XElement> request = req.Descendants("hotelcancelpolicyrequest");



                    #region Expedia
                    if (supplierid == "20")
                    {
                        ExpediaService expobj = new ExpediaService(req.Descendants("CustomerID").Single().Value);
                        XElement CancellationPolicy = expobj.CancellationPolicy(req);
                        return CancellationPolicy;
                    }
                    #endregion

                    #region No Supplier Found
                    else
                    {
                        #region Server Not Responding
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
                               new XElement("HotelDetailwithcancellationResponse",
                                   new XElement("ErrorTxt", "Server is not responding")
                                           )
                                       )
                      ));
                        return searchdoc;
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    #region Exception
                    IEnumerable<XElement> request = req.Descendants("hotelcancelpolicyrequest");
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
                           new XElement("HotelDetailwithcancellationResponse",
                               new XElement("ErrorTxt", ex.Message)
                                       )
                                   )
                  ));
                    return searchdoc;
                    #endregion
                }
                #endregion
            }
            else
            {
                #region Invalid Credential
                IEnumerable<XElement> request = req.Descendants("hotelcancelpolicyrequest");
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
                       new XElement("HotelDetailwithcancellationResponse",
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