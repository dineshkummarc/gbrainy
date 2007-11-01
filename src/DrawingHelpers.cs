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

