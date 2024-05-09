using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace TravillioXMLOutService.Supplier.Expedia
{
    public class ExpediaStatic
    {
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adap;
        public DataTable GetCityCode(string CityCode, string CountryID, int SupplierId)
        {
            DataTable dt = new DataTable("CityCode");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetExpediaCity", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@cityId", CityCode);
                        cmd.Parameters.AddWithValue("@countryId", CountryID);
                        cmd.Parameters.AddWithValue("@suplId", SupplierId);
                        using (adap = new SqlDataAdapter(cmd))
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

        public DataTable GetStaticHotels(string HotelCode, string HotelName,string CityName, string CountryCode, int MinRating, int MaxRating)
        {
            DataTable dt = new DataTable("Hotels");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetExpediaHotels", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HotelCode", HotelCode);
                        cmd.Parameters.AddWithValue("@HotelName", HotelName);
                        cmd.Parameters.AddWithValue("@cityCode", CityName);
                        cmd.Parameters.AddWithValue("@CountryCode", CountryCode);
                        cmd.Parameters.AddWithValue("@MinStarRating", MinRating);
                        cmd.Parameters.AddWithValue("@MaxStarRating", MaxRating);
                        using (adap = new SqlDataAdapter(cmd))
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
        public DataTable GetHotelDetails(string HotelId)
        {
            DataTable dt = new DataTable("HotelDetail");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetExpediaHotelDetail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HotelCode", HotelId);
                        using (adap = new SqlDataAdapter(cmd))
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


        public string GetHotelPolicy(string HotelCode)
        {
            string hotelpolicy = string.Empty;
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetExpediaHotelPolicy", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HotelCode", HotelCode);
                                               
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            conn.Close();
                            hotelpolicy = dt.Rows[0]["HotelPolicy"].ToString().Trim();
                            return hotelpolicy;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return hotelpolicy;

            }
        }
    }
}