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

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;

import javax.swing.JPanel;

/**
 * A class representing a wizard page.
 * @author Alex Bikfalvi
 *
 */
public final class WizardPage extends JPanel {
	private static final long serialVersionUID = 1612857058918719788L;
	
	private String title;
	private String textBack;
	private String textNext;
	private String textCancel;

	private int mnemonicBack = -1;
	private int mnemonicNext = -1;
	private int mnemonicCancel = -1;
	
	private boolean allowBack = true;
	private boolean allowNext = true;
	private boolean allowCancel = true;
	
	private ActionListener changedListener = null;
	private WizardListener commitListener = null;
	private WizardListener rollbackListener = null;

	/**
	 * Gets the wizard page title.
	 * @return The title.
	 */
	public String getTitle() {
		return this.title;
	}

	/**
	 * Sets the wizard page title.
	 * @param title The title.
	 */
	public void setTitle(String title) {
		this.title = title;
		this.onChanged();
	}
	
	/**
	 * Gets the text for the Back button.
	 * @return The text.
	 */
	public String getTextBack() {
		return this.textBack;
	}

	/**
	 * Sets the text for the Back button.
	 * @param textBack The text.
	 */
	public void setTextBack(String textBack) {
		this.textBack = textBack;
		this.onChanged();
	}

	/**
	 * Gets the text for the Next button.
	 * @return The text.
	 */
	public String getTextNext() {
		return this.textNext;
	}

	/**
	 * Sets the text for the Next button.
	 * @param textNext The text.
	 */
	public void setTextNext(String textNext) {
		this.textNext = textNext;
		this.onChanged();
	}

	/**
	 * Gets the text for the Cancel button.
	 * @return The text.
	 */
	public String getTextCancel() {
		return this.textCancel;
	}

	/**
	 * Sets the text for the Cancel button.
	 * @param textCancel The text.
	 */
	public void setTextCancel(String textCancel) {
		this.textCancel = textCancel;
		this.onChanged();
	}
	
	/**
	 * Gets the mnemonic index for the Back button.
	 * @return The index.
	 */
	public int getMnemonicBack() {
		return mnemonicBack;
	}

	/**
	 * Sets the mnemonic index for the Back button.
	 * @param mnemonicBack The index.
	 */
	public void setMnemonicBack(int mnemonicBack) {
		this.mnemonicBack = mnemonicBack;
		this.onChanged();
	}

	/**
	 * Gets the mnemonic index for the Next button.
	 * @return The index.
	 */
	public int getMnemonicNext() {
		return mnemonicNext;
	}

	/**
	 * Sets the mnemonic index for the Next button.
	 * @param mnemonicNext The index.
	 */
	public void setMnemonicNext(int mnemonicNext) {
		this.mnemonicNext = mnemonicNext;
		this.onChanged();
	}

	/**
	 * Gets the mnemonic index for the Cancel button.
	 * @return The index.
	 */
	public int getMnemonicCancel() {
		return mnemonicCancel;
	}

	/**
	 * Sets the mnemonic index for the Cancel button.
	 * @param mnemonicCancel The index.
	 */
	public void setMnemonicCancel(int mnemonicCancel) {
		this.mnemonicCancel = mnemonicCancel;
		this.onChanged();
	}	
	
	/**
	 * Gets whether the Back button is enabled for this page.
	 * @return True if the Back button is enabled, false otherwise.
	 */
	public boolean isAllowBack() {
		return this.allowBack;
	}

	/**
	 * Sets whether the Back button is enabled for this page.
	 * @param allowBack True if the Back button is enabled, false otherwise.
	 */
	public void setAllowBack(boolean allowBack) {
		this.allowBack = allowBack;
		this.onChanged();
	}

	/**
	 * Gets whether the Next button is enabled for this page.
	 * @return True if the Next button is enabled, false otherwise.
	 */
	public boolean isAllowNext() {
		return this.allowNext;
	}

	/**
	 * Sets whether the Next button is enabled for this page.
	 * @param allowNext True if the Next button is enabled, false otherwise.
	 */
	public void setAllowNext(boolean allowNext) {
		this.allowNext = allowNext;
		this.onChanged();
	}

	/**
	 * Gets whether the Cancel button is enabled for this page.
	 * @return True if the Cancel button is enabled, false otherwise.
	 */
	public boolean isAllowCancel() {
		return this.allowCancel;
	}

	/**
	 * Sets whether the Cancel button is enabled for this page.
	 * @param allowCancl True if the Cancel button is enabled, false otherwise.
	 */	
	public void setAllowCancel(boolean allowCancel) {
		this.allowCancel = allowCancel;
		this.onChanged();
	}	
	
	/**
	 * Sets the page changed listener.
	 * @param changedListener The listener.
	 */
	public void setChangedListener(ActionListener changedListener) {
		this.changedListener = changedListener;
	}
	
	/**
	 * Sets the page commit listener.
	 * @param commitListener The listener.
	 */
	public void setCommitListener(WizardListener commitListener) {
		this.commitListener = commitListener;
	}
	
	/**
	 * Adds a page rollback listener.
	 * @param rollbackListener The listener.
	 */
	public void addRollbackListener(WizardListener rollbackListener) {
		this.rollbackListener = rollbackListener;
	}
	
	/**
	 * A method called when the page has changed.
	 */
	private void onChanged() {
		if (null != this.changedListener) this.changedListener.actionPerformed(new ActionEvent(this, 0, null));
	}
}
