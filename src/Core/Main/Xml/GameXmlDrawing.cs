/*
 * Copyright (C) 2010-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Xml
{
	internal class GameXmlDrawing
	{
		GameXml game_xml;
		List <OptionDrawingObject> options;

		static string option_prefix = "[option_prefix]";

		public GameXmlDrawing (GameXml game_xml)
		{
			this.game_xml = game_xml;
		}

		internal List <OptionDrawingObject> Options {
			get { return options; }
			set { options = value; }
		}

		public int GetOptionCorrectAnswerIndex ()
		{
			if (options == null || options.Count < 0)
				return -1;

			for (int i = 0; i < options.Count; i++)
			{
				OptionDrawingObject option = options [i];
				if (option.Correct == true)
				{
					return i;
				}
			}
			throw new InvalidOperationException ("GameXmlDrawing.GetOptionCorrectAnswerIndex. Invalid value.");
		}

		public void CreateDrawingObjects (DrawingObject [] drawing_objects)
		{
			OptionDrawingObject option;
			double x = 0, y = 0, width = 0, height = 0;
			bool first = true;
			int randomized_options = 0;

			if (drawing_objects == null)
				return;

			// Calculate the size of container from the options and count the number of random options
			foreach (DrawingObject draw_object in drawing_objects)
			{
				option = draw_object as OptionDrawingObject;

				if (option == null)
					continue;

				if (option.RandomizedOrder)
					randomized_options++;

				if (first == true)
				{
					x = option.X;
					y = option.Y;
					width = option.Width;
					height = option.Height;
					first = false;
					continue;
				}

				if (option.X < x)
					x = option.X;

				if (option.Y < y)
					y = option.Y;

				if (option.X + option.Width > width)
					width = option.X + option.Width;

				if (option.Y + option.Height > height)
					height = option.Y + option.Height;
			}

			if (first == true)
				return;

			RandomizeOptions (drawing_objects, randomized_options);

			Container container = new Container (x, y, width - x, height - y);
			game_xml.AddWidget (container);

			if (Options == null)
				Options = new List <OptionDrawingObject> ();

			// Create drawing widgets objects
			int idx = 0;
			foreach (DrawingObject draw_object in drawing_objects)
			{
				option = draw_object as OptionDrawingObject;

				if (option == null)
					continue;

				DrawableArea drawable_area = new DrawableArea (option.Width, option.Height);
				drawable_area.X = option.X;
				drawable_area.Y = option.Y; // + i * 0.15;
				container.AddChild (drawable_area);

				drawable_area.Data = idx;
				drawable_area.DataEx = game_xml.Answer.GetMultiOption (idx);
				Options.Add (option);

				idx++;
				drawable_area.DrawEventHandler += DrawOption;
			}
		}

		void RandomizeOptions (DrawingObject [] drawing_objects, int randomized_options)
		{
			OptionDrawingObject option;

			// Randomize the order of the options
			if (randomized_options > 0)
			{
				OptionDrawingObject [] originals;
				ArrayListIndicesRandom random_indices;
				int index = 0;

				random_indices = new ArrayListIndicesRandom (randomized_options);
				originals = new OptionDrawingObject [randomized_options];
				random_indices.Initialize ();

				// Backup originals
				for (int i = 0; i < drawing_objects.Length; i++)
				{
					option = drawing_objects[i] as OptionDrawingObject;

					if (option == null)
						continue;

					originals[index] = option.Copy ();
					index++;
				}

				// Swap
				index = 0;
				for (int i = 0; i < drawing_objects.Length; i++)
				{
					option = drawing_objects[i] as OptionDrawingObject;

					if (option == null)
						continue;

					option.CopyRandomizedProperties (originals [random_indices [index]]);
					index++;
				}
			}
		}

		public void DrawObjects (CairoContextEx gr, DrawingObject [] drawing_objects, int? option)
		{
			if (drawing_objects == null)
				return;

			foreach (DrawingObject draw_object in drawing_objects)
			{
				if (draw_object is OptionDrawingObject)
					continue;

				if (draw_object is TextDrawingObject)
				{
					string text;
					TextDrawingObject draw_string = draw_object as TextDrawingObject;

					text = game_xml.CatalogGetString (draw_string.Text);
					text = game_xml.ReplaceVariables (text);

					switch (draw_string.Size) {
					case TextDrawingObject.Sizes.Small:
						gr.SetPangoFontSize (0.018);
						break;
					case TextDrawingObject.Sizes.Medium:
						gr.SetPangoNormalFontSize (); // 0.022
						break;
					case TextDrawingObject.Sizes.Large:
						gr.SetPangoLargeFontSize (); // 0.0325
						break;
					case TextDrawingObject.Sizes.XLarge:
						gr.SetPangoFontSize (0.06);
						break;
					case TextDrawingObject.Sizes.XXLarge:
						gr.SetPangoFontSize (0.08);
						break;
					default:
						throw new InvalidOperationException ("Invalid value");
					}

					if (draw_string.Centered) {
						gr.DrawTextCentered (draw_string.X, draw_string.Y, text);
					} else {
						gr.MoveTo (draw_string.X, draw_string.Y);
						if (option == null)
							gr.ShowPangoText (text);
						else
							gr.ShowPangoText (GetOptionPrefix (text, (int) option));

						gr.Stroke ();
					}
				}
				else if (draw_object is ImageDrawingObject)
				{
					ImageDrawingObject image = draw_object as ImageDrawingObject;

					if (String.IsNullOrEmpty (image.Filename) == false)
					{
						string dir;
						IConfiguration config;

						config = ServiceLocator.Instance.GetService <IConfiguration> ();
						dir = config.Get <string> (ConfigurationKeys.GamesGraphics);

						gr.DrawImageFromFile (Path.Combine (dir, image.Filename),
							image.X, image.Y, image.Width, image.Height);
					}
				}
			}
		}

		internal void DrawOption (object sender, DrawEventArgs e)
		{
			int idx = (int) e.Data;

			if (Options.Count == 0)
				return;

			OptionDrawingObject _option = Options [idx];
			e.Context.SetPangoLargeFontSize ();

			DrawObjects (e.Context, _option.DrawingObjects, idx);
		}

		string GetOptionPrefix (string str, int option)
		{
			string answer;

			// Translators: This the option to select in a multioption answer. For example "A) Mother"
			answer = String.Format (game_xml.CatalogGetString ("{0})"), game_xml.Answer.GetMultiOption (option));
			return str.Replace (option_prefix, answer);
		}
	}
}
