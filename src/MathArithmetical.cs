/*
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

public class MathArithmetical : Game
{
	public enum Operation
	{
		Addition = 0,	
		Substraction,	
		Multiplication,
		LastOperation
	}

	private int []operands;
	private Operation operation;
	private int max_operand;
	private int max_operations;

	public override string Name {
		get {return Catalog.GetString ("Arithmetical");}
	}

	public override Types Type {
		get { return Game.Types.MathTrainer;}
	}

	public override string Question {
		get {return Catalog.GetString ("What is the result of the arithmetical operation?");} 
	}

	public override void Initialize ()
	{
		int result = 0, operations = 0;
		operation = (Operation) random.Next ((int) Operation.LastOperation);

		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			max_operations = 2;
			max_operand = 50;
			break;
		case Difficulty.Medium:
			max_operations = 3;
			max_operand = 100;
			break;
		case Difficulty.Master:
			max_operations = 5;
			max_operand = 500;
			break;
		}

		switch (operation) {
		case Operation.Addition:
		case Operation.Substraction:
			operations = 2 + random.Next (max_operations);
				break;
		case Operation.Multiplication:
			operations = 2 + random.Next (1);
			break;
		}

		operands = new int [operations];

		result = operands[0] = 10 + random.Next (max_operand);
		for (int i = 1; i < operands.Length; i ++)
		{
			operands[i] = 10 + random.Next (max_operand);
			switch (operation) {
			case Operation.Addition:
				result += operands[i];
				break;	
			case Operation.Substraction:
				result -= operands[i];
				break;
			case Operation.Multiplication:
				result *= operands[i];
				break;
			}
		}
		right_answer = result.ToString ();
	}
	
	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{	
		double operand_y = DrawAreaY + 0.2, operand_space = 0.1;
		double aligned_pos = 0.58;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);
	
		SetLargeFont (gr);
		for (int i = 0; i < operands.Length - 1; i++)
		{
			gr.DrawTextAlignedRight (aligned_pos, operand_y, operands[i].ToString ());
			gr.MoveTo (DrawAreaX + 0.2, operand_y + 0.05);	

			switch (operation) {
			case Operation.Addition:
				gr.ShowText ("+");
				break;	
			case Operation.Substraction:
				gr.ShowText ("-");
				break;
			case Operation.Multiplication:
				gr.ShowText ("*");
				break;
			}

			operand_y += operand_space;
		}

		gr.DrawTextAlignedRight (aligned_pos, operand_y, operands[operands.Length - 1].ToString ());

		operand_y += 0.05;
		gr.MoveTo (DrawAreaX + 0.2, operand_y);
		gr.LineTo (DrawAreaX + 0.5, operand_y);
		gr.Stroke ();

		if (DrawAnswer) {
			operand_y += 0.05;
			gr.DrawTextAlignedRight (aligned_pos, operand_y + 0.05, right_answer);
			gr.Stroke ();
		}

	}

}


