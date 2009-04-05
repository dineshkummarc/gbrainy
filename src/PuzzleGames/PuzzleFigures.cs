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

public class PuzzleFigures : Game
{
	private int [] figures  = new int []
	{
		0, 0, 1, 1, 2, 2,
		1, 2, 2, 0, 1, 0,
		2, 1, 0, 2, 0, 1
	};

	private ArrayListIndicesRandom random_indices;
	private const double figure_width = 0.1, figure_height = 0.1, space_width = 0.05, space_height = 0;

	public override string Name {
		get {return Catalog.GetString ("Figures");}
	}

	public override string Question {
		get {return Catalog.GetString ("What is the next logical sequence of objects in the last column? See below the convention when giving the answer.");} 
	}


	public override string Answer {
		get { 
			string answer = base.Answer + " ";

			answer += Catalog.GetString ("It is the only combination that you can build with the given elements without repeating them.");

			return answer;
		}
	}

	public override void Initialize ()
	{
		random_indices = new ArrayListIndicesRandom (6);
		random_indices.Initialize ();

		StringBuilder sb = new StringBuilder (3);
	
		sb.Append (GetPossibleAnswer (figures[random_indices [5]]));
		sb.Append (GetPossibleAnswer (figures[6 + random_indices [5]]));
		sb.Append (GetPossibleAnswer (figures[(2 * 6) + random_indices [5]]));

		right_answer = sb.ToString ();
	}

	private void AnswerCoding (CairoContextEx gr, double x, double y)
	{
		double pos_x = x;

		gr.MoveTo (pos_x, y - 0.01);
		y += 0.05;
		gr.ShowPangoText (Catalog.GetString ("Convention when giving the answer is:"));

		gr.MoveTo (pos_x, y + 0.05);
		gr.ShowPangoText (String.Format (Catalog.GetString ("{0} ->"), GetPossibleAnswer (0)));
		gr.Stroke ();
		gr.DrawDiamond (x + 0.1, y, 0.1);
		gr.Stroke ();
	
		pos_x += 0.3;
		gr.MoveTo (pos_x, y + 0.05);
		gr.ShowPangoText (String.Format (Catalog.GetString ("{0} ->"), GetPossibleAnswer (1)));
		gr.Stroke ();
		pos_x += 0.1;
		gr.Arc (pos_x + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);	
		gr.Stroke ();

		pos_x += 0.2;
		gr.MoveTo (pos_x, y + 0.05);
		gr.ShowPangoText (String.Format (Catalog.GetString ("{0} ->"), GetPossibleAnswer (2)));
		gr.Stroke ();
		pos_x += 0.1;
		gr.DrawEquilateralTriangle (pos_x, y, 0.1);
		gr.Stroke ();

		y += 0.16;
		gr.MoveTo (x, y);		
		gr.ShowPangoText (String.Format (Catalog.GetString ("E.g: {0}{1}{2} (diamond, triangle, circle)"),
			GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2)));

	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{		
		int element;
		const double figure_width = 0.1, figure_height = 0.1, space_width = 0.05, space_height = 0.1;
		double x = DrawAreaX, y = DrawAreaY;

		base.Draw (gr, area_width, area_height);

		for (int i = 0; i < (DrawAnswer ? 6 : 5) ; i++)
		{
			element = random_indices [i];
			y = DrawAreaY;
			for (int n = 0; n < 3; n++) 
			{
				switch ((int) figures[(n * 6) + element])
				{
					case 0:
						gr.DrawDiamond (x, y, 0.1);
						break;
					case 1:
						gr.Arc (x + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);	
						break;
					case 2:
						gr.DrawEquilateralTriangle (x, y, 0.1);
						break;
					default:
						break;
				}
				gr.Stroke ();
				y+= figure_height + space_height;		
			}
			x+= figure_width + space_width;
		}

		if (DrawAnswer == false) {
			y = DrawAreaY;
			gr.Save ();
			gr.SetPangoFontSize (0.1);
			for (int n = 0; n < 3; n++) {
				gr.MoveTo (x, y - 0.02);
				gr.ShowPangoText ("?");
				gr.Stroke ();
				y+= figure_height + space_height;
			}
			gr.SetPangoNormalFontSize ();
			gr.Restore ();	
		}

		AnswerCoding (gr, DrawAreaX, y);
	}

	public override bool CheckAnswer (string answer)
	{
		answer = TrimAnswer (answer);
		return base.CheckAnswer (answer);
	}
}


