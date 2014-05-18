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
using InetApi.Net.Core;

namespace Mercury.Topology
{
	/// <summary>
	/// A class representing the result of an AS-level traceroute.
	/// </summary>
	public sealed class ASTracerouteResult
	{
        private readonly MultipathTracerouteResult traceroute;

        /// <summary>
        /// Creates a new AS traceroute result.
        /// </summary>
        /// <param name="traceroute">The IP-level traceroute.</param>
        public ASTracerouteResult(MultipathTracerouteResult traceroute)
        {
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

        #endregion
    }
}
