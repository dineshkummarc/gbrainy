/*
 * Copyright (C) 2008-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Mono.Unix;
using System.Collections;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;
using gbrainy.Core.Views;

namespace gbrainy.Clients.Classical
{
	public class PlayerHistoryDialog : GtkDialog
	{
		[Glade.Widget] Box history_preview;
		[Glade.Widget] Label label_playerhistory;
		[Glade.Widget] Gtk.CheckButton checkbutton_total;
		[Glade.Widget] Gtk.CheckButton checkbutton_memory;
		[Glade.Widget] Gtk.CheckButton checkbutton_logic;
		[Glade.Widget] Gtk.CheckButton checkbutton_calculation;
		[Glade.Widget] Gtk.CheckButton checkbutton_verbal;

		CairoPreview drawing_area;
		PlayerHistory history;

		public PlayerHistoryDialog (PlayerHistory history) : base ("playerhistory")
		{
			string label;

			this.history = history;
			label = Catalog.GetString ("The graphic below shows the player's game score evolution. ");
			label +=  Catalog.GetPluralString ("You need more than one game recorded to see the score evolution.",
				"It is built using the results of {0} last recorded games.", 
				PlayerHistory.Games.Count < 2 ? 1 : 2);

			label_playerhistory.Text = String.Format (label, PlayerHistory.Games.Count);

			drawing_area = new CairoPreview (this);
			history_preview.Add (drawing_area);
			drawing_area.Visible = true;

	 		checkbutton_total.Label = Catalog.GetString ("Total");
	 		checkbutton_logic.Label = Game.GetGameTypeDescription (Game.Types.LogicPuzzle);
	 		checkbutton_calculation.Label = Game.GetGameTypeDescription (Game.Types.MathTrainer);
	 		checkbutton_memory.Label = Game.GetGameTypeDescription (Game.Types.MemoryTrainer);
	 		checkbutton_verbal.Label = Game.GetGameTypeDescription (Game.Types.VerbalAnalogy);

	 		checkbutton_total.Active = checkbutton_memory.Active = checkbutton_logic.Active = checkbutton_calculation.Active = checkbutton_verbal.Active = true;
		}

		public PlayerHistory PlayerHistory {
			get {return history; }
		}

		void OnTotalToggled (object sender, EventArgs args)
		{
			drawing_area.View.ShowTotal = checkbutton_total.Active;
			drawing_area.QueueDraw ();
		}

		void OnLogicToggled (object sender, EventArgs args)
		{
			drawing_area.View.ShowLogic = checkbutton_logic.Active;
			drawing_area.QueueDraw ();
		}

		void OnMemoryToggled (object sender, EventArgs args)
		{
			drawing_area.View.ShowMemory = checkbutton_memory.Active;
			drawing_area.QueueDraw ();
		}

		void OnCalculationToggled (object sender, EventArgs args)
		{
			drawing_area.View.ShowCalculation = checkbutton_calculation.Active;
			drawing_area.QueueDraw ();
		}

		void OnVerbalToggled (object sender, EventArgs args)
		{
			drawing_area.View.ShowVerbal = checkbutton_verbal.Active;
			drawing_area.QueueDraw ();
		}

		public class CairoPreview : DrawingArea 
		{
			PlayerHistoryDialog dlg;
			PlayerHistoryView view;

			public CairoPreview (PlayerHistoryDialog dlg)
			{
				this.dlg = dlg;
				view = new PlayerHistoryView (dlg.PlayerHistory);
			}

			public PlayerHistoryView View {
				get { return view; }
			}
	
			protected override bool OnExposeEvent (Gdk.EventExpose args)
			{
				if(!IsRealized)
					return false;

				int w, h, nw, nh;
				double x = 0, y = 0;

				Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
				CairoContextEx cr = new CairoContextEx (cc.Handle, this);
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

				view.Draw (cr, nw, nh, Direction == Gtk.TextDirection.Rtl);

				((IDisposable)cc).Dispose();
				((IDisposable)cr).Dispose();
	   			return base.OnExposeEvent(args);
			}
		}	
	}
}
