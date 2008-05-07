/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Glade;
using Gtk;
using Mono.Unix;
using System.Collections;

public class PlayerHistoryDialog : GtkDialog
{
	[Glade.Widget] Box history_preview;
	[Glade.Widget] Label label_playerhistory;
	[Glade.Widget] Gtk.CheckButton checkbutton_total;
	[Glade.Widget] Gtk.CheckButton checkbutton_memory;
	[Glade.Widget] Gtk.CheckButton checkbutton_logic;
	[Glade.Widget] Gtk.CheckButton checkbutton_calculation;

	CairoPreview drawing_area;

	public PlayerHistoryDialog () : base ("playerhistory")
	{
		label_playerhistory.Text = String.Format (Catalog.GetString ("The graphic below shows the player's game score evolution. It is built using the results of {0} last recorded games."), gbrainy.history.Games.Count);

		drawing_area = new CairoPreview (this);
		history_preview.Add (drawing_area);
		drawing_area.Visible = true;
 		checkbutton_total.Active = checkbutton_memory.Active = checkbutton_logic.Active = checkbutton_calculation.Active = true;
	}
	
	void OnTotalToggled (object sender, EventArgs args)
	{
		drawing_area.QueueDraw ();
	}

	void OnLogicToggled (object sender, EventArgs args)
	{
		drawing_area.QueueDraw ();
	}

	void OnMemoryToggled (object sender, EventArgs args)
	{
		drawing_area.QueueDraw ();
	}

	void OnCalculationToggled (object sender, EventArgs args)
	{
		drawing_area.QueueDraw ();
	}

	public class CairoPreview : DrawingArea 
	{
		const double area_h = 0.8, area_w = 0.9, point_size = 0.005 * 1.25;
		Cairo.Color math_color = new Cairo.Color (0.56, 0.71, 0.20);    // 8fb735
		Cairo.Color logic_color = new Cairo.Color (0.81, 0.54, 0.23);   // d18c3b
		Cairo.Color memory_color = new Cairo.Color (0.73, 0.22, 0.51);  // bb3a84
		Cairo.Color total_color = new Cairo.Color (0, 0, 0.6);
		Cairo.Color text_color = new Cairo.Color (0, 0, 0);
		Cairo.Color axis_color = new Cairo.Color (0.15, 0.15, 0.15);
		PlayerHistoryDialog dlg;
	
		public CairoPreview (PlayerHistoryDialog dlg)
		{
			this.dlg = dlg;
		}

		private void DrawLegend (CairoContextEx cr, double x, double y)
		{
			double line_size = 0.05, offset_x = 0.01, second_row = 0.05;
			
			cr.Color = total_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y + 0.01);
			cr.ShowText (Catalog.GetString ("Total"));
			cr.Stroke ();

			cr.Color = logic_color;
			cr.MoveTo (x, y + second_row);
			cr.LineTo (x + line_size, y + second_row);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y + 0.01 + second_row);
			cr.ShowText (Catalog.GetString ("Logic"));
			cr.Stroke ();

			x += 0.5;
			cr.Color = memory_color;
			cr.MoveTo (x, y);
			cr.LineTo (x + line_size, y);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y + 0.01);
			cr.ShowText (Catalog.GetString ("Memory"));
			cr.Stroke ();

			cr.Color = math_color;
			cr.MoveTo (x, y + second_row);
			cr.LineTo (x + line_size, y + second_row);
			cr.Stroke ();
			cr.Color = text_color;
			cr.MoveTo (x + line_size + offset_x, y + 0.01 + second_row);
			cr.ShowText (Catalog.GetString ("Calculation"));
			cr.Stroke ();
		}

		private void DrawLines (CairoContextEx cr, double x, double y)
		{
			double px, py;
			PlayerHistory history = gbrainy.history;
			double ratio = (area_w / (history.Games.Count - 1)); 

			if (history.Games.Count == 0)
				return;
			
			if (dlg.checkbutton_logic.Active) { // Logic
				cr.Color = logic_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].logic_score / 100));
				for (int i = 1; i < history.Games.Count; i++)
				{
					px = x + (ratio * i);
					py = y + area_h - (area_h * history.Games[i].logic_score / 100);
					cr.LineTo (px, py);
				}
				cr.Stroke ();
			}

			if (dlg.checkbutton_calculation.Active) { // Math
				cr.Color = math_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].math_score / 100));
				for (int i = 1; i < history.Games.Count; i++)
				{
					px = x + (ratio * i);
					py = y + area_h - (area_h * history.Games[i].math_score / 100);
					cr.LineTo (px, py);
				}
				cr.Stroke ();
			}
	
			if (dlg.checkbutton_memory.Active) { // Memory
				cr.Color = memory_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].memory_score / 100));
				for (int i = 1; i < history.Games.Count; i++)
				{
					px = x + (ratio * i);
					py = y + area_h - (area_h * history.Games[i].memory_score / 100);
					cr.LineTo (px, py);
				}
				cr.Stroke ();
			}

			if (dlg.checkbutton_memory.Active) { // Total			
				cr.Color = total_color;
				cr.MoveTo (x, area_h - (area_h * history.Games[0].total_score / 100));
				for (int i = 1; i < history.Games.Count; i++)
				{
					px = x + (ratio * i);
					py = y + area_h - (area_h * history.Games[i].total_score / 100);
					cr.LineTo (px, py);
				}
				cr.Stroke ();
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			if(!IsRealized)
				return false;

			int w, h, nw, nh;
			double x = 0, y = 0;

			Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
			CairoContextEx cr = new CairoContextEx (cc.Handle);   
			args.Window.GetSize (out w, out h);

			nh = nw = Math.Min (w, h);

			if (nw < w) {
				x = (w - nw) / 2;
			}

			if (nh < h) {
				y = (h - nh) / 2;
			}
		
			cr.Translate (x, y);
			cr.Scale (nw, nh);
			cr.SetNormalFont ();

			// Background
			cr.Color = new Cairo.Color (1, 1, 1);
			cr.Paint ();
			cr.Stroke ();

			x = 0.05; 
			y = 0.05;
			cr.LineWidth = point_size;
			cr.Color = axis_color;

			DrawLines (cr, x, y);
			DrawLegend (cr, x, y + area_h + 0.05);

			// Draw Axis
			cr.MoveTo (x, y);
			cr.LineTo (x, y + area_h);
			cr.LineTo (x + area_w, y + area_h);
			cr.Stroke ();

			((IDisposable)cc).Dispose();
			((IDisposable)cr).Dispose();
   			return base.OnExposeEvent(args);
		}
	}	
}
