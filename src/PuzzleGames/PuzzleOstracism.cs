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

public class PuzzleOstracism : Game
{
	private ArrayListIndicesRandom random_indices;
	private const int wrong_answer = 2;
	private string [] equations = new string []
	{
		"21 x 60 = 1260",
		"15 x 93 = 1395",
		"70 x 16 = 1120",
		"43 x 51 = 1453",
		"80 x 16 = 1806",
	};

	public override string Name {
		get {return Catalog.GetString ("Ostracism");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which equation does not belong to the group? Answer A, B, C, D or E.");} 
	}


	public override string Tip {
		get { return Catalog.GetString ("The criteria for deciding if an equation belongs to the group is not arithmetical.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("In all equations the digits from the left side should also appear in the right side.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		random_indices = new ArrayListIndicesRandom (equations.Length);
		random_indices.Initialize ();
		right_answer = string.Empty;

		for (int i = 0; i < random_indices.Count; i++)
		{
			if (random_indices[i] == wrong_answer) {
				right_answer += ((char) (65 + i));
				break;
			}
		}
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.15, y = DrawAreaY + 0.2;

		base.Draw (gr, area_width, area_height);

		gr.SetPangoLargeFontSize ();		
		for (int i = 0; i < random_indices.Count; i++)
		{
			gr.MoveTo (x, y);
			gr.ShowPangoText (((char)( 65 + i)) + ") " +  equations [random_indices[i]]);
			y += 0.1;
		}
	}
}


