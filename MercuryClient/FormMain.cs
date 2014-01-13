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
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Mercury.Collections.Generic;
using Mercury.Globalization;
using Mercury.Net.Core;
using Mercury.Threading;
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
		/// <summary>
		/// A class representing the information for a traceroute.
		/// </summary>
		private sealed class TracerouteInfo
		{
			/// <summary>
			/// Creates a new traceroute information instance.
			/// </summary>
			/// <param name="site">The destination site.</param>
			public TracerouteInfo(string site)
			{
				this.Site = site;
				this.Address = null;
				this.Count = 0;
			}

			// Public properties.

			/// <summary>
			/// Gets the traceroute site.
			/// </summary>
			public string Site { get; private set; }
			/// <summary>
			/// Gets or sets the IP address.
			/// </summary>
			public IPAddress Address { get; set; }
			/// <summary>
			/// Gets or sets the number of attempts for this traceroute.
			/// </summary>
			public int Count { get; set; }
		}

		private static readonly string[] locales = { "ca", "de", "en", "es", "fr", "pt", "ro" };
		private readonly List<Language> languages = new List<Language>();
		private readonly List<Territory> territories = new List<Territory>();

		private static readonly string uriGetUrls = "http://inetanalytics.nets.upf.edu/getUrls?countryCode={0}";

		private bool countryUser = false;
		private LocationResult location = null;

		private readonly object sync = new object();

		private readonly AsyncWebRequest asyncWebRequest = new AsyncWebRequest();
		private IAsyncResult asyncWebResult = null;
		private readonly ManualResetEvent waitAsync = new ManualResetEvent(true);

		private readonly Traceroute traceroute;
		private readonly TracerouteSettings tracerouteSettings;
		private const int tracerouteConcurrent = 1;
		private readonly Queue<TracerouteInfo> traceroutePending = new Queue<TracerouteInfo>();
		private readonly HashSet<TracerouteInfo> tracerouteRunning = new HashSet<TracerouteInfo>();
		private readonly HashSet<TracerouteInfo> tracerouteCompleted = new HashSet<TracerouteInfo>();
		private readonly CancellationToken tracerouteCancel = new CancellationToken();

		private bool completed = false;
		private bool canceling = false;
		private bool canceled = false;

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

			// Create the traceroute settings.
			this.tracerouteSettings = new TracerouteSettings();
			this.tracerouteSettings.MaximumFailedHops = 10;
			this.tracerouteSettings.StopTracerouteOnFail = true;
			this.tracerouteSettings.StopHopOnSuccess = true;

			// Create the traceroute.
			this.traceroute = new Traceroute(this.tracerouteSettings);
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
			this.labelFinish.Text = WizardResources.GetString("LabelFinish");
			this.wizardPageRun.TextNext = WizardResources.GetString("WizardStartText");
		}

		/// <summary>
		/// An event handler called when the user closes the wizard.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnClosing(object sender, FormClosingEventArgs e)
		{
			// Call the canceling event handler.
			this.OnCanceling(sender, e);
		}

		/// <summary>
		/// An event handler called when the user clicks the cancel button.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnCanceling(object sender, CancelEventArgs e)
		{
			lock (this.sync)
			{
				// If the wizard is already canceled, do nothing.
				if (this.canceled) return;
				// If the wizard is already canceling.
				if (this.canceling)
				{
					// Cancel the event.
					e.Cancel = true;
					// Return.
					return;
				}

				// If there are pending web or traceroute requests.
				if ((null != this.asyncWebResult) || (this.tracerouteRunning.Count > 0))
				{
					// Ask the user whether to cancel.
					if (MessageBox.Show(this, "Cancelling the wizard will prevent uploading any measurement result. Do you want to continue?", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						// Cancel the close event.
						e.Cancel = true;
						// Set the canceling flag.
						this.canceling = true;
						// Disable the cancel button.
						this.wizardPageRun.AllowCancel = false;
						// Cancel the asynchronous operations.
						this.OnCancel();
					}
				}
				else
				{
					// Set the canceled to true.
					this.canceled = true;
					// Close the form.
					this.Close();
				}
			}
		}

		/// <summary>
		/// Cancels all pending asynchronous operations.
		/// </summary>
		private void OnCancel()
		{
			// Execute the cancellation on the thread pool.
			ThreadPool.QueueUserWorkItem((object state) =>
				{
					// Wait for the asynchronous handle.
					this.waitAsync.WaitOne();

					this.Invoke(() =>
						{
							lock (this.sync)
							{
								// Set the canceled flag.
								this.canceled = true;
							}
							// Close the form.
							this.Close();
						});
				});
		}

		/// <summary>
		/// An event handler called when the user clicks the finish button.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnFinished(object sender, EventArgs e)
		{
			// Close the form.
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
			Process.Start("http://inetanalytics.nets.upf.edu/mercury/faq");
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
			if (!this.completed)
			{
				// Cancel the page change.
				e.Cancel = true;
				// Start the wizard.
				this.OnStartDownload();
			}
		}

		/// <summary>
		/// Starts the wizard download.
		/// </summary>
		private void OnStartDownload()
		{
			// Get the selected country.
			Territory country = this.comboBoxCountry.SelectedItem as Territory;
			// Get the country.
			string countryCode = country != null ? country.Type.ToUpperInvariant() : string.Empty;

			// Disable the start button.
			this.wizardPageRun.AllowNext = false;
			// Show the progress.
			this.progressBar.Visible = true;
			this.progressBar.Style = ProgressBarStyle.Marquee;
			this.labelProgress.Text = WizardResources.GetString("LabelProgressDownloadStart");

			lock (this.sync)
			{
				try
				{
					// Reset the wait handle.
					this.waitAsync.Reset();
					// Download the web destinations.
					this.asyncWebResult = this.asyncWebRequest.Begin(new Uri(string.Format(FormMain.uriGetUrls, countryCode)), (AsyncWebResult asyncResult) =>
						{
							lock (this.sync)
							{
								this.asyncWebResult = null;
							}

							try
							{
								// Complete the web request.
								string data;
								this.asyncWebRequest.End(asyncResult, out data);
								// Parse the list of sites.
								string[] sites = data.Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

								// Set the wait handle.
								this.waitAsync.Set();

								this.Invoke(() =>
									{
										// Begin the traceroutes.
										this.OnStartTraceroute(sites);
									});
							}
							catch (Exception exception)
							{
								this.Invoke(() =>
									{
										// Set the wait handle.
										this.waitAsync.Set();
										// Show an error message.
										MessageBox.Show(
											this,
											string.Format("Cannot connect to the Mercury web server. Check your Internet connection and try again.{0}{1}Technical information: {2}", Environment.NewLine, Environment.NewLine, exception.Message),
											"Mercury Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
										// Enable the start button.
										this.wizardPageRun.AllowNext = true;
									});
							}
						}, null);
				}
				catch (Exception exception)
				{
					// Set the wait handle.
					this.waitAsync.Set();
					// Show an error message.
					MessageBox.Show(
						this,
						string.Format("Cannot connect to the Mercury web server. Check your Internet connection and try again.{0}{1}Technical information: {2}", Environment.NewLine, Environment.NewLine, exception.Message),
						"Mercury Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					// Enable the start button.
					this.wizardPageRun.AllowNext = true;
				}
			}
		}

		/// <summary>
		/// Starts the wizard traceroute.
		/// </summary>
		/// <param name="sites">The list of sites.</param>
		private void OnStartTraceroute(string[] sites)
		{
			// Set the progress.
			this.progressBar.Style = ProgressBarStyle.Continuous;
			this.progressBar.Minimum = 0;
			this.progressBar.Maximum = sites.Length;
			this.progressBar.Value = 0;
			this.labelProgress.Text = string.Format(WizardResources.GetString("LabelProgressDownloadFinish"), sites.Length);

			// Set the pending traceroutes.
			lock (this.sync)
			{
				this.traceroutePending.Clear();
				this.tracerouteRunning.Clear();
				this.tracerouteCompleted.Clear();

				foreach (string site in sites)
				{
					this.traceroutePending.Enqueue(new TracerouteInfo(site));
				}
			}

			// Reset the wait handle.
			this.waitAsync.Reset();
			// Reset the cancellation token.
			this.tracerouteCancel.Reset();

			// For all the concurrent traceroutes.
			for (int index = 0; index < FormMain.tracerouteConcurrent; index++)
			{
				this.OnStartTraceroute();
			}
		}

		/// <summary>
		/// Starts an individual traceroute.
		/// </summary>
		private void OnStartTraceroute()
		{
			// Start the traceroute on the thread pool.
			ThreadPool.QueueUserWorkItem((object state) =>
				{
					TracerouteInfo info;

					lock (this.sync)
					{
						// If the sites queue is empty.
						if (this.traceroutePending.Count == 0)
						{
							this.Invoke(() =>
								{
									// If the wizard is not completed.
									if (!this.completed)
									{
										// Set the completed flag.
										this.completed = true;
										// Enable the start button.
										this.wizardPageRun.AllowNext = true;
									}
								});
							return;
						}
						// Get a site from the queue.
						info = this.traceroutePending.Dequeue();
						// Add the site to the running list.
						this.tracerouteRunning.Add(info);
					}

					try
					{
						// If the IP address is null.
						if (null == info.Address)
						{
							// Get the IP addresses.
							IPAddress[] ipAddresses = Dns.GetHostAddresses(info.Site);
							// Set the first IP address.
							info.Address = ipAddresses[0];
						}

						// If the list of IP addresses has at least one address.
						if (null != info.Address)
						{
							// Begin a traceroute for the specified destination.
							TracerouteResult result = this.traceroute.Run(info.Address, this.tracerouteCancel);

							// Upload the result.
						}
					}
					catch
					{
						// Start another traceroute.
						this.OnStartTraceroute();
					}
				});
		}
	}
}
