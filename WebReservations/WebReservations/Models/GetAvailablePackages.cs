using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebReservations.Models
{
    public class GetAvailablePackages
    {
        public string datepickerArrival { get; set; }
        public string datepickerDeparture { get; set; }
        public int adultCount { get; set; }
        public int childCount { get; set; }
        public string groupCode { get; set; }
        public string promoCode { get; set; }
        public string travelId { get; set; }
    }
}