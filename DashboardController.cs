using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ATGV3.Business;
using ATGV3.Web;
using ATGV3.Business.Model;
using ATGV3.Web.Helpers;
using System.Web.Script.Serialization;

namespace ATGV3.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly DashboardBusiness _dashboardBusiness;

        public DashboardController()
        {
            _dashboardBusiness = new DashboardBusiness(_applicationServicesSetup, _user);
        }

        // GET: Dashboard
        public ActionResult Index()
        {

            //if user does not have permission to view the dashboard
           
            using (var OfficeBusiness = new OfficeBusiness(_applicationServicesSetup, _user))
            {
                ViewBag.AccountingGroup = OfficeBusiness.GetAccountingGroupByOffice(_user.OfficeId);
            }

            this.SetSelectors();
            return View();
        }

        public ActionResult Dev()
        {
            return View();
        }

        public ActionResult Internal()
        {
            return View();
        }

        private void SetSelectors()
        {
            using (OfficeBusiness officeBusiness = new OfficeBusiness(_applicationServicesSetup, _user))
            {
                if (_user.IsAdmin())
                    ViewBag.OfficeSelector = Helpers.SelectListHelper.Offices(officeBusiness.ActiveOffices(), "-- Select an Office --");


                else if (_user.IsManager() || _user.IsEmployee())
                {
                    ViewBag.GroupsSelector = Helpers.SelectListHelper.Groups(officeBusiness.GetUserGroupsByOffice(_user.OfficeId), "-- Select a Group --");
                }
            }

            using (UserBusiness userBusiness = new UserBusiness(_applicationServicesSetup, _user))
            {
                ViewBag.UserSelector = Helpers.SelectListHelper.Users(userBusiness.GetAllUsers(), "-- Select a User --");
            }
        }

        #region Ajax Calls
        [Route("Dashboard/YtdStats")]
        public ActionResult YtdStats(int? officeId = null, int? userId = null, int? groupId = null)
        {
            return Json(_dashboardBusiness.GetYTDStats(officeId, userId, groupId), JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoadsCovered(int? officeId = null, int? userId = null, int? groupId = null)
        {
            return Json(_dashboardBusiness.LoadsCovered(officeId, userId, groupId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrdersCreated(int? officeId = null, int? userId = null, int? groupId = null)
        {
            return Json(_dashboardBusiness.OrdersCreated(officeId, userId, groupId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOfficeStats(int? month = null, int? year = null)
        {
            if (month == null) month = DateTime.Now.Month;
            if (year == null) year = DateTime.Now.Year;

            return Json(_dashboardBusiness.OfficeStats(month.Value, year.Value), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOverdueInvoices()
        {
            return Json(_dashboardBusiness.GetOverdueInvoices(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRevenueByGroup(int? month, int? year, int? officeId)
        {
            DateTimesForMonth times = getDatesForMonth(month, year);
            return Json(_dashboardBusiness.GetRevenueByGroup(times.StartOfMonth, times.EndOfMonth, officeId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRevenueByUser(int? month, int? year, int? officeId)
        {
            DateTimesForMonth times = getDatesForMonth(month, year);
            return Json(_dashboardBusiness.GetRevenueByUser(times.StartOfMonth, times.EndOfMonth, officeId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRevenueBySalesManager(int? month, int? year, int? officeId)
        {
            DateTimesForMonth times = getDatesForMonth(month, year);
            return Json(_dashboardBusiness.GetRevenueBySalesManager(times.StartOfMonth, times.EndOfMonth, officeId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoadsCoveredByUser(int? month, int? year, int? officeId)
        {
            DateTimesForMonth times = getDatesForMonth(month, year);
            return Json(_dashboardBusiness.GetLoadsCoveredByUser(times.StartOfMonth, times.EndOfMonth, officeId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetYtdRevenueByCustomer(int custId)
        {
            return Json(_dashboardBusiness.GetYtdRevenueByCustomer(custId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopCustomersByRevenue(DateTime? begin, DateTime? end)
        {
            return Json(_dashboardBusiness.GetTopCustomersByRevenue(begin, end), JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult PendingSettlements()
        {
            using (AccountingBusiness accountingBusiness = new AccountingBusiness(_applicationServicesSetup, _user))
            {
                var results = accountingBusiness.GetSettlements();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }

        private DateTimesForMonth getDatesForMonth(int? month, int? year)
        {
            if (month == null || year == null) return new DateTimesForMonth();
            var startOfMonth = new DateTime(year.Value, month.Value, 1);
            return new DateTimesForMonth()
            {
                StartOfMonth = startOfMonth,
                EndOfMonth = startOfMonth.AddMonths(1).AddDays(-1)
            };
        }

        private class DateTimesForMonth
        {
            public DateTime? StartOfMonth { get; set; }
            public DateTime? EndOfMonth { get; set; }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _dashboardBusiness.Dispose();
            base.Dispose(disposing);
        }
    }
}