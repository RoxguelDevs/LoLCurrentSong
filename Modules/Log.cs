using System;
using System.IO;

namespace LoLCurrentSong.Modules
{
    class Log
    {

        public const int VERBORSE = 0;
        public const int DEBUG = 1;
        public const int NOTICE = 2;
        public const int WARNING = 3;
        public const int ERROR = 4;
        public const int FATAL = 5;

        public static string LogDir;
        public static int LogLevel = WARNING;

        private static readonly string uncatchedFile = @"C:\temp\LoLCurrentSong.log";

        public static void Verborse(string module, string msg = null)
        {
            if (msg is null)
            {
                Write(VERBORSE, module);
            }
            else
            {
                Write(VERBORSE, module, msg);
            }
        }
        public static void Debug(string module, string msg = null)
        {
            if (msg is null)
            {
                Write(DEBUG, module);
            }
            else
            {
                Write(DEBUG, module, msg);
            }
        }
        public static void Notice(string module, string msg = null)
        {
            if (msg is null)
            {
                Write(NOTICE, module);
            }
            else
            {
                Write(NOTICE, module, msg);
            }
        }
        public static void Warning(string module, string msg = null)
        {
            if (msg is null)
            {
                Write(WARNING, module);
            }
            else
            {
                Write(WARNING, module, msg);
            }
        }
        public static void Error(string module, string msg = null)
        {
            if (msg is null)
            {
                Write(ERROR, module);
            }
            else
            {
                Write(ERROR, module, msg);
            }
        }
        public static void Fatal(string module, string msg = null)
        {
            if (msg is null)
            {
                Write(FATAL, module, null);
            }
            else
            {
                Write(FATAL, module, msg);
            }
        }

        public static void Write(string msg)
        {
            try
            {
                string filename = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
                Directory.CreateDirectory(LogDir);

                var ws = System.IO.File.AppendText(uncatchedFile);
                ws.WriteLine("File: " + Path.Combine(LogDir, filename));
                ws.Close();

                StreamWriter sw = File.AppendText(Path.Combine(LogDir, filename));
                string text = String.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
                sw.WriteLine(text, 0, text.Length);
                sw.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                var ws = File.AppendText(uncatchedFile);
                ws.WriteLine("Error al escribir LOG: " + err.Message);
                ws.Close();
            }
        }
        public static void Write(int type, string msg)
        {
            Write(type, msg, null);
        }
        public static void Write(int type, string module, string msg)
        {
            if (LogLevel > type)
            {
                return;
            }
            if (msg == null)
            {
                msg = module;
            }
            else
            {
                msg = String.Format("[{0}] {1}", module, msg);
            }
            string typeText = _TextTypeLog(type);
            if (typeText == null)
            {
                Write(msg);
            }
            else
            {
                Write(String.Format("[{0}] {1}", typeText, msg));
            }
        }

        private static string _TextTypeLog(int type)
        {
            switch (type)
            {
                default:
                    return null;
                case VERBORSE:
                    return "Verborse";
                case DEBUG:
                    return "Debug";
                case NOTICE:
                    return "Notice";
                case WARNING:
                    return "Warning";
                case ERROR:
                    return "Error";
                case FATAL:
                    return "Fatal Error";
            }
        }

    }
}
