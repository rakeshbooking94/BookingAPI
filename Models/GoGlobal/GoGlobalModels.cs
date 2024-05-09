using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravillioXMLOutService.Models.GoGlobal
{

    public class GoGlobalCredential
    {

        //string agency = "1521643";
        //string user = "NOVODESTINOXML";
        //string _password = "QVVTZNNK3YZP";
        //int maxwaittime = 30;

        string agency = "";
        string user = "";
        string _password = "";
        string currency = "";
        string host = "";


        public GoGlobalCredential(string _customerid)
        {
            try
            {                              
                try
                {
                    XElement suppliercred = supplier_Cred.getsupplier_credentials(_customerid, "27");
                    agency = suppliercred.Descendants("Agency").FirstOrDefault().Value; ;
                    user = suppliercred.Descendants("User").FirstOrDefault().Value;
                    _password = suppliercred.Descendants("Password").FirstOrDefault().Value;
                    currency = suppliercred.Descendants("Currency").FirstOrDefault().Value;
                    host = suppliercred.Descendants("host").FirstOrDefault().Value;
                    // AgentPassword = "111017XHO8";
                }
                catch
                {
                    //agency = "1521643";
                    //user = "NOVODESTINOXML";
                    //_password = "QVVTZNNK3YZP";
                }


            }
            catch
            {
                // agency = "1521643";
                // user = "NOVODESTINOXML";
                //_password = "QVVTZNNK3YZP";
               
            }
        }
        public string Agency
        {
            get
            {

               
                return agency;
            }
        }


        public string User
        {
            get
            {

                return user;
            }
        }
        public string password
        {
            get
            {
                return _password;
            }
        }

        public string Currency
        {
            get
            {
                return currency;
            }
        }

        public string Host
        {
            get
            {
                return host;
            }
        }


        public int Supplier
        {
            get
            {
                return 27;
            }
        }

        public int MaxWaitTime
        {
            get
            {
                try
                {
                    return (Convert.ToInt32(ConfigurationManager.AppSettings["secondcutime"].ToString()) / 1000) - 10;
                }
                catch { return (90000 / 1000)-10; }
            }
        }



    }

    public class GoGlobal_Detail
    {
        public string CityCode
        {
            get;
            set;
        }
        public string CityID
        {
            get;
            set;
        }
        public string HotelCode
        {
            get;
            set;
        }

        public string HotelName
        {
            get;
            set;
        }

        public string MinRating
        {
            get;
            set;
        }
        public string MaxRating
        {
            get;
            set;
        }
        public string TrackNumber
        {
            get;
            set;
        }
        public string xmltype
        {
            get;
            set;
        }
    }
    public class Hotel
    {
        public dynamic Header
        {
            get;
            set;
        }
        public dynamic Hotels
        {
            get;
            set;
        }

      











    }
        



}
