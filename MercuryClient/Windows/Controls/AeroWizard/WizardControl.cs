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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Mercury.Windows;
using Mercury.Windows.Native;
using Mercury.Windows.VisualStyles;

namespace Mercury.Windows.Controls.AeroWizard
{
	/// <summary>
	/// Styles that can be applied to the body of a <see cref="WizardControl"/> when on XP or earlier or when a Basic theme is applied.
	/// </summary>
	public enum WizardClassicStyle
	{
		/// <summary>Windows Vista style theme with large fonts and white background.</summary>
		AeroStyle,
		/// <summary>Windows XP style theme with control color background.</summary>
		BasicStyle,
		/// <summary>Use <see cref="BasicStyle"/> on Windows XP and <see cref="AeroStyle"/> for later versions.</summary>
		Automatic
	}

	/// <summary>
	/// Control providing an "Aero Wizard" style interface.
	/// </summary>
	[Designer(typeof(Design.WizardControlDesigner))]
	[ToolboxItem(true)]
	[Description("Creates an Aero Wizard interface.")]
	[DefaultProperty("Pages"), DefaultEvent("SelectedPageChanged")]
	public partial class WizardControl :
#if DEBUG
		UserControl
#else
		Control
#endif
		, ISupportInitialize
	{
		private IContainer components = null;

		internal WizardPageContainer pageContainer;
		private ThemedTableLayoutPanel titlePanel;
		private ThemedTableLayoutPanel headerPanel;
		private ThemedTableLayoutPanel commandPanel;
		private ThemedTableLayoutPanel contentPanel;
		internal Label headerLabel;
		private Button cancelButton;
		internal Button nextButton;
		internal ThemedImageButton backButton;
		private ThemedLabel titleLabel;
		private ThemedLabel titleImage;
		private Panel commandAreaBorder;
		private Panel bodyPanel;
		private ImageList titleImageList;

		private static bool isMin6;

		private WizardClassicStyle classicStyle = WizardClassicStyle.AeroStyle;
		private Point formMoveLastMousePos;
		private bool formMoveTracking;
		private Form parentForm;
		private Icon titleImageIcon;
		private bool titleImageIconSet = false;

		internal int contentCol = 1;

		static WizardControl()
		{
			isMin6 = System.Environment.OSVersion.Version.Major >= 6;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WizardControl"/> class.
		/// </summary>
		public WizardControl()
		{
			this.InitializeComponent();

			OnRightToLeftChanged(EventArgs.Empty);

			if (!Application.RenderWithVisualStyles)
			{
				this.titlePanel.BackColor = SystemColors.Control;
			}
			this.titlePanel.SetTheme(VisualStyleElementEx.AeroWizard.TitleBar.Active);
			this.headerPanel.SetTheme(VisualStyleElementEx.AeroWizard.HeaderArea.Normal);
			this.contentPanel.SetTheme(VisualStyleElementEx.AeroWizard.ContentArea.Normal);
			this.commandPanel.SetTheme(VisualStyleElementEx.AeroWizard.CommandArea.Normal);

			// Get localized defaults for button text
			this.ResetBackButtonToolTipText();
			this.ResetTitle();
			this.ResetTitleIcon();

			// Connect to page add and remove events to track property changes
			this.Pages.BeforeCleared += this.OnPagesCleared;
			this.Pages.AfterItemInserted += this.OnPagesItemInserted;
			this.Pages.AfterItemRemoved += this.OnPagesItemRemoved;
			this.Pages.AfterItemSet += this.OnPagesItemSet;
		}

		// Public properties.

		/// <summary>
		/// Occurs when the Cancel button has been clicked and the form is closing.
		/// </summary>
		/// <remarks>The <see cref="WizardControl.Cancelled"/> event is obsolete in version 1.2; use the <see cref="WizardControl.Cancelling"/> event instead.</remarks>
		[Obsolete("The Cancelled event is obsolete in version 1.2; use the Cancelling event instead.")]
		[Category("Behavior"), Description("Occurs when the Cancel button has been clicked and the form is closing.")]
		public event EventHandler Cancelled;

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
		/// Gets or sets the state of the back button.
		/// </summary>
		/// <value>The state of the back button.</value>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WizardCommandButtonState BackButtonState
		{
			get { return pageContainer.BackButtonState; }
		}

		/// <summary>
		/// Gets or sets the back button tool tip text.
		/// </summary>
		/// <value>The back button tool tip text.</value>
		[Category("Wizard"), Localizable(true), Description("The back button tool tip text")]
		public string BackButtonToolTipText
		{
			get { return backButton.ToolTipText; }
			set { backButton.ToolTipText = value; base.Invalidate(); }
		}

		/// <summary>
		/// Gets the state of the cancel button.
		/// </summary>
		/// <value>The state of the cancel button.</value>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WizardCommandButtonState CancelButtonState
		{
			get { return pageContainer.CancelButtonState; }
		}
		
		/// <summary>
		/// Gets or sets the cancel button text.
		/// </summary>
		/// <value>The cancel button text.</value>
		[Category("Wizard"), Localizable(true), Description("The cancel button text")]
		public string CancelButtonText
		{
			get { return this.pageContainer.CancelButtonText; }
			set { this.pageContainer.CancelButtonText = value; Refresh(); }
		}

		/// <summary>
		/// Gets or sets the style applied to the body of a <see cref="WizardControl"/> when on XP or earlier or when a Basic theme is applied.
		/// </summary>
		/// <value>A <see cref="WizardClassicStyle"/> value which determines the style.</value>
		[Category("Wizard"), DefaultValue(typeof(WizardClassicStyle), "AeroStyle"), Description("The style used in Windows Classic mode or on Windows XP")]
		public WizardClassicStyle ClassicStyle
		{
			get { return classicStyle; }
			set { classicStyle = value; ConfigureStyles(HasGlass() || UseAeroStyle); base.Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the finish button text.
		/// </summary>
		/// <value>The finish button text.</value>
		[Category("Wizard"), Localizable(true), Description("The finish button text")]
		public string FinishButtonText
		{
			get { return this.pageContainer.FinishButtonText; }
			set { this.pageContainer.FinishButtonText = value; Refresh(); }
		}

		/// <summary>
		/// Gets or sets the page header text.
		/// </summary>
		/// <value>The page header text.</value>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string HeaderText
		{
			get { return headerLabel.Text; }
			set { headerLabel.Text = value; Refresh(); }
		}

		/// <summary>
		/// Gets or sets the shield icon on the next button.
		/// </summary>
		/// <value><c>true</c> if Next button should display a shield; otherwise, <c>false</c>.</value>
		/// <exception cref="PlatformNotSupportedException">Setting a UAF shield on a button only works on Vista and later versions of Windows.</exception>
		[DefaultValue(false), Category("Wizard"), Description("Show a shield icon on the next button")]
		public Boolean NextButtonShieldEnabled
		{
			get { return this.pageContainer.NextButtonShieldEnabled; }
			set { this.pageContainer.NextButtonShieldEnabled = value; }
		}

		/// <summary>
		/// Gets the state of the next button.
		/// </summary>
		/// <value>The state of the next button.</value>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WizardCommandButtonState NextButtonState
		{
			get { return pageContainer.NextButtonState; }
		}

		/// <summary>
		/// Gets or sets the next button text.
		/// </summary>
		/// <value>The next button text.</value>
		[Category("Wizard"), Localizable(true), Description("The next button text.")]
		public string NextButtonText
		{
			get { return this.pageContainer.NextButtonText; }
			set { this.pageContainer.NextButtonText = value; Refresh(); }
		}

		/// <summary>
		/// Gets the collection of wizard pages in this wizard control.
		/// </summary>
		/// <value>The <see cref="WizardPageCollection"/> that contains the <see cref="WizardPage"/> objects in this <see cref="WizardControl"/>.</value>
		[Category("Wizard"), Description("Collection of wizard pages.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public WizardPageCollection Pages
		{
			get { return this.pageContainer.Pages; }
		}

		/// <summary>
		/// Gets the currently selected wizard page.
		/// </summary>
		/// <value>The selected wizard page. <c>null</c> if no page is active.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual WizardPage SelectedPage
		{
			get { return this.pageContainer.SelectedPage; }
			internal set { this.pageContainer.SelectedPage = value; if (value != null) this.HeaderText = value.Text; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to suppress changing the parent form's icon to match the wizard's <see cref="TitleIcon"/>.
		/// </summary>
		/// <value><c>true</c> to not change the parent form's icon to match this wizard's icon; otherwise, <c>false</c>.</value>
		[Category("Wizard"), DefaultValue(false), Description("Indicates whether to suppress changing the parent form's icon to match the wizard's")]
		public bool SuppressParentFormIconSync { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to spupress changing the parent form's caption to match the wizard's <see cref="Title"/>.
		/// </summary>
		/// <value><c>true</c> to not change the parent form's caption (Text) to match this wizard's title; otherwise, <c>false</c>.</value>
		[Category("Wizard"), DefaultValue(false), Description("Indicates whether to suppress changing the parent form's caption to match the wizard's")]
		public bool SuppressParentFormCaptionSync { get; set; }

		/// <summary>
		/// Gets or sets the title for the wizard.
		/// </summary>
		/// <value>The title text.</value>
		[Category("Wizard"), Localizable(true), Description("Title for the wizard")]
		public string Title
		{
			get { return titleLabel.Text; }
			set { titleLabel.Text = value; base.Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the optionally displayed icon next to the wizard title.
		/// </summary>
		/// <value>The title icon.</value>
		[Category("Wizard"), Localizable(true), Description("Icon next to the wizard title")]
		public Icon TitleIcon
		{
			get { return titleImageIcon; }
			set
			{
				titleImageIcon = value;
				titleImageList.Images.Clear();
				if (titleImageIcon != null)
				{
					// Resolve for different DPI settings and ensure that if icon is not a standard size, such as 20x20, 
					// that the larger one (24x24) is downsized and not the smaller upsized. (thanks demidov)
					titleImage.Size = titleImageList.ImageSize = SystemInformation.SmallIconSize;
					titleImageList.Images.Add(new Icon(value, SystemInformation.SmallIconSize + new Size(1, 1)));
					titleImage.ImageIndex = 0;
				}
				titleImageIconSet = true;
				base.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the current culture.
		/// </summary>
		[Category("Wizard"), Description("The wizard culture.")]
		public CultureInfo Culture
		{
			get { return WizardResources.Culture; }
			set
			{
				// Set the resources culture.
				WizardResources.Culture = value;
				// Update the wizard controls.
				this.titleLabel.Text = WizardResources.WizardTitle;
				this.pageContainer.BackButtonText = WizardResources.WizardBackText;
				this.cancelButton.Text = WizardResources.WizardCancelText;
				this.nextButton.Text = WizardResources.WizardNextText;
				this.backButton.Text = WizardResources.WizardBackText;
				this.backButton.ToolTipText = WizardResources.WizardBackButtonToolTip;
				this.pageContainer.NextButtonText = WizardResources.WizardNextText;
				this.pageContainer.BackButtonText = WizardResources.WizardBackText;
				this.pageContainer.FinishButtonText = WizardResources.WizardFinishText;
			}
		}

		// Internal properties.

		/// <summary>
		/// Gets the index of the selected page.
		/// </summary>
		internal int SelectedPageIndex
		{
			get { return this.pageContainer.SelectedPageIndex; }
		}

		// Private properties.

		/// <summary>
		/// Indicates whether the wizard uses the Aero style.
		/// </summary>
		private bool UseAeroStyle
		{
			get { return classicStyle == WizardClassicStyle.AeroStyle || (classicStyle == WizardClassicStyle.Automatic && DesktopWindowManager.CompositionSupported && Application.RenderWithVisualStyles); }
		}

		// Public methods.

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		public void BeginInit()
		{
		}

		/// <summary>
		/// Signals the object that initialization is complete.
		/// </summary>
		public void EndInit()
		{
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
			this.pageContainer.NextPage(nextPage, skipCommit);
		}

		/// <summary>
		/// Returns to the previous page.
		/// </summary>
		public virtual void PreviousPage()
		{
			this.pageContainer.PreviousPage();
		}

		/// <summary>
		/// Restarts the wizard pages from the first page.
		/// </summary>
		public void RestartPages()
		{
			this.pageContainer.RestartPages();
		}

		// Protected methods.

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Raises the <see cref="WizardControl.Cancelled"/> event.
		/// </summary>
		/// <remarks>The <see cref="WizardControl.OnCancelled"/> method is obsolete in version 1.2; use the <see cref="WizardControl.OnCancelling"/> method instead.</remarks>
		[Obsolete("The OnCancelled method is obsolete in version 1.2; use the OnCancelling method instead.")]
		protected virtual void OnCancelled()
		{
			EventHandler h = Cancelled;
			if (h != null)
				h(this, EventArgs.Empty);

			if (!ControlExtensions.IsDesignMode(this))
				CloseForm(DialogResult.Cancel);
		}

		/// <summary>
		/// Raises the <see cref="WizardControl.Cancelling"/> event.
		/// </summary>
		protected virtual void OnCancelling()
		{
			CancelEventHandler h = Cancelling;
			CancelEventArgs arg = new CancelEventArgs(true);
			if (h != null)
				h(this, arg);

			if (arg.Cancel)
#pragma warning disable 618
				OnCancelled();
#pragma warning restore 618
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.ControlAdded"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.ControlEventArgs"/> that contains the event data.</param>
		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			if (e.Control is WizardPage)
			{
				Controls.Remove(e.Control);
				Pages.Add(e.Control as WizardPage);
			}
		}

		/// <summary>
		/// Raises the <see cref="WizardControl.Finished"/> event.
		/// </summary>
		protected virtual void OnFinished()
		{
			EventHandler h = Finished;
			if (h != null)
				h(this, EventArgs.Empty);

			if (!ControlExtensions.IsDesignMode(this))
				CloseForm(DialogResult.OK);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			pageContainer.Focus();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		[EnvironmentPermission(SecurityAction.Demand)]
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			this.SetLayout();
			if (isMin6 && !ControlExtensions.IsDesignMode(this))
				DesktopWindowManager.CompositionChanged += DesktopWindowManagerCompositionChanged;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.HandleDestroyed"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (isMin6 && !ControlExtensions.IsDesignMode(this))
				DesktopWindowManager.CompositionChanged -= DesktopWindowManagerCompositionChanged;
			base.OnHandleDestroyed(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.ParentChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (parentForm != null)
				parentForm.Load -= OnParentFormLoad;
			parentForm = base.Parent as Form;
			this.Dock = DockStyle.Fill;
			if (parentForm != null)
				parentForm.Load += OnParentFormLoad;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.RightToLeftChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			bool r2l = (ControlExtensions.GetRightToLeftProperty(this) == System.Windows.Forms.RightToLeft.Yes);
			Bitmap btnStrip = WizardResources.AeroBackButtonStrip;
			if (r2l) btnStrip.RotateFlip(RotateFlipType.RotateNoneFlipX);
			backButton.SetImageListImageStrip(btnStrip, Orientation.Vertical);
			backButton.StylePart = r2l ? 2 : 1;
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

		// Private methods.

		/// <summary>
		/// Initializes the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
			this.titleImageList = new ImageList(this.components);
			this.commandAreaBorder = new Panel();
			this.bodyPanel = new Panel();
			this.contentPanel = new ThemedTableLayoutPanel();
			this.pageContainer = new WizardPageContainer();
			this.backButton = new ThemedImageButton();
			this.cancelButton = new Button();
			this.nextButton = new Button();
			this.headerPanel = new ThemedTableLayoutPanel();
			this.headerLabel = new Label();
			this.commandPanel = new ThemedTableLayoutPanel();
			this.titlePanel = new ThemedTableLayoutPanel();
			this.titleLabel = new ThemedLabel();
			this.titleImage = new ThemedLabel();
			this.bodyPanel.SuspendLayout();
			this.contentPanel.SuspendLayout();
			((ISupportInitialize)(this.pageContainer)).BeginInit();
			this.headerPanel.SuspendLayout();
			this.commandPanel.SuspendLayout();
			this.titlePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// titleImageList
			// 
			this.titleImageList.ColorDepth = ColorDepth.Depth32Bit;
			this.titleImageList.ImageSize = new Size(16, 16);
			this.titleImageList.TransparentColor = Color.Transparent;
			// 
			// commandAreaBorder
			// 
			this.commandAreaBorder.BackColor = SystemColors.ControlLight;
			this.commandAreaBorder.Dock = DockStyle.Bottom;
			this.commandAreaBorder.Location = new Point(0, 368);
			this.commandAreaBorder.Margin = new Padding(0);
			this.commandAreaBorder.Size = new Size(609, 1);
			this.commandAreaBorder.TabIndex = 2;
			// 
			// bodyPanel
			// 
			this.bodyPanel.Controls.Add(this.contentPanel);
			this.bodyPanel.Controls.Add(this.headerPanel);
			this.bodyPanel.Dock = DockStyle.Fill;
			this.bodyPanel.Location = new Point(0, 32);
			this.bodyPanel.Size = new Size(609, 336);
			this.bodyPanel.TabIndex = 1;
			// 
			// contentArea
			// 
			this.contentPanel.BackColor = SystemColors.Window;
			this.contentPanel.ColumnCount = 3;
			this.contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 38F));
			this.contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			this.contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 19F));
			this.contentPanel.Controls.Add(this.pageContainer, 1, 0);
			this.contentPanel.Dock = DockStyle.Fill;
			this.contentPanel.Location = new Point(0, 59);
			this.contentPanel.RowCount = 2;
			this.contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			this.contentPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
			this.contentPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			this.contentPanel.Size = new Size(609, 277);
			this.contentPanel.TabIndex = 1;
			this.contentPanel.Paint += new PaintEventHandler(this.contentArea_Paint);
			// 
			// pageContainer
			// 
			this.pageContainer.BackButton = this.backButton;
			this.pageContainer.BackButtonText = WizardResources.WizardBackText;
			this.pageContainer.CancelButton = this.cancelButton;
			this.pageContainer.Dock = DockStyle.Fill;
			this.pageContainer.Location = new Point(38, 0);
			this.pageContainer.Margin = new Padding(0);
			this.pageContainer.NextButton = this.nextButton;
			this.pageContainer.Size = new Size(552, 258);
			this.pageContainer.TabIndex = 0;
			this.pageContainer.Cancelling += new CancelEventHandler(this.OnPageContainerCancelling);
			this.pageContainer.Finished += new System.EventHandler(this.OnPageContainerFinished);
			this.pageContainer.SelectedPageChanged += new System.EventHandler(this.OnPageContainerSelectedPageChanged);
			// 
			// backButton
			// 
			this.backButton.Anchor = AnchorStyles.Left;
			this.backButton.Enabled = false;
			this.backButton.Image = null;
			this.backButton.Location = new Point(0, 0);
			this.backButton.Margin = new Padding(0, 0, 0, 2);
			this.backButton.Size = new Size(30, 30);
			this.backButton.StyleClass = "NAVIGATION";
			this.backButton.TabIndex = 0;
			this.backButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			this.cancelButton.AutoSize = true;
			this.cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.cancelButton.Location = new Point(510, 10);
			this.cancelButton.Margin = new Padding(7, 0, 0, 0);
			this.cancelButton.MinimumSize = new Size(80, 0);
			this.cancelButton.Size = new Size(80, 25);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = WizardResources.WizardCancelText;
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// nextButton
			// 
			this.nextButton.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			this.nextButton.AutoSize = true;
			this.nextButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.nextButton.Location = new Point(423, 10);
			this.nextButton.Margin = new Padding(0);
			this.nextButton.MinimumSize = new Size(80, 0);
			this.nextButton.Size = new Size(80, 23);
			this.nextButton.TabIndex = 0;
			this.nextButton.Text = WizardResources.WizardNextText;
			this.nextButton.UseVisualStyleBackColor = true;
			// 
			// header
			// 
			this.headerPanel.AutoSize = true;
			this.headerPanel.BackColor = SystemColors.Window;
			this.headerPanel.ColumnCount = 1;
			this.headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			this.headerPanel.Controls.Add(this.headerLabel, 0, 0);
			this.headerPanel.Dock = DockStyle.Top;
			this.headerPanel.Location = new Point(0, 0);
			this.headerPanel.RowCount = 1;
			this.headerPanel.RowStyles.Add(new RowStyle());
			this.headerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 59F));
			this.headerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 59F));
			this.headerPanel.Size = new Size(609, 59);
			this.headerPanel.TabIndex = 0;
			// 
			// headerLabel
			// 
			this.headerLabel.AutoSize = true;
			this.headerLabel.BackColor = Color.Transparent;
			this.headerLabel.Font = new Font(Window.DefaultFont.FontFamily, 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			this.headerLabel.ForeColor = Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(112)))), ((int)(((byte)(171)))));
			this.headerLabel.Location = new Point(38, 19);
			this.headerLabel.Margin = new Padding(38, 19, 0, 19);
			this.headerLabel.Size = new Size(77, 21);
			this.headerLabel.TabIndex = 0;
			this.headerLabel.Text = WizardResources.WizardHeader;
			// 
			// commandArea
			// 
			this.commandPanel.AutoSize = true;
			this.commandPanel.BackColor = SystemColors.Control;
			this.commandPanel.ColumnCount = 3;
			this.commandPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			this.commandPanel.ColumnStyles.Add(new ColumnStyle());
			this.commandPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 19F));
			this.commandPanel.Controls.Add(this.cancelButton, 1, 1);
			this.commandPanel.Controls.Add(this.nextButton, 0, 1);
			this.commandPanel.Dock = DockStyle.Bottom;
			this.commandPanel.Location = new Point(0, 369);
			this.commandPanel.RowCount = 3;
			this.commandPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
			this.commandPanel.RowStyles.Add(new RowStyle());
			this.commandPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
			this.commandPanel.Size = new Size(609, 45);
			this.commandPanel.TabIndex = 3;
			// 
			// titleBar
			// 
			this.titlePanel.AutoSize = true;
			this.titlePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.titlePanel.BackColor = SystemColors.ActiveCaption;
			this.titlePanel.ColumnCount = 3;
			this.titlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
			this.titlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
			this.titlePanel.ColumnStyles.Add(new ColumnStyle());
			this.titlePanel.Controls.Add(this.titleLabel, 2, 0);
			this.titlePanel.Controls.Add(this.titleImage, 1, 0);
			this.titlePanel.Controls.Add(this.backButton, 0, 0);
			this.titlePanel.Dock = DockStyle.Top;
			this.titlePanel.Location = new Point(0, 0);
			this.titlePanel.RowCount = 1;
			this.titlePanel.RowStyles.Add(new RowStyle());
			this.titlePanel.Size = new Size(609, 32);
			this.titlePanel.SupportGlass = true;
			this.titlePanel.TabIndex = 0;
			this.titlePanel.WatchFocus = true;
			this.titlePanel.MouseDown += new MouseEventHandler(this.TitleBarMouseDown);
			this.titlePanel.MouseMove += new MouseEventHandler(this.TitleBarMouseMove);
			this.titlePanel.MouseUp += new MouseEventHandler(this.TitleBarMouseUp);
			// 
			// title
			// 
			this.titleLabel.Anchor = AnchorStyles.Left;
			this.titleLabel.AutoSize = true;
			this.titleLabel.ForeColor = SystemColors.ActiveCaptionText;
			this.titleLabel.Location = new Point(55, 6);
			this.titleLabel.Margin = new Padding(0);
			this.titleLabel.Padding = new Padding(0, 2, 0, 2);
			this.titleLabel.Size = new Size(79, 19);
			this.titleLabel.TabIndex = 2;
			this.titleLabel.Text = WizardResources.WizardTitle;
			this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// titleImage
			// 
			this.titleImage.Anchor = AnchorStyles.Left;
			this.titleImage.AutoSize = true;
			this.titleImage.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.titleImage.ImageList = this.titleImageList;
			this.titleImage.Location = new Point(32, 8);
			this.titleImage.Margin = new Padding(0, 0, 7, 0);
			this.titleImage.MinimumSize = new Size(16, 16);
			this.titleImage.Size = new Size(16, 16);
			this.titleImage.TabIndex = 1;
			// 
			// WizardControl
			// 
			this.Controls.Add(this.bodyPanel);
			this.Controls.Add(this.commandAreaBorder);
			this.Controls.Add(this.commandPanel);
			this.Controls.Add(this.titlePanel);
			this.Font = new Font(Window.DefaultFont.FontFamily, 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			this.Size = new Size(609, 414);
			this.bodyPanel.ResumeLayout(false);
			this.bodyPanel.PerformLayout();
			this.contentPanel.ResumeLayout(false);
			((ISupportInitialize)(this.pageContainer)).EndInit();
			this.headerPanel.ResumeLayout(false);
			this.headerPanel.PerformLayout();
			this.commandPanel.ResumeLayout(false);
			this.commandPanel.PerformLayout();
			this.titlePanel.ResumeLayout(false);
			this.titlePanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void CloseForm(DialogResult dlgResult)
		{
			Form form = base.FindForm();
			if (form != null && form.Modal)
				form.DialogResult = dlgResult;
		}

		private void ConfigureStyles(bool aero = true)
		{
			if (aero)
			{
				bodyPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
				headerPanel.BackColor = contentPanel.BackColor = SystemColors.Window;
				headerLabel.Font = new System.Drawing.Font(Window.DefaultFont.FontFamily, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(112)))), ((int)(((byte)(171)))));
				titleLabel.Font = this.Font;
			}
			else
			{
				bodyPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				headerPanel.BackColor = contentPanel.BackColor = SystemColors.Control;
				headerLabel.Font = new System.Drawing.Font(Window.DefaultFont.FontFamily, 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				headerLabel.ForeColor = SystemColors.ControlText;
				titleLabel.Font = new Font(this.Font, FontStyle.Bold);
			}
		}

		private void ConfigureWindowFrame()
		{
			if (HasGlass())
			{
				titlePanel.BackColor = Color.Black;
				ConfigureStyles();
				try
				{
					if (parentForm != null)
						DesktopWindowManager.ExtendFrameIntoClientArea(parentForm, new Padding(0) { Top = titlePanel.Height });
				}
				catch { titlePanel.BackColor = commandPanel.BackColor; }
			}
			else
			{
				titlePanel.BackColor = commandPanel.BackColor;
				ConfigureStyles(UseAeroStyle);
			}

			if (parentForm != null)
			{
				if (!this.SuppressParentFormCaptionSync)
					parentForm.Text = this.Title;
				if (!this.SuppressParentFormIconSync && this.titleImageIcon != null)
				{
					parentForm.Icon = this.TitleIcon;
					parentForm.ShowIcon = true;
				}
				VisualStyleRendererExtension.SetWindowThemeAttribute(parentForm, NativeMethods.WindowThemeNonClientAttributes.NoDrawCaption | NativeMethods.WindowThemeNonClientAttributes.NoDrawIcon | NativeMethods.WindowThemeNonClientAttributes.NoSysMenu);
				parentForm.Invalidate();
			}
		}

		private void contentArea_Paint(object sender, PaintEventArgs pe)
		{
			if (ControlExtensions.IsDesignMode(this) && this.Pages.Count == 0)
			{
				string noPagesText = WizardResources.WizardNoPagesNotice;
				Rectangle r = this.GetContentAreaRectangle(false);

				r.Inflate(-2, -2);
				//pe.Graphics.DrawRectangle(SystemPens.GrayText, r);
				ControlPaint.DrawFocusRectangle(pe.Graphics, r);

				SizeF textSize = pe.Graphics.MeasureString(noPagesText, this.Font);
				r.Inflate((r.Width - (int)textSize.Width) / -2, (r.Height - (int)textSize.Height) / -2);
				pe.Graphics.DrawString(noPagesText, this.Font, SystemBrushes.GrayText, r);
			}
		}

		private void DesktopWindowManagerCompositionChanged(object sender, EventArgs e)
		{
			ConfigureWindowFrame();
		}

		/// <summary>
		/// Gets the content area rectangle.
		/// </summary>
		/// <param name="parentRelative">if set to <c>true</c> rectangle is relative to parent.</param>
		/// <returns>Coordinates of content area.</returns>
		private Rectangle GetContentAreaRectangle(bool parentRelative)
		{
			int[] cw = contentPanel.GetColumnWidths();
			int[] ch = contentPanel.GetRowHeights();
			Rectangle r = new Rectangle(cw[contentCol - 1], 0, cw[contentCol], ch[0]);
			if (parentRelative)
				r.Offset(contentPanel.Location);
			return r;
		}

		private bool HasGlass()
		{
			return isMin6 && DesktopWindowManager.IsCompositionEnabled();
		}

		private void OnPageContainedButtonStateChanged(object sender, EventArgs e)
		{
			commandPanel.Visible = (cancelButton.Enabled || nextButton.Enabled || cancelButton.Visible || nextButton.Visible);
		}

		private void OnPageContainerCancelling(object sender, CancelEventArgs e)
		{
			OnCancelling();
		}

		private void OnPageContainerFinished(object sender, EventArgs e)
		{
			OnFinished();
		}

		private void OnPageContainerSelectedPageChanged(object sender, EventArgs e)
		{
			if (this.pageContainer.SelectedPage != null)
				this.HeaderText = this.pageContainer.SelectedPage.Text;
			OnSelectedPageChanged();
		}

		/// <summary>
		/// An event handler called before the collection of pages has been cleared.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesCleared(object sender, EventArgs e)
		{
			foreach (WizardPage page in this.Pages)
			{
				page.TextChanged -= this.OnPageTextChanged;
			}
		}

		/// <summary>
		/// An event handler called after a page was inserted in the collection.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesItemInserted(object sender, WizardPageCollection.ItemChangedEventArgs e)
		{
			e.Item.TextChanged += this.OnPageTextChanged;
		}

		/// <summary>
		/// An event handler called after a page was removed from the collection.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesItemRemoved(object sender, WizardPageCollection.ItemChangedEventArgs e)
		{
			e.Item.TextChanged -= this.OnPageTextChanged;
		}

		/// <summary>
		/// An event handler called when a page was set in the collection.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		private void OnPagesItemSet(object sender, WizardPageCollection.ItemSetEventArgs e)
		{
			if (null != e.OldItem) e.OldItem.TextChanged -= this.OnPageTextChanged;
			if (null != e.NewItem) e.NewItem.TextChanged += this.OnPageTextChanged;
		}

		/// <summary>
		/// An event handler called when the page text has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPageTextChanged(object sender, EventArgs e)
		{
			WizardPage page = sender as WizardPage;
			if (this.SelectedPage == page)
			{
				this.HeaderText = page.Text;
			}
		}

		private void OnParentFormLoad(object sender, EventArgs e)
		{
			this.ConfigureWindowFrame();
		}

		private void ResetBackButtonToolTipText()
		{
			this.BackButtonToolTipText = WizardResources.WizardBackButtonToolTip;
		}

		private void ResetBackButtonText()
		{
			this.pageContainer.ResetBackButtonText();
		}

		private void ResetCancelButtonText()
		{
			this.pageContainer.ResetCancelButtonText();
		}

		private void ResetFinishButtonText()
		{
			pageContainer.ResetFinishButtonText();
		}

		private void ResetNextButtonText()
		{
			pageContainer.ResetNextButtonText();
		}

		private void ResetTitle()
		{
			Title = WizardResources.WizardTitle;
		}

		private void ResetTitleIcon()
		{
			TitleIcon = WizardResources.Icon;
			titleImageIconSet = false;
		}

		[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		private void SetLayout()
		{
			if (isMin6 && Application.RenderWithVisualStyles)
			{
				VisualStyleRenderer theme;
				using (Graphics g = this.CreateGraphics())
				{
					// Back button
					theme = new VisualStyleRenderer(VisualStyleElementEx.Navigation.BackButton.Normal);
					Size bbSize = theme.GetPartSize(g, ThemeSizeType.Draw);

					// Title
					theme.SetParameters(VisualStyleElementEx.AeroWizard.TitleBar.Active);
					titlePanel.Height = Math.Max(VisualStyleRendererExtension.GetMargins2(theme, g, MarginProperty.ContentMargins).Top, bbSize.Height + 2);
					titlePanel.ColumnStyles[0].Width = bbSize.Width + 4F;
					titlePanel.ColumnStyles[1].Width = titleImageIcon != null ? titleImageList.ImageSize.Width + 4F : 0;
					backButton.Size = bbSize;

					// Header
					theme.SetParameters(VisualStyleElementEx.AeroWizard.HeaderArea.Normal);
					headerLabel.Margin = VisualStyleRendererExtension.GetMargins2(theme, g, MarginProperty.ContentMargins);
					headerLabel.ForeColor = theme.GetColor(ColorProperty.TextColor);

					// Content
					theme.SetParameters(VisualStyleElementEx.AeroWizard.ContentArea.Normal);
					this.BackColor = theme.GetColor(ColorProperty.FillColor);
					Padding cp = VisualStyleRendererExtension.GetMargins2(theme, g, MarginProperty.ContentMargins);
					contentPanel.ColumnStyles[0].Width = cp.Left;
					contentPanel.RowStyles[1].Height = cp.Bottom;

					// Command
					theme.SetParameters(VisualStyleElementEx.AeroWizard.CommandArea.Normal);
					cp = VisualStyleRendererExtension.GetMargins2(theme, g, MarginProperty.ContentMargins);
					commandPanel.RowStyles[0].Height = cp.Top;
					commandPanel.RowStyles[2].Height = cp.Bottom;
					commandPanel.ColumnStyles[2].Width = contentPanel.ColumnStyles[contentCol + 1].Width = cp.Right;
				}
			}
			else
			{
				backButton.Size = new Size(WizardResources.AeroBackButtonStrip.Width, WizardResources.AeroBackButtonStrip.Height / 4);
				this.BackColor = UseAeroStyle ? SystemColors.Window : SystemColors.Control;
			}
		}

		private bool ShouldSerializeBackButtonToolTipText()
		{
			return BackButtonToolTipText != WizardResources.WizardBackButtonToolTip;
		}

		private bool ShouldSerializeBackButtonText()
		{
			return pageContainer.ShouldSerializeBackButtonText();
		}

		private bool ShouldSerializeCancelButtonText()
		{
			return pageContainer.ShouldSerializeCancelButtonText();
		}

		private bool ShouldSerializeFinishButtonText()
		{
			return pageContainer.ShouldSerializeFinishButtonText();
		}

		private bool ShouldSerializeNextButtonText()
		{
			return pageContainer.ShouldSerializeNextButtonText();
		}

		private bool ShouldSerializeTitle()
		{
			return Title != WizardResources.WizardTitle;
		}

		private bool ShouldSerializeTitleIcon()
		{
			return titleImageIconSet;
		}

		private void TitleBarMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Control c = titlePanel.GetChildAtPoint(e.Location);
				if (c != backButton)
				{
					formMoveTracking = true;
					formMoveLastMousePos = this.PointToScreen(e.Location);
				}
			}

			base.OnMouseDown(e);
		}

		private void TitleBarMouseMove(object sender, MouseEventArgs e)
		{
			if (formMoveTracking)
			{
				Point screen = this.PointToScreen(e.Location);

				Point diff = new Point(screen.X - formMoveLastMousePos.X, screen.Y - formMoveLastMousePos.Y);

				Point loc = this.parentForm.Location;
				loc.Offset(diff);
				this.parentForm.Location = loc;

				formMoveLastMousePos = screen;
			}

			base.OnMouseMove(e);
		}

		private void TitleBarMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				formMoveTracking = false;

			base.OnMouseUp(e);
		}
	}
}