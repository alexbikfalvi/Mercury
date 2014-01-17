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
import java.awt.EventQueue;
import java.awt.Font;
import java.awt.Toolkit;

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

/**
 * A class representing the Mercury client main frame.
 * @author Alex Bikfalvi
 *
 */
public class FrameMain extends JFrame {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1597877587805876042L;
	private JPanel contentPane;
	private JTextField textFieldProvider;
	private JTextField textFieldCity;

	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				try {
					FrameMain frame = new FrameMain();
					frame.setVisible(true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	/**
	 * Create the frame.
	 */
	public FrameMain() {
		// Set the look-and-feel.
		try {
			UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
		} catch (ClassNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InstantiationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UnsupportedLookAndFeelException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
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
		
		JButton buttonCancel = new JButton("Cancel");
		buttonCancel.setMnemonic('C');
		buttonCancel.setBounds(495, 388, 89, 23);
		contentPane.add(buttonCancel);
		
		JButton buttonNext = new JButton("Next");
		buttonNext.setMnemonic('N');
		buttonNext.setBounds(396, 388, 89, 23);
		contentPane.add(buttonNext);
		
		JButton buttonBack = new JButton("Back");
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
		
		JLabel labelLanguage = new JLabel("Language:");
		labelLanguage.setDisplayedMnemonic('L');
		labelLanguage.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelLanguage.setBounds(10, 111, 130, 20);
		panelLocale.add(labelLanguage);
		
		JComboBox<?> comboBoxLanguage = new JComboBox<Object>();
		labelLanguage.setLabelFor(comboBoxLanguage);
		comboBoxLanguage.setBounds(150, 112, 300, 20);
		panelLocale.add(comboBoxLanguage);
		
		JComboBox<?> comboBoxCountry = new JComboBox<Object>();
		comboBoxCountry.setBounds(150, 143, 300, 20);
		panelLocale.add(comboBoxCountry);
		
		JLabel labelCountry = new JLabel("Country:");
		labelCountry.setDisplayedMnemonic('o');
		labelCountry.setFont(new Font("Segoe UI", Font.PLAIN, 11));
		labelCountry.setBounds(10, 142, 130, 20);
		panelLocale.add(labelCountry);
		
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
		
		JLabel labelTitle = new JLabel("Select your language and country");
		labelTitle.setForeground(new Color(19, 112, 171));
		labelTitle.setFont(new Font("Segoe UI", Font.PLAIN, 16));
		labelTitle.setBounds(84, 30, 500, 23);
		contentPane.add(labelTitle);
	}
}
