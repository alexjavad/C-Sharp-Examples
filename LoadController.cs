using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ATGV3.Business;
using ATGV3.Business.Model;
using ATGV3.Business.Model.Custom;
using ATGV3.Web.Helpers;
using Kendo.Mvc.UI;
using ATGV3.Business.ValueObjects;
using ATGV3.Business.Services;
using ATGV3.Web.Models;
using ATGV3.Business.Workflow;
using ATGV3.Common;
using System.Data.Entity.Validation;
using System.Text;
using Newtonsoft.Json;

namespace ATGV3.Web.Controllers
{
    public class LoadController : BaseController
    {
        private readonly LoadBusiness _loadBusiness;

        public LoadController()
        {
            if (_user == null) _user = GetSystemUser();
            _loadBusiness = new LoadBusiness(_applicationServicesSetup, _user);

        }

        // GET: Load
        public ActionResult Index()
        {
            var Loadboard = new LoadboardViewModel();
            Loadboard.StatusSelector = SelectListHelper.Generic(LoadBusiness.GetLoadStatuses(_user));
            
            //retrieve save searches
            Loadboard.SavedSearches = _user.SavedSearches.Where(i => i.Category == Globals.SearchCategories.Loadboard).ToList();

            //retrieve save loadboard settings
            var LoadboardSettings = _user.Settings.FirstOrDefault(i => i.SettingName == "Loadboard");

            if (LoadboardSettings != null)
                Loadboard.Settings = LoadboardSettings.SettingValue;


            using (var OfficeBusiness = new OfficeBusiness(_applicationServicesSetup, _user))
            {
                Loadboard.OfficeGroups = OfficeBusiness.GetUserGroupsByOffice(_user.OfficeId, new[] { Globals.Related.OfficeGroup.Members });

                //if just an agent, only show the groups you are apart of.
                if (_user.IsAgent() && !_user.IsEmployee())
                {
                    Loadboard.OfficeGroups = Loadboard.OfficeGroups.Where(i => i.Members.Select(u => u.Id == _user.Id).Any()).ToList();
                }
            }

            return View(Loadboard);
        }

        public ActionResult Claim(int LoadId)
        {
            var Load = _loadBusiness.Get(LoadId, new[] { 
                Globals.Related.Load.Order, 
                Globals.Related.Load.OrderCustomer,
                Globals.Related.Load.AssignedToUser,
                Globals.Related.Load.OrderUserAssigned,
                Globals.Related.Load.InternalNotes,
            });

            Claim claim = new Claim(Load);

            return View(claim);
        }


        public ActionResult Claims()
        {
            var Claims = _loadBusiness.GetActiveClaims().Select(claim => new ClaimSummary()
            {
                OrderId = claim.OrderId,
                LoadNumber = claim.LoadNumber,
                ClaimDescription = claim.ClaimDescription,
                ClaimValue = claim.ClaimValue.Value,
                Date = claim.ClaimDate.Value.ToShortDateString(),
                Status = claim.ClaimStatus,
                Office = claim.Order.UserAssigned.Office.Name,
                Agent = claim.Order.UserAssigned.Name,
                Customer = claim.Customer.Name,
                Carrier = claim.Carrier.Name
            }).ToList();

            return View(Claims);
        }

        public ActionResult Holds()
        {
            using (OrderBusiness orderBusiness = new OrderBusiness(_applicationServicesSetup, _user))
            {
                var Holds = orderBusiness.GetBillingHolds();
                return View(Holds);
            }
        }

        [HttpPost]
        public ActionResult SaveClaim(Claim claim)
        {
            var Load = _loadBusiness.Get(claim.LoadId);
            claim.FillClaim(Load);

            _loadBusiness.Update(Load);

            return SuccessMessage();
        }


        [HttpGet]
        public JsonResult GetLoadBoard()
        {
            var result = LoadBoardCentral.Instance.GetLoadBoard(_user);

            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search()
        {
            ViewBag.States = SelectListHelper.States("", "State");
            return View();
        }


        #region AjaxActions
        [HttpPost]
        public ActionResult LaneSearchFilter(Address origin, int? originRadius, Address dest, int? destRadius)
        {
            List<vLoadLane> results = _loadBusiness.LaneSearch(origin, originRadius, dest, destRadius);
            if (results == null)
            {
                return ValidationErrorResponse("Search failed, one of your inputs could have been misspelled.  Please correct, and try again.");
            }
            return Json(results);
        }


        public ActionResult Lookup(string query)
        {
            var parts = query.Split('-');

            int orderId;
            if (!Int32.TryParse(parts[0], out orderId))
                return ErrorMessage("Invalid order id.  Input was not a number");

            using (OrderBusiness _orderBusiness = new OrderBusiness(_applicationServicesSetup, _user))
            {
                var order = _orderBusiness.Get(orderId, new[] { Globals.Related.Order.UserAssigned });
                if (order == null)
                    return NotFound();
                else
                    return SuccessMessage(new { Id = orderId });
            }

        }


        public ActionResult AddComment(string id /* Load Id */, CheckCallViewModel comment)
        {
            comment.ObjectId = int.Parse(id);

            if (_loadBusiness.AddComment(int.Parse(id), comment.ToLoadComment()))
            {
                return SuccessMessage(new { User = _user.Name });
            }
            else
                return ErrorMessage("Error occured Saving your comment");

        }

        [HttpPost]
        public ActionResult RequestCheck(int id, decimal Amount, string Comment = "")
        {
            try
            {
                if (_user.IsAdmin() || _user.HasPermission(Globals.Permission.ApproveEChecks))
                {
                    var Payments = _loadBusiness.GetECheck(id, Amount);

                    return SuccessMessage(new { Payments = Payments.Select(i => new PaymentViewModel(i)) });
                }

                else
                {
                    using (ECheckQueue queueBusiness = new ECheckQueue(_applicationServicesSetup, _user))
                    {
                        queueBusiness.Add(Globals.RequestTypes.Check, Amount, Comment, id);
                        return SuccessMessage();
                    }
                }
            }
            catch (Exception e)
            {
                return ErrorMessage(e.Message);
            }
        }

        public ActionResult CheckDetails(string id)
        {
            using (var ECheck = new EFS(_applicationServicesSetup))
            {
                return Json(ECheck.getMoneyCodeDetails(id), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult VoidCheck(string TxnId, int PaymentId)
        {
            if (_loadBusiness.VoidCheck(PaymentId, TxnId))
            {
                return SuccessMessage();
            }
            else
                return ErrorMessage("Failed to void check");

        }

        public ActionResult GetCarrierSummary(int id /*Carrier Id */)
        {
            using (CarrierBusiness carrierBusiness = new CarrierBusiness(_applicationServicesSetup, _user))
            {

                Carrier carrier = carrierBusiness.Get(id, new string[] { 
                    Globals.Related.Carrier.CarrierContacts,
                    Globals.Related.Carrier.CarrierRatings,
                    Globals.Related.Carrier.Blacklists
                });

                List<string> Warnings = new List<string>();

                foreach (CarrierContact c in carrier.CarrierContacts)
                    c.Carrier = null;


                return Json(new
                {
                    Id = carrier.Id,
                    Name = carrier.Name,
                    Contacts = carrier.CarrierContacts,
                    DOT = carrier.DOT,
                    MC = carrier.MC,
                    Address = new Address(carrier.PhysicalAddress1, carrier.PhysicalAddress2, carrier.PhysicalCity, carrier.PhysicalState, carrier.PhysicalZip),
                    Rating = carrier.AverageRating,
                    OfficeRating = carrier.OfficeRating,
                    IsActive = carrier.IsActive(),
                    HasExpiredCargoInsurance = carrier.HasExpiredCargoInsurance,
                    HasExpiredLiabilityInsurance = carrier.HasExpiredLiabilityInsurance,
                    Warnings = _loadBusiness.GetCarrierWarnings(carrier),
                    AdvancesAllowed = !carrier.AllowAdvances,
                    Mode = carrier.Mode,
                    Blacklisted = carrier.CarrierBlacklists.Where(i => i.OfficeId == _user.OfficeId).Any()
                }, JsonRequestBehavior.AllowGet);


            }

        }

        [HttpPost]
        public ActionResult FlagLoad(FlagViewModel Flag)
        {
            OrderFlag _flag = Flag.ToFlag();

            using (OrderFlagBusiness flagBusiness = new OrderFlagBusiness(_applicationServicesSetup, _user))
            {
                if (_flag.Id > 0)
                    flagBusiness.Modify(_flag);
                else
                {
                    int Id = flagBusiness.Create(_flag);
                    var NewFlag = flagBusiness.GetById(Id, new[]{
                        Globals.Related.OrderFlag.FlaggedByUser
                    });

                    Flag = new FlagViewModel(NewFlag);
                }


                if (flagBusiness.ValidationErrors.Any())
                {
                    ValidationErrorResponse(flagBusiness.ValidationErrors);
                }
            }

            return Json(Flag);
        }

        public ActionResult RemoveFlag(int id /* Flag Id*/ )
        {

            using (OrderFlagBusiness flagBusiness = new OrderFlagBusiness(_applicationServicesSetup, _user))
            {
                flagBusiness.ClearFlag(id);

                if (flagBusiness.ValidationErrors.Any())
                {
                    return ValidationErrorResponse(flagBusiness.ValidationErrors);
                }
                else
                    return SuccessMessage();
            }
        }



        public ActionResult ChangeStatus(int id, string Status)
        {
            _loadBusiness.ChangeStatus(id, Status);
            return SuccessMessage();
        }


        public ActionResult AddFlagComment(int id /* Flag Id*/, string comment)
        {
            using (OrderFlagBusiness flagBusiness = new OrderFlagBusiness(_applicationServicesSetup, _user))
            {
                var _Comment = flagBusiness.AddComment(id, comment);

                if (_Comment.Id > 0)
                    return SuccessMessage(new CommentViewModel(_Comment));
                else
                    return ValidationErrorResponse(flagBusiness.ValidationErrors);
            }
        }

        public ActionResult FlagComments(int id /* Flag Id*/)
        {
            using (OrderFlagBusiness flagBusiness = new OrderFlagBusiness(_applicationServicesSetup, _user))
            {
                return Json(
                    flagBusiness.GetCommentsForFlag(id).Select(i => new CommentViewModel(i)),
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        public ActionResult CalculateMileage(string[] zips)
        {

            var Mileage = _loadBusiness.CalculateMileage(zips);

            if (Mileage != null)
                return SuccessMessage(Mileage);
            else
                return (_loadBusiness.ValidationErrors.Any())
                ? ErrorMessage(_loadBusiness.ValidationErrors)
                : ErrorMessage("Could Not calculate Mileage.  Third party service returned an error");

        }

        public ActionResult EditHistory(int id)
        {
            var Load = _loadBusiness.Get(id, new string[] { 
                    Globals.Related.Load.Histories, 
                    Globals.Related.Load.HistoriesUser 
                });

            return Json(Load.LoadHistories.Select(i => new
            {
                Action = i.Action,
                TimeStamp = i.TimeStamp.ToShortDateString() + " " + i.TimeStamp.ToLongTimeString(),
                Item = i.Description,
                OldValue = i.OldValue,
                NewValue = i.NewValue,
                EditedBy = i.AspNetUser.Name
            }), JsonRequestBehavior.AllowGet);

        }


        public ActionResult SearchTrackingNumber(string query)
        {
            return Json(_loadBusiness.FindByTrackingNumber(query), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferenceNumberSearch(string term)
        {
            return Json(
                _loadBusiness.GetRepository().Select<Order>(i => i.ReferenceNumber.StartsWith(term)).ToList(),
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult GetEquipmentOptions(string mode)
        {
            return Json(_loadBusiness.GetEquipmentOptions(mode), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadboardLoadDetails(int id)
        {
            var Load = _loadBusiness.Get(id, new[] { 
                Globals.Related.Load.Stops, 
                Globals.Related.Load.Carrier,
                Globals.Related.Load.CarrierContacts,
                Globals.Related.Load.CarrierContact,
                Globals.Related.Load.Customer,
                Globals.Related.Load.CustomerContacts
                //Globals.Related.Load.Comments,
                //Globals.Related.Load.CommentUser,

            });

            foreach (var stop in Load.Stops)
            {
                stop.Load = null;
            }

            var ret = new
            {
                Carrier = new
                {
                    Id = Load.CarrierId,
                    Name = Load.Carrier.Name,
                    MC = Load.Carrier.MC,
                    DOT = Load.Carrier.DOT,
                    Driver = Load.Driver,
                    DriverPhone = Load.DriverPhone,
                    Contacts = Load.Carrier.CarrierContacts
                                .Where(i => i.Status == Globals.ContactStatus.Active)
                                .Select(i => new Contact(i)).FirstOrDefault(),
                    MainContact = new Contact(Load.CarrierContact),
                },
                Customer = new
                {
                    Name = Load.Customer.Name,
                    Id = Load.CustomerId,
                    Contacts = Load.Customer.CustomerContacts
                                .Where(i => i.Type == Globals.CustomerContactType.Freight && i.Status == Globals.ContactStatus.Active)
                                .Select(i => new Contact(i))
                },
                Stops = Load.Stops.OrderBy(i => i.StopDateTime),
                //Comments = Load.LoadComments.Select(comment=> new CheckCallViewModel(comment))
            };

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarkStopCompleted(int id, List<Stop> Stops)
        {
            var Load = _loadBusiness.Get(id, new[] { Globals.Related.Load.Stops });

            foreach (var Stop in Load.Stops)
            {
                var theStop = Stops.FirstOrDefault(stop => stop.Id == Stop.Id);
                Stop.DriverInTime = theStop.DriverInTime;
                Stop.DriverOutTime = theStop.DriverOutTime;
                Stop.Arrived = theStop.Arrived;
                Stop.ModifiedDate = DateTime.UtcNow;
            }

            _loadBusiness.Update(Load);

            return SuccessMessage();
        }

        #region MacroPoint Actions

        public ActionResult MacroPointBeginTracking(int LoadId, string DriverPhone, string TimeZone, DateTime TrackingStartDate, List<MacroPointStopViewModel> ArrivalTimes)
        {

            var Arrivals = ArrivalTimes.Select(stop => new MacroPointStop
            {
                Id = (int)stop.Id,
                ETA = stop.ETA,
                EDT = stop.EDT
            });

            bool isSuccess = _loadBusiness.MacroPointBeginTracking(LoadId, DriverPhone, TrackingStartDate, TimeZone, Arrivals); ;
            if (isSuccess)
                return SuccessMessage();
            else
                return ErrorMessage("MacroPoint service experienced an error");
        }

        //Used to Ping MacroPoint service for an immediate LocationUpdate.
        public ActionResult UpdateLocation(int loadId)
        {
            Load load = _loadBusiness.Get(loadId);
            string message = null;
            if (load != null && load.MacroPointOrderId != null)
            {
                message = _loadBusiness.UpdateLocation(load.MacroPointOrderId);
            }
            if (message == "SUCCESS") return SuccessMessage(null, "Location Update Request - Received. Please allow 5 minutes to see your update.");
            return ErrorMessage(message);
        }


        //*** NOTE: This URL is setup on MacroPoint's website (www.macropoint-lite.com) on the Company Settings tab. Set it to point to the production site (currently: www.atgfr8.com; the full URL will be www.atgfr8.com/Load/LocationUpdate)
        [AllowAnonymous]
        public HttpStatusCode LocationUpdate(string ID, decimal Latitude, decimal Longitude, decimal Uncertainty, string Street1,
            string Street2, string Neighborhood, string City, string State, string Postal, string Country, DateTime LocationDateTimeUTC, DateTime ApproxLocationDateTimeInLocalTime, string MPOrderID)
        {
            try
            {
                var logText = string.Format("MacroPoint LocationUpdate -- ID: {0}, City: {1}, State: {2}, ApproxLocationDateTimeInLocalTime: {3}, MPOrderId: {4}", ID, City, State, ApproxLocationDateTimeInLocalTime, MPOrderID);
                _applicationServices.LogWriteFull(logText, LoggingSeverity.Information, LoggingCategory.Controller);

                Load load = _loadBusiness.GetLoadByMacroPointId(MPOrderID);
                if (load != null)
                {
                    LoadComment comment = new LoadComment()
                    {
                        LoadId = load.Id,
                        UserId = _user.Id,
                        Comment = "MacroPoint Location Update",
                        City = City,
                        State = State,
                        Latitude = Latitude,
                        Longitude = Longitude,
                        CreatedDate = ApproxLocationDateTimeInLocalTime,
                        LocationTime = ApproxLocationDateTimeInLocalTime
                    };
                    _loadBusiness.AddComment(load.Id, comment);
                }
            }
            catch (Exception e)
            {
                _applicationServices.HandleException(e, ExceptionCategory.Controller);
                return HttpStatusCode.InternalServerError;
            }
            return HttpStatusCode.OK;
        }

        [AllowAnonymous]
        public HttpStatusCode ScheduleAlert(string MPOrderID, string ID, string StopType, string StopID, string StopName, string AddressLine1, string City, string State, string PostalCode, int ScheduleAlertCode,
            string ScheduledStartTimeInLocalTimeForStop, string ScheduledEndTimeInLocalTimeForStop, string ScheduleAlertText)
        {
            try
            {
                var logText = string.Format("MacroPoint Schedule Alert -- ID: {0}, ScheduleCode: {1}, ScheduleAlertText: {2}", ID, MacroPointService.ScheduleAlertMessage(ScheduleAlertCode), ScheduleAlertText);
                _applicationServices.LogWriteFull(logText, LoggingSeverity.Information, LoggingCategory.Controller);

                Load load = _loadBusiness.GetLoadByMacroPointId(MPOrderID);
                LoadComment comment = new LoadComment()
                {
                    LoadId = load.Id,
                    UserId = _user.Id,
                    Comment = "MacroPoint Schedule Alert --  " + MacroPointService.ScheduleAlertMessage(ScheduleAlertCode) + " | " + ScheduleAlertText,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    LocationTime = DateTime.Now.ToUniversalTime()
                };
                _loadBusiness.AddComment(load.Id, comment);
            }
            catch (Exception e)
            {
                _applicationServices.HandleException(e, ExceptionCategory.Controller);
                return HttpStatusCode.BadRequest;
            }
            return HttpStatusCode.OK;
        }

        [AllowAnonymous]
        public HttpStatusCode TrackingStatusChange(string ID, string Code, string Message, string MPOrderID)
        {
            try
            {
                var logText = string.Format("MacroPoint TrackingStatusChange -- ID: {0}, Code: {1}, Message: {2}, MPOrderID: {3}", ID, Code, Message, MPOrderID);
                _applicationServices.LogWriteFull(logText, LoggingSeverity.Information, LoggingCategory.Controller);

                Load load = _loadBusiness.GetLoadByMacroPointId(MPOrderID);
                if (load != null)
                {
                    LoadComment comment = new LoadComment()
                    {
                        LoadId = load.Id,
                        UserId = _user.Id,
                        Comment = "MacroPoint Tracking Status Change --  " + MacroPointService.TrackingStatus(Code),
                        CreatedDate = DateTime.Now.ToUniversalTime(),
                        LocationTime = DateTime.Now.ToUniversalTime()
                    };
                    _loadBusiness.AddComment(load.Id, comment);

                    string userMessage = string.Format("Tracking Status Change (Order #: {0}) - {1}", load.OrderId, MacroPointService.TrackingStatus(Code));
                    List<int> usersToMessage = new List<int> { load.Order.AssignedId };
                    if (load.AssignedTo != null) usersToMessage.Add(load.AssignedTo.Value);
                    if (load.CoveredBy != null) usersToMessage.Add(load.CoveredBy.Value);

                    _loadBusiness.NotifyUsers(userMessage, usersToMessage, "Order:" + load.OrderId);
                }

            }
            catch (Exception e)
            {
                return HttpStatusCode.BadRequest;
            }
            return HttpStatusCode.OK;
        }


        #endregion
        #endregion

        public void Dispose()
        {
            _loadBusiness.Dispose();
            base.Dispose();
        }
    }
}