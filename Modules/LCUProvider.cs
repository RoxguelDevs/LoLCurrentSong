using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoLCurrentSong.Modules
{
    class LCUProvider
    {
        private const string HTTP_METHOD_DELETE = "DELETE";
        private const string HTTP_METHOD_GET = "GET";
        private const string HTTP_METHOD_POST = "POST";
        private const string HTTP_METHOD_PUT = "PUT";

        private string EndPoint = "https://riot:{0}@127.0.0.1:{1}{2}";
        private string IdAccount = null;
        private string Port = null;
        private string Token = null;
        private string Username = null;
        private int Attempts = 0;
        private int LimitAttempts = 5;

        public LCUProvider()
        {

        }

        public async Task CheckSession()
        {
            if (Token == null || Port == null)
            {
                LoadSession();
                Attempts++;
                if (Attempts >= LimitAttempts)
                {
                    throw new Exception("Limit of attempts exceeded. LCU is Open?");
                }
                await CheckSession();
            }
            else
            {
                Attempts = 0;
                string res = await Request("/lol-login/v1/session");
                var json = JObject.Parse(res);
                if ((bool)json["connected"])
                {
                    IdAccount = (string)json["accountId"];
                    Username = (string)json["username"];
                }
                else
                {
                    throw new Exception("Wrong session");
                }
            }
        }

        public void LoadSession()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "wmic";
            cmd.StartInfo.Arguments = "process where \"name ='LeagueClientUx.exe'\" get commandline /format:list";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            string stdOut = cmd.StandardOutput.ReadToEnd();

            cmd.WaitForExit();

            foreach (string el in stdOut.Split(' '))
            {
                if (el.StartsWith("\"--app-port="))
                {
                    Port = el.Replace("\"", "").Replace("--app-port=", "");
                }
                else if (el.StartsWith("\"--remoting-auth-token="))
                {
                    Token = el.Replace("\"", "").Replace("--remoting-auth-token=", "");
                }
            }

            if (Attempts < 2 && (Token == null || Port == null))
            {
                Log.Notice("LCU is open?");
            }
        }

        public async Task<string> StatusMessage()
        {
            string message = "";
            try
            {
                await CheckSession();
                message = await Request("/lol-chat/v1/me");
            }
            catch (Exception err)
            {
                if (Attempts < 2)
                {
                    Log.Notice("LCUProvider", err.Message);
                }
            }
            return message;
        }
        public async Task StatusMessage(string msg)
        {
            try
            {
                await CheckSession();
                string res = await Request("/lol-chat/v1/me", HTTP_METHOD_PUT, new Dictionary<string, string> { { "statusMessage", msg } });
            }
            catch (Exception err)
            {
                if (Attempts < 2)
                {
                    Log.Notice("LCUProvider", err.Message);
                }
            }
        }

        private async Task<string> Request(string path)
        {
            return await Request(path, HTTP_METHOD_GET);
        }
        private async Task<string> Request(string path, string method)
        {
            return await Request(path, HTTP_METHOD_GET, new Dictionary<string, string> { });
        }
        private async Task<string> Request(string path, string method, Dictionary<string, string> values)
        {
            string url = string.Format(EndPoint, Token, Port, path);

            HttpClientHandler handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential("riot", Token),
                ServerCertificateCustomValidationCallback = (se, cert, chain, sslerror) => { return true; },
            };
            HttpClient client = new HttpClient(handler);
            StringContent content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;

            switch (method)
            {
                case HTTP_METHOD_GET:
                    response = await client.GetAsync(url);
                    break;
                case HTTP_METHOD_POST:
                    response = await client.PostAsync(url, content);
                    break;
                case HTTP_METHOD_PUT:
                    response = await client.PutAsync(url, content);
                    break;
                case HTTP_METHOD_DELETE:
                    response = await client.DeleteAsync(url);
                    break;
                default:
                    throw new Exception("Method not allow");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
