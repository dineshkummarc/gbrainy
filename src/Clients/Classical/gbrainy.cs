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
using Gnome;
using Mono.Unix;
using System.Diagnostics;
using Gdk;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;
using gbrainy.Core.Platform;

#if MONO_ADDINS
using Mono.Addins;
using Mono.Addins.Setup;
#endif

namespace gbrainy.Clients.Classical
{
	public class GtkClient: Program
	{
		[GtkBeans.Builder.Object("gbrainy")] Gtk.Window app_window;
		[GtkBeans.Builder.Object] Gtk.ToggleAction toolbar_menuitem;
		[GtkBeans.Builder.Object] Box drawing_vbox;
		[GtkBeans.Builder.Object] Gtk.VBox question_vbox;
		[GtkBeans.Builder.Object] Gtk.VBox solution_vbox;
		[GtkBeans.Builder.Object] Gtk.Entry answer_entry;
		[GtkBeans.Builder.Object] Gtk.Button answer_button;
		[GtkBeans.Builder.Object] Gtk.Button tip_button;
		[GtkBeans.Builder.Object] Gtk.Button next_button;
		[GtkBeans.Builder.Object] Gtk.Statusbar statusbar;
		[GtkBeans.Builder.Object] Gtk.Toolbar toolbar;
		[GtkBeans.Builder.Object] Gtk.MenuBar menubar;
		[GtkBeans.Builder.Object] Gtk.Action help_menu;
		[GtkBeans.Builder.Object] Gtk.Action pause_menuitem;
		[GtkBeans.Builder.Object] Gtk.Action finish_menuitem;
		[GtkBeans.Builder.Object] Gtk.Action newgame_menuitem;
		[GtkBeans.Builder.Object] Gtk.Action allgames_menuitem;
		[GtkBeans.Builder.Object] Gtk.Action logic_menuitem;
		[GtkBeans.Builder.Object] Gtk.Action verbal_menuitem;
		[GtkBeans.Builder.Object] Gtk.Action memory_menuitem;
		[GtkBeans.Builder.Object] Gtk.Action calculation_menuitem;
		[GtkBeans.Builder.Object] Gtk.UIManager uimanager;
		DrawingArea drawing_area;
		GameSession session;
		ToolButton all_tbbutton, logic_tbbutton, calculation_tbbutton, memory_tbbutton, verbal_tbbutton, pause_tbbutton, finish_tbbutton;
		bool low_res;
		bool full_screen;
		SimpleLabel question_label;
		SimpleLabel solution_label;
		bool margins = false;
		double offset_x, offset_y;
		int drawing_square;

		public GtkClient (string [] args, params object [] props)
		: base ("gbrainy", Defines.VERSION, Modules.UI,  args, props)
		{
			Gtk.MenuItem extensions_menu;

			Catalog.Init ("gbrainy", Defines.GNOME_LOCALE_DIR);
			Unix.FixLocaleInfo ();

			GtkBeans.Builder builder = new GtkBeans.Builder ("gbrainy.ui");
			builder.Autoconnect (this);

			BuildToolBar ();
			session = new GameSession ();
			session.DrawRequest += SessionDrawRequest;
			session.UpdateUIElement += SessionUpdateUIElement;
			session.SynchronizingObject = new GtkSynchronize ();

			session.Difficulty = (Game.Difficulty) Preferences.GetIntValue (Preferences.DifficultyKey);
			drawing_area = new DrawingArea ();
			drawing_area.ExposeEvent += OnDrawingAreaExposeEvent;
			GameSensitiveUI ();

			// For low resolutions, hide the toolbar and made the drawing area smaller
			if (drawing_area.Screen.Width> 0 && drawing_area.Screen.Height > 0) {
				if (drawing_area.Screen.Height < 700) {
					drawing_vbox.HeightRequest = 300;
					low_res = true;
				}
			}

			question_label = new SimpleLabel ();
			question_label.HeightMargin = 2;
			question_vbox.Add (question_label);

			solution_label = new SimpleLabel ();
			solution_label.HeightMargin = 2;
			solution_vbox.Add (solution_label);

			EventBox eb = new EventBox (); // Provides a window for drawing area windowless widget
			
			eb.Events = Gdk.EventMask.PointerMotionMask;
			drawing_vbox.Add (eb);
	
			eb.Add (drawing_area);

			eb.MotionNotifyEvent += OnMouseMotionEvent;
			eb.ButtonPressEvent += OnHandleButtonPress;

			app_window.IconName = "gbrainy";
			app_window.ShowAll ();

			if (Preferences.GetBoolValue (Preferences.ToolbarKey) == false || low_res == true)
				toolbar_menuitem.Active = false;

			extensions_menu = uimanager.GetWidget ("/ui/menubar/settings_topmenu/extensionsmenu/") as MenuItem;
		#if MONO_ADDINS
			extensions_menu.Activated += delegate (object sender, EventArgs ar) { Mono.Addins.Gui.AddinManagerWindow.Run (app_window);};
		#else
			extensions_menu.Visible = false;
		#endif
			ActiveInputControls (false);
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

		void OnDrawingAreaExposeEvent (object o, ExposeEventArgs ar)
		{
			Gdk.EventExpose args = ar.Event;

			int w, h;
			args.Window.GetSize (out w, out h);

			Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
			CairoContextEx cr = new CairoContextEx (cc.Handle, drawing_area);

			// We want a square drawing area for the puzzles then the figures are shown as designed. 
			// For example, squares are squares. This also makes sure that proportions are kept when resizing
			drawing_square = Math.Min (w, h);	

			if (drawing_square < w)
				offset_x = (w - drawing_square) / 2;

			if (drawing_square < h)
				offset_y = (h - drawing_square) / 2;

			if (margins)
				SetMargin ((int) offset_x);
			else
				SetMargin (2);

			cr.Translate (offset_x, offset_y);
			session.Draw (cr, drawing_square, drawing_square, drawing_area.Direction == Gtk.TextDirection.Rtl);

			((IDisposable)cc).Dispose();
			((IDisposable)cr).Dispose();
		}
		
		void OnMouseMotionEvent (object o, MotionNotifyEventArgs ev_args)
		{
			SendMouseEvent (ev_args.Event.X, ev_args.Event.Y, MouseEventType.Move);
		}

		void OnHandleButtonPress (object o, ButtonPressEventArgs ev_args)
		{
			if (ev_args.Event.Type != EventType.TwoButtonPress)
				return;

			SendMouseEvent (ev_args.Event.X, ev_args.Event.Y, MouseEventType.DoubleClick);
		}

		void SendMouseEvent (double ev_x, double ev_y, MouseEventType type)
		{
			double x, y;

			x = ev_x - offset_x;
			y = ev_y - offset_y;

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

		public void ActiveInputControls (bool active)
		{
			bool answer, entry, next, tip;

			answer = entry = next = tip = active;

			if (active == true && session.CurrentGame != null && session.CurrentGame.ButtonsActive == true && String.IsNullOrEmpty (session.CurrentGame.Tip ) == false)
				tip = true;
			else
				tip = false;
	
			switch (session.Status) {
			case GameSession.SessionStatus.NotPlaying:
			case GameSession.SessionStatus.Finished:
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

		public void SetMargin (int margin)
		{
			question_label.WidthMargin = margin;
			solution_label.WidthMargin = margin;
		}

		void UpdateSolution (string solution)
		{		
			solution_label.Text = solution;
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
			toolbar.ShowArrow = false;

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

		void GameSensitiveUI () 
		{
			//Toolbar buttons and menu items that are sensitive when the user is playing
			bool playing;
			Game.Types available;

			playing = (session.Status == GameSession.SessionStatus.Playing);
			finish_tbbutton.Sensitive = pause_tbbutton.Sensitive = playing;

			available = session.AvailableGames;

			if (playing == false && ((available & Game.Types.LogicPuzzle) == Game.Types.LogicPuzzle))
				logic_menuitem.Sensitive = logic_tbbutton.Sensitive = true;
			else
				logic_menuitem.Sensitive = logic_tbbutton.Sensitive = false;

			if (playing == false && ((available & Game.Types.MemoryTrainer) == Game.Types.MemoryTrainer))
				memory_menuitem.Sensitive = memory_tbbutton.Sensitive = true;
			else
				memory_menuitem.Sensitive = memory_tbbutton.Sensitive = false;

			if (playing == false && ((available & Game.Types.MathTrainer) == Game.Types.MathTrainer))
				calculation_menuitem.Sensitive = calculation_tbbutton.Sensitive = true;
			else
				calculation_menuitem.Sensitive = calculation_tbbutton.Sensitive = false;

			if (playing == false && ((available & Game.Types.VerbalAnalogy) == Game.Types.VerbalAnalogy))
				verbal_menuitem.Sensitive = verbal_tbbutton.Sensitive = true;
			else
				verbal_menuitem.Sensitive = verbal_tbbutton.Sensitive = false;

			if (playing == false && (available != Game.Types.None))
				allgames_menuitem.Sensitive = all_tbbutton.Sensitive = true;
			else
				allgames_menuitem.Sensitive = all_tbbutton.Sensitive = false;

			pause_menuitem.Sensitive = finish_menuitem.Sensitive = playing;
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
			AboutDialog about = new AboutDialog ();
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
			session.NewSession ();
			GetNextGame ();
			GameSensitiveUI ();
			UpdateSolution (Catalog.GetString ("Once you have an answer type it in the \"Answer:\" entry box and press the \"OK\" button."));
			UpdateStatusBar ();
		}

		void OnMathOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.CalculationTrainers);
		}

		void OnVerbalOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.VerbalAnalogies);
		}

		void OnMemoryOnly (object sender, EventArgs args)
		{
			OnNewGame (GameSession.Types.MemoryTrainers);
		}

		void OnPreferences (object sender, EventArgs args)
		{
			PreferencesDialog dialog;

			dialog = new PreferencesDialog (session.PlayerHistory);
			if ((Gtk.ResponseType) dialog.Run () == ResponseType.Ok) {
				session.Difficulty = (Game.Difficulty) Preferences.GetIntValue (Preferences.DifficultyKey);
			}
			dialog.Destroy ();
		}

		void OnCustomGame (object sender, EventArgs args)
		{
			ResponseType rslt;
			CustomGameDialog dialog;

			dialog = new CustomGameDialog (session.GameManager);
			rslt = (Gtk.ResponseType) dialog.Run ();
			dialog.Destroy ();

			if (rslt == ResponseType.Ok && dialog.NumOfGames > 0)
				OnNewGame (session.Type = GameSession.Types.Custom);
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
			session.EndSession ();
	
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
				pause_tbbutton.StockId = "pause";
				pause_tbbutton.Label = Catalog.GetString ("Pause");
				ActiveInputControls (true);
			} else {
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

		private void OnToolbarActivate (object sender, System.EventArgs args)
		{
			int width, height;
			Requisition requisition;

			requisition =  toolbar.SizeRequest ();
			app_window.GetSize (out width, out height);
			toolbar.Visible = !toolbar.Visible;
			Preferences.SetBoolValue (Preferences.ToolbarKey, toolbar.Visible);
			Preferences.Save ();
			app_window.Resize (width, height - requisition.Height);
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
				margins = true;
				app_window.Fullscreen ();
			}
			else {
				margins = false;
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
			} catch {}

			GtkClient gui = new GtkClient (args);
			gui.Run ();	
		}
	}
}
