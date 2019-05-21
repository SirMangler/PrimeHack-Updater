using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;

namespace PrimeHack_Updator
{
    class Updater
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checking for latest updates.");

            string html = getVersionJSON();
            if (html == null || html.Length == 0)
            {
                Console.WriteLine("Press any key to launch.");
                Console.ReadKey();
                runPrimeHack();
            }

            dynamic j = JObject.Parse(html);

            string version = (string)j.tag_name;
            string currentversion = getVersion().Replace("\r\n", "");

            if (version.Equals(currentversion))
            {
                runPrimeHack();
            }
            else
            {
                while (true)
                {
                    Process[] runningProcesses = Process.GetProcessesByName("Dolphin");
                    if (runningProcesses.Length != 0)
                    {
                        Console.WriteLine("Dolphin is already running. Please close Dolphin to continue updating. Hit any key to continue.");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            JArray ja = j.assets;
            dynamic assets = ja[0];
            string url = assets.browser_download_url;

            downloadLatest(url);
            System.IO.File.WriteAllLines(".\\version.txt", new string[] { version });

            Console.WriteLine("Moving profiles into Documents.");
            cutProfiles();

            Console.WriteLine("Updated successfully.");
            runPrimeHack();
        }

        public static void runPrimeHack()
        {
            Process.Start(".\\Dolphin.exe");

            System.Environment.Exit(1);
        }

        public static void cutProfiles()
        {
            if (Directory.Exists(".\\Profiles\\"))
            {
                string[] files = Directory.GetFiles(".\\Profiles\\");
                string docspath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\Dolphin Emulator\Config\Profiles\Wiimote\");

                if (!Directory.Exists(docspath))
                {
                    Directory.CreateDirectory(docspath);
                }

                foreach (string file in files)
                {
                    File.Copy(file, docspath + file.Replace(".\\Profiles\\", ""), true);
                }

                Directory.Delete(".\\Profiles\\", true);
            }
        }

        public static string getVersion()
        {
            if (File.Exists(".\\version.txt"))
            {
                return System.IO.File.ReadAllText(".\\version.txt");
            }
            else
            {
                File.Create("version.txt");
                return "-1";
            }
        }

        public static void downloadLatest(string url)
        {
            Console.WriteLine("Downloading: " + url);
            using (var client = new WebClient())
            {
                client.DownloadFile(url, Path.GetTempPath() + "\\PrimeHackRelease.zip");
            }

            Console.WriteLine("Extracting PrimeHackRelease.zip");

            ZipArchive archive = ZipFile.OpenRead(Path.GetTempPath() + "\\PrimeHackRelease.zip");
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(".\\", file.FullName);
                string directory = Path.GetDirectoryName(completeFileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (file.Name != "")
                    file.ExtractToFile(completeFileName, true);
            }

            archive.Dispose();

            Console.WriteLine("Deleting PrimeHackRelease.zip");
            File.Delete(Path.GetTempPath() + "\\PrimeHackRelease.zip");
        }

        public static string getVersionJSON()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            | SecurityProtocolType.Tls11
            | SecurityProtocolType.Tls12
            | SecurityProtocolType.Ssl3;

            string html = string.Empty;
            string url = @"https://api.github.com/repos/shiiion/Ishiiruka/releases/latest";

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
