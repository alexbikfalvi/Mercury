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
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Mercury.Windows.Controls.AeroWizard
{
	/// <summary>
	/// Represents a single page in a <see cref="WizardControl"/>.
	/// </summary>
	[Designer(typeof(Design.WizardPageDesigner)), DesignTimeVisible(true)]
	[DefaultProperty("Text"), DefaultEvent("Commit")]
	[ToolboxItem(false)]
	public partial class WizardPage : Control
	{
		private bool allowCancel = true, allowNext = true, allowBack = true;
		private bool showCancel = true, showNext = true;
		private bool isFinishPage = false;
		private string helpText = null;

		private string textNext = null;
		private string textFinish = null;

		private LinkLabel helpLink;

		/// <summary>
		/// Initializes a new instance of the <see cref="WizardPage"/> class.
		/// </summary>
		public WizardPage()
		{
			// Set the default properties.
			base.Margin = Padding.Empty;
			base.Text = WizardResources.WizardHeader;
			this.Suppress = false;
		}

		// Public events.

		/// <summary>
		/// Occurs when the user has clicked the Next/Finish button but before the page is changed.
		/// </summary>
		[Category("Wizard"), Description("Occurs when the user has clicked the Next/Finish button but before the page is changed.")]
		public event EventHandler<WizardPageConfirmEventArgs> Commit;

		/// <summary>
		/// Occurs when <see cref="HelpText"/> is set and the user has clicked the link at bottom of the content area.
		/// </summary>
		[Category("Wizard"), Description("Occurs when the user has clicked the help link.")]
		public event EventHandler HelpClicked;

		/// <summary>
		/// Occurs when this page is entered.
		/// </summary>
		[Category("Wizard"), Description("Occurs when this page is entered.")]
		public event EventHandler<WizardPageInitEventArgs> Initialize;

		/// <summary>
		/// Occurs when the user has clicked the Back button but before the page is changed.
		/// </summary>
		[Category("Wizard"), Description("Occurs when the user has clicked the Back button.")]
		public event EventHandler<WizardPageConfirmEventArgs> Rollback;

		// Public properties.

		/// <summary>
		/// Gets or sets a value indicating whether to enable the Back button.
		/// </summary>
		/// <value><c>true</c> if Back button is enabled; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Category("Behavior"), Description("Indicates whether to enable the Back button")]
		public virtual bool AllowBack
		{
			get { return allowBack; }
			set
			{
				if (allowBack != value)
				{
					allowBack = value;
					UpdateOwner();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable the Cancel button.
		/// </summary>
		/// <value><c>true</c> if Cancel button is enabled; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Category("Behavior"), Description("Indicates whether to enable the Cancel button")]
		public virtual bool AllowCancel
		{
			get { return allowCancel; }
			set
			{
				if (allowCancel != value)
				{
					allowCancel = value;
					this.UpdateOwner();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable the Next/Finish button.
		/// </summary>
		/// <value><c>true</c> if Next/Finish button is enabled; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Category("Behavior"), Description("Indicates whether to enable the Next/Finish button")]
		public virtual bool AllowNext
		{
			get { return allowNext; }
			set
			{
				if (allowNext != value)
				{
					allowNext = value;
					this.UpdateOwner();
				}
			}
		}

		/// <summary>
		/// Gets or sets the help text. When value is not <c>null</c>, a help link will be displayed at the bottom left of the content area. When clicked, the <see cref="OnHelpClicked"/> method will call the <see cref="HelpClicked"/> event.
		/// </summary>
		/// <value>
		/// The help text to display.
		/// </value>
		[DefaultValue((string)null), Category("Appearance"), Description("Help text to display on hyperlink at bottom left of content area.")]
		public string HelpText
		{
			get { return this.helpText; }
			set
			{
				if (this.helpLink == null)
				{
					this.helpLink = new LinkLabel() { AutoSize = true, Dock = DockStyle.Bottom, Text = "Help", Visible = false };
					this.helpLink.LinkClicked += new LinkLabelLinkClickedEventHandler(OnHelpLinkClicked);
					this.Controls.Add(this.helpLink);
				}
				this.helpText = value;
				if (helpText == null)
				{
					this.helpLink.Visible = false;
				}
				else
				{
					this.helpLink.Text = helpText;
					this.helpLink.Visible = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this page is the last page in the sequence and should display the Finish text instead of the Next text on the Next/Finish button.
		/// </summary>
		/// <value><c>true</c> if this page is a finish page; otherwise, <c>false</c>.</value>
		[DefaultValue(false), Category("Behavior"), Description("Indicates whether this page is the last page")]
		public virtual bool IsFinishPage
		{
			get { return isFinishPage; }
			set
			{
				if (isFinishPage != value)
				{
					isFinishPage = value;
					this.UpdateOwner();
				}
			}
		}

		/// <summary>
		/// Gets or sets the next page that should be used when the user clicks the Next button or when the <see cref="WizardControl.NextPage()"/> method is called. This is used to override the default behavior of going to the next page in the sequence defined within the <see cref="WizardControl.Pages"/> collection.
		/// </summary>
		/// <value>The wizard page to go to.</value>
		[DefaultValue(null), Category("Behavior"),
		Description("Specify a page other than the next page in the Pages collection as the next page.")]
		public virtual WizardPage NextPage { get; set; }

		/// <summary>
		/// Gets the <see cref="WizardControl"/> for this page.
		/// </summary>
		/// <value>The <see cref="WizardControl"/> for this page.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual WizardPageContainer Owner { get; internal set; }

		/// <summary>
		/// Gets or sets a value indicating whether to show the Cancel button. If both <see cref="ShowCancel"/> and <see cref="ShowNext"/> are <c>false</c>, then the bottom command area will not be shown.
		/// </summary>
		/// <value><c>true</c> if Cancel button should be shown; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Category("Behavior"), Description("Indicates whether to show the Cancel button")]
		public virtual bool ShowCancel
		{
			get { return showCancel; }
			set
			{
				if (showCancel != value)
				{
					showCancel = value;
					UpdateOwner();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show the Next/Finish button. If both <see cref="ShowCancel"/> and <see cref="ShowNext"/> are <c>false</c>, then the bottom command area will not be shown.
		/// </summary>
		/// <value><c>true</c> if Next/Finish button should be shown; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Category("Behavior"), Description("Indicates whether to show the Next/Finish button")]
		public virtual bool ShowNext
		{
			get { return showNext; }
			set
			{
				if (showNext != value)
				{
					showNext = value;
					this.UpdateOwner();
				}
			}
		}

		/// <summary>
		/// Gets or sets the height and width of the control.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The <see cref="T:System.Drawing.Size"/> that represents the height and width of the control in pixels.
		/// </returns>
		[Browsable(false)]
		public new Size Size { get { return base.Size; } set { base.Size = value; } }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="WizardPage"/> is suppressed and not shown in the normal flow.
		/// </summary>
		/// <value>
		///   <c>true</c> if suppressed; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(false), Category("Behavior"), Description("Suppresses this page from viewing if selected as next.")]
		public bool Suppress { get; set; }

		/// <summary>
		/// Gets or sets the text displayed on the Next button for this page.
		/// </summary>
		[DefaultValue((string)null), Category("Appearance"), Description("The text displayed on the Next button for this page.")]
		public string TextNext
		{
			get { return this.textNext; }
			set
			{
				if (this.textNext != value)
				{
					this.textNext = value;
					this.UpdateOwner();
				}
			}
		}

		/// <summary>
		/// Gets or sets the text displayed on the Finish button for this page.
		/// </summary>
		[DefaultValue((string)null), Category("Appearance"), Description("The text displayed on the Finish button for this page.")]
		public string TextFinish
		{
			get { return this.textFinish; }
			set
			{
				if (this.textFinish != value)
				{
					this.textFinish = value;
					this.UpdateOwner();
				}
			}
		}

		// Public methods.

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this wizard page.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this wizard page.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0} (\"{1}\")", this.Name, this.Text);
		}

		// Internal methods.

		/// <summary>
		/// Commits the page, rasing the <see cref="Commit" /> event.
		/// </summary>
		/// <returns><c>True</c> if handler does not set the <see cref="WizardPageConfirmEventArgs.Cancel"/> to <c>true</c>; otherwise, <c>false</c>.</returns>
		internal bool CommitPage()
		{
			return this.OnCommit();
		}

		/// <summary>
		/// Initializes the page.
		/// </summary>
		/// <param name="prevPage">The previous wizard page.</param>
		internal void InitializePage(WizardPage prevPage)
		{
			this.OnInitialize(prevPage);
		}

		/// <summary>
		/// Rolls-back the page.
		/// </summary>
		/// <returns><c>True</c> if handler does not set the <see cref="WizardPageConfirmEventArgs.Cancel"/> to <c>true</c>; otherwise, <c>false</c>.</returns>
		internal bool RollbackPage()
		{
			return this.OnRollback();
		}

		// Protected methods.

		/// <summary>
		/// Disposes the current object.
		/// </summary>
		/// <param name="disposing"><c>True</c> if the managed resources are disposed, <c>false</c> otherwise.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (null != this.helpLink)
				{
					this.helpLink.Dispose();
				}
			}
			// Call the base class method.
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the required creation parameters when the control handle is created.
		/// </summary>
		/// <returns>A <see cref="T:System.Windows.Forms.CreateParams" /> that contains the required creation parameters when the handle to the control is created.</returns>
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				CreateParams createParams = base.CreateParams;
				Form parent = this.FindForm();
				bool parentRightToLeftLayout = parent != null ? parent.RightToLeftLayout : false;
				if ((this.RightToLeft == RightToLeft.Yes) && parentRightToLeftLayout)
				{
					createParams.ExStyle |= 0x500000; // WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT
					createParams.ExStyle &= ~0x7000; // WS_EX_RIGHT | WS_EX_RTLREADING | WS_EX_LEFTSCROLLBAR
				}
				return createParams;
			}
		}

		/// <summary>
		/// Raises the <see cref="Commit" /> event.
		/// </summary>
		/// <returns><c>True</c> if handler does not set the <see cref="WizardPageConfirmEventArgs.Cancel"/> to <c>true</c>; otherwise, <c>false</c>.</returns>
		protected virtual bool OnCommit()
		{
			WizardPageConfirmEventArgs e =  new WizardPageConfirmEventArgs(this);
			if (this.Commit != null) this.Commit(this,e);
			return !e.Cancel;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnGotFocus(EventArgs e)
		{
			// Call the base class focus.
			base.OnGotFocus(e);
			// Focus the next control.
			Control firstChild = this.GetNextControl(this, true);
			if (firstChild != null)
			{
				firstChild.Focus();
			}
		}

		/// <summary>
		/// Raises the <see cref="HelpClicked"/> event.
		/// </summary>
		protected virtual void OnHelpClicked()
		{
			if (this.HelpClicked != null) this.HelpClicked(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="Initialize"/> event.
		/// </summary>
		/// <param name="prevPage">The page that was previously selected.</param>
		protected virtual void OnInitialize(WizardPage prevPage)
		{
			if (this.Initialize != null) this.Initialize(this, new WizardPageInitEventArgs(this, prevPage));
		}

		/// <summary>
		/// Raises the <see cref="Rollback"/> event.
		/// </summary>
		/// <returns><c>True</c> if handler does not set the <see cref="WizardPageConfirmEventArgs.Cancel"/> to <c>true</c>; otherwise, <c>false</c>.</returns>
		protected virtual bool OnRollback()
		{
			WizardPageConfirmEventArgs e = new WizardPageConfirmEventArgs(this);
			if (this.Rollback != null) this.Rollback(this, e);
			return !e.Cancel;
		}

		// Private methods.

		/// <summary>
		/// An event handler called when the user clicks on the page help link.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnHelpLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OnHelpClicked();
		}

		/// <summary>
		/// A method called when updating the owner of a wizard page.
		/// </summary>
		private void UpdateOwner()
		{
			if ((this.Owner != null) && (this == this.Owner.SelectedPage))
			{
				this.Owner.OnUpdateButtons();
			}
		}
	}

	/// <summary>
	/// Arguments supplied to the <see cref="WizardPage"/> events.
	/// </summary>
	public class WizardPageConfirmEventArgs : EventArgs
	{
		/// <summary>
		/// Creates a new wizard page confirm event arguments.
		/// </summary>
		/// <param name="page">The wizard page.</param>
		internal WizardPageConfirmEventArgs(WizardPage page)
		{
			this.Cancel = false;
			this.Page = page;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this action is to be cancelled or allowed.
		/// </summary>
		/// <value><c>true</c> if cancel; otherwise, <c>false</c> to allow. Default is <c>false</c>.</value>
		[DefaultValue(false)]
		public bool Cancel { get; set; }

		/// <summary>
		/// Gets the <see cref="WizardPage"/> that has raised the event.
		/// </summary>
		/// <value>The wizard page.</value>
		public WizardPage Page { get; private set; }
	}

	/// <summary>
	/// Arguments supplied to the <see cref="WizardPage.Initialize"/> event.
	/// </summary>
	public class WizardPageInitEventArgs : WizardPageConfirmEventArgs
	{
		/// <summary>
		/// Creates a wizard page init event arguments instance.
		/// </summary>
		/// <param name="page">The wizard page.</param>
		/// <param name="prevPage">The previous wizard page.</param>
		internal WizardPageInitEventArgs(WizardPage page, WizardPage prevPage)
			: base(page)
		{
			this.PreviousPage = prevPage;
		}

		/// <summary>
		/// Gets the <see cref="WizardPage"/> that was previously selected when the event was raised.
		/// </summary>
		/// <value>The previous wizard page.</value>
		public WizardPage PreviousPage { get; private set; }
	}
}