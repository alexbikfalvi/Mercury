/* 
 * Copyright (C) 2013 Alex Bikfalvi
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
using System.Net.NetworkInformation;
using Mercury.Properties;

using System.Net;
using System.Net.Sockets;
using Mercury.Net.Core;

namespace Mercury
{
	/// <summary>
	/// A class representing the main program of the Mercury traceroute.
	/// </summary>
	public sealed class Program : IDisposable
	{
		private string destination = null;
		private MultipathTracerouteSettings settings;

		/// <summary>
		/// Creates a new program instance.
		/// </summary>
		/// <param name="args"></param>
		public Program(string[] args)
		{
			// Write the program information.
			Program.WriteLine(ConsoleColor.White, Resources.Title);

			// Create the traceroute settings.
			this.settings = new MultipathTracerouteSettings();

			// Parse the arguments.
			for (int index = 0; index < args.Length; index++)
			{
				this.ParseArgument(args, ref index);
			}

			// Show the syntax.
			Program.WriteLine(ConsoleColor.Gray, Resources.Syntax);
			// Show the interfaces.
			Program.ListInterfaces();

			// If the destination is not set, throw an exception.
			if (null == destination) throw new ArgumentException("Invalid syntax");
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
			Program.WriteLine(ConsoleColor.Gray, "Destination:");
			Program.WriteLine(ConsoleColor.Cyan, "  {0}", this.destination);

			// Resolve the destination IP address.
			IPAddress[] addresses = Dns.GetHostAddresses(this.destination);

			// Show the destination IP addresses.
			Program.WriteLine(ConsoleColor.Gray, "Addresses:");
			foreach (IPAddress address in addresses)
			{
				Program.WriteLine(ConsoleColor.Cyan, "  {0}", address);
			}

			// Run the traceroute.
			foreach (IPAddress address in addresses)
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
		/// <param name="index">The argument index.</param>
		private void ParseArgument(string[] args, ref int index)
		{
			// Parse the argument.
			switch (args[index].Trim())
			{
				case "-A": break;
				default:
					this.destination = args[index];
					break;
			}
		}

		/// <summary>
		/// Shows the information on the local IP interfaces.
		/// </summary>
		private static void ListInterfaces()
		{
			// Show the local IP addresses.
			Program.WriteLine(ConsoleColor.Gray, "Interfaces:");
			foreach (NetworkInterface iface in NetworkInterface.GetAllNetworkInterfaces())
			{
				Program.WriteLine(ConsoleColor.White, "  {0}", iface.Name);
				Program.WriteLine(ConsoleColor.Gray, "    Type: ", ConsoleColor.Cyan, iface.NetworkInterfaceType.ToString());
				Program.WriteLine(ConsoleColor.Gray, "    Status: ", ConsoleColor.Cyan, iface.OperationalStatus.ToString());
				Program.WriteLine(ConsoleColor.Gray, "    Speed: ", ConsoleColor.Cyan, iface.Speed.ToString());

				// Get the IP properties of the interface.
				IPInterfaceProperties properties = iface.GetIPProperties();

				// 
				foreach (UnicastIPAddressInformation info in properties.UnicastAddresses)
				{
					switch (info.Address.AddressFamily)
					{
						case AddressFamily.InterNetwork:
							Program.Write(ConsoleColor.Yellow, "      IPv4", ConsoleColor.Cyan, "  {0}", info.Address);
							Program.WriteLine(ConsoleColor.White, " / ", ConsoleColor.Cyan, "{0}", info.IPv4Mask);
							break;
						case AddressFamily.InterNetworkV6:
							Program.Write(ConsoleColor.Yellow, "      IPv6", ConsoleColor.Cyan, "  {0}", info.Address);
							Program.WriteLine(ConsoleColor.Yellow, "  {0}  {1}  {2}",
								info.Address.IsIPv6LinkLocal ? "Link-local" : string.Empty,
								info.Address.IsIPv6Multicast ? "Multicast" : string.Empty,
								info.Address.IsIPv6SiteLocal ? "Site-local" : string.Empty);
							break;
					}
				}
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
	}
}
