/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;
using Mono.Unix;
using System.Timers;
using Gtk;
using System.Collections;

public class MemoryWords : Memory
{
	private ArrayListIndicesRandom words_order;
	private ArrayList words;
	private const int total_words = 35;
	private const int showed = 9;
	private int answer;

	public override string Name {
		get {return Catalog.GetString ("Coloured text");}
	}

	public override string Question {
		get {return Catalog.GetString ("Memorize all the words."); }
	}

	public override string MemoryQuestion {
		get { 
			return String.Format (Catalog.GetString ("There is a missing word from the previous list. Which one is the missing word?"));}
	}

	public override void Initialize ()
	{
		int tmp;
		words = new ArrayListIndicesRandom (total_words);

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

		words_order = new ArrayListIndicesRandom (total_words);
		words_order.Initialize ();
		answer = random.Next (showed);
		tmp = (int) words_order [answer];
		right_answer = (string) words [tmp];
		base.Initialize ();
	}
	
	public override void DrawPossibleAnswers (Cairo.Context gr, int area_width, int area_height)
	{
		double x= DrawAreaX + 0.1, y = DrawAreaY + 0.1;
		int cnt = 0;

		for (int i = 0; i < showed; i++)
		{
			if (i == answer)
				continue;

			gr.MoveTo (x, y);
			gr.ShowText ((string) words[(int)words_order[i]]);
			gr.Stroke ();

			if (cnt  == 2 || cnt == 5) {
				y += 0.2;
				x = DrawAreaX + 0.1;
			} else {
				x+= 0.2;
			}
			cnt++;
		}
		
	}	
	
	public override void DrawObjectToMemorize (Cairo.Context gr, int area_width, int area_height)
	{
		double x= DrawAreaX + 0.2, y = DrawAreaY + 0.2;
		base.DrawObjectToMemorize (gr, area_width, area_height);

		for (int i = 0; i < showed; i++)
		{
			gr.MoveTo (x, y);
			gr.ShowText ((string) words[(int)words_order[i]]);
			gr.Stroke ();
			
			if (i  == 2 || i == 5) {
				y += 0.2;
				x = DrawAreaX + 0.2;
			} else {
				x+= 0.2;
			}
		}
	}
}


