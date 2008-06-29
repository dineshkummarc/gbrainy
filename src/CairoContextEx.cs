/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Runtime.InteropServices;

public class CairoContextEx : Cairo.Context
{
	public CairoContextEx (IntPtr state) : base (state)
	{

	}

	// Used by GeneratePDF
	public CairoContextEx (Cairo.Surface s) : base (s)
	{

	}
	
	// No dispose of resources on this class
	protected override void Dispose (bool disposing)
	{
	}
	
	public void DrawTextAlignedRight (double x, double y, string str)
	{
		TextExtents extents;	

		extents = TextExtents (str);
		MoveTo (x - extents.Width, y);
		ShowText (str);
		Stroke ();
	}

	// From a giving point centers the text into it
	public void DrawTextCentered (double x, double y, string str)
	{
		TextExtents extents;
		extents = TextExtents (str);
		MoveTo (x -extents.Width / 2, y + extents.Height / 2);
		ShowText (str);
		Stroke ();
	}

	public double DrawStringWithWrapping (double x, double y, double line_space, string str)
	{
		TextExtents extents;
		StringBuilder sb = new StringBuilder ();
		int idx = 0, prev = 0;			

		while (idx < str.Length) {
			prev = idx;
			idx = str.IndexOf (' ', prev + 1);
			if (idx == -1)
				idx = str.Length;

			extents = TextExtents (sb.ToString () + str.Substring (prev, idx - prev));
			if (extents.Width > 1.0 - x - 0.05) {
				MoveTo (x, y);
				ShowText (sb.ToString ());
				Stroke ();
				y += line_space;
				sb = new StringBuilder ();
				prev++;
			} 

			sb.Append (str.Substring (prev, idx - prev)); 

			if (str.Length == idx) {
				MoveTo (x, y);
				ShowText (sb.ToString ());
				Stroke ();					
			}				
		}

		return y;
	}

	public void DrawEquilateralTriangle (double x, double y, double size)
	{
		MoveTo (x + (size / 2), y);
		LineTo (x, y + size);
		LineTo (x + size, y + size);
		LineTo (x + (size / 2), y);
		Stroke ();	
	}

	public void DrawDiamond (double x, double y, double size)
	{
		MoveTo (x + size / 2, y);
		LineTo (x, y + size / 2);
		LineTo (x + size / 2, y + size);
		LineTo (x + size, y + size / 2);
		LineTo (x + size / 2, y);
		Stroke ();
	}

	public void SetLargeFont ()
	{
		SetFontSize (0.05);
	}

	public void SetNormalFont ()
	{
		SetFontSize (0.03);
	}

	public void FillGradient (double x, double y, double w, double h)
	{
		Save ();
		LinearGradient shadow = new LinearGradient (x, y, x + w, y + h);
		shadow.AddColorStop (0, new Color (0, 0, 0, 0.3));
		shadow.AddColorStop (0.5, new Color (0, 0, 0, 0.1));
		Source = shadow;
		Fill ();
		Restore ();
	}

	public void FillGradient (double x, double y, double w, double h, Color color)
	{
		Save ();
		LinearGradient shadow = new LinearGradient (x, y, x + w, y + h);
		shadow.AddColorStop (0, new Color (color.R, color.G, color.B, color.A));
		shadow.AddColorStop (0.5, new Color (color.R, color.G, color.B, color.A * 0.7));
		Source = shadow;
		Fill ();
		Restore ();
	}

	virtual public void DrawBackground ()
	{
		try {
			using (SVGImage image = new SVGImage (Defines.DATA_DIR + "background.svg")) 
			{
				Save ();
				Scale (0.999 / image.Width, 0.999 / image.Height);
				image.RenderToCairo (Handle);
				Restore ();
			}

		} catch {
		}
	}

}



