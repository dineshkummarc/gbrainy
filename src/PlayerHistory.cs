/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class PlayerHistory
{
	private string file, config_path;
	private List <GameHistory> games;

	[Serializable]
	public class GameHistory
	{
		public int games_played;
		public int games_won;
		public int total_score;
		public int math_score;
		public int logic_score;
		public int memory_score;
	}

	public PlayerHistory ()
	{
		games = new List <GameHistory> ();
		config_path = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
		config_path = Path.Combine (config_path, "gbrainy");
		file = Path.Combine (config_path, "PlayerHistory.xml");
		Load ();
	}

	public List <GameHistory> Games {
		get { return games; }
	}

	public void Clean ()
	{
		games.Clear ();
		Save ();
	}

	public void SaveGameSession (GameSession session)
	{
		GameHistory history = new GameHistory ();

		if (session.GamesPlayed < gbrainy.preferences.GetIntValue (Preferences.MinPlayedGamesKey))
			return;
	
		history.games_played = session.GamesPlayed;
		history.games_won = session.GamesWon;
		history.math_score = session.MathScore;
		history.logic_score = session.LogicScore;
		history.memory_score = session.MemoryScore;
		history.total_score = session.TotalScore;

		if (games.Count >= gbrainy.preferences.GetIntValue (Preferences.MaxStoredGamesKey))
			games.RemoveAt (0);

		games.Add (history);
		Save ();
	}

	private void Save ()
	{
		try {

			if (!Directory.Exists (config_path))
				Directory.CreateDirectory (config_path);

			using (FileStream str = File.Create (file))
			{
				XmlSerializer bf = new XmlSerializer (typeof (List <GameHistory>));
				bf.Serialize (str, games);
			}
		}
		
		catch (Exception)
		{
		}
	}

	private void Load ()
	{
		try {
			using (FileStream str = File.OpenRead (file))
			{
				XmlSerializer bf = new XmlSerializer (typeof (List <GameHistory>));
			    	games = (List <GameHistory>) bf.Deserialize(str);
			}
		}
		catch (Exception)
		{
		}
	}
	
}


