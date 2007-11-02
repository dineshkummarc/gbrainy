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
using Gdk;
using Gtk;

public class GameDrawingArea : DrawingArea
{
	public Game puzzle = null;

	private void DrawBackground (Cairo.Context gr)
	{
		int columns = 40;
		int rows = 40;
		double rect_w = 1.0 / rows;
		double rect_h = 1.0 / columns;

		gr.Save ();

		gr.Color = new Cairo.Color (1, 1, 1);
		gr.Paint ();	

		gr.Color = new Cairo.Color (0.8, 0.8, 0.8);
		gr.LineWidth = 0.001;
		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {			
				gr.Rectangle (row * rect_w, column * rect_h, rect_w, rect_h);
			}
		}
		gr.Stroke ();
		gr.Restore ();		
	}

	private void DrawWelcome (Cairo.Context gr, int area_width, int area_height)
	{
		gr.Scale (area_width, area_height);
		DrawBackground (gr);

		gr.Color = new Cairo.Color (0.1, 0.1, 0.1);
		gr.SelectFontFace ("Sans", Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
		gr.SetFontSize (0.04);

		gr.MoveTo (0.2, 0.2);
		gr.ShowText (Catalog.GetString ("Welcome to gbrainy") + " " + Defines.VERSION);
		gr.Stroke ();
		
		gr.MoveTo (0.1, 0.4);
		// Keep the translated version of this string under 40 characters long 
		gr.ShowText (Catalog.GetString ("Use the Game menu to start a new game"));
		gr.Stroke ();

	}

	protected override bool OnExposeEvent (Gdk.EventExpose args)
	{
		if(!IsRealized)
			return false;

		int w, h;
		args.Window.GetSize (out w, out h);
		Cairo.Context cr = Gdk.CairoHelper.Create (args.Window);
	
		if (puzzle == null)
			DrawWelcome (cr, w, h);
		else
			puzzle.Draw (cr, w, h);

			((IDisposable)cr).Dispose();
			return base.OnExposeEvent(args);
	}

}


