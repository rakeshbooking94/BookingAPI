using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TravillioXMLOutService.Models.GoGlobal;
using Newtonsoft.Json;
using System.Data;
using TravillioXMLOutService.Models;
using System.Text.RegularExpressions;
using System.Threading;
using System.Configuration;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TravillioXMLOutService.Supplier.GoGlobals
{

      public class GoGlobal : IDisposable
    {
        bool Test = true;
        string customerid = string.Empty;
        int SuupplierId = 27;
        string desiredCurrency = "";
        #region Credentails of GoGlobal
        string agency = "";
        string user = "";
        string password = "";
        int MaxWaitTime = 30;
        string Host = "";
        public GoGlobal(string _customerid) 
        {
            GoGlobalCredential _credential = new GoGlobalCredential(_customerid);
            agency = _credential.Agency;
            user = _credential.User;
            password = _credential.password;
            SuupplierId = _credential.Supplier;
            MaxWaitTime = _credential.MaxWaitTime;
            desiredCurrency = _credential.Currency;
            Host = _credential.Host;
            customerid = _customerid;
        }





        #endregion


        XElement reqTravayoo;
        string dmc = string.Empty;



        //int CotAge = 2;     //versio 2.0  
         int CotAge = 0;  //versio 2.2
        // no need to add tag in Req if Cot     and greater than cot Age will count as Extrabed   
        //Cot will allow only with one or two Adult

        int childmaxage = 12;



        #region HotelSearch
        public List<XElement> GoGlobal_HotelAvailabilitySearch(XElement main_req, string xtype)
        {
            #region get cut off time
            int timeOutsup = 0;
            try
            {
                timeOutsup = supplier_Cred.secondcutoff_time(main_req.Descendants("HotelID").FirstOrDefault().Value);
            }
            catch { }
            #endregion
            string SupplierCityId = string.Empty;
            List<XElement> goGlobalresponse = new List<XElement>();
            int timeOut = Convert.ToInt32(ConfigurationManager.AppSettings["gg_timeout"]);
            // here is we making chunks for hotel code 
            string HotelID = string.Empty;

            string paxNation = GetSupplierCountryId(main_req);

            int chunksize = 500;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["chunksize_gg"]))
            {
                chunksize = Convert.ToInt32(ConfigurationManager.AppSettings["chunksize_gg"]);
            }

            string cityid = GetSupplierCityId(main_req);
            DataTable dt = GetHotelsDetailList(cityid);

            if (main_req.Descendants("HotelID").Count() > 0 && main_req.Descendants("HotelID").FirstOrDefault().Value != "")
            {
                HotelID = main_req.Descendants("HotelID").FirstOrDefault().Value;
                goGlobalresponse = SearchHotel(main_req, xtype, null, cityid, dt, timeOutsup,paxNation);
            }
            else
            {
                var statiProperties = dt.AsEnumerable().Select(r => r.Field<string>("HotelId")).ToList();
                List<List<string>> splitList = GoglobalHelper.TSplitPropertyList(statiProperties, chunksize);

                List<XElement> thr1 = new List<XElement>();
                List<XElement> thr2 = new List<XElement>();
                List<XElement> thr3 = new List<XElement>();
                List<XElement> thr4 = new List<XElement>();
                List<XElement> thr5 = new List<XElement>();
                List<XElement> thr6 = new List<XElement>();


                List<Thread> threadlist;

                #region Parallel
                //BlockingCollection<List<XElement>> _block = new BlockingCollection<List<XElement>>();
                //int k = 0;
                //Parallel.ForEach(splitList, new ParallelOptions { MaxDegreeOfParallelism = 8 }, async i =>
                //{
                //    List<XElement> cust = new List<XElement>();
                //    cust = SearchHotel(main_req, xtype, i, cityid, dt);
                //    k = k + 1;
                //    _block.Add(cust);
                //});


                //foreach (var r in _block)
                //{
                //    try
                //    {
                //        if (r != null)
                //        {
                //            goGlobalresponse.AddRange(r);
                //        }
                //    }
                //    catch { }
                //}
                #endregion

                int Number = splitList.Count;
                bool checkod = true;
                if (Number % 2 == 0)
                {
                    checkod = false;
                }
                else
                {
                    Number = Number + 1;
                }
                //timeOutsup = 32000;
                int count = 1;
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                #region commented
                //for (int i = 0; i < Number; i += 4)
                //{
                //    #region threads




                //    if (splitList.Count() == 1)
                //    {
                //        threadlist = new List<Thread>
                //        {
                //        new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[0],cityid ,dt,timeOutsup)),
                //        };

                //        threadlist.ForEach(t => t.Start());
                //        threadlist.ForEach(t => t.Join(timeOutsup));
                //        threadlist.ForEach(t => t.Abort());
                //        goGlobalresponse.AddRange(thr1);
                //    }


                //    if (splitList.Count() == 2)
                //    {
                //        threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[0],cityid,dt,timeOutsup)),
                //                     new Thread(()=> thr2 =SearchHotel(main_req,xtype, splitList[1],cityid,dt,timeOutsup)),
                //                };
                //        threadlist.ForEach(t => t.Start());
                //        threadlist.ForEach(t => t.Join(timeOutsup));
                //        threadlist.ForEach(t => t.Abort());
                //        if (thr1 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr1);
                //            }
                //            catch { }
                //        }
                //        if (thr2 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr2);
                //            }
                //            catch { }
                //        }
                //    }

                //    if (splitList.Count() == 3)
                //    {
                //        threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[0],cityid,dt,timeOutsup)),
                //                     new Thread(()=> thr2 =SearchHotel(main_req,xtype, splitList[1],cityid,dt,timeOutsup)),
                //                      new Thread(()=> thr3 =SearchHotel(main_req,xtype, splitList[2],cityid,dt,timeOutsup)),
                //                };
                //        threadlist.ForEach(t => t.Start());
                //        threadlist.ForEach(t => t.Join(timeOutsup));
                //        threadlist.ForEach(t => t.Abort());
                //        //goGlobalresponse.AddRange(thr1.Concat(thr2).Concat(thr3));
                //        if (thr1 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr1);
                //            }
                //            catch { }
                //        }
                //        if (thr2 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr2);
                //            }
                //            catch { }
                //        }
                //        if (thr3 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr3);
                //            }
                //            catch { }
                //        }
                //    }


                //    if (splitList.Count() == 4)
                //    {
                //        threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[0],cityid,dt,timeOutsup)),
                //                     new Thread(()=> thr2 =SearchHotel(main_req,xtype, splitList[1],cityid,dt,timeOutsup)),
                //                      new Thread(()=> thr3 =SearchHotel(main_req,xtype, splitList[2],cityid,dt,timeOutsup)),
                //                        new Thread(()=> thr4 =SearchHotel(main_req,xtype, splitList[3],cityid,dt,timeOutsup)),
                //                };
                //        threadlist.ForEach(t => t.Start());
                //        threadlist.ForEach(t => t.Join(timeOutsup));
                //        threadlist.ForEach(t => t.Abort());
                //        if (thr1 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr1);
                //            }
                //            catch { }
                //        }
                //        if (thr2 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr2);
                //            }
                //            catch { }
                //        }
                //        if (thr3 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr3);
                //            }
                //            catch { }
                //        }
                //        if (thr4 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr4);
                //            }
                //            catch { }
                //        }

                //    }

                //    if (splitList.Count() == 5)
                //    {
                //        threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[0],cityid,dt,timeOutsup)),
                //                     new Thread(()=> thr2 =SearchHotel(main_req,xtype, splitList[1],cityid,dt,timeOutsup)),
                //                      new Thread(()=> thr3 =SearchHotel(main_req,xtype, splitList[2],cityid,dt,timeOutsup)),
                //                       new Thread(()=> thr4 =SearchHotel(main_req,xtype, splitList[3],cityid,dt,timeOutsup)),
                //                        new Thread(()=> thr5 =SearchHotel(main_req,xtype, splitList[4],cityid,dt,timeOutsup)),
                //                };
                //        threadlist.ForEach(t => t.Start());
                //        threadlist.ForEach(t => t.Join(timeOutsup));
                //        threadlist.ForEach(t => t.Abort());
                //        if (thr1 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr1);
                //            }
                //            catch { }
                //        }
                //        if (thr2 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr2);
                //            }
                //            catch { }
                //        }
                //        if (thr3 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr3);
                //            }
                //            catch { }
                //        }
                //        if (thr4 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr4);
                //            }
                //            catch { }
                //        }

                //        if (thr5 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr5);
                //            }
                //            catch { }
                //        }
                //    }

                //    if (splitList.Count() >= 6)
                //    {
                //        threadlist = new List<Thread>
                //                {   
                //                    new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[0],cityid,dt,timeOutsup)),
                //                     new Thread(()=> thr2 =SearchHotel(main_req,xtype, splitList[1],cityid,dt,timeOutsup)),
                //                      new Thread(()=> thr3 =SearchHotel(main_req,xtype, splitList[2],cityid,dt,timeOutsup)),
                //                       new Thread(()=> thr4 =SearchHotel(main_req,xtype, splitList[3],cityid,dt,timeOutsup)),
                //                        new Thread(()=> thr5 =SearchHotel(main_req,xtype, splitList[4],cityid,dt,timeOutsup)),
                //                         new Thread(()=> thr6 =SearchHotel(main_req,xtype, splitList[5],cityid,dt,timeOutsup)),
                //                };
                //        threadlist.ForEach(t => t.Start());
                //        threadlist.ForEach(t => t.Join(timeOutsup));
                //        threadlist.ForEach(t => t.Abort());
                //        if (thr1 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr1);
                //            }
                //            catch { }
                //        }
                //        if (thr2 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr2);
                //            }
                //            catch { }
                //        }
                //        if (thr3 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr3);
                //            }
                //            catch { }
                //        }
                //        if (thr4 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr4);
                //            }
                //            catch { }
                //        }

                //        if (thr5 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr5);
                //            }
                //            catch { }
                //        }

                //        if (thr6 != null)
                //        {
                //            try
                //            {
                //                goGlobalresponse.AddRange(thr6);
                //            }
                //            catch { }
                //        }
                //    }

                //    #endregion
                //    timeOutsup = timeOutsup - Convert.ToInt32(timer.ElapsedMilliseconds);
                //    count++;
                //}
                #endregion
                if (splitList.Count() == 1)
                {
                    threadlist = new List<Thread>
                        {
                        new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[0],cityid ,dt,timeOutsup,paxNation)),
                        };

                    threadlist.ForEach(t => t.Start());
                    threadlist.ForEach(t => t.Join(timeOutsup));
                    threadlist.ForEach(t => t.Abort());
                    try
                    {
                        XElement res = new XElement("Hotels", thr1);
                        foreach (XElement hotel in res.Descendants("Hotel"))
                            goGlobalresponse.Add(hotel);
                    }
                    catch { }
                    timeOutsup = timeOutsup - Convert.ToInt32(timer.ElapsedMilliseconds);
                }
                else
                {
                    for (int i = 0; i < Number/2; i += 2)
                    {
                        if (timeOutsup >= 2)
                        {
                            if (checkod == true && i == Number-1)
                            {
                                threadlist = new List<Thread>
                                            {
                                            new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[i],cityid ,dt,timeOutsup,paxNation)),
                                            };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOutsup));
                                threadlist.ForEach(t => t.Abort());
                                try
                                {
                                    XElement res = new XElement("Hotels", thr1);
                                    foreach (XElement hotel in res.Descendants("Hotel"))
                                        goGlobalresponse.Add(hotel);
                                }
                                catch { }
                            }
                            else
                            {
                                threadlist = new List<Thread>
                                            {
                                           new Thread(()=> thr1 = SearchHotel(main_req,xtype, splitList[i],cityid ,dt,timeOutsup,paxNation)),
                                           new Thread(()=> thr2 = SearchHotel(main_req,xtype, splitList[i+1],cityid ,dt,timeOutsup,paxNation))
                                            };
                                threadlist.ForEach(t => t.Start());
                                threadlist.ForEach(t => t.Join(timeOutsup));
                                threadlist.ForEach(t => t.Abort());
                                if (thr1 != null)
                                {
                                    try
                                    {
                                        XElement res = new XElement("Hotels", thr1);
                                        foreach (XElement hotel in res.Descendants("Hotel"))
                                            goGlobalresponse.Add(hotel);
                                    }
                                    catch { }
                                }
                                if (thr2 != null)
                                {
                                    try
                                    {
                                        XElement res = new XElement("Hotels", thr2);
                                        foreach (XElement hotel in res.Descendants("Hotel"))
                                            goGlobalresponse.Add(hotel);
                                    }
                                    catch { }
                                }
                            }
                            timeOutsup = timeOutsup - Convert.ToInt32(timer.ElapsedMilliseconds);
                        }
                    }
                }
            }



            return goGlobalresponse;
        }

        public List<XElement> SearchHotel(XElement main_req, string xtype, List<string> splitList, string cityid, DataTable staticHotelsData, int timeout ,string paxNation)
        {
            List<XElement> HotelsList = new List<XElement>();
            try
            {
                string SupplierCityId = null;

                string goglobalReq = convert_mainReqIn_goGlobalReq(main_req, out SupplierCityId, splitList, cityid, paxNation);
                if (goglobalReq == null || SupplierCityId == null || SupplierCityId == "")
                {
                    return null;
                }

                #region start
                try
                {
                    bool error = false;
                    string Err = null;
                    dynamic response = null;
                    try
                    {
                        response = GoGlobalSupplierResponse_Search(timeout, goglobalReq, main_req, "Search", ref error, ref Err, null);
                    }
                    catch (Exception ex)
                    {
                        CustomException ex1 = new CustomException(ex);
                        ex1.MethodName = "GoGlobalSupplierResponse";
                        ex1.PageName = "GoGlobal";
                        ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                        ex1.TranID = main_req.Descendants("TransID").Single().Value;
                        SaveAPILog saveex = new SaveAPILog();
                        saveex.SendCustomExcepToDB(ex1);
                    }

                    if (error == true)
                        return null;

                    try
                    {
                        //DataTable staticHotelsData = GetHotelsDetailList(SupplierCityId); 
                        HotelsList = GetHotelListGoGlobal(response, staticHotelsData, null, null, main_req, xtype);
                       
                    }
                    catch (Exception ex)
                    {
                        CustomException ex1 = new CustomException(ex);
                        ex1.MethodName = "GoGlobal_HotelAvailabilitySearch1";
                        ex1.PageName = "GoGlobal";
                        ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                        ex1.TranID = main_req.Descendants("TransID").Single().Value;
                        SaveAPILog saveex = new SaveAPILog();
                        saveex.SendCustomExcepToDB(ex1);
                    }





                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobal_HotelAvailabilitySearch2";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                    ex1.TranID = main_req.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);


                }

                #endregion

            }
            catch
            {
                
            }
            return HotelsList;
        }

        public string convert_mainReqIn_goGlobalReq(XElement main_req, out string supcityid, List<string> splitList ,string city_id, string pax_Nation)
        { 
            supcityid = "";
            string GoGlobalReq = string.Empty;
           
            #region RoomTag
            if (main_req.Descendants("Room").Count() > 4)
                return null;

            var Rms = new XElement("Rooms");
            string adultcountpre = string.Empty; string childagepre = string.Empty; int i = 0;
            XElement RoomType = null; int rid = 1;

            bool reqtype = true;

            var _main = from m in main_req.Descendants("RoomPax")
                        select m;
            foreach (XElement element in _main)
            {
                if (element.Attribute("Adult") == null)
                    element.Add(new XAttribute("Adult", ""));
                if (element.Attribute("ChildAge") == null)
                    element.Add(new XAttribute("ChildAge", ""));


                element.Attribute("Adult").Value = element.Element("Adult").Value;
               
                int child = 0;
                if (element.Element("Child")!=null)
                int.TryParse(element.Element("Child").Value, out child);
                if (child > 0)
                {
                    var childages = from m in element.Descendants("ChildAge")
                                    orderby m.Value
                                    select m;
                    string _ch = String.Empty;
                    foreach (XElement e in childages)
                    {
                        _ch += e.Value + ",";

                    }

                    element.Attribute("ChildAge").Value = _ch;
                    //element.Add(new XAttribute("ChildAge", _ch));

                }
                else
                {
                    element.Attribute("ChildAge").Value ="";
                   // element.Add(new XAttribute("ChildAge", ""));
                }
            }

            var main = from m in main_req.Descendants("RoomPax")
                       orderby m.Attribute("Adult").Value, m.Attribute("ChildAge").Value
                       select m;
       
            foreach (XElement element in main)
            {
                string adultcount = string.Empty; string childage = string.Empty;

                int cotcount = 0;
                int convertedadult = 0;
                int realchildcount = 0;
                if (i == 0)
                {
                    adultcountpre = adultcount = element.Attribute("Adult").Value;
                    childagepre = childage = element.Attribute("ChildAge").Value;
                    RoomType = new XElement("Room", new XAttribute("Adults", adultcount), new XAttribute("RoomCount", rid), new XAttribute("ChildCount", 0));
                    RoomType.Add(childtag2(childage, ref cotcount, ref convertedadult, ref realchildcount));
                    if (cotcount > 0)
                        RoomType.Add(new XAttribute("CotCount", cotcount));
                    if (convertedadult > 0)
                        RoomType.Attribute("Adults").Value = (Convert.ToInt16( adultcount )+ convertedadult).ToString();
                    if (realchildcount > 0)
                        RoomType.Attribute("ChildCount").Value = realchildcount.ToString();
                    i++;
                }
                else
                {
                    adultcount = element.Attribute("Adult").Value;
                    childage = element.Attribute("ChildAge").Value;

                    if (adultcountpre == adultcount && childagepre == childage)
                    {
                        rid++;
                        RoomType = new XElement("Room", new XAttribute("Adults", adultcount), new XAttribute("RoomCount", rid), new XAttribute("ChildCount", 0));
                        RoomType.Add(childtag2(childage, ref cotcount, ref convertedadult, ref realchildcount));
                        if (cotcount > 0)
                            RoomType.Add(new XAttribute("CotCount", cotcount));
                        if (convertedadult > 0)
                            RoomType.Attribute("Adults").Value = (adultcount + convertedadult);
                        if (realchildcount > 0)
                            RoomType.Attribute("ChildCount").Value = realchildcount.ToString();
                    }
                    else
                    {
                        rid = 1;
                        Rms.Add(RoomType);
                        RoomType = new XElement("Room", new XAttribute("Adults", adultcount), new XAttribute("RoomCount", rid), new XAttribute("ChildCount", 0));
                        RoomType.Add(childtag2(childage, ref cotcount, ref convertedadult, ref realchildcount));
                        if (cotcount > 0)
                            RoomType.Add(new XAttribute("CotCount", cotcount));
                        if (convertedadult > 0)
                            RoomType.Attribute("Adults").Value = (adultcount + convertedadult);
                        if (realchildcount > 0)
                            RoomType.Attribute("ChildCount").Value = realchildcount.ToString();

                        adultcountpre = adultcount;
                        childagepre = childage;



                    }

                   
                }
                if (cotcount > 1)
                    return null;
                if (!reqtype)
                    return null;
            }//make room section

            Rms.Add(RoomType);

            #endregion

            
            XElement Meal = new XElement("FilterRoomBasises", new XElement("FilterRoomBasis"));

            // needed more info for code 
            #region Star code
           
            string min = main_req.Descendants("MinStarRating").FirstOrDefault().Value;
            string max = main_req.Descendants("MaxStarRating").FirstOrDefault().Value;

            String Stars = String.Empty;
             
            #endregion

            string cityid = city_id;
            supcityid = city_id;
            string paxNation = pax_Nation;

           

            string HotelID = main_req.Descendants("HotelID").FirstOrDefault().Value;
            string HotelList = String.Empty;
            if(HotelID!="" && HotelID!="0")
            {
               HotelList= GetHotelList(HotelID, cityid, "");
               MaxWaitTime = Convert.ToInt32(ConfigurationManager.AppSettings["gg_timeout"]);
            }
            else
            {
                
                if (splitList.Count() > 0)
                {
                    int k = 0;
                    HotelList = "<Hotels>";
                    foreach (var row in splitList)
                    {
                        //if (k < 501)
                        {
                            HotelList += "<HotelId>" + Convert.ToString(row) + "</HotelId>";
                        }
                        //k++;
                    }

                    HotelList += "</Hotels>";
                }
                
                MaxWaitTime = Convert.ToInt32(ConfigurationManager.AppSettings["gg_timeout"]);
            }
             
            string ArrivalDate = main_req.Descendants("FromDate").FirstOrDefault().Value;
            int nights = 0;

            try
            {

                DateTime fromDate = DateTime.ParseExact(ArrivalDate, "dd/MM/yyyy", null);
                DateTime toDate = DateTime.ParseExact(main_req.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                nights = (int)(toDate - fromDate).TotalDays;
                //"2019-11-11"
                string Day = fromDate.Day.ToString();
                if (fromDate.Day < 10)
                    Day = "0" + Day;

                string Month = fromDate.Month.ToString();
                if (fromDate.Month < 10)
                    Month = "0" + Month;

                ArrivalDate = fromDate.Year + "-" + Month + "-" + Day;


            }
            catch { }

             

            var cur = '"' + desiredCurrency + '"';

             
            GoGlobalReq = @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                            <soap12:Body>
                            <MakeRequest xmlns=""http://www.goglobal.travel/"">
                            <requestType>11</requestType>
                            <xmlRequest><![CDATA[
                            <Root>
                            <Header>
                               <Agency>" + agency + @"</Agency>
		                        <User>" + user + @"</User>
		                        <Password>" + password + @"</Password>
		                        <Operation>HOTEL_SEARCH_REQUEST</Operation>
		                        <OperationType>Request</OperationType>
                            </Header>
                             <Main Version=""2.2"" ResponseFormat=""JSON"" IncludeGeo=""true""  Currency=" + cur + @">
		                        <SortOrder>1</SortOrder>
	                            <MaximumWaitTime>" + MaxWaitTime + @"</MaximumWaitTime>
		                        <FilterRoomBasises>
			                        <FilterRoomBasis></FilterRoomBasis>
		                        </FilterRoomBasises>
		                        <HotelName></HotelName>
		                        <Apartments>false</Apartments>
                               <CityCode>" + cityid + @"</CityCode>"
                                + HotelList+
		                        @"<ArrivalDate>" + ArrivalDate + @"</ArrivalDate>
		                        <Nights>" + nights + @"</Nights>
                                <Nationality>" + paxNation + @"</Nationality>
                                    " + Stars + @"
		                            " + Rms + @"
	                        </Main>
                        </Root>
                        ]]></xmlRequest>
                        </MakeRequest>
                        </soap12:Body>
                        </soap12:Envelope>";

                     
           
        //<CityCode>" + cityid + @"</CityCode>

            //Add remarks ?????
            //Children are between ages 2 and 10 or between 0 to 18 when using version 2.2 and above ????
            //All characters (names) should be non-unicode only.???

           // XElement supplierreq = XElement.Parse(sss);

            return GoGlobalReq;

        }

        private string convertStarForReq(string min, string max)
        {
            try 
            {
                int _min = Convert.ToInt16(min);
                int _max = Convert.ToInt16(max);

                string Stars = "<Star>";

                for (int i = _min; i <= _max; i++)
                {

                    Stars += i + ",";

                }

                Stars += "</Star>";


                return Stars;

            }
            catch 
            {
                return "";
            }

        
        }

        private int convertStarForResponse(string star)
        {
            double _star = 0;
            if (star != null && star != string.Empty)
            {
                
                double.TryParse(star,out _star);
                if (_star < 2)
                    return 1;
                else if (_star < 3)
                    return 2;
                else if (_star < 4)
                    return 3;
                else if (_star < 5)
                    return 4;
                else
                    return 5;

            }
            return 3;

        }

        public List<XElement> childtag(string chage,ref int  cotcount)
        {
           
            List<XElement> childs = new List<XElement>();
            if (chage != null && chage != "")
            {
                string[] chs = chage.Split(',');
                foreach (var c in chs)
                {
                    if (c != "")
                    {
                        int _c=0;
                        int.TryParse(c,out _c );
                        if (_c <= CotAge)
                            cotcount++;
                        else
                        {
                            XElement child = new XElement("ChildAge", c.ToString());
                            childs.Add(child);
                        }
                    }

                }

            }

            return childs;
        }

        public List<XElement> childtag2(string chage, ref int cotcount, ref int convertedadult ,ref int  realchildcount)
        {

            List<XElement> childs = new List<XElement>();
            if (chage != null && chage != "")
            {
                string[] chs = chage.Split(',');
                foreach (var c in chs)
                {
                    if (c != "")
                    {
                        int _c = 0;
                        int.TryParse(c, out _c);
                        if (_c <= CotAge)
                            cotcount++;
                        else   if (_c > childmaxage)
                            convertedadult++;
                        else
                        {
                            XElement child = new XElement("ChildAge", c.ToString());
                            childs.Add(child);
                            realchildcount++;
                        }
                    }

                }

            }

            return childs;
        }


        public string GetSupplierCityId(XElement req)
        {
            GoGlobal_Detail goglbModel = new GoGlobal_Detail();
            GoGlobal_Static goglbStatic = new GoGlobal_Static();
            goglbModel.CityCode = req.Descendants("CityID").FirstOrDefault().Value;
            DataTable dtcity = goglbStatic.GetCity_GoGlobal(goglbModel);
            string citycode = string.Empty;
            citycode = "";
            if (dtcity != null)
            {
                if (dtcity.Rows.Count != 0)
                {
                    citycode = dtcity.Rows[0]["citycode"].ToString();
                     return citycode;
                }
            }

            return citycode;  
          
        }
        public string GetSupplierCountryId(XElement req)
        {

            GoGlobal_Static goglbStatic = new GoGlobal_Static();
            string CountryId = req.Descendants("PaxNationality_CountryID").FirstOrDefault().Value;
            string CountrySupplierCode = goglbStatic.GetCountrySupplierCOuntryCode_GoGlobal(CountryId);
            if (CountrySupplierCode == "")
            {
                CountryId = req.Descendants("PaxNationality_CountryID").FirstOrDefault().Value;
                CountrySupplierCode = goglbStatic.GetCountrySupplierCOuntryCode_GoGlobal(CountryId);
            }


            //GoGlobal_Detail goglbModel = new GoGlobal_Detail();
            //GoGlobal_Static goglbStatic = new GoGlobal_Static();
            //string CountryId= req.Descendants("CountryID").FirstOrDefault().Value;
            //DataTable dtcity = goglbStatic.GetCountry_GoGlobal(CountryId);
            //string countrycode = string.Empty;
            ////countrycode = req.Descendants("CountryCode").FirstOrDefault().Value;
            //if (dtcity != null)
            //{
            //    if (dtcity.Rows.Count != 0)
            //    {
            //        countrycode = dtcity.Rows[0]["countrycode"].ToString();
            //        return countrycode;
            //    }
            //}
           
            return CountrySupplierCode;
            

        }
        public string GetHotelList(string HotelId, string  CityId,string Name)
        {
            GoGlobal_Detail goglbModel = new GoGlobal_Detail();
            GoGlobal_Static goglbStatic = new GoGlobal_Static();
            
            DataTable dt= goglbStatic.GetHotelList_GoGlobal(HotelId, CityId, Name);

            string hotellist = "";
            if (dt!= null)
            {
              
                foreach (DataRow row in dt.Rows)
                {
                    hotellist += "<HotelId>" + row["hotelid"].ToString() + "</HotelId>";
                }



            }
            if (hotellist!="")
            hotellist = "<Hotels>"+hotellist+"</Hotels>";


            return hotellist;
        }
        public DataTable GetHotelsDetailList(string CityId)
        {
            GoGlobal_Detail goglbModel = new GoGlobal_Detail();
            GoGlobal_Static goglbStatic = new GoGlobal_Static();

            DataTable dt = goglbStatic.GetHotelList_GoGlobal("", CityId, "");       


            return dt;

        }




        private IEnumerable<XElement> GetHotelListGoGlobal(dynamic hotel, DataTable hotelstatic, DataTable starrating, XElement fac, XElement main_req,string xtype)
        {      
            string xmloutcustid ="";
            string xmlouttype ="";
            string NotFindInData = "";
            #region Hotels
            List<XElement> hotellst = new List<XElement>();
            try
            {
                var supmain = (from m in main_req.Descendants("SupplierID")
                               where m.Value == SuupplierId.ToString()
                               select m).FirstOrDefault();
                if (supmain != null)
                {
                    xmloutcustid = supmain.Attribute("custID").Value;
                    //xmloutcustid = supmain.Descendants("CustomerID").Single().Value;
                    xmlouttype = supmain.Attribute("xmlout").Value;
                }
                Int32 length = 0;
                try
                {

                    length = Convert.ToInt32(hotel.Hotels.Count);
                }
                catch
                {
                    CustomException ex1 = new CustomException();
                    ex1.MethodName = "GetHotelListGoGlobal/No HOtel Found";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                    ex1.TranID = main_req.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    return null; 
                }
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        #region Fetch hotel
                        string Thumbnail = "";
                        string Location = "";
                        string Longitude = "";
                        string Latitude = "";
                        string HotelName = Convert.ToString(hotel.Hotels[i].HotelName.Value);
                        string HotelID = Convert.ToString(hotel.Hotels[i].HotelCode.Value);
                        string CountryId = Convert.ToString(hotel.Hotels[i].CountryId.Value);
                        string CityId = Convert.ToString(hotel.Hotels[i].CityId.Value);
                        if (hotel.Hotels[i].Location != null)
                        {
                            Location = Convert.ToString(hotel.Hotels[i].Location.Value);
                            Location = replace(Location);
                        }
                        if (hotel.Hotels[i].Thumbnail != null)
                            Thumbnail = Convert.ToString(hotel.Hotels[i].Thumbnail.Value);
                        if (hotel.Hotels[i].Longitude != null)
                            Longitude = Convert.ToString(hotel.Hotels[i].Longitude.Value);
                        if (hotel.Hotels[i].Latitude != null)
                            Latitude = Convert.ToString(hotel.Hotels[i].Latitude.Value);

                        DataRow[] row = hotelstatic.Select("hotelid = " + "'" + HotelID + "'");
                        string CountryName = "";
                        string CountryCode = "";
                        string CityCode = "";
                        string CityName = "";
                        string Address = "";
                        if (row != null && row.Count() > 0)
                        {
                            try
                            {
                                CountryName = row[0]["country"].ToString();
                                CountryCode = row[0]["countryid"].ToString();
                                CityCode = row[0]["cityid"].ToString();
                                CityName = row[0]["city"].ToString();
                                Address = row[0]["Address"].ToString();
                                try
                                {
                                    if (Address.Contains('|'))
                                        Address = Address.Split('|')[0];
                                }
                                catch { }
                                Address = replace(Address);
                            }
                            catch { }
                        }
                        else
                        {
                            NotFindInData += HotelID + ",";
                        }
                        string Description = "HI";
                        string StarRating = "";
                        if (hotel.Hotels[i].Offers[0].Category != null)
                        {
                            StarRating = Convert.ToString(hotel.Hotels[i].Offers[0].Category.Value);//
                            //DataRow[] starrow = starrating.Select("StarRating = " + "'" + StarRating + "'");
                            StarRating = convertStarForResponse(StarRating).ToString();
                        }
                        string HotelImageSmall = Thumbnail;
                        string HotelImageLarge = Thumbnail;
                        string MapLink = "";
                        string Currency = "";
                        if (hotel.Hotels[i].Offers[0].Currency != null)
                            Currency = Convert.ToString(hotel.Hotels[i].Offers[0].Currency.Value);

                        string Offers = "";
                        string Facility = "";
                        IEnumerable<XElement> facility = null;
                        decimal minRate = 0;
                        int minindx = 0;
                        string star = string.Empty;
                        #region Get Min Rate
                        try
                        {
                            int count = hotel.Hotels[i].Offers.Count;
                            for (int n = 0; n < count; n++)
                            {

                                decimal totalrate = Convert.ToDecimal(hotel.Hotels[i].Offers[n].TotalPrice.Value);
                                if (minindx == 0)
                                {
                                    minRate = totalrate;
                                }
                                if (totalrate < minRate)
                                {
                                    minRate = totalrate;
                                }
                                minindx++;

                            }

                        }
                        catch { minRate = hotel.Hotels[i].Offers[0].TotalPrice.Value; }
                        #endregion

                        hotellst.Add(new XElement("Hotel",
                                               new XElement("HotelID", HotelID),
                                               new XElement("HotelName", HotelName),
                                               new XElement("PropertyTypeName", ""),
                                               new XElement("CountryID", CountryId),
                                               new XElement("CountryName", CountryName),
                                               new XElement("CountryCode", CountryId),
                                               new XElement("CityId", ""),
                                               new XElement("CityCode", CityId),
                                               new XElement("CityName", CityName),
                                               new XElement("AreaId", Location),
                                               new XElement("AreaName", Location),
                                               new XElement("RequestID", ""),
                                               new XElement("Address", Address),
                                               new XElement("Location", Location),
                                               new XElement("Description", Convert.ToString("")),
                                               new XElement("StarRating", StarRating),
                                               new XElement("MinRate", Convert.ToString(minRate)),
                                               new XElement("HotelImgSmall", HotelImageSmall),
                                               new XElement("HotelImgLarge", HotelImageLarge),
                                               new XElement("MapLink", ""),
                                               new XElement("Longitude", Longitude),
                                               new XElement("Latitude", Latitude),
                                                 new XElement("xmloutcustid", xmloutcustid),
                                               new XElement("xmlouttype", xmlouttype),
                                               new XElement("DMC", xtype),
                                               new XElement("SupplierID", SuupplierId),
                                               new XElement("Currency", Currency),
                                               new XElement("Offers", "")
                                               , new XElement("Facilities", null)
                            //facility)
                                               , new XElement("Rooms", ""

                                                   )
                        ));

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        //CustomException ex1 = new CustomException(ex);
                        //ex1.MethodName = "GetHotelListGoGlobal";
                        //ex1.PageName = "GoGlobal";
                        //ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                        //ex1.TranID = main_req.Descendants("TransID").Single().Value;
                        //SaveAPILog saveex = new SaveAPILog();
                        //saveex.SendCustomExcepToDB(ex1);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GetHotelListGoGlobal";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }
            if (NotFindInData!="")
            {               
                CustomException ex1 = new CustomException();
                ex1.MsgName = "HotelId Not Found";
                ex1.ExcSource = NotFindInData;
                ex1.MethodName = "GetHotelListGoGlobal";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }
            return hotellst;
            #endregion
        }



        private string replace(string r)
        {
            r = Regex.Replace(r, @"[^\u0000-\u007F]", string.Empty);
            r = r.Replace("\"", string.Empty);

            r = r.Replace(@"\\", string.Empty);
            ////try
            ////{
              
            ////}
            ////catch
            ////{
            ////    return "";
            ////}
            return r;

        }


        #endregion



        #region RoomAvail


   
        public List<XElement> getroomavail_GoGlobalOUT(XElement req)
        {
            List<XElement> roomavailabilityresponse = new List<XElement>();
            try
            {
                #region changed
                string dmc = string.Empty;
                List<XElement> htlele = req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "27").ToList();
                for (int i = 0; i < htlele.Count(); i++)
                {
                    string custID = string.Empty;
                    string custName = string.Empty;
                    string htlid = htlele[i].Attribute("GHtlID").Value;
                    string xmlout = string.Empty;
                    try
                    {
                        xmlout = htlele[i].Attribute("xmlout").Value;
                    }
                    catch { xmlout = "false"; }
                    if (xmlout == "true")
                    {
                        try
                        {
                            customerid = htlele[i].Attribute("custID").Value;
                            dmc = htlele[i].Attribute("custName").Value;
                        }
                        catch { custName = "HA"; }
                    }
                    else
                    {
                        try
                        {
                            customerid = htlele[i].Attribute("custID").Value;
                        }
                        catch { }
                        dmc = "GoGlobal";
                    }
                    XElement getrom = RoomAvailability(req, dmc, htlid);
                    roomavailabilityresponse.Add(getrom.Descendants("Hotel").FirstOrDefault());
                }
                #endregion
                return roomavailabilityresponse;
            }
            catch { return null; }
        }	

        public XElement RoomAvailability(XElement main_req, string xtype, string htlid)
        {
            //reqTravayoo = main_req = travayooReqest();
            dynamic response = null;
            XElement roomresponse = null;
            string hotelid = "";
            try
            {
            string goglobalReq = convert_mainroomReqIn_goGlobalReq(main_req,out hotelid);
            if (goglobalReq == null)
            {
                roomresponse = new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"));
                return roomresponse;
            }
            bool error = false;
            string Err = null;

            response = GoGlobalSupplierResponse_Room(0, goglobalReq, main_req, "RoomAvail", ref error, ref Err,  hotelid);
            if (error == true)
            {
                roomresponse = new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"));
                return roomresponse; 
            
            }
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GoGlobal_RoomSearch1";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }

            try
            {
                List<XElement> strgrp = GetHotelRoomListingGoGlobal(response, null, null, hotelid, null, xtype, main_req);

                roomresponse = new XElement
              ("searchResponse",
              new XElement
              ("Hotels",

               new XElement
              ("Hotel",
                new XAttribute("HotelID", main_req.Descendants("HotelID").FirstOrDefault().Value),
                   new XAttribute("HotelName", main_req.Descendants("HotelName").FirstOrDefault().Value),
                    new XElement("DMC", xtype),
                    //   new XAttribute("CountryID",""),
                    //      new XAttribute("CountryName",""),
                    //         new XAttribute("CountryCode",""),
                    //            new XAttribute("CityId",""),
                    //               new XAttribute("CityCode",""),
                    //                  new XAttribute("CityName",""),
                    //                     new XAttribute("AreaId",""),
                    //                      new XAttribute("AreaName",""),
                    //            new XAttribute("Address",""),
                    //               new XAttribute("Location",""),
                    //                  new XAttribute("Description",""),
                    //                     new XAttribute("StarRating",""),

                      //                      new XAttribute("MinRate",""),
                    //               new XAttribute("HotelImgSmall",""),
                    //                  new XAttribute("HotelImgLarge",""),
                    //                     new XAttribute("MapLink",""),

                      //                             new XAttribute("Longitude",""),
                    //               new XAttribute("Latitude",""),
                    //                  new XAttribute("SupplierID",""),
                    //                     new XAttribute("Currency",""),
                    //                          new XAttribute("Offers",""),
                                                strgrp
              )


              )



              );


            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GoGlobal_RoomSearch2";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_req.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                roomresponse = new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"));
                
            }




            return roomresponse;





            //add strgrp in main_req
            //try
            //{
            //    XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
            //    XElement roomResponse = main_req.Element(namespc + "Body");

            //    roomResponse.Add(roomresponse);
            //    return roomResponse;

            //}
            //catch (Exception ex)
            //{
            //    CustomException ex1 = new CustomException(ex);
            //    ex1.MethodName = "GoGlobal_RoomSearch3";
            //    ex1.PageName = "GoGlobal";
            //    ex1.CustomerID = main_req.Descendants("CustomerID").Single().Value;
            //    ex1.TranID = main_req.Descendants("TransID").Single().Value;
            //    SaveAPILog saveex = new SaveAPILog();
            //    saveex.SendCustomExcepToDB(ex1);
            //    return null;
            //}


        }


        public string convert_mainroomReqIn_goGlobalReq(XElement main_req, out string hotelid)
        {
            //hotelid = get_supplierhotelid(main_req.Descendants("HotelID").FirstOrDefault().Value);
           // hotelid = main_req.Descendants("HotelID").FirstOrDefault().Value;
            XElement htlele = main_req.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == "27").FirstOrDefault();
            hotelid = htlele.Attribute("GHtlID").Value;
          
            string cityid = GetSupplierCityId(main_req);
            string paxNation = GetSupplierCountryId(main_req);

            XElement Meal = new XElement("FilterRoomBasises", new XElement("FilterRoomBasis"));

            string ArrivalDate = main_req.Descendants("FromDate").FirstOrDefault().Value;
            int nights = 0;

            try
            {

                DateTime fromDate = DateTime.ParseExact(ArrivalDate, "dd/MM/yyyy", null);
                DateTime toDate = DateTime.ParseExact(main_req.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                nights = (int)(toDate - fromDate).TotalDays;
                //"2019-11-11"
                string Day = fromDate.Day.ToString();
                if (fromDate.Day < 10)
                    Day = "0" + Day;

                string Month = fromDate.Month.ToString();
                if (fromDate.Month < 10)
                    Month = "0" + Month;

                ArrivalDate = fromDate.Year + "-" + Month + "-" + Day;


            }
            catch { }

            #region RoomTag
            if (main_req.Descendants("Room").Count() > 4)
                return null;

            var Rms = new XElement("Rooms");
            string adultcountpre = string.Empty; string childagepre = string.Empty; int i = 0;
            XElement RoomType = null; int rid = 1;

            bool reqtype = true;

            var _main = from m in main_req.Descendants("RoomPax")
                        select m;
            foreach (XElement element in _main)
            {
                if (element.Attribute("Adult") == null)
                    element.Add(new XAttribute("Adult", ""));
                if (element.Attribute("ChildAge") == null)
                { element.Add(new XAttribute("ChildAge", "")); }
               


                element.Attribute("Adult").Value = element.Element("Adult").Value;

                int child = 0;
                if (element.Element("Child")!=null)
                int.TryParse(element.Element("Child").Value, out child);
                if (child > 0)
                {
                    var childages = from m in element.Descendants("ChildAge")
                                    orderby m.Value
                                    select m;
                    string _ch = String.Empty;
                    foreach (XElement e in childages)
                    {
                        _ch += e.Value + ",";

                    }

                    element.Attribute("ChildAge").Value = _ch;
                    //element.Add(new XAttribute("ChildAge", _ch));

                }
                else
                {
                    element.Attribute("ChildAge").Value = "";
                    // element.Add(new XAttribute("ChildAge", ""));
                }
            }

            var main = from m in main_req.Descendants("RoomPax")
                       orderby m.Attribute("Adult").Value, m.Attribute("ChildAge").Value
                       select m;

            foreach (XElement element in main)
            {
                string adultcount = string.Empty; string childage = string.Empty;


                int cotcount = 0;
                int convertedadult = 0;
                int realchildcount = 0;
                if (i == 0)
                {
                    adultcountpre = adultcount = element.Attribute("Adult").Value;
                    childagepre = childage = element.Attribute("ChildAge").Value;
                    RoomType = new XElement("Room", new XAttribute("Adults", adultcount), new XAttribute("RoomCount", rid), new XAttribute("ChildCount", 0));
                    RoomType.Add(childtag2(childage, ref cotcount, ref convertedadult, ref realchildcount));
                    if (cotcount > 0)
                        RoomType.Add(new XAttribute("CotCount", cotcount));
                    if (convertedadult > 0)
                        RoomType.Attribute("Adults").Value = (Convert.ToInt16(adultcount) + convertedadult).ToString();
                    if (realchildcount > 0)
                        RoomType.Attribute("ChildCount").Value = realchildcount.ToString();
                    i++;
                }
                else
                {
                    adultcount = element.Attribute("Adult").Value;
                    childage = element.Attribute("ChildAge").Value;

                    if (adultcountpre == adultcount && childagepre == childage)
                    {
                        rid++;
                        RoomType = new XElement("Room", new XAttribute("Adults", adultcount), new XAttribute("RoomCount", rid), new XAttribute("ChildCount", 0));
                        RoomType.Add(childtag2(childage, ref cotcount, ref convertedadult, ref realchildcount));
                        if (cotcount > 0)
                            RoomType.Add(new XAttribute("CotCount", cotcount));
                        if (convertedadult > 0)
                            RoomType.Attribute("Adults").Value = (Convert.ToInt16(adultcount) + convertedadult).ToString();
                        if (realchildcount > 0)
                            RoomType.Attribute("ChildCount").Value = realchildcount.ToString();
                    }
                    else
                    {
                        rid = 1;
                        Rms.Add(RoomType);
                        RoomType = new XElement("Room", new XAttribute("Adults", adultcount), new XAttribute("RoomCount", rid), new XAttribute("ChildCount", 0));
                        RoomType.Add(childtag2(childage, ref cotcount, ref convertedadult, ref realchildcount));
                        if (cotcount > 0)
                            RoomType.Add(new XAttribute("CotCount", cotcount));
                        if (convertedadult > 0)
                            RoomType.Attribute("Adults").Value = (Convert.ToInt16(adultcount) + convertedadult).ToString();
                        if (realchildcount > 0)
                            RoomType.Attribute("ChildCount").Value = realchildcount.ToString();

                        adultcountpre = adultcount;
                        childagepre = childage;



                    }

                }
                if (cotcount > 1)
                    return null;
                if (!reqtype)
                    return null;
            }//make room section

            Rms.Add(RoomType);

            #endregion


            // needed more info for code 
            #region Star code
            //           XElement Stars = new XElement("Stars");
            //           StarCode	Description
            //1	1 star
            //2	1.5 stars
            //3	2 stars
            //4	2.5 stars
            //5	3 stars
            //6	3.5 stars
            //7	4 stars
            //8	4.5 star
            //9	5 stars
            //10	5.5 stars
            //11	6 stars


            #endregion

            MaxWaitTime = 5;
            var cur = '"' + desiredCurrency + '"';

            cityid = "";
            /*change by ravish 09102020      
             <API-AgencyID>" + agency + @"</API-AgencyID>
             <API-Operation>HOTEL_SEARCH_REQUEST</API-Operation>*/
            string sss = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
<soap12:Body>
<MakeRequest xmlns=""http://www.goglobal.travel/"">
<requestType>11</requestType>
<xmlRequest><![CDATA[
<Root>
<Header>

	   <Agency>" + agency + @"</Agency>
		<User>" + user + @"</User>
		<Password>" + password + @"</Password>
		<Operation>HOTEL_SEARCH_REQUEST</Operation>
		<OperationType>Request</OperationType>
</Header>
  <Main Version=""2.2"" ResponseFormat=""JSON"" IncludeGeo=""true""  Currency=" + cur + @">
		<SortOrder>1</SortOrder>
		 <MaximumWaitTime>" + MaxWaitTime + @"</MaximumWaitTime>
		<FilterRoomBasises>
			<FilterRoomBasis></FilterRoomBasis>
		</FilterRoomBasises>
		<HotelName></HotelName>
        <Hotels>
			<HotelId>"
                + hotelid +
                @"</HotelId>
		</Hotels>
		<Apartments>false</Apartments>
			<CityCode>" + cityid + @"</CityCode>
		<ArrivalDate>" + ArrivalDate + @"</ArrivalDate>
			<Nights>" + nights + @"</Nights>
          <Nationality>" + paxNation + @"</Nationality>
		 " + Rms + @"
	</Main>
</Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";

            return sss;

            // XDocument reqxml = XDocument.Parse(sss);

            // return reqxml;

        }


        public string get_supplierhotelid(string giataid)
        {
            
            GoGlobal_Static goglbStatic = new GoGlobal_Static();

            DataTable dtcity = goglbStatic.GetSupplierId_fromGiataId(giataid);
            string Htlcode = string.Empty;
       
            if (dtcity != null)
            {
                if (dtcity.Rows.Count != 0)
                {
                    Htlcode = dtcity.Rows[0]["hotelid"].ToString();
                }
            }
            return Htlcode;// Use Dummy        

            return "255369";
        }


        int SupplierId = 27;
        int totalroomreq = 1;
        public IEnumerable<XElement> GetHotelRoomListingGoGlobal(dynamic roomlist, XElement mealtype, string code, string Hotelcode, string currency, string dmctype, XElement mainreq)
        {


            //string path = "C:\\Users\\gauravp.INGSOFT\\Desktop\\xxx.xml";
            // XElement Static_goresponse = XElement.Load(path);
            //string HotelCode = "255369";
            //XElement Hotel = Static_goresponse.Descendants("item").Where(x => x.Descendants("HotelCode").FirstOrDefault().Value == HotelCode).FirstOrDefault();

            XElement BreakupsLog=new XElement("Breakups");
            string BreakupsReqestes = "";
            DateTime startTime = DateTime.Now;
            List<XElement> strgrp = new List<XElement>();
            #region Notes: The maximum number of rooms that can be retrieved by a single search request is nine (9)
            #endregion
            int nights = 0;
            int totalroom = 1;
            try
            {
                totalroom = Convert.ToInt32(mainreq.Descendants("RoomPax").Count());
                DateTime fromDate = DateTime.ParseExact(mainreq.Descendants("FromDate").Single().Value, "dd/MM/yyyy", null);
                DateTime toDate = DateTime.ParseExact(mainreq.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                nights = (int)(toDate - fromDate).TotalDays;
            }
            catch { }
            #region Room Count
            for (int i = 0; i < roomlist.Hotels[0].Offers.Count; i++)
            {
                List<XElement> str = new List<XElement>();
                XElement pricebrkups = null;
                string roomset_code = roomlist.Hotels[0].Offers[i].HotelSearchCode;

                string mealplancode = roomlist.Hotels[0].Offers[i].RoomBasis;
                
                string mealname = MealType(mealplancode);
                mealplancode = MealTypeCode(mealplancode);

                decimal totalamt = Convert.ToDecimal(roomlist.Hotels[0].Offers[i].TotalPrice) / totalroom; ;


                int _roomtypes = roomlist.Hotels[0].Offers[i].Rooms.Count;
                string RoomType = roomlist.Hotels[0].Offers[i].Rooms[0];
                string Currency = roomlist.Hotels[0].Offers[i].Currency;
                string goglobal_breakupReq = get_pricebrekupReq(roomset_code);

                //BreakupsReqestes += roomset_code + ",";
               // XElement response_breakup = GoGlobalSupplierResponseXMLBrekUp(goglobal_breakupReq, mainreq, roomset_code, ref BreakupsLog);

                pricebrkups = GetRoomBrekUpGoGlobal_ForRoomSearch(totalamt, nights);

                for (int m = 0; m < totalroom; m++)
                {


                    if (_roomtypes > 1)
                        RoomType = roomlist.Hotels[0].Offers[i].Rooms[m];




                    IEnumerable<XElement> totroomprc = null;
                  

                    string AdultNo = "0";
                    string totalchild = "0";

                    try
                    {


                        var RoomPax = mainreq.Descendants("RoomPax").Skip(m).FirstOrDefault();
                        AdultNo = RoomPax.Descendants("Adult").FirstOrDefault().Value;
                        totalchild = RoomPax.Descendants("Child").FirstOrDefault().Value;

                        //pricebrkups = GetRoomBrekUpGoGlobal(response_breakup, m + 1,mainreq);

                    }
                    catch (Exception ex)
                    {
                        CustomException ex1 = new CustomException(ex);
                        ex1.MethodName = "GetHotelRoomListingGoGlobal/" + roomset_code+"GetRoomBrekUpGoGlobal";
                        ex1.PageName = "GoGlobal";
                        ex1.CustomerID = mainreq.Descendants("CustomerID").Single().Value;
                        ex1.TranID = mainreq.Descendants("TransID").Single().Value;
                        SaveAPILog saveex = new SaveAPILog();
                        saveex.SendCustomExcepToDB(ex1);
                    }




                    #region With Board Bases

                    str.Add(new XElement("Room",
                         new XAttribute("ID", roomset_code),
                         new XAttribute("SuppliersID", SupplierId),
                         new XAttribute("RoomSeq", m + 1),
                         new XAttribute("SessionID", roomset_code),
                         new XAttribute("RoomType", RoomType),
                         new XAttribute("OccupancyID", Convert.ToString("")),
                         new XAttribute("OccupancyName", Convert.ToString("")),
                         new XAttribute("MealPlanID", Convert.ToString("")),
                         new XAttribute("MealPlanName", mealname),
                         new XAttribute("MealPlanCode", mealplancode),
                         new XAttribute("MealPlanPrice", ""),
                         new XAttribute("PerNightRoomRate", Convert.ToString("0")),
                         new XAttribute("TotalRoomRate", Convert.ToString(totalamt)),
                         new XAttribute("CancellationDate", ""),
                         new XAttribute("CancellationAmount", ""),
                         new XAttribute("isAvailable", "true"),
                         new XElement("RequestID", Convert.ToString("")),
                         new XElement("Offers", ""),
                         new XElement("PromotionList", ""),
                         new XElement("CancellationPolicy", ""),
                         new XElement("Amenities", new XElement("Amenity", "")),
                         new XElement("Images", new XElement("Image", new XAttribute("Path", ""))),
                         new XElement("Supplements", ""),
                         pricebrkups,
                        //new XElement("PriceBreakups", totroomprc),
                             new XElement("AdultNum", AdultNo),
                             new XElement("ChildNum", totalchild)
                         ));
                    #endregion
                }
                if (totalroom == str.ToList().Count())
                {
                    strgrp.Add(new XElement("RoomTypes", new XAttribute("Index", i + 1), new XAttribute("TotalRate", Convert.ToString(roomlist.Hotels[0].Offers[i].TotalPrice)), new XAttribute("HtlCode", Hotelcode ?? ""), new XAttribute("CrncyCode", Currency), new XAttribute("DMCType", dmctype),
                                 str));
                }
            }
            #endregion
            #region Log Save
        
            //APILogDetail log = new APILogDetail();
            //log.customerID = Convert.ToInt64(mainreq.Descendants("CustomerID").Single().Value);
            //log.TrackNumber = mainreq.Descendants("TransID").Single().Value;
            //log.LogTypeID = 1;
            //log.LogType = "BreakUps";
            //log.SupplierID = SuupplierId;
            //log.logrequestXML = BreakupsReqestes.ToString();
            //log.logresponseXML = BreakupsLog.ToString();
            //log.StartTime = startTime;
            //log.EndTime = DateTime.Now;
            //try
            //{
            //    SaveAPILog savelogpro = new SaveAPILog();
            //    savelogpro.SaveAPILogs(log);
            //}
            //catch (Exception ex)
            //{
            //    CustomException ex1 = new CustomException(ex);
            //    ex1.MethodName = "GetHotelRoomListingGoGlobal/" + "SaveBreakups";
            //    ex1.PageName = "GoGlobal";
            //    ex1.CustomerID = mainreq.Descendants("CustomerID").Single().Value;
            //    ex1.TranID = mainreq.Descendants("TransID").Single().Value;
            //    SaveAPILog saveex = new SaveAPILog();
            //    saveex.SendCustomExcepToDB(ex1);
            //}
            #endregion
            return strgrp;

           

       






        }


        static string MealType(string value)
        {
            switch (value)
            {
                case "BB":
                    return "BED AND BREAKFAST";
                case "CB":
                    return "CONTINENTAL BREAKFAST";
                case "AI":
                    return "ALL-INCLUSIVE";
                case "FB":
                    return "FULL-BOARD";
                case "HB":
                    return "HALF-BOARD";
                case "RO":
                    return "ROOM ONLY";
                case "BD":
                    return "BED AND DINNER";
                default:
                    {
                        return value;
                    }

            }
        }


        static string MealTypeCode(string value)
        {
            switch (value)
            {
                case "CB":
                    return "BB";
                case "BD":
                    return "HB";
                default:
                    {
                        return value;
                    }

            }
        }

        public XElement travayooReqest()
        {
            string sss = @"<?xml version=""1.0"" encoding=""utf-8""?><soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soapenv:Header>
    <Authentication >
      <AgentID>TRV</AgentID>
      <UserName>Travillio</UserName>
      <Password>ing@tech</Password>
      <ServiceType>HT_001</ServiceType>
      <ServiceVersion>v1.0</ServiceVersion>
    </Authentication>
  </soapenv:Header>
  <soapenv:Body>
    <searchRequest >
      <Response_Type>XML</Response_Type>
      <HotelID>176166</HotelID>
      <RequestID></RequestID>
      <FromDate>18/09/2019</FromDate>
      <ToDate>20/09/2019</ToDate>
      <Nights>2</Nights>
      <CountryID>1</CountryID>
      <CountryName>United Arab Emirates</CountryName>
      <CityID>1312</CityID>
      <CityCode>DXB</CityCode>
      <CityName>DUBAI</CityName>
      <AreaID>0</AreaID>
      <AreaName></AreaName>
      <MinStarRating>0</MinStarRating>
      <MaxStarRating>5</MaxStarRating>
      <HotelName></HotelName>
      <PaxNationality_CountryID>111</PaxNationality_CountryID>
      <PaxNationality_CountryCode>IN</PaxNationality_CountryCode>
      <PaxResidenceID>111</PaxResidenceID>
      <PaxResidenceName>India</PaxResidenceName>
      <CurrencyID>1</CurrencyID>
      <DesiredCurrencyCode></DesiredCurrencyCode>
      <CustomerID>10011</CustomerID>
      <TransID>zzaaa122cf39db1b1a6r</TransID>
      <OnRequest>true</OnRequest>
      <GiataList>
        <GiataHotelList GHtlID=""176166"" GSupID=""4"" GRequestID="""" xmlout=""false"" custID=""10011"" custName=""HotelBeds"" sessionKey="""" sourcekey="""" publishedkey="""" />
      </GiataList>
      <Rooms>
        <RoomPax>
          <Adult>1</Adult>
          <Child>0</Child>
          <ChildAges>0</ChildAges>
        </RoomPax>
         <RoomPax>
          <Adult>1</Adult>
          <Child>0</Child>
          <ChildAges>0</ChildAges>
        </RoomPax>
      </Rooms>
      <MealPlanList>
        <MealType />
      </MealPlanList>
      <PropertyType />
      <SuppliersList>
        <SupplierID xmlout=""false"">4</SupplierID>
      </SuppliersList>
      <SubAgentID>82285</SubAgentID>
      <AgentUserID>0</AgentUserID>
      <NationalityName>India</NationalityName>
    </searchRequest>
  </soapenv:Body>
</soapenv:Envelope>";

            //            sss = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
            //<soap12:Body>
            //<MakeRequest xmlns=""http://www.goglobal.travel/"">
            //<requestType>11</requestType>
            //<xmlRequest><![CDATA[
            //<Root>
            //<Header>
            //	   <Agency>1521643</Agency>
            //		<User>NOVODESTINOXML</User>
            //		<Password>QVVTZNNK3YZP</Password>
            //		<Operation>HOTEL_SEARCH_REQUEST</Operation>
            //		<OperationType>Request</OperationType>
            //</Header>
            //  <Main Version=""2"" ResponseFormat=""JSON"">
            //		<SortOrder>1</SortOrder>
            //		<FilterPriceMin>0</FilterPriceMin>
            //		<FilterPriceMax>10000</FilterPriceMax>
            //		<MaximumWaitTime>30</MaximumWaitTime>
            //		<MaxResponses>1000</MaxResponses>
            //		<FilterRoomBasises>
            //			<FilterRoomBasis></FilterRoomBasis>
            //		</FilterRoomBasises>
            //		<HotelName></HotelName>
            //		<Apartments>false</Apartments>
            //		<CityCode>17260</CityCode>
            //		<ArrivalDate>2019-11-11</ArrivalDate>
            //		<Nights>3</Nights>
            //		<Rooms>
            //			<Room Adults=""1"" RoomCount=""1"" ></Room>
            //		</Rooms>
            //	</Main>
            //</Root>
            //]]></xmlRequest>
            //</MakeRequest>
            //</soap12:Body>
            //</soap12:Envelope>";




            XElement reqTravayoo = XElement.Parse(sss);

            return reqTravayoo;

        }




        public string get_pricebrekupReq(string roomcode)
        {

            string sss = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
<soap12:Body>
<MakeRequest xmlns=""http://www.goglobal.travel/"">
<requestType>14</requestType>
<xmlRequest><![CDATA[
<Root>
<Header>
        <Agency>" + agency + @"</Agency>
		<User>" + user + @"</User>
		<Password>" + password + @"</Password>
		<Operation>PRICE_BREAKDOWN_REQUEST</Operation>
		<OperationType>Request</OperationType>
</Header>
<Main>
<HotelSearchCode>" + roomcode + @"</HotelSearchCode>
</Main>
</Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";

            return sss;

            // XDocument reqxml = XDocument.Parse(sss);

            // return reqxml;

        }

    


        public XElement GetRoomBrekUpGoGlobal_ForRoomSearch(decimal PerRoomPrice, int TotalNigts)
        {

        
            decimal PerNightPrice = PerRoomPrice / TotalNigts;
            var ResponseBreakups = new XElement("PriceBreakups");

            for (int n = 1; n <= TotalNigts; n++)
            {

                ResponseBreakups.Add(new XElement("Price", new XAttribute("Night", n), new XAttribute("PriceValue", PerNightPrice)));

            }

            return ResponseBreakups;



         
        }
        



        #endregion



        #region PreBook

        public XElement PreBooking(XElement MainpreBookReq, string xmlout)
        {

            
            dmc = xmlout;




            string sss = "";

            XElement PreBookRoom = null;
            string transID = MainpreBookReq.Descendants("TransID").FirstOrDefault().Value;
            string RoomId = MainpreBookReq.Descendants("Room").FirstOrDefault().Attribute("ID").Value;// "548263/9139220948992809322/0";
            string roomset_code = RoomId;
            string remark_PreBook = string.Empty;
            string remark_roomavail = string.Empty;
            string canceldate_PreBook = string.Empty;
            decimal PrePrice = 0;
            decimal NewPrice = 0;
            bool sameprice = true;
            try
            {           // XElement reqPreBookxml = XElement.Parse(sss);
              
            
               


                decimal.TryParse(MainpreBookReq.Descendants("RoomTypes").FirstOrDefault().Attribute("TotalRate").Value, out PrePrice);



                DateTime fromDate = DateTime.ParseExact(MainpreBookReq.Descendants("FromDate").Single().Value, "dd/MM/yyyy", null);
             

                string Day = fromDate.Day.ToString();
                if (fromDate.Day < 10)
                    Day = "0" + Day;

                string Month = fromDate.Month.ToString();
                if (fromDate.Month < 10)
                    Month = "0" + Month;

                string  ArrivalDate = fromDate.Year + "-" + Month + "-" + Day;





                sameprice = chkPriceGoGlobal(PrePrice, roomset_code, ArrivalDate, MainpreBookReq, out  remark_PreBook, ref NewPrice, ref canceldate_PreBook);

                

                if (sameprice == false && NewPrice==0)
               {
                   PreBookRoom = new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"));
                   goto returnatexception;

               }
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GetPreBook/chkPriceGoGlobal";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = MainpreBookReq.Descendants("CustomerID").Single().Value;
                ex1.TranID = MainpreBookReq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);

                PreBookRoom = new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"));
                goto returnatexception;
           
            }





       
            try
            {           // XElement reqPreBookxml = XElement.Parse(sss);

                PreBookRoom = getPreBookFromDB(transID, roomset_code, MainpreBookReq, sameprice, NewPrice, dmc, remark_PreBook,ref  remark_roomavail);
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GetPreBook/getPreBookFromDB";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = MainpreBookReq.Descendants("CustomerID").Single().Value;
                ex1.TranID = MainpreBookReq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);

                PreBookRoom = new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"));
                goto returnatexception;
            }


            decimal[] TOtalPrices = null;
            
            try
            {
                XElement response_breakup = null;
                try
                {
                    // Supplier Hit For BreakUp
                    string goglobal_breakupReq = get_pricebrekupReq(RoomId);
                    response_breakup = GoGlobalSupplierResponseXML(goglobal_breakupReq, MainpreBookReq, "BreakUp", "PRICE_BREAKDOWN_REQUEST");


                  

                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GetPreBook/GoGlobalSupplierResponseXML";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = MainpreBookReq.Descendants("CustomerID").Single().Value;
                    ex1.TranID = MainpreBookReq.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    PreBookRoom = new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"));
                    goto returnatexception;


                }
                int nights = 0;
                int totalroom = 1;
                try
                {
                    totalroom = Convert.ToInt32(MainpreBookReq.Descendants("RoomPax").Count());
                    DateTime fromDate = DateTime.ParseExact(MainpreBookReq.Descendants("FromDate").Single().Value, "dd/MM/yyyy", null);
                    DateTime toDate = DateTime.ParseExact(MainpreBookReq.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                    nights = (int)(toDate - fromDate).TotalDays;
                }
                catch { }

                //ResponseBreakups.Add(new XElement("Price", new XAttribute("Night", nightno), new XAttribute("PriceValue", pernighPrices)));
                TOtalPrices = new decimal[nights];
                decimal update_ForRoom = 0;
  
                if (sameprice == false)
                {
                    PrePrice = NewPrice;
                     update_ForRoom = NewPrice / totalroom;
                     PreBookRoom.Descendants("RoomTypes").FirstOrDefault().Attribute("TotalRate").Value = NewPrice.ToString();
                    //PreBookRoom.Descendants("Room").FirstOrDefault().Attribute("TotalRoomRate").Value = update_ForRoom.ToString();
                }
                decimal Total_InBrkUp = 0;
                XElement pricebrkups_firstroom =  new XElement("PriceBreakups");
            for (int m = 0; m < totalroom; m++)
            {
                int roomseq = m + 1;
                XElement pricebrkups = GetRoomBrekUpGoGlobal(response_breakup, roomseq, MainpreBookReq);
                if (pricebrkups == null)
                    pricebrkups = pricebrkups_firstroom;
                else
                    pricebrkups_firstroom = pricebrkups;

                XElement PreBooka = (from ra in PreBookRoom.Descendants("Room")
                                where ra.Attribute("RoomSeq").Value == roomseq.ToString()
                                select ra).FirstOrDefault();

                if (sameprice == false)
                PreBooka.Attribute("TotalRoomRate").Value = update_ForRoom.ToString();

                XElement Breakup = PreBooka.Element("PriceBreakups");

                Breakup.ReplaceWith(  pricebrkups );

                for (int mm = 0; mm < nights; mm++)
                {
                    decimal _nht = 0;
                    XElement NHT = (from ra in pricebrkups.Descendants("Price")
                                  where ra.Attribute("Night").Value == (mm + 1).ToString()
                                  select ra).FirstOrDefault();
                    if (NHT != null)
                    {
                        string nht = NHT.Attribute("PriceValue").Value;

                        decimal.TryParse(nht, out _nht);

                        TOtalPrices[mm] += _nht;
                        Total_InBrkUp += _nht;
                    }
                }

            }

            Decimal value1 = Decimal.Floor(Total_InBrkUp);
            Decimal value2 = Decimal.Floor(NewPrice);
            if (sameprice == false && value1 != value2)
             {
                 TOtalPrices = new decimal[nights];
                 Total_InBrkUp = 0;
                 for (int m = 0; m < totalroom; m++)
                 {
                     int roomseq = m + 1;
                     //XElement pricebrkups = GetRoomBrekUpGoGlobal(response_breakup, roomseq, MainpreBookReq);
                     XElement pricebrkups = GetRoomBrekUpGoGlobal_ForRoomSearch(update_ForRoom, nights);

                     XElement PreBooka = (from ra in PreBookRoom.Descendants("Room")
                                          where ra.Attribute("RoomSeq").Value == roomseq.ToString()
                                          select ra).FirstOrDefault();

                     XElement Breakup = PreBooka.Element("PriceBreakups");

                     Breakup.ReplaceWith(pricebrkups);

                     for (int mm = 0; mm < nights; mm++)
                     {
                         decimal _nht = 0;
                         XElement NHT = (from ra in pricebrkups.Descendants("Price")
                                         where ra.Attribute("Night").Value == (mm + 1).ToString()
                                         select ra).FirstOrDefault();
                         if (NHT != null)
                         {
                             string nht = NHT.Attribute("PriceValue").Value;

                             decimal.TryParse(nht, out _nht);

                             TOtalPrices[mm] += _nht;
                             Total_InBrkUp += _nht;
                         }
                     }

                 }


             }

            
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GetPreBook2";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = MainpreBookReq.Descendants("CustomerID").Single().Value;
                ex1.TranID = MainpreBookReq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                PreBookRoom = new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"));
                goto returnatexception;
            }
            //add responseprebook in main_prebookreq

            XElement CXL = null;
            try
            {
                //remark_PreBook = "STARTING 07/01/2020 CXL-PENALTY FEE IS CNX FEE  - 1 NT X 100.00% ";

               // remark_PreBook = "PLEASE NOTE THAT A SEPARATE CITY TAX WILL BE CHARGED TO THE CUSTOMERS UPON ARRIVAL. THE CITY TAX IS NOT INCLUDED IN THE ROOM RATES";
                //Cxl
                List<decimal> Brkup = new List<decimal>(TOtalPrices);
                string arriveldate = MainpreBookReq.Descendants("FromDate").Single().Value;
                CXL = calculateCxlPolicy(arriveldate, remark_PreBook, PrePrice, Brkup,canceldate_PreBook,remark_roomavail);
                PreBookRoom.Descendants("RoomTypes").FirstOrDefault().Add(CXL);
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GetPreBook/calculateCxlPolicy";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = MainpreBookReq.Descendants("CustomerID").Single().Value;
                ex1.TranID = MainpreBookReq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }


            returnatexception:
            try
            {

                XElement res = new XElement(MainpreBookReq);
                XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
                var r = res.Element(namespc + "Body");

                r.Add(PreBookRoom);
                return res;
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GetPreBook3";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = MainpreBookReq.Descendants("CustomerID").Single().Value;
                ex1.TranID = MainpreBookReq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                PreBookRoom = new XElement("HotelPreBookingResponse", new XElement("ErrorTxt", "Room is not available"));
                return PreBookRoom;
            }








        }



        public XElement getPreBookFromDB(string transID, string RoomId, XElement MainpreBookReq, bool sameprice, decimal NewPrice, string dmc, string NewRemark, ref string roomremark)
        {
            
            XElement pricechange = new XElement("NewPrice", null);
            if(sameprice==false)
                pricechange = new XElement("NewPrice", NewPrice);

            GoGlobal_Static goglbStatic = new GoGlobal_Static();

            DataTable dt = goglbStatic.GetRoomLog_GoGlobal(transID);
            string RoomResponse = string.Empty;

            if (dt != null)
            {
                if (dt.Rows.Count != 0)
                {
                    RoomResponse = dt.Rows[0]["logresponseXML"].ToString();
                }
            }
            string Term = "";
            string Special = "";

            roomremark=Term = goglbStatic.GetRemarkFromRoomLog_GoGlobal(transID, RoomId, ref Special);

            if (NewRemark == null || NewRemark == "")
            Term = Term + "   ...   " + Special;
            else
                Term = NewRemark + "  ....   " + Special;

            XElement RoomAvail = XElement.Parse(RoomResponse);

            XDocument RoomAvail2 = XDocument.Parse(RoomResponse);


            // Get RoomAvail response from DB from TransID
             
            string sss = "";



            string HName =String.Empty;
            string HCode = String.Empty;

            string htlid = string.Empty;
            string htlname = string.Empty;
            try
            {
                htlid = MainpreBookReq.Descendants("HotelID").FirstOrDefault().Value;
                htlname = MainpreBookReq.Descendants("HotelName").FirstOrDefault().Value;
            }
            catch { }

            var h = RoomAvail.Descendants("Hotel").FirstOrDefault();

            string HotelID = HName;
            string HotelName = HCode;
            string Currency = "";

            var PreBook = (from ra in RoomAvail.Descendants("RoomTypes")
                           where ra.Element("Room").Attribute("ID").Value == RoomId
                           select ra).FirstOrDefault();

            if (PreBook!=null)
             Currency = PreBook.Attribute("CrncyCode").Value;

            sss = @"
     <HotelPreBookingResponse>
      " + pricechange + @"
      <Hotels>
        <Hotel>
          <HotelID>" + htlid + @"</HotelID>
          <HotelName>" + htlname + @"</HotelName>
          <Status>true</Status>
          <TermCondition>" + "" + @"</TermCondition>
          <HotelImgSmall />
          <HotelImgLarge />
          <MapLink />
          <DMC>" + dmc + @"</DMC>
          <Currency>" + Currency + @"</Currency>
          <Offers />
          <Rooms>
          " + PreBook + @"
          </Rooms>
        </Hotel>
      </Hotels>
    </HotelPreBookingResponse>";






            XElement PreBookXML = XElement.Parse(sss);
            try
            {
                string[] trmc = Regex.Split(Term, "STARTING");

                if (trmc.Count() > 0)
                {
                    PreBookXML.Descendants("TermCondition").FirstOrDefault().Value = trmc[0];
                }
                else
                {
                    PreBookXML.Descendants("TermCondition").FirstOrDefault().Value = Term;
                }
            }
            catch
            {
                PreBookXML.Descendants("TermCondition").FirstOrDefault().Value = Term;
            }
            return PreBookXML;




        }


        public bool chkPriceGoGlobal(decimal PrePrice, string code, string date, XElement MainpreBookReq, out string remark, ref decimal NewPrice, ref string CancellationDeadline)
        {
           



            decimal LastPrice = 0;
            bool sameprice = false;

            /*change by ravish 09102020      
                 <API-AgencyID>" + agency + @"</API-AgencyID>
                <API-Operation>BOOKING_VALUATION_REQUEST</API-Operation>
            */
            string chk_price_req = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
<soap12:Body>
<MakeRequest xmlns=""http://www.goglobal.travel/"">
<requestType>9</requestType>
<xmlRequest><![CDATA[
<Root>
			<Header>
			    <Agency>" + agency + @"</Agency>
		        <User>" + user + @"</User>
		        <Password>" + password + @"</Password>
			    <Operation>BOOKING_VALUATION_REQUEST</Operation>
			    <OperationType>Request</OperationType>
			</Header>
		    <Main Version=""2.0"">
                <HotelSearchCode>" + code + @"</HotelSearchCode>
                <ArrivalDate>" + date + @"</ArrivalDate>
            </Main>
</Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";

            XElement BOOKING_VALUATION = GoGlobalSupplierResponseXML(chk_price_req, MainpreBookReq, "PreBook", "BOOKING_VALUATION_REQUEST");

            if (BOOKING_VALUATION.Descendants("Error").Any())
            {
                remark = BOOKING_VALUATION.Descendants("Error").FirstOrDefault().Value;
                return false;

            }
            remark = BOOKING_VALUATION.Descendants("Remarks").FirstOrDefault().Value;




            if (BOOKING_VALUATION.Descendants("CancellationDeadline").Any())
            {
                CancellationDeadline = BOOKING_VALUATION.Descendants("CancellationDeadline").FirstOrDefault().Value;              

            }




            string Rates = BOOKING_VALUATION.Descendants("Rates").FirstOrDefault().Value;
            if (Rates != "")
            {
                string[] ax = Regex.Split(Rates, @"[^0-9\.]+");
                decimal.TryParse(string.Join("", ax), out LastPrice);
                NewPrice = LastPrice;
            }
            else
            {
                sameprice = true;
            }
            //string a = Rates;
            //string b = string.Empty;
            //decimal val;

            //for (int i = 0; i < a.Length; i++)
            //{
            //    if (Char.IsDigit(a[i]) || a[i].ToString() == ".")
            //        b += a[i];
            //}

            //if (b.Length > 0)
            //{
            //    val = decimal.Parse(b);

            //    if (val == PrePrice)
            //        sameprice = true;
            //}

            if (LastPrice == PrePrice)
                sameprice = true;


            return sameprice;




        }





        public XElement GetRoomBrekUpGoGlobal(XElement breakup, int roomcount, XElement mainreq)
        {

            //            string sss = @"<Root>
            //    <Header>
            //         <Agency>" + agency + @"</Agency>
            //		<User>" + user + @"</User>
            //		<Password>" + password + @"</Password>
            //        <Operation>PRICE_BREAKDOWN_RESPONSE</Operation>
            //        <OperationType>Response</OperationType>
            //    </Header>
            //    <Main>
            //        <HotelName>ww</HotelName>
            //        <Room>
            //            <RoomType><![CDATA[SINGLE STANDARD]]></RoomType>
            //            <Children>0</Children>
            //            <Cots>0</Cots>
            //            <PriceBreakdown>
            //                <FromDate>2019-11-20</FromDate>
            //                <ToDate>2019-11-21</ToDate>
            //                <Price>42.85</Price>
            //                <Currency>EUR</Currency>
            //            </PriceBreakdown>
            //            <PriceBreakdown>
            //                <FromDate>2019-11-21</FromDate>
            //                <ToDate>2019-11-23</ToDate>
            //                <Price>42.83</Price>
            //                <Currency>EUR</Currency>
            //            </PriceBreakdown>
            //        </Room>
            //        <Room>
            //            <RoomType><![CDATA[SINGLE STANDARD]]></RoomType>
            //            <Children>0</Children>
            //            <Cots>0</Cots>
            //            <PriceBreakdown>
            //                <FromDate>2019-11-20</FromDate>
            //                <ToDate>2019-11-23</ToDate>
            //                <Price>42.83</Price>
            //                <Currency>EUR</Currency>
            //            </PriceBreakdown>
            //        </Room>
            //    </Main>
            //</Root>";


            // breakup = XElement.Parse(sss);
            int nightno = 0;
            var ResponseBreakups = new XElement("PriceBreakups");

            if (breakup.Descendants("Room").Count() >= roomcount)
            {



                var Room = breakup.Descendants("Room").Skip(roomcount - 1).FirstOrDefault();

                var pcount = Room.Descendants("PriceBreakdown").Count();
                for (int p = 0; p < pcount; p++)
                {

                    var Prices = Room.Descendants("PriceBreakdown").Skip(p).FirstOrDefault();
                    try
                    {

                        DateTime fromDate = DateTime.ParseExact(Prices.Descendants("FromDate").Single().Value, "yyyy-MM-dd", null);
                        DateTime toDate = DateTime.ParseExact(Prices.Descendants("ToDate").Single().Value, "yyyy-MM-dd", null);
                        int nights = (int)(toDate - fromDate).TotalDays;
                        string pernighPrices = Prices.Descendants("Price").Single().Value;

                        for (int n = 0; n < nights; n++)
                        {
                            nightno++;
                            ResponseBreakups.Add(new XElement("Price", new XAttribute("Night", nightno), new XAttribute("PriceValue", pernighPrices)));

                        }
                    }
                    catch (Exception ex)
                    {

                        CustomException ex1 = new CustomException(ex);
                        ex1.MethodName = "GetRoomBrekUpGoGlobal";
                        ex1.PageName = "GoGlobal";
                        ex1.CustomerID = mainreq.Descendants("CustomerID").Single().Value;
                        ex1.TranID = mainreq.Descendants("TransID").Single().Value;
                        SaveAPILog saveex = new SaveAPILog();
                        saveex.SendCustomExcepToDB(ex1);

                    }
                }


            }
            else
            {
                ResponseBreakups = null;
            }





            return ResponseBreakups;


        }








        #endregion



        #region Cxl


        public XElement getCXLpolicy(XElement main_cxlreq)
        {
            //get these from main_cxlreq

            //string HotelID = main_cxlreq.Descendants("HotelID").FirstOrDefault().Value;
            XElement responsecxl = null;
            try
            {


                   string ArrivalDate = main_cxlreq.Descendants("FromDate").FirstOrDefault().Value;
                   int nights = 0;

                

                    DateTime fromDate = DateTime.ParseExact(ArrivalDate, "dd/MM/yyyy", null);
                    DateTime toDate = DateTime.ParseExact(main_cxlreq.Descendants("ToDate").Single().Value, "dd/MM/yyyy", null);
                    nights = (int)(toDate - fromDate).TotalDays;
                   







                string RoomID = main_cxlreq.Descendants("RoomID").FirstOrDefault().Value;
                string TransID = main_cxlreq.Descendants("TransID").FirstOrDefault().Value;

                string HName;
                string HCode;
                XElement CXL = get_goglobalCxlPolicy(main_cxlreq,nights, ArrivalDate, TransID, RoomID, out HName, out HCode);
                string cxlpolicies = @"<HotelDetailwithcancellationResponse>
      <Hotels>
        <Hotel>
          <HotelID>" + HCode + @"</HotelID>
          <HotelName>" + HName + @"</HotelName>
          <HotelImgSmall />
          <HotelImgLarge />
          <MapLink />
          <DMC>GoGlobal</DMC>
          <Currency />
          <Offers />
          <Rooms>
            <Room ID="""" RoomType="""" MealPlanPrice="""" PerNightRoomRate="""" TotalRoomRate="""" CancellationDate="""">
             "
                + CXL +
               @"
            </Room>
          </Rooms>
        </Hotel>
      </Hotels>
    </HotelDetailwithcancellationResponse>";

                responsecxl = XElement.Parse(cxlpolicies);

            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "getCXLpolicy/get_goglobalCxlPolicy";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_cxlreq.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_cxlreq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }



           


         


            //add responsecxl in main_cxlreq
            try
            {

                XElement res = new XElement(main_cxlreq);


                XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
                var r = res.Element(namespc + "Body");

                r.Add(responsecxl);
                return res;

            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "getCXLpolicy";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_cxlreq.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_cxlreq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                return null;
            }








          

        }

        public XElement get_goglobalCxlPolicy(XElement main_cxlreq, int TotalNights, string arrivaldate, string TransId, string RoomID, out  string HotelName, out  string HotelCode)
        {
            

            GoGlobal_Static goglbStatic = new GoGlobal_Static();

            DataTable dtcity = goglbStatic.GetSupRoomLog_GoGlobal(TransId);
            string SupRoomResponse = string.Empty;

            if (dtcity != null)
            {
                if (dtcity.Rows.Count != 0)
                {
                    SupRoomResponse = dtcity.Rows[0]["logresponseXML"].ToString();
                }
            }



            //get this response from DB via TransId
           

            

             XElement roomresponse_fromDB = XElement.Parse(SupRoomResponse);
             XElement rrr = (from x in roomresponse_fromDB.Descendants("HotelSearchCode")
                            where x.Value == RoomID
                            select x.Parent).FirstOrDefault();



             decimal Price = 0; 
             HotelName = roomresponse_fromDB.Descendants("HotelName").FirstOrDefault().Value;
             HotelCode = roomresponse_fromDB.Descendants("HotelCode").FirstOrDefault().Value;


            string TotalPrice = rrr.Descendants("TotalPrice").FirstOrDefault().Value;
            decimal.TryParse(TotalPrice, out Price);


            string rmk = rrr.Descendants("Remark").FirstOrDefault().Value;

            //&lt;BR /&gt; CXL charges apply as follows:  STARTING 07/11/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE.&lt;BR /&gt; CXL charges apply as follows:  STARTING 07/11/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE.
            //Should Split by BR

//            CXL charges apply as follows: STARTING 07/11/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE
//We have a regex that is used to identify the policies in the remarks section:
  //---------------------------------------------------------------------------------------------------------------
          
            //(?i)STARTING (?<startdate>\\d{1,2}/\\d{1,2}/\\d{4}) ((?:OR|AND) NO SHOW[ ]+)
    
  //---------------------------------------------------------------------------------------------------------------
     //?CXL-PENALTY FEE[ ]+IS (?:PRICE OF )?(?<value>\\d+(\\.\\d{1,2})?)
     //(?<apply>
    
     //(?<pct>%)[ ]+OF 
    
     //(
     //?<based>

     //(FIRST NIGHT|BOOKING)
            
     //PRICE|TOTAL
     //)                
                
     //| NIGHTS| \\w{3}
            
     // )
     //eg1.  30% of Booking Price       -->2. "pct" will simply hold "%" if exists indicating the value is percent and not a flat value.
     //eg2.  30% of  First Night        -->3. if present, check the "based" value for "FIRST NIGHT", otherwise it is based on the booking price.
     //eg3.  3 of Nights                -->4. if not, check "apply" for "NIGHTS" if the value is # of nights or ISO cur code if it is monetary.

  //-----------------------------------------------------------------------------------------------------------------------

//without any line breaks.

//Using the named groups you can get the following:
//1. "startdate" will hold the date where this policy starts in format dd/MM/yyyy (just in case it will match on single digit values).
//2. "pct" will simply hold "%" if exists indicating the value is percent and not a flat value.
//3. if present, check the "based" value for "FIRST NIGHT", otherwise it is based on the booking price.
//4. if not, check "apply" for "NIGHTS" if the value is # of nights or ISO cur code if it is monetary.
//5. "value" will hold a value of integer with possibly 1 or 2 decimal places.

//As per this pattern, all policy related info will start with the word "STARTING" and should include "CXL-PENALTY FEE".

//So you can have % of total/nights or number of nights.




             List<decimal> BrkUp=new List<decimal>();

             decimal PerNightPrice = Price / TotalNights;
             var ResponseBreakups = new XElement("PriceBreakups");

             for (int n = 1; n <= TotalNights; n++)
             {

                 BrkUp.Add(PerNightPrice);

             }

             XElement CXL = null;
             try
             {

                  CXL = calculateCxlPolicy(arrivaldate, rmk, Price, BrkUp);

             }
             catch (Exception ex)
             {
                 CustomException ex1 = new CustomException(ex);
                 ex1.MethodName = "get_goglobalCxlPolicy/calculateCxlPolicy";
                 ex1.PageName = "GoGlobal";
                 ex1.CustomerID = main_cxlreq.Descendants("CustomerID").Single().Value;
                 ex1.TranID = main_cxlreq.Descendants("TransID").Single().Value;
                 SaveAPILog saveex = new SaveAPILog();
                 saveex.SendCustomExcepToDB(ex1);
             }


             return CXL;

        }


        public XElement calculateCxlPolicy(string arrivaldate, string rmk, decimal Price, List<decimal> BrkUp, string canceldate_PreBook = null, string remark_roomavail = null)
        {

         

            //&lt;BR /&gt; CXL charges apply as follows:  STARTING 07/11/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE.&lt;BR /&gt; CXL charges apply as follows:  STARTING 07/11/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE.
            //Should Split by BR

            //            CXL charges apply as follows: STARTING 07/11/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE
            //We have a regex that is used to identify the policies in the remarks section:
            //---------------------------------------------------------------------------------------------------------------

            //(?i)STARTING (?<startdate>\\d{1,2}/\\d{1,2}/\\d{4}) ((?:OR|AND) NO SHOW[ ]+)

            //---------------------------------------------------------------------------------------------------------------
            //?CXL-PENALTY FEE[ ]+IS (?:PRICE OF )?(?<value>\\d+(\\.\\d{1,2})?)
            //(?<apply>

            //(?<pct>%)[ ]+OF 

            //(
            //?<based>

            //(FIRST NIGHT|BOOKING)

            //PRICE|TOTAL
            //)                

            //| NIGHTS| \\w{3}

            // )
            //eg1.  30% of Booking Price       -->2. "pct" will simply hold "%" if exists indicating the value is percent and not a flat value.
            //eg2.  30% of  First Night        -->3. if present, check the "based" value for "FIRST NIGHT", otherwise it is based on the booking price.
            //eg3.  3 of Nights                -->4. if not, check "apply" for "NIGHTS" if the value is # of nights or ISO cur code if it is monetary.

            //-----------------------------------------------------------------------------------------------------------------------

            //without any line breaks.

            //Using the named groups you can get the following:
            //1. "startdate" will hold the date where this policy starts in format dd/MM/yyyy (just in case it will match on single digit values).
            //2. "pct" will simply hold "%" if exists indicating the value is percent and not a flat value.
            //3. if present, check the "based" value for "FIRST NIGHT", otherwise it is based on the booking price.
            //4. if not, check "apply" for "NIGHTS" if the value is # of nights or ISO cur code if it is monetary.
            //5. "value" will hold a value of integer with possibly 1 or 2 decimal places.

            //As per this pattern, all policy related info will start with the word "STARTING" and should include "CXL-PENALTY FEE".

            //So you can have % of total/nights or number of nights.







            //" STARTING 07/11/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE"

           
                    //"CxlDeadLine": "25/Dec/2019",
                    //"NonRef": false,
                    
                   
                    //"Remark": " IN CASE OF AMENDMENTS FROM 04/11/2019/NAME CHANGES FROM 04/11/2019, RESERVATIONS HAVE TO BE CANCELLED AND REBOOKED SUBJECT TO AVAILABILITY AND RATES AT THE TIME OF REBOOKING<br />Car park NO. <br />Check-in hour 15:00 - . <br />STARTING 25/12/2019 CXL-PENALTY FEE IS 50.00% OF BOOKING PRICE, STARTING 31/12/2019 CXL-PENALTY FEE IS 100.00% OF BOOKING PRICE.<br />Continental Room and Breakfast",
                    //"Preferred": false

            //IN CASE OF AMENDMENTS FROM 02/12/2019/NAME CHANGES FROM 02/12/2019, RESERVATIONS HAVE TO BE CANCELLED AND REBOOKED SUBJECT TO AVAILABILITY AND RATES AT THE TIME OF REBOOKING&lt;br /&gt;null&lt;br /&gt;null&lt;br /&gt;Pay local Charge: 38.46 EUR&lt;br /&gt;Car park YES (without additional debit notes). &lt;br /&gt;STARTING 05/03/2020 CXL-PENALTY FEE IS 33.33% OF BOOKING PRICE.&lt;br /&gt;Continental Room and Breakfast
            var Cxl = new XElement("CancellationPolicies");
          
           int index = rmk.IndexOf("STARTING", 0);
           if (index >= 0)
           {
               rmk = rmk.Substring(index);
           }
           else
           {
               int _index = rmk.IndexOf("NO SHOW", 0);
               if (_index >= 0)
                   rmk = rmk.Substring(_index);

               index = _index;
           }
           if (index < 0)
           {

               if (canceldate_PreBook != null)
               {
                   string[] cxdt = canceldate_PreBook.Split('-');
                   int y = 0;
                   int m = 0;
                   int d = 0;

                   int.TryParse(cxdt[0], out y);
                   int.TryParse(cxdt[1], out m);
                   int.TryParse(cxdt[2], out d);
                   if (y > 0 && m > 0 && d > 0)
                   {
                       DateTime lastcxdate = new DateTime(y, m, d);

                       string currentdate = lastcxdate.ToString("dd/MM/yyyy");

                       Cxl.Add(new XElement("CancellationPolicy",
                          new XAttribute("LastCancellationDate", currentdate),
                          new XAttribute("ApplicableAmount", Price),
                             new XAttribute("NoShowPolicy", 0)
                          ));
                       currentdate = lastcxdate.AddDays(-1).ToString("dd/MM/yyyy");
                       Cxl.Add(new XElement("CancellationPolicy",
                          new XAttribute("LastCancellationDate", currentdate),
                          new XAttribute("ApplicableAmount", 0),
                             new XAttribute("NoShowPolicy", 0)
                          ));



                   }
                   else
                   {
                       string currentdate = DateTime.Now.ToString("dd/MM/yyyy");

                       Cxl.Add(new XElement("CancellationPolicy",
                          new XAttribute("LastCancellationDate", currentdate),
                          new XAttribute("ApplicableAmount", Price),
                             new XAttribute("NoShowPolicy", 0)
                          ));
                       currentdate = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                       Cxl.Add(new XElement("CancellationPolicy",
                          new XAttribute("LastCancellationDate", currentdate),
                          new XAttribute("ApplicableAmount", 0),
                             new XAttribute("NoShowPolicy", 0)
                          ));
                   }
               }
               else
               {
                   //2020-03-10

                   string currentdate = DateTime.Now.ToString("dd/MM/yyyy");

                   Cxl.Add(new XElement("CancellationPolicy",
                      new XAttribute("LastCancellationDate", currentdate),
                      new XAttribute("ApplicableAmount", Price),
                         new XAttribute("NoShowPolicy", 0)
                      ));
                   currentdate = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                   Cxl.Add(new XElement("CancellationPolicy",
                      new XAttribute("LastCancellationDate", currentdate),
                      new XAttribute("ApplicableAmount", 0),
                         new XAttribute("NoShowPolicy", 0)
                      ));

               }


               return Cxl;
           }
           //string[] words = rmk.Split(',');

           string[] words = Regex.Split(rmk, "STARTING");

           for (int k = 0; k < words.Count(); k++)
           {
               if (words[k].ToString().Trim() != "")
               {
                   words[k] = "STARTING " + words[k];
               }
           }

           string firstcxlDate = null;
           foreach (var word in words)
           {
               string cxldate ="";
               int index1 = word.IndexOf("STARTING", 0);
               if (index1 >= 0)
               {
                   int _index1 = word.IndexOf("NO SHOW", 0);
                   if (_index1 < 0)
                   {
                       cxldate = getBetween(word, "STARTING", "CXL-PENALTY");

                       decimal applicableAmount = find_CxlPrice(word, Price, BrkUp);

                       if (applicableAmount > 0)
                       {
                           Cxl.Add(new XElement("CancellationPolicy",
                           new XAttribute("LastCancellationDate", cxldate),
                           new XAttribute("ApplicableAmount", applicableAmount),
                              new XAttribute("NoShowPolicy", 0)
                           ));
                       }
                   }
                   else
                   {

                       cxldate = getBetween(word, "STARTING", "NO SHOW");

                       decimal applicableAmount = find_CxlPrice(word, Price, BrkUp);
                       if (applicableAmount > 0)
                       {
                           Cxl.Add(new XElement("CancellationPolicy",
                            new XAttribute("LastCancellationDate", cxldate),
                            new XAttribute("ApplicableAmount", applicableAmount),
                                     new XAttribute("NoShowPolicy", 1)
                                            ));
                       }
                   }

               }
               else
               {
                   int index2 = word.IndexOf("NO SHOW", 0);
                   if (index2 >= 0)
                   {
                       cxldate = arrivaldate;
                       decimal applicableAmount = find_CxlPrice(word, Price, BrkUp);
                       if (applicableAmount > 0)
                       {
                           Cxl.Add(new XElement("CancellationPolicy",
                      new XAttribute("LastCancellationDate", cxldate),
                      new XAttribute("ApplicableAmount", applicableAmount),
                         new XAttribute("NoShowPolicy", 1)
                      ));
                       }


                   }
               }

               if (string.IsNullOrEmpty(firstcxlDate))
                   firstcxlDate = cxldate;

           }

            

           if (firstcxlDate != null)
           {
               		

               try 
               {
                   var smalldate = Cxl.Descendants("CancellationPolicy").Min(x => DateTime.ParseExact(x.Attribute("LastCancellationDate").Value, "dd/MM/yyyy", null));
                   string smlestdt = Convert.ToDateTime(smalldate).ToString("dd/MM/yyyy");
                   DateTime CxlDeadLine = DateTime.ParseExact(smlestdt.Trim(), "dd/MM/yyyy", null);


                   DateTime FreeDate = CxlDeadLine.AddDays(-1);
                   Cxl.Add(new XElement("CancellationPolicy",
                             new XAttribute("LastCancellationDate", FreeDate.ToString("dd/MM/yyyy")),
                             new XAttribute("ApplicableAmount", 0),
                                new XAttribute("NoShowPolicy", 0)
                             ));
               }
               catch
               {

               }
              

           }





            return Cxl;

        }



        
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start).Trim();
            }
            else
            {
                return "";
            }
        }




        public decimal find_CxlPrice(string str, decimal Price, List<decimal> BrkUp)
        {
            decimal applicableAmount = 0;
            bool NT= str.Contains("NT");
            if (NT == false)
            {
                int index = str.IndexOf('%', 0);
                decimal val = findnumeric(str);
               
                if (index > 0)
                {
                    if (str.Contains("BOOKING PRICE") == true)
                    {
                        applicableAmount = Price * val / 100;
                    }
                    else if (str.Contains("FIRST NIGHT") == true)
                    {
                        applicableAmount = BrkUp[0] * val / 100;
                    }
                }
                else
                {
                    if (str.Contains("NIGHTS") == true)
                    {
                        if (val <= BrkUp.Count)
                        {
                            for (int i = 0; i < val; i++)
                            {
                                applicableAmount += BrkUp[i];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < val; i++)
                            {
                                applicableAmount += BrkUp[i];
                            }
                        }
                    }
                    else if (str.Contains(desiredCurrency) == true)
                    {
                        applicableAmount = val;
                    }
                }

            }
            else
            {
                int night=0;
                decimal val = findnumeric2(str, out  night);


                decimal avg_price = Price / BrkUp.Count;


                if (night < BrkUp.Count)
                    for (int i = 0; i < night; i++)
                    {
                        applicableAmount += avg_price * val / 100;
                    }

            }

            return applicableAmount;
        }


        public static decimal findnumeric(string str)
        {
          
            int index = str.IndexOf("IS", 0);
            if (index >= 0)
               str = str.Substring(index);
            //decimal Price = 0;
            //string[] ax = Regex.Split(str, @"[^0-9\.]+");
            //decimal.TryParse(string.Join("", ax), out Price);
            //return Price;

//Case1. % of Booking Price
//STARTING 25/12/2019 CXL-PENALTY FEE IS 50.00% OF BOOKING PRICE. 
//Case2. First Night Price
//STARTING 31/12/2019 CXL-PENALTY FEE IS 100.00% OF FIRST NIGHT PRICE.
//Case3.  Night Wise Price
//STARTING 31/12/2019 CXL-PENALTY FEE IS 3 OF NIGHTS.
//Case4. Fix Price
//STARTING 31/12/2019 CXL-PENALTY FEE IS 30 EUR.



//No Show Case1.
//NO SHOW CXL-PENALTY FEE IS 50.00% OF BOOKING PRICE. 
//No Show Case2.
//STARTING 25/12/2019 NO SHOW CXL-PENALTY FEE IS 50.00% OF BOOKING PRICE, STARTING 26/12/2019 NO SHOW CXL-PENALTY FEE IS 50.00% OF BOOKING PRICE.












            decimal val = 0;
            string[] words = str.Split(' ');
            bool startchkdigit = false;
            foreach (string word in words)
            {
                if (word != "")
                {
                    if (startchkdigit == true)
                    {

                        string a = word;
                        if (Char.IsDigit(a[0]))
                        {
                            string b = string.Empty;


                            for (int i = 0; i < a.Length; i++)
                            {
                                if (a[i].ToString() != "%")
                                    b += a[i];
                            }

                            if (b.Length > 0)
                            {
                                try
                                {
                                    val = decimal.Parse(b);
                                }
                                catch
                                {
                                    startchkdigit = false;
                                }
                                //break;
                            }
                        }



                    }
                    else if (word == "IS")
                    {


                        startchkdigit = true;

                    }
                }

            }


            return val;

        }
        public static decimal findnumeric2(string str,out int night)
        {
            night = 0;
            //remark_PreBook = "STARTING 07/01/2020 CXL-PENALTY FEE IS CNX FEE  - 1 NT X 100.00% ";
            string str1 = "";
            string str2 = "";
            int index = str.IndexOf("NT", 0);
            if (index >= 0)
            {

                int index2 = str.IndexOf("FEE", 0);
                str2 = str.Substring(index2);
                int index3 = str2.IndexOf("NT", 0);

                str1 = str2.Substring(index3);
               
            }

            decimal val = 0;
            string[] words = str1.Split(' ');
            
            foreach (string word in words)
            {
                if (word != "")
                {
                   
                        string a = word;
                        if (Char.IsDigit(a[0]))
                        {
                            string b = string.Empty;


                            for (int i = 0; i < a.Length; i++)
                            {
                                if (a[i].ToString() != "%")
                                    b += a[i];
                            }

                            if (b.Length > 0)
                                val = decimal.Parse(b);
                            break;

                        }
                                      
                }

            }
             words = str2.Split(' ');

            foreach (string word in words)
            {
                if (word != "")
                {

                    string a = word;
                    if (Char.IsDigit(a[0]))
                    {
                        string b = string.Empty;


                        for (int i = 0; i < a.Length; i++)
                        {
                            if (a[i].ToString() != "%")
                                b += a[i];
                        }

                        if (b.Length > 0)
                            night = int.Parse(b);
                        break;

                    }

                }

            }

            return val;

        }

        #endregion



        #region Hotel Detail


        public XElement HotelDetailGoGlobal(XElement main_HIreq)
        {

            string sss = "";
            string Description = "";


            string HotelImage = string.Empty; string HotelId = string.Empty;
            string goglobal_HIReq = convertHotelDetailMainReqInGoGlobal(main_HIreq, out HotelImage, out HotelId);
            XElement response_HI = GoGlobalSupplierResponseXML(goglobal_HIReq, main_HIreq, "HotelDetail", "HOTEL_INFO_REQUEST");









            if(response_HI.Descendants("Error").FirstOrDefault()!=null)
            {
                 string Error = response_HI.Descendants("DebugError").FirstOrDefault().Value;
                   sss = @"<hoteldescResponse>
      <ErrorTxt>"
       +Error +
      @"</ErrorTxt>
    </hoteldescResponse>";

                   goto iferror;
               
            }
            string Phone = response_HI.Descendants("Phone").FirstOrDefault().Value;
            string Fax = response_HI.Descendants("Fax").FirstOrDefault().Value;
             Description = response_HI.Descendants("Description").FirstOrDefault().Value;
            string HotelFacilities = response_HI.Descendants("HotelFacilities").FirstOrDefault().Value;
            var Cxl = new XElement("Facilities");
            string[] _HotelFacilities = HotelFacilities.Split(',');
            foreach (string f in _HotelFacilities)
            {
                if(f!="")
                Cxl.Add(new XElement("Facility", f.Trim()));


            }
            //<Images><Image Path="", Caption=""></Image></Images>

            var Images = new XElement("Images", new XElement("Image", new XAttribute("Path", HotelImage), new XAttribute("Caption", "")));
            foreach (XElement element in response_HI.Descendants("Pictures"))
            {

                foreach (XElement childEllement in element.Descendants())
                {
                    Images.Add(new XElement("Image", new XAttribute("Path", childEllement.Value), new XAttribute("Caption", "")));
                }
            }


            var ContactDetails = new XElement("ContactDetails", new XElement("Phone", Phone), new XElement("Fax", Fax));







            sss = @"<hoteldescResponse>
      <Hotels>
        <Hotel>
          <HotelID>" + HotelId + @"</HotelID>
          <Description>" + "" + @"</Description>" +
                    Images + 
                    Cxl + ContactDetails +
                    @"<CheckinTime />
          <CheckoutTime />
        </Hotel>
      </Hotels>
    </hoteldescResponse>";


            iferror:

            response_HI = XDocument.Parse(sss).Root;

            try
            {
                if (response_HI.Descendants("Description").Any())
                response_HI.Descendants("Description").FirstOrDefault().Value = Description;

                XElement res = new XElement(main_HIreq);
                XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
                var r = res.Element(namespc + "Body");

                r.Add(response_HI);
                return res;

            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "HotelDetailGoGlobal";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = response_HI.Descendants("CustomerID").FirstOrDefault().Value;
                ex1.TranID = response_HI.Descendants("TransID").FirstOrDefault().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);

                string Error = ex.Message;
                sss = @"<hoteldescResponse>
                         <ErrorTxt>"
                         + Error +
                           @"</ErrorTxt>
                          </hoteldescResponse>";


                XElement res = new XElement(main_HIreq);
                XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
                var r = res.Element(namespc + "Body");

                r.Add(response_HI);
                return res;






               
            }


          


        }




        public string convertHotelDetailMainReqInGoGlobal(XElement main_HIreq, out  string HotelImage, out  string HotelId)
        {


            HotelId = main_HIreq.Descendants("HotelID").FirstOrDefault().Value;
            string TransID = main_HIreq.Descendants("TransID").FirstOrDefault().Value;
            GoGlobal_Static goglbStatic = new GoGlobal_Static();
            string HotelSearchCode = goglbStatic.GetOfferCodeFromSearchLog_GoGlobal(TransID, HotelId, out HotelImage);
            
            //use HotelSearchCode
            /*change by ravish 09102020      
                 <API-AgencyID>" + agency + @"</API-AgencyID>
                 <API-Operation>HOTEL_INFO_REQUEST</API-Operation>
            */

          string  req = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
            <soap12:Body>
            <MakeRequest xmlns=""http://www.goglobal.travel/"">
            <requestType>6</requestType>
            <xmlRequest><![CDATA[
            <Root>
			         <Header>
			            <Agency>" + agency + @"</Agency>
		                <User>" + user + @"</User>
		                <Password>" + password + @"</Password>
			            <Operation>HOTEL_INFO_REQUEST</Operation>
			            <OperationType>Request</OperationType>
			         </Header>
			            <Main>
			            <HotelSearchCode>" + HotelSearchCode + @"</HotelSearchCode>  
			            </Main>
			            </Root>
            ]]></xmlRequest>
            </MakeRequest>
            </soap12:Body>
            </soap12:Envelope>";

          return req;



            // return reqxml;

        }



   




        #endregion




        #region Insert Book

        public XElement BookingResponse_GoGlobal(XElement main_Bookreq)
        {

            XElement MainBookResponse = null;

            try
            {
           
            //main_Bookreq = XElement.Parse(sss);
            


            XElement supplierBookResponse = null;  // Get Supplier Response
          
            string goGlobal_Bookreq = null;
            try
            {
               
              
                   



                 goGlobal_Bookreq = convert_mainBookReqInGoGlobalreq(main_Bookreq);
               
                if(goGlobal_Bookreq==null)
                {
                    CustomException ex1 = new CustomException();
                    ex1.MethodName = "BookingResponse_GoGlobal/convert_mainBookReqInGoGlobalreq/wrong request";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                    ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);


                    string Response = @"<HotelBookingResponse>
                                             <ErrorTxt>"
                                             + "wrong request" +
                                             @"</ErrorTxt>
                                            </HotelBookingResponse>";

                        XElement BookResponse = XElement.Parse(Response);
                         return BookResponse;
                }
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "BookingResponse_GoGlobal/convert_mainBookReqInGoGlobalreq";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);

                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(main_Bookreq.Descendants("CustomerID").Single().Value);
                log.TrackNumber = main_Bookreq.Descendants("TransactionID").Single().Value;
                log.LogTypeID = 5;
                log.LogType = "Book";
                log.SupplierID = SuupplierId;
                log.logrequestXML = main_Bookreq.ToString();
                log.logresponseXML = "";
                log.StartTime = DateTime.Now;
                log.EndTime = DateTime.Now;
                SaveAPILog savelogpro = new SaveAPILog();
                savelogpro.SaveAPILogs(log);
              



            }

            try
            {

                supplierBookResponse = GoGlobalSupplierResponseXML(goGlobal_Bookreq, main_Bookreq, "Book", "BOOKING_INSERT_REQUEST");

            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "BookingResponse_GoGlobal/GoGlobalSupplierResponseXML";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }


            try
            {
                 MainBookResponse = convert_GoGlobalResponseInMain(supplierBookResponse, main_Bookreq);

               
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "BookingResponse_GoGlobal/convert_GoGlobalResponseInMain";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }

            }
            catch (Exception ex)
            {

               


                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "BookingResponse_GoGlobal/convert_GoGlobalResponseInMain";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);

                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(main_Bookreq.Descendants("CustomerID").Single().Value);
                log.TrackNumber = main_Bookreq.Descendants("TransactionID").Single().Value;
                log.LogTypeID = 5;
                log.LogType = "Book";
                log.SupplierID = SuupplierId;
                log.logrequestXML = main_Bookreq.ToString();
                log.logresponseXML = "";
                log.StartTime = DateTime.Now;
                log.EndTime = DateTime.Now;
                SaveAPILog savelogpro = new SaveAPILog();
                savelogpro.SaveAPILogs(log);
              

            }

            return MainBookResponse;


          
        }


        public string convert_mainBookReqInGoGlobalreq(XElement main_Bookreq)
        {

            string sss = string.Empty;

            if (main_Bookreq.Descendants("Room").Count() > 4)
                return null;


            string HotelSearchCode = main_Bookreq.Descendants("HotelID").FirstOrDefault().Value;



            var Rms = new XElement("Rooms");
            string adultcountpre = string.Empty; string childagepre = string.Empty; int i = 0;
            XElement RoomType = null; int rid = 1;

            List<XElement> main = null;
            bool reqtype = true;
            bool childexist = false;
            if (main_Bookreq.Descendants("Child").Any())
            {
                main = (from m in main_Bookreq.Descendants("Room")
                       orderby m.Attribute("Adult").Value, m.Attribute("ChildAge").Value
                       select m).ToList();
                childexist = true;
            }
            else
            {
                main = (from m in main_Bookreq.Descendants("Room")
                        orderby m.Attribute("Adult").Value
                        select m).ToList();
            }
            foreach (XElement element in main)
            {
                string adultcount = string.Empty; string childage = string.Empty;
              
                int Cots = 0;
                int convertinadult = 0;
                if (i == 0)
                {

                    HotelSearchCode = element.Attribute("RoomTypeID").Value;

                    adultcountpre = adultcount = element.Attribute("Adult").Value;
                    if(childexist==true)
                    childagepre = childage = element.Attribute("ChildAge").Value;
                    RoomType = new XElement("RoomType", new XAttribute("Adults", adultcount));
                    XElement Room = new XElement("Room", new XAttribute("RoomID", rid));
                    Room.Add(returnPaxesInRoom2(element, out reqtype, ref Cots, ref convertinadult));
                    if (Cots > 0)
                        RoomType.Add(new XAttribute("CotCount", Cots));
                    if (convertinadult > 0)
                        RoomType.Attribute("Adults").Value = (Convert.ToInt16(adultcount) + convertinadult).ToString();
                    RoomType.Add(Room);
                    i++;
                }
                else
                {
                    adultcount = element.Attribute("Adult").Value;
                    if (childexist == true)
                    childage = element.Attribute("ChildAge").Value;

                    if (adultcountpre == adultcount && childagepre == childage)
                    {
                        rid++;
                        XElement Room = new XElement("Room", new XAttribute("RoomID", rid));
                        Room.Add(returnPaxesInRoom2(element, out reqtype, ref Cots, ref convertinadult));
                        if (Cots > 0)
                            RoomType.Add(new XAttribute("CotCount", Cots));
                        if (convertinadult > 0)
                            RoomType.Attribute("Adults").Value = (Convert.ToInt16(adultcount) + convertinadult).ToString();
                        RoomType.Add(Room);
                    }
                    else
                    {
                        rid = 1;
                        Rms.Add(RoomType);
                        RoomType = new XElement("RoomType", new XAttribute("Adults", adultcount));
                        XElement Room = new XElement("Room", new XAttribute("RoomID", rid));
                        Room.Add(returnPaxesInRoom2(element, out reqtype, ref Cots, ref convertinadult));
                        if (Cots > 0)
                            RoomType.Add(new XAttribute("CotCount", Cots));
                        if (convertinadult > 0)
                            RoomType.Attribute("Adults").Value = (Convert.ToInt16(adultcount) + convertinadult).ToString();
                        RoomType.Add(Room);

                        adultcountpre = adultcount;
                        childagepre = childage;



                    }

                }



                if (!reqtype)
                    return null;




                //foreach (XElement childEllement in element.Descendants())
                //{

                //}

            }//make room section

            Rms.Add(RoomType);


            string AgentReference = "Test AgRef";
      
            string ArrivalDate = main_Bookreq.Descendants("FromDate").FirstOrDefault().Value;

            string ToDate = main_Bookreq.Descendants("ToDate").FirstOrDefault().Value;


       


            int nights = 0;

            try
            {

                DateTime fromDate = DateTime.ParseExact(ArrivalDate, "dd/MM/yyyy", null);

                //string ToDate = "2/12/2019";
                //DateTime Dt = DateTime.Now;
                //DateTime.TryParse(ToDate, out Dt);


                DateTime toDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);


                nights = (int)(toDate - fromDate).TotalDays;
              
                string Day = fromDate.Day.ToString();
                if (fromDate.Day < 10)
                    Day = "0" + Day;

                string Month = fromDate.Month.ToString();
                if (fromDate.Month < 10)
                    Month = "0" + Month;

                ArrivalDate = fromDate.Year + "-" + Month + "-" + Day;




            }
            catch { }


            string remark = main_Bookreq.Descendants("SpecialRemarks").FirstOrDefault().Value;//SpecialRemarks


            string NoAlternativeHotel = "1"; //No Advise if not Book
            /*change by ravish 09102020      
                 <API-AgencyID>" + agency + @"</API-AgencyID>
                 <API-Operation>HOTEL_INSERT_REQUEST</API-Operation>
            */
          
                string supplierreq = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
            <soap12:Body>
            <MakeRequest xmlns=""http://www.goglobal.travel/"">
            <requestType>2</requestType>
            <xmlRequest><![CDATA[
            <Root>
            <Header>
 
	              <Agency>" + agency + @"</Agency>
		            <User>" + user + @"</User>
		            <Password>" + password + @"</Password>
		            <Operation>BOOKING_INSERT_REQUEST</Operation>
		            <OperationType>Request</OperationType>
            </Header>
             <Main Version=""2.2"">
            <AgentReference>" + AgentReference + @"</AgentReference>
            <HotelSearchCode>" + HotelSearchCode + @"</HotelSearchCode>
            <ArrivalDate>" + ArrivalDate + @"</ArrivalDate>
            <Nights>" + nights + @"</Nights>
            <NoAlternativeHotel>" + NoAlternativeHotel + @"</NoAlternativeHotel>
            <Leader LeaderPersonID=""1""/>"

                    + Rms +

    @"<Remark>" + remark + @"</Remark></Main>
</Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";

              
                //Add remarks ?????
                //Children are between ages 2 and 10 or between 0 to 18 when using version 2.2 and above ????
                //All characters (names) should be non-unicode only.???

                //XElement supplierreq = XElement.Parse(sss);
                return supplierreq;
           

        }



        public List<XElement> returnPaxesInRoom(XElement RoomInMainReq, out bool reqtype, ref int cots)
        {
            reqtype = true;
            List<XElement> PaxesReturn = new List<XElement>();

            var Paxes = RoomInMainReq;
            int total = Paxes.Descendants("GuestType").Count();
            int adultcount = Paxes.Descendants("GuestType").Where(a => a.Value == "Adult").Count();
            int childcount = Paxes.Descendants("GuestType").Where(a => a.Value == "Child").Count();
            var childtags =  Paxes.Descendants("PaxInfo").Where(a => a.Descendants("GuestType").FirstOrDefault().Value=="Child").ToList();


            int cotcount = childtags.Descendants("Age").Where(a =>   Convert.ToInt16(a.Value) <= CotAge).Count();
            if (
                     total > 5      //  Adult + child can,t greater than 5
                || childcount > 4   //   child can,t greater than 4
                || cotcount > 1     //   cot can,t greater than 1
                || (cotcount >= 1 && adultcount >= 2)  //   cot can be with only 1 or 2 adult
                || (cotcount >= 1 && childcount >= 1)      //   cot can,t be with any child(Extrabed)
                //  child has Extrabed Tag in Req but cot has no Tag in Req

                )
            {
                reqtype = false;


                return PaxesReturn;
            }


            var Paxess = from P in Paxes.Descendants("PaxInfo")
                         orderby P.Element("GuestType").Value, P.Element("IsLead").Value descending
                         select P;
            int personid = 1;
            foreach (XElement element in Paxess)
            {

                string GuestType = element.Element("GuestType").Value;
                string Title = (element.Element("Title").Value).ToUpper();

                if (Title == "MR" || Title == "MRS")
                    Title = Title + ".";
                //  only  MR. / MRS. / MISS / MS case Sensative




                string FirstName = element.Element("FirstName").Value.Trim();
                string MiddleName = element.Element("MiddleName").Value.Trim();
                string LastName = element.Element("LastName").Value.Trim();

                if (MiddleName != "")
                    FirstName = FirstName + " " + MiddleName;

                int Age = 0;
                int.TryParse(element.Element("Age").Value, out Age);

                if (GuestType == "Adult")
                {
                    XElement PersonName =
                        new XElement("PersonName",
                        new XAttribute("PersonID", personid),
                         new XAttribute("Title", Title),
                           new XAttribute("FirstName", FirstName.Trim()),
                              new XAttribute("LastName", LastName.Trim())
                        );
                    PaxesReturn.Add(PersonName);
                }
                else if (Age > CotAge)
                {
                    XElement ExtraBed =
                       new XElement("ExtraBed",
                       new XAttribute("PersonID", personid),
                          new XAttribute("FirstName", FirstName.Trim()),
                             new XAttribute("LastName", LastName.Trim()),
                              new XAttribute("ChildAge", Age)
                       );
                    PaxesReturn.Add(ExtraBed);
                }
                else
                {
                    cots++;
                }
                personid++;
            }


            return PaxesReturn;
        }

        public List<XElement> returnPaxesInRoom2(XElement RoomInMainReq, out bool reqtype, ref int cots, ref int convertinadult)
        {
            reqtype = true;
            List<XElement> PaxesReturn = new List<XElement>();

            var Paxes = RoomInMainReq;
            int total = Paxes.Descendants("GuestType").Count();
            int adultcount = Paxes.Descendants("GuestType").Where(a => a.Value == "Adult").Count();
            int childcount = Paxes.Descendants("GuestType").Where(a => a.Value == "Child").Count();
            var childtags = Paxes.Descendants("PaxInfo").Where(a => a.Descendants("GuestType").FirstOrDefault().Value == "Child").ToList();


            int cotcount = childtags.Descendants("Age").Where(a => Convert.ToInt16(a.Value) <= CotAge).Count();
            if (
                     total > 5      //  Adult + child can,t greater than 5
                || childcount > 4   //   child can,t greater than 4
                || cotcount > 1     //   cot can,t greater than 1
                || (cotcount >= 1 && adultcount >= 2)  //   cot can be with only 1 or 2 adult
                || (cotcount == 1 && childcount > cotcount)      //   cot can,t be with any child(Extrabed)
                //  child has Extrabed Tag in Req but cot has no Tag in Req

                )
            {
                reqtype = false;


                return PaxesReturn;
            }

            List<XElement> PaxesReturnAdult = new List<XElement>();
            List<XElement> PaxesReturnExbed = new List<XElement>();
            var Paxess = from P in Paxes.Descendants("PaxInfo")
                         orderby P.Element("GuestType").Value, P.Element("IsLead").Value descending
                         select P;
            int personid = 1;
            foreach (XElement element in Paxess)
            {

                string GuestType = element.Element("GuestType").Value;
                string Title = (element.Element("Title").Value).ToUpper();

                if (Title == "MR" || Title == "MRS")
                    Title = Title + ".";
                //  only  MR. / MRS. / MISS / MS case Sensative




                string FirstName = element.Element("FirstName").Value.Trim();
                string MiddleName = element.Element("MiddleName").Value.Trim();
                string LastName = element.Element("LastName").Value.Trim();

                if (MiddleName != "")
                    FirstName = FirstName + " " + MiddleName;

                int Age = 0;
                int.TryParse(element.Element("Age").Value, out Age);

                if (GuestType == "Adult")
                {
                    XElement PersonName =
                        new XElement("PersonName",
                        new XAttribute("PersonID", personid),
                         new XAttribute("Title", Title),
                           new XAttribute("FirstName", FirstName.Trim()),
                              new XAttribute("LastName", LastName.Trim())
                        );
                    PaxesReturnAdult.Add(PersonName);
                }
                else if (Age > childmaxage)
                {
                    //child is convert in Adult 
                    if (Title == "Master" || Title == "Sheikh")
                        Title = "MS.";
                    else if (Title == "MISS" || Title == "Sheikha")
                        Title = "MISS";
                    else
                        Title = "MS.";


                    XElement PersonName =
                         new XElement("PersonName",
                         new XAttribute("PersonID", personid),
                          new XAttribute("Title", Title),
                            new XAttribute("FirstName", FirstName.Trim()),
                               new XAttribute("LastName", LastName.Trim())
                         );
                    PaxesReturnAdult.Add(PersonName);
                    convertinadult++;
                }
                else if (Age > CotAge)
                {
                    XElement ExtraBed =
                       new XElement("ExtraBed",
                       new XAttribute("PersonID", personid),
                          new XAttribute("FirstName", FirstName.Trim()),
                             new XAttribute("LastName", LastName.Trim()),
                              new XAttribute("ChildAge", Age)
                       );
                    PaxesReturnExbed.Add(ExtraBed);
                }
                else
                {
                    cots++;
                }
                personid++;
            }
            PaxesReturn.AddRange(PaxesReturnAdult);

            PaxesReturnExbed = PaxesReturnExbed.OrderBy(a => a.Attribute("ChildAge").Value).ToList();
            PaxesReturn.AddRange(PaxesReturnExbed);
             personid = 1;
            foreach (XElement element in PaxesReturn)
            {

                element.Attribute("PersonID").Value = personid.ToString();
                personid++;
            }
            return PaxesReturn;
        }

        public XElement convert_GoGlobalResponseInMain(XElement supplierResponse, XElement main_Bookreq)
        {


            string remark = "";
            string SupplierReferenceNo = "";
            //supplierResponse = XElement.Parse(sss);
            XElement _main_Bookreq = main_Bookreq.Descendants("HotelBookingRequest").FirstOrDefault();

            string hotelid = _main_Bookreq.Element("HotelID").Value;
            string HotelName = _main_Bookreq.Element("HotelName").Value;
            string FromDate = _main_Bookreq.Element("FromDate").Value;
            string ToDate = _main_Bookreq.Element("ToDate").Value;
            int AdultNo = _main_Bookreq.Descendants("Adult").Count();
            int ChildNo = _main_Bookreq.Descendants("Child").Count();
            string TransID = _main_Bookreq.Element("TransID").Value;
            string mainBookResponse = "";
            bool errfound = false;
            foreach (XElement element in supplierResponse.Descendants())
            {
                if (element.Name == "Error")
                {
                    errfound = true;
                    break;
                }
            }






            if (errfound == false)
            {


                string TotalPrice = supplierResponse.Descendants("TotalPrice").FirstOrDefault().Value;
                string Currency = supplierResponse.Descendants("Currency").FirstOrDefault().Value;
                string VoucherRemark = supplierResponse.Descendants("Remark").FirstOrDefault().Value; ;
                string ConfirmationNumber = supplierResponse.Descendants("GoBookingCode").FirstOrDefault().Value;

                string Status = String.Empty;
                string BookingStatus = supplierResponse.Descendants("BookingStatus").FirstOrDefault().Value;
                //StatusCode	Status Name
                //     RQ	Requested
                //     C	Confirmed
                //     RX	Req. Cancellation
                //     X	    Cancelled
                //     RJ	Rejected
                //     VCH	Voucher Issued
                //     VRQ	Voucher Req.


                if (BookingStatus == "RJ")
                {
                    mainBookResponse = @"
                                           <HotelBookingResponse>
                                            <ErrorTxt>
                                           " + "Reject From Supplier" + @"
                                            </ErrorTxt>
                                            </HotelBookingResponse>";
                    goto returnresponse;
                }
                else if (BookingStatus == "RQ") 
                {
                    try
                    {

                        Status = chkBookingStatus(ConfirmationNumber, TotalPrice, main_Bookreq);
                    }
                    catch (Exception ex)
                    {
                        CustomException ex1 = new CustomException(ex);
                        ex1.MethodName = "convert_GoGlobalResponseInMain/chkBookingStatus";
                        ex1.PageName = "GoGlobal";
                        ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                        ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                        SaveAPILog saveex = new SaveAPILog();
                        saveex.SendCustomExcepToDB(ex1);
                    }
                }
                else if (BookingStatus == "C")
                {
                    Status = "C";
                }
                    string Voucherstatus = "";
                    if (Status == "C")// move to Voucher process after confirm
                    {
                        #region voucher process

                        try
                        {    //get voucher detail
                            SupplierReferenceNo = VoucherBookingGoGlobal(ConfirmationNumber, main_Bookreq, ref remark);
                            if (SupplierReferenceNo == "")
                            {
                                //                                mainBookResponse = @"
                                //                                            <HotelBookingResponse>
                                //                                               <ErrorTxt>
                                //                                               " + "Supplier referece no blank" + @"
                                //                                                </ErrorTxt>
                                //                                              </HotelBookingResponse>";
                                //                                goto returnresponse;
                                #region if error in voucher status (said by supplier)
                                var gsr = main_Bookreq.Descendants("Room").ToList();
                                XElement guestsr = convert_GuestDetail(gsr);
                                mainBookResponse = @"
                                     <HotelBookingResponse>
                                      <Hotels>
                                      <HotelID>" + hotelid + @"</HotelID>
                                         <HotelName>" + HotelName + @"</HotelName>
                                      <FromDate>" + FromDate + @"</FromDate>
                                          <ToDate>" + ToDate + @"</ToDate>
                                              <AdultPax>" + AdultNo + @"</AdultPax>
                                      <ChildPax>" + ChildNo + @"</ChildPax>
                                      <TotalPrice>" + TotalPrice + @"</TotalPrice>
                                             <CurrencyID>" + Currency + @"</CurrencyID>
                                             <CurrencyCode>" + Currency + @"</CurrencyCode>
                                            <MarketID />
                                           <MarketName />
                                         <HotelImgSmall />
                                        <HotelImgLarge />
                                         <MapLink />
                                     <VoucherRemark>" + "please contact admin for voucher's remarks." + @"</VoucherRemark>
                                           <TransID>" + TransID + @"</TransID>
                                        <ConfirmationNumber>" + ConfirmationNumber + @"</ConfirmationNumber>
                                             <Status>" + "Success" + @"</Status>
                                                 <PassengerDetail>"
                                                      + guestsr +
                                                      @"</PassengerDetail>
                                              </Hotels>
                                             </HotelBookingResponse>";
                                goto returnresponse;
                                #endregion
                            }

                            //chk voucher status
                            try
                            {
                                Voucherstatus = chkBookingStatus_ForVoucher(ConfirmationNumber, main_Bookreq);
                            }
                            catch (Exception ex)
                            {
                                CustomException ex1 = new CustomException(ex);
                                ex1.MethodName = "convert_GoGlobalResponseInMain/chkBookingStatus_ForVoucher";
                                ex1.PageName = "GoGlobal";
                                ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                                ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                                SaveAPILog saveex = new SaveAPILog();
                                saveex.SendCustomExcepToDB(ex1);

                                #region if error in voucher status (said by supplier)
                                var gsr = main_Bookreq.Descendants("Room").ToList();
                                XElement guestsr = convert_GuestDetail(gsr);
                                mainBookResponse = @"
                                     <HotelBookingResponse>
                                      <Hotels>
                                      <HotelID>" + hotelid + @"</HotelID>
                                         <HotelName>" + HotelName + @"</HotelName>
                                      <FromDate>" + FromDate + @"</FromDate>
                                          <ToDate>" + ToDate + @"</ToDate>
                                              <AdultPax>" + AdultNo + @"</AdultPax>
                                      <ChildPax>" + ChildNo + @"</ChildPax>
                                      <TotalPrice>" + TotalPrice + @"</TotalPrice>
                                             <CurrencyID>" + Currency + @"</CurrencyID>
                                             <CurrencyCode>" + Currency + @"</CurrencyCode>
                                            <MarketID />
                                           <MarketName />
                                         <HotelImgSmall />
                                        <HotelImgLarge />
                                         <MapLink />
                                     <VoucherRemark>" + "please contact admin for voucher's remarks." + @"</VoucherRemark>
                                           <TransID>" + TransID + @"</TransID>
                                        <ConfirmationNumber>" + ConfirmationNumber + @"</ConfirmationNumber>
                                             <Status>" + "Success" + @"</Status>
                                                 <PassengerDetail>"
                                                      + guestsr +
                                                      @"</PassengerDetail>
                                              </Hotels>
                                             </HotelBookingResponse>";
                                goto returnresponse;
                                #endregion
                            }

                        }
                        catch (Exception ex)
                        {
                            CustomException ex1 = new CustomException(ex);
                            ex1.MethodName = "convert_GoGlobalResponseInMain/VoucherBookingGoGlobal";
                            ex1.PageName = "GoGlobal";
                            ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                            ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                            SaveAPILog saveex = new SaveAPILog();
                            saveex.SendCustomExcepToDB(ex1);

                            #region if error in voucher status (said by supplier)
                            var gsr = main_Bookreq.Descendants("Room").ToList();
                            XElement guestsr = convert_GuestDetail(gsr);
                            mainBookResponse = @"
                                     <HotelBookingResponse>
                                      <Hotels>
                                      <HotelID>" + hotelid + @"</HotelID>
                                         <HotelName>" + HotelName + @"</HotelName>
                                      <FromDate>" + FromDate + @"</FromDate>
                                          <ToDate>" + ToDate + @"</ToDate>
                                              <AdultPax>" + AdultNo + @"</AdultPax>
                                      <ChildPax>" + ChildNo + @"</ChildPax>
                                      <TotalPrice>" + TotalPrice + @"</TotalPrice>
                                             <CurrencyID>" + Currency + @"</CurrencyID>
                                             <CurrencyCode>" + Currency + @"</CurrencyCode>
                                            <MarketID />
                                           <MarketName />
                                         <HotelImgSmall />
                                        <HotelImgLarge />
                                         <MapLink />
                                     <VoucherRemark>" + "" + @"</VoucherRemark>
                                           <TransID>" + TransID + @"</TransID>
                                        <ConfirmationNumber>" + ConfirmationNumber + @"</ConfirmationNumber>
                                             <Status>" + "Success" + @"</Status>
                                                 <PassengerDetail>"
                                                  + guestsr +
                                                  @"</PassengerDetail>
                                              </Hotels>
                                             </HotelBookingResponse>";
                            goto returnresponse;
                            #endregion

                        }
                        if (Voucherstatus.ToUpper() == "VRQ")  // chk status if voucher on request
                        {


                            if (Voucherstatus.ToUpper() == "VRQ")// voucher  remain on request so send for cancel
                            {

                                Status = "RQ";
                            }
                        }
                        if (Voucherstatus.ToUpper() == "VCH")  // make response after Voucher Success
                        {
                            if (SupplierReferenceNo != null && SupplierReferenceNo != "")
                            {
                                var gs = main_Bookreq.Descendants("Room").ToList();

                                XElement guests = convert_GuestDetail(gs);
                                // remark = supplierResponse.Descendants("Remark").FirstOrDefault().Value;
                                string NumberShowOnVoucher = ConfirmationNumber + "_" + SupplierReferenceNo;// also will use in booking cancel request
                                mainBookResponse = @"
                                     <HotelBookingResponse>
                                      <Hotels>
                                      <HotelID>" + hotelid + @"</HotelID>
                                         <HotelName>" + HotelName + @"</HotelName>
                                      <FromDate>" + FromDate + @"</FromDate>
                                          <ToDate>" + ToDate + @"</ToDate>
                                              <AdultPax>" + AdultNo + @"</AdultPax>
                                      <ChildPax>" + ChildNo + @"</ChildPax>
                                      <TotalPrice>" + TotalPrice + @"</TotalPrice>
                                             <CurrencyID>" + Currency + @"</CurrencyID>
                                             <CurrencyCode>" + Currency + @"</CurrencyCode>
                                            <MarketID />
                                           <MarketName />
                                         <HotelImgSmall />
                                        <HotelImgLarge />
                                         <MapLink />
                                     <VoucherRemark>" + "" + @"</VoucherRemark>
                                           <TransID>" + TransID + @"</TransID>
                                        <ConfirmationNumber>" + NumberShowOnVoucher + @"</ConfirmationNumber>
                                             <Status>" + "Success" + @"</Status>
                                                 <PassengerDetail>"
                                                      + guests +
                                                      @"</PassengerDetail>
                                              </Hotels>
                                             </HotelBookingResponse>";
                                goto returnresponse;
                            }
                            else
                            {
                                mainBookResponse = @"
                                            <HotelBookingResponse>
                                               <ErrorTxt>
                                               " + "Supplier referece no blank" + @"
                                                </ErrorTxt>
                                              </HotelBookingResponse>";
                                goto returnresponse;
                            }
                        }
                        #endregion

                    }
                    if (Status == "RQ") // Remain OnRequest Even after check Status So Cancel it Now
                    {

                        #region Cancel Process // if not confirm and voucher
                        string Cxstatus = "";
                        try
                        {
                             Cxstatus = CancelBookingGoGlobal(ConfirmationNumber, main_Bookreq,"CancelInBook");
                        }
                        catch (Exception ex)
                        {
                            CustomException ex1 = new CustomException(ex);
                            ex1.MethodName = "convert_GoGlobalResponseInMain/CancelBookingGoGlobal-" + Voucherstatus;
                            ex1.PageName = "GoGlobal";
                            ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                            ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                            SaveAPILog saveex = new SaveAPILog();
                            saveex.SendCustomExcepToDB(ex1);
                        }
                        if(Cxstatus.ToUpper()=="X")  // Cancel from Supplier
                        {
                            mainBookResponse = @" <HotelBookingResponse>
                                                 <ErrorTxt>
                                                 " + "Cancel After OnRequest-" + Voucherstatus +@"
                                                 </ErrorTxt>
                                                </HotelBookingResponse>";
                            goto returnresponse;
                        }
                        else if (Cxstatus.ToUpper() == "RX") // Requested for Cancel from Supplier
                        {
                            try
                            {
                                Cxstatus = chkBookingStatus_ForCancel(ConfirmationNumber, main_Bookreq);
                            }
                            catch (Exception ex)
                            {
                                CustomException ex1 = new CustomException(ex);
                                ex1.MethodName = "convert_GoGlobalResponseInMain/chkBookingStatus_ForCancel-" + Voucherstatus;
                                ex1.PageName = "GoGlobal";
                                ex1.CustomerID = main_Bookreq.Descendants("CustomerID").Single().Value;
                                ex1.TranID = main_Bookreq.Descendants("TransactionID").Single().Value;
                                SaveAPILog saveex = new SaveAPILog();
                                saveex.SendCustomExcepToDB(ex1);
                            }
                            if (Cxstatus.ToUpper() == "X")  // Cancel from Supplier
                            {
                                mainBookResponse = @" <HotelBookingResponse>
                                                 <ErrorTxt>
                                                 " + "Cancel After OnRequest-" + Voucherstatus+@"
                                                 </ErrorTxt>
                                                </HotelBookingResponse>";
                                goto returnresponse;
                            }
                            else
                            {
                                mainBookResponse = @" <HotelBookingResponse>
                                                 <ErrorTxt>
                                                 " + "Can,t Cancel After OnRequest-" + Voucherstatus+@"
                                                 </ErrorTxt>
                                                </HotelBookingResponse>";
                                goto returnresponse;
                            }

                        }
                        else
                        {
                            mainBookResponse = @"
                                           <HotelBookingResponse>
                                            <ErrorTxt>
                                           " + Cxstatus + @"
                                            </ErrorTxt>
                                            </HotelBookingResponse>";
                            goto returnresponse;
                        }
                        #endregion


                    }
                    else if (Status == "RJ") // Reject at Supplier End
                    {
                        mainBookResponse = @"
                                           <HotelBookingResponse>
                                            <ErrorTxt>
                                           " + "Reject From Supplier" + @"
                                            </ErrorTxt>
                                            </HotelBookingResponse>";
                        goto returnresponse;
                    }
                    else  // No Reject , No Confirm , No Cancel so take it as Fail
                    {
                        mainBookResponse = @"
                                            <HotelBookingResponse>
                                            <ErrorTxt>
                                            " + "Fail" + @"
                                            </ErrorTxt>
                                            </HotelBookingResponse>";
                        goto returnresponse;
                    }
              

              
            }
            else
            {
//                 <Main>
//    <Error code="303"><![CDATA[Cannot parse Arrival Date!]]></Error>
//    <DebugError incident="303" timestamp="2019-11-06 11:04:41"><![CDATA[The following process did not complete in an orderly manner - BOOKING_INSERT_REQUEST
//The following may help resolve the issue: Search criteria missmatch(ArrivalDate,nights). Expected ArrivalDate:2019-12-16 but found: 2019-12-16, Expected nights: 4 but found: 1. No booking has been made.
//If you cannot correct this problem yourself, please forward this message to GoGlobal Technical support at xml@goglobal.travel]]></DebugError>
//  </Main>



                string error = supplierResponse.Descendants("Error").FirstOrDefault().Value + "===>";
                error += supplierResponse.Descendants("DebugError").FirstOrDefault().Value; ;
                mainBookResponse = @"
      <HotelBookingResponse>
      <ErrorTxt>
       " + error + @"
      </ErrorTxt>
    </HotelBookingResponse>";
            }


            returnresponse:
            XElement _mainBookResponse = XElement.Parse(mainBookResponse);
            
           
            try
            {
                if (_mainBookResponse.Descendants("VoucherRemark").Any())
                _mainBookResponse.Descendants("VoucherRemark").FirstOrDefault().Value = remark;


                XElement res = new XElement(main_Bookreq);
                XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
                var r = res.Element(namespc + "Body");

                r.Add(_mainBookResponse);
                return res;
            }
            catch
            {
                return null;
            }




        }



        public XElement convert_GuestDetail(List<XElement> guests)
        {

            XElement Guests = new XElement("GuestDetails");

            foreach (var g in guests)
            {
                var _g = g.Descendants("PaxInfo").FirstOrDefault();

                XElement guest = new XElement
                    ("Room"
                    , new XAttribute("ID", g.Attribute("RoomTypeID").Value)
                     , new XAttribute("RoomType", g.Attribute("RoomType").Value)
                        , new XAttribute("ServiceID", "")
                     , new XAttribute("MealPlanID", g.Attribute("MealPlanID").Value)
                        , new XAttribute("MealPlanName", g.Attribute("MealPlanID").Value)
                     , new XAttribute("MealPlanCode", g.Attribute("MealPlanID").Value)
                        , new XAttribute("MealPlanPrice", g.Attribute("MealPlanPrice").Value)
                     , new XAttribute("PerNightRoomRate", "")
                       , new XAttribute("RoomStatus", "true")
                          , new XAttribute("TotalRoomRate", g.Attribute("TotalRoomRate").Value),


                          new XElement
                              ("RoomGuest",
                                 new XElement("GuestType", _g.Element("GuestType").Value),
                                    new XElement("Title", _g.Element("Title").Value),
                                       new XElement("FirstName", _g.Element("FirstName").Value.Trim()),
                                          new XElement("MiddleName", _g.Element("MiddleName").Value.Trim()),
                                             new XElement("LastName", _g.Element("LastName").Value.Trim()),
                                                new XElement("IsLead", _g.Element("IsLead").Value),
                                                   new XElement("Age", _g.Element("Age").Value)
                              ),

                              new XElement
                              ("Supplements")





                    );


                Guests.Add(guest);


            }


            return Guests;

        }


        public string chkBookingStatus(string GoGlobalCode, string TPrice,XElement mainReq)
        {
            /*change by ravish 09102020      
                 <API-AgencyID>" + agency + @"</API-AgencyID>
                <API-Operation>BOOKING_STATUS_REQUEST</API-Operation>
            */
            string sss = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
<soap12:Body>
<MakeRequest xmlns=""http://www.goglobal.travel/"">
<requestType>5</requestType>
<xmlRequest><![CDATA[
<Root>
<Header>

       <Agency>" + agency + @"</Agency>
		<User>" + user + @"</User>
		<Password>" + password + @"</Password>
		<Operation>BOOKING_STATUS_REQUEST</Operation>
		<OperationType>Request</OperationType>
</Header>
<Main>
<GoBookingCode>" + GoGlobalCode + @"</GoBookingCode>

</Main>
</Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";


            XElement response_chkStatus = null;// GoGlobalSupplierResponseXML(sss);
            response_chkStatus = GoGlobalSupplierResponseXML(sss, mainReq, "chkStatus", "BOOKING_STATUS_REQUEST");

         



          //  response_chkStatus = XElement.Parse(sss);
            response_chkStatus = response_chkStatus.Descendants("GoBookingCode").FirstOrDefault();

            string Status = response_chkStatus.Attribute("Status").Value;
            string Price = response_chkStatus.Attribute("TotalPrice").Value;
            string Code = response_chkStatus.Value;
            //StatusCode	Status Name
            //     RQ	Requested
            //     C	Confirmed
            //     RX	Req. Cancellation
            //     X	    Cancelled
            //     RJ	Rejected
            //     VCH	Voucher Issued
            //     VRQ	Voucher Req.

            string Mess = Status;
            //if (Status == "C" )
            //{
            //    // if(Code==GoGlobalCode  &&  Price==TPrice)  ?? should chk or not ???????
            //    Mess = "Sucess";

            //}
            //else
            //{
            //    Mess = Status;
            //}


            return Mess;
        }
        public string chkBookingStatus_ForVoucher(string GoGlobalCode, XElement mainReq)
        {
            /*change by ravish 09102020      
                 <API-AgencyID>" + agency + @"</API-AgencyID>
                 <API-Operation>HOTEL_STATUS_REQUEST</API-Operation>
            */
            string sss = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
<soap12:Body>
<MakeRequest xmlns=""http://www.goglobal.travel/"">
<requestType>5</requestType>
<xmlRequest><![CDATA[
                <Root>
                <Header>
                       <Agency>" + agency + @"</Agency>
		                <User>" + user + @"</User>
		                <Password>" + password + @"</Password>
		                <Operation>BOOKING_STATUS_REQUEST</Operation>
		                <OperationType>Request</OperationType>
                </Header>
                <Main>
                <GoBookingCode>" + GoGlobalCode + @"</GoBookingCode>

                </Main>
                </Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";


            XElement response_chkStatus = null;// GoGlobalSupplierResponseXML(sss);
            response_chkStatus = GoGlobalSupplierResponseXML(sss, mainReq, "chkStatusForVoucher", "BOOKING_STATUS_REQUEST");





            //  response_chkStatus = XElement.Parse(sss);
            response_chkStatus = response_chkStatus.Descendants("GoBookingCode").FirstOrDefault();

            string Status = response_chkStatus.Attribute("Status").Value;
           // string Price = response_chkStatus.Attribute("TotalPrice").Value;
           // string Code = response_chkStatus.Value;
            //StatusCode	Status Name
            //     RQ	Requested
            //     C	Confirmed
            //     RX	Req. Cancellation
            //     X	    Cancelled
            //     RJ	Rejected
            //     VCH	Voucher Issued
            //     VRQ	Voucher Req.

            string Mess = Status;
            //if (Status == "C" || Status == "VCH")
            //{
            //    // if(Code==GoGlobalCode  &&  Price==TPrice)  ?? should chk or not ???????
            //    Mess = "Sucess";

            //}


            return Mess;
        }
        public string chkBookingStatus_ForCancel(string GoGlobalCode, XElement mainReq)
        {
            /*change by ravish 09102020      
                 <API-AgencyID>" + agency + @"</API-AgencyID>
                 <API-Operation>HOTEL_STATUS_REQUEST</API-Operation>
            */
            string sss = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
<soap12:Body>
<MakeRequest xmlns=""http://www.goglobal.travel/"">
<requestType>5</requestType>
<xmlRequest><![CDATA[
                    <Root>
                    <Header>
                            <Agency>" + agency + @"</Agency>
		                    <User>" + user + @"</User>
		                    <Password>" + password + @"</Password>
		                    <Operation>BOOKING_STATUS_REQUEST</Operation>
		                    <OperationType>Request</OperationType>
                    </Header>
                    <Main>
                    <GoBookingCode>" + GoGlobalCode + @"</GoBookingCode>

                    </Main>
                    </Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";


            XElement response_chkStatus = null;// GoGlobalSupplierResponseXML(sss);
            response_chkStatus = GoGlobalSupplierResponseXML(sss, mainReq, "chkStatusForCancel", "BOOKING_STATUS_REQUEST");





            //  response_chkStatus = XElement.Parse(sss);
            response_chkStatus = response_chkStatus.Descendants("GoBookingCode").FirstOrDefault();

            string Status = response_chkStatus.Attribute("Status").Value;
           // string Price = response_chkStatus.Attribute("TotalPrice").Value;
           // string Code = response_chkStatus.Value;
            //StatusCode	Status Name
            //     RQ	Requested
            //     C	Confirmed
            //     RX	Req. Cancellation
            //     X	    Cancelled
            //     RJ	Rejected
            //     VCH	Voucher Issued
            //     VRQ	Voucher Req.

            string Mess = Status;
          

            return Mess;
        }

        private string VoucherBookingGoGlobal(string code, XElement reqmain,ref  string TotalRemark)
        {

            /*change by ravish 09102020      
                 <API-AgencyID>" + agency + @"</API-AgencyID>
                 <API-Operation>VOUCHER_DETAILS_REQUEST</API-Operation>
            */


            string bookingvoucher = @"<?xml version=""1.0"" encoding=""utf-8""?>
        <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
        <soap12:Body>
        <MakeRequest xmlns=""http://www.goglobal.travel/"">
        <requestType>8</requestType>
        <xmlRequest><![CDATA[
        <Root>
			        <Header>
	                    <Agency>" + agency + @"</Agency>
		                <User>" + user + @"</User>
		                <Password>" + password + @"</Password>
			            <Operation>VOUCHER_DETAILS_REQUEST</Operation>
			            <OperationType>Request</OperationType>
			        </Header>
		            <Main Version=""2.0"">
                            <GoBookingCode>" + code + @"</GoBookingCode>
                            <GetEmergencyPhone>true</GetEmergencyPhone>
                    </Main>
        </Root>
        ]]></xmlRequest>
        </MakeRequest>
        </soap12:Body>
        </soap12:Envelope>";

            XElement BOOKING_Voucher = GoGlobalSupplierResponseXML(bookingvoucher, reqmain, "Voucher", "VOUCHER_DETAILS_REQUEST");



//            <Main>
//<GoBookingCode>62345</GoBookingCode>
//<HotelName>ASTORIA</HotelName>
//<Address>VIALE BARI 11</Address>
//<Phone>39-080-4323320</Phone>
//<Fax>39-080-4321290</Fax>
//<CheckInDate>03/Mar/11</CheckInDate>
//<RoomBasis>BB</RoomBasis>
//<Nights>3</Nights>
//<Rooms>Room for 1 Adult + Children / DELUXE NON-SMOKING : JOHN DOE MR., JAYNE DOE MRS., JULIANNE DOE CHD.; 1 Single / STANDARD : JACK DOE</Rooms>
//<Remarks>If possible, provide non-smoking room</Remarks>
//<BookingRemarks type="Agent">
//<Remark>
//<![CDATA[If possible, provide non-smoking room]]>
//</Remark>
//</BookingRemarks>
//<BookedAndPayableBy>Supplier</BookedAndPayableBy>
//<SupplierReferenceNumber>AB12345</SupplierReferenceNumber>
//<EmergencyPhone>44 54 7791234</EmergencyPhone>
//</Main>
//</Root>

            string supplierreferenceNO = "";
            bool errfound = false;
            foreach (XElement element in BOOKING_Voucher.Descendants())
            {
                if (element.Name == "Error")
                {
                    errfound = true;
                    break;
                }
            }


            if(errfound==true)
            {

                TotalRemark = "";
                return supplierreferenceNO;

            }
           
            if (BOOKING_Voucher.Descendants("SupplierReferenceNumber").FirstOrDefault() != null)
                supplierreferenceNO = BOOKING_Voucher.Descendants("SupplierReferenceNumber").FirstOrDefault().Value;

            string Remarks = "";
            if (BOOKING_Voucher.Descendants("Remarks").FirstOrDefault() != null)
                Remarks = BOOKING_Voucher.Descendants("Remarks").FirstOrDefault().Value;

            var rmain = (from m in BOOKING_Voucher.Descendants("Remark")
                         select m.Value).ToList();
            string Remark = "";

            foreach (var r in rmain)
            {
                Remark += r + "...";
            }




            string BookedAndPayableBy = "";
            if (BOOKING_Voucher.Descendants("BookedAndPayableBy").FirstOrDefault() != null)
                BookedAndPayableBy = BOOKING_Voucher.Descendants("BookedAndPayableBy").FirstOrDefault().Value;


            string EmergencyPhone = "";
            if (BOOKING_Voucher.Descendants("EmergencyPhone").FirstOrDefault() != null)
                EmergencyPhone = BOOKING_Voucher.Descendants("EmergencyPhone").FirstOrDefault().Value;

            string Phone = "";
            if (BOOKING_Voucher.Descendants("Phone").FirstOrDefault() != null)
                Phone = BOOKING_Voucher.Descendants("Phone").FirstOrDefault().Value;


             TotalRemark =  Remark + ".. BookedAndPayableBy: " + BookedAndPayableBy + ".. Phone: " + Phone + ".. EmergencyPhone: " + EmergencyPhone;

            return supplierreferenceNO;
        }


        #endregion



        #region Cancel Booking


        public XElement CancelBookingGoGlobal(XElement MainCancelBookReq)
        {

           

            string CancelResponse = "";
            XElement MainCancelBookResponse = null;
            try
            {

            string bookingcode = MainCancelBookReq.Descendants("ConfirmationNumber").FirstOrDefault().Value;
            string TotalPrice = MainCancelBookReq.Descendants("TotalPrice").FirstOrDefault().Value;
            string[] code = bookingcode.Split('_');
            bookingcode = code[0].ToString();
            string status = CancelBookingGoGlobal(bookingcode, MainCancelBookReq);
           


            if (status == "Success")
            {
                CancelResponse = @"<HotelCancellationResponse>
      <Rooms>
        <Room>
          <Cancellation>
            <Amount>" + TotalPrice + @"</Amount>
            <Status>" + status + @"</Status>
          </Cancellation>
        </Room>
      </Rooms>
    </HotelCancellationResponse>";
            }
            else
            {
                string Cxstatus = "";
                try
                {
                    Cxstatus = chkBookingStatus_ForCancel(bookingcode, MainCancelBookReq);
                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "CancelBookingGoGlobal/chkBookingStatus_ForCancel";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = MainCancelBookReq.Descendants("CustomerID").Single().Value;
                    ex1.TranID = MainCancelBookReq.Descendants("TransactionID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                }
                if (Cxstatus.ToUpper() == "X")  // Cancel from Supplier
                {
                    CancelResponse = @"<HotelCancellationResponse>
      <Rooms>
        <Room>
          <Cancellation>
            <Amount>" + TotalPrice + @"</Amount>
            <Status>" + status + @"</Status>
          </Cancellation>
        </Room>
      </Rooms>
    </HotelCancellationResponse>";
                }
                else
                {
                    CancelResponse = @" <HotelBookingResponse>
                                                 <ErrorTxt>
                                                 " + "Can,t Cancel " + @"
                                                 </ErrorTxt>
                                                </HotelBookingResponse>";
                  
                }
            }

             MainCancelBookResponse = XElement.Parse(CancelResponse);
              }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "CancelBookingGoGlobal1";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = MainCancelBookReq.Descendants("CustomerID").Single().Value;
                ex1.TranID = MainCancelBookReq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
                CancelResponse = @"<HotelCancellationResponse>
      <ErrorTxt>"
                + ex.Message +
              @"</ErrorTxt>
    </HotelCancellationResponse>";
                MainCancelBookResponse = XElement.Parse(CancelResponse);
            }

            try
            {

                XElement res = new XElement(MainCancelBookReq);
                XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
                var r = res.Element(namespc + "Body");

                r.Add(MainCancelBookResponse);
                return res;
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "CancelBookingGoGlobal2";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = MainCancelBookReq.Descendants("CustomerID").Single().Value;
                ex1.TranID = MainCancelBookReq.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);

                CancelResponse = @"<HotelCancellationResponse>
      <ErrorTxt>"
              + ex.Message +
            @"</ErrorTxt>
    </HotelCancellationResponse>";
                MainCancelBookResponse = XElement.Parse(CancelResponse);

                XElement res = new XElement(MainCancelBookReq);
                XNamespace namespc = "http://schemas.xmlsoap.org/soap/envelope/";
                var r = res.Element(namespc + "Body");

                r.Add(MainCancelBookResponse);
                return res;



            }






        }




        private string CancelBookingGoGlobal(string code, XElement reqmain, string LogType = "Cancel")
        {

            /*change by ravish 09102020      
     <API-AgencyID>" + agency + @"</API-AgencyID>
     <API-Operation>HOTEL_SEARCH_REQUEST</API-Operation>
*/


            string bookingcancel = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
<soap12:Body>
<MakeRequest xmlns=""http://www.goglobal.travel/"">
<requestType>3</requestType>
<xmlRequest><![CDATA[
<Root>
			<Header>
			  <Agency>" + agency + @"</Agency>
		<User>" + user + @"</User>
		<Password>" + password + @"</Password>
			  <Operation>BOOKING_CANCEL_REQUEST</Operation>
			  <OperationType>Request</OperationType>
			</Header>
		 <Main Version=""2.0"">
          <GoBookingCode>" + code + @"</GoBookingCode>
         
         </Main>
</Root>
]]></xmlRequest>
</MakeRequest>
</soap12:Body>
</soap12:Envelope>";

            XElement BOOKING_Cancel = GoGlobalSupplierResponseXML(bookingcancel, reqmain, LogType, "BOOKING_CANCEL_REQUEST");


            // List of Statuses
            //C = Confirm
            //RQ = Request- Status is not final, you need to send status check and expect to receive C or RJ)
            //RX = Request Cancel (Status is not final, you need to send status check and expect to receive X)
            //X = Cancel
            //RJ = Reject



            string status = "";
            if (BOOKING_Cancel.Descendants("BookingStatus").FirstOrDefault() != null)
                status = BOOKING_Cancel.Descendants("BookingStatus").FirstOrDefault().Value;
            else
            {
                if (BOOKING_Cancel.Descendants("Error").FirstOrDefault() != null)
                    status = BOOKING_Cancel.Descendants("Error").FirstOrDefault().Value;
            }




            if (status == "x" || status == "X")
                return "Success";
            else
                return status;



        }




        #endregion





        #region Coommon

        public dynamic GoGlobalSupplierResponse_Search(int timeout, string sup_req, XElement reqMain, string LogType, ref bool error, ref string errors, string htlid = null)
        {
            string sss = sup_req;
            XDocument reqxml = null;
            try
            {
                reqxml = XDocument.Parse(sss);
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GoGlobalSupplierResponse_parse";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }
            string pageContent = string.Empty;
            XDocument responsexml = new XDocument();
            DateTime startTime = DateTime.Now;
            Stream responseStream = null;
            HttpWebResponse myhttpresponse = null;
            try
            {
                string request = reqxml.ToString();
                string suprequest = sup_req;
                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                log.LogTypeID = returnLogTypeId(LogType);
                log.LogType = LogType;
                log.SupplierID = SuupplierId;
                log.logrequestXML = suprequest.ToString();
                log.StartTime = startTime;

                string host = Host;
                string soapaction = host + "?op=MakeRequest";
                try
                {
                    string url = host;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                    HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    myhttprequest.Method = "POST";
                    myhttprequest.Headers.Add(@"SOAPAction:" + soapaction + "");
                    byte[] data = Encoding.ASCII.GetBytes(request);
                    myhttprequest.Timeout = timeout;
                    myhttprequest.ContentType = "application/soap+xml;charset=UTF-8";
                    myhttprequest.Headers.Add("API-AgencyID", agency);
                    myhttprequest.Headers.Add("API-Operation", "HOTEL_SEARCH_REQUEST");
                    myhttprequest.ContentLength = data.Length;
                    myhttprequest.KeepAlive = true;
                    Stream requestStream = myhttprequest.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                    myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();
                    responseStream = myhttpresponse.GetResponseStream();
                    StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                    pageContent = myReader.ReadToEnd();
                    myReader.Close();
                    responseStream.Close();
                    myhttpresponse.Close();
                    responsexml = XDocument.Parse(pageContent);
                }
                catch (Exception ex)
                {
                    try
                    {
                        log.EndTime = DateTime.Now;
                        log.logresponseXML = responsexml.ToString();
                        int lgid = returnLogTypeId(LogType);
                        if (lgid == 1 && LogType == "Search")
                        {
                            SaveAPILog savelogpro = new SaveAPILog();
                            savelogpro.SaveAPILogs_search(log);
                        }
                        else
                        {
                            SaveAPILog savelogpro = new SaveAPILog();
                            savelogpro.SaveAPILogs(log);
                        }
                    }
                    catch (Exception)
                    { }
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobalSupplierResponse_MakeRequest";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                    ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                }
                XNamespace namespc = "http://www.goglobal.travel/";
                string strResponseCode = String.Empty;
                XElement jsonsave = null;
                IEnumerable<XElement> responses = responsexml.Descendants(namespc + "MakeRequestResponse");
                foreach (XElement response in responses)
                {
                    strResponseCode = (string)response.Element(namespc + "MakeRequestResult");
                    jsonsave = new XElement("jsonreturn", strResponseCode);
                }
                var xmlresponse = "";
                bool exceptioInjson = false;
                try
                {
                    var srchresponse = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(Encoding.ASCII.GetBytes(strResponseCode), new XmlDictionaryReaderQuotas()));
                    XElement availresponse = XElement.Parse(srchresponse.ToString());
                    XElement doc = RemoveAllNamespaces(availresponse);
                    xmlresponse = doc.ToString();
                }
                catch
                {
                    xmlresponse = strResponseCode;
                    try
                    {

                        error = xmlresponse.Contains("Error");
                        errors = xmlresponse;
                    }
                    catch
                    {

                    }
                    exceptioInjson = true;
                }
                #region Log Save
                log.logresponseXML = xmlresponse.ToString();
                log.StartTime = startTime;
                log.EndTime = DateTime.Now;
                try
                {
                    int lgid = returnLogTypeId(LogType);
                    if (lgid == 1 && LogType == "Search")
                    {
                        SaveAPILog savelogpro = new SaveAPILog();
                        savelogpro.SaveAPILogs_search(log);
                    }
                    else
                    {
                        SaveAPILog savelogpro = new SaveAPILog();
                        savelogpro.SaveAPILogs(log);
                    }
                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobalSupplierResponse/" + LogType;
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                    ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                }
                #endregion
                if (exceptioInjson == false)
                {
                    dynamic HotelList = JsonConvert.DeserializeObject(strResponseCode);
                    return HotelList;
                }
                else
                {
                    return strResponseCode;
                }
            }
            catch (WebException ex)
            {
                #region Save in apilog table
                if (ex.Response != null)
                {
                    try
                    {
                        var responses = ex.Response;
                        var dataStream = responses.GetResponseStream();
                        var reader = new StreamReader(dataStream);
                        var details = reader.ReadToEnd();
                        string suprequest = sup_req;
                        APILogDetail log = new APILogDetail();
                        log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                        log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                        log.LogTypeID = returnLogTypeId(LogType);
                        log.LogType = LogType;
                        log.SupplierID = SuupplierId;
                        log.logrequestXML = suprequest.ToString();
                        log.logresponseXML = details.ToString();
                        log.StartTime = startTime;
                        log.EndTime = DateTime.Now;
                        int lgid = returnLogTypeId(LogType);
                        if (lgid == 1 && LogType == "Search")
                        {
                            SaveAPILog savelogpro = new SaveAPILog();
                            savelogpro.SaveAPILogs_search(log);
                        }
                        else
                        {
                            SaveAPILog savelogpro = new SaveAPILog();
                            savelogpro.SaveAPILogs(log);
                        }
                    }
                    catch { }
                }
                #endregion
            }
            finally
            {

            }
            return responsexml;
        }

        public dynamic GoGlobalSupplierResponse_Room(int timeout, string sup_req, XElement reqMain, string LogType, ref bool error, ref string errors, string htlid = null)
        {
            // new XDeclaration("1.0", "utf-8", "yes"),


            string sss = sup_req;
            XDocument reqxml = null;
            try
            {
                reqxml = XDocument.Parse(sss);
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GoGlobalSupplierResponse_parse";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }





            string pageContent = string.Empty;
            // LoginDetail rc = new LoginDetail();
            XDocument responsexml = new XDocument();
            DateTime startTime = DateTime.Now;
            Stream responseStream = null;
            HttpWebResponse myhttpresponse = null;
            try
            {
                string request = reqxml.ToString();
                //string host = "http://xml.qa.goglobal.travel/XMLWebService.asmx";
                //string soapaction = "http://xml.qa.goglobal.travel/XMLWebService.asmx?op=MakeRequest";
                string suprequest = sup_req;
                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                log.LogTypeID = returnLogTypeId(LogType);
                log.LogType = LogType;
                log.SupplierID = SuupplierId;
                log.logrequestXML = suprequest.ToString();
                log.StartTime = startTime;


                string host = Host;
                string soapaction = host + "?op=MakeRequest";

                try
                {
                    string url = host;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                    HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    myhttprequest.Method = "POST";
                    myhttprequest.Headers.Add(@"SOAPAction:" + soapaction + "");
                    byte[] data = Encoding.ASCII.GetBytes(request);
                    if (LogType == "Search")
                    {
                        myhttprequest.Timeout = timeout;
                    }
                    myhttprequest.ContentType = "application/soap+xml;charset=UTF-8";
                    myhttprequest.Headers.Add("API-AgencyID", agency);
                    myhttprequest.Headers.Add("API-Operation", "HOTEL_SEARCH_REQUEST");
                    myhttprequest.ContentLength = data.Length;
                    myhttprequest.KeepAlive = true;
                    Stream requestStream = myhttprequest.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                    myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();


                    responseStream = myhttpresponse.GetResponseStream();
                    StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                    pageContent = myReader.ReadToEnd();
                    myReader.Close();
                    responseStream.Close();
                    myhttpresponse.Close();
                    responsexml = XDocument.Parse(pageContent);

                }
                catch (Exception ex)
                {


                    try
                    {
                        log.EndTime = DateTime.Now;
                        log.logresponseXML = responsexml.ToString();

                        SaveAPILog savelogpro = new SaveAPILog();
                        savelogpro.SaveAPILogs_room(log);

                    }
                    catch (Exception)
                    { }
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobalSupplierResponse_MakeRequest";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                    ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);

                }







                //pageContent = responsexml.Root.Element("MakeRequestResult").Value;
                XNamespace namespc = "http://www.goglobal.travel/";




                string strResponseCode = String.Empty;
                XElement jsonsave = null;


                IEnumerable<XElement> responses = responsexml.Descendants(namespc + "MakeRequestResponse");
                foreach (XElement response in responses)
                {
                    strResponseCode = (string)response.Element(namespc + "MakeRequestResult");
                    jsonsave = new XElement("jsonreturn", strResponseCode);
                    //reqMain.Descendants("searchRequest").FirstOrDefault().Add(jsonsave);
                }


                var xmlresponse = "";
                bool exceptioInjson = false;
                try
                {
                    var srchresponse = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(Encoding.ASCII.GetBytes(strResponseCode), new XmlDictionaryReaderQuotas()));
                    XElement availresponse = XElement.Parse(srchresponse.ToString());
                    XElement doc = RemoveAllNamespaces(availresponse);

                    if (LogType == "RoomAvail")
                    {
                        doc.Descendants("Hotels").FirstOrDefault().Add(jsonsave);
                        if (srchresponse.Descendants("HotelQty").FirstOrDefault().Value != "1" || srchresponse.Descendants("HotelCode").FirstOrDefault().Value != htlid)
                        {
                            error = true;
                        }

                    }

                    xmlresponse = doc.ToString();
                }
                catch
                {
                    xmlresponse = strResponseCode;
                    try
                    {

                        error = xmlresponse.Contains("Error");
                        errors = xmlresponse;
                    }
                    catch
                    {

                    }

                    exceptioInjson = true;
                }

                #region Log Save
                /*  string suprequest = sup_req;
                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                log.LogTypeID = returnLogTypeId(LogType);
                log.LogType = LogType;
                log.SupplierID = SuupplierId;
                log.logrequestXML = suprequest.ToString();*/
                log.logresponseXML = xmlresponse.ToString();
                log.StartTime = startTime;
                log.EndTime = DateTime.Now;
                try
                {

                    SaveAPILog savelogpro = new SaveAPILog();
                    savelogpro.SaveAPILogs_room(log);

                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobalSupplierResponse/" + LogType;
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                    ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                }
                #endregion
                if (exceptioInjson == false)
                {
                    dynamic HotelList = JsonConvert.DeserializeObject(strResponseCode);


                    return HotelList;
                }
                else
                {
                    return strResponseCode;
                }



            }
            catch (WebException ex)
            {
                #region Save in apilog table
                if (ex.Response != null)
                {
                    try
                    {
                        var responses = ex.Response;
                        var dataStream = responses.GetResponseStream();
                        var reader = new StreamReader(dataStream);
                        var details = reader.ReadToEnd();
                        string suprequest = sup_req;
                        APILogDetail log = new APILogDetail();
                        log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                        log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                        log.LogTypeID = returnLogTypeId(LogType);
                        log.LogType = LogType;
                        log.SupplierID = SuupplierId;
                        log.logrequestXML = suprequest.ToString();
                        log.logresponseXML = details.ToString();
                        log.StartTime = startTime;
                        log.EndTime = DateTime.Now;

                        SaveAPILog savelogpro = new SaveAPILog();
                        savelogpro.SaveAPILogs_room(log);

                    }
                    catch { }
                }
                #endregion
                //CustomException ex1 = new CustomException(ex);
                //ex1.MethodName = "searchresponsenew";
                //ex1.PageName = "HotelsProHotelSearch";
                //ex1.CustomerID = reqTravayoo.Descendants("CustomerID").Single().Value;
                //ex1.TranID = reqTravayoo.Descendants("TransID").Single().Value;
                //SaveAPILog saveex = new SaveAPILog();
                //saveex.SendCustomExcepToDB(ex1);
                //return HotelsList;
            }
            finally
            {

            }
            return responsexml;
        }


        public dynamic GoGlobalSupplierResponse(int timeout, string sup_req, XElement reqMain, string LogType, ref bool error, ref string errors, string htlid = null)
        {
            // new XDeclaration("1.0", "utf-8", "yes"),


            string sss = sup_req;
            XDocument reqxml = null;
            try
            {
                reqxml = XDocument.Parse(sss);
            }
            catch (Exception ex)
            {
                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GoGlobalSupplierResponse_parse";
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);
            }





            string pageContent = string.Empty;
            // LoginDetail rc = new LoginDetail();
            XDocument responsexml = new XDocument();
            DateTime startTime = DateTime.Now;
            Stream responseStream = null;
            HttpWebResponse myhttpresponse = null;
            try
            {
                string request = reqxml.ToString();
                //string host = "http://xml.qa.goglobal.travel/XMLWebService.asmx";
                //string soapaction = "http://xml.qa.goglobal.travel/XMLWebService.asmx?op=MakeRequest";
                string suprequest = sup_req;
                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                log.LogTypeID = returnLogTypeId(LogType);
                log.LogType = LogType;
                log.SupplierID = SuupplierId;
                log.logrequestXML = suprequest.ToString();
                log.StartTime = startTime;


                string host = Host;
                string soapaction = host + "?op=MakeRequest";

                try
                {
                    string url = host;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                    HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    myhttprequest.Method = "POST";
                    myhttprequest.Headers.Add(@"SOAPAction:" + soapaction + "");
                    byte[] data = Encoding.ASCII.GetBytes(request);
                    if (LogType == "Search")
                    {
                        myhttprequest.Timeout = timeout;
                    }
                    myhttprequest.ContentType = "application/soap+xml;charset=UTF-8";
                    myhttprequest.Headers.Add("API-AgencyID", agency);
                    myhttprequest.Headers.Add("API-Operation", "HOTEL_SEARCH_REQUEST");
                    myhttprequest.ContentLength = data.Length;
                    myhttprequest.KeepAlive = true;
                    Stream requestStream = myhttprequest.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                    myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();


                    responseStream = myhttpresponse.GetResponseStream();
                    StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                    pageContent = myReader.ReadToEnd();
                    myReader.Close();
                    responseStream.Close();
                    myhttpresponse.Close();
                    responsexml = XDocument.Parse(pageContent);

                }
                catch (Exception ex)
                {


                    try
                    {
                        log.EndTime = DateTime.Now;
                        log.logresponseXML = responsexml.ToString();

                        SaveAPILog savelogpro = new SaveAPILog();
                        savelogpro.SaveAPILogs(log);

                    }
                    catch (Exception)
                    { }
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobalSupplierResponse_MakeRequest";
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                    ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);

                }







                //pageContent = responsexml.Root.Element("MakeRequestResult").Value;
                XNamespace namespc = "http://www.goglobal.travel/";




                string strResponseCode = String.Empty;
                XElement jsonsave = null;


                IEnumerable<XElement> responses = responsexml.Descendants(namespc + "MakeRequestResponse");
                foreach (XElement response in responses)
                {
                    strResponseCode = (string)response.Element(namespc + "MakeRequestResult");
                    jsonsave = new XElement("jsonreturn", strResponseCode);
                    //reqMain.Descendants("searchRequest").FirstOrDefault().Add(jsonsave);
                }


                var xmlresponse = "";
                bool exceptioInjson = false;
                try
                {
                    var srchresponse = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(Encoding.ASCII.GetBytes(strResponseCode), new XmlDictionaryReaderQuotas()));
                    XElement availresponse = XElement.Parse(srchresponse.ToString());
                    XElement doc = RemoveAllNamespaces(availresponse);

                    if (LogType == "RoomAvail")
                    {
                        doc.Descendants("Hotels").FirstOrDefault().Add(jsonsave);
                        if (srchresponse.Descendants("HotelQty").FirstOrDefault().Value != "1" || srchresponse.Descendants("HotelCode").FirstOrDefault().Value != htlid)
                        {
                            error = true;
                        }

                    }

                    xmlresponse = doc.ToString();
                }
                catch
                {
                    xmlresponse = strResponseCode;
                    try
                    {

                        error = xmlresponse.Contains("Error");
                        errors = xmlresponse;
                    }
                    catch
                    {

                    }

                    exceptioInjson = true;
                }

                #region Log Save
                /*  string suprequest = sup_req;
                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                log.LogTypeID = returnLogTypeId(LogType);
                log.LogType = LogType;
                log.SupplierID = SuupplierId;
                log.logrequestXML = suprequest.ToString();*/
                log.logresponseXML = xmlresponse.ToString();
                log.StartTime = startTime;
                log.EndTime = DateTime.Now;
                try
                {

                    SaveAPILog savelogpro = new SaveAPILog();
                    savelogpro.SaveAPILogs(log);

                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobalSupplierResponse/" + LogType;
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                    ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                }
                #endregion
                if (exceptioInjson == false)
                {
                    dynamic HotelList = JsonConvert.DeserializeObject(strResponseCode);


                    return HotelList;
                }
                else
                {
                    return strResponseCode;
                }



            }
            catch (WebException ex)
            {
                #region Save in apilog table
                if (ex.Response != null)
                {
                    try
                    {
                        var responses = ex.Response;
                        var dataStream = responses.GetResponseStream();
                        var reader = new StreamReader(dataStream);
                        var details = reader.ReadToEnd();
                        string suprequest = sup_req;
                        APILogDetail log = new APILogDetail();
                        log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                        log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                        log.LogTypeID = returnLogTypeId(LogType);
                        log.LogType = LogType;
                        log.SupplierID = SuupplierId;
                        log.logrequestXML = suprequest.ToString();
                        log.logresponseXML = details.ToString();
                        log.StartTime = startTime;
                        log.EndTime = DateTime.Now;

                        SaveAPILog savelogpro = new SaveAPILog();
                        savelogpro.SaveAPILogs(log);

                    }
                    catch { }
                }
                #endregion
                //CustomException ex1 = new CustomException(ex);
                //ex1.MethodName = "searchresponsenew";
                //ex1.PageName = "HotelsProHotelSearch";
                //ex1.CustomerID = reqTravayoo.Descendants("CustomerID").Single().Value;
                //ex1.TranID = reqTravayoo.Descendants("TransID").Single().Value;
                //SaveAPILog saveex = new SaveAPILog();
                //saveex.SendCustomExcepToDB(ex1);
                //return HotelsList;
            }
            finally
            {

            }
            return responsexml;
        }




      
        public XElement GoGlobalSupplierResponseXML(string sup_req,XElement reqMain,string LogType,string operation)
        {
            // new XDeclaration("1.0", "utf-8", "yes"),


            string sss = sup_req;

            XDocument reqxml = XDocument.Parse(sss);

            string pageContent = string.Empty;
            // LoginDetail rc = new LoginDetail();
            XDocument responsexml = new XDocument();
            DateTime startTime = DateTime.Now;
            try
            {
                
                
                
                string request = reqxml.ToString();
                string host = Host;
                string soapaction = host + "?op=MakeRequest";





                string url = host;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myhttprequest.Method = "POST";
                myhttprequest.Headers.Add(@"SOAPAction:" + soapaction + "");
                //myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                byte[] data = Encoding.ASCII.GetBytes(request);
                myhttprequest.ContentType = "application/soap+xml;charset=UTF-8";               
                myhttprequest.Headers.Add("API-AgencyID", agency);
                myhttprequest.Headers.Add("API-Operation", operation);
                myhttprequest.ContentLength = data.Length;
                myhttprequest.KeepAlive = true;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();
                Stream responseStream = myhttpresponse.GetResponseStream();
                StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                pageContent = myReader.ReadToEnd();
                myReader.Close();
                responseStream.Close();
                myhttpresponse.Close();
                responsexml = XDocument.Parse(pageContent);
                XNamespace namespc = "http://www.goglobal.travel/";
                string strResponseCode = String.Empty;
                IEnumerable<XElement> responses = responsexml.Descendants(namespc + "MakeRequestResponse");
                foreach (XElement response in responses)
                {
                    strResponseCode = (string)response.Element(namespc + "MakeRequestResult");

                }

                responsexml = XDocument.Parse(strResponseCode);


                #region Log Save
                string suprequest = sup_req;
                APILogDetail log = new APILogDetail();
                log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                if (LogType == "Book" || LogType == "Voucher" || LogType == "chkStatus" || LogType == "chkStatusForCancel" || LogType == "chkStatusForVoucher" || LogType == "CancelInBook")
                    log.TrackNumber = reqMain.Descendants("TransactionID").Single().Value;

                log.LogTypeID = returnLogTypeId(LogType);
                log.LogType = LogType;
                log.SupplierID = SuupplierId;
                log.logrequestXML = suprequest.ToString();
                log.logresponseXML = responsexml.ToString();
                log.StartTime = startTime;
                log.EndTime = DateTime.Now;
                try
                {
                    SaveAPILog savelogpro = new SaveAPILog();
                    savelogpro.SaveAPILogs(log);
                }
                catch (Exception ex)
                {
                    CustomException ex1 = new CustomException(ex);
                    ex1.MethodName = "GoGlobalSupplierResponse/" + LogType;
                    ex1.PageName = "GoGlobal";
                    ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                    ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                    if (LogType == "Book" || LogType == "Voucher" || LogType == "chkStatus" || LogType == "chkStatusForCancel" || LogType == "chkStatusForVoucher" || LogType == "CancelInBook")
                        ex1.TranID = reqMain.Descendants("TransactionID").Single().Value;


                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                }
                #endregion


            }
            catch (WebException ex)
            {
                #region Save in apilog table
                if (ex.Response != null)
                {
                    try
                    {
                        var responses = ex.Response;
                        var dataStream = responses.GetResponseStream();
                        var reader = new StreamReader(dataStream);
                        var details = reader.ReadToEnd();
                        string suprequest = sup_req;
                        APILogDetail log = new APILogDetail();
                        log.customerID = Convert.ToInt64(reqMain.Descendants("CustomerID").Single().Value);
                        log.TrackNumber = reqMain.Descendants("TransID").Single().Value;
                        if (LogType == "Book" || LogType == "Voucher" || LogType == "chkStatus" || LogType == "chkStatusForCancel" || LogType == "chkStatusForVoucher" || LogType == "CancelInBook" || LogType == "CancelInBook")
                           log.TrackNumber  = reqMain.Descendants("TransactionID").Single().Value;


                        log.LogTypeID = returnLogTypeId(LogType); 
                        log.LogType = LogType;
                        log.SupplierID = SuupplierId;
                        log.logrequestXML = suprequest.ToString();
                        log.logresponseXML = details.ToString();
                        log.StartTime = startTime;
                        log.EndTime = DateTime.Now;
                        SaveAPILog savelogpro = new SaveAPILog();
                        savelogpro.SaveAPILogs(log);
                    }
                    catch
                    {
                    }
                }
                #endregion
                //CustomException ex1 = new CustomException(ex);
                //ex1.MethodName = "searchresponsenew";
                //ex1.PageName = "HotelsProHotelSearch";
                //ex1.CustomerID = reqTravayoo.Descendants("CustomerID").Single().Value;
                //ex1.TranID = reqTravayoo.Descendants("TransID").Single().Value;
                //SaveAPILog saveex = new SaveAPILog();
                //saveex.SendCustomExcepToDB(ex1);
                //return HotelsList;
            }
            finally
            {

            }
            return responsexml.Root;
        }

        public XElement GoGlobalSupplierResponseXMLBrekUp(string sup_req, XElement reqMain, string RoomCode, ref XElement Breakups)
        {
            // new XDeclaration("1.0", "utf-8", "yes"),

          
            string sss = sup_req;

            XDocument reqxml = XDocument.Parse(sss);
          


            string pageContent = string.Empty;
            // LoginDetail rc = new LoginDetail();
            XDocument responsexml = new XDocument();
            DateTime startTime = DateTime.Now;
            try
            {
                string request = reqxml.ToString();
                //string host = "http://xml.qa.goglobal.travel/XMLWebService.asmx";
                //string soapaction = "http://xml.qa.goglobal.travel/XMLWebService.asmx?op=MakeRequest";

                string host = Host;
                string soapaction = host + "?op=MakeRequest";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                string url = host;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myhttprequest.Method = "POST";
                myhttprequest.Headers.Add(@"SOAPAction:" + soapaction + "");
                // myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                byte[] data = Encoding.ASCII.GetBytes(request);
                myhttprequest.ContentType = "application/soap+xml;charset=UTF-8";
                myhttprequest.ContentLength = data.Length;
                myhttprequest.KeepAlive = true;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();
                Stream responseStream = myhttpresponse.GetResponseStream();
                StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                pageContent = myReader.ReadToEnd();
                myReader.Close();
                responseStream.Close();
                myhttpresponse.Close();
                responsexml = XDocument.Parse(pageContent);
                XNamespace namespc = "http://www.goglobal.travel/";
                string strResponseCode = String.Empty;
                IEnumerable<XElement> responses = responsexml.Descendants(namespc + "MakeRequestResponse");
                foreach (XElement response in responses)
                {
                    strResponseCode = (string)response.Element(namespc + "MakeRequestResult");

                }

                responsexml = XDocument.Parse(strResponseCode);

             

            }
            catch (WebException ex)
            {

                CustomException ex1 = new CustomException(ex);
                ex1.MethodName = "GoGlobalSupplierResponseXMLBrekUp/" + RoomCode;
                ex1.PageName = "GoGlobal";
                ex1.CustomerID = reqMain.Descendants("CustomerID").Single().Value;
                ex1.TranID = reqMain.Descendants("TransID").Single().Value;
                SaveAPILog saveex = new SaveAPILog();
                saveex.SendCustomExcepToDB(ex1);


            }
            finally
            {

            }
            return responsexml.Root;
        }
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

        private int returnLogTypeId(string LogType)
        {
            if (LogType == "Search")
                return 1;
            else if (LogType == "RoomAvail")
                return 2;
            else if (LogType == "PreBook")
                return 4;
            else if (LogType == "BreakUp")
                return 4;
            else if (LogType == "Book" || LogType == "Voucher")
                return 5;
            else if (LogType == "HotelDetail")
                return 102;
            else if (LogType == "chkStatus" || LogType == "chkStatusForCancel" || LogType == "chkStatusForVoucher" || LogType == "CancelInBook")
                return 5;
            else if (LogType == "Cancel" )
                return 6;
            return 1;
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