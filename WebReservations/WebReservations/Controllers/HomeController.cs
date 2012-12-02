using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using WebReservations.Lib;
using WebReservations.Availability;
using WebReservations.Models;
using WebReservations.Information;

namespace WebReservations.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAvailableRooms(GetAvailableRooms formParams)
        {

            AvailabilityService avail = new AvailabilityService();
            Object response = avail.GetAvailableRooms(DateTime.Parse(formParams.datepickerArrival), DateTime.Parse(formParams.datepickerDeparture), 1, formParams.adultCount, formParams.childCount);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Test()
        {
            AvailabilityService avail = new AvailabilityService();
            Object response = avail.GetAvailableRooms(DateTime.Parse("2012-12-19"), DateTime.Parse("2012-12-20").AddDays(1));
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TestPackage()
        {
            AvailabilityService avail = new AvailabilityService();
            Object response = avail.GetAvailablePackages(DateTime.Parse("2012-12-19"), DateTime.Parse("2012-12-20").AddDays(1));
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /*
        public JsonResult TestItemGroups()
        {
            AvailabilityServiceSoapClient service = new AvailabilityServiceSoapClient();
            FetchAvailableItemsRequest itemRequest = new FetchAvailableItemsRequest();
            FetchItemGroupsRequest itemGroupReq = new FetchItemGroupsRequest();
            HotelReference hotelRef = new HotelReference();
            HotelSearchCriterion hotelSearch = new HotelSearchCriterion();
            OGHeader og = new OGHeader();
            EndPoint origin = new EndPoint();
            EndPoint dest = new EndPoint();
            FetchItemGroupsResponse itemGroupResp = new FetchItemGroupsResponse();

            origin.entityID = System.Configuration.ConfigurationManager.AppSettings["OriginEntityId"];
            origin.systemType = System.Configuration.ConfigurationManager.AppSettings["OriginSystemType"];
            dest.entityID = System.Configuration.ConfigurationManager.AppSettings["DestEntityId"];
            dest.systemType = System.Configuration.ConfigurationManager.AppSettings["DestSystemType"];

            Random random = new Random();
            long transId = DateTime.Now.Ticks;
            og.Origin = origin;
            og.Destination = dest;
            og.transactionID = transId.ToString();
            og.timeStamp = DateTime.Now;
            og.timeStampSpecified = true;

            hotelRef.chainCode = "CHA";
            hotelRef.hotelCode = "WCCH";
            hotelSearch.HotelRef = hotelRef;

            itemGroupReq.HotelReference = hotelRef;
            itemGroupReq.groupOnly = true;
            itemGroupResp = service.FetchItemGroups(ref og, itemGroupReq);
            //itemRequest.
            return Json(itemGroupResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TestItems()
        {
            AvailabilityServiceSoapClient service = new AvailabilityServiceSoapClient();
            FetchAvailableItemsRequest itemRequest = new FetchAvailableItemsRequest();
            FetchItemGroupsRequest itemGroupReq = new FetchItemGroupsRequest();
            HotelReference hotelRef = new HotelReference();
            HotelSearchCriterion hotelSearch = new HotelSearchCriterion();
            OGHeader og = new OGHeader();
            EndPoint origin = new EndPoint();
            EndPoint dest = new EndPoint();
            FetchItemGroupsResponse itemGroupResp = new FetchItemGroupsResponse();
            FetchAvailableItemsResponse itemResp = new FetchAvailableItemsResponse();
            InventoryItemElement invItem = new InventoryItemElement();

            origin.entityID = System.Configuration.ConfigurationManager.AppSettings["OriginEntityId"];
            origin.systemType = System.Configuration.ConfigurationManager.AppSettings["OriginSystemType"];
            dest.entityID = System.Configuration.ConfigurationManager.AppSettings["DestEntityId"];
            dest.systemType = System.Configuration.ConfigurationManager.AppSettings["DestSystemType"];

            Random random = new Random();
            long transId = DateTime.Now.Ticks;
            og.Origin = origin;
            og.Destination = dest;
            og.transactionID = transId.ToString();
            og.timeStamp = DateTime.Now;
            og.timeStampSpecified = true;

            hotelRef.chainCode = "CHA";
            hotelRef.hotelCode = "WCCH";
            hotelSearch.HotelRef = hotelRef;
            /*
            invItem.

            itemRequest.HotelReference = hotelRef;
            itemRequest.InventoryItem
             * * /
            //itemRequest.
            return Json(itemGroupResp, JsonRequestBehavior.AllowGet);
        }
        */

        public JsonResult TestQueryRoomTypes()
        {
            InformationService infoService = new InformationService();
            LovResponse response = infoService.getRoomTypes();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TestQueryFeautures()
        {
            InformationService infoService = new InformationService();
            LovResponse response = infoService.getFeatures();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TestGetRates()
        {
            InformationService infoService = new InformationService();
            RateResponse response = infoService.getRates(DateTime.Parse("2012-12-19"), DateTime.Parse("2012-12-20").AddDays(1));
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //
        
    }
}
