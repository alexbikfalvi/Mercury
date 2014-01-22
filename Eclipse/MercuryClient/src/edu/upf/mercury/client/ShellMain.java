/* 
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

import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.wb.swt.SWTResourceManager;
import com.bikfalvi.java.windows.controls.Wizard;
import org.eclipse.swt.SWT;
import org.eclipse.swt.layout.FormLayout;
import org.eclipse.swt.layout.FormData;
import org.eclipse.swt.layout.FormAttachment;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Label;

public class ShellMain {

	protected Shell shell;

	/**
	 * Launch the application.
	 * @param args
	 */
	public static void main(String[] args) {
		try {
			ShellMain window = new ShellMain();
			window.open();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	/**
	 * Open the window.
	 */
	public void open() {
		Display display = Display.getDefault();
		createContents();
		shell.open();
		shell.layout();
		while (!shell.isDisposed()) {
			if (!display.readAndDispatch()) {
				display.sleep();
			}
		}
	}

	/**
	 * Create contents of the window.
	 */
	protected void createContents() {
		shell = new Shell();
		shell.setMinimumSize(new Point(600, 450));
		shell.setSize(new Point(600, 450));
		shell.setImage(SWTResourceManager.getImage(ShellMain.class, "/edu/upf/mercury/client/resources/GraphBarColor_16.png"));
		shell.setSize(450, 300);
		shell.setText("Mercury Client");
		shell.setLayout(new FormLayout());
		
		Label labelLogo = new Label(shell, SWT.NONE);
		labelLogo.setImage(SWTResourceManager.getImage(ShellMain.class, "/edu/upf/mercury/client/resources/GraphBarColor_64.png"));
		FormData fd_labelLogo = new FormData();
		fd_labelLogo.top = new FormAttachment(0, 10);
		fd_labelLogo.left = new FormAttachment(0, 10);
		labelLogo.setLayoutData(fd_labelLogo);
		
		Label labelTitle = new Label(shell, SWT.NONE);
		labelTitle.setFont(SWTResourceManager.getFont("Segoe UI", 14, SWT.NORMAL));
		FormData fd_labelTitle = new FormData();
		fd_labelTitle.top = new FormAttachment(0, 35);
		fd_labelTitle.left = new FormAttachment(labelLogo, 6);
		labelTitle.setLayoutData(fd_labelTitle);
		labelTitle.setText("Title");

	}
}
