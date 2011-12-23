/*
 * Copyright (C) 2007-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Linq;
using System.Collections.Generic;

namespace gbrainy.Core.Main
{
	// Manages a list of games to played within a session based on difficulty, game types and other parameters
	public class GameSessionPlayList
	{
		IEnumerator <int> enumerator;
		List <int> play_list; // Play list for the Selected difficulty, game types
		GameDifficulty difficulty;
		bool color_blind;
		bool dirty;
		GameSession.Types game_type;
		GameManager manager;

		public GameSessionPlayList (GameManager manager)
		{
			this.manager = manager;
			play_list = new List <int> ();
			game_type = GameSession.Types.AllGames;
			difficulty = GameDifficulty.Medium;
			RandomOrder = true;
			dirty = true;
		}

		public bool ColorBlind {
			get { return color_blind;}
			set {
				if (color_blind == value)
					return;

				color_blind = value;
				dirty = true;
			}
		}

		public GameDifficulty Difficulty {
			set {
				if (difficulty == value)
					return;

				difficulty = value;
				dirty = true;
			}
			get { return difficulty; }
		}

		public GameManager GameManager {
			get { return manager;}
			set {
				if (manager == value)
					return;

				manager = value;
				dirty = true;
			}
		}

		public GameSession.Types GameType {
			get {return game_type; }
			set {
				if (game_type == value)
					return;

				game_type = value;
				dirty = true;
			}
		}

		// Indicates if the PlayList for CustomGames is delivered in RandomOrder
 		public bool RandomOrder { get; set; }

		// Establish the PlayList (the indices of the array to available games)
		public int [] PlayList {
			get { return play_list.ToArray ();}
			set {
				play_list = new List <int> (value);
				dirty = false;
				UpdateEnumerator ();
			}
		}

		void BuildListIfIsDirty ()
		{
			if (dirty == false)
				return;

			if ((game_type & GameSession.Types.Custom) != GameSession.Types.Custom)
				BuildPlayList (manager.AvailableGames);

			dirty = false;
		}

		// Taking a GameLocator list builds the play_list
		void BuildPlayList (GameLocator [] all_games)
		{
			if ((game_type & GameSession.Types.Custom) == GameSession.Types.Custom)
				throw new InvalidOperationException ();

			ArrayListIndicesRandom indices = new ArrayListIndicesRandom (all_games.Length);
			indices.Initialize ();
			play_list.Clear ();

			// Decide which game types are part of the list
			bool logic, memory, calculation, verbal;
			if ((game_type & GameSession.Types.AllGames) == GameSession.Types.AllGames)
			{
				logic = memory = calculation = verbal = true;
			}
			else
			{
				logic = (game_type & GameSession.Types.LogicPuzzles) == GameSession.Types.LogicPuzzles;
				calculation = (game_type & GameSession.Types.Calculation) == GameSession.Types.Calculation;
				memory = (game_type & GameSession.Types.Memory) == GameSession.Types.Memory;
				verbal = (game_type & GameSession.Types.VerbalAnalogies) == GameSession.Types.VerbalAnalogies;
			}

			// Create item arrays for games types
			List <int> logic_indices = new List <int> ();
			List <int> calculation_indices = new List <int> ();
			List <int> memory_indices = new List <int> ();
			List <int> verbal_indices = new List <int> ();

			if (logic)
				logic_indices.AddRange (indices.Where (a => all_games[a].GameType == GameTypes.LogicPuzzle));

			if (memory)
				memory_indices.AddRange (indices.Where (a => all_games[a].GameType == GameTypes.Memory));

			if (calculation)
				calculation_indices.AddRange (indices.Where (a => all_games[a].GameType == GameTypes.Calculation));

			if (verbal)
				verbal_indices.AddRange (indices.Where (a => all_games[a].GameType == GameTypes.VerbalAnalogy));

			CreateListWithDistributedGameTypes (logic_indices, calculation_indices, memory_indices, verbal_indices);
			enumerator = play_list.GetEnumerator ();
		}

		void CreateListWithDistributedGameTypes (List <int> logic_indices, List <int> calculation_indices, List <int> memory_indices,
			List <int> verbal_indices)
		{
			int total = logic_indices.Count + memory_indices.Count + calculation_indices.Count + verbal_indices.Count;
			int pos_logic, pos_memory, pos_calculation, pos_verbal;
			Random random = new Random ();

			pos_logic = pos_memory = pos_calculation = pos_verbal = 0;

			while (play_list.Count < total)
			{
				switch (random.Next (3)) {
				case 0:
					if (pos_calculation < calculation_indices.Count) play_list.Add (calculation_indices[pos_calculation++]);
					if (pos_logic < logic_indices.Count) play_list.Add (logic_indices[pos_logic++]);
					if (pos_memory < memory_indices.Count) play_list.Add (memory_indices[pos_memory++]);
					if (pos_verbal < verbal_indices.Count) play_list.Add (verbal_indices[pos_verbal++]);
					break;
				case 1:
					if (pos_memory < memory_indices.Count) play_list.Add (memory_indices[pos_memory++]);
					if (pos_calculation < calculation_indices.Count) play_list.Add (calculation_indices[pos_calculation++]);
					if (pos_verbal < verbal_indices.Count) play_list.Add (verbal_indices[pos_verbal++]);
					if (pos_logic < logic_indices.Count) play_list.Add (logic_indices[pos_logic++]);
					break;
				case 2:
					if (pos_calculation < calculation_indices.Count) play_list.Add (calculation_indices[pos_calculation++]);
					if (pos_verbal < verbal_indices.Count) play_list.Add (verbal_indices[pos_verbal++]);
					if (pos_memory < memory_indices.Count) play_list.Add (memory_indices[pos_memory++]);
					if (pos_logic < logic_indices.Count) play_list.Add (logic_indices[pos_logic++]);
					break;
				}
			}
		}

		void UpdateEnumerator ()
		{
			if (RandomOrder == true)
			{
				ArrayListIndicesRandom random = new ArrayListIndicesRandom (play_list.Count);
				random.RandomizeFromArray (play_list);
				enumerator = random.GetEnumerator ();
			}
			else
				enumerator = play_list.GetEnumerator ();
		}

		public Game GetPuzzle ()
		{
			Game puzzle, first = null;

			BuildListIfIsDirty ();

			while (true) {

				if (enumerator.MoveNext () == false) { // All the games have been played, restart again
					enumerator = play_list.GetEnumerator ();
					enumerator.MoveNext ();
				}

				puzzle = (Game) Activator.CreateInstance ((Type) manager.AvailableGames [enumerator.Current].TypeOf, true);
				puzzle.Variant = manager.AvailableGames [enumerator.Current].Variant;

				if (first != null && first.GetType () == puzzle.GetType ())
					break;

				if (puzzle.IsPlayable == false)
					continue;

				if (ColorBlind == true && puzzle.UsesColors == true)
					continue;

				if (first == null)
					first = puzzle;

				if ((puzzle.Difficulty & difficulty) == difficulty)
					break;
			}

			puzzle.CurrentDifficulty = Difficulty;
			return puzzle;
		}
	}
}
