/* 
 * Copyright (C) 2014 Alex Bikfalvi
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

namespace Mercury.Collections.Generic
{
	/// <summary>
	/// A generic class representing a hash set.
	/// </summary>
	/// <typeparam name="T">The elements type.</typeparam>
	public class HashSet<T> : IEnumerable<T>, ICollection<T>
	{
		private readonly Dictionary<T, T> items;

		/// <summary>
		/// Creates an empty hash set instance.
		/// </summary>
		public HashSet()
		{
			this.items = new Dictionary<T, T>();
		}

		/// <summary>
		/// Creates a hash set instance of the specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		public HashSet(int capacity)
		{
			this.items = new Dictionary<T, T>(capacity);
		}

		// Protected properties.

		/// <summary>
		/// Gets the number of elements in the hash-set.
		/// </summary>
		public int Count { get { return this.items.Count; } }
		/// <summary>
		/// Gets whether the hash-set is read-only.
		/// </summary>
		public bool IsReadOnly { get { return false; } }

		// Public methods.

		/// <summary>
		/// Returns the enumerator for the elements in the hash set.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return this.items.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns the enumerator for the elements in the hash set.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Adds an item to the hash-set. If the item already exists, the method throws an exception.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(T item)
		{
			this.items.Add(item, item);
		}

		/// <summary>
		/// Clears the hash-set.
		/// </summary>
		public void Clear()
		{
			this.items.Clear();
		}

		/// <summary>
		/// Determines whether the hash-set contains the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns><b>True</b> if the hash-set contains the item, <b>false</b> otherwise.</returns>
		public bool Contains(T item)
		{
			return this.items.ContainsKey(item);
		}

		/// <summary>
		/// Copies the items from the current hash-set to the specified array.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int index)
		{
			this.items.Keys.CopyTo(array, index);
		}

		/// <summary>
		/// Removes the specified item from the hash-set.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns><b>True</b> if the item was removed successfullu, <b>false</b> otherwise.</returns>
		public bool Remove(T item)
		{
			return this.items.Remove(item);
		}
	}
}
