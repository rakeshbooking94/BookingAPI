using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Hotel.Model;

namespace TravillioXMLOutService.Hotel.Repository
{
    public class HotelRepository
    {
        public List<HotelModel> GetAllHotelList(XElement req)
        {
            SqlHelper.ExecuteDataset()


            public DataTable GetCityCode(string CityCode, string CountryID, int SupplierId)
            {
                DataTable dt = new DataTable("CityCode");
               

            }

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




            req.Element("FromDate").Value.RTHWKDate(),

            @SupplierId int= NULL,
@HotelCode nvarchar(50) = NULL,    
@HotelName nvarchar(50)= NULL,    
@CityCode nvarchar(50)= NULL,    
@CountryCode nvarchar(50)= NULL,    
@MinStarRating int= NULL,
@MaxStarRating int= NULL

        }

    }
}