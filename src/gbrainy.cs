/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Text;


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
	[Glade.Widget] Gtk.Toolbar toolbar;
	GameDrawingArea drawing_area;
	GameSession session;
	const int ok_buttonid = -5;
	ToolButton pause_tbbutton;
	public static PlayerHistory history = null;
	public static Preferences preferences = null;
 
	public gbrainy (string [] args, params object [] props)
	: base ("gbrainy", Defines.VERSION, Modules.UI,  args, props)
	{
		Catalog.Init ("gbrainy", Defines.GNOME_LOCALE_DIR);

		IconFactory icon_factory = new IconFactory ();
                AddIcon (icon_factory, "logic-games", "logic-games-32.png");
		AddIcon (icon_factory, "math-games", "math-games-32.png");
		AddIcon (icon_factory, "memory-games", "memory-games-32.png");
		AddIcon (icon_factory, "pause", "pause-32.png");
		AddIcon (icon_factory, "resume", "resume-32.png");
		AddIcon (icon_factory, "endgame", "endgame-32.png");
		AddIcon (icon_factory, "allgames", "allgames-32.png");
		icon_factory.AddDefault ();

		Glade.XML gXML = new Glade.XML (null, "gbrainy.glade", "gbrainy", null);
		gXML.Autoconnect (this);

		toolbar.IconSize = Gtk.IconSize.Dnd;
	
		Tooltips tooltips = new Tooltips ();

		ToolButton button = new ToolButton ("allgames");
		button.SetTooltip (tooltips, Catalog.GetString ("Play all the games"), null);
		button.Label = Catalog.GetString ("All");
		button.Clicked += OnAllGames;
		toolbar.Insert (button, -1);

		button = new ToolButton ("logic-games");
		button.SetTooltip (tooltips, Catalog.GetString ("Play games that challenge your reasoning and thinking"), null);
		button.Label = Catalog.GetString ("Logic");
		button.Clicked += OnLogicOnly;
		toolbar.Insert (button, -1);

		button = new ToolButton ("math-games");
		button.Label = Catalog.GetString ("Calculation");
		button.SetTooltip (tooltips, Catalog.GetString ("Play games that challenge your mental calculation skills"), null);
		button.Clicked += OnMathOnly;
		toolbar.Insert (button, -1);

		button = new ToolButton ("memory-games");
		button.Label = Catalog.GetString ("Memory");
		button.SetTooltip (tooltips, Catalog.GetString ("Play games that challenge your short term memory"), null);
		button.Clicked += OnMemoryOnly;
		toolbar.Insert (button, -1);

		pause_tbbutton = new ToolButton ("pause");
		pause_tbbutton.Label = Catalog.GetString ("Pause");
		pause_tbbutton.SetTooltip (tooltips, Catalog.GetString ("Pause the game"), null);
		pause_tbbutton.Clicked += OnPauseGame;
		toolbar.Insert (pause_tbbutton, -1);

		button = new ToolButton ("endgame");
		button.SetTooltip (tooltips, Catalog.GetString ("End the game and show score"), null);
		button.Label = Catalog.GetString ("Finish");
		button.Clicked += OnEndGame;
		toolbar.Insert (button, -1);

		session = new GameSession (this);	

		if (history == null)
			history = new PlayerHistory ();

		if (preferences == null)
			preferences = new Preferences ();

		drawing_area = new GameDrawingArea ();
		drawing_vbox.Add (drawing_area);
		//app_window.Resize (500, 700);
		//app_window.SizeAllocated += new SizeAllocatedHandler (OnSizeAllocated);
		app_window.IconName = "gbrainy";
		app_window.ShowAll ();

		question_label.Text = string.Empty;
		ActiveInputControls (false);
		//OnMemoryOnly (this, EventArgs.Empty); // temp
	}

	public void UpdateStatusBar ()
	{
		statusbar.Push (0, session.StatusText);
	}

	public void ActiveInputControls (bool active)
	{
		bool answer, entry, next, tip;

		answer = entry = next = tip = active;

		if (active == true && session.CurrentGame != null && session.CurrentGame.ButtonsActive == true && session.CurrentGame.Tip != string.Empty)
			tip = true;
		else
			tip = false;
	
		switch (session.Status) {
		case GameSession.SessionStatus.NotPlaying:
			answer = false;
			entry =  false;
			next = false;
			tip = false;
			break;
		case GameSession.SessionStatus.Playing:
			break;
		case GameSession.SessionStatus.Answered:
			answer = false;
			entry =  false;
			tip = false;
			break;
		}

		answer_button.Sensitive = answer;
		answer_entry.Sensitive = entry;
		next_button.Sensitive = next;
		tip_button.Sensitive = tip;

		if (entry == true)
			answer_entry.GrabFocus ();
	}

	public void UpdateQuestion (string question)
	{
		question_label.Text = question;
	}

	public void QueueDraw ()
	{
		drawing_area.QueueDraw ();
	}

	void OnNextGameAfterCountDown (object source, EventArgs e)
	{
		drawing_area.mode = GameDrawingArea.Modes.Puzzle;
		ActiveInputControls (session.CurrentGame.ButtonsActive);
		drawing_area.puzzle = session.CurrentGame;		
		UpdateQuestion (session.CurrentGame.Question);
		answer_entry.Text = string.Empty;
		UpdateStatusBar ();
		session.CurrentGame.DrawAnswer = false;
		drawing_area.QueueDraw ();

		if (session.CurrentGame as Memory != null)
			(session.CurrentGame as Memory).StartTimer ();
	}	

	private void GetNextGame ()
	{
		solution_label.Text = String.Empty;
		UpdateQuestion (String.Empty);
		session.NextGame ();
		
		if (preferences.GetBoolValue (Preferences.MemQuestionWarnKey) && session.Type != GameSession.Types.MemoryTrainers && ((session.CurrentGame as Memory)  != null)) {
			ActiveInputControls (false);
			drawing_area.OnDrawCountDown (OnNextGameAfterCountDown);
		}
		else
			OnNextGameAfterCountDown (this, EventArgs.Empty);
	}
	
	void OnMenuAbout (object sender, EventArgs args)
	{
		Gtk.AboutDialog about = new Gtk.AboutDialog ();
		StringBuilder license = new StringBuilder (256);
		string [] authors = new string [] {
			"Jordi Mas i Hernandez <jmas@softcatala.org>",
		};

		// Name of the people that translated the application
		string translators = Catalog.GetString ("translator-credits");

		if (translators == "translator-credits")
			translators = null;

		license.Append (Catalog.GetString ("This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as  published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.\n\n"));
		license.Append (Catalog.GetString ("This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.\n\n"));
		license.Append (Catalog.GetString ("You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA."));

		about.Name = "gbrainy";
		about.Version = Defines.VERSION;
		about.Authors = authors;
		about.Documenters = null;
		about.Logo = LoadFromAssembly ("gbrainy.svg");

		about.Copyright = "(c) 2007-2008 Jordi Mas i Hernandez\n";
		about.Copyright += Catalog.GetString ("Based on ideas by Terry Stickels, MENSA books and Jordi Mas.");

		about.Comments = Catalog.GetString ("A brain teaser and trainer game to have fun and to keep your brain trained.");
		about.Website = "http://live.gnome.org/gbrainy";
		about.WebsiteLabel = Catalog.GetString ("gbrainy web site");
		about.TranslatorCredits = translators;
		about.IconName = null;
		about.License = license.ToString ();
		about.WrapLicense = true;
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
			session.CurrentGame.Won = true;
			answer = "<span color ='#00A000'>" + Catalog.GetString ("Congratulations.") + "</span>";
		} else
			answer = Catalog.GetString ("Incorrect answer.");

		session.ScoreGame ();
		session.EnableTimer = false;
		answer_entry.Text = String.Empty;
		UpdateStatusBar ();
		solution_label.Markup = answer + " " + session.CurrentGame.Answer;
		session.CurrentGame.DrawAnswer = true;
		session.Status = GameSession.SessionStatus.Answered;
		ActiveInputControls (true);
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

		session.ScoreGame ();
		GetNextGame ();
		session.EnableTimer = true;
	}

	void OnTip (object sender, EventArgs args)
	{
		if (session.CurrentGame == null)
			return;

		solution_label.Text = session.CurrentGame.TipString;
	}

	void OnNewGame ()
	{
		session.NewSession ();
		GetNextGame ();
		solution_label.Text = Catalog.GetString ("Once you have an answer type it in the \"Answer:\" entry box and press the \"OK\" button.");
		UpdateStatusBar ();
	}

	void OnMathOnly (object sender, EventArgs args)
	{
		session.Type = GameSession.Types.CalculationTrainers;
		OnNewGame ();
	}

	void OnMemoryOnlyAfterCountDown (object source, EventArgs e)
	{
		OnNewGame ();
	}

	void OnMemoryOnly (object sender, EventArgs args)
	{
		question_label.Text = string.Empty;
		solution_label.Text = string.Empty;
		session.Type = GameSession.Types.MemoryTrainers;

		if (preferences.GetBoolValue (Preferences.MemQuestionWarnKey))
			drawing_area.OnDrawCountDown (OnMemoryOnlyAfterCountDown);
		else
			OnNewGame ();
	}

	void OnPreferences (object sender, EventArgs args)
	{
		PreferencesDialog dialog;

		dialog = new PreferencesDialog ();
		if (dialog.Run () == ok_buttonid) {
			session.GameManager.Difficulty = (Game.Difficulty) preferences.GetIntValue (Preferences.DifficultyKey);
		}
		dialog.Dialog.Destroy ();
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
		drawing_area.mode = GameDrawingArea.Modes.Scores;
		drawing_area.GameSession = session.Copy ();
	
		history.SaveGameSession (session);
		session.EndSession ();
		drawing_area.puzzle = null;
		question_label.Text = string.Empty;
		solution_label.Text = string.Empty;
		UpdateStatusBar ();
		drawing_area.QueueDraw ();
		ActiveInputControls (false);
	}

	void OnPauseGame (object sender, EventArgs args)
	{
		if (session.Paused) {
 			session.Resume ();
			ActiveInputControls (true);
		} else {
			session.Pause ();
			ActiveInputControls (false);
		}
		UpdateStatusBar ();
	}

	private void OnToolbarActivate (object sender, System.EventArgs args)
	{
		int width, height;
		Requisition requisition;

		requisition =  toolbar.SizeRequest ();
		app_window.GetSize (out width, out height);
		toolbar.Visible = !toolbar.Visible;
		app_window.Resize (width, height - requisition.Height);
	}

	void OnHistory (object sender, EventArgs args)
	{
		PlayerHistoryDialog dialog;

		dialog = new PlayerHistoryDialog ();
		dialog.Run ();
		dialog.Dialog.Destroy ();	
	}	

	private void OnSizeAllocated (object obj, SizeAllocatedArgs args)
	{
		//Console.WriteLine ("OnSizeAllocated");
	}

	private void AddIcon (IconFactory stock, string stockid, string resource)
	{
		Gtk.IconSet iconset = stock.Lookup (stockid);
		
		if (iconset != null)
			return;

		iconset = new Gtk.IconSet ();
		Gdk.Pixbuf img = Gdk.Pixbuf.LoadFromResource (resource);
		IconSource source = new IconSource ();
		source.Pixbuf = img;
		iconset.AddSource (source);
		stock.Add (stockid, iconset);		
	}

	[DllImport ("libc")] // Linux
	private static extern int prctl (int option, byte [] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

	[DllImport ("libc")] // BSD
	private static extern void setproctitle (byte [] fmt, byte [] str_arg);
 
	public static void SetProcessName (string name)
	{
		int platform = (int) Environment.OSVersion.Platform;		
		if (platform != 4 && platform != 128)
			return;

		try {
			if (prctl (15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes (name + "\0"),
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0) {
				throw new ApplicationException ("Error setting process name: " + 
					Mono.Unix.Native.Stdlib.GetLastError ());
			}
		} catch (EntryPointNotFoundException) {
			setproctitle (Encoding.ASCII.GetBytes ("%s\0"), 
				Encoding.ASCII.GetBytes (name + "\0"));
		}
	}
	
	public static void Main (string [] args) 
	{
		try {
			SetProcessName ("gbrainy");
		} catch {}

		gbrainy gui = new gbrainy (args);
		gui.Run ();	
	}
}



