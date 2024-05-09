using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.GoGlobals
{
    public static class GoglobalHelper
    {
        public static List<List<T>> TSplitPropertyList<T>(this List<T> htlList, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < htlList.Count; i += size)
                list.Add(htlList.GetRange(i, Math.Min(size, htlList.Count - i)));
            return list;
        } 

    }
}