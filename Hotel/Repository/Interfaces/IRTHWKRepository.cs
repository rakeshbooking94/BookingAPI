using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravillioXMLOutService.Hotel.Repository.Interfaces
{
    public interface IRTHWKRepository
    {
        Task<string> DownlaodHotelAsync();

        Task<string> HotelSearchAsync(XElement req);
        Task<string> RoomSearchAsync(XElement req);
        Task<string> CancellationPolicyAsync(XElement req);
        Task<string> PreBookingAsync(XElement req);
        Task<string> HotelBookingAsync(XElement req);
        Task<string> CancelBookingAsync(XElement req);
    }
}
