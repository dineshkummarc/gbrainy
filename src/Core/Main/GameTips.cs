/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Core.Main
{
	static public class GameTips
	{
		static ArrayListIndicesRandom random_indices;
		static int idx = 0;
	
		static public int Count {
			get { return 11; }
		}

		// Gets a random tip from the list
		static public string Tip {
			get {
				if (idx + 1 >= Count || random_indices == null) {
					random_indices = new ArrayListIndicesRandom (Count);
					random_indices.Initialize ();
					idx = 0;
				}

				return GetTip (idx++); 
			}

		}

		static public string GetTip (int tip)
		{
			switch (tip) {
			case 0:
				return Catalog.GetString ("Read the instructions carefully and identify the data and given clues.");
			case 1:
				return Catalog.GetString ("To score the player gbrainy uses the time and tips needed to complete each game.");
			case 2:
				return Catalog.GetString ("In logic games, elements that may seem irrelevant can be very important.");
			case 3:
				return Catalog.GetString ("Break the mental blocks and look into the boundaries of problems.");
			case 4:
				return Catalog.GetString ("Enjoy making mistakes, they are part of the learning process.");
			case 5:
				return Catalog.GetString ("Do all the problems, even the difficult ones. Improvement comes from practising.");
			case 6:
				return Catalog.GetString ("Play on a daily basis, you will notice progress soon.");
			case 7: // Translators: Custom Game Selection is a menu option
				return Catalog.GetString ("Use the 'Custom Game Selection' to choose exactly which games you want to play.");
			case 8:
				return Catalog.GetString ("Use the Settings to adjust the difficulty level of the game.");
			case 9:
				return Catalog.GetString ("Association of elements is a common technique for remembering things.");
			case 10:
				return Catalog.GetString ("Grouping elements into categories is a common technique for remembering things.");
			}

			return string.Empty;
		}
	}
}
