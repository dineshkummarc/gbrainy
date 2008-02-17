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

public class PuzzleTetris : Game
{
	private ArrayListIndicesRandom random_indices_questions;
	private ArrayListIndicesRandom random_indices_answers;
	private const double rect_witdh = 0.04, rect_height = 0.04, space_figures = 0.22;

	public override string Name {
		get {return Catalog.GetString ("Tetris");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which is the figure that completes the sequence below (A, B or C)?");} 
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("It is the figure that completes all the possible combinations with four blocks without taking into account rotations.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		random_indices_questions = new ArrayListIndicesRandom (4);
		random_indices_questions.Initialize ();

		random_indices_answers = new ArrayListIndicesRandom (3);
		random_indices_answers.Initialize ();

		for (int i = 0; i < random_indices_answers.Count; i++) {
			if ((int) random_indices_answers [i] == 0) {
				right_answer += (char) (65 + i);
				break;
			}
		}
	}

	private void DrawQuestionFigures (CairoContextEx gr, double x, double y, int figure)
	{
		switch (figure) {
		case 0:
			// XX
			// XX
			for (int i = 0; i < 2; i++) {
				gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
				gr.Rectangle (x + i * rect_witdh, y - rect_height, rect_witdh, rect_height);
			}
			gr.Stroke ();
			break;
		case 1:
			//  X
			// XXX
			for (int i = 0; i < 3; i++) {
				gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
			}
			gr.Rectangle (x + 1 * rect_witdh, y - rect_height, rect_witdh, rect_height);
			gr.Stroke ();
			break;
		case 2:
			//   X
			// XXX
			for (int i = 0; i < 3; i++) {
				gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
			}
			gr.Rectangle (x + 2 * rect_witdh, y - rect_height, rect_witdh, rect_height);
			gr.Stroke ();
			break;
		case 3:
			// XXXX
			for (int i = 0; i < 4; i++) {
				gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
			}
			gr.Stroke ();
			break;
		}
	}

	private void DrawAnswerFigures (CairoContextEx gr, double x, double y, int figure)
	{
		switch (figure) {
		case 0:
			//  XX
			// XX
			for (int i = 0; i < 2; i++) {
				gr.Rectangle (x + i * rect_witdh, y, rect_witdh, rect_height);
				gr.Rectangle (x + rect_witdh + i * rect_witdh, y -  rect_height, rect_witdh, rect_height);
			}
			break;
		case 1:
			// X
			// X
			// X
			// X
			for (int i = 0; i < 4; i++) {
				gr.Rectangle (x, y -  rect_height * i, rect_witdh, rect_height);
			}
			break;
		case 2:
			// XXX
			//  X
			for (int i = 0; i < 3; i++) {
				gr.Rectangle (x + i * rect_witdh, y - rect_height, rect_witdh, rect_height);
			}
			gr.Rectangle (x + rect_witdh, y, rect_witdh, rect_height);
			break;
		}
		gr.Stroke ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX, y = DrawAreaY + 0.1;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);
		
		for (int i = 0; i < 4; i++) {
			DrawQuestionFigures (gr, x, y, (int) random_indices_questions [i]);
			x += space_figures;
		}

		gr.MoveTo (0.1, 0.4);
		gr.ShowText (Catalog.GetString ("Possible answers are:"));

		x = 0.2;
		y = 0.6;
		for (int i = 0; i < 3; i++) {
			DrawAnswerFigures (gr, x, y, (int) random_indices_answers [i]);
			gr.MoveTo (x, y + 0.15);
			gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), (char) (65 + i)));
			x += space_figures;
		}
	}
}


