namespace LoLCurrentSong
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.ctxMenuTrayIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemTitle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemToogleRun = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStartup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCloseApp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeeLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuTrayIcon.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.ctxMenuTrayIcon;
            this.trayIcon.Text = "notifyIcon1";
            this.trayIcon.Visible = true;
            // 
            // ctxMenuTrayIcon
            // 
            this.ctxMenuTrayIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemTitle,
            this.toolStripSeparator1,
            this.menuItemToogleRun,
            this.menuItemStartup,
            this.menuItemSeeLogs,
            this.menuItemCloseApp});
            this.ctxMenuTrayIcon.Name = "ctxMenuTrayIcon";
            this.ctxMenuTrayIcon.Size = new System.Drawing.Size(182, 142);
            // 
            // menuItemTitle
            // 
            this.menuItemTitle.Enabled = false;
            this.menuItemTitle.Name = "menuItemTitle";
            this.menuItemTitle.Size = new System.Drawing.Size(181, 22);
            this.menuItemTitle.Text = "LoLCurrentSong";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // menuItemToogleRun
            // 
            this.menuItemToogleRun.Name = "menuItemToogleRun";
            this.menuItemToogleRun.Size = new System.Drawing.Size(181, 22);
            this.menuItemToogleRun.Text = "Sincronizar estado";
            this.menuItemToogleRun.Click += new System.EventHandler(this.MenuItemToogleRun_Click);
            // 
            // menuItemStartup
            // 
            this.menuItemStartup.Name = "menuItemStartup";
            this.menuItemStartup.Size = new System.Drawing.Size(181, 22);
            this.menuItemStartup.Text = "Iniciar con Windows";
            this.menuItemStartup.Click += new System.EventHandler(this.MenuItemStartup_Click);
            // 
            // menuItemCloseApp
            // 
            this.menuItemCloseApp.Name = "menuItemCloseApp";
            this.menuItemCloseApp.Size = new System.Drawing.Size(181, 22);
            this.menuItemCloseApp.Text = "Cerrar";
            this.menuItemCloseApp.Click += new System.EventHandler(this.MenuItemCloseApp_Click);
            // 
            // menuItemSeeLogs
            // 
            this.menuItemSeeLogs.Name = "menuItemSeeLogs";
            this.menuItemSeeLogs.Size = new System.Drawing.Size(181, 22);
            this.menuItemSeeLogs.Text = "Ver registros de hoy";
            this.menuItemSeeLogs.Click += new System.EventHandler(this.MenuItemSeeLogs_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.ctxMenuTrayIcon.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip ctxMenuTrayIcon;
        private System.Windows.Forms.ToolStripMenuItem menuItemCloseApp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemTitle;
        private System.Windows.Forms.ToolStripMenuItem menuItemToogleRun;
        private System.Windows.Forms.ToolStripMenuItem menuItemStartup;
        private System.Windows.Forms.ToolStripMenuItem menuItemSeeLogs;
    }
}

