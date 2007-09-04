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

using Gtk;
using System;
using Cairo;
using Mono.Unix;
using System.Timers;

public class GameSession
{
	private gbrainy app;
	private TimeSpan game_time;
	private int games_played;
	private int games_won;
	private Game current_game;
	private GameManager game_manager;
	private System.Timers.Timer timer;
	private bool paused;
	private string current_time;
	private TimeSpan one_sec = TimeSpan.FromSeconds (1);
	
	public GameSession (gbrainy brainy)
	{
		game_manager = new GameManager ();
		game_time = TimeSpan.Zero;
		games_played = 0;
		games_won = 0;
		current_game = null;
		timer = null;
		paused = false;
		app = brainy;

		timer = new System.Timers.Timer ();
		timer.Elapsed += TimerUpdater;
		timer.Interval = (1 * 1000); // 1 second
	}

	public GameType GameType {
		get {return game_manager.GameType; }
		set {game_manager.GameType = value; }
	}
	
	public TimeSpan GameTime {
		get {return game_time; }
		set {game_time = value; }
	}

	public int GamesPlayed {
		get {return games_played; }
		set { games_played = value;}
	}
		
	public int GamesWon {
		get {return games_won; }
		set {games_won = value; }
	}

	public bool Paused {
		get {return paused; }
		set {paused = value; }
	}

	public Game CurrentGame {
		get {return current_game; }
		set {current_game = value; }
	}

	public bool EnableTimer {
		get {return timer.Enabled; }
		set {timer.Enabled = value; }
	}

	public string TimePlayed {
		get {
			return (current_time == null) ? TimeSpanToStr (TimeSpan.FromSeconds (0)) : current_time;
		}
	}

	public string TimePerGame {
		get {
			TimeSpan average;

			average = (games_played > 0) ? TimeSpan.FromSeconds (game_time.Seconds / games_played) : game_time;
			return TimeSpanToStr (average);
		}
	}

	public string StatusText {
		get {
			String text;
			text = String.Format (Catalog.GetString ("Games played: {0} ({1} won)"), games_played, games_won);
			text += String.Format (Catalog.GetString ("- Time: {0}"), current_time);

			if (current_game != null)
				text += " " + String.Format (Catalog.GetString ("- Current game: {0}"), current_game.Name);
	
			return text;
		}
	}
	
	public void NewSession ()
	{
		games_played = 0;
		games_won = 0;
		game_time = TimeSpan.Zero;
		timer.Enabled = true;
	}

	public void EndSession ()
	{
		if (current_game != null)
			current_game.Finish ();

		timer.Enabled = false;
		paused = false;
		current_game = null;
		games_played = 0;
		games_won = 0;
		game_time = TimeSpan.Zero;
		current_time = TimeSpanToStr (game_time);
	}

	public void NextGame ()
	{	
		if (current_game != null)
			current_game.Finish ();

		games_played++;
		current_game = game_manager.GetPuzzle (app);
	}

	public void Pause ()
	{
		timer.Enabled = false;
		paused = true;
		current_time = Catalog.GetString ("Paused");
	}

	public void Resume ()
	{
		timer.Enabled = true;
		paused = false;
	}	

	private void TimerUpdater (object source, ElapsedEventArgs e)
	{ 	
		game_time = game_time.Add (one_sec);

		lock (this) {
			current_time = TimeSpanToStr (game_time);
		}

		Application.Invoke (delegate {	app.UpdateStatusBar (); } );
	}

	private string TimeSpanToStr (TimeSpan time)
	{
		string fmt = time.ToString ();
		int i = fmt.IndexOf ('.');
		if (i > 0 && fmt.Length - i > 2)
			fmt = fmt.Substring (0, i);

		return fmt;
	}


}

