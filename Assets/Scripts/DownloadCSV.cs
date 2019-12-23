using System.Net;

public static class SheetsDownloader
{
    /// <param name="url">Google Sheets link (set to view 'Anyone with the link'), without stuff after slash: /edit.. or /view..
    /// For example https://docs.google.com/spreadsheets/d/12345U57wZNjsOtOB3ULxXofn45axbHiA-qX37kF2sOM </param>
    /// <param name="format">Data format, for example "csv" or "tsv"</param>
    /// <returns></returns>
    public static string Download(string url, string format)
    {
        const string FORMAT_PREFIX = "/export?gid=0&format=";
        string suffix = FORMAT_PREFIX + format;
        string str = GetResponseText(url + suffix);

        return str;
    }

    static string GetResponseText(string address)
    {
        var request = (HttpWebRequest)WebRequest.Create(address);

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            var encoding = System.Text.Encoding.UTF8;// System.Text.Encoding.GetEncoding(response.CharacterSet);

            using (var responseStream = response.GetResponseStream())
            using (var reader = new System.IO.StreamReader(responseStream, encoding))
                return reader.ReadToEnd();
        }
    }
}