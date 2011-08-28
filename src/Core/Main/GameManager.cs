/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Reflection;

using gbrainy.Core.Services;

#if MONO_ADDINS
using Mono.Addins;
using Mono.Addins.Setup;
#endif

using gbrainy.Core.Main.Verbal;
using gbrainy.Core.Main.Xml;

namespace gbrainy.Core.Main
{
	// Manages all the games available on the system
	public class GameManager
	{
		static Type[] VerbalAnalogiesInternal = new Type[]
		{
			typeof (AnalogiesQuestionAnswer),
			typeof (AnalogiesMultipleOptions),
			typeof (AnalogiesPairOfWordsOptions),
			typeof (AnalogiesPairOfWordsCompare),
		};

		List <GameLocator> available_games; 	// List of all available games in the system
		int cnt_logic, cnt_memory, cnt_calculation, cnt_verbal;
		static bool domain_load;

		public GameManager ()
		{
			available_games = new List <GameLocator> ();
			cnt_logic = cnt_memory = cnt_calculation = cnt_verbal = 0;
		}

		public GameTypes AvailableGameTypes {
			get {
				GameTypes types = GameTypes.None;

				if (cnt_logic > 0)
					types |= GameTypes.LogicPuzzle;

				if (cnt_calculation > 0)
					types |= GameTypes.Calculation;

				if (cnt_memory > 0)
					types |= GameTypes.Memory;

				if (cnt_verbal > 0)
					types |= GameTypes.VerbalAnalogy;

				return types;
			}
		}

		// Returns all the games available for playing
		public GameLocator [] AvailableGames {
			get { return available_games.ToArray (); }
		}

		// Gives the Assembly.Load used in GameManager the right path to load the application assemblies
		static Assembly ResolveAssemblyLoad (object sender, ResolveEventArgs args)
		{
			IConfiguration config = ServiceLocator.Instance.GetService <IConfiguration> ();
			string asm_dir = config.Get <string> (ConfigurationKeys.AssembliesDir);
        		string full_name = System.IO.Path.Combine (asm_dir, args.Name);
			return Assembly.LoadFile (full_name);
   		}

		// Dynamic load of the gbrainy.Games.Dll assembly
		public void LoadAssemblyGames (string file)
		{
			const string CLASS = "gbrainy.Games.GameList";
			const string LOGIC_METHOD = "LogicPuzzles";
			const string CALCULATION_METHOD = "Calculation";
			const string MEMORY_METHOD = "Memory";

			Assembly asem;
			Type type = null;
			PropertyInfo prop;
			object obj;

			try
			{
				if (domain_load == false)
				{
					AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler (ResolveAssemblyLoad);
					domain_load = true;
				}

				asem = Assembly.Load (file);

				foreach (Type t in asem.GetTypes())
				{
					if (t.FullName == CLASS)
					{
						type = t;
						break;
					}
				}

				obj = Activator.CreateInstance (type);

				prop = type.GetProperty (LOGIC_METHOD);
				if (prop != null)
					cnt_logic += AddGamesAndVariations ((Type []) prop.GetValue (obj, null));

				prop = type.GetProperty (MEMORY_METHOD);
				if (prop != null)
					cnt_memory += AddGamesAndVariations ((Type []) prop.GetValue (obj, null));

				prop = type.GetProperty (CALCULATION_METHOD);
				if (prop != null)
					cnt_calculation += AddGamesAndVariations ((Type []) prop.GetValue (obj, null));
			}

			catch (Exception e)
			{
				Console.WriteLine ("GameManager.LoadAssemblyGames. Could not load file {0}. Error {1}", file, e);
			}
		}

		// XML are stored using the Variant as a pointer to the game + the internal variant
		public void LoadGamesFromXml (string file)
		{
			// Load defined XML games
			GamesXmlFactory xml_games;

			xml_games = new GamesXmlFactory ();
			xml_games.Read (file);

			Type type = typeof (GameXml);
			int cnt = 0;

			foreach (GameXmlDefinition game in xml_games.Definitions)
			{
				// If the game has variants the game definition is used as reference
				// but only the variants are playable. The first variant is used as game (IsGame = true)
				available_games.Add (new GameLocator (type, cnt++, game.Type, true));

				switch (game.Type) {
				case GameTypes.LogicPuzzle:
					cnt_logic++;
					break;
				case GameTypes.Calculation:
					cnt_calculation++;
					break;
				default:
					break;
				}

				for (int i = 1; i < game.Variants.Count; i++)
				{
					available_games.Add (new GameLocator (type, cnt++, game.Type, false));
				}
			}
		}

		// Load Mono plugins
		public void LoadPlugins ()
		{

	#if MONO_ADDINS
			try {
				ExtensionNodeList addins;
				Game game;
				Type [] type = new Type [1];
				string dir = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "gbrainy");

				AddinManager.Initialize (dir);
				AddinManager.Registry.Update (null);
				new SetupService (AddinManager.Registry);

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/logic");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external logic game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/memory");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external memory game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/calculation");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external calculation game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}

				addins = AddinManager.GetExtensionNodes ("/gbrainy/games/verbal");
				foreach (TypeExtensionNode node in addins) {
					game = (Game) node.CreateInstance ();
					Console.WriteLine ("Loading external verbal analogy game: {0}", game);
					type [0] = game.GetType ();
					AddGamesAndVariations (type);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine ("GameManager.LoadPlugins. Error loading plugins. Error {0}", e);
			}
	#endif
		}

		// Load an XML file with analogies
		public void LoadVerbalAnalogies (string file)
		{
			AnalogiesFactory.Read (file);
			cnt_verbal += AddVerbalGamesAndVariations (VerbalAnalogiesInternal);
		}

		// Unload previous assembly, xml and verbal analogies loaded games
		public void ResetAvailableGames ()
		{
			available_games.Clear ();
		}

		public string GetGamesSummary (ITranslations translations)
		{
			String s = string.Empty;
	#if MONO_ADDINS
			s += translations.GetString ("Extensions database:") + " " +
					System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "gbrainy");

			s += Environment.NewLine;
	#endif
			// Translators: 'Games registered' is the games know to gbrainy (build-in and load from addins-in and external files)
			s += String.Format (translations.GetString ("{0} games registered: {1} logic puzzles, {2} calculation trainers, {3} memory trainers, {4} verbal analogies"),
					cnt_logic + cnt_memory + cnt_calculation + cnt_verbal,
					cnt_logic, cnt_calculation, cnt_memory, cnt_verbal);

			return s;
		}

		// Adds all the games and its variants into the available games list
		int AddGamesAndVariations (Type [] types)
		{
			Game game;
			int cnt = 0;

			foreach (Type type in types)
			{
				game = (Game) Activator.CreateInstance (type, true);
				for (int i = 0; i < game.Variants; i++)
				{
					available_games.Add (new GameLocator (type, i, game.Type, game.Variants == 1));
				}
				cnt += game.Variants;
			}
			return cnt;
		}

		// Adds all the games and its variants into the available games list
		int AddVerbalGamesAndVariations (Type [] types)
		{
			Game game;
			int cnt = 0;

			foreach (Type type in types)
			{
				game = (Game) Activator.CreateInstance (type, true);
				for (int i = 0; i < game.Variants; i++)
				{
					available_games.Add (new GameLocator (type, i, game.Type, true));
				}
				cnt += game.Variants;
			}
			return cnt;
		}
	}
}
