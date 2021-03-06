﻿/*
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
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Mercury.Windows.Controls.AeroWizard.Design
{
	internal class WizardPageDesigner : ParentControlDesigner
	{
		private static string[] propsToRemove = new string[] { "Anchor", "AutoScrollOffset", "AutoSize", "BackColor",
			"BackgroundImage", "BackgroundImageLayout", "ContextMenuStrip", "Cursor", "Dock", "Enabled", "Font",
			"ForeColor", "Location", "Margin", "MaximumSize", "MinimumSize", "Padding", /*"Size",*/ "TabStop", "UseWaitCursor",
			"Visible" };

		private WizardPageDesignerActionList actionList;

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if (actionList == null)
					actionList = new WizardPageDesignerActionList(this);
				return new DesignerActionListCollection(new DesignerActionList[] { actionList });
			}
		}

		public override SelectionRules SelectionRules
		{
			get { return (SelectionRules.Visible | SelectionRules.Locked); }
		}

		protected override bool EnableDragRect
		{
			get { return false; }
		}

		public override bool CanBeParentedTo(IDesigner parentDesigner)
		{
			return ((parentDesigner != null) && (parentDesigner.Component is WizardPageContainer));
		}

		public override void Initialize(System.ComponentModel.IComponent component)
		{
			base.Initialize(component);
			DesignerActionService service = this.GetService(typeof(DesignerActionService)) as DesignerActionService;
			if (service != null)
				service.Remove(component);
		}

		internal void OnDragDropInternal(DragEventArgs de)
		{
			this.OnDragDrop(de);
		}

		internal void OnDragEnterInternal(DragEventArgs de)
		{
			this.OnDragEnter(de);
		}

		internal void OnDragLeaveInternal(EventArgs e)
		{
			this.OnDragLeave(e);
		}

		internal void OnDragOverInternal(DragEventArgs e)
		{
			this.OnDragOver(e);
		}

		internal void OnGiveFeedbackInternal(GiveFeedbackEventArgs e)
		{
			this.OnGiveFeedback(e);
		}

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			Rectangle clientRectangle = this.Control.ClientRectangle;
			clientRectangle.Width--;
			clientRectangle.Height--;
			ControlPaint.DrawFocusRectangle(pe.Graphics, clientRectangle);
			base.OnPaintAdornments(pe);
		}

		protected override void PreFilterProperties(System.Collections.IDictionary properties)
		{
			base.PreFilterProperties(properties);
			foreach (string p in propsToRemove)
				if (properties.Contains(p))
					properties.Remove(p);
		}

		internal class WizardPageDesignerActionList : DesignerActionList
		{
			private WizardPageDesigner wizardPageDesigner;

			public WizardPageDesignerActionList(WizardPageDesigner pageDesigner)
				: base(pageDesigner.Component)
			{
				wizardPageDesigner = pageDesigner;
				base.AutoShow = true;
			}

			public bool AllowBack
			{
				get { return this.Page.AllowBack; }
				set { SetProperty("AllowBack", value); }
			}

			public bool AllowCancel
			{
				get { return this.Page.AllowCancel; }
				set { SetProperty("AllowCancel", value); }
			}

			public bool AllowNext
			{
				get { return this.Page.AllowNext; }
				set { SetProperty("AllowNext", value); }
			}

			public bool IsFinishPage
			{
				get { return this.Page.IsFinishPage; }
				set { SetProperty("IsFinishPage", value); }
			}

			public WizardPage NextPage
			{
				get { return this.Page.NextPage; }
				set { SetProperty("NextPage", value); }
			}

			public bool ShowCancel
			{
				get { return this.Page.ShowCancel; }
				set { SetProperty("ShowCancel", value); }
			}

			public bool ShowNext
			{
				get { return this.Page.ShowNext; }
				set { SetProperty("ShowNext", value); }
			}

			private WizardPage Page
			{
				get { return this.Component as WizardPage; }
			}

			public override DesignerActionItemCollection GetSortedActionItems()
			{
				DesignerActionItemCollection col = new DesignerActionItemCollection();
				col.Add(new DesignerActionHeaderItem("Buttons"));
				col.Add(new DesignerActionPropertyItem("AllowBack", "Back Button Enabled", "Buttons", "Enables the Back button when this page is shown."));
				col.Add(new DesignerActionPropertyItem("AllowCancel", "Cancel Button Enabled", "Buttons", "Enables the Cancel button when this page is shown."));
				col.Add(new DesignerActionPropertyItem("ShowCancel", "Cancel Button Visible", "Buttons"));
				col.Add(new DesignerActionPropertyItem("AllowNext", "Next Button Enabled", "Buttons"));
				col.Add(new DesignerActionPropertyItem("ShowNext", "Next Button Visible", "Buttons"));
				col.Add(new DesignerActionHeaderItem("Behavior"));
				col.Add(new DesignerActionPropertyItem("IsFinishPage", "Mark As Finish Page", "Behavior"));
				col.Add(new DesignerActionPropertyItem("NextPage", "Set Next Page:", "Behavior"));
				return col;
			}

			private void SetProperty(string propertyName, object value)
			{
				PropertyDescriptor property = TypeDescriptor.GetProperties(this.Page)[propertyName];
				if (property != null)
					property.SetValue(this.Page, value);
			}

			private void TestMeth()
			{
				MessageBox.Show("Hello");
			}
		}
	}
}