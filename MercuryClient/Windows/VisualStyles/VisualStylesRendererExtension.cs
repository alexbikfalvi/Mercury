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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DotNetApi.Windows.Native;

namespace DotNetApi.Windows.VisualStyles
{
	internal static class VisualStyleRendererExtension
	{
		private delegate void DrawWrapperMethod(IntPtr hdc);

		public static void DrawGlassBackground(VisualStyleRenderer rnd, IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle, bool rightToLeft = false)
		{
			DrawWrapper(rnd, dc, bounds,
				delegate(IntPtr memoryHdc)
				{
					NativeMethods.Rect rBounds = new NativeMethods.Rect(bounds);
					NativeMethods.Rect rClip = new NativeMethods.Rect(clipRectangle);
					// Draw background
					if (rightToLeft) NativeMethods.SetLayout(memoryHdc, 1);
					NativeMethods.DrawThemeBackground(rnd.Handle, memoryHdc, rnd.Part, rnd.State, ref rBounds, ref rClip);
					NativeMethods.SetLayout(memoryHdc, 0);
				}
			);
		}

		public static void DrawGlassIcon(VisualStyleRenderer rnd, Graphics g, Rectangle bounds, ImageList imgList, int imgIndex)
		{
			DrawWrapper(rnd, g, bounds,
				delegate(IntPtr memoryHdc)
				{
					NativeMethods.Rect rBounds = new NativeMethods.Rect(bounds);
					NativeMethods.DrawThemeIcon(rnd.Handle, memoryHdc, rnd.Part, rnd.State, ref rBounds, imgList.Handle, imgIndex);
				}
			);
		}

		public static void DrawGlassImage(VisualStyleRenderer rnd, Graphics g, Rectangle bounds, Image img, bool disabled = false)
		{
			DrawWrapper(rnd, g, bounds,
				delegate(IntPtr memoryHdc)
				{
					using (Graphics mg = Graphics.FromHdc(memoryHdc))
					{
						if (disabled)
							ControlPaint.DrawImageDisabled(mg, img, bounds.X, bounds.Y, Color.Transparent);
						else
							mg.DrawImage(img, bounds);
					}
				}
			);
		}

		public static void DrawGlowingText(VisualStyleRenderer rnd, IDeviceContext dc, Rectangle bounds, string text, Font font, Color color, System.Windows.Forms.TextFormatFlags flags)
		{
			DrawWrapper(rnd, dc, bounds,
				delegate(IntPtr memoryHdc) {
					// Create and select font
					using (NativeMethods.SafeDCObjectHandle fontHandle = new NativeMethods.SafeDCObjectHandle(memoryHdc, font.ToHfont()))
					{
						// Draw glowing text
						NativeMethods.DrawThemeTextOptions dttOpts = new NativeMethods.DrawThemeTextOptions(true);
						dttOpts.TextColor = color;
						dttOpts.GlowSize = 10;
						dttOpts.AntiAliasedAlpha = true;
						NativeMethods.Rect textBounds = new NativeMethods.Rect(4, 0, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
						NativeMethods.DrawThemeTextEx(rnd.Handle, memoryHdc, rnd.Part, rnd.State, text, text.Length, (int)flags, ref textBounds, ref dttOpts);
					}
				}
			);
		}

		/*public static void DrawGlowingText(this VisualStyleRenderer rnd, IDeviceContext dc, Rectangle bounds, string text, Font font, Color color, System.Windows.Forms.TextFormatFlags flags)
		{
			using (SafeGDIHandle primaryHdc = new SafeGDIHandle(dc))
			{
				// Create a memory DC so we can work offscreen
				using (SafeCompatibleDCHandle memoryHdc = new SafeCompatibleDCHandle(primaryHdc))
				{
					// Create a device-independent bitmap and select it into our DC
					BITMAPINFO info = new BITMAPINFO(bounds.Width, -bounds.Height);
					using (SafeDCObjectHandle dib = new SafeDCObjectHandle(memoryHdc, GDI.CreateDIBSection(primaryHdc, ref info, 0, 0, IntPtr.Zero, 0)))
					{
						// Create and select font
						using (SafeDCObjectHandle fontHandle = new SafeDCObjectHandle(memoryHdc, font.ToHfont()))
						{
							// Draw glowing text
							DrawThemeTextOptions dttOpts = new DrawThemeTextOptions(true);
							dttOpts.TextColor = color;
							dttOpts.GlowSize = 10;
							dttOpts.AntiAliasedAlpha = true;
							NativeMethods.RECT textBounds = new NativeMethods.RECT(4, 0, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
							DrawThemeTextEx(rnd.Handle, memoryHdc, rnd.Part, rnd.State, text, text.Length, (int)flags, ref textBounds, ref dttOpts);

							// Copy to foreground
							const int SRCCOPY = 0x00CC0020;
							GDI.BitBlt(primaryHdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, memoryHdc, 0, 0, SRCCOPY);
						}
					}
				}
			}
		}*/

		public static void DrawText(VisualStyleRenderer rnd, IDeviceContext dc, ref Rectangle bounds, string text, System.Windows.Forms.TextFormatFlags flags, NativeMethods.DrawThemeTextOptions options)
		{
			NativeMethods.Rect rc = new NativeMethods.Rect(bounds);
			using (SafeGDIHandle hdc = new SafeGDIHandle(dc))
			{
				NativeMethods.DrawThemeTextEx(rnd.Handle, hdc, rnd.Part, rnd.State, text, text.Length, (int)flags, ref rc, ref options);
			}
			bounds = rc;
		}

		[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public static Padding GetMargins2(VisualStyleRenderer rnd, IDeviceContext dc, MarginProperty prop)
		{
			NativeMethods.Rect rc;
			using (SafeGDIHandle hdc = new SafeGDIHandle(dc))
			{
				NativeMethods.GetThemeMargins(rnd.Handle, hdc, rnd.Part, rnd.State, (int)prop, IntPtr.Zero, out rc);
			}
			return new Padding(rc.Left, rc.Top, rc.Right, rc.Bottom);
		}

		/// <summary>
		/// Sets attributes to control how visual styles are applied to a specified window.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="attr">The attributes to apply or disable.</param>
		/// <param name="enable">if set to <c>true</c> enable the attribute, otherwise disable it.</param>
		public static void SetWindowThemeAttribute(IWin32Window window, NativeMethods.WindowThemeNonClientAttributes attr, bool enable = true)
		{
			NativeMethods.WTA_OPTIONS ops = new NativeMethods.WTA_OPTIONS();
			ops.Flags = attr;
			ops.Mask = enable ? (uint)attr : 0;
			try { NativeMethods.SetWindowThemeAttribute(window.Handle, NativeMethods.WindowThemeAttributeType.WTA_NONCLIENT, ref ops, Marshal.SizeOf(ops)); }
			catch (EntryPointNotFoundException) { }
			catch { throw; }
		}

		private static void DrawWrapper(VisualStyleRenderer rnd, IDeviceContext dc, Rectangle bounds, DrawWrapperMethod func)
		{
			using (SafeGDIHandle primaryHdc = new SafeGDIHandle(dc))
			{
				// Create a memory DC so we can work offscreen
				using (NativeMethods.SafeCompatibleDCHandle memoryHdc = new NativeMethods.SafeCompatibleDCHandle(primaryHdc))
				{
					// Create a device-independent bitmap and select it into our DC
					NativeMethods.BitmapInfo info = new NativeMethods.BitmapInfo(bounds.Width, -bounds.Height);
					using (NativeMethods.SafeDCObjectHandle dib = new NativeMethods.SafeDCObjectHandle(memoryHdc, NativeMethods.CreateDIBSection(primaryHdc, ref info, 0, IntPtr.Zero, IntPtr.Zero, 0)))
					{
						// Call method
						func(memoryHdc);

						// Copy to foreground
						const int SRCCOPY = 0x00CC0020;
						NativeMethods.BitBlt(primaryHdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, memoryHdc, 0, 0, SRCCOPY);
					}
				}
			}
		}
	}
}