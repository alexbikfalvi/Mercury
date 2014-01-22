/** 
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

package com.bikfalvi.java.threading;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/**
 * A class representing a cached thread pool.
 * @author Alex Bikfalvi
 *
 */
public final class ThreadPool {
	private static ExecutorService threadPool;
	
	/**
	 * Initializes the static field.
	 */
	static {
		// Create the thread pool.
		ThreadPool.threadPool = Executors.newCachedThreadPool();
	}

	/**
	 * Executes the specified task on the thread pool.
	 * @param task
	 */
	public static void execute(Runnable task) {
		ThreadPool.threadPool.execute(task);
	}
}
