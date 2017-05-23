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
        public DateTime ConvertedTime;
        public DateTime DesiredDate;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        GoogleAPIKeyEncoder objGoogleAPIKeyEncoder = new GoogleAPIKeyEncoder();
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
                logger.Error("CountriesTimeZoneConversion;GetTimeZoneMethod; Exception:" + ex.Message);
                //throw;
            }

            return ConvertedTime;
        }
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
                logger.Error("CountriesTimeZoneConversion;GetTimeZoneMethod; Exception:" + ex.Message);
                //throw;
            }

            return DesiredDate;

        }
        private string GenerateTimeStamp()
        {
            string str = String.Empty;

            try
            {
                str = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
            }
            catch (Exception ex)
            {
                logger.Error("CountriesTimeZoneConversion;GenerateTimeStamp; Exception:" + ex.Message);
            }
            return str;
        }
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
                logger.Debug("Creating URI of GoogleAddressAPI =" + requestUri + "End of Creating URI for GoogleAddressAPI");
                logger.Debug("Creating URI of GoogleAddressAPI using WebRequest: Request=" + req + " End");
                logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve LatLng Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) with address: " + AddressToGoogle);
                DateTime startTimeTx1 = DateTime.Now;
                var response = req.GetResponse();
                DateTime endTimeTx1 = DateTime.Now;
                TimeSpan Tx1 = endTimeTx1.Subtract(startTimeTx1);
                logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve LatLng Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) | Call duration: " + Tx1.Milliseconds);
                logger.Debug("Getting the response from GoogleAddressAPI using GetResponse method");
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = (from xml in xdoc.Descendants("result") select xml).FirstOrDefault();
                //Code End - Ebaad Chowdhry - 23/1/2017 - Scope: CR/Incident/Defect - Implemented address capturing via the Google Places API
                if (result != null)
                {


                    var locationElement = result.Element("geometry").Element("location");
                    var lat = locationElement.Element("lat");
                    var lng = locationElement.Element("lng");
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
                  
                    logger.Debug("Creating URI of GoogleLatLongAPI using WebRequest:request1=" + req);
                    logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve TimeOffset Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) for timestamp: " + timestamp);
                    DateTime startTimeTx2 = DateTime.Now;
                    var response1 = req1.GetResponse();
                    DateTime endTimeTx2 = DateTime.Now;
                    TimeSpan Tx2 = endTimeTx2.Subtract(startTimeTx2);
                    logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve TimeOffset Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) | Call duration: " + Tx2.Milliseconds);
                    logger.Debug("Getting the response from GoogleAddressAPI using GetResponse method");
                    logger.Debug("Getting the response of GoogleLatLongAPI using WebRequest");
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
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("CountriesTimeZoneConversion;GoogleAddressCall; Exception:" + ex.Message);
            }
            return ConvertedTime;
        }
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
                logger.Debug("Creating URI of GoogleAddressAPI =" + requestUri + "End of Creating URI for GoogleAddressAPI");
                logger.Debug("Creating URI of GoogleAddressAPI using WebRequest: Request=" + req + " End");
                logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve LatLng Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) with address: " + AddressToGoogle);
                DateTime startTimeTx4 = DateTime.Now;
                var response = req.GetResponse();
                DateTime endTimeTx4 = DateTime.Now;
                TimeSpan Tx4 = endTimeTx4.Subtract(startTimeTx4);
                logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve LatLng Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) | Call duration: " + Tx4.Milliseconds);
                logger.Debug("Getting the response from GoogleAddressAPI using GetResponse method");
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = (from xml in xdoc.Descendants("result") select xml).FirstOrDefault();
                //Code End - Ebaad Chowdhry - 23/1/2017 - Scope: CR/Incident/Defect - Implemented address capturing via the Google Places API
                if (result != null)
                {


                    var locationElement = result.Element("geometry").Element("location");
                    var lat = locationElement.Element("lat");
                    var lng = locationElement.Element("lng");
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
                    logger.Debug("Creating URI of GoogleLatLongAPI using WebRequest:request1=" + req);
                    logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve TimeOffset Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) with timestamp " + timestamp);
                    DateTime startTimeTx3 = DateTime.Now;
                    var response1 = req1.GetResponse();
                    DateTime endTimeTx3 = DateTime.Now;
                    TimeSpan Tx3 = endTimeTx3.Subtract(startTimeTx3);
                    logger.Debug("In CountriesTimeZoneConversion GoogleAddressCall Method to retrieve TimeOffset Values | Call to CountriesTimeZoneConversion.cs (CountriesTimeZoneConversion.GoogleAddressCall) | Call duration: " + Tx3.Milliseconds);
                    logger.Debug("Getting the response of GoogleLatLongAPI using WebRequest");
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
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("CountriesTimeZoneConversion;GoogleAddressCallForDesiredDate; Exception:" + ex.Message);
            }
            return DesiredDate;
        }
    }
}
