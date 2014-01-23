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

/**
 * A class representing a wizard event.
 * @author Alex Bikfalvi
 *
 */
public class WizardEvent extends ActionEvent {

	private static final long serialVersionUID = -5109929834251494645L;
	
	private boolean canceled = false;

	/**
	 * Creates a new wizard event for the specified source.
	 * @param source The source.
	 */
	public WizardEvent(Object source) {
		// Call the base class constructor.
		super(source, 0, null);
	}

	/**
	 * Indicates whether the event has been canceled.
	 * @return True if the event has been canceled, false otherwise.
	 */
	public boolean isCanceled() {
		return this.canceled;
	}
	
	/**
	 * Cancels the current event.
	 */
	public void cancel() {
		this.canceled = true;
	}
}
