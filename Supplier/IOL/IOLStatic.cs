using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Supplier.IOL
{
    public class IOLStatic
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
                    using (cmd = new SqlCommand("usp_GetIOLCity", conn))
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
        public DataTable GetStaticHotels(string HotelCode, string HotelName, string CityName, string CountryCode, int MinRating, int MaxRating)
        {
            DataTable dt = new DataTable("Hotels");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetIOLHotels", conn))
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
        //public DataTable getRoomfromroom(string tracknumber, string preID)
        //{
        //    DataTable dt = new DataTable("Hotels");
        //    try
        //    {
        //        using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
        //        {
        //            using (cmd = new SqlCommand("usp_GetIOLHotelFromSearch", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@trackNumber", tracknumber);
        //                cmd.Parameters.AddWithValue("@preID", preID);
        //                cmd.Parameters.AddWithValue("@type", 1);
        //                using (adap = new SqlDataAdapter(cmd))
        //                {
        //                    conn.Open();
        //                    adap.Fill(dt);
        //                    conn.Close();
        //                    return dt;
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return dt;

        //    }


        //}
        public DataTable GetCXLPolicy(string tracknumber, string preID)
        {
            DataTable dt = new DataTable("Policy");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetIOLHotelFromSearch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@trackNumber", tracknumber);
                        cmd.Parameters.AddWithValue("@preID", preID);
                        cmd.Parameters.AddWithValue("@type", 1);
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
        public DataTable GetSingleHotelDetail(string HotelCode)
        {
            DataTable dt = new DataTable("Hotels");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetIOLSingleHotelDetail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HotelCode", HotelCode);
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
        public DataTable GetBookingResponse(string tracknumber)
        {
            DataTable dt = new DataTable("BookingResponse");
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("usp_GetBookingResponse", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@trackNumber", tracknumber);
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
    }
}