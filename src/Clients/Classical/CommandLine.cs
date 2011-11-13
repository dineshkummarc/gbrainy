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
using System.Text;
using Mono.Unix;
using System.Collections.Generic;
using System.Reflection;

using gbrainy.Core.Main;
using gbrainy.Core.Services;

namespace gbrainy.Clients.Classical
{
	public class CommandLine
	{
		string [] args;
		int [] play_list;
		bool cont_execution;
		ITranslations translations;

		public static readonly char GAME_SEPARATOR = ',';

		public CommandLine (ITranslations translations, string [] args)
		{
			this.translations = translations;
			this.args = args;
			RandomOrder = true;
			play_list = new int [0];
		}

		public bool Continue {
			get { return cont_execution; }
		}

		public int [] PlayList {
			get { return play_list; }
		}

		public bool RandomOrder { get; set; }

		public void Parse ()
		{
			cont_execution = true;

			for (int idx = 0; idx < args.Length; idx++)
			{
				switch (args [idx]) {
				case "--customgame":
					string [] names;

					if (idx + 1 >= args.Length)
						break;

					idx++;
					names = args [idx].Split (GAME_SEPARATOR);

					for (int i = 0; i < names.Length; i++)
						names[i] = names[i].Trim ();

					BuildPlayList (names);
					break;
				case "--gamelist":
					GameList ();
					cont_execution = false;
					break;
				case "--norandom":
					RandomOrder = false;
					break;
				case "--version":
					cont_execution = false;
					break;
				case "--versions":
					Versions ();
					cont_execution = false;
					break;
				case "--help":
				case "--usage":
					Usage ();
					cont_execution = false;
					break;
				default:
					Console.WriteLine (Catalog.GetString ("Unknown command line parameter {0}"), args [idx]);
					break;
				}
			}
		}

		public static void Version ()
		{
			Console.WriteLine ("gbrainy " + Defines.VERSION + " " +
				// Translators: {0} is a date
				String.Format (Catalog.GetString ("(built on {0})"), Defines.BUILD_TIME));
		}

		void GameList ()
		{
			GameLocator [] games;
			GameManager gm = new GameManager ();

			GtkClient.GameManagerPreload (gm);
			games = gm.AvailableGames;

			Console.WriteLine (Catalog.GetString ("List of available games"));

			for (int i = 0; i < games.Length; i++)
			{
				if (games[i].IsGame == false)
					continue;

				Game game = (Game) Activator.CreateInstance (games[i].TypeOf, true);
				game.translations = translations;
				game.Variant = games[i].Variant;
				Console.WriteLine (" {0}", game.Name);
			}
		}

		void BuildPlayList (string [] names)
		{
			List <int> list = new List <int> ();
			GameLocator [] games;
			GameManager gm = new GameManager ();
			GtkClient.GameManagerPreload (gm);
			games = gm.AvailableGames;

			for (int i = 0; i < games.Length; i++)
			{
				Game game = (Game) Activator.CreateInstance (games[i].TypeOf, true);
				game.translations = translations;
				game.Variant = games[i].Variant;

				try
				{
					for (int n = 0; n < names.Length; n++)
					{
						if (String.Compare (game.Name, names [n], StringComparison.OrdinalIgnoreCase) == 0)
							list.Add (i);
					}

				}
				catch (Exception e)
				{
					Console.WriteLine ("CommandLine.BuildPlayList. Error adding {0} {1}", game.Name, e.Message);
				}
			}

			play_list = list.ToArray ();
		}

		static void Usage ()
		{
			StringBuilder str;

			str = new StringBuilder ();
			str.AppendLine (Catalog.GetString ("Usage: gbrainy [options]"));
			str.AppendLine (Catalog.GetString ("  --version\t\t\tPrint version information."));
			str.AppendLine (Catalog.GetString ("  --help\t\t\tPrint this usage message."));
			str.AppendLine (Catalog.GetString ("  --gamelist\t\t\tShows the list of available games."));
			str.AppendLine (Catalog.GetString ("  --customgame [game1, gameN]\tSpecifies a list of games to play during a custom game."));
			str.AppendLine (Catalog.GetString ("  --norandom \t\t\tThe custom game list provided will not be randomized."));
			str.AppendLine (Catalog.GetString ("  --versions \t\t\tShow dependencies."));

			Console.WriteLine (str.ToString ());
		}

		static void Versions ()
		{
			Console.WriteLine ("Mono .NET Version: " + Environment.Version.ToString());
			Console.WriteLine (String.Format("{0}Assembly Version Information:", Environment.NewLine));

			foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName name = asm.GetName();
				Console.WriteLine ("\t" + name.Name + " (" + name.Version.ToString () + ")");
			}
		}
	}
}
