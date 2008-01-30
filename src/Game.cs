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
using Cairo;
using Mono.Unix;

abstract public class Game
{
	public enum Types
	{	
		None			= 0,
		LogicPuzzle		= 2,
		MemoryTrainer		= 4,
		MathTrainer		= 8
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
	private Cairo.Color default_background;
	protected string right_answer;
	protected Random random;
	private TimeSpan game_time;
	private bool won;
	private bool tip_used;
	private Difficulty difficulty;

	public Game ()
	{
		random = new Random ();
		application = null;
		draw_answer = false;
		default_color = new Cairo.Color (0, 0, 0);
		default_background = new Color (1, 1, 1);
		won = false;
		tip_used = false;
		difficulty = Difficulty.Medium;
	}

	public abstract string Question {
		get;
	}

	public virtual string Answer {
		get {
			return String.Format (Catalog.GetString ("The right answer is {0}."), right_answer);
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

	// Avergate time in second that a player is expected to complete this game
	public int AverageTime {
		get {
			switch (Type) {
			case Types.MemoryTrainer:
				return 30;
			case Types.MathTrainer:
				return 60;				
			}
			return 120; // Default for all games (logic)
		}
	}

	//
	// Score algoritm return a value between 0 and 10
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
			return (int) score;
		}
	}

	
	public abstract void Initialize ();
	public abstract void Draw (Cairo.Context gr, int width, int height);
	public virtual void Finish () {}

	public virtual void DrawPreview (Cairo.Context gr, int width, int height)
	{
		Draw (gr, width, height);
	}

	public virtual bool CheckAnswer (string answer)
	{
		return (String.Compare (answer, right_answer, true) == 0);
	}
	
	public void SetLargeFont (Cairo.Context gr)
	{
		gr.SetFontSize (0.05);
	}

	public void SetNormalFont (Cairo.Context gr)
	{
		gr.SetFontSize (0.03);
	}

	virtual public void PrepareGC (Cairo.Context gr)
	{
		gr.LineWidth = LineWidth;
		gr.Color = DefaultDrawingColor;
		gr.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold);
		SetNormalFont (gr);
	}

	protected void DrawBackground (Cairo.Context gr)
	{
		int columns = 40;
		int rows = 40;
		double rect_w = 1.0 / rows;
		double rect_h = 1.0 / columns;

		gr.Save ();

		gr.Color = default_background;
		gr.Paint ();	
		
		gr.Color = new Cairo.Color (0.9, 0.9, 0.9);
		gr.LineWidth = 0.001;
		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {			
				gr.Rectangle (row * rect_w, column * rect_h, rect_w, rect_h);
			}
		}
		gr.Stroke ();
		gr.Restore ();		
	}
}

