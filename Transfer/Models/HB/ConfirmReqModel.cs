using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Transfer.Models.HB
{
    public class ConfirmReqModel : BaseModel
    {        
        public Holder holder { get; set; }
        public List<Transfer> transfers { get; set; }
        public string clientReference { get; set; }
        public string welcomeMessage { get; set; }
        public string remark { get; set; }
    }
    public class Holder
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class Transfer
    {
        public string rateKey { get; set; }
        public List<TransferDetail> transferDetails { get; set; }
    }

    public class TransferDetail
    {
        public string type { get; set; }
        public string direction { get; set; }
        public string code { get; set; }
        public object companyName { get; set; }
    }




















}