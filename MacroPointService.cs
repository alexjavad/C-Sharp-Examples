using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using ATGV3.Business.Model;
using ATGV3.Business.ValueObjects;
using ATGV3.Common;

namespace ATGV3.Business.Services
{

    /* 
     *******************************************************************************************
     *       Things to Know:
     *       
     *    The LoadController houses the appropriate controller actions that will MUST be setup on MacroPoint's website (www.macropoint-lite.com) in order for you to receive data back from the MacroPoint API.
     *    Put simply, you MUST register the appropriate URLs on macropoint's site, or you will not receive data back. Make sure that the site that is registered on the Company Settings tab (on their site) is set to
     *    our production website (www.atgfr8.com).
     * 
     * 
     * 
     * 
     ******************************************************************************************* 
    */


    public class MacroPointService
    {
        private string MacroPointID { get; set; }
        private string MacroPointPassword { get; set; }
        private ApplicationServicesSetup _appServicesSetup { get; set; }
        private ApplicationServices _appServices { get; set; }
        private XNamespace macropointXmlNs = @"http://macropoint-lite.com/xml/1.0";

        public MacroPointService(ApplicationServicesSetup appServicesSetup)
        {
            _appServicesSetup = appServicesSetup;
            _appServices = new ApplicationServices(_appServicesSetup);
            MacroPointID = _appServices.ApplicationSettings["MacroPointID"];
            MacroPointPassword = _appServices.ApplicationSettings["MacroPointPassword"];

        }

        /// <summary>
        /// returns a unique string identifier that represents the MacroPoint tracking session for the provided Load, null otherwise
        /// </summary>
        /// <param name="load"></param>
        /// <param name="trackingStartDateTime"></param>
        /// <param name="timeZone"></param>
        /// <param name="stops"></param>
        /// <returns></returns>
        public string TrackOrder(Load load, DateTime trackingStartDateTime, string timeZone, IEnumerable<MacroPointStop> stops)
        {
            string requestXml = buildRequestXmlFromLoad(load, trackingStartDateTime, timeZone, stops);
            XDocument response = sendRequestToBeginTracking(requestXml);
            if (response == null) return null;
            XElement orderId = response.Descendants("OrderID").FirstOrDefault();
            return orderId != null ? orderId.Value : null;
        }
        
        /// <summary>
        /// Returns 'SUCCESS' if a request for a location update was successfully issued, otherwise - returns an error message.
        /// </summary>
        /// <param name="MPOrderID"></param>
        /// <returns></returns>
        public string UpdateLocation(string MPOrderID)
        {
            string destinationUrl = "https://macropoint-lite.com/api/1.0/orders/updatelocation/" + MPOrderID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            request.Credentials = new NetworkCredential(MacroPointID, MacroPointPassword);
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Close();
            HttpWebResponse response = null;
            try { response = (HttpWebResponse)request.GetResponse(); }
            catch (WebException e)
            {
                string errorMessage = "";
                using (WebResponse resp = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)resp;
                    using (Stream data = resp.GetResponseStream())
                    {
                        XDocument responseXmlDoc = XDocument.Load(new XmlTextReader(data));
                        XElement errors = responseXmlDoc.Descendants("Errors").FirstOrDefault();
                        string errorCode = errors.Descendants("Code").FirstOrDefault().Value;
                        switch (errorCode)
                        {
                            case "1001":
                                errorMessage = "Insufficient Funds";
                                break;
                            case "2000":
                                errorMessage = "Order ID not valid";
                                break;
                            case "2001":
                                errorMessage = "Order is completed or expired";
                                break;
                            case "3000":
                                errorMessage = "Feature not provisioned - Contact MacroPoint Account Representative at (216) 369-0144";
                                break;
                            case "5000":
                                errorMessage = "Order not eligible for location update right now. Try again later.";
                                break;
                            case "9999":
                                errorMessage = "Unexpected error occurred - Administrator notified";
                                break;
                        }
                    }
                }
                return errorMessage;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            if (response != null && response.StatusCode == HttpStatusCode.OK) return "SUCCESS";
            return null;
        }

        private string buildRequestXmlFromLoad(Load load, DateTime trackingStartDateTime, string timeZone, IEnumerable<MacroPointStop> stops)
        {
            //Establish Variables
            string trackingNumberType = "Mobile";
            string trackingNumber = load.DriverPhone;
            string trackStartDateTime = trackingStartDateTime.AddHours(-4).ToString("yyyy-MM-dd H:mm") + " " + timeZone;
            
            //Notifications
            string partnerMacropointID = MacroPointID;
            string loadIdentifier = load.OrderId + "-" + load.OrderLoadSeq;
            int duration = computeTrackingDuration(load, trackingStartDateTime);
            string trackingDurationInHours = duration.ToString();
            string trackingIntervalInMinutes = "240";

            //Trip Sheet
            XElement tripSheetNode = buildTripSheetNode(load, stops);

            XDocument xml = new XDocument(
                new XElement(macropointXmlNs + "Order",
                    new XElement(macropointXmlNs + "Number",
                        new XAttribute("Type", trackingNumberType),
                        new XText(trackingNumber)
                     ),
                     new XElement(macropointXmlNs + "TrackStartDateTime",
                        new XText(trackStartDateTime)
                     ),
                    new XElement(macropointXmlNs + "Notifications",
                        new XElement(macropointXmlNs + "Notification",
                            new XElement(macropointXmlNs + "PartnerMPID",
                                new XText(partnerMacropointID)
                            ),
                            new XElement(macropointXmlNs + "IDNumber",
                                new XText(loadIdentifier)
                            ),
                            new XElement(macropointXmlNs + "TrackDurationInHours",
                                new XText(trackingDurationInHours)
                            ),
                            new XElement(macropointXmlNs + "TrackIntervalInMinutes",
                                new XText(trackingIntervalInMinutes)
                            )
                         )
                     ),
                    tripSheetNode
                 )
             );

            return xml.ToString();
        }

        /// <summary>
        /// Returns the tracking duration, in hours, as the difference between the 'begin tracking time' and the destination's stop date time (by default set to the end of the day)
        /// </summary>
        /// <param name="load"></param>
        /// <param name="trackingStartDateTime"></param>
        /// <returns></returns>
        private int computeTrackingDuration(Load load, DateTime trackingStartDateTime)
        {
            DateTime destStopDateTime = load.Stops.FirstOrDefault(i => i.Destination == true).StopDateTime.Date.AddDays(1);

            TimeSpan diff = destStopDateTime.Subtract(trackingStartDateTime);
            int diffInHours = diff.Days * 24 + diff.Hours;
            return AddHoursUntilDivisibleBy4(diffInHours);
        }

        private int AddHoursUntilDivisibleBy4(int hours)
        {
            if (hours % 4 == 0) return hours + 1;
            else
            {
                while (hours % 4 != 0)
                {
                    hours++;
                }
                return hours;
            }
        }

        private XElement buildTripSheetNode(Load load, IEnumerable<MacroPointStop> stops)
        {
            List<StopXmlData> stopXmlData = new List<StopXmlData>();

            XElement node = new XElement(macropointXmlNs + "TripSheet",
                                new XElement(macropointXmlNs + "Stops")
                            );

            foreach (Stop stop in load.Stops)
            {
                string stopID = stop.Id.ToString();
                string stopName = load.Customer.Name + " - " + stop.Address1 + ", " + stop.City + ", " + stop.State;
                string stopType = stop.PickUp ? "Pickup" : (stop.DropOff ? "DropOff" : "");
                string addressLine1 = stop.Address1;
                string addressLine2 = stop.Address2 != null ? stop.Address2 : "";
                string city = stop.City;
                string state = stop.State;
                string zipCode = stop.Zip;

                StopXmlData d = new StopXmlData()
                {
                    StopID = stopID,
                    StopName = stopName,
                    StopType = stopType,
                    AddressLine1 = addressLine1,
                    AddressLine2 = addressLine2,
                    City = city,
                    State = state,
                    Zip = zipCode != null ? zipCode : ""
                };

                foreach (var s in stops)
                {
                    if (s.Id == stop.Id)
                    {
                        string stopDate = stop.StopDateTime.ToShortDateString();
                        DateTime stopETA = DateTime.Parse(stopDate + " " + s.ETA);
                        DateTime stopEDT = DateTime.Parse(stopDate + " " + s.EDT);

                        d.ETA = stopETA;
                        d.EDT = stopEDT;
                    }
                }
                stopXmlData.Add(d);
            }

            var stopsOrdered = from stop in stopXmlData
                               orderby stop.ETA ascending
                               select stop;

            foreach (var stop in stopsOrdered)
            {
                node.Descendants().First().Add(new XElement(macropointXmlNs + "Stop",
                    new XElement(macropointXmlNs + "StopID", new XText(stop.StopID)),
                    new XElement(macropointXmlNs + "Name", new XText(stop.StopName)),
                    new XElement(macropointXmlNs + "StopType", new XText(stop.StopType)),
                    new XElement(macropointXmlNs + "Address",
                        new XElement(macropointXmlNs + "Line1", new XText(stop.AddressLine1)),
                        new XElement(macropointXmlNs + "Line2", new XText(stop.AddressLine2)),
                        new XElement(macropointXmlNs + "City", new XText(stop.City)),
                        new XElement(macropointXmlNs + "StateOrProvince", new XText(stop.State)),
                        new XElement(macropointXmlNs + "PostalCode", new XText(stop.Zip))
                      ),
                    new XElement(macropointXmlNs + "StartDateTime", new XText(stop.ETA.ToString("yyyy-MM-ddTHH:mmZ"))),
                    new XElement(macropointXmlNs + "EndDateTime", new XText(stop.EDT.ToString("yyyy-MM-ddTHH:mmZ")))
                ));
            }

            return node;
        }

        private XDocument sendRequestToBeginTracking(string requestXml)
        {
            string destinationUrl = "https://macropoint-lite.com/api/1.0/orders/createorder";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            request.Credentials = new NetworkCredential(MacroPointID, MacroPointPassword);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            try { response = (HttpWebResponse)request.GetResponse(); }
            catch (Exception e)
            {
                return null;
            }

            Stream responseStream = response.GetResponseStream();
            XmlReader reader = new XmlTextReader(responseStream);

            XDocument responseXmlDoc = XDocument.Load(reader);

            //Close XMLReader
            try { reader.Close(); }
            catch { }
            try { responseStream.Close(); }
            catch { }
            try { response.Close(); }
            catch { }
            try { requestStream.Close(); }
            catch { }

            return responseXmlDoc;

        }

        /// <summary>
        /// Takes in the code returned by MacroPoint's Order Status Change API and returns the associated message.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string TrackingStatus(string code)
        {
            string message = "";
            switch (code)
            {
                case "1001":
                    message = "Requesting Installation";
                    break;
                case "1020":
                    message = "Driver Accepted. Ready to Track.";
                    break;
                case "1021":
                    message = "Tracking Now";
                    break;
                case "1022":
                    message = "Tracking Completed Successfully";
                    break;
                case "1050":
                    message = "Stopped By Creator";
                    break;
                case "1051":
                    message = "Stopped Early By Driver";
                    break;
                case "1052":
                    message = "Denied By Driver";
                    break;
                case "1053":
                    message = "Location Hidden By Driver";
                    break;
                case "1060":
                    message = "Tracking - Waiting For Update";
                    break;
                case "1061":
                    message = "Incompatible Phone";
                    break;
                case "1062":
                    message = "Invalid Number - Not Trackable";
                    break;
                case "1063":
                    message = "Landline Not Trackable";
                    break;
                case "1064":
                    message = "Driver Refused Installation";
                    break;
                case "1070":
                    message = "Expired Without Installation";
                    break;
                case "1071":
                    message = "Expired Without Location";
                    break;
                case "1110":
                    message = "Trip Sheet Deployed";
                    break;
                case "1111":
                    message = "Trip Sheet In Progress";
                    break;
                case "1112":
                    message = "Trip Sheet Completed";
                    break;
                case "1113":
                    message = "Trip Sheet Expired";
                    break;
            }
            return message;
        }

        public static string ScheduleAlertMessage(int code)
        {
            string message = "";
            switch (code)
            {
                case 0:
                    message = "Cannot Determine";
                    break;
                case 1:
                    message = "On Time/Ahead of Schedule";
                    break;
                case 2:
                    message = "Behind schedule, but can still arrive on time.";
                    break;
                case 3:
                    message = "Behind schedule. Will most likely NOT make it on time.";
                    break;
                case 4:
                    message = "Past Appointment Time";
                    break;
            }
            return message;
        }

        //Rounds to the nearest provided minutes. Ex: RoundUp(DateTime.Now, TimeSpan.FromMinutes(15))
        //public DateTime RoundToNearest(DateTime dt, TimeSpan d)
        //{
        //    var delta = dt.Ticks % d.Ticks;
        //    bool roundUp = delta > d.Ticks / 2;
        //    var offset = roundUp ? d.Ticks : 0;

        //    return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        //}
    }

    internal class StopXmlData
    {
        public string StopID { get; set; }
        public string StopName { get; set; }
        public string StopType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public DateTime ETA { get; set; } //estimated time of arrival
        public DateTime EDT { get; set; } //estimated departure time
    }
}
