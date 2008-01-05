/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
 * Copyright (C) 2007 Javier Mª Mora Merchán <jamarier@gmail.com>
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

public class PuzzleMatrixNumbers : Game
{
	public enum Operation
	{
		MultiplyAndAdd = 0,	// Multiplies two elements and adds a third
		MutilplyAndSubs,	// Multiplies two elements and substracts a third
		AddAndSubs,		// Adds two elements and  substracts a third 
		LastOperation
	}

	private int [] numbers;
	private Operation operation;
	private bool orientation;
	private const int rows = 4, columns = 4;

	public override string Name {
		get {return Catalog.GetString ("Matrix Numbers");}
	}

	public override string Question {
		get {return Catalog.GetString ("The numbers in the matrix follow a logic. Which is the number that should replace the question mark?");}
	}

	public override string Tip {
		get { 
			if (orientation) 
				return Catalog.GetString ("The logic is arithmetical and works vertically.");
			else 
				return Catalog.GetString ("The logic is arithmetical and works horizontally.");
		}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";

			switch (operation) {
			case Operation.MultiplyAndAdd:
				if (orientation) {
					answer += String.Format(Catalog.GetString("The fourth row is calculated multiplying the first two rows and adding the third."));
				} else {
					answer += String.Format(Catalog.GetString("The fourth column is calculated multiplying the first two columns and adding the third."));
				}
				break;
			case Operation.MutilplyAndSubs:
				if (orientation) {
					answer += String.Format(Catalog.GetString("The fourth row is calculated multiplying the first two rows and subtracting the third."));
				} else {
					answer += String.Format(Catalog.GetString("The fourth column is calculated multiplying the first two columns and subtracting the third."));
				}
				break;
			case Operation.AddAndSubs:
				if (orientation) {
					answer += String.Format(Catalog.GetString("The fourth row is calculated adding the first two rows and subtracting the third."));
				} else {
					answer += String.Format(Catalog.GetString("The fourth column is calculated adding the first two columns and subtracting the third."));
				}
				break;
			}
			return answer;
		}
	}

	public override void Initialize ()
	{
		operation = (Operation) random.Next ((int) Operation.LastOperation);
		orientation = (random.Next ((int) 2) == 0) ? true : false;
		numbers = new int [4 * 4];

		int coordinateA, coordinateB;

		if (orientation) {
			coordinateA=4; 
			coordinateB=1;
		}
		else {
			coordinateA=1;
			coordinateB=4;
		}

		
		for (int n = 0; n < 3; n++)
			for (int i = 0; i < 4; i++) 
				numbers[n*coordinateA + i*coordinateB] = random.Next (10) + random.Next (5);

		for (int i = 0; i < 4; i++) {
			switch (operation) {
			case Operation.MultiplyAndAdd:
				numbers[3*coordinateA + i*coordinateB] = (numbers [0*coordinateA + i*coordinateB ] * numbers[1*coordinateA + i*coordinateB]) + numbers[2*coordinateA + i*coordinateB];
				break;
			case Operation.MutilplyAndSubs:
				numbers[3*coordinateA + i*coordinateB] = (numbers [0*coordinateA + i*coordinateB ] * numbers[1*coordinateA + i*coordinateB]) - numbers[2*coordinateA + i*coordinateB];
				break;
			case Operation.AddAndSubs:
				numbers[3*coordinateA + i*coordinateB] = (numbers [0*coordinateA + i*coordinateB ] + numbers[1*coordinateA + i*coordinateB]) - numbers[2*coordinateA + i*coordinateB];
				break;
			default:
				break;
			}			
		}

		right_answer = numbers[3*coordinateA + 3*coordinateB].ToString ();
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{
		double rect_w = DrawAreaWidth / rows;
		double rect_h = DrawAreaHeight / columns;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);

		for (int column = 0; column < columns; column++) {
			for (int row = 0; row < rows; row++) {
				gr.Rectangle (DrawAreaX + row * rect_w, DrawAreaY + column * rect_h, rect_w, rect_h);

				if (row != 3  || column != 3) {
					gr.MoveTo (0.04 + DrawAreaX + column * rect_w, (rect_h / 2) + DrawAreaY + row * rect_h);
					gr.ShowText ( (numbers[column + (row * 4)]).ToString() );
				}
			}
		}

		gr.MoveTo (0.04 + DrawAreaX + 3 * rect_w, (rect_h / 2) + DrawAreaY + 3 * rect_h);
		gr.ShowText ("?");
		gr.Stroke ();
	}

}
