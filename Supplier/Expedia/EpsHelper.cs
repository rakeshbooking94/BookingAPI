using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TravillioXMLOutService.Supplier.Expedia
{
    public static class EpsHelper
    {
        public static string getAuthHeader(string apiKey, string secret)
        {
            long ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;

            var bytes = Encoding.UTF8.GetBytes(apiKey + secret + ts);
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                var signature = hashedInputStringBuilder.ToString();
                var authHeaderValue = "EAN APIKey=" + apiKey + ",Signature=" + signature + ",timestamp=" + ts;
                return authHeaderValue.ToLower();
            }
        }
        public static string ModifyToEpsDate(this string strDate)
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
        public static string epsOccupancy(this List<XElement> roompaxlist)
        {
            string occupancy = string.Empty;
            foreach (var roompax in roompaxlist)
            {
                string roomOcpy = string.Empty;
                roomOcpy = "occupancy=" + roompax.Element("Adult").Value;
                string chAge = string.Empty;
                foreach (var child in roompax.Descendants("ChildAge").ToList())
                {
                    chAge = string.IsNullOrEmpty(chAge) ? child.Value : chAge + "," + child.Value;
                }
                if (!string.IsNullOrEmpty(chAge))
                {
                    roomOcpy = roomOcpy + "-" + chAge;
                }
                occupancy = string.IsNullOrEmpty(occupancy) ? roomOcpy : occupancy + "&" + roomOcpy;
            }
            return occupancy;
        
        }
        public static List<string> epsPaxList(this List<XElement> roompaxlist)
        {
            List<string> occupancy = new List<string>();
            foreach (var roompax in roompaxlist)
            {
                string roomOcpy = string.Empty;
                roomOcpy =  roompax.Element("Adult").Value;
                string chAge = string.Empty;
                foreach (var child in roompax.Descendants("ChildAge").ToList())
                {
                    chAge = string.IsNullOrEmpty(chAge) ? child.Value : chAge + "," + child.Value;
                }
                if (!string.IsNullOrEmpty(chAge))
                {
                    roomOcpy = roomOcpy + "-" + chAge;
                }
                occupancy.Add(roomOcpy);
            }
            return occupancy;

        }

        public static string RoomOcp(this XElement roompax)
        {
            string roomOcpy = string.Empty;
            roomOcpy = roompax.Element("Adult").Value;
            string chAge = string.Empty;
            foreach (var child in roompax.Descendants("ChildAge").ToList())
            {
                chAge = string.IsNullOrEmpty(chAge) ? child.Value : chAge + "," + child.Value;
            }
            if (!string.IsNullOrEmpty(chAge))
            {
                roomOcpy = roomOcpy + "-" + chAge;
            }
            return roomOcpy;
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
        public static int ModifyToStar(this string str)
        {
            int rating = 0;
            if (!string.IsNullOrEmpty(str))
            {
                rating = (int)Convert.ToDecimal(str);
            }
            return rating;

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
        public static DateTime EpsCxlDate(this string strDate)
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
        public static string getEpsProperties(this List<string> propertyList)
        {
            string EpsProperty = string.Empty;
            if (propertyList != null)
            {
                foreach (string prop in propertyList)
                {
                    EpsProperty = EpsProperty + (string.IsNullOrEmpty(EpsProperty) ? "property_id=" + prop : "&property_id=" + prop);
                }
            }
            return EpsProperty;
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
        public static List<List<T>> SplitPropertyList<T>(this List<T> htlList, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < htlList.Count; i += size)
                list.Add(htlList.GetRange(i, Math.Min(size, htlList.Count - i)));
            return list;
        }
        public static DateTime StringToDate(this string strDate)
        {
            if (!string.IsNullOrEmpty(strDate))
            {
                DateTime dte = DateTime.ParseExact(strDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return dte.Date;

            }
            else
            {
                return DateTime.MinValue;
            }

        }
    }
}