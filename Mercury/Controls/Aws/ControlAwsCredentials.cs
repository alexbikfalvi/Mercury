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
using System.Security;
using System.Windows.Forms;

namespace Mercury.Controls.Aws
{
	/// <summary>
	/// A control that allows the user to enter the Amazon Web Services credentials.
	/// </summary>
	public partial class ControlAwsCredentials : UserControl
	{
		/// <summary>
		/// Creates a new control instance.
		/// </summary>
		public ControlAwsCredentials()
		{
			this.InitializeComponent();
		}

		// Public properties.

		/// <summary>
		/// Gets or sets the Amazon Web Services access key.
		/// </summary>
		public SecureString AccessKey
		{
			get { return this.textBoxAccessKey.SecureText; }
			set { this.textBoxAccessKey.SecureText = value; }
		}
		/// <summary>
		/// Gets or sets the Amazon Web Services secret key.
		/// </summary>
		public SecureString SecretKey
		{
			get { return this.textBoxSecretKey.SecureText; }
			set { this.textBoxSecretKey.SecureText = value; }
		}

		// Public methods.

		/// <summary>
		/// Clears the controls to their default values.
		/// </summary>
		public void Clear()
		{
			this.textBoxAccessKey.Clear();
			this.textBoxSecretKey.Clear();
		}
	}
}
