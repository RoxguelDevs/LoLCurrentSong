using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using LoLCurrentSong.Modules;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace LoLCurrentSong
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Config.LoadConfig();
            
            if (HandlerArgs(args))
            {
                Log.Notice("Program CLI", "App stopping");
                return;
            }

            if (ReadExternalConnection())
            {
                Log.Notice("Program CLI", "App stopping");
                return;
            }

            Log.Notice("Program", "Program is running");
            App.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            Application.Run();
        }

        private static bool ReadExternalConnection()
        {
            Stream stdin = Console.OpenStandardInput();
            int length = 0;

            byte[] lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);

            char[] buffer = new char[length];
            using (StreamReader reader = new StreamReader(stdin))
            {
                while (reader.Peek() >= 0)
                {
                    reader.Read(buffer, 0, buffer.Length);
                }
            }

            string message = new string(buffer);
            if (message.Length > 0) // && message[0] == '"' && message[message.Length - 1] == '"'
            {
                message = message.Substring(1, message.Length - 2);
                message = message.Replace("\\\"", "\"");
                Log.Debug("ReadExternalConnection", message);
            }

            Regex re = new Regex("(?<=\")[^\"]*(?=\")|[^\" ]+");
            string[] args = re.Matches(message).Cast<Match>().Select(m => m.Value).ToArray();
            return HandlerArgs(args);
        }

        private static bool HandlerArgs(string[] args)
        {
            bool endApp = false;
            if (args.Length > 0)
            {
                Log.Notice("Program CLI", "App started as CLI with " + args.Length + " args");
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--set-status":
                            i++;
                            if (i > args.Length)
                            {
                                throw new Exception("New status not found");
                            }
                            string status = args[i];
                            endApp = true;
                            ExternalProvider.WriteStatusFile(status);
                            break;
                    }
                }
            }
            return endApp;
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
            ExternalProvider.RegisterHost();
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
                                if (res.Equals(""))
                                {
                                    Thread.Sleep(10 * 1000);
                                }
                                else
                                {
                                    var json = JObject.Parse(res);
                                    InitStatus = (string)json["statusMessage"];
                                    Log.Verborse("App", "Initial status: " + InitStatus);
                                    await UpdateStatus(lcu);
                                    if (!Aborting)
                                    {
                                        Waiting = true;
                                        FirstTime = false;
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                            else
                            {
                                await UpdateStatus(lcu);
                                if (!Aborting)
                                {
                                    Waiting = true;
                                    FirstTime = false;
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        catch (Newtonsoft.Json.JsonReaderException err)
                        {
                            Log.Notice("App", "JsonReaderException: " + err.Message);
                        }
                        catch(Exception err)
                        {
                            Log.Error(err.Message);
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

            string song = ExternalProvider.ReadStatusFile();
            if (song != "")
            {

                Log.Verborse("App", "Updating LoL status with External Provider");
            }
            else
            {
                song = SpotifyProvider.Song();
                if (song != "")
                {
                    Log.Verborse("App", "Updating LoL status with Spotify Provider");
                    if (song.Length > (LIMIT_STATUS - 2))
                    {
                        song = SpotifyProvider.Track();
                    }
                }
                else
                {
                    Log.Verborse("App", "Spotify is close or stopped?");
                }
            }

            
            if (song.Length > (LIMIT_STATUS - 2))
            {
                song = song.Substring(0, LIMIT_STATUS - 3) + "…";
            }
            else if (song.Length > (LIMIT_STATUS - 2))
            {
                song = song.Substring(0, LIMIT_STATUS - 2);
            }

            if (song.Equals("") || string.IsNullOrEmpty(song))
            {
                Log.Verborse("App", "Empty current song, updating LoL status with initial message");
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
