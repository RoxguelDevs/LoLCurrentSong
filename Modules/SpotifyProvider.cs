using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLCurrentSong.Modules
{
    class SpotifyProvider
    {
        private static readonly string Separator = " - ";
        public static string Song()
        {
            Process proc = Process.GetProcessesByName("Spotify").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            if (proc == null)
            {
                return "";
            }
            else if (string.Equals(proc.MainWindowTitle, "Spotify", StringComparison.InvariantCultureIgnoreCase))
            {
                Log.Debug("Spotify Provider", "Window title: " + proc.MainWindowTitle);
                return "";
            }
            else if (string.Equals(proc.MainWindowTitle, "Spotify Premium", StringComparison.InvariantCultureIgnoreCase))
            {
                Log.Debug("Spotify Provider", "Window title: " + proc.MainWindowTitle);
                return "";
            }
            Log.Debug("Spotify Provider", "Window title: " + proc.MainWindowTitle);
            return proc.MainWindowTitle;
        }

        public static string Artist()
        {
            string song = Song();
            return song.Substring(0, song.IndexOf(Separator)).Trim();
        }

        public static string Track()
        {
            string song = Song();
            return song.Substring(song.IndexOf(Separator) + Separator.Length).Trim();
        }
    }
}
