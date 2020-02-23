using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimeHack_Updater
{
    class VersionCheck
    {
        public static string getVersion(string html)
        {
            if (html == null || html.Length == 0)
            {
                VersionCheckError();
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
                Updater.ui.writeLine("Failed to retrieve version info: " + e.Message);
                VersionCheckError();
            }

            return html;
        }

        public static void VersionCheckError()
        {
            DialogResult result = MessageBox.Show("Failed to download the metadata for the latest version. Try again?", "PrimeHack Updater", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

            if (result == DialogResult.Yes)
            {
                Updater.restartAsAdmin();
            }
            else if (result == DialogResult.No)
            {
                Updater.runPrimeHack("");
            }
            else if (result == DialogResult.Cancel)
            {
                Environment.Exit(0);
            }
        }
    }
}
