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

public class PuzzleMatrixGroups : Game
{
	private int [] numbers;
	private int good_pos;
	private const int rows = 4, columns = 4;
	private int divisor;

	public override string Name {
		get {return Catalog.GetString ("Matrix groups");}
	}

	public override string Question {
		get {return Catalog.GetString ("One of the black numbers in the matrix should be blue. Which one?");}
	}

	public override string Tip {
		get { return Catalog.GetString ("All the blue numbers share an arithmetical property.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += String.Format (Catalog.GetString ("Every blue number can be divided by {0}."), divisor);
			return answer;
		}
	}

	public override void Initialize ()
	{
		numbers = new int [rows * columns];
		bool completed = false;
		int count;
		int good = 1 + random.Next (5);

		switch (random.Next (2)) {
		case 0:
			divisor = 2;
			break;
		case 1:
			divisor = 3;
			break;
		}
		
		while (completed == false) {
			count = 0;
			for (int n = 0; n < rows; n++) {
				for (int i = 0; i < columns; i++) {
					numbers[(n*rows) + i] = GetUnique ((n*rows) + i);
					if (numbers[(n*rows) + i] % divisor == 0) {
						count++;
						if  (count == good) {
							good_pos =  (n*rows) + i;
						}
					}
				}
			}
			
			if (count > 5 && count < 10)
				completed = true;
		}
		right_answer = numbers[good_pos].ToString ();
	}

	private int GetUnique (int max)
	{
		int unique = 0, i;
		bool found = false;

		while (found == false)
		{
			unique = 1 + random.Next (100);
			for (i = 0; i < max; i++) {
				if (unique == numbers [i]) {
					break;
				}
			}
			if (i == max)
				found = true;
		}
		return unique;
	}	

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{
		double rect_w = DrawAreaWidth / rows;
		double rect_h = DrawAreaHeight / columns;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);
		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {
				
				gr.Color = DefaultDrawingColor;
				gr.Rectangle (DrawAreaX + row * rect_w, DrawAreaY + column * rect_h, rect_w, rect_h);
				gr.Stroke ();
				gr.MoveTo (0.04 + DrawAreaX + column * rect_w, (rect_h / 2) + DrawAreaY + row * rect_h);

				if (numbers[column + (row * 4)] % divisor == 0 && good_pos != column + (row * 4))
					gr.Color = new Cairo.Color (0, 0, 0.8);
				else
					gr.Color = DefaultDrawingColor;
	
				gr.ShowText ( (numbers[column + (row * 4)]).ToString() );
				gr.Stroke ();
			}
		}
	}
}

