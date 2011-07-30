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
using System.Collections.Generic;
using NUnit.Framework;

using gbrainy.Core.Main;

namespace gbrainy.Test.Core
{
	[TestFixture]
	public class GameSessionPlayListTest : UnitTestSupport
	{
		GameManager manager;

		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
			manager = new GameManager ();
		}

		[Test]
		public void CustomGamesNoRandomOrder ()
		{
			Dictionary <int, string> dictionary;
			GameLocator [] games;
			GameSessionPlayList play_list = new GameSessionPlayList (manager);

			List <int> list = new List <int> ();
			games = manager.AvailableGames;

			// Create a hash to map from game name to locator
			dictionary = new Dictionary <int, string> (games.Length);
			for (int i = 0; i < games.Length; i++)
			{
				if (games[i].IsGame == false)
					continue;

				Game game = (Game) Activator.CreateInstance (games[i].TypeOf, true);
				game.Variant = games[i].Variant;
				dictionary.Add (i, game.Name);
				list.Add (dictionary.Count - 1);
			}

			Game current;
			string name;

			play_list.RandomOrder = false;
			play_list.PlayList = list.ToArray ();

			for (int i = 0; i < list.Count; i++)
			{
				current = play_list.GetPuzzle ();

				try
				{
					name = dictionary [i];
					Assert.AreEqual (true, name == current.Name);
				}
				catch (KeyNotFoundException)
				{

				}
			}
		}

		[Test]
		public void ColorBlind ()
		{
			Game game;
			GameSessionPlayList play_list = new GameSessionPlayList (manager);

			play_list.ColorBlind = true;
			for (int i = 0; i < play_list.PlayList.Length; i++)
			{
				game = play_list.GetPuzzle ();
				Assert.AreEqual (false, game.UsesColors);
			}
		}

		[Test]
		public void Difficulty ()
		{
			Game game;
			GameSessionPlayList play_list = new GameSessionPlayList (manager);

			play_list.Difficulty = GameDifficulty.Easy;
			for (int i = 0; i < play_list.PlayList.Length; i++)
			{
				game = play_list.GetPuzzle ();
				Assert.AreEqual (GameDifficulty.Easy, game.Difficulty);
			}
		}
	}
}
