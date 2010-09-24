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
			get {return Catalog.GetString ("Memorize words");}
		}

		public override string MemoryQuestion {
			get { 
				return Catalog.GetString ("There is a missing word from the previous list. Which one is the missing word?");}
		}

		protected override void Initialize ()
		{
			int tmp;
			words = new List <string> (total_words);

			// Body parts
			words.Add (Catalog.GetString ("wrist"));
			words.Add (Catalog.GetString ("elbow"));
			words.Add (Catalog.GetString ("armpit"));
			words.Add (Catalog.GetString ("hand"));
			words.Add (Catalog.GetString ("chest"));
			
			//Fishes
			words.Add (Catalog.GetString ("sardine"));
			words.Add (Catalog.GetString ("trout"));
			words.Add (Catalog.GetString ("monkfish"));
			words.Add (Catalog.GetString ("cod"));
			words.Add (Catalog.GetString ("salmon"));

			// Vegetables
			words.Add (Catalog.GetString ("potato"));
			words.Add (Catalog.GetString ("ginger"));			
			words.Add (Catalog.GetString ("pepper"));
			words.Add (Catalog.GetString ("garlic"));
			words.Add (Catalog.GetString ("pumpkin"));

			// Bicycle
			words.Add (Catalog.GetString ("brake"));
			words.Add (Catalog.GetString ("pedal"));
			words.Add (Catalog.GetString ("chain"));			
			words.Add (Catalog.GetString ("wheel"));
			words.Add (Catalog.GetString ("handlebar"));

			// Music
			words.Add (Catalog.GetString ("drummer"));
			words.Add (Catalog.GetString ("speaker"));
			words.Add (Catalog.GetString ("lyrics"));
			words.Add (Catalog.GetString ("beat"));			
			words.Add (Catalog.GetString ("song"));

			// Weather
			words.Add (Catalog.GetString ("cloud"));
			words.Add (Catalog.GetString ("rain"));
			words.Add (Catalog.GetString ("storm"));
			words.Add (Catalog.GetString ("fog"));
			words.Add (Catalog.GetString ("rainbow"));

			// Animals
			words.Add (Catalog.GetString ("rabbit"));
			words.Add (Catalog.GetString ("mouse"));
			words.Add (Catalog.GetString ("monkey"));
			words.Add (Catalog.GetString ("bear"));
			words.Add (Catalog.GetString ("wolf"));

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
			right_answer = words [tmp];
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
