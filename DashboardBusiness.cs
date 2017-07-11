using System;
using System.Collections.Generic;
using System.Linq;
using ATGV3.Business.Model;
using ATGV3.Common;
using System.Data.SqlClient;

namespace ATGV3.Business
{
    public class DashboardBusiness : IDisposable
    {
        private readonly AspNetUser _user;
        private readonly ApplicationServices _appServices;
        private readonly ATGV3Entities _db;

        public DashboardBusiness(ApplicationServicesSetup appServicesSetup, AspNetUser user)
        {
            _user = user;
            _appServices = new ApplicationServices(appServicesSetup);
            _db = new ATGV3Entities();
        }

        public List<MonthlyStats> GetYTDStats(int? officeId, int? userId, int? groupId)
        {
            List<MonthlyStats> results = null;

            if (officeId != null || userId != null || groupId != null)
                return _Map(_db.spYtdStats(officeId, groupId, userId).ToList());


            //default behavior
            if (_user.IsAdmin())
            {
                results = _db.Database.SqlQuery<MonthlyStats>("select * from vCompanyYtdStats").ToList();
                return results;
            }

            else if (_user.IsEmployee() || _user.IsManager())

                return _Map(_db.spYtdStats(_user.OfficeId, null, null).ToList());


            else
                return _Map(_db.spYtdStats(null, null, _user.Id).ToList());

        }

        public List<MonthlyStats> LoadsCovered(int? officeId = null, int? userId = null, int? groupId = null)
        {
            List<LoadsCoveredByMonthResult> list = _db.GetLoadsCoveredByMonth(officeId, userId, groupId).ToList();
            List<MonthlyStats> results = list.Select(i => new MonthlyStats 
            { 
               LoadsCovered = i.LoadsCovered.Value,
               Month = i.Month,
               Year = i.Year
            }).ToList();

            return results;
        }

        public List<MonthlyStats> OrdersCreated(int? officeId = null, int? userId = null, int? groupId = null)
        {
            List<OrdersCreatedByMonthResult> list = _db.GetOrdersCreatedByMonth(officeId, userId, groupId).ToList();
            List<MonthlyStats> results = list.Select(i => new MonthlyStats
            {
                OrdersCreated = i.OrdersCreated.Value,
                Month = i.Month,
                Year = i.Year
            }).ToList();

            return results;
        }

        private List<MonthlyStats> _Map(List<YtdStats> list)
        {
            List<MonthlyStats> results = list.Select(i => new MonthlyStats
            {
                Revenue = i.Revenue,
                Cost = i.Cost,
                Profit = i.Profit,
                NumLoads = i.NumLoads,
                Month = i.Month,
                Year = i.Year
            }).ToList();
            

            var currentYear = DateTime.Now.Year;

            //fill in blank months
            foreach (int year in new int[] { currentYear - 1, currentYear })
            {
                for (var month = 1; month < 12; month++)
                {

                    if (results.FirstOrDefault(i => i.Year == year && i.Month == month) == null)
                    {
                        results.Add(new MonthlyStats()
                        {
                            Month = month,
                            Year = year,
                            Cost = 0,
                            Revenue = 0
                        });
                    }
                }
            }

            return results.OrderBy(x => x.Month.Value + x.Year.Value).ToList();
        }


        public List<OfficeStats> OfficeStats(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var param1 = new SqlParameter("p0", startDate.ToShortDateString() );
            var param2 = new SqlParameter("p1", endDate.ToShortDateString() );

            List<OfficeStats> res = _db.Database.SqlQuery<OfficeStats>("execute spOfficeStatsByMonth @p0, @p1", param1, param2).ToList();
            return res;
        }

        public List<OutstandingInvoicesResult> GetOverdueInvoices()
        {
            List<OutstandingInvoicesResult> res = _db.GetOutstandingInvoices(_user.Id).ToList();
            return res;
        }

        public List<RevenueByOfficeGroupResult> GetRevenueByGroup(DateTime? begin, DateTime? end, int? officeId)
        {
            int _officeId = officeId != null ? officeId.Value :_user.OfficeId;
            List<RevenueByOfficeGroupResult> res = _db.RevenueByOfficeGroup(_officeId, begin, end).Where(r => r.Name != "Admin" && r.Name != "Managers" && r.Name != "The Office").ToList();
            return res;
        }

        public List<RevenueByUserResult> GetRevenueByUser(DateTime? begin, DateTime? end, int? officeId)
        {
            int _officeId = officeId != null ? officeId.Value : _user.OfficeId;
            List<RevenueByUserResult> res = _db.GetRevenueByUser(_officeId, begin, end).ToList();
            return res;
        }

        public List<RevenueBySalesManagerResult> GetRevenueBySalesManager(DateTime? begin, DateTime? end, int? officeId)
        {
            int _officeId = officeId != null ? officeId.Value : _user.OfficeId;
            List<RevenueBySalesManagerResult> res = _db.GetRevenueBySalesManager(_officeId, begin, end).ToList();
            return res;
        }

        public List<LoadsCoveredByUserResult> GetLoadsCoveredByUser(DateTime? begin, DateTime? end, int? officeId)
        {
            int _officeId = officeId != null ? officeId.Value : _user.OfficeId;
            List<LoadsCoveredByUserResult> res = _db.GetLoadsCoveredByUser(_officeId, begin, end).ToList();
            return res;
        }

        public List<TopCustomersByRevenueResult> GetTopCustomersByRevenue(DateTime? begin, DateTime? end)
        {
            List<TopCustomersByRevenueResult> res = _db.GetTopCustomersByRevenue(begin, end, _user.Id).ToList();
            return res;
        }

        public List<YtdRevenueByCustomerResult> GetYtdRevenueByCustomer(int custId)
        {
            List<YtdRevenueByCustomerResult> results = _db.GetYtdRevenueByCustomer(custId).ToList();

            var currentYear = DateTime.Now.Year;

            //fill in blank months
            foreach(int year in new int[]{ currentYear-1, currentYear}){
                for(var month = 1; month < 12; month++){

                    if (results.FirstOrDefault(i => i.Year == year && i.Month == month) == null)
                    {
                        results.Add( new YtdRevenueByCustomerResult(){
                            Month = month,
                            Year = year,
                            Cost = 0,
                            Profit = 0,
                            Revenue = 0
                        });
                    }
                }
            }

            return results.OrderBy(x => x.Month.Value + x.Year.Value).ToList();
        }

        /*
        public int OrdersCreatedByMonth(int? officeId, int? userId, int? groupId)
        {
             _db.spOrdersCreatedByMonth(officeId, userId, groupId)
        }*/

        public void Dispose()
        {
            _db.Dispose();
            _appServices.Dispose();
        }
    }

    public class OfficeStats
    {
        public int OfficeId { get; set; }
        public string Name { get; set; }
        public int NumLoads { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
    }

    public class MonthlyStats : YtdStats
    {
        public int LoadsCovered { get; set; }
        public int OrdersCreated { get; set; }
        public decimal? Margin
        {
            get
            {
                if (Revenue == 0 || Revenue == null) return 0;

                var margin = 100 * (1 - Cost / Revenue);
                return Decimal.Round(margin.Value, 2);
            }
        }
    }
}