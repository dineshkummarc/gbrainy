/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

public class GameOverDialog
{
	private Glade.XML xml;
	private Gtk.Dialog dialog;
	private const string dialog_name = "gameover";
	[Widget] Gtk.Label timeplayed;
	[Widget] Gtk.Label gamesplayed;
	[Widget] Gtk.Label timepergame;

	public GameOverDialog ()
	{
		dialog = null;
		xml = new Glade.XML (null, "gbrainy.glade", "gameover", "gbrainy");
		xml.Autoconnect (this);
	}

	public void Run ()
	{
		Dialog.Run ();
	}

	public string TimePayed {
		set { 
			timeplayed.Text = value;
		}
	}

	public string GamesPlayed {
		set { 
			gamesplayed.Text = value;
		}
	}

	public string TimePerGame {
		set { 
			timepergame.Text = value;
		}
	}


	public Gtk.Dialog Dialog {
		get {
			if (dialog == null)
				dialog = (Gtk.Dialog) xml.GetWidget (dialog_name);
				
			return dialog;
		}
	}
	
}
