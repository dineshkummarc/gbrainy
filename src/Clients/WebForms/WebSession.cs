using System;
using System.Web.SessionState;
using gbrainy.Core;

namespace WebForms
{
	public class WebSession
	{
		public HttpSessionState Session { get; set; }
		public DateTime Started { get; set; }
		public gbrainy.Core.Main.GameSession GameState { get; set; }
		public int LanguageIndex  { get; set; }

		public WebSession (HttpSessionState session)
		{
			Session = session;
			Started = DateTime.Now;
            	}
        }
}

