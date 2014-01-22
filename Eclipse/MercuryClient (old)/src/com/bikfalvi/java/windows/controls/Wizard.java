package com.bikfalvi.java.windows.controls;

import javax.swing.JPanel;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;

import javax.swing.JLabel;
import javax.swing.ImageIcon;
import javax.swing.border.EmptyBorder;
import java.awt.FlowLayout;
import javax.swing.JButton;
import java.awt.CardLayout;

public class Wizard extends JPanel {
	private JPanel panelTitle;
	private JPanel panelWizard;

	/**
	 * Create the panel.
	 */
	public Wizard() {
		setBackground(Color.WHITE);
		setLayout(new BorderLayout(0, 0));
		
		panelTitle = new JPanel();
		panelTitle.setBackground(Color.WHITE);
		panelTitle.setMinimumSize(new Dimension(10, 86));
		panelTitle.setPreferredSize(new Dimension(10, 86));
		add(panelTitle, BorderLayout.NORTH);
		panelTitle.setLayout(new BorderLayout(0, 0));
		
		JLabel labelIcon = new JLabel((String) null);
		labelIcon.setBorder(new EmptyBorder(0, 10, 0, 10));
		labelIcon.setIcon(new ImageIcon(Wizard.class.getResource("/edu/upf/mercury/resources/GraphBarColor_64.png")));
		panelTitle.add(labelIcon, BorderLayout.WEST);
		
		JLabel labelTitle = new JLabel("New label");
		labelTitle.setForeground(new Color(19, 112, 171));
		labelTitle.setFont(new Font("Segoe UI", Font.PLAIN, 16));
		panelTitle.add(labelTitle, BorderLayout.CENTER);
		
		JPanel panelButtons = new JPanel();
		panelButtons.setBackground(Color.WHITE);
		panelButtons.setBorder(new EmptyBorder(5, 5, 5, 0));
		panelButtons.setSize(new Dimension(0, 43));
		panelButtons.setPreferredSize(new Dimension(10, 43));
		panelButtons.setMinimumSize(new Dimension(10, 43));
		add(panelButtons, BorderLayout.SOUTH);
		panelButtons.setLayout(new FlowLayout(FlowLayout.RIGHT, 5, 5));
		
		JButton buttonBack = new JButton("Back");
		buttonBack.setPreferredSize(new Dimension(89, 23));
		buttonBack.setMinimumSize(new Dimension(89, 23));
		buttonBack.setMaximumSize(new Dimension(89, 23));
		panelButtons.add(buttonBack);
		
		JButton buttonNext = new JButton("Next");
		buttonNext.setPreferredSize(new Dimension(89, 23));
		buttonNext.setMinimumSize(new Dimension(89, 23));
		buttonNext.setMaximumSize(new Dimension(89, 23));
		panelButtons.add(buttonNext);
		
		JButton buttonCancel = new JButton("Cancel");
		buttonCancel.setMaximumSize(new Dimension(89, 23));
		buttonCancel.setMinimumSize(new Dimension(89, 23));
		buttonCancel.setPreferredSize(new Dimension(89, 23));
		panelButtons.add(buttonCancel);
		
		panelWizard = new JPanel();
		add(panelWizard, BorderLayout.CENTER);
		panelWizard.setLayout(new CardLayout(0, 0));

	}

	public JPanel getPanelWizard() {
		return panelWizard;
	}
}
