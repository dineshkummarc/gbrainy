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
using System.Web;
using System.Web.UI;

namespace WebForms
{
	public partial class Default : System.Web.UI.Page
	{
		WebSession web_session;

		void Page_Load (object o, EventArgs e)
        	{
			web_session = Global.Sessions [Session.SessionID];

                	intro_label.Text = LanguageSupport.GetString (web_session, "gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained. It includes:");
			logic_label.Text = LanguageSupport.GetString (web_session, "Logic puzzles. Challenge your reasoning and thinking skills.");
			calculation_label.Text = LanguageSupport.GetString (web_session,"Mental calculation. Arithmetical operations that test your mental calculation abilities.");
			memory_label.Text = LanguageSupport.GetString (web_session, "Memory trainers. To prove your short term memory.");
			verbal_label.Text = LanguageSupport.GetString (web_session, "Verbal analogies. Challenge your verbal aptitude.");
	        }

		protected void OnStartGame (Object sender, EventArgs e)
		{
			Logger.Debug ("Default.OnStartGame. Start game button click");
			Response.Redirect ("Game.aspx");
		}
	}
}

