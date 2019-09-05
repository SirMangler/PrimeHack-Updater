using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InternalUpdater
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Severe Error. No arguments supplied. This should be impossible. Please reinstall PrimeHack.", "PrimeHack Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            while (true)
            {
                Process[] runningProcesses = Process.GetProcessesByName("PrimeHack Updater.exe");
                if (runningProcesses.Length != 0)
                {
                    Console.WriteLine("PrimeHack Updater is running. Please close it to continue updating. Hit any key to continue.");
                    Console.ReadKey();
                }
                else
                {
                    break;
                }
            }

            string path = "";
            for (int i = 1; i < args.Length; i++)
            {
                path = path+args[i]+" ";
            }

            try
            {
                downloadLatest(args[0], path);
            } catch (Exception e)
            {
                MessageBox.Show("Download Failed.\nError: "+e.Message, "PrimeHack Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void downloadLatest(string url, string extractpath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12
                | SecurityProtocolType.Ssl3;

            Console.WriteLine("Downloading: " + url);
            using (var client = new WebClient())
            {
                Console.WriteLine("Downloading to: "+extractpath);
                client.DownloadFile(url, extractpath);
            }

            Process.Start(extractpath);
            Environment.Exit(0);
        }
    }
}
