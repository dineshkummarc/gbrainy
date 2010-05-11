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

namespace gbrainyTest
{
	[TestFixture]
	public class GameManagerTest
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			Console.WriteLine ("GameManagerTest constructor");
		}

		//Lists the games without tip
		public void GamesWithNoTip ()
		{
			int notip = 0;
			GameManager manager = new GameManager ();

			Type[] games = manager.CustomGames;

			foreach (Type type in games)
			{
				Game game = (Game) Activator.CreateInstance (type, true);
				if (game.TipString == String.Empty)
				{
					notip++;
					//Console.WriteLine ("Game with no tip {0} - {1}", game.Name, game);
				}
			}
			Console.WriteLine ("Games without tip: {0} ({1}%)", notip, notip * 100 / games.Length);
		}

		[Test]
		public void GamesNoDuplicatedName ()
		{
			Dictionary <string, bool> dictionary;
			GameManager manager = new GameManager ();
			Type[] games = manager.CustomGames;

			dictionary = new Dictionary <string, bool> (games.Length);

			foreach (Type type in games)
			{
				Game game = (Game) Activator.CreateInstance (type, true);

				Assert.AreEqual (false, dictionary.ContainsKey (game.Name),
					String.Format ("Game name {0} is duplicated", game.Name));

				dictionary.Add (game.Name, true);
			}
		}
	}
}
