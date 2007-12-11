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
using System.Timers;
using Gtk;
using System.Collections;

public class MemoryFigures : Memory
{

	private ArrayListIndicesRandom figures;
	private const int rows = 3;
	private const int columns = 4;
	private const double start_x = 0.25;
	private const double start_y = 0.1;
	private const double figure_size = 0.08;
	private double rect_w, rect_h;
	private int question_pos, question_answer;

	public enum FigureType
	{
		Triangle,
		Rectangle,
		Diamond,
		Cercle,
		TriangleWithLine,
		RectangleWithLine,
		//DiamondWithLine,
		//CercleWithLine,
		Length
	}

	public override string Name {
		get {return Catalog.GetString ("Memory figures");}
	}

	public override string Question {
		get {return Catalog.GetString ("Memorize in which position there is every figure"); }
	}

	public override string MemoryQuestion {
		get { 
			return Catalog.GetString ("In which cell is the other figure like the one shown below? (type the cell number)" );}
	}

	public override void Initialize ()
	{
		int fig1, fig2;
		rect_w = 0.6 / columns;
		rect_h = DrawAreaHeight / rows;
		figures = new ArrayListIndicesRandom ((int) FigureType.Length * 2);
		figures.Initialize ();
		question_pos = (int) random.Next ((int) FigureType.Length * 2);

		for (int figure = 0; figure < (int) FigureType.Length * 2; figure++)
		{	
			if (figure == question_pos)
				continue;
	
			fig1 = (int) figures[figure];
			fig2 = (int) figures[question_pos];

			if (fig1 >= (int) FigureType.Length) fig1 -= (int) FigureType.Length;
			if (fig2 >= (int) FigureType.Length) fig2 -= (int) FigureType.Length;

			if (fig1 == fig2) {
				question_answer = figure + 1;
				break;
			}
		}
		right_answer = question_answer.ToString ();
		base.Initialize ();
	}

	public override void DrawPossibleAnswers (Cairo.Context gr, int area_width, int area_height)
	{
		int col = 1, fig;
		double x = start_x, y = start_y;
		gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, 1);
		DrawGrid (gr, area_width, area_height, 1);

		if (DrawAnswer ==  true) {
			DrawAllFigures (gr, area_width, area_height, 1);
			return;
		}

		SetLargeFont (gr);
		for (int figure = 0; figure < figures.Count; figure++, col++)
		{
			fig = (int)figures[figure];
			if (fig >= (int) FigureType.Length) fig -= (int) FigureType.Length;

			if (figure == question_pos)
				DrawFigure (gr, x, y, alpha, (FigureType) fig);
			else {
				gr.MoveTo (x + 0.04, y + 0.1);
				gr.ShowText ((figure + 1).ToString ());
				gr.Stroke ();
			}

			if (col >= columns) {
				col = 0;
				y += rect_h;
				x = start_x;
			} else {
				x += rect_w;
			}
		}
	}

	public override void DrawObjectToMemorizeFading (Cairo.Context gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorizeFading (gr, area_width, area_height);
		DrawGrid (gr, area_width, area_height, alpha);
		DrawAllFigures (gr, area_width, area_height, alpha);
	}
	
	public override void DrawObjectToMemorize (Cairo.Context gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorize (gr, area_width, area_height);
		DrawGrid (gr, area_width, area_height, alpha);
		DrawAllFigures (gr, area_width, area_height, alpha);
	}

	private void DrawAllFigures (Cairo.Context gr, int area_width, int area_height, double alpha)
	{
		int col = 1, fig;
		double x = start_x, y = start_y;
		gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
		for (int figure = 0; figure < figures.Count; figure++, col++)
		{
			fig = (int)figures[figure];
			if (fig >= (int) FigureType.Length) 
				fig -= (int) FigureType.Length;

			DrawFigure (gr, x, y, alpha, (FigureType) fig);

			if (col >= columns) {
				col = 0;
				y += rect_h;
				x = start_x;
			} else {
				x += rect_w;
			}
		}
	}

	private void DrawFigure (Cairo.Context gr, double x, double y, double alpha, FigureType fig)
	{
		double space_x = 0.04, space_y = 0.08;

		switch (fig) {
		case FigureType.Triangle:
			DrawingHelpers.DrawEquilateralTriangle (gr, x + space_x, y + space_y, figure_size);
			break;
		case FigureType.Rectangle:
			gr.Rectangle (x + space_x, y + space_y, figure_size, figure_size);
			gr.Stroke ();
			break;
		case FigureType.Diamond:
			DrawingHelpers.DrawDiamond  (gr, x + space_x, y + space_y, figure_size);
			break;
		case FigureType.Cercle:
			gr.Arc (x + space_x + figure_size / 2, y + space_y + figure_size / 2, figure_size / 2, 0, 2 * Math.PI);	
			gr.Stroke ();
			break;
		case FigureType.TriangleWithLine:
			DrawingHelpers.DrawEquilateralTriangle (gr, x + space_x, y + space_y, figure_size);
			gr.MoveTo (x + space_x + figure_size / 2, y + space_y);
			gr.LineTo (x + space_x + figure_size / 2, y + space_y + figure_size);
			gr.Stroke ();
			break;
		case FigureType.RectangleWithLine:
			gr.Rectangle (x + space_x, y + space_y, figure_size, figure_size);
			gr.MoveTo (x + space_x, y + space_y);
			gr.LineTo (x + space_x + figure_size, y + space_y + figure_size);
			gr.MoveTo (x + space_x + figure_size, y + space_y);
			gr.LineTo (x + space_x, y + space_y + figure_size);
			gr.Stroke ();
			break;
		/*case FigureType.DiamondWithLine:
			DrawingHelpers.DrawDiamond  (gr, x + space_x, y + space_y, figure_size);
			gr.MoveTo (x + space_x + figure_size / 2, y + space_y);
			gr.LineTo (x + space_x + figure_size / 2, y + space_y + figure_size);
			gr.Stroke ();
			break;
		case FigureType.CercleWithLine:
			gr.Arc (x + space_x + figure_size / 2, y + space_y + figure_size / 2, figure_size / 2, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.MoveTo (x + space_x + figure_size / 2, y + space_y);
			gr.LineTo (x + space_x + figure_size / 2, y + space_y + figure_size);
			gr.Stroke ();
			break;*/
		}
	}

	private void DrawGrid (Cairo.Context gr, int area_width, int area_height, double alpha)
	{
		double x = start_x, y = start_y;
		gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {
				gr.Rectangle (x + column * rect_w, y + row * rect_h, rect_w, rect_h);
			}
		}
		gr.Stroke ();
	}
}


