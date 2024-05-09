using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Models.Restel
{
    public class RestelLogAccess
    {
        static SqlConnection con;
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adap;
        public void connection()
        {
            string connect = ConfigurationManager.ConnectionStrings["INGMContext"].ToString();
            con = new SqlConnection(connect);
            con.Open();
        }
        public DataTable GetMiki_RoomList(string trackID)
        {
            DataTable dt = new DataTable("RoomList");
            
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetRestel_RoomList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@transid", trackID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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
        public DataTable RestelCityMapping(string CityId, string SupplierID)
        {
            DataTable dt = new DataTable("Details");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_GadouCityMapping", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@CityID", CityId);
            //    cmd.Parameters.AddWithValue("@SupplierID", SupplierID);
            //    cmd.Parameters.Add("@retVal", SqlDbType.Int);
            //    cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
            //    connection();
            //    dt.Load(cmd.ExecuteReader());
            //    return dt;
            //}
            //catch
            //{
            //    return dt;
            //}
            //finally
            //{
            //    con.Close();
            //}

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GadouCityMapping", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityID", CityId);
                        cmd.Parameters.AddWithValue("@SupplierID", SupplierID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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
        public DataTable GetLog(string trackID, int LogtypeID, int SupplierID)
        {
            DataTable dt = new DataTable("LogTable");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_GetLog_XMLs", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue(@"transid", trackID);
            //    cmd.Parameters.AddWithValue("@logtypeID", LogtypeID);
            //    cmd.Parameters.AddWithValue("@Supplier", SupplierID);
            //    cmd.Parameters.Add("@retVal", SqlDbType.Int);
            //    cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
            //    connection();
            //    dt.Load(cmd.ExecuteReader());
            //    return dt;
            //}
            // catch(Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    con.Close();
            //}

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetLog_XMLs", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue(@"transid", trackID);
                        cmd.Parameters.AddWithValue("@logtypeID", LogtypeID);
                        cmd.Parameters.AddWithValue("@Supplier", SupplierID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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
        public DataTable GetHotelsList(string CityID, string HotelId, string HotelName)
        {
            DataTable dt = new DataTable("Hotels");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_GetRestel_HotelList_Test", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@CityID", CityID);
            //    cmd.Parameters.Add("@retVal", SqlDbType.Int);
            //    cmd.Parameters.AddWithValue("@HotelCode", HotelId);
            //    cmd.Parameters.AddWithValue("@HotelName", HotelName);
            //    cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
            //    connection();
            //    dt.Load(cmd.ExecuteReader());
            //    return dt;
            //}
            //catch(Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    con.Close();
            //}

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetRestel_HotelList_Test", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityID", CityID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters.AddWithValue("@HotelCode", HotelId);
                        cmd.Parameters.AddWithValue("@HotelName", HotelName);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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
        public DataTable GetRestelFacilities(string CityID)
        {
            DataTable dt = new DataTable("Facilities");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_GetRestel_Facilities", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@CityID", CityID);
            //    cmd.Parameters.Add("@retVal", SqlDbType.Int);
            //    cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
            //    connection();
            //    dt.Load(cmd.ExecuteReader());
            //    return dt;
            //}
            //catch(Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    con.Close();
            //}

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetRestel_Facilities", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityID", CityID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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
        public DataTable GetRestelHotelDetails(string HotelID)
        {
            DataTable dt = new DataTable("Details");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_GetRestel_HotelDetails", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@HotelID", HotelID);
            //    cmd.Parameters.Add("@retVal", SqlDbType.Int);
            //    cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
            //    connection();
            //    dt.Load(cmd.ExecuteReader());
            //    return dt;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    con.Close();
            //}

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetRestel_HotelDetails", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HotelID", HotelID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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