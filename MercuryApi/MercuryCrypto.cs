/* 
 * Copyright (C) 2012-2013 Alex Bikfalvi
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
using DotNetApi.Security;

namespace MercuryApi
{
	/// <summary>
	/// A class used to encrypt and decrypt security-sensitive configuration data.
	/// </summary>
	public static class MercuryCrypto
	{
		private static byte[] cryptoKey = { 0x39, 0xCB, 0x0C, 0x11, 0x52, 0x82, 0x19, 0x0C, 0x53, 0x09, 0x0D, 0x0C, 0x66, 0x70, 0x8C, 0x46, 0x06, 0x13, 0x36, 0x98, 0xC7, 0xFD, 0xA7, 0x8A, 0x2E, 0xA0, 0xB5, 0x6B, 0x60, 0xA8, 0x03, 0x8D };
		private static byte[] cryptoIV = { 0x48, 0xE6, 0x63, 0x73, 0xFF, 0x90, 0x09, 0xF3, 0xC5, 0xF8, 0xE9, 0x22, 0xFA, 0x32, 0x8C, 0xAA };

		/// <summary>
		/// Encrypts the specified string into a byte array.
		/// </summary>
		/// <param name="value">The string to encrypt.</param>
		/// <returns>The encrypted string as a byte array.</returns>
		public static byte[] EncryptString(this SecureString value)
		{
			return value.EncryptSecureStringAes(MercuryCrypto.cryptoKey, MercuryCrypto.cryptoIV);
		}

		/// <summary>
		/// Decrypts the specified byte array buffer into a string.
		/// </summary>
		/// <param name="value">The byte array to decrypt.</param>
		/// <returns>The descrypted data as a string.</returns>
		public static SecureString DecryptString(this byte[] value)
		{
			return value.DecryptSecureStringAes(MercuryCrypto.cryptoKey, MercuryCrypto.cryptoIV);
		}
	}
}
