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

using Cairo;
using Mono.Unix;
using System;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Logic
{
	public class PuzzleClocks : Game
	{
		private const double figure_size = 0.3;
		private const double radian = Math.PI / 180;
		private int addition;
		private int []handles;
		private const int clocks = 4;
		private const int handle_num = 2;

		public override string Name {
			get {return Catalog.GetString ("Clocks");}
		}

		public override string Question {
			get {return (String.Format (
				Catalog.GetString ("To what number should the large handle of the '{0}' clock point? Answer using numbers."),
				GetPossibleFigureAnswer (3)));}
		}

		public override string Answer {
			get { 
				string answer = base.Answer + " ";
				answer += String.Format (Catalog.GetString ("Starting from the first clock sum {0} to the value indicated by the hands."), addition);
				return answer;
			}
		}

		public override string Tip {
			get { return Catalog.GetString ("The clocks do not follow the time logic.");}
		}

		public override void Initialize ()
		{
			int position;

			// In all these cases the clock logic clearly do not work since the small hand is in the next hour
			switch (random.Next (3)) {
			case 0:
				position = 48;
				addition = 5;
				break;
			case 1:
				position = 38;
				addition = 15;
				break;
			case 2:
			default:
				position = 24;
				addition = 5;
				break;
			}

			handles = new int [clocks * handle_num];

			for (int i = 0; i < handles.Length; i++, i++)
			{
				handles [i] = position / 10;
				handles [i + 1] = position - ((position / 10) * 10);
				position += addition;
			}
		
			right_answer = handles[7].ToString ();

			/*DateTime dt1 = new DateTime (2008, 2, 20, handles[0], handles[1] * 5, 0);
			DateTime dt2 = new DateTime (2008, 2, 20, handles[2], handles[3] * 5, 0);
			Console.WriteLine ("t1 {0}", dt1);
			Console.WriteLine ("t2 {0}", dt2);
			Console.WriteLine ("Time diff {0} from 1st to 2nd", dt2-dt1);

			dt1 = new DateTime (2008, 2, 20, handles[2], handles[3] * 5, 0);
			dt2 = new DateTime (2008, 2, 20, handles[4], handles[5] * 5, 0);
			Console.WriteLine ("t1 {0}", dt1);
			Console.WriteLine ("t2 {0}", dt2);
			Console.WriteLine ("Time diff {0} from 1st to 2nd", dt2-dt1);

			dt1 = new DateTime (2008, 2, 20, handles[4], handles[5] * 5, 0);
			dt2 = new DateTime (2008, 2, 20, handles[6], handles[7] * 5, 0);
			Console.WriteLine ("t1 {0}", dt1);
			Console.WriteLine ("t2 {0}", dt2);
			Console.WriteLine ("Time diff {0} from 1st to 2nd", dt2-dt1);*/

		}	

		static void DrawClock (CairoContextEx gr, double x, double y, int hand_short, int hand_large, bool draw_large)
		{
			const double radius = figure_size / 2;
			double x0, y0;
			int num, degrees;

			gr.Arc (x, y, radius, 0, 2 * Math.PI);
			gr.Stroke ();
			for (degrees = 0; degrees < 360; degrees+= 30) {
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				 // Small lines
				gr.MoveTo (x + 0.9 * x0, y + 0.9 * y0);
				gr.LineTo (x + x0, y + y0);
				gr.Stroke ();
				// Numbers
				num = (degrees / 30) + 3;
				if (num > 12) num = num - 12;

				gr.DrawTextCentered (x + x0 * 0.75,  y + y0 * 0.75, num.ToString ());
				gr.Stroke ();
			}

			if (draw_large) {
				// Hand Large
				degrees = (hand_large - 3) * 30; 
				x0 = radius * Math.Cos (degrees * radian);
				y0 = radius * Math.Sin (degrees * radian);
				gr.MoveTo (x, y);
				gr.LineTo (x + x0 * 0.55, y + y0 * 0.55);
				gr.Stroke ();
			}
			// Hand Short
			degrees = (hand_short - 3) * 30; 
			x0 = radius * Math.Cos (degrees * radian);
			y0 = radius * Math.Sin (degrees * radian);
			gr.MoveTo (x, y);
			gr.LineTo (x + x0 * 0.4, y + y0 * 0.4);
			gr.Stroke ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX + 0.1, y = DrawAreaY + 0.05;

			base.Draw (gr, area_width, area_height, rtl);

			DrawClock (gr, x + 0.1, y + 0.1, handles[0], handles[1], true);
			gr.MoveTo (x + 0.03, y + 0.29);
			gr.ShowPangoText (GetPossibleFigureAnswer (0));
			gr.Stroke ();
	
			DrawClock (gr, x + 0.5, y + 0.1, handles[2], handles[3], true);
			gr.MoveTo (x + 0.43, y + 0.29);
			gr.ShowPangoText (GetPossibleFigureAnswer (1));
			gr.Stroke ();

			DrawClock (gr, x + 0.1, y + 0.52, handles[4], handles[5], true);
			gr.MoveTo (x + 0.03, y + 0.71);
			gr.ShowPangoText (GetPossibleFigureAnswer (2));
			gr.Stroke ();

			DrawClock (gr, x + 0.5, y + 0.52, handles[6], handles[7], DrawAnswer == true);
			gr.MoveTo (x + 0.43, y + 0.71);
			gr.ShowPangoText (GetPossibleFigureAnswer (3));
			gr.Stroke ();

		}
	}
}
