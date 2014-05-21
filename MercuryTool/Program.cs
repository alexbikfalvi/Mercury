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
    public sealed class Program : IDisposable
    {
        private readonly object sync = new object();

        private string[] destinations = null;

        private int parallelCount = 8;
        private int processingCount = 0;

        private readonly Dictionary<IPAddress, ASTraceroutePath[]> cacheIps = new Dictionary<IPAddress, ASTraceroutePath[]>();

        private readonly ManualResetEvent wait = new ManualResetEvent(false);

        private readonly MultipathTracerouteSettings ipSettings = new MultipathTracerouteSettings();
        private readonly ASTracerouteSettings asSettings = new ASTracerouteSettings();

        private readonly ASTraceroute tracerouteAs;

        private readonly IPAddress sourceAddress;

        private readonly IPAddress publicAddress;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
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
            if (destinations.Length == 0)
            {
                // Get URLs using the current locale.
                using (WebClient client = new WebClient())
                {
                    this.destinations = client.DownloadString(
                        "http://inetanalytics.nets.upf.edu/getUrls?countryCode={0}".FormatWith(RegionInfo.CurrentRegion.TwoLetterISORegionName)).Split(
                        new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            lock (this.sync) 
            {
                // Run the number of parallel destinations on the thread pool.
                for (; (processingCount < parallelCount) && (processingCount < destinations.Length); processingCount++)
                {
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
        /// Executes the Mercury tool for the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void Run(string destination)
        {
            
            try
            {
                // Create the IP level traceroute.
                using (MultipathTraceroute ipTraceroute = new MultipathTraceroute(this.ipSettings))
                {
                    // TODO: Run the DNS client to get all IP addresses for the current destination.
                    IPAddress[] destinationAddresses = Dns.GetHostAddresses(destination);

                    foreach (IPAddress destinationAddress in destinationAddresses)
                    {

                        if (cacheIps.ContainsKey(destinationAddress)) //If destionation IP is contained in cache, we re-use it!
                        {
                            Program.WriteLine(ConsoleColor.Magenta, "Processing URL from CACHE: " + destination + " to ip: " + destinationAddress.ToString());
                            Run(this.cacheIps[destinationAddress], destination, sourceAddress, destinationAddress);
                        }
                        else //If destionation IP is not contained in cache we process it!
                        {
                            Program.WriteLine(ConsoleColor.Cyan, "Processing URL: " + destination + " to ip: " + destinationAddress.ToString());
                            try
                            {
                                this.Run(ipTraceroute, destination, sourceAddress, destinationAddress);
                            }
                            catch (Exception exception)
                            {
                                Program.WriteLine(ConsoleColor.Red, "Traceroute failed {0} ({1} --> {2}) / {3}", destination, sourceAddress, destinationAddress, exception.Message);
                            }
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Program.WriteLine(ConsoleColor.Red, "Traceroute failed {0} / {1}", destination, exception.Message);
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
                else
                {
                    // Signal the wait handle.
                    this.wait.Set();
                }
            }
        }

        /// <summary>
        /// Run method that processes the destination IP
        /// </summary>
        /// <param name="tracerouteIp"></param>
        /// <param name="destination"></param>
        /// <param name="sourceAddress"></param>
        /// <param name="destinationAddress"></param>
        public void Run(MultipathTraceroute tracerouteIp, string destination, IPAddress sourceAddress, IPAddress destinationAddress)
        {

            //System.Threading.Thread.Sleep(2000);

            // Run the IPv4 traceroute.
            MultipathTracerouteResult resultIp = tracerouteIp.RunIpv4(sourceAddress, destinationAddress, CancellationToken.None, null);

            // Run the AS-level traceroute.
            ASTracerouteResult resultAs = tracerouteAs.Run(resultIp, CancellationToken.None, null);
            // We add the results to the cache to reduce the number of processes
           cacheIps[destinationAddress] = resultAs.PathsStep4;
            
            // Upload the traceroute result to Mercury.
           if (resultAs.PathsStep4 != null)
           {
               uploadTraces(resultAs.PathsStep4, destination, destinationAddress);
               // Write the destination.
               Program.Write(ConsoleColor.White, "Success Traceroute to........");
           }
           else
           {
               // Write the destination.
               Program.Write(ConsoleColor.White, "NO Traceroute to........");
           } 
            Program.Write(ConsoleColor.Cyan, destination.PadLeft(50));
            Program.WriteLine(ConsoleColor.Yellow, "{0} --> {1}", sourceAddress, destinationAddress);
        }


        /// <summary>
        /// Run method that is executed because destination IP is gathered from CACHE,
        /// so we only need to upload it
        /// </summary>
        /// <param name="Paths"></param>
        /// <param name="destination"></param>
        /// <param name="sourceAddress"></param>
        /// <param name="destinationAddress"></param>
        public void Run(ASTraceroutePath[] Paths, string destination, IPAddress sourceAddress, IPAddress destinationAddress)
        {

            // Upload the traceroute result to Mercury.
            if (Paths != null)
            {
                uploadTraces(Paths, destination, destinationAddress);
                // Write the destination.
                Program.Write(ConsoleColor.White, "CACHE: Success Traceroute to........");
            }
            else
            {
                // Write the destination.
                Program.Write(ConsoleColor.White, "CACHE: NO Traceroute to........");
            }
            Program.Write(ConsoleColor.Cyan, destination.PadLeft(50));
            Program.WriteLine(ConsoleColor.Yellow, "{0} --> {1}", sourceAddress, destinationAddress);
        }

        public void Dispose()
        {
            this.wait.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private methods

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




        /// <summary>
        /// Processes all the AS Relationships in a path
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <returns>The AS path.</returns>
        private ASTraceroutePath obtainASRelationships(ASTraceroutePath path)
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
                /*THIS IS NEW!! GUESSING RELs with MISSING HOPs in the MIDDLE*/
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
        private MercuryAsTracerouteStats obtainTracerouteStatistics(ASTraceroutePath path)
        {
            int asHops = 0;
            int c2pRels = 0, p2pRels = 0, p2cRels = 0, s2sRels = 0, ixpRels = 0, nfRels = 0;
            bool completed = false;
            int flags = (int)path.Flags;

            //We set completed to true if Flags is...
            if (flags > 0x0) completed = true;

            //OLD We count the asHops - 1
            //OLD asHops = path.Hops.Count() - 1;
            //We count the asHops as relationships -1
            asHops = path.relationships.Count() - 1;

            //We count the asRelationships
            foreach (MercuryAsTracerouteRelationship rel in path.relationships)
            {
                if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.CustomerToProvider) { c2pRels++; }
                else if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.PeerToPeer) { p2pRels++; }
                else if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.ProviderToCustomer) { p2cRels++; }
                else if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.SiblingToSibling) { s2sRels++; }
                else if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.InternerExchangePoint) { ixpRels++; }
                else if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.NotFound) { nfRels++; }
            }

            if (flags < 0x2) completed = true;

            return new MercuryAsTracerouteStats(asHops, c2pRels, p2pRels, p2cRels, s2sRels, ixpRels, nfRels, completed, flags);
        }


        /// <summary>
        /// Obtains the geomapping for the source and destination IP Address of a path
        /// </summary>
        /// <param name="srcIp">The source IP address.</param>
        /// <param name="dstIp">The destination IP address.</param>
        /// <returns>The an array with 4 positions. [0] srcCity [1]srcCountry [2]dstCity [3] dstCountry</returns>
        private String[] getGeoMappings(IPAddress srcIp, IPAddress dstIp)
        {
            //Geo

            String srcCity = "", srcCountry = "", dstCity = "", dstCountry = "";
            List<MercuryIpToGeoMapping> ip2geoMappings = MercuryService.GetIp2GeoMappings(new IPAddress[]
                    {
                        srcIp,dstIp
                    });
            foreach (MercuryIpToGeoMapping geo in ip2geoMappings)
            {
                if (geo.Address.ToString() == srcIp.ToString())
                {
                    srcCity = geo.City;
                    srcCountry = geo.CountryName;
                }
                if (geo.Address.ToString() == dstIp.ToString())
                {
                    dstCity = geo.City;
                    dstCountry = geo.CountryName;
                }
            }

            return new string[] { srcCity, srcCountry, dstCity, dstCountry };
        }



        /// <summary>
        /// Generates the Traceroute AS object to be sent to the Mercury Platform
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <param name="dst">The URL destination (e.g. upf.edu).</param>
        /// <param name="publicIP">The public IP address.</param>
        /// <param name="srcIp">The source IP address of the host (usually a private IP address).</param>
        /// <param name="dstIp">The destination IP address of the URL destination.</param>
        /// <returns>The AS path.</returns>
        private MercuryAsTraceroute generateTracerouteAS(ASTraceroutePath path, String dst,
                                                    IPAddress publicIP, IPAddress srcIp, IPAddress dstIp)
        {

            //Let's play with hops!
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
            String[] geoMappings = getGeoMappings(publicIP, dstIp);
            String srcCity = geoMappings[0]; String srcCountry = geoMappings[1]; String dstCity = geoMappings[2]; String dstCountry = geoMappings[3];

            //We obtain the traceroute AS Relationship before processing Stats
            path = obtainASRelationships(path);
            //We obtain the traceroute Stats
            MercuryAsTracerouteStats tracerouteASStats = obtainTracerouteStatistics(path);

            //We create the tracerouteAS object
            MercuryAsTraceroute tracerouteAS = new MercuryAsTraceroute(srcAs, srcAsName, srcIp.ToString(), publicIP.ToString(), srcCity, srcCountry,
                dstAs, dstAsName, dstIp.ToString(), dst, dstCity, dstCountry, DateTime.UtcNow, tracerouteASStats);

            //We add the hops
            tracerouteAS.Hops = asHopsAux;
            //We add the relationships
            tracerouteAS.Relationships = path.relationships;

            return tracerouteAS;
        }

        private void uploadTraces(ASTraceroutePath[] Paths, String destination, IPAddress destinationAddress)
        {
    
            // Here we upload the MercuryAsTraceroute(es) to the Mercury Platform
       
            List<MercuryAsTraceroute> tracerouteASes = new List<MercuryAsTraceroute>();
            foreach (ASTraceroutePath path in Paths)
            {
                //We only process paths with at least 1 hop
                if(path.Hops.Count() > 0)
                    tracerouteASes.Add(generateTracerouteAS(path, destination, publicAddress, sourceAddress, destinationAddress));
            }
            String mercuryPlatformResponse = MercuryService.addTracerouteASes(tracerouteASes);
            
        }



        #endregion

    }
}
