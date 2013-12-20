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

namespace DotNetApi.Windows.Native
{
	/// <summary>
	/// A class with native methods.
	/// </summary>
	internal static partial class NativeMethods
	{
		public class SafeDCObjectHandle : SafeHandle
		{
			public SafeDCObjectHandle(IntPtr hdc, IntPtr hObj)
				: base(IntPtr.Zero, true)
			{
				if (hdc != null)
				{
					NativeMethods.SelectObject(hdc, hObj);
					base.SetHandle(hObj);
				}
			}

			public override bool IsInvalid
			{
				get { return base.handle == IntPtr.Zero; }
			}

			public static implicit operator IntPtr(SafeDCObjectHandle h)
			{
				return h.DangerousGetHandle();
			}

			protected override bool ReleaseHandle()
			{
				if (!IsInvalid)
					NativeMethods.DeleteObject(base.handle);
				return true;
			}
		}

		public class SafeCompatibleDCHandle : SafeHandle
		{
			public SafeCompatibleDCHandle(IntPtr hdc)
				: base(IntPtr.Zero, true)
			{
				if (hdc != null)
				{
					base.SetHandle(NativeMethods.CreateCompatibleDC(hdc));
				}
			}

			public override bool IsInvalid
			{
				get { return base.handle == IntPtr.Zero; }
			}

			public static implicit operator IntPtr(SafeCompatibleDCHandle h)
			{
				return h.DangerousGetHandle();
			}

			protected override bool ReleaseHandle()
			{
				if (!IsInvalid)
					NativeMethods.DeleteDC(base.handle);
				return true;
			}
		}
	}

	
	internal class SafeGDIHandle : SafeHandle
	{
		private IDeviceContext idc;

		[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public SafeGDIHandle(IDeviceContext dc)
			: base(IntPtr.Zero, true)
		{
			if (dc != null)
			{
				idc = dc;
				base.SetHandle(idc.GetHdc());
			}
		}

		public override bool IsInvalid
		{
			get { return base.handle == IntPtr.Zero; }
		}

		[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public static implicit operator IntPtr(SafeGDIHandle h)
		{
			return h.DangerousGetHandle();
		}

		protected override bool ReleaseHandle()
		{
			if (idc != null)
				idc.ReleaseHdc();
			return true;
		}
	}
}
