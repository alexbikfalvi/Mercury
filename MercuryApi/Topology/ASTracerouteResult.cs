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
using System.Collections.Generic;
using InetApi.Net.Core;

namespace Mercury.Topology
{
	/// <summary>
	/// A class representing the result of an AS-level traceroute.
	/// </summary>
	public sealed class ASTracerouteResult
	{
        private readonly MultipathTracerouteResult traceroute;
        private readonly ASTracerouteCallback callback;

        private readonly ASTracerouteState state = new ASTracerouteState();

        /// <summary>
        /// Creates a new AS traceroute result.
        /// </summary>
        /// <param name="traceroute">The IP-level traceroute.</param>
        /// <param name="callback">The callback method.</param>
        public ASTracerouteResult(MultipathTracerouteResult traceroute, ASTracerouteCallback callback)
        {
            // Set the traceroute.
            this.traceroute = traceroute;

            // Set the callback.
            this.callback = callback;

            // Create the paths.
            this.PathsStep1 = new ASTraceroutePath[MultipathTracerouteResult.AlgorithmsCount, traceroute.Settings.FlowCount, traceroute.Settings.AttemptsPerFlow];
            this.PathsStep2 = new ASTraceroutePath[MultipathTracerouteResult.AlgorithmsCount, traceroute.Settings.FlowCount, traceroute.Settings.AttemptsPerFlow];
            this.PathsStep3 = new ASTraceroutePath[MultipathTracerouteResult.AlgorithmsCount, traceroute.Settings.FlowCount, traceroute.Settings.AttemptsPerFlow];
            this.PathsStep4 = new ASTraceroutePath[MultipathTracerouteResult.AlgorithmsCount, traceroute.Settings.FlowCount, traceroute.Settings.AttemptsPerFlow];
        }

        #region Public properties

        /// <summary>
        /// The step 1 AS paths.
        /// </summary>
        public ASTraceroutePath[, ,] PathsStep1 { get; private set; }
        /// <summary>
        /// The step 1 AS paths.
        /// </summary>
        public ASTraceroutePath[, ,] PathsStep2 { get; private set; }
        /// <summary>
        /// The step 1 AS paths.
        /// </summary>
        public ASTraceroutePath[, ,] PathsStep3 { get; private set; }
        /// <summary>
        /// The step 1 AS paths.
        /// </summary>
        public ASTraceroutePath[, ,] PathsStep4 { get; private set; }
        /// <summary>
        /// Gets the list of flows.
        /// </summary>
        public MultipathTracerouteFlow[] Flows { get { return this.traceroute.Flows; } }
        /// <summary>
        /// The flow count.
        /// </summary>
        public byte FlowCount { get { return this.traceroute.Settings.FlowCount; } }
        /// <summary>
        /// The attempt count.
        /// </summary>
        public byte AttemptCount { get { return this.traceroute.Settings.AttemptsPerFlow; } }
        /// <summary>
        /// The multipath algorithms.
        /// </summary>
        public IEnumerable<MultipathTracerouteResult.ResultAlgorithm> Algorithms { get { return this.traceroute.Algorithms; } }

        #endregion

        #region Internal methods

        /// <summary>
        /// Calls the callback method with the specified state and list of parameters.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="parameters">The parameters.</param>
        internal void Callback(ASTracerouteState.StateType state, params object[] parameters)
        {
            // Set the state.
            this.state.Type = state;
            this.state.Parameters = parameters;
            // Call the callback method.
            if (null != this.callback) this.callback(this, this.state);
        }

        #endregion
    }
}
