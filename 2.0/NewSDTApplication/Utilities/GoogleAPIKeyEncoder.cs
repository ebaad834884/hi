using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace NewSDTApplication.Utilities
{
    public class GoogleAPIKeyEncoder
    {
        public string GoogleKey(string url)
        {
            //Below keystring comes from web.config

            var keyString = ConfigurationManager.AppSettings["GoogleKey"].ToString();
            // "x-6pVe_k8m5B-8NjtFS38aHQVKw=";
            ASCIIEncoding encoding = new ASCIIEncoding();

            //URL-safe decoding
            byte[] privateKeyBytes = Convert.FromBase64String(keyString.Replace("-", "+").Replace("_", "/"));

            Uri objURI = new Uri(url);
            byte[] encodedPathAndQueryBytes = encoding.GetBytes(objURI.LocalPath + objURI.Query);

            //compute the hash
            HMACSHA1 algorithm = new HMACSHA1(privateKeyBytes);
            byte[] hash = algorithm.ComputeHash(encodedPathAndQueryBytes);

            //convert the bytes to string and make url-safe by replacing '+' and '/' characters
            string EncodedGoogleKey = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

            //Add the signature to the existing URI.
            return objURI.Scheme + "://" + objURI.Host + objURI.LocalPath + objURI.Query + "&Key=" + EncodedGoogleKey;
        }
    }
}