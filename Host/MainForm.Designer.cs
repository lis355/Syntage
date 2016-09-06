namespace SimplyHost
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OpenFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.FMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bypassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PluginPanel = new System.Windows.Forms.Panel();
            this.programsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenFileDlg
            // 
            this.OpenFileDlg.Filter = "Plugins (*.dll)|*.dll|All Files (*.*)|*.*";
            // 
            // FMenu
            // 
            this.FMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.bypassToolStripMenuItem,
            this.programsToolStripMenuItem});
            this.FMenu.Location = new System.Drawing.Point(0, 0);
            this.FMenu.Name = "FMenu";
            this.FMenu.Size = new System.Drawing.Size(600, 24);
            this.FMenu.TabIndex = 8;
            this.FMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printParametersToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // printParametersToolStripMenuItem
            // 
            this.printParametersToolStripMenuItem.Name = "printParametersToolStripMenuItem";
            this.printParametersToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.printParametersToolStripMenuItem.Text = "Copy Parameters To Keyboard";
            this.printParametersToolStripMenuItem.Click += new System.EventHandler(this.printParametersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(231, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // bypassToolStripMenuItem
            // 
            this.bypassToolStripMenuItem.Name = "bypassToolStripMenuItem";
            this.bypassToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.bypassToolStripMenuItem.Text = "Bypass";
            this.bypassToolStripMenuItem.Click += new System.EventHandler(this.bypassToolStripMenuItem_Click);
            // 
            // PluginPanel
            // 
            this.PluginPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PluginPanel.Location = new System.Drawing.Point(0, 24);
            this.PluginPanel.Name = "PluginPanel";
            this.PluginPanel.Size = new System.Drawing.Size(600, 300);
            this.PluginPanel.TabIndex = 0;
            // 
            // programsToolStripMenuItem
            // 
            this.programsToolStripMenuItem.Name = "programsToolStripMenuItem";
            this.programsToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.programsToolStripMenuItem.Text = "Programs";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(600, 324);
            this.Controls.Add(this.PluginPanel);
            this.Controls.Add(this.FMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.FMenu;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VST.NET SimplyHost";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.FMenu.ResumeLayout(false);
            this.FMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.OpenFileDialog OpenFileDlg;
        public System.Windows.Forms.MenuStrip FMenu;
        public System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        public System.Windows.Forms.Panel PluginPanel;
        private System.Windows.Forms.ToolStripMenuItem bypassToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem printParametersToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        public System.Windows.Forms.ToolStripMenuItem programsToolStripMenuItem;
    }
}

