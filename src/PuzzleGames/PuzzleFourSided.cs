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

public class PuzzleFourSided : Game
{
	int type;
	public override string Name {
		get {return Catalog.GetString ("Four sided");}
	}

	public override string Question {
		get {return Catalog.GetString ("How many four sided figures do you count in the figure below?");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("A four sided figure can be embedded inside another figure.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";

			answer += String.Format (Catalog.GetString ("The four sided figures are made by connecting the following points: {0}"),
				(type == 0) ? "abde, degh, bcef, efhi, acdf, dfgi, abhg, bcih, acig, aghe, aefc, deig, bcie." : 
				"abde, degh, bcef, efhi, acdf, dfgi, abhg, bcih, acig, aghe, aefc, deig, bcie, acde, cehi, abeg, egif.");

			return answer;
		}
	}

	public override void Initialize ()
	{
		if (CurrentDifficulty==Difficulty.Easy)
			type = 0;
		else
			type = random.Next (2);

		
		if (type == 0)	
			right_answer = "13";
		else
			right_answer = "17";
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.1, y = DrawAreaY + 0.1, w = 0.6, h = 0.6;

		base.Draw (gr, area_width, area_height);
		
		gr.Rectangle (x, y, w, h);
		gr.Stroke ();

		// Lines
		gr.MoveTo (x + w /2, y);
		gr.LineTo (x + w /2, y + h);
		gr.Stroke ();
		gr.MoveTo (x, y + h /2);
		gr.LineTo (x + w, y + h / 2);
		gr.Stroke ();

		// Diagonals
		gr.MoveTo (x, y);
		gr.LineTo (x + w, y + h);
		gr.Stroke ();

		if (type == 1) {
			gr.MoveTo (x + w, y);
			gr.LineTo (x, y + h);
			gr.Stroke ();
		}

		if (DrawAnswer == false)
			return;

		// References
		gr.MoveTo (x - 0.04, y - 0.02);
		gr.ShowText ("a");
		gr.Stroke ();

		gr.MoveTo (x + w / 2 - 0.02, y - 0.02);
		gr.ShowText ("b");
		gr.Stroke ();

		gr.MoveTo (x + w + 0.02, y - 0.02);
		gr.ShowText ("c");
		gr.Stroke ();

		gr.MoveTo (x - 0.04, y + h /2 - 0.02);
		gr.ShowText ("d");
		gr.Stroke ();

		gr.MoveTo (x + w / 2 - 0.04, y  + h /2 - 0.04);
		gr.ShowText ("e");
		gr.Stroke ();

		gr.MoveTo (x + w + 0.02, y  + h /2 - 0.02);
		gr.ShowText ("f");
		gr.Stroke ();

		gr.MoveTo (x - 0.04, y + h + 0.04);
		gr.ShowText ("g");
		gr.Stroke ();

		gr.MoveTo (x + w / 2 - 0.02, y + h + 0.04);
		gr.ShowText ("h");
		gr.Stroke ();

		gr.MoveTo (x + w + 0.02, y + h + 0.04);
		gr.ShowText ("i");
		gr.Stroke ();

	}

}


