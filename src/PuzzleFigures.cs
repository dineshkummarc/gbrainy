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

public class PuzzleFigures : Game
{
	private int [] figures  = new int []
	{
		0, 0, 1, 1, 2, 2,
		1, 2, 2, 0, 1, 0,
		2, 1, 0, 2, 0, 1
	};

	private ArrayListIndicesRandom random_indices;
	private const double figure_width = 0.1, figure_height = 0.1, space_width = 0.05, space_height = 0;

	public override string Name {
		get {return Catalog.GetString ("Figures");}
	}

	public override string Question {
		get {return Catalog.GetString ("What is the next logical sequence of objects?");} 
	}

	public override void Initialize ()
	{
		random_indices = new ArrayListIndicesRandom (6);
		random_indices.Initialize ();

		StringBuilder sb = new StringBuilder (3);
		
		sb.Append ((char) (65 + figures[(int) random_indices [5]]));
		sb.Append ((char) (65 + figures[6 + (int) random_indices [5]]));
		sb.Append ((char) (65 + figures[(2 * 6) + (int) random_indices [5]]));

		right_answer = sb.ToString ();
	}

	private void AnswerCoding (Cairo.Context gr, double x, double y)
	{
		double pos_x = x;

		gr.MoveTo (pos_x, y);
		y += 0.05;
		gr.ShowText (Catalog.GetString ("Convention when giving the answer is:"));

		gr.MoveTo (pos_x, y + 0.05);
		gr.ShowText ("A ->");
		gr.Stroke ();
		DrawingHelpers.DrawDiamond (gr, x + 0.1, y, 0.1);
		gr.Stroke ();
	
		pos_x += 0.3;
		gr.MoveTo (pos_x, y + 0.05);
		gr.ShowText ("B ->");
		gr.Stroke ();
		pos_x += 0.1;
		gr.Arc (pos_x + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);	
		gr.Stroke ();

		pos_x += 0.2;
		gr.MoveTo (pos_x, y + 0.05);
		gr.ShowText ("C ->");
		gr.Stroke ();
		pos_x += 0.1;
		DrawingHelpers.DrawEquilateralTriangle (gr, pos_x, y, 0.1);
		gr.Stroke ();

		y += 0.18;
		gr.MoveTo (x, y);		
		gr.ShowText (Catalog.GetString ("E.g: ACB (diamond, circle, triangle)"));	
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{		
		int element;
		double figure_width = 0.1, figure_height = 0.1, space_width = 0.05, space_height = 0.1;
		double x = DrawAreaX, y = DrawAreaY;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		for (int i = 0; i < (DrawAnswer ? 6 : 5) ; i++)
		{
			element = (int) random_indices [i];
			y = DrawAreaY;
			for (int n = 0; n < 3; n++) 
			{
				switch ((int) figures[(n * 6) + element])
				{
					case 0:
						DrawingHelpers.DrawDiamond (gr, x, y, 0.1);
						break;
					case 1:
						gr.Arc (x + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);	
						break;
					case 2:
						DrawingHelpers.DrawEquilateralTriangle (gr, x, y, 0.1);
						break;
					default:
						break;
				}
				gr.Stroke ();
				y+= figure_height + space_height;		
			}
			x+= figure_width + space_width;
		}

		if (DrawAnswer == false) {
			y = DrawAreaY + 0.08;
			gr.Save ();
			gr.SetFontSize (0.1);
			for (int n = 0; n < 3; n++) {
				gr.MoveTo (x, y);
				gr.ShowText ("?");
				gr.Stroke ();
				y+= figure_height + space_height;		
			}
			gr.Restore ();	
			y -= 0.08;
		}

		AnswerCoding (gr, DrawAreaX, y);
	}

}


