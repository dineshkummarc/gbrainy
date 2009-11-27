/*
 * Copyright (C) 2007-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Core.Views
{
	public class FinishView : IDrawable
	{
		GameSession session;
		const int tips_shown = 4;

		public FinishView (GameSession session)
		{
			this.session = session;
		}

		static void DrawBand (CairoContextEx gr, double x, double y)
		{
			gr.Save ();
			gr.Rectangle (x, y, 1 - 0.06, 0.06);
			gr.Color = new Cairo.Color (0, 0, 0.2, 0.2);
			gr.Fill ();
			gr.Restore ();		
		}

		static void DrawBar (CairoContextEx gr, double x, double y, double w, double h, double percentage)
		{
			double per = percentage / 100;
	
			gr.Rectangle (x, y - h * per, w, h * per);
			gr.FillGradient (x, y - h * per, w, h * per, new Cairo.Color (0, 0, 1));
			gr.MoveTo (x, (y - 0.04) - h * per);
			gr.ShowPangoText (String.Format ("{0}%", percentage));
			gr.Stroke ();

			gr.Save ();
			gr.Color = new Cairo.Color (0, 0, 0);	
			gr.MoveTo (x, y);
			gr.LineTo (x, y - h * per);
			gr.LineTo (x + w, y - h * per);
			gr.LineTo (x + w, y);
			gr.LineTo (x, y);
			gr.Stroke ();
			gr.Restore ();
		}

		void DrawGraphicBar (CairoContextEx gr, double x, double y)
		{
			const double area_w = 0.9, area_h = 0.28;
			const double bar_w = 0.05, bar_h = area_h - 0.02;
			const double space_x = 0.09;
		
			gr.LineWidth = 0.005;

			// Axis
			gr.MoveTo (x, y);
			gr.LineTo (x, y + area_h);
			gr.LineTo (x + area_w, y + area_h);
			gr.Stroke ();

			x = x + space_x;
			DrawBar (gr, x, y + area_h, bar_w, bar_h, session.TotalScore);
			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Total"));

			x = x + space_x * 2;
			DrawBar (gr, x, y + area_h, bar_w, bar_h, session.LogicScore);
			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, 	Catalog.GetString ("Logic")); 

			x = x + space_x * 2;
			DrawBar (gr, x, y + area_h, bar_w, bar_h, session.MathScore);
			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Calculation"));

			x = x + space_x * 2;
			DrawBar (gr, x, y + area_h, bar_w, bar_h, session.MemoryScore);
			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Memory"));

			x = x + space_x * 2;
			DrawBar (gr, x, y + area_h, bar_w, bar_h, session.VerbalScore);
			gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Verbal"));
		}

		public void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double y = 0.04, x = 0.05;
			const double space_small = 0.02;
			string s;

			gr.Scale (area_width, area_height);
			gr.DrawBackground ();
			gr.Color = new Cairo.Color (0, 0, 0, 1);

			gr.MoveTo (x, y);
			gr.ShowPangoText (Catalog.GetString ("Score"), false, -1, 0);
			DrawBand (gr, 0.03, y - 0.01);

			y += 0.08;
			gr.MoveTo (x, y);

			if (session.GamesPlayed >= 10) {
				if (session.TotalScore >= 90)
					s = String.Format (Catalog.GetString ("Outstanding results"));
				else if (session.TotalScore >= 80) 
					s = String.Format (Catalog.GetString ("Excellent results"));
				else if (session.TotalScore >= 50) 
					s = String.Format (Catalog.GetString ("Good results"));
				else if (session.TotalScore >= 30) 
					s = String.Format (Catalog.GetString ("Poor results"));
				else s = String.Format (Catalog.GetString ("Disappointing results"));
			} else 
				s = String.Empty;

			gr.MoveTo (x, y);

			if (s == String.Empty)
				gr.ShowPangoText (String.Format (Catalog.GetString ("Games won: {0} ({1} played)"), session.GamesWon, session.GamesPlayed));	
			else 
				gr.ShowPangoText (String.Format (Catalog.GetString ("{0}. Games won: {1} ({2} played)"), s, session.GamesWon, session.GamesPlayed));	

			y += 0.06;
			gr.MoveTo (x, y);
			gr.ShowPangoText (String.Format (Catalog.GetString ("Time played {0} (average per game {1})"), session.GameTime, session.TimePerGame));
		
			y += 0.1;
			DrawGraphicBar (gr, x, y);
			y += 0.4;

			gr.MoveTo (x, y);
			gr.ShowPangoText (Catalog.GetString ("Tips for your next games"), false, -1, 0);
			DrawBand (gr, 0.03, y - 0.01);

			y += 0.08;

			for (int i = 0; i < tips_shown; i++)
			{
				y = gr.DrawStringWithWrapping (x, y,  "- " + GameTips.Tip);
				if (y > 0.88)
					break;

				y += space_small;
			}

			gr.Stroke ();
		}
	}
}
