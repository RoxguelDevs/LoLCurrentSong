using System;
using System.IO;

namespace LoLCurrentSong.Modules
{
    static class Config
    {
        public static string AppName = "LoLCurrentSong";

        private static string _CurrentDirectory;
        private static readonly string _File = "config.ini";

        public static void LoadConfig()
        {
            LoadDefault();

            string[] configLines = new string[] { };
            try
            {
                string dirConfig = Path.Combine(_CurrentDirectory, _File);
                configLines = File.ReadAllLines(dirConfig);
            }
            catch (FileNotFoundException)
            {
                Log.Notice(String.Format("Config file undefined ({0})", _File));
            }

            foreach (string line in configLines)
            {
                // Check is blank line or comment
                if (line.Length == 0 || line.IndexOf('[') == 0 || line.IndexOf('#') == 0 || line.IndexOf(';') == 0)
                {
                    continue;
                }

                // Get ID and value
                string[] configLine = line.Split('=');
                if (configLine.Length == 1)
                {
                    continue;
                }

                // Check is null
                if (configLine[1].IndexOf('[') == 0)
                {
                    configLine[1] = "(null)";
                }
                // Check is a string
                else if (configLine[1].IndexOf('"') == 0 && configLine[1].IndexOf('"') == (configLine[1].Length - 1))
                {
                    configLine[1] = configLine[1].Substring(1, configLine[1].Length - 2);
                }
                // Check is boolean or null
                else if (configLine[1].ToLower().Equals("false") || configLine[1].ToLower().Equals("true") || configLine[1].ToLower().Equals("null"))
                {
                    configLine[1] = "(" + configLine[1] + ")";
                }

                try
                {
                    // Assign env variables
                    switch (configLine[0].Trim().ToUpper())
                    {
                        case "LOG_DIR": Log.LogDir = configLine[1].Trim(); break;
                        case "LOG_LEVEL": Log.LogLevel = int.Parse(configLine[1].Trim()); break;
                    }
                }
                catch (Exception)
                {
                    Log.Notice(String.Format("Error assign config parameter {0} as {1}", configLine[0], configLine[1]));
                }
            }
        }

        public static void LoadDefault()
        {
            _CurrentDirectory = Path.GetDirectoryName((typeof(Config)).Assembly.Location);
            Log.LogDir = Path.Combine(_CurrentDirectory, "log");
            Log.LogLevel = 3; // Log.WARNING
        }
    }
}
