using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravillioXMLOutService.Hotel.Model;

namespace TravillioXMLOutService.Hotel.Repository.Interfaces
{
    public interface IRTHWKRepository
    {


        Task<string> HotelSearchAsync(RequestModel reqModel);
        Task<string> RoomSearchAsync(RequestModel reqModel);
        Task<string> PreBookingAsync(RequestModel reqModel);
        Task<string> CancellationPolicyAsync(RequestModel reqModel);
        Task<string> CancelBookingAsync(RequestModel reqModel);
        Task<string> HotelBookingAsync(RequestModel reqModel);
        Task<string> HotelBookingConfirmAsync(RequestModel reqModel);

        Task<string> DownlaodHotelAsync();
    }
}
