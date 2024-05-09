using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Xml;
using TravillioXMLOutService.App_Code;
using TravillioXMLOutService.Models;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using System.Collections;
using TravillioXMLOutService.Supplier.DotW;
using TravillioXMLOutService.Supplier.RTS;
using TravillioXMLOutService.Models.Darina;
using TravillioXMLOutService.Supplier.JacTravel;
using TravillioXMLOutService.Common.JacTravel;
using TravillioXMLOutService.Supplier.Miki;
using TravillioXMLOutService.Supplier.Restel;
using TravillioXMLOutService.Supplier.Extranet;
using TravillioXMLOutService.Supplier.TouricoHolidays;
using TravillioXMLOutService.Supplier.Darina;
using TravillioXMLOutService.Supplier.Juniper;
using TravillioXMLOutService.Supplier.Godou;
using TravillioXMLOutService.Supplier.SalTours;
using TravillioXMLOutService.Supplier.SunHotels;
using TravillioXMLOutService.Supplier.Hoojoozat;
using TravillioXMLOutService.Supplier.TravelGate;
using TravillioXMLOutService.Supplier.TBOHolidays;
using TravillioXMLOutService.Supplier.VOT;
using TravillioXMLOutService.Supplier.EBookingCenter;
using TravillioXMLOutService.Supplier.XMLOUTAPI.GetRoom.Common;
using TravillioXMLOutService.Supplier.GoGlobals;
using TravillioXMLOutService.Supplier.Welcomebeds;
using TravillioXMLOutService.Supplier.Expedia;
using TravillioXMLOutService.Supplier.Withinearth;
using TravillioXMLOutService.Supplier.IOL;

namespace TravillioXMLOutService.App_Code
{
    public class TrvRoomAvailabilityNew : IDisposable
    {
        string availDarina = string.Empty;
        XElement reqTravillio;
        int sup_cutime = 90000;
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
                    writer.WriteLine("---------------------------Hotel Availability Response-----------------------------------------");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void WriteToFileResponseTime(string text)
        {
            try
            {
                string path = Convert.ToString(HttpContext.Current.Server.MapPath(@"~\log.txt"));
                using (StreamWriter writer = new StreamWriter(path, true))
                {


                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine("---------------------------Time Calculate-----------------------------------------");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region Hotel Availability (XML OUT for Travayoo)
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
                    int darina = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "1").Count();
                    int tourico = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "2").Count();
                    int extranet = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "3").Count();
                    int hotelbeds = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "4").Count();
                    int DOTW = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "5").Count();
                    int hotelspro = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "6").Count();
                    int travco = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "7").Count();
                    int JacTravel = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "8").Count();
                    int RTS = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "9").Count();
                    int Miki = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "11").Count();
                    int restel = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "13").Count();
                    int JuniperW2M = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "16").Count();
                    int EgyptExpress = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "17").Count();
                    int SalTour = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "19").Count();
                    int expedia = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "20").Count();
                    int tbo = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "21").Count();
                    int LOH = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "23").Count();
                    int goglobal = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "27").Count();
                    int cosmobeds = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "29").Count();
                    int stuba = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "30").Count();
                    int Gadou = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "31").Count();
                    int yalago = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "32").Count();
                    int LCI = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "35").Count();
                    int SunHotels = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "36").Count();
                    int totalstay = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "37").Count();
                    int SmyRooms = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "39").Count();
                    int AlphaTours = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "41").Count();
                    int welcomebeds = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "44").Count();
                    int Hoojoozat = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "45").Count();
                    int vot = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "46").Count();
                    int ebookingcenter = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "47").Count();
                    int wte = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "48").Count();
                    int iol = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "50").Count();
                    int bookingexpress = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "501").Count();
                    int etripcenter = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "502").Count();
                    int holidaysarabia = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "503").Count();
                    int novodestino = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "504").Count();
                    int plaza = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "533").Count();
                    int cbh = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "534").Count();
                    int adventures = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "627").Count();
                    int gtsbeds = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "628").Count();
                    int h2go = req.Descendants("GiataHotelList").Attributes("GSupID").Where(x => x.Value == "629").Count();

                    if (darina > 0 || tourico > 0 || extranet > 0 || hotelbeds > 0 || DOTW > 0 || hotelspro > 0 || travco > 0 || JacTravel > 0 || RTS > 0 || Miki > 0 || restel > 0 || JuniperW2M > 0 || EgyptExpress > 0 || SalTour > 0 || expedia > 0 || tbo > 0 || LOH > 0 || goglobal > 0 || cosmobeds > 0 || stuba > 0 || Gadou > 0 || yalago > 0 || LCI > 0 || SunHotels > 0 || totalstay > 0 || SmyRooms > 0 || AlphaTours > 0 || welcomebeds > 0 || Hoojoozat > 0 || vot > 0 || ebookingcenter > 0 || wte > 0 || iol > 0 || bookingexpress > 0 || etripcenter > 0 || holidaysarabia > 0 || novodestino > 0 || plaza > 0 || cbh > 0 || adventures > 0 || gtsbeds > 0 || h2go > 0)
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
                            List<XElement> darinahotelavailabilityresp = new List<XElement>();
                            List<XElement> touricohotelavailabilityresp = new List<XElement>();
                            List<XElement> extranethotelavailabilityresp = new List<XElement>();
                            List<XElement> hotelbedshotelavailabilityresp = new List<XElement>();
                            XElement DOTWhotelavailabilityresp = null;
                            List<XElement> hotelsprohotelavailabilityresp = new List<XElement>();
                            XElement travcohotelavailabilityresp = null;
                            List<XElement> JacTravelhotelavailabilityresp = null;
                            List<XElement> RTShotelavailabilityresp = new List<XElement>();
                            XElement Mikihotelavailabilityresp = null;
                            XElement restelhotelavailabilityresp = null;
                            XElement JuniperW2Mhotelavailabilityresp = null;
                            XElement EgyptExpresshotelavailabilityresp = null;
                            XElement SalTourhotelavailabilityresp = null;
                            XElement expediahtlavailresp = null;
                            XElement tbohotelavailabilityresp = null;
                            XElement JuniperLOHhotelavailabilityresp = null;
                            List<XElement> goglobalhotelavailabilityresp = new List<XElement>();
                            XElement cosmobedsRoomshotelavailabilityresp = null;
                            XElement stubaRoomshotelavailabilityresp = null;
                            XElement yalagoRoomshotelavailabilityresp = null;
                            XElement JuniperLCIhotelavailabilityresp = null;
                            XElement Gadouhotelavailabilityresp = null;
                            XElement SunHotelshotelavailabilityresp = null;
                            List<XElement> totalstayhotelavailabilityresp = null;
                            XElement SmyRoomshotelavailabilityresp = null;
                            XElement AlphaTourshotelavailabilityresp = null;
                            XElement welcomebedshotelavailabilityresp = null;
                            XElement Hoojoozathotelavailabilityresp = null;
                            XElement vothotelavailabilityresp = null;
                            XElement ebookingcenterhotelavailabilityresp = null;
                            XElement wteRoomshotelavailabilityresp = null;
                            XElement iolRoomshotelavailabilityresp = null;
                            XElement bookingexpresshotelavailabilityresp = null;
                            XElement etripcenterhotelavailabilityresp = null;
                            XElement holidaysarabiahotelavailabilityresp = null;
                            XElement novodestinohotelavailabilityresp = null;
                            XElement plazahotelavailabilityresp = null;
                            XElement cbhhotelavailabilityresp = null;
                            XElement adventureshotelavailabilityresp = null;
                            XElement gtsbedshotelavailabilityresp = null;
                            XElement h2gohotelavailabilityresp = null;
                            Thread tid1 = null;
                            Thread tid2 = null;
                            Thread tid3 = null;
                            Thread tid4 = null;
                            Thread tid5 = null;
                            Thread tid6 = null;
                            Thread tid7 = null;
                            Thread tid8 = null;
                            Thread tid9 = null;
                            Thread tid11 = null;
                            Thread tid13 = null;
                            Thread tid16 = null;
                            Thread tid17 = null;
                            Thread tid19 = null;
                            Thread tid20 = null;
                            Thread tid21 = null;
                            Thread tid23 = null;
                            Thread tid27 = null;
                            Thread tid29 = null;
                            Thread tid30 = null;
                            Thread tid31 = null;
                            Thread tid32 = null;
                            Thread tid35 = null;
                            Thread tid36 = null;
                            Thread tid37 = null;
                            Thread tid39 = null;
                            Thread tid41 = null;
                            Thread tid44 = null;
                            Thread tid45 = null;
                            Thread tid46 = null;
                            Thread tid47 = null;
                            Thread tid48 = null;
                            Thread tid50 = null;
                            Thread tid501 = null;
                            Thread tid502 = null;
                            Thread tid503 = null;
                            Thread tid504 = null;
                            Thread tid533 = null;
                            Thread tid534 = null;
                            Thread tid627 = null;
                            Thread tid628 = null;
                            Thread tid629 = null;
                            #region Bind Static Data
                            XElement doccurrency = null;
                            XElement docmealplan = null;
                            XElement dococcupancy = null;
                            XElement statictouricohotellist = null;
                            XElement CosmobedsMealPlan = null;
                            XElement StubaMealPlan = null;
                            XElement yalagoMealPlan = null;
                            XElement SmyRoomsMealPlans = null;
                            XElement hpromealtype = null;
                            XElement wteMealPlans = null;
                            XElement iolMealPlans = null;
                            if (darina > 0)
                            {
                                doccurrency = XElement.Load(HttpContext.Current.Server.MapPath(@"~\App_Data\Darina\currency.xml"));
                                docmealplan = XElement.Load(HttpContext.Current.Server.MapPath(@"~\App_Data\Darina\MealPlan.xml"));
                                dococcupancy = XElement.Load(HttpContext.Current.Server.MapPath(@"~\App_Data\Darina\Occupancy.xml"));
                            }
                            if (tourico > 0)
                            {
                                statictouricohotellist = XElement.Load(HttpContext.Current.Server.MapPath(@"~\App_Data\Tourico\HotelInfo.xml"));
                            }
                            if (hotelspro > 0)
                            {
                                hpromealtype = XElement.Load(HttpContext.Current.Server.MapPath(@"~\App_Data\HotelsPro\mealtype.xml"));
                            }
                            if (cosmobeds > 0)
                            {
                                CosmobedsMealPlan = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/Cosmobeds_Meals.xml"));
                            }
                            if (stuba > 0)
                            {
                                StubaMealPlan = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/Stuba_Meals.xml"));
                            }
                            if (yalago > 0)
                            {
                                yalagoMealPlan = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/Yalago_Meals.xml"));
                            }
                            if (SmyRooms > 0)
                            {
                                SmyRoomsMealPlans = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/Smy_Meals.xml"));
                            }
                            if (wte > 0)
                            {
                                wteMealPlans = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/Withinearth_Meals.xml"));
                            }
                            if (iol > 0)
                            {
                                iolMealPlans = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SmyRooms/IOL_Meals.xml"));
                            }
                            #endregion
                            #region Darina
                            if (darina > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "1").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "Darina";
                                    //}
                                    dr_Roomavail drreq = new dr_Roomavail();
                                    tid1 = new Thread(new ThreadStart(() => { darinahotelavailabilityresp = drreq.GetRoomAvail_DarinaOUT_merge(req, dococcupancy, docmealplan, doccurrency); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Tourico
                            if (tourico > 0)
                            {
                                try
                                {
                                    string dmc = string.Empty;
                                    XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "2").FirstOrDefault();
                                    string htlid = htlele.Attribute("GHtlID").Value;
                                    string xmlout = string.Empty;
                                    try
                                    {
                                        xmlout = htlele.Attribute("xmlout").Value;
                                    }
                                    catch { xmlout = "false"; }
                                    if (xmlout == "true")
                                    {
                                        dmc = "HA";
                                    }
                                    else
                                    {
                                        dmc = "Tourico";
                                    }
                                    Tr_GetRoomAvail hbreq = new Tr_GetRoomAvail();
                                    XElement trcredential = null; //XElement.Load(HttpContext.Current.Server.MapPath(@"~\App_Data\Tourico\Credential.xml"));
                                    //XElement statictouricohotellist = XElement.Load(HttpContext.Current.Server.MapPath(@"~\App_Data\Tourico\HotelInfo.xml"));
                                    tid2 = new Thread(new ThreadStart(() => { touricohotelavailabilityresp = hbreq.GetRoomAvail_Tourico(req, trcredential, statictouricohotellist, htlid, dmc); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Extranet
                            if (extranet > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "3").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "Extranet";
                                    //}
                                    ExtGetRoomAvail extreq = new ExtGetRoomAvail();
                                    tid3 = new Thread(new ThreadStart(() => { extranethotelavailabilityresp = extreq.GetRoomAvail_ExtranetOUT_merge(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Hotelbeds
                            if (hotelbeds > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "4").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "HotelBeds";
                                    //}
                                    RoomAvailabilityHotelBeds hbreq = new RoomAvailabilityHotelBeds();
                                    tid4 = new Thread(new ThreadStart(() => { hotelbedshotelavailabilityresp = hbreq.getroomavail_HBOUT(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region DOTW
                            if (DOTW > 0)
                            {
                                try
                                {
                                    DotwService dotwObj = new DotwService();
                                    tid5 = new Thread(new ThreadStart(() => { DOTWhotelavailabilityresp = dotwObj.GetRoomAvail_DOTWOUT(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region HotelsPro
                            if (hotelspro > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "6").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "HotelsPro";
                                    //}
                                    HotelsProRoomAvail hpreq = new HotelsProRoomAvail();
                                    tid6 = new Thread(new ThreadStart(() => { hotelsprohotelavailabilityresp = hpreq.getroomavailability_hpro(req, hpromealtype); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Travco
                            if (travco > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "7").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "Travco";
                                    //}
                                    Travco travcoObj = new Travco();
                                    tid7 = new Thread(new ThreadStart(() => { travcohotelavailabilityresp = travcoObj.GetRoomAvail_travcoOUT(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region JacTravels
                            if (JacTravel > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "8").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "JacTravel";
                                    //}
                                    Jac_RoomRequest hbreq = new Jac_RoomRequest();
                                    tid8 = new Thread(new ThreadStart(() => { JacTravelhotelavailabilityresp = hbreq.getroomavailability_jactotal(req, 8); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region RTS
                            if (RTS > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "9").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "RTS";
                                    //}
                                    HTlStaticData obj = new HTlStaticData();
                                    XDocument doc = obj.GetRTSRoomAvailable(req, 9);
                                    RTS_RoomAvail romavailbj = new RTS_RoomAvail();
                                    tid9 = new Thread(new ThreadStart(() => { RTShotelavailabilityresp = romavailbj.getroomavailability_RTS(doc, req).ToList(); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Miki
                            if (Miki > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "11").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "Miki";
                                    //}
                                    MikiInternal mik = new MikiInternal();
                                    tid11 = new Thread(new ThreadStart(() => { Mikihotelavailabilityresp = mik.GetRoomAvail_mikiOUT(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Restel
                            if (restel > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "13").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "Restel";
                                    //}
                                    RestelServices rs = new RestelServices();
                                    //tid13 = new Thread(new ThreadStart(() => { restelhotelavailabilityresp = rs.RoomAvailability(req, dmc, htlid, 13); }));
                                    tid13 = new Thread(new ThreadStart(() => { restelhotelavailabilityresp = rs.RoomAvailability_restel(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Juniper W2M
                            if (JuniperW2M > 0)
                            {
                                try
                                {
                                    #region Juniper
                                    //int customerid = Convert.ToInt32(req.Descendants("CustomerID").Single().Value);
                                    //JuniperResponses rs = new JuniperResponses(16, customerid);
                                    //tid16 = new Thread(new ThreadStart(() => { JuniperW2Mhotelavailabilityresp = rs.RoomAvailability_juniper(req, "W2M", "16"); }));
                                    JuniperResponses rs = new JuniperResponses();
                                    tid16 = new Thread(new ThreadStart(() => { JuniperW2Mhotelavailabilityresp = rs.RoomAvailability_juniper(req, "16"); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region EgyptExpress
                            if (EgyptExpress > 0)
                            {
                                try
                                {
                                    #region EgyptExpress
                                    //int customerid = Convert.ToInt32(req.Descendants("CustomerID").Single().Value);
                                    JuniperResponses rs = new JuniperResponses();
                                    tid17 = new Thread(new ThreadStart(() => { EgyptExpresshotelavailabilityresp = rs.RoomAvailability_juniper(req, "17"); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Sal Tours
                            if (SalTour > 0)
                            {
                                try
                                {
                                    string dmc = string.Empty;
                                    XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "19").FirstOrDefault();
                                    string htlid = htlele.Attribute("GHtlID").Value;
                                    string xmlout = string.Empty;
                                    try
                                    {
                                        xmlout = htlele.Attribute("xmlout").Value;
                                    }
                                    catch { xmlout = "false"; }
                                    if (xmlout.ToUpper() == "TRUE")
                                        dmc = "HA";
                                    else
                                        dmc = "SALTOURS";
                                    SalServices sser = new SalServices();
                                    tid19 = new Thread(new ThreadStart(() => { SalTourhotelavailabilityresp = sser.RoomAvailability(req, dmc, htlid); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Expedia
                            if (expedia > 0)
                            {
                                try
                                {
                                    ExpediaService expobj = new ExpediaService();
                                    tid20 = new Thread(new ThreadStart(() => { expediahtlavailresp = expobj.GetRoomAvail_ExpediaOUT(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region TBO Holidays
                            if (tbo > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "21").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout.ToUpper() == "TRUE")
                                    //    dmc = "HA";
                                    //else
                                    //    dmc = "TBO";
                                    TBOServices tbs = new TBOServices();
                                    tid21 = new Thread(new ThreadStart(() => { tbohotelavailabilityresp = tbs.getroomavail_tboOUT(req); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region LOH
                            if (LOH > 0)
                            {
                                try
                                {
                                    #region LOH
                                    //int customerid = Convert.ToInt32(req.Descendants("CustomerID").Single().Value);
                                    JuniperResponses rs = new JuniperResponses();
                                    tid23 = new Thread(new ThreadStart(() => { JuniperLOHhotelavailabilityresp = rs.RoomAvailability_juniper(req, "23"); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region GoGlobal
                            if (goglobal > 0)
                            {
                                try
                                {
                                    #region GoGlobal
                                    GoGlobal rs = new GoGlobal(req.Descendants("CustomerID").FirstOrDefault().Value);
                                    tid27 = new Thread(new ThreadStart(() => { goglobalhotelavailabilityresp = rs.getroomavail_GoGlobalOUT(req); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Cosmobeds
                            if (cosmobeds > 0)
                            {
                                try
                                {
                                    TGServices tgs = new TGServices();
                                    tid29 = new Thread(new ThreadStart(() => { cosmobedsRoomshotelavailabilityresp = tgs.GetRoomAvail_smyroomOUT(req, CosmobedsMealPlan, 29); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region stuba
                            if (stuba > 0)
                            {
                                try
                                {
                                    TGServices tgs = new TGServices();
                                    tid30 = new Thread(new ThreadStart(() => { stubaRoomshotelavailabilityresp = tgs.GetRoomAvail_smyroomOUT(req, StubaMealPlan, 30); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Gadou
                            if (Gadou > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "31").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout.ToUpper() == "TRUE")
                                    //    dmc = "HA";
                                    //else
                                    //    dmc = "GADOU";
                                    JuniperResponses gds = new JuniperResponses();
                                    tid31 = new Thread(new ThreadStart(() => { Gadouhotelavailabilityresp = gds.RoomAvailability_juniper(req, "31"); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region YALAGO
                            if (yalago > 0)
                            {
                                try
                                {
                                    TGServices tgs = new TGServices();
                                    tid32 = new Thread(new ThreadStart(() => { yalagoRoomshotelavailabilityresp = tgs.GetRoomAvail_smyroomOUT(req, yalagoMealPlan, 32); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region LCI
                            if (LCI > 0)
                            {
                                try
                                {
                                    #region LCI
                                    JuniperResponses rs = new JuniperResponses();
                                    tid35 = new Thread(new ThreadStart(() => { JuniperLCIhotelavailabilityresp = rs.RoomAvailability_juniper(req, "35"); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region SunHotels
                            if (SunHotels > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "36").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "SunHotels";
                                    //}
                                    #region Juniper
                                    //int customerid = Convert.ToInt32(req.Descendants("CustomerID").Single().Value);
                                    SunHotelsResponse objRs = new SunHotelsResponse();
                                    tid36 = new Thread(new ThreadStart(() => { SunHotelshotelavailabilityresp = objRs.GetRoomAvail_sunhotelOUT(req); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Total Stay
                            if (totalstay > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "37").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "TotalStay";
                                    //}
                                    //IEnumerable<XElement> responsehotels = null;
                                    Jac_RoomRequest hbreq = new Jac_RoomRequest();
                                    tid37 = new Thread(new ThreadStart(() => { totalstayhotelavailabilityresp = hbreq.getroomavailability_jactotal(req, 37); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region SmyRooms
                            if (SmyRooms > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "39").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout.ToUpper() == "TRUE")
                                    //    dmc = "HA";
                                    //else
                                    //    dmc = "SMYROOMS";
                                    //TGServices tgs = new TGServices(39, req.Descendants("CustomerID").FirstOrDefault().Value);
                                    TGServices tgs = new TGServices();
                                    tid39 = new Thread(new ThreadStart(() => { SmyRoomshotelavailabilityresp = tgs.GetRoomAvail_smyroomOUT(req, SmyRoomsMealPlans,39); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region AlphaTours
                            if (AlphaTours > 0)
                            {
                                try
                                {
                                    #region AlphaTours
                                    //int customerid = Convert.ToInt32(req.Descendants("CustomerID").Single().Value);
                                    JuniperResponses rs = new JuniperResponses();
                                    tid41 = new Thread(new ThreadStart(() => { AlphaTourshotelavailabilityresp = rs.RoomAvailability_juniper(req, "41"); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Hoojoozat
                            if (Hoojoozat > 0)
                            {
                                try
                                {
                                    string dmc = string.Empty;
                                    XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "45").FirstOrDefault();
                                    string htlid = htlele.Attribute("GHtlID").Value;
                                    string xmlout = string.Empty;
                                    try
                                    {
                                        xmlout = htlele.Attribute("xmlout").Value;
                                    }
                                    catch { xmlout = "false"; }
                                    if (xmlout == "true")
                                    {
                                        dmc = "HA";
                                    }
                                    else
                                    {
                                        dmc = "Hoojoozat";
                                    }
                                    #region Hoojoozat
                                    string customerid = req.Descendants("CustomerID").Single().Value;
                                    HoojService rs = new HoojService(customerid);
                                    tid45 = new Thread(new ThreadStart(() => { Hoojoozathotelavailabilityresp = rs.RoomAvailability(req, dmc, htlid); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region WelcomeBeds
                            if (welcomebeds > 0)
                            {
                                try
                                {
                                    #region WelcomeBeds
                                    WelcomebedsService rs = new WelcomebedsService();
                                    tid44 = new Thread(new ThreadStart(() => { welcomebedshotelavailabilityresp = rs.GetRoomAvail_welcomebedsOUT(req); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Vot
                            if (vot > 0)
                            {
                                try
                                {
                                    //string dmc = string.Empty;
                                    //XElement htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "46").FirstOrDefault();
                                    //string htlid = htlele.Attribute("GHtlID").Value;
                                    //string xmlout = string.Empty;
                                    //try
                                    //{
                                    //    xmlout = htlele.Attribute("xmlout").Value;
                                    //}
                                    //catch { xmlout = "false"; }
                                    //if (xmlout == "true")
                                    //{
                                    //    dmc = "HA";
                                    //}
                                    //else
                                    //{
                                    //    dmc = "VOT";
                                    //}
                                    #region Vot
                                    //string customerid = req.Descendants("CustomerID").Single().Value;
                                    VOTService rs = new VOTService();
                                    tid46 = new Thread(new ThreadStart(() => { vothotelavailabilityresp = rs.GetRoomAvail_votOUT(req); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Ebookingcenter
                            if (ebookingcenter > 0)
                            {
                                try
                                {
                                    #region Ebookingcenter
                                    //string customerid = req.Descendants("CustomerID").Single().Value;
                                    EBookingService rs = new EBookingService();
                                    tid47 = new Thread(new ThreadStart(() => { ebookingcenterhotelavailabilityresp = rs.GetRoomAvail_ebookingcenterOUT(req); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Within Earth
                            if (wte > 0)
                            {
                                try
                                {
                                    WTEService wteobj = new WTEService(req.Descendants("CustomerID").FirstOrDefault().Value);
                                    tid48 = new Thread(new ThreadStart(() => { wteRoomshotelavailabilityresp = wteobj.GetRoomAvail_wteOUT(req, wteMealPlans, 48); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region IOL
                            if (iol > 0)
                            {
                                try
                                {
                                    IOLService iolobj = new IOLService(req.Descendants("CustomerID").FirstOrDefault().Value);
                                    tid50 = new Thread(new ThreadStart(() => { iolRoomshotelavailabilityresp = iolobj.GetRoomAvail_iolOUT(req, iolMealPlans, 50); }));
                                }
                                catch { }
                            }
                            #endregion
                            #region Booking Express
                            if (bookingexpress > 0)
                            {
                                try
                                {
                                    #region Booking Express
                                    xmlGetroom rs = new xmlGetroom();
                                    tid501 = new Thread(new ThreadStart(() => { bookingexpresshotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 501); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region ETripCenter
                            if (etripcenter > 0)
                            {
                                try
                                {
                                    #region ETripCenter
                                    xmlGetroom rs = new xmlGetroom();
                                    tid502 = new Thread(new ThreadStart(() => { etripcenterhotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 502); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Holidays Arabia
                            if (holidaysarabia > 0)
                            {
                                try
                                {
                                    #region Holidays Arabia
                                    xmlGetroom rs = new xmlGetroom();
                                    tid503 = new Thread(new ThreadStart(() => { holidaysarabiahotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 503); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Novo Destino
                            if (novodestino > 0)
                            {
                                try
                                {
                                    #region Novo Destino
                                    xmlGetroom rs = new xmlGetroom();
                                    tid504 = new Thread(new ThreadStart(() => { novodestinohotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 504); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Plaza
                            if (plaza > 0)
                            {
                                try
                                {
                                    #region Plaza
                                    xmlGetroom rs = new xmlGetroom();
                                    tid533 = new Thread(new ThreadStart(() => { plazahotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 533); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region CBH
                            if (cbh > 0)
                            {
                                try
                                {
                                    #region CBH
                                    xmlGetroom rs = new xmlGetroom();
                                    tid534 = new Thread(new ThreadStart(() => { cbhhotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 534); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Adventures
                            if (adventures > 0)
                            {
                                try
                                {
                                    #region Adventures
                                    xmlGetroom rs = new xmlGetroom();
                                    tid627 = new Thread(new ThreadStart(() => { adventureshotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 627); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region GTSBeds
                            if (gtsbeds > 0)
                            {
                                try
                                {
                                    #region GTSBeds
                                    xmlGetroom rs = new xmlGetroom();
                                    tid628 = new Thread(new ThreadStart(() => { gtsbedshotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 628); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region H2GO
                            if (h2go > 0)
                            {
                                try
                                {
                                    #region H2GO
                                    xmlGetroom rs = new xmlGetroom();
                                    tid629 = new Thread(new ThreadStart(() => { h2gohotelavailabilityresp = rs.GetRoomAvail_bookingexpressOUT(req, 629); }));
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion
                            #region Thread Start
                            try
                            {
                                if (darina > 0)
                                {
                                    tid1.Start();
                                }
                                if (tourico > 0)
                                {
                                    tid2.Start();
                                }
                                if (extranet > 0)
                                {
                                    tid3.Start();
                                }
                                if (hotelbeds > 0)
                                {
                                    tid4.Start();
                                }
                                if (DOTW > 0)
                                {
                                    tid5.Start();
                                }
                                if (hotelspro > 0)
                                {
                                    tid6.Start();
                                }
                                if (travco > 0)
                                {
                                    tid7.Start();
                                }
                                if (JacTravel > 0)
                                {
                                    tid8.Start();
                                }
                                if (RTS > 0)
                                {
                                    tid9.Start();
                                }
                                if (Miki > 0)
                                {
                                    tid11.Start();
                                }
                                if (restel > 0)
                                {
                                    tid13.Start();
                                }
                                if (JuniperW2M > 0)
                                {
                                    tid16.Start();
                                }
                                if (EgyptExpress > 0)
                                {
                                    tid17.Start();
                                }
                                if (SalTour > 0)
                                {
                                    tid19.Start();
                                }
                                if (expedia > 0)
                                {
                                    tid20.Start();
                                }
                                if (tbo > 0)
                                {
                                    tid21.Start();
                                }
                                if (LOH > 0)
                                {
                                    tid23.Start();
                                }
                                if (goglobal > 0)
                                {
                                    tid27.Start();
                                }
                                if (cosmobeds > 0)
                                {
                                    tid29.Start();
                                }
                                if (stuba > 0)
                                {
                                    tid30.Start();
                                }
                                if (Gadou > 0)
                                {
                                    tid31.Start();
                                }
                                if (yalago > 0)
                                {
                                    tid32.Start();
                                }
                                if (LCI > 0)
                                {
                                    tid35.Start();
                                }
                                if (SunHotels > 0)
                                {
                                    tid36.Start();
                                }
                                if (totalstay > 0)
                                {
                                    tid37.Start();
                                }
                                if (SmyRooms > 0)
                                {
                                    tid39.Start();
                                }
                                if (AlphaTours > 0)
                                {
                                    tid41.Start();
                                }
                                if (welcomebeds > 0)
                                {
                                    tid44.Start();
                                }
                                if (Hoojoozat > 0)
                                {
                                    tid45.Start();
                                }
                                if (vot > 0)
                                {
                                    tid46.Start();
                                }
                                if (ebookingcenter > 0)
                                {
                                    tid47.Start();
                                }
                                if (wte > 0)
                                {
                                    tid48.Start();
                                }
                                if (iol > 0)
                                {
                                    tid50.Start();
                                }
                                if (bookingexpress > 0)
                                {
                                    tid501.Start();
                                }
                                if (etripcenter > 0)
                                {
                                    tid502.Start();
                                }
                                if (holidaysarabia > 0)
                                {
                                    tid503.Start();
                                }
                                if (novodestino > 0)
                                {
                                    tid504.Start();
                                }
                                if (plaza > 0)
                                {
                                    tid533.Start();
                                }
                                if (cbh > 0)
                                {
                                    tid534.Start();
                                }
                                if (adventures > 0)
                                {
                                    tid627.Start();
                                }
                                if (gtsbeds > 0)
                                {
                                    tid628.Start();
                                }
                                if (h2go > 0)
                                {
                                    tid629.Start();
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
                            if (darina > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid1.Join(newTime);
                                }
                                catch { }
                                //tid1.Join();
                            }
                            if (tourico > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid2.Join(newTime);
                                }
                                catch { }
                            }
                            if (extranet > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid3.Join(newTime);
                                }
                                catch { }
                            }
                            if (hotelbeds > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid4.Join(newTime);
                                }
                                catch { }
                            }
                            if (DOTW > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid5.Join(newTime);
                                }
                                catch { }
                            }
                            if (hotelspro > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid6.Join(newTime);
                                }
                                catch { }
                            }
                            if (travco > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid7.Join(newTime);
                                }
                                catch { }
                            }
                            if (JacTravel > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid8.Join(newTime);
                                }
                                catch { }
                            }
                            if (RTS > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid9.Join(newTime);
                                }
                                catch { }
                            }
                            if (Miki > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid11.Join(newTime);
                                }
                                catch { }
                            }
                            if (restel > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid13.Join(newTime);
                                }
                                catch { }
                            }
                            if (JuniperW2M > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid16.Join(newTime);
                                }
                                catch { }
                            }
                            if (EgyptExpress > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid17.Join(newTime);
                                }
                                catch { }
                            }
                            if (SalTour > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid19.Join(newTime);
                                }
                                catch { }
                            }
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
                            if (tbo > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid21.Join(newTime);
                                }
                                catch { }
                            }
                            if (LOH > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid23.Join(newTime);
                                }
                                catch { }
                            }
                            if (goglobal > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid27.Join(newTime);
                                }
                                catch { }
                            }
                            if (cosmobeds > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid29.Join(newTime);
                                }
                                catch { }
                            }
                            if (stuba > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid30.Join(newTime);
                                }
                                catch { }
                            }
                            if (Gadou > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid31.Join(newTime);
                                }
                                catch { }
                            }
                            if (yalago > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid32.Join(newTime);
                                }
                                catch { }
                            }
                            if (LCI > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid35.Join(newTime);
                                }
                                catch { }
                            }
                            if (SunHotels > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid36.Join(newTime);
                                }
                                catch { }
                            }
                            if (totalstay > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid37.Join(newTime);
                                }
                                catch { }
                            }
                            if (SmyRooms > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid39.Join(newTime);
                                }
                                catch { }
                            }
                            if (AlphaTours > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid41.Join(newTime);
                                }
                                catch { }
                            }
                            if (welcomebeds > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid44.Join(newTime);
                                }
                                catch { }
                            }
                            if (Hoojoozat > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid45.Join(newTime);
                                }
                                catch { }
                            }
                            if (vot > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid46.Join(newTime);
                                }
                                catch { }
                            }
                            if (ebookingcenter > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid47.Join(newTime);
                                }
                                catch { }
                            }
                            if (wte > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid48.Join(newTime);
                                }
                                catch { }
                            }
                            if (iol > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid50.Join(newTime);
                                }
                                catch { }
                            }
                            if (bookingexpress > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid501.Join(newTime);
                                }
                                catch { }
                            }
                            if (etripcenter > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid502.Join(newTime);
                                }
                                catch { }
                            }
                            if (holidaysarabia > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid503.Join(newTime);
                                }
                                catch { }
                            }
                            if (novodestino > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid504.Join(newTime);
                                }
                                catch { }
                            }
                            if (plaza > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid533.Join(newTime);
                                }
                                catch { }
                            }
                            if (cbh > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid534.Join(newTime);
                                }
                                catch { }
                            }
                            if (adventures > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid627.Join(newTime);
                                }
                                catch { }
                            }
                            if (gtsbeds > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid628.Join(newTime);
                                }
                                catch { }
                            }
                            if (h2go > 0)
                            {
                                try
                                {
                                    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                                    if (newTime < 0)
                                    {
                                        newTime = 0;
                                    }
                                    tid629.Join(newTime);
                                }
                                catch { }
                            }
                            #endregion
                            #region Thread Abort
                            if (tid1 != null && tid1.IsAlive)
                                tid1.Abort();
                            if (tid2 != null && tid2.IsAlive)
                                tid2.Abort();
                            if (tid3 != null && tid3.IsAlive)
                                tid3.Abort();
                            if (tid4 != null && tid4.IsAlive)
                                tid4.Abort();
                            if (tid5 != null && tid5.IsAlive)
                                tid5.Abort();
                            if (tid6 != null && tid6.IsAlive)
                                tid6.Abort();
                            if (tid7 != null && tid7.IsAlive)
                                tid7.Abort();
                            if (tid8 != null && tid8.IsAlive)
                                tid8.Abort();
                            if (tid9 != null && tid9.IsAlive)
                                tid9.Abort();
                            if (tid11 != null && tid11.IsAlive)
                                tid11.Abort();
                            if (tid13 != null && tid13.IsAlive)
                                tid13.Abort();
                            if (tid16 != null && tid16.IsAlive)
                                tid16.Abort();
                            if (tid17 != null && tid17.IsAlive)
                                tid17.Abort();
                            if (tid19 != null && tid19.IsAlive)
                                tid19.Abort();
                            if (tid20 != null && tid20.IsAlive)
                                tid20.Abort();
                            if (tid21 != null && tid21.IsAlive)
                                tid21.Abort();
                            if (tid23 != null && tid23.IsAlive)
                                tid23.Abort();
                            if (tid27 != null && tid27.IsAlive)
                                tid27.Abort();
                            if (tid29 != null && tid29.IsAlive)
                                tid29.Abort();
                            if (tid30 != null && tid30.IsAlive)
                                tid30.Abort();
                            if (tid31 != null && tid31.IsAlive)
                                tid31.Abort();
                            if (tid32 != null && tid32.IsAlive)
                                tid32.Abort();
                            if (tid35 != null && tid35.IsAlive)
                                tid35.Abort();
                            if (tid36 != null && tid36.IsAlive)
                                tid36.Abort();
                            if (tid37 != null && tid37.IsAlive)
                                tid37.Abort();
                            if (tid39 != null && tid39.IsAlive)
                                tid39.Abort();
                            if (tid41 != null && tid41.IsAlive)
                                tid41.Abort();
                            if (tid44 != null && tid44.IsAlive)
                                tid44.Abort();
                            if (tid45 != null && tid45.IsAlive)
                                tid45.Abort();
                            if (tid46 != null && tid46.IsAlive)
                                tid46.Abort();
                            if (tid47 != null && tid47.IsAlive)
                                tid47.Abort();
                            if (tid48 != null && tid48.IsAlive)
                                tid48.Abort();
                            if (tid50 != null && tid50.IsAlive)
                                tid50.Abort();
                            if (tid501 != null && tid501.IsAlive)
                                tid501.Abort();
                            if (tid502 != null && tid502.IsAlive)
                                tid502.Abort();
                            if (tid503 != null && tid503.IsAlive)
                                tid503.Abort();
                            if (tid504 != null && tid504.IsAlive)
                                tid504.Abort();
                            if (tid627 != null && tid627.IsAlive)
                                tid627.Abort();
                            if (tid628 != null && tid628.IsAlive)
                                tid628.Abort();
                            if (tid629 != null && tid629.IsAlive)
                                tid629.Abort();
                            #endregion
                            #region Merge
                            try
                            {
                                if (darinahotelavailabilityresp != null)
                                {
                                    if (darinahotelavailabilityresp.Count > 0)
                                    {
                                        XElement response = new XElement("Hotels", darinahotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                }
                                if (touricohotelavailabilityresp != null)
                                {
                                    if (touricohotelavailabilityresp.Count > 0)
                                    {
                                        XElement response = new XElement("Hotels", touricohotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                }
                                if (extranethotelavailabilityresp != null)
                                {
                                    if (extranethotelavailabilityresp.Count > 0)
                                    {
                                        XElement response = new XElement("Hotels", extranethotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                }
                                if (hotelbedshotelavailabilityresp != null)
                                {
                                    if (hotelbedshotelavailabilityresp.Count > 0)
                                    {
                                        XElement response = new XElement("Hotels", hotelbedshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                }
                                //if (DOTWhotelavailabilityresp != null)
                                if (DOTWhotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", DOTWhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (hotelsprohotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        if (hotelsprohotelavailabilityresp.Count > 0)
                                        {
                                            XElement response = new XElement("Hotels", hotelsprohotelavailabilityresp.Descendants("RoomTypes").ToList());
                                            hotelavailabilityresp.Add(response);
                                        }
                                    }
                                    catch { }
                                }
                                if (travcohotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", travcohotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (JacTravelhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", JacTravelhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (RTShotelavailabilityresp != null)
                                {
                                    if (RTShotelavailabilityresp.Count > 0)
                                    {
                                        XElement response = new XElement("Hotels", RTShotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                }
                                if (Mikihotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", Mikihotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (restelhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", restelhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (JuniperW2Mhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", JuniperW2Mhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (EgyptExpresshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", EgyptExpresshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (SalTourhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", SalTourhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (expediahtlavailresp != null)
                                {
                                    XElement response = new XElement("Hotels", expediahtlavailresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (tbohotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", tbohotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (JuniperLOHhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", JuniperLOHhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (goglobalhotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        if (goglobalhotelavailabilityresp.Count > 0)
                                        {
                                            XElement response = new XElement("Hotels", goglobalhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                            hotelavailabilityresp.Add(response);
                                        }
                                    }
                                    catch { }
                                }
                                if (cosmobedsRoomshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", cosmobedsRoomshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (stubaRoomshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", stubaRoomshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (Gadouhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", Gadouhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (yalagoRoomshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", yalagoRoomshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (JuniperLCIhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", JuniperLCIhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (SunHotelshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", SunHotelshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (totalstayhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", totalstayhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (SmyRoomshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", SmyRoomshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (AlphaTourshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", AlphaTourshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (welcomebedshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", welcomebedshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (Hoojoozathotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", Hoojoozathotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (vothotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", vothotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (ebookingcenterhotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", ebookingcenterhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (wteRoomshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", wteRoomshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (iolRoomshotelavailabilityresp != null)
                                {
                                    XElement response = new XElement("Hotels", iolRoomshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                    hotelavailabilityresp.Add(response);
                                }
                                if (bookingexpresshotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", bookingexpresshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (etripcenterhotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", etripcenterhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (holidaysarabiahotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", holidaysarabiahotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (novodestinohotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", novodestinohotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (plazahotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", plazahotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (cbhhotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", cbhhotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (adventureshotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", adventureshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (gtsbedshotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", gtsbedshotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
                                }
                                if (h2gohotelavailabilityresp != null)
                                {
                                    try
                                    {
                                        XElement response = new XElement("Hotels", h2gohotelavailabilityresp.Descendants("RoomTypes").ToList());
                                        hotelavailabilityresp.Add(response);
                                    }
                                    catch { }
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
        #endregion
        #region Test
        public HttpWebRequest CreateWebRequestTourico()
        {

            string _url = "http://demo-hotelws.touricoholidays.com/HotelFlow.svc/bas";
            string _action = "http://tourico.com/webservices/hotelv3/IHotelFlow/SearchHotels";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
            webRequest.Headers.Add(@"SOAPAction:" + _action + "");
            webRequest.ContentType = "text/xml;charset=utf-8";
            webRequest.Method = "POST";
            webRequest.Host = "demo-hotelws.touricoholidays.com";
            return webRequest;
        }

        XElement GetHotelInfoTourico(XElement requesttourico)
        {
            XElement loc;
            HttpWebRequest request = CreateWebRequestTourico();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:aut='http://schemas.tourico.com/webservices/authentication' xmlns:hot='http://tourico.com/webservices/hotelv3'>
                                        <soapenv:Header>
                                              <aut:AuthenticationHeader>
                                                 <aut:LoginName>HOL916</aut:LoginName>
                                                 <aut:Password>111111</aut:Password>
                                                 <aut:Culture>en_US</aut:Culture> 
                                                 <aut:Version></aut:Version>
                                              </aut:AuthenticationHeader>
                                           </soapenv:Header>
                                          <soapenv:Body>
                                              <hot:SearchHotels>
                                                 <hot:request>
                                                    <hot1:Destination>NYC</hot1:Destination>
                                                    <hot1:HotelCityName>New York</hot1:HotelCityName>
                                                    <hot1:HotelLocationName></hot1:HotelLocationName>
                                                    <hot1:HotelName></hot1:HotelName>
                                                    <hot1:CheckIn>2017-06-01</hot1:CheckIn>
                                                    <hot1:CheckOut>2017-06-05</hot1:CheckOut>
                                                    <hot1:RoomsInformation>
                                                     <hot1:RoomInfo>
                                                          <hot1:AdultNum>1</hot1:AdultNum>
                                                          <hot1:ChildNum>0</hot1:ChildNum>
                                                          <hot1:ChildAges>
                                                             <hot1:ChildAge age='0'/>
                                                          </hot1:ChildAges>
                                                       </hot1:RoomInfo>
                                                    </hot1:RoomsInformation>
                                                    <hot1:MaxPrice>0</hot1:MaxPrice>
                                                    <hot1:StarLevel>0</hot1:StarLevel>
                                                    <hot1:AvailableOnly>true</hot1:AvailableOnly>
                                                    <hot1:PropertyType>NotSet</hot1:PropertyType>
                                                    <hot1:ExactDestination>true</hot1:ExactDestination>
                                                 </hot:request>
                                                 <hot:features>
                                                    <hot:Feature name="" value=""/>
                                                 </hot:features>
                                              </hot:SearchHotels>
                                             </soapenv:Body></soapenv:Envelope>"
                );

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    XDocument xmlData = XDocument.Parse(soapResult);
                    //xmlData.RemoveXmlns();
                    loc = xmlData.Descendants("HotelList").FirstOrDefault();

                }
            }

            return loc;
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