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
using System.Text;
using Mono.Unix;

public class PuzzleBalance : Game
{
	private const int elements = 5;
	private int [] balances = new int []
	{
		2,3,2,0,0,	1,3,1,1,1,
		3,3,1,0,0,	2,2,2,1,0,
		2,2,3,0,0,	3,2,1,1,0,
		1,2,2,0,0,	3,1,1,0,0,
		2,2,2,2,0,	3,1,1,3,0,
	};

	private ArrayListIndicesRandom random_indices;
	private const double figure_width = 0.1, figure_height = 0.1, space_width = 0.05, space_height = 0;

	public override string Name {
		get {return Catalog.GetString ("Balance");}
	}

	public override string Question {
		get {return Catalog.GetString ("How many triangles are needed in the right part of the last figure to keep it balanced?");} 
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("Every triangle counts as 1, each diamond as a 2 and each square as 3.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		int ans = 0;
		random_indices = new ArrayListIndicesRandom (5);
		random_indices.Initialize ();

		for (int i = 0; i < elements; i++)	
			ans += balances [(((int) (random_indices[2])) * elements * 2) + i];

		right_answer = ans.ToString ();
	}

	private void DrawTriangle (Cairo.Context gr, double x, double y, double size)
	{
		gr.MoveTo (x, y);
		gr.LineTo (x - (size / 2), y + size);
		gr.LineTo (x + (size / 2), y + size);
		gr.LineTo (x, y);
		gr.Stroke ();
	}

	private void DrawDiamond (Cairo.Context gr, double x, double y, double size)
	{
		gr.MoveTo (x, y);
		gr.LineTo (x - (size / 2), y + (size / 2));
		gr.LineTo (x, y + size);
		gr.LineTo (x + size / 2, y + (size / 2));
		gr.LineTo (x, y);
	}

	public void DrawBalance (Cairo.Context gr, double x, double y, int index, bool full)
	{
		double width = 0.5;
		double fig_x = x + 0.1, fig_y = y - 0.11;
		int total = (full == true) ? (elements * 2) : elements; 

		gr.Rectangle (x + 0.05, y - 0.12, 0.38, 0.08);
		gr.Stroke ();

		gr.Rectangle (x + 0.5, y - 0.12, 0.38, 0.08);
		gr.Stroke ();

		for (int i = 0; i < total; i++) {
			switch (balances[i + index * elements * 2]) {
			case 1:
				DrawTriangle (gr, fig_x, fig_y, 0.05);
				break;
			case 2:
				DrawDiamond (gr, fig_x, fig_y, 0.05);
				break;
			case 3:
				gr.Rectangle (fig_x - 0.02, fig_y + 0.005, 0.045, 0.045);
				break;
			}
			
			if (i == elements - 1)
				fig_x = x + 0.54;
			else
				fig_x += 0.07;
		}

		x += 0.2;
		y += 0.01;
		gr.MoveTo (x, y);
		gr.LineTo (x + width, y);
		gr.LineTo (x + width, y - 0.05);
		gr.Stroke ();
		
		gr.MoveTo (x , y);
		gr.LineTo (x , y - 0.05);
		gr.Stroke ();
		
		DrawTriangle (gr, x + (width / 2), y, 0.08);
		gr.Stroke ();

	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{		
		double x = 0.05, y = DrawAreaY + 0.1;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		DrawBalance (gr, x, y, (int) random_indices [0], true);
		y += 0.3;
		
		DrawBalance (gr, x, y, (int) random_indices [1], true);
		y += 0.3;

		DrawBalance (gr, x, y, (int) random_indices [2], false);
	}

}

