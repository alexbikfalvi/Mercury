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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.wizardControl = new DotNetApi.Windows.Controls.AeroWizard.WizardControl();
			this.wizardPageLocale = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.comboBoxCountry = new System.Windows.Forms.ComboBox();
			this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
			this.labelCountry = new System.Windows.Forms.Label();
			this.labelLanguage = new System.Windows.Forms.Label();
			this.wizardPageRun = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.wizardPageFinish = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).BeginInit();
			this.wizardPageLocale.SuspendLayout();
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			resources.ApplyResources(this.wizardControl, "wizardControl");
			this.wizardControl.Name = "wizardControl";
			this.wizardControl.Pages.AddRange(new DotNetApi.Windows.Controls.AeroWizard.WizardPage[] {
            this.wizardPageLocale,
            this.wizardPageRun,
            this.wizardPageFinish});
			// 
			// wizardPageLocale
			// 
			this.wizardPageLocale.AllowBack = false;
			this.wizardPageLocale.AllowNext = false;
			this.wizardPageLocale.Controls.Add(this.comboBoxCountry);
			this.wizardPageLocale.Controls.Add(this.comboBoxLanguage);
			this.wizardPageLocale.Controls.Add(this.labelCountry);
			this.wizardPageLocale.Controls.Add(this.labelLanguage);
			this.wizardPageLocale.Name = "wizardPageLocale";
			resources.ApplyResources(this.wizardPageLocale, "wizardPageLocale");
			// 
			// comboBoxCountry
			// 
			this.comboBoxCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCountry.FormattingEnabled = true;
			resources.ApplyResources(this.comboBoxCountry, "comboBoxCountry");
			this.comboBoxCountry.Name = "comboBoxCountry";
			// 
			// comboBoxLanguage
			// 
			this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLanguage.FormattingEnabled = true;
			resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
			this.comboBoxLanguage.Name = "comboBoxLanguage";
			this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.OnLanguageChanged);
			// 
			// labelCountry
			// 
			resources.ApplyResources(this.labelCountry, "labelCountry");
			this.labelCountry.Name = "labelCountry";
			// 
			// labelLanguage
			// 
			resources.ApplyResources(this.labelLanguage, "labelLanguage");
			this.labelLanguage.Name = "labelLanguage";
			// 
			// wizardPageRun
			// 
			this.wizardPageRun.Name = "wizardPageRun";
			resources.ApplyResources(this.wizardPageRun, "wizardPageRun");
			// 
			// wizardPageFinish
			// 
			this.wizardPageFinish.HelpText = "";
			this.wizardPageFinish.IsFinishPage = true;
			this.wizardPageFinish.Name = "wizardPageFinish";
			resources.ApplyResources(this.wizardPageFinish, "wizardPageFinish");
			// 
			// openFileDialog
			// 
			resources.ApplyResources(this.openFileDialog, "openFileDialog");
			// 
			// saveFileDialog
			// 
			resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
			// 
			// FormMain
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.wizardControl);
			this.Name = "FormMain";
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).EndInit();
			this.wizardPageLocale.ResumeLayout(false);
			this.wizardPageLocale.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private DotNetApi.Windows.Controls.AeroWizard.WizardControl wizardControl;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageLocale;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageRun;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageFinish;
		private System.Windows.Forms.Label labelCountry;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label labelLanguage;
		private System.Windows.Forms.ComboBox comboBoxLanguage;
		private System.Windows.Forms.ComboBox comboBoxCountry;

	}
}

