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
	[Glade.Widget("gBrainy")] Gtk.Window app_window;
	[Glade.Widget("drawing_vbox")] Box drawing_vbox;
	[Glade.Widget("question_label")] Gtk.Label question_label;
	[Glade.Widget("solution_label")] Gtk.Label solution_label;
	[Glade.Widget] Gtk.Entry answer_entry;
	[Glade.Widget] Gtk.Label numgames_label;
	[Glade.Widget] Gtk.Button answer_button;
	[Glade.Widget] Gtk.Button tip_button;
	[Glade.Widget] Gtk.Button next_button;
	[Glade.Widget] AppBar appbar;
	CairoGraphic drawing_area;
	GameSession session;

	public gbrainy (string [] args, params object [] props)
	: base ("gbrainy", Defines.VERSION, Modules.UI,  args, props)
	{
		Catalog.Init ("gbrainy", Defines.GNOME_LOCALE_DIR);

		Glade.XML gXML = new Glade.XML (null, "gbrainy.glade", "gBrainy", null);
		gXML.Autoconnect (this);
		

		session = new GameSession (this);		
		drawing_area = new CairoGraphic ();
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
		appbar.SetStatus (session.StatusText);
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

		about.Comments = Catalog.GetString ("A brain teaser and trainner game to have fun and to keep your brain trained.");
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
		if (session.CurrentGame == null)
			return;
	
		if (session.CurrentGame.CheckAnswer (answer_entry.Text) == true) {
			session.GamesWon++;
		}

		session.EnableTimer = false;
		UpdateStatusBar ();
		answer_button.Sensitive = false;
		solution_label.Text = session.CurrentGame.Answer;
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
		solution_label.Text = Catalog.GetString ("Once you have an answer type it in \"Your Answer:\" entry box and press the \"Ok\" button.");
		session.NewSession ();
		GetNextGame ();		
		UpdateStatusBar ();
	}

	void OnMathOnly (object sender, EventArgs args)
	{
		session.GameType = GameType.MathTrainers;
		OnNewGame ();
	}

	void OnMemoryOnly (object sender, EventArgs args)
	{
		session.GameType = GameType.MemoryTrainers;
		OnNewGame ();
	}

	void OnLogicOnly (object sender, EventArgs args)
	{
		session.GameType = GameType.LogicPuzzles;
		OnNewGame ();
	}

	void OnAllGames (object sender, EventArgs args)
	{
		session.GameType = GameType.AllGames;
		OnNewGame ();		
	}

	void OnTrainersOnly (object sender, EventArgs args)
	{
		session.GameType = GameType.TrainersOnly;
		OnNewGame ();		
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

	public class CairoGraphic : DrawingArea 
	{
		public Game puzzle = null;

		private void DrawBackground (Cairo.Context gr)
		{
			int columns = 40;
			int rows = 40;
			double rect_w = 1.0 / rows;
			double rect_h = 1.0 / columns;

			gr.Save ();

			gr.Color = new Cairo.Color (1, 1, 1);
			gr.Paint ();	
	
			gr.Color = new Cairo.Color (0.8, 0.8, 0.8);
			gr.LineWidth = 0.001;
			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {			
					gr.Rectangle (row * rect_w, column * rect_h, rect_w, rect_h);
				}
			}
			gr.Stroke ();
			gr.Restore ();		
		}

		private void DrawWelcome (Cairo.Context gr, int area_width, int area_height)
		{
			gr.Scale (area_width, area_height);
			DrawBackground (gr);

			gr.Color = new Cairo.Color (0.1, 0.1, 0.1);
			gr.SelectFontFace ("Sans", Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
			gr.SetFontSize (0.04);

			gr.MoveTo (0.2, 0.2);
			gr.ShowText (Catalog.GetString ("Welcome to gbrainy") + " " + Defines.VERSION);
			gr.Stroke ();
			
			gr.MoveTo (0.1, 0.4);
			gr.ShowText (Catalog.GetString ("Use the Game menu to start a new game"));
			gr.Stroke ();

		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			if(!IsRealized)
				return false;
   
			int w, h;
			args.Window.GetSize (out w, out h);
			Cairo.Context cr = Gdk.CairoHelper.Create (args.Window);
		
			if (puzzle == null)
				DrawWelcome (cr, w, h);
			else
				puzzle.Draw (cr, w, h);

   			((IDisposable)cr).Dispose();
   			return base.OnExposeEvent(args);
		}

	}
}



