/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using Cairo;
using Mono.Unix;
using System;

public class PuzzleHandshakes : Game
{
	int people, handshakes;

	public override string Name {
		get {return Catalog.GetString ("Handshakes");}
	}

	public override string Question {
		get {return String.Format (
			Catalog.GetString ("In a party all the people is introduced to the others. There are {0} handeshakes in total. How many people is in the party?"), 				handshakes);
		}
	}

	public override string Tip {
		get { return Catalog.GetString ("Try to imagine a situation where you were meeting a smaller number of people.");}
	}

	public override void Initialize ()
	{
		handshakes = 0;

		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			people = 4 + random.Next (4);
			break;
		case Difficulty.Master:
			people = 5 + random.Next (8);
			break;		
		case Difficulty.Medium:
		default:
			people = 5 + random.Next (4);
			break;		
		}
		
		for (int i = 1; i < people; i++)
			handshakes += i;
		
		right_answer = people.ToString ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		base.Draw (gr, area_width, area_height);
	}
}
