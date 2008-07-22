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

public class PuzzleLargerShape : Game
{
	private const double rect_witdh = 0.04, rect_height = 0.04, space_figures = 0.22;
	private ArrayListIndicesRandom random_indices;
	private const int answers = 4;
	private char[] quest1, quest2, answer;
	private int ranswer;
	private ColorPalette palette;

	private static char A = 'A';
	private static char B = 'B';
	private static char X = 'X'; // Transparent

	/* Game 1 */
	private int g1rightanswer = 0;
	private char [] g1question_A  = new char []
	{
		A, B,
		X, X,
	};

	private char [] g1question_B  = new char []
	{
		B, A, X,
		A, X, X,
		X, X, X,
	};

	private char [] g1answer  = new char []
	{
		// Figure A
		B, A, X,
		A, X, X,
		X, B, A,
		// Figure B
		A, X, X,
		X, B, A,
		X, A, B,
		// Figure C
		A, B, X,
		B, X, X,
		X, B, A,

		// Figure D
		X, X, X,
		A, B, A,
		X, A, B,
	};

	/* Game 2 */	
	private int g2rightanswer = 0;
	private char [] g2question_A  = new char []
	{
		B, A,
		A, B,
	};

	private char [] g2question_B  = new char []
	{
		A, A, X,
		A, X, X,
		X, X, X,
	};

	private char [] g2answer  = new char []
	{
		// Figure A
		A, A, X,
		A, A, B,
		X, B, A,
		// Figure B
		A, X, X,
		X, B, A,
		X, A, A,
		// Figure C
		A, B, X,
		B, A, B,
		X, B, A,

		// Figure D
		X, X, X,
		A, A, A,
		X, A, B,
	};

	public override string Name {
		get {return Catalog.GetString ("Larger shape");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which larger shape can you make combining the first two figures?");} 
	}

	public override void Initialize ()
	{
		palette = new ColorPalette (ColorPalette.Id.PrimaryColors);
		palette.Initialize ();

		switch (random.Next (2)) {
		case 0:
			quest1 = g1question_A;
			quest2 = g1question_B;
			answer = g1answer;
			ranswer = g1rightanswer;
			break;
		case 1:
			quest1 = g2question_A;
			quest2 = g2question_B;
			answer = g2answer;
			ranswer = g2rightanswer;
			break;
		}

		random_indices = new ArrayListIndicesRandom (answers);
		random_indices.Initialize ();

		for (int i = 0; i < answers; i++)
		{
			if ((int) random_indices[i] == ranswer) {
				right_answer += (char) (65 + i);
				break;
			}
		}
	}

	private Color ColorForPortion (char portion)
	{
		if (portion == A)
			return palette.Cairo (1);

		return palette.Cairo (0);
	}

	private void DrawSquare (CairoContextEx gr, double x, double y)
	{
		// XX
		// XX
		for (int i = 0; i < 2; i++) {
			if (quest1 [i] != X) {
				gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);	
				gr.FillGradient (x + i * rect_witdh, y, rect_witdh, rect_height, ColorForPortion (quest1 [i]));
			}
			gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
			gr.Stroke ();

			if (quest1 [i + 2] != X) {
				gr.Rectangle (x + i * rect_witdh, y + rect_height, rect_witdh, rect_height);
				gr.FillGradient (x + i * rect_witdh, y + rect_height, rect_witdh, rect_height, ColorForPortion (quest1 [i + 2]));
			}
			gr.Rectangle (x + i * rect_witdh, y + rect_height, rect_witdh, rect_height);
			gr.Stroke ();
		}
	}

	private void DrawLShape (CairoContextEx gr, double x, double y)
	{
		// XXX  
		// X
		// X
		for (int i = 0; i < 3; i++) { // XXXX
			if (quest2 [i] != X) {
				gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);	
				gr.FillGradient (x + i * rect_witdh, y, rect_witdh, rect_height, ColorForPortion (quest2 [i]));
			}
			gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
			gr.Stroke ();
		}

		for (int i = 0; i < 2; i++) {
			if (quest2 [(i + 1) * 3] != X) {
				gr.Rectangle (x, y + rect_height * (i + 1), rect_witdh, rect_height);
				gr.FillGradient (x, y + rect_height * (i + 1), rect_witdh, rect_height, ColorForPortion (quest2 [(i + 1) * 3]));
			}
			gr.Rectangle (x, y + rect_height * (i + 1), rect_witdh, rect_height);
			gr.Stroke ();
		}
	}

	private void DrawPossibleAnswer (CairoContextEx gr, double x, double y, char [] portions, int figure, int seq)
	{
		int columns = 3, rows = 3;
		double rect_w = 0.05, rect_h = 0.05;
		int index = figure * columns * rows;

		for (int row = 0; row < rows; row++) {
			for (int column = 0; column < columns; column++) {
				if (portions [index + column + (3 * row)] != X) {
					gr.Rectangle (x + column * rect_w, y + row * rect_h, rect_w, rect_h);
					gr.FillGradient (x + column * rect_w, y + row * rect_h, rect_w, rect_h, ColorForPortion (portions [index + column + (3 * row)]));
				}
				gr.Rectangle (x + column * rect_w, y + row * rect_h, rect_w, rect_h);
				gr.Stroke ();
			}
		}

		gr.MoveTo (x, y + 0.2);
		gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), (char) (65 + seq)));
		gr.Stroke ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.1, y = DrawAreaY;

		base.Draw (gr, area_width, area_height);
	
		DrawSquare (gr, x, y);
		DrawLShape (gr, x + 0.4, y);

		gr.MoveTo (0.1, 0.35);
		gr.ShowText (Catalog.GetString ("Possible answers are:"));
		gr.Stroke ();
		
		DrawPossibleAnswer (gr, x, y + 0.32, answer, random_indices [0], 0);
		DrawPossibleAnswer (gr, x + 0.4, y + 0.32, answer, random_indices [1], 1);
		DrawPossibleAnswer (gr, x, y + 0.6, answer, random_indices [2], 2);
		DrawPossibleAnswer (gr, x + 0.4, y + 0.6, answer, random_indices [3], 3);
	}
}
