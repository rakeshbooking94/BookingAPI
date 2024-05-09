using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Xml.Linq;
using System.Xml;
using System.Configuration;
using System.Text.RegularExpressions;

namespace TravillioXMLOutService.Supplier.Welcomebeds
{
    public static class WBHelper
    {
        public static XElement RemoveXmlns(this XElement doc)
        {
            doc.Descendants().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();
            foreach (var elem in doc.Descendants())
                elem.Name = elem.Name.LocalName;

            return doc;
        }
        public static string WBDateString(this string strDate)
        {
            if (!string.IsNullOrEmpty(strDate))
            {
                DateTime dt = DateTime.ParseExact(strDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return dt.ToString("yyyy-MM-dd");
            }
            else
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd");
            }
        }
        public static int ModifyToInt(this string str)
        {
            int num = 0;
            if (!string.IsNullOrEmpty(str))
            {
                num = int.Parse(str);
            }
            return num;

        }
        public static long ModifyToLong(this string str)
        {
            long num = 0;
            if (!string.IsNullOrEmpty(str))
            {
                num = long.Parse(str);
            }
            return num;

        }
        public static decimal ModifyToDecimal(this string str)
        {
            decimal num = 0;
            if (!string.IsNullOrEmpty(str))
            {
                num = Convert.ToDecimal(str);
            }
            return num;

        }
        public static DateTime ConvertToDate(this string strDate)
        {
            if (!string.IsNullOrEmpty(strDate))
            {
                DateTime dt = DateTime.ParseExact(strDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return dt;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public static DateTime WBCxlDate(this string strDate)
        {
            if (!string.IsNullOrEmpty(strDate))
            {

                //string offset = strDate.Split('+')[1];
                DateTime dt = DateTime.ParseExact(strDate.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                DateTimeOffset wboffset = new DateTimeOffset(dt.Ticks, new TimeSpan(-1, 0, 0));
                DateTime localdate = wboffset.LocalDateTime;
                return localdate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public static DateTime ModifyToCxlDate(this string strDate)
        {
            if (!string.IsNullOrEmpty(strDate))
            {

                //string offset = strDate.Split('+')[1];
                DateTime dt = DateTime.ParseExact(strDate.Trim(), "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                DateTimeOffset wboffset = new DateTimeOffset(dt.Ticks, new TimeSpan(-1, 0, 0));
                DateTime localdate = wboffset.LocalDateTime;
                return localdate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public static Dictionary<DateTime, decimal> AddCxlPolicy(this Dictionary<DateTime, decimal> cxlPolicies, DateTime Cxldate, decimal cxlCharges)
        {
            if (cxlPolicies.Count == 0)
            {
                cxlPolicies.Add(Cxldate, cxlCharges);
            }
            else
            {
                int count = cxlPolicies.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = cxlPolicies.ElementAt(i);
                    if (item.Key == Cxldate)
                    {
                        cxlPolicies[item.Key] = item.Value + cxlCharges;
                        
                    }
                    else if (item.Key < Cxldate)
                    {
                        if (!cxlPolicies.ContainsKey(Cxldate))
                        {
                            cxlPolicies.Add(Cxldate, cxlCharges);
                        }
                        
                    }
                    else
                    {
                        if (!cxlPolicies.ContainsKey(Cxldate))
                        {
                            cxlPolicies.Add(Cxldate, cxlCharges);
                        }
                        //cxlPolicies[item.Key] = item.Value + cxlCharges;
                        
                    }
                }

            }
            return cxlPolicies;
        }
        public static string WBGender(this string strTitle)
        {
            if (!string.IsNullOrEmpty(strTitle))
            {
                if (strTitle == "Ms" || strTitle == "Mrs")
                {
                    return "Female";
                }
                else
                {
                    return "Male";
                }
            }
            else
            {
                return "Male";
            }
        }
        public static XElement getHotelImages(this string imagesStr)
        {
            if (string.IsNullOrEmpty(imagesStr))
            {
                return new XElement("Images");
            }
            else
            {
                XElement images = XElement.Parse(imagesStr);
                return images;

            }
        }
        public static XElement getHotelFacilities(this string facilityStr)
        {
            if (string.IsNullOrEmpty(facilityStr))
            {
                return new XElement("Facilities");
            }
            else
            {
                XElement facilities = XElement.Parse(facilityStr);
                return facilities;

            }
        }
        public static string replaceAscii(this string str)
        {
            str = Regex.Replace(str, @"[^\u0000-\u007F]", string.Empty);
            str = str.Replace("\"", string.Empty);

            str = str.Replace(@"\\", string.Empty);
            ////try
            ////{

            ////}
            ////catch
            ////{
            //// return "";
            ////}
            return str;

        }


        public static List<List<T>> SplitPropertyList<T>(this List<T> htlList, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < htlList.Count; i += size)
                list.Add(htlList.GetRange(i, Math.Min(size, htlList.Count - i)));
            return list;
        } 

    }
}