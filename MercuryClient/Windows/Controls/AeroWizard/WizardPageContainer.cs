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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Mercury.Windows.Native;

namespace Mercury.Windows.Controls.AeroWizard
{
	/// <summary>
	/// Button state for buttons controlling the wizard.
	/// </summary>
	public enum WizardCommandButtonState
	{
		/// <summary>Button is enabled and can be clicked.</summary>
		Enabled,
		/// <summary>Button is disabled and cannot be clicked.</summary>
		Disabled,
		/// <summary>Button is hidden from the user.</summary>
		Hidden
	}

	/// <summary>
	/// Control providing a collection of wizard style navigatable pages.
	/// </summary>
	[Designer(typeof(Design.WizardBaseDesigner))]
	[ToolboxItem(true)]
	[Description("Provides a container for wizard pages.")]
	[DefaultProperty("Pages"), DefaultEvent("SelectedPageChanged")]
	public class WizardPageContainer : ContainerControl, ISupportInitialize
	{
		private readonly WizardPageCollection pages;
		private readonly Stack<WizardPage> pageHistory = new Stack<WizardPage>();

		private WizardPage selectedPage = null;

		private ButtonBase backButton;
		private ButtonBase nextButton;
		private ButtonBase cancelButton;

		private bool initialized = false;
		private bool initializing = false;
		private bool nextButtonShieldEnabled = false;

		private string nextButtonText;
		private string finishButtonText;

		/// <summary>
		/// Initializes a new instance of the <see cref="WizardPageContainer"/> class.
		/// </summary>
		public WizardPageContainer()
		{
			this.pages = new WizardPageCollection(this);
			
			this.pages.BeforeCleared += this.OnPagesCleared;
			this.pages.AfterItemInserted += this.OnPagesItemInserted;
			this.pages.AfterItemRemoved += this.OnPagesItemRemoved;
			this.pages.AfterItemSet += this.OnPagesItemSet;

			this.OnRightToLeftChanged(EventArgs.Empty);

			// Get localized defaults for button text
			this.ResetBackButtonText();
			this.ResetCancelButtonText();
			this.ResetFinishButtonText();
			this.ResetNextButtonText();
		}

		/// <summary>
		/// Occurs when the button's state has changed.
		/// </summary>
		[Category("Property Changed"), Description("Occurs when any of the button's state has changed.")]
		public event EventHandler ButtonStateChanged;

		/// <summary>
		/// Occurs when the user clicks the Cancel button and allows for programmatic cancellation.
		/// </summary>
		[Category("Behavior"), Description("Occurs when the user clicks the Cancel button and allows for programmatic cancellation.")]
		public event CancelEventHandler Cancelling;

		/// <summary>
		/// Occurs when the user clicks the Next/Finish button and the page is set to <see cref="WizardPage.IsFinishPage"/> or this is the last page in the <see cref="Pages"/> collection.
		/// </summary>
		[Category("Behavior"), Description("Occurs when the user clicks the Next/Finish button on last page.")]
		public event EventHandler Finished;

		/// <summary>
		/// Occurs when the <see cref="WizardControl.SelectedPage"/> property has changed.
		/// </summary>
		[Category("Property Changed"), Description("Occurs when the SelectedPage property has changed.")]
		public event EventHandler SelectedPageChanged;

		/// <summary>
		/// Gets or sets the button assigned to control backing up through the pages.
		/// </summary>
		/// <value>The back button control.</value>
		[Category("Wizard"), Description("Button used to command backward wizard flow.")]
		public ButtonBase BackButton
		{
			get { return backButton; }
			set
			{
				if (this.backButton != null)
				{
					this.backButton.Click -= this.OnBackButtonClick;
				}
				this.backButton = value;
				if (this.backButton != null)
				{
					this.backButton.Click += this.OnBackButtonClick;
				}
			}
		}

		/// <summary>
		/// Gets or sets the button assigned to control moving forward through the pages.
		/// </summary>
		/// <value>The next button control.</value>
		[Category("Wizard"), Description("Button used to command forward wizard flow.")]
		public ButtonBase NextButton
		{
			get { return nextButton; }
			set
			{
				if (this.nextButton != null)
				{
					this.nextButton.Click -= this.NextButtonClick;
				}
				this.nextButton = value;
				if (this.nextButton != null)
				{
					this.nextButton.Click += this.NextButtonClick;
				}
			}
		}

		/// <summary>
		/// Gets or sets the button assigned to cancelling the page flow.
		/// </summary>
		/// <value>The cancel button control.</value>
		[Category("Wizard"), Description("Button used to cancel wizard flow.")]
		public ButtonBase CancelButton
		{
			get { return cancelButton; }
			set
			{
				if (this.cancelButton != null)
				{
					this.cancelButton.Click -= this.OnCancelButtonClick;
				}
				this.cancelButton = value;
				if (this.cancelButton != null)
				{
					this.cancelButton.Click += this.OnCancelButtonClick;
				}
			}
		}

		/// <summary>
		/// Gets or sets the state of the back button.
		/// </summary>
		/// <value>The state of the back button.</value>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WizardCommandButtonState BackButtonState
		{
			get { return this.GetCmdButtonState(this.backButton); }
			internal set { this.SetButtonState(this.backButton, value); }
		}

		/// <summary>
		/// Gets or sets the back button text.
		/// </summary>
		/// <value>The cancel button text.</value>
		[Category("Wizard"), Localizable(true), Description("The back button text")]
		public string BackButtonText
		{
			get { return this.GetCmdButtonText(this.backButton); }
			set { this.SetButtonText(this.backButton, value); }
		}

		/// <summary>
		/// Gets the state of the cancel button.
		/// </summary>
		/// <value>The state of the cancel button.</value>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WizardCommandButtonState CancelButtonState
		{
			get { return this.GetCmdButtonState(this.cancelButton); }
			internal set { this.SetButtonState(this.cancelButton, value); }
		}

		/// <summary>
		/// Gets or sets the cancel button text.
		/// </summary>
		/// <value>The cancel button text.</value>
		[Category("Wizard"), Localizable(true), Description("The cancel button text")]
		public string CancelButtonText
		{
			get { return this.GetCmdButtonText(this.cancelButton); }
			set { this.SetButtonText(this.cancelButton, value); }
		}

		/// <summary>
		/// Gets or sets the finish button text.
		/// </summary>
		/// <value>The finish button text.</value>
		[Category("Wizard"), Localizable(true), Description("The finish button text")]
		public string FinishButtonText
		{
			get { return this.finishButtonText; }
			set
			{
				finishButtonText = value;
				if (selectedPage != null && selectedPage.IsFinishPage && !ControlExtensions.IsDesignMode(this))
				{
					SetButtonText(NextButton, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the shield icon on the next button.
		/// </summary>
		/// <value><c>true</c> if Next button should display a shield; otherwise, <c>false</c>.</value>
		/// <exception cref="PlatformNotSupportedException">Setting a UAF shield on a button only works on Vista and later versions of Windows.</exception>
		[DefaultValue(false), Category("Wizard"), Description("Show a shield icon on the next button")]
		public Boolean NextButtonShieldEnabled
		{
			get { return nextButtonShieldEnabled; }
			set
			{
				if (System.Environment.OSVersion.Version.Major >= 6)
				{
					const uint BCM_FIRST = 0x1600;                      //Normal button.
					const uint BCM_SETSHIELD = (BCM_FIRST + 0x000C);    //Elevated button.

					nextButtonShieldEnabled = value;

					if (value)
					{
						NextButton.FlatStyle = FlatStyle.System;
						NativeMethods.SendMessage(NextButton.Handle, BCM_SETSHIELD, (IntPtr)0, (IntPtr)0xFFFFFFFF);
					}
					else
					{
						NextButton.FlatStyle = FlatStyle.Standard;
						NativeMethods.SendMessage(NextButton.Handle, BCM_FIRST, (IntPtr)0, (IntPtr)0xFFFFFFFF);
					}

					NextButton.Invalidate();
				}
				else
					throw new PlatformNotSupportedException();
			}
		}

		/// <summary>
		/// Gets the state of the next button.
		/// </summary>
		/// <value>The state of the next button.</value>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WizardCommandButtonState NextButtonState
		{
			get { return GetCmdButtonState(NextButton); }
			internal set { SetButtonState(NextButton, value); }
		}

		/// <summary>
		/// Gets or sets the next button text.
		/// </summary>
		/// <value>The next button text.</value>
		[Category("Wizard"), Localizable(true), Description("The next button text.")]
		public string NextButtonText
		{
			get { return this.nextButtonText; }
			set
			{
				this.nextButtonText = value;
				if (!ControlExtensions.IsDesignMode(this) && (this.selectedPage == null || !this.selectedPage.IsFinishPage))
				{
					this.SetButtonText(this.NextButton, value);
				}
			}
		}

		/// <summary>
		/// Gets the collection of wizard pages in this wizard control.
		/// </summary>
		/// <value>The <see cref="WizardPageCollection"/> that contains the <see cref="WizardPage"/> objects in this <see cref="WizardControl"/>.</value>
		[Category("Wizard"), Description("Collection of wizard pages.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public WizardPageCollection Pages { get { return this.pages; } }

		/// <summary>
		/// Gets the currently selected wizard page.
		/// </summary>
		/// <value>The selected wizard page. <c>null</c> if no page is active.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual WizardPage SelectedPage
		{
			get
			{
				if ((this.selectedPage == null) || (this.Pages.Count == 0))
					return null;
				return this.selectedPage;
			}
			internal set
			{
				if (value != null && !Pages.Contains(value))
					throw new ArgumentException("WizardPage is not in the Pages collection for the control.");

				if (value != selectedPage)
				{
					WizardPage prev = selectedPage;
					if (selectedPage != null)
						selectedPage.Hide();
					selectedPage = value;
					int idx = SelectedPageIndex;
					if (!ControlExtensions.IsDesignMode(this))
					{
						while (idx < Pages.Count - 1 && selectedPage.Suppress)
							selectedPage = Pages[++idx];
					}
					if (selectedPage != null)
					{
						//this.HeaderText = selectedPage.Text;
						selectedPage.InitializePage(prev);
						selectedPage.Dock = DockStyle.Fill;
						selectedPage.PerformLayout();
						selectedPage.Show();
						selectedPage.BringToFront();
						selectedPage.Focus();
					}
					this.OnUpdateButtons();
					this.OnSelectedPageChanged();
				}
			}
		}

		/// <summary>
		/// Gets the index of the currently selected page.
		/// </summary>
		/// <value>The index of the selected page.</value>
		internal int SelectedPageIndex
		{
			get
			{
				if (selectedPage == null)
					return -1;
				return Pages.IndexOf(selectedPage);
			}
		}

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		public void BeginInit()
		{
			initializing = true;
		}

		/// <summary>
		/// Signals the object that initialization is complete.
		/// </summary>
		public void EndInit()
		{
			initializing = false;
		}

		/// <summary>
		/// Advances to the next page in the sequence.
		/// </summary>
		public void NextPage()
		{
			this.NextPage(null);
		}

		/// <summary>
		/// Advances to the specified page.
		/// </summary>
		/// <param name="nextPage">The wizard page to go to next.</param>
		/// <param name="skipCommit">if set to <c>true</c> skip <see cref="WizardPage.Commit"/> event.</param>
		/// <exception cref="ArgumentException">When specifying a value for nextPage, it must already be in the Pages collection.</exception>
		public virtual void NextPage(WizardPage nextPage, bool skipCommit = false)
		{
			// When in design mode.
			if (ControlExtensions.IsDesignMode(this))
			{
				// Get the current page.
				int idx = this.SelectedPageIndex;
				// Select the next page.
				if (idx < this.Pages.Count - 1)
				{
					this.SelectedPage = this.Pages[idx + 1];
				}
				return;
			}

			if (skipCommit || SelectedPage.CommitPage())
			{
				this.pageHistory.Push(SelectedPage);

				if (nextPage != null)
				{
					if (!Pages.Contains(nextPage))
					{
						throw new ArgumentException("When specifying a value for nextPage, it must already be in the Pages collection.", "nextPage");
					}
					SelectedPage = nextPage;
				}
				else
				{
					WizardPage selNext = GetNextPage(SelectedPage);

					// Check for last page
					if (SelectedPage.IsFinishPage || selNext == null)
					{
						OnFinished();
						return;
					}

					// Set new SelectedPage value
					SelectedPage = selNext;
				}
			}
		}

		/// <summary>
		/// Returns to the previous page.
		/// </summary>
		public virtual void PreviousPage()
		{
			if (ControlExtensions.IsDesignMode(this))
			{
				int idx = SelectedPageIndex;
				if (idx > 0)
					SelectedPage = Pages[idx - 1];
				return;
			}

			if (SelectedPage.RollbackPage())
				SelectedPage = pageHistory.Pop();
		}

		/// <summary>
		/// Restarts the wizard pages from the first page.
		/// </summary>
		public void RestartPages()
		{
			if (selectedPage != null)
				selectedPage.Hide();
			selectedPage = null;
			pageHistory.Clear();
			initialized = false;
		}

		// Protected methods.

		/// <summary>
		/// Raises the <see cref="WizardControl.Cancelling"/> event.
		/// </summary>
		protected virtual void OnCancelling()
		{
			CancelEventHandler h = Cancelling;
			CancelEventArgs arg = new CancelEventArgs(true);
			if (h != null)
				h(this, arg);
		}

		/// <summary>
		/// Raises the <see cref="WizardControl.Finished"/> event.
		/// </summary>
		protected virtual void OnFinished()
		{
			EventHandler h = Finished;
			if (h != null)
				h(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (selectedPage != null)
				selectedPage.Focus();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			Initialize();
		}

		/// <summary>
		/// Raises the <see cref="WizardControl.SelectedPageChanged"/> event.
		/// </summary>
		protected void OnSelectedPageChanged()
		{
			EventHandler temp = SelectedPageChanged;
			if (temp != null)
				temp(this, EventArgs.Empty);
		}

		/// <summary>
		/// Updates the buttons according to current sequence and history.
		/// </summary>
		protected internal void OnUpdateButtons()
		{
			if (selectedPage == null)
			{
				this.CancelButtonState = ControlExtensions.IsDesignMode(this) ? WizardCommandButtonState.Disabled : WizardCommandButtonState.Enabled;
				this.NextButtonState = this.BackButtonState = WizardCommandButtonState.Hidden;
			}
			else
			{
				if (ControlExtensions.IsDesignMode(this))
				{
					this.CancelButtonState = WizardCommandButtonState.Disabled;
					this.NextButtonState = this.SelectedPageIndex == this.Pages.Count - 1 ? WizardCommandButtonState.Disabled : WizardCommandButtonState.Enabled;
					this.BackButtonState = this.SelectedPageIndex <= 0 ? WizardCommandButtonState.Disabled : WizardCommandButtonState.Enabled;
				}
				else
				{
					this.CancelButtonState = this.selectedPage.ShowCancel ? (this.selectedPage.AllowCancel && !ControlExtensions.IsDesignMode(this) ? WizardCommandButtonState.Enabled : WizardCommandButtonState.Disabled) : WizardCommandButtonState.Hidden;
					this.NextButtonState = this.selectedPage.ShowNext ? (this.selectedPage.AllowNext ? WizardCommandButtonState.Enabled : WizardCommandButtonState.Disabled) : WizardCommandButtonState.Hidden;
					if (this.selectedPage.IsFinishPage || this.IsLastPage(SelectedPage))
						SetButtonText(NextButton, FinishButtonText);
					else
						SetButtonText(NextButton, NextButtonText);
					BackButtonState = (pageHistory.Count == 0 || !selectedPage.AllowBack) ? WizardCommandButtonState.Disabled : WizardCommandButtonState.Enabled;
				}
			}
		}

		// Internal methods.

		internal void ResetBackButtonText()
		{
			BackButtonText = WizardResources.WizardBackText;
		}

		internal void ResetCancelButtonText()
		{
			CancelButtonText = WizardResources.WizardCancelText;
		}

		internal void ResetFinishButtonText()
		{
			FinishButtonText = WizardResources.WizardFinishText;
		}

		internal void ResetNextButtonText()
		{
			NextButtonText = WizardResources.WizardNextText;
		}

		internal bool ShouldSerializeBackButtonText()
		{
			return BackButtonText != WizardResources.WizardBackText;
		}

		internal bool ShouldSerializeCancelButtonText()
		{
			return CancelButtonText != WizardResources.WizardCancelText;
		}

		internal bool ShouldSerializeFinishButtonText()
		{
			return FinishButtonText != WizardResources.WizardFinishText;
		}

		internal bool ShouldSerializeNextButtonText()
		{
			return NextButtonText != WizardResources.WizardNextText;
		}

		// Private methods.

		private void OnBackButtonClick(object sender, EventArgs e)
		{
			PreviousPage();
		}

		private void OnCancelButtonClick(object sender, EventArgs e)
		{
			OnCancelling();
		}

		private WizardCommandButtonState GetCmdButtonState(ButtonBase btn)
		{
			if (btn == null || !btn.Visible)
				return WizardCommandButtonState.Hidden;
			else
			{
				if (btn.Enabled)
					return WizardCommandButtonState.Enabled;
				else
					return WizardCommandButtonState.Disabled;
			}
		}

		private string GetCmdButtonText(ButtonBase btn)
		{
			return btn == null ? string.Empty : btn.Text;
		}

		/// <summary>
		/// Returns the next page for the specified wizard page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns>The next page, or ,<c>null</c> if there is no next page.</returns>
		private WizardPage GetNextPage(WizardPage page)
		{
			if (page == null || page.IsFinishPage)
				return null;

			do
			{
				int pgIdx = Pages.IndexOf(page);
				if (page.NextPage != null)
					page = page.NextPage;
				else if (pgIdx == Pages.Count - 1)
					page = null;
				else
					page = Pages[pgIdx + 1];
			} while (page != null && page.Suppress);

			return page;
		}

		/// <summary>
		/// Initialize the control.
		/// </summary>
		private void Initialize()
		{
			//  If the control is not initialized.
			if (!initialized)
			{
				// Select the first page or update the buttons.
				if (this.Pages.Count > 0)
					this.SelectedPage = Pages[0];
				else
					this.OnUpdateButtons();
				// Set the control as initialized.
				initialized = true;
			}
		}

		/// <summary>
		/// Determines whether the specified page is the last page.
		/// </summary>
		/// <param name="page">The wizard page.</param>
		/// <returns><c>true</c> if the specified page is the last page, <c>false</c> otherwise.</returns>
		private bool IsLastPage(WizardPage page)
		{
			return this.GetNextPage(page) == null;
		}

		/// <summary>
		/// An event handler called when the user clicks on the <b>Next</b> button.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void NextButtonClick(object sender, EventArgs e)
		{
			this.NextPage();
		}

		/// <summary>
		/// An event handler called before the page collection has been cleared.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesCleared(object sender, EventArgs e)
		{
			this.SelectedPage = null;
			this.Controls.Clear();
		}

		/// <summary>
		/// An event handler called when a page is inserted.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesItemInserted(object sender, WizardPageCollection.ItemChangedEventArgs e)
		{
			this.OnPagesItemInserted(e.Item, !this.initializing);
		}

		/// <summary>
		/// An event handler called when a page is inserted.
		/// </summary>
		/// <param name="item">The wizard page.</param>
		/// <param name="selectAfterAdd"><c>true</c> if the page is selected.</param>
		private void OnPagesItemInserted(WizardPage item, bool selectAfterAdd)
		{
			item.Owner = this;
			item.Visible = false;
			if (!this.Contains(item))
				this.Controls.Add(item);
			if (selectAfterAdd)
				this.SelectedPage = item;
		}

		/// <summary>
		/// An event handler called when a page is removed.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesItemRemoved(object sender, WizardPageCollection.ItemChangedEventArgs e)
		{
			this.Controls.Remove(e.Item);
			if (e.Item == selectedPage && this.Pages.Count > 0)
				this.SelectedPage = this.Pages[e.Index == this.Pages.Count ? e.Index - 1 : e.Index];
			else
				this.OnUpdateButtons();
		}

		/// <summary>
		/// An event handler called when a page is set.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesItemSet(object sender, WizardPageCollection.ItemSetEventArgs e)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the state for the specified button.
		/// </summary>
		/// <param name="button">The button.</param>
		/// <param name="state">The button state.</param>
		private void SetButtonState(ButtonBase button, WizardCommandButtonState state)
		{
			if (button == null) return;

			switch (state)
			{
				case WizardCommandButtonState.Disabled:
					button.Enabled = false;
					button.Visible = true;
					break;
				case WizardCommandButtonState.Hidden:
					button.Enabled = false;
					if (button != this.BackButton)
						button.Visible = false;
					break;
				case WizardCommandButtonState.Enabled:
				default:
					button.Enabled = button.Visible = true;
					break;
			}

			// Raise the button changed event.
			if (this.ButtonStateChanged != null) this.ButtonStateChanged(this, EventArgs.Empty);

			// Invalidate the conrol.
			base.Invalidate();
		}

		/// <summary>
		/// Sets the text for the specified button.
		/// </summary>
		/// <param name="button">The button.</param>
		/// <param name="text">The text.</param>
		private void SetButtonText(ButtonBase button, string text)
		{
			if (button == null) return;
	
			// Set the button text.
			button.Text = text;
			// Invalidate the control.
			base.Invalidate();
		}
	}
}