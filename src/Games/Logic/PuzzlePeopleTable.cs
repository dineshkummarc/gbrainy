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

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzlePeopleTable : Game
	{
		private const double figure_size = 0.15;
		private string ques1, ques2;
	
		private class Circle
		{	
			public double x;
			public double y;

			public Circle (double x, double y) 
			{
				this.x = x;
				this.y = y;
			}
		}

		public override string Name {
			get {return Translations.GetString ("People at a table");}
		}

		public override string Question {
			get {return String.Format (Translations.GetString ("A group of people are sitting at a round table spaced out evenly. How many people are there if the {0} person is across from the {1}?"), ques1, ques2);} 
		}

		public override string Rationale {
			get {
				return Translations.GetString ("Subtracting the two positions you find out how many people are seated half way around the table. Doubling this number leaves you with the total amount of people.");
			}
		}

		protected override void Initialize ()
		{
			switch (random.Next (3)) {
			case 0:
				ques1 = Translations.GetString ("5th");
				ques2 = Translations.GetString ("19th");
				Answer.Correct = "28";
				break;
			case 1:
				ques1 = Translations.GetString ("4th");
				ques2 = Translations.GetString ("12th");
				Answer.Correct = "16";
				break;
			case 2:
				ques1 = Translations.GetString ("9th");
				ques2 = Translations.GetString ("22nd");
				Answer.Correct = "26";
				break;
			}			
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);
			gr.DrawImageFromAssembly ("people_table.svg", 0.2, 0.2, 0.6, 0.6);

			gr.DrawTextCentered (0.5, 0.85,
				Translations.GetString ("Two people in the table sitting across each other"));

		}
	}
}
