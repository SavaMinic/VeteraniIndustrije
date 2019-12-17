using System;
using System.Net;

public class DownloadCSV
{
    public class WebClientEx : WebClient
    {
        public WebClientEx(CookieContainer container)
        {
            this.container = container;
        }



        private readonly CookieContainer container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = container;
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                container.Add(cookies);
            }
        }
    }

    public static string FromSheets(string url, string format)
    {
        /*
         1. Your Google SpreadSheet document must be set to 'Anyone with the link' can view it
         2. To get URL press SHARE (top right corner) on Google SpreeadSheet and copy "Link to share".
         3. Now add "&output=csv" parameter to this link
         4. Your link will look like:
            https://docs.google.com/spreadsheet/ccc?key=1234abcd1234abcd1234abcd1234abcd1234abcd1234&usp=sharing&output=json

        or https://spreadsheets.google.com/feeds/list/1T3-e6tCywND4s5K0HtBjPwJdwp_USsRCLa21hM9qrSM/od6/public/values?alt=json&callback=dataCallback

        https://docs.google.com/spreadsheets/d/1lz8TU57wZNjsOtOB3ULxXofn45axbHiA-qX37kF2sOM/export?gid=0&format=json
        */

        const string FORMAT_PREFIX = "/export?gid=0&format=";
        string suffix = FORMAT_PREFIX + format;
        string str = GetResponseText(url + suffix);

        /*
        const string PREFIX = "meta property=\"og:description\" content=";
        int i = str.IndexOf(PREFIX);
        UnityEngine.Debug.Log(i);
        int si = i + PREFIX.Length + 1;
        str = str.Substring(si);

        const string SUFFIX = "\"><meta name=\"google\"";
        int ei = str.IndexOf(SUFFIX);
        str = str.Substring(0, ei);
        */

        return str;

        //WebClientEx wc = new WebClientEx(new CookieContainer());
        //wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0");
        //wc.Headers.Add("DNT", "1");
        //wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        //wc.Headers.Add("Accept-Encoding", "deflate");
        //wc.Headers.Add("Accept-Language", "en-US,en;q=0.5");

        //return wc.DownloadString(url);
    }

    public static string GetResponseText(string address)
    {
        var request = (HttpWebRequest)WebRequest.Create(address);

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            var encoding = System.Text.Encoding.GetEncoding(response.CharacterSet);

            using (var responseStream = response.GetResponseStream())
            using (var reader = new System.IO.StreamReader(responseStream, encoding))
                return reader.ReadToEnd();
        }
    }
}