using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace SSDataUploader
{
    public class Log
    {
        private static string lpath;

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string readValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, lpath);
            return temp.ToString();
        }
        public static void Info(string user, string info, string txt)
        {
            string LOG_WPATH = Properties.Settings.Default.LogPath;

            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ";
            string curday = DateTime.Now.Day.ToString();
            string curmon = DateTime.Now.Month.ToString();
            if (curday.Length == 1)
            {
                curday = "0" + curday;
            }
            if (curmon.Length == 1)
            {
                curmon = "0" + curmon;
            }
            string logTime = DateTime.Now.Year.ToString() + "-" + curmon + "-" + curday + "-";
            string chk = LOG_WPATH + "\\" + logTime + user + ".txt";
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(chk));
            if (!File.Exists(chk))
            {
                StreamWriter sw = new StreamWriter(chk, true);
                sw.WriteLine(sLogFormat + " " + info + " => " + txt);
                sw.Flush();
                sw.Close();
            }
            else
            {
                StreamWriter sw = File.AppendText(chk);
                sw.WriteLine(sLogFormat + " " + info + " => " + txt);
                sw.Flush();
                sw.Close();
            }
        }
        public static void error(string user, string info, string txt)
        {
            string LOG_WPATH = Properties.Settings.Default.LogPath;

            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ";
            string curday = DateTime.Now.Day.ToString();
            string curmon = DateTime.Now.Month.ToString();
            if (curday.Length == 1)
            {
                curday = "0" + curday;
            }
            if (curmon.Length == 1)
            {
                curmon = "0" + curmon;
            }
            string logTime = DateTime.Now.Year.ToString() + "-" + curmon + "-" + curday + "-";
            string chk = LOG_WPATH + "\\" + logTime + user + ".err";
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(chk));
            if (!File.Exists(chk))
            {
                StreamWriter sw = new StreamWriter(chk, true);
                sw.WriteLine(sLogFormat + " " + info + " => " + txt);
                sw.Flush();
                sw.Close();
            }
            else
            {
                StreamWriter sw = File.AppendText(chk);
                sw.WriteLine(sLogFormat + " " + info + " => " + txt);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
