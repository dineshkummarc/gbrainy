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
using System.Text;
using Mono.Unix;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Logic
{
	public class PuzzleNumericSequence : Game
	{
		private const int max_num = 6;
		private int[] numbers;
		private int formula;

		public override string Name {
			get {return Catalog.GetString ("Numeric sequence");}
		}

		public override string Question {
			get {return Catalog.GetString ("The next sequence follows a logic. What number should replace the question mark?");} 
		}

		public override string Tip {
			get { return Catalog.GetString ("Every number in the sequence is related to the previous one.");}
		}

		public override string Rationale {
			get { 
				switch (formula) {
				case 0:
					return Catalog.GetString ("Every number in the sequence is the result of subtracting 1 from the previous number and multiplying it by 2.");
				case 1:
					return Catalog.GetString ("Every number in the sequence is the result of adding 1 to the previous number and multiplying it by 3.");
				case 2:
					return Catalog.GetString ("Every number in the sequence is the result of subtracting 2 from the previous number and multiplying it by -2.");
				default:
					return string.Empty;
				}
			}
		}

		public override void Initialize ()
		{
			int[] seeds;

			formula = random.Next (CurrentDifficulty == Difficulty.Easy ? 2 : 3);
			numbers =  new int [max_num];

			switch (formula) {
			case 0:
				seeds = new int [] {3, 4, 5};
				break;
			case 1:
				// Skipping seed value 5 -> 5,18,57,174,525
				// The difference between given adjacent pairs ( right -left) is a sequence 13,39,117,351. 
				// They can be related as 13 x 3 raised to sequential powers 0,1,2,3 ie 13*3^1 and so on.
				seeds = new int [] {3, 4, 6};
				break;
			case 2:
				seeds = new int [] {3, 4, 5};
				break;
			default:
				throw new InvalidOperationException ();
			}

			numbers[0] = seeds [random.Next (seeds.Length)];

			for (int i = 1; i < max_num; i++) {
				switch (formula) {
				case 0:
					numbers[i] = (numbers[i - 1] - 1) * 2;
					break;
				case 1:
					numbers[i] = (numbers[i - 1] + 1) * 3;
					break;
				case 2:
					numbers[i] = (numbers[i -1] - 2) * (-2);
					break;
				default:
					throw new InvalidOperationException ();
				}
			}

			right_answer = numbers[max_num-1].ToString ();
		}


		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			StringBuilder sequence = new StringBuilder (64);

			base.Draw (gr, area_width, area_height, rtl);
			gr.SetPangoLargeFontSize ();

			for (int num = 0; num < max_num - 1; num++)
			{
				sequence.Append (numbers[num]);
				sequence.Append (", ");
			}
			sequence.Append ("?");

			gr.DrawTextCentered (0.5, DrawAreaY + 0.3, sequence.ToString ());
		}
	}
}
