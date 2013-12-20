/*
 * Copyright (c) 2013 David Hall
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using DotNetApi.Windows.Native;

namespace DotNetApi.Windows.VisualStyles
{
	/// <summary>
	/// Main DWM class, provides glass sheet effect and blur behind.
	/// </summary>
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public static class DesktopWindowManager
	{
		/// <summary>
		/// A class reprsenting a message window.
		/// </summary>
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private sealed class MessageWindow : NativeWindow, IDisposable
		{
			private const int wmDwmColorizationColorChanged = 0x0320;
			private const int wmDwmCompositionChanged = 0x031E;
			private const int wmDwmRenderingChanged = 0x031F;

			/// <summary>
			/// Creates a new message window instance.
			/// </summary>
			public MessageWindow()
			{
				CreateParams cp = new CreateParams() { Style = 0, ExStyle = 0, ClassStyle = 0, Parent = IntPtr.Zero };
				cp.Caption = base.GetType().Name;
				this.CreateHandle(cp);
			}

			/// <summary>
			/// Disposes the current object.
			/// </summary>
			public void Dispose()
			{
				// Destroy the handle.
				this.DestroyHandle();
				// Suppress the finalizer.
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Processes a window message.
			/// </summary>
			/// <param name="m">The message</param>
			[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
			protected override void WndProc(ref Message m)
			{
				// If a Desktop Window Manager message.
				if (m.Msg >= MessageWindow.wmDwmCompositionChanged && m.Msg <= MessageWindow.wmDwmColorizationColorChanged)
				{
					// Process the message.
					this.ExecuteEvents(m.Msg - MessageWindow.wmDwmCompositionChanged);
				}
				// Call the base class method.
				base.WndProc(ref m);
			}

			/// <summary>
			/// Executes the events corresponding to the DWM window messages.
			/// </summary>
			/// <param name="idx">The event index.</param>
			private void ExecuteEvents(int idx)
			{
				if (eventHandlerList != null)
				{
					lock (DesktopWindowManager.sync)
					{
						try { ((EventHandler)eventHandlerList[keys[idx]]).Invoke(null, EventArgs.Empty); }
						catch { };
					}
				}
			}
		}

		private static readonly object colorizationColorChangedKey = new object();
		private static readonly object compositionChangedKey = new object();
		private static readonly object nonClientRenderingChangedKey = new object();
		private static readonly object[] keys = new object[] { compositionChangedKey, nonClientRenderingChangedKey, colorizationColorChangedKey };
		private static readonly object sync = new object();

		private static EventHandlerList eventHandlerList;
		private static MessageWindow window;

		/// <summary>
		/// Occurs when the colorization color has changed.
		/// </summary>
		public static event EventHandler ColorizationColorChanged
		{
			add { AddEventHandler(colorizationColorChangedKey, value); }
			remove { RemoveEventHandler(colorizationColorChangedKey, value); }
		}

		/// <summary>
		/// Occurs when the desktop window composition has been enabled or disabled.
		/// </summary>
		public static event EventHandler CompositionChanged
		{
			add { AddEventHandler(compositionChangedKey, value); }
			remove { RemoveEventHandler(compositionChangedKey, value); }
		}

		/// <summary>
		/// Occurs when the non-client area rendering policy has changed.
		/// </summary>
		public static event EventHandler NonClientRenderingChanged
		{
			add { AddEventHandler(nonClientRenderingChangedKey, value); }
			remove { RemoveEventHandler(nonClientRenderingChangedKey, value); }
		}

		/// <summary>
		/// Gets or sets the current color used for Desktop Window Manager (DWM) glass composition. This value is based on the current color scheme and can be modified by the user.
		/// </summary>
		/// <value>The color of the glass composition.</value>
		public static Color CompositionColor
		{
			get
			{
				if (!CompositionSupported)
					return Color.Transparent;
				int value = (int)Microsoft.Win32.Registry.CurrentUser.GetValue(@"Software\Microsoft\Windows\DWM\ColorizationColor", 0);
				return Color.FromArgb(value);
			}
			set
			{
				if (!CompositionSupported)
					return;
				NativeMethods.ColorizationParams p = new NativeMethods.ColorizationParams();
				NativeMethods.DwmGetColorizationParameters(ref p);
				p.Color1 = (uint)value.ToArgb();
				NativeMethods.DwmSetColorizationParameters(ref p, 1);
				Microsoft.Win32.Registry.CurrentUser.SetValue(@"Software\Microsoft\Windows\DWM\ColorizationColor", value.ToArgb(), Microsoft.Win32.RegistryValueKind.DWord);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether composition (Windows Aero) is enabled.
		/// </summary>
		/// <value><c>true</c> if composition is enabled; otherwise, <c>false</c>.</value>
		public static bool CompositionEnabled
		{
			get { return IsCompositionEnabled(); }
			set { if (CompositionSupported) EnableComposition(value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether composition (Windows Aero) is supported.
		/// </summary>
		/// <value><c>true</c> if composition is supported; otherwise, <c>false</c>.</value>
		public static bool CompositionSupported
		{
			get { return System.Environment.OSVersion.Version.Major >= 6; }
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the <see cref="CompositionColor"/> is transparent.
		/// </summary>
		/// <value><c>true</c> if transparent; otherwise, <c>false</c>.</value>
		public static bool TransparencyEnabled
		{
			get
			{
				if (!CompositionSupported)
					return false;
				int value = (int)Microsoft.Win32.Registry.CurrentUser.GetValue(@"Software\Microsoft\Windows\DWM\ColorizationOpaqueBlend", 1);
				return value == 0;
			}
			set
			{
				if (!CompositionSupported)
					return;
				NativeMethods.ColorizationParams p = new NativeMethods.ColorizationParams();
				NativeMethods.DwmGetColorizationParameters(ref p);
				p.Opaque = value ? 0u : 1u;
				NativeMethods.DwmSetColorizationParameters(ref p, 1);
				Microsoft.Win32.Registry.CurrentUser.SetValue(@"Software\Microsoft\Windows\DWM\ColorizationOpaqueBlend", p.Opaque, Microsoft.Win32.RegistryValueKind.DWord);
			}
		}

		/// <summary>
		/// Enable the Aero "Blur Behind" effect on the whole client area. Background must be black.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="enabled"><c>true</c> to enable blur behind for this window, <c>false</c> to disable it.</param>
		public static void EnableBlurBehind(IWin32Window window, bool enabled)
		{
			EnableBlurBehind(window, null, null, enabled, false);
		}

		/// <summary>
		/// Enable the Aero "Blur Behind" effect on a specific region of a drawing area. Background must be black.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="graphics">The graphics area on which the region resides.</param>
		/// <param name="region">The region within the client area to apply the blur behind.</param>
		/// <param name="enabled"><c>true</c> to enable blur behind for this region, <c>false</c> to disable it.</param>
		/// <param name="transitionOnMaximized"><c>true</c> if the window's colorization should transition to match the maximized windows; otherwise, <c>false</c>.</param>
		public static void EnableBlurBehind(IWin32Window window, System.Drawing.Graphics graphics, System.Drawing.Region region, bool enabled, bool transitionOnMaximized)
		{
			NativeMethods.BlurBehind bb = new NativeMethods.BlurBehind(enabled);
			if (graphics != null && region != null)
				bb.SetRegion(graphics, region);
			if (transitionOnMaximized)
				bb.TransitionOnMaximized = true;
			NativeMethods.DwmEnableBlurBehindWindow(window.Handle, ref bb);
		}

		/// <summary>
		/// Enables or disables Desktop Window Manager (DWM) composition.
		/// </summary>
		/// <param name="value"><c>true</c> to enable DWM composition; <c>false</c> to disable composition.</param>
		public static void EnableComposition(bool value)
		{
			NativeMethods.DwmEnableComposition(value ? 1 : 0);
		}

		/// <summary>
		/// Excludes the specified child control from the glass effect.
		/// </summary>
		/// <param name="parent">The parent control.</param>
		/// <param name="control">The control to exclude.</param>
		/// <exception cref="ArgumentNullException">Occurs if control is null.</exception>
		/// <exception cref="ArgumentException">Occurs if control is not a child control.</exception>
		public static void ExcludeChildFromGlass(Control parent, Control control)
		{
			if (control == null)
				throw new ArgumentNullException("control");
			if (!parent.Contains(control))
				throw new ArgumentException("Control must be a child control.");

			if (IsCompositionEnabled())
			{
				System.Drawing.Rectangle clientScreen = parent.RectangleToScreen(parent.ClientRectangle);
				System.Drawing.Rectangle controlScreen = control.RectangleToScreen(control.ClientRectangle);

				NativeMethods.Margins margins = new NativeMethods.Margins(controlScreen.Left - clientScreen.Left, controlScreen.Top - clientScreen.Top,
					clientScreen.Right - controlScreen.Right, clientScreen.Bottom - controlScreen.Bottom);

				// Extend the Frame into client area
				NativeMethods.DwmExtendFrameIntoClientArea(parent.Handle, ref margins);
			}
		}

		/// <summary>
		/// Extends the window frame beyond the client area.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="padding">The padding to use as the area into which the frame is extended.</param>
		public static void ExtendFrameIntoClientArea(IWin32Window window, Padding padding)
		{
			NativeMethods.Margins m = new NativeMethods.Margins(padding);
			NativeMethods.DwmExtendFrameIntoClientArea(window.Handle, ref m);
		}

		/// <summary>
		/// Indicates whether Desktop Window Manager (DWM) composition is enabled.
		/// </summary>
		/// <returns><c>true</c> if is composition enabled; otherwise, <c>false</c>.</returns>
		public static bool IsCompositionEnabled()
		{
			if (!System.IO.File.Exists(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), NativeMethods.dllDwmapi)))
				return false;
			int res = 0;
			NativeMethods.DwmIsCompositionEnabled(ref res);
			return res != 0;
		}

		/// <summary>
		/// Adds a new event handler.
		/// </summary>
		/// <param name="id">The handler index.</param>
		/// <param name="value">The handler.</param>
		private static void AddEventHandler(object id, EventHandler value)
		{
			lock (sync)
			{
				if (window == null)
					window = new MessageWindow();
				if (eventHandlerList == null)
					eventHandlerList = new EventHandlerList();
				eventHandlerList.AddHandler(id, value);
			}
		}

		/// <summary>
		/// Removes an existing event handler.
		/// </summary>
		/// <param name="id">The handler index.</param>
		/// <param name="value">The handler.</param>
		private static void RemoveEventHandler(object id, EventHandler value)
		{
			lock (sync)
			{
				if (eventHandlerList != null)
				{
					eventHandlerList.RemoveHandler(id, value);
				}
			}
		}
	}
}