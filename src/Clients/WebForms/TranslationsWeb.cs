/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.IO;

using gbrainy.Core.Services;

namespace gbrainy.Clients.WebForms
{
	public class TranslationsWeb : ITranslations
	{
		public delegate string GetLanguageFromSessionHandler ();
		static readonly object sync = new object ();
		
		public int TranslationPercentage {
			get { return 100;}
		}

		public GetLanguageFromSessionHandler OnGetLanguageFromSession;

		public void Init (string package, string localedir)
		{
			Catalog.Init (package, localedir);
		}
		
		string GetLanguageFromSession ()
		{
			if (OnGetLanguageFromSession == null)
				return LanguageSupport.Languages [0].LangCode;
			
			return OnGetLanguageFromSession ();
		}

		public string GetString (string s)
		{
			string str;

			lock (sync)
			{
				string code = GetLanguageFromSession ();
				SetContext (code);
				str = Catalog.GetString (s);
			}
			return str;
		}

		public string GetPluralString (string s, string p, int n)
		{
			string str;

			lock (sync)
			{
				string code = GetLanguageFromSession ();
				SetContext (code);
				str = Catalog.GetPluralString (s, p, n);
			}
			return str;
		}

		void SetContext (string code)
		{
			string langcode = LanguageSupport.GetFromCode (code).LocaleLanguage;
			Environment.SetEnvironmentVariable ("LANGUAGE", langcode);
			Init ("gbrainy", Defines.LOCALE_DIR);
		}
	}
}

