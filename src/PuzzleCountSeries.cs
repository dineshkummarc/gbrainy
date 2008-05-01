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
using Mono.Unix;

public class PuzzleCountSeries : Game
{
	enum GameType
	{
		HowManyNines,
		HowManySmallerDigits,
		HowManyBiggerDigits,
		Length		
	}

	private Game game;
	private string question;
	private string answer;

	public override string Name {
		get {return Catalog.GetString ("Count series");}
	}

	public override string Question {
		get {return question;} 
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			return answer;
		}
	}

	public override void Initialize ()
	{
		switch ((GameType) random.Next ((int) GameType.Length))
		{
			case GameType.HowManyNines:
				question = "How many 9 digits are needed to represent the the numbers between 10 to 100?";
				right_answer = "20";
				break;

			case GameType.HowManyBiggerDigits:
				question = "How many numbers of two digits have the first digit bigger than the second (e.g.: 20 and 21)?";
				right_answer = "45";
				break;

			case GameType.HowManySmallerDigits:
				question = "How many numbers of two digits have the first digit smaller than the second (e.g.: 12 and 13)?";
				right_answer = "36";
				break;
		}
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		gr.Scale (area_width, area_height);
		DrawBackground (gr);
	}
}


