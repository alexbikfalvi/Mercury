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
using System.Net;
using System.Net.NetworkInformation;
using Mercury.Net.Information;
using Mercury.Properties;

using System.Net.Sockets;
using Mercury.Net.Core;

namespace Mercury
{
	/// <summary>
	/// A class representing the main program of the Mercury traceroute.
	/// </summary>
	public sealed class Program : IDisposable
	{
		private static readonly List<IPUnicastAddressInformation> localAddresses = new List<IPUnicastAddressInformation>();

		private string destination = null;
		private IPUnicastAddressInformation localAddress = null;
		private MultipathTracerouteSettings settings;

		/// <summary>
		/// Creates a new program instance.
		/// </summary>
		/// <param name="args"></param>
		public Program(string[] args)
		{
			// Write the program information.
			Program.WriteLine(ConsoleColor.White, Resources.Title);

			// Update the list of local IP addresses.
			Program.UpdateLocalAddresses();

			// Create the traceroute settings.
			this.settings = new MultipathTracerouteSettings();

			// Parse the arguments.
			for (int argumentIndex = 0, parameterIndex = 0; argumentIndex < args.Length; argumentIndex++)
			{
				this.ParseArgument(args, ref argumentIndex, ref parameterIndex);
			}

			// If not all parameters are set, throw an exception.
			if (null == this.localAddress) throw new ArgumentException("The local address is missing.");
			if (null == this.destination) throw new ArgumentException("The destination is missing.");
		}

		// Public methods.

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
				// Show the list of local IP addresses.
				Program.ShowLocalAddresses();
				// Show the error.
				Program.WriteLine(ConsoleColor.Red, "ERROR  ", ConsoleColor.Gray, exception.Message);
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

		// Private methods.

		/// <summary>
		/// Runs the current traceroute.
		/// </summary>
		private void Run()
		{
			// Show the destination.
			Program.WriteLine(ConsoleColor.Gray, "Destination: ", ConsoleColor.Cyan, "{0}", this.destination);
			// Show the local address.
			Program.WriteLine(ConsoleColor.Gray, "Source address: ", ConsoleColor.Cyan, "{0}", this.localAddress.Address);

			// Resolve the remote IP address.
			IPAddress[] remoteAddresses = Dns.GetHostAddresses(this.destination);

			// Show the remote IP addresses.
			Program.Write(ConsoleColor.Gray, "Remote addresses:");
			foreach (IPAddress address in remoteAddresses)
			{
				Program.Write(ConsoleColor.Cyan, "  {0}", address);
			}
			Console.WriteLine();

			// Run the traceroute.
			foreach (IPAddress address in remoteAddresses)
			{
				try
				{
					this.Run(address);
				}
				catch (Exception exception)
				{
					Program.WriteLine(ConsoleColor.Red, "ERROR  ", ConsoleColor.Gray, exception.Message);
				}
			}
		}

		/// <summary>
		/// Runs the traceroute for the specified IP address.
		/// </summary>
		/// <param name="address">The destination address.</param>
		private void Run(IPAddress address)
		{
			// Show the destination.
			Program.WriteLine(ConsoleColor.Gray, "Traceroute to: ", ConsoleColor.Yellow, address.ToString());

			// Compute the protocol type.
			ProtocolType protocolType;
			switch (address.AddressFamily)
			{
				case AddressFamily.InterNetwork: protocolType = ProtocolType.Icmp; break;
				default: throw new NotSupportedException(string.Format("The IP address family {0} for address {0} is not supported.", address.AddressFamily, address));
			}

			// Create a receive socket.
			using (Socket socketReceive = new Socket(address.AddressFamily, SocketType.Raw, protocolType))
			{
				// socketReceive.Bind();

				// Create a send socket.
				using (Socket socketSend = new Socket(address.AddressFamily, SocketType.Raw, protocolType))
				{
					//socketSend.Send(new byte[] { 0x00, 0x00, 0x00 });
				}
			}
		}

		/// <summary>
		/// Parses the argument from the list at the specified index.
		/// </summary>
		/// <param name="args">The list of arguments.</param>
		/// <param name="argumentIndex">The argument index.</param>
		/// <param name="parameterIndex">The parameter index.</param>
		private void ParseArgument(string[] args, ref int argumentIndex, ref int parameterIndex)
		{
			// Parse the argument.
			switch (args[argumentIndex].Trim())
			{
				default:
					switch(parameterIndex++)
					{
						case 0: this.localAddress = Program.localAddresses[int.Parse(args[argumentIndex])]; break;
						case 1: this.destination = args[argumentIndex]; break;
						default: throw new ArgumentException(string.Format("Unknown argument: {0}", args[argumentIndex]));
					}
					break;
			}
		}

		/// <summary>
		/// Updates the information on the local IP addresses.
		/// </summary>
		private static void UpdateLocalAddresses()
		{
			// Clear the current addresses list.
			Program.localAddresses.Clear();

			// For each network interface.
			foreach (NetworkInterface iface in NetworkInterface.GetAllNetworkInterfaces())
			{
				// Skip the interfaces that are not up.
				if (iface.OperationalStatus != OperationalStatus.Up) continue;

				// Get the IP properties of the interface.
				IPInterfaceProperties properties = iface.GetIPProperties();

				// If the interface does not have a DNS server address.
				if (properties.DnsAddresses.Count == 0) continue;

				// For all unicast IP addresses of this interface.
				foreach (UnicastIPAddressInformation info in properties.UnicastAddresses)
				{
					// If the IP address is IPv4 or IPv6.
					if ((info.Address.AddressFamily == AddressFamily.InterNetwork) || (info.Address.AddressFamily == AddressFamily.InterNetworkV6))
					{
						// Add a new unicast address information instance to the local addresses list.
						Program.localAddresses.Add(new IPUnicastAddressInformation(iface, info));
					}
				}
			}
		}

		/// <summary>
		/// Shows the information on the local IP addresses.
		/// </summary>
		private static void ShowLocalAddresses()
		{
			// Show the local IP addresses.
			Program.WriteLine(ConsoleColor.Gray, "Local addresses:");

			// For all the local IP addresses.
			int addressIndex = 0;
			Program.WriteLine(ConsoleColor.Gray, "{0,4} | {1,-40} | {2,-6} | {3,-30} | {4,-15} | {5,-10} | {6,-5} | {7, -20}", "#", "Address", "Family", "Interface", "Type", "Speed", "Flags", "DNS");
			foreach (IPUnicastAddressInformation info in Program.localAddresses)
			{
				Program.Write(ConsoleColor.Cyan, "{0,4}", addressIndex++);
				Program.Write(ConsoleColor.Gray, " | ", ConsoleColor.Yellow, "{0,-40}", info.Address);
				Program.Write(ConsoleColor.Gray, " | ", ConsoleColor.Cyan, "{0,-6}", (info.Address.AddressFamily == AddressFamily.InterNetwork) ? "IPv4" : "IPv6");
				Program.Write(ConsoleColor.Gray, " | ", ConsoleColor.Cyan, "{0,-30}", info.Interface.Name);
				Program.Write(ConsoleColor.Gray, " | ", ConsoleColor.Cyan, "{0,-15}", info.Interface.NetworkInterfaceType.ToString());
				Program.Write(ConsoleColor.Gray, " | ", ConsoleColor.Cyan, "{0,-10}", info.Interface.Speed);
				Program.Write(ConsoleColor.Gray, " | ", ConsoleColor.Cyan, "{0,-5}", 
					string.Format("{0}{1}{2}",
					info.Address.IsIPv6LinkLocal ? "L" : string.Empty,
					info.Address.IsIPv6Multicast ? "M" : string.Empty,
					info.Address.IsIPv6SiteLocal ? "S" : string.Empty)
					);

				// Show the DNS addresses.
				IPAddressCollection dnsAddresses = info.Interface.GetIPProperties().DnsAddresses;
				Program.WriteLine(ConsoleColor.Gray, " | ", ConsoleColor.Cyan, "{0,-20}", dnsAddresses[0]);
				for (int dnsIndex = 1; dnsIndex < dnsAddresses.Count; dnsIndex++)
				{
					Program.Write(ConsoleColor.Gray, "{0,4} | {0,-40} | {0,-6} | {0,-30} | {0,-15} | {0,-10} | {0,-5} | ", string.Empty);
					Program.WriteLine(ConsoleColor.Cyan, "{0,-20}", dnsAddresses[dnsIndex]);
				}
			}
			Console.WriteLine();
			Program.Write(ConsoleColor.Gray, "  Flags: ");
			Program.Write(ConsoleColor.Cyan, "L", ConsoleColor.Gray, " Link-local  ");
			Program.Write(ConsoleColor.Cyan, "M", ConsoleColor.Gray, " Multicast  ");
			Program.Write(ConsoleColor.Cyan, "S", ConsoleColor.Gray, " Site-local  ");
			Console.WriteLine();
			Console.WriteLine();
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
	}
}
