﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravillioXMLOutService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITransferService" in both code and config file together.
    //[ServiceContract]   
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ITransferService
    {
        #region Transfer
        [OperationContract, XmlSerializerFormat]
        Task<object> TransferAvailabilityAsync(XElement req);
        [OperationContract, XmlSerializerFormat]
        Task<object> PreBookTransferAsync(XElement req);
        [OperationContract, XmlSerializerFormat]
        Task<object> ConfirmBookingTransferAsync(XElement req);
        [OperationContract, XmlSerializerFormat]
        Task<object> CancelBookingTransferAsync(XElement req);
        [OperationContract, XmlSerializerFormat]
        object CXLPolicyTransfer(XElement req);

        #endregion
    }
}
