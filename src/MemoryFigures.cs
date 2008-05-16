/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	private int rows;
	private int columns;
	private const double start_x = 0.25;
	private const double start_y = 0.1;
	private const double figure_size = 0.08;
	private double rect_w, rect_h;
	private int question_pos, question_answer;
	private int figures_active;

	public enum FigureType
	{
		Triangle,
		Rectangle,
		Diamond,
		Cercle,
		TriangleWithLine,
		RectangleWithLine,
		DiamondWithLine,
		CercleWithLine,
	}

	public override string Name {
		get {return Catalog.GetString ("Memory figures");}
	}

	public override string MemoryQuestion {
		get { 
			return Catalog.GetString ("In which cell is the other figure like the one shown below? Answer the cell number." );}
	}

	public override void Initialize ()
	{
		int fig1, fig2;

		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			figures_active = 4;
			rows = columns = 3;
			break;
		case Difficulty.Medium:
			figures_active = 6;
			rows = 3;
			columns = 4;			
			break;
		case Difficulty.Master:
			figures_active = 8;
			columns = rows = 4;
			break;
		}

		rect_w = 0.6 / columns;
		rect_h = 0.8 / rows;
		figures = new ArrayListIndicesRandom (figures_active * 2);
		figures.Initialize ();
		question_pos = random.Next (figures_active * 2);

		for (int figure = 0; figure < figures_active * 2; figure++)
		{	
			if (figure == question_pos)
				continue;
	
			fig1 = figures[figure];
			fig2 = figures[question_pos];

			if (fig1 >= figures_active) fig1 -= figures_active;
			if (fig2 >= figures_active) fig2 -= figures_active;

			if (fig1 == fig2) {
				question_answer = figure + 1;
				break;
			}
		}
		right_answer = question_answer.ToString ();
		base.Initialize ();
	}

	public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height)
	{
		int col = 1, fig;
		double x = start_x, y = start_y;
		gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, 1);
		DrawGrid (gr, area_width, area_height);

		if (DrawAnswer ==  true) {
			DrawAllFigures (gr, area_width, area_height);
			return;
		}

		gr.SetLargeFont ();
		for (int figure = 0; figure < figures.Count; figure++, col++)
		{
			fig = (int)figures[figure];
			if (fig >= figures_active) fig -= figures_active;

			if (figure == question_pos)
				DrawFigure (gr, x, y, (FigureType) fig);
			else {
				gr.DrawTextCentered (x + rect_w / 2, y + rect_h / 2, (figure + 1).ToString ());
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

	public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorize (gr, area_width, area_height);
		DrawGrid (gr, area_width, area_height);
		DrawAllFigures (gr, area_width, area_height);
	}

	private void DrawAllFigures (CairoContextEx gr, int area_width, int area_height)
	{
		int col = 1, fig;
		double x = start_x, y = start_y;
		gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
		for (int figure = 0; figure < figures.Count; figure++, col++)
		{
			fig = (int)figures[figure];
			if (fig >= figures_active) 
				fig -= figures_active;

			DrawFigure (gr, x, y, (FigureType) fig);

			if (col >= columns) {
				col = 0;
				y += rect_h;
				x = start_x;
			} else {
				x += rect_w;
			}
		}
	}

	private void DrawFigure (CairoContextEx gr, double x, double y, FigureType fig)
	{
		double space_x, space_y;

		space_x = (rect_w - figure_size) / 2;
		space_y = (rect_h - figure_size) / 2;

		switch (fig) {
		case FigureType.Triangle:
			gr.DrawEquilateralTriangle (x + space_x, y + space_y, figure_size);
			break;
		case FigureType.Rectangle:
			gr.Rectangle (x + space_x, y + space_y, figure_size, figure_size);
			gr.Stroke ();
			break;
		case FigureType.Diamond:
			gr.DrawDiamond (x + space_x, y + space_y, figure_size);
			break;
		case FigureType.Cercle:
			gr.Arc (x + space_x + figure_size / 2, y + space_y + figure_size / 2, figure_size / 2, 0, 2 * Math.PI);	
			gr.Stroke ();
			break;
		case FigureType.TriangleWithLine:
			gr.DrawEquilateralTriangle (x + space_x, y + space_y, figure_size);
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
		case FigureType.DiamondWithLine:
			gr.DrawDiamond (x + space_x, y + space_y, figure_size);
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
			break;
		}
	}

	private void DrawGrid (CairoContextEx gr, int area_width, int area_height)
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


