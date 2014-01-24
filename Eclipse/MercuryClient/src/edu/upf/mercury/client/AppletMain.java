package edu.upf.mercury.client;

import java.awt.CardLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.ImageIcon;
import javax.swing.JApplet;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JProgressBar;
import javax.swing.JTextField;
import javax.swing.JTextPane;
import javax.swing.border.EmptyBorder;

import edu.upf.mercury.client.wizard.WizardPage;

public class AppletMain extends JApplet {

	/**
	 * Private variables.
	 */
	private static final long serialVersionUID = 8035013844894771161L;

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
	
	/**
	 * Create the applet.
	 */
	public AppletMain() {
		final AppletMain frame = this;		
		
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
				//frame.onLanguageChanged();
			}
		});
		labelLanguage.setLabelFor(comboBoxLanguage);
		comboBoxLanguage.setBounds(150, 112, 300, 20);
		pageLocale.add(comboBoxLanguage);
		
		comboBoxCountry = new JComboBox();
		comboBoxCountry.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				//frame.onCountryChanged();
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
		
		JProgressBar progressBar = new JProgressBar();
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
		
		// Create the wizard.
		//this.wizard = new Wizard(this.panelWizard, this.labelTitle, this.buttonBack, this.buttonNext, this.buttonCancel,
		//		new WizardPage[] { this.pageLocale, this.pageForm, this.pageRun, this.pageFinish });
	}
	
}
