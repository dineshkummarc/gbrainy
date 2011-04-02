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
using NUnit.Framework;

using gbrainy.Games.Logic;

namespace gbrainy.Test.Games.Logic
{
	[TestFixture]
	public class PuzzleNumericRelationTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
		}

		[Test]
		public void ValidateAllAdding ()
		{
			int [] sequence;
			bool validates;

			// Invalid Formula.AllAdding sequence
			// since it validates 6 + 9 + 15 = 30, 1 + 6 + 23, 4 + 11 + 15 = 30 (Adding the numbers)
			// but (ThirdSubstracting) also 6 + 9 = 15, 4 + 11 = 15
			sequence = new [] {6, 9, 15, 1, 6, 23, 4, 11, 15};
			validates = PuzzleNumericRelation.Validate (sequence, PuzzleNumericRelation.Formula.AllAdding, 4);
			Assert.AreEqual (false, validates);

			// Valid Formula.AllAdding sequence
			sequence = new [] {6, 9, 14, 1, 5, 23, 4, 11, 14};
			validates = PuzzleNumericRelation.Validate (sequence, PuzzleNumericRelation.Formula.AllAdding, 4);
			Assert.AreEqual (true, validates);
		}

		[Test]
		public void ValidateThirdSubstracting ()
		{
			int [] sequence;
			bool validates;

			// Invalid Formula.ThirdSubstracting sequence
			// since it validates 7 + 10 = -3 but (ThirdSubstracting) also 7 + 10 -3 = 7 + 2 + 5 (adding the numbers)
			sequence = new [] {7, 10, -3, 6, 8, -2, 7, 2, 5};
			validates = PuzzleNumericRelation.Validate (sequence, PuzzleNumericRelation.Formula.ThirdSubstracting, 5);
			Assert.AreEqual (false, validates);

			// Valid Formula.ThirdSubstracting sequence
			sequence = new [] {7, 10, -3, 6, 8, -2, 7, 2, 6};
			validates = PuzzleNumericRelation.Validate (sequence, PuzzleNumericRelation.Formula.ThirdSubstracting, 5);
			Assert.AreEqual (true, validates);
		}
	}
}

