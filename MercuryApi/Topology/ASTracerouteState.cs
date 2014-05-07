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

namespace InetApi.Net.Core
{
	/// <summary>
	/// A structure representing the state of an AS traceroute.
	/// </summary>
	public sealed class ASTracerouteState
	{
		/// <summary>
		/// An enumeration representing the state type.
		/// </summary>
		public enum StateType
		{
			Unknown = -1
		}

		/// <summary>
		/// Internal constructor.
		/// </summary>
		internal ASTracerouteState()
		{
			this.Type = StateType.Unknown;
			this.Parameters = null;
		}

		/// <summary>
		/// Creates a new AS traceroute state.
		/// </summary>
		/// <param name="type">The state type.</param>
		/// <param name="parameters">The state parameyets.</param>
		public ASTracerouteState(StateType type, params object[] parameters)
		{
			this.Type = type;
			this.Parameters = parameters;
		}

		#region Public properties

		/// <summary>
		/// Gets the state type.
		/// </summary>
		public StateType Type { get; internal set; }
		/// <summary>
		/// Gets the list of parameters.
		/// </summary>
		public object[] Parameters { get; internal set; }

		#endregion
	}
}
