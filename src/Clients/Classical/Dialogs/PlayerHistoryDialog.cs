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

using gbrainy.Core.Main;
using gbrainy.Core.Views;
using gbrainy.Core.Services;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class PlayerHistoryDialog : BuilderDialog
	{
		[Builder.Object] Box history_preview;
		[Builder.Object] Label label_playerhistory;
		[Builder.Object] Gtk.CheckButton checkbutton_total;
		[Builder.Object] Gtk.CheckButton checkbutton_memory;
		[Builder.Object] Gtk.CheckButton checkbutton_logic;
		[Builder.Object] Gtk.CheckButton checkbutton_calculation;
		[Builder.Object] Gtk.CheckButton checkbutton_verbal;

		CairoPreview drawing_area;

		public PlayerHistoryDialog (ITranslations translations, PlayerHistory history) : base (translations, "PlayerHistoryDialog.ui", "playerhistory")
		{
			string intro, built;

			intro = Catalog.GetString ("The graph below shows the player's game score evolution.");

			if (history.Games.Count < 2)
			{
				built = Catalog.GetString ("You need more than one game session recorded to see the score evolution.");
			}
			else 
			{
				built =  String.Format (Catalog.GetPluralString ("It is built using the results of {0} recorded game session.",
					"It is built using the results of the last {0} recorded game sessions.",
					history.Games.Count),
					history.Games.Count);	
			}

			// Translators: "The graph below" +  "It is built using" sentences
			label_playerhistory.Text = String.Format (Catalog.GetString ("{0} {1}"), intro, built);

			drawing_area = new CairoPreview (translations, history);
			history_preview.Add (drawing_area);
			drawing_area.Visible = true;

	 		checkbutton_total.Label = Catalog.GetString ("Total");
	 		checkbutton_logic.Label = GameTypesDescription.GetLocalized (translations, GameTypes.LogicPuzzle);
	 		checkbutton_calculation.Label = GameTypesDescription.GetLocalized (translations, GameTypes.Calculation);
	 		checkbutton_memory.Label = GameTypesDescription.GetLocalized (translations, GameTypes.Memory);
	 		checkbutton_verbal.Label = GameTypesDescription.GetLocalized (translations, GameTypes.VerbalAnalogy);

	 		checkbutton_total.Active = checkbutton_memory.Active = checkbutton_logic.Active = checkbutton_calculation.Active = checkbutton_verbal.Active = true;
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

		class CairoPreview : DrawingArea
		{
			PlayerHistoryView view;

			public CairoPreview (ITranslations translations, PlayerHistory history)
			{
				view = new PlayerHistoryView (translations, history);
			}

			public PlayerHistoryView View {
				get { return view; }
			}

			protected override bool OnDrawn (Cairo.Context cc)
			{
				if(!IsRealized)
					return false;

				int w, h, nw, nh;
				double x = 0, y = 0;

				CairoContextEx cr = new CairoContextEx (cc.Handle);
				cr.PangoFontDescription = PangoContext.FontDescription;

				w = Window.Width;
				h = Window.Height;

				nh = nw = Math.Min (w, h);

				if (nw < w) {
					x = (w - nw) / 2d;
				}

				if (nh < h) {
					y = (h - nh) / 2d;
				}

				cr.Translate (x, y);
				cr.Scale (nw, nh);

				view.Draw (cr, nw, nh, Direction == Gtk.TextDirection.Rtl);

				((IDisposable)cr).Dispose();
				return true;
			}
		}
	}
}
