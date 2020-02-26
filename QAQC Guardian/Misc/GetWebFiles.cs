using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace QAQC_Guardian.Misc
{
    static class GetWebFiles
    {
        public static string GetDirectoryListingRegexForUrl(string url)
        {
            if (url.Equals(url.ToString()))
            {
                return "<a href=\".*\">(?<name>.*)</a>";
            }
            throw new NotSupportedException();
        }

        public static List<string> Get(string url)
        {
            string start = url.Remove(url.IndexOf("/sites"));
            string newPath = url.Replace(start, "");

            var results = new List<string>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UseDefaultCredentials = true;
            request.PreAuthenticate = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();

                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(Globals.PathFiles + "test.txt"))
                    {
                        file.Write(html);
                    }

                    Regex regex = new Regex(GetDirectoryListingRegexForUrl(newPath));
                    MatchCollection matches = regex.Matches(html);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Success)
                            {
                                results.Add(match.Groups["name"].ToString());
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
