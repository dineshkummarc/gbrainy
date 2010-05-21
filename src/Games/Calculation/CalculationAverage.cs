/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Games.Calculation
{
	public class CalculationAverage : Game
	{
		const int options_cnt = 4;
		const int correct_pos = 0;
		double []numbers;
		double []options;
		ArrayListIndicesRandom random_indices;
		double correct;

		public override string Name {
			get {return Catalog.GetString ("Average");}
		}

		public override Types Type {
			get { return Game.Types.MathTrainer;}
		}

		public override string Question {
			get {
				string nums = string.Empty;
	
				for (int i = 0; i < numbers.Length - 1; i++)
					nums += numbers[i] + ", ";

				nums += numbers [numbers.Length - 1];

				return String.Format (
					Catalog.GetString ("Given the numbers: {0}. Which of the following numbers is the nearest to the average? Answer {1}, {2}, {3} or {4}."), nums,
					GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3));}
		}

		public override string Tip {
			get { return Catalog.GetString ("The average of a list of numbers is the sum of all of the numbers divided by the number of items in the list.");}
		}

		public override string Rationale {
			get { 
				return String.Format (Catalog.GetString ("The result of the operation is {0:##0.###}."), 
					correct);				
			}
		}

		public override void Initialize ()
		{
			bool duplicated;
			int nums, options_next, dist, num_size, which = 0;

			switch (CurrentDifficulty) {
			case Difficulty.Easy:
				nums = 3;
				dist = nums * 3;
				num_size = 50;
				break;
			default:
			case Difficulty.Medium:
				nums = 5;
				dist = nums * 3;
				num_size = 150;
				break;
			case Difficulty.Master:
				nums = 7;
				dist = nums * 3;
				num_size = 500;
				break;
			}

			numbers = new double [nums];
			options = new double [options_cnt];

			do
			{
				// Random set of numbers
				correct = 0;
				for (int i = 0; i < nums; i++)
				{
					numbers [i] = 10 + random.Next (num_size) + dist;
					correct += numbers [i];
				}

				correct = correct / nums;

			} while (correct != Math.Truncate (correct));

			options [correct_pos] = correct;
			options_next = correct_pos + 1;

			// Generate possible answers
			while (options_next < options_cnt) {
				double ans;

				ans = correct + random.Next (dist);
				duplicated = false;	

				// No repeated answers
				for (int num = 0; num < options_next; num++)
				{
					// Due to decimal precission
					if (options [num] == ans || options [num] == ans + 1 || options [num] == ans - 1) {
						duplicated = true;
						break;
					}
				}

				if (duplicated)
					continue;
			
				options [options_next] = ans;
				options_next++;
			}

			random_indices = new ArrayListIndicesRandom (options_cnt);
			random_indices.Initialize ();
		
			for (int i = 0; i < options_cnt; i++)
			{
				if (random_indices [i] == correct_pos) {
					which = i;
					break;
				}
			}

			right_answer += GetPossibleAnswer (which);

			// Options
			double x = DrawAreaX + 0.25, y = DrawAreaY + 0.16;
			Container container = new Container (x, y,  1 - (x * 2), 0.6);
			AddWidget (container);
	
			for (int i = 0; i < options_cnt; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.3, 0.1);
				drawable_area.X = x;
				drawable_area.Y = y + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = GetPossibleAnswer (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;
					int indx = random_indices[n];

					e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.02, 0.02);
					e.Context.ShowPangoText (String.Format ("{0}) {1:##0.###}", GetPossibleAnswer (n) , options [indx]));
				};
			}
		}
	}
}
