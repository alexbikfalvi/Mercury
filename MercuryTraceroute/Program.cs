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
using InetCommon.Net;
using Mercury.Properties;

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

		private MultipathTraceroute traceroute;
		private MultipathTracerouteSettings settings;

		private bool flagIpv6 = false;
		private bool flagResolveAs = false;
		private bool flagUseUdp = true;


		/// <summary>
		/// Creates a new program instance.
		/// </summary>
		/// <param name="args"></param>
		public Program(string[] args)
		{
			// Write the program information.
			Program.WriteLine(ConsoleColor.White, Resources.Title);

			// Get the current interfaces.
			this.interfaces = NetworkConfiguration.GetNetworkInterfacesWithUnicastAddresses();

			// Create the traceroute settings.
			this.settings = new MultipathTracerouteSettings();

			// Create the traceroute.
			this.traceroute = new MultipathTraceroute(this.settings);

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
			if (null == this.destination) throw new ArgumentException("The destination is missing.");
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
			// Show the local interfaces.
			Program.WriteLine(ConsoleColor.Gray, "Network interfaces");
			Program.WriteLine(ConsoleColor.Gray, "{0}|{1}|{2}|{3}|{4}",
				"#".PadRight(3), "Name".PadRight(30), "IPv4 Address".PadRight(16), "IPv6 Address".PadRight(40), "DNS Server");
			for (int index = 0; index < this.interfaces.Length; index++)
			{
				IPAddress addressIPv4 = NetworkConfiguration.GetLocalUnicastAddress(this.interfaces[index], AddressFamily.InterNetwork);
				IPAddress addressIPv6 = NetworkConfiguration.GetLocalUnicastAddress(this.interfaces[index], AddressFamily.InterNetworkV6);
				IPAddress addressDns = NetworkConfiguration.GetDnsServer(this.interfaces[index]);

				Program.WriteLine(ConsoleColor.Cyan, "{0}|{1}|{2}|{3}|{4}",
					index.ToString().PadRight(3),
					this.interfaces[index].Name.PadRight(30),
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
			IPAddress dnsAddress = this.dnsServer != null ? this.dnsServer : this.iface != null ? NetworkConfiguration.GetDnsServer(this.interfaces[this.iface.Value]) : NetworkConfiguration.GetDnsServer();
			
			// Show the DNS server.
			Program.WriteLine(ConsoleColor.Gray, "DNS server.............................", ConsoleColor.Cyan, dnsAddress.ToString());
			// Show the destination.
			Program.WriteLine(ConsoleColor.Gray, "Destination............................", ConsoleColor.Cyan, this.destination);

			// Resolve the remote IP address.
			List<IPAddress> remoteAddresses = new List<IPAddress>();
			try
			{
				DnsMessage dnsResponse = this.dnsClient.Resolve(this.destination, localAddress, dnsAddress, this.flagIpv6 ? RecordType.Aaaa : RecordType.A, RecordClass.INet);
				if (this.flagIpv6)
				{
					foreach (DnsRecordBase record in dnsResponse.AnswerRecords.Where(rec => rec is AaaaRecord))
					{
						// Get the IP address.
						IPAddress address = (record as AaaaRecord).Address;
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
						IPAddress address = (record as ARecord).Address;
						// Show the remote IP address.
						Program.WriteLine(ConsoleColor.Gray, "Remote address.........................", ConsoleColor.Cyan, address.ToString());
						// Add the address.
						remoteAddresses.Add(address);
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
			Program.WriteLine(ConsoleColor.Gray, "Attempts per flow......................", ConsoleColor.Cyan, this.settings.AttemptsPerFlow.ToString());
			Program.WriteLine(ConsoleColor.Gray, "Flow count.............................", ConsoleColor.Cyan, this.settings.FlowCount.ToString());
			Program.WriteLine(ConsoleColor.Gray, "Maximum hops...........................", ConsoleColor.Cyan, this.settings.MaximumHops.ToString());
			Program.WriteLine(ConsoleColor.Gray, "Maximum unknown hops...................", ConsoleColor.Cyan, this.settings.MaximumUnknownHops.ToString());
			Program.WriteLine(ConsoleColor.Gray, "Hop timeout (ms).......................", ConsoleColor.Cyan, this.settings.HopTimeout.ToString());
			Program.WriteLine(ConsoleColor.Gray, "Data length (bytes)....................", ConsoleColor.Cyan, this.settings.DataLength.ToString());

			// Run the traceroute for each remote address.
			foreach (IPAddress remoteAddress in remoteAddresses)
			{
				this.Run(localAddress, remoteAddress);
			}
		}

		/// <summary>
		/// Runs the traceroute for the specified IP address.
		/// </summary>
		/// <param name="localAddress">The local IP address.</param>
		/// <param name="remoteAddress">The remote IP address.</param>
		private void Run(IPAddress localAddress, IPAddress remoteAddress)
		{
			// Show the destination.
			Console.WriteLine();
			Program.WriteLine(ConsoleColor.Gray, "Traceroute ", ConsoleColor.Green, "{0} <--> {1}", localAddress, remoteAddress);

			// Create a cancellation token source.
			using (CancellationTokenSource cancel = new CancellationTokenSource())
			{
				try
				{
					// Run an ICMP traceroute.
					this.traceroute.RunIpv4(localAddress, remoteAddress, cancel.Token, null);
				}
				catch (Exception exception)
				{
					Program.WriteLine(ConsoleColor.Red, "ERROR  {0}", exception.Message);
					return;
				}
			}

			//// Compute the protocol type.
			//ProtocolType protocolType;
			//switch (address.AddressFamily)
			//{
			//	case AddressFamily.InterNetwork: protocolType = ProtocolType.Icmp; break;
			//	default: throw new NotSupportedException(string.Format("The IP address family {0} for address {0} is not supported.", address.AddressFamily, address));
			//}

			//// Create a receive socket.
			//using (Socket socketReceive = new Socket(address.AddressFamily, SocketType.Raw, protocolType))
			//{
			//	// socketReceive.Bind();

			//	// Create a send socket.
			//	using (Socket socketSend = new Socket(address.AddressFamily, SocketType.Raw, protocolType))
			//	{
			//		//socketSend.Send(new byte[] { 0x00, 0x00, 0x00 });
			//	}
			//}
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
				case "-6": this.flagIpv6 = true; break;
				case "-a": this.flagResolveAs = true; break;
				case "-c": this.settings.AttemptsPerFlow = int.Parse(args[++argumentIndex]); break;
				case "-d": this.dnsServer = IPAddress.Parse(args[++argumentIndex]); break;
				case "-f": this.settings.FlowCount = int.Parse(args[++argumentIndex]); break;
				case "-h": this.settings.MaximumHops = int.Parse(args[++argumentIndex]); break;
				case "-i": this.iface = int.Parse(args[++argumentIndex]); break;
				case "-m": this.settings.MaximumUnknownHops = int.Parse(args[++argumentIndex]); break;
				case "-o": this.settings.HopTimeout = int.Parse(args[++argumentIndex]); break;
				case "-s": this.settings.DataLength = int.Parse(args[++argumentIndex]); break;
				case "-t": this.dnsClient.QueryTimeout = int.Parse(args[++argumentIndex]); break;
				case "-u": this.flagUseUdp = true; break;
				default: throw new ArgumentException("Option {0} unknown.".FormatWith(args[argumentIndex]));
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
		}

		#endregion
	}
}
