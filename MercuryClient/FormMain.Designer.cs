namespace Mercury
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.wizardControl = new Mercury.Windows.Controls.AeroWizard.WizardControl();
			this.wizardPageLocale = new Mercury.Windows.Controls.AeroWizard.WizardPage();
			this.comboBoxCountry = new System.Windows.Forms.ComboBox();
			this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
			this.labelCountry = new System.Windows.Forms.Label();
			this.labelLanguage = new System.Windows.Forms.Label();
			this.wizardPageRun = new Mercury.Windows.Controls.AeroWizard.WizardPage();
			this.labelProgress = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.labelInfo = new System.Windows.Forms.Label();
			this.wizardPageFinish = new Mercury.Windows.Controls.AeroWizard.WizardPage();
			this.labelFinish = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).BeginInit();
			this.wizardPageLocale.SuspendLayout();
			this.wizardPageRun.SuspendLayout();
			this.wizardPageFinish.SuspendLayout();
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			this.wizardControl.Culture = new System.Globalization.CultureInfo("en-US");
			this.wizardControl.Location = new System.Drawing.Point(0, 0);
			this.wizardControl.Name = "wizardControl";
			this.wizardControl.Pages.AddRange(new Mercury.Windows.Controls.AeroWizard.WizardPage[] {
            this.wizardPageLocale,
            this.wizardPageRun,
            this.wizardPageFinish});
			this.wizardControl.Size = new System.Drawing.Size(584, 412);
			this.wizardControl.TabIndex = 0;
			this.wizardControl.Cancelling += new System.ComponentModel.CancelEventHandler(this.OnCanceling);
			this.wizardControl.Finished += new System.EventHandler(this.OnFinished);
			// 
			// wizardPageLocale
			// 
			this.wizardPageLocale.AllowBack = false;
			this.wizardPageLocale.AllowNext = false;
			this.wizardPageLocale.Controls.Add(this.comboBoxCountry);
			this.wizardPageLocale.Controls.Add(this.comboBoxLanguage);
			this.wizardPageLocale.Controls.Add(this.labelCountry);
			this.wizardPageLocale.Controls.Add(this.labelLanguage);
			this.wizardPageLocale.HelpText = "For what is this information?";
			this.wizardPageLocale.Name = "wizardPageLocale";
			this.wizardPageLocale.NextPage = this.wizardPageRun;
			this.wizardPageLocale.Size = new System.Drawing.Size(537, 257);
			this.wizardPageLocale.TabIndex = 0;
			this.wizardPageLocale.Text = "Select your language and country";
			this.wizardPageLocale.HelpClicked += new System.EventHandler(this.OnLocaleHelp);
			// 
			// comboBoxCountry
			// 
			this.comboBoxCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCountry.FormattingEnabled = true;
			this.comboBoxCountry.Location = new System.Drawing.Point(80, 117);
			this.comboBoxCountry.Name = "comboBoxCountry";
			this.comboBoxCountry.Size = new System.Drawing.Size(315, 23);
			this.comboBoxCountry.TabIndex = 7;
			this.comboBoxCountry.SelectedIndexChanged += new System.EventHandler(this.OnCountryChanged);
			// 
			// comboBoxLanguage
			// 
			this.comboBoxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLanguage.FormattingEnabled = true;
			this.comboBoxLanguage.Location = new System.Drawing.Point(80, 88);
			this.comboBoxLanguage.Name = "comboBoxLanguage";
			this.comboBoxLanguage.Size = new System.Drawing.Size(315, 23);
			this.comboBoxLanguage.TabIndex = 6;
			this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.OnLanguageChanged);
			// 
			// labelCountry
			// 
			this.labelCountry.AutoSize = true;
			this.labelCountry.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelCountry.Location = new System.Drawing.Point(3, 120);
			this.labelCountry.Name = "labelCountry";
			this.labelCountry.Size = new System.Drawing.Size(53, 15);
			this.labelCountry.TabIndex = 3;
			this.labelCountry.Text = "&Country:";
			// 
			// labelLanguage
			// 
			this.labelLanguage.AutoSize = true;
			this.labelLanguage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelLanguage.Location = new System.Drawing.Point(3, 91);
			this.labelLanguage.Name = "labelLanguage";
			this.labelLanguage.Size = new System.Drawing.Size(62, 15);
			this.labelLanguage.TabIndex = 5;
			this.labelLanguage.Text = "&Language:";
			// 
			// wizardPageRun
			// 
			this.wizardPageRun.Controls.Add(this.labelProgress);
			this.wizardPageRun.Controls.Add(this.progressBar);
			this.wizardPageRun.Controls.Add(this.labelInfo);
			this.wizardPageRun.Name = "wizardPageRun";
			this.wizardPageRun.NextPage = this.wizardPageFinish;
			this.wizardPageRun.Size = new System.Drawing.Size(537, 257);
			this.wizardPageRun.TabIndex = 1;
			this.wizardPageRun.Text = "Measuring the Internet";
			this.wizardPageRun.TextNext = "&Start";
			this.wizardPageRun.Commit += new System.EventHandler<Mercury.Windows.Controls.AeroWizard.WizardPageConfirmEventArgs>(this.OnRunCommit);
			this.wizardPageRun.Initialize += new System.EventHandler<Mercury.Windows.Controls.AeroWizard.WizardPageInitEventArgs>(this.OnRunInitialize);
			// 
			// labelProgress
			// 
			this.labelProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelProgress.Location = new System.Drawing.Point(0, 157);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(537, 100);
			this.labelProgress.TabIndex = 3;
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(3, 103);
			this.progressBar.MarqueeAnimationSpeed = 20;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(531, 16);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.TabIndex = 2;
			this.progressBar.Visible = false;
			// 
			// labelInfo
			// 
			this.labelInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelInfo.Location = new System.Drawing.Point(0, 0);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(537, 100);
			this.labelInfo.TabIndex = 1;
			// 
			// wizardPageFinish
			// 
			this.wizardPageFinish.AllowCancel = false;
			this.wizardPageFinish.Controls.Add(this.labelFinish);
			this.wizardPageFinish.HelpText = "";
			this.wizardPageFinish.IsFinishPage = true;
			this.wizardPageFinish.Name = "wizardPageFinish";
			this.wizardPageFinish.ShowCancel = false;
			this.wizardPageFinish.Size = new System.Drawing.Size(537, 256);
			this.wizardPageFinish.TabIndex = 2;
			this.wizardPageFinish.Text = "Finished";
			// 
			// labelFinish
			// 
			this.labelFinish.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelFinish.Location = new System.Drawing.Point(0, 0);
			this.labelFinish.Name = "labelFinish";
			this.labelFinish.Size = new System.Drawing.Size(537, 100);
			this.labelFinish.TabIndex = 2;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "XML files (*.xml)|*.xml";
			this.openFileDialog.Title = "Open Locales File";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Resource files (*.resx)|*.resx";
			this.saveFileDialog.Title = "Save Resource File";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 412);
			this.Controls.Add(this.wizardControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this.Text = "Mercury Client";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).EndInit();
			this.wizardPageLocale.ResumeLayout(false);
			this.wizardPageLocale.PerformLayout();
			this.wizardPageRun.ResumeLayout(false);
			this.wizardPageFinish.ResumeLayout(false);
			this.wizardPageFinish.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Mercury.Windows.Controls.AeroWizard.WizardControl wizardControl;
		private Mercury.Windows.Controls.AeroWizard.WizardPage wizardPageLocale;
		private Mercury.Windows.Controls.AeroWizard.WizardPage wizardPageRun;
		private Mercury.Windows.Controls.AeroWizard.WizardPage wizardPageFinish;
		private System.Windows.Forms.Label labelCountry;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label labelLanguage;
		private System.Windows.Forms.ComboBox comboBoxLanguage;
		private System.Windows.Forms.ComboBox comboBoxCountry;
		private System.Windows.Forms.Label labelInfo;
		private System.Windows.Forms.Label labelProgress;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelFinish;

	}
}

