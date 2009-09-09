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

public class AnalogiesQuestionAnswer : Analogies
{
	static protected Dictionary <int, Analogy> analogies;
	static protected ArrayListIndicesRandom analogies_indices;
	static protected int analogies_index = 0;

	public AnalogiesQuestionAnswer ()
	{

	}

	public override string Name {
		get { return "AnalogiesQuestionAnswer";}
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

	public override void Initialize ()
	{
		if (analogies == null) {
			analogies = AnalogiesFactory.Get (Analogy.Type.QuestionAnswer);
		}

		current = GetNext ();

		if (current == null)
			return;

		if (current.answers != null) 
			right_answer = current.answers [current.right];

		Console.WriteLine ("Name:" + Name + " " + current.ToString ());
	}
}
