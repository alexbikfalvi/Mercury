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

namespace MercuryApi
{
	/// <summary>
	/// A class that represents the static Mercury configuration.
	/// </summary>
	public static class MercuryStatic
	{
		internal static SecureString awsAccessKey;
		internal static SecureString awsSecretKey;

		/// <summary>
		/// Gets the Amazon Web Services access key.
		/// </summary>
		public static SecureString AwsAccessKey { get { return MercuryStatic.awsAccessKey; } }
		/// <summary>
		/// Gets the Amazon Web Services secret access key.
		/// </summary>
		public static SecureString AwsSecretKey { get { return MercuryStatic.awsSecretKey; } }
	}
}
