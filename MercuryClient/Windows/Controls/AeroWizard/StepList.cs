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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DotNetApi.Windows.Controls.AeroWizard
{
	/// <summary>
	/// Shows a list of all the pages in the WizardControl
	/// </summary>
	[ProvideProperty("StepText", typeof(WizardPage))]
    [ProvideProperty("StepTextIndentLevel", typeof(WizardPage))]
    internal class StepList : ScrollableControl, IExtenderProvider
	{
		private WizardControl parent;
		private Dictionary<WizardPage, string> stepTexts = new Dictionary<WizardPage, string>();
        private Dictionary<WizardPage, int> indentLevels = new Dictionary<WizardPage, int>();

		/// <summary>
		/// Initializes a new instance of the <see cref="StepList"/> class.
		/// </summary>
		public StepList()
		{
		}

		/// <summary>
		/// Gets the default size of the control.
		/// </summary>
		/// <returns>The default <see cref="T:System.Drawing.Size" /> of the control.</returns>
		protected override Size DefaultSize { get { return new Size(150, 200); } }

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.ParentChanged" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			SetupControl(ControlExtensions.GetParent<WizardControl>(this));
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (parent == null) return;

			using (Font ptrFont = new Font("Marlett", this.Font.Size), boldFont = new Font(this.Font, FontStyle.Bold))
			{
				int itemHeight = (int)Math.Ceiling(TextRenderer.MeasureText(e.Graphics, "Wg", this.Font).Height * 1.2);
                int lPad = TextRenderer.MeasureText(e.Graphics, "4", ptrFont, new Size(0, itemHeight), TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding).Width;
				const int rPad = 4;
				Rectangle rect = new Rectangle(lPad, 0, this.Width - lPad - rPad, itemHeight);
				Rectangle prect = new Rectangle(0, 0, lPad, itemHeight);
				WizardPageCollection pages = parent.Pages;
				bool hit = false;
				for (int i = 0; i < pages.Count && rect.Y < (this.Height - itemHeight); i++)
				{
					if (!pages[i].Suppress)
					{
						Color fc = this.ForeColor, bc = this.BackColor;
                        bool isSelected = parent.SelectedPage == pages[i];
                        int level = GetStepTextIndentLevel(pages[i]);
                        prect.X = lPad * level;
                        rect.X = lPad * (level + 1);
						if (isSelected)
						{
							hit = true;
						}
						else if (!hit)
						{
							fc = SystemColors.GrayText;
						}
						using (Brush br = new SolidBrush(bc))
						{
							e.Graphics.FillRectangle(br, Rectangle.Union(rect, prect));
						}
						TextRenderer.DrawText(e.Graphics, hit ? "4" : "a", ptrFont, prect, fc, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
						TextRenderer.DrawText(e.Graphics, GetStepText(pages[i]), isSelected ? boldFont : this.Font, rect, fc, TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter);
						prect.Y = rect.Y += itemHeight;
					}
				}
			}
		}

		/// <summary>
		/// Specifies whether this object can provide its extender properties to the specified object.
		/// </summary>
		/// <param name="extendee">The object to receive the extender properties.</param>
		/// <returns><b>True</b> if this object can provide extender properties to the specified object; <b>false</b> otherwise.</returns>
		bool IExtenderProvider.CanExtend(object extendee)
		{
			return (extendee is WizardPage);
		}

		/// <summary>
		/// Gets the step text.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns>Step text for the specified wizard page.</returns>
		[DefaultValue((string)null), Category("Appearance"), Description("Alternate text to provide to the StepList. Default value comes the Text property of the WizardPage.")]
		public string GetStepText(WizardPage page)
		{
			string value;
			if (stepTexts.TryGetValue(page, out value))
				return value;
			return page.Text;
		}

		/// <summary>
		/// Sets the step text.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="value">The value.</param>
		public void SetStepText(WizardPage page, string value)
		{
			if (string.IsNullOrEmpty(value) || value == page.Text)
				stepTexts.Remove(page);
			else
				stepTexts[page] = value;
			Refresh();
		}

        /// <summary>
        /// Gets the step text indent level.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>Step text indent level for the specified wizard page.</returns>
        [DefaultValue(0), Category("Appearance"), Description("Indentation level for text provided to the StepList.")]
        public int GetStepTextIndentLevel(WizardPage page)
        {
            int value;
            if (indentLevels.TryGetValue(page, out value))
                return value;
            return 0;
        }

        /// <summary>
        /// Sets the step text indent level.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="value">The indent level.</param>
        public void SetStepTextIndentLevel(WizardPage page, int value)
        {
            if (value < 0) value = 0;
            if (value == 0)
                indentLevels.Remove(page);
            else
                indentLevels[page] = value;
			this.Refresh();
        }

		// Private methods.

		/// <summary>
		/// Initializes the control.
		/// </summary>
		/// <param name="parent">The control parent.</param>
		private void SetupControl(WizardControl parent)
		{
			// If the current parent is not null.
			if (this.parent != null)
			{
				// Remove the parent pages event handlers.
				WizardPageCollection pages = parent.Pages;
				pages.AfterCleared -= this.OnPagesChanged;
				pages.AfterItemInserted -= this.OnPagesChanged;
				pages.AfterItemRemoved -= this.OnPagesChanged;
				pages.AfterItemSet -= this.OnPagesChanged;
			}
			// Set the new parent.
			this.parent = parent;
			// If the new parent is not null.
			if (parent != null)
			{
				// Add the parent pages event handlers.
				WizardPageCollection pages = parent.Pages;
				pages.AfterCleared += this.OnPagesChanged;
				pages.AfterItemInserted += this.OnPagesChanged;
				pages.AfterItemRemoved += this.OnPagesChanged;
				pages.AfterItemSet += this.OnPagesChanged;
			}
			// Refresh the control.
			this.Refresh();
		}

		/// <summary>
		/// An event handler called when the list of pages ahs changed.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesChanged(object sender, EventArgs e)
		{
			// Refresh.
			this.Refresh();
		}

		/// <summary>
		/// Indicates whether the step text should be serialized.
		/// </summary>
		/// <param name="page">The step wizard page.</param>
		/// <returns><b>True</b> if the text should be serialized, <b>false</b> otherwise.</returns>
		private bool ShouldSerializeStepText(WizardPage page)
		{
			return (GetStepText(page) != page.Text);
		}

		/// <summary>
		/// Resets the step text.
		/// </summary>
		/// <param name="page">The step wizard page.</param>
		private void ResetStepText(WizardPage page)
		{
			SetStepText(page, null);
		}
    }
}
