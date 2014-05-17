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
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using DotNetApi;
using InetApi.Net.Core;
using InetApi.Net.Core.Dns;
using InetApi.Net.Core.Protocols;
using InetCommon.Net;
using Mercury.Properties;
using Mercury.Topology;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mercury
{
	/// <summary>
	/// A class representing the main program of the Mercury traceroute.
	/// </summary>
	public sealed class Program : IDisposable
	{
		private readonly NetworkInterface[] interfaces;

		private IPAddress dnsServer = null;
		private readonly DnsClient dnsClient = new DnsClient(5000);

		private string destination = null;
		private int? iface = null;

		private MultipathTraceroute tracerouteIp;
		private MultipathTracerouteSettings settingsIp;

		private ASTraceroute tracerouteAs;
		private ASTracerouteSettings settingsAs;

		private bool flagIpv6 = false;
		private int verbosity = 2;
        private string fileLoadIp = null;
        private string fileSaveIp = null;

		/// <summary>
		/// Creates a new program instance.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public Program(string[] args)
		{
			// Write the program information.
			Program.WriteLine(ConsoleColor.White, Resources.Title);

			// Get the current interfaces.
			this.interfaces = NetworkConfiguration.GetNetworkInterfacesWithUnicastAddresses();

			// Create the IP-level traceroute settings.
			this.settingsIp = new MultipathTracerouteSettings();

			// Create the IP-level traceroute.
			this.tracerouteIp = new MultipathTraceroute(this.settingsIp);

			// Create the AS-level traceroute settings.
			this.settingsAs = new ASTracerouteSettings();

			// Create the AS-level traceroute.
			this.tracerouteAs = new ASTraceroute(this.settingsAs);

			// Parse the arguments.
			for (int argumentIndex = 0; argumentIndex < args.Length; argumentIndex++)
			{
				// If the argument is an option.
				if (this.IsOption(args, argumentIndex))
				{
					// Parse the option.
					this.ParseOption(args, ref argumentIndex);
				}
				else if (null == this.destination)
				{
					// Set the destination.
					this.destination = args[argumentIndex];
				}
				else throw new ArgumentException("Unknown arguments {0}.".FormatWith(args[argumentIndex]));
			}

			// If not all parameters are set, throw an exception.
			if ((null == this.destination) && (null == this.fileLoadIp))
                throw new ArgumentException("The destination or input file are missing.");
		}

		#region Public methods

		/// <summary>
		/// The main method.
		/// </summary>
		/// <param name="args">The arguments.</param>
		static void Main(string[] args)
		{
			try
			{
				// Create a new program.
				using (Program program = new Program(args))
				{
					// Run the program.
					program.Run();
				}
			}
			catch (Exception exception)
			{
				// Show the syntax.
				Program.WriteLine(ConsoleColor.Gray, Resources.Syntax);
				// Show the error.
				Program.WriteLine(ConsoleColor.Red, "ERROR  {0}", exception.Message);
			}

			// Reset the console.
			Console.ResetColor();
		}

		/// <summary>
		/// Disposes the current object.
		/// </summary>
		public void Dispose()
		{
			// Suppress the finalizer.
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Runs the current traceroute.
		/// </summary>
		private void Run()
		{
            // If the traceroute file is null.
            if (null == this.fileLoadIp)
            {
                // Run the live traceroute.
                this.RunLive();
            }
            else
            {
                // Load the traceroute result from file.
                this.RunFile();
            }
		}

        /// <summary>
        /// Runs the current traceroute live.
        /// </summary>
        private void RunLive()
        {
            // Show the local interfaces.
            Program.WriteLine(ConsoleColor.Gray, "Network interfaces");
            Program.WriteLine(ConsoleColor.Gray, "{0}|{1}|{2}|{3}|{4}",
                "#".PadRight(3), "Name".PadRight(40), "IPv4 Address".PadRight(16), "IPv6 Address".PadRight(40), "DNS Server");
            for (int index = 0; index < this.interfaces.Length; index++)
            {
                IPAddress addressIPv4 = NetworkConfiguration.GetLocalUnicastAddress(this.interfaces[index], AddressFamily.InterNetwork);
                IPAddress addressIPv6 = NetworkConfiguration.GetLocalUnicastAddress(this.interfaces[index], AddressFamily.InterNetworkV6);
                IPAddress addressDns = NetworkConfiguration.GetDnsServer(this.interfaces[index]);

                Program.WriteLine(ConsoleColor.Cyan, "{0}|{1}|{2}|{3}|{4}",
                    index.ToString().PadRight(3),
                    this.interfaces[index].Name.PadRight(40),
                    addressIPv4 != null ? addressIPv4.ToString().PadRight(16) : string.Empty.PadRight(16),
                    addressIPv6 != null ? addressIPv6.ToString().PadRight(40) : string.Empty.PadRight(40),
                    addressDns != null ? addressDns.ToString() : string.Empty);
            }
            Console.WriteLine();

            // Validate the interface.
            if (this.iface != null ? this.iface >= this.interfaces.Length : false)
            {
                Program.WriteLine(ConsoleColor.Red, "ERROR  The interface index {0} is out-of-range.", this.iface);
                return;
            }

            // Get the local address.
            IPAddress localAddress = this.iface != null ?
                NetworkConfiguration.GetLocalUnicastAddress(this.interfaces[this.iface.Value], this.flagIpv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork) :
                NetworkConfiguration.GetLocalUnicastAddress(this.flagIpv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork);

            // Validate the local address.
            if (null == localAddress)
            {
                Program.WriteLine(ConsoleColor.Red, "ERROR  There is no {0} address.", this.flagIpv6 ? "IPv6" : "IPv4");
                return;
            }

            // Show the local IP address.
            Program.WriteLine(ConsoleColor.Gray, "Local address..........................", ConsoleColor.Cyan, localAddress.ToString());

            // Get the DNS server.
            IPAddress dnsAddress = this.dnsServer != null ? this.dnsServer : this.iface != null ? NetworkConfiguration.GetDnsServer(this.interfaces[this.iface.Value]) : NetworkConfiguration.GetDnsServer(this.interfaces);

            // Show the DNS server.
            Program.WriteLine(ConsoleColor.Gray, "DNS server.............................", ConsoleColor.Cyan, dnsAddress.ToString());
            // Show the destination.
            Program.WriteLine(ConsoleColor.Gray, "Destination............................", ConsoleColor.Cyan, this.destination);

            // Resolve the remote IP address.
            List<IPAddress> remoteAddresses = new List<IPAddress>();
            try
            {
                IPAddress address;
                // Try parse the destination name to a remote address.
                if (IPAddress.TryParse(this.destination, out address))
                {
                    // Show the remote IP address.
                    Program.WriteLine(ConsoleColor.Gray, "Remote address.........................", ConsoleColor.Cyan, address.ToString());
                    // Add the address.
                    remoteAddresses.Add(address);
                }
                else
                {
                    DnsMessage dnsResponse = this.dnsClient.Resolve(this.destination, localAddress, dnsAddress, this.flagIpv6 ? RecordType.Aaaa : RecordType.A, RecordClass.INet);
                    if (this.flagIpv6)
                    {
                        foreach (DnsRecordBase record in dnsResponse.AnswerRecords.Where(rec => rec is AaaaRecord))
                        {
                            // Get the IP address.
                            address = (record as AaaaRecord).Address;
                            // Show the remote IP address.
                            Program.WriteLine(ConsoleColor.Gray, "Remote address.........................", ConsoleColor.Cyan, address.ToString());
                            // Add the address.
                            remoteAddresses.Add(address);
                        }
                    }
                    else
                    {
                        foreach (DnsRecordBase record in dnsResponse.AnswerRecords.Where(rec => rec is ARecord))
                        {
                            // Get the IP address.
                            address = (record as ARecord).Address;
                            // Show the remote IP address.
                            Program.WriteLine(ConsoleColor.Gray, "Remote address.........................", ConsoleColor.Cyan, address.ToString());
                            // Add the address.
                            remoteAddresses.Add(address);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Program.WriteLine(ConsoleColor.Red, "ERROR  Cannot resolve the name {0} using DNS server {1}. {2}", this.destination, dnsAddress, exception.Message);
                return;
            }

            // Validate the remore addresses.
            if (0 == remoteAddresses.Count)
            {
                Program.WriteLine(ConsoleColor.Red, "ERROR  There are no remote addresses.");
                return;
            }

            // Show the traceroute settings.
            Console.WriteLine();
            Program.WriteLine(ConsoleColor.Gray, "Attempts per flow......................", ConsoleColor.Cyan, this.settingsIp.AttemptsPerFlow.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Flow count.............................", ConsoleColor.Cyan, this.settingsIp.FlowCount.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Minimum hops...........................", ConsoleColor.Cyan, this.settingsIp.MinimumHops.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Maximum hops...........................", ConsoleColor.Cyan, this.settingsIp.MaximumHops.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Maximum unknown hops...................", ConsoleColor.Cyan, this.settingsIp.MaximumUnknownHops.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Attempt delay (ms).....................", ConsoleColor.Cyan, this.settingsIp.AttemptDelay.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Hop timeout (ms).......................", ConsoleColor.Cyan, this.settingsIp.HopTimeout.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Data length (bytes)....................", ConsoleColor.Cyan, this.settingsIp.DataLength.ToString());

            // Run the traceroute for each remote address.
            foreach (IPAddress remoteAddress in remoteAddresses)
            {
                this.RunLive(localAddress, remoteAddress);
            }
        }

		/// <summary>
		/// Runs the traceroute for the specified IP address.
		/// </summary>
		/// <param name="localAddress">The local IP address.</param>
		/// <param name="remoteAddress">The remote IP address.</param>
		private void RunLive(IPAddress localAddress, IPAddress remoteAddress)
		{
			// Show the destination.
			Console.WriteLine();
			Program.WriteLine(ConsoleColor.Gray, "Traceroute ", ConsoleColor.Green, "{0} <--> {1}", localAddress, remoteAddress);

			// Create a cancellation token source.
			using (CancellationTokenSource cancel = new CancellationTokenSource())
			{
				try
				{
                    // The IP-level traceroute result.
                    MultipathTracerouteResult resultIp = this.tracerouteIp.RunIpv4(localAddress, remoteAddress, cancel.Token, (MultipathTracerouteResult result, MultipathTracerouteState state) =>
                    {
                        // Level 3 verbosity.
                        if (this.verbosity >= 3)
                        {
                            switch (state.Type)
                            {
                                case MultipathTracerouteState.StateType.PacketCapture:
                                    Program.WriteLine(ConsoleColor.Yellow, "PACKET-CAPTURE ", ConsoleColor.Gray, (state.Parameters[0] as ProtoPacketIp).ToString());
                                    {
                                        ProtoPacket payload = (state.Parameters[0] as ProtoPacketIp).Payload;
                                        if (null != payload)
                                        {
                                            Program.WriteLine(ConsoleColor.Gray, "               Payload: {0}", payload.ToString());
                                            if (payload is ProtoPacketIcmpTimeExceeded)
                                            {
                                                Program.WriteLine(ConsoleColor.Gray, "                        {0}", (payload as ProtoPacketIcmpTimeExceeded).IpHeader.ToString());
                                                Program.WriteLine(ConsoleColor.Gray, "                        IPv4 PAYLOAD Data: {0}", (payload as ProtoPacketIcmpTimeExceeded).IpPayload.ToExtendedString());
                                            }
                                            if (payload is ProtoPacketIcmpDestinationUnreachable)
                                            {
                                                Program.WriteLine(ConsoleColor.Gray, "                        {0}", (payload as ProtoPacketIcmpDestinationUnreachable).IpHeader.ToString());
                                                Program.WriteLine(ConsoleColor.Gray, "                        IPv4 PAYLOAD Data: {0}", (payload as ProtoPacketIcmpDestinationUnreachable).IpPayload.ToExtendedString());
                                            }
                                        }
                                    }
                                    break;
                                case MultipathTracerouteState.StateType.PacketError:
                                    Program.WriteLine(ConsoleColor.Red, "PACKET-ERROR ", ConsoleColor.Gray, (state.Parameters[0] as Exception).Message);
                                    break;
                                case MultipathTracerouteState.StateType.BeginAlgorithm:
                                    Program.WriteLine(ConsoleColor.Cyan, "BEGIN-ALGORITHM ", ConsoleColor.Gray, "Algorithm: {0}", (MultipathTraceroute.MultipathAlgorithm)state.Parameters[0]);
                                    break;
                                case MultipathTracerouteState.StateType.EndAlgorithm:
                                    Program.WriteLine(ConsoleColor.Cyan, "END-ALGORITHM ", ConsoleColor.Gray, "Algorithm: {0}", (MultipathTraceroute.MultipathAlgorithm)state.Parameters[0]);
                                    break;
                                case MultipathTracerouteState.StateType.BeginFlow:
                                    {
                                        byte flow = (byte)state.Parameters[0];
                                        Program.WriteLine(ConsoleColor.Cyan, "BEGIN-FLOW ", ConsoleColor.Gray, "Index: {0} ID: {1}", flow, result.Flows[flow].Id);
                                        Program.WriteLine(ConsoleColor.Gray, "           ICMP identifier: 0x{0:X4} ICMP checksum: 0x{1:X4}", result.Flows[flow].IcmpId, result.Flows[flow].IcmpChecksum);
                                    }
                                    break;
                                case MultipathTracerouteState.StateType.EndFlow:
                                    {
                                        byte flow = (byte)state.Parameters[0];
                                        Program.WriteLine(ConsoleColor.Cyan, "END-FLOW ", ConsoleColor.Gray, "Index: {0}", flow);
                                    }
                                    break;
                                case MultipathTracerouteState.StateType.BeginTtl:
                                    Program.WriteLine(ConsoleColor.Cyan, "BEGIN-TTL ", ConsoleColor.Gray, "TTL: {0}", (byte)state.Parameters[0]);
                                    break;
                                case MultipathTracerouteState.StateType.EndTtl:
                                    Program.WriteLine(ConsoleColor.Cyan, "END-TTL ", ConsoleColor.Gray, "TTL: {0}", (byte)state.Parameters[0]);
                                    break;
                                case MultipathTracerouteState.StateType.RequestExpired:
                                    {
                                        MultipathTracerouteResult.RequestState requestState = (MultipathTracerouteResult.RequestState)state.Parameters[0];
                                        Program.WriteLine(ConsoleColor.Magenta, "REQUEST-EXPIRED ", ConsoleColor.Gray, "Type: {0} Timestamp: {1} Timeout: {2} Flow: {3} Attempt: {4} TTL: {5}",
                                            requestState.Flow, requestState.Timestamp, requestState.Timeout, requestState.Flow, requestState.Attempt, requestState.TimeToLive);
                                    }
                                    break;
                            }
                        }

                        if (state.Type == MultipathTracerouteState.StateType.EndAlgorithm)
                        {
                            this.DisplayIpTracerouteResult(result, (MultipathTraceroute.MultipathAlgorithm)state.Parameters[0]);
                        }
                    });

                    // Save the result if requested.
                    if (null != this.fileSaveIp)
                    {
                        using (FileStream file = File.Create(this.fileSaveIp))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(file, resultIp);
                        }
                    }
                    // Run the AS-level traceroute.
					ASTracerouteResult resultAs = this.tracerouteAs.Run(resultIp, cancel.Token, (ASTracerouteResult result, ASTracerouteState state) =>
						{
						});
				}
				catch (Exception exception)
				{
					Program.WriteLine(ConsoleColor.Red, "ERROR  {0}", exception.Message);
					return;
				}
			}
		}

        /// <summary>
        /// Load the IP-level traceroute from a file.
        /// </summary>
        private void RunFile()
        {
            MultipathTracerouteResult resultIp;

            // Load the traceroute result from file.
            using (FileStream file = File.OpenRead(this.fileLoadIp))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                resultIp = formatter.Deserialize(file) as MultipathTracerouteResult;
            }
            
            // Show the local IP address.
            Program.WriteLine(ConsoleColor.Gray, "Local address..........................", ConsoleColor.Cyan, resultIp.LocalAddress.ToString());

            // Show the remote IP address.f
            Program.WriteLine(ConsoleColor.Gray, "Remote address.........................", ConsoleColor.Cyan, resultIp.RemoteAddress.ToString());

            // Show the traceroute settings.
            Console.WriteLine();
            Program.WriteLine(ConsoleColor.Gray, "Attempts per flow......................", ConsoleColor.Cyan, resultIp.Settings.AttemptsPerFlow.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Flow count.............................", ConsoleColor.Cyan, resultIp.Settings.FlowCount.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Minimum hops...........................", ConsoleColor.Cyan, resultIp.Settings.MinimumHops.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Maximum hops...........................", ConsoleColor.Cyan, resultIp.Settings.MaximumHops.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Maximum unknown hops...................", ConsoleColor.Cyan, resultIp.Settings.MaximumUnknownHops.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Attempt delay (ms).....................", ConsoleColor.Cyan, resultIp.Settings.AttemptDelay.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Hop timeout (ms).......................", ConsoleColor.Cyan, resultIp.Settings.HopTimeout.ToString());
            Program.WriteLine(ConsoleColor.Gray, "Data length (bytes)....................", ConsoleColor.Cyan, resultIp.Settings.DataLength.ToString());

			// Show the traceroute result.
			if ((resultIp.Settings.Algorithm & MultipathTraceroute.MultipathAlgorithm.Icmp) != 0)
			{
				this.DisplayIpTracerouteResult(resultIp, MultipathTraceroute.MultipathAlgorithm.Icmp);
			}
			if ((resultIp.Settings.Algorithm & MultipathTraceroute.MultipathAlgorithm.Udp) != 0)
			{
				this.DisplayIpTracerouteResult(resultIp, MultipathTraceroute.MultipathAlgorithm.Udp);
			}
			if ((resultIp.Settings.Algorithm & MultipathTraceroute.MultipathAlgorithm.UdpTest) != 0)
			{
				this.DisplayIpTracerouteResult(resultIp, MultipathTraceroute.MultipathAlgorithm.UdpTest);
			}
		}

        /// <summary>
		/// Display the result of the IP traceroute.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="algorithm">The algorithm.</param>
		private void DisplayIpTracerouteResult(MultipathTracerouteResult result, MultipathTraceroute.MultipathAlgorithm algorithm)
		{
			switch (algorithm)
			{
				case MultipathTraceroute.MultipathAlgorithm.Icmp:
					// Display the ICMP result.
					for (byte flow = 0; flow < result.Flows.Length; flow++)
					{
						Console.WriteLine();
						Program.WriteLine(ConsoleColor.Gray, "Protocol...............................", ConsoleColor.Yellow, "ICMP");
						Console.WriteLine();
						Program.WriteLine(ConsoleColor.Gray, "Flow index.............................", ConsoleColor.Cyan, flow.ToString());
						Program.WriteLine(ConsoleColor.Gray, "Flow identifier........................", ConsoleColor.Cyan, result.Flows[flow].Id.ToString());
						Program.WriteLine(ConsoleColor.Gray, "Flow short identifier..................", ConsoleColor.Cyan, "0x{0:X4}", result.Flows[flow].ShortId);
						Program.WriteLine(ConsoleColor.Gray, "Flow ICMP identifier...................", ConsoleColor.Cyan, "0x{0:X4}", result.Flows[flow].IcmpId);
						Program.WriteLine(ConsoleColor.Gray, "Flow ICMP checksum.....................", ConsoleColor.Cyan, "0x{0:X4}", result.Flows[flow].IcmpChecksum);
						Console.WriteLine();

						// Display the results header.
						Program.Write(ConsoleColor.White, " TTL ");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							Program.Write(ConsoleColor.White, "| Attempt {0}(RTT) ", attempt.ToString().PadRight(8));
						}
						Console.WriteLine();

						// Compute the maximum time-to-live.
						byte maximumTtl = byte.MinValue;
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							maximumTtl = maximumTtl < result.IcmpStatistics[flow, attempt].MaximumTimeToLive ? result.IcmpStatistics[flow, attempt].MaximumTimeToLive : maximumTtl;
						}

						// Display the hops data.
						for (byte ttl = 0; ttl < this.settingsIp.MinimumHops + maximumTtl - 1; ttl++)
						{
							Program.Write(ConsoleColor.Gray, (this.settingsIp.MinimumHops + ttl).ToString().PadLeft(5));
							for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
							{
								switch (result.IcmpData[flow, ttl, attempt].State)
								{
									case MultipathTracerouteData.DataState.NotSet:
										Program.Write(ConsoleColor.White, "| ", ConsoleColor.Yellow, "Not set".PadRight(22));
										break;
									case MultipathTracerouteData.DataState.RequestSent:
										Program.Write(ConsoleColor.White, "| ", ConsoleColor.Red, "Timeout".PadRight(22));
										break;
									case MultipathTracerouteData.DataState.ResponseReceived:
										{
											int rtt = (int)(result.IcmpData[flow, ttl, attempt].ResponseTimestamp - result.IcmpData[flow, ttl, attempt].RequestTimestamp).TotalMilliseconds;
											Program.Write(ConsoleColor.White, "| ", ConsoleColor.Green, result.IcmpData[flow, ttl, attempt].Address.ToString().PadRight(16));
											Program.Write(ConsoleColor.Gray, "({0})".FormatWith(rtt).PadRight(6));
										}
										break;
								}
							}
							Console.WriteLine();
						}

						// Display the results footer.
						Program.Write(ConsoleColor.White, "=====");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							Program.Write(ConsoleColor.White, "|=======================", attempt.ToString().PadRight(8));
						}
						Console.WriteLine();

						// Display the results statistics.
						Program.Write(ConsoleColor.White, "     ");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							switch (result.IcmpStatistics[flow, attempt].State)
							{
								case MultipathTracerouteStatistics.StatisticsState.Completed:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Green, "Completed".PadRight(22));
									break;
								case MultipathTracerouteStatistics.StatisticsState.Unreachable:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Red, "Unreachable".PadRight(22));
									break;
								default:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Yellow, "Unknown".PadRight(22));
									break;
							}
						}
						Console.WriteLine();
					}
					break;
				case MultipathTraceroute.MultipathAlgorithm.Udp:
					// Display the ICMP result.
					for (byte flow = 0; flow < result.Flows.Length; flow++)
					{
						Console.WriteLine();
						Program.WriteLine(ConsoleColor.Gray, "Protocol...............................", ConsoleColor.Yellow, "UDP");
						Console.WriteLine();
						Program.WriteLine(ConsoleColor.Gray, "Flow index.............................", ConsoleColor.Cyan, flow.ToString());
						Program.WriteLine(ConsoleColor.Gray, "Flow identifier........................", ConsoleColor.Cyan, result.Flows[flow].Id.ToString());
						Program.WriteLine(ConsoleColor.Gray, "Flow short identifier..................", ConsoleColor.Cyan, "0x{0:X4}", result.Flows[flow].ShortId);
						Program.WriteLine(ConsoleColor.Gray, "Flow UDP source port...................", ConsoleColor.Cyan, "{0}", result.Flows[flow].UdpSourcePort);
						Program.WriteLine(ConsoleColor.Gray, "Flow UDP destination port..............", ConsoleColor.Cyan, "{0}", result.Flows[flow].UdpDestinationPort);
						Program.WriteLine(ConsoleColor.Gray, "Flow UDP identifier....................", ConsoleColor.Cyan, "0x{0:X4}", result.Flows[flow].UdpId);
						Console.WriteLine();

						// Display the results header.
						Program.Write(ConsoleColor.White, " TTL ");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							Program.Write(ConsoleColor.White, "| Attempt {0}(RTT) ", attempt.ToString().PadRight(8));
						}
						Console.WriteLine();

						// Compute the maximum time-to-live.
						byte maximumTtl = byte.MinValue;
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							maximumTtl = maximumTtl < result.UdpStatistics[flow, attempt].MaximumTimeToLive ? result.UdpStatistics[flow, attempt].MaximumTimeToLive : maximumTtl;
						}

						// Display the hops data.
						for (byte ttl = 0; ttl < maximumTtl - this.settingsIp.MinimumHops + 1; ttl++)
						{
							Program.Write(ConsoleColor.Gray, (this.settingsIp.MinimumHops + ttl).ToString().PadLeft(5));
							for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
							{
								switch (result.UdpData[flow, ttl, attempt].State)
								{
									case MultipathTracerouteData.DataState.NotSet:
										Program.Write(ConsoleColor.White, "| ", ConsoleColor.Yellow, "Not set".PadRight(22));
										break;
									case MultipathTracerouteData.DataState.RequestSent:
										Program.Write(ConsoleColor.White, "| ", ConsoleColor.Red, "Timeout".PadRight(22));
										break;
									case MultipathTracerouteData.DataState.ResponseReceived:
										{
											int rtt = (int)(result.UdpData[flow, ttl, attempt].ResponseTimestamp - result.UdpData[flow, ttl, attempt].RequestTimestamp).TotalMilliseconds;
											Program.Write(ConsoleColor.White, "| ", ConsoleColor.Green, result.UdpData[flow, ttl, attempt].Address.ToString().PadRight(16));
											Program.Write(ConsoleColor.Gray, "({0})".FormatWith(rtt).PadRight(6));
										}
										break;
								}
							}
							Console.WriteLine();
						}

						// Display the results footer.
						Program.Write(ConsoleColor.White, "=====");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							Program.Write(ConsoleColor.White, "|=======================", attempt.ToString().PadRight(8));
						}
						Console.WriteLine();

						// Display the results statistics.
						Program.Write(ConsoleColor.White, "     ");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							switch (result.UdpStatistics[flow, attempt].State)
							{
								case MultipathTracerouteStatistics.StatisticsState.Completed:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Green, "Completed".PadRight(22));
									break;
								case MultipathTracerouteStatistics.StatisticsState.Unreachable:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Red, "Unreachable".PadRight(22));
									break;
								default:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Yellow, "Unknown".PadRight(22));
									break;
							}
						}
						Console.WriteLine();
					}
					break;
				case MultipathTraceroute.MultipathAlgorithm.UdpTest:
					// Display the ICMP result.
					for (byte flow = 0; flow < result.Flows.Length; flow++)
					{
						Console.WriteLine();
						Program.WriteLine(ConsoleColor.Gray, "Protocol...............................", ConsoleColor.Yellow, "UDP (TEST)");
						Console.WriteLine();
						Program.WriteLine(ConsoleColor.Gray, "Flow index.............................", ConsoleColor.Cyan, flow.ToString());
						Program.WriteLine(ConsoleColor.Gray, "Flow identifier........................", ConsoleColor.Cyan, result.Flows[flow].Id.ToString());
						Program.WriteLine(ConsoleColor.Gray, "Flow short identifier..................", ConsoleColor.Cyan, "0x{0:X4}", result.Flows[flow].ShortId);
						Program.WriteLine(ConsoleColor.Gray, "Flow UDP source port...................", ConsoleColor.Cyan, "{0}", result.Flows[flow].UdpSourcePort);
						Program.WriteLine(ConsoleColor.Gray, "Flow UDP destination port..............", ConsoleColor.Cyan, "{0}", result.Flows[flow].UdpDestinationPort);
						Program.WriteLine(ConsoleColor.Gray, "Flow UDP identifier....................", ConsoleColor.Cyan, "0x{0:X4}", result.Flows[flow].UdpId);
						Console.WriteLine();

						// Display the results header.
						Program.Write(ConsoleColor.White, " TTL ");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							Program.Write(ConsoleColor.White, "| Attempt {0}      ", attempt.ToString().PadRight(8));
						}
						Console.WriteLine();

						// Compute the maximum time-to-live.
						byte maximumTtl = byte.MinValue;
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							maximumTtl = maximumTtl < result.UdpStatistics[flow, attempt].MaximumTimeToLive ? result.UdpStatistics[flow, attempt].MaximumTimeToLive : maximumTtl;
						}

						// Display the hops data.
						for (byte ttl = 0; ttl < maximumTtl - this.settingsIp.MinimumHops + 1; ttl++)
						{
							Program.Write(ConsoleColor.Gray, (this.settingsIp.MinimumHops + ttl).ToString().PadLeft(5));
							for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
							{
								switch (result.UdpData[flow, ttl, attempt].State)
								{
									case MultipathTracerouteData.DataState.NotSet:
										Program.Write(ConsoleColor.White, "| ", ConsoleColor.Yellow, "Not set".PadRight(22));
										break;
									case MultipathTracerouteData.DataState.RequestSent:
										Program.Write(ConsoleColor.White, "| ", ConsoleColor.Red, "Timeout".PadRight(22));
										break;
									case MultipathTracerouteData.DataState.ResponseReceived:
										{
											int rtt = (int)(result.UdpData[flow, ttl, attempt].ResponseTimestamp - result.UdpData[flow, ttl, attempt].RequestTimestamp).TotalMilliseconds;

											if (result.UdpData[flow, ttl, attempt].Response.Payload is ProtoPacketIcmpTimeExceeded)
											{
												ProtoPacketIcmpTimeExceeded icmpTimeExceeded = result.UdpData[flow, ttl, attempt].Response.Payload as ProtoPacketIcmpTimeExceeded;
												ushort identification = icmpTimeExceeded.IpHeader.Identification;
												ushort checksum = (ushort)((icmpTimeExceeded.IpPayload[6] << 8) | icmpTimeExceeded.IpPayload[7]);

												Program.Write(ConsoleColor.White, "| ", ConsoleColor.Green, "0x{0} {1} 0x{2}      ",
													identification.ToString("X4").PadRight(4),
													identification == checksum ? "==" : "!=",
													checksum.ToString("X4").PadRight(4));
											}
											else if (result.UdpData[flow, ttl, attempt].Response.Payload is ProtoPacketIcmpDestinationUnreachable)
											{
												ProtoPacketIcmpDestinationUnreachable icmpTimeExceeded = result.UdpData[flow, ttl, attempt].Response.Payload as ProtoPacketIcmpDestinationUnreachable;
												ushort identification = icmpTimeExceeded.IpHeader.Identification;
												ushort checksum = (ushort)((icmpTimeExceeded.IpPayload[6] << 8) | icmpTimeExceeded.IpPayload[7]);

												Program.Write(ConsoleColor.White, "| ", ConsoleColor.Green, "0x{0} {1} 0x{2}      ",
													identification.ToString("X4").PadRight(4),
													identification == checksum ? "==" : "!=",
													checksum.ToString("X4").PadRight(4));
											}
											else
											{
												Program.Write(ConsoleColor.White, "| ", ConsoleColor.Red, "Unknown".PadRight(22));
											}
										}
										break;
								}
							}
							Console.WriteLine();
						}

						// Display the results footer.
						Program.Write(ConsoleColor.White, "=====");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							Program.Write(ConsoleColor.White, "|=======================", attempt.ToString().PadRight(8));
						}
						Console.WriteLine();

						// Display the results statistics.
						Program.Write(ConsoleColor.White, "     ");
						for (byte attempt = 0; attempt < this.settingsIp.AttemptsPerFlow; attempt++)
						{
							switch (result.IcmpStatistics[flow, attempt].State)
							{
								case MultipathTracerouteStatistics.StatisticsState.Completed:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Green, "Completed".PadRight(22));
									break;
								case MultipathTracerouteStatistics.StatisticsState.Unreachable:
									Program.Write(ConsoleColor.White, "| ", ConsoleColor.Red, "Unreachable".PadRight(22));
									break;
							}
						}
						Console.WriteLine();
					}
					break;
					// Run the AS-level traceroute.
					//ASTracerouteResult resultAs = this.tracerouteAs.Run(resultIp, cancel.Token, (ASTracerouteResult result, ASTracerouteState state) =>
					//	{
					//	});
			}
		}

		/// <summary>
		/// Indicates whether the argument at the specified index is an option.
		/// </summary>
		/// <param name="args">The list of arguments.</param>
		/// <param name="argumentIndex"></param>
		/// <returns><b>True</b> if the argument is an option, <b>false</b> otherwise.</returns>
		private bool IsOption(string[] args, int argumentIndex)
		{
			return string.IsNullOrWhiteSpace(args[argumentIndex].Trim()) ? false : args[argumentIndex].Trim()[0] == '-';
		}

		/// <summary>
		/// Parses the option arguments from the list at the specified index.
		/// </summary>
		/// <param name="args">The list of arguments.</param>
		/// <param name="argumentIndex">The argument index.</param>
		private void ParseOption(string[] args, ref int argumentIndex)
		{
			// Parse the argument.
			switch (args[argumentIndex].Trim())
			{
				case "-a": this.settingsIp.Algorithm = this.ParseAlgorithm(args, ref argumentIndex); break;
				case "-c": this.settingsIp.AttemptsPerFlow = byte.Parse(args[++argumentIndex]); break;
				case "-d": this.dnsServer = IPAddress.Parse(args[++argumentIndex]); break;
				case "-e": this.settingsIp.AttemptDelay = int.Parse(args[++argumentIndex]); break;
				case "-f": this.settingsIp.FlowCount = byte.Parse(args[++argumentIndex]); break;
				case "-h": this.settingsIp.MaximumUnknownHops = byte.Parse(args[++argumentIndex]); break;
				case "-i": this.iface = int.Parse(args[++argumentIndex]); break;
				case "-o": this.settingsIp.HopTimeout = int.Parse(args[++argumentIndex]); break;
				case "-p-min": this.settingsIp.MinimumPort = ushort.Parse(args[++argumentIndex]); break;
				case "-p-max": this.settingsIp.MaximumPort = ushort.Parse(args[++argumentIndex]); break;
				case "-s": this.settingsIp.DataLength = int.Parse(args[++argumentIndex]); break;
				case "-t": this.dnsClient.QueryTimeout = int.Parse(args[++argumentIndex]); break;
				case "-t-min": this.settingsIp.MinimumHops = byte.Parse(args[++argumentIndex]); break;
				case "-t-max": this.settingsIp.MaximumHops = byte.Parse(args[++argumentIndex]); break;
				case "-v": this.verbosity = int.Parse(args[++argumentIndex]); break;
                case "--ip-in": this.fileLoadIp = args[++argumentIndex]; break;
                case "--ip-out": this.fileSaveIp = args[++argumentIndex]; break;
				default: throw new ArgumentException("Option {0} unknown.".FormatWith(args[argumentIndex]));
			}
		}

		/// <summary>
		/// Parses the algorithm.
		/// </summary>
		/// <param name="args">The list of arguments.</param>
		/// <param name="argumentIndex">The argument index.</param>
		/// <returns>The algorithm.</returns>
		private MultipathTraceroute.MultipathAlgorithm ParseAlgorithm(string[] args, ref int argumentIndex)
		{
			switch (int.Parse(args[++argumentIndex]))
			{
				case 0: return MultipathTraceroute.MultipathAlgorithm.Icmp | MultipathTraceroute.MultipathAlgorithm.UdpIdentification;
				case 1: return MultipathTraceroute.MultipathAlgorithm.Icmp | MultipathTraceroute.MultipathAlgorithm.UdpChecksum;
				case 2: return MultipathTraceroute.MultipathAlgorithm.Icmp | MultipathTraceroute.MultipathAlgorithm.UdpIdentification | MultipathTraceroute.MultipathAlgorithm.UdpChecksum;
				case 3: return MultipathTraceroute.MultipathAlgorithm.Icmp;
				case 4: return MultipathTraceroute.MultipathAlgorithm.UdpIdentification;
				case 5: return MultipathTraceroute.MultipathAlgorithm.UdpChecksum;
				case 6: return MultipathTraceroute.MultipathAlgorithm.UdpIdentification | MultipathTraceroute.MultipathAlgorithm.UdpChecksum;
				case 7: return MultipathTraceroute.MultipathAlgorithm.UdpTest;
				default: throw new ArgumentException("Algorithm unknown.");
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
