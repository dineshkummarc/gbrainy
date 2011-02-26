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
using Mono.Unix;

namespace gbrainy.Clients.WebForms
{
	static class LanguageSupport
	{
		public class Language
		{
			public string Name { get; set; }
			public string LocaleLanguage { get; set; }
			public string LangCode  { get; set; }

			public Language (string name, string code)
			{
				Name = name;
				LocaleLanguage = code;
			}
		};
		
		// code -> language
		static Dictionary <string, Language> langmap;
		
		static LanguageSupport ()
		{
			string code;
			int idx;
			
			langmap = new Dictionary <string, Language> ();
			foreach (Language language in languages)
			{
				idx = language.LocaleLanguage.IndexOf (".");
				code =  language.LocaleLanguage.Substring (0, idx);
				language.LangCode = code;
				langmap.Add (code, language);
			}
			
		}
				
		// List of exposed locales
		static Language [] languages =
		{
			new Language ("English", "en_US.utf8"), // English always the first one, used as default
			new Language ("Afrikaans", "af_ZA.utf8"),
			new Language ("Catalan", "ca_ES.utf8"),
			new Language ("Czech", "cs_CZ.utf8"),
			new Language ("Danish", "da_DK.utf8"),
			new Language ("German", "de_DE.utf8"),
			new Language ("Basque", "eu_ES.utf8"),
			new Language ("Spanish", "es_ES.utf8"),
			new Language ("French", "fr_FR.utf8"),
			new Language ("Galician", "gl_ES.utf8"),
			new Language ("Hungarian", "hu_HU.utf8"),
			new Language ("Dutch", "nl_NL.utf8"),
			new Language ("Portuguese", "pt_PT.utf8"),
			new Language ("Romanian", "ro_RO.utf8"),
			new Language ("Brazilian Portuguese", "pt_BR.utf8"),
			new Language ("Slovenian", "sl_SI.utf8"),
			new Language ("Swedish", "sv_SE.utf8"),
			new Language ("Serbian", "sr_RS.utf8")
		};

		static public Language [] Languages
		{
			get { return languages;}

		}

		static public Language GetFromCode (string code)
		{
			if (langmap.ContainsKey (code) == false)
				return languages [0];
			
			return langmap [code];
		}
	}
}

