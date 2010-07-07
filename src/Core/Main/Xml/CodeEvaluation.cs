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
using System.Text;
using System.Text.RegularExpressions;

using Mono.CSharp;
using Mono.Unix;

namespace gbrainy.Core.Main.Xml
{
	// Code evaluation functions
	public static class CodeEvaluation
	{
		static bool? monofix_needed;

		static public void EvaluateVariables (string code)
		{
			string eval;

			try
			{
				// Using's for the variables section
				// We need to evaluate either declarations (like using) or expression/statements separately
				eval = "using System;\n";
				Mono.CSharp.Evaluator.Run (eval);

				// Infrastructure for the user available
				eval = "Random random = new Random ();\n";
				Mono.CSharp.Evaluator.Run (eval);
				Mono.CSharp.Evaluator.Run (code);
			}

			catch (Exception e)
			{
				Console.WriteLine ("GameXml. Error in games.xml: {0} when evaluating variable definition [{1}]", e.Message, code);
			}
		}

		static public string ReplaceVariables (string str)
		{
			const string exp = "\\[[a-z_]+\\]+";
			string var, vars, var_value;
			Regex regex;
			Match match;

			if (String.IsNullOrEmpty (str))
				return str;

			regex = new Regex (exp, RegexOptions.IgnoreCase);
			match = regex.Match (str);

			vars = Evaluator.GetVars ();
			vars = FixGetVars (vars);

			while (String.IsNullOrEmpty (match.Value) == false)
			{
				var = match.Value.Substring (1, match.Value.Length - 2);
				var_value = GetVarValue (vars, var);

				if (String.IsNullOrEmpty (var_value) == false)
					str = str.Replace (match.Value, var_value);

				match = match.NextMatch ();
			}
			return str;
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
			int pos = 0, cur = 0, tmp_pos, keyword;

			while (pos != -1)
			{
				pos = keyword = -1;
				// Look for the nearest of these keywords
				for (int i = 0; i < keywords.Length; i++)
				{
					tmp_pos = str.IndexOf (keywords [i], cur);
					if (tmp_pos == -1)
						continue;

					if (pos == -1 || pos > 0 && tmp_pos < pos) {
						keyword = i;
						pos = tmp_pos;
					}
				}

				if (pos == -1)
					continue;

				output.Append (str.Substring (cur, pos - cur));
				output.AppendLine ();
				output.Append (str.Substring (pos, keywords[keyword].Length));
				cur = pos + keywords[keyword].Length;
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
	}
}
