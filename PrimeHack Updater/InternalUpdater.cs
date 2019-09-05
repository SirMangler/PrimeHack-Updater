using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PrimeHack_Updater
{
    class InternalUpdater
    {
        public static void update()
        {
            string html = VersionCheck.getJSONInfo(@"https://api.github.com/repos/SirMangler/PrimeHack-Updater/releases/latest");

            dynamic j = JObject.Parse(html);
            JArray ja = j.assets;
            dynamic assets = ja[0];
            string url = assets.browser_download_url;

            string temp = Path.Combine(Path.GetTempPath(), "PrimeHack_InternalUpdater.exe");

           if (!File.Exists(temp)) 
                File.WriteAllBytes(temp, PrimeHack_Updater.Properties.Resources.InternalUpdater);            
            
            Process p = new Process();
            p.StartInfo.FileName = temp;
            p.StartInfo.Arguments = url+" " +System.Reflection.Assembly.GetEntryAssembly().Location;

            p.Start();

            Environment.Exit(1);
        }
    }
}
