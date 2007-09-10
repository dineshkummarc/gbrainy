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

using Cairo;
using Mono.Unix;
using System;

public class PuzzleFigureLetter : Game
{
	private QuestionType question;
	
	enum QuestionType
	{
		TwoRectangles	= 0,
		TwoCercles,
		ThreeCercles,
		Length		
	}

	public override string Name {
		get {return Catalog.GetString ("Words and figures");}
	}

	public override string Question {
		get {return Catalog.GetString ("The figures and the text are related. Which text should go under the last figure?");} 
	}

	public override string Tip {
		get { return Catalog.GetString ("Every character of the word represents a property of the figure.");}
	}

	public override string Answer {
		get { 
			string answer = base.Answer + " ";
			answer += Catalog.GetString ("'A' indicates that the figures overlap, 'B' that are rectangles, 'C' that are circles, 'D' that the figures are separated, 'E' that there are three figures and 'F' that there are two figures.");
			return answer;
		}
	}

	public override void Initialize ()
	{
		question = (QuestionType) random.Next ((int) QuestionType.Length);

		switch (question) {
		case QuestionType.TwoRectangles:
			right_answer = "ABF";
			break;
		case QuestionType.TwoCercles:
			right_answer = "CDF";
			break;
		case QuestionType.ThreeCercles:
			right_answer = "ACE";
			break;
		}
	}

	public override bool CheckAnswer (string answer)
	{	

		switch (question) {
		case QuestionType.TwoRectangles:		
			if ((String.Compare (answer, "ABF", true) == 0) 
				|| (String.Compare (answer, "AFB", true) == 0)
				|| (String.Compare (answer, "BAF", true) == 0)
				|| (String.Compare (answer, "BFA", true) == 0)
				|| (String.Compare (answer, "FBA", true) == 0)
				|| (String.Compare (answer, "FAB", true) == 0)) {
				return true;
			}
			break;
		case QuestionType.TwoCercles:		
			if ((String.Compare (answer, "CDF", true) == 0)
				|| (String.Compare (answer, "CFD", true) == 0)
				|| (String.Compare (answer, "DCF", true) == 0)
				|| (String.Compare (answer, "DFC", true) == 0)
				|| (String.Compare (answer, "FCD", true) == 0)
				|| (String.Compare (answer, "FDC", true) == 0)) {
				return true;
			}
			break;
		case QuestionType.ThreeCercles:		
			if ((String.Compare (answer, "ACE", true) == 0)
				|| (String.Compare (answer, "AEC", true) == 0)
				|| (String.Compare (answer, "CAE", true) == 0)
				|| (String.Compare (answer, "CEA", true) == 0)
				|| (String.Compare (answer, "EAC", true) == 0)
				|| (String.Compare (answer, "ECA", true) == 0)) {
				return true;
			}
			break;
		}
				
		return false;
	}

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{
		double x = DrawAreaX + 0.05;
		double y = DrawAreaY + 0.1;

		gr.Scale (area_width, area_height);

		DrawBackground (gr);
		PrepareGC (gr);

		// Three circles
		gr.Arc (x + 0.06, y, 0.05, 0, 2 * Math.PI);
		gr.Stroke ();
		gr.Arc (x, y + 0.1, 0.05, 0, 2 * Math.PI);
		gr.Stroke ();
		gr.Arc (x + 0.11, y + 0.1, 0.05, 0, 2 * Math.PI);
		gr.Stroke ();
		gr.MoveTo (x + 0.02, y + 0.2);
		gr.ShowText ("DCE");
		gr.Stroke ();

		// Two linked circles
		gr.Arc (x + 0.3, y, 0.05, 0, 2 * Math.PI);
		gr.Stroke ();
		gr.Arc (x + 0.3 + 0.06, y + 0.05, 0.05, 0, 2 * Math.PI);
		gr.Stroke ();
		gr.MoveTo (x + 0.30, y + 0.2);
		gr.ShowText ("ACF");
		gr.Stroke ();

		// Two rectangles
		gr.Rectangle (x + 0.5, y, 0.1, 0.1);
		gr.Rectangle (x + 0.62, y, 0.1, 0.1);
		gr.Stroke ();
		gr.MoveTo (x + 0.58, y + 0.2);
		gr.ShowText ("BDF");
		gr.Stroke ();

	
		// Tree rectangles
		gr.Rectangle (x - 0.05, y + 0.35, 0.1, 0.1);
		gr.Rectangle (x + 0.06 - 0.05, y + 0.37, 0.1, 0.1);
		gr.Rectangle (x + 0.12 - 0.05, y + 0.39, 0.1, 0.1);
		gr.Stroke ();
		gr.MoveTo (x + 0.04, y + 0.55);
		gr.ShowText ("ABE");
		gr.Stroke ();

		x += 0.25;
		y += 0.35;

		switch (question) {
		case QuestionType.TwoRectangles:
			gr.Rectangle (x, y, 0.1, 0.1);
			gr.Rectangle (x + 0.05, y + 0.03, 0.1, 0.1);
			gr.Stroke ();
			gr.MoveTo (x + 0.05, y + 0.2);
			break;
		case QuestionType.TwoCercles:
			gr.Arc (x + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.Arc (x + 0.12 + 0.05, y + 0.05, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.MoveTo (x + 0.1, y + 0.2);
			break;
		case QuestionType.ThreeCercles:
			gr.Arc (x + 0.05 + 0.06, y + 0.04, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.Arc (x + 0.05, y + 0.06 + 0.04, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.Arc (x + 0.05  + 0.11, y + 0.06 + 0.04, 0.05, 0, 2 * Math.PI);
			gr.Stroke ();
			gr.MoveTo (x + 0.1, y + 0.2);
			break;
		}

		gr.ShowText ("?");
		gr.Stroke ();		
	}

}


