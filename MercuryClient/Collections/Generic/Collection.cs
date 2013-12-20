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
using System.Collections.Generic;

namespace DotNetApi.Collections.Generic
{
	/// <summary>
	/// A class representing a non-generic collection.
	/// </summary>
	public class Collection<T> : CollectionBase, IList<T>, ICollection<T>, IEnumerable<T>
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

		private readonly object sync = new object();

		/// <summary>
		/// Creates a new collection with the default capacity.
		/// </summary>
		public Collection()
			: base()
		{
		}

		/// <summary>
		/// Creates a new collection with the specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		public Collection(int capacity)
			: base(capacity)
		{
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

		// Public properties.

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
		/// </summary>
		public bool IsReadOnly { get { return false; } }
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public T this[int index]
		{
			get { return (T)this.List[index]; }
			set { this.List[index] = value; }
		}

		// Public methods.

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns> An <see cref="T:System.Collections.IEnumerator<T>"/> object that can be used to iterate through the collection.</returns>
		public new IEnumerator<T> GetEnumerator()
		{
			return this.List.GetEnumerator() as IEnumerator<T>;
		}

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection<T>"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection<T>"/>.</param>
		public void Add(T item)
		{
			this.List.Add(item);
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection<T>"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection<T>"/>.</param>
		/// <returns>True if item is found in the <see cref="T:System.Collections.Generic.ICollection<T>"/>; otherwise, false.</returns>
		public bool Contains(T item)
		{
			return this.List.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection<T>"/> to an System.Array, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection<T>"/>.
		/// </summary>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection<T>"/>.</param>
		/// <returns>true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection<T>"/>, otherwise, false.</returns>
		public bool Remove(T item)
		{
			if (this.List.Contains(item))
			{
				this.List.Remove(item);
				return true;
			}
			else return false;
		}

		/// <summary>
		/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList<T>"/>
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList<T>"/>.</param>
		/// <returns>The index of item if found in the list; otherwise, -1.</returns>
		public int IndexOf(T item)
		{
			return this.List.IndexOf(item);
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.Generic.IList<T>"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList<T>"/>.</param>
		public void Insert(int index, T item)
		{
			this.List.Insert(index, item);
		}

		// Protected methods.

		/// <summary>
		/// Performs additional custom processes when clearing the contents of the collection.
		/// </summary>
		protected override void OnClear()
		{
			// Raise the event.
			if (this.BeforeCleared != null) this.BeforeCleared(this, EventArgs.Empty);
			// Call the base class method.
			base.OnClear();
		}

		/// <summary>
		/// Performs additional custom processes after clearing the contents of the collection.
		/// </summary>
		protected override void OnClearComplete()
		{
			// Raise the event.
			if (this.AfterCleared != null) this.AfterCleared(this, EventArgs.Empty);
			// Call the base class method.
			base.OnClearComplete();
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, object value)
		{
			// Raise the event.
			if (this.BeforeItemInserted != null) this.BeforeItemInserted(this, new ItemChangedEventArgs(index, (T)value));
			// Call the base class method.
			base.OnInsert(index, value);
		}

		/// <summary>
		/// Performs additional custom processes after inserting a new element into the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsertComplete(int index, object value)
		{
			// Raise the event.
			if (this.AfterItemInserted != null) this.AfterItemInserted(this, new ItemChangedEventArgs(index, (T)value));
			// Call the base class method.
			base.OnInsertComplete(index, value);
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which value can be found.</param>
		/// <param name="value">The value of the element to remove from index.</param>
		protected override void OnRemove(int index, object value)
		{
			// Raise the event.
			if (this.BeforeItemRemoved != null) this.BeforeItemRemoved(this, new ItemChangedEventArgs(index, (T)value));
			// Call the base class method.
			base.OnRemove(index, value);
		}

		/// <summary>
		/// Performs additional custom processes after removing an element from the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which value can be found.</param>
		/// <param name="value">The value of the element to remove from index.</param>
		protected override void OnRemoveComplete(int index, object value)
		{
			// Raise the event.
			if (this.AfterItemRemoved != null) this.AfterItemRemoved(this, new ItemChangedEventArgs(index, (T)value));
			// Call the base class method.
			base.OnRemoveComplete(index, value);
		}

		/// <summary>
		/// Performs additional custom processes before setting a value in the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			// Raise the event.
			if (this.BeforeItemSet != null) this.BeforeItemSet(this, new ItemSetEventArgs(index, (T)oldValue, (T)newValue));
			// Call the base class method.
			base.OnSet(index, oldValue, newValue);
		}

		/// <summary>
		/// Performs additional custom processes after setting a value in the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			// Raise the event.
			if (this.AfterItemSet != null) this.AfterItemSet(this, new ItemSetEventArgs(index, (T)oldValue, (T)newValue));
			// Call the base class method.
			base.OnSetComplete(index, oldValue, newValue);
		}
	}
}
