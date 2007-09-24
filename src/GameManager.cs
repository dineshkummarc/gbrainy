/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	};

	static Type[] MathTrainers = new Type[] 
	{
		typeof (MathArithmetical),
	};

	static Type[] MemoryTrainers = new Type[] 
	{
		typeof (MemoryColouredFigures),
		typeof (MemoryNumbers),
		typeof (MemoryColouredText),
		typeof (MemoryWords),
	};

	private GameSession.Types game_type;
	private ArrayListIndicesRandom list;
	private IEnumerator enumerator;
	private Type[] games;

	public GameManager ()
	{
		game_type = GameSession.Types.None;
		Console.WriteLine ("Games registered: {0}: {1} logic puzzles, {2} math trainers, {3} memory trainers", 
			LogicPuzzles.Length + MathTrainers.Length + MemoryTrainers.Length,
			LogicPuzzles.Length, MathTrainers.Length, MemoryTrainers.Length);
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

	public Type[] Games {
		get { return games; }
		set { 
			games = value; 
			list = new ArrayListIndicesRandom (games.Length);
			Initialize ();
		}
	}

	private void BuildGameList ()
	{
		int cnt = 0, index = 0;

		if (GameType == GameSession.Types.Custom)
			return;

		if ((game_type & GameSession.Types.LogicPuzzles) == GameSession.Types.LogicPuzzles)
			cnt += LogicPuzzles.Length;

		if ((game_type & GameSession.Types.MathTrainers) == GameSession.Types.MathTrainers)
			cnt += MathTrainers.Length;

		if ((game_type & GameSession.Types.MemoryTrainers) == GameSession.Types.MemoryTrainers)
			cnt += MemoryTrainers.Length;
		
		games = new Type [cnt];

		if ((game_type & GameSession.Types.LogicPuzzles) == GameSession.Types.LogicPuzzles) {
			for (int i = 0; i < LogicPuzzles.Length; i++, index++)
				games[index] = LogicPuzzles [i];
		}

		if ((game_type & GameSession.Types.MathTrainers) == GameSession.Types.MathTrainers) {
			for (int i = 0; i < MathTrainers.Length; i++, index++)
				games[index] = MathTrainers [i];
		}

		if ((game_type & GameSession.Types.MemoryTrainers) == GameSession.Types.MemoryTrainers) {
			for (int i = 0; i < MemoryTrainers.Length; i++, index++)
				games[index] = MemoryTrainers [i];
		}

		list = new ArrayListIndicesRandom (cnt);
		Initialize ();
	}

	private void Initialize ()
	{
		list.Initialize ();
		enumerator = list.GetEnumerator ();
	}
	
	public Game GetPuzzle (gbrainy app)
	{
		Game puzzle;
		if (enumerator.MoveNext () == false) { // All the games have been played, restart again 
			Console.WriteLine ("New games list");
			Initialize ();
			enumerator.MoveNext ();
		}

		puzzle =  (Game) Activator.CreateInstance (games [(int) enumerator.Current], true);
		//puzzle =  (Game) Activator.CreateInstance (MemoryTrainers [2], true);
		puzzle.App = app;
		puzzle.Initialize ();
		return puzzle;
	}
	
}


