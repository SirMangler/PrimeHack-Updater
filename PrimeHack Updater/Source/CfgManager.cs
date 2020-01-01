using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeHack_Updater
{
    class CfgManager
    {
        string version = "";
        string isopath = "";
        bool mainbranch = true;

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

            string[] lines = new string[3];
            lines[0] = "version="+version;
            lines[1] = "isopath=" + isopath;
            lines[2] = "mainbranch=" + mainbranch;

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
