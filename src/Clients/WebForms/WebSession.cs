using System;
using System.Web.SessionState;

namespace gbrainy.Clients.WebForms
{
	public class WebSession
	{
		public bool NextGame;
		public HttpSessionState Session { get; set; }
		public DateTime Started { get; set; }
		public gbrainy.Core.Main.GameSession GameState { get; set; }
		public string LanguageCode  { get; set; }

		public WebSession (HttpSessionState session)
		{
			Session = session;
			Started = DateTime.Now;
            	}
        }
}

