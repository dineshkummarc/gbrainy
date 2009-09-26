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

public class CalculationProportions : Game
{
	const int options_cnt = 4;
	const int correct_pos = 0;
	double []options;
	ArrayListIndicesRandom random_indices;
	double num, den, percentage, correct;

	public override string Name {
		get {return Catalog.GetString ("Proportions");}
	}

	public override Types Type {
		get { return Game.Types.MathTrainer;}
	}

	public override string Question {
		get {
			return String.Format (
				Catalog.GetString ("A {0}/{1} of 'number A' is {2}% of a 'number B'. 'number A' divided by a 'number B' is? Answer {3}, {4}, {5} or {6}."), 
				num, den, percentage, GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2), GetPossibleAnswer (3));}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";

			answer += String.Format (Catalog.GetString ("The result of the operation is {0:##0.###}. You have to divide {1}/100 by {2}/{3}."), 
				correct, percentage, num, den);
			return answer;
		}
	}

	public override void Initialize ()
	{
		int options_next, random_max, which = 0;
		
		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			random_max = 10;
			break;
		default:
		case Difficulty.Medium:
			random_max = 15;
			break;
		case Difficulty.Master:
			random_max = 25;
			break;
		}

		do {
			// Fraction
			num = 15 + random.Next (random_max) * 2;
			den = 1 + random.Next (random_max);
			percentage = 50 + random.Next (random_max);
		} while (num / den == 1); 	

		options = new double [options_cnt];

		options_next = 0;
		options [options_next++] = correct = percentage / 100 / (num / den);
		options [options_next++] = percentage / 50 * (num / den);
		options [options_next++] = percentage / 100 / (den / num);
		options [options_next++] = percentage / 150 * (den / num);

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
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{	
		double x = DrawAreaX + 0.25, y = DrawAreaY + 0.16;
		int indx;

		base.Draw (gr, area_width, area_height);

		gr.SetPangoLargeFontSize ();

		for (int i = 0; i < options_cnt; i++)
		{
			gr.MoveTo (x, y);
			indx = random_indices[i];
			gr.ShowPangoText (String.Format ("{0}) {1:##0.###}", GetPossibleAnswer (i) , options [indx]));

			y = y + 0.15;
		}
	}
}

