namespace MercuryClient
{
	partial class Form1
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
			this.wizardControl1 = new DotNetApi.Windows.Controls.AeroWizard.WizardControl();
			this.wizardPage1 = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.wizardPage2 = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// wizardControl1
			// 
			this.wizardControl1.Location = new System.Drawing.Point(0, 0);
			this.wizardControl1.Name = "wizardControl1";
			this.wizardControl1.Pages.Add(this.wizardPage1);
			this.wizardControl1.Pages.Add(this.wizardPage2);
			this.wizardControl1.Size = new System.Drawing.Size(284, 262);
			this.wizardControl1.TabIndex = 0;
			// 
			// wizardPage1
			// 
			this.wizardPage1.Name = "wizardPage1";
			this.wizardPage1.Size = new System.Drawing.Size(237, 107);
			this.wizardPage1.TabIndex = 0;
			this.wizardPage1.Text = "Hello";
			// 
			// wizardPage2
			// 
			this.wizardPage2.Name = "wizardPage2";
			this.wizardPage2.Size = new System.Drawing.Size(237, 107);
			this.wizardPage2.TabIndex = 1;
			this.wizardPage2.Text = "Fuck off";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.wizardControl1);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private DotNetApi.Windows.Controls.AeroWizard.WizardControl wizardControl1;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPage1;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPage2;

	}
}

