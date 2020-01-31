using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PrimeHack_Updater
{
    class Updater
    {
        static string sysversion = "1.5.6";
        static CfgManager cfg = new CfgManager();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [STAThread]
        static void Main(string[] args)
        {
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

            migrate();

            string repo;
            if (cfg.isMainBranch()) repo = @"https://api.github.com/repos/shiiion/dolphin/releases/latest";
            else repo = @"https://api.github.com/repos/shiiion/Ishiiruka/releases/latest";

            html = VersionCheck.getJSONInfo(repo);
            remoteversion = VersionCheck.getVersion(html);

            if (cfg.isVersionsEqual(remoteversion))
            {
                runPrimeHack(args);
            }
            else
            {
                AllocConsole();

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

            if (!WriteAccess(".\\"))
                restartAsAdmin();

            dynamic j = JObject.Parse(html);
            JArray ja = j.assets;
            dynamic assets = ja[0];
            string url = assets.browser_download_url;

            downloadLatest(url);

            cfg.setVersion(remoteversion);

            Console.WriteLine("Updated successfully.");
            runPrimeHack(args);
        }

        public static void restartAsAdmin()
        {
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!hasAdministrativeRight)
            {
                string fileName = Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = fileName;

                try
                {
                    Process.Start(processInfo);
                }
                catch (Win32Exception)
                {
                    
                }

                System.Environment.Exit(1);
            }
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
        public static void migrate()
        {
            string primesettings = "";
            bool beam = false;

            if (File.Exists("hack_config.ini"))
            {
                Console.WriteLine("Importing PrimeHack settings");
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
                        primesettings += "PrimeHack/Camera Sensitivity = " + line.Replace("sensitivity = ", "") + "\n";
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

                    String nline;

                    if (line.StartsWith("index_0"))
                    {
                        if (beam)
                            nline = "PrimeHack/Beam 1 = " + line.Replace("index_0 = ", "");
                        else
                            nline = "PrimeHack/Visor 1 = " + line.Replace("index_0 = ", "");

                        primesettings += Regex.Replace(nline, "\b&\b", " & ") + "\n";

                        continue;
                    }

                    if (line.StartsWith("index_1"))
                    {
                        if (beam)
                            nline = "PrimeHack/Beam 2 = " + line.Replace("index_1 = ", "");
                        else
                            nline = "PrimeHack/Visor 2 = " + line.Replace("index_1 = ", "");

                        primesettings += Regex.Replace(nline, "\b&\b", " & ") + "\n";

                        continue;
                    }

                    if (line.StartsWith("index_2"))
                    {
                        if (beam)
                            nline = "PrimeHack/Beam 3 = " + line.Replace("index_2 = ", "");
                        else
                            nline = "PrimeHack/Visor 3 = " + line.Replace("index_2 = ", "");

                        primesettings += Regex.Replace(nline, "\b&\b", " & ") + "\n";

                        continue;
                    }

                    if (line.StartsWith("index_3"))
                    {
                        if (beam)
                            nline = "PrimeHack/Beam 4 = " + line.Replace("index_3 = ", "");
                        else
                            nline = "PrimeHack/Visor 4 = " + line.Replace("index_3 = ", "");

                        primesettings += Regex.Replace(nline, "\b&\b", " & ") + "\n";

                        continue;
                    }
                }

                String config = DE + "\\Config\\WiimoteNew.ini";
                if (!File.Exists(config))
                {
                    FileStream f = File.Create(config);
                    f.Close();

                    primesettings = "[Wiimote1]\n" + primesettings;

                    File.WriteAllLines(config, primesettings.Split('\n'));
                }
                else
                {
                    string[] oldwiimotenew = File.ReadAllLines(config);
                    List<string> newwiimote = new List<string>();

                    newwiimote.AddRange(oldwiimotenew);
                    newwiimote.Insert(2, primesettings.Substring(0, primesettings.Length - 1));

                    File.WriteAllLines(config, newwiimote.ToArray());
                }

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

            if (!cfg.isMainBranch())
            {
                if (!WriteAccess(".\\"))
                    p.StartInfo.Verb = "runas";
            }

            p.Start();

            System.Environment.Exit(1);
        }

        //Modified version of https://stackoverflow.com/a/3769421
        public static bool WriteAccess(string folderName)
        {
            if ((File.GetAttributes(folderName) & FileAttributes.ReadOnly) != 0)
                return false;

            // Get the access rules of the specified files (user groups and user names that have access to the file)
            var rules = Directory.GetAccessControl(folderName).GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

            // Get the identity of the current user and the groups that the user is in.
            var groups = WindowsIdentity.GetCurrent().Groups;
            string sidCurrentUser = WindowsIdentity.GetCurrent().User.Value;

            // Check if writing to the file is explicitly denied for this user or a group the user is in.
            if (rules.OfType<FileSystemAccessRule>().Any(r => (groups.Contains(r.IdentityReference) || r.IdentityReference.Value == sidCurrentUser) && r.AccessControlType == AccessControlType.Deny && (r.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData))
                return false;

            // Check if writing is allowed
            return rules.OfType<FileSystemAccessRule>().Any(r => (groups.Contains(r.IdentityReference) || r.IdentityReference.Value == sidCurrentUser) && r.AccessControlType == AccessControlType.Allow && (r.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData);
        }

        public static void downloadLatest(string url)
        {
            Console.WriteLine("New Update!\nDownloading: " + url);
            using (var client = new TimedWebClient())
            {
                client.Proxy = null;
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
