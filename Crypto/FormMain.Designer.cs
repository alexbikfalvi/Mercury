namespace Crypto
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
			this.buttonGenerate = new System.Windows.Forms.Button();
			this.textBoxIV = new System.Windows.Forms.TextBox();
			this.textBoxKey = new System.Windows.Forms.TextBox();
			this.labelIV = new System.Windows.Forms.Label();
			this.labelKey = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonGenerate
			// 
			this.buttonGenerate.Location = new System.Drawing.Point(12, 12);
			this.buttonGenerate.Name = "buttonGenerate";
			this.buttonGenerate.Size = new System.Drawing.Size(75, 23);
			this.buttonGenerate.TabIndex = 0;
			this.buttonGenerate.Text = "Generate";
			this.buttonGenerate.UseVisualStyleBackColor = true;
			this.buttonGenerate.Click += new System.EventHandler(this.OnGenerate);
			// 
			// textBoxIV
			// 
			this.textBoxIV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxIV.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxIV.Location = new System.Drawing.Point(46, 41);
			this.textBoxIV.Multiline = true;
			this.textBoxIV.Name = "textBoxIV";
			this.textBoxIV.ReadOnly = true;
			this.textBoxIV.Size = new System.Drawing.Size(527, 100);
			this.textBoxIV.TabIndex = 2;
			// 
			// textBoxKey
			// 
			this.textBoxKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxKey.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxKey.Location = new System.Drawing.Point(46, 150);
			this.textBoxKey.Multiline = true;
			this.textBoxKey.Name = "textBoxKey";
			this.textBoxKey.ReadOnly = true;
			this.textBoxKey.Size = new System.Drawing.Size(527, 100);
			this.textBoxKey.TabIndex = 4;
			// 
			// labelIV
			// 
			this.labelIV.AutoSize = true;
			this.labelIV.Location = new System.Drawing.Point(12, 44);
			this.labelIV.Name = "labelIV";
			this.labelIV.Size = new System.Drawing.Size(20, 13);
			this.labelIV.TabIndex = 1;
			this.labelIV.Text = "IV:";
			// 
			// labelKey
			// 
			this.labelKey.AutoSize = true;
			this.labelKey.Location = new System.Drawing.Point(12, 153);
			this.labelKey.Name = "labelKey";
			this.labelKey.Size = new System.Drawing.Size(28, 13);
			this.labelKey.TabIndex = 3;
			this.labelKey.Text = "Key:";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(585, 262);
			this.Controls.Add(this.labelKey);
			this.Controls.Add(this.labelIV);
			this.Controls.Add(this.textBoxKey);
			this.Controls.Add(this.textBoxIV);
			this.Controls.Add(this.buttonGenerate);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "FormMain";
			this.Text = "Cryptographic Generator";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonGenerate;
		private System.Windows.Forms.TextBox textBoxIV;
		private System.Windows.Forms.TextBox textBoxKey;
		private System.Windows.Forms.Label labelIV;
		private System.Windows.Forms.Label labelKey;
	}
}

