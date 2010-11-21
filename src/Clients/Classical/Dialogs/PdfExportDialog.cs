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
using System.IO;
using Gtk;
using Mono.Unix;

using gbrainy.Core.Main;
using gbrainy.Clients.Classical.Widgets;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class PdfExportDialog : BuilderDialog
	{
		[GtkBeans.Builder.Object] Gtk.HBox hbox_file;
		[GtkBeans.Builder.Object] Gtk.SpinButton games_spinbutton;	
		[GtkBeans.Builder.Object] Gtk.SpinButton gamesperpage_spinbutton;
		[GtkBeans.Builder.Object] Gtk.CheckButton colorblindcheckbutton;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_easy;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_medium;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_master;
		[GtkBeans.Builder.Object] Gtk.CheckButton checkbox_logic;
		[GtkBeans.Builder.Object] Gtk.CheckButton checkbox_calculation;
		[GtkBeans.Builder.Object] Gtk.CheckButton checkbox_verbal;
	
		BrowseFile file;

		public PdfExportDialog () : base ("PdfExportDialog.ui", "pdfexportbox")
		{
			games_spinbutton.Value = 10;
			gamesperpage_spinbutton.Value = 4;
			checkbox_logic.Active = checkbox_calculation.Active = checkbox_verbal.Active = true;

			// Use defaults from Preferences
		 	switch ((GameDifficulty) Preferences.GetIntValue (Preferences.DifficultyKey)) {
			case GameDifficulty.Easy:	
				rb_easy.Active = rb_easy.HasFocus = true;
				break;
			case GameDifficulty.Medium:
				rb_medium.Active = rb_medium.HasFocus = true;
				break;
			case GameDifficulty.Master:
				rb_master.Active = rb_master.HasFocus = true;
				break;
			}
			// File selection
			string def_file;

			def_file = System.IO.Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments),
				"games.pdf");

			file = new BrowseFile (hbox_file, def_file, true);

			FileFilter[] filters = new FileFilter [2];
			filters[0] = new FileFilter ();
			filters[0].AddPattern ("*.pdf");
			filters[0].Name = Catalog.GetString ("PDF files");

			filters[1] = new FileFilter ();
			filters[1].AddPattern ("*.*");
			filters[1].Name = Catalog.GetString ("All files");

			file.Filters = filters;
		}

		GameDifficulty Difficulty {
			get {
				if (rb_easy.Active)
					return GameDifficulty.Easy;

				if (rb_master.Active)
					return GameDifficulty.Master;

				return GameDifficulty.Medium;
			}
		}

		void OnOK (object sender, EventArgs args)
		{
			GameSession.Types types = GameSession.Types.None;

			if (checkbox_logic.Active)
				types |= GameSession.Types.LogicPuzzles;

			if (checkbox_calculation.Active)
				types |= GameSession.Types.CalculationTrainers;

			if (checkbox_verbal.Active)
				types |= GameSession.Types.VerbalAnalogies; 
			
			GeneratePdf (types,
				(int) games_spinbutton.Value,
				(int) gamesperpage_spinbutton.Value, 
				Difficulty,
				colorblindcheckbutton.Active,
				file.Filename);
		}

		void GeneratePdf (GameSession.Types types, int num_games, int gamespage, GameDifficulty difficulty, bool colorblind, string filename)
		{
			Game [] games;
			GameManager gm;
			
			games = new Game [num_games];
			gm = new GameManager ();
			gm.ColorBlind = colorblind;
			gm.Difficulty = difficulty;
			GtkClient.GameManagerPreload (gm);
			gm.GameType = types;
		
			for (int n = 0; n < num_games; n++)
			{
				 games [n] = gm.GetPuzzle (); 
			}
			
			PdfExporter.GeneratePdf (games, gamespage, filename);
		}
	}
}
