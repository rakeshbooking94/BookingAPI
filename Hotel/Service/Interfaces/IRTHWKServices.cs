using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravillioXMLOutService.Hotel.Service.NewFolder1
{
    internal interface IRTHWKServices
    {
        Task<List<XElement>> HotelAvailabilityAsync(XElement req);
        Task<XElement> GetRoomAvailabilityAsync(XElement req);
        Task<XElement> CancellationPolicyAsync(XElement req);
        Task<XElement> PreBookingAsync(XElement req);
        Task<XElement> HotelBookingAsync(XElement req);
        Task<XElement> CancelBookingAsync(XElement req);
    }
}
