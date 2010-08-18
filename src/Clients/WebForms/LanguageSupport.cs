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
using Mono.Unix;

namespace WebForms
{
	static public class LanguageSupport
	{
		public class Language
		{
			public string Name { get; set; }
			public string LangCode { get; set; }

			public Language (string name, string code)
			{
				Name = name;
				LangCode = code;
			}
		};

		static Language [] languages =
		{
			new Language ("English", "en_US.utf8"),
			new Language ("Catalan", "ca_ES.utf8"),
			new Language ("Spanish", "es_ES.utf8"),
			new Language ("German", "de_DE.utf8")
		};

		static public Language [] Languages
		{
			get { return languages;}

		}

		static public Language GetFromIndex (int i)
		{
			return languages [i];
		}

		static public String GetString (WebSession session, string str)
		{
			// GetText
			string s = null;

			Environment.SetEnvironmentVariable ("LANGUAGE",
				LanguageSupport.GetFromIndex (session.LanguageIndex).LangCode);

			Catalog.Init ("gbrainy", "locale/");
			s = Catalog.GetString (str);

			if (String.IsNullOrEmpty (s) == true)
				return str;

			return s;
		}

	}
}

