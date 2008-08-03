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

public class MemoryFiguresNumbers : Memory
{
	private int [] numbers;
	private int rows, columns, squares;
	private double rect_w, rect_h;
	private ArrayListIndicesRandom answers_order;
	private const int answers = 4;
	private const double block_space = 0.35;

	public override string Name {
		get {return Catalog.GetString ("Figures with numbers");}
	}

	public override string MemoryQuestion {
		get { return Catalog.GetString ("Which one of these squares was previously shown (A, B, C or D)?");}
	}

	public override void Initialize ()
	{
		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			columns = rows = 2;
			break;
		case Difficulty.Medium:
			columns = rows = 3;
			break;
		case Difficulty.Master:
			columns = rows = 4;
			break;
		}

		rect_w = 0.3 / rows;
		rect_h = 0.3 / columns;
		squares = rows * columns;
		numbers = new int [squares * 4];
		
		for (int n = 0; n < rows; n++)
			for (int i = 0; i < columns; i++)
				numbers[(n*columns) + i] = random.Next (10) + random.Next (5);

		Randomize (numbers, 0, squares);
		Randomize (numbers, 0, squares * 2);
		Randomize (numbers, 0, squares * 3);

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

	private void Randomize (int []nums, int source, int target)
	{	
		int elements = 4 + random.Next (2);
		bool done = false;

		while (done == false) {
			for (int i = 0; i < squares; i++) {
				nums[i + target] = nums[i + source];
			}

			for (int i = 0; i < elements; i++) {
				nums[target + random.Next (squares)] = random.Next (10) + random.Next (5);
			}

			// Is not valid if it is already present
			bool equals = true;
			for (int answer = 0; answer < answers; answer++) {
				if (answer * squares == target)
					continue;

				equals = true;
				for (int i = 0; i < squares; i++) {
					if (nums[i + target] != nums[i + (answer * squares)]) {
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
	

	public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX , y = DrawAreaY;
		gr.Color = DefaultDrawingColor;
		for (int i = 0; i < answers_order.Count; i++) {
			if (i == 2) {
				y += 0.45;
				x = DrawAreaX;
			}
			DrawSquare (gr, x, y, numbers, squares * (int) answers_order[i]);
			gr.MoveTo (x, y + block_space);
			gr.ShowPangoText (String.Format (Catalog.GetString ("Figure {0}"), (char) (65 + i)));
			gr.Stroke ();
			x += block_space + 0.08;
		}
	}

	public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorize (gr, area_width, area_height);
		DrawSquare (gr, 0.3 + DrawAreaX, DrawAreaY + 0.1, numbers, 0);
	}

	private void DrawSquare (CairoContextEx gr, double x, double y, int[] nums, int index)
	{
		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {
				gr.Rectangle (x + row * rect_w, y + column * rect_h, rect_w, rect_h);
				gr.DrawTextCentered (x + (rect_w / 2) + column * rect_w, y + (rect_h / 2) + row * rect_h, 
					(nums[index + column + (row * columns)]).ToString ());
			}
		}
		gr.Stroke ();
	}

}
