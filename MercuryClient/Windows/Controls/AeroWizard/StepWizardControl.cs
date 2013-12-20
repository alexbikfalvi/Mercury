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

namespace DotNetApi.Windows.Controls.AeroWizard
{
	/// <summary>
	/// Wizard control that shows a step summary on the left of the wizard page area.
	/// </summary>
	public class StepWizardControl : WizardControl
	{
        private StepList list;
        private Splitter splitter;

		/// <summary>
		/// Initializes a new instance of the <see cref="StepWizardControl"/> class.
		/// </summary>
		public StepWizardControl()
		{
            this.pageContainer.Controls.Add(splitter = new Splitter() { Dock = DockStyle.Left, BorderStyle = BorderStyle.FixedSingle, Width = 1 });
            this.pageContainer.Controls.Add(list = new StepList() { Dock = DockStyle.Left });
			this.Pages.AfterCleared += this.OnAfterPagesCleared;
		}

        /// <summary>
        /// Gets or sets the width of the step list.
        /// </summary>
        /// <value>
        /// The width of the step list.
        /// </value>
        [DefaultValue(150), Category("Appearance"), Description("Determines width of step list on left.")]
        public int StepListWidth
        {
            get { return list.Width; }
            set { list.Width = value; }
        }

		// Private methods.

		/// <summary>
		/// An event handler called after the pages list has been cleared.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
        private void OnAfterPagesCleared(object sender, EventArgs e)
        {
            this.pageContainer.Controls.Add(splitter);
            this.pageContainer.Controls.Add(list);
        }
	}
}
