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
	public class PuzzleCube : Game
	{
		private char question;
		const int pairs = 4;

		private int[] question_answer = 
		{
			1, 4,
			6, 2,
			4, 1,
			2, 6,
		};

		public override string Name {
			get {return Catalog.GetString ("Cube");}
		}

		public override string Question {
			get {return String.Format (Catalog.GetString ("When folded as a cube, which face on the figure is opposite the face with a {0} drawn on it? Answer the number written on face."), question);} 
		}

		public override void Initialize ()
		{
			int pair = random.Next (pairs);
			question = (char) (48 + question_answer[pair * 2]);
			right_answer += (char) (48 + question_answer[(pair * 2) + 1]);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.1;
			double y = DrawAreaY + 0.1;
			const double txtoff_x = 0.04;
			const double txtoff_y = 0.03;

			base.Draw (gr, area_width, area_height, rtl);

			gr.Rectangle (x + 0.1, y, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.1 + txtoff_x, y + txtoff_y);
			gr.ShowPangoText ("1");

			gr.Rectangle (x + 0.2, y, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.2 + txtoff_x, y + txtoff_y);
			gr.ShowPangoText ("2");

			gr.Rectangle (x + 0.2, y + 0.1, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.2 + txtoff_x, y + 0.1 + txtoff_y);
			gr.ShowPangoText ("3");

			gr.Rectangle (x + 0.3, y + 0.1, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.3 + txtoff_x, y + 0.1 + txtoff_y);
			gr.ShowPangoText ("4");

			gr.Rectangle (x + 0.4, y + 0.1, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.4 + txtoff_x, y + 0.1 + txtoff_y);
			gr.ShowPangoText ("5");

			gr.Rectangle (x + 0.4, y + 0.2, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.4 + txtoff_x, y + 0.2 + txtoff_y);
			gr.ShowPangoText ("6");
		}
	}
}
