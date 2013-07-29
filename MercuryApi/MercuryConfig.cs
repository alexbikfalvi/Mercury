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
using System.ComponentModel;
using System.Security;
using Microsoft.Win32;
using DotNetApi.Security;

namespace MercuryApi
{
	/// <summary>
	/// A class representing the Mercury configuration.
	/// </summary>
	public class MercuryConfig : IDisposable
	{
		private RegistryKey rootKey;
		private string rootPath;
		private string root;

		private SecureString awsAccessKeyId = null;
		private SecureString awsSecretAccessKey = null;

		/// <summary>
		/// Creates a new Mercury configuration instance using the specified root registry key.
		/// </summary>
		/// <param name="rootKey">The root registry key of the Mercury configuration.</param>
		/// <param name="rootPath">The root registry path of the Mercury configuration.</param>
		public MercuryConfig(RegistryKey rootKey, string rootPath)
		{
			// Set the root key and path.
			this.rootKey = rootKey;
			this.rootPath = rootPath;
			this.root = string.Format("{0}\\{1}", this.rootKey.Name, this.rootPath);

			// Initialize the static configuration.
			MercuryStatic.awsAccessKey = this.AwsAccessKey;
			MercuryStatic.awsSecretKey = this.AwsSecretKey;
		}

		// Public properties.

		/// <summary>
		/// Gets or sets the Amazon Web Services access key.
		/// </summary>
		[DisplayName("Access key")]
		[Description("The Amazon Web Services account access key. This value cannot be changed here. Use the Amazon credentials dialog instead.")]
		[Category("Amazon Web Services")]
		[PasswordPropertyText(true)]
		public SecureString AwsAccessKey
		{
			get
			{
				try
				{
					SecureString value;
					return null != (value = (Registry.GetValue(this.root + "\\Amazon", "AccessKey", null) as byte[]).DecryptString()) ? value : SecureStringExtensions.Empty;
				}
				catch (Exception) { return SecureStringExtensions.Empty; }
			}
			set
			{
				Registry.SetValue(this.root + "\\Amazon", "AccessKey", value.EncryptString(), RegistryValueKind.Binary);
				MercuryStatic.awsAccessKey = value;
			}
		}
		/// <summary>
		/// Gets or sets the Amazon Web Services secret key.
		/// </summary>
		[DisplayName("Secret key")]
		[Description("The Amazon Web Services account secret key. This value cannot be changed here. Use the Amazon credentials dialog instead.")]
		[Category("Amazon Web Services")]
		[PasswordPropertyText(true)]
		public SecureString AwsSecretKey
		{
			get
			{
				try
				{
					SecureString value;
					return null != (value = (Registry.GetValue(this.root + "\\Amazon", "SecretKey", null) as byte[]).DecryptString()) ? value : SecureStringExtensions.Empty;
				}
				catch (Exception) { return SecureStringExtensions.Empty; }
			}
			set
			{
				Registry.SetValue(this.root + "\\Amazon", "SecretKey", value.EncryptString(), RegistryValueKind.Binary);
				MercuryStatic.awsSecretKey = value;
			}
		}

		// Public methods.

		/// <summary>
		/// Dispose the Mercury configuration.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
