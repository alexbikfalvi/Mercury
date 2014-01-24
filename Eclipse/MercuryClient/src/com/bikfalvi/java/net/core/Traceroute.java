/* 
 * Copyright (C) 2013-2014 Manuel Plancin, Alex Bikfalvi
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

package com.bikfalvi.java.net.core;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.bikfalvi.java.threading.CancellationToken;

/**
 * A class representing a traceroute for the Internet Protocol.
 */
public final class Traceroute
{
	private final TracerouteSettings settings;

	/**
	 * Creates a new traceroute instance.
	 * @param settings The traceroute settings.
	 */
	public Traceroute(TracerouteSettings settings)
	{
		this.settings = settings;
	}

	/**
	 * Runs a traceroute to the specified destination.
	 * @param destination The destination.
	 * @param cancellationToken The cancellation token.
	 * @return The r-esult of the traceroute operation.
	 * @throws IOException 
	 * @throws InterruptedException 
	 */
	public TracerouteResult run(InetAddress destination, CancellationToken cancellationToken) throws IOException, InterruptedException
	{
		// Create the traceroute command.
		String osName = System.getProperty("os.name").toLowerCase();
		String[] command;
		if (osName.indexOf("windows") != -1) {
			// Windows.
			command = new String[] { "tracert", "-d", "-w", Integer.toString(this.settings.getTimeout()), "-h", Byte.toString(this.settings.getMaximumHops()), destination.getHostAddress() }; 
		} else if (osName.indexOf("mac os x") != -1) {
			// Mac OS.
			command = new String[] { "traceroute", "-n", "-w", Integer.toString((int)Math.ceil(this.settings.getTimeout() / 1000.0)), "-m", Byte.toString(this.settings.getMaximumHops()), destination.getHostAddress() };
		} else {
			// Other.
			command = new String[] { "traceroute", "-n", "-w", Integer.toString((int)Math.ceil(this.settings.getTimeout() / 1000.0)), "-m", Byte.toString(this.settings.getMaximumHops()), destination.getHostAddress() }; 
		}
		
		// Create a process builder.
		final ProcessBuilder builder = new ProcessBuilder(command);
		final Process process = builder.start();
		
		final InputStream stream = process.getInputStream();
		final InputStreamReader reader = new InputStreamReader(stream);
		final BufferedReader bufferedReader = new BufferedReader(reader);
		final ArrayList<String> lines = new ArrayList<String>();
		
		// Read the lines from the process output.
		for (String line = null; (line = bufferedReader.readLine()) != null; ) {
			
			// If the operation is cancelled.
			if (cancellationToken.isCanceled()) {
				// Kill the process.
				process.destroy();
				// Return null.
				return null;
			}
			
			// Add the process output line.
			lines.add(line);
		}
		
		// Wait for the process to complete.
		int code = process.waitFor();

		// Create the traceroute result.
		TracerouteResult result = new TracerouteResult(destination);
		
		// Patterns
		final String patternHop = "(^\\s+\\d{1,2}|^\\d{1,2})";
		final String patternIp = "([0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3})";

		Matcher matcherReply;
		
		// If the command was successfull.
		if (code == 0) {
			// Parse all lines.
			for (String line : lines) {
				// If the line matches a hop.
				if ((matcherReply = Pattern.compile(patternHop).matcher(line)).find()) {
					try {
						// Parse the TTL.
						int ttl = Integer.parseInt(matcherReply.group(0).trim());
						InetAddress address = null; 
						
						try {
							// Parse the address.
							if ((matcherReply = Pattern.compile(patternIp).matcher(line)).find()) {
								address = InetAddress.getByName(matcherReply.group(0));
							}
						}
						catch (UnknownHostException e) {							
						}
						
						// Add the hop to the traceroute result.
						result.add(new TracerouteHop(ttl, address));
					} catch(NumberFormatException e) {
					}
				}
			}
		}

		// Return the result.
		return result;
	}
}
