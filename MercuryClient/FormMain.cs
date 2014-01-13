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
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Mercury.Globalization;
using Mercury.Web;
using Mercury.Web.Location;
using Mercury.Windows.Controls.AeroWizard;
using Mercury.Windows.Forms;

using System.IO;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mercury
{
	/// <summary>
	/// The main form.
	/// </summary>
	public partial class FormMain : ThreadSafeForm
	{
		private static string[] locales = { "ca", "de", "en", "es", "fr", "pt", "ro" };
		private readonly List<Language> languages = new List<Language>();
		private readonly List<Territory> territories = new List<Territory>();

		private readonly object sync = new object();
		private bool countryUser = false;
		private LocationResult location = null;

		/// <summary>
		/// Creates a new form instance.
		/// </summary>
		public FormMain()
		{
			// Initialize the component.
			this.InitializeComponent();

			// Set the languages.
			this.SetLanguages();

			// Set the current country.
			this.SetCountry();
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
		/// Set the languages.
		/// </summary>
		private void SetLanguages()
		{
			Language currentLanguage = null;

			// Create the lanugages.
			this.languages.Clear();
			foreach (string locale in FormMain.locales)
			{
				Language language = Mercury.Globalization.Resources.Locales[locale].Languages[locale];
				this.languages.Add(language);
				if (language.Equals(Thread.CurrentThread.CurrentUICulture))
				{
					currentLanguage = language;
				}
			}
			
			// Sort the languages by name.
			this.languages.Sort(delegate(Language left, Language right)
			{
				return left.Name.CompareTo(right.Name);
			});

			// Add the languages to the language combo box.
			this.comboBoxLanguage.Items.Clear();
			this.comboBoxLanguage.Items.AddRange(this.languages.ToArray());

			// Select the current language, if exists.
			if (null != currentLanguage)
			{
				this.comboBoxLanguage.SelectedIndex = this.comboBoxLanguage.Items.IndexOf(currentLanguage);
			}

			// Reset the user country selection.
			this.countryUser = false;
		}

		/// <summary>
		/// Sets the current country.
		/// </summary>
		private void SetCountry()
		{
			// Create a location request.
			LocationRequest request = new LocationRequest();

			try
			{
				// Begin an asynchronous request.
				IAsyncResult asyncResult = request.Begin((AsyncWebResult result) =>
					{
						try
						{
							lock (this.sync)
							{
								// End the asynchronous request.
								this.location = request.End(result);
							}

							this.Invoke(() =>
								{
									// If the user did not select a country.
									if (!this.countryUser)
									{
										// Create a new territory for the current country.
										Territory territory = new Territory(this.location.CountryCode, this.location.CountryName);
										// Select the current country.
										this.comboBoxCountry.SelectedIndex = this.comboBoxCountry.Items.IndexOf(territory);
									}
								});
						}
						catch { }
					}, null);
			}
			catch { }
		}

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
				if (Regex.IsMatch(territory.Type, "[(A-Z|a-z)][(A-Z|a-z)]"))
				{
					this.territories.Add(territory);
				}
			}

			// Sort the territories by name.
			this.territories.Sort(delegate(Territory left, Territory right)
			{
				return left.Name.CompareTo(right.Name);
			});

			// Save the selected country.
			Territory selectedCountry = this.comboBoxCountry.SelectedItem as Territory;

			// Clear the countries.
			this.comboBoxCountry.Items.Clear();
			this.comboBoxCountry.Items.AddRange(this.territories.ToArray());

			// Find the selected language.
			if (null != selectedCountry)
			{
				this.comboBoxCountry.SelectedIndex = this.comboBoxCountry.Items.IndexOf(selectedCountry);
			}

			// Update the language.
			this.OnUpdateCulture(new CultureInfo(language.Type));

			// Call the country changed event handler.
			this.OnCountryChanged(sender, e);
		}

		/// <summary>
		/// An event handler called when the country has changed.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnCountryChanged(object sender, EventArgs e)
		{
			// Enable the next button.
			this.wizardPageLocale.AllowNext = this.comboBoxCountry.SelectedIndex >= 0;

			// Set the user country selection.
			this.countryUser = true;
		}

		/// <summary>
		/// Updates the current culture.
		/// </summary>
		/// <param name="culture"></param>
		private void OnUpdateCulture(CultureInfo culture)
		{
			// Set the thread culture.
			Thread.CurrentThread.CurrentUICulture = culture;
			
			// Set the controls.
			this.wizardControl.Culture = culture;
			this.wizardPageLocale.Text = WizardResources.GetString("PageLocalesTitle");
			this.wizardPageRun.Text = WizardResources.GetString("PageRunTitle");
			this.wizardPageFinish.Text = WizardResources.GetString("PageFinishTitle");
			this.wizardPageLocale.HelpText = WizardResources.GetString("HelpLocales");
			this.labelLanguage.Text = WizardResources.GetString("LabelLanguage");
			this.labelCountry.Text = WizardResources.GetString("LabelCountry");
			this.labelInfo.Text = WizardResources.GetString("LabelInfo");
			this.labelProgress.Text = WizardResources.GetString("LabelProgress");
			this.labelFinish.Text = WizardResources.GetString("LabelFinish");
		}

		/// <summary>
		/// An event handler called when the user clicks the cancel button.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnCanceling(object sender, CancelEventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// An event handler called when the user clicks the finish button.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnFinished(object sender, EventArgs e)
		{
			this.Close();
		}


		/// <summary>
		/// Loads the locales from the specified XML file and saves them to a resource file.
		/// </summary>
		private void OnLoadLocales()
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
				catch (Exception exception)
				{
					MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		/// <summary>
		/// An event handler called when the user clicks on the locale help link.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnLocaleHelp(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// An event raised when the user enterso on the run page.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnRunInitialize(object sender, WizardPageInitEventArgs e)
		{
			this.wizardControl.NextButtonText = "Start";
		}

		/// <summary>
		/// An event raised when the user clicks on the run page commit button.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnRunCommit(object sender, WizardPageConfirmEventArgs e)
		{
			// Cancel the page change.
			e.Cancel = true;
		}
	}
}
