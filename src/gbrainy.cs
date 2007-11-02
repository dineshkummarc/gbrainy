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

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Cairo;
using Gtk;
using Gdk;
using Gnome;
using Mono.Unix;


public class gbrainy: Program
{
	[Glade.Widget("gbrainy")] Gtk.Window app_window;
	[Glade.Widget] Box drawing_vbox;
	[Glade.Widget] Gtk.Label question_label;
	[Glade.Widget] Gtk.Label solution_label;
	[Glade.Widget] Gtk.Entry answer_entry;
	[Glade.Widget] Gtk.Button answer_button;
	[Glade.Widget] Gtk.Button tip_button;
	[Glade.Widget] Gtk.Button next_button;
	[Glade.Widget] Gtk.Statusbar statusbar;
	GameDrawingArea drawing_area;
	GameSession session;
	const int ok_buttonid = -5;

	public gbrainy (string [] args, params object [] props)
	: base ("gbrainy", Defines.VERSION, Modules.UI,  args, props)
	{
		Catalog.Init ("gbrainy", Defines.GNOME_LOCALE_DIR);

		Glade.XML gXML = new Glade.XML (null, "gbrainy.glade", "gbrainy", null);
		gXML.Autoconnect (this);
		

		session = new GameSession (this);		
		drawing_area = new GameDrawingArea ();
		drawing_vbox.Add (drawing_area);
		//app_window.Resize (500, 700);
		//app_window.SizeAllocated += new SizeAllocatedHandler (OnSizeAllocated);
		app_window.IconName = "gbrainy";
	        app_window.ShowAll ();		
		
		drawing_area.puzzle = null;
		question_label.Text = string.Empty;
		ActiveButtons (false);
		//OnMemoryOnly (this, EventArgs.Empty); // temp
	}

	public void UpdateStatusBar ()
	{
		statusbar.Push (0, session.StatusText);
	}

	public void ActiveButtons (bool active)
	{
		answer_button.Sensitive = active;
		next_button.Sensitive = active;

		if (active == true && session.CurrentGame != null && session.CurrentGame.ButtonsActive == true && session.CurrentGame.Tip != string.Empty)
			tip_button.Sensitive = true;
		else
			tip_button.Sensitive = false;
	}

	public void UpdateQuestion (string question)
	{
		question_label.Markup = "<span size='large' weight='bold'>" + question + "</span>";
	}

	public void QueueDraw ()
	{
		drawing_area.QueueDraw ();
	}
	

	private void GetNextGame ()
	{
		solution_label.Text = String.Empty;
		session.NextGame ();
		ActiveButtons (session.CurrentGame.ButtonsActive);
		drawing_area.puzzle = session.CurrentGame;
		UpdateQuestion (session.CurrentGame.Question);
		answer_entry.Text = string.Empty;
		UpdateStatusBar ();
		session.CurrentGame.DrawAnswer = false;
		drawing_area.QueueDraw ();
	}

	void OnMenuAbout (object sender, EventArgs args)
	{
		string [] authors = new string [] {
			"Jordi Mas i Hernandez <jmas@softcatala.org>",
		};

		// Name of persons or people that translated the application
		string translators = Catalog.GetString ("translator-credits");

		if (translators == "translator-credits")
			translators = null;

		Gtk.AboutDialog about = new Gtk.AboutDialog ();
		about.Name = "gbrainy";
		about.Version = Defines.VERSION;
		about.Authors = authors;
		about.Documenters = null;
		about.Logo = LoadFromAssembly ("gbrainy.svg");

		about.Copyright = "(c) 2007 Jordi Mas i Hernandez\n";
		about.Copyright += "Based on ideas by Terry Stickels, MENSA books and myself.\n";

		about.Comments = Catalog.GetString ("A brain teaser and trainer game to have fun and to keep your brain trained.");
		about.Website = "http://live.gnome.org/gbrainy";
		about.WebsiteLabel = Catalog.GetString ("gbrainy web site");
		about.TranslatorCredits = translators;
		about.IconName = null;
		about.Run ();
		about.Destroy ();
	}

	static public Pixbuf LoadFromAssembly (string resource)
	{
		try {
			return new Pixbuf (System.Reflection.Assembly.GetEntryAssembly (), resource);
		} catch {
			return null;
		}
	}

	void OnAnswerButtonClicked (object sender, EventArgs args)
	{
		string answer;
		if (session.CurrentGame == null)
			return;
	
		if (answer_button.Sensitive == true && session.CurrentGame.CheckAnswer (answer_entry.Text) == true) {
			session.GamesWon++;
			answer = "<span color ='#00A000'>" + Catalog.GetString ("Congratulations.") + "</span>";
		} else
			answer = Catalog.GetString ("Incorrect answer.");

		session.EnableTimer = false;
		answer_entry.Text = String.Empty;
		UpdateStatusBar ();
		answer_button.Sensitive = false;
		solution_label.Markup = answer + " " + session.CurrentGame.Answer;
		session.CurrentGame.DrawAnswer = true;
		drawing_area.QueueDraw ();
	}		

	void OnQuit (object sender, EventArgs args)
	{
		Quit ();	
	}	

	void OnDeleteWindow (object sender, DeleteEventArgs args)
	{
		Quit ();	
	}	

	void OnNextButtonClicked (object sender, EventArgs args)
	{
		if (answer_entry.Text.Length > 0) {
			OnAnswerButtonClicked (sender, args);
			return;
		}

		GetNextGame ();
		session.EnableTimer = true;
	}

	void OnTip (object sender, EventArgs args)
	{
		if (session.CurrentGame == null)
			return;

		solution_label.Text = session.CurrentGame.Tip;
	}

	void OnNewGame ()
	{
		session.NewSession ();
		GetNextGame ();
		solution_label.Text = Catalog.GetString ("Once you have an answer type it in \"Your answer:\" entry box and press the \"Ok\" button.");
		UpdateStatusBar ();
	}

	void OnMathOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.MathTrainers;
		OnNewGame ();
	}

	void OnMemoryOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.MemoryTrainers;
		OnNewGame ();
	}

	void OnCustomGame (object sender, EventArgs args)
	{
		int rslt;
		CustomGameDialog dialog;

		dialog = new CustomGameDialog (session.GameManager);		
		rslt = (int) dialog.Run ();
		dialog.Dialog.Destroy ();

		if (rslt == ok_buttonid && dialog.NumOfGames > 0) {
			session.Type = GameSession.Types.Custom;
			OnNewGame ();
		}
	}

	void OnLogicOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.LogicPuzzles;
		OnNewGame ();
	}

	void OnAllGames (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.AllGames;
		OnNewGame ();		
	}

	void OnTrainersOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.TrainersOnly;
		OnNewGame ();		
	}

	void OnAnswerActivate (object sender, EventArgs args)
	{
		if (answer_entry.Text.Length > 0) {
			OnAnswerButtonClicked (sender, args);
			return;
		}
	}

	void OnEndGame (object sender, EventArgs args)
	{
		GameOverDialog dialog;
		dialog = new GameOverDialog ();
		dialog.TimePayed = session.TimePlayed;
		dialog.GamesPlayed = String.Format (Catalog.GetString ("{0} ({1} won)"), session.GamesPlayed, session.GamesWon);
		dialog.TimePerGame = session.TimePerGame;
	
		session.EndSession ();
		drawing_area.puzzle = null;
		question_label.Text = string.Empty;
		solution_label.Text = string.Empty;
		UpdateStatusBar ();
		drawing_area.QueueDraw ();
		ActiveButtons (false);
		
		dialog.Run ();
		dialog.Dialog.Destroy ();
	}

	void OnPauseGame (object sender, EventArgs args)
	{
		if (session.Paused) {
 			session.Resume ();
			ActiveButtons (true);
		} else {
			session.Pause ();
			ActiveButtons (false);
		}
		UpdateStatusBar ();
	}

	private void OnSizeAllocated (object obj, SizeAllocatedArgs args)
	{
		//Console.WriteLine ("OnSizeAllocated");
	}
	
	public static void Main (string [] args) 
	{
		gbrainy gui = new gbrainy (args);
		gui.Run ();	
	}
}



