/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

public class CalculationFractions : Game
{
	public enum Operation
	{
		Addition,	
		Subtraction,
		LastOperation
	}

	class FormulaFraction
	{
		public int numerator, denominator;
		public Operation operation;

		public FormulaFraction (int numerator, int denominator, Operation operation)
		{
			this.numerator = numerator;
			this.denominator = denominator;
			this.operation = operation;
		}
		
		public double Result {
			get {
				return (double) numerator / (double) denominator; 
			}	
		}
	}

	private int fractions_num, demominator_max, factor_max;
	private FormulaFraction[] fractions;
	private const string format_string = "{0:###.###}";

	public override string Name {
		get {return Catalog.GetString ("Fractions");}
	}

	public override Types Type {
		get { return Game.Types.MathTrainer;}
	}

	public override string Question {
		get {return String.Format (Catalog.GetString ("What is the result of the given operation? You can answer using either a fraction or a number."));} 
	}

	private int Factor {
		get {
			switch (random.Next (factor_max)) {
			case 0:
			default:
				return 2;
			case 1:
				return 3;
			case 2:
				return 5;
			case 3: 
				return 7;
			}
		}
	}

	public override void Initialize ()
	{
		double rslt = 0;
		int factor = Factor;

		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			fractions_num = 2;
			demominator_max = 5;
			factor_max = 2;
			break;
		default:
		case Difficulty.Medium:
			fractions_num = 4;
			demominator_max = 3;
			factor_max = 3;
			break;
		case Difficulty.Master:
			fractions_num = 4;
			demominator_max = 5;
			factor_max = 4;
			break;
		}

		fractions = new FormulaFraction [fractions_num];
		for (int i = 0; i < fractions_num; i++) {
			fractions[i] = new FormulaFraction (1 + random.Next (10), (1 + random.Next (demominator_max)) * factor,
				(Operation) random.Next ((int) Operation.LastOperation));

			if (i == 0)
				fractions[0].operation = Operation.LastOperation; // No operation

			switch (fractions[i].operation) {
			case Operation.Addition:
				rslt += fractions[i].Result;
				break;
			case Operation.Subtraction:
				rslt -= fractions[i].Result;
				break;
			default:
				rslt = fractions[i].Result;
				break;
			}			
		}

		right_answer = String.Format (format_string, rslt);
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{	
		double fraction_size = 0.17;
		double x =  0.5  - (fractions_num * fraction_size / 2), y = DrawAreaY + 0.3;
		double offset_x = 0.12;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);
		gr.SetLargeFont ();

		for (int i = 0; i < fractions_num; i++) 
		{
			// Numerator
			gr.DrawTextAlignedRight (x + offset_x, y, fractions[i].numerator.ToString ());

			// Sign
			gr.MoveTo (x, y + 0.03);	
			switch (fractions[i].operation) {
			case Operation.Addition:
				gr.ShowText ("+");
				break;	
			case Operation.Subtraction:
				gr.ShowText ("-");
				break;
			}
			gr.Stroke ();

			// Line
			gr.MoveTo (x + 0.05, y + 0.02);
			gr.LineTo (x + offset_x + 0.02,  y + 0.02);
			gr.Stroke ();

			// Denominator
			gr.DrawTextAlignedRight (x + offset_x, y + 0.1, fractions[i].denominator.ToString ());

			x += fraction_size;
		}	
	}

	public override bool CheckAnswer (string answer)
	{	
		string num_a = string.Empty;
		string num_b = string.Empty;
		double a, b;
		double rslt;
		bool first = true;		

		for (int c = 0; c < answer.Length; c++)
		{
			if (answer[c] < '0' || answer[c] > '9') {
				if (answer[c] != '-' && answer[c] != '.' && answer[c] != ',') {
					first = false;
					continue;
				}
			}
			
			if (first == true)
				num_a += answer[c];
			else
				num_b += answer[c];
		}

		try {

			if (num_b != string.Empty) {
				a = Double.Parse (num_a);
				b = Double.Parse (num_b);
				rslt = (double) a / (double) b;
			} else {
				rslt = Double.Parse (num_a);
			}

		}

		catch (FormatException) {
			return false;
		}
		return right_answer.Equals (String.Format (format_string, rslt));
	}
}

