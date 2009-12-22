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

using gbrainy.Games.Calculation;
using gbrainy.Games.Logic;
using gbrainy.Games.Memory;

namespace gbrainy.Games
{
	public class GameList
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
			typeof (PuzzlePercentage),
			typeof (PuzzleTimeNow),
			typeof (Puzzle3DCube),
		};

		static Type[] CalculationTrainersInternal = new Type[] 
		{
			typeof (CalculationArithmetical),
			typeof (CalculationGreatestDivisor),
			typeof (CalculationTwoNumbers),
			typeof (CalculationCloserFraction),
			typeof (CalculationOperator),
			typeof (CalculationFractions),
			typeof (CalculationPrimes),
			typeof (CalculationAverage),
			typeof (CalculationProportions),
			typeof (CalculationRatio),
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
			typeof (MemoryFiguresAndText),
		};

		public static Type [] LogicPuzzles
		{
			get {
				return LogicPuzzlesInternal;
			}
		}

		public static Type [] CalculationTrainers
		{
			get {
				return CalculationTrainersInternal;
			}
		}

		public static Type [] MemoryTrainers
		{
			get {
				return MemoryTrainersInternal;
			}
		}
	}
}
