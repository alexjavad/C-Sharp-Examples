﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ATGV3.Business.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ATGV3Entities : DbContext
    {
        public ATGV3Entities()
            : base("name=ATGV3Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Carrier> Carriers { get; set; }
        public virtual DbSet<CarrierAlert> CarrierAlerts { get; set; }
        public virtual DbSet<CarrierHistory> CarrierHistories { get; set; }
        public virtual DbSet<CustomerComment> CustomerComments { get; set; }
        public virtual DbSet<CustomerContact> CustomerContacts { get; set; }
        public virtual DbSet<CustomerHistory> CustomerHistories { get; set; }
        public virtual DbSet<CustomerPayment> CustomerPayments { get; set; }
        public virtual DbSet<Equipment> Equipments { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Lane> Lanes { get; set; }
        public virtual DbSet<LoadHistory> LoadHistories { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderHistory> OrderHistories { get; set; }
        public virtual DbSet<ProspectContact> ProspectContacts { get; set; }
        public virtual DbSet<CarrierContact> CarrierContacts { get; set; }
        public virtual DbSet<LoadComment> LoadComments { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<VendorLineItem> VendorLineItems { get; set; }
        public virtual DbSet<VendorContract> VendorContracts { get; set; }
        public virtual DbSet<OrderComment> OrderComments { get; set; }
        public virtual DbSet<CarrierComment> CarrierComments { get; set; }
        public virtual DbSet<CarrierRating> CarrierRatings { get; set; }
        public virtual DbSet<Office> Offices { get; set; }
        public virtual DbSet<ProspectComment> ProspectComments { get; set; }
        public virtual DbSet<Load> Loads { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<OrderLineItem> OrderLineItems { get; set; }
        public virtual DbSet<LoadLineItem> LoadLineItems { get; set; }
        public virtual DbSet<Stop> Stops { get; set; }
        public virtual DbSet<AppMessage> AppMessages { get; set; }
        public virtual DbSet<AppMessageUser> AppMessageUsers { get; set; }
        public virtual DbSet<OrderFlag> OrderFlags { get; set; }
        public virtual DbSet<OrderFlagComment> OrderFlagComments { get; set; }
        public virtual DbSet<ProspectCustomer> ProspectCustomers { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<AspNetUserSetting> AspNetUserSettings { get; set; }
        public virtual DbSet<OrderTemplate> OrderTemplates { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<config> configs { get; set; }
        public virtual DbSet<CarrierBlacklist> CarrierBlacklists { get; set; }
        public virtual DbSet<vApprovedQueue> vApprovedQueues { get; set; }
        public virtual DbSet<vLoadDetail> vLoadDetails { get; set; }
        public virtual DbSet<vOpenOrderBalance> vOpenOrderBalances { get; set; }
        public virtual DbSet<vOrderFinancial> vOrderFinancials { get; set; }
        public virtual DbSet<vUploadQueue> vUploadQueues { get; set; }
        public virtual DbSet<OrderAdjustment> OrderAdjustments { get; set; }
        public virtual DbSet<CollectionsAction> CollectionsActions { get; set; }
        public virtual DbSet<CollectionBucket> CollectionBuckets { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<UserPermission> UserPermissions { get; set; }
        public virtual DbSet<UserSavedSearch> UserSavedSearches { get; set; }
        public virtual DbSet<vLoadDetailsDAT> vLoadDetailsDATs { get; set; }
        public virtual DbSet<vLoadLane> vLoadLanes { get; set; }
    
        public virtual ObjectResult<LoadboardLine> uspLoadBoard()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoadboardLine>("uspLoadBoard");
        }
    
        public virtual int SettleOrders(Nullable<int> orderId)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SettleOrders", orderIdParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> spCustomer_AvailableCredit(Nullable<int> customer_id)
        {
            var customer_idParameter = customer_id.HasValue ?
                new ObjectParameter("customer_id", customer_id) :
                new ObjectParameter("customer_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("spCustomer_AvailableCredit", customer_idParameter);
        }
    
        public virtual ObjectResult<Nullable<System.DateTime>> spCustomer_LastLoadDate(Nullable<int> customerId)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("customerId", customerId) :
                new ObjectParameter("customerId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.DateTime>>("spCustomer_LastLoadDate", customerIdParameter);
        }
    
        public virtual ObjectResult<CustomerOrderSummary> spCustomerOrderSummary(Nullable<int> customer_id)
        {
            var customer_idParameter = customer_id.HasValue ?
                new ObjectParameter("customer_id", customer_id) :
                new ObjectParameter("customer_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CustomerOrderSummary>("spCustomerOrderSummary", customer_idParameter);
        }
    
        public virtual ObjectResult<CustomerOutstandingOrders> spCustomerOutstandingOrders(Nullable<int> customer_id)
        {
            var customer_idParameter = customer_id.HasValue ?
                new ObjectParameter("customer_id", customer_id) :
                new ObjectParameter("customer_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CustomerOutstandingOrders>("spCustomerOutstandingOrders", customer_idParameter);
        }
    
        public virtual ObjectResult<Stop> spLoad_TopStops(string stopType, Nullable<int> customerId)
        {
            var stopTypeParameter = stopType != null ?
                new ObjectParameter("StopType", stopType) :
                new ObjectParameter("StopType", typeof(string));
    
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Stop>("spLoad_TopStops", stopTypeParameter, customerIdParameter);
        }
    
        public virtual ObjectResult<Stop> spLoad_TopStops(string stopType, Nullable<int> customerId, MergeOption mergeOption)
        {
            var stopTypeParameter = stopType != null ?
                new ObjectParameter("StopType", stopType) :
                new ObjectParameter("StopType", typeof(string));
    
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Stop>("spLoad_TopStops", mergeOption, stopTypeParameter, customerIdParameter);
        }
    
        public virtual ObjectResult<CustomerFinancials> spCustomerFinancials(Nullable<int> customer_id)
        {
            var customer_idParameter = customer_id.HasValue ?
                new ObjectParameter("customer_id", customer_id) :
                new ObjectParameter("customer_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CustomerFinancials>("spCustomerFinancials", customer_idParameter);
        }
    
        public virtual ObjectResult<Stop> spUniqueStops(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Stop>("spUniqueStops", userIdParameter);
        }
    
        public virtual ObjectResult<Stop> spUniqueStops(Nullable<int> userId, MergeOption mergeOption)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Stop>("spUniqueStops", mergeOption, userIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> spGetCustomersByUser(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spGetCustomersByUser", userIdParameter);
        }
    
        public virtual int spGetDormantAccounts(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spGetDormantAccounts", userIdParameter);
        }
    
        public virtual int spOutstandingInvoices(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spOutstandingInvoices", currUserIdParameter);
        }
    
        public virtual int spRevenueByOfficeGroup(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spRevenueByOfficeGroup", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual int spTopCustomersByRevenue(Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate, Nullable<int> currentUserId)
        {
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var currentUserIdParameter = currentUserId.HasValue ?
                new ObjectParameter("CurrentUserId", currentUserId) :
                new ObjectParameter("CurrentUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spTopCustomersByRevenue", beginDateParameter, endDateParameter, currentUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> spUserLoadsCoveredThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spUserLoadsCoveredThisMonth", currUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> spUserLoadsCreatedThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spUserLoadsCreatedThisMonth", currUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> spUserOfficeLoadsCoveredThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spUserOfficeLoadsCoveredThisMonth", currUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> spUserOfficeLoadsCreatedThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spUserOfficeLoadsCreatedThisMonth", currUserIdParameter);
        }
    
        public virtual ObjectResult<DormantAccountResult> GetDormantAccounts(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DormantAccountResult>("GetDormantAccounts", userIdParameter);
        }
    
        public virtual ObjectResult<RevenueByOfficeGroupResult> RevenueByOfficeGroup(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RevenueByOfficeGroupResult>("RevenueByOfficeGroup", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<OutstandingInvoicesResult> GetOutstandingInvoices(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<OutstandingInvoicesResult>("GetOutstandingInvoices", currUserIdParameter);
        }
    
        public virtual ObjectResult<TopCustomersByRevenueResult> GetTopCustomersByRevenue(Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate, Nullable<int> currentUserId)
        {
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var currentUserIdParameter = currentUserId.HasValue ?
                new ObjectParameter("CurrentUserId", currentUserId) :
                new ObjectParameter("CurrentUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TopCustomersByRevenueResult>("GetTopCustomersByRevenue", beginDateParameter, endDateParameter, currentUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> UserLoadsCreatedThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("UserLoadsCreatedThisMonth", currUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> UserLoadsCoveredThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("UserLoadsCoveredThisMonth", currUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> UserOfficeLoadsCreatedThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("UserOfficeLoadsCreatedThisMonth", currUserIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> UserOfficeLoadsCoveredThisMonth(Nullable<int> currUserId)
        {
            var currUserIdParameter = currUserId.HasValue ?
                new ObjectParameter("CurrUserId", currUserId) :
                new ObjectParameter("CurrUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("UserOfficeLoadsCoveredThisMonth", currUserIdParameter);
        }
    
        public virtual int spRevenueByUser(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spRevenueByUser", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<RevenueByUserResult> GetRevenueByUser(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RevenueByUserResult>("GetRevenueByUser", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual int spLoadsCoveredByUser(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spLoadsCoveredByUser", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<LoadsCoveredByUserResult> GetLoadsCoveredByUser(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoadsCoveredByUserResult>("GetLoadsCoveredByUser", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual int spYtdRevenueByCustomer(Nullable<int> customerId)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spYtdRevenueByCustomer", customerIdParameter);
        }
    
        public virtual ObjectResult<YtdRevenueByCustomerResult> GetYtdRevenueByCustomer(Nullable<int> customerId)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<YtdRevenueByCustomerResult>("GetYtdRevenueByCustomer", customerIdParameter);
        }
    
        public virtual int spRevenueBySalesManager(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spRevenueBySalesManager", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<RevenueBySalesManagerResult> GetRevenueBySalesManager(Nullable<int> currUserOfficeId, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var currUserOfficeIdParameter = currUserOfficeId.HasValue ?
                new ObjectParameter("CurrUserOfficeId", currUserOfficeId) :
                new ObjectParameter("CurrUserOfficeId", typeof(int));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("BeginDate", beginDate) :
                new ObjectParameter("BeginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RevenueBySalesManagerResult>("GetRevenueBySalesManager", currUserOfficeIdParameter, beginDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<YtdStats> spYtdStats(Nullable<int> officeId, Nullable<int> groupId, Nullable<int> userId)
        {
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            var groupIdParameter = groupId.HasValue ?
                new ObjectParameter("GroupId", groupId) :
                new ObjectParameter("GroupId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<YtdStats>("spYtdStats", officeIdParameter, groupIdParameter, userIdParameter);
        }
    
        public virtual ObjectResult<CollectionsResult> spCollectionsList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CollectionsResult>("spCollectionsList");
        }
    
        public virtual ObjectResult<NeedsPaperworkQueueItem> spNeedPaperworkQueue(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NeedsPaperworkQueueItem>("spNeedPaperworkQueue", userIdParameter);
        }
    
        public virtual ObjectResult<LoadsByCustomerResult> GetLoadsByCustomer(Nullable<int> customerId)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoadsByCustomerResult>("GetLoadsByCustomer", customerIdParameter);
        }
    
        public virtual ObjectResult<SettlementResult> spSettlements(Nullable<System.DateTime> settledDate, Nullable<int> officeId)
        {
            var settledDateParameter = settledDate.HasValue ?
                new ObjectParameter("SettledDate", settledDate) :
                new ObjectParameter("SettledDate", typeof(System.DateTime));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SettlementResult>("spSettlements", settledDateParameter, officeIdParameter);
        }
    
        public virtual ObjectResult<PendingApprovalsResult> spGetPendingApprovals(Nullable<int> currentUserId, Nullable<int> accountingGroupId)
        {
            var currentUserIdParameter = currentUserId.HasValue ?
                new ObjectParameter("CurrentUserId", currentUserId) :
                new ObjectParameter("CurrentUserId", typeof(int));
    
            var accountingGroupIdParameter = accountingGroupId.HasValue ?
                new ObjectParameter("AccountingGroupId", accountingGroupId) :
                new ObjectParameter("AccountingGroupId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PendingApprovalsResult>("spGetPendingApprovals", currentUserIdParameter, accountingGroupIdParameter);
        }
    
        public virtual ObjectResult<PendingApprovalsResult> GetPendingApprovals(Nullable<int> currentUserId, Nullable<int> accountingGroupId)
        {
            var currentUserIdParameter = currentUserId.HasValue ?
                new ObjectParameter("CurrentUserId", currentUserId) :
                new ObjectParameter("CurrentUserId", typeof(int));
    
            var accountingGroupIdParameter = accountingGroupId.HasValue ?
                new ObjectParameter("AccountingGroupId", accountingGroupId) :
                new ObjectParameter("AccountingGroupId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PendingApprovalsResult>("GetPendingApprovals", currentUserIdParameter, accountingGroupIdParameter);
        }
    
        public virtual int spSettleOrderLineItems(string orderIds)
        {
            var orderIdsParameter = orderIds != null ?
                new ObjectParameter("OrderIds", orderIds) :
                new ObjectParameter("OrderIds", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spSettleOrderLineItems", orderIdsParameter);
        }
    
        public virtual int spSettleOrders(string orderIds)
        {
            var orderIdsParameter = orderIds != null ?
                new ObjectParameter("OrderIds", orderIds) :
                new ObjectParameter("OrderIds", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spSettleOrders", orderIdsParameter);
        }
    
        public virtual int spGetCarrierOldLoadsData(Nullable<int> carrierId)
        {
            var carrierIdParameter = carrierId.HasValue ?
                new ObjectParameter("CarrierId", carrierId) :
                new ObjectParameter("CarrierId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spGetCarrierOldLoadsData", carrierIdParameter);
        }
    
        public virtual ObjectResult<CarrierOldLoadsDataResult> GetCarrierOldLoadsData(Nullable<int> carrierId)
        {
            var carrierIdParameter = carrierId.HasValue ?
                new ObjectParameter("CarrierId", carrierId) :
                new ObjectParameter("CarrierId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CarrierOldLoadsDataResult>("GetCarrierOldLoadsData", carrierIdParameter);
        }
    
        public virtual int spBulkPayOrders(string orderIds)
        {
            var orderIdsParameter = orderIds != null ?
                new ObjectParameter("OrderIds", orderIds) :
                new ObjectParameter("OrderIds", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spBulkPayOrders", orderIdsParameter);
        }
    
        public virtual int UnsettleThings(string date)
        {
            var dateParameter = date != null ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UnsettleThings", dateParameter);
        }
    
        public virtual int UnsettleByDate(string date)
        {
            var dateParameter = date != null ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UnsettleByDate", dateParameter);
        }
    
        public virtual int BulkPayOrders(string orderIds)
        {
            var orderIdsParameter = orderIds != null ?
                new ObjectParameter("OrderIds", orderIds) :
                new ObjectParameter("OrderIds", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BulkPayOrders", orderIdsParameter);
        }
    
        public virtual ObjectResult<spLoadsCoveredByMonthResult> spLoadsCoveredByMonth(Nullable<int> officeId, Nullable<int> userId, Nullable<int> groupId)
        {
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var groupIdParameter = groupId.HasValue ?
                new ObjectParameter("GroupId", groupId) :
                new ObjectParameter("GroupId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spLoadsCoveredByMonthResult>("spLoadsCoveredByMonth", officeIdParameter, userIdParameter, groupIdParameter);
        }
    
        public virtual ObjectResult<spOrdersCreatedByMonthResult> spOrdersCreatedByMonth(Nullable<int> officeId, Nullable<int> userId, Nullable<int> groupId)
        {
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var groupIdParameter = groupId.HasValue ?
                new ObjectParameter("GroupId", groupId) :
                new ObjectParameter("GroupId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spOrdersCreatedByMonthResult>("spOrdersCreatedByMonth", officeIdParameter, userIdParameter, groupIdParameter);
        }
    
        public virtual ObjectResult<spSettlementReportLine> spSettlementReport(Nullable<System.DateTime> settledDate, Nullable<int> officeId)
        {
            var settledDateParameter = settledDate.HasValue ?
                new ObjectParameter("SettledDate", settledDate) :
                new ObjectParameter("SettledDate", typeof(System.DateTime));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spSettlementReportLine>("spSettlementReport", settledDateParameter, officeIdParameter);
        }
    
        public virtual int spDeleteCarrierEquipment_DELETEME(Nullable<int> carrierId)
        {
            var carrierIdParameter = carrierId.HasValue ?
                new ObjectParameter("CarrierId", carrierId) :
                new ObjectParameter("CarrierId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spDeleteCarrierEquipment_DELETEME", carrierIdParameter);
        }
    
        public virtual int spDeleteCarrierLane_DELETEME(Nullable<int> carrierId)
        {
            var carrierIdParameter = carrierId.HasValue ?
                new ObjectParameter("CarrierId", carrierId) :
                new ObjectParameter("CarrierId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spDeleteCarrierLane_DELETEME", carrierIdParameter);
        }
    
        public virtual ObjectResult<LoadsCoveredByMonthResult> GetLoadsCoveredByMonth(Nullable<int> officeId, Nullable<int> userId, Nullable<int> groupId)
        {
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var groupIdParameter = groupId.HasValue ?
                new ObjectParameter("GroupId", groupId) :
                new ObjectParameter("GroupId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoadsCoveredByMonthResult>("GetLoadsCoveredByMonth", officeIdParameter, userIdParameter, groupIdParameter);
        }
    
        public virtual ObjectResult<OrdersCreatedByMonthResult> GetOrdersCreatedByMonth(Nullable<int> officeId, Nullable<int> userId, Nullable<int> groupId)
        {
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var groupIdParameter = groupId.HasValue ?
                new ObjectParameter("GroupId", groupId) :
                new ObjectParameter("GroupId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<OrdersCreatedByMonthResult>("GetOrdersCreatedByMonth", officeIdParameter, userIdParameter, groupIdParameter);
        }
    }
}