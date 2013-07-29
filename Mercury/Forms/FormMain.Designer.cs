namespace Mercury.Forms
{
	partial class FormMain
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.sideMenu = new DotNetApi.Windows.Controls.SideMenu();
			this.sideTreeViewSettings = new DotNetApi.Windows.Controls.SideTreeView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.sideTreeViewBrowser = new DotNetApi.Windows.Controls.SideTreeView();
			this.sideMenuItemBrowser = new DotNetApi.Windows.Controls.SideMenuItem();
			this.sideMenuItemSettings = new DotNetApi.Windows.Controls.SideMenuItem();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.labelNotAvailable = new System.Windows.Forms.Label();
			this.controlSettings = new Mercury.Controls.ControlSettings();
			this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
			this.toolStripContainer.ContentPanel.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.sideMenu.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer
			// 
			// 
			// toolStripContainer.BottomToolStripPanel
			// 
			this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
			// 
			// toolStripContainer.ContentPanel
			// 
			this.toolStripContainer.ContentPanel.Controls.Add(this.splitContainer);
			this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(684, 416);
			this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer.Name = "toolStripContainer";
			this.toolStripContainer.Size = new System.Drawing.Size(684, 462);
			this.toolStripContainer.TabIndex = 0;
			this.toolStripContainer.Text = "toolStripContainer1";
			// 
			// toolStripContainer.TopToolStripPanel
			// 
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
			// 
			// statusStrip
			// 
			this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.statusStrip.Location = new System.Drawing.Point(0, 0);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(684, 22);
			this.statusStrip.TabIndex = 0;
			// 
			// splitContainer
			// 
			this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.sideMenu);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.labelNotAvailable);
			this.splitContainer.Panel2.Controls.Add(this.controlSettings);
			this.splitContainer.Size = new System.Drawing.Size(684, 416);
			this.splitContainer.SplitterDistance = 228;
			this.splitContainer.TabIndex = 0;
			// 
			// sideMenu
			// 
			this.sideMenu.Controls.Add(this.sideTreeViewSettings);
			this.sideMenu.Controls.Add(this.sideTreeViewBrowser);
			this.sideMenu.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sideMenu.ItemHeight = 48;
			this.sideMenu.Items.AddRange(new DotNetApi.Windows.Controls.SideMenuItem[] {
            this.sideMenuItemBrowser,
            this.sideMenuItemSettings});
			this.sideMenu.Location = new System.Drawing.Point(0, 0);
			this.sideMenu.MinimizedItemWidth = 25;
			this.sideMenu.MinimumPanelHeight = 50;
			this.sideMenu.Name = "sideMenu";
			this.sideMenu.Padding = new System.Windows.Forms.Padding(0, 28, 0, 152);
			this.sideMenu.SelectedIndex = 0;
			this.sideMenu.SelectedItem = this.sideMenuItemBrowser;
			this.sideMenu.Size = new System.Drawing.Size(226, 414);
			this.sideMenu.TabIndex = 0;
			this.sideMenu.VisibleItems = 2;
			// 
			// sideTreeViewSettings
			// 
			this.sideTreeViewSettings.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.sideTreeViewSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sideTreeViewSettings.FullRowSelect = true;
			this.sideTreeViewSettings.HideSelection = false;
			this.sideTreeViewSettings.ImageIndex = 0;
			this.sideTreeViewSettings.ImageList = this.imageList;
			this.sideTreeViewSettings.ItemHeight = 20;
			this.sideTreeViewSettings.Location = new System.Drawing.Point(0, 28);
			this.sideTreeViewSettings.Name = "sideTreeViewSettings";
			this.sideTreeViewSettings.SelectedImageIndex = 0;
			this.sideTreeViewSettings.ShowLines = false;
			this.sideTreeViewSettings.ShowRootLines = false;
			this.sideTreeViewSettings.Size = new System.Drawing.Size(226, 234);
			this.sideTreeViewSettings.TabIndex = 1;
			this.sideTreeViewSettings.Visible = false;
			this.sideTreeViewSettings.ControlChanged += new DotNetApi.Windows.Controls.SideTreeViewControlChangedEventHandler(this.OnControlChanged);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "GlobeStar");
			this.imageList.Images.SetKeyName(1, "Settings");
			// 
			// sideTreeViewBrowser
			// 
			this.sideTreeViewBrowser.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.sideTreeViewBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sideTreeViewBrowser.FullRowSelect = true;
			this.sideTreeViewBrowser.HideSelection = false;
			this.sideTreeViewBrowser.ImageIndex = 0;
			this.sideTreeViewBrowser.ImageList = this.imageList;
			this.sideTreeViewBrowser.ItemHeight = 20;
			this.sideTreeViewBrowser.Location = new System.Drawing.Point(0, 28);
			this.sideTreeViewBrowser.Name = "sideTreeViewBrowser";
			this.sideTreeViewBrowser.SelectedImageIndex = 0;
			this.sideTreeViewBrowser.ShowLines = false;
			this.sideTreeViewBrowser.ShowRootLines = false;
			this.sideTreeViewBrowser.Size = new System.Drawing.Size(226, 234);
			this.sideTreeViewBrowser.TabIndex = 0;
			this.sideTreeViewBrowser.Visible = false;
			this.sideTreeViewBrowser.ControlChanged += new DotNetApi.Windows.Controls.SideTreeViewControlChangedEventHandler(this.OnControlChanged);
			// 
			// sideMenuItemBrowser
			// 
			this.sideMenuItemBrowser.Control = this.sideTreeViewBrowser;
			this.sideMenuItemBrowser.ImageLarge = global::Mercury.Resources.GlobeBrowse_32;
			this.sideMenuItemBrowser.ImageSmall = global::Mercury.Resources.GlobeBrowse_16;
			this.sideMenuItemBrowser.Index = -1;
			this.sideMenuItemBrowser.Text = "Browser";
			// 
			// sideMenuItemSettings
			// 
			this.sideMenuItemSettings.Control = this.sideTreeViewSettings;
			this.sideMenuItemSettings.ImageLarge = global::Mercury.Resources.ConfigurationSettings_32;
			this.sideMenuItemSettings.ImageSmall = global::Mercury.Resources.ConfigurationSettings_16;
			this.sideMenuItemSettings.Index = -1;
			this.sideMenuItemSettings.Text = "Settings";
			// 
			// menuStrip
			// 
			this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.menuItemHelp});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(684, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// menuItemFile
			// 
			this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemExit});
			this.menuItemFile.Name = "menuItemFile";
			this.menuItemFile.Size = new System.Drawing.Size(37, 20);
			this.menuItemFile.Text = "&File";
			// 
			// menuItemExit
			// 
			this.menuItemExit.Name = "menuItemExit";
			this.menuItemExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.menuItemExit.Size = new System.Drawing.Size(152, 22);
			this.menuItemExit.Text = "E&xit";
			this.menuItemExit.Click += new System.EventHandler(this.OnExit);
			// 
			// menuItemHelp
			// 
			this.menuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAbout});
			this.menuItemHelp.Name = "menuItemHelp";
			this.menuItemHelp.Size = new System.Drawing.Size(44, 20);
			this.menuItemHelp.Text = "&Help";
			// 
			// menuItemAbout
			// 
			this.menuItemAbout.Name = "menuItemAbout";
			this.menuItemAbout.Size = new System.Drawing.Size(116, 22);
			this.menuItemAbout.Text = "&About...";
			// 
			// labelNotAvailable
			// 
			this.labelNotAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelNotAvailable.Location = new System.Drawing.Point(0, 0);
			this.labelNotAvailable.Name = "labelNotAvailable";
			this.labelNotAvailable.Size = new System.Drawing.Size(450, 414);
			this.labelNotAvailable.TabIndex = 1;
			this.labelNotAvailable.Text = "Feature not available";
			this.labelNotAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// controlSettings
			// 
			this.controlSettings.Config = null;
			this.controlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlSettings.Location = new System.Drawing.Point(0, 0);
			this.controlSettings.Name = "controlSettings";
			this.controlSettings.Padding = new System.Windows.Forms.Padding(4);
			this.controlSettings.Size = new System.Drawing.Size(450, 414);
			this.controlSettings.TabIndex = 0;
			this.controlSettings.Visible = false;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 462);
			this.Controls.Add(this.toolStripContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.Name = "FormMain";
			this.Text = "Mercury Console";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.BottomToolStripPanel.PerformLayout();
			this.toolStripContainer.ContentPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.sideMenu.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem menuItemFile;
		private System.Windows.Forms.ToolStripMenuItem menuItemExit;
		private System.Windows.Forms.ToolStripMenuItem menuItemHelp;
		private System.Windows.Forms.ToolStripMenuItem menuItemAbout;
		private System.Windows.Forms.SplitContainer splitContainer;
		private DotNetApi.Windows.Controls.SideMenu sideMenu;
		private DotNetApi.Windows.Controls.SideMenuItem sideMenuItemBrowser;
		private DotNetApi.Windows.Controls.SideTreeView sideTreeViewBrowser;
		private System.Windows.Forms.ImageList imageList;
		private DotNetApi.Windows.Controls.SideMenuItem sideMenuItemSettings;
		private DotNetApi.Windows.Controls.SideTreeView sideTreeViewSettings;
		private Controls.ControlSettings controlSettings;
		private System.Windows.Forms.Label labelNotAvailable;
	}
}

