
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using TravillioXMLOutService.Hotel.Model;

namespace TravillioXMLOutService.Hotel.Helper
{

    public static class RTHWKHelper
    {
        static readonly XElement _credentialList;
        static RTHWKHelper()
        {
            _credentialList = XElement.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/SupplierCredential/SupplierCredentials.xml"));
        }
        public static RTHWKCredentials ReadCredential(this string custId, int supplId)
        {
            RTHWKCredentials _credential = null;
            try
            {
                if (!string.IsNullOrEmpty(custId))
                {
                    var data = _credentialList.Descendants("credential").Where(x =>
                    x.Attribute("customerid").Value == custId &&
                    x.Attribute("supplierid").Value == supplId.ToString()).FirstOrDefault();
                    _credential = new RTHWKCredentials
                    {
                        BaseUrl = "https://api.worldota.net/api/b2b/v3/",
                        ClientId = data.Element("ClientId").Value,
                        SecretKey = data.Element("SecretKey").Value,
                        CustomerId = data.Attribute("customerid").Value,
                        SupplierId = data.Attribute("supplierid").GetValueOrDefault(0),
                        Currency = data.Element("Currency").Value,
                        Culture = data.Element("Culture").Value
                    };
                }
                return _credential;
            }
            catch
            {
                return null;
            }
        }


        public static string RTHWKDate(this string strDate)
        {
            DateTime dte = DateTime.ParseExact(strDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dte.ToString("yyyy-MM-dd");
        }
        public static string RTHWKResidenc(this string str)
        {
            return str.ToLower();
        }

        public static string RTHWKCurrency(this string str)
        {
            return "USD";
        }

        public static string RTHWKlanguage()
        {
            return "en";
        }

        public static List<int> Children(this XAttribute item)
        {
            List<int> lst = new List<int>();

            if (item != null)
            {
                lst = item.Value.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            }

            return lst;
        }



        public static string CxlDate(this XElement item)
        {
            string date = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd");
            if (item.Element("fromDate") != null)
            {
                date = item.Element("fromDate").Value;
            }
            else if (item.Element("toDate") != null)
            {
                date = item.Element("toDate").Value;
            }
            return date;

        }


        public static ApiAction GetAction(this string ElementName)
        {
            if (ElementName == "SearchRequest")
            {
                return ApiAction.Search;
            }
            else if (ElementName == "PrebookRequest")
            {
                return ApiAction.PreBook;

            }
            else if (ElementName == "bookRequest")
            {
                return ApiAction.Book;
            }
            else if (ElementName == "TransferCXLPolicyRequest")
            {
                return ApiAction.CXLPolicy;
            }
            else
            {
                return ApiAction.Cancel;
            }

        }







        public static string cleanForJSON(this string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            System.Text.StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        int cint = Convert.ToInt32(c);
                        if (cint >= 123 || (cint >= 32 && cint <= 46) || (cint >= 58 && cint <= 64) || ((cint >= 91 && cint <= 96) && cint != 92))
                        {
                            sb.Append(String.Format("\\u{0:x4} ", cint).Trim());
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }


        public static string cleanFormJSON(this string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }
            str = Regex.Replace(str, @"\\u(?<Value>[a-zA-Z0-9]{4})",
         m =>
         {
             return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
         });

            str = str.Replace("\\n", "\n");
            str = str.Replace("\\b", "\b");
            str = str.Replace("\\f", "\f");
            str = str.Replace("\\t", "\t");
            str = str.Replace("\\r", "\r");
            str = str.Replace("\\", "/");
            return str.ToString();

        }


        public static XElement MergPolicy(this List<XElement> PolicyList, decimal _totalAmount)
        {
            List<XElement> cxlList = new List<XElement>();
            IEnumerable<XElement> dateLst = PolicyList.Descendants("cancellationPolicy").
               GroupBy(r => new
               {
                   r.Attribute("lastDate").Value.GetDateTime("yyyy-MM-ddTHH:mm:ss").Date,
                   noshow = r.Attribute("noShow").Value
               }).Select(y => y.First()).
               OrderBy(p => p.Attribute("lastDate").Value.GetDateTime("yyyy-MM-ddTHH:mm:ss"));


            if (dateLst.Count() > 0)
            {
                foreach (var item in dateLst)
                {
                    string date = item.Attribute("lastDate").Value;
                    string noShow = item.Attribute("noShow").Value;
                    decimal datePrice = 0.0m;
                    foreach (var rm in PolicyList)
                    {
                        var prItem = rm.Descendants("cancellationPolicy").
                       Where(pq => (pq.Attribute("noShow").Value == noShow && pq.Attribute("lastDate").Value == date)).
                       FirstOrDefault();
                        if (prItem != null)
                        {
                            var price = prItem.Attribute("amount").Value;
                            datePrice += Convert.ToDecimal(price);
                        }
                        else
                        {
                            if (noShow == "1")
                            {
                                datePrice += _totalAmount;
                            }
                            else
                            {
                                var lastItem = rm.Descendants("cancellationPolicy").
                                    Where(pq => (pq.Attribute("noShow").Value == noShow && pq.Attribute("lastDate").Value.GetDateTime("yyyy-MM-ddTHH:mm:ss") < date.GetDateTime("yyyy-MM-ddTHH:mm:ss")));
                                if (lastItem.Count() > 0)
                                {
                                    var lastDate = lastItem.Max(y => y.Attribute("lastDate").Value);
                                    var lastprice = rm.Descendants("cancellationPolicy").
                                        Where(pq => (pq.Attribute("noShow").Value == noShow && pq.Attribute("lastDate").Value == lastDate)).
                                        FirstOrDefault().Attribute("amount").Value;
                                    datePrice += Convert.ToDecimal(lastprice);
                                }
                            }
                        }
                    }
                    XElement pItem = new XElement("cancellationPolicy",
                        new XAttribute("lastDate", date),
                        new XAttribute("amount", datePrice),
                        new XAttribute("noShow", noShow));
                    cxlList.Add(pItem);
                }

                cxlList = cxlList.GroupBy(x => new { x.Attribute("lastDate").Value.GetDateTime("yyyy-MM-ddTHH:mm:ss").Date, x.Attribute("noShow").Value }).
                    Select(y => new XElement("cancellationPolicy",
                        new XAttribute("lastDate", y.Key.Date.ToString("d MMM, yy")),
                        new XAttribute("amount", y.Max(p => Convert.ToDecimal(p.Attribute("amount").Value))),
                        new XAttribute("noShow", y.Key.Value))).OrderBy(p => p.Attribute("lastDate").Value.GetDateTime("d MMM, yy").Date).ToList();



                var fItem = cxlList.FirstOrDefault();

                if (Convert.ToDecimal(fItem.Attribute("amount").Value) != 0.0m)
                {
                    cxlList.Insert(0, new XElement("cancellationPolicy", new XAttribute("lastDate", fItem.Attribute("lastDate").Value.GetDateTime("d MMM, yy").AddDays(-1).Date.ToString("d MMM, yy")), new XAttribute("amount", "0.00"), new XAttribute("noShow", "0")));
                }
            }
            XElement cxlItem = new XElement("cancellationPolicyList", cxlList);
            return cxlItem;
        }























        /// </summary>  
        #region Enums
        public enum ApiAction
        {
            //[Description("Searching")]
            Search = 1,
            //[Description("Cancellation Policy")]
            CXLPolicy = 3,
            //[Description("PreBooking")]
            PreBook = 4,
            //[Description("Booking")]
            Book = 5,
            //[Description("Cancellation")]
            Cancel = 6,

        }



        #endregion

    }

}