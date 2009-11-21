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
using Mono.Unix;

namespace gbrainy.Clients.Classical
{
	public class GtkDialog
	{
		public Glade.XML xml;
		public Gtk.Dialog dialog;
		public string dialog_name;

		public GtkDialog (string dialog_name)
		{
			this.dialog_name = dialog_name;
			xml = new Glade.XML (null, "gbrainy.glade", dialog_name, "gbrainy");
			xml.Autoconnect (this);
			Dialog.IconName = "gbrainy";
			dialog = null;
		}

		public ResponseType Run ()
		{
			return (ResponseType) Dialog.Run ();
		}

		public Gtk.Dialog Dialog {
			get {
				if (dialog == null)
					dialog = (Gtk.Dialog) xml.GetWidget (dialog_name);
			
				return dialog;
			}
		}
	}
}
