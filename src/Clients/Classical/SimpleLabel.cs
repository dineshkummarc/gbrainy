/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Clients.Classical
{
	//
	// Simple Label control that draws a piece of text
	//
	public class SimpleLabel : DrawingArea
	{
		string text;
		int width_margin, height_margin;

		public SimpleLabel ()
		{

		}

		public string Text {
			get { return text;}
			set {
				if (text == value)
					return;

				text = value;
				QueueDraw ();
			}
		}

		public int WidthMargin {
			set {
				if (width_margin == value)
					return;

				width_margin = value;
				QueueDraw ();
			}
		}

		public int HeightMargin {
			set {
				if (height_margin == value)
					return;

				height_margin = value;
				QueueDraw ();
			}
		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			if (String.IsNullOrEmpty (text))
				return base.OnExposeEvent (args);

			int winWidth, winHeight;
			Gdk.GC light = Style.ForegroundGC (StateType.Normal);
			args.Window.GetSize (out winWidth, out winHeight);

			using (Pango.Layout layout = new Pango.Layout (this.PangoContext))
			{
				if (Direction == Gtk.TextDirection.Rtl)
					layout.Alignment = Pango.Alignment.Right;					
				else
					layout.Alignment = Pango.Alignment.Left;
		
				layout.Width = (winWidth - width_margin * 2) * (int) Pango.Scale.PangoScale;
				layout.SetMarkup (text);
				args.Window.DrawLayout (light, width_margin, height_margin, layout);
			}

			return base.OnExposeEvent (args);
		}
	}
}
