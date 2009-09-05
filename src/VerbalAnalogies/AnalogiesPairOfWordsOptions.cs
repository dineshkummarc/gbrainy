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

using System;
using System.IO;
using System.Collections.Generic;

using Cairo;
using Mono.Unix;
using Gtk;

public class AnalogiesPairOfWordsOptions : Analogies
{
	static protected Dictionary <int, Analogy> analogies;
	static protected ArrayListIndicesRandom analogies_indices;
	static protected int analogies_index = 0;

	public AnalogiesPairOfWordsOptions ()
	{

	}

	public override string Name {
		get { return "AnalogiesPairOfWordsOptions";}
	}

	public override ArrayListIndicesRandom Indices {
		get { return analogies_indices; }
		set { analogies_indices = value; }
	}

	public override int CurrentIndex {
		get { return analogies_index; }
		set { analogies_index = value; }
	}

	public override Dictionary <int, Analogy> List {
		get { return analogies; }
	}

	public override string Question {
		get {
			string str = string.Empty;
			for (int n = 0; n < current.answers.Length; n++)
			{
				str+= GetPossibleAnswer (n);

				if (n +1 < current.answers.Length) {
					// Translators: this the separator used when concatenating possible options for answering verbal analogies
					// For example: "Possible correct answers are: a, b, c, d."						
					str += Catalog.GetString (", ");
				}
			}

			return String.Format (Catalog.GetString (
				"Given the pair of words '{0}', which of the possible answers has the closest in relationship to the given pair? Answer {1}."),
				current.question,
				str);
		}
	}

	public override void Initialize ()
	{
		if (analogies == null) {
			analogies = AnalogiesFactory. Get (Analogy.Type.PairOfWordsOptions);
		}

		current = GetNext ();

		if (current == null)
			return;

		right_answer = GetPossibleAnswer (current.right);
		Console.WriteLine ("Name:" + Name + " " + current.ToString ());
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX, y = DrawAreaY + 0.1;

		base.Draw (gr, area_width, area_height);

		if (current == null || current.answers == null || current.answers.Length <= 1)
			return;

		gr.SetPangoLargeFontSize ();
		gr.MoveTo (0.1, y);
		gr.ShowPangoText (Catalog.GetString ("Possible answers are:"));
		y += 0.12;
		x += 0.1;
		for (int n = 0; n < current.answers.Length; n++)
		{
			gr.MoveTo (x, y);
			gr.ShowPangoText (String.Format ("{0}) {1}", GetPossibleAnswer (n), current.answers[n].ToString ()));
			gr.Stroke ();
			y += 0.15;
		}
	}
}
