/* 
 * Copyright (C) 2013 Alex Bikfalvi
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

using System;
using System.Windows.Forms;
using DotNetApi.Windows;
using DotNetApi.Windows.Controls;
using MercuryApi;

namespace Mercury.Forms
{
	/// <summary>
	/// Represents the main form of the Mercury application.
	/// </summary>
	public partial class FormMain : Form
	{
		// The Mercuty API main object.
		private MercuryApi.Mercury mercury;

		// The selected control.
		private Control selectedControl = null;

		// The tree nodes.
		private TreeNode treeNodeTopSites;
		private TreeNode treeNodeSettings;

		/// <summary>
		/// Creates a new form instance.
		/// </summary>
		/// <param name="mercury">The instance of the Mercury API.</param>
		public FormMain(MercuryApi.Mercury mercury)
		{
			// Initialize the main form.
			this.InitializeComponent();

			// Apply default formatting.
			Formatting.SetFont(this);

			// Set the selected control.
			this.selectedControl = this.labelNotAvailable;

			// Set the Mercury instance.
			this.mercury = mercury;

			// Set the control properties.
			this.controlSettings.Config = this.mercury.Config;

			// Create the tree nodes.
			this.treeNodeTopSites = new TreeNode("Top sites",
				this.imageList.Images.IndexOfKey("GlobeStar"),
				this.imageList.Images.IndexOfKey("GlobeStar"));
			this.treeNodeSettings = new TreeNode("Settings",
				this.imageList.Images.IndexOfKey("Settings"),
				this.imageList.Images.IndexOfKey("Settings"));

			// Set the tree node tags.
			this.treeNodeSettings.Tag = this.controlSettings;

			// Add the tree nodes to the side controls.
			this.sideTreeViewBrowser.Nodes.Add(this.treeNodeTopSites);
			this.sideTreeViewSettings.Nodes.Add(this.treeNodeSettings);

			// Initialize the side controls to expand the tree nodes.
			this.sideTreeViewBrowser.Initialize();
			this.sideTreeViewSettings.Initialize();
		}

		/// <summary>
		/// An event handler called when the control selection has changed.
		/// </summary>
		/// <param name="sender">The sender control.</param>
		/// <param name="control">The selected control.</param>
		private void OnControlChanged(Control sender, Control control)
		{
			// If the current selected control and the new selected control are the same, do nothing.
			if (control == this.selectedControl) return;

			// Hide the current control.
			if (this.selectedControl != null) this.selectedControl.Hide();

			// If the new control is not null.
			if (control != null)
			{
				// Show the new control.
				control.Show();
				// Update the selected control.
				this.selectedControl = control;
			}
			else
			{
				// Show the not available label.
				this.labelNotAvailable.Show();
				// Update the selected control.
				this.selectedControl = this.labelNotAvailable;
			}
		}

		/// <summary>
		/// An event handler called when the user the selects the exit menu.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnExit(object sender, EventArgs e)
		{
			// Close the window.
			this.Close();
		}
	}
}
