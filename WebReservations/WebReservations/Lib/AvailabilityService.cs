using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebReservations.Availability;
using System.Diagnostics;

namespace WebReservations.Lib
{
    public class AvailabilityService
    {
        protected AvailabilityRequest ws;
        protected RegionalAvailabilityExtRequest reg;
        protected AvailabilityServiceSoapClient cli;
        protected OGHeader og;
        protected EndPoint origin;
        protected EndPoint dest;
        protected AvailRequestSegment segment;
        protected HotelSearchCriterion hotelSearch;
        protected HotelReference hotelRef;
        protected RoomStayCandidate roomStay;
        protected AvailabilityRequest request;
        protected FetchAvailablePackagesRequest packageRequest;
        protected Availability.TimeSpan timeSpan;
        protected MinMaxRate rate;
        protected AvailabilityResponse response;
        protected FetchAvailablePackagesResponse packageResponse;
        protected Exception errors;
        protected List<Object> finalResponse = new List<Object>();
        protected RatePlanCandidate ratePlan;
        public Object tempObj;


        public AvailabilityService()
        {
            this.ws = new AvailabilityRequest();
            this.reg = new RegionalAvailabilityExtRequest();
            this.cli = new AvailabilityServiceSoapClient();
            this.og = new OGHeader();
            this.origin = new EndPoint();
            this.dest = new EndPoint();
            this.segment = new AvailRequestSegment();
            this.hotelSearch = new HotelSearchCriterion();
            this.hotelRef = new HotelReference();
            this.roomStay = new RoomStayCandidate();
            this.request = new AvailabilityRequest();
            this.packageRequest = new FetchAvailablePackagesRequest();
            this.timeSpan = new Availability.TimeSpan();
            this.rate = new MinMaxRate();
            this.response = new AvailabilityResponse();
            this.packageResponse = new FetchAvailablePackagesResponse();
            this.ratePlan = new RatePlanCandidate();
            this.tempObj = new Object();
        }

        protected void _InitializeHeader()
        {
            this.origin.entityID = System.Configuration.ConfigurationManager.AppSettings["OriginEntityId"];
            this.origin.systemType = System.Configuration.ConfigurationManager.AppSettings["OriginSystemType"];
            this.dest.entityID = System.Configuration.ConfigurationManager.AppSettings["DestEntityId"];
            this.dest.systemType = System.Configuration.ConfigurationManager.AppSettings["DestSystemType"];

            Random random = new Random();
            long transId = DateTime.Now.Ticks;
            this.og.Origin = origin;
            this.og.Destination = dest;
            this.og.transactionID = transId.ToString();
            this.og.timeStamp = DateTime.Now;
            this.og.timeStampSpecified = true;
        }

        public void _InitializeTimeSpan(DateTime startDate, DateTime endDate)
        {
            this.timeSpan.StartDate = startDate;
            this.timeSpan.Item = endDate; 
        }

        public void _InitializeHotelRef(string chainCode, string hotelCode)
        {
            this.hotelRef.chainCode = chainCode;
            this.hotelRef.hotelCode = hotelCode;
            this.hotelSearch.HotelRef = hotelRef;
        }

        public Object GetAvailableRooms(DateTime startDate, DateTime endDate, int numRoom = 1, int numAdult = 1, int numChild = 0, string rateType = "normal", string rateCode = "", string currencyCode = "PHP", string chainCode = "CHA", string hotelCode = "WCCH")
        {

            this._InitializeHeader();

            this.request.summaryOnly = true;

            /** /
            this.ratePlan.ratePlanCode = "OWCCH";
            this.roomStay.roomTypeCode = "SUPK";
            this.segment.RatePlanCandidates = new RatePlanCandidate[1];
            this.segment.RatePlanCandidates[0] = this.ratePlan;
            /**/

            this._InitializeTimeSpan(startDate, endDate);

            this.rate.currencyCode = currencyCode;
            this.rate.minimumRateSpecified = true;
            this.rate.maximumRateSpecified = true;
            this.rate.decimals = 2;
            this.rate.decimalsSpecified = true;
            this.rate.minimumRate = 1.00;
            this.rate.maximumRate = 1000000.00;
            
            this.segment.numberOfRooms = numRoom;
            this.segment.numberOfRoomsSpecified = true;
            this.segment.totalNumberOfGuests = numAdult;
            this.segment.totalNumberOfGuestsSpecified = true;
            this.segment.numberOfChildren = numChild;
            this.segment.numberOfChildrenSpecified = true;
            this.segment.availReqType = AvailRequestType.Room;


            this._InitializeHotelRef(chainCode, hotelCode);

            this.roomStay.invBlockCode = null;

            this.segment.HotelSearchCriteria = new HotelSearchCriterion[1];
            this.segment.RoomStayCandidates = new RoomStayCandidate[1];
            this.request.AvailRequestSegment = new AvailRequestSegment[1];

            this.segment.HotelSearchCriteria[0] = this.hotelSearch;
            this.segment.RoomStayCandidates[0] = this.roomStay;
            this.segment.RateRange = this.rate;
            this.segment.StayDateRange = this.timeSpan;

            switch (rateType)
            {
                case "corporate":
                    this.segment.RatePlanCandidates = new Availability.RatePlanCandidate[1];
                    this.segment.RatePlanCandidates[0] = new Availability.RatePlanCandidate();
                    this.segment.RatePlanCandidates[0].qualifyingIdType = "CORPORATE";
                    this.segment.RatePlanCandidates[0].qualifyingIdValue = rateCode;
                    break;
                case "travelAgent":
                    this.segment.RatePlanCandidates = new Availability.RatePlanCandidate[1];
                    this.segment.RatePlanCandidates[0] = new Availability.RatePlanCandidate();
                    this.segment.RatePlanCandidates[0].qualifyingIdType = "TRAVEL_AGENT";
                    this.segment.RatePlanCandidates[0].qualifyingIdValue = rateCode;
                    break;
                case "promotion":
                    this.segment.RatePlanCandidates = new Availability.RatePlanCandidate[1];
                    this.segment.RatePlanCandidates[0] = new Availability.RatePlanCandidate();
                    this.segment.RatePlanCandidates[0].promotionCode = rateCode;
                    break;
                case "normal":
                default:
                    break;
            }

            this.request.AvailRequestSegment[0] = this.segment;
            try
            {
                response = this.response = cli.Availability(ref this.og, this.request);
            }
            catch (Exception e)
            {
                this.errors = e;
            }


            if (response.Result.GDSError == null)
            {
                var temp_result = new
                {
                    statusCode = 0,
                    statusMessage = "",
                    roomTypes = response.AvailResponseSegments[0].RoomStayList[0].RoomTypes,
                    roomTypesCount = response.AvailResponseSegments[0].RoomStayList[0].RoomTypes.Count<RoomType>(),
                    roomRates = response.AvailResponseSegments[0].RoomStayList[0].RoomRates,
                    roomRatesCount = response.AvailResponseSegments[0].RoomStayList[0].RoomRates.Count<RoomRate>()
                };
                this.tempObj = temp_result;
            }
            else
            {
                var temp_result = new
                {
                    statusCode = response.Result.GDSError.errorCode,
                    statusMessage = response.Result.GDSError.Value,
                    roomTypes = "",
                    roomTypesCount = "",
                    roomRates = "",
                    roomRatesCount = ""
                };
                this.tempObj = temp_result;
            }
            this.finalResponse.Add(this.tempObj);
            //return this.finalResponse;
            return this.tempObj;
            //return response;
        }

        public Object GetAvailablePackages(DateTime startDate, DateTime endDate, int numRoom = 1, int numAdult = 1, int numChild = 0, string chainCode = "CHA", string hotelCode = "WCCH")
        {
            this._InitializeHeader();
            this._InitializeHotelRef(chainCode, hotelCode);
            this._InitializeTimeSpan(startDate, endDate);

            this.packageRequest.StayDateRange = timeSpan;
            this.packageRequest.HotelReference = hotelRef;

            this.packageRequest.NumberOfRooms = numRoom;
            this.packageRequest.NumberOfRoomsSpecified = true;

            this.packageRequest.NumberOfAdults = numAdult;
            this.packageRequest.NumberOfAdultsSpecified = true;

            this.packageRequest.NumberOfChildren = numChild;
            this.packageRequest.NumberOfChildrenSpecified = true;

            this.packageResponse = cli.FetchAvailablePackages(ref this.og, this.packageRequest);
            return this.packageResponse;
        }
    }
}