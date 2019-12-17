using System;
using System.Net;

public class DownloadCSV
{
    public static string FromSheets(string url, string format)
    {
        /*
         1. Your Google SpreadSheet document must be set to 'Anyone with the link' can view it
         2. Remove everything past the last slash, like /edit... or /view..
         You link should look something like:
            https://docs.google.com/spreadsheets/d/12345U57wZNjsOtOB3ULxXofn45axbHiA-qX37kF2sOM
        */

        const string FORMAT_PREFIX = "/export?gid=0&format=";
        string suffix = FORMAT_PREFIX + format;
        string str = GetResponseText(url + suffix);

        return str;
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