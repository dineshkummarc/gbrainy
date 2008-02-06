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
		typeof (PuzzleAlphabeticSequence),
		typeof (PuzzleSquareDots),
		typeof (PuzzleNumericRelation),
		typeof (PuzzleNextFigure),
		typeof (PuzzleSquareSheets),
		typeof (PuzzleCube),
		typeof (PuzzleFigureLetter),
		typeof (PuzzleDivideCircle),
		typeof (PuzzleMatrixGroups),
		typeof (PuzzleBalance),
		typeof (PuzzlePairs),
		typeof (PuzzleOstracism),
		typeof (PuzzleFigurePattern),
		typeof (PuzzlePeopleTable),
		typeof (PuzzleMissingSlice),
		typeof (PuzzleLines),
		typeof (PuzzleTetris),
		typeof (PuzzleMissingPiece),
		typeof (PuzzleMostInCommon)
	};

	static Type[] MathTrainers = new Type[] 
	{
		typeof (MathArithmetical),
		typeof (MathGreaterDivisor),
		typeof (MathTwoNumbers),
		typeof (MathWhichNumber),
		typeof (MathOperator),
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
	private ArrayList games;
	private Game.Difficulty difficulty;

	static GameManager ()
	{
		Console.WriteLine ("Games registered: {0}: {1} logic puzzles, {2} math trainers, {3} memory trainers", 
			LogicPuzzles.Length + MathTrainers.Length + MemoryTrainers.Length,
			LogicPuzzles.Length, MathTrainers.Length, MemoryTrainers.Length);
	}

	public GameManager ()
	{
		game_type = GameSession.Types.None;
		difficulty = Game.Difficulty.Medium;
		games = new ArrayList ();
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
	public Type[] Games {
		get { return (Type []) games.ToArray (typeof (Type)); }
		set {
			games = new ArrayList (value.Length);
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

		if ((game_type & GameSession.Types.LogicPuzzles) == GameSession.Types.LogicPuzzles) {
			for (int i = 0; i < LogicPuzzles.Length; i++)
					games.Add (LogicPuzzles [i]);
		}

		if ((game_type & GameSession.Types.MathTrainers) == GameSession.Types.MathTrainers) {
			for (int i = 0; i < MathTrainers.Length; i++)
				games.Add (MathTrainers [i]);
		}

		if ((game_type & GameSession.Types.MemoryTrainers) == GameSession.Types.MemoryTrainers) {
			for (int i = 0; i < MemoryTrainers.Length; i++)
				games.Add (MemoryTrainers [i]);
		}

		list = new ArrayListIndicesRandom (games.Count);
		Initialize ();
	}

	private void Initialize ()
	{
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
			//puzzle =  (Game) Activator.CreateInstance (MemoryTrainers [2], true);
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
	
}


