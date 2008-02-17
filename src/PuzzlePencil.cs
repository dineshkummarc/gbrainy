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

public class PuzzlePencil : Game
{
	private ArrayListIndicesRandom random_indices;
	private const double figure_width = 0.1, figure_height = 0.1, space_width = 0.1, space_height = 0.15;
	private const double figure_size = 0.2;
	private const int figures = 5;
	private const int answer_index = 4;

	public override string Name {
		get {return Catalog.GetString ("Pencil");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which of the following figures cannot be drawn without crossing any previous lines and lifting the pencil?");} 
	}

	public override void Initialize ()
	{
		random_indices = new ArrayListIndicesRandom (figures);
		random_indices.Initialize ();
		right_answer = string.Empty;

		for (int i = 0; i < random_indices.Count; i++) {
			if ((int) random_indices[i] != answer_index)
				continue;
			
			right_answer += (char) (65 + (int) i);
			break;
		}	
	}

	private void DrawTriangle (CairoContextEx gr, double x, double y)
	{
		gr.MoveTo (x + (figure_size / 2), y);
		gr.LineTo (x, y + figure_size);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.LineTo (x + (figure_size / 2), y);
		gr.LineTo (x + (figure_size / 2), y + figure_size);
		gr.Stroke ();	
	}

	private void DrawDiamon (CairoContextEx gr, double x, double y)
	{
		gr.MoveTo (x, y);
		gr.LineTo (x - (figure_size / 2), y + (figure_size / 2));
		gr.LineTo (x, y + figure_size);
		gr.LineTo (x + figure_size / 2, y + (figure_size / 2));
		gr.LineTo (x, y);
		gr.LineTo (x, y + figure_size);
		gr.Stroke ();
	}

	private void DrawRectangleWithTriangles (CairoContextEx gr, double x, double y)
	{
		gr.Rectangle (x, y, figure_size, figure_size);
		gr.Stroke ();	
	
		gr.MoveTo (x, y + figure_size);
		gr.LineTo (x + figure_size / 4, y);
		gr.LineTo (x + figure_size / 2, y + figure_size);

		gr.Stroke ();		
	
		gr.MoveTo (x + figure_size / 2, y + figure_size);
		gr.LineTo (x + figure_size / 4 * 3, y);
		gr.LineTo (x + figure_size, y + figure_size);

		gr.Stroke ();
	}

	private void DrawThreeTriangles (CairoContextEx gr, double x, double y)
	{
		gr.MoveTo (x, y);
		gr.LineTo (x, y + figure_size);
		gr.LineTo (x + figure_size, y);
		gr.LineTo (x, y);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.LineTo (x + figure_size, y);

		gr.Stroke ();
		
	}

	private void DrawHouse (CairoContextEx gr, double x, double y)
	{
		gr.MoveTo (x, y + figure_size);
		gr.LineTo (x, y + figure_size / 2);
		gr.LineTo (x + figure_size / 2, y);
		gr.LineTo (x + figure_size, y + figure_size / 2);
		gr.LineTo (x, y + figure_size / 2);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.LineTo (x + figure_size, y + figure_size / 2);
		gr.LineTo (x, y + figure_size);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.Stroke ();
		
	}

	private void DrawRectangleWithCross (CairoContextEx gr, double x, double y)
	{
		gr.Rectangle (x, y, figure_size, figure_size);

		gr.MoveTo (x, y);
		gr.LineTo (x + figure_size, y + figure_size);
		gr.Stroke ();

		gr.MoveTo (x + figure_size, y);
		gr.LineTo (x, y + figure_size);
		gr.Stroke ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX, y = DrawAreaY;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		for (int figure = 0; figure < figures; figure++)
		{
			switch ((int) random_indices[figure]) {
			case 0:
				DrawTriangle (gr, x, y);
				break;
			case 1:
				DrawDiamon (gr, x + 0.1, y);
				break;
			//case 2:
			//	DrawHouse (gr, x, y);				
			//	break;
			case 2:
				DrawRectangleWithTriangles (gr, x, y);
				break;
			case 3:
				DrawThreeTriangles (gr, x, y);
				break;
			case answer_index:
				DrawRectangleWithCross (gr, x, y);
				break;
			
			}			
						
			gr.MoveTo (x, y + figure_size + 0.05);
			gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), (char) (65 + figure)));

			if (figure == 2) {
				x = DrawAreaX;
				y += figure_size + space_height;

			} else {						
				x += figure_size + space_width;		
			}
		}

	}

}


