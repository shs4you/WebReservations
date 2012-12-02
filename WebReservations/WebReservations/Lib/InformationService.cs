using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using WebReservations.Information;

namespace WebReservations.Lib
{
    public class InformationService
    {
        protected QueryLovRequest queryLovRequest = new QueryLovRequest();
        protected QueryLovResponse queryLovResponse = new QueryLovResponse();
        protected InformationSoapClient cli = new InformationSoapClient();
        protected OGHeader og = new OGHeader();
        protected EndPoint origin = new EndPoint();
        protected EndPoint dest = new EndPoint();
        protected LovRequest lovReq = new LovRequest();
        protected LovQueryQualifierType[] qualiferTypes = new LovQueryQualifierType[1];
        protected LovQueryType2 queryType = new LovQueryType2();
        protected RateQueryType rateType = new RateQueryType();
        protected RateRequest rateRequest = new RateRequest();
        protected Information.TimeSpan timeSpan = new Information.TimeSpan();

        public InformationService()
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

        public LovResponse getQuery(string queryType)
        {
            this.queryType.LovIdentifier = queryType;
            this.lovReq.Item = this.queryType;
            return this.cli.QueryLov(ref this.og, this.lovReq);
        }

        public LovResponse getRoomTypes(string queryType = "ROOMTYPES")
        {
            this.qualiferTypes[0] = new LovQueryQualifierType();
            this.qualiferTypes[0].qualifierType = System.Configuration.ConfigurationManager.AppSettings["OriginSystemType"];
            this.qualiferTypes[0].Value = System.Configuration.ConfigurationManager.AppSettings["OriginEntityId"];
            this.queryType.LovQueryQualifier = this.qualiferTypes;
            return this.getQuery(queryType);
        }

        public LovResponse getFeatures(string queryType = "FEATURE")
        {
            return this.getQuery(queryType);
        }

        public RateResponse getRates(DateTime startDate, DateTime endDate, string hotelCode = "WCCH", string rateCode = "OWCCH")
        {
            this.rateType.hotelCode = hotelCode;
            this.rateType.rateCode = rateCode;
            this.timeSpan.StartDate = startDate;
            this.timeSpan.Item = endDate;
            this.rateType.TimeSpan = this.timeSpan;
            this.rateRequest.RateQuery = this.rateType;
            return this.cli.QueryRate(ref this.og, this.rateRequest);
        }
    }
}