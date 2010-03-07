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

using gbrainy.Core.Main;

namespace gbrainy.Core.Libraries
{
	public class CairoContextEx : Cairo.Context
	{
		Pango.Layout layout;
		double font_size;
		static SVGImage image = null;

		const double width_margin = 0.04;
		const double line_spacing = 0.018;

		public CairoContextEx (IntPtr state, Gtk.Widget widget) : base (state)
		{
			layout = Pango.CairoHelper.CreateLayout (this);

			// We do not honor DPI settings or font sizes (just font name)
			// User should resize the window
			layout.FontDescription = widget.PangoContext.FontDescription.Copy ();
			SetPangoNormalFontSize ();
		}

		// Used by GeneratePDF
		public CairoContextEx (Cairo.Surface s) : base (s)
		{
			layout = Pango.CairoHelper.CreateLayout (this);
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
			and Pango paints on the top-left of the coordinates
		*/


		// Shows a text from the current position. No Width defined then no RTL positioning
		public void ShowPangoText (string str)
		{
			Cairo.Matrix old = Matrix;

			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();		
			layout.SetText (str);
			layout.SingleParagraphMode = true;
			Pango.CairoHelper.ShowLayout (this, layout);
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

				Pango.CairoHelper.ShowLayout (this, layout);
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
			MoveTo ((old.X0 + x * old.Xx) - w, y * old.Yy);
			Pango.CairoHelper.ShowLayout (this, layout);
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
			MoveTo ((old.X0 + x * old.Xx) - w / 2, old.Y0 + (y * old.Yy) - h / 2);
			Pango.CairoHelper.ShowLayout (this, layout);
			Matrix = old;
		}

		public void DrawStringWithWrapping (double x, double y, string str, double max_width)
		{
			int w, h;
			Cairo.Matrix old = Matrix;

			if (max_width < 0 || max_width > 1)
				throw new InvalidOperationException ("Invalid maximum width value");

			MoveTo (x, y);
			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();

			layout.Width = (int) (max_width * old.Xx * Pango.Scale.PangoScale);
			layout.Spacing = (int) (line_spacing * (old.Yy * Pango.Scale.PangoScale));

			layout.SingleParagraphMode = false;
			layout.SetText (str);
			Pango.CairoHelper.ShowLayout (this, layout);
			layout.GetPixelSize (out w, out h);
			Matrix = old;
		}

		public void MeasureString (string str, double max_width, bool wrapping, out double width, out double height)
		{
			int w, h;
			Cairo.Matrix old = Matrix;

			if (max_width < 0 || max_width > 1)
				throw new InvalidOperationException ("Invalid maximum width value");

			UpdateFontSize ();
			Matrix = new Cairo.Matrix ();

			layout.Width = (int) (max_width * old.Xx * Pango.Scale.PangoScale);
			layout.Spacing = (int) (line_spacing * (old.Xx * Pango.Scale.PangoScale));

			layout.SingleParagraphMode = !wrapping;
			layout.SetText (str);
			layout.GetPixelSize (out w, out h);
			Matrix = old;
			height = h / old.Yy;
			width = w / old.Xx;
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

		public void DrawClock (double x, double y, double size, int hand_short, int hand_large)
		{
			const double radian = Math.PI / 180;
			double radius = size / 2;
			double x0, y0;
			int num, degrees;

			Arc (x, y, radius, 0, 2 * Math.PI);
			Stroke ();
			for (degrees = 0; degrees < 360; degrees+= 30) {
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				 // Small lines
				MoveTo (x + 0.9 * x0, y + 0.9 * y0);
				LineTo (x + x0, y + y0);
				Stroke ();
				// Numbers
				num = (degrees / 30) + 3;
				if (num > 12) num = num - 12;

				DrawTextCentered (x + x0 * 0.75,  y + y0 * 0.75, num.ToString ());
				Stroke ();
			}

			if (hand_large >=1 && hand_large <= 12 ) {
				// Hand Large
				degrees = (hand_large - 3) * 30;
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				MoveTo (x, y);
				LineTo (x + x0 * 0.55, y + y0 * 0.55);
				Stroke ();
			}

			if (hand_short >=1 && hand_short <= 12) {
				// Hand Short
				degrees = (hand_short - 3) * 30;
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				MoveTo (x, y);
				LineTo (x + x0 * 0.4, y + y0 * 0.4);
				Stroke ();
			}
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
					image = new SVGImage (System.Reflection.Assembly.GetCallingAssembly (), "background.svg");

				Save ();
				Rectangle (0, 0, 1, 1);
				Scale (0.999 / image.Width, 0.999 / image.Height);
				image.RenderToCairo (Handle);
				Restore ();

			} catch {
			}
		}

		public void DrawImageFromAssembly (string  resource, double x, double y, double width, double height)
		{
			try {
				using (SVGImage image = new SVGImage (System.Reflection.Assembly.GetCallingAssembly (), resource))
				{
					DrawImage (image, x, y, width, height);
				}
			}
			catch (Exception)
			{
				return;
			}
		}

		public void DrawImageFromFile (string filename, double x, double y, double width, double height)
		{
			try {
				using (SVGImage image = new SVGImage (filename))
				{
					DrawImage (image, x, y, width, height);
				}
			}
			catch (Exception)
			{
				return;
			}
		}

		void DrawImage (SVGImage image, double x, double y, double width, double height)
		{		
			Save ();
			Translate (x, y);
			Scale (width / image.Width, height / image.Height);
			image.RenderToCairo (Handle);
			Restore ();
		}
	}
}
