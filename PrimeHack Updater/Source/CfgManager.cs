using System;
using System.IO;

namespace PrimeHack_Updater
{
    class CfgManager
    {
        string version = "";
        string isopath = "";
        bool mainbranch = true;
        bool immersive_mode = false;
         
        public CfgManager()
        {
            loadCfg();
        }

        public void loadCfg()
        {
            if (File.Exists("./updater.cfg"))
            {
                foreach (string line in File.ReadAllLines("./updater.cfg"))
                {
                    if (line.StartsWith("version="))
                    {
                        version = line.Replace("version=", "");
                        continue;
                    }

                    if (line.StartsWith("isopath="))
                    {
                        isopath = line.Replace("isopath=", "");
                        continue;
                    }

                    if (line.StartsWith("mainbranch="))
                    {
                        Boolean.TryParse(line.Replace("mainbranch=", ""), out mainbranch);
                        continue;
                    }

                    if (line.StartsWith("immersive_mode="))
                    {
                        Boolean.TryParse(line.Replace("immersive_mode=", ""), out immersive_mode);
                        continue;
                    }
                }
            } else
            {
                if (!Updater.WriteAccess(".\\"))
                    Updater.restartAsAdmin();

                File.Create("./updater.cfg");
            }
        }

        public string getISOPath()
        {
            if (!File.Exists("./updater.cfg"))
                loadCfg();

            return isopath;
        }

        public bool getImmersiveMode()
        {
            if (!File.Exists("./updater.cfg"))
                loadCfg();

            return immersive_mode;
        }

        public string getVersion()
        {
            if (!File.Exists("./updater.cfg"))
                loadCfg();

            return version;
        }

        public bool isMainBranch()
        {
            if (!File.Exists("./updater.cfg"))
                loadCfg();

            return mainbranch;
        }

        public void saveCfg()
        {
            if (!Updater.WriteAccess(".\\"))
                Updater.restartAsAdmin();

            if (!File.Exists("./updater.cfg"))
                loadCfg();

            string[] lines = new string[4];
            lines[0] = "version="+version;
            lines[1] = "isopath=" + isopath;
            lines[2] = "mainbranch=" + mainbranch;
            lines[3] = "immersive_mode=" + immersive_mode;

            File.WriteAllLines("./updater.cfg", lines);

            if (File.Exists("./version.txt"))
                File.Delete("./version.txt");
        }

        public void setVersion(string ver)
        {
            version = ver + (isMainBranch() ? "" : "I");
            saveCfg();
        }

        public void setISOPath(string path)
        {
            isopath = path;
            saveCfg();
        }

        public void setImmersiveMode(bool mode)
        {
            immersive_mode = mode;
            saveCfg();
        }

        public void setMainbranch(bool branch)
        {
            mainbranch = branch;
            saveCfg();
        }

        public bool isVersionsEqual(string remote)
        {
            if (version.Equals(remote + (isMainBranch() ? "" : "I")))
            {
                return true;
            }

            return false;
        }
    }
}
