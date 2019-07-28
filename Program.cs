using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LoLCurrentSong.Modules;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace LoLCurrentSong
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Config.LoadConfig();
            Log.Notice("Program", "Program is running");
            App.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            Application.Run();
        }
    }

    static class App
    {
        private const int LIMIT_STATUS = 25;
        private const int INTERVAL_UPDATE = 15;

        private static bool Started = false;
        private static bool FirstTime = true;
        private static bool Waiting = false;
        private static bool Stopping = false;
        private static bool Aborting = false;
        private static string InitStatus = null;
        private static string LastSong = null;

        public static void Start()
        {
            Log.Debug("App", "App is starting");
            Started = true;
            FirstTime = true;
            Waiting = false;
            Stopping = false;
            Aborting = false;
            InitStatus = null;
            LastSong = null;
            Task t = Task.Run(Run);
        }

        public static bool IsStarted() => Started;

        public static void Stop()
        {
            Log.Debug("App", "App is stopping");
            Started = false;
            Stopping = true;
            Waiting = false;
        }

        public static void Abort()
        {
            Log.Debug("App", "App is aborting");
            Started = false;
            Stopping = false;
            Waiting = false;
            Aborting = true;
        }

        private static async Task Run()
        {
            var lcu = new LCUProvider();
            while (Started || Stopping)
            {
                int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (FirstTime || Stopping || (unixTimestamp % INTERVAL_UPDATE == 0))
                {
                    if (FirstTime || Stopping || !Waiting)
                    {
                        try
                        {
                            if (FirstTime || InitStatus == null)
                            {
                                string res = await lcu.StatusMessage();
                                var json = JObject.Parse(res);
                                InitStatus = (string)json["statusMessage"];
                            }
                            await UpdateStatus(lcu);
                        }
                        catch (Newtonsoft.Json.JsonReaderException err)
                        {
                            Log.Notice("App", "JsonReaderException: " + err.Message);
                        }
                        catch(Exception err)
                        {
                            Log.Error(err.Message);
                        }
                        if (!Aborting)
                        {
                            Waiting = true;
                            FirstTime = false;
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Waiting = false;
                    }
                }
            }
        }

        private static async Task UpdateStatus(LCUProvider lcu)
        {
            if (Stopping)
            {
                Log.Verborse("App", "Updating LoL status with initial message");
                await lcu.StatusMessage(InitStatus);
                Abort();
                return;
            }

            Log.Verborse("App", "Updating LoL status");

            string song = SpotifyProvider.Song();

            if (song.Length > (LIMIT_STATUS - 2))
            {
                song = SpotifyProvider.Track();
            }
            if (song.Length > (LIMIT_STATUS - 2))
            {
                song = song.Substring(0, LIMIT_STATUS - 3) + "…";
            }
            else if (song.Length > (LIMIT_STATUS - 2))
            {
                song = song.Substring(0, LIMIT_STATUS - 2);
            }

            if (song.Equals(""))
            {
                Log.Verborse("App", "Updating LoL status with initial message. Spotify is close or stopped?");
                await lcu.StatusMessage(InitStatus);
            }
            else if (LastSong == null || !LastSong.Equals(song))
            {
                Log.Verborse("App", "Updating LoL status with " + song);
                await lcu.StatusMessage(string.Format("🎶 {0}", song));
                LastSong = song;
            }
            else
            {
                Log.Verborse("App", "LoL status not update");
            }
        }
    }
}
