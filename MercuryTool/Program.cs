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
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using DotNetApi;
using InetApi.Net.Core;
using Mercury.Api;
using Mercury.Topology;
using Mercury.Services;

namespace MercuryTool
{
    /// <summary>
    /// The Mercury tool program class.
    /// </summary>
    public sealed class Program : IDisposable
    {
        private readonly object sync = new object();

        private string[] destinations = null;

        private int parallelCount = 8;
        private int processingCount = 0;
        private int completedCount = 0;
        //private int waitTimeBeforeInitialThreads = 2000; //in millis

        private readonly Dictionary<IPAddress, ASTraceroutePath[]> cache = new Dictionary<IPAddress, ASTraceroutePath[]>();

        private readonly ManualResetEvent wait = new ManualResetEvent(false);

        private readonly MultipathTracerouteSettings ipSettings = new MultipathTracerouteSettings();
        private readonly ASTracerouteSettings asSettings = new ASTracerouteSettings();

        private readonly PacketCapture packetCapture;

        private readonly ASTraceroute tracerouteAs;

        private readonly IPAddress sourceAddress;

        private readonly IPAddress publicAddress;

        /// <summary>
        /// Creates a new program instance.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        public Program(string[] args)
        {
            //we can collect URLs from args[0] comma-separated URLs "facebook.com,google.com,twitter.com"
            if (args.Length > 0)
            {
                Console.WriteLine("URLs to process: " + args[0]);
                this.destinations = args[0].Split(new char[] { ',' });
            }
                
            // Set the source address.
            this.sourceAddress = Dns.GetHostAddresses(Dns.GetHostName()).First(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            // Create the packet capture
            this.packetCapture = new PacketCapture(this.sourceAddress, CancellationToken.None);

            // Set the public address.
            MercuryLocalInformation localInformation = MercuryService.GetLocalInformation();
            this.publicAddress = localInformation.Address;

            // Create the AS traceroute.
            this.tracerouteAs = new ASTraceroute(this.asSettings);
        }

        #region Static methods

        static void Main(string[] args)
        {
            try
            {
                // Create the program.
                using (Program program = new Program(args))
                {
                    // Run the program.
                    program.Run();
                    //program.RunSeq();
                }
            }
            catch (Exception exception)
            {
                // Show the error.
                Program.WriteLine(ConsoleColor.Red, "ERROR  {0}", exception.Message);
            }

            // Reset the console.
            Console.ResetColor();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Executes the Mercury tool.
        /// </summary>
        public void Run()
        {
            //If we do not have parameters, we load destinations from Server
            if (destinations == null)
            {
                // Get URLs using the current locale.
                using (WebClient client = new WebClient())
                {
                    this.destinations = client.DownloadString(
                        "http://inetanalytics.nets.upf.edu/getUrls?countryCode=ES").Split(
                        //"http://inetanalytics.nets.upf.edu/getUrls?countryCode={0}".FormatWith(RegionInfo.CurrentRegion.TwoLetterISORegionName)).Split(
                        new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            lock (this.sync) 
            {
                // Run the number of parallel destinations on the thread pool.
                for (; (processingCount < parallelCount) && (processingCount < destinations.Length); processingCount++)
                {
                    //Thread.Sleep(waitTimeBeforeInitialThreads);
                    ThreadPool.QueueUserWorkItem((object destination) => 
                    { 
                        this.Run(destination as string); 
                    }, this.destinations[processingCount]);
                }
            }
            // Wait for the threads to complete.
            this.wait.WaitOne();
        }
        
        /// <summary>
        /// Sequencial Run
        /// </summary>
        public void RunSeq()
        {
            //If we do not have parameters, we load destinations from Server
            if (destinations == null)
            {
                // Get URLs using the current locale.
                using (WebClient client = new WebClient())
                {
                    this.destinations = client.DownloadString(
                        "http://inetanalytics.nets.upf.edu/getUrls?countryCode=ES").Split(
                        //"http://inetanalytics.nets.upf.edu/getUrls?countryCode={0}".FormatWith(RegionInfo.CurrentRegion.TwoLetterISORegionName)).Split(
                        new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            foreach (String destination in destinations)
            {
                this.RunSeq(destination);
            }
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public void Dispose()
        {
            // Disposes the wait handle.
            this.wait.Dispose();
            // Suppress the finalizer.
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Executes the Mercury tool for the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void Run(string destination)
        {
            try
            {
                // Create the IP level traceroute.
                using (MultipathTraceroute ipTraceroute = new MultipathTraceroute(this.ipSettings, packetCapture))
                {
                    // TODO: Run the DNS client to get all IP addresses for the current destination.
                    IPAddress[] destinationAddresses = Dns.GetHostAddresses(destination);

                    // Request from Mercury the AS corresponding to the destination addresses.
                    this.tracerouteAs.Cache.Update(destinationAddresses);

                    HashSet<int> uniqueAses = new HashSet<int>();
                    List<IPAddress> uniqueAddresses = new List<IPAddress>();

                    // For each destination address.
                    foreach (IPAddress destinationAddress in destinationAddresses)
                    {
                        HashSet<ASInformation> ases = new HashSet<ASInformation>(this.tracerouteAs.Cache.GetCache(destinationAddress), new ASInformation.EqualityComparer());
                        if ((ases.Count == 0) || (ases.Count > 1))
                        {
                            uniqueAddresses.Add(destinationAddress);
                        }
                        else if (ases.First().AsNumber <= 0)
                        {
                            uniqueAddresses.Add(destinationAddress);
                        }
                        else if (!uniqueAses.Contains(ases.First().AsNumber))
                        {
                            uniqueAddresses.Add(destinationAddress);
                            uniqueAses.Add(ases.First().AsNumber);
                        }
                    }

                    lock (this.sync)
                    {
                        // Show the destinations for the current traceroute.
                        Program.WriteLine(ConsoleColor.Green, "START ", ConsoleColor.Cyan, destination);
                        Console.Write("\t");
                        foreach (IPAddress address in uniqueAddresses)
                        {
                            HashSet<ASInformation> ases = new HashSet<ASInformation>(this.tracerouteAs.Cache.GetCache(address), new ASInformation.EqualityComparer());
                            Program.Write(ConsoleColor.White, "{0} ", address);
                            Program.Write(ConsoleColor.Yellow, "[ ");
                            foreach (ASInformation asInfo in ases)
                            {
                                Program.Write(ConsoleColor.Yellow, "AS{0} ", asInfo.AsNumber);
                            }
                            Program.Write(ConsoleColor.Yellow, "] ");
                        }
                        Console.WriteLine();
                    }

                    // For all addresses with unique ASes.
                    foreach (IPAddress destinationAddress in uniqueAddresses)
                    {
                        ASTraceroutePath[] result;
                        bool foundInCache = false;
                        lock (this.sync)
                        {
                            foundInCache = this.cache.TryGetValue(destinationAddress, out result);
                        }
                        if (foundInCache)
                        {
                            this.Complete(this.cache[destinationAddress], destination, sourceAddress, destinationAddress, true);
                        }
                        else
                        {
                            try
                            {
                                this.Run(ipTraceroute, destination, sourceAddress, destinationAddress);
                            }
                            catch (Exception exception)
                            {
                                lock (this.sync)
                                {
                                    Program.WriteLine(ConsoleColor.Red, "ERROR", ConsoleColor.Gray, "{0} ({1}) {2}", destination, destinationAddress, exception.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Program.WriteLine(ConsoleColor.Red, "ERROR", ConsoleColor.Gray, "{0} {1}", destination, exception.Message);
            }

            lock (this.sync)
            {
                // If not all destinations are completed.
                if (this.processingCount < this.destinations.Length)
                {
                    ThreadPool.QueueUserWorkItem((object dst) =>
                    {
                        this.Run(dst as string);
                    }, this.destinations[processingCount]);

                    // Increment the processing count.
                    this.processingCount++;
                }
                this.completedCount++;

                if (this.completedCount == destinations.Length)
                {
                    // Signal the wait handle.
                    this.wait.Set();
                }
            }
        }

        /// <summary>
        /// Sequencial Run
        /// </summary>
        /// <param name="destination"></param>
        public void RunSeq(string destination)
        {
            try
            {
                // Create the IP level traceroute.
                using (MultipathTraceroute ipTraceroute = new MultipathTraceroute(this.ipSettings, packetCapture))
                {
                    // TODO: Run the DNS client to get all IP addresses for the current destination.
                    IPAddress[] destinationAddresses = Dns.GetHostAddresses(destination);

                    // Request from Mercury the AS corresponding to the destination addresses.
                    this.tracerouteAs.Cache.Update(destinationAddresses);

                    HashSet<int> uniqueAses = new HashSet<int>();
                    List<IPAddress> uniqueAddresses = new List<IPAddress>();

                    // For each destination address.
                    foreach (IPAddress destinationAddress in destinationAddresses)
                    {
                        HashSet<ASInformation> ases = new HashSet<ASInformation>(this.tracerouteAs.Cache.GetCache(destinationAddress), new ASInformation.EqualityComparer());
                        if ((ases.Count == 0) || (ases.Count > 1))
                        {
                            uniqueAddresses.Add(destinationAddress);
                        }
                        else if (ases.First().AsNumber <= 0)
                        {
                            uniqueAddresses.Add(destinationAddress);
                        }
                        else if (!uniqueAses.Contains(ases.First().AsNumber))
                        {
                            uniqueAddresses.Add(destinationAddress);
                            uniqueAses.Add(ases.First().AsNumber);
                        }
                    }

                    lock (this.sync)
                    {
                        // Show the destinations for the current traceroute.
                        Program.WriteLine(ConsoleColor.Green, "START ", ConsoleColor.Cyan, destination);
                        Console.Write("\t");
                        foreach (IPAddress address in uniqueAddresses)
                        {
                            HashSet<ASInformation> ases = new HashSet<ASInformation>(this.tracerouteAs.Cache.GetCache(address), new ASInformation.EqualityComparer());
                            Program.Write(ConsoleColor.White, "{0} ", address);
                            Program.Write(ConsoleColor.Yellow, "[ ");
                            foreach (ASInformation asInfo in ases)
                            {
                                Program.Write(ConsoleColor.Yellow, "AS{0} ", asInfo.AsNumber);
                            }
                            Program.Write(ConsoleColor.Yellow, "] ");
                        }
                        Console.WriteLine();
                    }

                    // For all addresses with unique ASes.
                    foreach (IPAddress destinationAddress in uniqueAddresses)
                    {
                        ASTraceroutePath[] result;
                        bool foundInCache = false;
                        lock (this.sync)
                        {
                            foundInCache = this.cache.TryGetValue(destinationAddress, out result);
                        }
                        if (foundInCache)
                        {
                            this.Complete(this.cache[destinationAddress], destination, sourceAddress, destinationAddress, true);
                        }
                        else
                        {
                            try
                            {
                                this.Run(ipTraceroute, destination, sourceAddress, destinationAddress);
                            }
                            catch (Exception exception)
                            {
                                lock (this.sync)
                                {
                                    Program.WriteLine(ConsoleColor.Red, "ERROR", ConsoleColor.Gray, "{0} ({1}) {2}", destination, destinationAddress, exception.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Program.WriteLine(ConsoleColor.Red, "ERROR", ConsoleColor.Gray, "{0} {1}", destination, exception.Message);
            }

        }


        /// <summary>
        /// Run method that processes the destination IP
        /// </summary>
        /// <param name="tracerouteIp">The IP-level traceroute.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceAddress">The source IP address.</param>
        /// <param name="destinationAddress">The destination IP address.</param>
        public void Run(MultipathTraceroute tracerouteIp, string destination, IPAddress sourceAddress, IPAddress destinationAddress)
        {
            // Run the IP-level traceroute.
            MultipathTracerouteResult resultIp = tracerouteIp.RunIpv4(sourceAddress, destinationAddress, CancellationToken.None, null);

            // Run the AS-level traceroute.
            ASTracerouteResult resultAs = tracerouteAs.Run(resultIp, CancellationToken.None, null);

            // We add the results to the cache to reduce the number of processes
            lock (this.sync)
            {
                cache[destinationAddress] = resultAs.PathsStep4;
            }

            // Complete the traceroute.
            this.Complete(resultAs.PathsStep4, destination, sourceAddress, destinationAddress, false);
        }


        /// <summary>
        /// Completes an AS-level traceroute.
        /// </summary>
        /// <param name="paths">The current path.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationAddress">The destination address.</param>
        /// <param name="cache">If true, the traceroute was retrieved from the cache.</param>
        public void Complete(ASTraceroutePath[] paths, string destination, IPAddress sourceAddress, IPAddress destinationAddress, bool cache)
        {
            // Write the traceroute information.s
            lock (this.sync)
            {
                // Show the destinations for the current traceroute.
                Program.Write(ConsoleColor.Red, "FINISH ", ConsoleColor.Gray, "{0} ({1} --> {2}) ", destination, sourceAddress, destinationAddress);
                Program.WriteLine(ConsoleColor.Yellow, cache ? "CACHE" : "LIVE");
                foreach (ASTraceroutePath path in paths)
                {
                    Console.Write("\t");
                    if (path.Flags == ASTracerouteFlags.None)
                    {
                        Program.Write(ConsoleColor.Gray, "Path: ", ConsoleColor.Green, "{0:X8} ", (uint)path.Flags);
                    }
                    else if (path.Flags.IsSuccessful())
                    {
                        Program.Write(ConsoleColor.Gray, "Path: ", ConsoleColor.Yellow, "{0:X8} ", (uint)path.Flags);
                    }
                    else
                    {
                        Program.Write(ConsoleColor.Gray, "Path: ", ConsoleColor.Red, "{0:X8} ", (uint)path.Flags);
                    }
                    foreach (ASTracerouteHop hop in path.Hops)
                    {
                        if (hop.AsNumber.HasValue)
                            Program.Write(ConsoleColor.White, "AS{0} ", hop.AsNumber.Value);
                        else
                            Program.Write(ConsoleColor.Magenta, "AS? ");
                    }
                    Console.WriteLine();
                }
            }

            // Upload the traceroute result to Mercury.
            if (paths != null)
            {
                // Upload the traceroute.
                this.Upload(paths, destination, destinationAddress);
            }
            else
            {
                // Write the destination.
                Program.Write(ConsoleColor.Red, "NO Traceroute upload (because no paths) for........"+destination);
            }
        }

        /// <summary>
        /// Processes all the AS relationships in an AS path.
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <returns>The AS path.</returns>
        private ASTraceroutePath ObtainASRelationships(ASTraceroutePath path)
        {
            for (byte i = 0; i < path.Hops.Count() - 1; i++) //We end before the last hop
            {
                if (path.Hops.ElementAt(i).AsSet.Count() != 0 && path.Hops.ElementAt(i + 1).AsSet.Count() != 0) //If NOT missing hops
                {
                    MercuryAsTracerouteRelationship rel = null;
                    int as0 = (int)path.Hops.ElementAt(i).AsNumber;
                    int as1 = (int)path.Hops.ElementAt(i + 1).AsNumber;

                    //If is IXP
                    if (path.Hops.ElementAt(i + 1).AsSet.First().Type == ASInformation.AsType.Ixp)
                    {
                        rel = new MercuryAsTracerouteRelationship(MercuryAsTracerouteRelationship.RelationshipType.InternerExchangePoint, as0, as1, i);
                    }
                    else
                    {
                        rel = MercuryService.GetAsRelationship(as0, as1);
                        rel.Hop = i;
                    }
                    path.relationships.Add(rel);
                }
                if (path.Hops.ElementAt(i).AsSet.Count() != 0 && path.Hops.ElementAt(i + 1).AsSet.Count() == 0) //Here we try to guess relationships with a missing hop in the middle
                {
                    if (i + 2 < path.Hops.Count()) //We check that we not overpass the limit
                    {
                        MercuryAsTracerouteRelationship rel = null;
                        int as0 = (int)path.Hops.ElementAt(i).AsNumber;
                        int as1 = (int)path.Hops.ElementAt(i + 2).AsNumber;

                        //If is IXP
                        if (path.Hops.ElementAt(i + 2).AsSet.First().Type == ASInformation.AsType.Ixp)
                        {
                            rel = new MercuryAsTracerouteRelationship(MercuryAsTracerouteRelationship.RelationshipType.InternerExchangePoint, as0, as1, i);
                        }
                        else
                        {
                            rel = MercuryService.GetAsRelationship(as0, as1);
                            rel.Hop = i;
                        }
                        path.relationships.Add(rel);
                        i++; //we increment index because we jump a missing hop
                    }
                }
            }
            return path;
        }


        /// <summary>
        /// Generates the statistics of a path
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <returns>The AS Traceroute Stats.</returns>
        private MercuryAsTracerouteStats ComputeTracerouteStatistics(ASTraceroutePath path)
        {
            int asHops = 0;
            int c2pRels = 0, p2pRels = 0, p2cRels = 0, s2sRels = 0, ixpRels = 0, nfRels = 0;
            bool completed = false;
            int flags = (int)path.Flags;

            asHops = path.relationships.Count();

            //We set completed to true if Flags is...
            if (path.Flags.IsSuccessful()) completed = true;

            // Count the AS relationships.
            foreach (MercuryAsTracerouteRelationship rel in path.relationships)
            {
                switch (rel.Relationship)
                {
                    case MercuryAsTracerouteRelationship.RelationshipType.CustomerToProvider: c2pRels++; break;
                    case MercuryAsTracerouteRelationship.RelationshipType.PeerToPeer: p2pRels++; break;
                    case MercuryAsTracerouteRelationship.RelationshipType.ProviderToCustomer: p2cRels++; break;
                    case MercuryAsTracerouteRelationship.RelationshipType.SiblingToSibling: s2sRels++; break;
                    case MercuryAsTracerouteRelationship.RelationshipType.InternerExchangePoint: ixpRels++; break;
                    case MercuryAsTracerouteRelationship.RelationshipType.NotFound: nfRels++; break;
                }
            }

            return new MercuryAsTracerouteStats(asHops, c2pRels, p2pRels, p2cRels, s2sRels, ixpRels, nfRels, completed, flags);
        }


        /// <summary>
        /// Obtains the geomapping for the source and destination IP Address of a path
        /// </summary>
        /// <param name="srcIp">The source IP address.</param>
        /// <param name="dstIp">The destination IP address.</param>
        /// <returns>The an array with 4 positions. [0] srcCity [1]srcCountry [2]dstCity [3] dstCountry</returns>
        private String[] GetGeoMappings(IPAddress srcIp, IPAddress dstIp)
        {
            List<MercuryIpToGeoMapping> mappings = MercuryService.GetIp2GeoMappings(new IPAddress[] { srcIp, dstIp });
            string srcCity = mappings.First(mapping => mapping.Address.Equals(srcIp)).City;
            string srcCountry = mappings.First(mapping => mapping.Address.Equals(srcIp)).CountryName;
            string dstCity = mappings.First(mapping => mapping.Address.Equals(dstIp)).City;
            string dstCountry = mappings.First(mapping => mapping.Address.Equals(dstIp)).CountryName;
            return new string[] { srcCity, srcCountry, dstCity, dstCountry };
        }

        /// <summary>
        /// Generates the Traceroute AS object to be sent to the Mercury platform.
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <param name="dst">The URL destination (e.g. upf.edu).</param>
        /// <param name="publicIP">The public IP address.</param>
        /// <param name="srcIp">The source IP address of the host (usually a private IP address).</param>
        /// <param name="dstIp">The destination IP address of the URL destination.</param>
        /// <returns>The AS path.</returns>
        private MercuryAsTraceroute GenerateAsTraceroute(ASTraceroutePath path, String dst,
            IPAddress publicIP, IPAddress srcIp, IPAddress dstIp)
        {
            List<MercuryAsTracerouteHop> asHopsAux = new List<MercuryAsTracerouteHop>();
            byte hopCount = 0;
            foreach (ASTracerouteHop asHop in path.Hops)
            {
                if (asHop.AsSet.Count > 0)
                {
                    foreach (ASInformation info in asHop.AsSet)
                    {
                        MercuryAsTracerouteHop hop = new MercuryAsTracerouteHop(hopCount, info);
                        asHopsAux.Add(hop);
                    }
                }
                else
                {
                    //Here we add missing hops
                    asHopsAux.Add(new MercuryAsTracerouteHop(hopCount));
                }
                hopCount++;
            }

            //Now we search the src and the dst hops
            int srcAs = -1, dstAs = -1;
            String srcAsName = null, dstAsName = null;
            if (path.Hops.Count() > 0)
            {
                if (path.Hops.First().AsSet.Count() > 0)
                {
                    srcAs = (int)path.Hops.First().AsNumber;
                    srcAsName = path.Hops.First().AsSet.First().AsName;
                }

                if (path.Hops.Last().AsSet.Count() > 0)
                {
                    dstAs = (int)path.Hops.Last().AsSet.First().AsNumber;
                    dstAsName = path.Hops.Last().AsSet.First().AsName;
                }
            }

            //Now we obtain the geomappings
            String[] geoMappings = GetGeoMappings(publicIP, dstIp);
            String srcCity = geoMappings[0]; String srcCountry = geoMappings[1]; String dstCity = geoMappings[2]; String dstCountry = geoMappings[3];

            //We obtain the traceroute AS Relationship before processing Stats
            if (path.relationships.Count() == 0) //if not cached relationships... we process them
            {
                path = this.ObtainASRelationships(path);
            }
            //We obtain the traceroute Stats
            MercuryAsTracerouteStats tracerouteASStats = ComputeTracerouteStatistics(path);

            //We create the tracerouteAS object
            MercuryAsTraceroute tracerouteAS = new MercuryAsTraceroute(srcAs, srcAsName, srcIp.ToString(), publicIP.ToString(), srcCity, srcCountry,
                dstAs, dstAsName, dstIp.ToString(), dst, dstCity, dstCountry, DateTime.UtcNow, tracerouteASStats);

            //We add the hops
            tracerouteAS.Hops = asHopsAux;
            //We add the relationships
            tracerouteAS.Relationships = path.relationships;

            return tracerouteAS;
        }

        /// <summary>
        /// Uploads the list of paths for a traceroute to the Mercury service.
        /// </summary>
        /// <param name="paths">The list of paths.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationAddress">The destination address.</param>
        private void Upload(ASTraceroutePath[] paths, String destination, IPAddress destinationAddress)
        {
            List<MercuryAsTraceroute> traceroutes = new List<MercuryAsTraceroute>(
                from path in paths where path.Hops.Count > 0 select this.GenerateAsTraceroute(path, destination, publicAddress, sourceAddress, destinationAddress)
                );
            if (traceroutes.Count > 0)
            {
                //MercuryService.UploadAsTraceroute(traceroutes);
            }
        }

        /// <summary>
        /// Writes a text to the console.
        /// </summary>
        /// <param name="color">The text foreground color.</param>
        /// <param name="format">The text format.</param>
        /// <param name="arguments">The text arguments.</param>
        private static void Write(ConsoleColor color, string format, params object[] arguments)
        {
            Console.ForegroundColor = color;
            Console.Write(format, arguments);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a line to the console.
        /// </summary>
        /// <param name="color">The text foreground color.</param>
        /// <param name="format">The text format.</param>
        /// <param name="arguments">The text arguments.</param>
        private static void WriteLine(ConsoleColor color, string format, params object[] arguments)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(format, arguments);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a text to the console.
        /// </summary>
        /// <param name="color1">The first foreground color.</param>
        /// <param name="text">The first text.</param>
        /// <param name="color2">The second foreground color.</param>
        /// <param name="format">The second text format.</param>
        /// <param name="arguments">The text arguments.</param>
        private static void Write(ConsoleColor color1, string text, ConsoleColor color2, string format, params object[] arguments)
        {
            Console.ForegroundColor = color1;
            Console.Write(text);
            Console.ForegroundColor = color2;
            Console.Write(format, arguments);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a line to the console.
        /// </summary>
        /// <param name="color1">The first foreground color.</param>
        /// <param name="text">The first text.</param>
        /// <param name="color2">The second foreground color.</param>
        /// <param name="format">The second text format.</param>
        /// <param name="arguments">The text arguments.</param>
        private static void WriteLine(ConsoleColor color1, string text, ConsoleColor color2, string format, params object[] arguments)
        {
            Console.ForegroundColor = color1;
            Console.Write(text);
            Console.ForegroundColor = color2;
            Console.WriteLine(format, arguments);
            Console.ResetColor();
        }
        #endregion
    }
}
