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

using Gtk;
using gbrainy.Core.Services;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class BuilderDialog : Gtk.Dialog
	{
		protected ITranslations Translations { get; private set;}

		public BuilderDialog (ITranslations translations, string resourceName, string dialogName) : 
			this ((System.Reflection.Assembly) null, resourceName, dialogName)
		{
			Translations = translations;
		}

		public BuilderDialog (System.Reflection.Assembly assembly, string resourceName, string dialogName) : 
			this (new Builder (assembly, resourceName, null),dialogName)
		{
		}

		public BuilderDialog (Builder builder, string dialogName) //: base (builder.GestRawObject (dialogName))
		{
			builder.Autoconnect (this);
			IconName = "gbrainy";
		}
	}
}
