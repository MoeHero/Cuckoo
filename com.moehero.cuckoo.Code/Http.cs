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
            req.KeepAlive = true;
            req.CookieContainer = request.Cookie;

            if(request.Method.ToUpper() == "POST") {
                req.ContentType = "application/x-www-form-urlencoded";
                var data = Encoding.UTF8.GetBytes(request.Body);
                using(var stream = req.GetRequestStream()) stream.Write(data, 0, data.Length);
            }
            try {
                var response = req.GetResponse();
                using(var streamReader = new StreamReader(response.GetResponseStream())) {
                    return streamReader.ReadToEnd();
                }
            } catch {
                return SendRequest(request);
            }
        }

        public static Task<string> Get(string url, CookieContainer cookie = null) {
            return Task.Run(() => SendRequest(new HttpRequest(url) { Cookie = cookie }));
        }

        public async static Task<JObject> GetJson(string url, CookieContainer cookie = null) {
            try {
                return JObject.Parse(await Get(url, cookie));
            } catch {
                //TODO 返回错误信息
                return JObject.Parse("{'code': -9999}");
            }
        }

        public static Task<string> Post(string url, string body, CookieContainer cookie = null) {
            return Task.Run(() => SendRequest(new HttpRequest(url) { Method = "POST", Body = body, Cookie = cookie }));
        }

        public async static Task<JObject> PostJson(string url, string body, CookieContainer cookie = null) {
            try {
                return JObject.Parse(await Post(url, body, cookie));
            } catch {
                //TODO 返回错误信息
                return JObject.Parse("{'code': -9999}");
            }
        }

        public static string BuildParam(object param) {
            var _param = new Dictionary<string, string>();
            foreach(var p in param.GetType().GetProperties())
                _param.Add(p.Name, p.GetValue(param).ToString());
            _param = _param.OrderBy(r => r.Key).ToDictionary(r => r.Key, r => r.Value);
            return _param.Aggregate("", (current, p) => $"{current}{p.Key}={UrlEncode(p.Value)}&").TrimEnd('&');
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
