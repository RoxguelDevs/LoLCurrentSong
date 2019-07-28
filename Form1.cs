using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LoLCurrentSong.Modules;
using Microsoft.Win32;

namespace LoLCurrentSong
{
    public partial class Form1 : Form
    {
        private RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public Form1()
        {
            this.SetVisibleCore(false);
            InitializeComponent();

            menuItemToogleRun.Checked = App.IsStarted();

            trayIcon.Icon = this.Icon;
            trayIcon.Text = Config.AppName;
            trayIcon.Visible = true;

            menuItemStartup.Checked = rkApp.GetValue(Config.AppName) != null;
        }

        private void MenuItemCloseApp_Click(object sender, EventArgs e)
        {
            Log.Verborse("Form1", "Request close app");
            App.Stop();
            this.Close();
            this.Dispose();
            Application.Exit();
        }

        private void MenuItemToogleRun_Click(object sender, EventArgs e)
        {
            if (menuItemToogleRun.Checked)
            {
                Log.Verborse("Form1", "Request stop app");
                App.Stop();
            }
            else
            {
                Log.Verborse("Form1", "Request start app");
                App.Start();
            }
            menuItemToogleRun.Checked = App.IsStarted();
        }

        private void MenuItemStartup_Click(object sender, EventArgs e)
        {
            if (menuItemStartup.Checked)
            {
                Log.Verborse("Form1", "Request not startup with Windows");
                rkApp.DeleteValue(Config.AppName);
            }
            else
            {
                Log.Verborse("Form1", "Request startup with Windows");
                rkApp.SetValue(Config.AppName, Application.ExecutablePath);
            }
            menuItemStartup.Checked = rkApp.GetValue(Config.AppName) != null;
        }
    }
}
