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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Unix;

using gbrainy.Core.Views;
using gbrainy.Core.Libraries;
using gbrainy.Core.Toolkit;

namespace gbrainy.Core.Main
{
	abstract public class Game : IDrawable, IDrawRequest, IMouseEvent
	{
		// See: GetGameTypeDescription
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

		[Flags]
		public enum AnswerCheckAttributes
		{
			None			= 0,
			Trim			= 2,
			IgnoreCase		= 4,
			IgnoreSpaces		= 8,
			MatchAll		= 16,
		}

		public const char AnswerSeparator = '|';

		public class AnswerEventArgs : EventArgs
		{
			public AnswerEventArgs (string answer)
			{
				Answer = answer;
			}

			public string Answer { get; set; }
		}

		private bool draw_answer;
		private Cairo.Color default_color;
		protected string right_answer;
		protected Random random;
		private TimeSpan game_time;
		private bool tip_used;
		private bool preview;
		private Difficulty difficulty;
		private ISynchronizeInvoke synchronize;
		private List <Toolkit.Container> containers;

		public event EventHandler DrawRequest;
		public event EventHandler <UpdateUIStateEventArgs> UpdateUIElement;
		public event EventHandler <AnswerEventArgs> AnswerEvent;

		protected Game ()
		{
			random = new Random ();
			default_color = new Cairo.Color (0, 0, 0);
			difficulty = Difficulty.Medium;
			containers = new List <Toolkit.Container> ();
		}

		// Used by games to request a redraw of the view
		protected void OnDrawRequest ()
		{
			if (DrawRequest == null)
				return;

			DrawRequest (this, EventArgs.Empty);
		}

		// Used by games to request a question repaint
		protected void UpdateQuestion (string question)
		{
			if (UpdateUIElement == null)
				return;

			UpdateUIElement (this, new UpdateUIStateEventArgs (UpdateUIStateEventArgs.EventUIType.QuestionText, 
				question));
		}

		public abstract string Question {
			get;
		}

		public virtual string Answer {
			get {
				return String.Format (Catalog.GetString ("The correct answer is {0}."), right_answer);
			}
		}

		public ISynchronizeInvoke SynchronizingObject { 
			set { 
				synchronize = value;
			}
			get { return synchronize; }
		}

		public bool IsPreviewMode {
			get {return preview; }
			set {preview = value; }
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

		// How to check the answer
		public virtual AnswerCheckAttributes CheckAttributes {
			get { return AnswerCheckAttributes.Trim | AnswerCheckAttributes.IgnoreCase; }
		}

		public virtual string AnswerCheckExpression {
			get { return ".+"; }
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

		// An initialized game cannot be playable (for example, missing external files)
		public virtual bool IsPlayable {
			get { return true;}
		}

		public virtual bool UsesColors {
			get { return false;}
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

		// Expected time in seconds that a player is expected to complete this game
		public int ExpectedTime {
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
				case Types.VerbalAnalogy:
					return (int) (30 * factor);
				}
				return (int) (120 * factor); // Default for all games (logic)
			}
		}

		protected Widget [] Widgets {
			get { return containers.ToArray (); }
		}

		//
		// Score algorithm returns a value between 0 and 10
		//
		public virtual int Score (string answer)
		{
			double score;
			double seconds = GameTime.TotalSeconds;

			if (CheckAnswer (answer) == false)
				return 0;

			score = 10;
	
			// Time
			if (seconds > ExpectedTime * 3) {
				score = score * 0.6;
			}
			else if (seconds > ExpectedTime * 2) {
				score = score * 0.7;
			} else if (seconds > ExpectedTime) {
				score = score * 0.8;
			}

			if (tip_used)
				score = score * 0.8;

			return (int) score;
		}

		public void AddWidget (Toolkit.Container container)
		{
			if (containers.Contains (container))
				throw new InvalidOperationException ("Child already exists in container");

			foreach (Toolkit.Container previous in containers)
			{
				if (previous.X == container.X && previous.Y == container.Y &&
					 previous.Width == container.Width && previous.Height == container.Height)
				{
					throw new InvalidOperationException ("Child on the same area exists in container. Overlapping drawings.");
				}
			}

			container.DrawRequest += delegate (object sender, EventArgs e)
			{
				OnDrawRequest ();
			};

			// If the user has selected an item we should propagate an answer
			container.SelectedEvent += delegate (object sender, SeletectedEventArgs e)
			{
				if (AnswerEvent != null)
					AnswerEvent (this, new AnswerEventArgs ((string) e.DataEx));
			};

			containers.Add (container);
		}
	
		public abstract void Initialize ();
		public virtual void Finish () {}

		static public string GetPossibleAnswer (int answer)
		{
			switch (answer) {
				// Translators Note
				// The following series of answers may need to be adapted
				// in cultures with alphabets different to the Latin one.
				// The idea is to enumerate a sequence of possible answers
				// For languages represented with the Latin alphabet use 
				// the same than English
			case 0: // First possible answer for a series (e.g.: Figure A)
				return Catalog.GetString ("A");
			case 1: // Second possible answer for a series
				return Catalog.GetString ("B");
			case 2: // Third possible answer for a series
				return Catalog.GetString ("C");
			case 3: // Fourth possible answer for a series
				return Catalog.GetString ("D");
			case 4: // Fifth possible answer for a series
				return Catalog.GetString ("E");
			case 5: // Sixth possible answer for a series
				return Catalog.GetString ("F");
			case 6: // Seventh possible answer for a series
				return Catalog.GetString ("G");
			case 7: // Eighth possible answer for a series
				return Catalog.GetString ("H");
			default:
				return string.Empty;
			}
		}

		public string GetPossibleFigureAnswer (int answer)
		{
			return String.Format (Catalog.GetString ("Figure {0}"), GetPossibleAnswer (answer));
		}

		protected void InitDraw (CairoContextEx gr, int width, int height, bool rtl)
		{
			gr.Scale (width, height);
			gr.DrawBackground ();
			gr.Color = default_color;
			gr.LineWidth = LineWidth;		
		}

		public virtual void Draw (CairoContextEx gr, int width, int height, bool rtl)
		{
			InitDraw (gr, width, height, rtl);

			foreach (Toolkit.Container container in containers)
				container.Draw (gr, width, height, rtl);
		}

		public virtual void DrawPreview (CairoContextEx gr, int width, int height, bool rtl)
		{
			Draw (gr, width, height, rtl);
		}

		public virtual bool CheckAnswer (string answer)
		{
			Regex regex;
			Match match;
			AnswerCheckAttributes type;
			bool ignore_case, ignore_spaces;

			if (String.IsNullOrEmpty (answer))
				return false;

			ignore_case = (CheckAttributes & AnswerCheckAttributes.IgnoreCase) == AnswerCheckAttributes.IgnoreCase;
			ignore_spaces = (CheckAttributes & AnswerCheckAttributes.IgnoreSpaces) == AnswerCheckAttributes.IgnoreSpaces;
			regex = new Regex (AnswerCheckExpression);

			string [] right_answers = right_answer.Split (AnswerSeparator);

			for (int i = 0; i < right_answers.Length; i++)
			{
				right_answers [i] = right_answers[i].Trim ();

				if (ignore_spaces)
					right_answers [i] = RemoveWhiteSpace (right_answers [i]);
			}

			if ((CheckAttributes & AnswerCheckAttributes.Trim) == AnswerCheckAttributes.Trim)
				answer = answer.Trim ();

			if (ignore_spaces)
				answer = RemoveWhiteSpace (answer);

			// All strings from the list of expected answers (two numbers: 22 | 44) must present in the answer
			if ((CheckAttributes & AnswerCheckAttributes.MatchAll) == AnswerCheckAttributes.MatchAll)
			{
				match = regex.Match (answer);
				while (String.IsNullOrEmpty (match.Value) == false)
				{
					for (int i = 0; i < right_answers.Length; i++)
					{
						if (String.Compare (match.Value, right_answers[i], ignore_case) == 0)
						{
							right_answers[i] = null;
							break;
						}
					}
					match = match.NextMatch ();
				}

				// Have all the expected answers been matched?
				for (int i = 0; i < right_answers.Length; i++)
				{
					if (right_answers[i] != null)
						return false;
				}

				return true;
			}
			else // Any string from the list of possible answers (answer1 | answer2) present in the answer will do it 
			{
				foreach (string s in right_answers)
				{
					match = regex.Match (answer);
					if (String.Compare (match.Value, s, ignore_case) == 0)
						return true;
				}
			}
			return false;
		}

		static string RemoveWhiteSpace (string source)
		{
			string str = string.Empty;
			for (int n = 0; n < source.Length; n++)
			{
				if (char.IsWhiteSpace (source [n]) == false)
					str += source [n];
			}
			return str;
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

		// Type enum to string representation
		static public string GetGameTypeDescription (Types type)
		{
			string str;

			switch (type) 
			{
				case Game.Types.LogicPuzzle:
					str = Catalog.GetString ("Logic");
					break;
				case Game.Types.MemoryTrainer:
					str = Catalog.GetString ("Memory");
					break;
				case Game.Types.MathTrainer:
					str = Catalog.GetString ("Calculation");
					break;
				case Game.Types.VerbalAnalogy:
					str = Catalog.GetString ("Verbal");
					break;
				default:
					str = string.Empty;
					break;
			}
			return str;
		}

		public void DisableMouseEvents ()
		{
			foreach (Toolkit.Container container in containers) 
				foreach (Widget widget in container.Children)
					widget.Sensitive = false;
		}

		public void MouseEvent (object obj, MouseEventArgs args)
		{
			foreach (Toolkit.Container container in containers)
				container.MouseEvent (obj, args);
		}
	}
}
