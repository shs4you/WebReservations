﻿using System;
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
        protected Availability.TimeSpan timeSpan;
        protected MinMaxRate rate;
        protected AvailabilityResponse response;


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
            this.timeSpan = new Availability.TimeSpan();
            this.rate = new MinMaxRate();
            this.response = new AvailabilityResponse();
        }

        public AvailabilityResponse GetAvailableRooms(DateTime startDate, DateTime endDate, int numRoom = 1, int numAdult = 1, int numChild = 0, string rateType = "normal", string rateCode = "", string currencyCode = "PHP", string chainCode = "CHA", string hotelCode = "WCCH")
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

            this.request.summaryOnly = true;

            this.timeSpan.StartDate = startDate;
            this.timeSpan.Item = endDate;

            this.rate.currencyCode = currencyCode;
            this.rate.minimumRateSpecified = false;
            this.rate.maximumRateSpecified = false;

            this.segment.numberOfRooms = numRoom;
            this.segment.numberOfRoomsSpecified = true;
            this.segment.totalNumberOfGuests = numAdult;
            this.segment.totalNumberOfGuestsSpecified = true;
            this.segment.numberOfChildren = numChild;
            this.segment.numberOfChildrenSpecified = true;
            this.segment.availReqType = AvailRequestType.Room;


            this.hotelRef.chainCode = chainCode;
            this.hotelRef.hotelCode = hotelCode;
            this.hotelSearch.HotelRef = hotelRef;

            this.roomStay.invBlockCode = null;

            this.segment.HotelSearchCriteria = new HotelSearchCriterion[1];
            this.segment.RoomStayCandidates = new RoomStayCandidate[1];
            this.request.AvailRequestSegment = new AvailRequestSegment[1];

            this.segment.HotelSearchCriteria[0] = hotelSearch;
            this.segment.RoomStayCandidates[0] = roomStay;
            this.segment.RateRange = rate;
            this.segment.StayDateRange = timeSpan;
            
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

            this.request.AvailRequestSegment[0] = segment;
            response = this.response = cli.Availability(ref og, request);

            return response;
        }
    }
}