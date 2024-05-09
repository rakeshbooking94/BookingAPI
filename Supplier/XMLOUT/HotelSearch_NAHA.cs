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
using TravillioXMLOutService.Common.DotW;
using TravillioXMLOutService.Supplier.JacTravel;
using TravillioXMLOutService.Supplier.Extranet;
using TravillioXMLOutService.Common.JacTravel;
using TravillioXMLOutService.Supplier.TouricoHolidays;
using TravillioXMLOutService.Supplier.GIATA;
using TravillioXMLOutService.Models.Darina;
using TravillioXMLOutService.Supplier.Darina;
using TravillioXMLOutService.Models.HotelsPro;
using TravillioXMLOutService.Models.Tourico;
using TravillioXMLOutService.Models.Common;
using TravillioXMLOutService.Models.Travco;
using TravillioXMLOutService.Supplier.RTS;
using TravillioXMLOutService.Models.RTS;
using TravillioXMLOutService.Models.Juniper;
using TravillioXMLOutService.Supplier.Juniper;
using TravillioXMLOutService.Models.Restel;
using TravillioXMLOutService.Supplier.Restel;
using TravillioXMLOutService.Models.JacTravel;
using TravillioXMLOutService.Supplier.Miki;
using TravillioXMLOutService.Models.Godou;
using TravillioXMLOutService.Supplier.Godou;
using TravillioXMLOutService.Supplier.SalTours;
using TravillioXMLOutService.Supplier.SunHotels;
using TravillioXMLOutService.Supplier.Hoojoozat;
using TravillioXMLOutService.Supplier.TravelGate;
using TravillioXMLOutService.Supplier.TBOHolidays;
using TravillioXMLOutService.Supplier.VOT;
using TravillioXMLOutService.Models.EBookingCenter;
using TravillioXMLOutService.Supplier.EBookingCenter;
using TravillioXMLOutService.Supplier.XMLOUTAPI;
using TravillioXMLOutService.Supplier.GoGlobals;
using TravillioXMLOutService.Supplier.Welcomebeds;
using TravillioXMLOutService.Supplier.Expedia;
using TravillioXMLOutService.Supplier.Withinearth;
using TravillioXMLOutService.Supplier.IOL;

namespace TravillioXMLOutService.Supplier.XMLOUT
{
    public class HotelSearch_NAHA : IDisposable
    {
        int sup_cutime = 100000;
        DotwService dotwObj;
        //GodouServices gds;
        dr_Hotelsearch darinareq;
        Tr_HotelSearch touricoreq;
        ExtHotelSearch extreq;
        HotelBeds hbedreq;
        HotelsProHotelSearch htlproreq;
        MikiInternal miki;
        SalServices sal;
        TGServices TGS;
        string touricouserlogin = string.Empty;
        string touricopassword = string.Empty;
        string touricoversion = string.Empty;
        XElement statictouricohotellist;
        string availDarina = string.Empty;
        string hoteldetailDarina = string.Empty;
        XElement reqTravillio;
        List<Tourico.Hotel> hotelavailabilityresult;
        List<XElement> hotelavailabilitylistextranet;
        List<XElement> jactravelslist = null;
        List<XElement> totalstaylist = null;
        XElement HProfac = null;
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
        public void WriteToFileResponseTimeHotel(string text)
        {
            try
            {
                string path = Convert.ToString(HttpContext.Current.Server.MapPath(@"~\log.txt"));
                using (StreamWriter writer = new StreamWriter(path, true))
                {


                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    //writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    //writer.WriteLine("---------------------------Time Calculate-----------------------------------------");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region Hotel Availability (XML OUT for Travayoo)
        public XElement HotelAvail_NAHA(XElement req, string dmc)
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
                    int darina = req.Descendants("SupplierID").Where(x => x.Value == "1" && x.Attribute("xmlout").Value == "false").Count();
                    int tourico = req.Descendants("SupplierID").Where(x => x.Value == "2" && x.Attribute("xmlout").Value == "false").Count();
                    int extranet = req.Descendants("SupplierID").Where(x => x.Value == "3" && x.Attribute("xmlout").Value == "false").Count();
                    int hotelbeds = req.Descendants("SupplierID").Where(x => x.Value == "4" && x.Attribute("xmlout").Value == "false").Count();
                    int DOTW = req.Descendants("SupplierID").Where(x => x.Value == "5" && x.Attribute("xmlout").Value == "false").Count();
                    int hotelspro = req.Descendants("SupplierID").Where(x => x.Value == "6" && x.Attribute("xmlout").Value == "false").Count();
                    int travco = req.Descendants("SupplierID").Where(x => x.Value == "7" && x.Attribute("xmlout").Value == "false").Count();
                    int JacTravel = req.Descendants("SupplierID").Where(x => x.Value == "8" && x.Attribute("xmlout").Value == "false").Count();
                    int RTS = req.Descendants("SupplierID").Where(x => x.Value == "9" && x.Attribute("xmlout").Value == "false").Count();
                    int Miki = req.Descendants("SupplierID").Where(x => x.Value == "11" && x.Attribute("xmlout").Value == "false").Count();
                    int restel = req.Descendants("SupplierID").Where(x => x.Value == "13" && x.Attribute("xmlout").Value == "false").Count();
                    int JuniperW2M = req.Descendants("SupplierID").Where(x => x.Value == "16" && x.Attribute("xmlout").Value == "false").Count();
                    int EgyptExpress = req.Descendants("SupplierID").Where(x => x.Value == "17" && x.Attribute("xmlout").Value == "false").Count();
                    int SalTour = req.Descendants("SupplierID").Where(x => x.Value == "19" && x.Attribute("xmlout").Value == "false").Count();
                    int expedia = req.Descendants("SupplierID").Where(x => x.Value == "20" && x.Attribute("xmlout").Value == "false").Count();
                    int tbo = req.Descendants("SupplierID").Where(x => x.Value == "21" && x.Attribute("xmlout").Value == "false").Count();
                    int LOH = req.Descendants("SupplierID").Where(x => x.Value == "23" && x.Attribute("xmlout").Value == "false").Count();
                    int goglobal = req.Descendants("SupplierID").Where(x => x.Value == "27" && x.Attribute("xmlout").Value == "false").Count();
                    int cosmobeds = req.Descendants("SupplierID").Where(x => x.Value == "29" && x.Attribute("xmlout").Value == "false").Count();
                    int stuba = req.Descendants("SupplierID").Where(x => x.Value == "30" && x.Attribute("xmlout").Value == "false").Count();
                    int Gadou = req.Descendants("SupplierID").Where(x => x.Value == "31" && x.Attribute("xmlout").Value == "false").Count();
                    int yalago = req.Descendants("SupplierID").Where(x => x.Value == "32" && x.Attribute("xmlout").Value == "false").Count();
                    int LCI = req.Descendants("SupplierID").Where(x => x.Value == "35" && x.Attribute("xmlout").Value == "false").Count();
                    int SunHotels = req.Descendants("SupplierID").Where(x => x.Value == "36" && x.Attribute("xmlout").Value == "false").Count();
                    int totalstay = req.Descendants("SupplierID").Where(x => x.Value == "37" && x.Attribute("xmlout").Value == "false").Count();
                    int SmyRooms = req.Descendants("SupplierID").Where(x => x.Value == "39" && x.Attribute("xmlout").Value == "false").Count();
                    int AlphaTours = req.Descendants("SupplierID").Where(x => x.Value == "41" && x.Attribute("xmlout").Value == "false").Count();
                    int welcomebeds = req.Descendants("SupplierID").Where(x => x.Value == "44" && x.Attribute("xmlout").Value == "false").Count();
                    int Hoojoozat = req.Descendants("SupplierID").Where(x => x.Value == "45" && x.Attribute("xmlout").Value == "false").Count();
                    int vot = req.Descendants("SupplierID").Where(x => x.Value == "46" && x.Attribute("xmlout").Value == "false").Count();
                    int ebookingcenter = req.Descendants("SupplierID").Where(x => x.Value == "47" && x.Attribute("xmlout").Value == "false").Count();
                    int wte = req.Descendants("SupplierID").Where(x => x.Value == "48" && x.Attribute("xmlout").Value == "false").Count();
                    int iol = req.Descendants("SupplierID").Where(x => x.Value == "50" && x.Attribute("xmlout").Value == "false").Count();
                    //int bookingexpress = req.Descendants("SupplierID").Where(x => x.Value == "501" && x.Attribute("xmlout").Value == "false").Count();
                    //int etripcenter = req.Descendants("SupplierID").Where(x => x.Value == "502" && x.Attribute("xmlout").Value == "false").Count();
                    //int holidaysarabia = req.Descendants("SupplierID").Where(x => x.Value == "503" && x.Attribute("xmlout").Value == "false").Count();
                    //int novodestino = req.Descendants("SupplierID").Where(x => x.Value == "504" && x.Attribute("xmlout").Value == "false").Count();
                    //int plaza = req.Descendants("SupplierID").Where(x => x.Value == "533" && x.Attribute("xmlout").Value == "false").Count();
                    //int cbh = req.Descendants("SupplierID").Where(x => x.Value == "534" && x.Attribute("xmlout").Value == "false").Count();
                    //int adventures = req.Descendants("SupplierID").Where(x => x.Value == "627" && x.Attribute("xmlout").Value == "false").Count();
                    //int gtsbeds = req.Descendants("SupplierID").Where(x => x.Value == "628" && x.Attribute("xmlout").Value == "false").Count();
                    //int h2go = req.Descendants("SupplierID").Where(x => x.Value == "629" && x.Attribute("xmlout").Value == "false").Count();
                    #region darina & extranet
                    try
                    {
                        string cntid = req.Descendants("CountryID").FirstOrDefault().Value;
                        if (cntid != "1")
                        {
                            darina = 0;
                        }
                        if (cntid != "56" && cntid != "67" && cntid != "1" && cntid != "21")
                        {
                            extranet = 0;
                        }
                    }
                    catch { }
                    #endregion
                    if (darina > 0 || tourico > 0 || extranet > 0 || hotelbeds > 0 || DOTW > 0 || hotelspro > 0 || travco > 0 || JacTravel > 0 || RTS > 0 || Miki > 0 || restel > 0 || JuniperW2M > 0 || EgyptExpress > 0 || SalTour > 0 || expedia > 0 || tbo > 0 || LOH > 0 || goglobal > 0 || cosmobeds > 0 || stuba > 0 || Gadou > 0 || yalago > 0 || LCI > 0 || SunHotels > 0 || totalstay > 0 || SmyRooms > 0 || AlphaTours > 0 || welcomebeds > 0 || Hoojoozat > 0 || vot > 0 || ebookingcenter > 0 || wte > 0 || iol > 0)
                    {                        
                        #region get cut off time
                        try
                        {
                            sup_cutime = supplier_Cred.cutoff_time(req.Descendants("HotelID").FirstOrDefault().Value);
                        }
                        catch { }
                        #endregion
                        #region Timer
                        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                        timer.Start();
                        #endregion
                        IEnumerable<XElement> darinahotellist = null;
                        List<XElement> responsetourico = null;
                        List<XElement> responsehotelspro = null;
                        List<XElement> hotelbedslist = null;
                        List<XElement> dotwlist = null;
                        List<XElement> travcolist = null;
                        List<XElement> RTSlst = null;
                        List<XElement> Mikilst = null;
                        List<XElement> Restellst = null;
                        List<XElement> JuniparW2Mlst = null;
                        List<XElement> egyptexpresslst = null;
                        List<XElement> Sallst = null;
                        List<XElement> Expedialst = null;
                        List<XElement> TBOlst = null;
                        List<XElement> JuniparLOHlst = null;
                        List<XElement> gogloballst = null;
                        List<XElement> cosmobedslst = null;
                        List<XElement> stubalst = null;
                        List<XElement> yalagolst = null;
                        List<XElement> JuniparLCIlst = null;
                        List<XElement> alphatourslst = null;
                        List<XElement> welcomebedslst = null;
                        List<XElement> Smylst = null;
                        List<XElement> sunhotelslst = null;
                        List<XElement> hoojhotelslst = null;
                        List<XElement> vothotelslst = null;
                        List<XElement> ebookingcenterhotelslst = null;
                        List<XElement> wtelst = null;
                        List<XElement> iollst = null;
                        //List<XElement> bookingexpresslst = null;
                        //List<XElement> etripcenterlst = null;
                        //List<XElement> holidaysarabialst = null;
                        //List<XElement> novodestinolst = null;
                        //List<XElement> plazalst = null;
                        //List<XElement> cbhlst = null;
                        //List<XElement> adventureslst = null;
                        //List<XElement> gtsbedslst = null;
                        //List<XElement> h2golst = null;
                        #region Bind Static Data
                        XElement doccurrency = null;
                        XElement docmealplan = null;
                        XElement dococcupancy = null;
                        XElement staticdatahb = null;
                        XElement travco_htlstatic = null;
                        XElement travco_htlstar = null;
                        XElement travcocitymapping = null;
                        string rtspath = string.Empty;                       
                        List<XElement> Gadoulst = null;
                        XElement ebookingcenterdocnationality = null;
                        if (darina == 1)
                        {
                            doccurrency = dr_staticdata.drn_doccurrency();
                            docmealplan = dr_staticdata.drn_docmealplan();
                            dococcupancy = dr_staticdata.drn_dococcupancy();
                            darinareq = new dr_Hotelsearch();
                        }
                        if (tourico == 1)
                        {
                            statictouricohotellist = trc_statichtl.tourico_htlstatic();
                            touricoreq = new Tr_HotelSearch();
                        }
                        if (extranet == 1)
                        {
                            extreq = new ExtHotelSearch();
                        }
                        if (hotelbeds == 1)
                        {
                            hbedreq = new HotelBeds();
                        }
                        if (DOTW == 1)// && Convert.ToInt32(req.Descendants("RoomPax").Count()) == 1)
                        {
                            string custID = string.Empty;                            
                            try
                            {
                                //cusid=10028 (multiroom entry)
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "5" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;                                
                            }
                            catch { }
                            if (custID == "10028" || custID == "40077")// Change By Rahul MultiRooms work only on customerID: 10028vnbn
                            {
                                dotwObj = new DotwService(custID);
                            }
                            else if (Convert.ToInt32(req.Descendants("RoomPax").Count()) == 1)
                            {

                                dotwObj = new DotwService(custID);
                            }
                            
                        }
                        if (hotelspro == 1)
                        {
                            //HProfac = htlpro_facility.hotelspro_facility();
                            htlproreq = new HotelsProHotelSearch();
                        }
                        if (travco == 1)
                        {
                            //travcocitymapping = travco_static.travco_citymap();
                            //travco_htlstatic = travco_static.travco_statichtl();
                            travco_htlstar = travco_static.travco_starcat();
                        }
                        if (JacTravel == 1 || totalstay > 0)
                        {
                            //jaccitymapping = jac_staticdata.jac_citymapping();
                        }
                        if (RTS == 1)
                        {
                            //rtspath = RTS_citymap.rts_citylist();
                        }
                        if (Miki == 1)
                        {
                            miki = new MikiInternal();
                        }
                        if (restel == 1)
                        {
                            //restelcitymapping = restel_citymapping.restel_city();
                        }
                        if (Gadou == 1)
                        {
                            //gds = new GodouServices();
                            //gadouCurrencies = Gadou_Currency.Gadaou_Currencies();
                        }
                        if(ebookingcenter == 1)
                        {
                            ebookingcenterdocnationality = EBookingStatic.ebook_nationality();
                        }
                        #endregion
                        #region Thread Initialize
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
                        //Thread tid501 = null;
                        //Thread tid502 = null;
                        //Thread tid503 = null;
                        //Thread tid504 = null;
                        //Thread tid533 = null;
                        //Thread tid534 = null;
                        //Thread tid627 = null;
                        //Thread tid628 = null;
                        //Thread tid629 = null;
                        if (darina == 1)
                        {
                            if (Convert.ToInt32(req.Descendants("RoomPax").Count()) == 1)
                            {
                                string custID = string.Empty;
                                string custName = string.Empty;
                                try
                                {
                                    custID = req.Descendants("SupplierID").Where(x => x.Value == "1" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                    custName = "Darina";
                                    tid1 = new Thread(new ThreadStart(() => { darinahotellist = darinareq.darinaavailcombined(req, doccurrency, docmealplan, dococcupancy, custID, custName); }));
                                }
                                catch { custName = "Darina";
                                darina = 0;
                                }
                               
                            }
                        }
                        if (tourico == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            tid2 = new Thread(new ThreadStart(() => { responsetourico = touricoreq.HotelSearch_Tourico(reqTravillio, statictouricohotellist); }));
                        }
                        if (extranet == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 7)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "3" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                custName = "Extranet";
                                tid3 = new Thread(new ThreadStart(() => { hotelavailabilitylistextranet = extreq.CheckHtlAvailabilityExtranet(reqTravillio, custID, custName); }));
                            }
                            catch { custName = "Extranet";
                            extranet = 0;
                            }
                           
                        }
                        if (hotelbeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "4" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                custName = "HotelBeds";
                                tid4 = new Thread(new ThreadStart(() => { hotelbedslist = hbedreq.CheckAvailabilityHotelBedsMergeThreadFinal(reqTravillio, staticdatahb, custID, custName); }));
                            }
                            catch { custName = "HotelBeds";
                            hotelbeds = 0;
                            }
                          
                        }
                        if (DOTW == 1)// && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 2)
                        {
                            // Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "5" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                custName = "DOTW";
                                if (custID == "10028" || custID == "40077") // Change By Rahul MultiRooms work only on customerID: 10028
                                {
                                    tid5 = new Thread(new ThreadStart(() => { dotwlist = dotwObj.HtlSearchReq(reqTravillio, custID, "DOTW"); }));
                                }
                                else if (Convert.ToInt32(req.Descendants("RoomPax").Count()) == 1)
                                {
                                    tid5 = new Thread(new ThreadStart(() => { dotwlist = dotwObj.HtlSearchReq(reqTravillio, custID, "DOTW"); }));
                                }
                            }
                            catch { custName = "DOTW";
                            DOTW = 0;
                            }
                            
                           // tid5 = new Thread(new ThreadStart(() => { dotwlist = dotwObj.HtlSearchReq(reqTravillio, custID, "DOTW"); }));
                        }
                        if (hotelspro == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6)
                        {
                             bool roomPax = true;
                             int totalPax = 0;
                            foreach (XElement room in reqTravillio.Descendants("RoomPax"))
                            {
                                //roomPax = Convert.ToInt32(room.Element("Adult").Value) < 6 ? Convert.ToInt32(room.Element("Child").Value) <= 2 ? true : false : false;
                                totalPax = totalPax + Convert.ToInt32(room.Element("Adult").Value);
                                totalPax = totalPax + Convert.ToInt32(room.Element("Child").Value);
                                if(totalPax > 6)
                                {
                                    roomPax = false;
                                }
                            }
                            if (roomPax)
                            {
                                string custID = string.Empty;
                                string custName = string.Empty;
                                try
                                {
                                    custID = req.Descendants("SupplierID").Where(x => x.Value == "6" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                    custName = "HPro";
                                    tid6 = new Thread(new ThreadStart(() => { responsehotelspro = htlproreq.CheckAvailabilityHotelsProThread(reqTravillio, HProfac, custID, custName); }));
                                    tid6.Priority = System.Threading.ThreadPriority.Highest;
                                }
                                catch { custName = "HPro";
                                 hotelspro = 0;
                                }
                                //tid6.IsBackground = true;
                            }
                            else
                            {
                                hotelspro = 0;
                            }
                        }
                        if (travco == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "7" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                Travco travobj = new Travco(custID);
                                tid7 = new Thread(new ThreadStart(() => { travcolist = travobj.getHotelAvailbalityMerge(reqTravillio, travco_htlstatic, travco_htlstar, travcocitymapping, "Travco"); }));
                            }
                            catch {
                                custName = "travco";
                                travco = 0;
                            }
                            
                        }
                        if (JacTravel == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
                        {
                            try
                            {
                                int totalPax = 0;
                                foreach (XElement item in reqTravillio.Descendants("RoomPax"))
                                {
                                    totalPax = totalPax + Convert.ToInt32(item.Element("Adult").Value);
                                    totalPax = totalPax + Convert.ToInt32(item.Element("Child").Value);
                                }
                                if (totalPax < 10)
                                {
                                    string custID = string.Empty;
                                    try
                                    {
                                        custID = req.Descendants("SupplierID").Where(x => x.Value == "8" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                    }
                                    catch { }
                                    JacTravel_Intial obj = new JacTravel_Intial();
                                    obj.MyEvent += obj_MyEvent;
                                    tid8 = new Thread(new ThreadStart(() => { obj.CallHtlSearch(reqTravillio,custID, "JacTravels", 8); }));
                                }
                                else { JacTravel = 0; }
                            }
                            catch { JacTravel = 0; }
                        }
                        if (totalstay == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
                        {
                            try
                            {
                                int totalPax = 0;
                                foreach (XElement item in reqTravillio.Descendants("RoomPax"))
                                {
                                    totalPax = totalPax + Convert.ToInt32(item.Element("Adult").Value);
                                    totalPax = totalPax + Convert.ToInt32(item.Element("Child").Value);
                                }
                                if (totalPax < 10)
                                {
                                    string custID = string.Empty;
                                    try
                                    {
                                        custID = req.Descendants("SupplierID").Where(x => x.Value == "37" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                    }
                                    catch { }
                                    JacTravel_Intial obj1 = new JacTravel_Intial();
                                    obj1.MyEvent += obj_MyEvent1;
                                    tid37 = new Thread(new ThreadStart(() => { obj1.CallHtlSearch(reqTravillio,custID, "TotalStay", 37); }));
                                }
                                else { totalstay = 0; }
                            }
                            catch { totalstay = 0; }
                        }
                        if (RTS == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "9" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                custName = "RTS";
                                RTSIntial obj = new RTSIntial();
                                tid9 = new Thread(new ThreadStart(() => { RTSlst = obj.CallSearch(reqTravillio, rtspath, custID, custName); }));
    
                            }
                            catch { custName = "RTS";
                            RTS = 0;
                            }
                                               }
                        if (Miki == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "11" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                custName = "Miki";
                                tid11 = new Thread(new ThreadStart(() => { Mikilst = miki.HotelSearchRequest(reqTravillio, custID, custName); }));
                            }
                            catch { custName = "Miki";
                            Miki = 0;


                            }
                                                   }
                        if (restel == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5 && Convert.ToInt32(req.Descendants("Nights").First().Value) <= 15)
                        {
                            try
                            {
                                RestelServices rs = new RestelServices();
                                #region Condition Check
                                bool condition1 = true;
                                bool condition2 = true;
                                int count = 0;
                                int roomcount = req.Descendants("RoomPax").Count();
                                List<string> PaxAllowed = new List<string>(new string[] { "1-0", "1-1", "1-2", "2-0", "2-1", "2-2", "3-0", "3-1", "4-0", "5-0", "6-0", "7-0", "8-0" });
                                List<string> occupancy = new List<string>();
                                foreach (XElement pax in req.Descendants("RoomPax"))
                                {
                                    string occ = pax.Element("Adult").Value + "-" + pax.Element("Child").Value;
                                    occupancy.Add(occ);
                                    if (PaxAllowed.Contains(occ))
                                        count++;
                                }
                                if (roomcount == count)
                                    condition2 = true;
                                else
                                    condition2 = false;
                                if (req.Descendants("RoomPax").Count() == 4)
                                {
                                    if (occupancy.Distinct().Count() <= 3)
                                        condition1 = true;
                                    else
                                        condition1 = false;
                                }
                                #endregion
                                if (condition1 && condition2)
                                {
                                    string custID = string.Empty;
                                    string custName = string.Empty;
                                    try
                                    {
                                        custID = req.Descendants("SupplierID").Where(x => x.Value == "13" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                        custName = "Restel";

                                        tid13 = new Thread(new ThreadStart(() => { Restellst = rs.ThreadedHotelSearch(reqTravillio, "Restel", 13, custID, custName); }));
                                        tid13.Priority = System.Threading.ThreadPriority.AboveNormal;
                                    }
                                    catch { custName = "Restel";
                                    restel = 0;
                                    }
                                }
                                else
                                    restel = 0;
                            }
                            catch { restel = 0; }
                        }
                        if (JuniperW2M == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "16" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                int customerid = Convert.ToInt32(custID);
                                JuniperResponses objRs = new JuniperResponses(16, customerid);
                                tid16 = new Thread(new ThreadStart(() => { JuniparW2Mlst = objRs.ThreadedHotelSearch(reqTravillio, "W2M"); }));
                                tid16.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch {
                                custName = "JuniperW2M";
                                JuniperW2M = 0;
                            }
                         
                        }
                        if (EgyptExpress == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "17" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                int customerid = Convert.ToInt32(custID);
                                JuniperResponses objRs = new JuniperResponses(17, customerid);
                                tid17 = new Thread(new ThreadStart(() => { egyptexpresslst = objRs.ThreadedHotelSearch(reqTravillio, "EgyptExpress"); }));
                            }
                            catch {
                                custName="EET";
                                  EgyptExpress = 0; 
                            }
                        }
                        if (SalTour == 1 && req.Descendants("RoomPax").Count() < 10)
                        {
                            sal = new SalServices();
                            tid19 = new Thread(new ThreadStart(() => { Sallst = sal.HotelAvailability(reqTravillio, "SALTOURS"); }));
                        }
                        if (expedia == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 9)
                        {
                            string custID = string.Empty;
                            string custName = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "20" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                int customerid = Convert.ToInt32(custID);
                                ExpediaService objRs = new ExpediaService(custID);
                                tid20 = new Thread(new ThreadStart(() => { Expedialst = objRs.HotelAvailability(reqTravillio, custID, "Expedia"); }));
                                tid20.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch {
                                expedia = 0;
                                custName = "Expedia";
                            }
                        }
                        if (tbo == 1 && req.Descendants("RoomPax").Count() < 10)
                        {
                            bool roomPax = true;
                            foreach (XElement room in reqTravillio.Descendants("RoomPax"))
                            {
                                roomPax = Convert.ToInt32(room.Element("Adult").Value) <= 6 ? Convert.ToInt32(room.Element("Child").Value) <= 2 ? true : false : false;
                            }
                            if (roomPax)
                            {
                                string custID = string.Empty;
                                try
                                {
                                    custID = req.Descendants("SupplierID").Where(x => x.Value == "21" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                    int customerid = Convert.ToInt32(custID);
                                    TBOServices tbs = new TBOServices();
                                    tid21 = new Thread(new ThreadStart(() => { TBOlst = tbs.HotelSearch(reqTravillio, custID, "TBO"); }));
                                    tid21.Priority = System.Threading.ThreadPriority.AboveNormal;
                                }
                                catch { tbo = 0; }
                            }
                            else
                                tbo = 0;
                        }
                        if (LOH == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "23" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                int customerid = Convert.ToInt32(custID);
                                JuniperResponses objRs = new JuniperResponses(23, customerid);
                                tid23 = new Thread(new ThreadStart(() => { JuniparLOHlst = objRs.ThreadedHotelSearch(reqTravillio, "LOH"); }));
                                tid23.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch
                            {
                                LOH = 0;
                            }
                        }
                        if (goglobal == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "27" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                GoGlobal rs = new GoGlobal(custID);
                                tid27 = new Thread(new ThreadStart(() => { gogloballst = rs.GoGlobal_HotelAvailabilitySearch(reqTravillio, "GoGlobal"); }));
                                tid27.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch { goglobal = 0; }
                        }
                        if (cosmobeds == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            string cosmobeds_custID = string.Empty;
                            try
                            {
                                cosmobeds_custID = req.Descendants("SupplierID").Where(x => x.Value == "29" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                int customerid = Convert.ToInt32(cosmobeds_custID);
                                TGServices_cosmo TGScos = new TGServices_cosmo(29, cosmobeds_custID);
                                tid29 = new Thread(new ThreadStart(() => { cosmobedslst = TGScos.HotelSearch(req, "COSMOBEDS", cosmobeds_custID); }));
                                tid29.Priority = System.Threading.ThreadPriority.AboveNormal;
                            
                            }
                            catch { cosmobeds = 0; }
                        }
                        if (stuba == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            string stuba_custID = string.Empty;
                            try
                            {
                                stuba_custID = req.Descendants("SupplierID").Where(x => x.Value == "30" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                int customerid = Convert.ToInt32(stuba_custID);
                                TGServices_stuba TGSx = new TGServices_stuba(30, stuba_custID);
                                tid30 = new Thread(new ThreadStart(() => { stubalst = TGSx.HotelSearch(req, "STUBA", stuba_custID); }));
                                tid30.Priority = System.Threading.ThreadPriority.AboveNormal;
                            
                            }
                            catch { stuba = 0; }
                           
                        }
                        if (Gadou == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            //tid31 = new Thread(new ThreadStart(() => { Gadoulst = gds.HotelAvailablitySearch(reqTravillio, gadouCurrencies, "GADOU"); }));
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "31" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                            }
                            catch { }
                            int customerid = Convert.ToInt32(custID);
                            JuniperResponses objRs = new JuniperResponses(31, customerid);
                            tid31 = new Thread(new ThreadStart(() => { Gadoulst = objRs.ThreadedHotelSearch(reqTravillio, "GADOU"); }));
                        }
                        if (yalago == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            string yalago_custID = string.Empty;
                            try
                            {
                                yalago_custID = req.Descendants("SupplierID").Where(x => x.Value == "32" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                int customerid = Convert.ToInt32(yalago_custID);
                                TGServices_yalago yTGS = new TGServices_yalago(32, yalago_custID);
                                tid32 = new Thread(new ThreadStart(() => { yalagolst = yTGS.HotelSearch(req, "YALAGO", yalago_custID); }));
                                tid32.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch { yalago = 0; }
                           
                        }
                        if (LCI == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "35" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                int customerid = Convert.ToInt32(custID);
                                JuniperResponses objRs = new JuniperResponses(35, customerid);
                                tid35 = new Thread(new ThreadStart(() => { JuniparLCIlst = objRs.ThreadedHotelSearch(reqTravillio, "LCI"); }));
                                tid35.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch { LCI = 0; }
                           
                        }
                        if (SunHotels == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10 && !(req.Descendants("MinStarRating").FirstOrDefault().Value.Equals("0") && req.Descendants("MaxStarRating").FirstOrDefault().Value.Equals("0")))
                        {
                            int paxes = req.Descendants("Adult").Select(x => Convert.ToInt32(x.Value)).Sum(); 
                            int child = req.Descendants("Child").Select(x => Convert.ToInt32(x.Value)).Sum();
                            if (paxes < 10 && child < 10)
                            {
                                string custID = string.Empty;
                                try
                                {
                                    custID = req.Descendants("SupplierID").Where(x => x.Value == "36" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                    int customerid = Convert.ToInt32(custID);
                                    SunHotelsResponse objRs = new SunHotelsResponse(36, customerid);
                                    tid36 = new Thread(new ThreadStart(() => { sunhotelslst = objRs.ThreadedHotelSearch(reqTravillio, custID, "SunHotels"); }));
                                    tid36.Priority = System.Threading.ThreadPriority.AboveNormal;
                                }
                                catch { SunHotels = 0; }
                              
                            }
                            else
                            {
                                SunHotels = 0;
                            }
                        }
                        if (SmyRooms == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "39" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;

                                int customerid = Convert.ToInt32(custID);
                                TGServices smTGS = new TGServices(39, custID);
                                tid39 = new Thread(new ThreadStart(() => { Smylst = smTGS.HotelSearch(req, "SMYROOMS", custID); }));
                            }
                            catch { SmyRooms = 0; }
                        }
                        if (AlphaTours == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "41" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                            }
                            catch { }
                            int customerid = Convert.ToInt32(custID);
                            JuniperResponses objRs = new JuniperResponses(41, customerid);
                            tid41 = new Thread(new ThreadStart(() => { alphatourslst = objRs.ThreadedHotelSearch(reqTravillio, "AlphaTours"); }));
                        }
                        if (welcomebeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "44" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                            }
                            catch { }
                            string customerid = req.Descendants("CustomerID").FirstOrDefault().Value;
                            WelcomebedsService objwl = new WelcomebedsService(customerid);
                            tid44 = new Thread(new ThreadStart(() => { welcomebedslst = objwl.HotelAvailability(reqTravillio, customerid, "WelcomeBeds"); }));
                        }
                        if (Hoojoozat == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string customerid = req.Descendants("CustomerID").Single().Value;
                            HoojService objHooj = new HoojService(customerid);
                            tid45 = new Thread(new ThreadStart(() => { hoojhotelslst = objHooj.HotelAvailability(reqTravillio, "Hoojoozat"); }));
                        }
                        if (vot == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "46" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                            }
                            catch { }
                            string customerid = custID;
                            VOTService objVot = new VOTService(customerid);
                            tid46 = new Thread(new ThreadStart(() => { vothotelslst = objVot.VotHotelAvailability(reqTravillio,custID, "VOT"); }));
                        }
                        if (ebookingcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            string custID = string.Empty;
                            try
                            {
                                custID = req.Descendants("SupplierID").Where(x => x.Value == "47" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                string customerid = custID;
                                EBookingService objebookcntr = new EBookingService(customerid);
                                tid47 = new Thread(new ThreadStart(() => { ebookingcenterhotelslst = objebookcntr.HotelAvailability(reqTravillio, ebookingcenterdocnationality, custID, "EBookingCenter"); }));
                    
                            }
                            catch { ebookingcenter = 0; }
                               }
                        if (wte == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            string wte_custID = string.Empty;
                            try
                            {
                                wte_custID = req.Descendants("SupplierID").Where(x => x.Value == "48" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                WTEService wteobj = new WTEService(wte_custID);
                                tid48 = new Thread(new ThreadStart(() => { wtelst = wteobj.HotelAvailability(req, "Withinearth", wte_custID); }));
                                tid48.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch {wte=0; }

                          
                        }
                        if (iol == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            string iol_custID = string.Empty;
                            try
                            {
                                iol_custID = req.Descendants("SupplierID").Where(x => x.Value == "50" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                                IOLService iolobj = new IOLService(iol_custID);
                                tid50 = new Thread(new ThreadStart(() => { iollst = iolobj.HotelAvailability(req, "IOL", iol_custID); }));
                                tid50.Priority = System.Threading.ThreadPriority.AboveNormal;
                            }
                            catch { iol = 0; }


                        }
                        //if(bookingexpress == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "501" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid501 = new Thread(new ThreadStart(() => { bookingexpresslst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "BookingExpress", 501, newTime); }));
                        //    tid501.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (etripcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "502" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid502 = new Thread(new ThreadStart(() => { etripcenterlst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "ETripCenter", 502, newTime); }));
                        //    tid502.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (holidaysarabia == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "503" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid503 = new Thread(new ThreadStart(() => { holidaysarabialst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "HolidaysArabia", 503, newTime); }));
                        //    tid503.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (novodestino == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "504" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid504 = new Thread(new ThreadStart(() => { novodestinolst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "NovoDestino", 504 , newTime); }));
                        //    tid504.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (plaza == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "533" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid533 = new Thread(new ThreadStart(() => { plazalst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "W2B", 533, newTime); }));
                        //    tid533.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (cbh == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "534" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid534 = new Thread(new ThreadStart(() => { cbhlst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "CBHXML", 534, newTime); }));
                        //    tid534.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (adventures == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "627" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid627 = new Thread(new ThreadStart(() => { adventureslst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "Adventures", 627, newTime); }));
                        //    tid627.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (gtsbeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "628" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid628 = new Thread(new ThreadStart(() => { gtsbedslst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "GTSBeds", 628, newTime); }));
                        //    tid628.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        //if (h2go == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //    string custID = string.Empty;
                        //    try
                        //    {
                        //        custID = req.Descendants("SupplierID").Where(x => x.Value == "629" && x.Attribute("xmlout").Value == "false").FirstOrDefault().Attribute("custID").Value;
                        //    }
                        //    catch { }
                        //    xmlHotelSearch objbook = new xmlHotelSearch();
                        //    tid629 = new Thread(new ThreadStart(() => { h2golst = objbook.hotelSearchXMLOUTAPI(reqTravillio, custID, "H2GO", 629, newTime); }));
                        //    tid629.Priority = System.Threading.ThreadPriority.Highest;
                        //}
                        #endregion
                        List<Tourico.Hotel> hotellisttourico = new List<Tourico.Hotel>();
                        #region Thread Start
                        try
                        {
                            if (darina == 1)
                            {
                                if (Convert.ToInt32(req.Descendants("RoomPax").Count()) == 1)
                                {
                                    tid1.Start();
                                }
                            }
                            if (tourico == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid2.Start();
                            }
                            if (extranet == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 7)
                            {
                                tid3.Start();
                            }
                            if (hotelbeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid4.Start();
                            }
                            if (DOTW == 1 && tid5 != null)
                            {
                                tid5.Start();
                            }
                            if (hotelspro == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6)
                            {
                                tid6.Start();
                            }
                            if (travco == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
                            {
                                tid7.Start();
                            }
                            if (JacTravel == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
                            {
                                tid8.Start();
                            }
                            if (RTS == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid9.Start();
                            }
                            if (Miki == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6)
                            {
                                tid11.Start();
                            }
                            if (restel == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5 && Convert.ToInt32(req.Descendants("Nights").First().Value) <= 15)
                            {
                                tid13.Start();
                            }
                            if (JuniperW2M == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid16.Start();
                            }
                            if (EgyptExpress == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid17.Start();
                            }
                            if (SalTour == 1 && req.Descendants("RoomPax").Count() < 10)
                            {
                                tid19.Start();
                            }
                            if (expedia == 1 && req.Descendants("RoomPax").Count() < 9)
                            {
                                tid20.Start();
                            }
                            if (tbo == 1 && req.Descendants("RoomPax").Count() < 10)
                            {
                                tid21.Start();
                            }
                            if (LOH == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid23.Start();
                            }
                            if (goglobal == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid27.Start();
                            }
                            if (cosmobeds == 1 && req.Descendants("RoomPax").Count() < 5)
                            {
                                tid29.Start();
                            }
                            if (stuba == 1 && req.Descendants("RoomPax").Count() < 5)
                            {
                                tid30.Start();
                            }
                            if (Gadou == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid31.Start();
                            }
                            if (yalago == 1 && req.Descendants("RoomPax").Count() < 5)
                            {
                                tid32.Start();
                            }
                            if (LCI == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid35.Start();
                            }
                            if (SunHotels == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10 && !(req.Descendants("MinStarRating").FirstOrDefault().Value.Equals("0") && req.Descendants("MaxStarRating").FirstOrDefault().Value.Equals("0")))
                            {
                                tid36.Start();
                            }
                            if (totalstay == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
                            {
                                tid37.Start();
                            }
                            if (SmyRooms == 1 && req.Descendants("RoomPax").Count() < 5)
                            {
                                tid39.Start();
                            }
                            if (AlphaTours == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid41.Start();
                            }
                            if (welcomebeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid44.Start();
                            }
                            if (Hoojoozat == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid45.Start();
                            }
                            if (vot == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid46.Start();
                            }
                            if (ebookingcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            {
                                tid47.Start();
                            }
                            if (wte == 1 && req.Descendants("RoomPax").Count() < 5)
                            {
                                tid48.Start();
                            }
                            if (iol == 1 && req.Descendants("RoomPax").Count() < 5)
                            {
                                tid50.Start();
                            }
                            //if (bookingexpress == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid501.Start();
                            //}
                            //if (etripcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid502.Start();
                            //}
                            //if (holidaysarabia == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid503.Start();
                            //}
                            //if (novodestino == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid504.Start();
                            //}
                            //if (plaza == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid533.Start();
                            //}
                            //if (cbh == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid534.Start();
                            //}
                            //if (adventures == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid627.Start();
                            //}
                            //if (gtsbeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid628.Start();
                            //}
                            //if (h2go == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                            //{
                            //    tid629.Start();
                            //}
                        }
                        catch (ThreadStateException te)
                        {

                        }
                        #endregion
                        
                        //#region Timer
                        //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                        //timer.Start();
                        //#endregion
                        #region Thread Join
                        if (darina == 1)
                        {
                            if (Convert.ToInt32(req.Descendants("RoomPax").Count()) == 1)
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
                            }
                        }
                        if (tourico == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (extranet == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 7)
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
                        if (hotelbeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (DOTW == 1)// && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 2)
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
                        if (hotelspro == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6)
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
                        if (travco == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
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
                        if (JacTravel == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
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
                        if (RTS == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (Miki == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6)
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
                        if (restel == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5 && Convert.ToInt32(req.Descendants("Nights").First().Value) <= 15)
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
                        if (JuniperW2M == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (EgyptExpress == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (SalTour == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (expedia == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 9)
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
                        if (tbo == 1 && req.Descendants("RoomPax").Count() < 10)
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
                        if (LOH == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (goglobal == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (cosmobeds == 1 && req.Descendants("RoomPax").Count() < 5)
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
                        if (stuba == 1 && req.Descendants("RoomPax").Count() < 5)
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
                        if (Gadou == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (yalago == 1 && req.Descendants("RoomPax").Count() < 5)
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
                        if (LCI == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (SunHotels == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10 && !(req.Descendants("MinStarRating").FirstOrDefault().Value.Equals("0") && req.Descendants("MaxStarRating").FirstOrDefault().Value.Equals("0")))
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
                        if (totalstay == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
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
                        if (SmyRooms == 1 && req.Descendants("RoomPax").Count() < 5)
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
                        if (AlphaTours == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (welcomebeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (Hoojoozat == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (vot == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (ebookingcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
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
                        if (wte == 1 && req.Descendants("RoomPax").Count() < 5)
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
                        if (iol == 1 && req.Descendants("RoomPax").Count() < 5)
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
                        //if (bookingexpress == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid501.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (etripcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid502.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (holidaysarabia == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid503.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (novodestino == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid504.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (plaza == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid533.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (cbh == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid534.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (adventures == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid627.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (gtsbeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid628.Join(newTime);
                        //    }
                        //    catch { }
                        //}
                        //if (h2go == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    try
                        //    {
                        //        int newTime = sup_cutime - Convert.ToInt32(timer.ElapsedMilliseconds);
                        //        if (newTime < 0)
                        //        {
                        //            newTime = 0;
                        //        }
                        //        tid629.Join(newTime);
                        //    }
                        //    catch { }
                        //}
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
                        if(tid39!=null&& tid39.IsAlive)
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
                        //if (tid501 != null && tid501.IsAlive)
                        //    tid501.Abort();
                        //if (tid502 != null && tid502.IsAlive)
                        //    tid502.Abort();
                        //if (tid503 != null && tid503.IsAlive)
                        //    tid503.Abort();
                        //if (tid504 != null && tid504.IsAlive)
                        //    tid504.Abort();
                        //if (tid533 != null && tid533.IsAlive)
                        //    tid533.Abort();
                        //if (tid534 != null && tid534.IsAlive)
                        //    tid534.Abort();
                        //if (tid627 != null && tid627.IsAlive)
                        //    tid627.Abort();
                        //if (tid628 != null && tid628.IsAlive)
                        //    tid628.Abort();
                        //if (tid629 != null && tid629.IsAlive)
                        //    tid629.Abort();
                        #endregion
                        #region Merge Hotel's List
                        IEnumerable<XElement> request = req.Descendants("searchRequest");
                        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                        #region Get Hotel List
                        IEnumerable<XElement> darinahotels = null;
                        List<XElement> listextanethtl = null;
                        List<XElement> listtouricohtl = null;
                        List<XElement> listhotelbedshtl = null;
                        List<XElement> listdotwhtl = null;
                        List<XElement> listhotelsprohtl = null;
                        List<XElement> listtravcohtl = null;
                        List<XElement> listjactravelhtl = null;
                        List<XElement> listtotalstayhtl = null;
                        List<XElement> listRTShtl = null;
                        List<XElement> listMikihtl = null;
                        List<XElement> listRestelhtl = null;
                        List<XElement> listJuniperW2mtl = null;
                        List<XElement> listegyptexprstl = null;
                        List<XElement> listalphtourstl = null;
                        List<XElement> listSalhtl = null;
                        List<XElement> listExpediahtl = null;
                        List<XElement> listTBOhtl = null;
                        List<XElement> listJuniperLOHtl = null;
                        List<XElement> listgoglobal = null;
                        List<XElement> listcosmobedshtl = null;
                        List<XElement> liststubahtl = null;
                        List<XElement> listyalagohtl = null;
                        List<XElement> listJuniperLCItl = null;
                        List<XElement> listGadouhtl = null;
                        List<XElement> listsunhotelstl = null;
                        List<XElement> listSmyhtl = null;
                        List<XElement> listwelcomebedshotelst = null;
                        List<XElement> listhoojhotelstl = null;
                        List<XElement> listvothotelstl = null;
                        List<XElement> listebookingcenterhotelstl = null;
                        List<XElement> listwtehtl = null;
                        List<XElement> listiolhtl = null;
                        //List<XElement> listbookingexpresshotelstl = null;
                        //List<XElement> listetripcenterhotelstl = null;
                        //List<XElement> listholidaysarabiahotelstl = null;
                        //List<XElement> listnovodestinohotelstl = null;
                        //List<XElement> listplazahotelstl = null;
                        //List<XElement> listcbhhotelstl = null;
                        //List<XElement> listadventureshotelstl = null;
                        //List<XElement> listgtsbedshotelstl = null;
                        //List<XElement> listh2gohotelstl = null;
                        List<Task> tasks = new List<Task>();
                        #region Darina
                        if (darina == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) == 1)
                        {
                            if (darinahotellist != null)
                            {
                                if (darinahotellist.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { darinahotels = darinahotellist; }));
                                        //darinahotels = darinahotellist;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region Tourico
                        if (tourico == 1)
                        {
                            if (responsetourico != null)
                            {
                                if (responsetourico.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { listtouricohtl = responsetourico; }));
                                        //listtouricohtl = responsetourico;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region Extranet
                        if (extranet == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 7)
                        {
                            if (hotelavailabilitylistextranet != null)
                            {
                                if (hotelavailabilitylistextranet.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { listextanethtl = hotelavailabilitylistextranet; }));
                                        //listextanethtl = hotelavailabilitylistextranet;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region HotelBeds
                        if (hotelbeds == 1)
                        {
                            if (hotelbedslist != null)
                            {
                                if (hotelbedslist.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { ; }));
                                        listhotelbedshtl = hotelbedslist;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region DOTW
                        if (DOTW == 1)
                        {

                            if (dotwlist != null)
                            {
                                try
                                {
                                    if (dotwlist.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listdotwhtl = dotwlist; }));
                                        //listdotwhtl = dotwlist;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region HotelsPro
                        if (hotelspro == 1)
                        {
                            if (responsehotelspro != null)
                            {
                                if (responsehotelspro.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { listhotelsprohtl = responsehotelspro; }));
                                        //listhotelsprohtl = responsehotelspro;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region Travco
                        if (travco == 1)
                        {
                            if (travcolist != null)
                            {
                                if (travcolist.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { listtravcohtl = travcolist; }));
                                        //listtravcohtl = travcolist;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region Jac Travel
                        if (JacTravel == 1)
                        {
                            if (jactravelslist != null)
                            {
                                if (jactravelslist.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { listjactravelhtl = jactravelslist; }));
                                        //listjactravelhtl = jactravelslist;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region Total Stay
                        if (totalstay == 1)
                        {
                            if (totalstaylist != null)
                            {
                                if (totalstaylist.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { listtotalstayhtl = totalstaylist; }));
                                        //listtotalstayhtl = totalstaylist;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region RTS
                        if (RTS == 1)
                        {

                            if (RTSlst != null)
                            {
                                if (RTSlst.Count() > 0)
                                {
                                    try
                                    {
                                        tasks.Add(Task.Run(() => { listRTShtl = RTSlst; }));
                                        //listRTShtl = RTSlst;
                                    }
                                    catch { }
                                }
                            }
                        }
                        #endregion
                        #region MIKI
                        if (Miki == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 6)
                        {
                            if (Mikilst != null)
                            {
                                try
                                {
                                    if (Mikilst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listMikihtl = Mikilst; }));
                                        //listMikihtl = Mikilst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region Restel
                        if (restel == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 5)
                        {
                            if (Restellst != null)
                            {
                                try
                                {
                                    if (Restellst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listRestelhtl = Restellst; }));
                                        //listRestelhtl = Restellst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region JuniperW2M
                        if (JuniperW2M == 1)
                        {
                            if (JuniparW2Mlst != null)
                            {
                                try
                                {
                                    if (JuniparW2Mlst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listJuniperW2mtl = JuniparW2Mlst; }));
                                        //listJuniperW2mtl = JuniparW2Mlst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region Egypt Express
                        if (EgyptExpress == 1)
                        {
                            if (egyptexpresslst != null)
                            {
                                try
                                {
                                    if (egyptexpresslst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listegyptexprstl = egyptexpresslst; }));
                                        //listegyptexprstl = egyptexpresslst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region Sal Tours
                        if (SalTour == 1)
                        {
                            try
                            {
                                if (Sallst.Count() > 0)
                                {
                                    tasks.Add(Task.Run(() => { listSalhtl = Sallst; }));
                                    //listSalhtl = Sallst;
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region Expedia
                        if (expedia == 1)
                        {
                            try
                            {
                                if (Expedialst.Count > 0)
                                {
                                    tasks.Add(Task.Run(() => { listExpediahtl = Expedialst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region TBO Holiday
                        if (tbo == 1)
                        {
                            try
                            {
                                if (TBOlst.Count > 0)
                                {
                                    tasks.Add(Task.Run(() => { listTBOhtl = TBOlst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region LOH
                        if (LOH == 1)
                        {
                            if (JuniparLOHlst != null)
                            {
                                try
                                {
                                    if (JuniparLOHlst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listJuniperLOHtl = JuniparLOHlst; }));
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region GoGlobal
                        if (goglobal == 1)
                        {
                            if (gogloballst != null)
                            {
                                try
                                {
                                    if (gogloballst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listgoglobal = gogloballst; }));
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region Cosmobeds
                        if (cosmobeds == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            try
                            {
                                if (cosmobedslst.Count() > 0)
                                {
                                    tasks.Add(Task.Run(() => { listcosmobedshtl = cosmobedslst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region stuba
                        if (stuba == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            try
                            {
                                if (stubalst.Count() > 0)
                                {
                                    tasks.Add(Task.Run(() => { liststubahtl = stubalst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region Godou
                        if (Gadou == 1)
                        {
                            if (Gadoulst != null)
                            {
                                try
                                {
                                    if (Gadoulst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listGadouhtl = Gadoulst; }));
                                        //listGadouhtl = Gadoulst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region YALAGO
                        if (yalago == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            try
                            {
                                if (yalagolst.Count() > 0)
                                {
                                    tasks.Add(Task.Run(() => { listyalagohtl = yalagolst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region LCI
                        if (LCI == 1)
                        {
                            if (JuniparLCIlst != null)
                            {
                                try
                                {
                                    if (JuniparLCIlst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listJuniperLCItl = JuniparLCIlst; }));
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region SunHotels
                        if (SunHotels == 1)
                        {
                            if (sunhotelslst != null)
                            {
                                try
                                {
                                    if (sunhotelslst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listsunhotelstl = sunhotelslst; }));
                                        //listsunhotelstl = sunhotelslst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region SmyRooms
                        if (SmyRooms == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            try
                            {
                                if (Smylst.Count() > 0)
                                {
                                    tasks.Add(Task.Run(() => { listSmyhtl = Smylst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region Alpha Tours
                        if (AlphaTours == 1)
                        {
                            if (alphatourslst != null)
                            {
                                try
                                {
                                    if (alphatourslst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listalphtourstl = alphatourslst; }));
                                        //listalphtourstl = alphatourslst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region WelcomeBeds
                        if (welcomebeds == 1)
                        {
                            if (welcomebedslst != null)
                            {
                                try
                                {
                                    if (welcomebedslst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listwelcomebedshotelst = welcomebedslst; }));
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region Hoojoozat
                        if (Hoojoozat == 1)
                        {
                            if (hoojhotelslst != null)
                            {
                                try
                                {
                                    if (hoojhotelslst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listhoojhotelstl = hoojhotelslst; }));
                                        //listhoojhotelstl = hoojhotelslst;
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region VOT
                        if (vot == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            if (vothotelslst != null)
                            {
                                try
                                {
                                    if (vothotelslst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listvothotelstl = vothotelslst; }));
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region Ebookingcenter
                        if (ebookingcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        {
                            if (ebookingcenterhotelslst != null)
                            {
                                try
                                {
                                    if (ebookingcenterhotelslst.Count() > 0)
                                    {
                                        tasks.Add(Task.Run(() => { listebookingcenterhotelstl = ebookingcenterhotelslst; }));
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion
                        #region Within Earth
                        if (wte == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            try
                            {
                                if (wtelst.Count() > 0)
                                {
                                    tasks.Add(Task.Run(() => { listwtehtl = wtelst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        #region IOL
                        if (iol == 1 && req.Descendants("RoomPax").Count() < 5)
                        {
                            try
                            {
                                if (iollst.Count() > 0)
                                {
                                    tasks.Add(Task.Run(() => { listiolhtl = iollst; }));
                                }
                            }
                            catch { }
                        }
                        #endregion
                        //#region Booking Express
                        //if (bookingexpress == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (bookingexpresslst != null)
                        //    {
                        //        try
                        //        {
                        //            if (bookingexpresslst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listbookingexpresshotelstl = bookingexpresslst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region ETrip Center
                        //if (etripcenter == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (etripcenterlst != null)
                        //    {
                        //        try
                        //        {
                        //            if (etripcenterlst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listetripcenterhotelstl = etripcenterlst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region Holidays Arabia
                        //if (holidaysarabia == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (holidaysarabialst != null)
                        //    {
                        //        try
                        //        {
                        //            if (holidaysarabialst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listholidaysarabiahotelstl = holidaysarabialst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region Novo Destino
                        //if (novodestino == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (novodestinolst != null)
                        //    {
                        //        try
                        //        {
                        //            if (novodestinolst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listnovodestinohotelstl = novodestinolst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region Plaza
                        //if (plaza == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (plazalst != null)
                        //    {
                        //        try
                        //        {
                        //            if (plazalst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listplazahotelstl = plazalst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region CBH
                        //if (cbh == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (cbhlst != null)
                        //    {
                        //        try
                        //        {
                        //            if (cbhlst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listcbhhotelstl = cbhlst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region Adventures
                        //if (adventures == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (adventureslst != null)
                        //    {
                        //        try
                        //        {
                        //            if (adventureslst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listadventureshotelstl = adventureslst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region GTSBeds
                        //if (gtsbeds == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (gtsbedslst != null)
                        //    {
                        //        try
                        //        {
                        //            if (gtsbedslst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listgtsbedshotelstl = gtsbedslst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        //#region H2GO
                        //if (h2go == 1 && Convert.ToInt32(req.Descendants("RoomPax").Count()) < 10)
                        //{
                        //    if (h2golst != null)
                        //    {
                        //        try
                        //        {
                        //            if (h2golst.Count() > 0)
                        //            {
                        //                tasks.Add(Task.Run(() => { listh2gohotelstl = h2golst; }));
                        //            }
                        //        }
                        //        catch { }
                        //    }
                        //}
                        //#endregion
                        Task.WaitAll(tasks.ToArray());
                        #endregion

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
                                       darinahotels,
                                          listtouricohtl,
                                          listextanethtl,
                                           listhotelbedshtl,
                                           listdotwhtl,
                                           listhotelsprohtl,
                                           listtravcohtl,
                                           listjactravelhtl,
                                           listtotalstayhtl,
                                           listRTShtl,
                                           listMikihtl,
                                           listRestelhtl,
                                            listJuniperW2mtl,
                                            listegyptexprstl,
                                            listSalhtl,
                                            listExpediahtl,
                                            listTBOhtl,
                                             listJuniperLOHtl,
                                             listgoglobal,
                                             listcosmobedshtl,
                                             liststubahtl,
                                             listGadouhtl,
                                             listyalagohtl,
                                             listJuniperLCItl,
                                             listsunhotelstl,
                                             listSmyhtl,
                                             listalphtourstl,
                                             listwelcomebedshotelst,
                                             listhoojhotelstl,
                                             listvothotelstl,
                                             listebookingcenterhotelstl,
                                             listwtehtl,
                                             listiolhtl
                                             //listbookingexpresshotelstl,
                                             //listetripcenterhotelstl,
                                             //listholidaysarabiahotelstl,
                                             //listnovodestinohotelstl,
                                             //listplazahotelstl,
                                             //listcbhhotelstl,
                                             //listadventureshotelstl,
                                             //listgtsbedshotelstl,
                                             //listh2gohotelstl
                                       )
                      ))));
                       
                        //try
                        //{
                        //    APILogDetail log = new APILogDetail();
                        //    log.customerID = Convert.ToInt64(reqTravillio.Descendants("CustomerID").Single().Value);
                        //    log.TrackNumber = reqTravillio.Descendants("TransID").Single().Value;
                        //    log.LogTypeID = 151;
                        //    log.LogType = "Bend";
                        //    SaveAPILog savelog = new SaveAPILog();
                        //    savelog.SaveAPILogs(log);
                        //}
                        //catch { }
                        //searchdoc = GiataMapping_Hotel.MapGiataData(searchdoc);
                        return searchdoc;
                        #endregion
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
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "CreateCheckAvailability";
                    ex1.PageName = "HotelSearch_NAHA";
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
        
        
        
        
        void obj_MyEvent(List<XElement> lst)
        {
            if (jactravelslist == null)
            {
                jactravelslist = lst;
            }
            foreach (XElement item in lst)
            {
                jactravelslist.Add(item);
            }
        }
        void obj_MyEvent1(List<XElement> lst)
        {
            try
            {
                if (totalstaylist == null)
                {
                    totalstaylist = lst;
                }
                else
                {
                    foreach (XElement item in lst)
                    {
                        totalstaylist.Add(item);
                    }
                }
            }
            catch { totalstaylist = null; }
        }
        #region Remove Namespaces
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            XElement xmlDocumentWithoutNs = removeAllNamespaces(xmlDocument);
            return xmlDocumentWithoutNs;
        }

        private static XElement removeAllNamespaces(XElement xmlDocument)
        {
            var stripped = new XElement(xmlDocument.Name.LocalName);
            foreach (var attribute in
                    xmlDocument.Attributes().Where(
                    attribute =>
                        !attribute.IsNamespaceDeclaration &&
                        String.IsNullOrEmpty(attribute.Name.NamespaceName)))
            {
                stripped.Add(new XAttribute(attribute.Name.LocalName, attribute.Value));
            }
            if (!xmlDocument.HasElements)
            {
                stripped.Value = xmlDocument.Value;
                return stripped;
            }
            stripped.Add(xmlDocument.Elements().Select(
                el =>
                    RemoveAllNamespaces(el)));
            return stripped;
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