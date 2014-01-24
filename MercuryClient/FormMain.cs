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
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Mercury.Collections.Generic;
using Mercury.Globalization;
using Mercury.Net.Core;
using Mercury.Json;
using Mercury.Threading;
using Mercury.Web;
using Mercury.Web.Location;
using Mercury.Windows.Controls.AeroWizard;
using Mercury.Windows.Forms;

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
		private static readonly string uriUploadSession = "http://mercury.upf.edu/mercury/api/traceroute/addTracerouteSession";
		private static readonly string uriUploadTrace = "http://mercury.upf.edu/mercury/api/traceroute/uploadTrace";

		private bool countryUser = false;
		private bool cityUser = false;
		private LocationResult location = null;

		private readonly object sync = new object();

		private readonly AsyncWebRequest asyncWebRequest = new AsyncWebRequest();
		private IAsyncResult asyncWebResult = null;
		private readonly ManualResetEvent waitAsync = new ManualResetEvent(true);

		private const int tracerouteConcurrent = 20;
		private const int tracerouteRetries = 3;

		private readonly Traceroute traceroute;
		private readonly TracerouteSettings tracerouteSettings;
		private readonly Queue<TracerouteInfo> traceroutePending = new Queue<TracerouteInfo>();
		private readonly HashSet<TracerouteInfo> tracerouteRunning = new HashSet<TracerouteInfo>();
		private readonly HashSet<TracerouteInfo> tracerouteCompleted = new HashSet<TracerouteInfo>();
		private readonly HashSet<TracerouteInfo> tracerouteFailed = new HashSet<TracerouteInfo>();
		private readonly CancellationToken tracerouteCancel = new CancellationToken();
		private DateTime tracerouteTimestamp;

		private bool completed = false;
		private bool canceling = false;
		private bool canceled = false;

		private DateTime sessionTimestamp;
		private string sessionId;

		private string timeSecondRemaining;
		private string timeSecondsRemaining;
		private string timeMinuteRemaining;
		private string timeMinutesRemaining;
		private string timeMinutesSecondsRemaining;
		private string timeSecond;
		private string timeSeconds;
		private string timeMinute;
		private string timeMinutes;

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
			// If there is no network connection, do nothing.
			if (!NetworkInterface.GetIsNetworkAvailable()) return;

			// Create a location request.
			LocationRequest request = new LocationRequest();

			if (!this.countryUser)
			{
				this.pictureBoxCountry.Image = WizardResources.Busy;
				this.pictureBoxCountry.Visible = true;
			}

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
									// Hide the busy picture.
									this.pictureBoxCountry.Visible = false;
									// If the user did not select a country.
									if (!this.countryUser)
									{
										// Create a new territory for the current country.
										Territory territory = new Territory(this.location.CountryCode, this.location.CountryName);
										// Select the current country.
										this.comboBoxCountry.SelectedIndex = this.comboBoxCountry.Items.IndexOf(territory);
									}
									// If the user did not select a city.
									if (!this.cityUser)
									{
										// Select the current city.
										this.textBoxCity.Text = this.location.City;
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
			this.wizardPageForm.Text = WizardResources.GetString("PageFormTitle");
			this.wizardPageRun.Text = WizardResources.GetString("PageRunTitle");
			this.wizardPageFinish.Text = WizardResources.GetString("PageFinishTitle");
			this.wizardPageLocale.HelpText = WizardResources.GetString("Help");
			this.wizardPageForm.HelpText = WizardResources.GetString("Help");

			this.wizardPageLocale.TextNext = WizardResources.GetString("WizardNextText");
			this.wizardPageForm.TextNext = WizardResources.GetString("WizardNextText");
			this.wizardPageRun.TextNext = WizardResources.GetString("WizardStartText");
			this.wizardPageFinish.TextFinish = WizardResources.GetString("WizardFinishText");

			this.labelLanguage.Text = WizardResources.GetString("LabelLanguage");
			this.labelCountry.Text = WizardResources.GetString("LabelCountry");
			this.labelForm.Text = WizardResources.GetString("LabelForm");
			this.labelProvider.Text = WizardResources.GetString("LabelProvider");
			this.labelProviderExample.Text = WizardResources.GetString("LabelProviderExample");
			this.labelCity.Text = WizardResources.GetString("LabelCity");
			this.labelInfo.Text = WizardResources.GetString("LabelInfo");
			this.labelFinish.Text = WizardResources.GetString("LabelFinish");
			this.wizardPageRun.TextNext = WizardResources.GetString("WizardStartText");

			this.timeSecondRemaining = WizardResources.GetString("TimeSecondRemaining");
			this.timeSecondsRemaining = WizardResources.GetString("TimeSecondsRemaining");
			this.timeMinuteRemaining = WizardResources.GetString("TimeMinuteRemaining");
			this.timeMinutesRemaining = WizardResources.GetString("TimeMinutesRemaining");
			this.timeMinutesSecondsRemaining = WizardResources.GetString("TimeMinutesSecondsRemaining");
			this.timeSecond = WizardResources.GetString("TimeSecond");
			this.timeSeconds = WizardResources.GetString("TimeSeconds");
			this.timeMinute = WizardResources.GetString("TimeMinute");
			this.timeMinutes = WizardResources.GetString("TimeMinutes");
		}

		/// <summary>
		/// An event handler called when the user closes the wizard.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnClosing(object sender, FormClosingEventArgs e)
		{
			lock (this.sync)
			{
				if (!this.completed)
				{
					// Call the canceling event handler.
					this.OnCanceling(sender, e);
				}
			}
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
					if (MessageBox.Show(this, WizardResources.GetString("CancelMessageText"), WizardResources.GetString("CancelMessageTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						// Set the canceling flag.
						this.canceling = true;
						// Disable the cancel and back buttons.
						this.wizardPageRun.AllowBack = false;
						this.wizardPageRun.AllowCancel = false;
						// Disable the progress timer.
						this.timer.Enabled = false;
						// Update the progress label.
						this.labelProgress.Text = WizardResources.GetString("LabelProgressCancel");
						this.labelTime.Text = string.Empty;
						// Cancel the asynchronous operations.
						this.OnCancel();
					}
					// Cancel the close event.
					e.Cancel = true;
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
			// Cancel the traceroute.
			this.tracerouteCancel.Cancel();

			// Execute the cancellation on the thread pool.
			ThreadPool.QueueUserWorkItem((object state) =>
				{
					// Cancel the web request.
					lock (this.sync)
					{
						if (null != this.asyncWebResult)
						{
							this.asyncWebRequest.Cancel(this.asyncWebResult);
						}
					}
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
		/// An event handler called when the user clicks on the help link.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnHelp(object sender, EventArgs e)
		{
			Process.Start("http://inetanalytics.nets.upf.edu/mercury/faq");
		}

		/// <summary>
		/// An event raised when the user enters on the run page.
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
			// Verify the network connection.
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				// Show an error message.
				MessageBox.Show(this, WizardResources.GetString("ConnectionMessageText"), WizardResources.GetString("ConnectionMessageTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				// Cancel the page change.
				e.Cancel = true;
				// Return.
				return;
			}

			// If the current location is not set.
			if (null == this.location)
			{
				// Start a location request.
				this.SetCountry();
			}

			// If the wizard is not completed.
			if (!this.completed)
			{
				// Cancel the page change.
				e.Cancel = true;
				// Start the wizard session.
				this.OnStartSession();
			}
		}

		/// <summary>
		/// Starts a session
		/// </summary>
		private void OnStartSession()
		{
			// Set the session identifier and timestamp.
			this.sessionId = this.GetSession();
			this.sessionTimestamp = DateTime.Now;

			// Disable the start button.
			this.wizardPageRun.AllowNext = false;
			this.wizardPageRun.AllowBack = false;
			// Update the progress.
			this.progressBar.Visible = true;
			this.progressBar.Style = ProgressBarStyle.Marquee;
			this.labelProgress.Text = WizardResources.GetString("LabelProgressSession");

			lock (this.sync)
			{
				try
				{
					// Reset the wait handle.
					this.waitAsync.Reset();
					
					// Create the web request.
					AsyncWebResult asyncState = AsyncWebRequest.Create(new Uri(FormMain.uriUploadSession), (AsyncWebResult asyncResult) =>
						{
							lock (this.sync)
							{
								this.asyncWebResult = null;
							}

							try
							{
								// Complete the web request.
								this.asyncWebRequest.End(asyncResult);

								// Set the wait handle.
								this.waitAsync.Set();

								this.Invoke(() =>
									{
										// Begin the download of destination web sites.
										this.OnStartDownload();
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
											string.Format(WizardResources.GetString("MercuryMessageText"), Environment.NewLine, Environment.NewLine, exception.Message),
											WizardResources.GetString("MercuryMessageTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
										// Enable the start button.
										this.wizardPageRun.AllowNext = true;
										this.progressBar.Visible = false;
										this.labelProgress.Text = string.Empty;
									});
							}
						}, null);

					// Set the request headers.
					asyncState.Request.Method = "POST";
					asyncState.Request.Accept = "text/html,application/xhtml+xml,application/xml";
					asyncState.Request.ContentType = "application/json;charset=UTF-8";

					// Create the traceroute JSON object.
					JsonObject obj = new JsonObject(
						new JsonProperty("sessionId", this.sessionId.ToString()),
						new JsonProperty("author", "MercuryClient"),
						new JsonProperty("description", "MercuryClient"),
						new JsonProperty("dateStart", this.sessionTimestamp.ToUniversalTime().ToString(@"yyyy-MM-ddTHH:mm:ss.fffZ"))
						);

					string val = obj.ToString();

					// Append the data.
					asyncState.SendData.Append(val, Encoding.UTF8);

					// Execute the requesy.
					this.asyncWebResult = this.asyncWebRequest.Begin(asyncState);
				}
				catch (Exception exception)
				{
					// Set the wait handle.
					this.waitAsync.Set();
					// Show an error message.
					MessageBox.Show(
						this,
						string.Format(WizardResources.GetString("MercuryMessageText"), Environment.NewLine, Environment.NewLine, exception.Message),
						WizardResources.GetString("MercuryMessageTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					// Enable the start button.
					this.wizardPageRun.AllowNext = true;
					this.progressBar.Visible = false;
					this.labelProgress.Text = string.Empty;
				}
			}
		}

		/// <summary>
		/// Starts the wizard download.
		/// </summary>
		private void OnStartDownload()
		{
			// Update the progress.
			this.labelProgress.Text = WizardResources.GetString("LabelProgressDownload");

			// Get the selected country.
			Territory country = this.comboBoxCountry.SelectedItem as Territory;
			// Get the country.
			string countryCode = country != null ? country.Type.ToUpperInvariant() : string.Empty;

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
											string.Format(WizardResources.GetString("MercuryMessageText"), Environment.NewLine, Environment.NewLine, exception.Message),
											WizardResources.GetString("MercuryMessageTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
										// Enable the start button.
										this.wizardPageRun.AllowNext = true;
										this.progressBar.Visible = false;
										this.labelProgress.Text = string.Empty;
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
						string.Format(WizardResources.GetString("MercuryMessageText"), Environment.NewLine, Environment.NewLine, exception.Message),
						WizardResources.GetString("MercuryMessageTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					// Enable the start button.
					this.wizardPageRun.AllowNext = true;
					this.progressBar.Visible = false;
					this.labelProgress.Text = string.Empty;
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
			this.labelProgress.Text = string.Format(WizardResources.GetString("LabelProgressTraceroute"), sites.Length);

			// Initialize the traceroute state.
			this.OnTracerouteInitialize(sites);

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
							if (this.tracerouteRunning.Count == 0)
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
											this.wizardPageRun.AllowBack = true;
											this.progressBar.Visible = false;
											this.labelProgress.Text = string.Empty;
											this.labelTime.Text = string.Empty;
											this.timer.Enabled = false;
											// Switch to the finish page.
											this.wizardControl.NextPage();
										}
									});
							}
							return;
						}

						// Get a site from the pending list.
						info = this.OnTraceroutePendingToRunning();
					}

					// Increment the traceroute information count.
					info.Count++;

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

						// Begin a traceroute for the specified destination.
						TracerouteResult result = this.traceroute.Run(info.Address, this.tracerouteCancel);

						if (this.tracerouteCancel.IsCanceled)
						{
							// Set the traceroute as pending.
							this.OnTracerouteRunningToPending(info);
						}
						else
						{
							// Update the progress.
							this.Invoke(() =>
							{
								lock (this.sync)
								{
									if (!this.tracerouteCancel.IsCanceled)
									{
										this.progressBar.Value = this.tracerouteCompleted.Count;
										this.labelProgress.Text = string.Format(WizardResources.GetString("LabelProgressCompleted"),
											this.tracerouteCompleted.Count,
											this.traceroutePending.Count + this.tracerouteRunning.Count + this.tracerouteCompleted.Count + this.tracerouteFailed.Count);
									}
								}
							});

							// Upload the result.
							bool success = false;

							for (int attempt = 0; (attempt < 3) && (!success); attempt++)
							{
								success = this.OnUploadTraceroute(info, result);
							}

							if (success)
							{
								// Set the traceroute as completed.
								this.OnTracerouteRunningToCompleted(info);
							}
							else
							{
								throw new Exception();
							}
						}
					}
					catch
					{
						// If the traceroute count is less than the maximum retries.
						if (info.Count < FormMain.tracerouteRetries)
						{
							// Add the traceroute to the pending list.
							this.OnTracerouteRunningToPending(info);
						}
						else
						{
							// Add the traceroute to the failed list.
							this.OnTracerouteRunningToFailed(info);
						}
					}
					finally
					{
						// If the wizard is not canceled.
						if (!this.tracerouteCancel.IsCanceled)
						{
							// Start another traceroute.
							this.OnStartTraceroute();
						}
					}
				});
		}

		/// <summary>
		/// Initializes the traceroute state.
		/// </summary>
		private void OnTracerouteInitialize(IEnumerable<string> sites)
		{
			lock (this.sync)
			{
				// Clear the traceroute lists.
				this.traceroutePending.Clear();
				this.tracerouteRunning.Clear();
				this.tracerouteCompleted.Clear();
				this.tracerouteFailed.Clear();

				// Add the sites to the pending list.
				foreach (string site in sites)
				{
					this.traceroutePending.Enqueue(new TracerouteInfo(site));
				}

				// Reset the wait handle.
				this.waitAsync.Reset();
				// Reset the cancellation token.
				this.tracerouteCancel.Reset();
				// Set the traceroute timestamp.
				this.tracerouteTimestamp = DateTime.Now;
				// Enable the timer.
				this.timer.Enabled = true;
			}
		}

		/// <summary>
		/// Changes the state of a traceroute information from pending to running.
		/// </summary>
		/// <returns>The information of the next traceroute in the pending queue.</returns>
		private TracerouteInfo OnTraceroutePendingToRunning()
		{
			lock (this.sync)
			{
				// Get a traceroute from the pending list.
				TracerouteInfo info = this.traceroutePending.Dequeue();
				// If there are no running traceroutes.
				if (this.tracerouteRunning.Count == 0)
				{
					// Reset the wait handle.
					this.waitAsync.Reset();
				}
				// Add the traceroute to the running list.
				this.tracerouteRunning.Add(info);
				// Return the traceroute information.
				return info;
			}
		}

		/// <summary>
		/// Changes the state of a traceroute information from running to pending.
		/// </summary>
		/// <param name="info">The traceroute information.</param>
		private void OnTracerouteRunningToPending(TracerouteInfo info)
		{
			lock (this.sync)
			{
				// Remove the traceroute from the running list.
				this.tracerouteRunning.Remove(info);
				// Add the traceroute to the completed list.
				this.traceroutePending.Enqueue(info);

				// If there are no running traceroutes.
				if (this.tracerouteRunning.Count == 0)
				{
					// Set the wait handle.
					this.waitAsync.Set();
				}
			}
		}

		/// <summary>
		/// Changes the state of a traceroute information from running to completed.
		/// </summary>
		/// <param name="info">The traceroute information.</param>
		private void OnTracerouteRunningToCompleted(TracerouteInfo info)
		{
			lock (this.sync)
			{
				// Remove the traceroute from the running list.
				this.tracerouteRunning.Remove(info);
				// Add the traceroute to the completed list.
				this.tracerouteCompleted.Add(info);
				
				// If there are no running traceroutes.
				if (this.tracerouteRunning.Count == 0)
				{
					// Set the wait handle.
					this.waitAsync.Set();
				}
			}
		}

		/// <summary>
		/// Changes the state of a traceroute information from running to failed.
		/// </summary>
		/// <param name="info">The traceroute information.</param>
		private void OnTracerouteRunningToFailed(TracerouteInfo info)
		{
			lock (this.sync)
			{
				// Remove the traceroute from the running list.
				this.tracerouteRunning.Remove(info);
				// Add the traceroute to the failed list.
				this.tracerouteFailed.Add(info);

				// If there are no running traceroutes.
				if (this.tracerouteRunning.Count == 0)
				{
					// Set the wait handle.
					this.waitAsync.Set();
				}
			}
		}

		/// <summary>
		/// An event handler called when the user rolls-back the finish page.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnFinishRollback(object sender, WizardPageConfirmEventArgs e)
		{
			this.completed = false;
		}

		/// <summary>
		/// Uploads a traceroute to the Mercury web server.
		/// </summary>
		/// <param name="info">The traceroute information.</param>
		/// <param name="result">The traceroute result.</param>
		/// <returns><b>True</b> if the upload was successful, <b>false</b> otherwise.</returns>
		private bool OnUploadTraceroute(TracerouteInfo info, TracerouteResult result)
		{
			try
			{
				// Create a web request.
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FormMain.uriUploadTrace);

				// Set the request headers.
				request.Method = "POST";
				request.Accept = "text/html,application/xhtml+xml,application/xml";
				request.ContentType = "application/json;charset=UTF-8";

				// Create the hops JSON object.
				JsonArray hops = new JsonArray();

				foreach (TracerouteHop hop in result.Hops)
				{
					hops.Add(new JsonObject(
						new JsonProperty("id", hop.TimeToLive.ToString()),
						new JsonProperty("ip", hop.Address != null ? hop.Address.ToString() : "destination unreachable"),
						new JsonProperty("asn", new JsonArray()),
						new JsonProperty("rtt", new JsonArray(hop.AverageRoundtripTime.ToString()))
						));
				}

				// Create the traceroute JSON object.
				JsonObject obj = new JsonObject(
					new JsonProperty("sessionId", this.sessionId.ToString()),
					new JsonProperty("srcIp", this.location != null ? this.location.Address : "none"),
					new JsonProperty("dstIp", result.Destination.ToString()),
					new JsonProperty("srcName", Dns.GetHostName()),
					new JsonProperty("dstName", info.Site),
					new JsonProperty("hops", hops));

				using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
				{
					writer.Write(obj.ToString());
				}

				// Execute the request.
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					return response.StatusCode == HttpStatusCode.OK;
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// An event handler called when the timer expires.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnTimer(object sender, EventArgs e)
		{
			int pending;
			int completed;

			lock (this.sync)
			{
				// Compute the number of completed traceroutes.
				pending = this.traceroutePending.Count + this.tracerouteRunning.Count;
				completed = this.tracerouteCompleted.Count + this.tracerouteFailed.Count;
			}

			// If any of the completed 
			if (0 == completed)
			{
				// Clear the time remaining label.
				this.labelTime.Text = string.Empty;
			}
			else
			{
				// Compute the elapsed time.
				TimeSpan elapsedTime = DateTime.Now - this.tracerouteTimestamp;
				// Compute the remaining time.
				TimeSpan remainingTime = TimeSpan.FromTicks(elapsedTime.Ticks * pending / completed);
				// Set the label.
				this.labelTime.Text = this.GetDuration(remainingTime);
			}
		}

		/// <summary>
		/// Generates the session identifier.
		/// </summary>
		/// <returns>The session identifier.</returns>
		private string GetSession()
		{
			// Get the country.
			Territory country = this.comboBoxCountry.SelectedItem as Territory;

			// Create the identifier.
			return string.Format("{0}-{1}-{2}",
				country != null ? country.Type : string.Empty,
				this.textBoxCity.Text,
				this.textBoxProvider.Text);
		}

		/// <summary>
		/// Returns the string for the specified duration.
		/// </summary>
		/// <param name="duration">The duration.</param>
		/// <returns>The duration string.</returns>
		private string GetDuration(TimeSpan duration)
		{
			// Get the number of minutes.
			int minutes = (int)Math.Round(duration.TotalSeconds) / 60;
			int seconds = (int)Math.Round(duration.TotalSeconds) - minutes * 60;

			if (minutes > 0 && seconds > 0)
			{
				return string.Format(this.timeMinutesSecondsRemaining,
					minutes, minutes == 1 ? this.timeMinute : this.timeMinutes, seconds, seconds == 1 ? this.timeSecond : this.timeSeconds);
			}
			else if (minutes > 0)
			{
				return minutes == 1 ? string.Format(this.timeMinuteRemaining, minutes) : string.Format(this.timeMinutesRemaining, minutes);
			}
			else if (seconds > 0)
			{
				return seconds == 1 ? string.Format(this.timeSecondRemaining, seconds) : string.Format(this.timeSecondsRemaining, seconds);
			}
			else return string.Empty;
		}

		/// <summary>
		/// An event handler called when the city has changed.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnCityChanged(object sender, EventArgs e)
		{
			this.cityUser = true;
		}
	}
}
