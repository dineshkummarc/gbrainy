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

public class MathTwoNumbers : Game
{
	private int number_a, number_b;
	private int op1, op2;

	public override string Name {
		get {return Catalog.GetString ("Two numbers");}
	}

	public override Types Type {
		get { return Game.Types.MathTrainer;}
	}

	public override string Question {
		get {return String.Format (Catalog.GetString ("Which two numbers when added are {0} and when multiplied are {1}?"), op1, op2);} 
	}

	public override void Initialize ()
	{	
		number_a = 5 + random.Next (15);
		number_b = 3 + random.Next (10);

		op1 = number_a + number_b;
		op2 = number_a * number_b;

		right_answer = String.Format (Catalog.GetString ("{0} and {1}"), number_a, number_b);
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{	
		double x = DrawAreaX;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);
		SetLargeFont (gr);

		gr.MoveTo (x, DrawAreaY + 0.2);
		gr.ShowText (String.Format (Catalog.GetString ("number1 + number2 = {0}"), op1));
		
		gr.MoveTo (x, DrawAreaY + 0.4);
		gr.ShowText (String.Format (Catalog.GetString ("number1 * number2 = {0}"), op2));
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


