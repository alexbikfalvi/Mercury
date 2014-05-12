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
using System.Threading;
using InetApi.Net.Core;
using Mercury.Api;
using Mercury.Services;

namespace Mercury.Topology
{
	/// <summary>
	/// A delegate used for AS traceroute callback methods.
	/// </summary>
	/// <param name="result">The AS traceroute result.</param>
	public delegate void ASTracerouteCallback(ASTracerouteResult result, ASTracerouteState state);

	/// <summary>
	/// A class for an autonomous system traceroute.
	/// </summary>
	public sealed class ASTraceroute
	{
		private readonly ASTracerouteSettings settings;
		private readonly ASTracerouteCache cache;

		/// <summary>
		/// Creates a new AS traceroute instance.
		/// </summary>
		/// <param name="settings">The traceroute settings.</param>
		public ASTraceroute(ASTracerouteSettings settings)
		{
			this.settings = settings;
		}

		/// <summary>
		/// Runs an AS-level traceroute starting from the specified IP-level multipath traceroute.
		/// </summary>
		/// <param name="traceroute">The IP-level multipath traceroute.</param>
		/// <param name="cancel">The cancellation token.</param>
		/// <param name="callback">The callback methods.</param>
		/// <returns>The AS-level traceroute.</returns>
		public ASTracerouteResult Run(MultipathTracerouteResult traceroute, CancellationToken cancel, ASTracerouteCallback callback)
		{
			//LocalInformation localInformation = MercuryService.GetLocalInformation();
            MercuryLocalInformation myInfo = MercuryService.GetLocalInformation();
            /*
            //An example for obtaining the AS rel betwenn AS2 and AS3
            AsRelationship asRel = MercuryService.GetASRelationship(2, 3);

            //An example for obtaining the AS rels in BULK mode
            string myParameters1 = "pairs=2-3,766-3356,2589-6985";
            List<AsRelationship> asRels = MercuryService.GetASRelationships(myParameters1);

            //An example for obtaining the IP2ASN mappings in BULK max 1000 ips
            string myParameters2 = "ips=193.145.48.3,8.8.8.85";
            List<List<Ip2AsnMapping>> ip2asMappings = MercuryService.GetIp2AsnMappings(myParameters2);

            //An example for obtaining the IP2GEO in BULK mode max 1000 ips
            string myParameters3 = "ips=193.145.48.3,8.8.8.85";
            List<Ip2GeoMapping> ip2geoMappings = MercuryService.GetIp2GeoMappings(myParameters3);

            //Alex you don't need this... but just in case...
            //An example for obtaining TracerouteASes by destination domain*/
            String dst = "yimg.com";
            List<MercuryAsTraceroute> tases = MercuryService.GetTracerouteASesByDst(dst);

            //An example for uploading a TracerouteAS
            //First we generate a dummy object. Then this must be obtained from the result of the algorithm
            MercuryAsTraceroute tas = MercuryService.generateTracerouteAS();
            String result1 = MercuryService.addTracerouteAS(tas);
            //An example for uploading many TracerouteASes in BULK mode
            //First we generate dummy objects. Then this must be obtained from the result of the processing algorithm
            List<MercuryAsTraceroute> tases2 = new List<MercuryAsTraceroute>();
            tases.Add(tas);
            tases.Add(tas);
            tases.Add(tas);
            String result2 = MercuryService.addTracerouteASes(tases2);

            //An example for uploading TracerouteSettings. 
            //It returns the same ID if is new, it returns the existing ID if it already exists
            MercuryTracerouteSettings tset = MercuryService.generateTracerouteSettings();
            Guid result3 = MercuryService.AddTracerouteSettings(tset);

            //An example for uploading a TracerouteIp. Then this must be obtained from the result of the alogrithm
            MercuryIpTraceroute tip = MercuryService.generateTracerouteIp();
            String result4 = MercuryService.addTracerouteIp(tip);


			return null;
		}
	}
}
