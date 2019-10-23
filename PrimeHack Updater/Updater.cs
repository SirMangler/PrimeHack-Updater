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
        static string sysversion = "1.5.2";

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

            Console.WriteLine("Checking for Local Dolphin Configuration");
            portableMode();

            html = VersionCheck.getJSONInfo(@"https://api.github.com/repos/shiiion/dolphin/releases/latest"); // https://api.github.com/repos/shiiion/dolphin/releases/latest
            remoteversion = VersionCheck.getVersion(html);

            string currentversion = getVersion().Replace("\r\n", "");

            if (currentversion.Equals(remoteversion))
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

            updateVersionFile(remoteversion, null);

            Console.WriteLine("Moving profiles into Documents.");
            cutProfiles();

            Console.WriteLine("Updated successfully.");
            runPrimeHack(args);
        }

        public static void updateVersionFile(string version, string quicklaunch)
        {
            var lines = System.IO.File.ReadAllLines(".\\version.txt");

            if (quicklaunch == null)
            {             
                if (lines.Length > 1)
                {
                    quicklaunch = lines.Last();
                } else
                {
                    quicklaunch = "";
                }
                    
            }        

            if (version == null)
            {
                version = lines.First();
            }

            System.IO.File.WriteAllLines(".\\version.txt", new string[] { version, quicklaunch });
        }

        static string quickpath = null;
        public static void isoSelection()
        {
            string path = getQuickPath();

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
          
            updateVersionFile(null, path);
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
            if (!Directory.Exists(".\\User\\"))
            {
                Console.WriteLine("Local User Folder does not exist.");

                Directory.CreateDirectory(".\\User\\");

                try
                {
                    if (Directory.Exists(DE))
                    {
                        Console.WriteLine("Copying Dolphin Emulator files to local folder.");

                        foreach (string dirPath in Directory.GetDirectories(DE + "Config\\Profiles", "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(DE, ".\\User\\"));

                        foreach (string newPath in Directory.GetFiles(DE + "Config\\Profiles", "*.*",
                            SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(DE, ".\\User\\"), true);
                        }

                        string[] paths = new string[] {
                                //"Config\\Dolphin.ini",
                                "Config\\GFX.ini",
                                "Config\\Hotkeys.ini",
                                "Config\\UI.ini",
                                "Config\\WiimoteNew.ini",
                                "Load\\Titles.txt",
                                "Wii\\title\\00010000\\52334d45",
                                "Wii\\title\\00010000\\52334d50",
                                "Wii\\shared1\\00000000.app",
                                "Wii\\shared1\\00000001.app",
                                "Wii\\shared1\\00000002.app",
                                "Wii\\shared1\\content.map",
                                "Wii\\shared2\\menu\\FaceLib",
                                "Wii\\shared2\\sys\\net\\02\\config.dat",
                                "Wii\\shared2\\sys\\SYSCONF",
                                "Wii\\shared2\\wc24",
                                "Wii\\sys\\cert.sys",
                                "Wii\\sys\\uid.sys",
                                "Wii\\ticket\\00010002\\48414341.tik",
                                "Wii\\title\\00000001\\00000002",
                                "Wii\\title\\00010002\\48414341",
                                "GameSettings\\R3ME01.ini",
                                "GameSettings\\R3MP01.ini"
                    };

                        foreach (string path in paths)
                        {
                            string p = DE + path;
                            string c = ".\\User\\" + path;
                            string dir = c.Substring(0, c.LastIndexOf(Path.DirectorySeparatorChar));

                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                                
                            if (File.Exists(p))
                            {
                                File.Copy(p, c, true);
                            }                         
                        }

                        string[] lines = File.ReadAllLines(DE+ "Config\\Dolphin.ini");
                        List<String> newconfig = new List<String>();

                        foreach (string line in lines) 
                        {
                            if (line.Contains("Dolphin Emulator"))
                            {
                                string[] split = line.Split(new[] { "Dolphin Emulator" }, StringSplitOptions.None);
                                string newpath = "./User" + split[1];

                                string[] var = line.Split(new[] { " = " }, StringSplitOptions.None);

                                newconfig.Add(var[0] + " = " + newpath);
                                
                                continue;
                            }

                            newconfig.Add(line);
                         }

                        File.WriteAllLines(".\\User\\Config\\Dolphin.ini", newconfig);

                        DE = ".\\User\\";

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

                            string[] oldwiimotenew = File.ReadAllLines(".\\User\\Config\\WiimoteNew.ini");
                            List<string> newwiimote = new List<string>();

                            newwiimote.AddRange(oldwiimotenew);
                            newwiimote.Insert(2, primesettings.Substring(0, primesettings.Length-1));

                            File.WriteAllLines(".\\User\\Config\\WiimoteNew.ini", newwiimote.ToArray());
                        }                     
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine(e.InnerException);
                    Console.ReadLine();
                }
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

        public static void cutProfiles()
        {
            string docspath = DE + @"Config\Profiles\Wiimote\";
            if (Directory.Exists(".\\Profiles\\"))
            {
                string[] files = Directory.GetFiles(".\\Profiles\\");

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
            } else
            {
                Boolean setProfile = false;

                if (!File.Exists(docspath+"Metroid Prime Trilogy v1.ini"))
                {
                    File.WriteAllBytes(docspath + "Metroid Prime Trilogy v1.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.Metroid_Prime_Trilogy_v1));
                    File.WriteAllBytes(".\\Metroid Prime Trilogy v1 Manual.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.Metroid_Prime_Trilogy_v1_Manual));

                    File.WriteAllBytes(DE + @"Config\WiimoteNew.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.WiimoteNew));
                    setProfile = true;
                }

                if (!File.Exists(docspath + "Metroid Prime Trilogy v2.ini"))
                {
                    File.WriteAllBytes(docspath + "Metroid Prime Trilogy v2.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.Metroid_Prime_Trilogy_v2));
                    File.WriteAllBytes(".\\Metroid Prime Trilogy v2 Manual.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.Metroid_Prime_Trilogy_v2_Manual));

                    if (!setProfile)
                        File.WriteAllBytes(DE + @"Config\WiimoteNew.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.WiimoteNew));
                }

                if (!File.Exists(docspath + "Mendelli.ini"))
                {
                    File.WriteAllBytes(docspath + "Mendelli.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.Mendelli));
                    File.WriteAllBytes(".\\Mendelli Manual.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.Mendelli));
                   
                    if (!setProfile)
                        File.WriteAllBytes(DE + @"Config\WiimoteNew.ini", Encoding.ASCII.GetBytes(PrimeHack_Updater.Properties.Resources.WiimoteNew));
                }
            }
        }

        public static string getQuickPath()
        {
            if (File.Exists(".\\version.txt"))
            {
                var lines = System.IO.File.ReadAllLines(".\\version.txt");

                if (lines.Length > 1)
                {
                    return lines.Last();
                }
            }

            return "";
        }

        public static string getVersion()
        {
            if (File.Exists(".\\version.txt"))
            {
                try
                {
                    return System.IO.File.ReadLines(".\\version.txt").First();
                } catch (Exception)
                {
                    return "-1";
                }
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
