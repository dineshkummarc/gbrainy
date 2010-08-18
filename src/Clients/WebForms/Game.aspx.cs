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
using System.IO;
using System.Web;
using System.Web.UI;
using gbrainy.Core.Main;

namespace WebForms
{
	public partial class Game : System.Web.UI.Page
	{
		gbrainy.Core.Main.Game _game;
		gbrainy.Core.Main.GameSession session;
		WebSession web_session;
		static GameManager manager;

		static public GameManager CreateManager ()
		{
			manager = new GameManager ();
			manager.LoadAssemblyGames ("bin/gbrainy.Games.dll");

			manager.LoadVerbalAnalogies (System.IO.Path.Combine ("data/", "verbal_analogies.xml"));
			//manager.LoadGamesFromXml (System.IO.Path.Combine (Defines.DATA_DIR, "games.xml"));
			//manager.LoadPlugins ();

			manager.Difficulty = gbrainy.Core.Main.GameDifficulty.Medium;
			manager.GameType = gbrainy.Core.Main.GameSession.Types.LogicPuzzles |
				 gbrainy.Core.Main.GameSession.Types.CalculationTrainers |
				gbrainy.Core.Main.GameSession.Types.VerbalAnalogies;
			return manager;
		}

		private void Page_Load (Object sender, EventArgs e)
		{
			web_session = Global.Sessions [Session.SessionID];

			Logger.Debug ("Game.Page_Load. Page load starts. Session ID {0}, IsPostBack {1}", Session.SessionID,
				IsPostBack);

			if (web_session.GameState == null ||
				web_session.GameState.Status == GameSession.SessionStatus.Finished)
			{
				Logger.Debug ("Game.Page_Load creating new session");
				session = new gbrainy.Core.Main.GameSession ();
				session.GameManager = CreateManager ();
				session.New ();
				web_session.GameState = session;

				// If the first time that loads this does not have a session
				// send the user to the home page
				//Logger.Debug ("New Session, redirecting to Default.aspx");
				//Response.Redirect ("Default.aspx");
			} else
				session = web_session.GameState;

			if (IsPostBack == true) {
				Logger.Debug ("Game.Page_Load. Ignoring postback");
				return;
			}

			string answer = answer_textbox.Text;

			Logger.Debug ("Game.Page_Load. Got answer: {0}", answer);

			_game = GetNextGame ();

			UpdateGame ();

			nextgame_link.Text = "Next Game";
			endgame_link.Text = "End Game";
			Logger.Debug ("Game.Page_Load. Page load completed");
		}

		void UpdateGame ()
		{
			if (_game == null)
				return;

			status.Text = session.StatusText;
			question.Text = LanguageSupport.GetString (web_session, _game.Question);
		 	image.ImageUrl = CreateImage (web_session);
		}

		public gbrainy.Core.Main.Game GetNextGame ()
		{
			gbrainy.Core.Main.Game g = null;

			if (session.Status != gbrainy.Core.Main.GameSession.SessionStatus.Finished)
			{
				session.NextGame ();
				g = session.CurrentGame;
			}
			return g;
		}

		//
		static public string GetImageFileName (string sessionid)
		{
			string file;

			file = "tmp/" + sessionid + ".png";
			return file;
		}

		static public string CreateImage (WebSession _session)
		{
			Cairo.ImageSurface cairo_image = null;
			gbrainy.Core.Main.CairoContextEx cr = null;
			string file = string.Empty;

			try
			{
				cairo_image = new Cairo.ImageSurface (Cairo.Format.ARGB32, 400, 400);
				cr = new gbrainy.Core.Main.CairoContextEx (cairo_image, "sans 12", 96);
				file = GetImageFileName (_session.Session.SessionID);

				// Draw Image
				_session.GameState.Draw (cr, 400, 400, false);
				cairo_image.WriteToPng (file);

				if (File.Exists (file) == false)
					Logger.Error ("Game.CreateImage. Error writting {0}", file);
				else
					Logger.Debug ("Game.CreateImage. Wrote image {0}", file);
			}

			finally
			{
				if (cr != null)
					((IDisposable) cr).Dispose ();

				if (cairo_image != null)
					((IDisposable) cairo_image).Dispose ();
			}

		    	return file;
		}

		public virtual void OnClickNextGame (Object sender, EventArgs e)
		{
			Logger.Debug ("Game.OnClickNextGame");
			/*
			Console.WriteLine ("--> OnClick Next Game");
			Cache.Remove ("game");

			_game = GetNextGame ();
			UpdateGame ();
			*/

			// TODO: This should be done at GameSession.Level
			session.ScoreGame (String.Empty);
			// TODO: Use Ajax for dynamic loading
			Response.Redirect ("Game.aspx");
		}

		public virtual void OnClickAnswer (Object sender, EventArgs e)
		{
			Logger.Debug ("Game.OnClickAnswer");

			string answer = answer_textbox.Text;

			if (String.IsNullOrEmpty (answer) == false)
			{
				if (session.ScoreGame (answer) == true) {
					result_label.Text = LanguageSupport.GetString (web_session, "Congratulations.");
					result_label.CssClass = "CorrectAnswer";
				}
				else {
					result_label.Text = LanguageSupport.GetString (web_session, "Incorrect. ");
					result_label.CssClass = null;
				}

				rationale_label.Text = LanguageSupport.GetString (web_session, session.CurrentGame.Answer);
			} else

			answer_button.Enabled = false;
		}

		public virtual void OnClickEndGame (Object sender, EventArgs e)
		{
			Logger.Debug ("Game.OnClickEndGame");
			session.End ();

			Response.Redirect ("Finish.aspx");
		}

	}
}
