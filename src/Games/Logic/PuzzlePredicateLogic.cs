/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;
using gbrainy.Core.Toolkit;

namespace gbrainy.Games.Logic
{
	public class PuzzlePredicateLogic : Game
	{
		int question;
		ArrayListIndicesRandom random_indices;
		const int num_options = 4;

		internal struct Predicate
		{
			internal string question;
			internal string [] options;
			internal int answer_index;

			internal Predicate (string question, string op1, string op2, string op3, string op4, int answer_index)
			{
				this.question = question;
				this.answer_index = answer_index;
				
				options = new string [num_options];
				options[0] = op1;
				options[1] = op2;
				options[2] = op3;
				options[3] = op4;
			}
		};

		Predicate [] predicates = 
		{
			new Predicate (String.Format (Catalog.GetString ("If all painters are artists and some citizens of Barcelona are artists. Which of the following sentences is correct? Answer {0}, {1}, {2} or {3}."),
					GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3)),
				Catalog.GetString ("Some citizens of Barcelona are painters"),
				Catalog.GetString ("All citizens of Barcelona are painters"),
				Catalog.GetString ("No citizen of Barcelona is a painter"),
				Catalog.GetString ("None of the other options"),
				3),

			new Predicate (String.Format (Catalog.GetString ("If no ill artist is happy and some artists are happy. Which of the following sentences is correct? Answer {0}, {1}, {2} or {3}."),
					GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3)),
				Catalog.GetString ("Some artist are not ill"),
				Catalog.GetString ("Some painters are not artists"),
				Catalog.GetString ("All artists are happy"),
				Catalog.GetString ("None of the other options"),
				0),

			new Predicate (String.Format (Catalog.GetString ("If people that travels always buy a map and you are not going to travel. Which of the following sentences is correct? Answer {0}, {1}, {2} or {3}."),
				GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3)),
				Catalog.GetString ("You do not have any map"),
				Catalog.GetString ("You do not buy a map"),
				Catalog.GetString ("All people has a map"),
				Catalog.GetString ("None of the other options"),
				3),

			new Predicate (String.Format (Catalog.GetString ("If you whistle if you are happy and you only smile when you whistle. Which of the following sentences is correct? Answer {0}, {1}, {2} or {3}."),
				GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3)),
				Catalog.GetString ("You smile if you are happy"),
				Catalog.GetString ("You are only happy if you whistle"),
				Catalog.GetString ("You whistle if you are not happy"),
				Catalog.GetString ("None of the other options"),
				0),
		};

		public override string Name {
			get {return Catalog.GetString ("Predicate Logic");}
		}

		public override string Question {
			get {return predicates[question].question;} 
		}

		public override string Tip {
			get { return Catalog.GetString ("A triangle can be embedded inside another triangle.");}
		}

		public override string Answer {
			get { 
				string answer = base.Answer + " ";
				return answer;
			}
		}

		public override void Initialize ()
		{
			int answers;
			int correct_answer;

			question = random.Next (predicates.Length);

			correct_answer = predicates [question].answer_index;
			answers = predicates [question].options.Length;
			random_indices = new ArrayListIndicesRandom (answers - 1);
			random_indices.Initialize ();
			random_indices.Add (answers - 1);

			for (int i = 0; i < answers; i++)
			{
				if (random_indices[i] ==  correct_answer) {
					right_answer = GetPossibleAnswer (i);
					break;
				}
			}

			Container container = new Container (DrawAreaX, DrawAreaX + 0.2, 0.8, 0.6);
			AddWidget (container);
	
			for (int i = 0; i <  predicates[question].options.Length; i++)
			{
				int opt = random_indices [i];

				DrawableArea drawable_area = new DrawableArea (0.8, 0.1);
				drawable_area.X = DrawAreaX;
				drawable_area.Y = DrawAreaY + 0.2 + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = GetPossibleAnswer (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int data = (int) e.Data;
					int option = random_indices [data];
					
					e.Context.SetPangoNormalFontSize ();
					e.Context.MoveTo (0.05, 0.02);
					e.Context.ShowPangoText (String.Format (Catalog.GetString ("{0}) {1}"), GetPossibleAnswer (data),
						predicates[question].options[option].ToString ()));
					e.Context.Stroke ();
				};
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();
			gr.MoveTo (0.1, DrawAreaY + 0.05);
			gr.ShowPangoText (Catalog.GetString ("Possible answers are:"));
		}
	}
}
