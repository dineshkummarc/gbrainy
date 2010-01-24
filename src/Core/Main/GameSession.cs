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
using Mono.Unix;
using System.Timers;
using System.ComponentModel;

using gbrainy.Core.Views;
using gbrainy.Core.Libraries;

namespace gbrainy.Core.Main
{
	public class GameSession : IDrawable, IDrawRequest, IMouseEvent
	{
		[Flags]
		public enum Types
		{	
			None			= 0,
			LogicPuzzles		= 2,
			MemoryTrainers		= 4,
			CalculationTrainers	= 8,
			VerbalAnalogies		= 16,
			Custom			= 32,
			TrainersOnly		= MemoryTrainers | CalculationTrainers,
			AllGames		= MemoryTrainers | CalculationTrainers | LogicPuzzles
		}

		private enum ScoresType
		{
			None = 0,
			LogicPuzzles,
			MemoryTrainers,
			CalculationTrainers,
			VerbalAnalogies,
			Last			
		}

		public enum SessionStatus
		{
			NotPlaying,
			Playing,
			Answered,
			Finished,
		}

		// Data kept for every game type in raw data (processed with a formula after)
		public class Statistics
		{
			public int Played { get; set; }
			public int Won { get; set; }
			public int Scored { get; set; }
		}

		private TimeSpan game_time;
		private int games_played;
		private int games_won;
		private Game current_game;
		private GameManager game_manager;
		private System.Timers.Timer timer;
		private bool paused;
		private string current_time;
		private TimeSpan one_sec = TimeSpan.FromSeconds (1);
		private int total_score;
		private bool scored_game;
		private SessionStatus status;
		private ViewsControler controler;
		private ISynchronizeInvoke synchronize;
		private PlayerHistory history;
		private Statistics [] statistics;
		private int id;

		public event EventHandler DrawRequest;
		public event EventHandler <UpdateUIStateEventArgs> UpdateUIElement;
	
		public GameSession ()
		{
			id = 0;
			game_manager = new GameManager ();
			game_time = TimeSpan.Zero;

			timer = new System.Timers.Timer ();
			timer.Elapsed += TimerUpdater;
			timer.Interval = (1 * 1000); // 1 second

			statistics = new Statistics [(int) ScoresType.Last];

			for (int i = 0; i < (int) ScoresType.Last; i++)
				statistics [i] = new Statistics ();

			controler = new ViewsControler (this);
			Status = SessionStatus.NotPlaying;
			history = new PlayerHistory ();
		}

		public int ID {
			get {return id;}
		}

		public PlayerHistory PlayerHistory { 
			set { history = value; }
			get { return history; }
		}

		public ISynchronizeInvoke SynchronizingObject { 
			set { synchronize = value; }
			get { return synchronize; }
		}
	
		public Types Type {
			get {return game_manager.GameType; }
			set {game_manager.GameType = value; }
		}

		public Game.Difficulty Difficulty {
			get {return game_manager.Difficulty; }
			set {game_manager.Difficulty = value; }
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
			set {
				current_game = value; 
				controler.Game = value;
			}
		}

		public bool EnableTimer {
			get {return timer.Enabled; }
			set {timer.Enabled = value; }
		}

		public SessionStatus Status {
			get {return status; }
			set {
				status = value;
				controler.Status = value;
			}
		}

		public GameManager GameManager {
			get {return  game_manager;}
		}

		public int TotalScore {
			get {return total_score;}
		}

		public int LogicScore {
			get {
				if (statistics [(int) ScoresType.LogicPuzzles].Played == 0)
					return -1;

				return ScoreFormula (statistics [(int) ScoresType.LogicPuzzles]);
			}
		}

		public int MemoryScore {
			get {
				if (statistics [(int) ScoresType.MemoryTrainers].Played == 0)
					return -1;
	
				return ScoreFormula (statistics [(int) ScoresType.MemoryTrainers]);
			}
		}

		public int MathScore {
			get {
				if (statistics [(int) ScoresType.CalculationTrainers].Played == 0)
					return -1;

				return ScoreFormula (statistics [(int) ScoresType.CalculationTrainers]);
			}
		}

		public int VerbalScore {
			get {	
				if (statistics [(int) ScoresType.VerbalAnalogies].Played == 0)
					return -1;
	
				return ScoreFormula (statistics [(int) ScoresType.VerbalAnalogies]);
			}
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
				if (Status == SessionStatus.NotPlaying)
					return string.Empty;

				String text;
				text = String.Format (Catalog.GetString ("Games played: {0} ({1}% score)"),games_played, total_score);
				text += String.Format (Catalog.GetString (" - Time: {0}"), current_time);

				if (CurrentGame != null)
	 				text += " " + String.Format (Catalog.GetString ("- Game: {0}"), CurrentGame.Name);
	
				return text;
			}
		}
	
		public void NewSession ()
		{
			id++;
			if (Status != SessionStatus.NotPlaying)
				EndSession ();

			current_time = TimeSpanToStr (game_time);

			for (int i = 0; i < (int) ScoresType.Last; i++)
				statistics [i] = new Statistics ();

			total_score = 0;
			games_played = 0;
			games_won = 0;
			game_time = TimeSpan.Zero;
			timer.SynchronizingObject = SynchronizingObject;
			EnableTimer = true;
		}

		public void EndSession ()
		{
			history.SaveGameSession (this);

			if (CurrentGame != null)
				CurrentGame.Finish ();

			EnableTimer = false;
			timer.SynchronizingObject = null;

			paused = false;
			CurrentGame = null;
			Status = SessionStatus.Finished;
		}

		public void NextGame ()
		{	
			if (CurrentGame != null)
				CurrentGame.Finish ();

			games_played++;
			CurrentGame = game_manager.GetPuzzle ();
			CurrentGame.SynchronizingObject = SynchronizingObject;
			CurrentGame.DrawRequest += GameDrawRequest;
			CurrentGame.UpdateUIElement += GameUpdateUIElement;

			CurrentGame.Initialize ();

			CurrentGame.GameTime = TimeSpan.Zero;
			scored_game = false;
			Status = SessionStatus.Playing;
		}

		public void Pause ()
		{
			EnableTimer = false;
			paused = true;
			current_time = Catalog.GetString ("Paused");
		}

		public void Resume ()
		{
			EnableTimer = true;
			paused = false;
		}

		/*
			How the game session is scored

			* Every game has a scoring algorithm that scores the player performance within the game.
		   	  This takes into account time used and tips (result is from 0 to 10)
			* The results are added to the games and scores arrays where we store the results for
			  the different game types (verbal, logic, etc)
			* We apply a ScoreFormula function that balances the total result with the number of
	  		  games played (is not the same 100% games won playing 2 than 10 games) and the difficulty
			
			The final result is a number from 0 to 100
		*/

		public bool ScoreGame (string answer)
		{
			int score;
			bool won;
			int components = 0;

			if (CurrentGame == null || scored_game == true)
				return false;

			score = CurrentGame.Score (answer);
			if (score > 0) {
				GamesWon++;
				won = true;
			} else
				won = false;

			switch (CurrentGame.Type) {
			case Game.Types.LogicPuzzle:
				statistics [(int) ScoresType.LogicPuzzles].Scored += score;
				statistics [(int) ScoresType.LogicPuzzles].Played++;
				if (won) statistics [(int) ScoresType.LogicPuzzles].Won++;
				break;
			case Game.Types.MemoryTrainer:
				statistics [(int) ScoresType.MemoryTrainers].Scored += score;
				statistics [(int) ScoresType.MemoryTrainers].Played++;
				if (won) statistics [(int) ScoresType.MemoryTrainers].Won++;
				break;
			case Game.Types.MathTrainer:
				statistics [(int) ScoresType.CalculationTrainers].Scored += score;
				statistics [(int) ScoresType.CalculationTrainers].Played++;
				if (won) statistics [(int) ScoresType.CalculationTrainers].Won++;
				break;
			case Game.Types.VerbalAnalogy:
				statistics [(int) ScoresType.VerbalAnalogies].Scored += score;
				statistics [(int) ScoresType.VerbalAnalogies].Played++;
				if (won) statistics [(int) ScoresType.VerbalAnalogies].Won++;
				break;
			default:
				break;
			}

			total_score = 0;

			// Updates total score taking only into account played game types
			if (LogicScore >= 0) {
				total_score += LogicScore;
				components++;
			}

			if (MemoryScore >= 0) {
				total_score += MemoryScore;
				components++;
			}

			if (MathScore >= 0) {
				total_score += MathScore;
				components++;
			}

			if (VerbalScore >= 0) {
				total_score += VerbalScore;
				components++;
			}

			total_score = total_score / components;

			scored_game = true;
			return won;
		}

		//
		// Applies scoring formula to the session
		//
		int ScoreFormula (Statistics stats)
		{
			int logbase;
			double score, factor;

			switch (Difficulty) {
			case Game.Difficulty.Easy:
				logbase = 10;
				break;
			case Game.Difficulty.Medium:
				logbase = 20;
				break;
			case Game.Difficulty.Master:
				logbase = 30;
				break;
			default:
				throw new InvalidOperationException ("Invalid switch value");
			}

			// Simple percentage of games won vs played
			score = stats.Scored > 0 ? stats.Scored / stats.Played * 10 : 0;

			// Puts score of the game in prespective for the whole game
			factor = Math.Log (stats.Won + 2, logbase); // +2 to avoid log 0

			score = score * factor;

			if (score > 100) score = 100;

			return (int) score;
		}

		private void TimerUpdater (object source, ElapsedEventArgs e)
		{
			lock (this) {
				if (CurrentGame == null)
					return;

				game_time = game_time.Add (one_sec);
				CurrentGame.GameTime = CurrentGame.GameTime + one_sec;
				current_time = TimeSpanToStr (game_time);
			}

			if (UpdateUIElement == null)
				return;

			UpdateUIElement (this, new UpdateUIStateEventArgs (UpdateUIStateEventArgs.EventUIType.Time, null));
		}

		static private string TimeSpanToStr (TimeSpan time)
		{
			string fmt = time.ToString ();
			int i = fmt.IndexOf ('.');
			if (i > 0 && fmt.Length - i > 2)
				fmt = fmt.Substring (0, i);

			return fmt;
		}

		public void GameUpdateUIElement (object obj, UpdateUIStateEventArgs args)
		{
			if (UpdateUIElement != null)
				UpdateUIElement (this, args);
		}

		// A game has requested a redraw, scale the request to the object
		// subscribed to GameSession.GameDrawRequest
		public void GameDrawRequest (object o, EventArgs args)
		{
			if (DrawRequest != null)
				DrawRequest (this, EventArgs.Empty);
		}

		public virtual void Draw (CairoContextEx gr, int width, int height, bool rtl)
		{
			controler.CurrentView.Draw (gr, width, height, rtl);
		}

		public void MouseEvent (object obj, MouseEventArgs args)
		{
			if (controler.CurrentView as IMouseEvent != null)
				(controler.CurrentView as IMouseEvent).MouseEvent (this, args);
		}
	}
}
