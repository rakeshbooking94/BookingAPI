
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml;
using TravillioXMLOutService.Common.DotW;

namespace TravillioXMLOutService.Hotel.Helper
{
    public static class CommonHelper
    {
        static readonly string travyoPath = ConfigurationManager.AppSettings["TravyoPath"];
        static readonly string dotWPath = ConfigurationManager.AppSettings["DotWPath"];
        static readonly int SuplId;
        static int requestNo = 0;

        public static string AlterFormat(this string strDate, string oldFormat, string newFormat)
        {
            DateTime dte = DateTime.ParseExact(strDate.Trim(), oldFormat, CultureInfo.InvariantCulture);
            return dte.ToString(newFormat);
        }


        public static DateTime GetDateTime(this string strDate, string strTime, string dateTimeFormate)
        {
            string dateTimeStr = strDate + " " + strTime;
            DateTime dte = DateTime.ParseExact(dateTimeStr.Trim(), dateTimeFormate, CultureInfo.InvariantCulture);
            return dte;
        }

        public static DateTime GetDateTime(this string strDateTime, string dateTimeFormate)
        {
            DateTime dte = DateTime.ParseExact(strDateTime.Trim(), dateTimeFormate, CultureInfo.InvariantCulture);
            return dte;
        }

        public static IEnumerable<List<T>> CrossJoinLists<T>(List<List<T>> listofObjects)
        {
            var result = from obj in listofObjects.First()
                         select new List<T> { obj };
            for (var i = 1; i < listofObjects.Count(); i++)
            {
                var iLocal = i;
                result = from obj in result
                         from obj2 in listofObjects.ElementAt(iLocal)
                         select new List<T>(obj) { obj2 };
            }
            return result;
        }

        public static string IsXmlDateString(this string strDate)
        {
            string date = string.Empty;
            if (strDate != null)
            {
                DateTime _Startdate;
                DateTime.TryParse(strDate, out _Startdate);
                date = _Startdate.ToString("dd-MM-yyyy");
            }
            return date;
        }
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }




        public static List<List<T>> SplitHotelList<T>(this List<T> htlList, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < htlList.Count; i += size)
                list.Add(htlList.GetRange(i, Math.Min(size, htlList.Count - i)));
            return list;
        }








        public static string IsNullString(this string str)
        {
            string result = string.Empty;
            if (str != null)
            {
                result = str;
            }
            return result;
        }
        public static decimal IsNullNumber(this decimal? item)
        {
            return item.HasValue ? item.Value : 0.00m; ;
        }

        public static int ToINT(this XAttribute item, int defaultValue = 0)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToInt32(item.Value);
        }
        public static string GetValueOrDefault(this XElement item, string defaultValue = null)
        {
            if (item == null)
                return defaultValue;
            else
                return item.Value;
        }

        public static int GetValueOrDefault(this XElement item, int defaultValue = 0)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToInt32(item.Value);
        }
        public static int GetValueOrDefault(this XAttribute item, int defaultValue = 0)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToInt32(item.Value);
        }
        public static string GetValueOrDefault(this XAttribute attribute, string defaultValue = null)
        {
            if (attribute == null)
                return defaultValue;
            else
                return attribute.Value;
        }

        public static decimal GetValueOrDefault(this XAttribute attribute, decimal defaultValue = 0.0m)
        {
            if (attribute == null)
                return defaultValue;
            else
                return Convert.ToDecimal(attribute.Value);
        }
        public static IEnumerable<XElement> DescendantsOrEmpty(this XElement item, XName name)
        {
            IEnumerable<XElement> result;
            if (item != null)
                result = item.Descendants(name).Count() != 0 ? item.Descendants(name) : Enumerable.Empty<XElement>();
            else
                result = Enumerable.Empty<XElement>();
            return result;
        }
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (T obj in source)
                {
                    return false;
                }
            }
            return true;
        }
        public static XElement RemoveAllNamespaces(this XElement xmlDocument)
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





        public static XElement RemoveXmlns(this XElement doc)
        {
            doc.Descendants().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();
            foreach (var elem in doc.Descendants())
                elem.Name = elem.Name.LocalName;

            return doc;
        }


        public static List<XElement> rmIndex(this IEnumerable<XElement> rmlst, string atrName)
        {
            List<XElement> lst = rmlst.ToList();
            int index = 0;
            lst.ForEach(r => r.Attribute(atrName).Value = (++index).ToString());
            return lst;
        }
        public static string Escape(this string input)
        {
            char[] toEscape = "\0\x1\x2\x3\x4\x5\x6\a\b\t\n\v\f\r\xe\xf\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1a\x1b\x1c\x1d\x1e\x1f\x2C\"\\".ToCharArray();
            string[] literals = @"\0,\x0001,\x0002,\x0003,\x0004,\x0005,\x0006,\a,\b,\t,\n,\v,\f,\r,\x000e,\x000f,\x0010,\x0011,\x0012,\x0013,\x0014,\x0015,\x0016,\x0017,\x0018,\x0019,\x001a,\x001b,\x001c,\x001d,\x001e,\x001f\x002C".Split(new char[] { ',' });

            int i = input.IndexOfAny(toEscape);
            if (i < 0) return input;

            var sb = new System.Text.StringBuilder(input.Length + 5);
            int j = 0;
            do
            {
                sb.Append(input, j, i - j);
                var c = input[i];
                if (c < 0x20) sb.Append(literals[c]); else sb.Append(@"\").Append(c);
            } while ((i = input.IndexOfAny(toEscape, j = ++i)) > 0);

            return sb.Append(input, j, input.Length - j).ToString();
        }



        public static string DistinctiveId()
        {
            var ticks = DateTime.Now.Ticks;
            var guid = Guid.NewGuid().ToString();
            var uniqueSessionId = ticks.ToString() + '-' + guid;
            return uniqueSessionId;
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

        public static List<List<T>> BreakIntoChunks<T>(List<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }
            List<List<T>> retVal = new List<List<T>>();
            while (list.Count > 0)
            {
                int count = list.Count > chunkSize ? chunkSize : list.Count;
                retVal.Add(list.GetRange(0, count));
                list.RemoveRange(0, count);
            }

            return retVal;
        }


        public static decimal AttributetoDecimal(this XAttribute item)
        {
            decimal result = 0.0m;
            if (item != null)
            {
                result = Convert.ToDecimal(item.Value);
            }
            return result;
        }
        public static decimal ElementtoDecimal(this XElement item)
        {
            decimal result = 0.0m;
            if (item != null)
            {
                result = Convert.ToDecimal(item.Value);
            }
            return result;
        }
        public static decimal StringDecimal(this string item)
        {
            decimal result = 0.0m;
            if (item != null)
            {
                result = Convert.ToDecimal(item);
            }
            return result;
        }

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\0x02\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

    }
}