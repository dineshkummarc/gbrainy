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
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace gbrainy.Core.Main.Xml
{
	public class GamesXmlFactory
	{
		List <GameXmlDefinition> games;
		bool read = false;

		public GamesXmlFactory ()
		{
			games = new List <GameXmlDefinition> ();
		}

		public List <GameXmlDefinition> Definitions {
			get { return games; }
		}

		public void Read (string file)
		{
			GameXmlDefinition game;
			string name, str;
			bool processing_variant = false;
			int variant = 0;

			if (read == true)
				return;

			try
			{
				StreamReader myStream = new StreamReader (file);
				XmlTextReader reader = new XmlTextReader (myStream);
				game = null;

				while (reader.Read ())
				{
					name = reader.Name.ToLower ();
					switch (name) {
					case "games":
						break;
					case "type":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						game.Type = GameTypesDescription.FromString (reader.ReadElementString ());
						break;
					case "game":
						if (reader.NodeType == XmlNodeType.Element) {
							game = new GameXmlDefinition ();
						} else if (reader.NodeType == XmlNodeType.EndElement) {
							games.Add (game);
						}
						break;
					case "_name":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						game.Name = reader.ReadElementString ();
						break;
					case "difficulty":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						str = reader.ReadElementString ();
						str.Trim ();

						switch (str) {
						case "Easy":
							game.Difficulty = Game.Difficulty.Easy;
							break;
						case "Medium":
							game.Difficulty = Game.Difficulty.Medium;
							break;
						case "Master":
							game.Difficulty = Game.Difficulty.Master;
							break;
						case "All":
							game.Difficulty = Game.Difficulty.All;
							break;
						default:
							Console.WriteLine ("GameXmlFactory. Unknown difficulty level: {0}", str);
							break;
						}

						break;
					case "svg":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						ImageDrawingObject draw_image = new ImageDrawingObject ();

						if (processing_variant)
							game.Variants[variant].AddDrawingObject (draw_image);
						else
							game.AddDrawingObject (draw_image);

						draw_image.Filename = reader.GetAttribute ("file");

						str = reader.GetAttribute ("x");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.X = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.X = 0.1;

						str = reader.GetAttribute ("y");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.Y = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.Y = 0.1;

						str = reader.GetAttribute ("width");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.Width = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.Width = 0.8;

						str = reader.GetAttribute ("height");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.Height = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.Height = 0.8;

						break;
					case "string":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						TextDrawingObject draw_string = new TextDrawingObject ();

						if (processing_variant)
							game.Variants[variant].AddDrawingObject (draw_string);
						else
							game.AddDrawingObject (draw_string);

						draw_string.Text = reader.GetAttribute ("text");
	
						if (String.IsNullOrEmpty (draw_string.Text))
							draw_string.Text = reader.GetAttribute ("_text");

						str = reader.GetAttribute ("x");
						if (String.IsNullOrEmpty (str) == false)
							draw_string.X = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_string.X = 0.1;

						str = reader.GetAttribute ("y");
						if (String.IsNullOrEmpty (str) == false)
							draw_string.Y = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_string.Y = 0.1;

						str = reader.GetAttribute ("centered");
						if (String.Compare (str, "yes", true) == 0)
							draw_string.Centered = true;
						else
							draw_string.Centered = false;

						str = reader.GetAttribute ("size");
						if (String.Compare (str, "big", true) == 0)
							draw_string.Big = true;
						else
							draw_string.Big = false;

						break;
					case "_question":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].Question = reader.ReadElementString ();
						else
							game.Question = reader.ReadElementString ();

						break;
					case "_rationale":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].Rationale = reader.ReadElementString ();
						else
							game.Rationale = reader.ReadElementString ();

						break;
					case "answer":
					case "_answer":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].Answer = reader.ReadElementString ();
						else
							game.Answer = reader.ReadElementString ();

						break;
					case "_tip":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].Tip = reader.ReadElementString ();
						else
							game.Tip = reader.ReadElementString ();

						break;
					case "variant":
						if (reader.NodeType == XmlNodeType.Element) {
							game.NewVariant ();
							variant = game.Variants.Count - 1;
							processing_variant = true;
						} else if (reader.NodeType == XmlNodeType.EndElement) {
							processing_variant = false;
						}
						break;
					case "variables":
						if (processing_variant)
							game.Variants[variant].Variables = reader.ReadElementString ();
						else
							game.Variables = reader.ReadElementString ();
						break;
					default:
						if (String.IsNullOrEmpty (name) == false)
							Console.WriteLine ("GameXmlFactory. Unsupported tag: {0}", name);

						break;
					}
				}

				reader.Close ();
				read = true;

				GameXml.Definitions = games;
			}

			catch (Exception e)
			{
				read = true;
				Console.WriteLine ("GameXmlFactory. Error loading: {0}", e.Message);
			}
		}
	}
}
