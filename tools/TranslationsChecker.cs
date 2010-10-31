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
using System.Text;
using System.IO;
using System.Collections.Generic;
using MonoDevelop.Gettext;
using System.Text.RegularExpressions;

public class GameXmlToGetString
{
	public class Parser : CatalogParser
	{
		public Parser (string fileName, Encoding encoding) : base (fileName, encoding)
		{

		}

		List <string> GetExpressionVariables (string str)
		{
			Regex regex;
			Match match;
			List <string> strs = new List <string> ();
			string expression = "(\\[[a-z0-9._%+-]*\\])+";  // alike [age]

			regex = new Regex (expression, RegexOptions.IgnoreCase);
			match = regex.Match (str);

			if (String.IsNullOrEmpty (match.Value) == false)
			{
				while (String.IsNullOrEmpty (match.Value) == false)
				{
					strs.Add (match.Value);
					match = match.NextMatch ();
				}
			}
			return strs;
		}

		List <string> GetStringFormaters (string str)
		{
			Regex regex;
			Match match;
			List <string> strs = new List <string> ();
			string expression = "(\\{[a-z0-9.:_%+-]*\\})+";  // alike [age]

			regex = new Regex (expression, RegexOptions.IgnoreCase);
			match = regex.Match (str);

			if (String.IsNullOrEmpty (match.Value) == false)
			{
				while (String.IsNullOrEmpty (match.Value) == false)
				{
					strs.Add (match.Value);
					match = match.NextMatch ();
				}
			}
			return strs;
		}


		int Count (List <string> strings, string str)
		{
			int cnt = 0;

			foreach (string s in strings)
			{
				if (s == str)
					cnt++;
			}
			return cnt;

		}

		protected override bool OnEntry (string msgid, string msgidPlural, bool hasPlural,
		                         string[] translations, string flags,
		                         string[] references, string comment,
		                         string[] autocomments)
		{

			if (String.IsNullOrEmpty (translations [0]))
				return true;

			// This string uses [] but not variables expressions
			if (msgid == "Usage: gbrainy [options]")
				return true;

			if (flags.IndexOf ("fuzzy") != -1)
				return true;

			// Check Expression variables (like [age])
			List <string> source = GetExpressionVariables (msgid);

			for (int i = 0; i < translations.Length; i++)
			{
				List <string> target = GetExpressionVariables (translations [i]);

				foreach (string s in source)
				{
					if (Count (source, s) != Count (target, s))
					{
						Console.WriteLine ("Gbrainy expression variable error. In '{0}' string '{1}' count does not match", msgid, s);
					}
				}
			}

			// Check Formatters (like {0})
			source = GetStringFormaters (msgid);

			for (int i = 0; i < translations.Length; i++)
			{
				List <string> target = GetStringFormaters (translations [i]);

				foreach (string s in source)
				{
					if (Count (source, s) != Count (target, s))
					{
						Console.WriteLine ("String Formatter error. In '{0}' string '{1}' count does not match", msgid, s);
					}
				}
			}

			return true;
		}
	}

	/*
		This tool scans the LINGUAS files and searches for potential
		mismatching string formatters and expression variables that can
		cause problems at runtime.
	*/
	static void Main (string[] args)
	{
		string line, file;
		Stream read = File.OpenRead ("../po/LINGUAS");
		StreamReader sr = new StreamReader (read);

		while (true) {
			line = sr.ReadLine ();
			if (line == null)
				break;

			if (line.StartsWith ("#") == true)
				continue;

			file = "../po/" + line + ".po";

			Console.WriteLine ("Openning {0}", file);
			Parser parser = new Parser (file, Encoding.UTF8);
			parser.Parse ();
		}
	}
}

