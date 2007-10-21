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
using System.Timers;
using Gtk;
using System.Collections;

public class MemoryColouredText : Memory
{
	private ArrayListIndicesRandom colors_order;
	private Hashtable colors_mapping;
	private Colors question;
	private string question_colorname;

	enum Colors
	{
		Blue,
		Red,
		Green,
		Pink,
		Yellow,
		Brown,
		Last
	}
	
	public override string Name {
		get {return Catalog.GetString ("Coloured text");}
	}

	public override string Question {
		get {return Catalog.GetString ("Memorize the colours associated to every word"); }
	}

	public override string MemoryQuestion {
		get { 
			return String.Format (Catalog.GetString ("Which was the colour of the text that said '{0}'?"), question_colorname);}
	}

	private void GetColor (Colors clr, out string name, out Cairo.Color color)
	{
		switch (clr) {
		case Colors.Blue:
			color = new Cairo.Color (0, 0, 1);
			name = Catalog.GetString ("Blue");
			break;
		case Colors.Red:
			color = new Cairo.Color (1, 0, 0);
			name = Catalog.GetString ("Red");
			break;
		case Colors.Green:
			color = new Cairo.Color (0, 1, 0);
			name = Catalog.GetString ("Green");
			break;
		case Colors.Pink:
			color = new Cairo.Color (1.0, 0.75, 0.79);
			name = Catalog.GetString ("Pink");
			break;
		case Colors.Yellow:
			color = new Cairo.Color (1, 1, 0);
			name = Catalog.GetString ("Yellow");
			break;
		case Colors.Brown:
			color = new Cairo.Color (0.64, 0.12, 0.12);
			name = Catalog.GetString ("Brown");
			break;
		default:
			name = string.Empty;
			color = new Cairo.Color (0, 0, 0);
			break;
		}
	}

	public override void Initialize ()
	{
		string name;
		Cairo.Color color = new Cairo.Color (1, 1, 1);
		colors_mapping = new Hashtable ();
		colors_order = new ArrayListIndicesRandom ((int) Colors.Last);
		colors_order.Initialize ();
		
		question = (Colors) random.Next ((int) Colors.Last);
		for (int i = 0; i < colors_order.Count; i ++)
		{	
			GetColor ((Colors) colors_order[i], out name, out color);
			colors_mapping.Add (i, color);

			if ((Colors)i == question) {
				right_answer = name;
				GetColor ((Colors) i, out question_colorname, out color);
			}
		}
		
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

	public void DrawObject (Cairo.Context gr, int area_width, int area_height, double alpha)
	{
		double x= DrawAreaX + 0.2, y = DrawAreaY + 0.2;
		Cairo.Color color = new Cairo.Color (0, 0, 0);
		string name = null;

		for (int i = 0; i < colors_order.Count; i++)
		{
			GetColor ((Colors) i, out name, out color);
			color = (Cairo.Color) colors_mapping[i];
			color = new Cairo.Color (color.R, color.G, color.B, alpha);
			gr.MoveTo (x, y);
			gr.Color = color;
			gr.ShowText (name);
			gr.Stroke ();
			
			if (i == 2) {
				y += 0.2;
				x = DrawAreaX + 0.2;
			} else {
				x+= 0.2;
			}
		}
	}

}


