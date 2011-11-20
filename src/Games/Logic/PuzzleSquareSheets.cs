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

using gbrainy.Core.Main;

namespace gbrainy.Games.Logic
{
	public class PuzzleSquareSheets : Game
	{
		const double width = 0.4, height = 0.4; 

		public override string Name {
			get {return translations.GetString ("Square sheets");}
		}

		public override string Question {
			get {return translations.GetString ("What is the minimum number of square sheets of paper of any size required to create the figure? Lines indicate frontiers between different sheets.");}
		}

		public override string Tip {
			get { return translations.GetString ("The sheets should overlap.");}
		}

		public override string Rationale {
			get {
				// Translators: the translated version should not take more characters that the English original
				return translations.GetString ("A full sized square of paper (yellow), a 3/4 of the whole size square of paper (blue) in the bottom right corner, another 3/4 square of paper (green) in the top left corner and a 1/4 square of paper (red) in the top left corner.");
			}
		}

		public override bool UsesColors {
			get { return true;}
		}

		protected override void Initialize ()
		{
			Answer.Correct = "4";
		}

		void DrawQuestion (CairoContextEx gr, double x, double y)
		{
			gr.Rectangle (x, y, width, height);
			gr.Stroke ();

			gr.MoveTo (x, y + 0.1);
			gr.LineTo (x + width, y + 0.1);  // Container square
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
		}

		void DrawAnswer (CairoContextEx gr, double x, double y)
		{
			ColorPalette palette = new ColorPalette (translations);
			gr.Save ();

			// A full sized square of paper
			gr.Color = palette.Cairo (ColorPalette.Id.Yellow);
			gr.Rectangle (x, y, width, height);
			gr.Fill ();
			gr.Stroke ();

			// 3/4 of the whole size square of paper in the bottom right corner
			gr.Color = palette.Cairo (ColorPalette.Id.Blue);
			double w = 3d/4d * width;
			double h = 3d/4d * height;
			gr.Rectangle (x + (width - w), y + (height - h), w, h);
			gr.Fill ();
			gr.Stroke ();

			// 3/4 square of paper in the top left corner
			gr.Color = palette.Cairo (ColorPalette.Id.Green);
			gr.Rectangle (x, y, 3d/4d * width, 3d/4d * height);
			gr.Fill ();
			gr.Stroke ();
			
			// 1/4 square of paper in the top left corner
			gr.Color = palette.Cairo (ColorPalette.Id.Red);
			gr.Rectangle (x, y, 1d/4d * width, 1d/4d * height);
			gr.Fill ();
			gr.Stroke ();

			gr.Restore ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.2, y = DrawAreaY + 0.2;

			base.Draw (gr, area_width, area_height, rtl);

			if (Answer.Draw)
				DrawAnswer (gr, x, y);

			DrawQuestion (gr, x, y);
		}
	}
}


