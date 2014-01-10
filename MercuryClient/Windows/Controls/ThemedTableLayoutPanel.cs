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
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Mercury.Windows.VisualStyles;

namespace Mercury.Windows.Controls
{
	/// <summary>
	/// A class for a themed table layout panel.
	/// </summary>
	internal class ThemedTableLayoutPanel : TableLayoutPanel
	{
		private VisualStyleRenderer renderer;

		/// <summary>
		/// Creates a new insyance.
		/// </summary>
		public ThemedTableLayoutPanel()
		{
			this.SetTheme(VisualStyleElement.Window.Dialog.Normal);
		}

		// Public properties.

		/// <summary>
		/// Gets or sets whether the panel supports watch focus.
		/// </summary>
		[DefaultValue(false), Category("Behavior")]
		public bool WatchFocus { get; set; }
		/// <summary>
		/// Gets or sets whether the panel supports glass look-and-feel.
		/// </summary>
		[DefaultValue(false), Category("Appearance")]
		public bool SupportGlass { get; set; }

		// Public methods.

		/// <summary>
		/// Sets the current theme.
		/// </summary>
		/// <param name="element">The visual style element.</param>
		public void SetTheme(VisualStyleElement element)
		{
			if (VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(element))
				renderer = new VisualStyleRenderer(element);
			else
				renderer = null;
		}

		/// <summary>
		/// Sets the current theme.
		/// </summary>
		/// <param name="className">The visual style class name.</param>
		/// <param name="part">The part.</param>
		/// <param name="state">The state.</param>
		public void SetTheme(string className, int part, int state)
		{
			if (VisualStyleRenderer.IsSupported)
			{
				try
				{
					renderer = new VisualStyleRenderer(className, part, state);
					return;
				}
				catch { }
			}
			renderer = null;
		}

		// Protected methods.

		/// <summary>
		/// An event handler called when the handle was created.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			// Call the base class method.
			base.OnHandleCreated(e);
			// Attach to form events.
			OnAttachToFormEvents();
		}

		/// <summary>
		/// An event handler called when painting the control.
		/// </summary>
		/// <param name="e">The event handler.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (!ControlExtensions.IsDesignMode(this) && SupportGlass && DesktopWindowManager.IsCompositionEnabled())
			{
				try { e.Graphics.Clear(System.Drawing.Color.Black); }
				catch { }
			}
			else
			{
				if (ControlExtensions.IsDesignMode(this) || renderer == null)
					try { e.Graphics.Clear(this.BackColor); }
					catch { }
				else
					renderer.DrawBackground(e.Graphics, this.ClientRectangle, e.ClipRectangle);
			}
			// Call the base class method.
			base.OnPaint(e);
		}

		/// <summary>
		/// Attaches this control to the form events.
		/// </summary>
		private void OnAttachToFormEvents()
		{
			// Find the current form.
			Form form = this.FindForm();
			// If the form exists and the control watches for focus.
			if ((form != null) && WatchFocus)
			{
				form.Activated += new System.EventHandler(OnFormGotFocus);
				form.Deactivate += new System.EventHandler(OnFormLostFocus);
			}
		}

		/// <summary>
		/// An event handler called when the form gets focus.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnFormGotFocus(object sender, EventArgs e)
		{
			// Call the base class method.
			base.OnGotFocus(e);
			if (renderer != null)
			{
				renderer.SetParameters(renderer.Class, renderer.Part, 1);
			}
			// Refresh the control.
			base.Refresh();
		}

		/// <summary>
		/// An event handler called when the form losts focus.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnFormLostFocus(object sender, EventArgs e)
		{
			// Call the base class method.
			base.OnLostFocus(e);
			if (renderer != null)
			{
				renderer.SetParameters(renderer.Class, renderer.Part, 2);
			}
			// Refresh the control.
			Refresh();
		}
	}
}
