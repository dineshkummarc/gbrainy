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
using gbrainy.Core.Libraries;

using Mono.Unix;

namespace gbrainy.Core.Main.Verbal
{
	public abstract class Analogies : Game
	{
		protected Analogy current;

		public override string Name {
			get { return Catalog.GetString ("Verbal analogies"); }
		}

		public override string Question {
			get {
				if (current == null)
					return string.Empty;

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

		public override bool IsPlayable {
			get { return List.Count > 0;}
		}

		public override string Answer {
			get {
				string str;
				if (current == null || current.MultipleAnswers == false)
					return base.Answer;

				str = String.Format (Catalog.GetString ("Possible correct answers are: {0}."), AnswerValue);

				if (String.IsNullOrEmpty (Rationale))
					return str;

				return str += " " + Rationale;
			}
		}

		public override string Rationale {
			get {
				if (current == null)
					return string.Empty;
				
				return current.rationale;
			}
		}

		public override string AnswerValue {
			get { 
				if (current == null || current.MultipleAnswers == false)
					return right_answer;

				string [] items;
				string str = string.Empty;

				items = right_answer.Split (AnalogiesFactory.Separator);

				for (int i = 0 ; i < items.Length; i++)
				{
					str += items [i].Trim ();
					if (i + 1 < items.Length) {
						// Translators: this the separator used when concatenating multiple possible answers for verbal analogies
						// For example: "Possible correct answers are: sleep, rest."
						str += Catalog.GetString (", ");
					}
				}
				return str;
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

		// Returns true when this game manager has no more games to server
		public bool IsExhausted {
			get { return CurrentIndex + 1 >= List.Count;}
		}

		public abstract Dictionary <int, Analogy> List {
			get;
		}

		public Analogy GetNext ()
		{
			int idx;
			Analogy analogy; // Holds a deep copy
			Analogy analogy_ref; // Holds reference to the object
			ArrayListIndicesRandom indices = null;
			int new_right = 0;
			bool localized = true;

			if (List.Count == 0)
				return null;

			if (Indices == null || CurrentIndex + 1 >= List.Count) {
				Indices = new ArrayListIndicesRandom (List.Count);
				Indices.Initialize ();
			}
			else
				CurrentIndex++;

			idx = Indices [CurrentIndex];
		
			try
			{
				List.TryGetValue (idx, out analogy_ref);
			}

			catch (KeyNotFoundException)
			{
				return null;
			}

			analogy = analogy_ref.Copy ();

			if (analogy.answers != null) { // Randomize answers order
				string [] answers;
			
				indices = new ArrayListIndicesRandom (analogy.answers.Length);
				answers = new string [analogy.answers.Length];

				indices.Initialize ();

				for (int i = 0; i < indices.Count; i++)
				{
					if (GetText.StringExists (analogy.answers [indices[i]]) == false)
						localized = false;

					answers [i] = Catalog.GetString (analogy.answers [indices[i]]);
					if (indices[i] == analogy.right)
						new_right = i;
				}
				analogy.right = new_right;
				analogy.answers = answers;
			}

			if ((GetText.StringExists (analogy.question) == false) ||
				(String.IsNullOrEmpty (analogy.tip) == false && GetText.StringExists (analogy.tip) == false) ||
				(String.IsNullOrEmpty (analogy.rationale) == false && GetText.StringExists (analogy.rationale) == false)) 
				localized = false;

			if (localized == true) {
				analogy.question = Catalog.GetString (analogy.question);

				if (String.IsNullOrEmpty (analogy.tip) == false)
					analogy.tip = Catalog.GetString (analogy.tip);

				if (String.IsNullOrEmpty (analogy.rationale) == false)
					analogy.rationale = Catalog.GetString (analogy.rationale);
			} else {

				// Get analogy again
				List.TryGetValue (idx, out analogy_ref);
				analogy = analogy_ref.Copy ();

				if (analogy.answers != null) { // Randomize answers order
					string [] answers;

					answers = new string [analogy.answers.Length];

					for (int i = 0; i < indices.Count; i++)
						answers [i] = analogy.answers [indices[i]];

					analogy.right = new_right;
					analogy.answers = answers;
				}
			}

			return analogy;
		}
	}
}
