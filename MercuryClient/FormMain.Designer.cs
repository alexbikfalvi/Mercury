namespace MercuryClient
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
			this.wizardControl = new DotNetApi.Windows.Controls.AeroWizard.WizardControl();
			this.wizardPageStart = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.wizardPageRun = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.wizardPageFinish = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).BeginInit();
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			this.wizardControl.Location = new System.Drawing.Point(0, 0);
			this.wizardControl.Name = "wizardControl";
			this.wizardControl.Pages.AddRange(new DotNetApi.Windows.Controls.AeroWizard.WizardPage[] {
            this.wizardPageStart,
            this.wizardPageRun,
            this.wizardPageFinish});
			this.wizardControl.Size = new System.Drawing.Size(584, 412);
			this.wizardControl.TabIndex = 0;
			this.wizardControl.Title = "Mercury Client";
			// 
			// wizardPageStart
			// 
			this.wizardPageStart.AllowBack = false;
			this.wizardPageStart.Name = "wizardPageStart";
			this.wizardPageStart.Size = new System.Drawing.Size(537, 257);
			this.wizardPageStart.TabIndex = 0;
			this.wizardPageStart.Text = "Select your language and country";
			// 
			// wizardPageRun
			// 
			this.wizardPageRun.Name = "wizardPageRun";
			this.wizardPageRun.Size = new System.Drawing.Size(552, 258);
			this.wizardPageRun.TabIndex = 1;
			this.wizardPageRun.Text = "Measuring the Internet";
			// 
			// wizardPageFinish
			// 
			this.wizardPageFinish.HelpText = "";
			this.wizardPageFinish.IsFinishPage = true;
			this.wizardPageFinish.Name = "wizardPageFinish";
			this.wizardPageFinish.Size = new System.Drawing.Size(537, 257);
			this.wizardPageFinish.TabIndex = 2;
			this.wizardPageFinish.Text = "Page Title";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 412);
			this.Controls.Add(this.wizardControl);
			this.Name = "FormMain";
			this.Text = "Mercury Client";
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private DotNetApi.Windows.Controls.AeroWizard.WizardControl wizardControl;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageStart;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageRun;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageFinish;

	}
}

