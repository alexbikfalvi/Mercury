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

namespace DotNetApi.Collections.Generic
{
	/// <summary>
	/// A class representing a non-generic collection.
	/// </summary>
	public class Collection : IList, ICollection, IEnumerable
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
			public ItemChangedEventArgs(int index, object item)
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
			public object Item { get; private set; }
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
			public ItemSetEventArgs(int index, object oldItem, object newItem)
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
			public object OldItem { get; private set; }
			/// <summary>
			/// Gets the new item.
			/// </summary>
			public object NewItem { get; private set; }
		}

		private readonly ArrayList list;
		private readonly object sync = new object();

		/// <summary>
		/// Creates a new collection with the default capacity.
		/// </summary>
		public Collection()
		{
			this.list = new ArrayList();
		}

		/// <summary>
		/// Creates a new collection with the specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		public Collection(int capacity)
		{
			this.list = new ArrayList(capacity);
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
		/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		int Count { get { return this.list.Count; } }
		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
		/// </summary>
		bool IsSynchronized { get { return false; } }
		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		object SyncRoot { get { return this.sync; } }
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
		/// </summary>
		bool IsFixedSize { get { return false; } }
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
		/// </summary>
		bool IsReadOnly { get { return false; } }
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public object this[int index]
		{
			get { return this.list[index]; }
			set { this.OnItemSet(index, value); }
		}

		// Public methods.

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns> An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. <see cref="T:The System.Array"/> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.IList"/>.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"/> to add to the <see cref="T:System.Collections.IList"/></param>
		/// <returns>The position into which the new element was inserted.</returns>
		public int Add(object value)
		{
			// Raise the event.
			if (null != this.BeforeItemInserted) this.BeforeItemInserted(this, new ItemChangedEventArgs())
			int index = this.list.Add(value);
		}

		//
		// Summary:
		//     Removes all items from the System.Collections.IList.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.IList is read-only.
		void Clear();
		//
		// Summary:
		//     Determines whether the System.Collections.IList contains a specific value.
		//
		// Parameters:
		//   value:
		//     The System.Object to locate in the System.Collections.IList.
		//
		// Returns:
		//     true if the System.Object is found in the System.Collections.IList; otherwise,
		//     false.
		bool Contains(object value);
		//
		// Summary:
		//     Determines the index of a specific item in the System.Collections.IList.
		//
		// Parameters:
		//   value:
		//     The System.Object to locate in the System.Collections.IList.
		//
		// Returns:
		//     The index of value if found in the list; otherwise, -1.
		int IndexOf(object value);
		//
		// Summary:
		//     Inserts an item to the System.Collections.IList at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index at which value should be inserted.
		//
		//   value:
		//     The System.Object to insert into the System.Collections.IList.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.IList.
		//
		//   System.NotSupportedException:
		//     The System.Collections.IList is read-only.  -or- The System.Collections.IList
		//     has a fixed size.
		//
		//   System.NullReferenceException:
		//     value is null reference in the System.Collections.IList.
		void Insert(int index, object value);
		//
		// Summary:
		//     Removes the first occurrence of a specific object from the System.Collections.IList.
		//
		// Parameters:
		//   value:
		//     The System.Object to remove from the System.Collections.IList.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.IList is read-only.  -or- The System.Collections.IList
		//     has a fixed size.
		void Remove(object value);
		//
		// Summary:
		//     Removes the System.Collections.IList item at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the item to remove.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.IList.
		//
		//   System.NotSupportedException:
		//     The System.Collections.IList is read-only.  -or- The System.Collections.IList
		//     has a fixed size.
		void RemoveAt(int index);


		// Protected methods.

		/// <summary>
		/// Sets the item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The value.</param>
		protected virtual void OnItemSet(int index, object value)
		{
			// Save the old value.
			object old = this.list[index];
			// Call the event handler.
			if (null != this.BeforeItemSet) this.BeforeItemSet(this, new ItemSetEventArgs(index, old, value));
			// Set the item.
			this.list[index] = value;
			// Call the event handler.
			if (null != this.AfterItemSet) this.AfterItemSet(this, new ItemSetEventArgs(index, old, value));
		}
	}
}
