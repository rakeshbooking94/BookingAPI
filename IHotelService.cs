using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace TravillioXMLOutService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHotelService" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IHotelService
    {
        #region Hotel
        [OperationContract, XmlSerializerFormat]
        object HotelAvailability(XElement req);

        [OperationContract, XmlSerializerFormat]
        object HotelRoomsAvail(XElement req);

        [OperationContract, XmlSerializerFormat]
        object HotelDetails(XElement req);

        [OperationContract, XmlSerializerFormat]
        object HotelDetailWithCancellations(XElement req);

        [OperationContract, XmlSerializerFormat]
        object HotelPreBooking(XElement req);

        [OperationContract, XmlSerializerFormat]
        object HotelBookingConfirmation(XElement req);

        [OperationContract, XmlSerializerFormat]
        object HotelCancellation(XElement req);


        #endregion

    }
}
