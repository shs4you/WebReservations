using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using WebReservations.Lib;
using WebReservations.Availability;
using WebReservations.Models;

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

        //
        
    }
}
