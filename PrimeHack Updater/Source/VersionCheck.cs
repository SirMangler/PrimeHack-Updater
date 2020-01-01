using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PrimeHack_Updater
{
    class VersionCheck
    {
        public static string getVersion(string html)
        {
            if (html == null || html.Length == 0)
            {
                Console.WriteLine("Couldn't access JSON info. Press any key to launch.");
                Console.ReadKey();

                return "-1";
            }

            dynamic j = JObject.Parse(html);
            string version = (string) j.tag_name;

            return version;
        }

        public static string getJSONInfo(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            | SecurityProtocolType.Tls11
            | SecurityProtocolType.Tls12
            | SecurityProtocolType.Ssl3;

            string html = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.UserAgent = "PrimeHackUpdater";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to retrieve version info: " + e.Message);
            }

            return html;
        }
    }
}
