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

package edu.upf.mercury.client.applet;

import javax.swing.JApplet;

import edu.upf.mercury.client.FrameMain;

public class AppletMain extends JApplet {
	
	/**
	 * Private variables.
	 */
	private static final long serialVersionUID = 5829190804789930716L;

	/**
	 * Create the applet.
	 */
	public AppletMain() {
		try {
			FrameMain.main(null);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
}
