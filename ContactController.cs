using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Configuration;
//using NLog;
using System.Text;
using BusinessContactWebsiteTemplate.Repositories;
using System.Threading.Tasks;
using BusinessContactWebsiteTemplate.ViewModels;
using AutoMapper;

namespace BusinessContactWebsiteTemplate.Controllers
{
    public class ContactController : ApiController
    {

        //public static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private ProspectRepository _prospectRepo = new ProspectRepository(new DAL.letsgrow_databaseEntities());

        [Route("api/postcontact")]
        [HttpPost]
        public async Task<ResponseData> PostContactAsync(ProspectInfo prospect)
        {
            var response = SendEmail(prospect);
            var storeResponse = await StoreProspectInfo(prospect);
            return response;
        }

        private async Task<ResponseData> StoreProspectInfo(ProspectInfo prospect)
        {
            try
            {
                DAL.Prospect newProspect = Mapper.Map<DAL.Prospect>(prospect);
                _prospectRepo.Insert(newProspect);
                await _prospectRepo.Commit();
                return new ResponseData()
                {
                    success = true,
                    message = "Message successfully sent!"
                };
            }
            catch (Exception e)
            {
                string errorMessage = string.Format("Error in method: {0} | Exception type: {1} | Message: {2} | StackTrace: {3}", System.Reflection.MethodBase.GetCurrentMethod(),
                                        e.GetType(), e.Message, e.StackTrace);

                return new ResponseData()
                {
                    message = errorMessage,
                    success = false
                };
            }
        }

        private ResponseData SendEmail(ProspectInfo prospect)
        {
            try
            {
                SmtpClient mailer = new SmtpClient();
                mailer.Host = "mail." + Request.RequestUri.Host.Substring(0,3) == "www" ? Request.RequestUri.Host.Substring(4) : Request.RequestUri.Host;
                mailer.UseDefaultCredentials = false;
				//Removed NetworkCredential for security purposes
                MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["EmailContactOrigin"], ConfigurationManager.AppSettings["EmailContactReceiver"]);

                mail.Priority = MailPriority.High;
                mail.Subject = "New Prospect: " + prospect.Service;
                mail.IsBodyHtml = true;

                StringBuilder sb = new StringBuilder();
                sb.Append("<p><strong>Customer Name: </strong>" + prospect.Name + "</p>");
                sb.Append("<p><strong>Service Needed: </strong>" + prospect.Service + "</p>");
                sb.Append("<p><strong>Message: </strong></p>");
                sb.AppendLine("<pre style='font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: black;'>" + prospect.Message + "</pre> <br />");
                sb.Append("<span style='font-size:14px;'>Respond to customer: <br/>Email - " + prospect.Email + " <br/> Phone - <a href='tel:+1" + prospect.Phone + "'>" + prospect.Phone + "</a></span>");

                mail.Body = sb.ToString();
                mailer.Send(mail);

                return new ResponseData()
                {
                    success = true,
                    message = "Message successfully sent!"
                };
            }
            catch (Exception e)
            {

                string errorMessage = string.Format("Error in method: {0} | Exception type: {1} | Message: {2} | StackTrace: {3}", System.Reflection.MethodBase.GetCurrentMethod(), 
                                                        e.GetType(), e.Message, e.StackTrace);
                //Log.Error(errorMessage);
                return new ResponseData()
                {
                    success = false,
                    message = errorMessage
                };
            }
        }

        public class ResponseData
        {
            public string message { get; set; }
            public bool success { get; set; }
        }


    }
}
