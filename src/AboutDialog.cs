/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Glade;
using Gtk;
using Gdk;
using Mono.Unix;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;


public class AboutDialog : Gtk.AboutDialog
{
	public AboutDialog () : base ()
	{
		StringBuilder license = new StringBuilder (256);
		string [] authors = new string [] {
			"Jordi Mas i Hernandez <jmas@softcatala.org>",
		};

		// Name of the people that translated the application
		string translators = Catalog.GetString ("translator-credits");

		if (translators == "translator-credits")
			translators = null;

		license.Append (Catalog.GetString ("This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as  published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.\n\n"));
		license.Append (Catalog.GetString ("This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.\n\n"));
		license.Append (Catalog.GetString ("You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA."));

		Name = "gbrainy";
		Version = Defines.VERSION;
		Authors = authors;
		Documenters = null;
		Logo = LoadFromAssembly ("gbrainy.svg");

		Copyright = "(c) 2007-2008 Jordi Mas i Hernandez\n";
		Copyright += Catalog.GetString ("Based on ideas by Terry Stickels, MENSA books and Jordi Mas.");

		Comments = Catalog.GetString ("A brain teaser and trainer game to have fun and to keep your brain trained.");
		Website = "http://live.gnome.org/gbrainy";
		WebsiteLabel = Catalog.GetString ("gbrainy web site");
		TranslatorCredits = translators;
		IconName = null;
		License = license.ToString ();
		WrapLicense = true;
	}

	Pixbuf LoadFromAssembly (string resource)
	{
		try {
			return new Pixbuf (System.Reflection.Assembly.GetEntryAssembly (), resource);
		} catch {
			return null;
		}
	}
}
