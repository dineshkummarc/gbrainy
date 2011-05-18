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
using System.Text.RegularExpressions;

using gbrainy.Core.Main.Xml;

public class GameXmlToAssembly
{
	const int VarIdxDefinitions = 0;
	const int VarIdxAssigments = 1;

	// TODO: Can be this an extension method?
	// Not derived directly from StringBuilder to add as needed the methods required
	class StringBuilderIndentation
	{
		StringBuilder builder;

		public int Level { get; set; }

		public StringBuilderIndentation ()
		{
			builder = new StringBuilder ();
		}

		void InsertLevel ()
		{
			for (int i = 0; i < Level; i++)
				builder.Append ('\t');
		}

		public void Clear ()
		{
			builder = new StringBuilder ();
		}

		public void NoLevel (char val)
		{
			builder.Append (val);
		}


		public void Append (char val)
		{
			InsertLevel ();
			builder.Append (val);
		}

		public void Append (string val)
		{
			InsertLevel ();
			builder.Append (val);
		}

		public void AppendLine (string val)
		{
			InsertLevel ();
			builder.AppendLine (val);
		}

		public override string ToString ()
		{
			return builder.ToString ();
		}
	}

	// Reads a games.xml, uses GameTemplate.cs file, process both, and generates a GamesXml.cs with all the games
	static void Main ()
	{
		Dictionary <string, string> tokens = new Dictionary <string, string> ();
		GamesXmlFactory factory;

		TextWriter tw = new StreamWriter ("GamesXml.cs");
		factory = new GamesXmlFactory ();
		factory.Read ("../data/games.xml");
		Console.WriteLine ("Games read {0}", factory.Definitions.Count);

		tw.WriteLine ("using System;");
		tw.WriteLine ("using gbrainy.Core.Main;");
		tw.WriteLine ("using gbrainy.Core.Libraries;");
		tw.WriteLine ("using Mono.Unix;");
		tw.WriteLine ("using System.Collections.Generic;");
		tw.WriteLine ("using gbrainy.Core.Main.Xml;");
		tw.WriteLine ("");
		tw.WriteLine ("namespace gbrainy.Games");
		tw.WriteLine ("{");

		foreach (GameXmlDefinition definition in factory.Definitions)
		{
			tokens.Clear ();

			// Class definition
			tokens.Add ("@CLASSNAME@", RemoveSpaces (definition.Name));
			tokens.Add ("@NAME@", definition.Name);
			//tokens.Add ("@QUESTION@", GetStringFromDefinition (definition.Question));
			tokens.Add ("@TIP@", definition.Tip);
			//tokens.Add ("@RATIONALE@", GetStringFromDefinition (definition.Rationale));

			tokens.Add ("@VARIANTS_DEFINITION@", GetVariantsDefinitions (definition));
			//tokens.Add ("@VARIANTS_VARIABLES@", GetVariantsVariables (definition));

			string[] vars = GetVariantsVariables (definition);
			tokens.Add ("@VARIABLES_DEFINITION@", vars [VarIdxDefinitions]);
			tokens.Add ("@VARIABLES_ASSIGMENT@", vars [VarIdxAssigments]);

			string line;
			Stream read = File.OpenRead ("GameTemplate.cs");
			StreamReader sr = new StreamReader (read);

			bool write_line;
			while (true)
			{
				write_line = true;
				line = sr.ReadLine ();
				if (line == null)
					break;

				foreach (string token in tokens.Keys)
				{
					if (line.IndexOf (token) == -1)
						continue;

					line = line.Replace (token, tokens[token]);

					if (String.IsNullOrEmpty (line) == true)
						write_line = false;
				}

				if (write_line)
					tw.WriteLine (line);
			}
			read.Close ();
		}

		tw.WriteLine (BuildTable (factory));

		tw.WriteLine ("}");
		tw.Close ();
	}

	// string [0]
	static string[] GetVariantsVariables (GameXmlDefinition definition)
	{
		const string exp = "([^=]+) (=) ([^!]+)";
		StringBuilderIndentation builder_definitions = new StringBuilderIndentation ();
		StringBuilderIndentation builder_assigments = new StringBuilderIndentation ();
		string [] rslt = new string [2];
		GameXmlDefinitionVariant variant;

		builder_assigments.Level = 4;
		for (int i = 0; i < definition.Variants.Count; i++)
		{
			Regex regex;
			int prev = 0, next, pos;
			string line, vars;
			Match match;

			variant = definition.Variants [i];

			if (String.IsNullOrEmpty (variant.Variables))
				continue;

			// Header
			builder_definitions.Level = 2;
			builder_definitions.AppendLine (String.Format ("class VariantVariables{0}", i));
			builder_definitions.AppendLine ("{");

			builder_definitions.Level = 3;

			regex = new Regex (exp, RegexOptions.IgnoreCase);
			vars = variant.Variables;

			builder_definitions.AppendLine ("Random random = new Random ();");
			while (true)
			{
				pos = next = vars.IndexOf (';', prev);

				if (pos == -1)
					line = vars.Substring (prev, vars.Length - prev);
				else
					line = vars.Substring (prev, next + 1 - prev);

				line = line.Trim ();

				// Process line
				match = regex.Match (line);

				bool first_nonspace = false;
				StringBuilder var_name = new StringBuilder ();
				string var_def = match.Groups [1].ToString ();
				Console.WriteLine ("var_Def {0}", var_def);

				for (int n = var_def.Length - 1; n >= 0; n--)
				{
					if (var_def [n] == ' ' && first_nonspace == true)
						break;

					if (var_def [n] != ' ')
						first_nonspace = true;

					var_name.Insert (0, var_def [n]);
				}

				if (String.IsNullOrEmpty (line) == false)
				{
					builder_definitions.AppendLine (String.Format ("public {0};", match.Groups [1]));
					builder_assigments.AppendLine (String.Format ("{0} = {1}", var_name.ToString (), match.Groups [3]));
				}
				if (pos == -1)
					break;

				prev = next + 1;
			}

			// Footer
			builder_definitions.AppendLine (" ");
			builder_definitions.AppendLine (String.Format ("VariantVariables{0} ()", i));
			builder_definitions.AppendLine ("{");

			builder_definitions.Level = 0;
			builder_definitions.AppendLine (builder_assigments.ToString ());

			builder_definitions.Level = 3;
			builder_definitions.AppendLine ("}");
			builder_definitions.Level = 2;
			builder_definitions.AppendLine ("}");

			builder_assigments.Clear ();
		}

		rslt [VarIdxDefinitions] = builder_definitions.ToString ();
		rslt [VarIdxAssigments] = builder_assigments.ToString ();

		return rslt;
	}

	static string GetVariantsDefinitions (GameXmlDefinition definition)
	{
		StringBuilder builder = new StringBuilder ();

		foreach (GameXmlDefinitionVariant variant in definition.Variants)
		{
			builder.AppendLine ("\tvariant = new GameXmlDefinitionVariant ();");

			if (String.IsNullOrEmpty (variant.Tip) == false)
				builder.AppendLine (String.Format ("\tvariant.Tip = Catalog.GetString (\"{0}\");", variant.Tip));

			builder.AppendLine ("\tvariants.Add (variant);");
		}

		return builder.ToString ();
	}

	static string BuildTable (GamesXmlFactory factory)
	{
		StringBuilder builder = new StringBuilder ();

		builder.AppendLine ("\tpublic class GameList");
		builder.AppendLine ("\t{");
		builder.AppendLine ("\t\tstatic Type[] LogicPuzzlesInternal = new Type []");
		builder.AppendLine ("\t\t{");

		foreach (GameXmlDefinition definition in factory.Definitions)
		{
			builder.AppendLine (String.Format ("\t\t\ttypeof ({0}),", RemoveSpaces (definition.Name)));
		}

		builder.AppendLine ("\t\t};");

		builder.AppendLine ("\t\tpublic static Type [] LogicPuzzles");
		builder.AppendLine ("\t\t{");
		builder.AppendLine ("\t\t\tget {");
		builder.AppendLine ("\t\t\t\treturn LogicPuzzlesInternal;");
		builder.AppendLine ("\t\t\t}");
		builder.AppendLine ("\t\t}");

		builder.AppendLine ("\t}");

		return builder.ToString ();
	}

	static string RemoveSpaces (string str)
	{
		StringBuilder builder = new StringBuilder ();

		for (int i = 0; i < str.Length; i++)
		{
			if (char.IsWhiteSpace (str[i]))
				continue;

			builder.Append (str[i]);
		}

		return builder.ToString ();
	}

	static string GetStringFromDefinition (LocalizableString localizable)
	{
		if (localizable.IsPlural () == false)
			return "";

		return String.Format ("\t\tCatalog.GetPluralString (\"{0}\",\n\t\t\t\"{1}\",\n\t\t\t{2});\n",
			localizable.String,
			localizable.PluralString,
			"0");
	}
}
