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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Mercury.Windows.VisualStyles;

namespace Mercury.Windows.Controls.AeroWizard
{
	/// <summary>
	/// Image button that can be displayed on glass.
	/// </summary>
	[ToolboxBitmap(typeof(Button))]
	internal class XThemeImageButton : ThemedImageButton
	{
		private const string defaultText = "";

		private Image imageStrip;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThemeImageButton"/> class.
		/// </summary>
		public XThemeImageButton()
		{
			StyleClass = "BUTTON";
			StylePart = 1;
			Text = defaultText;
		}

		/// <summary>
		/// Gets or sets the compatible image strip used when visual style rendering is not available.
		/// </summary>
		/// <value>The compatible image strip.</value>
		[DefaultValue(null), Category("Appearance")]
		public Image CompatibleImageStrip
		{
			get { return imageStrip; }
			set { base.SetImageListImageStrip(imageStrip = value, Orientation.Vertical); }
		}

		/// <summary>
		/// Gets or sets the style class.
		/// </summary>
		/// <value>The style class.</value>
		[DefaultValue("BUTTON"), Category("Appearance")]
		public new string StyleClass { get; set; }

		/// <summary>
		/// Gets or sets the style part.
		/// </summary>
		/// <value>The style part.</value>
		[DefaultValue(1), Category("Appearance")]
		public new int StylePart { get; set; }

		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		/// <returns>
		/// The text associated with this control.
		///   </returns>
		[DefaultValue(defaultText), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>
		/// Primary function for painting the button. This method should be overridden instead of OnPaint.
		/// </summary>
		/// <param name="graphics">The graphics.</param>
		/// <param name="bounds">The bounds.</param>
		protected override void PaintButton(Graphics graphics, Rectangle bounds)
		{
			if (Application.RenderWithVisualStyles)
			{
				try
				{
					VisualStyleRenderer rnd = new VisualStyleRenderer(StyleClass, StylePart, (int)ButtonState);
					if (ControlExtensions.IsDesignMode(this) || !DesktopWindowManager.IsCompositionEnabled())
					{
						rnd.DrawParentBackground(graphics, bounds, this);
						rnd.DrawBackground(graphics, bounds);
					}
					else
					{
						VisualStyleRendererExtension.DrawGlassBackground(rnd, graphics, bounds, bounds);
					}
					return;
				}
				catch { }
			}
			else
			{
				base.PaintButton(graphics, bounds);
			}
		}
	}
}