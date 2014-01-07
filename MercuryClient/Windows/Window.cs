/* 
 * Copyright (C) 2012 Alex Bikfalvi
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DotNetApi.Windows
{
	/// <summary>
	/// Formats a control to the default configuration.
	/// </summary>
	public static class Window
	{
		private static readonly bool fontChange = false;
		private static readonly FontFamily fontDefaultFamily = null;
		private static readonly float fontDefaultSize;
		private static readonly List<string> fontReplaceList = new List<string>(new string[] { "Microsoft Sans Serif", "Tahoma" });

		private static readonly Font defaultFont;

		/// <summary>
		/// Initializes the parameters of the static class.
		/// </summary>
		static Window()
		{
			// Check the major version of the operating system.
			if (Environment.OSVersion.Version.Major == 5)
			{
				// Windows 2000 (5.0), XP (5.1) and Server 2003 (5.2): the default font is Tahoma.
				Window.fontDefaultFamily = SystemFonts.DialogFont.FontFamily;
				Window.fontDefaultSize = SystemFonts.DialogFont.Size;
				Window.fontChange = true;
			}
			else if (Environment.OSVersion.Version.Major >= 6)
			{
				// Windows Vista and above: the default font is SegoeUI.
				Window.fontDefaultFamily = SystemFonts.MessageBoxFont.FontFamily;
				Window.fontDefaultSize = SystemFonts.MessageBoxFont.Size;
				Window.fontChange = true;
			}

			// Create static fonts.
			Window.defaultFont = new Font(Window.fontDefaultFamily, Window.fontDefaultSize, SystemFonts.DefaultFont.Style);
		}

		// Public properties.
		
		/// <summary>
		/// Gets the default font.
		/// </summary>
		public static Font DefaultFont
		{
			get { return Window.defaultFont; }
		}

		// Public methods.

		/// <summary>
		/// Sets the font for the specified window control.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="font">The parent font.</param>
		public static void SetFont(Control control, Font font = null)
		{
			// If the control is null, exit.
			if (null == control) return;
			// If the font cannot be changed, return.
			if (!Window.fontChange) return;
			// Suspend the control layout.
			control.SuspendLayout();
			// Get the font of the current control.
			Font oldFont = control.Font;
			// The new font for the current control.
			Font newFont;
			// If the parent font has the same (or smaller) size and style as the old font.
			if (null != font ? (oldFont.Size == font.Size) && (oldFont.Style == font.Style) : false)
			{
				// Use the same font as the parent.
				newFont = font;
			}
			else if (null != font ? (Window.defaultFont.Size == font.Size) && (Window.defaultFont.Style == font.Style) : false)
			{
				// Use the default font.
				newFont = Window.defaultFont;
			}
			else
			{
				// Create a new font for the current control.
				newFont = new Font(Window.fontDefaultFamily, oldFont.Size, oldFont.Style);
			}
			// For all child controls.
			foreach (Control child in control.Controls)
			{
				// If the child font is in the replace list.
				if (Window.fontReplaceList.IndexOf(child.Font.Name) > -1)
				{
					// Set the font for the child control.
					Window.SetFont(child);
				}
			}
			// Set the font for the current control.
			control.Font = newFont;
			// Resume the control layout.
			control.ResumeLayout(false);
		}
	}
}
