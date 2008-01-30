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

public class PreferencesDialog
{
	private Glade.XML xml;
	private Gtk.Dialog dialog;
	private const string dialog_name = "preferences";
	[Glade.Widget] Gtk.SpinButton prefspinbutton;
	[Glade.Widget] Gtk.CheckButton prefcheckbutton;
	[Glade.Widget] Gtk.RadioButton rb_easy;
	[Glade.Widget] Gtk.RadioButton rb_medium;
	[Glade.Widget] Gtk.RadioButton rb_master;

	public PreferencesDialog ()
	{
		dialog = null;
		xml = new Glade.XML (null, "gbrainy.glade", dialog_name, "gbrainy");
		xml.Autoconnect (this);
	}
	
	public virtual int MemQuestionTime {
		get { return prefspinbutton.ValueAsInt;}
		set { prefspinbutton.Value = value; }
	}

	public virtual bool MemQuestionWarn {
		get { return prefcheckbutton.Active;}
		set { prefcheckbutton.Active = value;}
	}

	public virtual Game.Difficulty Difficulty {
		get {
			if (rb_easy.Active)
				return Game.Difficulty.Easy;

			if (rb_master.Active)
				return Game.Difficulty.Master;

			return Game.Difficulty.Medium;			
		}
		set {
			switch (value) {
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
	}

	public Gtk.Dialog Dialog {
		get {
			if (dialog == null)
				dialog = (Gtk.Dialog) xml.GetWidget (dialog_name);
				
			return dialog;
		}
	}

	public int Run ()
	{
		return Dialog.Run ();
	}
        
}
