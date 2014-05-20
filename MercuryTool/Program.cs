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

namespace MercuryTool
{
    public sealed class Program : IDisposable
    {
        private string[] destinations = null;

        private int workerThreadCount = 20;
        private const int completionThreadCount = 1024;

        private int resultsCount = 0;
        private readonly ManualResetEvent wait = new ManualResetEvent(false);

        private readonly MultipathTracerouteSettings ipSettings = new MultipathTracerouteSettings();
        private readonly ASTracerouteSettings asSettings = new ASTracerouteSettings();

        private readonly ASTraceroute tracerouteAs;

        private readonly IPAddress sourceAddress;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
        public Program(string[] args)
        {
            // Set the source address.
            this.sourceAddress = Dns.GetHostAddresses(Dns.GetHostName()).First(ip => ip.AddressFamily == AddressFamily.InterNetwork);

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
            // Get the current locale.
            using (WebClient client = new WebClient())
            {
                this.destinations = client.DownloadString(
                    "http://inetanalytics.nets.upf.edu/getUrls?countryCode={0}".FormatWith(RegionInfo.CurrentRegion.TwoLetterISORegionName)).Split(
                    new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }

            // Set the program thread pool.
            ThreadPool.SetMaxThreads(this.workerThreadCount, Program.completionThreadCount);

            // Run each destination on the thread pool.
            foreach (string destination in this.destinations)
            {
                ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        this.Run(destination);
                    });
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
            catch (Exception exception)
            {
                Program.WriteLine(ConsoleColor.Red, "Traceroute failed {0} / {1}", destination, exception.Message);
            }

            // Increment the results count.
            Interlocked.Increment(ref this.resultsCount);

            // If all destinations are completed.
            if (this.resultsCount == this.destinations.Length)
            {
                // Signal the wait handle.
                this.wait.Set();
            }
        }

        public void Run(MultipathTraceroute tracerouteIp, string destination, IPAddress sourceAddress, IPAddress destinationAddress)
        {
            // Run the IPv4 traceroute.
            MultipathTracerouteResult resultIp = tracerouteIp.RunIpv4(sourceAddress, destinationAddress, CancellationToken.None, null);

            // Run the AS-level traceroute.
            ASTracerouteResult resultAs = tracerouteAs.Run(resultIp, CancellationToken.None, null);

            // Upload the traceroute result to Mercury.

            // Write the destination.
            Program.Write(ConsoleColor.White, "Traceroute to........");
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

        #endregion

    }
}
