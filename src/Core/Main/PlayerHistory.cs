/*
 * Copyright (C) 2008-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Core.Main
{
	public class PlayerHistory
	{
		string file, config_path;
		List <GameHistory> games;
		int last_game;

		[Serializable]
		public class GameHistory
		{
			public int games_played;
			public int games_won;
			public int total_score;
			public int math_score;
			public int logic_score;
			public int memory_score;
			public int verbal_score;
		}

		public class PersonalRecord
		{
			public Game.Types GameType { get; set; }
			public int PreviousScore { get; set; }
			public int NewScore { get; set; }

			public PersonalRecord (Game.Types type, int previous_score, int new_score)
			{
				GameType = type;
				PreviousScore = previous_score;
				NewScore = new_score;
			}
		}

		public PlayerHistory ()
		{
			config_path = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
			config_path = Path.Combine (config_path, Defines.CONFIG_DIR);
			file = Path.Combine (config_path, "PlayerHistory.xml");
			last_game = -1;
		}

		public List <GameHistory> Games {
			get {
				if (games == null)
				{
					Load ();
					if (games == null) 
						games = new List <GameHistory> ();
				}
				return games; 
			}
		}

		public void Clean ()
		{
			Games.Clear ();
			Save ();
		}

		public void SaveGameSession (GameSession session)
		{
			if (session.GamesPlayed < Preferences.GetIntValue (Preferences.MinPlayedGamesKey)) {
				last_game = -1;
				return;
			}

			GameHistory history = new GameHistory ();
	
			history.games_played = session.GamesPlayed;
			history.games_won = session.GamesWon;
			history.math_score = session.MathScore;
			history.logic_score = session.LogicScore;
			history.memory_score = session.MemoryScore;
			history.total_score = session.TotalScore;
			history.verbal_score = session.VerbalScore;

			if (Games.Count >= Preferences.GetIntValue (Preferences.MaxStoredGamesKey))
				Games.RemoveAt (0);

			Games.Add (history);
			last_game = Games.Count - 1;
			Save ();
		}

		// Check if the last recorded game has been a personal record
		public List <PersonalRecord> GetLastGameRecords ()
		{
			List <PersonalRecord> records = new List <PersonalRecord> ();
			GameHistory higher;

			// We can start to talk about personal records after 5 plays
			if (last_game == -1 || Games.Count < 5)
				return records;

			higher = new GameHistory ();

			// Find the higher record for every type of game
			for (int i = 0; i < last_game; i++)
			{
				if (Games[i].logic_score > higher.logic_score) 
					higher.logic_score = Games[i].logic_score;

				if (Games[i].math_score > higher.math_score) 
					higher.math_score = Games[i].math_score;

				if (Games[i].memory_score > higher.memory_score) 
					higher.memory_score = Games[i].memory_score;

				if (Games[i].verbal_score > higher.verbal_score) 
					higher.verbal_score = Games[i].verbal_score;				
			}
			
			// It is a record?
			if (Games[last_game].logic_score > higher.logic_score)
				records.Add (new PersonalRecord (Game.Types.LogicPuzzle, higher.logic_score, Games[last_game].logic_score));

			if (Games[last_game].math_score > higher.math_score)
				records.Add (new PersonalRecord (Game.Types.MathTrainer, higher.math_score, Games[last_game].math_score));

			if (Games[last_game].memory_score > higher.memory_score)
				records.Add (new PersonalRecord (Game.Types.MemoryTrainer, higher.memory_score, Games[last_game].memory_score));

			if (Games[last_game].verbal_score > higher.verbal_score)
				records.Add (new PersonalRecord (Game.Types.VerbalAnalogy, higher.verbal_score, Games[last_game].verbal_score));

			return records;
		}

		private void Save ()
		{
			try {
				if (!Directory.Exists (config_path))
					Directory.CreateDirectory (config_path);

				using (FileStream str = File.Create (file))
				{
					XmlSerializer bf = new XmlSerializer (typeof (List <GameHistory>));
					bf.Serialize (str, Games);
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
}
