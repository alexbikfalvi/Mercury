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

package edu.upf.mercury.client.wizard;

import java.awt.CardLayout;
import java.awt.Component;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Arrays;

import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JPanel;

/**
 * A class representing a wizard.
 * @author Alex Bikfalvi
 *
 */
public final class Wizard {
	private final JPanel panel;
	private final JLabel labelTitle;
	private final JButton buttonBack;
	private final JButton buttonNext;
	private final JButton buttonCancel;
	
	private final WizardPage[] pages;
	private int selected = 0;
	
	private final CardLayout layout;
	
	private ActionListener finishedListener = null;
	private ActionListener cancelListener = null;
	
	public Wizard(
			JPanel panel,
			JLabel labelTitle,
			JButton buttonBack,
			JButton buttonNext,
			JButton buttonCancel,
			WizardPage[] pages) {
		
		// Validate the arguments.
		if (null == panel) throw new IllegalArgumentException("panel");
		if (null == labelTitle) throw new IllegalArgumentException("labelTitle");
		if (null == buttonBack) throw new IllegalArgumentException("buttonBack");
		if (null == buttonNext) throw new IllegalArgumentException("buttonNext");
		if (null == buttonCancel) throw new IllegalArgumentException("buttonCancel");
		if (null == pages) throw new IllegalArgumentException("pages");
		if (0 == pages.length) throw new IllegalArgumentException("pages");
		
		// Get the current object.
		final Wizard wizard = this;
		
		// Set the components.
		this.panel = panel;
		this.labelTitle = labelTitle;
		this.buttonBack = buttonBack;
		this.buttonNext = buttonNext;
		this.buttonCancel = buttonCancel;
		
		this.pages = pages;

		// Set the layout.
		this.layout = (CardLayout) this.panel.getLayout();
		
		// Remove the current component.
		for (Component component : this.panel.getComponents()) {
			this.layout.removeLayoutComponent(component);
		}
		
		// Add the page components.
		for (WizardPage page : this.pages) {
			// Add the page event listeners.
			page.setChangedListener(new ActionListener() {
				@Override
				public void actionPerformed(ActionEvent e) {
					wizard.onPageChanged((WizardPage) e.getSource());
				}
			});
			
			// Add the page to the layout.
			this.layout.addLayoutComponent(page, page.getName());
		}
		
		// Set the buttons event listeners.
		this.buttonBack.addActionListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				wizard.onBack(e);
			}
		});
		this.buttonNext.addActionListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				wizard.onNext(e);
			}
		});
		this.buttonCancel.addActionListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				wizard.onCancel(e);
			}
		});
	
		// Select the first page.
		this.update();
	}
	
	/**
	 * Sets the finished listener.
	 * @param finishedListener The listener.
	 */
	public void setFinishedListener(ActionListener finishedListener) {
		this.finishedListener = finishedListener;
	}

	/**
	 * Sets the cancel listener.
	 * @param cancelListener The listener.
	 */
	public void setCancelListener(ActionListener cancelListener) {
		this.cancelListener = cancelListener;
	}
	
	/**
	 * Switches to the previous page.
	 */
	public void back() {
		this.onBack(null);
	}
	
	/**
	 * Switches to the next page.
	 */
	public void next() {
		this.onNext(null);
	}

	/**
	 * Updates the wizard page selection for the current index.
	 */
	private void update() {
		// Selects the wizard page at the current index.
		this.layout.show(this.panel, this.pages[this.selected].getName());
		
		// Update the label title.
		this.labelTitle.setText(this.pages[this.selected].getTitle());
		this.buttonBack.setText(this.pages[this.selected].getTextBack());
		this.buttonNext.setText(this.pages[this.selected].getTextNext());
		this.buttonCancel.setText(this.pages[this.selected].getTextCancel());
		this.buttonBack.setDisplayedMnemonicIndex(this.pages[this.selected].getMnemonicBack());
		this.buttonNext.setDisplayedMnemonicIndex(this.pages[this.selected].getMnemonicNext());
		this.buttonCancel.setDisplayedMnemonicIndex(this.pages[this.selected].getMnemonicCancel());
		this.buttonBack.setEnabled(this.pages[this.selected].isAllowBack());
		this.buttonNext.setEnabled(this.pages[this.selected].isAllowNext());
		this.buttonCancel.setEnabled(this.pages[this.selected].isAllowCancel());
		
		// Initialize the page.
		this.pages[this.selected].initialize();
	}
	
	/**
	 * An event listener called when a page has changed.
	 * @param page The wizard page.
	 */
	private void onPageChanged(WizardPage page) {
		// If the page is at the selected index.
		if (Arrays.asList(this.pages).indexOf(page) == this.selected) {
			// Update the wizard page.
			this.update();
		}
	}

	/**
	 * An event listener called when the user clicks on the Back button.
	 * @param event The action event.
	 */
	private void onBack(ActionEvent event) {
		// If not at the first page.
		if (this.selected > 0) {
			// Check whether a roll-back is allowed.
			if (this.pages[this.selected].rollback()) {
				// Decrement the selected index.
				this.selected--;
				// Update the page.
				this.update();
			}
		}
	}
	
	/**
	 * An event listener called when the user clicks on the Next button.
	 * @param event The action event.
	 */
	private void onNext(ActionEvent event) {
		// If not at the last page.
		if (this.selected < this.pages.length - 1) {
			// Check whether a commit is allowed.
			if (this.pages[this.selected].commit()) {
				// Increment the selected index.
				this.selected++;
				// Update the page.
				this.update();
			}
		}
		else {
			// If the page is a finish page.
			if (this.pages[this.selected].isFinishPage()) {
				// Raise the finished event.
				if (null != this.finishedListener) this.finishedListener.actionPerformed(new ActionEvent(this, 0, null));
			}
		}
	}
	
	/**
	 * An event listener called when the user clicks on the Cancel button.
	 * @param event The action event.
	 */
	private void onCancel(ActionEvent event) {
		// Raise the canceled event.
		if (null != this.cancelListener) this.cancelListener.actionPerformed(new ActionEvent(this, 0, null));
	}
}
