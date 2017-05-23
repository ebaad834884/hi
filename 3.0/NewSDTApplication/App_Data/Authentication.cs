using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace NewSDTApplication
{
    public class Authentication
    {
    }

    public class Program
    {
        //Code Start-Hita - 12/05/2017 - US250/TA1680 
        private Program()
        {
        }
        //Code End-Hita - 12/05/2017 - US250/TA1680 

        public static void DetectMethod(string authToken)
        {
            // Development URL
            string uri = "https://dev.api.ge.com:443/gecorp/sentry/apitest";
            // Stage URL
            //string uri = "https://stage.api.ge.com:443/gecorp/sentry/apitest"
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string languageDetected = (string)dcs.ReadObject(stream);

                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
        public static void ProcessWebException(WebException e)
        {
            Console.WriteLine("{0}", e.ToString());
            // Obtain detailed error information
            string strResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
        }
        /// <summary>
        /// Access Token Class
        /// </summary>
        [DataContract]
        public class AccessToken
        {
            [DataMember]
            public string access_token { get; set; }
            [DataMember]
            public string token_type { get; set; }
            [DataMember]
            public string expires_in { get; set; }
            [DataMember]
            public string scope { get; set; }

            public class OAuthAuthentication
            {
               // public static readonly string OAuthUri = "https://fssfed.stage.ge.com/fss/as/token.oauth2?";
                private static string OAuthUri = ConfigurationManager.AppSettings["OAuthUri"].ToString();

                private string clientId;
                private string clientSecret;
                private string request;
                private AccessToken token;
                private Timer accessTokenRenewer;
                //Access token expires every 10 minutes. Renew it every 9 minutes only.
                private const int RefreshTokenDuration = 9;  // put this value in external cookie / Web config

                public OAuthAuthentication(string clientId, string clientSecret, string scope)
                {
                    this.clientId = clientId;
                    this.clientSecret = clientSecret;
                    //If clientid or client secret has special characters, encode before sending request
                    this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret), HttpUtility.UrlEncode(scope));
                    this.token = HttpPost(OAuthUri, this.request);
                    //this.token.access_token="1234" ;
                    //this.token.expires_in = DateTime.Now.AddHours(2).ToString();
                    //this.token.scope = "sfsdf";

                        HttpCookie aCookie = HttpContext.Current.Request.Cookies["AccessTokenObject"] as HttpCookie;
                        aCookie.Values["Token"] = this.token.access_token;
                        aCookie.Values["Expires"] = this.token.expires_in;
                        aCookie.Values["Token_Type"] = this.token.token_type;
                        aCookie.Expires = DateTime.Now.AddSeconds(Convert.ToDouble(this.token.expires_in));
                        aCookie.Path = @"%AppData%\Microsoft\Windows\Cookies";
                        HttpContext.Current.Response.Cookies.Add(aCookie);
                   
                    //renew the token every specfied minutes
                    //  accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback), this, TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                public AccessToken GetAccessToken()
                {
                    return this.token;
                }
                private void RenewAccessToken()
                {
                    AccessToken newAccessToken = HttpPost(OAuthUri, this.request);
                    //swap the new token with old one
                    //Note: the swap is thread unsafe
                    this.token = newAccessToken;
                    Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", this.clientId, this.token.access_token));
                }
                private void OnTokenExpiredCallback(object stateInfo)
                {
                    try
                    {
                        RenewAccessToken();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
                    }
                    finally
                    {
                        try
                        {
                            accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                        }
                    }
                }
                private AccessToken HttpPost(string OAuthUri, string requestDetails)
                {
                    //Prepare OAuth request 
                    WebRequest webRequest = WebRequest.Create(OAuthUri);
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.Method = "POST";
                    byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                    webRequest.ContentLength = bytes.Length;
                    using (Stream outputStream = webRequest.GetRequestStream())
                    {
                        outputStream.Write(bytes, 0, bytes.Length);
                    }
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccessToken));
                        //Get deserialized object from JSON stream
                        AccessToken token = (AccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                        return token;
                    }
                }
            }
        }
    }

    //public class SessionExpireAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        HttpContext ctx = HttpContext.Current;

    //        // check  sessions here
    //        if (HttpContext.Current.Session["SiebelHttpPostParams"] == null)
    //        {
    //            filterContext.Result = new RedirectResult("~/Shared/Error");
    //            return;
    //        }

    //        base.OnActionExecuting(filterContext);
    //    }
    //}
}