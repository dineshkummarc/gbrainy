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

public class PuzzleFigurePattern : Game
{
	private ArrayListIndicesRandom random_indices;

	enum Figures
	{
		TwoLines = 0,
		Cross,
		RotatedCross,
		Last
	};

	public override string Name {
		get {return Catalog.GetString ("Figure pattern");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which figure should replace the question mark (A, B or C)?");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("The third figure of every row involves combining somehow the first two figures.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("Superpose the first and second figures and remove the lines that they have in common then rotate the resulting figure 45 degrees.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		random_indices = new ArrayListIndicesRandom ((int) Figures.Last);
		random_indices.Initialize ();

		for (int i = 0; i < (int) Figures.Last; i++)
		{
			if ((int) random_indices[i] == (int) Figures.Cross) {
				right_answer += (char) (65 + i);
				break;
			}
		}
	}

	private void DrawRotatedCross (Cairo.Context gr, double x, double y, double size)
	{
		gr.MoveTo (x, y);
		gr.LineTo (x + size, y + size);
		gr.MoveTo (x + size, y);
		gr.LineTo (x, y + size);
		gr.Stroke ();
	}

	private void DrawTwoLines (Cairo.Context gr, double x, double y, double size)
	{
		gr.MoveTo (x, y);
		gr.LineTo (x + size, y);
		gr.MoveTo (x, y + size);
		gr.LineTo (x + size, y + size);
		gr.Stroke ();
	}

	private void DrawCross (Cairo.Context gr, double x, double y, double size)
	{
		gr.MoveTo (x + size / 2, y);
		gr.LineTo (x + size / 2, y + size);
		gr.MoveTo (x, y + size / 2);
		gr.LineTo (x + size, y + size / 2);
		gr.Stroke ();
	}
	
	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{
		double org_x = DrawAreaX + 0.1;
		double x = org_x, y = 0.08;
		double figure_size = 0.13, space_x = 0.1, space_y = 0.2;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		// First pattern
		gr.Rectangle (x, y, figure_size, figure_size);
		gr.MoveTo (x, y);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.MoveTo (x + figure_size, y);
		gr.LineTo (x, y + figure_size);
		gr.MoveTo (x + figure_size / 2, y);
		gr.LineTo (x + figure_size / 2, y + figure_size);
		gr.MoveTo (x, y + figure_size / 2);
		gr.LineTo (x + figure_size, y + figure_size / 2);
		gr.Stroke ();

		x += figure_size + space_x;
		gr.Rectangle (x, y, figure_size, figure_size);
		gr.MoveTo (x + figure_size / 2, y);
		gr.LineTo (x + figure_size / 2, y + figure_size);
		gr.MoveTo (x, y + figure_size / 2);
		gr.LineTo (x + figure_size, y + figure_size / 2);
		gr.Stroke ();

		x += figure_size + space_x;
		DrawCross (gr, x, y, figure_size);

		y += space_y;
		x = org_x;
		// Second pattern
		gr.Rectangle (x, y, figure_size, figure_size);
		gr.MoveTo (x, y);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.MoveTo (x + figure_size, y);
		gr.LineTo (x, y + figure_size);
		gr.Stroke ();

		x += figure_size + space_x;
		DrawRotatedCross (gr, x, y, figure_size);

		x += figure_size + space_x;
		DrawingHelpers.DrawDiamond (gr, x, y, figure_size);

		y += space_y;
		x = org_x;

		// Third pattern
		gr.MoveTo (x, y);
		gr.LineTo (x + figure_size, y);
		gr.LineTo (x, y + figure_size);
		gr.LineTo (x + figure_size, y  + figure_size);
		gr.Stroke ();

		x += figure_size + space_x;
		gr.MoveTo (x + figure_size, y);
		gr.LineTo (x, y);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.LineTo (x, y  + figure_size);
		gr.Stroke ();
		
		x += figure_size + space_x;
		gr.Save ();
		gr.MoveTo (x + 0.03, y + 0.1);
		gr.SetFontSize (figure_size);	
		gr.ShowText ("?");
		gr.Stroke ();
		gr.Restore ();
	
		gr.MoveTo (0.05, y + 0.01 + space_y);
		gr.ShowText (Catalog.GetString ("Possible answers are:"));

		// Answers
		x = org_x;
		y += space_y + 0.07;

		for (int i = 0; i < (int) Figures.Last; i++)
		{
		 	switch ((Figures) random_indices[i]) {
			case Figures.TwoLines:
				DrawTwoLines (gr, x, y, figure_size);
				break;
			case Figures.Cross:
				DrawCross (gr, x, y, figure_size);
				break;
			case Figures.RotatedCross:
				DrawRotatedCross (gr, x, y, figure_size);
				break;
			}
			
			gr.MoveTo (x, y + 0.2);
			gr.ShowText (Catalog.GetString ("Figure") + " " + (char) (65 + i));

			x += figure_size + space_x;			
		}
	}
}


