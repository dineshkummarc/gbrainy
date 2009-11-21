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

using Cairo;
using Mono.Unix;
using System;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Logic
{
	public class PuzzleTimeNow : Game
	{
		const double figure_size = 0.3;
		int after, position_a, position_b, ans;

		public override string Name {
			get {return Catalog.GetString ("Time now");}
		}

		public override string Question {
			get {return (String.Format (
				// Translators: {1} and {2} are replaced by hours
				Catalog.GetString ("{0} hours ago it was as long after {1} as it was before {2} on the same day. What is the time now?"),
				after, position_a, position_b));}
		}

		public override void Initialize ()
		{
			after = 4 + random.Next (3);
			position_a = 2 + random.Next (3);
			position_b = position_a + 6;

			ans = after + position_b;

			if (ans > 12)
				ans = ans - 12;  

			right_answer = ans.ToString ();
		}	

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);
			gr.DrawClock (DrawAreaX + 0.4, DrawAreaY + 0.4, figure_size, 
				0, 0 /* No hands */);

			gr.MoveTo (DrawAreaX + 0.3, DrawAreaY + 0.3 + figure_size);
			gr.ShowPangoText (Catalog.GetString ("Sample clock"));
			gr.Stroke ();

		}
	}
}
