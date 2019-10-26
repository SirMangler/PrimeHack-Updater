using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;

namespace PrimeHack_Updater
{
    class Updater
    {
        static string sysversion = "1.5.3";
        static CfgManager cfg = new CfgManager();

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Checking for latest updates.");

            string html = VersionCheck.getJSONInfo(@"https://api.github.com/repos/SirMangler/PrimeHack-Updater/releases/latest");
            string remoteversion = VersionCheck.getVersion(html);

            if (!remoteversion.Equals(sysversion))
            {
                DialogResult dialogResult = MessageBox.Show("PrimeHack Updater has a new update. Do you want it to update itself?", "PrimeHack Updater", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    InternalUpdater.update();
                }
            }

            Console.WriteLine("Importing PrimeHack settings");
            portableMode();

            string repo;
            if (cfg.isMainBranch())
                repo = @"https://api.github.com/repos/shiiion/dolphin/releases/latest";
            else repo = @"https://api.github.com/repos/shiiion/Ishiiruka/releases/latest";

            html = VersionCheck.getJSONInfo(repo);
            remoteversion = VersionCheck.getVersion(html);

            if (cfg.isVersionsEqual(remoteversion))
            {
                runPrimeHack(args);
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

            cfg.setVersion(remoteversion);

            Console.WriteLine("Updated successfully.");
            runPrimeHack(args);
        }

        static string quickpath = null;
        public static void isoSelection()
        {
            string path = cfg.getISOPath();

            if (path.Equals("NEVER")) return;

            if (path.Equals(""))
            {
                path = getSelectionResult();
            } else
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show("Cannot find file: " + path);

                    path = getSelectionResult();
                }
            }

            quickpath = path;

            cfg.setISOPath(quickpath);
        }

        public static string getSelectionResult()
        {
            string path = "";

            using (var dialog = new ISOSelectionDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    OpenFileDialog filedialog = new OpenFileDialog();
                    filedialog.Filter = "All GC/Wii files|*.elf;*.dol;*.gcm;*.tgc;*.iso;*.wbfs;*.ciso;*.gcz;*.wad;*.dff";
                    filedialog.FilterIndex = 1;

                    if (STAShowDialog(filedialog) == DialogResult.OK)
                    {
                        path = filedialog.FileName;
                    }
                    else
                    {
                        isoSelection();
                    }
                }
                else if (result == DialogResult.No)
                {
                    path = "NEVER";
                }
                else
                {
                    path = "";
                }
            }

            return path;
        }

        public static DialogResult STAShowDialog(FileDialog dialog)
        {
            DialogState state = new DialogState();

            state.dialog = dialog;

            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);

            t.SetApartmentState(System.Threading.ApartmentState.STA);

            t.Start();

            t.Join();

            return state.result;
        }

        static string DE = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\Dolphin Emulator\");
        public static void portableMode()
        {
            string[] lines = File.ReadAllLines(DE+ "Config\\Dolphin.ini");

            string primesettings = "";
            bool beam = false;

            if (File.Exists("hack_config.ini"))
            {
                foreach (string line in File.ReadLines("hack_config.ini"))
                {
                    if (line.StartsWith("[beam]"))
                    {
                        beam = true;
                        continue;
                    }

                    if (line.StartsWith("[visor]"))
                    {
                        beam = false;
                        continue;
                    }

                    if (line.StartsWith("sensitivity"))
                    {
                        primesettings += "PrimeHack/Camera Sensitivity = "+line.Replace("sensitivity = ", "") +"\n";
                        continue;
                    }

                    if (line.StartsWith("cursor_sensitivity"))
                    {
                        primesettings += "PrimeHack/Cursor Sensitivity = " + line.Replace("cursor_sensitivity = ", "") + "\n";
                        continue;
                    }

                    if (line.StartsWith("fov"))
                    {
                        primesettings += "PrimeHack/Field of View = " + line.Replace("fov = ", "") + "\n";
                        continue;
                    }

                    if (line.StartsWith("inverted_y"))
                    {
                        primesettings += "PrimeHack/Invert Y axis = " + line.Replace("inverted_y = ", "") + "\n";
                        continue;
                    }

                    if (line.StartsWith("index_0"))
                    {
                        if (beam)
                            primesettings += "PrimeHack/Beam 1 = " + line.Replace("index_0 = ", "").Replace("&", " & ") + "\n";
                        else
                            primesettings += "PrimeHack/Visor 1 = " + line.Replace("index_0 = ", "").Replace("&", " & ") + "\n";

                        continue;
                    }

                    if (line.StartsWith("index_1"))
                    {
                        if (beam)
                            primesettings += "PrimeHack/Beam 2 = " + line.Replace("index_1 = ", "").Replace("&", " & ") + "\n";
                        else
                            primesettings += "PrimeHack/Visor 2 = " + line.Replace("index_1 = ", "").Replace("&", " & ") + "\n";

                        continue;
                    }

                    if (line.StartsWith("index_2"))
                    {
                        if (beam)
                            primesettings += "PrimeHack/Beam 3 = " + line.Replace("index_2 = ", "").Replace("&", " & ") + "\n";
                        else
                            primesettings += "PrimeHack/Visor 3 = " + line.Replace("index_2 = ", "").Replace("&", " & ") + "\n";

                        continue;
                    }

                    if (line.StartsWith("index_3"))
                    {
                        if (beam)
                            primesettings += "PrimeHack/Beam 4 = " + line.Replace("index_3 = ", "").Replace("&", " & ") + "\n";
                        else
                            primesettings += "PrimeHack/Visor 4 = " + line.Replace("index_3 = ", "").Replace("&", " & ") + "\n";

                        continue;
                    }
                }

                string[] oldwiimotenew = File.ReadAllLines(DE+"\\Config\\WiimoteNew.ini");
                List<string> newwiimote = new List<string>();

                newwiimote.AddRange(oldwiimotenew);
                newwiimote.Insert(2, primesettings.Substring(0, primesettings.Length-1));

                File.WriteAllLines(DE+"\\Config\\WiimoteNew.ini", newwiimote.ToArray());

                File.Delete(".\\hack_config.ini");
            }                     
        }

        public static void runPrimeHack(string[] args)
        {
            Console.WriteLine("ISO Selection");
            isoSelection();

            Process p = new Process();
            p.StartInfo.FileName = ".\\Dolphin.exe";
            p.StartInfo.UseShellExecute = true;

            if (quickpath != null)
            {
                if (!quickpath.Equals("") && !quickpath.Equals("NEVER"))
                {
                    p.StartInfo.Arguments = "-e \"" + quickpath + "\"";
                } else
                {
                    p.StartInfo.Arguments = string.Join(" ", args);
                }
            } else
            {
                p.StartInfo.Arguments = string.Join(" ", args);
            }

            p.Start();

            //Console.ReadKey();
            System.Environment.Exit(1);
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
                    //Console.WriteLine("Transferring: " + completeFileName);
                    file.ExtractToFile(completeFileName, true);
                }
            }

            archive.Dispose();

            Console.WriteLine("Deleting PrimeHackRelease.zip");
            File.Delete(Path.GetTempPath() + "\\PrimeHackRelease.zip");
        }

        public class DialogState
        {
            public DialogResult result;
            public FileDialog dialog;

            public void ThreadProcShowDialog()
            {
                result = dialog.ShowDialog();
            }
        }
    }
}
