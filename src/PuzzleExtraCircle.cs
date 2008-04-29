/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

public class PuzzleExtraCircle : Game
{
	private const int total_slices = 6;
	private const int circles = 4;
	private const double radius = 0.1;
	private const double radian = Math.PI / 180;
	private int ans_pos;
	private Color[] cercle_colors;
	private Color[] badcercle_colors;
	private int[] start_indices;
	private ColorPalette cp;

	public override string Name {
		get {return Catalog.GetString ("Extra circle");}
	}

	public override string Question {
		get {return Catalog.GetString ("Which is the circle that does not belong to the group (it is not a sequence)? (A, B, C or D)");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("All the circles share a common property except for one.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += String.Format (Catalog.GetString ("In all the circles the colour slides follow the same order except for this one."));
			return answer;
		}
	}

	public override void Initialize ()
	{
		ArrayListIndicesRandom random_indices = new ArrayListIndicesRandom (total_slices);
		Color clr;

		cp = new ColorPalette (ColorPalette.Id.Last);
		cp.Initialize ();

		cercle_colors = new Color [total_slices];
		badcercle_colors =  new Color [total_slices];
		for (int i = 0; i < total_slices; i++) {
			cercle_colors [i] = cp.Cairo (i);
			badcercle_colors [i] = cp.Cairo (i);
		}
		
		// Correct answer
		random_indices.Initialize ();
		clr = badcercle_colors [(int) random_indices[0]];
		badcercle_colors [(int) random_indices[0]] =  badcercle_colors [(int) random_indices[1]];
		badcercle_colors [(int) random_indices[1]] = clr;

		// Indices
		start_indices = new int [circles];
		for (int i = 0; i < circles; i++)
			start_indices[i] = (int) random_indices[i];

		ans_pos = random.Next (circles);
		right_answer += (char) (65 + ans_pos);
	}

	private void DrawSlice (CairoContextEx gr, double x, double y, double dg, Color color)
	{
		double x1, y1, degrees;

		degrees = radian * (60 + dg);
		gr.MoveTo (x, y);
		x1 = x + radius * Math.Cos (degrees);
		y1 = y + radius * Math.Sin (degrees);
		gr.LineTo (x1, y1);
		
		degrees = dg * radian;
		gr.MoveTo (x, y);
		x1 = x + radius * Math.Cos (degrees);
		y1 = y + radius * Math.Sin (degrees);
		gr.LineTo (x1, y1);

		gr.Arc (x, y, radius, dg * radian, radian * (60 + dg));
		gr.FillGradient (x, y, radius, radius, color);
		gr.Stroke ();
	}

	private void DrawCircle (CairoContextEx gr, double x, double y, Color[] colors, int color_indice)
	{		
		double dg;
		gr.Arc (x, y, radius, 0, 2 * Math.PI);
		gr.Stroke ();
		
		gr.Save ();
		for (int slice = 0; slice < total_slices; slice++) 
		{	
			dg = slice * (360 / total_slices);
			DrawSlice (gr, x, y, dg, colors [color_indice]);
			
			color_indice++;
			if (color_indice >= colors.Length)
				color_indice = 0;
			
		}
		gr.Restore ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{		
		double x = DrawAreaX, y = DrawAreaY;
		Color [] colors;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		x+= radius / 2;
		y+= radius / 2;

		for (int i = 0; i < circles; i++)
		{
			if (ans_pos == i)
				colors = badcercle_colors;
			else
				colors = cercle_colors;

			DrawCircle (gr, x + i * 0.23, y + 0.2, colors, start_indices[i]);

			gr.MoveTo (x - 0.07 + i * 0.22, y + 0.38);
			gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), (char) (65 + i)));
			gr.Stroke ();
		}
	}
}

