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
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Mono.CSharp;
using System.Text.RegularExpressions;

using Mono.Unix;

namespace gbrainy.Core.Main
{
	static public class GamesXmlFactory
	{
		static List <GameXmlDefinition> games;
		static bool read = false;
		static bool? monofix_needed;

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
					case "games":
						break;
					case "type":
						break;
					case "game":
						if (reader.NodeType == XmlNodeType.Element) {
							game = new GameXmlDefinition ();
						} else if (reader.NodeType == XmlNodeType.EndElement) {

							for (int i = 0; i < game.Variants.Count; i++)
							{
								game.Variants[i].Question = ReplaceVariables (game.Variants[i].Question);
								game.Variants[i].Answer = ReplaceVariables (game.Variants[i].Answer);
							}

							game.Question = ReplaceVariables (game.Question);
							game.Answer = ReplaceVariables (game.Answer);
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
					case "variables":
					{
						string eval;

						// Using's for the variables section
						// We need to evaluate either declarations (like using) or expression/statements separately
						eval = "using System;\n";
						Mono.CSharp.Evaluator.Run (eval);

						// Infrastructure for the user available
						eval = "Random random = new Random ();\n";
						Mono.CSharp.Evaluator.Run (eval);

						eval = reader.ReadElementString ();
						Mono.CSharp.Evaluator.Run (eval);
						break;
					}
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

		// Before Mono 2.6 (rev. 156533) there is no line separator between vars
		static string FixGetVars (string str)
		{
			if (monofix_needed == null)
			{
				string eval, vars;

				eval = "int a = 1; int b = 1;";
				Evaluator.Run (eval);
				vars = Evaluator.GetVars ();

				monofix_needed = vars.IndexOf (System.Environment.NewLine) == -1;
			}

			if (monofix_needed == false)
				return str;

			// We just guarantee that int, doubles, and float are separated as modern Mono versions do
			StringBuilder output = new StringBuilder ();
			string [] keywords = new string [] {"int", "double", "float"};
			int pos = 0, cur = 0;

			while (pos != -1)
			{
				for (int i = 0; i < keywords.Length; i++)
				{
					pos = str.IndexOf (keywords [i], cur);
					if (pos != -1)
					{
						output.Append (str.Substring (cur, pos - cur));
						output.AppendLine ();
						output.Append (str.Substring (pos, keywords[i].Length));
						cur = pos + keywords[i].Length;
						break;
					}
				}
			}

			output.Append (str.Substring (cur, str.Length - cur));
			return output.ToString ();
		}

		static string GetVarValue (string vars, string _var)
		{
			const string exp = "([a-z0-9._%+-]+) ([a-z0-9._%+-]+) (=) ([0-9]+)";
			Match match;
			int idx, cur, newline_len;
			string line;

			Regex regex = new Regex (exp, RegexOptions.IgnoreCase);

			newline_len = System.Environment.NewLine.Length;
			cur = 0;

			do
			{
				// Process a line
				idx = vars.IndexOf (System.Environment.NewLine, cur);
				if (idx == -1) idx = vars.Length;

				line = vars.Substring (cur, idx - cur);
				cur = idx + newline_len;
				match = regex.Match (line);

				//  "int num = 2";
				//   group 1 -> int,  group 2 -> num,  group 3 -> =, group 4 -> 2
				if (match.Groups.Count == 5)
				{
					if (match.Groups[2].Value == _var)
						return match.Groups[4].Value;
				}

			} while (cur < vars.Length);

			return string.Empty;
		}

		static string ReplaceVariables (string str)
		{
			const string exp = "\\[[a-z]+\\]+";
			string eval, var, vars, var_value;
			Regex regex;
			Match match;

			if (String.IsNullOrEmpty (str))
				return str;

			regex = new Regex (exp, RegexOptions.IgnoreCase);
			match = regex.Match (str);

			while (String.IsNullOrEmpty (match.Value) == false)
			{
				var = match.Value.Substring (1, match.Value.Length - 2);
				vars = Evaluator.GetVars ();
				vars = FixGetVars (vars);

				var_value = GetVarValue (vars, var);

				if (String.IsNullOrEmpty (var_value) == false)
					str = str.Replace (match.Value, var_value);

				match = match.NextMatch ();
			}
			return str;
		}
	}
}
