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

package com.bikfalvi.java.globalization;

/**
 * A class representing a culture.
 * @author Alex Bikfalvi
 *
 */
public class Culture {
	private final CultureId id;
	private final LanguageCollection languages;
	private final ScriptCollection scripts;
	private final TerritoryCollection territories;

	/**
	 * Creates a culture instance for the specified culture.
	 * @param id The culture.
	 */
	public Culture(CultureId id) {
		this.id = id;
		this.languages = new LanguageCollection(id);
		this.scripts = new ScriptCollection(id);
		this.territories = new TerritoryCollection(id);
	}

	/**
	 * Creates a culture instance for the specified culture.
	 * @param culture The culture.
	 * @param languages An iteration of languages.
	 * @param scripts An iteration of scripts.
	 * @param territories An iteration of territories.
	 */
	public Culture(CultureId culture, Iterable<Language> languages, Iterable<Script> scripts, Iterable<Territory> territories) {
		this.id = culture;
		this.languages = new LanguageCollection(culture, languages);
		this.scripts = new ScriptCollection(culture, scripts);
		this.territories = new TerritoryCollection(culture, territories);
	}

	/**
	 * Gets the culture identifier.
	 * @return The culture identifier.
	 */
	public CultureId getId() {
		return id;
	}

	/**
	 * Gets the languages.
	 * @return The languages.
	 */
	public LanguageCollection getLanguages() {
		return languages;
	}

	/**
	 * Gets the scripts.
	 * @return The scripts.
	 */
	public ScriptCollection getScripts() {
		return scripts;
	}

	/**
	 * Gets the territories.
	 * @return The territories.
	 */
	public TerritoryCollection getTerritories() {
		return territories;
	}
}

