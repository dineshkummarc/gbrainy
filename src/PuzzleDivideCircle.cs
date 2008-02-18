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

public class PuzzleDivideCircle : Game
{
	private const double figure_size = 0.15;
	private int dots;
	
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
		get {return Catalog.GetString ("Divide circles");}
	}

	public override string Question {
		get {return Catalog.GetString ("In the last figure, in how many regions is the circle divided into when all dots are connected?");} 
	}

	public override void Initialize ()
	{
		dots = 5 + random.Next (2);

		switch (dots) {
		case 5:
			right_answer = "16";
			break;
		case 6:
			right_answer = "30";
			break;
		}			
	}

	private void DrawAndConnectPoints (CairoContextEx gr, double x, double y, Cercle[] cercles, bool connect)
	{
		double point_size = 0.01;
		for (int i = 0; i < cercles.Length; i++) {
			gr.Arc (x + point_size + cercles[i].x, y + point_size + cercles[i].y, point_size, 0, 2 * Math.PI);
			gr.Fill ();
			gr.Stroke ();
		}

		if (connect == false)
			return;
		
		gr.Save ();
		gr.LineWidth = 0.003;
		double offset = point_size;
		for (int from = 0; from < cercles.Length; from++) {
			for (int to = 0; to < cercles.Length; to++) {
				gr.MoveTo (x + cercles[from].x+ offset, y + cercles[from].y + offset);
				gr.LineTo (x + cercles[to].x + offset, y + cercles[to].y + offset);
				gr.Stroke ();
			}
		}
		gr.Restore ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.05, y = DrawAreaY;
		double pos_x = x;
		double pos_y = y;
		Cercle[] cercles = null;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);

		// First cercle
		gr.Arc (pos_x + figure_size, y + figure_size, figure_size, 0, 2 * Math.PI);
		gr.Stroke ();
		DrawAndConnectPoints (gr, pos_x, pos_y, 
			new Cercle [] {
				new Cercle (0.14, 0),
				new Cercle (0.14, 0.29),
			}, true);

		gr.MoveTo (pos_x, pos_y + figure_size * 2 + 0.05);
		gr.ShowText (String.Format (Catalog.GetString ("Has {0} regions"), 2));
		gr.Stroke ();

		// Second cercle
		pos_x += 0.4;
		gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
		gr.Stroke ();		
		DrawAndConnectPoints (gr, pos_x, pos_y,
			new Cercle [] {
				new Cercle (0.01, 0.06),
				new Cercle (0.27, 0.06),
				new Cercle (0.14, 0.29),
			}, true);

		gr.MoveTo (pos_x, pos_y + figure_size * 2 + 0.05);
		gr.ShowText (String.Format (Catalog.GetString ("Has {0} regions"), 4));
		gr.Stroke ();

		// Third cercle
		pos_x = x;
		pos_y += 0.45;
		gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
		gr.Stroke ();		
		DrawAndConnectPoints (gr, pos_x, pos_y,
			new Cercle [] {
				new Cercle (0.01, 0.06),
				new Cercle (0.27, 0.06),
				new Cercle (0.01, 0.21),
				new Cercle (0.27, 0.21),
			}, true);

		gr.MoveTo (pos_x, pos_y + figure_size * 2 + 0.05);
		gr.ShowText (String.Format (Catalog.GetString ("Has {0} regions"), 8));
		gr.Stroke ();

		switch (dots) {
		case 5:
			cercles =  new Cercle [] {
				new Cercle (0.01, 0.06),
				new Cercle (0.27, 0.06),
				new Cercle (0.01, 0.21),
				new Cercle (0.27, 0.21),
				new Cercle (0.14, 0),
			};
			break;
		case 6:
			cercles =  new Cercle [] {
				new Cercle (0.01, 0.06),
				new Cercle (0.27, 0.06),
				new Cercle (0.01, 0.21),
				new Cercle (0.27, 0.21),
				new Cercle (0.14, 0),
				new Cercle (0.14, 0.29)
			};
			break;
		}

		// Forth cercle
		pos_x += 0.4;
		gr.Arc (pos_x + figure_size, pos_y + figure_size, figure_size, 0, 2 * Math.PI);
		gr.Stroke ();		
		DrawAndConnectPoints (gr, pos_x, pos_y, cercles, DrawAnswer);
	}

}


