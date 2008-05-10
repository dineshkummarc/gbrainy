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

public class PuzzleMissingPiece : Game
{

	private ArrayListIndicesRandom random_indices;
	private const double sub_figure = 0.15;

	public override string Name {
		get {return Catalog.GetString ("Missing piece");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which square completes the figure below (A, B or C)?");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("The logic works at row level.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("In every row the third square is made by flipping the first square and superimposing it on the second square, followed by removing the matching lines.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		random_indices = new ArrayListIndicesRandom (3);
		random_indices.Initialize ();

		for (int i = 0; i < random_indices.Count; i++) {
			if (random_indices [i] == 0) {
				right_answer += (char) (65 + i);
				break;
			}
		}
	}

	private void DrawFigureSequence (CairoContextEx gr, double x, double y, int sequence, bool last_block)
	{
		gr.Rectangle (x, y, sub_figure, sub_figure);
		gr.Rectangle (x + sub_figure, y, sub_figure, sub_figure);

		if (last_block)
			gr.Rectangle (x + sub_figure * 2, y, sub_figure, sub_figure);

		switch (sequence) {
		case 0:
			gr.MoveTo (x, y + sub_figure);
			gr.LineTo (x + sub_figure, y);
			gr.MoveTo (x, y);
			gr.LineTo (x + sub_figure, y + sub_figure);
			x+= sub_figure;
			gr.MoveTo (x, y);
			gr.LineTo (x + sub_figure, y  + sub_figure);
			x+= sub_figure;
			gr.MoveTo (x, y + sub_figure);
			gr.LineTo (x + sub_figure, y);
			break;
		case 1:
			gr.MoveTo (x + sub_figure, y);
			gr.LineTo (x, y + sub_figure);
			gr.MoveTo (x + sub_figure / 2, y + sub_figure);
			gr.LineTo (x + sub_figure, y + sub_figure / 2);
			x+= sub_figure;
			gr.MoveTo (x, y + sub_figure / 2);
			gr.LineTo (x + sub_figure / 2, y + sub_figure);
			x+= sub_figure;
			gr.MoveTo (x, y);
			gr.LineTo (x + sub_figure, y + sub_figure);
			break;
		case 2:
			gr.MoveTo (x + sub_figure / 2, y);
			gr.LineTo (x + sub_figure, y + sub_figure / 2);
			gr.MoveTo (x, y + sub_figure);
			gr.LineTo (x + sub_figure / 2, y + sub_figure / 2);
			gr.LineTo (x + sub_figure, y + sub_figure);
			x+= sub_figure;
			gr.MoveTo (x, y + sub_figure / 2);
			gr.LineTo (x + sub_figure / 2, y);
			break;
		}

		gr.Stroke ();
	}

	private void DrawAnswerFigures (CairoContextEx gr, double x, double y, int figure)
	{
		gr.Rectangle (x, y, sub_figure, sub_figure);

		switch (figure) {
		case 0:
			gr.MoveTo (x, y + sub_figure);
			gr.LineTo (x + sub_figure / 2, y + sub_figure / 2);
			gr.LineTo (x + sub_figure, y + sub_figure);
			break;
		case 1:
			gr.MoveTo (x, y + sub_figure);
			gr.LineTo (x + sub_figure, y);
			break;
		case 2:
			gr.MoveTo (x, y);
			gr.LineTo (x + sub_figure, y + sub_figure);
			break;
		}
		gr.Stroke ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.15, y = DrawAreaY;
		int figure;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);
		
		for (int i = 0; i < 2; i++)
			DrawFigureSequence (gr, x, y + sub_figure * i , i, true);

		DrawFigureSequence (gr, x, y + sub_figure * 2 , 2, false);

		gr.MoveTo (0.1, 0.66);
		gr.ShowText (Catalog.GetString ("Possible answers are:"));

		x = DrawAreaX + 0.1;
		for (int i = 0; i < random_indices.Count; i++) {
			figure = random_indices [i];
			DrawAnswerFigures (gr, x + (0.08 + sub_figure) * i, 0.70, figure);
			gr.MoveTo (x + (0.08 + sub_figure) * i, 0.9);
			gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), (char) (65 + i)));
		}
	}
}

