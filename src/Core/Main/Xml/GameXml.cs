/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.IO;

using Mono.Unix;

namespace gbrainy.Core.Main.Xml
{
	public class GameXml : Game
	{
		// Every GameXml instance is capable of locating any XML defined game
		// This struct translates from a Variant that is global to all games
		// to a specific game + variant
		public struct DefinitionLocator
		{
			public int Game { get; set; }
			public int Variant { get; set; }

			public DefinitionLocator (int game, int variant) : this ()
			{
				Game = game;
				Variant = variant;
			}
		};

		// Shared with all instances
		static List <GameXmlDefinition> games;
		static List <DefinitionLocator> locators;

		DefinitionLocator current;
		GameXmlDefinition game;
		string question, answer, rationale, answer_value;

		public override GameAnswerCheckAttributes CheckAttributes  {
			get {
				GameAnswerCheckAttributes attrib;

				if (game.Variants.Count > 0 && game.Variants[current.Variant].CheckAttributes != GameAnswerCheckAttributes.None)
					attrib = game.Variants[current.Variant].CheckAttributes;
				else
					attrib =  game.CheckAttributes;

				if (attrib == GameAnswerCheckAttributes.None)
					return base.CheckAttributes;

				return attrib;
			}
		}

		public override string AnswerCheckExpression {
			get {
				string expression;

				if (game.Variants.Count > 0 && String.IsNullOrEmpty (game.Variants[current.Variant].AnswerCheckExpression) == false)
					expression = game.Variants[current.Variant].AnswerCheckExpression;
				else
					expression =  game.AnswerCheckExpression;

				if (String.IsNullOrEmpty (expression))
					return base.AnswerCheckExpression;

				return expression;
			}
		}

		public override string AnswerValue {
			get {
				if (String.IsNullOrEmpty (answer_value))
					return base.AnswerValue;

				return answer_value;
			}
		}

		static public List <GameXmlDefinition> Definitions {
			set {
				games = value;
				locators = new List <DefinitionLocator> ();
			}
		}

		public override GameTypes Type {
			get { return game.Type;}
		}

		public override string Name {
			get { return Catalog.GetString (game.Name); }
		}

		public override string Question {
			get { return question; }
		}

		public override string Rationale {
			get { return rationale; }
		}

		public override string Tip {
			get {
				if (game.Variants.Count > 0 && game.Variants[current.Variant].Tip != null)
					return Catalog.GetString (game.Variants[current.Variant].Tip);
				else
					if (String.IsNullOrEmpty (game.Tip) == false)
						return Catalog.GetString (game.Tip);
					else
						return null;
			}
		}

		protected override void Initialize ()
		{
			string variables;
			bool variants;
			LocalizableString localizable_question, localizable_rationale;

			variants = game.Variants.Count > 0;

			if (variants && game.Variants[current.Variant].Variables != null)
				variables = game.Variants[current.Variant].Variables;
			else
				variables = game.Variables;

			if (variants && game.Variants[current.Variant].Question != null)
				localizable_question = game.Variants[current.Variant].Question;
			else
				localizable_question = game.Question;

			if (variants && game.Variants[current.Variant].Rationale != null)
				localizable_rationale = game.Variants[current.Variant].Rationale;
			else
				localizable_rationale = game.Rationale;

			if (String.IsNullOrEmpty (variables) == false)
			{
				// Evaluate code
				CodeEvaluation.EvaluateVariables (variables);

				try {

					if (String.IsNullOrEmpty (localizable_question.Value) == false)
						localizable_question.ValueComputed = Int32.Parse (CodeEvaluation.ReplaceVariables (localizable_question.Value));

					if (localizable_rationale != null && String.IsNullOrEmpty (localizable_rationale.Value) == false)
						localizable_rationale.ValueComputed = Int32.Parse (CodeEvaluation.ReplaceVariables (localizable_rationale.Value));
				}
				catch (Exception e)
				{
					Console.WriteLine ("GameXml.Initialize {0}", e);
				}
			}

			if (variants && game.Variants[current.Variant].Question != null)
				question = CatalogGetString (game.Variants[current.Variant].Question);
			else
				question = CatalogGetString (game.Question);

			if (variants && game.Variants[current.Variant].Answer != null)
				answer = CatalogGetString (game.Variants[current.Variant].Answer);
			else
				answer = CatalogGetString (game.Answer);

			if (variants && game.Variants[current.Variant].Rationale != null)
				rationale = CatalogGetString (game.Variants[current.Variant].Rationale);
			else
				rationale = CatalogGetString (game.Rationale);

			if (variants && game.Variants[current.Variant].AnswerShow != null)
				answer_value = game.Variants[current.Variant].AnswerShow;
			else
				answer_value = game.AnswerShow;

			if (String.IsNullOrEmpty (variables) == false)
			{
				question = CodeEvaluation.ReplaceVariables (question);
				rationale = CodeEvaluation.ReplaceVariables (rationale);
				answer = CodeEvaluation.ReplaceVariables (answer);
				answer_value = CodeEvaluation.ReplaceVariables (answer_value);
			}

			right_answer = answer;
		}

		public override int Variants {
			get {
				if (locators.Count == 0)
					BuildLocationList ();

				return locators.Count;
			}
		}

		public override int Variant {
			set {
				base.Variant = value;

				DefinitionLocator locator;

				locator = locators [Variant];
				current.Game = locator.Game;
				current.Variant = locator.Variant;
				game = games [locator.Game];
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			DrawObjects (gr, game); // Draw objects shared by all variants

			if (game.Variants.Count > 0)
				DrawObjects (gr, game.Variants[current.Variant]); // Draw variant specific objects
		}

		void DrawObjects (CairoContextEx gr, GameXmlDefinitionVariant definition)
		{
			if (definition.DrawingObjects != null)
			{
				foreach (DrawingObject draw_object in definition.DrawingObjects)
				{
					if (draw_object is TextDrawingObject)
					{
						string text;
						TextDrawingObject draw_string = draw_object as TextDrawingObject;

						text = CatalogGetString (draw_string.Text);
						text = CodeEvaluation.ReplaceVariables (text);

						if (draw_string.Big)
							gr.SetPangoLargeFontSize ();
						else
							gr.SetPangoNormalFontSize ();

						if (draw_string.Centered) {
							gr.DrawTextCentered (draw_string.X, draw_string.Y, text);
						} else {
							gr.MoveTo (draw_string.X, draw_string.Y);
							gr.ShowPangoText (text);
							gr.Stroke ();
						}
					}
					else if (draw_object is ImageDrawingObject)
					{
						ImageDrawingObject image = draw_object as ImageDrawingObject;

						if (String.IsNullOrEmpty (image.Filename) == false)
						{
							gr.DrawImageFromFile (Path.Combine (Defines.DATA_DIR, image.Filename),
								image.X, image.Y, image.Width, image.Height);
						}
					}
				}
			}
		}

		void BuildLocationList ()
		{
			locators.Clear ();

			for (int game = 0; game < games.Count; game++)
			{
				locators.Add (new DefinitionLocator (game, 0));
				for (int variant = 0; variant < games[game].Variants.Count; variant++)
					locators.Add (new DefinitionLocator (game, variant));
			}
		}

		// Protect from calling with null (exception)
		string CatalogGetString (string str)
		{
			if (String.IsNullOrEmpty (str))
				return str;

			return Catalog.GetString (str);
		}

		// Protect from calling with null + resolve plurals
		string CatalogGetString (LocalizableString localizable)
		{
			if (localizable == null)
				return string.Empty;

			if (localizable.IsPlural () == false)
				return CatalogGetString (localizable.String);

			return Catalog.GetPluralString (localizable.String, localizable.PluralString, localizable.ValueComputed);
		}
	}
}
