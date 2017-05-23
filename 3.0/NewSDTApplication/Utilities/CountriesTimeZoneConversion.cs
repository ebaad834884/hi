using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDTLogger;
using System.Net;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Configuration;

namespace NewSDTApplication.Utilities
{
    public class CountriesTimeZoneConversion
    {
        private DateTime ConvertedTime;
        private DateTime DesiredDate;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        GoogleAPIKeyEncoder objGoogleAPIKeyEncoder = new GoogleAPIKeyEncoder();
        //Addition of IP address in logs - US185 - 18/4/2017 by Ebaad (This line will provide us with Client IP Address)
        string IP = "IP: " + (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();

        /// <summary>
        /// Converts the time as per the given Country's time zone by adding specified hours. 
        /// </summary>
        /// <param name="CountryName"></param>
        /// <param name="PostCode"></param>
        /// <returns></returns>
        public DateTime GetTimeZone(string CountryName, string PostCode)
        {
            string AddressToGoogle = string.Empty;
            DateTime ConvertedTime = DateTime.MinValue;

            try
            {

                string[] countryList = ConfigurationManager.AppSettings["TimezoneCountry"].Split(';');
                if (!string.IsNullOrEmpty(PostCode))
                {
                    if (CountryName == countryList[0].ToString())
                    {
                        ConvertedTime = DateTime.UtcNow.AddHours(9);
                    }
                    else if (CountryName == countryList[1].ToString())
                    {
                        string[] adress = PostCode.Split(',');
                        AddressToGoogle = String.Join(",", adress, 2, adress.Length - 2);
                        ConvertedTime = GoogleAddressCall(AddressToGoogle);
                    }
                    else if (CountryName == countryList[2].ToString())
                    {
                        ConvertedTime = DateTime.UtcNow.AddHours(10);
                    }
                    else if (CountryName == countryList[3].ToString())
                    {
                        ConvertedTime = DateTime.UtcNow.AddHours(8);
                    }
                    //else if (CountryName == "Thailand" || CountryName == "Indonesia")
                    //{
                    //    ConvertedTime = DateTime.UtcNow.AddHours(7);
                    //}

                    else
                    {
                        string[] adress = PostCode.Split(',');
                        if (adress.Length > 4)
                        {
                            AddressToGoogle = String.Join(",", adress, 2, adress.Length - 2);

                        }
                        else
                        {
                            AddressToGoogle = String.Join(",", adress, 1, adress.Length - 1);
                        }

                        ConvertedTime = GoogleAddressCall(AddressToGoogle);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("In CountriesTimeZoneConversion GetTimeZone Method | Exception:" + ex.Message + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                //throw;
            }

            return ConvertedTime;
        }

        /// <summary>
        /// Returns a new DateTime by adding the specified number of hours in the Desired date as per the country name.
        /// </summary>
        /// <param name="GehcDesiredDate"></param>
        /// <param name="CountryName"></param>
        /// <param name="PostCode"></param>
        /// <returns></returns>
        public DateTime ConvertDesiredDate(DateTime GehcDesiredDate, string CountryName, string PostCode)
        {
            string AddressToGoogle = string.Empty;
            DateTime DesiredDate = DateTime.MinValue;
            //DateTime ConvertedTime = DateTime.MinValue;
            string[] countryList = ConfigurationManager.AppSettings["TimezoneCountry"].Split(';');

            try
            {
                if (!string.IsNullOrEmpty(PostCode))
                {
                    if (CountryName == countryList[0].ToString())
                    {
                        DesiredDate = GehcDesiredDate.AddHours(9);
                    }


                    else if (CountryName == countryList[1].ToString())
                    {
                        string[] adress = PostCode.Split(',');
                        AddressToGoogle = String.Join(",", adress, 2, adress.Length - 2);
                        DesiredDate = GoogleAddressCallForDesiredDate(AddressToGoogle, GehcDesiredDate);
                    }
                    else if (CountryName == countryList[2].ToString())
                    {

                        DesiredDate = GehcDesiredDate.AddHours(10);

                    }
                    else if (CountryName == countryList[3].ToString())
                    {

                        DesiredDate = GehcDesiredDate.AddHours(8);

                    }
                    else
                    {
                        string[] adress = PostCode.Split(',');
                        if (adress.Length > 4)
                        {
                            AddressToGoogle = String.Join(",", adress, 2, adress.Length - 2);
                        }
                        else
                        {
                            AddressToGoogle = String.Join(",", adress, 1, adress.Length - 1);
                        }

                        DesiredDate = GoogleAddressCallForDesiredDate(AddressToGoogle, GehcDesiredDate);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("In CountriesTimeZoneConversion GetTimeZone Method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
                //throw;
            }

            return DesiredDate;

        }

        /// <summary>
        /// Returns a timestamp by subtracting specified date time from the current date and time.
        /// </summary>
        /// <returns></returns>
        private string GenerateTimeStamp()
        {
            string str = String.Empty;

            try
            {
                str = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
            }
            catch (Exception ex)
            {
                logger.Error("In CountriesTimeZoneConversion GenerateTimeStamp Method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            }
            return str;
        }

        /// <summary>
        /// Captures the address using Google Places API.
        /// </summary>
        /// <param name="AddressToGoogle"></param>
        /// <returns></returns>
        public DateTime GoogleAddressCall(string AddressToGoogle)
        {
            try
            {
                //Code Start - Ebaad Chowdhry - 23/1/2017 - Scope: CR/Incident/Defect - Implemented address capturing via the Google Places API
                var keyString = ConfigurationManager.AppSettings["GooglePlacesKey"].ToString();
                var requestUri = string.Format(ConfigurationManager.AppSettings["GoogleAddressAPI"].ToString() + "query={0}&key={1}", Uri.EscapeDataString(AddressToGoogle), keyString);
                var req = HttpWebRequest.Create(requestUri);
                req.Method = "GET";
                req.ContentType = "application/xml";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GoogleProxyURL2"]))
                {
                    WebProxy proxyurl = new WebProxy(ConfigurationManager.AppSettings["GoogleProxyURL2"]);
                    req.Proxy = proxyurl;
                }
               
                DateTime startTimeTx1 = DateTime.Now;
                var response = req.GetResponse();
                DateTime endTimeTx1 = DateTime.Now;
                TimeSpan Tx1 = endTimeTx1.Subtract(startTimeTx1);
               
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = (from xml in xdoc.Descendants("result") select xml).FirstOrDefault();
                //Code End - Ebaad Chowdhry - 23/1/2017 - Scope: CR/Incident/Defect - Implemented address capturing via the Google Places API
                if (result != null)
                {


                    var locationElement = result.Element("geometry").Element("location");
                    var lat = locationElement.Element("lat");
                    var lng = locationElement.Element("lng");
                    logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method | To retrieve LatLng Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) ~ " + IP + " ~ Request : Address: " + AddressToGoogle + " | URL: " + requestUri + " ~ Response: Lat: " + lat + " Lng: " + lng + " ~ Call duration: " + Tx1.TotalMilliseconds); 
                    string timestamp = GenerateTimeStamp();
                    //string url = "https://maps.googleapis.com/maps/api/timezone/xml?location=39.6034810,-119.6822510&timestamp=1331161200";
                    //string url = "https://maps.googleapis.com/maps/api/timezone/xml?location=" + lat.Value + "," + lng.Value + "&timestamp=" + timestamp;
                    string url = ConfigurationManager.AppSettings["GoogleLatLongAPI"] + lat.Value + "," + lng.Value + "&timestamp=" + timestamp;

                    //google encoded key Added by phanikanth p
                    var requesturltimestamp = objGoogleAPIKeyEncoder.GoogleKey(url);
                    HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create(requesturltimestamp);
                    req1.Method = "GET";
                    req1.ContentType = "application/xml";
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GoogleProxyURL2"]))
                    {
                        WebProxy proxyurl = new WebProxy(ConfigurationManager.AppSettings["GoogleProxyURL2"]);
                        req1.Proxy = proxyurl;
                    }
                  
                   
                    DateTime startTimeTx2 = DateTime.Now;
                    var response1 = req1.GetResponse();
                    DateTime endTimeTx2 = DateTime.Now;
                    TimeSpan Tx2 = endTimeTx2.Subtract(startTimeTx2);
                   
                    if (response1 != null)
                    {
                        var xdoc1 = XDocument.Load(response1.GetResponseStream());
                        var RawOffset = xdoc1.Element("TimeZoneResponse").Element("raw_offset").Value;
                        var DstOFFset = xdoc1.Element("TimeZoneResponse").Element("dst_offset").Value;
                        var TimeZoneId = xdoc1.Element("TimeZoneResponse").Element("time_zone_id");
                        var TimeZoneName = xdoc1.Element("TimeZoneResponse").Element("time_zone_name");
                        double Basehours = Convert.ToDouble(RawOffset) / 3600;
                        double DSThours = Convert.ToDouble(DstOFFset) / 3600;
                        ConvertedTime = DateTime.UtcNow.AddHours(Basehours + DSThours);
                        logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method | To retrieve TimeOffset Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) ~ " + IP + " ~ Request: Timestamp : " + timestamp + " ~ Response: Converted Time: " + ConvertedTime + " ~ Call duration: " + Tx2.TotalMilliseconds);                   
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("In CountriesTimeZoneConversion GoogleAddressCall method | Exception:" + ex.Message + "~" + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            }
            return ConvertedTime;
        }

        /// <summary>
        /// Retrieves longitude, latitude, TimeOffset Values.Returns the Desired date by adding the retrieved TimeOffset value.
        /// </summary>
        /// <param name="AddressToGoogle"></param>
        /// <param name="GehcDesiredDate"></param>
        /// <returns></returns>
        public DateTime GoogleAddressCallForDesiredDate(string AddressToGoogle, DateTime GehcDesiredDate)
        {
            try
            {
              
                //Code Start - Ebaad Chowdhry - 23/1/2017 - Scope: US70 - Implemented address capturing via the Google Places API
                var keyString = ConfigurationManager.AppSettings["GooglePlacesKey"].ToString();
                var requestUri = string.Format(ConfigurationManager.AppSettings["GoogleAddressAPI"].ToString() + "query={0}&key={1}", Uri.EscapeDataString(AddressToGoogle), keyString);
                var req = HttpWebRequest.Create(requestUri);
                req.Method = "GET";
                req.ContentType = "application/xml";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GoogleProxyURL2"]))
                {
                    WebProxy proxyurl = new WebProxy(ConfigurationManager.AppSettings["GoogleProxyURL2"]);
                    req.Proxy = proxyurl;
                }
               
                DateTime startTimeTx4 = DateTime.Now;
                var response = req.GetResponse();
                DateTime endTimeTx4 = DateTime.Now;
                TimeSpan Tx4 = endTimeTx4.Subtract(startTimeTx4);
             
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = (from xml in xdoc.Descendants("result") select xml).FirstOrDefault();
                //Code End - Ebaad Chowdhry - 23/1/2017 - Scope: CR/Incident/Defect - Implemented address capturing via the Google Places API
                if (result != null)
                {


                    var locationElement = result.Element("geometry").Element("location");
                    var lat = locationElement.Element("lat");
                    var lng = locationElement.Element("lng");
                    logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method | To retrieve LatLng Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCallForDesiredDate) ~ " + IP + " ~ Request : Address: " + AddressToGoogle + " | URL: " + requestUri + " ~ Response: Lat: " + lat + " Lng: " + lng + " ~ Call duration: " + Tx4.TotalMilliseconds); 
                    string timestamp = GenerateTimeStamp();
                    //string url = "https://maps.googleapis.com/maps/api/timezone/xml?location=39.6034810,-119.6822510&timestamp=1331161200";
                    //string url = "https://maps.googleapis.com/maps/api/timezone/xml?location=" + lat.Value + "," + lng.Value + "&timestamp=" + timestamp;
                    string url = ConfigurationManager.AppSettings["GoogleLatLongAPI"] + lat.Value + "," + lng.Value + "&timestamp=" + timestamp;
                    //google encoded key Added by phanikanth p
                    var requesturltimestamp = objGoogleAPIKeyEncoder.GoogleKey(url);
                    HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create(requesturltimestamp);
                    req1.Method = "GET";
                    req1.Accept = "application/xml";
                    req1.ContentType = "application/xml";
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GoogleProxyURL2"]))
                    {
                        WebProxy proxyurl = new WebProxy(ConfigurationManager.AppSettings["GoogleProxyURL2"]);
                        req1.Proxy = proxyurl;
                    }
                    //var request1 = WebRequest.Create(url);
                   
                    DateTime startTimeTx3 = DateTime.Now;
                    var response1 = req1.GetResponse();
                    DateTime endTimeTx3 = DateTime.Now;
                    TimeSpan Tx3 = endTimeTx3.Subtract(startTimeTx3);
                    
                    if (response1 != null)
                    {
                        var xdoc1 = XDocument.Load(response1.GetResponseStream());
                        var RawOffset = xdoc1.Element("TimeZoneResponse").Element("raw_offset").Value;
                        var DstOFFset = xdoc1.Element("TimeZoneResponse").Element("dst_offset").Value;
                        var TimeZoneId = xdoc1.Element("TimeZoneResponse").Element("time_zone_id");
                        var TimeZoneName = xdoc1.Element("TimeZoneResponse").Element("time_zone_name");
                        double Basehours = Convert.ToDouble(RawOffset) / 3600;
                        double DSThours = Convert.ToDouble(DstOFFset) / 3600;
                        DesiredDate = GehcDesiredDate.AddHours(Basehours + DSThours);
                        logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method | To retrieve TimeOffset Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) ~ " + IP + " ~ Request: Timestamp : " + timestamp + " ~ Response: Converted Time: " + DesiredDate + " ~ Call duration: " + Tx3.TotalMilliseconds);                   
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("In CountriesTimeZoneConversion GoogleAddressCallForDesiredDate method | Exception:" + ex.Message + " ~ " + IP + "~ Request : NA ~ Response : NA ~ Call Duration : NA ");
            }
            return DesiredDate;
        }
    }
}
