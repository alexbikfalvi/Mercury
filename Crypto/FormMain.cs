using System;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Crypto
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();
		}

		private void OnGenerate(object sender, EventArgs e)
		{
			// Clear the text boxes.
			this.textBoxIV.Clear();
			this.textBoxKey.Clear();
			// Create an AES cryptographic service provider and transform.
			using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
			{
				// Generate an IV.
				aesProvider.GenerateIV();
				// Generate a key.
				aesProvider.GenerateKey();
				// Display the IV.
				foreach (byte b in aesProvider.IV)
				{
					this.textBoxIV.AppendText(string.Format("0x{0:X2}, ", b));
				}
				// Display the key.
				foreach (byte b in aesProvider.Key)
				{
					this.textBoxKey.AppendText(string.Format("0x{0:X2}, ", b));
				}
			}
		}
	}
}
