namespace Mercury.Controls.Aws
{
	partial class ControlAwsCredentials
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Security.SecureString secureString1 = new System.Security.SecureString();
			System.Security.SecureString secureString2 = new System.Security.SecureString();
			this.labelAccessKey = new System.Windows.Forms.Label();
			this.labelSecretKey = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.textBoxAccessKey = new DotNetApi.Windows.Controls.SecureTextBox();
			this.textBoxSecretKey = new DotNetApi.Windows.Controls.SecureTextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// labelAccessKey
			// 
			this.labelAccessKey.AutoSize = true;
			this.labelAccessKey.Location = new System.Drawing.Point(17, 77);
			this.labelAccessKey.Name = "labelAccessKey";
			this.labelAccessKey.Size = new System.Drawing.Size(65, 13);
			this.labelAccessKey.TabIndex = 1;
			this.labelAccessKey.Text = "&Access key:";
			// 
			// labelSecretKey
			// 
			this.labelSecretKey.AutoSize = true;
			this.labelSecretKey.Location = new System.Drawing.Point(17, 103);
			this.labelSecretKey.Name = "labelSecretKey";
			this.labelSecretKey.Size = new System.Drawing.Size(61, 13);
			this.labelSecretKey.TabIndex = 3;
			this.labelSecretKey.Text = "&Secret key:";
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTitle.Location = new System.Drawing.Point(75, 34);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(250, 20);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "Amazon Web Services credentials";
			// 
			// pictureBox
			// 
			this.pictureBox.Image = global::Mercury.Resources.KeyVerticalCube_48;
			this.pictureBox.Location = new System.Drawing.Point(20, 20);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(48, 48);
			this.pictureBox.TabIndex = 6;
			this.pictureBox.TabStop = false;
			// 
			// textBoxAccessKey
			// 
			this.textBoxAccessKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxAccessKey.Location = new System.Drawing.Point(99, 74);
			this.textBoxAccessKey.Name = "textBoxAccessKey";
			this.textBoxAccessKey.SecureText = secureString1;
			this.textBoxAccessKey.Size = new System.Drawing.Size(270, 20);
			this.textBoxAccessKey.TabIndex = 2;
			// 
			// textBoxSecretKey
			// 
			this.textBoxSecretKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSecretKey.Location = new System.Drawing.Point(99, 100);
			this.textBoxSecretKey.Name = "textBoxSecretKey";
			this.textBoxSecretKey.SecureText = secureString2;
			this.textBoxSecretKey.Size = new System.Drawing.Size(270, 20);
			this.textBoxSecretKey.TabIndex = 4;
			// 
			// ControlAwsCredentials
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxSecretKey);
			this.Controls.Add(this.textBoxAccessKey);
			this.Controls.Add(this.labelAccessKey);
			this.Controls.Add(this.labelSecretKey);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.pictureBox);
			this.Name = "ControlAwsCredentials";
			this.Size = new System.Drawing.Size(400, 200);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelAccessKey;
		private System.Windows.Forms.Label labelSecretKey;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.PictureBox pictureBox;
		private DotNetApi.Windows.Controls.SecureTextBox textBoxAccessKey;
		private DotNetApi.Windows.Controls.SecureTextBox textBoxSecretKey;
	}
}
