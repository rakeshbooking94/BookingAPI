using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravillioXMLOutService.Models;
using TravillioXMLOutService.Repository.Transfer;
using TravillioXMLOutService.Transfer.Helpers;
using TravillioXMLOutService.Transfer.Models;
using TravillioXMLOutService.Transfer.Models.HB;
using TravillioXMLOutService.Transfer.Services.Interfaces;

namespace TravillioXMLOutService.Transfer.Services
{
    public class HotelBedService : IHotelBedService, IDisposable
    {
        RequestModel reqModel;
        HBCredentials model;
        HotelBedRepository _repo;
        public HotelBedService(string customerId)
        {
            model = new HBCredentials();
            #region Credentials
            XElement _credentials = CommonHelper.ReadCredential(customerId, model.SupplierId.ToString());
            try
            {
                model.ServiceHost = _credentials.Element("ServiceHost").Value;
                model.UserName = _credentials.Element("UserName").Value;
                model.Password = _credentials.Element("Password").Value;
                _repo = new HotelBedRepository(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }

        RequestModel CreateReqModel(XElement _travyoReq)
        {
            reqModel = new RequestModel();
            reqModel.StartTime = DateTime.Now;
            reqModel.Customer = Convert.ToInt64(_travyoReq.Attribute("CustomerID").Value);
            reqModel.TrackNo = _travyoReq.Attribute("TransID").Value;
            reqModel.ActionId = (int)_travyoReq.Name.LocalName.GetAction();
            reqModel.Action = _travyoReq.Name.LocalName.GetAction().ToString();
            return reqModel;
        }
        #region Search
        public async Task<XElement> GetSearchAsync(XElement _travyoReq)
        {
            XElement response = null;
            reqModel = CreateReqModel(_travyoReq);
            reqModel.EndTime = DateTime.Now;
            var pickup = _travyoReq.Descendants("Itinerary").FirstOrDefault().Element("PickUp");
            var dropup = _travyoReq.Descendants("Itinerary").FirstOrDefault().Element("DropOff");
            int pickupType = pickup.Attribute("type").ToINT();
            int dropupType = dropup.Attribute("type").ToINT();
            string pickupCode = pickup.Attribute("Code").Value;
            string dropupCode = dropup.Attribute("Code").Value;
            string inDate = pickup.Element("PickUpTime").Attribute("date").Value + " " + pickup.Element("PickUpTime").Attribute("time").Value;
            DateTime pickUpdate = inDate.GetDateTime("d MMM, yy HH:mm");
            SearchModel model = new SearchModel()
            {
                language = "en",
                ftype = pickupType == 1 ? "IATA" : "ATLAS",
                fcode = pickupCode,
                ttype = dropupType == 1 ? "IATA" : "ATLAS",
                tcode = dropupCode,
                departing = pickUpdate.ToString("yyyy-MM-ddTHH:mm:ss"),
                adults = _travyoReq.Element("Occupancy").Attribute("adults").ToINT(),
                children = _travyoReq.Element("Occupancy").Attribute("children").ToINT(),
                infants = 0
            };
            int searchType = _travyoReq.Descendants("Itinerary").Count();
            if (searchType > 1)
            {
                var Outpickup = _travyoReq.Descendants("Itinerary").Where(x => x.Attribute("type").Value == "OUT").FirstOrDefault().Element("PickUp");
                string outDate = Outpickup.Element("PickUpTime").Attribute("date").Value + " " + pickup.Element("PickUpTime").Attribute("time").Value;
                DateTime outDatetime = outDate.GetDateTime("d MMM, yy HH:mm");
                model.comeback = outDatetime.ToString("yyyy-MM-ddTHH:mm:ss");

                reqModel.RequestStr = $"transfer-api/1.0/availability/{model.language}/from/{model.ftype}/" +
           $"{model.fcode}/to/{model.ttype}/{model.tcode}/{model.departing}/{model.comeback}/" +
       $"{model.adults}/{model.children}/{model.infants}";

            }
            else
            {
                reqModel.RequestStr = $"transfer-api/1.0/availability/{model.language}/from/{model.ftype}/" +
      $"{model.fcode}/to/{model.ttype}/{model.tcode}/{model.departing}/" +
  $"{model.adults}/{model.children}/{model.infants}";
            }

            SearchResponseModel rsmodel = await _repo.GetSearchAsync(reqModel);

            if (rsmodel != null)
            {
                IEnumerable<XElement> srvTransfers;
                IEnumerable<XElement> joinTransfers;
                srvTransfers = from srv in rsmodel.services
                               select travayooResponse(srv, rsmodel.search);
                int count = srvTransfers.Where(x => x.Attribute("type").Value == "OUT").Count();
                if (count > 0)
                {
                    joinTransfers = from srv in srvTransfers.Where(x => x.Attribute("type").Value == "IN")
                                    from srvOut in srvTransfers.Where(x => x.Attribute("type").Value == "OUT")
                                    let _amount = srv.Element("price").Attribute("totalAmount").GetValueOrDefault(0.0m) + srvOut.Element("price").Attribute("totalAmount").GetValueOrDefault(0.0m)
                                    select new XElement("serviceTransfer", new XAttribute("supplierId", 10),
                                    new XAttribute("currency", srv.Element("price").Attribute("currencyId").Value), srv, srvOut,
                                    srv.Descendants("cancellationList").Concat(srv.Descendants("cancellationList")).ToList().MergPolicy(_amount));
                }
                else
                {
                    joinTransfers = from srv in srvTransfers
                                    let _amount = srv.Element("price").Attribute("totalAmount").GetValueOrDefault(0.0m)
                                    select new XElement("serviceTransfer", new XAttribute("supplierId", 10),
                                    new XAttribute("currency", srv.Element("price").Attribute("currencyId").Value), srv,
                                    srv.Descendants("cancellationList").ToList().MergPolicy(_amount));
                }
                joinTransfers.Descendants("cancellationList").Remove();
                response = new XElement("searchResponse", new XElement("serviceTransfers",
    new XAttribute("adults", rsmodel.search.occupancy.adults),
    new XAttribute("children", rsmodel.search.occupancy.children),
    new XAttribute("childrenAge", _travyoReq.Element("Occupancy").Attribute("childrenAge").Value),
    new XAttribute("infants", rsmodel.search.occupancy.infants), joinTransfers.Take(30)));
                response.Descendants("cancellationList").Remove();
            }
            else
            {
                response = new XElement("searchResponse", new XElement("serviceTransfers",
new XAttribute("adults", model.adults),
new XAttribute("children", model.children),
 new XAttribute("childrenAge", _travyoReq.Element("Occupancy").Attribute("childrenAge").Value),
new XAttribute("infants", model.infants), new XElement("ErrorTxt", "Unable to find any transfer service ")));

            }

            return response;
            //doc.Add(response);  strDate = strDate.AlterFormat("yyyyMMdd", "d MMM, yy");
            //doc.Save(ConfigurationManager.AppSettings["fileDirectory"] + string.Format("response-{0}.xml", DateTime.Now.Ticks));
        }

        public PickupInformation pickupInformation { get; set; }
        XElement pickupTime(PickupInformation item, Search req)
        {
            XElement _node = null;
            if (!string.IsNullOrEmpty(item.date))
            {
                _node = new XElement("pickUpTime",
                    new XAttribute("date", item.date.AlterFormat("yyyy-MM-dd", "d MMM, yy")),
                    new XAttribute("time", item.time.AlterFormat("HH:mm:ss", "HH:mm")));
            }
            else
            {

                if (item.from.code == req.from.code)
                {
                    _node = new XElement("pickUpTime",
                  new XAttribute("date", req.departure.date.AlterFormat("yyyy-MM-dd", "d MMM, yy")),
                  new XAttribute("time", req.departure.time.AlterFormat("HH:mm:ss", "HH:mm")));
                }
                else
                {
                    _node = new XElement("pickUpTime",
                                     new XAttribute("date", req.comeBack.date.AlterFormat("yyyy-MM-dd", "d MMM, yy")),
                                     new XAttribute("time", req.comeBack.time.AlterFormat("HH:mm:ss", "HH:mm")));
                }


            }
            return _node;
        }

        XElement travayooResponse(TravillioXMLOutService.Transfer.Models.HB.Services srv, Search req)
        {

            var model = new XElement("transfer", new XAttribute("id", srv.id), new XAttribute("type", srv.pickupInformation.@from.code == req.from.code ? "IN" : "OUT"),
                new XAttribute("newPrice", string.Empty),
                                  new XAttribute("direction", srv.direction),
                                    new XAttribute("transferType", srv.transferType),
                                    pickupTime(srv.pickupInformation, req),
                                  new XElement("pickUp", new XAttribute("code", srv.pickupInformation.@from.code),
                                  new XAttribute("type", srv.pickupInformation.@from.type),
                                  new XElement("name", srv.pickupInformation.@from.description),
                                  new XElement("description", srv.pickupInformation.pickup.description),
                                  new XElement("address", srv.pickupInformation.pickup.address.IsNullString(),
                                  new XAttribute("number", srv.pickupInformation.pickup.number.IsNullString()),
                                  new XAttribute("town", srv.pickupInformation.pickup.town.IsNullString()),
                                  new XAttribute("zip", srv.pickupInformation.pickup.zip.IsNullString()),
                                  new XAttribute("altitude", srv.pickupInformation.pickup.altitude.IsNullNumber()),
                                  new XAttribute("latitude", srv.pickupInformation.pickup.latitude.IsNullNumber()),
                                  new XAttribute("longitude", srv.pickupInformation.pickup.longitude.IsNullNumber()),
                                  new XAttribute("stopName", srv.pickupInformation.pickup.stopName.IsNullString()),
                                  new XAttribute("pickupId", srv.pickupInformation.pickup.pickupId.IsNullString()),
                                  new XAttribute("image", srv.pickupInformation.pickup.image.IsNullString()))),
                                  new XElement("dropOff", new XAttribute("code", srv.pickupInformation.to.code),
                                  new XAttribute("type", srv.pickupInformation.to.type),
                                  new XElement("name", srv.pickupInformation.to.description)),
                                  new XElement("price", new XAttribute("totalAmount", srv.price.totalAmount),
                                  new XAttribute("currencyId", srv.price.currencyId), new XElement("rateKey", srv.rateKey)),
                                  new XElement("capacity", new XAttribute("minPaxCapacity", srv.minPaxCapacity), new XAttribute("maxPaxCapacity", srv.maxPaxCapacity),
                                  new XAttribute("lugtype", string.Empty), new XElement("description", string.Empty)),
                                  new XElement("guideLines",
                                  from cnt in srv.content.transferDetailInfo
                                  select new XElement("description", new XAttribute("id", cnt.id), new XAttribute("type", cnt.type),
                                  new XAttribute("title", cnt.name), cnt.description)),
                                  new XElement("images",
                                  from img in srv.content.images
                                  select new XElement("image", new XAttribute("type", img.type), img.url)),
                                  new XElement("category", new XAttribute("code", srv.content.category.code), srv.content.category.name),
                                  new XElement("vehicle", new XAttribute("code", srv.content.vehicle.code), srv.content.vehicle.name),
                                       new XElement("remarks",
                                  from note in srv.content.transferRemarks
                                  select new XElement("remark", new XAttribute("type", note.type), new XAttribute("mandatory", note.mandatory), note.description)),
                                  new XElement("waitingTime", new XAttribute("cwtime", string.Empty), new XAttribute("domestic", string.Empty),
                                  new XAttribute("international", string.Empty)),
                                  new XElement("journeyTime", new XAttribute("jtime", string.Empty)),
                                  new XElement("cancellationList",
                                  from cxl in srv.cancellationPolicies
                                  select new XElement("cancellationPolicy", new XAttribute("lastDate", cxl.@from),
                                  new XAttribute("amount", cxl.amount), new XAttribute("noShow", 0))
                                  ));

            return model;

        }

        #endregion

        #region PreBook
        public async Task<XElement> GetPreBookSearchAsync(XElement _travyoReq)
        {
            var req = new LogRequestModel();
            req.CustomerId = Convert.ToInt64(_travyoReq.Attribute("CustomerID").Value);
            req.TrackNumber = _travyoReq.Attribute("TransID").Value;
            req.IpAddress = _travyoReq.Attribute("IpAddress").Value;
            req.LogTypeId = 1;
            req.SupplierId = 10;
            var respHb = await _repo.GetPreBookSearchAsync(req);
            var result = from srv in respHb.services
                         join reqSrv in _travyoReq.Descendants("Itinerary") on srv.rateKey equals reqSrv.Element("ratekey").Value
                         select travayooResponse(srv, respHb.search);
            IEnumerable<XElement> joinTransfers;
            int count = result.Where(x => x.Attribute("type").Value == "OUT").Count();
            if (count > 0)
            {

                joinTransfers = from srv in result.Where(x => x.Attribute("type").Value == "IN")
                                from srvOut in result.Where(x => x.Attribute("type").Value == "OUT")
                                let _amount = srv.Element("price").Attribute("totalAmount").GetValueOrDefault(0.0m) + srvOut.Element("price").Attribute("totalAmount").GetValueOrDefault(0.0m)
                                select new XElement("serviceTransfer", new XAttribute("supplierId", 10),
                                new XAttribute("currency", srv.Element("price").Attribute("currencyId").Value), srv, srvOut,
                                srv.Descendants("cancellationList").Concat(srv.Descendants("cancellationList")).ToList().MergPolicy(_amount));
            }
            else
            {
                joinTransfers = from srv in result
                                let _amount = srv.Element("price").Attribute("totalAmount").GetValueOrDefault(0.0m)
                                select new XElement("serviceTransfer", new XAttribute("supplierId", 10),
                                new XAttribute("currency", srv.Element("price").Attribute("currencyId").Value), srv,
                                srv.Descendants("cancellationList").ToList().MergPolicy(_amount));
            }
            var response = new XElement("PrebookResponse", new XElement("serviceTransfers",
new XAttribute("adults", respHb.search.occupancy.adults),
new XAttribute("children", respHb.search.occupancy.children),
new XAttribute("infants", respHb.search.occupancy.infants), joinTransfers));
            response.Descendants("cancellationList").Remove();
            return response;
        }
        #endregion


        #region Confirm


        public async Task<XElement> ConfirmBookingAsync(XElement _travyoReq)
        {
            XElement response = null;
            reqModel = CreateReqModel(_travyoReq);
            reqModel.EndTime = DateTime.Now;
            var guest = _travyoReq.Descendants("PaxItem").Where(x => x.Attribute("isLead").Value == "true").FirstOrDefault();
            ConfirmReqModel req = new ConfirmReqModel()
            {
                language = "en",
                clientReference = _travyoReq.Attribute("TransactionID").Value,
                welcomeMessage = _travyoReq.Element("Note").Value,
                remark = _travyoReq.Element("Comment").Value,
                holder = new Holder
                {
                    name = guest.Attribute("firstName").Value,
                    surname = guest.Attribute("lastName").Value,
                    email = guest.Attribute("email").Value,
                    phone = guest.Attribute("phone").Value,
                },
                transfers = _travyoReq.Descendants("Itinerary").Select(item => new TransfeReq
                {
                    rateKey = item.Element("ratekey").Value,
                    transferDetails = this.transferDetails(item)
                }).ToList()
            };
            reqModel.RequestStr = $"transfer-api/1.0/bookings/";
            ConfirmResponseModel rsmodel = await _repo.GetConfirmAsync(reqModel, req);
            if (rsmodel != null)
            {
                var bokg = rsmodel.bookings.FirstOrDefault();
                response = new XElement("bookResponse", new XElement("serviceTransfer",
                    new XAttribute("supplierId", 10),
                    new XAttribute("supName", bokg.supplier.name),
                    new XAttribute("vatNumber", bokg.supplier.vatNumber),
                    new XAttribute("status", bokg.status),
                    new XAttribute("bookConfirmno", bokg.reference),
                    new XAttribute("voucherRefno", bokg.reference),
                    new XAttribute("currency", bokg.currency),
                    new XElement("voucherRemarks", bokg.supplier.name + " vatNumber " + bokg.supplier.vatNumber),
                    bindResponse(bokg, _travyoReq)));
            }
            else
            {
                response = new XElement("bookResponse", new XAttribute("supplierId", 10),
                        new XElement("ErrorTxt", "Unable to book transfer service "));
            }
            return response;
        }


        List<TransferDetail> transferDetails(XElement _req)
        {
            XElement x = _req.Element("pickUp");
            XElement y = _req.Element("dropOff");
            List<TransferDetail> lst = new List<TransferDetail>();
            if (x.Attribute("type").Value == "IATA")
            {
                lst.Add(new TransferDetail
                {
                    type = "FLIGHT",
                    direction = _req.Attribute("direction").Value,
                    code = x.Attribute("travelCode").Value,
                    companyName = x.Attribute("travelName").Value,
                });
            }
            if (y.Attribute("type").Value == "IATA")
            {
                lst.Add(new TransferDetail
                {
                    type = "FLIGHT",
                    direction = _req.Attribute("direction").Value,
                    code = y.Attribute("travelCode").Value,
                    companyName = y.Attribute("travelName").Value,
                });
            }
            return lst;
        }

        IEnumerable<XElement> bindResponse(Booking bokModel, XElement req)
        {
            var transfers = from srv in bokModel.transfers
                            join trip in req.Descendants("Itinerary") on srv.id equals trip.Attribute("index").ToINT(-1)
                            select BindResponseElemnet(srv, trip);

            return transfers;

        }



        XElement BindResponseElemnet(Transfers srv, XElement trip)
        {

            var _item = new XElement("transfer", new XAttribute("id", srv.id), new XAttribute("status", srv.status),
                            new XAttribute("type", srv.transferType),
                                   new XAttribute("booktoken", trip.Attribute("booktoken").Value),
                                       new XAttribute("direction", ""),
                                   new XAttribute("price", srv.price.totalAmount),
                                     new XElement("productName", srv.category.name + " " + srv.vehicle.name),
                                     new XElement("rateKey", srv.rateKey),

                                   new XElement("pickUpTime", new XAttribute("date", srv.pickupInformation.date.AlterFormat("yyyy-MM-dd", "d MMM, yy")),
                                   new XAttribute("time", srv.pickupInformation.time.AlterFormat("HH:mm:ss", "HH:mm"))),

                                   new XElement("pickUp", new XAttribute("code", srv.pickupInformation.@from.code),
                                   new XElement("emergencyNumber", srv.sourceMarketEmergencyNumber),
                                   new XAttribute("type", srv.pickupInformation.@from.type),
                                   new XElement("name", srv.pickupInformation.@from.description),
                                   new XElement("description", srv.pickupInformation.pickup.description),
                                   new XElement("address", srv.pickupInformation.pickup.address.IsNullString(),
                                   new XAttribute("number", srv.pickupInformation.pickup.number.IsNullString()),
                                   new XAttribute("town", srv.pickupInformation.pickup.town.IsNullString()),
                                   new XAttribute("zip", srv.pickupInformation.pickup.zip.IsNullString()),
                                   new XAttribute("altitude", srv.pickupInformation.pickup.altitude.IsNullNumber()),
                                   new XAttribute("latitude", srv.pickupInformation.pickup.latitude.IsNullNumber()),
                                   new XAttribute("longitude", srv.pickupInformation.pickup.longitude.IsNullNumber()),
                                   new XAttribute("stopName", srv.pickupInformation.pickup.stopName.IsNullString()),
                                   new XAttribute("pickupId", srv.pickupInformation.pickup.pickupId.IsNullString()),
                                   new XAttribute("image", srv.pickupInformation.pickup.image.IsNullString()))),

                                   new XElement("dropOff", new XAttribute("code", srv.pickupInformation.to.code),
                                   new XAttribute("type", srv.pickupInformation.to.type),
                                   new XElement("name", srv.pickupInformation.to.description)),
                                          new XElement("remarks",
                                   from note in srv.content.transferRemarks
                                   select new XElement("remark", new XAttribute("type", note.type), new XAttribute("mandatory", note.mandatory), note.description)),
                                   new XElement("cancellationList",
                                   from cxl in srv.cancellationPolicies
                                   select new XElement("cancellationPolicy", new XAttribute("lastDate", cxl.@from),
                                   new XAttribute("amount", cxl.amount), new XAttribute("noShow", 0))
                                   ));

            return _item;

        }







        #endregion


        #region Cancel

        public async Task<XElement> CancelBookingAsync(XElement _travyoReq)
        {
            XElement response = null;
            reqModel = CreateReqModel(_travyoReq);
            reqModel.EndTime = DateTime.Now;
            reqModel.RequestStr = $"transfer-api/1.0/bookings/en/reference/";
            SearchResponseModel rsmodel = await _repo.GetSearchAsync(reqModel);
            return response;
        }

        #endregion

        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion






    }
}