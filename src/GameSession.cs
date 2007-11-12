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
	public enum Types
	{	
		None			= 0,
		LogicPuzzles		= 2,
		MemoryTrainers		= 4,
		MathTrainers		= 8,
		Custom			= 16,
		TrainersOnly		= MemoryTrainers | MathTrainers,
		AllGames		= MemoryTrainers | MathTrainers | LogicPuzzles
	}

	private enum ScoresType
	{
		None = 0,
		LogicPuzzles,
		MemoryTrainers,
		MathTrainers,
		Last			
	}

	public enum SessionStatus
	{
		NotPlaying,
		Playing,
		Answered,
	}

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
	private int [] scores;
	private int [] games;
	private int total_score;
	private bool scored_game;
	private SessionStatus status;
	
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
		
		scores = new int [(int) ScoresType.Last];
		games = new int [(int) ScoresType.Last];
		total_score = 0;
		scored_game = false;
		status = SessionStatus.NotPlaying;
	}

	public GameSession Copy ()
	{
		GameSession session = new GameSession (app);
		for (int i = 0; i < (int) ScoresType.Last; i++)
		{
			session.scores[i] = scores[i];
			session.games[i] = games[i];
		}

		session.total_score = total_score;
		session.games_played = games_played;
		session.games_won = games_won;
		session.game_time = game_time;		
		return session;
	}

	public Types Type {
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

	public SessionStatus Status {
		get {return status; }
		set {status = value; }
	}

	public GameManager GameManager {
		get {return  game_manager;}
	}

	public int TotalScore {
		get {return total_score;}
	}

	public int LogicScore {
		get {
			if (games [(int) ScoresType.LogicPuzzles] == 0)
				return 0;
			
			return scores [(int) ScoresType.LogicPuzzles] * 10 / games [(int) ScoresType.LogicPuzzles];
		}
	}

	public int MemoryScore {
		get {
			if (games [(int) ScoresType.MemoryTrainers] == 0)
				return 0;
			
			return scores [(int) ScoresType.MemoryTrainers] * 10 / games [(int) ScoresType.MemoryTrainers];
		}
	}

	public int MathScore {
		get {
			if (games [(int) ScoresType.MathTrainers] == 0)
				return 0;
			
			return scores [(int) ScoresType.MathTrainers] * 10 / games [(int) ScoresType.MathTrainers];
		}
	}

	public int LogicGamesPlayed {
		get { return games [(int) ScoresType.LogicPuzzles]; }
	}

	public int MemoryGamesPlayed {
		get { return games [(int) ScoresType.MemoryTrainers]; }
	}

	public int MathGamesPlayed {
		get { return games [(int) ScoresType.MathTrainers]; }
	}

	public string TimePlayed {
		get {
			return (current_time == null) ? TimeSpanToStr (TimeSpan.FromSeconds (0)) : current_time;
		}
	}

	public string TimePerGame {
		get {
			TimeSpan average;

			average = (games_played > 0) ? TimeSpan.FromSeconds (game_time.TotalSeconds / games_played) : game_time;
			return TimeSpanToStr (average);
		}
	}

	public string StatusText {
		get {
			if (status == SessionStatus.NotPlaying)
				return string.Empty;

			String text;
			text = String.Format (Catalog.GetString ("Games played: {0} ({1}% score)"),games_played, total_score);
			text += String.Format (Catalog.GetString (" - Time: {0}"), current_time);

			if (current_game != null)
 				text += " " + String.Format (Catalog.GetString ("- Game: {0}"), current_game.Name);
	
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
		scores = new int [(int) ScoresType.Last];
		games = new int [(int) ScoresType.Last];
		total_score = 0;
		scored_game = false;
		status = SessionStatus.NotPlaying;
	}

	public void NextGame ()
	{	
		if (current_game != null)
			current_game.Finish ();

		games_played++;
		current_game = game_manager.GetPuzzle (app);
		current_game.GameTime = TimeSpan.Zero;
		scored_game = false;
		status = SessionStatus.Playing;
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

	public void ScoreGame ()
	{
		if (scored_game == true)
			return;

		switch (current_game.Type) {
		case Game.Types.LogicPuzzle:
			scores [(int) ScoresType.LogicPuzzles] += current_game.Score;
			games [(int) ScoresType.LogicPuzzles]++;
			break;
		case Game.Types.MemoryTrainer:
			scores [(int) ScoresType.MemoryTrainers] += current_game.Score;
			games [(int) ScoresType.MemoryTrainers]++;
			break;
		case Game.Types.MathTrainer:
			scores [(int) ScoresType.MathTrainers] += current_game.Score;
			games [(int) ScoresType.MathTrainers]++;
			break;
		}
		
		total_score = 0;
		for (int i = 0; i < (int) ScoresType.Last; i++) {
			total_score += scores [i];
		}

		Console.WriteLine ("Score for this game {0}", current_game.Score);
		Console.WriteLine ("Total scores {0}, maximum possible score {1}", total_score, games_played * 10);
		total_score = total_score * 10 / games_played;

		scored_game = true;
	}	


	private void TimerUpdater (object source, ElapsedEventArgs e)
	{
		lock (this) {
			game_time = game_time.Add (one_sec);
			current_game.GameTime = current_game.GameTime + one_sec;
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

