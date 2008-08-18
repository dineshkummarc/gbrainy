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

public class PuzzleBuildTriangle : Game
{
	public enum Figures
	{
		TriangleA,
		TriangleB,
		TriangleC,
		Square,
		Diamon,
		LongRectangle,
		LongRectangleUp,
		TriangleD,
	}

	private const double figure_size = 0.1;
	private ArrayListIndicesRandom random_indices_answers;
	private string [] answers;
	private const int answer_num = 3;
	private int total_figures;
	private double space_figures;
	private double radian = Math.PI / 180;

	public override string Name {
		get {return Catalog.GetString ("Build a triangle");}
	}

	public override string Question {
		get {return String.Format (
			Catalog.GetString ("Which three pieces can you use together to build a triangle? Answer using the three figure names, e.g.: {0}{1}{2}."),
				GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2));}
	}

	public override string Tip {
		get { return Catalog.GetString ("The resulting triangle is isosceles.");}
	}

	public override void Initialize ()
	{
		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			total_figures = 6;
			space_figures = 0.26;
			break;
		case Difficulty.Medium:
		case Difficulty.Master:
			total_figures = 8;
			space_figures = 0.2;
			break;
		}

		random_indices_answers = new ArrayListIndicesRandom (total_figures);
		random_indices_answers.Initialize ();
		answers = new string[answer_num];

		for (int i = 0; i < random_indices_answers.Count; i++)
		{
			switch ((Figures) random_indices_answers[i]) {
			case Figures.TriangleB:
				answers[0] =  GetPossibleAnswer (i);
				break;
			case Figures.TriangleC:
				answers[1] =  GetPossibleAnswer (i);
				break;
			case Figures.Square:
				answers[2] =  GetPossibleAnswer (i);
				break;
			}
		}

		right_answer = answers[0] + answers[1] + answers[2];
	}

	private void DrawFigure (CairoContextEx gr, double x, double y, Figures figure)
	{
		switch (figure) {
		case Figures.TriangleA:
			gr.DrawEquilateralTriangle (x, y, figure_size);
			break;
		case Figures.TriangleB:
			gr.MoveTo (x, y);
			gr.LineTo (x, y + figure_size);
			gr.LineTo (x + figure_size, y);
			gr.LineTo (x, y);
			gr.Stroke ();
			break;
		case Figures.TriangleC:
			gr.MoveTo (x, y);
			gr.LineTo (x, y + figure_size);
			gr.LineTo (x + figure_size, y + figure_size);
			gr.LineTo (x, y);
			gr.Stroke ();
			break;
		case Figures.Square:
			gr.Rectangle (x, y, figure_size, figure_size);
			gr.Stroke ();
			break;
		case Figures.LongRectangle:
			gr.Rectangle (x, y + figure_size / 2, figure_size, figure_size / 2);
			gr.Stroke ();
			break;
		case Figures.LongRectangleUp:
			gr.Rectangle (x, y, figure_size, figure_size / 2);
			gr.Stroke ();
			break;
		case Figures.Diamon:
			gr.DrawDiamond (x, y, figure_size);
			break;
		case Figures.TriangleD:
			gr.MoveTo (x, y);
			gr.LineTo (x, y + figure_size * 0.7);
			gr.LineTo (x + figure_size * 0.7, y + figure_size * 0.7);
			gr.LineTo (x, y);
			gr.Stroke ();
			break;
		}
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.05, y = DrawAreaY + 0.1;
		double degrees, x1, x2, dist;

		base.Draw (gr, area_width, area_height);

		for (int i = 0; i < random_indices_answers.Count; i++)	
		{
			DrawFigure (gr, x, y, (Figures) random_indices_answers[i]);
			gr.MoveTo (x, y + 0.13);
			gr.ShowPangoText (GetPossibleFigureAnswer (i));

			if (i  == (total_figures / 2) - 1) {
				y+= 0.25;
				x= DrawAreaX + 0.05;
			}
			else
				x+= space_figures;
		}

		if (DrawAnswer == false)
			return;

		gr.MoveTo (DrawAreaX, y + 0.28);
		gr.ShowPangoText (Catalog.GetString ("The triangle is:"));
		gr.Stroke ();
		
		x = DrawAreaX + 0.35;
		y += 0.35;

		degrees = radian * 45;	// First triangle
		gr.MoveTo (x, y);
		x1 = x + figure_size * Math.Cos (degrees);
		gr.LineTo (x1, y + figure_size * Math.Sin (degrees));

		degrees = radian * (135);
		x2 = x + figure_size * Math.Cos (degrees);
		gr.MoveTo (x, y);
		gr.LineTo (x2, y + figure_size * Math.Sin (degrees));
		gr.LineTo (x1, y + figure_size * Math.Sin (degrees));
		dist = (x1 - x2);
		x += dist;

		degrees = radian * 45; // Second triangle
		gr.MoveTo (x, y);
		x1 = x + figure_size * Math.Cos (degrees);
		gr.LineTo (x1, y + figure_size * Math.Sin (degrees));

		degrees = radian * (135);
		x2 = x + figure_size * Math.Cos (degrees);
		gr.MoveTo (x, y);
		gr.LineTo (x2, y + figure_size * Math.Sin (degrees));
		gr.LineTo (x1, y + figure_size * Math.Sin (degrees));

		degrees = radian * (-45); // Bottom
		x =  DrawAreaX + 0.35;
		gr.MoveTo (x, y);
		gr.LineTo (x + figure_size * Math.Cos (degrees), y + figure_size * Math.Sin (degrees));

		x += dist;
		degrees = radian * (-135);
		gr.MoveTo (x, y);
		gr.LineTo (x + figure_size * Math.Cos (degrees), y + figure_size * Math.Sin (degrees));
		gr.Stroke ();	
	}

	public override bool CheckAnswer (string a)
	{	
		int matches = 0;

		a = TrimAnswer (a.ToUpper ());

		for (int i = 0; i < answer_num; i++) {
			answers [i] = answers [i].ToUpper ();
	
			if (a.Contains (answers[i]))
				matches++;
		}

		if (matches == answer_num)
			return true;

		return false;
	}
}


