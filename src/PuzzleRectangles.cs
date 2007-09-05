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

public class PuzzleRectangles : Game
{
	private double rows, columns;
	private int type;

	public override string Name {
		get {return Catalog.GetString ("Number of squares");}
	}

	public override string Question {
		get {return Catalog.GetString ("How many squares of any size can you count of in the figure bellow?");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("A square can be also built from other squares.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";

			switch (type) {
			case 0:
				answer += Catalog.GetString ("There are 16 single squares, 7 squares made by two single squares, 4 squares made by three squares and 1 square made by sixteen single squares.");
				break;
			case 1:
				answer += Catalog.GetString ("There are 9 single squares, 4 squares made by two single squares and 1 square made by nine single squares.");
				break;
			}
			return answer;
		}
	}

	public override void Initialize ()
	{
		type = random.Next (2);
		rows = 3;
		columns = 3;		

		if (type == 0) {
			rows++;
			columns++;
			right_answer = "30";
		} else {
			right_answer = "14";
		}
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
				gr.Rectangle (DrawAreaX + row * rect_w, DrawAreaY + column * rect_h, rect_w, rect_h);
				//gr.MoveTo (0.04 + DrawAreaX + column * rect_w, (rect_h / 2) + DrawAreaY + row * rect_h);
			}
		}

		gr.Stroke ();
	}

}


