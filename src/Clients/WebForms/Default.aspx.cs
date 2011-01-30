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
using System.Web.UI.WebControls;

namespace gbrainy.Clients.WebForms
{
	public partial class Default : System.Web.UI.Page
	{
		WebSession web_session;
		const string CookieName = "Lang";

		void Page_Load (object o, EventArgs e)
        	{
			Logger.Debug ("Default.Page_Load. IsPostBack {0}", IsPostBack);

			if (IsPostBack == true)
				return;
			
			web_session = Global.Sessions [Session.SessionID];

                	intro_label.Text = "gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained. It includes:";
			logic_label.Text = "Logic puzzles. Challenge your reasoning and thinking skills.";
			calculation_label.Text = "Mental calculation. Arithmetical operations that test your mental calculation abilities.";
			memory_label.Text = "Memory trainers. To prove your short term memory.";
			verbal_label.Text = "Verbal analogies. Challenge your verbal aptitude.";
			
			for (int i = 0; i <LanguageSupport.Languages.Length; i++)
			{
				languages_drop.Items.Add (new ListItem (LanguageSupport.Languages[i].Name,
					i.ToString ()));
			}

			if (Request.Cookies [CookieName] != null)
    				languages_drop.SelectedValue = Request.Cookies[CookieName].Value;
			else // Default language value
				languages_drop.SelectedValue = "0";

			Global.Sessions [Session.SessionID].LanguageIndex =
				Int32.Parse (languages_drop.SelectedValue);
	        }
		
		protected void OnSelectedIndexChanged (object sender, EventArgs e)
        	{
			if (Response.Cookies[CookieName].Value == languages_drop.SelectedValue)
				return;

			Response.Cookies[CookieName].Value = languages_drop.SelectedValue;
			Response.Cookies[CookieName].Expires =  DateTime.Now.AddYears (1);

			Logger.Debug ("MasterPage.OnSelectedIndexChanged. Set lang cookie to: {0}",
				languages_drop.SelectedValue);

			Global.Sessions [Session.SessionID].LanguageIndex =
				Int32.Parse (languages_drop.SelectedValue);

			///Response.Redirect ("Default.aspx");

        	}
		
		protected void OnStartGame (Object sender, EventArgs e)
		{
			web_session = Global.Sessions [Session.SessionID];
			web_session.GameState = null;
			Logger.Debug ("Default.OnStartGame. Start game button click");
			Response.Redirect ("Game.aspx");
		}
	}
}

