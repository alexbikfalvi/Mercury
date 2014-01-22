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

package edu.upf.mercury;

import java.awt.CardLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.EventQueue;
import java.awt.Font;
import java.awt.Toolkit;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Locale;

import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JProgressBar;
import javax.swing.JTextField;
import javax.swing.JTextPane;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;
import javax.swing.border.EmptyBorder;
import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;

import com.bikfalvi.java.globalization.CultureCollection;
import com.bikfalvi.java.globalization.CultureReader;
import com.bikfalvi.java.globalization.Language;
import com.bikfalvi.java.globalization.Territory;
import com.bikfalvi.java.web.WebCallback;
import com.bikfalvi.java.web.WebResult;
import com.bikfalvi.java.web.location.LocationRequest;
import com.bikfalvi.java.web.location.LocationResult;

import edu.upf.mercury.resources.Resources;

import java.awt.event.ActionListener;
import java.awt.event.ActionEvent;
import com.bikfalvi.java.windows.controls.Wizard;
import java.awt.BorderLayout;

/**
 * A class representing the Mercury client main frame.
 * @author Alex Bikfalvi
 *
 */
public class FrameMain extends JFrame {

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
	private static final String uriUploadTrace = "http://mercury.upf.edu/mercury/api/traceroute/uploadTrace";
	
	private boolean countryUser = false;
	private boolean cityUser = false;
	private final LocationRequest locationRequest = new LocationRequest();
	private LocationResult locationResult = null;
	
	private JPanel contentPane;
	private JTextField textFieldProvider;
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
		
		setResizable(false);
		setTitle("Mercury Client");
		setIconImage(Toolkit.getDefaultToolkit().getImage(FrameMain.class.getResource("/edu/upf/mercury/resources/GraphBarColor_16.png")));
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
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
		buttonBack.setEnabled(false);
		buttonBack.setMnemonic('B');
		buttonBack.setBounds(297, 388, 89, 23);
		contentPane.add(buttonBack);
		
		JPanel panelWizard = new JPanel();
		panelWizard.setBackground(Color.WHITE);
		panelWizard.setBounds(0, 86, 594, 291);
		contentPane.add(panelWizard);
		panelWizard.setLayout(new CardLayout(0, 0));
		
		JPanel panelLocale = new JPanel();
		panelLocale.setBackground(Color.WHITE);
		panelWizard.add(panelLocale, "name_13364620740774");
		panelLocale.setLayout(null);
		
		labelLanguage = new JLabel("Language:");
		labelLanguage.setDisplayedMnemonic('L');
		labelLanguage.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelLanguage.setBounds(10, 111, 130, 20);
		panelLocale.add(labelLanguage);
		
		comboBoxLanguage = new JComboBox();
		comboBoxLanguage.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent arg0) {
				frame.onLanguageChanged();
			}
		});
		labelLanguage.setLabelFor(comboBoxLanguage);
		comboBoxLanguage.setBounds(150, 112, 300, 20);
		panelLocale.add(comboBoxLanguage);
		
		comboBoxCountry = new JComboBox();
		comboBoxCountry.setBounds(150, 143, 300, 20);
		panelLocale.add(comboBoxCountry);
		
		labelCountry = new JLabel("Country:");
		labelCountry.setLabelFor(comboBoxCountry);
		labelCountry.setDisplayedMnemonic('o');
		labelCountry.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelCountry.setBounds(10, 142, 130, 20);
		panelLocale.add(labelCountry);
		
		labelCountryBusy = new JLabel("");
		labelCountryBusy.setVisible(false);
		labelCountryBusy.setIconTextGap(0);
		labelCountryBusy.setVerifyInputWhenFocusTarget(false);
		labelCountryBusy.setPreferredSize(new Dimension(48, 48));
		labelCountryBusy.setMinimumSize(new Dimension(48, 48));
		labelCountryBusy.setMaximumSize(new Dimension(48, 48));
		labelCountryBusy.setIcon(new ImageIcon(FrameMain.class.getResource("/edu/upf/mercury/resources/Busy.gif")));
		labelCountryBusy.setBounds(460, 130, 48, 48);
		panelLocale.add(labelCountryBusy);
		
		JPanel panelForm = new JPanel();
		panelForm.setBackground(Color.WHITE);
		panelWizard.add(panelForm, "name_13378861805415");
		panelForm.setLayout(null);
		
		JTextPane textForm = new JTextPane();
		textForm.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textForm.setText("We use this to learn more about your Internet connection. We do not collect any personal information without your consent, and all fields are optional.");
		textForm.setBounds(10, 11, 574, 50);
		panelForm.add(textForm);
		
		JLabel labelProvider = new JLabel("Provider:");
		labelProvider.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelProvider.setDisplayedMnemonic('P');
		labelProvider.setBounds(10, 109, 130, 20);
		panelForm.add(labelProvider);
		
		JLabel labelCity = new JLabel("City:");
		labelCity.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelCity.setDisplayedMnemonic('t');
		labelCity.setBounds(10, 161, 130, 20);
		panelForm.add(labelCity);
		
		textFieldProvider = new JTextField();
		textFieldProvider.setBounds(150, 110, 300, 20);
		panelForm.add(textFieldProvider);
		textFieldProvider.setColumns(10);
		
		textFieldCity = new JTextField();
		textFieldCity.setBounds(150, 162, 300, 20);
		panelForm.add(textFieldCity);
		textFieldCity.setColumns(10);
		
		JTextPane txtpnExampleAttComcast = new JTextPane();
		txtpnExampleAttComcast.setText("Example: AT&T, Comcast, Verizon, or the name of your company.");
		txtpnExampleAttComcast.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		txtpnExampleAttComcast.setBounds(150, 131, 434, 20);
		panelForm.add(txtpnExampleAttComcast);
		
		JPanel panelRun = new JPanel();
		panelRun.setBackground(Color.WHITE);
		panelWizard.add(panelRun, "name_14199107629293");
		panelRun.setLayout(null);
		
		JTextPane txtpnTheWizardIs = new JTextPane();
		txtpnTheWizardIs.setText("The wizard is ready to start the Internet measurements. This may take several minutes, depending on the speed of your Internet connection. To continue, click Start.");
		txtpnTheWizardIs.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		txtpnTheWizardIs.setBounds(10, 11, 574, 50);
		panelRun.add(txtpnTheWizardIs);
		
		JTextPane textTime = new JTextPane();
		textTime.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textTime.setBounds(10, 230, 574, 50);
		panelRun.add(textTime);
		
		JTextPane textProgress = new JTextPane();
		textProgress.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		textProgress.setBounds(10, 169, 574, 50);
		panelRun.add(textProgress);
		
		JProgressBar progressBar = new JProgressBar();
		progressBar.setVisible(false);
		progressBar.setBounds(10, 142, 574, 16);
		panelRun.add(progressBar);
		
		JPanel panelFinish = new JPanel();
		panelFinish.setBackground(Color.WHITE);
		panelWizard.add(panelFinish, "name_14227740574396");
		panelFinish.setLayout(null);
		
		JTextPane txtpnTheWizardHas = new JTextPane();
		txtpnTheWizardHas.setText("The wizard has finished the Internet measurements.\r\n\r\nTo close, click on Finish.");
		txtpnTheWizardHas.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		txtpnTheWizardHas.setBounds(10, 11, 574, 100);
		panelFinish.add(txtpnTheWizardHas);
		
		JLabel labelLogo = new JLabel("New label");
		labelLogo.setIcon(new ImageIcon(FrameMain.class.getResource("/edu/upf/mercury/resources/GraphBarColor_64.png")));
		labelLogo.setBounds(10, 11, 64, 64);
		contentPane.add(labelLogo);
		
		labelTitle = new JLabel("Select your language and country");
		labelTitle.setForeground(new Color(19, 112, 171));
		labelTitle.setFont(new Font("Segoe UI", Font.PLAIN, 16));
		labelTitle.setBounds(84, 30, 500, 23);
		contentPane.add(labelTitle);
	}
	
	/**
	 * Loads the application cultures.
	 * @throws IOException 
	 * @throws SAXException 
	 * @throws ParserConfigurationException 
	 */
	private static void loadCultures() throws ParserConfigurationException, SAXException, IOException {
		InputStream stream = (InputStream) FrameMain.class.getResource("/edu/upf/mercury/resources/Cultures.xml").getContent();
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
		InputStream stream = (InputStream) FrameMain.class.getResource("/edu/upf/mercury/resources/Translation.xml").getContent();
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
		this.labelLanguage.setText(FrameMain.translation.get("LabelLanguage", true));
		this.labelLanguage.setDisplayedMnemonicIndex(FrameMain.translation.getMnemonic("LabelLanguage"));
		
		this.labelCountry.setText(FrameMain.translation.get("LabelCountry", true));
		this.labelCountry.setDisplayedMnemonicIndex(FrameMain.translation.getMnemonic("LabelCountry"));
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
			if (!this.countryUser)
			{
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
			if (!this.cityUser)
			{
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
		
	}
}
