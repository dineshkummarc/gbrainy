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

using Cairo;
using Mono.Unix;
using System;

public class PuzzleLines : Game
{
	private const int max_types = 3;
	private int type;
	private int fig1, fig2;

	public override string Name {
		get {return Catalog.GetString ("Lines");}
	}

	public override string Question {
		get {return Catalog.GetString ("How many lines do you count of in the figures below? Consider a line a segment between two points with no crossing lines.");} 
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += String.Format (Catalog.GetString ("There are {0} lines in the figure to the left and {1} in the figure to the right."), fig1, fig2);
			return answer;
		}
	}

	public override string Tip {
		get { return Catalog.GetString ("It is an easy exercise if you systematically count the lines.");}
	}

	public override void Initialize ()
	{
		if (CurrentDifficulty==Difficulty.Easy)
			type = 0;
		else
			type = random.Next (max_types);		

		switch (type) {
		case 0:
			fig1 = 15;
			fig2 = 21;
			break;
		case 1:
			fig1 = 33;
			fig2 = 39;
			break;
		case 2:
			fig1 = 15;
			fig2 = 39;
			break;
		}

		right_answer = (fig1 + fig2).ToString ();
	}

	static private void DrawLine (CairoContextEx gr, double x, double y, double w, double h)
	{
		gr.MoveTo (x, y);
		gr.LineTo (x + w, y + h);
		gr.Stroke ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		base.Draw (gr, area_width, area_height);
	
		DrawLine (gr, 0.1, 0.2, 0.4, 0.38);
		DrawLine (gr, 0.1, 0.3, 0.4, 0.3);
		DrawLine (gr, 0.1, 0.4, 0.4, 0.25);

		if (type == 1)  {
			DrawLine (gr, 0.6, 0.3, -0.2, 0.35);
			DrawLine (gr, 0.5, 0.25, -0.2, 0.35);		
		}

		DrawLine (gr, 0.1, 0.25, 0.6, 0.1);
		DrawLine (gr, 0.25, 0.2, 0, 0.4);

		if (type == 2 || type == 1)  {
			DrawLine (gr, 0.85, 0.25, -0.2, 0.4);
			DrawLine (gr, 0.88, 0.25, -0.2, 0.4);
		}

		DrawLine (gr, 0.91, 0.25, -0.2, 0.4);
		DrawLine (gr, 0.8, 0.2, 0, 0.4);
		DrawLine (gr, 0.82, 0.2, 0, 0.4);
		DrawLine (gr, 0.6, 0.50, 0.25, 0);
		DrawLine (gr, 0.6, 0.53, 0.25, 0);
	}
}


