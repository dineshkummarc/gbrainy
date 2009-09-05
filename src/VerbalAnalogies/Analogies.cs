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
using System.Collections.Generic;

using Mono.Unix;

public abstract class Analogies : Game
{
	protected Analogy current;

	public override string Name {
		get { return Catalog.GetString ("Verbal analogies"); }
	}

	public override string Question {
		get {
			return current.question;
		}
	}

	public override string Tip {
		get {
			if (current == null)
				return null;
			else
				return current.tip;
		}
	}

	public override string Answer {
		get {
			if (current == null)
				return string.Empty;

			if (current.MultipleAnswers == true) 
			{
				string [] items;
				string str = string.Empty;
	
				items = right_answer.Split ('|');

				for (int i = 0 ; i < items.Length; i++)
				{
					str += items [i].Trim ();
					if (i + 1 < items.Length) {
						// Translators: this the separator used when concatenating multiple possible answers for verbal analogies
						// For example: "Possible correct answers are: sleep, rest."
						str += Catalog.GetString (", ");
					}

				}
				str = String.Format (Catalog.GetString ("Possible correct answers are: {0}."), str);
				return str;
			}

			if (String.IsNullOrEmpty (current.rationale) == false)
				return base.Answer + " " + current.rationale;

			return base.Answer;
		}
	}

	public override Types Type {
		get { return Game.Types.VerbalAnalogy;}
	}

	public abstract ArrayListIndicesRandom Indices {
		get;
		set;
	}

	public abstract int CurrentIndex {
		get;
		set;
	}

	public abstract Dictionary <int, Analogy> List {
		get;
	}

	public override void Initialize ()
	{
		current = GetNext ();

		if (current == null)
			return;
	}

	public Analogy GetNext ()
	{
		int idx;
		Analogy analogy;

		if (Indices == null || CurrentIndex + 1 >= List.Count) {
			Indices = new ArrayListIndicesRandom (List.Count);
			Indices.Initialize ();
		}
		else
			CurrentIndex++;

		idx = Indices [CurrentIndex];
		
		try
		{
			List.TryGetValue (idx, out analogy);
		}

		catch (KeyNotFoundException)
		{
			analogy = null;
		}

		if (analogy != null && analogy.answers != null) { // Randomize answers order

			ArrayListIndicesRandom indices;
			string [] answers;
			int new_right = 0;

			indices = new ArrayListIndicesRandom (analogy.answers.Length);
			answers = new string [analogy.answers.Length];

			indices.Initialize ();

			for (int i = 0; i < indices.Count; i++)
			{
				answers [i] = Catalog.GetString (analogy.answers [indices[i]]);
				if (indices[i] == analogy.right)
					new_right = i;
			}
			analogy.right = new_right;
			analogy.answers = answers;
		}

		analogy.question = Catalog.GetString (analogy.question);

		if (String.IsNullOrEmpty (analogy.tip) == false)
			analogy.tip = Catalog.GetString (analogy.tip);

		if (String.IsNullOrEmpty (analogy.rationale) == false)
			analogy.rationale = Catalog.GetString (analogy.rationale);

		return analogy;
	}

	public override bool CheckAnswer (string answer)
	{
		string [] items = right_answer.Split ('|');
		Console.WriteLine ("answer is null {0}", answer == null);

		foreach (string ans in items)
		{
			string str = ans.Trim ();

			if (String.Compare (str, answer, true) == 0)
				return true;
		}

		return base.CheckAnswer (answer);
	}

}
