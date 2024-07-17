using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.DataAccess;
using TravillioXMLOutService.Hotel.Model;
using Newtonsoft.Json.Linq;

namespace TravillioXMLOutService.Hotel.Repository
{
    public class HotelRepository
    {
        public List<HotelModel> GetAllHotelList(HotelSearch req)
        {
            List<HotelModel> lst = new List<HotelModel>();
            try
            {
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@SupplierId", req.SupplierId);
                param[1] = new SqlParameter("@HotelCode", req.HotelId);
                param[2] = new SqlParameter("@HotelName", req.CityCode);
                param[3] = new SqlParameter("@CountryCode", req.CountryCode);
                param[4] = new SqlParameter("@MinStarRating", req.MinRating);
                param[5] = new SqlParameter("@MaxStarRating", req.MaxRating);
                var ds = SqlHelper.ExecuteDataset(TravayooConnection.ServiceConnection,
                     CommandType.StoredProcedure, "GetAllHotelSearchProc", param);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].AsEnumerable().Select(x => new HotelModel
                    {
                        HotelId = x["hotelcode"] != DBNull.Value ? x["hotelcode"].ToString() : "",
                        HotelName = x["hotelname"] != DBNull.Value ? x["hotelname"].ToString() : "",
                        Rating = x["rating"] != DBNull.Value ? x["rating"].ToString() : "",
                        Latitude = x["Latitude"] != DBNull.Value ? x["Latitude"].ToString() : "",
                        Longitude = x["longitude"] != DBNull.Value ? x["longitude"].ToString() : "",
                        HotelAddress = x["address"] != DBNull.Value ? x["address"].ToString() : "",
                        CityName = x["city"] != DBNull.Value ? x["city"].ToString() : "",
                        CountryCode = x["country_code"] != DBNull.Value ? x["country_code"].ToString() : "",
                        HotelImage = x["image"] != DBNull.Value ? x["image"].ToString() : "",
                        //PostalCode = x["HotelId"] != DBNull.Value ? x["HotelId"].ToString() : "",
                    }).ToList();
                }

            }
            catch
            {
                throw new Exception("Hotel not avaialabe  in Giata");

            }
            return lst;
        }

    }
}