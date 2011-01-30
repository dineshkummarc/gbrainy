/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Gtk;
using Mono.Unix;
using System.Diagnostics;
using Gdk;

using gbrainy.Core.Main;
using gbrainy.Core.Platform;
using gbrainy.Core.Services;
using gbrainy.Clients.Classical.Dialogs;
using gbrainy.Clients.Classical.Widgets;

#if MONO_ADDINS
using Mono.Addins;
using Mono.Addins.Setup;
#endif

#if GNOME
using Gnome;
#endif

namespace gbrainy.Clients.Classical
{
	public class GtkClient
#if GNOME
		: Program
#endif
	{
		[GtkBeans.Builder.Object("gbrainy")] Gtk.Window app_window;
		[GtkBeans.Builder.Object] Gtk.CheckMenuItem showtoolbar_menuitem;
		[GtkBeans.Builder.Object] Box drawing_vbox;
		[GtkBeans.Builder.Object] Gtk.HBox main_hbox;
		[GtkBeans.Builder.Object] Gtk.VBox framework_vbox;
		[GtkBeans.Builder.Object] Gtk.Entry answer_entry;
		[GtkBeans.Builder.Object] Gtk.Button answer_button;
		[GtkBeans.Builder.Object] Gtk.Button tip_button;
		[GtkBeans.Builder.Object] Gtk.Button next_button;
		[GtkBeans.Builder.Object] Gtk.Statusbar statusbar;
		[GtkBeans.Builder.Object] Gtk.MenuBar menubar;
		[GtkBeans.Builder.Object] Gtk.MenuItem pause_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem finish_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem newgame_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem allgames_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem logic_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem calculation_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem memory_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem verbal_menuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem extensions_menuitem;
		[GtkBeans.Builder.Object] Gtk.RadioMenuItem vertical_radiomenuitem;
		[GtkBeans.Builder.Object] Gtk.RadioMenuItem horizontal_radiomenuitem;
		[GtkBeans.Builder.Object] Gtk.MenuItem toolbar_orientation_menuitem;

		Gtk.Toolbar toolbar;

		GameDrawingArea drawing_area;
		GameSession session;
		ToolButton all_tbbutton, logic_tbbutton, calculation_tbbutton, memory_tbbutton, verbal_tbbutton, pause_tbbutton, finish_tbbutton;
		bool low_res;
		bool full_screen;
		GameSession.Types initial_session;
		bool init_completed = false;

		public GtkClient ()
#if GNOME
		: base ("gbrainy", Defines.VERSION, Modules.UI, new string [0])
#endif
		{
			Catalog.Init ("gbrainy", Defines.GNOME_LOCALE_DIR);
			Unix.FixLocaleInfo ();
		}

		public GameSession Session {
			get { return session; }
		}

		public GameSession.Types InitialSessionType {
			get { return initial_session; }
			set { initial_session = value; }
		}

		public void Initialize ()
		{
			session = new GameSession ();
			GameManagerPreload (session.GameManager);
			Console.WriteLine (session.GameManager.GetGamesSummary ());

			session.GameManager.ColorBlind = Preferences.GetBoolValue (Preferences.ColorBlindKey);
			session.DrawRequest += SessionDrawRequest;
			session.UpdateUIElement += SessionUpdateUIElement;
			session.SynchronizingObject = new GtkSynchronize ();
			session.Difficulty = (GameDifficulty) Preferences.GetIntValue (Preferences.DifficultyKey);

			BuildUI ();
		}

		public static void GameManagerPreload (GameManager gm)
		{
			gm.LoadAssemblyGames (Defines.GAME_ASSEMBLY);
			gm.LoadVerbalAnalogies (System.IO.Path.Combine (Defines.DATA_DIR, Defines.VERBAL_ANALOGIES));
			gm.LoadGamesFromXml (System.IO.Path.Combine (Defines.DATA_DIR, Defines.GAMES_FILE));
			gm.LoadPlugins ();
		}

		void AttachToolBar ()
		{
			Gtk.Box.BoxChild child;

			if (toolbar != null)
			{
				Box box;
				
				switch (toolbar.Orientation) {
				case Gtk.Orientation.Vertical:
					box = main_hbox;
					break;
				case Gtk.Orientation.Horizontal:
				{
					box = framework_vbox;
					break;
				}
				default:
					throw new InvalidOperationException ();
				}
				
				bool contained = false;
				foreach (var ch in box.AllChildren)
				{
					if (ch == toolbar)
					{
						contained = true;
						break;
					}
				}
				if (contained == true)
					box.Remove (toolbar);
			}
			toolbar.Orientation = (Gtk.Orientation) Preferences.GetIntValue (Preferences.ToolbarOrientationKey);

			switch (toolbar.Orientation) {
			case Gtk.Orientation.Vertical:
				main_hbox.Add (toolbar);
				main_hbox.ReorderChild (toolbar, 0);
				child = ((Gtk.Box.BoxChild)(main_hbox[toolbar]));
				break;
			case Gtk.Orientation.Horizontal:
				framework_vbox.Add (toolbar);
				framework_vbox.ReorderChild (toolbar, 1);
				child = ((Gtk.Box.BoxChild)(framework_vbox[toolbar]));
				break;
			default:
				throw new InvalidOperationException ();
			}

			child.Expand = false;
			child.Fill = false;
			toolbar.ShowAll ();
			init_completed = true;
		}

		void BuildUI ()
		{
			bool show_toolbar;

			GtkBeans.Builder builder = new GtkBeans.Builder ("gbrainy.ui");
			builder.Autoconnect (this);

			show_toolbar = Preferences.GetBoolValue (Preferences.ToolbarShowKey) == true && low_res == false;

			// Toolbar creation
			toolbar = new Gtk.Toolbar ();
			toolbar.ToolbarStyle = ToolbarStyle.Both;
			BuildToolBar ();
			AttachToolBar ();

			drawing_area = new GameDrawingArea ();
			drawing_area.Drawable = session;
			GameSensitiveUI ();

			// For low resolutions, hide the toolbar and made the drawing area smaller
			if (drawing_area.Screen.Width> 0 && drawing_area.Screen.Height > 0) {
				if (drawing_area.Screen.Height < 700) {
					drawing_vbox.HeightRequest = 350;
					low_res = true;
				}
			}

			EventBox eb = new EventBox (); // Provides a window for drawing area windowless widget

			eb.Events = Gdk.EventMask.PointerMotionMask;
			drawing_vbox.Add (eb);

			eb.Add (drawing_area);

			eb.MotionNotifyEvent += OnMouseMotionEvent;
			eb.ButtonPressEvent += OnHandleButtonPress;

			show_toolbar = Preferences.GetBoolValue (Preferences.ToolbarShowKey) == true && low_res == false;

			// We only disable the Arrow if we are going to show the toolbar.
			// It has an impact on the total window width size even if you we do not show it
			if (show_toolbar)
				toolbar.ShowArrow = false;

			app_window.IconName = "gbrainy";

			app_window.ShowAll ();

			if (show_toolbar == false)
				showtoolbar_menuitem.Active = false;

			toolbar_orientation_menuitem.Sensitive = toolbar.Visible;

			// Check default radio button
			switch (toolbar.Orientation) {
			case Gtk.Orientation.Vertical:
				vertical_radiomenuitem.Active = true;
				break;
			case Gtk.Orientation.Horizontal:
				horizontal_radiomenuitem.Active = true;
				break;
			default:
				throw new InvalidOperationException ();
			}

		#if MONO_ADDINS
			extensions_menuitem.Activated += delegate (object sender, EventArgs ar) { Mono.Addins.Gui.AddinManagerWindow.Run (app_window);};
		#else
			extensions_menuitem.Visible = false;
		#endif
			ActiveInputControls (false);
		}

		public void ProcessDefaults ()
		{
			if (InitialSessionType != GameSession.Types.None)
				OnNewGame (InitialSessionType);
		}

		// Gamesession has requested a question refresh
		public void SessionUpdateUIElement (object o, UpdateUIStateEventArgs args)
		{
			switch (args.EventType) {
			case UpdateUIStateEventArgs.EventUIType.QuestionText:
				UpdateQuestion ((string) args.Data);
				ActiveInputControls (true);
				break;
			case UpdateUIStateEventArgs.EventUIType.Time:
				UpdateStatusBar ();
				break;
			default:
				throw new InvalidOperationException ("Unknown value");
			}
		}

		// Gamesession has requested a redraw of the drawingarea
		public void SessionDrawRequest (object o, EventArgs args)
		{
			drawing_area.QueueDraw ();
		}

		void OnMouseMotionEvent (object o, MotionNotifyEventArgs ev_args)
		{
			SendMouseEvent (ev_args.Event.X, ev_args.Event.Y, MouseEventType.Move);
		}

		void OnHandleButtonPress (object o, ButtonPressEventArgs ev_args)
		{
			if (ev_args.Event.Type != EventType.ButtonPress)
				return;

			SendMouseEvent (ev_args.Event.X, ev_args.Event.Y, MouseEventType.ButtonPress);
		}

		void SendMouseEvent (double ev_x, double ev_y, MouseEventType type)
		{
			double x, y;
			int drawing_square = drawing_area.DrawingSquare;

			x = ev_x - drawing_area.OffsetX;
			y = ev_y - drawing_area.OffsetY;

			if (x < 0 || y < 0 || x > drawing_square || y > drawing_square)
				return;

			x =  x / drawing_square;
			y =  y / drawing_square;

			session.MouseEvent (this, new MouseEventArgs (x, y, type));
		}

		public void UpdateStatusBar ()
		{
			statusbar.Push (0, session.StatusText);
		}

		// These are UI elements dependent of the game status
		public void ActiveInputControls (bool active)
		{
			bool answer, entry, next, tip, can_pause;

			can_pause = answer = entry = next = tip = active;

			if (active == true && session.CurrentGame != null && session.CurrentGame.ButtonsActive == true && String.IsNullOrEmpty (session.CurrentGame.Tip ) == false)
				tip = true;
			else
				tip = false;

			switch (session.Status) {
			case GameSession.SessionStatus.NotPlaying:
			case GameSession.SessionStatus.Finished:
				answer = false;
				entry = false;
				next = false;
				tip = false;
				can_pause = false;
				break;
			case GameSession.SessionStatus.Playing:
				if (session.CurrentGame != null) {
					can_pause = session.CurrentGame.ButtonsActive;
				}
				else {
					can_pause = true;
				}
				break;
			case GameSession.SessionStatus.Answered:
				answer = false;
				entry = false;
				tip = false;
				can_pause = false;
				break;
			}

			answer_button.Sensitive = answer;
			answer_entry.Sensitive = entry;
			next_button.Sensitive = next;
			tip_button.Sensitive = tip;
			pause_menuitem.Sensitive = pause_tbbutton.Sensitive = can_pause;

			if (entry == true)
				answer_entry.GrabFocus ();
		}

		public void UpdateQuestion (string question)
		{
			drawing_area.Question = question;
		}

		public void QueueDraw ()
		{
			drawing_area.QueueDraw ();
		}

		void UpdateSolution (string solution)
		{
			drawing_area.Solution = solution;
			QueueDraw ();
		}

		void BuildToolBar ()
		{
			IconFactory icon_factory = new IconFactory ();
		        AddIcon (icon_factory, "logic-games", "logic-games-32.png");
			AddIcon (icon_factory, "math-games", "math-games-32.png");
			AddIcon (icon_factory, "memory-games", "memory-games-32.png");
			AddIcon (icon_factory, "verbal-games", "verbal-games-32.png");
			AddIcon (icon_factory, "pause", "pause-32.png");
			AddIcon (icon_factory, "resume", "resume-32.png");
			AddIcon (icon_factory, "endgame", "endgame-32.png");
			AddIcon (icon_factory, "allgames", "allgames-32.png");
			icon_factory.AddDefault ();

			toolbar.IconSize = Gtk.IconSize.Dnd;

			all_tbbutton = new ToolButton ("allgames");
			all_tbbutton.TooltipText = Catalog.GetString ("Play all the games");
			all_tbbutton.Label = Catalog.GetString ("All");
			all_tbbutton.Clicked += OnAllGames;
			toolbar.Insert (all_tbbutton, -1);

			logic_tbbutton = new ToolButton ("logic-games");
			logic_tbbutton.TooltipText = Catalog.GetString ("Play games that challenge your reasoning and thinking");
			logic_tbbutton.Label = Catalog.GetString ("Logic");
			logic_tbbutton.Clicked += OnLogicOnly;
			toolbar.Insert (logic_tbbutton, -1);

			calculation_tbbutton = new ToolButton ("math-games");
			calculation_tbbutton.Label = Catalog.GetString ("Calculation");
			calculation_tbbutton.TooltipText = Catalog.GetString ("Play games that challenge your mental calculation skills");
			calculation_tbbutton.Clicked += OnMathOnly;
			toolbar.Insert (calculation_tbbutton, -1);

			memory_tbbutton = new ToolButton ("memory-games");
			memory_tbbutton.Label = Catalog.GetString ("Memory");
			memory_tbbutton.TooltipText = Catalog.GetString ("Play games that challenge your short term memory");
			memory_tbbutton.Clicked += OnMemoryOnly;
			toolbar.Insert (memory_tbbutton, -1);

			verbal_tbbutton = new ToolButton ("verbal-games");
			verbal_tbbutton.Label = Catalog.GetString ("Verbal");
			verbal_tbbutton.TooltipText = Catalog.GetString ("Play games that challenge your verbal aptitude");
			verbal_tbbutton.Clicked += OnVerbalOnly;
			toolbar.Insert (verbal_tbbutton, -1);

			pause_tbbutton = new ToolButton ("pause");
			pause_tbbutton.Label = Catalog.GetString ("Pause");
			pause_tbbutton.TooltipText = Catalog.GetString ("Pause or resume the game");
			pause_tbbutton.Clicked += OnPauseGame;
			toolbar.Insert (pause_tbbutton, -1);

			finish_tbbutton = new ToolButton ("endgame");
			finish_tbbutton.TooltipText = Catalog.GetString ("End the game and show score");
			finish_tbbutton.Label = Catalog.GetString ("Finish");
			finish_tbbutton.Clicked += OnEndGame;
			toolbar.Insert (finish_tbbutton, -1);
		}

		// These are UI elements independent of the game status, set only when the game starts / ends
		void GameSensitiveUI ()
		{
			//Toolbar buttons and menu items that are sensitive when the user is playing
			bool playing;
			GameTypes available;

			playing = (session.Status == GameSession.SessionStatus.Playing);
			finish_tbbutton.Sensitive = playing;

			available = session.AvailableGames;

			if (playing == false && ((available & GameTypes.LogicPuzzle) == GameTypes.LogicPuzzle))
				logic_menuitem.Sensitive = logic_tbbutton.Sensitive = true;
			else
				logic_menuitem.Sensitive = logic_tbbutton.Sensitive = false;

			if (playing == false && ((available & GameTypes.Calculation) == GameTypes.Calculation))
				memory_menuitem.Sensitive = memory_tbbutton.Sensitive = true;
			else
				memory_menuitem.Sensitive = memory_tbbutton.Sensitive = false;

			if (playing == false && ((available & GameTypes.Calculation) == GameTypes.Calculation))
				calculation_menuitem.Sensitive = calculation_tbbutton.Sensitive = true;
			else
				calculation_menuitem.Sensitive = calculation_tbbutton.Sensitive = false;

			if (playing == false && ((available & GameTypes.VerbalAnalogy) == GameTypes.VerbalAnalogy))
				verbal_menuitem.Sensitive = verbal_tbbutton.Sensitive = true;
			else
				verbal_menuitem.Sensitive = verbal_tbbutton.Sensitive = false;

			if (playing == false && (available != GameTypes.None))
				allgames_menuitem.Sensitive = all_tbbutton.Sensitive = true;
			else
				allgames_menuitem.Sensitive = all_tbbutton.Sensitive = false;

			finish_menuitem.Sensitive = playing;
			newgame_menuitem.Sensitive = !playing;
		}

		private void GetNextGame ()
		{
			UpdateSolution (String.Empty);
			UpdateQuestion (String.Empty);
			session.NextGame ();
			session.CurrentGame.AnswerEvent += OnAnswerFromGame;

			ActiveInputControls (session.CurrentGame.ButtonsActive);
			next_button.Sensitive = true;
			UpdateQuestion (session.CurrentGame.Question);
			answer_entry.Text = string.Empty;
			UpdateStatusBar ();
			session.CurrentGame.DrawAnswer = false;
			drawing_area.QueueDraw ();
		}

		// The user has clicked with the mouse in an answer and generated this event
		void OnAnswerFromGame (object obj, Game.AnswerEventArgs args)
		{
			answer_entry.Text = args.Answer;
			OnAnswerButtonClicked (this, EventArgs.Empty);
			session.CurrentGame.AnswerEvent -= OnAnswerFromGame; // User can only answer once
		}

		void OnMenuAbout (object sender, EventArgs args)
		{
			Dialogs.AboutDialog about = new Dialogs.AboutDialog ();
			about.Run ();
		}

		void OnMenuHelp (object sender, EventArgs args)
		{
			Unix.ShowUri (null, "ghelp:gbrainy",
				Gdk.EventHelper.GetTime (new Gdk.Event(IntPtr.Zero)));
		}

		void OnAnswerButtonClicked (object sender, EventArgs args)
		{
			string answer;

			if (session.CurrentGame == null)
				return;

			if (session.ScoreGame (answer_entry.Text) == true)
				answer = "<span color='#00A000'>" + Catalog.GetString ("Congratulations.") + "</span>";
			else
				answer = Catalog.GetString ("Incorrect answer.");

			session.EnableTimer = false;
			answer_entry.Text = String.Empty;
			UpdateStatusBar ();
			UpdateSolution (answer + " " + session.CurrentGame.Answer);

			session.CurrentGame.DrawAnswer = true;
			ActiveInputControls (true);
			next_button.GrabFocus ();
			drawing_area.QueueDraw ();
		}

		void OnQuit (object sender, EventArgs args)
		{
#if GNOME
			Quit ();
#else
			Gtk.Application.Quit ();
#endif

		}

		void OnDeleteWindow (object sender, DeleteEventArgs args)
		{
#if GNOME
			Quit ();
#else
			Gtk.Application.Quit ();
#endif
		}

		void OnNextButtonClicked (object sender, EventArgs args)
		{
			if (answer_entry.Text.Length > 0) {
				OnAnswerButtonClicked (sender, args);
				return;
			}

			session.ScoreGame (String.Empty);
			GetNextGame ();
			session.EnableTimer = true;
		}

		void OnTip (object sender, EventArgs args)
		{
			if (session.CurrentGame == null)
				return;

			UpdateSolution (session.CurrentGame.TipString);
		}

		void OnNewGame (GameSession.Types type)
		{
			session.Type = type;
			session.New ();
			GetNextGame ();
			GameSensitiveUI ();
			UpdateSolution (Catalog.GetString ("Once you have an answer type it in the \"Answer:\" entry box and press the \"OK\" button."));
			UpdateStatusBar ();
		}

		void OnMathOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.Calculation);
		}

		void OnVerbalOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.VerbalAnalogies);
		}

		void OnMemoryOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.Memory);
		}

		void OnPdfExport (object sender, EventArgs args)
		{
			PdfExportDialog pdf;
			
			pdf = new PdfExportDialog ();
			pdf.Run ();
			pdf.Destroy (); 
		}

		void OnPreferences (object sender, EventArgs args)
		{
			PreferencesDialog dialog;

			dialog = new PreferencesDialog (session.PlayerHistory);
			if ((Gtk.ResponseType) dialog.Run () == ResponseType.Ok) {
				session.Difficulty = (GameDifficulty) Preferences.GetIntValue (Preferences.DifficultyKey);
			}
			dialog.Destroy ();
		}

		void OnCustomGame (object sender, EventArgs args)
		{
			CustomGameDialog dialog;

			dialog = new CustomGameDialog (session.GameManager);
			dialog.Run ();
			dialog.Destroy ();

			if (dialog.SelectionDone == true)
				OnNewGame (GameSession.Types.Custom);
		}

		void OnLogicOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.LogicPuzzles);
		}

		void OnAllGames (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.AllGames);
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
			session.End ();

			UpdateSolution (String.Empty);
			UpdateQuestion (String.Empty);
			UpdateStatusBar ();
			GameSensitiveUI ();
			drawing_area.QueueDraw ();
			ActiveInputControls (false);
			SetPauseResumeButtonUI (true);
		}

		void SetPauseResumeButtonUI (bool pause)
		{
			if (pause) {
				drawing_area.Paused = false;
				pause_tbbutton.StockId = "pause";
				pause_tbbutton.Label = Catalog.GetString ("Pause");
				ActiveInputControls (true);
			} else {
				drawing_area.Paused = true;	
				pause_tbbutton.StockId = "resume";
				pause_tbbutton.Label = Catalog.GetString ("Resume");
				ActiveInputControls (false);
			}
			UpdateStatusBar ();
		}


		void SetPauseResumeButton (bool pause)
		{
			if (pause)
	 			session.Resume ();
			else
				session.Pause ();

			SetPauseResumeButtonUI (pause);
		}

		void OnPauseGame (object sender, EventArgs args)
		{
			SetPauseResumeButton (session.Paused);
		}

		void OnActivateToolbar (object sender, System.EventArgs args)
		{
			int width, height;
			Requisition requisition;

			requisition = toolbar.SizeRequest ();
			app_window.GetSize (out width, out height);
			toolbar.Visible = !toolbar.Visible;

			if (toolbar.Visible)
				toolbar.ShowArrow = false;

			toolbar_orientation_menuitem.Sensitive = toolbar.Visible;

			Preferences.SetBoolValue (Preferences.ToolbarShowKey, toolbar.Visible);
			Preferences.Save ();
			app_window.Resize (width, height - requisition.Height);
		}

		void OnVerticalToolbar (object sender, System.EventArgs args)
		{
			if (init_completed  == false)
				return;

			Preferences.SetIntValue (Preferences.ToolbarOrientationKey, (int) Gtk.Orientation.Vertical);
			Preferences.Save ();
			AttachToolBar ();
		}

		void OnHorizontalToolbar (object sender, System.EventArgs args)
		{
			if (init_completed  == false)
				return;

			Preferences.SetIntValue (Preferences.ToolbarOrientationKey, (int) Gtk.Orientation.Horizontal);
			Preferences.Save ();
			AttachToolBar ();
		}

		void OnHistory (object sender, EventArgs args)
		{
			PlayerHistoryDialog dialog;

			dialog = new PlayerHistoryDialog (session.PlayerHistory);
			dialog.Run ();
			dialog.Destroy ();
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

		void OnFullscreen (object sender, EventArgs args)
		{
			if (full_screen == false) {
				app_window.Fullscreen ();
			}
			else {
				app_window.Unfullscreen ();
			}

			full_screen = !full_screen;
		}

		void OnExtending (object sender, EventArgs args)
		{
			Process.Start ("http://live.gnome.org/gbrainy/Extending");
		}

		public static void Main (string [] args)
		{
			try {
				Unix.SetProcessName ("gbrainy");
			} 
			catch (Exception e)
			{
				Console.WriteLine ("gbrainy.Main. Could not set process name. Error {0}", e);
			}

			DateTime start_time = DateTime.Now;

			// Register services
			ServiceLocator.Instance.RegisterService <ITranslations> (new TranslationsCatalog ());

			GtkClient app = new GtkClient ();
			CommandLine.Version ();

			CommandLine line = new CommandLine (args);
			line.Parse ();

			if (line.Continue == false)
				return;
#if !GNOME
			Gtk.Application.Init ();
#endif

			app.Initialize ();
			if (line.PlayList.Length > 0) {
				app.Session.GameManager.PlayList = line.PlayList;
				app.InitialSessionType = GameSession.Types.Custom;
			}
			app.Session.GameManager.RandomOrder = line.RandomOrder;
			app.ProcessDefaults ();
			ThemeManager.Load ();

			TimeSpan span = DateTime.Now - start_time;
			Console.WriteLine (Catalog.GetString ("Startup time {0}"), span);
#if GNOME
			app.Run ();
#else
			Gtk.Application.Run ();
#endif
		}
	}
}
