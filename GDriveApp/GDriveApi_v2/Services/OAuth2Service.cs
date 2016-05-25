using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GDriveApi.Model;
using Newtonsoft.Json;

namespace GDriveApi_v2.Services
{
    public static class OAuth2Service
    {
        public static AuthResponse OAuth2_Creds;
        private static string clientId = ConfigurationSettings.AppSettings["client_id"];
        private static string secret = ConfigurationSettings.AppSettings["client_secret"];
        private static string redirectUri = "http://localhost:26774/Home/GoogleCallback";

        public static Uri OAuth2Url()
        {
            string scopes = "https://www.googleapis.com/auth/plus.login email";
            string oauth = string.Format("https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&scope={2}&response_type=code", clientId, redirectUri, scopes);
            return new Uri(oauth);
        }

        public static void GetToken(string authCode)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");
            string postData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", authCode, clientId, secret, redirectUri);
            
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            OAuth2_Creds = JsonConvert.DeserializeObject<AuthResponse>(responseString);
        }
    }
}
