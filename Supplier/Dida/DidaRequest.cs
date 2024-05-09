using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Models;

namespace TravillioXMLOutService.Supplier.Dida
{
    public class DidaRequest : IDisposable
    {
        public XDocument Request(XElement req, string url, string genguid, DidaLogModel model)
        {
            
            XDocument response = new XDocument();
            APILogDetail log = new APILogDetail
            {
                customerID = model.CustomerID,
                logrequestXML = req.ToString(),
                StartTime = DateTime.Now,
                SupplierID = model.SuplId,
                TrackNumber = model.TrackNo,
                LogType = model.LogType,
                LogTypeID = model.LogTypeID,
                preID = genguid
            };
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                myhttprequest.Method = "POST";
                ////myhttprequest.PreAuthenticate = true;
                //myhttprequest.Timeout = sup_timeout;
                myhttprequest.ContentType = "application/xml";
                byte[] data = Encoding.ASCII.GetBytes(req.ToString());
                myhttprequest.ContentLength = data.Length;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                using (HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(myhttpresponse.GetResponseStream()))
                    {
                        string responsess = streamReader.ReadToEnd();
                        response = XDocument.Parse(responsess);
                        log.logresponseXML = removeAllNamespaces(response.Root).ToString();
                        log.EndTime = DateTime.Now;
                    }
                }

            }
            catch (Exception ex)
            {
                log.logMsg = ex.Message;
                log.EndTime = DateTime.Now;
                response.Add(new XElement("Data", new XElement("Exception", ex.Message)));
                log.logresponseXML = response.ToString();
            }
            finally
            {
                try
                {
                    if (model.LogTypeID == 1 && model.LogType == "Search")
                    {
                        SaveAPILog savelog = new SaveAPILog();
                        savelog.SaveAPILogs_search_iol(log);
                    }
                    else if (model.LogTypeID == 2 && model.LogType == "RoomAvail")
                    {
                        SaveAPILog savelog = new SaveAPILog();
                        savelog.SaveAPILogs_room_iol(log);
                    }
                    else
                    {
                        SaveAPILog savelog = new SaveAPILog();
                        savelog.SaveAPILogs(log);
                    }
                }
                catch (Exception ex)
                {
                    CustomException custEx = new CustomException(ex);
                    custEx.MethodName = model.LogType;
                    custEx.PageName = "DidaRequest";
                    custEx.CustomerID = model.CustomerID.ToString();
                    custEx.TranID = model.TrackNo;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(custEx);
                }
            }
            return response;
        }

        public static XElement removeAllNamespaces(XElement e)
        {
            return new XElement(e.Name.LocalName,
              (from n in e.Nodes()
               select ((n is XElement) ? removeAllNamespaces(n as XElement) : n)),
                  (e.HasAttributes) ?
                    (from a in e.Attributes()
                     where (!a.IsNamespaceDeclaration)
                     select new XAttribute(a.Name.LocalName, a.Value)) : null);
        }
        #region Remove Namespaces
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            XElement xmlDocumentWithoutNs = rremoveAllNamespaces(xmlDocument);
            return xmlDocumentWithoutNs;
        }

        private static XElement rremoveAllNamespaces(XElement xmlDocument)
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