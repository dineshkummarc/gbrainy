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

public class PuzzlePeopleTable : Game
{
	private const double figure_size = 0.15;
	private string ques1, ques2;
	
	private class Cercle
	{	
		public double x;
		public double y;

		public Cercle (double x, double y) 
		{
			this.x = x;
			this.y = y;
		}
	}

	public override string Name {
		get {return Catalog.GetString ("People in a table");}
	}

	public override string Question {
		get {return String.Format (Catalog.GetString ("A group of people evenly separated is sat in a round table. How many people are if the {0} person is opposite to the {1}?"), ques1, ques2);} 
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("Substracting the two positions you get how many people is sat in half of the table then the double is the total amount of people.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		switch (random.Next (3)) {
		case 0:
			ques1 = Catalog.GetString ("5th");
			ques2 = Catalog.GetString ("19th");
			right_answer = "28";
			break;
		case 1:
			ques1 = Catalog.GetString ("4th");
			ques2 = Catalog.GetString ("12th");
			right_answer = "16";
			break;
		case 2:
			ques1 = Catalog.GetString ("9th");
			ques2 = Catalog.GetString ("22nd");
			right_answer = "26";
			break;
		}			
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.22, y = DrawAreaY + 0.2;
		double pos_x = x;
		double pos_y = y;
		Cercle[] cercles = null;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);
	
		cercles =  new Cercle [] {
			new Cercle (0.01, 0.06),
			new Cercle (0.27, 0.06),
			new Cercle (0.01, 0.21),
			new Cercle (0.27, 0.21),
			new Cercle (0.14, 0),
			new Cercle (0.14, 0.29)
		};

		// Cercle
		gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
		gr.Stroke ();		

		double point_size = 0.01;
		for (int i = 0; i < cercles.Length; i++) {
			gr.Arc (x + point_size + cercles[i].x, y + point_size + cercles[i].y, point_size, 0, 2 * Math.PI);
			gr.Fill ();
			gr.Stroke ();
		}

		gr.MoveTo (x + cercles[2].x + 0.01, y + cercles[2].y + 0.01);
		gr.LineTo (x + cercles[1].x + 0.01, y + cercles[1].y + 0.01);
		gr.Stroke ();
	}

}


