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
	public class PlayerHistoryView : IDrawable
	{
		const double area_h = 0.8, area_w = 0.9, point_size = 0.005 * 1.25;
		Cairo.Color math_color = new Cairo.Color (0.56, 0.71, 0.20);    // 8fb735
		Cairo.Color logic_color = new Cairo.Color (0.81, 0.54, 0.23);   // d18c3b
		Cairo.Color memory_color = new Cairo.Color (0.73, 0.22, 0.51);  // bb3a84
		Cairo.Color verbal_color = new Cairo.Color (0.68, 0.16, 0.17);  // af2b2c
		Cairo.Color total_color = new Cairo.Color (0, 0, 0.6);
		Cairo.Color text_color = new Cairo.Color (0, 0, 0);
		Cairo.Color axis_color = new Cairo.Color (0.15, 0.15, 0.15);
		PlayerHistory history;

		public PlayerHistoryView (PlayerHistory history)
		{
			this.history = history;
			ShowLogic = ShowMemory = ShowCalculation = ShowVerbal = true;
		}

		public bool ShowTotal { get; set; }
		public bool ShowLogic { get; set; }
		public bool ShowMemory { get; set; }
		public bool ShowCalculation { get; set; }
		public bool ShowVerbal { get; set; }
		
		void DrawLegend (CairoContextEx cr, double x, double y)
		{
			const double line_size = 0.05, offset_x = 0.01, second_row = 0.05, space_hor = 0.4;
			double old_width;

			old_width = cr.LineWidth;
			cr.LineWidth = 0.01;
	
			cr.Color = total_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01);
			cr.ShowPangoText (Catalog.GetString ("Total"));
			cr.Stroke ();

			cr.Color = logic_color;
			cr.MoveTo (x, y + second_row);
			cr.LineTo (x + line_size, y + second_row);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01 + second_row);
			cr.ShowPangoText (Game.GetGameTypeDescription (Game.Types.LogicPuzzle));
			cr.Stroke ();

			x += space_hor;
			cr.Color = memory_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01);
			cr.ShowPangoText (Game.GetGameTypeDescription (Game.Types.MemoryTrainer));
			cr.Stroke ();

			cr.Color = math_color;
			cr.MoveTo (x, y + second_row);
			cr.LineTo (x + line_size, y + second_row);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01 + second_row);
			cr.ShowPangoText (Game.GetGameTypeDescription (Game.Types.MathTrainer));
			cr.Stroke ();

			x += space_hor;
			cr.Color = verbal_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y - 0.01);
			cr.ShowPangoText (Game.GetGameTypeDescription (Game.Types.VerbalAnalogy));
			cr.Stroke ();

			cr.LineWidth = old_width;
		}

		void DrawLines (CairoContextEx cr, double x, double y)
		{
			double px, py;
			double ratio;
			int pos;

			if (history.Games.Count == 0)
				return;

			ratio = area_w / (history.Games.Count - 1);
	
			if (ShowLogic) {
				cr.Color = logic_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].logic_score / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].logic_score < 0)
						continue;

					px = x + (ratio * pos);
					py = y + area_h - (area_h * history.Games[i].logic_score / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowCalculation) {
				cr.Color = math_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].math_score / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].math_score < 0)
						continue;

					px = x + (ratio * pos);
					py = y + area_h - (area_h * history.Games[i].math_score / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowMemory) {
				cr.Color = memory_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].memory_score / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].memory_score < 0)
						continue;

					px = x + (ratio * pos);
					py = y + area_h - (area_h * history.Games[i].memory_score / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowVerbal) {
				cr.Color = verbal_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].verbal_score / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[i].verbal_score < 0)
						continue;

					px = x + (ratio * i);
					py = y + area_h - (area_h * history.Games[i].verbal_score / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}

			if (ShowTotal) {
				cr.Color = total_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].total_score / 100));

				pos = 1;
				for (int i = 1; i < history.Games.Count; i++)
				{
					if (history.Games[pos].total_score < 0)
						continue;

					px = x + (ratio * pos);
					py = y + area_h - (area_h * history.Games[i].total_score / 100);
					cr.LineTo (px, py);
					pos++;
				}
				cr.Stroke ();
			}
		}

		void DrawGrid (CairoContextEx cr, double x, double y)
		{
			// Draw Axis
			cr.MoveTo (x, y);
			cr.LineTo (x, y + area_h);
			cr.LineTo (x + area_w, y + area_h);
			cr.Stroke ();

			cr.Color = new Cairo.Color (0.8, 0.8, 0.8);
			cr.LineWidth = 0.001;
			for (double line_y = y; line_y < area_h + y; line_y += area_h / 10) {
				cr.MoveTo (x, line_y);
				cr.LineTo (x + area_w, line_y);
				cr.Stroke ();
			}
		}		

		public void Draw (CairoContextEx cr, int width, int height, bool rtl)
		{
			double x, y;

			// Background
			cr.Color = new Cairo.Color (1, 1, 1);
			cr.Paint ();
			cr.Stroke ();

			x = 0.05; 
			y = 0.05;
			cr.LineWidth = point_size;
			cr.Color = axis_color;

			cr.Rectangle (x, y, area_w, area_h);
			cr.Clip ();
			DrawLines (cr, x, y);
			cr.ResetClip ();
			DrawLegend (cr, x, y + area_h + 0.05);
			DrawGrid (cr, x, y);
		}
	}
}
