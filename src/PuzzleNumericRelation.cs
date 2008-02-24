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

public class PuzzleNumericRelation : Game
{
	private const int group_size = 3;
	private int sum_value;
	private int question;
	private int[] numbers;
	private int formula;
	private const int max_num = 9;

	public override string Name {
		get {return Catalog.GetString ("Numeric relation");}
	}

	public override string Question {
		get {return Catalog.GetString ("What number should replace the question mark?");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("The numbers are related arithmetically.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";

			switch (formula) {
			case 0:
				answer += String.Format (Catalog.GetString ("Every group of {0} numbers sums exactly {1}."), group_size, sum_value);
				break;
			case 1:
				answer += Catalog.GetString ("Divide the sequence in groups of three numbers. Every third number is calculated by multiplying by the two previous ones.");
				break;

			case 2:
				answer += Catalog.GetString ("Divide the sequence in groups of three numbers. Every third number is calculated by subtracting the second number from the first.");
				break;
			}
			return answer;
		}
	}

	public override void Initialize ()
	{
		int group = 0, inc = 0;

		if (CurrentDifficulty == Difficulty.Easy) {
			sum_value = 10 + random.Next (10);
			inc = 5;
		}
		else {
			sum_value = 30 + random.Next (10);
			inc = 12;
		}

		question = 1 + random.Next (max_num - 2);
		formula = random.Next (3);
		numbers =  new int [max_num];
		
		for (int i = 0; i < max_num; i++) {
			if (group == group_size - 1) {	
				switch (formula) {
				case 0:
					numbers[i] = sum_value - numbers[i - 1] - numbers[i - 2];
					break;
				case 1:
					numbers[i] = numbers[i - 1] * numbers[i - 2];
					break;
				case 2:
					numbers[i] = numbers[i - 2] - numbers[i - 1];
					break;
				}
				group = 0;
				continue;
			}
			numbers[i] = 1 + random.Next (inc);
			group++;
		}

		right_answer = numbers[question].ToString ();
	}


	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		StringBuilder sequence = new StringBuilder (64);

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);
		SetLargeFont (gr);

		for (int num = 0; num < max_num - 1; num++)
		{
			if (num == question) {
				sequence.Append ("?, ");
			} else {
				sequence.Append (numbers[num]);
				sequence.Append (", ");
			}
		}
		sequence.Append (numbers[max_num - 1]);
		gr.DrawTextCentered (0.5, DrawAreaY + 0.3, sequence.ToString ());
	}

}


