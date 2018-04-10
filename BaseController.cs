using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATGV3.Business.Model;
using ATGV3.Common;
using ATGV3.Web.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ATGV3.Business;
using System.Security.Principal;
using Microsoft.Owin.Security;

namespace ATGV3.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected ApplicationServices _applicationServices = ApplicationHelper.Instance.GetApplicationServices();
        protected ApplicationServicesSetup _applicationServicesSetup = ApplicationHelper.Instance.GetApplicationServicesSetup();
        protected AspNetUser _user;

        protected const int VALIDATION_ERROR_RESPONSE_CODE = 409;
        protected const int ERROR_RESPONSE_CODE = 500;
        protected ServerEventsHelper EventDispatcher;

        public BaseController()
        {
            _SetAppUser();
            EventDispatcher = new ServerEventsHelper();
            ViewBag.User = _user;
        }

        private void _SetAppUser()
        {
            var loggedInUserName = System.Web.HttpContext.Current.User.Identity.GetUserName();

            if (!string.IsNullOrEmpty(loggedInUserName))
            {
                //_user = ConvertObjectFromCache<AspNetUser>(_applicationServices.CacheService().Get(loggedInUserName));

                //get user from database
                if (_user == null)
                {
                    using (UserBusiness ub = new UserBusiness(_applicationServicesSetup))
                    {
                        _user = ub.GetUser(loggedInUserName, new[]{
                        Globals.Related.User.AspNetRoles,
                        Globals.Related.User.Groups,
                        Globals.Related.User.Settings,
                        Globals.Related.User.SavedSearches,
                        Globals.Related.User.Permissions
                    }, false);

                        //todo: put cache time into database
                        //_applicationServices.CacheService().Set(loggedInUserName, ConvertObjectToCache(_user), new DateTimeOffset(DateTime.Now.AddMinutes(600)));
                    }

                    if (!_user.Active)
                    {
                        RedirectToAction("Login", new { Code = "Locked" });
                    }
                }
            }

        }

        public static T ConvertObjectFromCache<T>(Object cacheObject)
        {
            if (cacheObject == null)
            {
                return default(T);
            }
            if (String.IsNullOrEmpty(cacheObject.ToString()))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(cacheObject.ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }


        public static string ConvertObjectToCache(object cacheObject)
        {
            return JsonConvert.SerializeObject(cacheObject, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        protected override void Dispose(bool disposing)
        {
            _applicationServices = null;
            _applicationServicesSetup = null;
            _user = null;
            base.Dispose(disposing);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Trace.Write("(Controller)Action Executing: " +
                filterContext.ActionDescriptor.ActionName);

            base.OnActionExecuting(filterContext);
        }


        protected string ConvertToJson(List<string> list)
        {
            return JsonConvert.SerializeObject(list);
        }

        protected ActionResult ValidationErrorResponse(string message)
        {

            HttpContext.Response.StatusCode = VALIDATION_ERROR_RESPONSE_CODE;
            return Json(new List<string>() { message }, JsonRequestBehavior.AllowGet);
        }
 
        protected ActionResult ValidationErrorResponse(IEnumerable<string> messages)
        {
            HttpContext.Response.StatusCode = VALIDATION_ERROR_RESPONSE_CODE;
            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult SuccessMessage(dynamic data = null, string message = null)
        {
            return Json(new { 
                Success = true, 
                data = data, 
                Message = message 
            }, JsonRequestBehavior.AllowGet);
        }

        [PreventDirectAccess]
        protected ActionResult ErrorMessage(string message)
        {
            return Json(new { 
                Success = false, 
                Message = message 
            }, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult ErrorMessage(IEnumerable<string> messages)
        {
            return Json(new
            {
                Success = false,
                Message = ConvertToJson(messages.ToList())
            }, JsonRequestBehavior.AllowGet);
        }

        [PreventDirectAccess]
        public ActionResult ServerError()
        {
            HttpContext.Response.StatusCode = 500;

            if(Request.IsAjaxRequest())
                return ErrorMessage("An error occured processing your last request");
            else
                return Redirect("~/500.html");
        }

        [PreventDirectAccess]
        public ActionResult AccessDenied()
        {
            HttpContext.Response.StatusCode = 403;

            if(Request.IsAjaxRequest())
                return ErrorMessage("Access to this Page is Denied");
            else
                return Redirect("~/403.html");
        }

        [PreventDirectAccess]
        public ActionResult NotFound()
        {
            HttpContext.Response.StatusCode = 404;

            if (Request.IsAjaxRequest())
            {
                return ErrorMessage("Page Not Found");
            }else
                return Redirect("~/404.html");
        }

        [PreventDirectAccess]
        public ActionResult PDF(string url = null, List<string> otherUrls = null)
        {
            if (url != null)
                url = "http://" + Request.Url.Authority + url;

            if (otherUrls == null)
                otherUrls = new List<string>();

            return File(
                _applicationServices.CreatePDF( url, otherUrls.ToArray()),
                "application/pdf"
            );
        }

        [PreventDirectAccess]
        public ActionResult PDF(byte[] pdf)
        {
            return File(pdf, "application/pdf");
        }

        protected AspNetUser GetSystemUser(){
            return ApplicationHelper.Instance.GetSystemUser();
        }
    }
}