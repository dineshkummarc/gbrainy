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
using Gtk;
using Mono.Unix;
using System.Collections;

using gbrainy.Core.Main;

namespace gbrainy.Clients.Classical
{
	public class PreferencesDialog : BuilderDialog
	{
		[GtkBeans.Builder.Object] Gtk.SpinButton prefspinbutton;
		[GtkBeans.Builder.Object] Gtk.SpinButton maxstoredspinbutton;
		[GtkBeans.Builder.Object] Gtk.SpinButton minplayedspinbutton;
		[GtkBeans.Builder.Object] Gtk.CheckButton prefcheckbutton;
		[GtkBeans.Builder.Object] Gtk.CheckButton colorblindcheckbutton;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_easy;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_medium;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_master;

		PlayerHistory history;

		public PreferencesDialog (PlayerHistory history) : base ("PreferencesDialog.ui", "preferences")
		{
			this.history = history;
			prefspinbutton.Value = Preferences.GetIntValue (Preferences.MemQuestionTimeKey);
			prefcheckbutton.Active = Preferences.GetBoolValue (Preferences.MemQuestionWarnKey);
			maxstoredspinbutton.Value = Preferences.GetIntValue (Preferences.MaxStoredGamesKey);
			minplayedspinbutton.Value = Preferences.GetIntValue (Preferences.MinPlayedGamesKey);
			colorblindcheckbutton.Active = Preferences.GetBoolValue (Preferences.ColorBlindKey);
		
			switch ((Game.Difficulty) Preferences.GetIntValue (Preferences.DifficultyKey)) {
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

		private void OnCleanHistory (object sender, EventArgs args)
		{
			history.Clean ();
		}

		private void OnOK (object sender, EventArgs args)
		{
			Preferences.SetIntValue (Preferences.MemQuestionTimeKey, (int) prefspinbutton.Value);
			Preferences.SetBoolValue (Preferences.MemQuestionWarnKey, prefcheckbutton.Active);
			Preferences.SetIntValue (Preferences.DifficultyKey, (int) Difficulty);
			Preferences.SetIntValue (Preferences.MaxStoredGamesKey, (int) maxstoredspinbutton.Value);
			Preferences.SetIntValue (Preferences.MinPlayedGamesKey, (int) minplayedspinbutton.Value);
			Preferences.SetBoolValue (Preferences.ColorBlindKey, colorblindcheckbutton.Active);
			Preferences.Save ();
		}
	}
}
