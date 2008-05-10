/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;

public class GameManager
{
	static Type[] LogicPuzzles = new Type[] 
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
	};

	static Type[] CalculationTrainers = new Type[] 
	{
		typeof (CalculationArithmetical),
		typeof (CalculationGreatestDivisor),
		typeof (CalculationTwoNumbers),
		typeof (CalculationWhichNumber),
		typeof (CalculationOperator),
		typeof (CalculationFractions),
	};

	static Type[] MemoryTrainers = new Type[] 
	{
		typeof (MemoryColouredFigures),
		typeof (MemoryNumbers),
		typeof (MemoryColouredText),
		typeof (MemoryWords),
		typeof (MemoryCountDots),
		typeof (MemoryFigures),
		typeof (MemoryIndications),
	};

	private GameSession.Types game_type;
	private ArrayListIndicesRandom list;
	private IEnumerator enumerator;
	private List <Type> games;
	private Game.Difficulty difficulty;

	static GameManager ()
	{
		Console.WriteLine ("Games registered: {0}: {1} logic puzzles, {2} math trainers, {3} memory trainers", 
			LogicPuzzles.Length + CalculationTrainers.Length + MemoryTrainers.Length,
			LogicPuzzles.Length, CalculationTrainers.Length, MemoryTrainers.Length);
	}

	public GameManager ()
	{
		game_type = GameSession.Types.None;
		difficulty = Game.Difficulty.Medium;
		games = new List <Type> ();
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
			Type[] list = new Type [LogicPuzzles.Length + CalculationTrainers.Length + MemoryTrainers.Length];
			int idx = 0;

			for (int i = 0; i < LogicPuzzles.Length; i++, idx++)
				list[idx] = LogicPuzzles [i];

			for (int i = 0; i < CalculationTrainers.Length; i++, idx++)
				list[idx] = CalculationTrainers [i];

			for (int i = 0; i < MemoryTrainers.Length; i++, idx++)
				list[idx] = MemoryTrainers [i];

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

	private void BuildGameList ()
	{
		if (GameType == GameSession.Types.Custom)
			return;
		
		games.Clear ();
		Random random = new Random ();

		// For all games, 1/3 of the total are logic, 1/3 Memory, 1/3 calculation
		if ((game_type & GameSession.Types.AllGames) == GameSession.Types.AllGames) {
			
			int idx_cal = 0, idx_mem = 0;
			ArrayListIndicesRandom idx_logic = new ArrayListIndicesRandom (LogicPuzzles.Length);
			ArrayListIndicesRandom idx_memory = new ArrayListIndicesRandom (MemoryTrainers.Length);
			ArrayListIndicesRandom idx_calculation = new ArrayListIndicesRandom (CalculationTrainers.Length);

			games.Clear ();
			idx_memory.Initialize ();
			idx_logic.Initialize ();
			idx_calculation.Initialize ();

			for (int i = 0; i < LogicPuzzles.Length; i++, idx_mem++, idx_cal++) {

				if (idx_cal == CalculationTrainers.Length) {
					idx_cal = 0;
					idx_calculation.Initialize ();
				}

				if (idx_mem == MemoryTrainers.Length) {
					idx_mem = 0;
					idx_memory.Initialize ();
				}

				switch (random.Next (3)) {
				case 0:
					games.Add (CalculationTrainers [idx_calculation[idx_cal]]);
					games.Add (LogicPuzzles [idx_logic[i]]);
					games.Add (MemoryTrainers [idx_memory[idx_mem]]);
					break;
				case 1:
					games.Add (MemoryTrainers [idx_memory[idx_mem]]);
					games.Add (CalculationTrainers [idx_calculation[idx_cal]]);
					games.Add (LogicPuzzles [idx_logic[i]]);
					break;
				case 2:
					games.Add (CalculationTrainers [idx_calculation[idx_cal]]);
					games.Add (MemoryTrainers [idx_memory[idx_mem]]);
					games.Add (LogicPuzzles [idx_logic[i]]);
					break;
				}
			}
		} else {

			if ((game_type & GameSession.Types.LogicPuzzles) == GameSession.Types.LogicPuzzles) {
				for (int i = 0; i < LogicPuzzles.Length; i++)
					games.Add (LogicPuzzles [i]);
			}

			if ((game_type & GameSession.Types.CalculationTrainers) == GameSession.Types.CalculationTrainers) {
				for (int i = 0; i < CalculationTrainers.Length; i++)
					games.Add (CalculationTrainers [i]);
			}

			if ((game_type & GameSession.Types.MemoryTrainers) == GameSession.Types.MemoryTrainers) {
				for (int i = 0; i < MemoryTrainers.Length; i++)
					games.Add (MemoryTrainers [i]);
			}
		}

		list = new ArrayListIndicesRandom (games.Count);
		Initialize ();
	}

	private void Initialize ()
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
			//puzzle =  (Game) Activator.CreateInstance (LogicPuzzles [32], true);
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
		int width = 300, height = 400, margin = 20, x, y, cnt, games_page = 4;
		Game puzzle;
		game_type = GameSession.Types.AllGames;
		BuildGameList ();
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

