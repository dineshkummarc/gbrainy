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
using System.Collections.Generic;

using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Verbal
{
	public class AnalogiesPairOfWordsOptions : Analogies
	{
		static protected Dictionary <int, Analogy> analogies;
		string samples, sample;

		public AnalogiesPairOfWordsOptions ()
		{
			if (analogies == null)
				analogies = AnalogiesFactory. Get (Analogy.Type.PairOfWordsOptions);
		}

		public override string Name {
			get { return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Pair of words #{0}"), Variant);}
		}

		public override Dictionary <int, Analogy> List {
			get { return analogies; }
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
					str+= GameAnswer.GetMultiOption (n);

					if (n +1 < current.answers.Length) {
						// Translators: this the separator used when concatenating possible options for answering verbal analogies
						// For example: "Possible correct answers are: a, b, c, d."						
						str += ServiceLocator.Instance.GetService <ITranslations> ().GetString (", ");
					}
				}

				return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString (
					"Given the relationship between the two words below, which word has the same relationship to '{0}'?"),
					sample);
			}
		}

		protected override void Initialize ()
		{
			current = GetNext ();

			if (current == null || current.answers == null)
				return;

			string [] items;

			items = current.question.Split (AnalogiesFactory.Separator);

			if (items.Length == 2)
				sample = items [1].Trim ();
			else
				sample = string.Empty;

			samples = items [0].Trim ();

			Answer.Correct = GameAnswer.GetMultiOption (current.right);

			Container container = new Container (DrawAreaX + 0.1, 0.50, 0.5, current.answers.Length * 0.15);
			AddWidget (container);
	
			for (int i = 0; i <  current.answers.Length; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.8, 0.1);
				drawable_area.X = DrawAreaX;
				drawable_area.Y = DrawAreaY + 0.25 + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = GameAnswer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					//e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.05, 0.02);
					e.Context.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}) {1}"), GameAnswer.GetMultiOption (n), current.answers[n].ToString ()));
				};
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double y = DrawAreaY;

			if (current == null || current.answers == null || current.answers.Length <= 1)
				return;

			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();

			gr.MoveTo (0.1, y + 0.12);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Possible answers are:"));
			gr.Stroke ();

			gr.DrawTextCentered (0.5, y,
				String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Words: {0}"), samples));

		}
	}
}
