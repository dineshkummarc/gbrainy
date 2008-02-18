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

public class PuzzleMoveFigure: Game
{
	private int lines;
	private int type;

	public override string Name {
		get {return Catalog.GetString ("Move figure");}
	}

	public override string Question {
		get {return Catalog.GetString ("What is the minimum number of circles to be moved in order to convert the left figure into the right figure?");} 
	}
	
	public override string Answer {
		get { 
			string answer = base.Answer + " ";

			switch (type) {
			case 0:
				answer += Catalog.GetString ("Move the circle from the first line to the second; move two circles from the fourth to the second line; and move the fifth line.");
				break;
			case 1:
				answer += Catalog.GetString ("Move the two first lines; move the first and last circle of the last line to the third line; and move sixth and seventh lines.");
				break;
			}
			return answer;
		}
	}

	public override void Initialize ()
	{	
		type = random.Next (2);
		lines = 4 + type;
		
		switch (type)
		{
			case 0:
				right_answer = "3";
				break;
			case 1:
				right_answer = "5";
				break;
		}
		
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double first_x, x, y;
		double figure_size = 0.07 + (0.01 * (5 - lines));
		double margin = 0;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		// Figure 1
		margin = ((1.0 - (figure_size * lines * 2)) / 2);

		x = first_x = margin + (figure_size * lines / 2) + figure_size / 2;
		y = DrawAreaY;
		for (int line = 0; line < lines + 1; line++)
		{
			for (int circles = 0; circles < line; circles++)
			{
				gr.Arc (x, y, figure_size / 2, 0, 2 * Math.PI);	
				gr.Stroke ();
				x += figure_size;
			}
			x = first_x = first_x - (figure_size / 2);
			y += figure_size;			
		}

		// Figure 2
		first_x = margin + (figure_size * lines);
		y = DrawAreaY + figure_size;
		for (int line = 0; line < lines; line++)
		{
			x = first_x = first_x + (figure_size / 2);
			for (int circles = 0; circles < lines - line; circles++)
			{
				gr.Arc (x, y, figure_size / 2, 0, 2 * Math.PI);	
				gr.Stroke ();
				x += figure_size;
			}
			y += figure_size;			
		}

	}

}


