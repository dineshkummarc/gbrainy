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

using Cairo;
using Mono.Unix;
using System;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Logic
{
	public class PuzzlePercentage : Game
	{
		enum GameType
		{
			Discount,
			Sales,
			Water,
			Total
		}

		string question, answer;
		GameType gametype;

		public override string Name {
			get {return Catalog.GetString ("Percentage");}
		}

		public override string Question {
			get {return question; }
		}

		public override string Answer {
			get {

				if (String.IsNullOrEmpty (answer) == true)
					return base.Answer;

				return base.Answer + " " + answer;
			}
		}

		public override void Initialize ()
		{
			int ans;

			gametype = (GameType) random.Next ((int) GameType.Total);

			switch (gametype)
			{
			case GameType.Discount:
				double price, discount, paid;

				do
				{
					discount = 10 + random.Next (30);
					price = 100 + random.Next (100);
					paid = price - (price * discount / 100);

				}  while (paid != Math.Truncate (paid));

				question = String.Format (
					Catalog.GetString ("After getting {0}% discount you have paid {1} monetary units for a TV set. What was the original price of the TV set?"),
					discount, paid);
				ans = (int)price;
				break;
			case GameType.Sales:
				double sales, increase, previous;

				do
				{
					previous = 10 + random.Next (90);
					increase = 10 + random.Next (20);
					sales = previous + (previous * increase / 100);

				}  while (sales != Math.Truncate (sales));

			
				question = String.Format (
					Catalog.GetString ("John's shop had sales of {0} monetary units. This was an increase of {1}% over last month. What were last month sales?"),
					sales, increase);
				ans = (int) previous;
				break;
			case GameType.Water:
				double decrease, percentage;

				do
				{
					decrease = (1 + random.Next (70));
					percentage = decrease / (100 - decrease) * 100;

				} while (percentage != Math.Truncate (percentage));
			
				question = String.Format (
					Catalog.GetString ("The amount of water in a bucket decreases by {0}%. By what percentage must the amount of water increase to reach its original value?"),
					decrease);

				answer = Catalog.GetString ("The objective is to obtain the same total amount.");
				ans = (int) percentage;
				break;
			default:
				throw new Exception ("Unexpected value");
			}

			right_answer = (ans).ToString ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);
		}

		public override bool CheckAnswer (string answer)
		{	
			if (gametype == GameType.Water) {
				if (String.Compare (answer, right_answer + "%", true) == 0) 
					return true;
			}

			if (String.Compare (answer, right_answer, true) == 0) 
				return true;

			return false;
		}
	}
}
