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

using Mono.Unix;

namespace gbrainy.Core.Main
{
	static public class GamesXmlFactory
	{
		static List <GameXmlDefinition> games;
		static bool read = false;

		static GamesXmlFactory ()
		{
			games = new List <GameXmlDefinition> ();
		}

		static public List <GameXmlDefinition> Definitions {
			get {
				Read ();
				return games;
			}
		}

		static public void Read ()
		{
			Read (Path.Combine (Defines.DATA_DIR, "games.xml"));
		}

		static void Read (string file)
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
					case "_difficulty":
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

						game.Image.Filename = reader.GetAttribute ("file");

						str = reader.GetAttribute ("x");
						if (String.IsNullOrEmpty (str) == false)
							game.Image.X = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							game.Image.X = 0.1;

						str = reader.GetAttribute ("y");
						if (String.IsNullOrEmpty (str) == false)
							game.Image.Y = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							game.Image.Y = 0.1;

						str = reader.GetAttribute ("width");
						if (String.IsNullOrEmpty (str) == false)
							game.Image.Width = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							game.Image.Width = 0.8;

						str = reader.GetAttribute ("height");
						if (String.IsNullOrEmpty (str) == false)
							game.Image.Height = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							game.Image.Height = 0.8;

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
					default:
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
