/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.IO;
using System.Collections.Generic;

using gbrainy.Core.Main;
using gbrainy.Core.Main.Verbal;
using gbrainy.Core.Main.Xml;
using gbrainy.Core.Services;
using System.Reflection;

public class GamesStatistics
{
	public static void GameManagerPreload (GameManager gm)
	{
		gm.LoadAssemblyGames (Defines.GAME_ASSEMBLY);
		gm.LoadVerbalAnalogies (System.IO.Path.Combine (Defines.DATA_DIR, Defines.VERBAL_ANALOGIES));
		gm.LoadGamesFromXml (System.IO.Path.Combine (Defines.DATA_DIR, Defines.GAMES_FILE));
		gm.LoadPlugins ();
	}

	static void InitCoreLibraries ()
	{
		// Register services
		ServiceLocator.Instance.RegisterService <ITranslations> (new TranslationsCatalog ());
		ServiceLocator.Instance.RegisterService <IConfiguration> (new MemoryConfiguration ());

		// Configuration
		ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.GamesDefinitions, Defines.DATA_DIR);
		ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.GamesGraphics, Defines.DATA_DIR);
		ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.ThemesDir, Defines.DATA_DIR);

		string mono_path = Environment.GetEnvironmentVariable ("MONO_PATH");

		if (String.IsNullOrEmpty (mono_path))
			mono_path = ".";

		// Configuration
		ServiceLocator.Instance.GetService <IConfiguration> ().Set (ConfigurationKeys.AssembliesDir, mono_path);
	}

	static int question_answer, multiple_options, words_options, words_compare, games_xml;
	static int tip, rationale;

	//Lists the games without tip
	static void GetStatistics (GameManager gm)
	{
		question_answer = multiple_options = words_options = words_compare = games_xml = 0;
		tip = rationale = 0;

		GameManager.GameLocator [] games = gm.AvailableGames;

		foreach (GameManager.GameLocator locator in games)
		{
			Game game = (Game) Activator.CreateInstance (locator.TypeOf, true);
			game.Variant = locator.Variant;

			if (game as AnalogiesQuestionAnswer != null)
				question_answer++;
			else if (game as AnalogiesMultipleOptions != null)
				multiple_options++;
			else if (game as AnalogiesPairOfWordsOptions != null)
				words_options++;
			else if (game as AnalogiesPairOfWordsCompare != null)
				words_compare++;

			if (game as GameXml != null)
				games_xml++;

			game.Begin ();

			if (String.IsNullOrEmpty (game.TipString) == false)
			{
				tip++;
			}

			if (String.IsNullOrEmpty (game.Rationale) == false)
			{
				rationale++;
			}
		}

		int verbal = question_answer + multiple_options + words_options + words_compare;
		int nonverbal = games.Length - verbal;

		Console.WriteLine ("All Games (including variations) - " + games.Length);
		Console.WriteLine ("  Games (no-verbal): {0} ({1}%)", nonverbal, nonverbal * 100 / games.Length);
		Console.WriteLine ("    Types");
		Console.WriteLine ("      Harcoded: {0} ({1}%)", nonverbal - games_xml, (nonverbal - games_xml) * 100 / nonverbal);
		Console.WriteLine ("      Xml: {0} ({1}%)", games_xml, games_xml * 100 / nonverbal);
		 // In verbal analogies, tip does not make much sense
		Console.WriteLine ("    No tip: {0} ({1}%)", nonverbal - tip, (nonverbal - tip) * 100 / nonverbal);
		Console.WriteLine ("  Games (verbal): {0} ({1}%)", verbal, verbal * 100 / games.Length);
		Console.WriteLine ("  No rationale: {0} ({1}%)", (games.Length - rationale), (games.Length - rationale) * 100 / games.Length);
		Console.WriteLine ("");
		Console.WriteLine ("Verbal analogies");
		Console.WriteLine ("  Question answer {0}", question_answer);
		Console.WriteLine ("  Multiple options {0}", multiple_options);
		Console.WriteLine ("  Pair of words options {0}", words_options);
		Console.WriteLine ("  Pair of words compare {0} (evil)", words_compare);
	}

	static void Main (string[] args)
	{
		InitCoreLibraries ();

		GameSession session = new GameSession ();
		GameManagerPreload (session.GameManager);

		Console.WriteLine ("gbrainy {0} (built on {1})", Defines.VERSION, Defines.BUILD_TIME);
		Console.WriteLine (session.GameManager.GetGamesSummary ());

		Console.WriteLine ("");
		GetStatistics (session.GameManager);
	}
}

