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
using System.Runtime.InteropServices;
using Pango;

#if GTK_2_8 // For GTK < 2.10

public static class PangoCairoHelper
{
	[DllImport ("libpangocairo-1.0.so.0")]
	private static extern void pango_cairo_show_layout (IntPtr cr, IntPtr layout);

	public static void ShowLayout (Cairo.Context cr, Pango.Layout layout)
	{
		pango_cairo_show_layout (cr == null ? IntPtr.Zero : cr.Handle,
		layout == null ? IntPtr.Zero : layout.Handle);
	}

	[DllImport ("libpangocairo-1.0.so.0")]
	private static extern IntPtr pango_cairo_create_layout (IntPtr cr);

	public static Pango.Layout CreateLayout (Cairo.Context cr)
	{
		IntPtr raw_ret = pango_cairo_create_layout (cr == null ? IntPtr.Zero : cr.Handle);
		return GLib.Object.GetObject (raw_ret) as Pango.Layout;
	}

	[DllImport ("libpangocairo-1.0.so.0")]
	private static extern void pango_cairo_layout_path (IntPtr cr, IntPtr layout);

	public static void LayoutPath (Cairo.Context cr, Pango.Layout layout,
	bool iUnderstandThePerformanceImplications)
	{
		pango_cairo_layout_path (cr == null ? IntPtr.Zero : cr.Handle,
		layout == null ? IntPtr.Zero : layout.Handle);
	}

	[DllImport ("libpangocairo-1.0.so.0")]
	private static extern void pango_cairo_context_set_resolution (IntPtr pango_context, double dpi);

	public static void ContextSetResolution (Pango.Context context, double dpi)
	{
		pango_cairo_context_set_resolution (context == null ? IntPtr.Zero : context.Handle, dpi);
	}

	[DllImport ("libpangocairo-1.0.so.0")]
	private static extern IntPtr pango_layout_get_context (IntPtr layout);

	public static Pango.Context LayoutGetContext (Pango.Layout layout)
	{
		IntPtr handle = pango_layout_get_context (layout.Handle);
		return handle.Equals (IntPtr.Zero) ? null : GLib.Object.GetObject (handle) as Pango.Context;
	}
}

#endif

public class CairoContextEx : Cairo.Context
{
	Pango.Layout layout;
	double font_size;
	static SVGImage image = null;

	const double line_space = 0.018;
	const double width_margin = 0.04;

	public CairoContextEx (IntPtr state, Gtk.Widget widget) : base (state)
	{
		CommonConstructor ();

	#if GTK_2_8
	#else
		double resolution = widget.Screen.Resolution;
		if (resolution != -1)  {
			Pango.Context c = layout.Context;
			Pango.CairoHelper.ContextSetResolution (c, resolution);
			c.Dispose ();
		}
	#endif

	}

	// Used by GeneratePDF
	public CairoContextEx (Cairo.Surface s) : base (s)
	{
		CommonConstructor ();
	}

	private void CommonConstructor ()
	{
#if GTK_2_8
		layout = PangoCairoHelper.CreateLayout (this);
#else
		layout = Pango.CairoHelper.CreateLayout (this);
#endif
		layout.FontDescription = FontDescription.FromString ("Sans");
		SetPangoNormalFontSize ();
	}

	// No dispose of resources on this class
	protected override void Dispose (bool disposing)
	{
		layout.Dispose ();
	}

	private void UpdateFontSize ()
	{
		layout.FontDescription.Size = (int) (font_size * Pango.Scale.PangoScale * Matrix.Xx);	
	}

	/*
		Font drawing using Pango

		* Pango does not work well with float numbers. We should work on 
		the device unit space and then translate to our user space.

		* Cairo Show.Text paints on the bottom-left of the coordinates
		and Pango paints on the top-left of the coordinate
	*/


	// Shows a text from the current position. No Width defined then no RTL positioning
	public void ShowPangoText (string str)
	{
		Cairo.Matrix old = Matrix;

		UpdateFontSize ();
		Matrix = new Cairo.Matrix ();		
		layout.SetText (str);
		layout.SingleParagraphMode = true;
#if GTK_2_8
		PangoCairoHelper.ShowLayout (this, layout);
#else
		Pango.CairoHelper.ShowLayout (this, layout);
#endif
		Matrix = old;
	}

	// Shows a text from the current position. Defines all the line as text drawing box
	public void ShowPangoText (string str, bool bold, double width, double rotation)
	{
		Pango.Alignment align = layout.Alignment;

		if (bold) {
			layout.FontDescription.Weight = Pango.Weight.Bold;
		}

		if (width == -1) { // Calculates maximum width in the user space
			layout.Width = (int) (((1 - width_margin) * Matrix.Xx - (CurrentPoint.X * Matrix.Xx)) * Pango.Scale.PangoScale);
		} else 
			layout.Width = (int) (width * Matrix.Xx * Pango.Scale.PangoScale);

		if (rotation != 0) {

			Cairo.Matrix old = Matrix;

			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();
			Rotate (rotation);
			layout.SetText (str);
			layout.SingleParagraphMode = true;
#if GTK_2_8
		PangoCairoHelper.ShowLayout  (this, layout);
#else
		Pango.CairoHelper.ShowLayout (this, layout);
#endif
			Matrix = old;
		}
		else
			ShowPangoText (str);
				
		layout.FontDescription.Weight = Pango.Weight.Normal;
		layout.Width = -1;
		layout.Alignment = align;
	}

	public void SetPangoNormalFontSize ()
	{
		font_size = 0.022;
	}

	public void SetPangoLargeFontSize ()
	{
		font_size = 0.0325;
	}

	public void SetPangoFontSize (double size)
	{
		font_size = size;
	}

	/*
		Draw text functions
	*/		
		
	// Used for fractions that right align is needed
	public void DrawTextAlignedRight (double x, double y, string str)
	{
		int w, h;
		Cairo.Matrix old = Matrix;

		UpdateFontSize ();
		Matrix = new Cairo.Matrix ();

		layout.SetText (str);
		layout.SingleParagraphMode = true;
		layout.Width = -1;
		layout.GetPixelSize (out w, out h);
		MoveTo ((old.X0 + x * old.Xx) - w, y * old.Xx);
#if GTK_2_8
		PangoCairoHelper.ShowLayout (this, layout);
#else
		Pango.CairoHelper.ShowLayout (this, layout);
#endif
		Matrix = old;
	}

	// From a giving point centers the text into it
	public void DrawTextCentered (double x, double y, string str)
	{
		int w, h;
		Cairo.Matrix old = Matrix;

		UpdateFontSize ();
		Matrix = new Cairo.Matrix ();

		layout.SetText (str);
		layout.SingleParagraphMode = true;
		layout.Width = -1;
		layout.GetPixelSize (out w, out h);
		MoveTo ((old.X0 + x * old.Xx) - w / 2, (y - font_size / 2) * old.Xx);
#if GTK_2_8
		PangoCairoHelper.ShowLayout  (this, layout);
#else
		Pango.CairoHelper.ShowLayout (this, layout);
#endif
		Matrix = old;
	}

	public double DrawStringWithWrapping (double x, double y, string str)
	{
		return DrawStringWithWrapping (x, y, str, -1);
	}

	public double DrawStringWithWrapping (double x, double y, string str, double width)
	{
		int w, h;
		Cairo.Matrix old = Matrix;

		MoveTo (x, y);
		UpdateFontSize ();
		Matrix = new Cairo.Matrix ();

		if (width == -1)
			layout.Width = (int) ((1.0 - x -  width_margin) * old.Xx * Pango.Scale.PangoScale);
		else	
			layout.Width = (int) (width * old.Xx * Pango.Scale.PangoScale);

		layout.Spacing = (int) (line_space * (old.Xx * Pango.Scale.PangoScale));
		layout.SingleParagraphMode = false;
		layout.SetText (str);
#if GTK_2_8
		PangoCairoHelper.ShowLayout (this, layout);
#else
		Pango.CairoHelper.ShowLayout (this, layout);
#endif
		layout.GetPixelSize (out w, out h);
		Matrix = old;
		return y + h / old.Xx;
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

	public void FillGradient (double x, double y, double w, double h)
	{
		Save ();
		LinearGradient shadow = new LinearGradient (x, y, x + w, y + h);
		shadow.AddColorStop (0, new Cairo.Color (0, 0, 0, 0.3));
		shadow.AddColorStop (0.5, new Cairo.Color (0, 0, 0, 0.1));
		Source = shadow;
		Fill ();
		Restore ();
		((IDisposable)shadow).Dispose ();
	}

	public void FillGradient (double x, double y, double w, double h, Cairo.Color color)
	{
		Save ();
		LinearGradient shadow = new LinearGradient (x, y, x + w, y + h);
		shadow.AddColorStop (0, new Cairo.Color (color.R, color.G, color.B, color.A));
		shadow.AddColorStop (0.5, new Cairo.Color (color.R, color.G, color.B, color.A * 0.7));
		Source = shadow;
		Fill ();
		Restore ();
		((IDisposable)shadow).Dispose ();
	}

	virtual public void DrawBackground ()
	{
		try {
			if (image == null)
				image = new SVGImage (Defines.DATA_DIR + "background.svg");

			Save ();
			Rectangle (0, 0, 1, 1);
			Scale (0.999 / image.Width, 0.999 / image.Height);
			image.RenderToCairo (Handle);
			Restore ();

		} catch {
		}
	}

}

