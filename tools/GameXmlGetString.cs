/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using Mono.Unix;

// This is an auto-generated file GameXmlToGetString tool. Do not edit manually
public class GameXmlSttringFactory
{
	void LoadStrings ()
	{
		int variable = 0;
		Catalog.GetPluralString ("How many degrees rotates the minute hand of a clock in 2 hours [num] minute?",
			"How many degrees rotates the minute hand of a clock in 2 hours [num] minutes?",
			variable);

		Catalog.GetPluralString ("John is 46 years old. His son is [difference] year younger than half of John's age. How old is John's son?",
			"John is 46 years old. His son is [difference] years younger than half of John's age. How old is John's son?",
			variable);

		Catalog.GetPluralString ("John's age is nowadays 2 times his son's age. [ago] year ago, John was [proportion] times older than his son. How old is John's son nowadays?",
			"John's age is nowadays 2 times his son's age. [ago] years ago, John was [proportion] times older than his son. How old is John's son nowadays?",
			variable);

		Catalog.GetPluralString ("[ago] year ago, John's age minus [ago] was equal to [proportion] times his son's age minus [ago].",
			"[ago] years ago, John's age minus [ago] was equal to [proportion] times his son's age minus [ago].",
			variable);

		Catalog.GetPluralString ("A file is protected by a password formed by a [digits] digit number represented in base 10 (ranging from 0 to 9). How many different passwords can you have?",
			"A file is protected by a password formed by a [digits] digits number represented in base 10 (ranging from 0 to 9). How many different passwords can you have?",
			variable);

		Catalog.GetPluralString ("A file is protected by a password formed by a [digits] digit represented in base 8 (ranging from 0 to 7). How many different passwords can you have?",
			"A file is protected by a password formed by a [digits] digits represented in base 8 (ranging from 0 to 7). How many different passwords can you have?",
			variable);

		Catalog.GetPluralString ("There is [games] tennis game played simultaneously. How many different forecasts are possible?",
			"There are [games] tennis games played simultaneously. How many different forecasts are possible?",
			variable);

		Catalog.GetPluralString ("How many matches does it take to determine the winner of a tennis tournament that starts with [players] player?",
			"How many matches does it take to determine the winner of a tennis tournament that starts with [players] players?",
			variable);

		Catalog.GetPluralString ("You have [money] monetary unit in your bank account at 10% compound interest annually. How much money will you have at end of 2 years?",
			"You have [money] monetary units in your bank account at 10% compound interest annually. How much money will you have at end of 2 years?",
			variable);

		Catalog.GetPluralString ("In a horse race there are people and horses. You can count [eyes] eye and [legs] leg. How many horses are present?",
			"In a horse race there are people and horses. You can count [eyes] eyes and [legs] legs. How many horses are present?",
			variable);

		Catalog.GetPluralString ("There is [men] person and [horses] horse.",
			"There are [men] people and [horses] horses.",
			variable);

		Catalog.GetPluralString ("John needs [john_time] hour to clean a warehouse and his friend half of the time. How many hours would take them to clean up the warehouse if they work together?",
			"John needs [john_time] hours to clean a warehouse and his friend half of the time. How many hours would take them to clean up the warehouse if they work together?",
			variable);

		Catalog.GetPluralString ("John needs [john_time] hour to clean a warehouse and his friend two times John's time. How many hours would take them to clean up the warehouse if they work together?",
			"John needs [john_time] hours to clean a warehouse and his friend two times John's time. How many hours would take them to clean up the warehouse if they work together?",
			variable);


	}
}


