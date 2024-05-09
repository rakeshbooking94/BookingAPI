using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using TravillioXMLOutService.Common.DotW;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Models.Juniper;

namespace TravillioXMLOutService.Supplier.Juniper
{
    public class JuniperOutward : IDisposable
    {

        public XDocument JuniperResponse(XDocument reqxml, string Action,string hosturl,Int64 CustID,string TransID,int SuplID) // add a parameter to take the action type which will go as the value in action header
        {

            string pageContent = string.Empty;

            LoginDetail rc = new LoginDetail();
            XDocument responsexml = new XDocument();
            DateTime starttime = DateTime.Now;
            try
            {
                string request = reqxml.ToString();
                string host = hosturl; 
                string soapaction = "http://www.juniper.es/webservice/2007/" + Action;
                string url = host;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myhttprequest.Method = "POST";
                myhttprequest.Timeout = 200000;
                myhttprequest.Headers.Add(@"SOAPAction:" + soapaction + "");
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                byte[] data = Encoding.ASCII.GetBytes(request);
                myhttprequest.ContentType = "text/xml;charset=UTF-8";
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

                try
                {
                    // to remove non printable contents
                    pageContent = Regex.Replace(pageContent, @"\p{C}+", string.Empty);
                }
                catch { }

                //pageContent = CommonHelper.RemoveInvalidXmlChars(pageContent);
                //pageContent = CommonHelper.CleanInvalidXmlChars(pageContent);
                pageContent = this.RemoveInvalidXmlChars(pageContent);
                responsexml = XDocument.Parse(pageContent);

            }
            catch (Exception ex)
            {
                //XElement responseRNS = removeAllNamespaces(pageContent);
                try
                {
                    XElement requestRNS = removeAllNamespaces(reqxml.Root);
                    APILogDetail log = new APILogDetail();
                    log.customerID = CustID;
                    log.TrackNumber = TransID;
                    log.SupplierID = SuplID;
                    log.logrequestXML = requestRNS.ToString();
                    log.logresponseXML = responsexml.ToString();
                    log.LogType = Action;
                    log.logMsg = ex.Message.ToString();
                    log.LogTypeID = 1;
                    log.StartTime = starttime;
                    log.EndTime = DateTime.Now;
                    SaveAPILog savelog = new SaveAPILog();
                    savelog.SaveAPILogwithResponseError(log);
                }
                catch (Exception exApi)
                {
                    #region Exception
                    CustomException ex1 = new CustomException(exApi);
                    ex1.MethodName = Action;
                    ex1.PageName = "JuniperOutward";
                    ex1.CustomerID = CustID.ToString();
                    ex1.TranID = TransID;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    #endregion
                }
                //throw ex;
            }
            finally
            {

            }
            return responsexml;
        }
        public XElement JuniperResponseSearch(XDocument reqxml, string Action, string hosturl, Int64 CustID, string TransID, int SuplID)
        {

            string pageContent = string.Empty;

            LoginDetail rc = new LoginDetail();
            XElement responsexml = null;
            DateTime starttime = DateTime.Now;
            try
            {
                string request = reqxml.ToString();
                string host = hosturl;
                string soapaction = "http://www.juniper.es/webservice/2007/" + Action;
                string url = host;
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myhttprequest.Method = "POST";
                myhttprequest.Headers.Add(@"SOAPAction:" + soapaction + "");
                myhttprequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                byte[] data = Encoding.ASCII.GetBytes(request);
                myhttprequest.ContentType = "text/xml;charset=UTF-8";
                myhttprequest.ContentLength = data.Length;
                myhttprequest.KeepAlive = true;
                Stream requestStream = myhttprequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);

                requestStream.Flush();
                requestStream.Close();
               
                
                HttpWebResponse myhttpresponse = (HttpWebResponse)myhttprequest.GetResponse();
                Stream responseStream = myhttpresponse.GetResponseStream();
                
                StreamReader myReader = new StreamReader(responseStream, Encoding.Default);
                pageContent = myReader.ReadToEnd();


                myReader.Close();
                responseStream.Close();
                myhttpresponse.Close();

                try
                {
                    // to remove non printable contents
                    //pageContent = Regex.Replace(pageContent, @"\p{C}+", string.Empty);
                }
                catch { }

                //pageContent = CommonHelper.RemoveInvalidXmlChars(pageContent);
                //pageContent = CommonHelper.CleanInvalidXmlChars(pageContent);
                pageContent = this.RemoveInvalidXmlCharsSearch(pageContent);
                responsexml = XElement.Parse(pageContent);

            }
            catch (Exception ex)
            {
                //XElement responseRNS = removeAllNamespaces(pageContent);
                try
                {
                    XElement requestRNS = removeAllNamespaces(reqxml.Root);
                    APILogDetail log = new APILogDetail();
                    log.customerID = CustID;
                    log.TrackNumber = TransID;
                    log.SupplierID = SuplID;
                    log.logrequestXML = requestRNS.ToString();
                    log.logresponseXML = pageContent.ToString();
                    log.LogType = Action;
                    log.logMsg = ex.Message.ToString();
                    log.LogTypeID = 1;
                    log.StartTime = starttime;
                    log.EndTime = DateTime.Now;
                    SaveAPILog savelog = new SaveAPILog();
                    savelog.SaveAPILogwithResponseError(log);
                }
                catch (Exception exApi)
                {
                    #region Exception
                    CustomException ex1 = new CustomException(exApi);
                    ex1.MethodName = Action;
                    ex1.PageName = "JuniperOutward";
                    ex1.CustomerID = CustID.ToString();
                    ex1.TranID = TransID;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(ex1);
                    #endregion
                }
                //throw ex;
                return null;
            }
            finally
            {

            }
            return responsexml;
        }

        public static Encoding GetFileEncoding(string srcFile)
        {

            // *** Use Default of Encoding.Default (Ansi CodePage)

            Encoding enc = Encoding.Default;



            // *** Detect byte order mark if any - otherwise assume default

            byte[] buffer = new byte[5];

            FileStream file = new FileStream(srcFile, FileMode.Open);

            file.Read(buffer, 0, 5);

            file.Close();



            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)

                enc = Encoding.UTF8;

            else if (buffer[0] == 0xfe && buffer[1] == 0xff)

                enc = Encoding.Unicode;

            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)

                enc = Encoding.UTF32;

            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)

                enc = Encoding.UTF7;



            return enc;

        }
        public static string ReadAllText(string path, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                return reader.ReadToEnd();
            }
        }
        private string RemoveInvalidXmlCharsSearch(string content)
        {
            //return new string(content.Where(ch => System.Xml.XmlConvert.IsXmlChar(ch)).ToArray());

            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(content, r, "", RegexOptions.Compiled);



        }
        public string RemoveInvalidXmlChars(string content)
        {
            //return new string(content.Where(ch => System.Xml.XmlConvert.IsXmlChar(ch)).ToArray());

            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(content, r, "", RegexOptions.Compiled);



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