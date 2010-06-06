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
	// Basic encapsulation of:
	//	- Cairo Context
	//	- Pango Drawing
	//	- RSvg image drawing
	//
	public class CairoContext : Cairo.Context
	{
		Pango.Layout layout;
		double font_size;

		const double width_margin = 0.04;
		const double line_spacing = 0.018;

		public CairoContext (IntPtr state, Gtk.Widget widget) : base (state)
		{
			layout = Pango.CairoHelper.CreateLayout (this);

			// We do not honor DPI settings or font sizes (just font name)
			// User should resize the window
			layout.FontDescription = widget.PangoContext.FontDescription.Copy ();
		}

		// Used by GeneratePDF
		public CairoContext (Cairo.Surface s, string font, int dpis) : base (s)
		{
			layout = Pango.CairoHelper.CreateLayout (this);
			layout.FontDescription = FontDescription.FromString (font);

			if (dpis > 0)  {
				Pango.Context c = layout.Context;
				Pango.CairoHelper.ContextSetResolution (c, dpis);
				c.Dispose ();
			}
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
				throw new ArgumentOutOfRangeException ("Invalid maximum width value");

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
				throw new ArgumentOutOfRangeException ("Invalid maximum width value");

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
			catch (Exception e)
			{
				Console.WriteLine ("CairoContext. Error '{0}' when drawing image from filename {1}", e.Message, filename);
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
