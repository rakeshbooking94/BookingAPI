using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravillioXMLOutService.Transfer.Models.HB
{

    public class ConfirmResponseModel
    {
        public List<Booking> bookings { get; set; }
    }







    public class Booking
    {
        public string reference { get; set; }
        public string bookingFileId { get; set; }
        public DateTime creationDate { get; set; }
        public string status { get; set; }
        public ModificationsPolicies modificationsPolicies { get; set; }
        public Holder holder { get; set; }
        public List<Transfers> transfers { get; set; }
        public string clientReference { get; set; }
        public string remark { get; set; }
        public InvoiceCompany invoiceCompany { get; set; }
        public Supplier supplier { get; set; }
        public double totalAmount { get; set; }
        public double totalNetAmount { get; set; }
        public double pendingAmount { get; set; }
        public string currency { get; set; }
        public List<Links> links { get; set; }
        public bool paymentDataRequired { get; set; }
    }


    public class InvoiceCompany
    {
        public string code { get; set; }
    }

 

    public class ModificationsPolicies
    {
        public bool cancellation { get; set; }
        public bool modification { get; set; }
    }

    public class Paxis
    {
        public string type { get; set; }
        public int age { get; set; }
    }





    public class Supplier
    {
        public string name { get; set; }
        public string vatNumber { get; set; }
    }

    
    public class Transfers
    {
        public int id { get; set; }
        public string rateKey { get; set; }
        public string status { get; set; }
        public string transferType { get; set; }
        public Vehicle vehicle { get; set; }
        public Category category { get; set; }
        public PickupInformation pickupInformation { get; set; }
        public List<Paxis> paxes { get; set; }
        public Content content { get; set; }
        public Price price { get; set; }
        public List<CancellationPolicy> cancellationPolicies { get; set; }
        public int? factsheetId { get; set; }
        public string arrivalFlightNumber { get; set; }
        public string departureFlightNumber { get; set; }
        public string arrivalShipName { get; set; }
        public string departureShipName { get; set; }
        public object arrivalTrainInfo { get; set; }
        public object departureTrainInfo { get; set; }
        public List<TransferDetail> transferDetails { get; set; }
        public string sourceMarketEmergencyNumber { get; set; }
        public List<Links> links { get; set; }
    }








}