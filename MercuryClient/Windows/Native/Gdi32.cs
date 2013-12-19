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
using System.Runtime.InteropServices;

namespace DotNetApi.Windows.Native
{
	/// <summary>
	/// A class with native methods.
	/// </summary>
	internal static partial class NativeMethods
	{
		const string GDI32 = "gdi32.dll";

		[DllImport(GDI32, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport(GDI32, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport(GDI32, ExactSpelling = true, SetLastError = true)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport(GDI32, ExactSpelling = true, SetLastError = true)]
		public static extern bool DeleteDC(IntPtr hdc);

		[DllImport(GDI32, ExactSpelling = true, SetLastError = true)]
		public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

		[DllImport(GDI32, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr CreateDIBSection(IntPtr hdc, ref NativeMethods.BitmapInfo pbmi, uint iUsage, IntPtr ppvBits, IntPtr hSection, uint dwOffset);

		[DllImport(GDI32, ExactSpelling = true, SetLastError = true)]
		public static extern uint SetLayout(IntPtr hdc, uint dwLayout);
	}
}