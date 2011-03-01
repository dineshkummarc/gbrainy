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
using System.Web.UI.HtmlControls;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;

using gbrainy.Core.Services;

namespace gbrainy.Clients.WebForms
{
	public partial class Game : System.Web.UI.Page
	{
		static GameManager manager;
		const int image_width = 400;
		const int image_height = 400;

		gbrainy.Core.Main.Game _game;
		gbrainy.Core.Main.GameSession session;
		WebSession web_session;

		static public GameManager CreateManager ()
		{
			if (manager != null)
				return manager;
			
			manager = new GameManager ();
			manager.LoadAssemblyGames (Defines.GAME_ASSEMBLY);
			manager.LoadVerbalAnalogies (System.IO.Path.Combine ("data/", Defines.VERBAL_ANALOGIES));
			manager.LoadGamesFromXml (System.IO.Path.Combine ("data/", "games.xml"));

			manager.Difficulty = gbrainy.Core.Main.GameDifficulty.Medium;
			manager.GameType = gbrainy.Core.Main.GameSession.Types.LogicPuzzles |
				 gbrainy.Core.Main.GameSession.Types.Calculation |
				gbrainy.Core.Main.GameSession.Types.VerbalAnalogies;
			return manager;
		}

		public WebSession WebSession {
			get {
				if (web_session == null)
					web_session = Global.Sessions [Session.SessionID];

				return web_session;
			}
		}

		string GetLanguageFromSessionHandler ()
		{
			return WebSession.LanguageCode;
		}

		// Page Life-Cycle Events
		//
		// - Page_Load
		// - Control events
		// - Page_LoadComplete
		//
		private void Page_LoadComplete (Object sender, EventArgs e)
		{
			if (WebSession.GameState != null && WebSession.GameState.Status == GameSession.SessionStatus.Finished)
				return;

			if (WebSession.NextGame == true)
			{
				_game = GetNextGame ();
				UpdateGame ();
			}
			WebSession.NextGame = false;
		}

		private void Page_Load (Object sender, EventArgs e)
		{
			if (IsPostBack == false)
				InitPage ();

			Logger.Debug ("Game.Page_Load. Page load starts. Session ID {0}, IsPostBack {1}", Session.SessionID,
				IsPostBack);

			HtmlForm form = (HtmlForm) Master.FindControl ("main_form");
			form.DefaultButton = answer_button.UniqueID;

			if (WebSession.GameState == null)
			{
				Logger.Debug ("Game.Page_Load creating new session");
				session = new gbrainy.Core.Main.GameSession ();
				session.GameManager = CreateManager ();
				session.New ();
				WebSession.GameState = session;
				Global.TotalGamesSessions++;

				_game = GetNextGame ();
				UpdateGame ();

				// If the first time that loads this does not have a session
				// send the user to the home page
				//Logger.Debug ("New Session, redirecting to Default.aspx");
				//Response.Redirect ("Default.aspx");
			} else if (WebSession.GameState != null && WebSession.GameState.Status == GameSession.SessionStatus.Finished)
			{
				// Finished game
				game_image.ImageUrl = Game.CreateImage (WebSession);
				answer_button.Enabled = false;
				answer_textbox.Text = string.Empty;
				answer_textbox.Enabled = false;
				nextgame_link.Enabled = false;
				endgames_button.Enabled = false;
				UpdateGame ();
			}
			else {
				session = WebSession.GameState;
				UpdateGame ();
			}

			if (IsPostBack == true) {
				Logger.Debug ("Game.Page_Load. Ignoring postback");
				return;
			}

			Logger.Debug ("Game.Page_Load. Page load completed");
		}

		void InitPage ()
		{
			TranslationsWeb service = (TranslationsWeb) ServiceLocator.Instance.GetService <ITranslations> ();
			service.OnGetLanguageFromSession = GetLanguageFromSessionHandler;

			game_image.Width = image_width;
			game_image.Height = image_height;

			nextgame_link.Text = "Next Game";

			// Toolbar
			allgames_label.Text = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All");
			endgames_label.Text = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Finish");
		}
#if _IMAGEMAP_

		string ProcessWidget (Widget widget)
		{
			string str;

			str = String.Format ("<area shape=\"rect\" coords=\"{0},{1},{2},{3}\" href=http://en.wikipedia.org/ />",
			                     widget.X, widget.Y, widget.Width, widget.Height);

			return str;
		}

		public string ImageMap
		{
			get {
				string str;

				if (_game == null)
					return "Nothing";

				str = String.Format ("<img src={0}  usemap=\"#mapname\" />",
					image.ImageUrl);

				foreach (Widget widget in _game.Widgets)
				{
					if (widget is Container)
					{
						foreach (Widget child in _game.Widgets)
						{
							str += ProcessWidget (child);
						}
					}
					else {
						str += ProcessWidget (widget);
					}
				}
				return str;
			}
		}
#endif
		void UpdateGame ()
		{
			if (_game == null)
				return;

			status.Text = session.StatusText;
			question.Text = _game.Question;
		 	game_image.ImageUrl = CreateImage (WebSession);
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
				cairo_image = new Cairo.ImageSurface (Cairo.Format.ARGB32, image_width, image_height);
				cr = new gbrainy.Core.Main.CairoContextEx (cairo_image, "sans 12", 96);
				file = GetImageFileName (_session.Session.SessionID);

				// Draw Image
				_session.GameState.Draw (cr, image_width, image_height, false);
				cairo_image.WriteToPng (file);

				if (File.Exists (file) == false)
					Logger.Error ("Game.CreateImage. Error writting {0}", file);
				else
					Logger.Debug ("Game.CreateImage. Wrote image {0}", file);

				// Prevent IE from caching the image
				file += "?" + DateTime.Now.Ticks;
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

			// TODO: This should be done at GameSession.Level
			session.ScoreGame (String.Empty);
			// TODO: Use Ajax for dynamic loading

			WebSession.NextGame = true;
			Response.Redirect ("Game.aspx");
		}

		public virtual void OnClickAnswer (Object sender, EventArgs e)
		{
			Logger.Debug ("Game.OnClickAnswer");

			string answer = answer_textbox.Text;

			if (String.IsNullOrEmpty (answer) == false)
			{
				if (web_session.GameState.ScoreGame (answer) == true) {
					result_label.Text = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Congratulations.");
					result_label.CssClass = "CorrectAnswer";
				}
				else {
					result_label.Text = ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Incorrect answer.");
					result_label.CssClass = null;
				}

				rationale_label.Text = WebSession.GameState.CurrentGame.AnswerText;
				answer_button.Enabled = false;
			}
		}

		protected void OnClickEndGame (Object sender, EventArgs e)
		{
			Logger.Debug ("Game.OnClickEndGame");

			if (session != null)
				session.End ();
			
			Global.TotalEndedSessions++;			
			Global.TotalGames += session.History.GamesPlayed;
			Global.TotalTimeSeconds += session.GameTime.Seconds;

			Response.Redirect ("Game.aspx");
		}

		protected void OnStartAllGames (Object sender, EventArgs e)
		{
			WebSession.GameState = null;
			Response.Redirect ("Game.aspx");
		}
	}
}
