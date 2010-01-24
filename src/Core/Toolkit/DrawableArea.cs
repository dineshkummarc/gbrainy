/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Libraries;
using gbrainy.Core.Main;

namespace gbrainy.Core.Toolkit
{
	public class DrawableArea : Widget
	{
		public virtual event WidgetDrawEventHandler DrawEventHandler;
		public virtual event WidgetSelectedEventHandler SelectedRequestEvent;
		bool hoover;

	    	public DrawableArea (double width, double height) : base (width, height)
		{

		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl) 
		{
			if (hoover == true)
  			{
				double lw = gr.LineWidth;
				double [] dashes = {0.01,  /* ink */
						   0.01,  /* skip */
				};

				gr.Save ();

				gr.Color = new Cairo.Color (0.5, 0.5, 0.5, 1);
				gr.SetDash (dashes, 0);
				gr.Rectangle (-lw, -lw, Width + lw * 2, Height + lw * 2);
				gr.Stroke ();
				gr.Restore ();
			}

			if (DrawEventHandler == null)
				return;
	
			DrawEventHandler (this, new DrawEventArgs (gr, Width, Height, rtl, Data));
		}

		public override void MouseEvent (object obj, MouseEventArgs args)
		{
			if (args.X == -1 || args.Y == -1) {
				if (hoover == true) {
					hoover = false;
					OnDrawRequest ();
				}
			} else {

				if (args.EventType == MouseEventType.DoubleClick) {
					OnSelected (new SeletectedEventArgs (Data, DataEx));
				} else {
					if (hoover == false) {
						hoover = true;
						OnDrawRequest ();
					}
				}
			}
		}		
	}
}
