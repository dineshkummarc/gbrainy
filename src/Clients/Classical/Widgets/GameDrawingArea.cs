/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Gtk;
using Cairo;
using Mono.Unix;

using gbrainy.Core.Main;

namespace gbrainy.Clients.Classical.Widgets
{
	// This client code because every client can decide how to composite its game view
	// For example, asking the question in another area, or using the same space for
	// question or answer, etc.
	public class GameDrawingArea : DrawingArea
	{
		bool paused;

		public IDrawable Drawable { get; set; }
		public string Question { get; set; }
		public string Solution { get; set; }
		public int DrawingSquare { get; private set; }
		public int OffsetX { get; private set; }
		public int OffsetY { get; private set; }
		public bool UseSolutionArea { get; set; }

		public bool Paused { 
			get { return paused; }
			set {
				paused = value;
				QueueDraw ();
			}
		}

		// Constants
		const int question_high = 55;
		const int solution_high = 55;
		const int total_margin = 0; // Margin applied as in-box for themes
		const double text_margin = 0.015;

		public GameDrawingArea ()
		{
			UseSolutionArea = true;
		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			if (!IsRealized)
				return false;

			int w, h, total_w, total_h;

			Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
			CairoContextEx cr = new CairoContextEx (cc.Handle, this);

			args.Window.GetSize (out total_w, out total_h);

			h = total_h - question_high - total_margin * 2;
			if (UseSolutionArea)
				h -= solution_high;

			w = total_w - total_margin * 2;

			// We want a square drawing area for the puzzles then the figures are shown as designed.
			// For example, squares are squares. This also makes sure that proportions are kept when resizing
			DrawingSquare = Math.Min (w, h);

			if (DrawingSquare < w)
				OffsetX = (w - DrawingSquare) / 2;
			else
				OffsetX = 0;

			if (DrawingSquare < h)
				OffsetY = (h - DrawingSquare) / 2;
			else
				OffsetY = 0;

			OffsetY += question_high + total_margin;

			cr.Save ();

			// Draw a background taking all the window area
			cr.Scale (total_w, total_h);
			cr.DrawBackground ();
			
			if (Paused == false) {
				DrawQuestionAndAnswer (cr, total_w, total_h);
			} else {
				cr.SetPangoFontSize (0.08);
				cr.DrawTextCentered (0.5, 0.5, Catalog.GetString ("Paused"));
				cr.Stroke ();
			}
			cr.Restore ();

			if (Paused == false)
			{
				// Draw the game area
				cr.Translate (OffsetX, OffsetY);
				cr.SetPangoNormalFontSize ();
				cr.Color = new Color (1, 1, 1, 0.5);
				Drawable.Draw (cr, DrawingSquare, DrawingSquare, Direction == Gtk.TextDirection.Rtl);
				cr.Stroke ();
			}

			((IDisposable)cc).Dispose();
			((IDisposable)cr).Dispose();
			return true;
		}

		void DrawQuestionAndAnswer (CairoContextEx cr, int width, int height)
		{
			double scaled_margin;
			double max_width;
			double line_space;

			line_space = cr.FontLineSpace;
			cr.FontLineSpace = 0.004;
			cr.SetPangoFontSize (0.018);

			scaled_margin = (double) total_margin / (double) width;
			max_width = 1 - (scaled_margin * 2) - (text_margin * 2);
			cr.UseMarkup = true;

			if (String.IsNullOrEmpty (Question) == false)
			{
				// Question drawing
				cr.DrawStringWithWrapping (scaled_margin + text_margin, scaled_margin + text_margin, Question, max_width);
			}

			if (UseSolutionArea && String.IsNullOrEmpty (Solution) == false)
			{
				// Solution drawing
				cr.DrawStringWithWrapping (scaled_margin + text_margin, 1 - 0.12 - scaled_margin - text_margin, Solution, max_width);
			}
			cr.UseMarkup = false;
			cr.Stroke ();
			cr.FontLineSpace = line_space;
		}
	}
}
