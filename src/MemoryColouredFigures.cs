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
using System.Timers;
using Gtk;

public class MemoryColouredFigures : Memory
{
	enum SquareColor
	{
		Color1 = 0,
		Color2,
		Color3,
		Length
	}

	private const int columns = 6, rows = 6;
	private const int squares = columns * rows;
	private const double rect_w = 0.3 / rows;
	private const double rect_h = 0.3 / columns;
	private SquareColor []squares_colours;
	private ArrayListIndicesRandom answers_order;
	private const int answers = 4;
	private ColorPalette palette;
	private int color_sheme;

	public override string Name {
		get {return Catalog.GetString ("Colored Figures");}
	}

	public override string Question {
		get {
			return Catalog.GetString ("Memorize the following figure in the time given");
		}
	}

	public override string MemoryQuestion {
		get { return Catalog.GetString ("Which of these figures was the one previously shown?");}
	}

	public override void Initialize ()
	{
		squares_colours = new SquareColor [squares * answers];
		color_sheme = random.Next (2);
		palette = new ColorPalette(ColorPalette.Id.PrimarySecundaryColors);
		palette.Initialize();

		for (int i = 0; i < squares; i++)	
			squares_colours[i] = (SquareColor) random.Next ((int) SquareColor.Length);
		
		Randomize (squares_colours, 0, squares);
		Randomize (squares_colours, 0, squares * 2);
		Randomize (squares_colours, 0, squares * 3);

		answers_order = new ArrayListIndicesRandom (answers);
		answers_order.Initialize ();

		right_answer = string.Empty;
		for (int i = 0; i < answers_order.Count; i++) {
			if ((int) answers_order[i] == 0) {
				right_answer += (char) (65 + (int) i);
				break;
			}
		}

		base.Initialize ();
	}

	private void Randomize (SquareColor []colours, int source, int target)
	{	
		int elements = 4 + random.Next (2);
		bool done = false;

		while (done == false) {
			for (int i = 0; i < squares; i++) {
				colours[i + target] = colours[i + source];
			}

			for (int i = 0; i < elements; i++) {
				colours[target + random.Next (squares)] = (SquareColor) random.Next ((int) SquareColor.Length);
			}

			// Is not valid if it is already present
			bool equals;
			for (int answer = 0; answer < answers; answer++) {
				if (answer * squares == target)
					continue;

				equals = true;
				for (int i = 0; i < squares; i++) {
					if (colours[i + target] != colours[i + (answer * squares)]) {
						equals = false;
						break;
					}
				}

				if (equals == true)
					break;
			}

			if (equals == false)
				done = true;
		}
	}

	public override void DrawObjectToMemorizeFading (Cairo.Context gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorizeFading (gr, area_width, area_height);
		palette.Alpha=alpha; 
		gr.Color = palette.Cairo(DefaultDrawingColor);
		DrawSquare (gr, DrawAreaX + 0.3, DrawAreaY + 0.1, squares_colours, 0);			
	}
	
	public override void DrawPossibleAnswers (Cairo.Context gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.05, y = DrawAreaY;
	
		palette.Alpha=1;
		gr.Color = palette.Cairo(DefaultDrawingColor);
		
		for (int i = 0; i < answers_order.Count; i++) {
			if (i == 2) {
				y += 0.4;
				x = DrawAreaX + 0.05;
			}
			DrawSquare (gr, x, y, squares_colours, squares * (int) answers_order[i]);
			gr.MoveTo (x, y + 0.34);
			gr.ShowText (Catalog.GetString ("Figure") + " " + (char) (65 + i));
			gr.Stroke ();
			x += 0.35;
		}
	}

	public override void DrawObjectToMemorize (Cairo.Context gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorize (gr, area_width, area_height);
		DrawSquare (gr, DrawAreaX + 0.3, DrawAreaY + 0.1, squares_colours, 0);
	}

	private void DrawSquare (Cairo.Context gr, double x, double y, SquareColor []colours, int index)
	{
		gr.Save ();
		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {

				// if you want 2 schemes (primary or secundary colors)
				gr.Color = palette.Cairo(ColorPalette.Id.First+ color_sheme*3 + (int)colours[index+(columns * row) + column] );
				// if you want 3 colors at random
				// gr.Color = palette.Cairo((int)colorus[index+(columns*row)+ column]);

				gr.Rectangle (x + row * rect_w, y + column * rect_h, rect_w, rect_h);
				gr.Fill ();
				gr.Stroke ();
			}
		}
		gr.Restore ();
		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {
				gr.Rectangle (x + row * rect_w, y + column * rect_h, rect_w, rect_h);
				gr.Stroke ();			
			}
		}
	}
}


