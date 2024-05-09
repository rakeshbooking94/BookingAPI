﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TravillioXMLOutService.Supplier.IOL
{
    
    public static class IOLHelper
    {
        public static long ModifyToLong(this string str)
        {
            long num = 0;
            if (!string.IsNullOrEmpty(str))
            {
                num = long.Parse(str);
            }
            return num;

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
        public static List<List<T>> SplitPropertyList<T>(this List<T> htlList, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < htlList.Count; i += size)
                list.Add(htlList.GetRange(i, Math.Min(size, htlList.Count - i)));
            return list;
        }
        public static int ModifyToStar(this string str)
        {
            try
            {
                int rating = 0;
                if (!string.IsNullOrEmpty(str))
                {
                    rating = (int)Convert.ToDecimal(str);
                }
                return rating;
            }
            catch { return 0; }

        }
        public static string ConvertHotelCodewith_underscoreIOL(this string hotelcode)
        {
            return hotelcode.Replace('-', '_');
        }        
        public static string ConvertHotelCodewith_dashIOL(this string hotelcode)
        {
            return hotelcode.Replace('_', '-');
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
        public static DateTime GetDateTime(this string strDateTime, string dateTimeFormate)
        {
            DateTime dte = DateTime.ParseExact(strDateTime.Trim(), dateTimeFormate, CultureInfo.InvariantCulture);
            return dte;
        }
    }
}