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
using Mono.Unix;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;

namespace gbrainy.Games.Logic
{
	public class PuzzleOstracism : Game
	{
		enum GameType
		{
			Equations	= 0,
			Numbers,
			Last = Numbers,
		};
		
		ArrayListIndicesRandom random_indices;
		GameType gametype;
		const int max_equations = 5;
		const int wrong_answer = 4;
		string [] equations_a = new string []
		{
			"21 x 60 = 1260",
			"43 x 51 = 1453",
			"80 x 16 = 1806",
			"15 x 93 = 1395",
			"70 x 16 = 1120",
		};

		string [] equations;

		public override string Name {
			get {return Catalog.GetString ("Ostracism");}
		}

		public override string Question {
			get {return String.Format (
				Catalog.GetString ("Which element does not belong to the group? It is not related to divisibility of the numbers. Answer {0}, {1}, {2}, {3} or {4}."),
				GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3), GetPossibleAnswer (4));}
		}

		public override string Tip {
			get {
				switch (gametype) {
				case GameType.Equations:
					return Catalog.GetString ("The criteria for deciding if an equation belongs to the group is not arithmetical.");
				case GameType.Numbers:
					return Catalog.GetString ("Consider that every number that belongs to the group has two parts that are related.");
				default:
					throw new InvalidOperationException ();
				}
			}	
		}

		public override string Rationale {
			get {
				switch (gametype) {
				case GameType.Equations:
					return Catalog.GetString ("In all the other equations the digits from the left side appear also in the right side.");
				case GameType.Numbers:
					return Catalog.GetString ("In all the other numbers the last three digits are the square of the first two digits.");
				default:
					throw new InvalidOperationException ();
				}
			}
		}

		protected override void Initialize ()
		{
			gametype = (GameType) random.Next ((int) GameType.Last + 1);

			switch (gametype) {
			case GameType.Equations:
				equations = equations_a;
				break;
			case GameType.Numbers:
			{
				int num, evens;
				int [] seeds = {25, 26, 28, 30, 18, 21, 22};
				ArrayListIndicesRandom random_seeds = new ArrayListIndicesRandom (seeds.Length);

				// Make sure that one of the numbers only is not even or odd to avoid
				// this rationale as valid answer too
				do
				{
					evens = 0;
					random_seeds.Initialize ();

					equations = new string [max_equations];
					for (int i = 0; i < max_equations - 1; i++)
					{
						num = seeds [random_seeds [i]];
						equations [i] = num.ToString () + (num * num).ToString ();
						if (num % 2 == 0) evens++;
					}
					num = seeds [random_seeds [max_equations - 1]];
					equations [max_equations - 1] = num.ToString () + ((num * num) -  20).ToString ();

					if (num % 2 == 0) evens++;

				} while (evens <= 1 || max_equations - evens <= 1 /* odds */);
				break;
			}
			default:
				throw new InvalidOperationException ();
			}

			random_indices = new ArrayListIndicesRandom (equations.Length);
			random_indices.Initialize ();
			right_answer = string.Empty;

			for (int i = 0; i < random_indices.Count; i++)
			{
				if (random_indices[i] == wrong_answer) {
					right_answer = GetPossibleAnswer (i);
					break;
				}
			}

			Container container = new Container (DrawAreaX + 0.1, DrawAreaY + 0.2, 0.5, random_indices.Count * 0.1);
			AddWidget (container);

			for (int i = 0; i < random_indices.Count; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.5, 0.1);
				drawable_area.X = DrawAreaX + 0.1;
				drawable_area.Y = DrawAreaY + 0.2 + i * 0.1;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = GetPossibleAnswer (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.05, 0.02);
					// Translators: this "option) answer" for example "a) "21 x 60 = 1260". This should not be changed for most of the languages
					e.Context.ShowPangoText (String.Format (Catalog.GetString ("{0}) {1}"), GetPossibleAnswer (n), equations [random_indices[n]]));
				};
			}
		}
	}
}
