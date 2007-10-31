/*
 * Copyright (C) 2007 Brandon Perry <bperry.volatile@gmail.com>
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

public class MathOperator : Game
{
	private double number_a, number_b, number_c, total;
	private string oper1, oper2, other_right_answer;
	private string[] opers = {"*", "+", "-", "/"};
	
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
		get {return String.Format (Catalog.GetString ("Which operators make {0}, {1}, and {2} equal {3}? (The result is rounded to the nearest integer)"), number_a, number_b, number_c, Round(total));} 
	}

	public override void Initialize ()
	{
		number_a = 5 + random.Next (1000);
		number_b = 3 + random.Next (1000);
		number_c = 7 + random.Next (1000);

		oper1 = opers[random.Next(4)];
		oper2 = opers[random.Next(4)];
			
		if (oper1 == "+") {
			total = (number_a + number_b);
			
			if (oper2 == "+")
				total = (total + number_c);
			else if (oper2 == "-")
				total = (total - number_c);
			else if (oper2 == "*")
				total = (total * number_c);
			else if (oper2 == "/")
				total = (total / number_c);
		}
		if (oper1 == "-"){
			total = (number_a - number_b);
			
			if (oper2 == "+")
				total = (total + number_c);
			else if (oper2 == "-")
				total = (total - number_c);
			else if (oper2 == "*")
				total = (total * number_c);
			else if (oper2 == "/")
				total = (total / number_c);
		}
		if (oper1 == "*"){
			total = (number_a * number_b);
			
			if (oper2 == "+")
				total = (total + number_c);
			else if (oper2 == "-")
				total = (total - number_c);
			else if (oper2 == "*")
				total = (total * number_c);
			else if (oper2 == "/")
				total = (total / number_c);
		}
		if (oper1 == "/"){
			total = (number_a / number_b);
			
			if (oper2 == "+")
				total = (total + number_c);
			else if (oper2 == "-")
				total = (total - number_c);
			else if (oper2 == "*")
				total = (total * number_c);
			else if (oper2 == "/")
				total = (total / number_c);
		}

		right_answer = oper1 + oper2;
		other_right_answer =  oper1 + " " + oper2;
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{	
		double aligned_pos = 0.58;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);
		gr.SetFontSize (0.05);

		DrawingHelpers.DrawTextAlignedRight (gr, aligned_pos, DrawAreaY + 0.2, number_a.ToString ());
		DrawingHelpers.DrawTextAlignedRight (gr, aligned_pos, DrawAreaY + 0.3, number_b.ToString ());
		DrawingHelpers.DrawTextAlignedRight (gr, aligned_pos, DrawAreaY + 0.4, number_c.ToString ());

		gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.45);
		gr.LineTo (DrawAreaX + 0.5, DrawAreaY + 0.45);
		gr.Stroke ();

		DrawingHelpers.DrawTextAlignedRight (gr, aligned_pos, DrawAreaY + 0.55, Round(total).ToString ());

		gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.25);
		gr.ShowText ((DrawAnswer == true) ? oper1 : "?");

		gr.MoveTo (DrawAreaX + 0.2, DrawAreaY + 0.35);
		gr.ShowText ((DrawAnswer == true) ? oper2 : "?");

	}

	public override bool CheckAnswer (string answer)
	{	
		if (answer == right_answer || answer == other_right_answer) 
			return true;
		else
			return false;	
	}
	
	public static double Round(double value_to_round)
    	{
		double floor_value = Math.Floor(value_to_round);
	      	if ((value_to_round - floor_value) > .5)
      		{
		        return (floor_value + 1);
	      	}
	      	else
	      	{
	        	return (floor_value);
	      	}
	}
}


