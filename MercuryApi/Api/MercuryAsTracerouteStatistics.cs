/* 
 * Copyright (C) 2014 Alex Bikfalvi, Manuel Palacin
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

namespace Mercury.Api
{
    /// <summary>
    /// A class for AS traceroute statistics.
    /// </summary>
    public class MercuryAsTracerouteStatistics
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private MercuryAsTracerouteStatistics() { }

        /// <summary>
        /// Creates a new AS traceroute statistics instance.
        /// </summary>
        /// <param name="asHops">The number of AS hops.</param>
        /// <param name="c2pRels">The number of customer-to-provider relationships.</param>
        /// <param name="p2pRels">The number of peer-to-peer relationships.</param>
        /// <param name="p2cRels">The number of provider-to-customer relationships.</param>
        /// <param name="s2sRels">The number of sibling-to-sibling relationships.</param>
        /// <param name="ixpRels">The number of IXP relationships.</param>
        /// <param name="nfRels">The number of relationships not found.</param>
        /// <param name="completed">Indicates whether the the traceroute is completed.</param>
        /// <param name="flags">The traceroute flags.</param>
        public MercuryAsTracerouteStatistics(
            int asHops,
            int c2pRels,
            int p2pRels,
            int p2cRels,
            int s2sRels,
            int ixpRels,
            int nfRels,
            bool completed,
            int flags)
        {
            this.AsHopCount = asHops;
            this.CustomerToProviderRelationships = c2pRels;
            this.PeerToPeerRelationships = p2pRels;
            this.ProviderToCustomerRelationships = p2cRels;
            this.SiblingToSiblingRelationships = s2sRels;
            this.IxpRelationships = ixpRels;
            this.NotFoundRelationships = nfRels;
            this.IsCompleted = completed;
            this.Flags = flags;
        }

        #region Public properties

        /// <summary>
        /// The number of AS hops.
        /// </summary>
        public int AsHopCount { get; private set; }
        /// <summary>
        /// The number of customer-to-provider relationships.
        /// </summary>
        public int CustomerToProviderRelationships { get; private set; }
        /// <summary>
        /// The number of peer-to-peer relationships.
        /// </summary>
        public int PeerToPeerRelationships { get; private set; }
        /// <summary>
        /// The number of provider-to-customer relationships.
        /// </summary>
        public int ProviderToCustomerRelationships { get; private set; }
        /// <summary>
        /// The number of sibling-to-sibling relationships.
        /// </summary>
        public int SiblingToSiblingRelationships { get; private set; }
        /// <summary>
        /// The number of IXP relationships.
        /// </summary>
        public int IxpRelationships { get; private set; }
        /// <summary>
        /// The number of not-found relationships.
        /// </summary>
        public int NotFoundRelationships { get; private set; }
        /// <summary>
        /// <b>True</b> if the traceroute is completed, <b>false</b> otherwise.
        /// </summary>
        public bool IsCompleted { get; private set; }
        /// <summary>
        /// The traceroute flags.
        /// </summary>
        public int Flags { get; private set; }

        #endregion
    }
}
