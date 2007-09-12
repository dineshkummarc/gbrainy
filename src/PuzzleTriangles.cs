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

public class PuzzleTriangles : Game
{
	public override string Name {
		get {return Catalog.GetString ("Triangles");}
	}

	public override string Question {
		get {return Catalog.GetString ("How many triangles of any size can you count in the figure below?");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("A triangle can be embedded inside another triangle.");}
	}

	public override void Initialize ()
	{
		right_answer = "13";
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.1, y = DrawAreaY + 0.2;
		double witdh = 0.6, height = 0.5;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);
		
		gr.MoveTo (x, y);
		gr.LineTo (x + witdh, y);		
		gr.LineTo (x + witdh / 2, y + height / 2);
		gr.LineTo (x, y);
		gr.LineTo (x + 0.45, y + height /4);
		gr.Stroke ();
	
		gr.MoveTo (x + witdh / 2, y);
		gr.LineTo (x + witdh / 2, y + height / 2);
		gr.Stroke ();

		gr.MoveTo (x + 0.152, y + 0.125);
		gr.LineTo (x + witdh, y);
		gr.Stroke ();
	}

}


