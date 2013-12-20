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
			this.wizardPage1 = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.wizardPage2 = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).BeginInit();
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			this.wizardControl.Location = new System.Drawing.Point(0, 0);
			this.wizardControl.Name = "wizardControl";
			this.wizardControl.Pages.Add(this.wizardPage1);
			this.wizardControl.Pages.Add(this.wizardPage2);
			this.wizardControl.Pages.Add(this.wizardPage1);
			this.wizardControl.Pages.Add(this.wizardPage2);
			this.wizardControl.Pages.Add(this.wizardPage1);
			this.wizardControl.Pages.Add(this.wizardPage2);
			this.wizardControl.Size = new System.Drawing.Size(584, 412);
			this.wizardControl.TabIndex = 0;
			this.wizardControl.Title = "Mercury Client";
			// 
			// wizardPage1
			// 
			this.wizardPage1.Name = "wizardPage1";
			this.wizardPage1.Size = new System.Drawing.Size(537, 257);
			this.wizardPage1.TabIndex = 0;
			this.wizardPage1.Text = "Page Title";
			// 
			// wizardPage2
			// 
			this.wizardPage2.Name = "wizardPage2";
			this.wizardPage2.Size = new System.Drawing.Size(537, 257);
			this.wizardPage2.TabIndex = 1;
			this.wizardPage2.Text = "Page Title";
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
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPage1;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPage2;

	}
}

