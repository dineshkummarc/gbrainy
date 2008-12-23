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
using Cairo;
using Mono.Unix;
using System.Timers;
using System.Collections.Generic;

public class MemorySample : Memory
{
	ArrayListIndicesRandom animals_order;
	List <string> animals;
	int showed;
	int answer;

	public override string Name {
		get {return "Memory sample";}
	}

	public override string MemoryQuestion {
		get { 
			return "There is a missing animal name from the previous list. Which one is missing?";}
	}

	public override void Initialize ()
	{
		int tmp;
		animals = new List <string> ();

		animals.Add (Catalog.GetString ("dog"));
		animals.Add (Catalog.GetString ("cat"));
		animals.Add (Catalog.GetString ("rat"));
		animals.Add (Catalog.GetString ("bird"));
		animals.Add (Catalog.GetString ("sardine"));
		animals.Add (Catalog.GetString ("trout"));
		animals.Add (Catalog.GetString ("monkfish"));
		animals.Add (Catalog.GetString ("cod"));
		animals.Add (Catalog.GetString ("salmon"));

		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			showed = 4;
			break;
		case Difficulty.Medium:
			showed = 6;
			break;
		case Difficulty.Master:
			showed = 8;
			break;
		}

		animals_order = new ArrayListIndicesRandom (animals.Count);
		animals_order.Initialize ();
		answer = random.Next (showed);
		tmp = animals_order [answer];
		right_answer = animals [tmp];
		base.Initialize ();
	}
	
	public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height)
	{
		double x= DrawAreaX + 0.125, y = DrawAreaY + 0.1;
		int cnt = 0;

		for (int i = 0; i < showed; i++)
		{
			if (i == answer)
				continue;

			gr.MoveTo (x, y);
			gr.ShowPangoText (animals[animals_order[i]]);
			gr.Stroke ();

			if ((cnt + 1) % 3 == 0) {
				y += 0.2;
				x = DrawAreaX + 0.125;
			} else {
				x+= 0.25;
			}
			cnt++;
		}

		gr.Color = new Color (0.9, 0.9, 0.9);
		gr.DrawTextCentered (0.5, DrawAreaY, "This is an extension sample");
	}
	
	public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorize (gr, area_width, area_height);
		DrawObject (gr, area_width, area_height);
	}
	
	void DrawObject (CairoContextEx gr, int area_width, int area_height)
	{
		double x= DrawAreaX + 0.125, y = DrawAreaY + 0.1;
		for (int i = 0; i < showed; i++)
		{
			gr.MoveTo (x, y);
			gr.ShowPangoText (animals[animals_order[i]]);
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

