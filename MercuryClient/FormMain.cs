/* 
 * Copyright (C) 2013 Alex Bikfalvi
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using DotNetApi.Globalization;
using DotNetApi.Windows.Forms;

using System.Resources;

namespace MercuryClient
{
	/// <summary>
	/// The main form.
	/// </summary>
	public partial class FormMain : ThreadSafeForm
	{
		/// <summary>
		/// Creates a new form instance.
		/// </summary>
		public FormMain()
		{
			// Initialize the component.
			this.InitializeComponent();

			LocaleCollection locales = Mercury.Globalization.Resources.Locales;
		}

		private void OnLoad(object sender, EventArgs e)
		{
			if (this.openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					using (FileStream file = new FileStream(this.openFileDialog.FileName, FileMode.Open))
					{
						using (LocaleReader reader = new LocaleReader(file))
						{
							LocaleCollection locales = reader.ReadLocaleCollection();

							if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
							{
								using (ResXResourceWriter writer = new ResXResourceWriter(this.saveFileDialog.FileName))
								{
									BinaryFormatter formatter = new BinaryFormatter();

									using (MemoryStream stream = new MemoryStream())
									{
										formatter.Serialize(stream, locales);
										writer.AddResource("Collection", stream.ToArray());
									}
								}
							}
						}
					}
				}
				catch(Exception exception)
				{
					MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
