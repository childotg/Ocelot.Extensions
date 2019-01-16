using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Ocelot.Extensions.Configuration.Common
{
    public static class SigningUtils
    {
        public static string SignUriForGoogleCloudStorage(string bucketName, string objectName, string accessKey, string secret)
        {
            var GCS_API_ENDPOINT = "https://storage.googleapis.com";

            var expiration = (Int32)(DateTime.UtcNow.AddHours(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var missing = string.Empty;
            var signature_string = $"GET\n{missing}\n{missing}\n{expiration}\n/{bucketName}/{objectName}";
            var utf8 = Encoding.UTF8.GetBytes(signature_string);

            HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));

            var hash = hmac.ComputeHash(utf8);
            var base64 = Convert.ToBase64String(hash);
            var signature = HttpUtility.UrlEncode(base64);

            var signed_url = $"{GCS_API_ENDPOINT}/{bucketName}/{objectName}?GoogleAccessId={accessKey}&Expires={expiration}&Signature={signature}";

            return signed_url;
        }

        public static string SignUriForAzureStorage(string accountName, string accountKey, string resourceUri, string type)
        {
            string e(string str) => HttpUtility.UrlEncode(str);

            var signedpermissions = "r";
            var signedstart = DateTime.UtcNow.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var signedexpiry = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var canonicalizedresource = $"/{type}/{accountName}/{resourceUri}";
            var signedversion = "2018-03-28";
            var missing = string.Empty;

            var stringToSign = string.Format(CultureInfo.InvariantCulture,
                "{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}",
                signedpermissions, signedstart, signedexpiry, canonicalizedresource,
                missing, missing, missing,
                signedversion,
                missing, missing, missing, missing, missing);

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(accountKey));
            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var token = $"?st={e(signedstart)}&se={e(signedexpiry)}&sp={e(signedpermissions)}"
                + $"&sv={e(signedversion)}&sr={e(type.Substring(0, 1))}&sig={e(signature)}";
            var url = $"https://{accountName}.{type}.core.windows.net/{resourceUri}{token}";
            return url;
        }
    }
}
