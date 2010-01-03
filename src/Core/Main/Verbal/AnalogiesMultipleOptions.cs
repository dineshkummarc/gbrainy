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

using gbrainy.Core.Libraries;

namespace gbrainy.Core.Main.Verbal
{
	public class AnalogiesMultipleOptions : Analogies
	{
		static protected Dictionary <int, Analogy> analogies;
		static protected ArrayListIndicesRandom analogies_indices;
		static protected int analogies_index = 0;

		public AnalogiesMultipleOptions ()
		{
			if (analogies == null)
				analogies = AnalogiesFactory. Get (Analogy.Type.MultipleOptions);
		}

		public override string Name {
			get { return Catalog.GetString ("Multiple options");}
		}

		public override string Question {
			get {
				string str = string.Empty;

				if (current == null)
					return string.Empty;

				if (current.answers == null)
					return current.question;

				for (int n = 0; n < current.answers.Length; n++)
				{
					str+= GetPossibleAnswer (n);

					if (n +1 < current.answers.Length) {
						// Translators: this the separator used when concatenating possible options for answering verbal analogies
						// For example: "Possible correct answers are: a, b, c, d."						
						str += Catalog.GetString (", ");
					}
				}

				// Translators: {0} is replaced by a question and {1} by the suggestions on how to answer
				// E.g: What is the correct option? Answer A, B, C.
				return String.Format (Catalog.GetString ("{0} Answer {1}."),
					current.question,
					str);
			}
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
			current = GetNext ();

			if (current == null || current.answers == null)
				return;

			right_answer = GetPossibleAnswer (current.right);
		}
	
		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX, y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			if (current == null || current.answers == null)
				return;

			gr.SetPangoLargeFontSize ();
			gr.MoveTo (0.1, y);
			gr.ShowPangoText (Catalog.GetString ("Possible answers are:"));

			gr.SetPangoNormalFontSize ();

			y += 0.15;
			x += 0.05;
			for (int n = 0; n < current.answers.Length; n++)
			{
				gr.MoveTo (x, y);
				gr.ShowPangoText (String.Format ("{0}) {1}", GetPossibleAnswer (n), current.answers[n].ToString ()));
				gr.Stroke ();
				y += 0.15;
			}
		}
	}
}
