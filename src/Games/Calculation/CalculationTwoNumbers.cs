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
using Cairo;
using Mono.Unix;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Calculation
{
	public class CalculationTwoNumbers : Game
	{
		int number_a, number_b;
		int op1, op2, max_operand;
		GameTypes type;

		enum GameTypes
		{
			Addition,
			Subtraction,
			Length
		};

		public override string Name {
			get {return Catalog.GetString ("Two numbers");}
		}

		public override Types Type {
			get { return Game.Types.MathTrainer;}
		}

		public override string Question {
			get {
				switch (type) {
				case GameTypes.Addition:
					return String.Format (Catalog.GetString ("Which two numbers when added are {0} and when multiplied are {1}?"), op1, op2);

				case GameTypes.Subtraction:
					return String.Format (Catalog.GetString ("Which two numbers when subtracted are {0} and when multiplied are {1}?"), op1, op2);
				default:
					throw new InvalidOperationException ();
				}
			}
		}

		public override void Initialize ()
		{
			type = (GameTypes) random.Next ((int) GameTypes.Length);

			switch (CurrentDifficulty) {
			case Difficulty.Easy:
				max_operand = 8;
				break;
			case Difficulty.Medium:
				max_operand = 10;
				break;
			case Difficulty.Master:
				max_operand = 15;
				break;
			}

			number_a = 5 + random.Next (max_operand);
			number_b = 3 + random.Next (max_operand);

			switch (type) {
			case GameTypes.Addition:
				op1 = number_a + number_b;
				break;
			case GameTypes.Subtraction:
				if (number_a < number_b) {
					int tmp = number_a;

					number_a = number_b;
					number_b = tmp;
				}
				op1 = number_a - number_b;
				break;
			default:
				throw new InvalidOperationException ();
			}

			op2 = number_a * number_b;

			right_answer = String.Format (Catalog.GetString ("{0} and {1}"), number_a, number_b);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{	
			double x = DrawAreaX + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			gr.MoveTo (x, DrawAreaY + 0.22);

			switch (type) {
			case GameTypes.Addition:
				gr.ShowPangoText (String.Format (Catalog.GetString ("number1 + number2 = {0}"), op1));
				break;
			case GameTypes.Subtraction:
				gr.ShowPangoText (String.Format (Catalog.GetString ("number1 - number2 = {0}"), op1));
				break;
			default:
				throw new InvalidOperationException ();
			}
		
			gr.MoveTo (x, DrawAreaY + 0.44);
			gr.ShowPangoText (String.Format (Catalog.GetString ("number1 * number2 = {0}"), op2));
		}

		public override bool CheckAnswer (string answer)
		{	
			string num_a = string.Empty;
			string num_b = string.Empty;
			bool first = true;
		
			for (int c = 0; c < answer.Length; c++)
			{
				if (answer[c] < '0' || answer[c] > '9') {
					first = false;
					continue;
				}
			
				if (first == true)
					num_a += answer[c];
				else
					num_b += answer[c];
			}

			try {
				if (Int32.Parse (num_a) == number_a && Int32.Parse (num_b) == number_b ||
					Int32.Parse (num_b) == number_a && Int32.Parse (num_a) == number_b)
					return true;
			}

			catch (FormatException) {
				return false;
			}
	
			return false;
		}
	}
}
