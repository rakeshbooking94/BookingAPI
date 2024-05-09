using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TravillioXMLOutService.Supplier.Welcomebeds
{
    public class WBStaticData
    {
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adap;
        public DataTable GetWBCityCode(string CityCode, int SupplierId)
        {
            DataTable dt = new DataTable("CityCode");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetTravcoCity", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@cityId", CityCode);
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
      
        public DataTable GetWBStaticHotels(string CityCode, string CountryCode, string TownCode, int MinRating, int MaxRating)
        {
            DataTable dt = new DataTable("Hotels");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetWBHotels", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityCode", CityCode);
                        cmd.Parameters.AddWithValue("@CountryCode", CountryCode);
                        cmd.Parameters.AddWithValue("@TownCode", TownCode);
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
        public DataTable GetWBHotelDetails(string HotelId)
        {
            DataTable dt = new DataTable("HotelDetail");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetWBHotelDetail", conn))
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


        public string GetWBLog(string trackID, int LogtypeID, int SupplierId)
        {
            string ResXml = string.Empty;
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetVotSearchResponse", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TrackNumber", trackID);
                        cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
                        cmd.Parameters.AddWithValue("@logTypeId", LogtypeID);
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            conn.Close();
                            ResXml = dt.Rows[0]["logresponseXML"].ToString().Trim();
                            return ResXml;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ResXml;

            }
        }
    }
}