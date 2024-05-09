using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Models.TravelGate
{
    public class TGDataAccess : IDisposable
    {
        static SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adap;
        private void connection()
        {
            string connect = ConfigurationManager.ConnectionStrings["INGMContext"].ToString();
            con = new SqlConnection(connect);
            con.Open();
        }
        public DataTable CityMapping(string CityID, string SuplID)
        {
            DataTable dt = new DataTable("City Mapping Details");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_GadouCityMapping", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@CityID", CityID);
            //    cmd.Parameters.AddWithValue("@SupplierID", SuplID);
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
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GadouCityMapping", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityID", CityID);
                        cmd.Parameters.AddWithValue("@SupplierID", SuplID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();
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
        public DataTable HotelList (string CityID,int supID)
        {
            DataTable dt = new DataTable("Hotel List Details");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_SmyHotelList", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@CityID", CityID);
            //    cmd.Parameters.AddWithValue("@supID", supID); 
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
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_SmyHotelList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityID", CityID);
                        cmd.Parameters.AddWithValue("@supID", supID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();
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
        public DataTable HotelDetails(string HotelIDs,int supID)
        {
            DataTable dt = new DataTable("Hotel Details");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_SmyHotelDetails", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@HotelID", HotelIDs);
            //    cmd.Parameters.AddWithValue("@supID", supID);
            //    cmd.Parameters.Add("@retVal", SqlDbType.Int);
            //    cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_SmyHotelDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HotelID", HotelIDs);
                        cmd.Parameters.AddWithValue("@supID", supID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();
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
        public DataTable HotelDetails_new(string CityIDs, int supID)
        {
            DataTable dt = new DataTable("Hotel Details");
            try
            {
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_SmyHotelDetails_new", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityID", CityIDs);
                        cmd.Parameters.AddWithValue("@supID", supID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();
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
        public DataTable SingleHotelDetails(string HotelID,int supID)
        {
            DataTable dt = new DataTable("Single Hotel Details");
            //try
            //{
            //    connection();
            //    SqlCommand cmd = new SqlCommand("SP_SmySingleHotelDetails", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@HotelID", HotelID);
            //    cmd.Parameters.AddWithValue("@supID", supID);
            //    cmd.Parameters.Add("@retVal", SqlDbType.Int);
            //    cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
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
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_SmySingleHotelDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HotelID", HotelID);
                        cmd.Parameters.AddWithValue("@supID", supID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();
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
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetLog_XMLs", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue(@"transid", trackID);
                        cmd.Parameters.AddWithValue("@logtypeID", LogtypeID);
                        cmd.Parameters.AddWithValue("@Supplier", SupplierID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();
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
        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}