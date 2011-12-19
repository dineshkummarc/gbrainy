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

using System.Collections.Generic;

using gbrainy.Core.Main;

namespace gbrainy.Games.Memory
{
	public class MemoryWords : Core.Main.Memory
	{
		private ArrayListIndicesRandom words_order;
		private List <string> words;
		private const int total_words = 35;
		private int showed;
		private int answer;

		public override string Name {
			get {return Translations.GetString ("Memorize words");}
		}

		public override string MemoryQuestion {
			get { 
				return Translations.GetString ("There is a missing word from the previous list. Which one is the missing word?");}
		}

		protected override void Initialize ()
		{
			int tmp;
			words = new List <string> (total_words);

			// Body parts
			words.Add (Translations.GetString ("wrist"));
			words.Add (Translations.GetString ("elbow"));
			words.Add (Translations.GetString ("armpit"));
			words.Add (Translations.GetString ("hand"));
			words.Add (Translations.GetString ("chest"));
			
			//Fishes
			words.Add (Translations.GetString ("sardine"));
			words.Add (Translations.GetString ("trout"));
			words.Add (Translations.GetString ("monkfish"));
			words.Add (Translations.GetString ("cod"));
			words.Add (Translations.GetString ("salmon"));

			// Vegetables
			words.Add (Translations.GetString ("potato"));
			words.Add (Translations.GetString ("ginger"));			
			words.Add (Translations.GetString ("pepper"));
			words.Add (Translations.GetString ("garlic"));
			words.Add (Translations.GetString ("pumpkin"));

			// Bicycle
			words.Add (Translations.GetString ("brake"));
			words.Add (Translations.GetString ("pedal"));
			words.Add (Translations.GetString ("chain"));			
			words.Add (Translations.GetString ("wheel"));
			words.Add (Translations.GetString ("handlebar"));

			// Music
			words.Add (Translations.GetString ("drummer"));
			words.Add (Translations.GetString ("speaker"));
			words.Add (Translations.GetString ("lyrics"));
			words.Add (Translations.GetString ("beat"));			
			words.Add (Translations.GetString ("song"));

			// Weather
			words.Add (Translations.GetString ("cloud"));
			words.Add (Translations.GetString ("rain"));
			words.Add (Translations.GetString ("storm"));
			words.Add (Translations.GetString ("fog"));
			words.Add (Translations.GetString ("rainbow"));

			// Animals
			words.Add (Translations.GetString ("rabbit"));
			words.Add (Translations.GetString ("mouse"));
			words.Add (Translations.GetString ("monkey"));
			words.Add (Translations.GetString ("bear"));
			words.Add (Translations.GetString ("wolf"));

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				showed = 6;
				break;
			case GameDifficulty.Medium:
				showed = 9;
				break;
			case GameDifficulty.Master:
				showed = 12;
				break;
			}

			words_order = new ArrayListIndicesRandom (total_words);
			words_order.Initialize ();
			answer = random.Next (showed);
			tmp = words_order [answer];
			Answer.Correct = words [tmp];
			base.Initialize ();
		}
	
		public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x= DrawAreaX + 0.125, y = DrawAreaY + 0.1;
			int cnt = 0;

			for (int i = 0; i < showed; i++)
			{
				if (i == answer)
					continue;

				gr.MoveTo (x, y);
				gr.ShowPangoText (words[words_order[i]]);
				gr.Stroke ();

				if ((cnt + 1) % 3 == 0) {
					y += 0.2;
					x = DrawAreaX + 0.125;
				} else {
					x+= 0.25;
				}
				cnt++;
			}
		}
	
		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			DrawObject (gr, area_width, area_height);
		}
	
		private void DrawObject (CairoContextEx gr, int area_width, int area_height)
		{
			double x= DrawAreaX + 0.125, y = DrawAreaY + 0.1;
			for (int i = 0; i < showed; i++)
			{
				gr.MoveTo (x, y);
				gr.ShowPangoText (words[words_order[i]]);
				gr.Stroke ();
			
				if ((i + 1) % 3 == 0) {
					y += 0.2;
					x = DrawAreaX + 0.125;
				} else {
					x+= 0.25;
				}
			}
		}
	}
}
