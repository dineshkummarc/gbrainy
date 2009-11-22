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
	public class GameSession : IDrawable, IDrawRequest
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
		private ViewsControler controler;
		private ISynchronizeInvoke synchronize;
		private PlayerHistory history;

		public event EventHandler DrawRequest;
		public event EventHandler <UpdateGameQuestionEventArgs> UpdateGameQuestion;
	
		public GameSession ()
		{
			game_manager = new GameManager ();
			game_time = TimeSpan.Zero;

			timer = new System.Timers.Timer ();
			timer.Elapsed += TimerUpdater;
			timer.Interval = (1 * 1000); // 1 second
		
			scores = new int [(int) ScoresType.Last];
			games = new int [(int) ScoresType.Last];
			controler = new ViewsControler (this);
			Status = SessionStatus.NotPlaying;
			history = new PlayerHistory ();
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
				if (games [(int) ScoresType.CalculationTrainers] == 0)
					return 0;
			
				return scores [(int) ScoresType.CalculationTrainers] * 10 / games [(int) ScoresType.CalculationTrainers];
			}
		}

		public int VerbalScore {
			get {
				if (games [(int) ScoresType.VerbalAnalogies] == 0)
					return 0;
			
				return scores [(int) ScoresType.VerbalAnalogies] * 10 / games [(int) ScoresType.VerbalAnalogies];
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
			if (Status != SessionStatus.NotPlaying)
				EndSession ();

			games_played = 0;
			games_won = 0;
			game_time = TimeSpan.Zero;
			timer.Enabled = true;
		}

		public void EndSession ()
		{
			history.SaveGameSession (this);

			if (CurrentGame != null)
				CurrentGame.Finish ();

			timer.Enabled = false;
			paused = false;
			CurrentGame = null;
			games_played = 0;
			games_won = 0;
			game_time = TimeSpan.Zero;
			current_time = TimeSpanToStr (game_time);
			scores = new int [(int) ScoresType.Last];
			games = new int [(int) ScoresType.Last];
			total_score = 0;
			scored_game = false;
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
			CurrentGame.UpdateGameQuestion += GameUpdateGameQuestion;

			CurrentGame.Initialize ();

			CurrentGame.GameTime = TimeSpan.Zero;
			scored_game = false;
			Status = SessionStatus.Playing;
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
			if (CurrentGame == null || scored_game == true)
				return;

			switch (CurrentGame.Type) {
			case Game.Types.LogicPuzzle:
				scores [(int) ScoresType.LogicPuzzles] += CurrentGame.Score;
				games [(int) ScoresType.LogicPuzzles]++;
				break;
			case Game.Types.MemoryTrainer:
				scores [(int) ScoresType.MemoryTrainers] += CurrentGame.Score;
				games [(int) ScoresType.MemoryTrainers]++;
				break;
			case Game.Types.MathTrainer:
				scores [(int) ScoresType.CalculationTrainers] += CurrentGame.Score;
				games [(int) ScoresType.CalculationTrainers]++;
				break;
			case Game.Types.VerbalAnalogy:
				scores [(int) ScoresType.VerbalAnalogies] += CurrentGame.Score;
				games [(int) ScoresType.VerbalAnalogies]++;
				break;
			default:
				break;
			}
		
			total_score = 0;
			for (int i = 0; i < (int) ScoresType.Last; i++) {
				total_score += scores [i];
			}

			total_score = total_score * 10 / games_played;
			scored_game = true;
		}	


		private void TimerUpdater (object source, ElapsedEventArgs e)
		{
			lock (this) {
				game_time = game_time.Add (one_sec);
				CurrentGame.GameTime = CurrentGame.GameTime + one_sec;
				current_time = TimeSpanToStr (game_time);
			}

			//Application.Invoke (delegate {	app.UpdateStatusBar (); } );
		}

		static private string TimeSpanToStr (TimeSpan time)
		{
			string fmt = time.ToString ();
			int i = fmt.IndexOf ('.');
			if (i > 0 && fmt.Length - i > 2)
				fmt = fmt.Substring (0, i);

			return fmt;
		}

		public void GameUpdateGameQuestion (object o, UpdateGameQuestionEventArgs args)
		{
			if (UpdateGameQuestion != null)
				UpdateGameQuestion (this, args);
		}

		// A game has requested a redraw, scale the request to the object
		// subscribed to GameSession.GameDrawRequest
		public void GameDrawRequest (object o, EventArgs args)
		{
			//Console.WriteLine ("GameSession.GameRequestRedraw {0}", o); 
			if (DrawRequest != null)
				DrawRequest (this, EventArgs.Empty);
		}

		public virtual void Draw (CairoContextEx gr, int width, int height, bool rtl)
		{
			//Console.WriteLine ("GameSession.Draw");
			controler.CurrentView.Draw (gr, width, height, rtl);
		}

	}
}
