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
using System.Windows.Forms;

namespace DotNetApi.Windows.Controls
{
	/// <summary>
	/// A class with control extension methods.
	/// </summary>
	public static class ControlExtensions
	{
		/// <summary>
		/// Performs an action on a control after its handle has been created. If the control's handle has already been created, the action is executed immediately.
		/// </summary>
		/// <param name="ctrl">This control.</param>
		/// <param name="action">The action to execute.</param>
		public static void CallWhenHandleValid(Control ctrl, Action<Control> action)
		{
			if (ctrl.IsHandleCreated)
			{
				action(ctrl);
			}
			else
			{
				LayoutEventHandler handler = null;
				handler = (sender, e) =>
				{
					if (ctrl.IsHandleCreated)
					{
						ctrl.Layout -= handler;
						action(ctrl);
					}
				};
				ctrl.Layout += handler;
			}
		}

		/// <summary>
		/// Gets the control in the list of parents of type <c>T</c>.
		/// </summary>
		/// <typeparam name="T">The <see cref="Control"/> based <see cref="Type"/> of the parent control to retrieve.</typeparam>
		/// <param name="ctrl">This control.</param>
		/// <returns>The parent control matching T or null if not found.</returns>
		public static T GetParent<T>(Control ctrl) where T : Control, new()
		{
			Control p = ctrl.Parent;
			while (p != null & !(p is T))
				p = p.Parent;
			return p as T;
		}

		/// <summary>
		/// Gets the right to left property.
		/// </summary>
		/// <param name="ctrl">This control.</param>
		/// <returns>Culture defined direction of text for this control.</returns>
		public static RightToLeft GetRightToLeftProperty(Control ctrl)
		{
			if (ctrl.RightToLeft == RightToLeft.Inherit)
				return GetRightToLeftProperty(ctrl.Parent);
			return ctrl.RightToLeft;
		}

		/// <summary>
		/// Determines whether this control is in design mode.
		/// </summary>
		/// <param name="ctrl">This control.</param>
		/// <returns><c>true</c> if in design mode; otherwise, <c>false</c>.</returns>
		public static bool IsDesignMode(Control ctrl)
		{
			Control p = ctrl;
			while (p != null)
			{
				var site = p.Site;
				if (site != null && site.DesignMode)
					return true;
				p = p.Parent;
			}
			return false;
		}
	}
}
