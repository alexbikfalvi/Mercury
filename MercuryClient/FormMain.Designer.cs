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
			this.wizardPageStart = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.labelCountry = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.labelLanguage = new System.Windows.Forms.Label();
			this.wizardPageRun = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.wizardPageFinish = new DotNetApi.Windows.Controls.AeroWizard.WizardPage();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.wizardControl)).BeginInit();
			this.wizardPageStart.SuspendLayout();
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			resources.ApplyResources(this.wizardControl, "wizardControl");
			this.wizardControl.Name = "wizardControl";
			this.wizardControl.Pages.AddRange(new DotNetApi.Windows.Controls.AeroWizard.WizardPage[] {
            this.wizardPageStart,
            this.wizardPageRun,
            this.wizardPageFinish});
			// 
			// wizardPageStart
			// 
			this.wizardPageStart.AllowBack = false;
			this.wizardPageStart.Controls.Add(this.buttonLoad);
			this.wizardPageStart.Controls.Add(this.labelCountry);
			this.wizardPageStart.Controls.Add(this.comboBox2);
			this.wizardPageStart.Controls.Add(this.comboBox1);
			this.wizardPageStart.Controls.Add(this.labelLanguage);
			this.wizardPageStart.Name = "wizardPageStart";
			resources.ApplyResources(this.wizardPageStart, "wizardPageStart");
			// 
			// buttonLoad
			// 
			resources.ApplyResources(this.buttonLoad, "buttonLoad");
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.OnLoad);
			// 
			// labelCountry
			// 
			resources.ApplyResources(this.labelCountry, "labelCountry");
			this.labelCountry.Name = "labelCountry";
			// 
			// comboBox2
			// 
			this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
            resources.GetString("comboBox2.Items"),
            resources.GetString("comboBox2.Items1"),
            resources.GetString("comboBox2.Items2"),
            resources.GetString("comboBox2.Items3"),
            resources.GetString("comboBox2.Items4"),
            resources.GetString("comboBox2.Items5"),
            resources.GetString("comboBox2.Items6")});
			resources.ApplyResources(this.comboBox2, "comboBox2");
			this.comboBox2.Name = "comboBox2";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4"),
            resources.GetString("comboBox1.Items5"),
            resources.GetString("comboBox1.Items6")});
			resources.ApplyResources(this.comboBox1, "comboBox1");
			this.comboBox1.Name = "comboBox1";
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
			this.wizardPageStart.ResumeLayout(false);
			this.wizardPageStart.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private DotNetApi.Windows.Controls.AeroWizard.WizardControl wizardControl;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageStart;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageRun;
		private DotNetApi.Windows.Controls.AeroWizard.WizardPage wizardPageFinish;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label labelLanguage;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label labelCountry;
		private System.Windows.Forms.Button buttonLoad;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;

	}
}

