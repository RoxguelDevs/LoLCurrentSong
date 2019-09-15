using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoLCurrentSong.Modules
{
    class ExternalProvider
    {
        private static RegistryKey rkChromeHost = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Google\\Chrome\\NativeMessagingHosts", true);
        private const string _StatusFileName = "status.tmp";
        public ExternalProvider()
        {
            
        }

        public static void RegisterHost()
        {
            string CurrentDirectory = Path.GetDirectoryName((typeof(Config)).Assembly.Location);
            rkChromeHost.CreateSubKey("com.roxguel.lol_current_song");
            rkChromeHost.OpenSubKey("com.roxguel.lol_current_song", true)
                .SetValue("", Path.Combine(CurrentDirectory, "external-manifest.json"));
        }

        public static string ReadStatusFile()
        {
            string CurrentDirectory = Path.GetDirectoryName((typeof(Config)).Assembly.Location);
            string filename = Path.Combine(CurrentDirectory, _StatusFileName);
            if (!File.Exists(filename))
            {
                return "";
            }
            string status = File.ReadAllText(filename);
            File.Delete(filename);
            return status;
        }

        public static void WriteStatusFile(string status)
        {
            string CurrentDirectory = Path.GetDirectoryName((typeof(Config)).Assembly.Location);
            File.WriteAllText(Path.Combine(CurrentDirectory, _StatusFileName), status);
            Log.Notice("External", "New status (pending): " + status);
        }
    }
}
