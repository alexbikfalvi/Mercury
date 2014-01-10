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
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace Mercury.Windows.Controls.AeroWizard
{
	/// <summary>
	/// A class representing the wizard resources.
	/// </summary>
	public static class WizardResources
	{
		private static ResourceManager resources;
		private static CultureInfo culture;

		/// <summary>
		/// Creates a new wizard resources wizard.
		/// </summary>
		static WizardResources()
		{
			// Load the wizard resources.
			WizardResources.resources = new ResourceManager("Mercury.Properties.Resources", typeof(WizardResources).Assembly);
			WizardResources.culture = Thread.CurrentThread.CurrentUICulture;
		}

		// Public properties.

		/// <summary>
		/// Gets or sets the current culture.
		/// </summary>
		public static CultureInfo Culture
		{
			get { return WizardResources.culture; }
			set { WizardResources.culture = value; }
		}

		/// <summary>
		///   Looks up a localized resource of type Bitmap.
		/// </summary>
		internal static Bitmap AeroBackButtonStrip
		{
			get
			{
				object obj = WizardResources.resources.GetObject("AeroBackButtonStrip", WizardResources.culture);
				return ((Bitmap)(obj));
			}
		}

		/// <summary>
		///   Looks up a localized resource of type Icon similar to (Icon).
		/// </summary>
		internal static Icon Icon
		{
			get
			{
				object obj = WizardResources.resources.GetObject("Icon", WizardResources.culture);
				return ((Icon)(obj));
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Returns to a previous page.
		/// </summary>
		internal static string WizardBackButtonToolTip
		{
			get
			{
				return WizardResources.GetString("WizardBackButtonToolTip");
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &amp;Back.
		/// </summary>
		internal static string WizardBackText
		{
			get
			{
				return WizardResources.GetString("WizardBackText");
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &amp;Cancel.
		/// </summary>
		internal static string WizardCancelText
		{
			get
			{
				return WizardResources.GetString("WizardCancelText");
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &amp;Finish.
		/// </summary>
		internal static string WizardFinishText
		{
			get
			{
				return WizardResources.GetString("WizardFinishText");
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Page Title.
		/// </summary>
		internal static string WizardHeader
		{
			get
			{
				return WizardResources.GetString("WizardHeader");
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &amp;Next.
		/// </summary>
		internal static string WizardNextText
		{
			get
			{
				return WizardResources.GetString("WizardNextText");
			}
		}

		/// <summary>
		///   Looks up a localized string similar to No wizard pages have been added..
		/// </summary>
		internal static string WizardNoPagesNotice
		{
			get
			{
				return WizardResources.GetString("WizardNoPagesNotice");
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Wizard Title.
		/// </summary>
		internal static string WizardTitle
		{
			get
			{
				return WizardResources.GetString("WizardTitle");
			}
		}

		// Public methods.

		/// <summary>
		/// Gets a string resource.
		/// </summary>
		/// <param name="name">The resource name.</param>
		/// <returns>The string resource.</returns>
		public static string GetString(string name)
		{
			string value;
			if (null == (value = WizardResources.resources.GetString(string.Format("{0}.{1}", name, WizardResources.culture.DisplayName))))
				if (null == (value = WizardResources.resources.GetString(string.Format("{0}.{1}", name, WizardResources.culture.TwoLetterISOLanguageName))))
					value = WizardResources.resources.GetString(name);
			return value;
		}
	}
}
