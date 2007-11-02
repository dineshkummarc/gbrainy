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
	private ColorPalette palette;
	private int question;
	private string question_colorname;

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

	public override void Initialize ()
	{
		palette = new ColorPalette(ColorPalette.Id.PrimarySecundaryColors);
		palette.Initialize ();
		
		question = random.Next ( palette.Count );
		right_answer = palette.Name( question );
		question_colorname = palette.Name( (ColorPalette.Id) question );
		
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
		palette.Alpha=alpha;

		double x= DrawAreaX + 0.2, y = DrawAreaY + 0.2;

		for (int i = 0; i < palette.Count ; i++)
		{
			gr.Color = palette.Cairo(i);
			gr.MoveTo (x, y);
			gr.ShowText ( palette.Name((ColorPalette.Id)i) );
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


