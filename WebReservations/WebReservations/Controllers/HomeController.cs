using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using WebReservations.Lib;
using WebReservations.Availability;

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

        public JsonResult Test()
        {
            AvailabilityRequest ws = new AvailabilityRequest();
            RegionalAvailabilityExtRequest reg = new RegionalAvailabilityExtRequest();
            AvailabilityServiceSoapClient cli = new AvailabilityServiceSoapClient();
            OGHeader og = new OGHeader();
            EndPoint ep = new EndPoint();
            EndPoint dest = new EndPoint();
            AvailRequestSegment segment = new AvailRequestSegment();
            HotelSearchCriterion hotel_search = new HotelSearchCriterion();
            HotelReference hotel_ref = new HotelReference();
            RoomStayCandidate room_stay = new RoomStayCandidate();
            AvailabilityRequest request = new AvailabilityRequest();
            Availability.TimeSpan time_span = new Availability.TimeSpan();
            MinMaxRate rate = new MinMaxRate();
            AvailabilityResponse response = new AvailabilityResponse();


            ep.entityID = "OWS";
            ep.systemType = "WEB";
            dest.entityID = "1T";
            dest.systemType = "PMS";

            Random random = new Random();
            int transId = random.Next(100, 999999);
            og.Origin = ep;
            og.Destination = dest;
            og.transactionID = transId.ToString("000000");
            og.timeStamp = DateTime.Now;
            og.timeStampSpecified = true;

            request.summaryOnly = true;

            time_span.StartDate = DateTime.Now;
            time_span.Item = DateTime.Now.AddDays(1);
            
            rate.currencyCode = "PHP";
            rate.minimumRateSpecified = false;
            rate.maximumRateSpecified = false;

            segment.numberOfRooms = 1;
            segment.numberOfRoomsSpecified = true;
            segment.totalNumberOfGuests = 1;
            segment.totalNumberOfGuestsSpecified = true;
            segment.numberOfChildrenSpecified = false;
            segment.availReqType = AvailRequestType.Room;


            hotel_ref.chainCode = "CHA";
            hotel_ref.hotelCode = "WCCH";
            hotel_search.HotelRef = hotel_ref;

            room_stay.invBlockCode = null;

            segment.HotelSearchCriteria = new HotelSearchCriterion[1];
            segment.RoomStayCandidates = new RoomStayCandidate[1];
            request.AvailRequestSegment = new AvailRequestSegment[1];

            segment.HotelSearchCriteria[0] = hotel_search;
            segment.RoomStayCandidates[0] = room_stay;
            segment.RateRange = rate;
            segment.StayDateRange = time_span;
            request.AvailRequestSegment[0] = segment;

            response = cli.Availability(ref og, request);


            /*
            og.Intermediaries = 
            ws.

            cli.Availability(og, );
             * */
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //
        
    }
}
