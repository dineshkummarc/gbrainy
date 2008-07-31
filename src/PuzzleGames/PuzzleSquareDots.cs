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

public class PuzzleSquareDots : Game
{
	private const double figure_size = 0.25; 
	private const int lines = 6;
	private const int columns = 6;
	private const int figures = 6;
	private static bool X = true;
	private static bool O = false;
	private const double space_figures = 0.05;
	private ArrayListIndicesRandom possible_answers;

	private bool [] puzzle_A  = new bool []
	{
		// Figure A
		O, O, O, O, O, O,
		O, O, O, O, O, O, 	
		O, O, X, X, O, O,	// Down, Diagonal down left
		O, O, X, X, O, O,	// Up, Diagonal up left
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		
		// Figure B
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, O, X, O, O, O,
		O, O, X, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,

		// Figure C
		O, O, O, O, O, O,
		O, X, X, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, X, X, O, O, O,
		O, O, O, O, O, O,

		// Wrong answer 1
		O, X, X, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, X, X, O, O, O,

		// Correct Answer
		X, O, X, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		X, O, X, O, O, O,

		// Wrong answer 2
		O, O, O, O, O, O,
		O, X, X, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, O, O, O, O, O,
		O, X, X, O, O, O,
	};

	public override string Name {
		get {return Catalog.GetString ("Square with dots");}
	}

	public override string Question {
		get {return Catalog.GetString ("What is the letter of the figure that represents the next logical figure in the sequence? Answer A, B or C.");} 
	}

	public override void Initialize ()
	{
		possible_answers = new ArrayListIndicesRandom (3);
		possible_answers.Initialize ();

		for (int i = 0; i < possible_answers.Count; i++) {
			if (possible_answers[i] == 0) {
				right_answer += (char) (65 + (int) i);
				break;
			}
		}
	}

	public void DrawFigure (CairoContextEx gr, double x, double y, bool [] puzzle, int index)
	{
		double pos_x = x, pos_y = y;
		double square_size = figure_size / lines;
		double center_square = square_size / 2;
		double radius_square = (square_size - (LineWidth *2)) / 2;

		gr.Rectangle (pos_x, pos_y, figure_size, figure_size);
		gr.Stroke ();

		for (int line = 0; line < lines - 1; line++) // Horizontal
		{
			pos_y += square_size;
			gr.MoveTo (pos_x, pos_y);
			gr.LineTo (pos_x + figure_size, pos_y);
			gr.Stroke ();
		}

		pos_y = y;
		for (int column = 0; column < columns - 1; column++) // Vertical
		{
			pos_x += square_size;
			gr.MoveTo (pos_x, pos_y);
			gr.LineTo (pos_x, pos_y + figure_size);
			gr.Stroke ();
		}

		pos_y = y + center_square;
		pos_x = x + center_square;
		
		for (int line = 0; line < lines; line++) // Circles
		{	
			for (int column = 0; column < columns; column++)
			{
				if (puzzle[index + (columns * line) + column] == false)
					continue;

				gr.Arc (pos_x + (square_size * column), pos_y, radius_square, 0, 2 * Math.PI);
				gr.Stroke ();
			}
			pos_y += square_size;
		}
	}

	public void DrawPossibleAnswer (CairoContextEx gr, double x, double y, int figure)
	{
		switch (figure) {
		case 0: // Good answer
			DrawFigure (gr, x, y, puzzle_A, columns * lines * 4);
			break;
		case 1:
			DrawFigure (gr, x, y, puzzle_A,  columns * lines * 3);
			break;
		case 2:
			DrawFigure (gr, x, y, puzzle_A, columns * lines * 5);
			break;
		}
	}
	
	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX, y = DrawAreaY;

		base.Draw (gr, area_width, area_height);

		DrawFigure (gr, x, y, puzzle_A, 0);
		DrawFigure (gr, x + figure_size + space_figures, y, puzzle_A, columns * lines);
		DrawFigure (gr, x + (figure_size + space_figures) * 2, y, puzzle_A, columns * lines * 2);
	
		y += figure_size + 0.10;
		gr.MoveTo (x, y - 0.02);
		gr.ShowPangoText (Catalog.GetString ("Possible answers are:"));
		gr.Stroke ();
		y += 0.05;

		for (int i = 0; i < possible_answers.Count; i++) {
			DrawPossibleAnswer (gr, x, y, possible_answers[i]);
			gr.MoveTo (x, y + figure_size + 0.05);
			gr.ShowPangoText (String.Format (Catalog.GetString ("Figure {0}"), (char) (65 + i)));
			gr.Stroke ();
			x+= figure_size + space_figures;
		}
	}
}


