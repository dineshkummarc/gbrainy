/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System;

public class Puzzle3DCube : Game
{
	int rows, columns, deepth;

	public override string Name {
		get {return Catalog.GetString ("Cube");}
	}

	public override string Question {
		get {return String.Format (Catalog.GetString ("How many cubes do you count in the figure below? (not counting the figure) Answer using a number."));} 
	}

	public override string Tip {
		get { return Catalog.GetString ("A cube is a regular solid object having six congruent square faces.");}
	}

	public override void Initialize ()
	{
		int ans, max_random;

		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			max_random = 1;
			break;
		case Difficulty.Master:
			max_random = 5;
			break;
		case Difficulty.Medium:
		default:
			max_random = 3;
			break;		
		}

		rows = columns = deepth = 4 + random.Next (max_random);
		ans = rows * columns * deepth;
		right_answer += ans.ToString ();	
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		const double size = 0.05;
		const double radian = Math.PI / 180;
		const int degrees = -45;
		const double radius = 0.035;
		const double size_y = 0.025;

		double x = DrawAreaX + 0.1;
		double y = DrawAreaY + 0.3;
		double x0, y0, offset_y;
		double offset_x = 0;
		
		base.Draw (gr, area_width, area_height);

		x = 1 - (2 * DrawAreaX) - (columns * size * 1.5);
		x = DrawAreaX + x /2;

		// Front face
		for (int pos_y = 0; pos_y < rows; pos_y++)
		{
			//  |
			//  |
			//
			gr.MoveTo (x, y + (pos_y * size));
			gr.LineTo (x, y + ((pos_y + 1) * size));
			gr.Stroke ();

			for (int pos_x = 0; pos_x < columns; pos_x++)
			{
				//  ---|
				//     |
				//
				gr.MoveTo (x + (size * pos_x) , y + (pos_y * size));
				gr.LineTo (x + (size * (pos_x + 1)), y + (pos_y * size));
				gr.LineTo (x + (size * (pos_x + 1)), y + (pos_y + 1) * size);
				gr.Stroke ();
			}
		}

		gr.MoveTo (x , y + (rows * size));
		gr.LineTo (x + (columns * size) , y + (rows * size));
		gr.Stroke ();

		// Back face
		for (int pos_y = 0; pos_y < rows; pos_y++)
		{
			offset_x = (0.025 * pos_y);

			//  |
			//  |
			//
			gr.MoveTo (x + offset_x, y - (pos_y * size_y));
			gr.LineTo (x + offset_x + 0.025, y - ((pos_y + 1)  * size_y));
			gr.Stroke ();

			for (int pos_x = 0; pos_x < columns; pos_x++)
			{
				gr.MoveTo (x + offset_x + (size * pos_x) , y - (pos_y * size_y));
				gr.LineTo (x + offset_x + (size * (pos_x + 1)), y - (pos_y * size_y));
				gr.LineTo (x + offset_x +  0.025 + (size * (pos_x + 1)), y - (pos_y + 1) * size_y);
				gr.Stroke ();
			}
		}

		offset_x = (0.025 * columns);
		gr.MoveTo (x + offset_x, y - (rows * size_y));
		gr.LineTo (x + offset_x + (size * columns) , y - (rows * size_y));
		gr.Stroke ();

		// Lateral face
		for (int pos_y = 0; pos_y < rows; pos_y++)
		{
			for (int pos_x = 0; pos_x < columns; pos_x++)
			{
				offset_x = (size * columns) + (0.025 * pos_x);
				offset_y = size_y * pos_x;
		
				gr.MoveTo (x + offset_x, y - offset_y + (pos_y * size));
				gr.LineTo (x + offset_x, y - offset_y + (pos_y + 1) * size);
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);

				gr.LineTo (x + offset_x + x0, y - offset_y + y0 +  (pos_y + 1) * size);
				gr.Stroke ();
			}
		}

		offset_x = (size * columns) + (0.025 * columns);
		offset_y = size_y * rows;	
		gr.MoveTo (x + offset_x, y - offset_y);
		gr.LineTo (x + offset_x, y - offset_y + (rows * size));
		gr.Stroke ();
	}
}	

