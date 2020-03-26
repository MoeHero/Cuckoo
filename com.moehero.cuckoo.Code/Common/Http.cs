using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace com.moehero.cuckoo.Code
{
    internal static class Http
    {
        static Http() {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }

        private static string SendRequest(HttpRequest request) {
            var req = (HttpWebRequest)WebRequest.Create(request.Url);
            req.Method = request.Method;
            req.Proxy = null;
            req.KeepAlive = false;
            req.CookieContainer = request.Cookie;
            req.AllowAutoRedirect = false;

            if(request.Method.ToUpper() == "POST") {
                req.ContentType = "application/x-www-form-urlencoded";
                var data = Encoding.UTF8.GetBytes(request.Body);
                using(var stream = req.GetRequestStream()) stream.Write(data, 0, data.Length);
            }
            try {
                var response = req.GetResponse();
                var streamReader = new StreamReader(response.GetResponseStream());
                var r = streamReader.ReadToEnd();
                streamReader.Close();
                return r;
            } catch {
                return SendRequest(request);
            }
        }

        public static Task<string> Get(string url, CookieContainer cookie = null) {
            return Task.Run(() => SendRequest(new HttpRequest(url) { Cookie = cookie }));
        }

        public async static Task<JObject> GetJson(string url, CookieContainer cookie = null) {
            return JObject.Parse(await Get(url, cookie));
        }

        public static Task<string> Post(string url, string body, CookieContainer cookie = null) {
            return Task.Run(() => SendRequest(new HttpRequest(url) { Method = "POST", Body = body, Cookie = cookie }));
        }

        public async static Task<JObject> PostJson(string url, string body, CookieContainer cookie = null) {
            return JObject.Parse(await Post(url, body, cookie));
        }

        public static string UrlEncode(string str) {
            if(str == null) return "";
            var result = "";
            foreach(var c in str) {
                if(HttpUtility.UrlEncode(c.ToString()).Length > 1) result += HttpUtility.UrlEncode(c.ToString()).ToUpper();
                else result += c;
            }
            return result;
        }
    }

    internal class HttpRequest
    {
        public HttpRequest(string url) {
            Url = url;
        }

        public string Url { get; }
        public string Body { get; set; }
        public string Method { get; set; } = "GET";
        public CookieContainer Cookie { get; set; }
    }
}
