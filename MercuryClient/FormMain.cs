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
using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
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
		private static string[] locales = { "ca", "de", "en", "es", "fr", "pt", "ro" };
		private readonly List<Language> languages = new List<Language>();
		private readonly List<Territory> territories = new List<Territory>();

		/// <summary>
		/// Creates a new form instance.
		/// </summary>
		public FormMain()
		{
			// Initialize the component.
			this.InitializeComponent();

			// Load the languages.
			this.LoadLanguages();
		}

		// Static methods.

		/// <summary>
		/// Compares two languages.
		/// </summary>
		/// <param name="left">The left language.</param>
		/// <param name="right">The right language.</param>
		/// <returns>The result of the comparison.</returns>
		private static int CompareLanguages(Language left, Language right)
		{
			return left.Name.CompareTo(right.Name);
		}

		// Private methods.

		/// <summary>
		/// Loads the languages.
		/// </summary>
		private void LoadLanguages()
		{
			// Create the lanugages.
			this.languages.Clear();
			foreach (string locale in FormMain.locales)
			{
				this.languages.Add(Mercury.Globalization.Resources.Locales[locale].Languages[locale]);
			}
			
			// Sort the languages by name.
			this.languages.Sort(delegate(Language left, Language right)
			{
				return left.Name.CompareTo(right.Name);
			});

			// Save the selected language.
			Language selectedLanguage = this.comboBoxLanguage.SelectedItem as Language;

			// Add the languages to the language combo box.
			this.comboBoxLanguage.Items.Clear();
			this.comboBoxLanguage.Items.AddRange(this.languages.ToArray());

			// Find the 
			this.comboBoxLanguage.SelectedIndex = this.comboBoxLanguage.Items.IndexOf(selectedLanguage);
		}


		/// <summary>
		/// Loads the locales from the specified XML file and saves them to a resource file.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		//private void OnLoadLocales(object sender, EventArgs e)
		//{
		//	if (this.openFileDialog.ShowDialog(this) == DialogResult.OK)
		//	{
		//		try
		//		{
		//			using (FileStream file = new FileStream(this.openFileDialog.FileName, FileMode.Open))
		//			{
		//				using (LocaleReader reader = new LocaleReader(file))
		//				{
		//					LocaleCollection locales = reader.ReadLocaleCollection();

		//					if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
		//					{
		//						using (ResXResourceWriter writer = new ResXResourceWriter(this.saveFileDialog.FileName))
		//						{
		//							BinaryFormatter formatter = new BinaryFormatter();

		//							using (MemoryStream stream = new MemoryStream())
		//							{
		//								formatter.Serialize(stream, locales);
		//								writer.AddResource("Collection", stream.ToArray());
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}
		//		catch(Exception exception)
		//		{
		//			MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		//		}
		//	}
		//}

		/// <summary>
		/// An event handler called when the language has changed.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnLanguageChanged(object sender, EventArgs e)
		{
			// Get the selected language.
			Language language = this.comboBoxLanguage.SelectedItem as Language;

			// Get the countries for this language.
			this.territories.Clear();
			foreach (Territory territory in Mercury.Globalization.Resources.Locales[language].Territories)
			{
				this.territories.Add(territory);
			}

			// Sort the territories by name.
			this.territories.Sort(delegate(Territory left, Territory right)
			{
				return left.Name.CompareTo(right.Name);
			});
			
			// Clear the countries.
			this.comboBoxCountry.Items.Clear();
			this.comboBoxCountry.Items.AddRange(this.territories.ToArray());

			//int index = this.comboBoxLanguage.SelectedIndex;
			//string locale = FormMain.locales[this.comboBoxLanguage.SelectedIndex];

			//this.comboBoxCountry.Items.Clear();
			//foreach (Language language in .Languages)
			//{
			//	this.comboBoxCountry.Items.Add(language.Name);
			//}
			//this.comboBoxCountry.SelectedIndex = index;
		}
	}
}
