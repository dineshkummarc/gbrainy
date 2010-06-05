/*
 * Copyright (C) 2009-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using gbrainy.Core.Toolkit;

namespace gbrainy.Core.Main.Verbal
{
	public class AnalogiesMultipleOptions : Analogies
	{
		static protected Dictionary <int, Analogy> analogies;

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

		public override Dictionary <int, Analogy> List {
			get { return analogies; }
		}

		public override void Initialize ()
		{
			current = GetNext ();

			if (current == null || current.answers == null)
				return;

			right_answer = GetPossibleAnswer (current.right);

			Container container = new Container (DrawAreaX + 0.1, 0.50, 0.5, current.answers.Length * 0.15);
			AddWidget (container);
	
			for (int i = 0; i <  current.answers.Length; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.8, 0.1);
				drawable_area.X = DrawAreaX;
				drawable_area.Y = DrawAreaY + 0.2 + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = GetPossibleAnswer (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					e.Context.MoveTo (0.05, 0.02);
					e.Context.ShowPangoText (String.Format (Catalog.GetString ("{0}) {1}"), GetPossibleAnswer (n), current.answers[n].ToString ()));
				};
			}
		}
	
		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			if (current == null || current.answers == null)
				return;

			gr.SetPangoLargeFontSize ();
			gr.MoveTo (0.1, DrawAreaY + 0.05);
			gr.ShowPangoText (Catalog.GetString ("Possible answers are:"));
		}
	}
}
