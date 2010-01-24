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
using System.Collections.Generic;

using gbrainy.Core.Libraries;
using gbrainy.Core.Main;

namespace gbrainy.Core.Toolkit
{
	// Horizontal Container
	public class Container : Widget
	{
		List <Widget> children;
		bool rtl;

		public Container (double x, double y, double width, double height) : base (width, height)
		{
			X = x;
			Y = y;
			children = new List <Widget> ();
		}

		public double X { get; set; }
		public double Y { get; set; }

		public Widget [] Children {
			get { return children.ToArray (); }
		}

		public void AddChild (Widget widget)
		{
			if (children.Contains (widget))
				throw new InvalidOperationException ("Child already exists in container");

			//
			// Propagate events from child controls to this container (parent)
			//
			widget.DrawRequest += delegate (object sender, EventArgs e)
			{
				OnDrawRequest ();
			};

			widget.SelectedEvent += delegate (object sender, Widget.SeletectedEventArgs e)
			{
				OnSelected (e);
			};

			children.Add (widget);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = X, y = Y;
	
			this.rtl = rtl;

			/*gr.Save ();
			gr.Color = new Cairo.Color (0, 0, 1);
			gr.Rectangle (X, Y, Width, Height);
			gr.Stroke ();
			gr.Restore ();*/

			//
			// Coordinates are stored right to left
			//
			if (rtl == false) {
				for (int i = 0; i < children.Count; i++)
				{
					gr.Save ();						
					gr.Translate (x, y);

					children[i].Draw (gr, area_width, area_height, rtl);
					gr.Restore ();
					x += children[i].Width;
				}
			} else {
				x += Width;
				for (int i = 0; i < children.Count; i++)
				{
					x -= children[i].Width;
					gr.Save ();
					gr.Translate (x, y);
					children[i].Draw (gr, area_width, area_height, rtl);
					gr.Restore ();
				}
			}
		}

		public override void MouseEvent (object obj, MouseEventArgs args)
		{
			double x = X, y = Y;

			if (rtl == true)
				x += Width;

			foreach (Widget child in Children)
			{
				if (rtl == true)
					x -= child.Width;

				if ((args.X >= x) && (args.X < x + child.Width) && 
					(args.Y >= y) && (args.Y < y + child.Height))
				{
					child.MouseEvent (this, args);
				} else {
					child.MouseEvent (this, new MouseEventArgs (-1, -1, args.EventType));
				}

				if (rtl == false)
					x += child.Width;
			}
		}
	}
}
