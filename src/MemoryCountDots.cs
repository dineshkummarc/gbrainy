/*
 * Copyright (C) 2007 Javier M Mora <javiermm@gmail.com>
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
using System.Collections;

public class MemoryCountDots : Memory
{
        private const int NUMCOLUMNS = 7;
	private const int MINDOTS = 3;
	private const int MAXDOTSCOLOR = 7;
	private const int MAXDOTS = 25;

	private ArrayListIndicesRandom location_order;
	private ArrayListIndicesRandom color_order;

	private int [] dotsPerColor;

	enum Colors
	{
		Blue, 
		Red, 
		Green, 
		Pink, 
		Yellow, 
		Brown,
		Black,
		Last
	}

	//better if I define a struct with a string and a Cairo.color.

	private static string[] ColorName= new string[] {
		Catalog.GetString("blue"),
		Catalog.GetString("red"),
		Catalog.GetString("green"),
		Catalog.GetString("pink"),
		Catalog.GetString("yellow"),
		Catalog.GetString("brown"),
		Catalog.GetString("black")
	};

	private static Cairo.Color[] CairoColor= new Cairo.Color[] {
		new Cairo.Color(0,0,1),
		new Cairo.Color(1,0,0),
		new Cairo.Color(0,1,0),
		new Cairo.Color(1.0, 0.75, 0.79),
		new Cairo.Color(1, 1, 0),
		new Cairo.Color(0.64, 0.12, 0.12),
		new Cairo.Color(0,0,0)
	};

	public override string Name {
		get {return Catalog.GetString ("Counting dots");}
	}

	public override string Question {
		get {return Catalog.GetString ("Try to memorize how many dots of each colour there are."); }
	}

	public override string MemoryQuestion {
		get { return String.Format(Catalog.GetString ("How many {0} balls were in the previous image?"),
						ColorName[(int)color_order[0]])     ; }
	}

	public override void Initialize ()
	{
	        location_order = new ArrayListIndicesRandom (NUMCOLUMNS*NUMCOLUMNS);
		location_order.Initialize();

		color_order = new ArrayListIndicesRandom ((int)Colors.Last);
		color_order.Initialize();

		// dotsPerColor is compared with iterator of dots. (this iterator is 0 based, so I
		// have to substract 1 to make dotsPerColor contents 0 based.
		dotsPerColor = new int [(int)Colors.Last];
		for (int i=0,before=-1; i<(int)Colors.Last; i++) {
			dotsPerColor[i] = before + MINDOTS + random.Next(MAXDOTSCOLOR-MINDOTS+1) ;
			before = dotsPerColor[i];
		}

		right_answer = (dotsPerColor[0]+1).ToString ();
		
		base.Initialize ();
	}

	public override void DrawPossibleAnswers (Cairo.Context gr, int area_width, int area_height)
	{
	}	
	
	public override void DrawObjectToMemorizeFading (Cairo.Context gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorizeFading (gr, area_width, area_height);
		DrawObject (gr, area_width, area_height, alpha);
	}

	public override void DrawObjectToMemorize (Cairo.Context gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorize (gr, area_width, area_height);
		DrawObject (gr, area_width, area_height, alpha);
	}

	private Cairo.Color Fade(Cairo.Color color, double alpha)
	{
		return new Cairo.Color (color.R, color.G, color.B, alpha);
	}

	public void DrawObject (Cairo.Context gr, int area_width, int area_height, double alpha)
	{
		double x = DrawAreaX + 0.15, y = DrawAreaY + 0.1 ;

		Cairo.Color color= CairoColor[(int)Colors.Black];
		color = Fade(color,alpha);
		double pos_x = x, pos_y = y;
		double figure_size = 0.6 ;
		double square_size = figure_size / NUMCOLUMNS ;
		double center_square = square_size / 2;
		double radius_square = .8 * (square_size - (LineWidth *2)) / 2;

		gr.Rectangle (pos_x, pos_y, figure_size, figure_size);
		gr.Stroke ();

		for (int line = 0; line < NUMCOLUMNS - 1; line++) // Horizontal
		{
			pos_y += square_size;
			gr.MoveTo (pos_x, pos_y);
			gr.LineTo (pos_x + figure_size, pos_y);
			gr.Stroke ();
		}

		pos_y = y;
		for (int column = 0; column < NUMCOLUMNS - 1; column++) // Vertical
		{
			pos_x += square_size;
			gr.MoveTo (pos_x, pos_y);
			gr.LineTo (pos_x, pos_y + figure_size);
			gr.Stroke ();
		}

		pos_y = y + center_square;
		pos_x = x + center_square;

		for (int i = 0,itcolor=0; i < MAXDOTS && itcolor<(int)Colors.Last; i++)
		{
			int dx,dy;
			dx = ((int)location_order[i]) % NUMCOLUMNS;
			dy = ((int)location_order[i]) / NUMCOLUMNS;
			color = CairoColor[(int)color_order[itcolor]];
			color = Fade(color, alpha);
			gr.Color = color;
			gr.Arc (pos_x+square_size*dx, pos_y+square_size*dy,radius_square,0,2*Math.PI);
			gr.Fill ();

			if (i==dotsPerColor[itcolor]) itcolor++;
		}
	}
}


