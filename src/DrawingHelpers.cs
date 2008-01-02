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
using System.Text;

// Utility class for common drawing operations
public class DrawingHelpers
{
	static DrawingHelpers ()
	{

	}
	
	static public void DrawTextAlignedRight (Cairo.Context gr, double x, double y, string str)
	{
		TextExtents extents;	

		extents = gr.TextExtents (str);
		gr.MoveTo (x - extents.Width, y);
		gr.ShowText (str);
		gr.Stroke ();
	}

	// From a giving point centers the text into it
	static public void DrawTextCentered (Cairo.Context gr, double x, double y, string str)
	{
		TextExtents extents;
		extents = gr.TextExtents (str);
		gr.MoveTo (x -extents.Width / 2, y + extents.Height / 2);
		gr.ShowText (str);
		gr.Stroke ();
	}

	static public double DrawStringWithWrapping (Cairo.Context gr, double x, double y, double line_space, string str)
	{
		TextExtents extents;
		StringBuilder sb = new StringBuilder ();
		int idx = 0, prev = 0;			

		while (idx < str.Length) {
			prev = idx;
			idx = str.IndexOf (' ', prev + 1);
			if (idx == -1)
				idx = str.Length;

			extents = gr.TextExtents (sb.ToString () + str.Substring (prev, idx - prev));
			if (extents.Width > 1.0 - x - 0.05) {
				gr.MoveTo (x, y);
				gr.ShowText (sb.ToString ());
				gr.Stroke ();
				y += line_space;
				sb = new StringBuilder ();
				prev++;
			} 

			sb.Append (str.Substring (prev, idx - prev)); 

			if (str.Length == idx) {
				gr.MoveTo (x, y);
				gr.ShowText (sb.ToString ());
				gr.Stroke ();					
			}				
		}

		return y;
	}

	static public void DrawEquilateralTriangle (Cairo.Context gr, double x, double y, double size)
	{
		gr.MoveTo (x + (size / 2), y);
		gr.LineTo (x, y + size);
		gr.LineTo (x + size, y + size);
		gr.LineTo (x + (size / 2), y);
		gr.Stroke ();	
	}

	static public void DrawDiamond (Cairo.Context gr, double x, double y, double size)
	{
		gr.MoveTo (x + size / 2, y);
		gr.LineTo (x, y + size / 2);
		gr.LineTo (x + size / 2, y + size);
		gr.LineTo (x + size, y + size / 2);
		gr.LineTo (x + size / 2, y);
		gr.Stroke ();
	}

}

