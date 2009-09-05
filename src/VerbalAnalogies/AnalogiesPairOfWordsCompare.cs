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

public class AnalogiesPairOfWordsCompare : Analogies
{
	static protected Dictionary <int, Analogy> analogies;
	static protected ArrayListIndicesRandom analogies_indices;
	static protected int analogies_index = 0;

	string samples, sample, question;

	public AnalogiesPairOfWordsCompare ()
	{

	}

	public override string Name {
		get { return "AnalogiesPairOfWordsCompare";}
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
			return String.Format (Catalog.GetString (
				"Given the pair of words below, which word has the closest relationship to '{0}'?"),
				sample);
		}
	}

	public override void Initialize ()
	{
		if (analogies == null) {
			analogies = AnalogiesFactory. Get (Analogy.Type.PairOfWordsCompare);
		}

		current = GetNext ();

		if (current == null)
			return;

		string [] items;

		items = current.question.Split ('|');

		if (items.Length == 2)
			sample = items [1].Trim ();
		else
			sample = string.Empty;

		samples = items [0].Trim ();

		right_answer = current.answers [current.right];
		Console.WriteLine ("Name:" + Name + " " + current.ToString ());
	}
	
	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX, y = DrawAreaY + 0.1;

		base.Draw (gr, area_width, area_height);

		if (current == null || current.answers == null)
			return;

		gr.SetPangoLargeFontSize ();
		gr.MoveTo (0.2, y + 0.25);
		gr.ShowPangoText (String.Format (Catalog.GetString ("Pair of words: {0}"), samples));
	}
}
