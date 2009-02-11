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

using System;
using Cairo;
using Mono.Unix;

public class PuzzleQuadrilaterals : Game
{
	enum Figures
	{
		FigureA,
		FigureB,
		FigureC,
		FigureD,
		FigureE,
		FigureF,
		Last
	};

	private ArrayListIndicesRandom random_indices;
	private const double figure_size = 0.15;

	public override string Name {
		get {return Catalog.GetString ("Quadrilaterals");}
	}

	public override string Question {
		get {return String.Format (
			Catalog.GetString ("Which of the following figures does not belong to the group? Answer {0}, {1}, {2}, {3}, {4} or {5}."),
				GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3), GetPossibleAnswer (4), 
				GetPossibleAnswer (5));}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("It is the only figure with all lines of equal length.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		random_indices = new ArrayListIndicesRandom ((int) Figures.Last);
		random_indices.Initialize ();

		for (int i = 0; i < (int) Figures.Last; i++)
		{
			if ((Figures) random_indices[i] == Figures.FigureA) {
				right_answer = GetPossibleAnswer (i);
				break;
			}
		}
	}

	static void DrawFigure (CairoContextEx gr, double x, double y, Figures figure)
	{
		switch (figure) {
		case Figures.FigureA:
			double x105, y105;

			x105 = figure_size * Math.Cos (105 * Math.PI / 180);
			y105 = figure_size * Math.Sin (105 * Math.PI / 180);
			gr.MoveTo (x, y);
			gr.LineTo (x + x105, y + y105);
			gr.LineTo (x + x105 + figure_size, y + y105);
			gr.Stroke ();
			gr.MoveTo (x + figure_size, y);
			gr.LineTo (x + figure_size + x105, y + y105);
			gr.Stroke ();
			gr.MoveTo (x, y);
			gr.LineTo (x + figure_size, y);
			break;

		case Figures.FigureB:
			gr.Rectangle (x, y, figure_size * 0.8, figure_size * 1.2);
			break;

		case Figures.FigureC:
			gr.MoveTo (x, y);
			gr.LineTo (x + figure_size * 1.3, y);
			gr.LineTo (x + figure_size * 1.3, y + figure_size);
			gr.LineTo (x , y + figure_size);
			gr.LineTo (x, y);
			break;

		case Figures.FigureD:
			gr.MoveTo (x + 0.03, y);
			gr.LineTo (x + figure_size - 0.03, y);
			gr.LineTo (x + figure_size, y + figure_size);
			gr.LineTo (x , y + figure_size);
			gr.LineTo (x + 0.03, y);
			break;

		case Figures.FigureE:
			gr.MoveTo (x + 0.03, y);
			gr.LineTo (x + figure_size - 0.04, y);
			gr.LineTo (x + figure_size - 0.04, y + figure_size);
			gr.LineTo (x , y + figure_size);
			gr.LineTo (x + 0.03, y);;
			break;

		case Figures.FigureF:
			gr.MoveTo (x, y);
			gr.LineTo (x, y + figure_size);
			gr.LineTo (x + figure_size, y + figure_size);
			gr.LineTo (x + figure_size - 0.02, y);
			gr.LineTo (x, y);
			break;
		}

		gr.Stroke ();

	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX, y = DrawAreaY, space_x = 0.15;

		base.Draw (gr, area_width, area_height);

		for (int i = 0; i < random_indices.Count; i++) {
			DrawFigure (gr, x, y, (Figures) random_indices[i]);
			gr.MoveTo (x, y - 0.02 + figure_size * 1.6);
			gr.ShowPangoText (GetPossibleFigureAnswer (i));

			if (i == 2) {
				x = DrawAreaX;
				y += figure_size * 3;
			} else 
				x += figure_size + space_x;
		}
	}
}


