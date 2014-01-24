/** 
 * Copyright (C) 2014 Alex Bikfalvi
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

package edu.upf.mercury.client;

import java.awt.AWTEvent;
import java.awt.CardLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.EventQueue;
import java.awt.Font;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.net.InetAddress;
import java.nio.charset.Charset;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.Locale;

import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JProgressBar;
import javax.swing.JTextField;
import javax.swing.JTextPane;
import javax.swing.Timer;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;
import javax.swing.border.EmptyBorder;
import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;

import com.bikfalvi.java.globalization.CultureCollection;
import com.bikfalvi.java.globalization.CultureReader;
import com.bikfalvi.java.globalization.Language;
import com.bikfalvi.java.globalization.Territory;
import com.bikfalvi.java.json.JsonArray;
import com.bikfalvi.java.json.JsonObject;
import com.bikfalvi.java.json.JsonProperty;
import com.bikfalvi.java.net.core.Traceroute;
import com.bikfalvi.java.net.core.TracerouteHop;
import com.bikfalvi.java.net.core.TracerouteResult;
import com.bikfalvi.java.net.core.TracerouteSettings;
import com.bikfalvi.java.threading.CancellationToken;
import com.bikfalvi.java.threading.ManualResetEvent;
import com.bikfalvi.java.threading.ThreadPool;
import com.bikfalvi.java.web.WebCallback;
import com.bikfalvi.java.web.WebRequest;
import com.bikfalvi.java.web.WebResult;
import com.bikfalvi.java.web.WebState;
import com.bikfalvi.java.web.location.LocationRequest;
import com.bikfalvi.java.web.location.LocationResult;

import edu.upf.mercury.client.resources.Resources;
import edu.upf.mercury.client.wizard.Wizard;
import edu.upf.mercury.client.wizard.WizardEvent;
import edu.upf.mercury.client.wizard.WizardListener;
import edu.upf.mercury.client.wizard.WizardPage;

/**
 * A class representing the Mercury client main frame.
 * @author Alex Bikfalvi
 *
 */
public class FrameMain extends JFrame {
	/**
	 * A class representing the information for a traceroute.
	 * @author Alex Bikfalvi
	 *
	 */
	private final class TracerouteInfo
	{
		private String site;
		private InetAddress address = null;
		private int count = 0;
		
		/**
		 * Creates a new traceroute information instance.
		 * @param site The destination site.
		 */
		public TracerouteInfo(String site)
		{
			this.site = site;
		}

		/**
		 * Gets the site for this traceroute.
		 * @return The site.
		 */
		public String getSite() {
			return site;
		}

		/**
		 * Gets the Internet address for this traceroute.
		 * @return The address.
		 */
		public InetAddress getAddress() {
			return address;
		}

		/**
		 * Sets the Internet address for this traceroute.
		 * @param address The address.
		 */
		public void setAddress(InetAddress address) {
			this.address = address;
		}

		/**
		 * Gets the attempts for this traceroute.
		 * @return The attempt count.
		 */
		public int getCount() {
			return count;
		}

		/**
		 * Sets the attempts for this traceroute.
		 * @param count The attempt count.
		 */
		public void setCount(int count) {
			this.count = count;
		}		
	}	
	
	/**
	 * Private variables. 
	 */
	private static final long serialVersionUID = 1597877587805876042L;
	private final Object sync = new Object();
	
	private static final String[] locales = { "ca", "de", "en", "es", "fr", "pt", "ro" };
	private static CultureCollection cultures;
	private static Resources translation;
	
	private final ArrayList<Language> languages = new ArrayList<Language>();
	private final ArrayList<Territory> territories = new ArrayList<Territory>();
	
	private static final String uriGetUrls = "http://inetanalytics.nets.upf.edu/getUrls?countryCode=%s";
	private static final String uriUploadSession = "http://mercury.upf.edu/mercury/api/traceroute/addTracerouteSession";
	//private static final String uriUploadTrace = "http://mercury.upf.edu/mercury/api/traceroute/uploadTrace";
	
	private final Wizard wizard;
	
	private boolean completed = false;
	private boolean canceling = false;
	private boolean canceled = false;

	private boolean countryUser = false;
	private boolean cityUser = false;
	
	private final String localHostname;
	private final LocationRequest locationRequest = new LocationRequest();
	private LocationResult locationResult = null;

	private final WebRequest webRequest = new WebRequest();
	
	private static final int tracerouteConcurrent = 20;
	private static final int tracerouteRetries = 3;

	private final Traceroute traceroute;
	private final TracerouteSettings tracerouteSettings;
	private final LinkedList<TracerouteInfo> traceroutePending = new LinkedList<TracerouteInfo>();
	private final HashSet<TracerouteInfo> tracerouteRunning = new HashSet<TracerouteInfo>();
	private final HashSet<TracerouteInfo> tracerouteCompleted = new HashSet<TracerouteInfo>();
	private final HashSet<TracerouteInfo> tracerouteFailed = new HashSet<TracerouteInfo>();
	private final CancellationToken tracerouteCancel = new CancellationToken(); 
	private Date tracerouteTimestamp;
	
	private Date sessionTimestamp;
	private String sessionId;
	
	private final Timer timer;
	
	private final ManualResetEvent waitAsync = new ManualResetEvent(true);
	
	private String timeSecondRemaining;
	private String timeSecondsRemaining;
	private String timeMinuteRemaining;
	private String timeMinutesRemaining;
	private String timeMinutesSecondsRemaining;
	private String timeSecond;
	private String timeSeconds;
	private String timeMinute;
	private String timeMinutes;	
	
	private JPanel contentPane;
	private JTextField textFieldCity;
	private JComboBox comboBoxLanguage;
	private JComboBox comboBoxCountry;
	private JLabel labelCountryBusy;
	private JLabel labelTitle;
	private JLabel labelLanguage;
	private JLabel labelCountry;
	private JButton buttonBack;
	private JButton buttonNext;
	private JButton buttonCancel;
	private JPanel panelWizard;
	private WizardPage pageLocale;
	private WizardPage pageForm;
	private WizardPage pageRun;
	private WizardPage pageFinish;
	private JTextPane textForm;
	private JLabel labelProvider;
	private JLabel labelCity;
	private JTextPane labelProviderExample;
	private JTextField textFieldProvider;
	private JTextPane textInfo;
	private JTextPane textProgress;
	private JTextPane textTime;
	private JTextPane textFinish;
	private JProgressBar progressBar;
	
	/**
	 * Launch the application.
	 * @throws IOException 
	 * @throws SAXException 
	 * @throws ParserConfigurationException 
	 */
	public static void main(String[] args) throws IOException, ParserConfigurationException, SAXException {
		// Load the cultures.
		FrameMain.loadCultures();
		
		// Load the translations.
		FrameMain.loadTranslation();
		
		// Start the main application.
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				// Set the look and feel.
				try {
					UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
				}
				catch (Exception e) {
					
				}
				// Create the main frame.
				FrameMain frame = new FrameMain();
				// Show the main frame.
				frame.setVisible(true);
				
				// Set the languages.
				frame.setLanguages();
				// Set the countries.
				frame.setCountries();
			}
		});
	}

	/**
	 * Create the frame.
	 * @throws UnsupportedLookAndFeelException 
	 * @throws IllegalAccessException 
	 * @throws InstantiationException 
	 * @throws ClassNotFoundException 
	 */
	public FrameMain() {
		final FrameMain frame = this;

		addWindowListener(new WindowAdapter() {
			@Override
			public void windowClosing(WindowEvent e) {
				frame.onClosing(e);
			}
		});
		
		setResizable(false);
		setTitle("Mercury Client");
		setIconImage(Toolkit.getDefaultToolkit().getImage(FrameMain.class.getResource("/edu/upf/mercury/client/resources/GraphBarColor_16.png")));
		setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
		setBounds(100, 100, 600, 450);
		contentPane = new JPanel();
		contentPane.setBackground(Color.WHITE);
		contentPane.setBorder(new EmptyBorder(5, 5, 5, 5));
		setContentPane(contentPane);
		contentPane.setLayout(null);
		
		buttonCancel = new JButton("Cancel");
		buttonCancel.setMnemonic('C');
		buttonCancel.setBounds(495, 388, 89, 23);
		contentPane.add(buttonCancel);
		
		buttonNext = new JButton("Next");
		buttonNext.setMnemonic('N');
		buttonNext.setBounds(396, 388, 89, 23);
		contentPane.add(buttonNext);
		
		buttonBack = new JButton("Back");
		buttonBack.setMnemonic('B');
		buttonBack.setBounds(297, 388, 89, 23);
		contentPane.add(buttonBack);
		
		panelWizard = new JPanel();
		panelWizard.setBackground(Color.WHITE);
		panelWizard.setBounds(0, 86, 594, 291);
		contentPane.add(panelWizard);
		panelWizard.setLayout(new CardLayout(0, 0));
		
		pageLocale = new WizardPage();
		pageLocale.setAllowNext(false);
		pageLocale.setAllowBack(false);
		pageLocale.setName("pageLocale");
		pageLocale.setBackground(Color.WHITE);
		panelWizard.add(pageLocale, "pageLocale");
		pageLocale.setLayout(null);
		
		labelLanguage = new JLabel("Language:");
		labelLanguage.setDisplayedMnemonic('L');
		labelLanguage.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelLanguage.setBounds(10, 111, 130, 20);
		pageLocale.add(labelLanguage);
		
		comboBoxLanguage = new JComboBox();
		comboBoxLanguage.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent arg0) {
				frame.onLanguageChanged();
			}
		});
		labelLanguage.setLabelFor(comboBoxLanguage);
		comboBoxLanguage.setBounds(150, 112, 300, 20);
		pageLocale.add(comboBoxLanguage);
		
		comboBoxCountry = new JComboBox();
		comboBoxCountry.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				frame.onCountryChanged();
			}
		});
		comboBoxCountry.setBounds(150, 143, 300, 20);
		pageLocale.add(comboBoxCountry);
		
		labelCountry = new JLabel("Country:");
		labelCountry.setLabelFor(comboBoxCountry);
		labelCountry.setDisplayedMnemonic('o');
		labelCountry.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelCountry.setBounds(10, 142, 130, 20);
		pageLocale.add(labelCountry);
		
		labelCountryBusy = new JLabel("");
		labelCountryBusy.setVisible(false);
		labelCountryBusy.setIconTextGap(0);
		labelCountryBusy.setVerifyInputWhenFocusTarget(false);
		labelCountryBusy.setPreferredSize(new Dimension(48, 48));
		labelCountryBusy.setMinimumSize(new Dimension(48, 48));
		labelCountryBusy.setMaximumSize(new Dimension(48, 48));
		labelCountryBusy.setIcon(new ImageIcon(FrameMain.class.getResource("/edu/upf/mercury/client/resources/Busy.gif")));
		labelCountryBusy.setBounds(460, 130, 48, 48);
		pageLocale.add(labelCountryBusy);
		
		pageForm = new WizardPage();
		pageForm.setName("pageForm");
		pageForm.setBackground(Color.WHITE);
		panelWizard.add(pageForm, "pageForm");
		pageForm.setLayout(null);
		
		textForm = new JTextPane();
		textForm.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textForm.setText("We use this to learn more about your Internet connection. We do not collect any personal information without your consent, and all fields are optional.");
		textForm.setBounds(10, 11, 574, 50);
		pageForm.add(textForm);
		
		labelProvider = new JLabel("Provider:");
		labelProvider.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelProvider.setDisplayedMnemonic('P');
		labelProvider.setBounds(10, 109, 130, 20);
		pageForm.add(labelProvider);
		
		labelCity = new JLabel("City:");
		labelCity.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelCity.setDisplayedMnemonic('t');
		labelCity.setBounds(10, 161, 130, 20);
		pageForm.add(labelCity);
		
		textFieldProvider = new JTextField();
		textFieldProvider.setBounds(150, 110, 300, 20);
		pageForm.add(textFieldProvider);
		textFieldProvider.setColumns(10);
		
		textFieldCity = new JTextField();
		textFieldCity.setBounds(150, 162, 300, 20);
		pageForm.add(textFieldCity);
		textFieldCity.setColumns(10);
		
		labelProviderExample = new JTextPane();
		labelProviderExample.setText("Example: AT&T, Comcast, Verizon, or the name of your company.");
		labelProviderExample.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelProviderExample.setBounds(150, 131, 434, 20);
		pageForm.add(labelProviderExample);
		
		pageRun = new WizardPage();
		pageRun.setName("pageRun");
		pageRun.setBackground(Color.WHITE);
		panelWizard.add(pageRun, "pageRun");
		pageRun.setLayout(null);
		
		textInfo = new JTextPane();
		textInfo.setText("The wizard is ready to start the Internet measurements. This may take several minutes, depending on the speed of your Internet connection. To continue, click Start.");
		textInfo.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textInfo.setBounds(10, 11, 574, 50);
		pageRun.add(textInfo);
		
		textTime = new JTextPane();
		textTime.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textTime.setBounds(10, 230, 574, 50);
		pageRun.add(textTime);
		
		textProgress = new JTextPane();
		textProgress.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textProgress.setBounds(10, 169, 574, 50);
		pageRun.add(textProgress);
		
		progressBar = new JProgressBar();
		progressBar.setVisible(false);
		progressBar.setBounds(10, 142, 574, 16);
		pageRun.add(progressBar);
		
		pageFinish = new WizardPage();
		pageFinish.setAllowCancel(false);
		pageFinish.setName("pageFinish");
		pageFinish.setBackground(Color.WHITE);
		panelWizard.add(pageFinish, "pageFinish");
		pageFinish.setLayout(null);
		
		textFinish = new JTextPane();
		textFinish.setText("The wizard has finished the Internet measurements.\r\n\r\nTo close, click on Finish.");
		textFinish.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textFinish.setBounds(10, 11, 574, 100);
		pageFinish.add(textFinish);
		
		JLabel labelLogo = new JLabel("New label");
		labelLogo.setIcon(new ImageIcon(FrameMain.class.getResource("/edu/upf/mercury/client/resources/GraphBarColor_64.png")));
		labelLogo.setBounds(10, 11, 64, 64);
		contentPane.add(labelLogo);
		
		labelTitle = new JLabel("Select your language and country");
		labelTitle.setForeground(new Color(19, 112, 171));
		labelTitle.setFont(new Font("Segoe UI", Font.PLAIN, 16));
		labelTitle.setBounds(84, 30, 500, 23);
		contentPane.add(labelTitle);
		
		// Set the local hostname.
		String hostname;
		try {
			hostname = InetAddress.getLocalHost().getHostName();
		}
		catch (Exception exception) {
			hostname = "none";
		}
		this.localHostname = hostname;
		
		// Create the wizard.
		this.wizard = new Wizard(this.panelWizard, this.labelTitle, this.buttonBack, this.buttonNext, this.buttonCancel,
				new WizardPage[] { this.pageLocale, this.pageForm, this.pageRun, this.pageFinish });
		this.wizard.setFinishedListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				frame.onFinished(e);
			}
		});
		this.wizard.setCancelListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				frame.onCanceling(e);
			}
		});
		this.pageRun.setCommitListener(new WizardListener() {
			@Override
			public void actionPerformed(WizardEvent e) {
				frame.onRunCommit(e);
			}
		});
		this.pageFinish.setRollbackListener(new WizardListener() {
			@Override
			public void actionPerformed(WizardEvent e) {
				frame.onFinishRollback(e);
			}
		});
		
		// Create the traceroute settings.
		this.tracerouteSettings = new TracerouteSettings();

		// Create the traceroute.
		this.traceroute = new Traceroute(this.tracerouteSettings);
		
		// Create the timer.
		this.timer = new Timer(1000, new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				frame.onTimer(e);
			}
		});
	}
	
	/**
	 * Loads the application cultures.
	 * @throws IOException 
	 * @throws SAXException 
	 * @throws ParserConfigurationException 
	 */
	private static void loadCultures() throws ParserConfigurationException, SAXException, IOException {
		InputStream stream = (InputStream) FrameMain.class.getResource("/edu/upf/mercury/client/resources/Cultures.xml").getContent();
		CultureReader reader = new CultureReader(stream);
		FrameMain.cultures = reader.readCultureCollection();
	}
	
	/**
	 * Loads the applications translation.
	 * @throws IOException 
	 * @throws SAXException 
	 * @throws ParserConfigurationException 
	 */
	private static void loadTranslation() throws IOException, ParserConfigurationException, SAXException {
		InputStream stream = (InputStream) FrameMain.class.getResource("/edu/upf/mercury/client/resources/Translation.xml").getContent();
		FrameMain.translation = new Resources(stream);
	}
	
	/**
	 * Sets the languages.
	 */
	private void setLanguages() {		
		// The current language.
		Language currentLanguage = null;
		
		// Create the languages.
		this.languages.clear();
		for (String locale : FrameMain.locales)
		{
			Language language = FrameMain.cultures.getCulture(locale).getLanguages().getLanguage(locale);
			this.languages.add(language);
			if (language.equals(Locale.getDefault().getLanguage()))
			{
				currentLanguage = language;
			}
		}
		
		// Sort the languages by name.
		Collections.sort(this.languages, new Comparator<Language>() {
			@Override
			public int compare(Language left, Language right) {
				return left.getName().compareTo(right.getName());
			}
		});

		// Add the languages to the language combo box.
		this.comboBoxLanguage.removeAllItems();
		for (Language language : this.languages) {
			this.comboBoxLanguage.addItem(language);
		}
		
		// Select the current language, if exists.
		for (int index = 0; index < this.comboBoxLanguage.getItemCount(); index++) {
			Object item = this.comboBoxLanguage.getItemAt(index);
			if (item.equals(currentLanguage)) {
				this.comboBoxLanguage.setSelectedIndex(index);
				index = this.comboBoxLanguage.getItemCount();
			}
		}

		// Reset the user country selection.
		this.countryUser = false;
	}
	
	/**
	 * Sets the countries.
	 */
	private void setCountries() {
		if (!this.countryUser) {
			this.labelCountryBusy.setVisible(true);
		}
		
		// Set the super class.
		final FrameMain frame = this;
		final LocationRequest request = this.locationRequest;
		
		// Show the busy picture.
		this.labelCountryBusy.setVisible(true);
				
		try {
			// Begin an asynchronous request for the current location.
			this.locationRequest.beginLocationRequest(new WebCallback() {
				@Override
				public void callback(final WebResult result) {
					// Update the location.
					EventQueue.invokeLater(new Runnable() {
						@Override
						public void run() {
							try {
								// End the asynchronous request for the current location.
								frame.setLocation(request.endLocationRequest(result));
								// Set the country.
								frame.setCountry();
							} catch (IOException e) {
							} catch (ParserConfigurationException e) {
							} catch (SAXException e) {
							}
						}
					});
				}
			});
		} catch (IOException e) {
		}
	}
	
	/**
	 * Sets the specified language as the user interface language.
	 * @param language The language.
	 */
	private void setLanguage(Language language) {
		// Sets the specified language as the current locale.
		Locale.setDefault(new Locale(language.getType()));
		
		// Set the user interface.
		this.pageLocale.setTitle(FrameMain.translation.get("PageLocaleTitle"));
		this.pageForm.setTitle(FrameMain.translation.get("PageFormTitle"));
		this.pageRun.setTitle(FrameMain.translation.get("PageRunTitle"));
		this.pageFinish.setTitle(FrameMain.translation.get("PageFinishTitle"));

		this.pageLocale.setTextBack(FrameMain.translation.get("WizardBackText", true));
		this.pageForm.setTextBack(FrameMain.translation.get("WizardBackText", true));
		this.pageRun.setTextBack(FrameMain.translation.get("WizardBackText", true));
		this.pageFinish.setTextBack(FrameMain.translation.get("WizardBackText", true));
		
		this.pageLocale.setTextNext(FrameMain.translation.get("WizardNextText", true));
		this.pageForm.setTextNext(FrameMain.translation.get("WizardNextText", true));
		this.pageRun.setTextNext(FrameMain.translation.get("WizardStartText", true));
		this.pageFinish.setTextNext(FrameMain.translation.get("WizardFinishText", true));
		
		this.pageLocale.setTextCancel(FrameMain.translation.get("WizardCancelText", true));
		this.pageForm.setTextCancel(FrameMain.translation.get("WizardCancelText", true));
		this.pageRun.setTextCancel(FrameMain.translation.get("WizardCancelText", true));
		this.pageFinish.setTextCancel(FrameMain.translation.get("WizardCancelText", true));
		
		this.pageLocale.setMnemonicBack(FrameMain.translation.getMnemonic("WizardBackText"));
		this.pageForm.setMnemonicBack(FrameMain.translation.getMnemonic("WizardBackText"));
		this.pageRun.setMnemonicBack(FrameMain.translation.getMnemonic("WizardBackText"));
		this.pageFinish.setMnemonicBack(FrameMain.translation.getMnemonic("WizardBackText"));
		
		this.pageLocale.setMnemonicNext(FrameMain.translation.getMnemonic("WizardNextText"));
		this.pageForm.setMnemonicNext(FrameMain.translation.getMnemonic("WizardNextText"));
		this.pageRun.setMnemonicNext(FrameMain.translation.getMnemonic("WizardNextText"));
		this.pageFinish.setMnemonicNext(FrameMain.translation.getMnemonic("WizardNextText"));
		
		this.pageLocale.setMnemonicCancel(FrameMain.translation.getMnemonic("WizardCancelText"));
		this.pageForm.setMnemonicCancel(FrameMain.translation.getMnemonic("WizardCancelText"));
		this.pageRun.setMnemonicCancel(FrameMain.translation.getMnemonic("WizardCancelText"));
		this.pageFinish.setMnemonicCancel(FrameMain.translation.getMnemonic("WizardCancelText"));
		
		this.labelLanguage.setText(FrameMain.translation.get("LabelLanguage", true));
		this.labelLanguage.setDisplayedMnemonicIndex(FrameMain.translation.getMnemonic("LabelLanguage"));
		
		this.labelCountry.setText(FrameMain.translation.get("LabelCountry", true));
		this.labelCountry.setDisplayedMnemonicIndex(FrameMain.translation.getMnemonic("LabelCountry"));
		
		this.textForm.setText(FrameMain.translation.get("LabelForm"));
		
		this.labelProvider.setText(FrameMain.translation.get("LabelProvider", true));
		this.labelProvider.setDisplayedMnemonic(FrameMain.translation.getMnemonic("LabelProvider"));
		
		this.labelProviderExample.setText(FrameMain.translation.get("LabelProviderExample"));
		
		this.labelCity.setText(FrameMain.translation.get("LabelCity", true));
		this.labelCity.setDisplayedMnemonic(FrameMain.translation.getMnemonic("LabelCity"));
		
		this.textInfo.setText(FrameMain.translation.get("LabelInfo"));
		this.textFinish.setText(FrameMain.translation.get("LabelFinish"));
		
		this.timeSecondRemaining = FrameMain.translation.get("TimeSecondRemaining");
		this.timeSecondsRemaining = FrameMain.translation.get("TimeSecondsRemaining");
		this.timeMinuteRemaining = FrameMain.translation.get("TimeMinuteRemaining");
		this.timeMinutesRemaining = FrameMain.translation.get("TimeMinutesRemaining");
		this.timeMinutesSecondsRemaining = FrameMain.translation.get("TimeMinutesSecondsRemaining");
		this.timeSecond = FrameMain.translation.get("TimeSecond");
		this.timeSeconds = FrameMain.translation.get("TimeSeconds");
		this.timeMinute = FrameMain.translation.get("TimeMinute");
		this.timeMinutes = FrameMain.translation.get("TimeMinutes");
	}
	
	/**
	 * Sets the specified location as the current location.
	 * @param location The location.
	 */
	private void setLocation(LocationResult location) {
		this.locationResult = location;
	}
	
	/**
	 * Sets the country from the current location as the selected country.
	 */
	private void setCountry() {
		// Hide the busy picture label.
		this.labelCountryBusy.setVisible(false);
		
		// If the location result is not null.
		if (null != this.locationResult) {
			// If the user did not select a country.
			if (!this.countryUser) {
				// Create a new territory for the current country.
				Territory territory = new Territory(this.locationResult.getCountryCode(), this.locationResult.getCountryName());
				// Select the current country, if it exists.
				for (int index = 0; index < this.comboBoxCountry.getItemCount(); index++) {
					Object item = this.comboBoxCountry.getItemAt(index);
					if (item.equals(territory)) {
						this.comboBoxCountry.setSelectedIndex(index);
						index = this.comboBoxCountry.getItemCount();
					}
				}									
			}
			// If the user did not select a city.
			if (!this.cityUser) {
				// Select the current city.
				this.textFieldCity.setText(this.locationResult.getCity());
			}							
		}		
	}
	
	/**
	 * An event handler called when the selected language has changed.
	 */
	private void onLanguageChanged() {
		// Get the selected language.
		Language language = (Language) this.comboBoxLanguage.getSelectedItem();
		
		// If the current language is null, do nothing.
		if (null == language) return;
		
		// Clear the territories.
		this.territories.clear();
		for (Territory territory : FrameMain.cultures.getCulture(language).getTerritories()) {
			if (territory.getType().matches("[(A-Z|a-z)][(A-Z|a-z)]")) {
				this.territories.add(territory);
			}
		}
		
		// Sort the territories by name.
		Collections.sort(this.territories, new Comparator<Territory>() {
			@Override
			public int compare(Territory left, Territory right) {
				return left.getName().compareTo(right.getName());
			}
		});
		
		// Save the selected country.
		Territory selectedCountry = (Territory) this.comboBoxCountry.getSelectedItem();
		
		// Clear the countries.
		this.comboBoxCountry.removeAllItems();
		for (Territory territory : this.territories) {
			this.comboBoxCountry.addItem(territory);
		}
		
		// Select the current country, if exists.
		for (int index = 0; index < this.comboBoxCountry.getItemCount(); index++) {
			Object item = this.comboBoxCountry.getItemAt(index);
			if (item.equals(selectedCountry)) {
				this.comboBoxCountry.setSelectedIndex(index);
				index = this.comboBoxCountry.getItemCount();
			}
		}
		
		// Set the language.
		this.setLanguage(language);
		
		// Call the country changed listener.
		this.onCountryChanged();		
	}
	
	/**
	 * An event handler called when the selected country has changed.
	 */
	private void onCountryChanged() {
		// Enable the next button.
		this.pageLocale.setAllowNext(this.comboBoxCountry.getSelectedIndex() >= 0);

		// Set the user country selection.
		this.countryUser = true;		
	}
	
	/**
	 * An event handler called when the window is closing.
	 * @param e The event.
	 */
	private void onClosing(WindowEvent e) {
		synchronized (this.sync) {
			if (!this.completed) {
				// Call the canceling event handler.
				if (!this.onCanceling(e)) {
					// If the wizard is not canceled, return.
					return;
				}
			}
			// Else, close the application.
			System.exit(0);
		}		
	}
	
	/**
	 * An event handler called when the wizard is canceling.
	 * @param e The event.
	 * @return True if the the wizard is canceled, false otherwise.
	 */
	private boolean onCanceling(AWTEvent e) {
		synchronized (this.sync) {
			// If the wizard is already canceled, return true.
			if (this.canceled) return true;
			// If the wizard is already canceling.
			if (this.canceling) return false;

			// If there are pending operations.
			if (this.tracerouteRunning.size() > 0) {
				// Ask the user whether to cancel.
				if (JOptionPane.showConfirmDialog(this, FrameMain.translation.get("CancelMessageText"), FrameMain.translation.get("CancelMessageTitle"), JOptionPane.YES_NO_OPTION, JOptionPane.QUESTION_MESSAGE) == JOptionPane.YES_OPTION) {
					// Set the canceling flag.
					this.canceling = true;
					// Disable the cancel and back buttons.
					this.pageRun.setAllowBack(false);
					this.pageRun.setAllowCancel(false);
					// Disable the progress timer.
					this.timer.stop();
					// Update the progress label.
					this.textProgress.setText(FrameMain.translation.get("LabelProgressCancel"));
					this.textTime.setText("");
					// Cancel the asynchronous operations.
					this.onCancel();
				}
				// Return false.
				return false;
			}
			else {
				// Set the canceled to true.
				this.canceled = true;
			}
			
			// Close the application.
			System.exit(0);
			
			// Return false.
			return true;
		}
	}
	
	/**
	 * Cancels all pending asynchronous operations.
	 */
	private void onCancel()
	{
		// Cancel the traceroute.
		this.tracerouteCancel.cancel();
		
		// Get the main frame.
		final FrameMain frame = this;

		// Execute the cancellation on the thread pool.
		ThreadPool.execute(new Runnable() {
			@Override
			public void run() {
				try {
					// Wait for the asynchronous handle.
					frame.waitAsync.waitOne();
				} catch (InterruptedException e) {
				}
				
				// Close the form.
				System.exit(0);
			}
		});
	}
	
	/**
	 * An event handler called when the user clicks the finish button.
	 * @param e The event.
	 */
	private void onFinished(AWTEvent e) {
		// Exit the application.
		System.exit(0);
	}
	
	/**
	 * An event raised when the user clicks on the run page commit button.
	 * @param e The event.
	 */
	private void onRunCommit(WizardEvent e) {
		// If the current location is not set.
		if (null == this.locationResult) {
			// Start a location request.
			this.setCountry();
		}

		// If the wizard is not completed.
		if (!this.completed) {
			// Cancel the page change.
			e.cancel();
			// Start the wizard session.
			this.onStartSession();
		}
	}
	
	/**
	 * Starts a session.
	 */
	private void onStartSession()
	{
		// Set the session identifier and timestamp.
		this.sessionId = this.getSession();
		this.sessionTimestamp = new Date();

		// Disable the start button.
		this.pageRun.setAllowNext(false);
		this.pageRun.setAllowBack(false);
		// Update the progress.
		this.progressBar.setVisible(true);
		this.progressBar.setValue(0);
		this.textProgress.setText(FrameMain.translation.get("LabelProgressSession"));

		synchronized (this.sync) {
			try {
				// Reset the wait handle.
				this.waitAsync.reset();

				// Create the traceroute JSON object.
				JsonObject obj = new JsonObject(
					new JsonProperty("sessionId", this.sessionId.toString()),
					new JsonProperty("author", "MercuryClient"),
					new JsonProperty("description", "MercuryClient"),
					new JsonProperty("dateStart", (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssZ")).format(this.sessionTimestamp))
					);
				
				// Create the web state for the upload session URL.
				final WebState state = new WebState(FrameMain.uriUploadSession);
				
				state.getConnection().setRequestMethod("POST");
				state.getConnection().setRequestProperty("Accept", "text/html,application/xhtml+xml,application/xml");
				state.getConnection().setRequestProperty("Content-Type", "application/json;charset=UTF-8");
				state.setData(obj.toString(), Charset.forName("UTF-8"));
				
				// The frame.
				final FrameMain frame = this;

				// Execute the request.
				this.webRequest.begin(state, new WebCallback() {
					@Override
					public void callback(WebResult result) {
						try {
							// Complete the web request.
							frame.webRequest.end(result);
	
							// Set the wait handle.
							frame.waitAsync.set();
	
							// Start the download.
							EventQueue.invokeLater(new Runnable() {
								@Override
								public void run() {
									// Begin the download of destination web sites.
									frame.onStartDownload();
								}
							});
						}
						catch (final Exception exception)
						{
							// Set the wait handle.
							frame.waitAsync.set();
							
							EventQueue.invokeLater(new Runnable() {
								@Override
								public void run() {
									// Show an error message.
									JOptionPane.showMessageDialog(
										frame,
										String.format(FrameMain.translation.get("MercuryMessageText"), exception.getMessage()),
										FrameMain.translation.get("MercuryMessageTitle"),
										JOptionPane.ERROR_MESSAGE);
									// Enable the start button.
									frame.pageRun.setAllowNext(true);
									frame.progressBar.setVisible(false);
									frame.textProgress.setText("");
								}
							});
						}
						
					}
				});				
			} catch (Exception exception) {
				// Set the wait handle.
				this.waitAsync.set();
				// Show an error message.
				JOptionPane.showMessageDialog(
						this,
						String.format(FrameMain.translation.get("MercuryMessageText"), exception.getMessage()),
						FrameMain.translation.get("MercuryMessageTitle"),
						JOptionPane.ERROR_MESSAGE);
				// Enable the start button.
				this.pageRun.setAllowNext(true);
				this.progressBar.setVisible(false);
				this.textProgress.setText("");
			}
		}
	}
	
	/**
	 * Starts the download of web sites.
	 */
	private void onStartDownload()
	{
		// Update the progress.
		this.textProgress.setText(FrameMain.translation.get("LabelProgressDownload"));

		// Get the selected country.
		Territory country = (Territory) this.comboBoxCountry.getSelectedItem();
		// Get the country.
		String countryCode = country != null ? country.getType().toUpperCase() : "";

		// The frame.
		final FrameMain frame = this;
		
		synchronized (this.sync)
		{
			try
			{
				// Reset the wait handle.
				this.waitAsync.reset();
				// Download the web destinations.
				this.webRequest.begin(String.format(FrameMain.uriGetUrls, countryCode), new WebCallback() {
					@Override
					public void callback(WebResult result) {
						try {
							// Complete the web request.
							WebResult webResult = frame.webRequest.end(result);
							
							// Parse the list of sites.
							final ArrayList<String> sites = new ArrayList<String>();
							for (String site : webResult.getResponseDataAsString().split("(\n|\r| )")) {
								if (!site.isEmpty()) {
									sites.add(site);
								}
							}

							// Set the wait handle.
							frame.waitAsync.set();

							EventQueue.invokeLater(new Runnable() {
								@Override
								public void run() {
									// Begin the traceroutes.
									frame.onStartTraceroute(sites);
								}
							});
						}
						catch (final Exception exception) {
							// Set the wait handle.
							frame.waitAsync.set();

							EventQueue.invokeLater(new Runnable() {
								@Override
								public void run() {
									// Show an error message.
									JOptionPane.showMessageDialog(
										frame,
										String.format(FrameMain.translation.get("MercuryMessageText"), exception.getMessage()),
										FrameMain.translation.get("MercuryMessageTitle"),
										JOptionPane.ERROR_MESSAGE);
									// Enable the start button.
									frame.pageRun.setAllowNext(true);
									frame.progressBar.setVisible(false);
									frame.textProgress.setText("");
								}
							});
						}
					}
				});
			}
			catch (Exception exception) {
				// Set the wait handle.
				this.waitAsync.set();
				// Show an error message.
				JOptionPane.showMessageDialog(
						this,
						String.format(FrameMain.translation.get("MercuryMessageText"), exception.getMessage()),
						FrameMain.translation.get("MercuryMessageTitle"),
						JOptionPane.ERROR_MESSAGE);
				// Enable the start button.
				this.pageRun.setAllowNext(true);
				this.progressBar.setVisible(false);
				this.textProgress.setText("");
			}
		}
	}

	/**
	 * Starts the wizard traceroute.
	 * @param sites The list of sites.
	 */
	private void onStartTraceroute(ArrayList<String> sites) {
		// Set the progress.
		this.progressBar.setMaximum(sites.size());
		this.progressBar.setValue(0);
		this.textProgress.setText(String.format(FrameMain.translation.get("LabelProgressTraceroute"), sites.size()));

		try {
			// Initialize the traceroute state.
			this.onTracerouteInitialize(sites);

			// For all the concurrent traceroutes.
			for (int index = 0; index < FrameMain.tracerouteConcurrent; index++) {
				this.onStartTraceroute();
			}			
		}
		catch (InterruptedException exception) {
			// Set the wait handle.
			this.waitAsync.set();
			// Show an error message.
			JOptionPane.showMessageDialog(
					this,
					String.format(FrameMain.translation.get("MercuryMessageText"), exception.getMessage()),
					FrameMain.translation.get("MercuryMessageTitle"),
					JOptionPane.ERROR_MESSAGE);
			// Enable the start button.
			this.pageRun.setAllowNext(true);
			this.progressBar.setVisible(false);
			this.textProgress.setText("");
		}
	}
	
	/**
	 * Initializes the traceroute state.
	 * @param sites The list of sites.
	 * @throws InterruptedException 
	 */
	private void onTracerouteInitialize(Iterable<String> sites) throws InterruptedException {
		synchronized (this.sync)
		{
			// Clear the traceroute lists.
			this.traceroutePending.clear();
			this.tracerouteRunning.clear();
			this.tracerouteCompleted.clear();
			this.tracerouteFailed.clear();

			// Add the sites to the pending list.
			for (String site : sites)
			{
				this.traceroutePending.add(new TracerouteInfo(site));
			}

			// Reset the wait handle.
			this.waitAsync.reset();
			// Reset the cancellation token.
			this.tracerouteCancel.reset();
			// Set the traceroute timestamp.
			this.tracerouteTimestamp = new Date();
			// Start the timer.
			this.timer.start();
		}
	}
	
	/**
	 * Starts a traceroute.
	 */
	private void onStartTraceroute() {
		// Set the frame.
		final FrameMain frame = this; 
		
		// Start the traceroute on the thread pool.
		ThreadPool.execute(new Runnable() {
			@Override
			public void run() {
				TracerouteInfo info = null;
				
				synchronized (frame.sync) {
					// If the sites queue is empty.
					if (frame.traceroutePending.size() == 0) {
						if (frame.tracerouteRunning.size() == 0) {
							EventQueue.invokeLater(new Runnable() {
								@Override
								public void run() {
									// If the wizard is not completed.
									if (!frame.completed)
									{
										// Set the completed flag.
										frame.completed = true;
										// Enable the start button.
										frame.pageRun.setAllowNext(true);
										frame.pageRun.setAllowBack(true);
										frame.progressBar.setVisible(false);
										frame.textProgress.setText("");
										frame.textTime.setText("");
										frame.timer.stop();
										// Switch to the finish page.
										frame.wizard.next();
									}	
								}
							});
						}
						return;
					}

					try {
						// Get a site from the pending list.
						info = frame.onTraceroutePendingToRunning();
					} catch (InterruptedException e) {
					}
				}
				
				if (null == info) return;

				// Increment the traceroute information count.
				info.setCount(info.getCount() + 1);

				try {
					// If the IP address is null.
					if (null == info.getAddress()) {
						// Set the IP addresses.
						info.setAddress(InetAddress.getByName(info.getSite()));
					}

					// Begin a traceroute for the specified destination.
					final TracerouteResult result = frame.traceroute.run(info.getAddress(), frame.tracerouteCancel);

					if (frame.tracerouteCancel.isCanceled()) {
						// Set the traceroute as pending.
						frame.onTracerouteRunningToPending(info);
					}
					else {
						// Update the progress.
						EventQueue.invokeAndWait(new Runnable() {
							@Override
							public void run() {
								synchronized (frame.sync)
								{
									if (!frame.tracerouteCancel.isCanceled())
									{
										frame.progressBar.setValue(frame.tracerouteCompleted.size());
										frame.textProgress.setText(String.format(FrameMain.translation.get("LabelProgressCompleted"),
												frame.tracerouteCompleted.size(),
												frame.traceroutePending.size() + frame.tracerouteRunning.size() + frame.tracerouteCompleted.size() + frame.tracerouteFailed.size()));
									}
								}
							}
						});

						// Upload the result.
						boolean success = false;

						for (int attempt = 0; (attempt < 3) && (!success); attempt++) {
							success = frame.onUploadTraceroute(info, result);
						}

						if (success) {
							// Set the traceroute as completed.
							frame.onTracerouteRunningToCompleted(info);
						}
						else {
							throw new Exception();
						}
					}
				}
				catch (Exception exception) {
					// If the traceroute count is less than the maximum retries.
					if (info.getCount() < FrameMain.tracerouteRetries) {
						// Add the traceroute to the pending list.
						frame.onTracerouteRunningToPending(info);
					}
					else {
						// Add the traceroute to the failed list.
						frame.onTracerouteRunningToFailed(info);
					}
				}
				finally {
					// If the wizard is not canceled.
					if (!frame.tracerouteCancel.isCanceled()) {
						// Start another traceroute.
						frame.onStartTraceroute();
					}
				}				
			}
		});
	}
	
	/**
	 * Changes the state of a traceroute information from pending to running.
	 * @return The information of the next traceroute in the pending queue.
	 * @throws InterruptedException 
	 */
	private TracerouteInfo onTraceroutePendingToRunning() throws InterruptedException {
		synchronized (this.sync) {
			// Get a traceroute from the pending list.
			TracerouteInfo info = this.traceroutePending.remove();
			// If there are no running traceroutes.
			if (this.tracerouteRunning.size() == 0) {
				// Reset the wait handle.
				this.waitAsync.reset();
			}
			// Add the traceroute to the running list.
			this.tracerouteRunning.add(info);
			// Return the traceroute information.
			return info;
		}
	}

	/**
	 * Changes the state of a traceroute information from running to pending.
	 * @param info The traceroute information.
	 */
	private void onTracerouteRunningToPending(TracerouteInfo info)
	{
		synchronized (this.sync) {
			// Remove the traceroute from the running list.
			this.tracerouteRunning.remove(info);
			// Add the traceroute to the completed list.
			this.traceroutePending.add(info);

			// If there are no running traceroutes.
			if (this.tracerouteRunning.size() == 0) {
				// Set the wait handle.
				this.waitAsync.set();
			}
		}
	}

	/**
	 * Changes the state of a traceroute information from running to completed.
	 * @param info The traceroute information.
	 */
	private void onTracerouteRunningToCompleted(TracerouteInfo info) {
		synchronized (this.sync) {
			// Remove the traceroute from the running list.
			this.tracerouteRunning.remove(info);
			// Add the traceroute to the completed list.
			this.tracerouteCompleted.add(info);
			
			// If there are no running traceroutes.
			if (this.tracerouteRunning.size() == 0) {
				// Set the wait handle.
				this.waitAsync.set();
			}
		}
	}

	/**
	 * Changes the state of a traceroute information from running to failed.
	 * @param info The traceroute information.
	 */
	private void onTracerouteRunningToFailed(TracerouteInfo info) {
		synchronized (this.sync) {
			// Remove the traceroute from the running list.
			this.tracerouteRunning.remove(info);
			// Add the traceroute to the failed list.
			this.tracerouteFailed.add(info);

			// If there are no running traceroutes.
			if (this.tracerouteRunning.size() == 0) {
				// Set the wait handle.
				this.waitAsync.set();
			}
		}
	}
	
	/**
	 * Uploads a traceroute to the Mercury web server.
	 * @param info The traceroute information.
	 * @param result The traceroute result.
	 * @return True if the upload was successful, false otherwise.
	 */
	private boolean onUploadTraceroute(TracerouteInfo info, TracerouteResult result) {
		// Create the hops JSON object.
		JsonArray hops = new JsonArray();

		for (TracerouteHop hop : result.getHops()) {
			hops.add(new JsonObject(
				new JsonProperty("id", Integer.toString(hop.getTimeToLive())),
				new JsonProperty("ip", hop.getAddress() != null ? hop.getAddress().getHostAddress() : "destination unreachable"),
				new JsonProperty("asn", new JsonArray()),
				new JsonProperty("rtt", new JsonArray())
				));
		}

		// Create the traceroute JSON object.
		JsonObject obj = new JsonObject(
			new JsonProperty("sessionId", this.sessionId),
			new JsonProperty("srcIp", this.locationResult != null ? this.locationResult.getAddress() : "none"),
			new JsonProperty("dstIp", result.getDestination().getHostAddress()),
			new JsonProperty("srcName", this.localHostname),
			new JsonProperty("dstName", info.getSite()),
			new JsonProperty("hops", hops));

		
		try {
			File file = new File(result.getDestination().getHostAddress() + "-" + new Date().getTime() + ".json");
 
			// if file doesnt exists, then create it
			if (!file.exists()) {
				file.createNewFile();
			}
 
			FileWriter fw = new FileWriter(file.getAbsoluteFile());
			BufferedWriter bw = new BufferedWriter(fw);
			bw.write(obj.toString());
			bw.close();
		} catch (IOException e) {
		}		
		
//		try {
//			// Create a web request.
//			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FormMain.uriUploadTrace);
//
//			// Set the request headers.
//			request.Method = "POST";
//			request.Accept = "text/html,application/xhtml+xml,application/xml";
//			request.ContentType = "application/json;charset=UTF-8";
//
//			// Create the hops JSON object.
//			JsonArray hops = new JsonArray();
//
//			foreach (TracerouteHop hop in result.Hops)
//			{
//				hops.Add(new JsonObject(
//					new JsonProperty("id", hop.TimeToLive.ToString()),
//					new JsonProperty("ip", hop.Address != null ? hop.Address.ToString() : "none"),
//					new JsonProperty("asn", new JsonArray()),
//					new JsonProperty("rtt", new JsonArray(hop.AverageRoundtripTime.ToString()))
//					));
//			}
//
//			// Create the traceroute JSON object.
//			JsonObject obj = new JsonObject(
//				new JsonProperty("sessionId", this.sessionId.ToString()),
//				new JsonProperty("srcIp", this.location != null ? this.location.Address : "none"),
//				new JsonProperty("dstIp", result.Destination.ToString()),
//				new JsonProperty("srcName", Dns.GetHostName()),
//				new JsonProperty("dstName", info.Site),
//				new JsonProperty("hops", hops));
//
//			using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
//			{
//				writer.Write(obj.ToString());
//			}
//
//			// Execute the request.
//			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
//			{
//				return response.StatusCode == HttpStatusCode.OK;
//			}
//		}
//		catch
//		{
//			return false;
//		}
		return true;
	}
	
	
	/**
	 * An event handler called when the user rolls-back the finish page.
	 * @param e The event.
	 */
	private void onFinishRollback(WizardEvent e)
	{
		this.completed = false;
	}
	
	/**
	 * An event handler called when the timer expires.
	 * @param e The event.
	 */
	private void onTimer(ActionEvent e)
	{
		int pending;
		int completed;

		synchronized (this.sync)
		{
			// Compute the number of completed traceroutes.
			pending = this.traceroutePending.size() + this.tracerouteRunning.size();
			completed = this.tracerouteCompleted.size() + this.tracerouteFailed.size();
		}

		// If any of the completed 
		if (0 == completed)
		{
			// Clear the time remaining label.
			this.textTime.setText("");
		}
		else
		{
			// Compute the elapsed time.
			long elapsedTime = (new Date()).getTime() - this.tracerouteTimestamp.getTime();
			// Compute the remaining time.
			long remainingTime = elapsedTime * pending / completed;
			// Set the label.
			this.textTime.setText(this.getDuration(remainingTime / 1000));
		}
	}
	
	/**
	 * Generates the session identifier.
	 * @return The session identifier.
	 */
	private String getSession() {
		// Get the country.
		Territory country = (Territory) this.comboBoxCountry.getSelectedItem();

		// Create the identifier.
		return String.format("%s-%s-%s",
			country != null ? country.getType() : "",
			this.textFieldCity.getText(),
			this.textFieldProvider.getText());
	}

	/**
	 * Returns the string for the specified duration.
	 * @param duration The duration.
	 * @return The duration string.
	 */
	private String getDuration(long duration) {
		// Get the number of minutes.
		int minutes = (int)Math.round(duration) / 60;
		int seconds = (int)Math.round(duration) - minutes * 60;

		if (minutes > 0 && seconds > 0) {
			return String.format(this.timeMinutesSecondsRemaining,
				minutes, minutes == 1 ? this.timeMinute : this.timeMinutes, seconds, seconds == 1 ? this.timeSecond : this.timeSeconds);
		}
		else if (minutes > 0) {
			return minutes == 1 ? String.format(this.timeMinuteRemaining, minutes) : String.format(this.timeMinutesRemaining, minutes);
		}
		else if (seconds > 0) {
			return seconds == 1 ? String.format(this.timeSecondRemaining, seconds) : String.format(this.timeSecondsRemaining, seconds);
		}
		else return "";
	}
	
}
