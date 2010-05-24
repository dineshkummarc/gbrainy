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

using Cairo;
using Mono.Unix;
using System;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Logic
{
	public class PuzzleSquareSheets : Game
	{
		public override string Name {
			get {return Catalog.GetString ("Square sheets");}
		}

		public override string Question {
			get {return Catalog.GetString ("What is the minimum number of square sheets of paper of any size required to create the figure? Lines indicate frontiers between different sheets.");} 
		}

		public override string Tip {
			get { return Catalog.GetString ("The sheets should overlap.");}
		}

		public override string Rationale {
			get {
				return Catalog.GetString ("The numbers in the figure reflect the different areas covered by each one of the sheets.");
			}
		}

		public override void Initialize ()
		{
			right_answer = "5";
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.2, y = DrawAreaY + 0.2, width = 0.4, height = 0.4;

			base.Draw (gr, area_width, area_height, rtl);

			gr.Rectangle (x, y, width, height);
			gr.Stroke ();
		
			gr.MoveTo (x, y + 0.1);
			gr.LineTo (x + width, y + 0.1);  // First horizontal
			gr.Stroke ();

			gr.MoveTo (x, y + 0.3);
			gr.LineTo (x + width - 0.1, y + 0.3); // Second horizontal
			gr.Stroke ();

			gr.MoveTo (x + 0.1, y);
			gr.LineTo (x + 0.1, y + height);  // First vertical
			gr.Stroke ();

			gr.MoveTo (x + 0.3, y);
			gr.LineTo (x + 0.3, y + height - 0.1);  // Second vertical
			gr.Stroke ();

			if (DrawAnswer == false)
				return;

			gr.LineTo (x + 0.04, y + 0.06);
			gr.ShowPangoText ("1");

			gr.LineTo (x + 0.18, y + 0.06);
			gr.ShowPangoText ("2");

			gr.LineTo (x + 0.34, y + 0.06);
			gr.ShowPangoText ("3");
		
			gr.LineTo (x + 0.04, y + 0.2);
			gr.ShowPangoText ("2");

			gr.LineTo (x + 0.18, y + 0.2);
			gr.ShowPangoText ("4");

			gr.LineTo (x + 0.34, y + 0.2);
			gr.ShowPangoText ("5");

			gr.LineTo (x + 0.04, y + 0.36);
			gr.ShowPangoText ("3");

		}
	}
}


