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

namespace Mercury.Windows.Native
{
	internal static partial class NativeMethods
	{
		internal const string dllUxtheme = "uxtheme.dll";

		public enum DrawThemeTextSystemFonts
		{
			Caption = 801,
			SmallCaption = 802,
			Menu = 803,
			Status = 804,
			MessageBox = 805,
			IconTitle = 806
		}

		public enum TextShadowType : int
		{
			None = 0,
			Single = 1,
			Continuous = 2
		}

		[Flags]
		public enum WindowThemeNonClientAttributes : uint
		{
			/// <summary>Do Not Draw The Caption (Text)</summary>
			NoDrawCaption = 0x00000001,
			/// <summary>Do Not Draw the Icon</summary>
			NoDrawIcon = 0x00000002,
			/// <summary>Do Not Show the System Menu</summary>
			NoSysMenu = 0x00000004,
			/// <summary>Do Not Mirror the Question mark Symbol</summary>
			NoMirrorHelp = 0x00000008
		}

		[Flags]
		public enum DrawThemeTextOptionsFlags : int
		{
			TextColor = 1,
			BorderColor = 2,
			ShadowColor = 4,
			ShadowType = 8,
			ShadowOffset = 16,
			BorderSize = 32,
			FontProp = 64,
			ColorProp = 128,
			StateId = 256,
			CalcRect = 512,
			ApplyOverlay = 1024,
			GlowSize = 2048,
			Callback = 4096,
			Composited = 8192
		}

		public enum WindowThemeAttributeType
		{
			WTA_NONCLIENT = 1,
		}

		[DllImport(NativeMethods.dllUxtheme)]
		public static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.Rect pRect, ref NativeMethods.Rect pClipRect);

		[DllImport(NativeMethods.dllUxtheme)]
		public static extern int DrawThemeIcon(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.Rect pRect, IntPtr himl, int iImageIndex);

		[DllImport(NativeMethods.dllUxtheme, CharSet = CharSet.Unicode)]
		public static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref NativeMethods.Rect pRect, ref DrawThemeTextOptions pOptions);

		[DllImport(NativeMethods.dllUxtheme, ExactSpelling = true)]
		public static extern int GetThemeMargins(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, IntPtr prc, out NativeMethods.Rect pMargins);

		[DllImport(NativeMethods.dllUxtheme, ExactSpelling = true, PreserveSig = false)]
		public static extern void SetWindowThemeAttribute(IntPtr hWnd, WindowThemeAttributeType wtype, ref WTA_OPTIONS attributes, int size);

		[StructLayout(LayoutKind.Sequential)]
		public struct DrawThemeTextOptions
		{
			int dwSize;
			DrawThemeTextOptionsFlags dwFlags;
			int crText;
			int crBorder;
			int crShadow;
			TextShadowType iTextShadowType;
			System.Drawing.Point ptShadowOffset;
			int iBorderSize;
			int iFontPropId;
			int iColorPropId;
			int iStateId;
			bool fApplyOverlay;
			int iGlowSize;
			int pfnDrawTextCallback;
			IntPtr lParam;

			public DrawThemeTextOptions(bool init) : this()
			{
				dwSize = Marshal.SizeOf(typeof(DrawThemeTextOptions));
			}

			public Color AlternateColor
			{
				get { return ColorTranslator.FromWin32(iColorPropId); }
				set
				{
					iColorPropId = ColorTranslator.ToWin32(value);
					dwFlags |= DrawThemeTextOptionsFlags.ColorProp;
				}
			}

			public DrawThemeTextSystemFonts AlternateFont
			{
				get { return (DrawThemeTextSystemFonts)iFontPropId; }
				set
				{
					iFontPropId = (int)value;
					dwFlags |= DrawThemeTextOptionsFlags.FontProp;
				}
			}

			public bool AntiAliasedAlpha
			{
				get { return (dwFlags & DrawThemeTextOptionsFlags.Composited) == DrawThemeTextOptionsFlags.Composited; }
				set
				{
					if (value)
						dwFlags |= DrawThemeTextOptionsFlags.Composited;
					else
						dwFlags &= ~DrawThemeTextOptionsFlags.Composited;
				}
			}

			public bool ApplyOverlay
			{
				get { return fApplyOverlay; }
				set
				{
					fApplyOverlay = value;
					dwFlags |= DrawThemeTextOptionsFlags.ApplyOverlay;
				}
			}

			public Color BorderColor
			{
				get { return ColorTranslator.FromWin32(crBorder); }
				set
				{
					crBorder = ColorTranslator.ToWin32(value);
					dwFlags |= DrawThemeTextOptionsFlags.BorderColor;
				}
			}

			public int BorderSize
			{
				get { return iBorderSize; }
				set
				{
					iBorderSize = value;
					dwFlags |= DrawThemeTextOptionsFlags.BorderSize;
				}
			}

			public int GlowSize
			{
				get { return iGlowSize; }
				set
				{
					iGlowSize = value;
					dwFlags |= DrawThemeTextOptionsFlags.GlowSize;
				}
			}

			public bool ReturnCalculatedRectangle
			{
				get { return (dwFlags & DrawThemeTextOptionsFlags.CalcRect) == DrawThemeTextOptionsFlags.CalcRect; }
				set
				{
					if (value)
						dwFlags |= DrawThemeTextOptionsFlags.CalcRect;
					else
						dwFlags &= ~DrawThemeTextOptionsFlags.CalcRect;
				}
			}

			public Color ShadowColor
			{
				get { return ColorTranslator.FromWin32(crShadow); }
				set
				{
					crShadow = ColorTranslator.ToWin32(value);
					dwFlags |= DrawThemeTextOptionsFlags.ShadowColor;
				}
			}

			public Point ShadowOffset
			{
				get { return new Point(ptShadowOffset.X, ptShadowOffset.Y); }
				set
				{
					ptShadowOffset = value;
					dwFlags |= DrawThemeTextOptionsFlags.ShadowOffset;
				}
			}

			public TextShadowType ShadowType
			{
				get { return iTextShadowType; }
				set
				{
					iTextShadowType = value;
					dwFlags |= DrawThemeTextOptionsFlags.ShadowType;
				}
			}

			public Color TextColor
			{
				get { return ColorTranslator.FromWin32(crText); }
				set
				{
					crText = ColorTranslator.ToWin32(value);
					dwFlags |= DrawThemeTextOptionsFlags.TextColor;
				}
			}
		}

		/// <summary>
		/// The Options of What Attributes to Add/Remove
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct WTA_OPTIONS
		{
			public WindowThemeNonClientAttributes Flags;
			public uint Mask;
		}
	}
}
