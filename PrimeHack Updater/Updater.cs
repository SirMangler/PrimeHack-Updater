using Newtonsoft.Json.Linq;
using PrimeHack_Updater;
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
        static string sysversion = "1.0.2";

        static void Main(string[] args)
        {
            Console.WriteLine("Checking for latest updates.");

            string html = VersionCheck.getJSONInfo(@"https://api.github.com/repos/SirMangler/PrimeHack-Updater/releases/latest");
            string remoteversion = VersionCheck.getVersion(html);

            if (remoteversion.Equals(sysversion))
            {
                Console.WriteLine("PrimeHack Updater has an available update. It is highly recommended you update in order for PrimeHack Updater to support the latest features!");
                Console.WriteLine("Update Link: https://github.com/SirMangler/PrimeHack-Updater/releases/");
                Console.WriteLine("Press Any Key To Continue");
                Console.ReadKey();
            }

            html = VersionCheck.getJSONInfo(@"https://api.github.com/repos/shiiion/Ishiiruka/releases/latest");
            remoteversion = VersionCheck.getVersion(html);

            string currentversion = getVersion().Replace("\r\n", "");

            if (currentversion.Equals(currentversion))
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

            dynamic j = JObject.Parse(html);
            JArray ja = j.assets;
            dynamic assets = ja[0];
            string url = assets.browser_download_url;

            downloadLatest(url);
            System.IO.File.WriteAllLines(".\\version.txt", new string[] { remoteversion });

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
                    String newpath = docspath + file.Replace(".\\Profiles\\", "");

                    if (!File.Exists(file))
                        File.Copy(file, newpath, true);      
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

                if (File.Exists(completeFileName))
                {
                    if (completeFileName.EndsWith("hack_config.ini"))
                        continue;

                    long ziptime = file.LastWriteTime.ToFileTime();
                    long oldtime = File.GetLastWriteTime(completeFileName).ToFileTime();

                    if (ziptime == oldtime)
                        continue;                
                }

                if (file.Name != "") {
                    Console.WriteLine("Transferring: " + completeFileName);
                    file.ExtractToFile(completeFileName, true);
                }
            }

            archive.Dispose();

            Console.WriteLine("Deleting PrimeHackRelease.zip");
            File.Delete(Path.GetTempPath() + "\\PrimeHackRelease.zip");
        }


    }
}
