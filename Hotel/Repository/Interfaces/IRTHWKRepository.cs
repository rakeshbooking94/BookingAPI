﻿using System;
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
        //Task<string> RoomSearchAsync(XElement req);
        //Task<string> CancellationPolicyAsync(XElement req);
        //Task<string> PreBookingAsync(XElement req);
        //Task<string> HotelBookingAsync(XElement req);
        //Task<string> CancelBookingAsync(XElement req);

        Task<string> DownlaodHotelAsync();
    }
}
