using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using TravillioXMLOutService.Models.GoGlobal;

namespace TravillioXMLOutService.Models
{
    public partial class GoGlobal_Static
    {
        static SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adap;
        private void connecttion()
        {
            string constr = ConfigurationManager.ConnectionStrings["INGMContext"].ToString();
            con = new SqlConnection(constr);
            con.Open();
        }

//        @cityid nvarchar(150)=null,  
//@countryid nvarchar(150)=null,  
//@hotelid nvarchar(150)=null,  
//@TransId nvarchar(150)=null,  
//@Type nvarchar(150)=null,  

        public DataTable GetCity_GoGlobal(GoGlobal_Detail htldetail)
        {
            DataTable dt = new DataTable("CityName");

            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        //SqlCommand com = new SqlCommand("SP_GetCityHotelBeds", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@cityid", htldetail.CityCode);
                        cmd.Parameters.AddWithValue("@Type", "cityid");
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
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
                //con.Dispose();
                return dt;
            }
        }

        public DataTable GetCountry_GoGlobal(string CountryId)
        {
            DataTable dt = new DataTable();

            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        //SqlCommand com = new SqlCommand("SP_GetCityHotelBeds", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@countryid", CountryId);
                    
                        cmd.Parameters.AddWithValue("@Type", "countryid");



                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
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
                //con.Dispose();
                return dt;
            }
        }

        public DataTable GetSupplierId_fromGiataId(string HotelId)
        {
            DataTable dt = new DataTable();

            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        //SqlCommand com = new SqlCommand("SP_GetCityHotelBeds", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@hotelid", HotelId);
                        cmd.Parameters.AddWithValue("@Type", "hotelid");

                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
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
                //con.Dispose();
                return dt;
            }
        }

        public DataTable GetRoomLog_GoGlobal(string TransId)
        {
            DataTable dt = new DataTable();

            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        //SqlCommand com = new SqlCommand("SP_GetCityHotelBeds", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransId", TransId);
                        cmd.Parameters.AddWithValue("@Type", "roomlog");
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
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
                //con.Dispose();
                return dt;
            }
        }

        public DataTable GetSupRoomLog_GoGlobal(string TransId)
        {
            DataTable dt = new DataTable();

            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        //SqlCommand com = new SqlCommand("SP_GetCityHotelBeds", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransId", TransId);
                        cmd.Parameters.AddWithValue("@Type", "suproomlog");
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
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
                //con.Dispose();
                return dt;
            }
        }

        public DataTable GetHotelList_GoGlobal(string HotelId, string  CityId,string Name)
        {

       
            DataTable dt = new DataTable("HotelList");
            try
            {
                
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal_HotelList", con))
                    {
                       
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CityCode", CityId);                   
                        cmd.Parameters.AddWithValue("@HotelCode", HotelId);
                        cmd.Parameters.AddWithValue("@HotelName", Name);

                      
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
                //con.Dispose();
                return dt;
            }
        }
        public string GetOfferCodeFromSearchLog_GoGlobal(string TransId,string HotelId,out string Image)
        {
            DataTable dt = new DataTable();
            Image = "";
            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        //SqlCommand com = new SqlCommand("SP_GetCityHotelBeds", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransId", TransId);
                        cmd.Parameters.AddWithValue("@hotelid", HotelId);
                        cmd.Parameters.AddWithValue("@Type", "supsearchlog");
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();
                          
                        }

                       

                        string hotellist = "";
                      
                        if (dt != null)
                        {

                            foreach (DataRow row in dt.Rows)
                            {
                                hotellist =  row["code"].ToString() ;
                                Image = row["images"].ToString();
                            }



                        }
                        return hotellist;




                    }
                }
            }
            catch (Exception ex)
            {
                //con.Dispose();
                return "";
            }
        }


        public string GetCountrySupplierCOuntryCode_GoGlobal(string countryid)
        {
            DataTable dt = new DataTable();
            string code = "";
            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        //SqlCommand com = new SqlCommand("SP_GetCityHotelBeds", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@countryid", countryid);                        
                        cmd.Parameters.AddWithValue("@Type", "countrysupcode");
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();

                        }



                      
                        if (dt != null)
                        {

                            foreach (DataRow row in dt.Rows)
                            {
                                code = row["code"].ToString();
                            
                            }



                        }
                        return code;




                    }
                }
            }
            catch (Exception ex)
            {
                //con.Dispose();
                return "";
            }
        }


        public string GetRemarkFromRoomLog_GoGlobal(string TransId,string Code,ref string special)
        {

            DataTable dt = new DataTable();
            special = "";
          string remark = "";
            try
            {
                //connecttion();
                using (con = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_GetGoGlobal", con))
                    {
                        
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransId", TransId);

                        cmd.Parameters.AddWithValue("@Type", "remark");
                        cmd.Parameters.AddWithValue("@code", Code);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        //connecttion();
                        //dt.Load(com.ExecuteReader());
                        //return dt;
                        using (adap = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adap.Fill(dt);
                            con.Close();

                        }



                       

                        if (dt != null)
                        {

                            foreach (DataRow row in dt.Rows)
                            {
                                remark = row["remark"].ToString();
                                special = row["special"].ToString();
                            }



                        }
                        return remark + "   <BR />   " + special;




                    }
                }
            }
            catch (Exception ex)
            {
                //con.Dispose();
                return "";
            }
        }

 
                         
       
    }
}