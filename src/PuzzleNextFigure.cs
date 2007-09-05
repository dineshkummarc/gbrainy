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

public class PuzzleNextFigure : Game
{
	private double rows, columns;
	private int type;
	private const double figure_size = 0.2;

	public enum CerclePosition 
	{
		None		= 0,
		Top		= 2,
		Right		= 4,
		Bottom		= 8,
		Left		= 16,
	}

	public override string Name {
		get {return Catalog.GetString ("Next figure");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which is the next logical figure in the sequence (A, B, or C)?");} 
	}

	public override void Initialize ()
	{
		right_answer = "C";
	}

	private void DrawDiamon (Cairo.Context gr, double x, double y, CerclePosition cercles)
	{	
		double distance = 0.04;

		gr.MoveTo (x + figure_size / 2, y);
		gr.LineTo (x, y + figure_size / 2);
		gr.LineTo (x + figure_size / 2, y + figure_size);
		gr.LineTo (x + figure_size, y + figure_size / 2);
		gr.LineTo (x + figure_size / 2, y);
		gr.Stroke ();

		if ((cercles & CerclePosition.Top) == CerclePosition.Top) {
			gr.Arc (x + figure_size / 2, y + distance, 0.01, 0, 2 * 3.14);	
			gr.Stroke ();
		}

		if ((cercles & CerclePosition.Right) == CerclePosition.Right) {
			gr.Arc (x + figure_size - distance, y + figure_size / 2, 0.01, 0, 2 * 3.14);	
			gr.Stroke ();
		}

		if ((cercles & CerclePosition.Bottom) == CerclePosition.Bottom) {
			gr.Arc (x + figure_size / 2, y + figure_size - distance, 0.01, 0, 2 * 3.14);	
			gr.Stroke ();
		}

		if ((cercles & CerclePosition.Left) == CerclePosition.Left) {
			gr.Arc (x + distance, y + figure_size / 2, 0.01, 0, 2 * 3.14);
			gr.Stroke ();
		}
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{
		double x = DrawAreaX;
		double y = DrawAreaY;
		double space_figures = figure_size + 0.1;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		DrawDiamon (gr, x, y, CerclePosition.Top | CerclePosition.Left);
		DrawDiamon (gr, x + space_figures , y, CerclePosition.Bottom);
		DrawDiamon (gr, x + space_figures * 2, y, CerclePosition.Top | CerclePosition.Right);
		
		y += figure_size + 0.10;
		gr.MoveTo (x, y);
		gr.ShowText (Catalog.GetString ("Possible answers are:"));
		gr.Stroke ();
		y += 0.06;

		
		DrawDiamon (gr, x, y, CerclePosition.Right | CerclePosition.Left);
		gr.MoveTo (x, y + figure_size + 0.05);
		gr.ShowText (Catalog.GetString ("Figure") + " A");
		gr.Stroke ();

		DrawDiamon (gr, x + space_figures, y, CerclePosition.Top | CerclePosition.Right);
		gr.MoveTo (x + space_figures, y + figure_size + 0.05);
		gr.ShowText (Catalog.GetString ("Figure") + " B");
		gr.Stroke ();

		DrawDiamon (gr, x+ space_figures * 2, y, CerclePosition.Bottom | CerclePosition.Top);
		gr.MoveTo (x + space_figures * 2, y + figure_size + 0.05);
		gr.ShowText (Catalog.GetString ("Figure") + " C");
		gr.Stroke ();
	}
}


