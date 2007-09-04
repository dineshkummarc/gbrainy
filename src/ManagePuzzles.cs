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

public class ManagePuzzles
{
	static Type[] PuzzlesTypes = new Type[] 
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
		typeof (MathTrainner)
	};

	private ArrayListIndicesRandom list;
	private IEnumerator enumerator;

	public ManagePuzzles ()
	{
		list = new ArrayListIndicesRandom (PuzzlesTypes.Length);
		Initialize ();
		Console.WriteLine ("Puzzles registered: {0}", PuzzlesTypes.Length);
	}

	private void Initialize ()
	{
		list.Initialize ();
		enumerator = list.GetEnumerator ();
	}
	
	public Game GetPuzzle ()
	{
		Game puzzle;
		if (enumerator.MoveNext () == false) { // All the games have been played, restart again 
			Console.WriteLine ("New games list");
			Initialize ();
			enumerator.MoveNext ();
		}

		//puzzle =  (Game) Activator.CreateInstance (PuzzlesTypes [(int) enumerator.Current], true);
		puzzle =  (Game) Activator.CreateInstance (PuzzlesTypes [14], true);
		puzzle.Initialize ();
		return puzzle;
	}
	
}


