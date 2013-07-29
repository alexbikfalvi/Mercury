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

namespace Mercury.Forms.Aws
{
	public partial class FormAwsCredentials : Form
	{
		public FormAwsCredentials()
		{
			// Initialize the component.
			this.InitializeComponent();

			// Apply default formatting.
			Formatting.SetFont(this);
		}

		/// <summary>
		/// Shows the dialog to get the Amazon Web Services credentials from the user.
		/// </summary>
		/// <param name="owner">The owner window.</param>
		/// <returns>The dialog result.</returns>
		public new DialogResult ShowDialog(IWin32Window owner)
		{
			// Clear the control values.
			this.control.Clear();
			// Show the dialog.
			return base.ShowDialog();
		}
	}
}
