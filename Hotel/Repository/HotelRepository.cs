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
using Newtonsoft.Json;
using System.Configuration;

namespace TravillioXMLOutService.Hotel.Repository
{
    public class HotelRepository
    {





        public DataTable GetStaticHotels(int SupplierId, string HotelCode, string HotelName, string CityName, string CountryCode, int MinRating, int MaxRating)
        {
            DataTable dt = new DataTable("Hotels");
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (var cmd = new SqlCommand("GetAllHotelSearchProc", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
                        cmd.Parameters.AddWithValue("@HotelCode", HotelCode);
                        cmd.Parameters.AddWithValue("@HotelName", HotelName);
                        cmd.Parameters.AddWithValue("@cityCode", CityName);
                        cmd.Parameters.AddWithValue("@CountryCode", CountryCode);
                        cmd.Parameters.AddWithValue("@MinStarRating", MinRating);
                        cmd.Parameters.AddWithValue("@MaxStarRating", MaxRating);
                        using (var adap = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            adap.Fill(dt);
                            conn.Close();
                            return dt;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return dt;

            }


        }








        public List<HotelModel> GetAllHotelList(HotelSearch req)
        {
            List<HotelModel> lst = new List<HotelModel>();
            try
            {
                //SqlParameter[] param = new SqlParameter[6];
                //param[0] = new SqlParameter("@SupplierId", req.SupplierId);
                //param[1] = new SqlParameter("@HotelCode", req.HotelId);
                //param[2] = new SqlParameter("@HotelName", req.CityCode);
                //param[3] = new SqlParameter("@CountryCode", req.CountryCode);
                //param[4] = new SqlParameter("@MinStarRating", req.MinRating);
                //param[5] = new SqlParameter("@MaxStarRating", req.MaxRating);
                //var ds = SqlHelper.ExecuteDataset(TravayooConnection.ServiceConnection,
                //     CommandType.StoredProcedure, "GetAllHotelSearchProc", param);



                var dt = GetStaticHotels(req.SupplierId, req.HotelId, req.HotelName, req.CityCode, req.CountryCode, req.MinRating, req.MaxRating);


                if (dt.Rows.Count > 0)
                {
                    lst = dt.AsEnumerable().Select(x => new HotelModel
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

        public string GetStaticHotel(string HotelCode)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (var cmd = new SqlCommand("RateHawkProc", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", 1);
                        cmd.Parameters.AddWithValue("@SupplierId", 24);
                        cmd.Parameters.AddWithValue("@HotelId", HotelCode);
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var content = rdr.GetString(0);
                                return content;
                            }
                        }
                        conn.Close();
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;

            }


        }










        public RTHWKHotelModel GetHotelDetail(string hotelId)
        {
            RTHWKHotelModel result = null;
            string json = this.GetStaticHotel(hotelId);
            try
            {
                result = JsonConvert.DeserializeObject<RTHWKHotelModel>(json);
                //SqlParameter[] param = new SqlParameter[3];
                //param[0] = new SqlParameter("@flag", 1);
                //param[1] = new SqlParameter("@SupplierId", 24);
                //param[2] = new SqlParameter("@HotelId", hotelId);

                //using (var reader = SqlHelper.ExecuteReader(TravayooConnection.ServiceConnection,
                //     CommandType.StoredProcedure, "RateHawkProc", param))
                //{
                //    if (reader.Read())
                //    {
                //        json = reader.GetString(reader.GetOrdinal("StaticContent"));
                       
                //    }
                //}
            }
            catch
            {
                throw new Exception("Hotel data not avaialabe  in  database");
            }
            return result;
        }











    }
}