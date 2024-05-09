using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Models;

namespace TravillioXMLOutService.Supplier.RTS
{
  public  class RequestClass
    {
      public XDocument HttpPostRequest(string url, string postData, XElement Req, string Type, int LogTypeID)
      {
          XDocument Responce = null;
          DateTime Reqstattime = DateTime.Now;
          DateTime ReqEndtime = DateTime.Now;
          try
          {

            
              HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
              myHttpWebRequest.Method = "POST";
              byte[] data = Encoding.ASCII.GetBytes(postData);


              myHttpWebRequest.ContentType = "text/xml;charset=UTF-8";

              myHttpWebRequest.ContentLength = data.Length;

              Stream requestStream = myHttpWebRequest.GetRequestStream();
              requestStream.Write(data, 0, data.Length);
              requestStream.Close();
              HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
              Stream responseStream = myHttpWebResponse.GetResponseStream();
              StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
              string pageContent = myStreamReader.ReadToEnd();
              myStreamReader.Close();
              responseStream.Close();
              myHttpWebResponse.Close();
              
              Responce = XDocument.Parse(HttpUtility.HtmlDecode(pageContent));

              ReqEndtime = DateTime.Now;

              APILogDetail log = new APILogDetail();
              log.customerID = Convert.ToInt64(Req.Descendants("CustomerID").Single().Value);
              log.LogTypeID = LogTypeID;
              log.LogType = Type;
              log.SupplierID = 9;
              log.StartTime = Reqstattime;
              log.EndTime = ReqEndtime;
              log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;

              log.logrequestXML = postData;
              log.logresponseXML = Responce.ToString();

              SaveAPILog savelog = new SaveAPILog();
              savelog.SaveAPILogs(log);


          }
          catch (Exception e)
          {
              ReqEndtime = DateTime.Now; 
            
              APILogDetail log = new APILogDetail();
              log.customerID = Convert.ToInt64(Req.Descendants("CustomerID").Single().Value);
              log.LogTypeID = LogTypeID;
              log.LogType = Type;
              log.SupplierID = 9;
              log.StartTime = Reqstattime;
              log.EndTime = ReqEndtime;
              log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;
              log.logrequestXML = postData;
              log.logresponseXML = e.Message.ToString();
              SaveAPILog savelog = new SaveAPILog();
              savelog.SaveAPILogs(log);
              Responce = new XDocument("Error", e.Message);
          }
         

          return Responce;

      }

      public XDocument HttpPostRequestbook(string url, string postData, XElement Req, string Type, int LogTypeID)
      {
          XDocument Responce = null;
          DateTime Reqstattime = DateTime.Now;
          DateTime ReqEndtime = DateTime.Now;
          try
          {

              try
              {
                  APILogDetail log4 = new APILogDetail();
                  log4.customerID = Convert.ToInt64(Req.Descendants("CustomerID").FirstOrDefault().Value);
                  log4.LogTypeID = 5;
                  log4.LogType = "Book";
                  log4.SupplierID = 9;
                  log4.StartTime = DateTime.Now;
                  log4.EndTime = DateTime.Now;
                  log4.TrackNumber = Req.Descendants("TransactionID").FirstOrDefault().Value;
                  log4.logrequestXML = postData.ToString();
                  log4.logresponseXML = "";
                  SaveAPILog savelog4 = new SaveAPILog();
                  savelog4.SaveAPILogs(log4);
              }
              catch { }

              HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
              myHttpWebRequest.Method = "POST";
              byte[] data = Encoding.ASCII.GetBytes(postData);


              myHttpWebRequest.ContentType = "text/xml;charset=UTF-8";

              myHttpWebRequest.ContentLength = data.Length;

              Stream requestStream = myHttpWebRequest.GetRequestStream();
              requestStream.Write(data, 0, data.Length);
              requestStream.Close();
              HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
              Stream responseStream = myHttpWebResponse.GetResponseStream();
              StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
              string pageContent = myStreamReader.ReadToEnd();
              myStreamReader.Close();
              responseStream.Close();
              myHttpWebResponse.Close();

              Responce = XDocument.Parse(HttpUtility.HtmlDecode(pageContent));

              ReqEndtime = DateTime.Now;

              APILogDetail log = new APILogDetail();
              log.customerID = Convert.ToInt64(Req.Descendants("CustomerID").Single().Value);
              log.LogTypeID = LogTypeID;
              log.LogType = Type;
              log.SupplierID = 9;
              log.StartTime = Reqstattime;
              log.EndTime = ReqEndtime;
              log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;

              log.logrequestXML = postData;
              log.logresponseXML = Responce.ToString();

              SaveAPILog savelog = new SaveAPILog();
              savelog.SaveAPILogs(log);


          }
          catch (Exception e)
          {
              ReqEndtime = DateTime.Now;

              APILogDetail log = new APILogDetail();
              log.customerID = Convert.ToInt64(Req.Descendants("CustomerID").Single().Value);
              log.LogTypeID = LogTypeID;
              log.LogType = Type;
              log.SupplierID = 9;
              log.StartTime = Reqstattime;
              log.EndTime = ReqEndtime;
              log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;
              log.logrequestXML = postData;
              log.logresponseXML = e.Message.ToString();
              SaveAPILog savelog = new SaveAPILog();
              savelog.SaveAPILogs(log);
              Responce = new XDocument("Error", e.Message);
          }


          return Responce;

      }

      public XDocument HttpPostRequestxmlout(string url, string postData, XElement Req, string Type, int LogTypeID,string custID)
      {
          XDocument Responce = null;
          DateTime Reqstattime = DateTime.Now;
          DateTime ReqEndtime = DateTime.Now;
          try
          {


              HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
              myHttpWebRequest.Method = "POST";
              byte[] data = Encoding.ASCII.GetBytes(postData);


              myHttpWebRequest.ContentType = "text/xml;charset=UTF-8";

              myHttpWebRequest.ContentLength = data.Length;

              Stream requestStream = myHttpWebRequest.GetRequestStream();
              requestStream.Write(data, 0, data.Length);
              requestStream.Close();
              HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
              Stream responseStream = myHttpWebResponse.GetResponseStream();
              StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
              string pageContent = myStreamReader.ReadToEnd();
              myStreamReader.Close();
              responseStream.Close();
              myHttpWebResponse.Close();
              pageContent=pageContent.Replace("'", " ");
              Responce = XDocument.Parse(HttpUtility.HtmlDecode(pageContent));

              ReqEndtime = DateTime.Now;
              if (LogTypeID == 1 && Type == "Search")
              {
                  APILogDetail log = new APILogDetail();
                  log.customerID = Convert.ToInt64(custID);
                  log.LogTypeID = LogTypeID;
                  log.LogType = Type;
                  log.SupplierID = 9;
                  log.StartTime = Reqstattime;
                  log.EndTime = ReqEndtime;
                  log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;

                  log.logrequestXML = postData;
                  log.logresponseXML = Responce.ToString();

                  SaveAPILog savelog = new SaveAPILog();
                  savelog.SaveAPILogs_search(log);
              }
              else if (LogTypeID == 2 && Type == "RoomAvail")
              {
                  APILogDetail log = new APILogDetail();
                  log.customerID = Convert.ToInt64(custID);
                  log.LogTypeID = LogTypeID;
                  log.LogType = Type;
                  log.SupplierID = 9;
                  log.StartTime = Reqstattime;
                  log.EndTime = ReqEndtime;
                  log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;

                  log.logrequestXML = postData;
                  log.logresponseXML = Responce.ToString();

                  SaveAPILog savelog = new SaveAPILog();
                  savelog.SaveAPILogs_room(log);
              }
              else
              {
                  APILogDetail log = new APILogDetail();
                  log.customerID = Convert.ToInt64(custID);
                  log.LogTypeID = LogTypeID;
                  log.LogType = Type;
                  log.SupplierID = 9;
                  log.StartTime = Reqstattime;
                  log.EndTime = ReqEndtime;
                  log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;

                  log.logrequestXML = postData;
                  log.logresponseXML = Responce.ToString();

                  SaveAPILog savelog = new SaveAPILog();
                  savelog.SaveAPILogs(log);
              }

          }
          catch (Exception e)
          {
              ReqEndtime = DateTime.Now;
              if (LogTypeID == 1 && Type == "Search")
              {
                  APILogDetail log = new APILogDetail();
                  log.customerID = Convert.ToInt64(custID);
                  log.LogTypeID = LogTypeID;
                  log.LogType = Type;
                  log.SupplierID = 9;
                  log.StartTime = Reqstattime;
                  log.EndTime = ReqEndtime;
                  log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;
                  log.logrequestXML = postData;
                  log.logresponseXML = e.Message.ToString();
                  SaveAPILog savelog = new SaveAPILog();
                  savelog.SaveAPILogs_search(log);
              }
              else if (LogTypeID == 2 && Type == "RoomAvail")
              {
                  APILogDetail log = new APILogDetail();
                  log.customerID = Convert.ToInt64(custID);
                  log.LogTypeID = LogTypeID;
                  log.LogType = Type;
                  log.SupplierID = 9;
                  log.StartTime = Reqstattime;
                  log.EndTime = ReqEndtime;
                  log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;
                  log.logrequestXML = postData;
                  log.logresponseXML = e.Message.ToString();
                  SaveAPILog savelog = new SaveAPILog();
                  savelog.SaveAPILogs_room(log);
              }
              else
              {
                  APILogDetail log = new APILogDetail();
                  log.customerID = Convert.ToInt64(custID);
                  log.LogTypeID = LogTypeID;
                  log.LogType = Type;
                  log.SupplierID = 9;
                  log.StartTime = Reqstattime;
                  log.EndTime = ReqEndtime;
                  log.TrackNumber = Type == "Book" ? Req.Descendants("TransactionID").Single().Value : Type == "Voucher" ? Req.Descendants("TransactionID").Single().Value : Req.Descendants("TransID").Single().Value;
                  log.logrequestXML = postData;
                  log.logresponseXML = e.Message.ToString();
                  SaveAPILog savelog = new SaveAPILog();
                  savelog.SaveAPILogs(log);
              }
              Responce = new XDocument("Error", e.Message);
          }


          return Responce;

      }
     
    }
}
