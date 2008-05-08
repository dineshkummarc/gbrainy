/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Collections;

public class PreferencesDialog : GtkDialog
{
	[Glade.Widget] Gtk.SpinButton prefspinbutton;
	[Glade.Widget] Gtk.CheckButton prefcheckbutton;
	[Glade.Widget] Gtk.RadioButton rb_easy;
	[Glade.Widget] Gtk.RadioButton rb_medium;
	[Glade.Widget] Gtk.RadioButton rb_master;

	public PreferencesDialog () : base ("preferences")
	{
		prefspinbutton.Value = gbrainy.preferences.GetIntValue (Preferences.MemQuestionTimeKey);
		prefcheckbutton.Active = gbrainy.preferences.GetBoolValue (Preferences.MemQuestionWarnKey);
			
		switch ((Game.Difficulty) gbrainy.preferences.GetIntValue (Preferences.DifficultyKey)) {
		case Game.Difficulty.Easy:
			rb_easy.Active = rb_easy.HasFocus = true;
			break;		
		case Game.Difficulty.Medium:
			rb_medium.Active = rb_medium.HasFocus = true;
			break;
		case Game.Difficulty.Master:
			rb_master.Active = rb_master.HasFocus = true;
			break;
		}

	}

	private Game.Difficulty Difficulty {
		get {
			if (rb_easy.Active)
				return Game.Difficulty.Easy;

			if (rb_master.Active)
				return Game.Difficulty.Master;

			return Game.Difficulty.Medium;			
		}
	}

	private void OnOK (object sender, EventArgs args)
	{
		gbrainy.preferences.SetIntValue (Preferences.MemQuestionTimeKey, (int) prefspinbutton.Value);
		gbrainy.preferences.SetBoolValue (Preferences.MemQuestionWarnKey, prefcheckbutton.Active);
		gbrainy.preferences.SetIntValue (Preferences.DifficultyKey, (int) Difficulty);
		gbrainy.preferences.Save ();
	}
}

