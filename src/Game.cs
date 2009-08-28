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

abstract public class Game
{
	public enum Types
	{	
		None			= 0,
		LogicPuzzle		= 2,
		MemoryTrainer		= 4,
		MathTrainer		= 8,
		VerbalAnalogy		= 16,
	}

	public enum Difficulty
	{
		None			= 0,
		Easy			= 2,
		Medium			= 4,
		Master			= 8,
	}

	private bool draw_answer;
	private gbrainy application;
	private Cairo.Color default_color;
	protected string right_answer;
	protected Random random;
	private TimeSpan game_time;
	private bool won;
	private bool tip_used;
	private Difficulty difficulty;
	private bool trace_score = false; // Set to true to debug scoring

	protected Game ()
	{
		random = new Random ();
		draw_answer = false;
		default_color = new Cairo.Color (0, 0, 0);
		won = false;
		tip_used = false;
		difficulty = Difficulty.Medium;
	}

	public abstract string Question {
		get;
	}

	public virtual string Answer {
		get {
			return String.Format (Catalog.GetString ("The correct answer is {0}."), right_answer);
		}
	}

	// Stores how difficult the game is
	public virtual Difficulty GameDifficulty {
		get {
			return Difficulty.Master | Difficulty.Medium | Difficulty.Easy;
		}
	}

	// The level of difficulty selected for the current game
	public virtual Difficulty CurrentDifficulty {
		set {
			difficulty = value;
		}
		get {
			return difficulty;
		}
	}

	public abstract string Name {
		get;
	}

	public string TipString {
		get { 
			string tip = Tip;
	
			if (tip != string.Empty)
				tip_used = true;

			return tip;
		}
	}

	public virtual string Tip {
		get { return string.Empty;}
	}
	
	public gbrainy App {
		get {return application; }
		set {application = value; }
	}

	public virtual bool ButtonsActive {
		get { return true;}
	}

	public virtual Types Type {
		get { return Types.LogicPuzzle;}
	}

	public bool DrawAnswer {
		get { return draw_answer; }
		set { draw_answer = value; }
	}

	public virtual double DrawAreaX {
		get {return 0.1;}
	}

	public virtual double DrawAreaY {
		get {return 0.1;}
	}

	public virtual double DrawAreaWidth {
		get {return 1 - DrawAreaX * 2;}
	}

	public virtual double DrawAreaHeight {
		get {return 1 - DrawAreaY * 2;}
	}

	public virtual double LineWidth {
		get {return 0.005; }
	}

	public virtual Cairo.Color DefaultDrawingColor {
		get {return default_color; }
	}

	public TimeSpan GameTime {
		get {return game_time; }
		set {game_time = value; }
	}

	public bool Won {
		get { return won; }
		set { won = value; }
	}

	// Average time in seconds that a player is expected to complete this game
	public int AverageTime {
		get {
			double factor;

			switch (CurrentDifficulty) {
			case Difficulty.Easy:
				factor = 1.3;
				break;
			case Difficulty.Master:
				factor = 0.7;
				break;		
			case Difficulty.Medium:
			default:
				factor = 1.0;
				break;		
			}
				
			switch (Type) {
			case Types.MemoryTrainer:
				return (int) (30 * factor);
			case Types.MathTrainer:
				return (int) (60 * factor);
			}
			return (int) (120 * factor); // Default for all games (logic)
		}
	}

	//
	// Score algorithm return a value between 0 and 10
	//
	public virtual int Score {
		get {
			double score;
			double seconds = GameTime.TotalSeconds;

			if (won == false) {
				score = 0;
			} else {		
				score = 10;
			
				// Time
				if (seconds > AverageTime * 3) {
					score = score * 0.6;
				}
				else if (seconds > AverageTime * 2) {
					score = score * 0.7;
				} else if (seconds > AverageTime) {
					score = score * 0.8;
				}
		
				if (tip_used) {
					score = score * 0.8;
				}
			}

			if (trace_score) {
				Console.WriteLine ("Score for game {0} is {1}. Used tip {2}, time used {3}, time expected {4}", 
					this, (int) score, tip_used, seconds, AverageTime);
			}

			return (int) score;
		}
	}
	
	public abstract void Initialize ();
	public virtual void Finish () {}

	static public string GetPossibleAnswer (int answer)
	{
		switch (answer) {
			// Translators Note
			// The following serie of answers may need to be adapted
			// in cultures with alphabets different to the Latin one.
			// The idea is to enumerate a sequence of possible answers
			// For languages represented with the Latin alphabet use 
			// the same than English
		case 0: // First possible answer for a serie (e.g.: Figure A)
			return Catalog.GetString ("A");
		case 1: // Second possible answer for a serie
			return Catalog.GetString ("B");
		case 2: // Third possible answer for a serie
			return Catalog.GetString ("C");
		case 3: // Fourth possible answer for a serie
			return Catalog.GetString ("D");
		case 4: // Fifth possible answer for a serie
			return Catalog.GetString ("E");
		case 5: // Sixth possible answer for a serie
			return Catalog.GetString ("F");
		case 6: // Seventh possible answer for a serie
			return Catalog.GetString ("G");
		case 7: // Eighth possible answer for a serie
			return Catalog.GetString ("H");
		default:
			return string.Empty;
		}
	}

	public string GetPossibleFigureAnswer (int answer)
	{
		return String.Format (Catalog.GetString ("Figure {0}"), GetPossibleAnswer (answer));
	}

	public virtual void Draw (CairoContextEx gr, int width, int height)
	{
		gr.Scale (width, height);
		gr.DrawBackground ();
		gr.Color = new Cairo.Color (0, 0, 0);
		gr.LineWidth = LineWidth;
	}

	public virtual void DrawPreview (CairoContextEx gr, int width, int height)
	{
		Draw (gr, width, height);
	}

	public virtual bool CheckAnswer (string answer)
	{
		return (String.Compare (answer, right_answer, true) == 0);
	}

	// When asking for a list of figures people trends to use spaces or commas
	// to separate the elements
	static public string TrimAnswer (string answer)
	{
		string rslt = string.Empty;

		for (int i = 0; i < answer.Length; i++)
		{
			if (answer[i]==' ' || answer[i] == ',')
				continue;

			rslt += answer[i];
		}
		return rslt;
	}
}

