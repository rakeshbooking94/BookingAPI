using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravillioXMLOutService.Transfer.Services.Interfaces
{
    internal interface IHotelBedService
    {
        Task<XElement> GetSearchAsync(XElement _travyoReq);
        Task<XElement> GetPreBookSearchAsync(XElement _travyoReq);
        Task<XElement> ConfirmBookingAsync(XElement _travyoReq);

        Task<XElement> CancelBookingAsync(XElement _travyoReq);
    }
}
