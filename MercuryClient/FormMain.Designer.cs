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
			if (disposing)
			{
				if (components != null)
				{
					this.components.Dispose();
				}
				this.waitAsync.Close();
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
			this.wizardControl = new Mercury.Windows.Controls.AeroWizard.WizardControl();
			this.wizardPageLocale = new Mercury.Windows.Controls.AeroWizard.WizardPage();
			this.pictureBoxCountry = new System.Windows.Forms.PictureBox();
			this.comboBoxCountry = new System.Windows.Forms.ComboBox();
			this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
			this.labelCountry = new System.Windows.Forms.Label();
			this.labelLanguage = new System.Windows.Forms.Label();
			this.wizardPageForm = new Mercury.Windows.Controls.AeroWizard.WizardPage();
			this.labelProviderExample = new System.Windows.Forms.Label();
			this.labelCity = new System.Windows.Forms.Label();
			this.textBoxCity = new System.Windows.Forms.TextBox();
			this.labelForm = new System.Windows.Forms.Label();
			this.textBoxProvider = new System.Windows.Forms.TextBox();
			this.labelProvider = new System.Windows.Forms.Label();
			this.wizardPageRun = new Mercury.Windows.Controls.AeroWizard.WizardPage();
			this.labelProgress = new System.Windows.Forms.Label();
			this.labelTime = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.labelInfo = new System.Windows.Forms.Label();
			this.wizardPageFinish = new Mercury.Windows.Controls.AeroWizard.WizardPage();
			this.labelFinish = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.timer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).BeginInit();
			this.wizardPageLocale.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCountry)).BeginInit();
			this.wizardPageForm.SuspendLayout();
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
            this.wizardPageForm,
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
			this.wizardPageLocale.Controls.Add(this.pictureBoxCountry);
			this.wizardPageLocale.Controls.Add(this.comboBoxCountry);
			this.wizardPageLocale.Controls.Add(this.comboBoxLanguage);
			this.wizardPageLocale.Controls.Add(this.labelCountry);
			this.wizardPageLocale.Controls.Add(this.labelLanguage);
			this.wizardPageLocale.HelpText = "For what is this information?";
			this.wizardPageLocale.Name = "wizardPageLocale";
			this.wizardPageLocale.NextPage = this.wizardPageForm;
			this.wizardPageLocale.Size = new System.Drawing.Size(537, 257);
			this.wizardPageLocale.TabIndex = 0;
			this.wizardPageLocale.Text = "Select your language and country";
			this.wizardPageLocale.HelpClicked += new System.EventHandler(this.OnHelp);
			// 
			// pictureBoxCountry
			// 
			this.pictureBoxCountry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBoxCountry.Location = new System.Drawing.Point(403, 119);
			this.pictureBoxCountry.Name = "pictureBoxCountry";
			this.pictureBoxCountry.Size = new System.Drawing.Size(36, 36);
			this.pictureBoxCountry.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxCountry.TabIndex = 1;
			this.pictureBoxCountry.TabStop = false;
			this.pictureBoxCountry.Visible = false;
			// 
			// comboBoxCountry
			// 
			this.comboBoxCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCountry.FormattingEnabled = true;
			this.comboBoxCountry.Location = new System.Drawing.Point(100, 125);
			this.comboBoxCountry.Name = "comboBoxCountry";
			this.comboBoxCountry.Size = new System.Drawing.Size(300, 23);
			this.comboBoxCountry.TabIndex = 3;
			this.comboBoxCountry.SelectedIndexChanged += new System.EventHandler(this.OnCountryChanged);
			// 
			// comboBoxLanguage
			// 
			this.comboBoxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLanguage.FormattingEnabled = true;
			this.comboBoxLanguage.Location = new System.Drawing.Point(100, 96);
			this.comboBoxLanguage.Name = "comboBoxLanguage";
			this.comboBoxLanguage.Size = new System.Drawing.Size(300, 23);
			this.comboBoxLanguage.TabIndex = 1;
			this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.OnLanguageChanged);
			// 
			// labelCountry
			// 
			this.labelCountry.AutoSize = true;
			this.labelCountry.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelCountry.Location = new System.Drawing.Point(3, 128);
			this.labelCountry.Name = "labelCountry";
			this.labelCountry.Size = new System.Drawing.Size(53, 15);
			this.labelCountry.TabIndex = 2;
			this.labelCountry.Text = "&Country:";
			// 
			// labelLanguage
			// 
			this.labelLanguage.AutoSize = true;
			this.labelLanguage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelLanguage.Location = new System.Drawing.Point(3, 99);
			this.labelLanguage.Name = "labelLanguage";
			this.labelLanguage.Size = new System.Drawing.Size(62, 15);
			this.labelLanguage.TabIndex = 0;
			this.labelLanguage.Text = "&Language:";
			// 
			// wizardPageForm
			// 
			this.wizardPageForm.Controls.Add(this.labelProviderExample);
			this.wizardPageForm.Controls.Add(this.labelCity);
			this.wizardPageForm.Controls.Add(this.textBoxCity);
			this.wizardPageForm.Controls.Add(this.labelForm);
			this.wizardPageForm.Controls.Add(this.textBoxProvider);
			this.wizardPageForm.Controls.Add(this.labelProvider);
			this.wizardPageForm.HelpText = "For what is this information?";
			this.wizardPageForm.Name = "wizardPageForm";
			this.wizardPageForm.NextPage = this.wizardPageRun;
			this.wizardPageForm.Size = new System.Drawing.Size(537, 257);
			this.wizardPageForm.TabIndex = 1;
			this.wizardPageForm.Text = "Tell us about your network connection";
			this.wizardPageForm.HelpClicked += new System.EventHandler(this.OnHelp);
			// 
			// labelProviderExample
			// 
			this.labelProviderExample.AutoSize = true;
			this.labelProviderExample.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelProviderExample.Location = new System.Drawing.Point(137, 121);
			this.labelProviderExample.Name = "labelProviderExample";
			this.labelProviderExample.Size = new System.Drawing.Size(362, 15);
			this.labelProviderExample.TabIndex = 3;
			this.labelProviderExample.Text = "Example: AT&T, Orange, Telefónica, or the name of your company.";
			this.labelProviderExample.UseMnemonic = false;
			// 
			// labelCity
			// 
			this.labelCity.AutoSize = true;
			this.labelCity.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelCity.Location = new System.Drawing.Point(3, 142);
			this.labelCity.Name = "labelCity";
			this.labelCity.Size = new System.Drawing.Size(31, 15);
			this.labelCity.TabIndex = 4;
			this.labelCity.Text = "&City:";
			// 
			// textBoxCity
			// 
			this.textBoxCity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxCity.Location = new System.Drawing.Point(140, 139);
			this.textBoxCity.Name = "textBoxCity";
			this.textBoxCity.Size = new System.Drawing.Size(300, 23);
			this.textBoxCity.TabIndex = 5;
			this.textBoxCity.TextChanged += new System.EventHandler(this.OnCityChanged);
			// 
			// labelForm
			// 
			this.labelForm.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelForm.Location = new System.Drawing.Point(0, 0);
			this.labelForm.Name = "labelForm";
			this.labelForm.Size = new System.Drawing.Size(537, 50);
			this.labelForm.TabIndex = 0;
			this.labelForm.Text = "We use this to learn more about your Internet connection. We do not collect any p" +
    "ersonal information without your consent, and all fields are optional.";
			// 
			// textBoxProvider
			// 
			this.textBoxProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxProvider.Location = new System.Drawing.Point(140, 95);
			this.textBoxProvider.Name = "textBoxProvider";
			this.textBoxProvider.Size = new System.Drawing.Size(300, 23);
			this.textBoxProvider.TabIndex = 2;
			this.textBoxProvider.TextChanged += new System.EventHandler(this.OnProviderChanged);
			// 
			// labelProvider
			// 
			this.labelProvider.AutoSize = true;
			this.labelProvider.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelProvider.Location = new System.Drawing.Point(3, 98);
			this.labelProvider.Name = "labelProvider";
			this.labelProvider.Size = new System.Drawing.Size(98, 15);
			this.labelProvider.TabIndex = 1;
			this.labelProvider.Text = "&Internet provider:";
			// 
			// wizardPageRun
			// 
			this.wizardPageRun.Controls.Add(this.labelProgress);
			this.wizardPageRun.Controls.Add(this.labelTime);
			this.wizardPageRun.Controls.Add(this.progressBar);
			this.wizardPageRun.Controls.Add(this.labelInfo);
			this.wizardPageRun.Name = "wizardPageRun";
			this.wizardPageRun.NextPage = this.wizardPageFinish;
			this.wizardPageRun.Size = new System.Drawing.Size(552, 258);
			this.wizardPageRun.TabIndex = 2;
			this.wizardPageRun.Text = "Measuring the Internet";
			this.wizardPageRun.TextNext = "&Start";
			this.wizardPageRun.Commit += new System.EventHandler<Mercury.Windows.Controls.AeroWizard.WizardPageConfirmEventArgs>(this.OnRunCommit);
			this.wizardPageRun.Initialize += new System.EventHandler<Mercury.Windows.Controls.AeroWizard.WizardPageInitEventArgs>(this.OnRunInitialize);
			// 
			// labelProgress
			// 
			this.labelProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelProgress.Location = new System.Drawing.Point(0, 158);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(552, 50);
			this.labelProgress.TabIndex = 2;
			// 
			// labelTime
			// 
			this.labelTime.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelTime.Location = new System.Drawing.Point(0, 208);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(552, 50);
			this.labelTime.TabIndex = 3;
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(3, 103);
			this.progressBar.MarqueeAnimationSpeed = 20;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(531, 16);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.TabIndex = 1;
			this.progressBar.Visible = false;
			// 
			// labelInfo
			// 
			this.labelInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelInfo.Location = new System.Drawing.Point(0, 0);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(552, 100);
			this.labelInfo.TabIndex = 0;
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
			this.wizardPageFinish.TabIndex = 3;
			this.wizardPageFinish.Text = "Finished";
			this.wizardPageFinish.Rollback += new System.EventHandler<Mercury.Windows.Controls.AeroWizard.WizardPageConfirmEventArgs>(this.OnFinishRollback);
			// 
			// labelFinish
			// 
			this.labelFinish.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelFinish.Location = new System.Drawing.Point(0, 0);
			this.labelFinish.Name = "labelFinish";
			this.labelFinish.Size = new System.Drawing.Size(537, 100);
			this.labelFinish.TabIndex = 0;
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
			// timer
			// 
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.OnTimer);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 412);
			this.Controls.Add(this.wizardControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(600, 450);
			this.Name = "FormMain";
			this.Text = "Mercury Client";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).EndInit();
			this.wizardPageLocale.ResumeLayout(false);
			this.wizardPageLocale.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCountry)).EndInit();
			this.wizardPageForm.ResumeLayout(false);
			this.wizardPageForm.PerformLayout();
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
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelFinish;
		private System.Windows.Forms.Label labelProgress;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.PictureBox pictureBoxCountry;
		private Windows.Controls.AeroWizard.WizardPage wizardPageForm;
		private System.Windows.Forms.Label labelProvider;
		private System.Windows.Forms.Label labelForm;
		private System.Windows.Forms.TextBox textBoxProvider;
		private System.Windows.Forms.Label labelCity;
		private System.Windows.Forms.TextBox textBoxCity;
		private System.Windows.Forms.Label labelProviderExample;

	}
}

