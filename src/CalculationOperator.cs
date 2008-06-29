/*
 * Copyright (C) 2007-2008 Brandon Perry <bperry.volatile@gmail.com>
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

public class CalculationOperator : Game
{
	private double number_a, number_b, number_c, total;
	private char[] opers = {'*', '+', '-', '/'};
	private char oper1, oper2;
	
	public override string Name {
		get {return Catalog.GetString ("Operator");}
	}
	
	public override string Tip {
		get {return String.Format( Catalog.GetString ("The first operator is {0}."), oper1);}
	}

	public override Types Type {
		get { return Game.Types.MathTrainer;}
	}

	public override string Question {
		get {return String.Format (Catalog.GetString ("Which operators make {0}, {1}, and {2} equal {3} (answer using +-/*)?"), number_a, number_b, number_c, total);} 
	}

	private double ProcessOperation (double total, double number, char op)
	{
		switch (op) {
		case '-':
			return total - number;
		case '*':
			return total * number;
		case '/':
			return total / number;
		case '+':
		default:
			return total + number;
		}
	}

	public override void Initialize ()
	{
		bool done = false;
		while (done == false) {
			number_a = 5 + random.Next (1000);
			number_b = 3 + random.Next (1000);
			number_c = 7 + random.Next (1000);

			oper1 = opers[random.Next(4)];
			oper2 = opers[random.Next(4)];

			total = ProcessOperation (number_a, number_b, oper1);
			total = ProcessOperation (total, number_c, oper2);

			if (total < 20000 && total > -20000 && total != 0 && total == (int) total)
				done = true;
		}

		right_answer = String.Format (Catalog.GetString ("{0} and {1}"), oper1, oper2);
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{	
		double aligned_pos = 0.58;

		base.Draw (gr, area_width, area_height);

		gr.SetLargeFont ();
		gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.2, number_a.ToString ());
		gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.3, number_b.ToString ());
		gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.4, number_c.ToString ());

		gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.45);
		gr.LineTo (DrawAreaX + 0.5, DrawAreaY + 0.45);
		gr.Stroke ();

		gr.DrawTextAlignedRight (aligned_pos, DrawAreaY + 0.55, total.ToString ());

		gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.25);
		gr.ShowText ((DrawAnswer == true) ? oper1.ToString () : "?");

		gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.35);
		gr.ShowText ((DrawAnswer == true) ?  oper2.ToString () : "?");

	}

	private bool IndexOf (char c, char [] chars)
	{
		for (int i = 0; i < chars.Length; i++)
			if (c == chars [i]) return true;

		return false;
	}

	public override bool CheckAnswer (string answer)
	{	
		char op1 = '\0', op2 = '\0';
		int c = 0;
		
		for (c = 0; c < answer.Length; c++)
		{
			if (IndexOf (answer[c], opers)) {
				op1 = answer[c];
				break;
			}			
		}

		for (c++; c < answer.Length; c++)
		{
			if (IndexOf (answer[c], opers)) {
				op2 = answer[c];
				break;
			}			
		}

		if (oper1 == op1 && oper2 == op2)
			return true;
	
		return false;		
	}	
}


