/*
 * Copyright (C) 2007-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Collections;
using System.Collections.Generic;
using Mono.Unix;

#if MONO_ADDINS
using Mono.Addins;
using Mono.Addins.Setup;
#endif


public class GameManager
{
	static Type[] LogicPuzzlesInternal = new Type[] 
	{
		typeof (PuzzleMatrixNumbers),
		typeof (PuzzleSquares),
		typeof (PuzzleFigures),
		typeof (PuzzleMoveFigure),
		typeof (PuzzleCirclesRectangle),
		typeof (PuzzlePencil),
		typeof (PuzzleTriangles),
		typeof (PuzzleCoverPercentage),
		typeof (PuzzleNumericSequence),
		typeof (PuzzleSquaresAndLetters),
		typeof (PuzzleSquareDots),
		typeof (PuzzleNumericRelation),
		typeof (PuzzleNextFigure),
		typeof (PuzzleSquareSheets),
		typeof (PuzzleCube),
		typeof (PuzzleFigureLetter),
		typeof (PuzzleDivideCircle),
		typeof (PuzzleMatrixGroups),
		typeof (PuzzleBalance),
		typeof (PuzzleTrianglesWithNumbers),
		typeof (PuzzleOstracism),
		typeof (PuzzleFigurePattern),
		typeof (PuzzlePeopleTable),
		typeof (PuzzleMissingSlice),
		typeof (PuzzleLines),
		typeof (PuzzleTetris),
		typeof (PuzzleMissingPiece),
		typeof (PuzzleMostInCommon),
		typeof (PuzzleBuildTriangle),
		typeof (PuzzleClocks),
		typeof (PuzzleCountCircles),
		typeof (PuzzleEquation),
		typeof (PuzzleQuadrilaterals),
		typeof (PuzzleExtraCircle),
		typeof (PuzzleCountSeries),
		typeof (PuzzleFourSided),
		typeof (PuzzleLargerShape),
		typeof (PuzzleHandshakes),
		typeof (PuzzleCounting),
	};

	static Type[] CalculationTrainersInternal = new Type[] 
	{
		typeof (CalculationArithmetical),
		typeof (CalculationGreatestDivisor),
		typeof (CalculationTwoNumbers),
		typeof (CalculationWhichNumber),
		typeof (CalculationOperator),
		typeof (CalculationFractions),
		typeof (CalculationPrimes),
	};

	static Type[] MemoryTrainersInternal = new Type[] 
	{
		typeof (MemoryColouredFigures),
		typeof (MemoryFiguresNumbers),
		typeof (MemoryColouredText),
		typeof (MemoryWords),
		typeof (MemoryCountDots),
		typeof (MemoryFigures),
		typeof (MemoryIndications),
		typeof (MemoryNumbers),
		typeof (MemoryFacts),
	};

	static Type[] VerbalAnalogiesInternal = new Type[] 
	{
		typeof (AnalogiesQuestionAnswer),
		typeof (AnalogiesMultipleOptions),
		typeof (AnalogiesPairOfWordsOptions),
		typeof (AnalogiesPairOfWordsCompare),
	};

	bool once;
	GameSession.Types game_type;
	ArrayListIndicesRandom list;
	IEnumerator enumerator;
	List <Type> games;
	Game.Difficulty difficulty;
	List <Type> LogicPuzzles;
	List <Type> CalculationTrainers;
	List <Type> MemoryTrainers;
	List <Type> VerbalAnalogies;
	
	public GameManager ()
	{
		game_type = GameSession.Types.None;
		difficulty = Game.Difficulty.Medium;
		games = new List <Type> ();
		LogicPuzzles = new List <Type> (LogicPuzzlesInternal);
		CalculationTrainers = new List <Type> (CalculationTrainersInternal);
		MemoryTrainers = new List <Type> (MemoryTrainersInternal);
		VerbalAnalogies = new List <Type> (VerbalAnalogiesInternal);
		LoadPlugins ();

		if (once == false) {
			once = true;
			Console.WriteLine (Catalog.GetString ("Games registered: {0}: {1} logic puzzles, {2} calculation trainers, {3} memory trainers, {4} verbal analogies"), 
				LogicPuzzles.Count + CalculationTrainers.Count + MemoryTrainers.Count + VerbalAnalogies.Count,
				LogicPuzzles.Count, CalculationTrainers.Count, MemoryTrainers.Count, VerbalAnalogies.Count);
		}
		//GeneratePDF ();
	}

	public GameSession.Types GameType {
		get {return game_type; }
		set {
			if (game_type == value)
				return;
			
			game_type = value;
			BuildGameList ();
		}
	}

	public Game.Difficulty Difficulty {
		set {
			difficulty = value;
			BuildGameList ();
		}
		get {
			return difficulty;
		}
	}

	// Used from CustomGameDialog only
	public Type[] CustomGames {
		get { 
			Type[] list = new Type [LogicPuzzles.Count + CalculationTrainers.Count + MemoryTrainers.Count + VerbalAnalogies.Count];
			int idx = 0;

			for (int i = 0; i < LogicPuzzles.Count; i++, idx++)
				list[idx] = LogicPuzzles [i];

			for (int i = 0; i < CalculationTrainers.Count; i++, idx++)
				list[idx] = CalculationTrainers [i];

			for (int i = 0; i < MemoryTrainers.Count; i++, idx++)
				list[idx] = MemoryTrainers [i];

			for (int i = 0; i < VerbalAnalogies.Count; i++, idx++)
				list[idx] = VerbalAnalogies [i];

			return list;
		}
		set {
			games = new List <Type> (value.Length);
			for (int i = 0; i < value.Length; i++)
				games.Add (value[i]);

			list = new ArrayListIndicesRandom (games.Count);
			Initialize ();
		}
	}

	void LoadPlugins ()
	{

#if MONO_ADDINS
		try {
			ExtensionNodeList addins;
			Game game;
			string dir = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "gbrainy");
		
			AddinManager.Initialize (dir);
			Console.WriteLine ("Pluggin database:" + dir);
			AddinManager.Registry.Update (null);
			new SetupService (AddinManager.Registry);

			addins = AddinManager.GetExtensionNodes ("/gbrainy/games/logic");
			foreach (TypeExtensionNode node in addins) {
				game = (Game) node.CreateInstance ();
				Console.WriteLine ("Loading external logic game: {0}", game);
				LogicPuzzles.Add (game.GetType ());
			}
		
			addins = AddinManager.GetExtensionNodes ("/gbrainy/games/memory");
			foreach (TypeExtensionNode node in addins) {
				game = (Game) node.CreateInstance ();
				Console.WriteLine ("Loading external memory game: {0}", game);
				MemoryTrainers.Add (game.GetType ());
			}

			addins = AddinManager.GetExtensionNodes ("/gbrainy/games/calculation");
			foreach (TypeExtensionNode node in addins) {
				game = (Game) node.CreateInstance ();
				Console.WriteLine ("Loading external calculation game: {0}", game);
				CalculationTrainers.Add (game.GetType ());
			}

			addins = AddinManager.GetExtensionNodes ("/gbrainy/games/verbal");
			foreach (TypeExtensionNode node in addins) {
				game = (Game) node.CreateInstance ();
				Console.WriteLine ("Loading external verbal analogy game: {0}", game);
				VerbalAnalogies.Add (game.GetType ());
			}
		}
		catch (Exception e)
		{
			Console.WriteLine (String.Format ("Exception {0} when loading the plugins", e));
		}
#endif
	}


	void BuildGameList ()
	{
		if (GameType == GameSession.Types.Custom)
			return;
		
		games.Clear ();
		Random random = new Random ();

		// For all games, 1/4 of the total are logic, 1/4 Memory, 1/4 calculation, 1/4 verbal analogies
		if ((game_type & GameSession.Types.AllGames) == GameSession.Types.AllGames) {
			
			int idx_cal = 0, idx_mem = 0, idx_verb = 0;
			ArrayListIndicesRandom idx_logic = new ArrayListIndicesRandom (LogicPuzzles.Count);
			ArrayListIndicesRandom idx_memory = new ArrayListIndicesRandom (MemoryTrainers.Count);
			ArrayListIndicesRandom idx_calculation = new ArrayListIndicesRandom (CalculationTrainers.Count);
			ArrayListIndicesRandom idx_verbal = new ArrayListIndicesRandom (VerbalAnalogies.Count);

			games.Clear ();
			idx_memory.Initialize ();
			idx_logic.Initialize ();
			idx_calculation.Initialize ();
			idx_verbal.Initialize ();

			for (int i = 0; i < LogicPuzzles.Count; i++, idx_mem++, idx_cal++, idx_verb++) {

				if (idx_cal == CalculationTrainers.Count) {
					idx_cal = 0;
					idx_calculation.Initialize ();
				}

				if (idx_mem == MemoryTrainers.Count) {
					idx_mem = 0;
					idx_memory.Initialize ();
				}

				if (idx_verb == VerbalAnalogies.Count) {
					idx_verb = 0;
					idx_verbal.Initialize ();
				}

				switch (random.Next (3)) {
				case 0:
					games.Add (CalculationTrainers [idx_calculation[idx_cal]]);
					games.Add (LogicPuzzles [idx_logic[i]]);
					games.Add (MemoryTrainers [idx_memory[idx_mem]]);
					games.Add (VerbalAnalogies [idx_verbal[idx_verb]]);
					break;
				case 1:
					games.Add (MemoryTrainers [idx_memory[idx_mem]]);
					games.Add (CalculationTrainers [idx_calculation[idx_cal]]);
					games.Add (VerbalAnalogies [idx_verbal[idx_verb]]);
					games.Add (LogicPuzzles [idx_logic[i]]);
					break;
				case 2:
					games.Add (CalculationTrainers [idx_calculation[idx_cal]]);
					games.Add (VerbalAnalogies [idx_verbal[idx_verb]]);
					games.Add (MemoryTrainers [idx_memory[idx_mem]]);
					games.Add (LogicPuzzles [idx_logic[i]]);
					break;
				}
			}
		} else {

			if ((game_type & GameSession.Types.LogicPuzzles) == GameSession.Types.LogicPuzzles) {
				for (int i = 0; i < LogicPuzzles.Count; i++)
					games.Add (LogicPuzzles [i]);
			}

			if ((game_type & GameSession.Types.CalculationTrainers) == GameSession.Types.CalculationTrainers) {
				for (int i = 0; i < CalculationTrainers.Count; i++)
					games.Add (CalculationTrainers [i]);
			}

			if ((game_type & GameSession.Types.MemoryTrainers) == GameSession.Types.MemoryTrainers) {
				for (int i = 0; i < MemoryTrainers.Count; i++)
					games.Add (MemoryTrainers [i]);
			}

			if ((game_type & GameSession.Types.VerbalAnalogies) == GameSession.Types.VerbalAnalogies) {
				for (int i = 0; i < VerbalAnalogies.Count; i++)
					games.Add (VerbalAnalogies [i]);
			}

		}

		list = new ArrayListIndicesRandom (games.Count);
		Initialize ();
	}

	void Initialize ()
	{
		if ((game_type & GameSession.Types.AllGames) == GameSession.Types.AllGames) { // The game list has been already randomized
			list.Clear ();
			for (int i = 0; i < games.Count; i++)
				list.Add (i);
		} else
			list.Initialize ();

		enumerator = list.GetEnumerator ();
	}
	
	public Game GetPuzzle (gbrainy app)
	{
		Game puzzle, first = null;

		while (true) {
			if (enumerator.MoveNext () == false) { // All the games have been played, restart again 
				Initialize ();
				enumerator.MoveNext ();
			}

			puzzle =  (Game) Activator.CreateInstance ((Type) games [(int) enumerator.Current], true);
			//puzzle =  (Game) Activator.CreateInstance (LogicPuzzles [37], true);
			if (first != null && first.GetType () == puzzle.GetType ())
				break;
				
			if (first == null)
				first = puzzle;

			if ((puzzle.GameDifficulty & difficulty) == difficulty)
				break;
		}

		puzzle.App = app;
		puzzle.CurrentDifficulty = Difficulty;
		puzzle.Initialize ();
		return puzzle;
	}

#if _PDF_
	// Generates a single PDF document with all the puzzles contained in gbrainy (4 games per page)
	public void GeneratePDF ()
	{
		int width = 400, height = 400, margin = 20, x, y, cnt, games_page = 4;
		Game puzzle;
		game_type = GameSession.Types.AllGames;
		Type [] allgames = CustomGames;
		
		for (int i = 0; i < allgames.Length; i++)
			games.Add (allgames [i]);

		PdfSurface pdf = new PdfSurface ("games.pdf", (width + margin) * 2, (height + margin) * games_page / 2);
		x = y = cnt = 0;
		CairoContextEx cr = new CairoContextEx (pdf);
		for (int game = 0; game < games.Count; game++)
		{
			puzzle =  (Game) Activator.CreateInstance ((Type) games [game], true);
			puzzle.Initialize ();
			cnt++;
			cr.Save ();
			cr.Translate (x, y);
			cr.Rectangle (0, 0, width, height);;	
			cr.Clip ();
			cr.Save ();
			puzzle.DrawPreview (cr, width, height);
			x += width + margin;
			if (x > width + margin) {
				x = 0;
				y += height + margin;
			}
			cr.Restore ();
			cr.MoveTo (50,  height - 10);
			cr.ShowText (String.Format ("Game: {0} / D:{1}", puzzle.Name, puzzle.GameDifficulty));
			cr.Stroke ();
			cr.Restore ();

			if (cnt >= games_page) {
				cr.ShowPage ();
				cnt = x = y = 0;
			}
		}
		pdf.Finish ();
		((IDisposable)cr).Dispose();
		return;
	}
#endif
}

