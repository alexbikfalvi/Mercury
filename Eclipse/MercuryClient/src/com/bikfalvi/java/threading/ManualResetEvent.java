/** 
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

package com.bikfalvi.java.threading;

import java.util.concurrent.CountDownLatch;

/**
 * A class representing a manual reset event.
 * @author Alex
 *
 */
public class ManualResetEvent {
    private final Object sync = new Object();
    private volatile CountDownLatch latch;

    /**
     * Creates a new manual reset event instance.
     * @param state True if the event is in the signalled state, false otherwise.
     */
    public ManualResetEvent(boolean state) {
    	this.latch = state ? new CountDownLatch(0) : new CountDownLatch(1);
    }
    
    /**
     * Sets the event in the signaled state.
     */
    public void set() {
   		this.latch.countDown();
    }
    
    /**
     * Resets the event into the non-signaled state.
     */
    public void reset() {
    	synchronized (this.sync) {
    		if (0 == this.latch.getCount()) {
    			this.latch = new CountDownLatch(1);
    		}
    	}
    }
    
    /**
     * Blocks the current thread until the event is in the signaled state.
     * @throws InterruptedException
     */
    public void waitOne() throws InterruptedException {
   		this.latch.await();
    }
}
