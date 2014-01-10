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

using System.Runtime.InteropServices;

namespace Mercury.Windows.Native
{
	/// <summary>
	/// A class with native methods.
	/// </summary>
	internal static partial class NativeMethods
	{
		/// <summary>
		/// A structure representing a bitmap information.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct BitmapInfo
		{
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
			public byte bmiColorsRgbBlue;
			public byte bmiColorsRgbGreen;
			public byte bmiColorsRgbRed;
			public byte bmiColorsRgbReserved;

			/// <summary>
			/// Creates a new bitmap information of the specified width and height.
			/// </summary>
			/// <param name="width">The bitmap width.</param>
			/// <param name="height">The bitmap height.</param>
			public BitmapInfo(int width, int height)
				: this()
			{
				this.biSize = Marshal.SizeOf(typeof(BitmapInfo));
				this.biWidth = width;
				this.biHeight = height;
				this.biPlanes = 1;
				this.biBitCount = 32;
			}
		}
	}
}
