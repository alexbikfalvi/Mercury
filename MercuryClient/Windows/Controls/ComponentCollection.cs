/* 
 * Copyright (C) 2013 Alex Bikfalvi
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections;
using System.ComponentModel;

namespace DotNetApi.Windows.Controls
{
	/// <summary>
	/// A collection of components.
	/// </summary>
	public class ComponentCollection<T> : CollectionBase where T : Component
	{
		// Public delegates.

		/// <summary>
		/// A delegate representing a collection item changed event handler.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		public delegate void ItemChangedEventHandler(object sender, ItemChangedEventArgs e);
		/// <summary>
		/// A delegate representing a collection item set event handler.
		/// </summary>
		/// <param name="sender">The sender object.</param>
		/// <param name="e">The event arguments.</param>
		public delegate void ItemSetEventHandler(object sender, ItemSetEventArgs e);

		/// <summary>
		/// A class representing a collection item changed event arguments.
		/// </summary>
		public class ItemChangedEventArgs : EventArgs
		{
			/// <summary>
			/// Creates a new event arguments instance.
			/// </summary>
			/// <param name="index">The changed index.</param>
			/// <param name="item">The changed item.</param>
			public ItemChangedEventArgs(int index, T item)
			{
				this.Index = index;
				this.Item = item;
			}

			// Public properties.

			/// <summary>
			/// Gets the changed index.
			/// </summary>
			public int Index { get; private set; }
			/// <summary>
			/// Gets the changed item.
			/// </summary>
			public T Item { get; private set; }
		}

		/// <summary>
		/// A class representing a collection item set event arguments.
		/// </summary>
		public class ItemSetEventArgs : EventArgs
		{
			/// <summary>
			/// Creates a new event arguments instance.
			/// </summary>
			/// <param name="index">The set index.</param>
			/// <param name="oldItem">The old item.</param>
			/// <param name="newItem">The new item.</param>
			public ItemSetEventArgs(int index, T oldItem, T newItem)
			{
				this.Index = index;
				this.OldItem = oldItem;
				this.NewItem = newItem;
			}

			// Public properties.

			/// <summary>
			/// Gets the set index.
			/// </summary>
			public int Index { get; private set; }
			/// <summary>
			/// Gets the old item.
			/// </summary>
			public T OldItem { get; private set; }
			/// <summary>
			/// Gets the new item.
			/// </summary>
			public T NewItem { get; private set; }
		}

		// Public properties.

		/// <summary>
		/// Gets or sets the item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The item.</returns>
		public T this[int index]
		{
			get { return this.List[index] as T; }
			set { this.List[index] = value; }
		}

		// Public events.

		/// <summary>
		/// An event raised before the collection is cleared.
		/// </summary>
		public event EventHandler BeforeCleared;
		/// <summary>
		/// An event raised after the collection is cleared.
		/// </summary>
		public event EventHandler AfterCleared;
		/// <summary>
		/// An event raised before an item is inserted into the collection.
		/// </summary>
		public event ItemChangedEventHandler BeforeItemInserted;
		/// <summary>
		/// An event raised after an item is inserted into the collection.
		/// </summary>
		public event ItemChangedEventHandler AfterItemInserted;
		/// <summary>
		/// An event raised before an item is removed from the collection.
		/// </summary>
		public event ItemChangedEventHandler BeforeItemRemoved;
		/// <summary>
		/// An event raised after an item is removed from the collection.
		/// </summary>
		public event ItemChangedEventHandler AfterItemRemoved;
		/// <summary>
		/// An event raised before an item is set in the collection.
		/// </summary>
		public event ItemSetEventHandler BeforeItemSet;
		/// <summary>
		/// An event raised after an item is set in the collection.
		/// </summary>
		public event ItemSetEventHandler AfterItemSet;

		// Public methods.

		/// <summary>
		/// Adds an item to the collection.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The position into which the new item was inserted,
		/// or -1 to indicate that the item was not inserted into the collection.</returns>
		public int Add(T item)
		{
			// Add the item.
			int result = this.List.Add(item);
			// Return the result.
			return result;
		}

		/// <summary>
		/// Adds a range of items to the collection.
		/// </summary>
		/// <param name="items">The range of items.</param>
		public void AddRange(T[] items)
		{
			// Add the items.
			foreach (T item in items)
			{
				this.Add(item);
			}
		}

		/// <summary>
		/// Determines the index of the specific item in the collection.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The index of value if found in the list; otherwise, -1.</returns>
		public int IndexOf(T item)
		{
			return this.List.IndexOf(item);
		}

		/// <summary>
		/// Inserts the item in the collection at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item</param>
		public void Insert(int index, T item)
		{
			// Insert the item.
			this.List.Insert(index, item);
		}

		/// <summary>
		/// Removes the item from the collection.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Remove(T item)
		{
			// Remove the item.
			this.List.Remove(item);
		}

		/// <summary>
		/// Verifies if the specified item is found in the collection.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns><b>True</b> if the element is found in the collection, or <b>false</b> otherwise.</returns>
		public bool Contains(T item)
		{
			return this.List.Contains(item);
		}

		// Protected methods.

		/// <summary>
		/// Validates the specified value as an item for this collection.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		protected override void OnValidate(Object value)
		{
			if (!(value is T)) throw new ArgumentException("The value object is not a valid item.", "value");
		}


		/// <summary>
		/// An event handler called before clearing the item collection.
		/// </summary>
		protected override void OnClear()
		{
			// Call the base class method.
			base.OnClear();
			// Raise the event.
			if (this.BeforeCleared != null) this.BeforeCleared(this, EventArgs.Empty);
		}

		/// <summary>
		/// An event handler called after clearing the item collection.
		/// </summary>
		protected override void OnClearComplete()
		{
			// Call the base class method.
			base.OnClearComplete();
			// Raise the event.
			if (this.AfterCleared != null) this.AfterCleared(this, EventArgs.Empty);
		}

		/// <summary>
		/// An event handler called before inserting an item into the collection.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The item.</param>
		protected override void OnInsert(int index, object value)
		{
			// Call the base class method.
			base.OnInsert(index, value);
			// Raise the event.
			if (this.BeforeItemInserted != null) this.BeforeItemInserted(this, new ItemChangedEventArgs(index, value as T));
		}

		/// <summary>
		/// An event handler called after inserting an item into the collection.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The item.</param>
		protected override void OnInsertComplete(int index, object value)
		{
			// Call the base class method.
			base.OnInsertComplete(index, value);
			// Raise the event.
			if (this.AfterItemInserted != null) this.AfterItemInserted(this, new ItemChangedEventArgs(index, value as T));
		}

		/// <summary>
		/// An event handler called before removing an item from the collection.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The item.</param>
		protected override void OnRemove(int index, object value)
		{
			// Call the base class method.
			base.OnRemove(index, value);
			// Raise the event.
			if (this.BeforeItemRemoved != null) this.BeforeItemRemoved(this, new ItemChangedEventArgs(index, value as T));
		}

		/// <summary>
		/// An event handler called after removing an item from the collection.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The item.</param>
		protected override void OnRemoveComplete(int index, object value)
		{
			// Call the base class method.
			base.OnRemoveComplete(index, value);
			// Raise the event.
			if (this.AfterItemRemoved != null) this.AfterItemRemoved(this, new ItemChangedEventArgs(index, value as T));
		}

		/// <summary>
		/// An event handler called before setting the value of an item from the collection.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="oldValue">The old item.</param>
		/// <param name="newValue">The new item.</param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			// Call the base class method.
			base.OnSet(index, oldValue, newValue);
			// Raise the event.
			if (this.BeforeItemSet != null) this.BeforeItemSet(this, new ItemSetEventArgs(index, oldValue as T, newValue as T));
		}

		/// <summary>
		/// An event handler called after setting the value of an item from the collection.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="oldValue">The old item.</param>
		/// <param name="newValue">The new item.</param>
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			// Call the base class method.
			base.OnSetComplete(index, oldValue, newValue);
			// Raise the event.
			if (this.AfterItemSet != null) this.AfterItemSet(this, new ItemSetEventArgs(index, oldValue as T, newValue as T));
		}
	}
}
