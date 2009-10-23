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

public class CalculationRatio : Game
{
	int number_a, number_b, ratio_a, ratio_b;

	public override string Name {
		get {return Catalog.GetString ("Ratio");}
	}

	public override Types Type {
		get { return Game.Types.MathTrainer;}
	}

	public override string Question {
		get {
			return String.Format (
				Catalog.GetString ("Two numbers that sum {0} have a ratio of {1} to {2}. Which are these numbers?"), 
				number_a + number_b, ratio_a, ratio_b);
		}
	}


	public override string Answer {
		get {
			string answer = base.Answer + " ";

			answer += String.Format (Catalog.GetString ("The second number is calculated by multiplying the first by {0} and dividing it by {1}."),
				ratio_a, ratio_b);
			return answer;
		}
	}

	public override string Tip {
		get { return Catalog.GetString ("A ratio specifies a proportion between two numbers. A ratio a:b means that for every 'a' parts you have 'b' parts.");}
	}

	public override void Initialize ()
	{
		int random_max;
		
		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			random_max = 5;
			break;
		default:
		case Difficulty.Medium:
			random_max = 8;
			break;
		case Difficulty.Master:
			random_max = 15;
			break;
		}

		number_a = 10 + random.Next (random_max);

		if (number_a % 2 !=0)
			number_a++;
		
		ratio_a = 2;
		ratio_b = 3 + random.Next (random_max);
		number_b = number_a / ratio_a * ratio_b;

		right_answer = String.Format (Catalog.GetString ("{0} and {1}"), number_a, number_b);
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{	
		double x = DrawAreaX + 0.1;

		base.Draw (gr, area_width, area_height);

		gr.SetPangoLargeFontSize ();

		gr.MoveTo (x, DrawAreaY + 0.22);
		gr.ShowPangoText (String.Format (Catalog.GetString ("number1 + number2 = {0}"), number_a + number_b));
		
		gr.MoveTo (x, DrawAreaY + 0.44);
		gr.ShowPangoText (String.Format (Catalog.GetString ("have a ratio of {0}:{1}"), ratio_a, ratio_b));
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

