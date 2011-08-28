/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	public class GameTips
	{
		ITranslations translations;
		ArrayListIndicesRandom random_indices;
		int idx = 0;

		public GameTips (ITranslations translations)
		{
			this.translations = translations;
		}
	
		public int Count {
			get { return 15; }
		}

		// Gets a random tip from the list
		public string Tip {
			get {
				if (idx + 1 >= Count || random_indices == null) {
					random_indices = new ArrayListIndicesRandom (Count);
					random_indices.Initialize ();
					idx = 0;
				}
				return GetTip (random_indices [idx++]);
			}
		}

		public string GetTip (int tip)
		{
			switch (tip) {
			case 0:
				return translations.GetString ("Read the instructions carefully and identify the data and given clues.");
			case 1:
				return translations.GetString ("To score the player gbrainy uses the time and tips needed to complete each game.");
			case 2:
				return translations.GetString ("In logic games, elements that may seem irrelevant can be very important.");
			case 3:
				return translations.GetString ("Try to approach a problem from different angles.");
			case 4:
				return translations.GetString ("Do not be afraid of making mistakes, they are part of the learning process.");
			case 5:
				return translations.GetString ("Do all the problems, even the difficult ones. Improvement comes from challeging yourself.");
			case 6:
				return translations.GetString ("Play on a daily basis, you will notice progress soon.");
			case 7: // Translators: Custom Game Selection is a menu option
				return translations.GetString ("Use the 'Custom Game Selection' to choose exactly which games you want to play.");
			case 8:
				return translations.GetString ("Use the Settings to adjust the difficulty level of the game.");
			case 9:
				return translations.GetString ("Association of elements is a common technique for remembering things.");
			case 10:
				return translations.GetString ("Grouping elements into categories is a common technique for remembering things.");
			case 11:
				return translations.GetString ("Build acronyms using the first letter of each fact to be remembered.");
			case 12:
				return translations.GetString ("The enjoyment obtained from a puzzle is proportional to the time spent on it.");
			case 13:
				return translations.GetString ("Think of breaking down every problem into simpler components.");
			case 14:
				return translations.GetString ("When answering verbal analogies pay attention to the verb tense.");
			default:
				throw new InvalidOperationException ();
			}
		}
	}
}
