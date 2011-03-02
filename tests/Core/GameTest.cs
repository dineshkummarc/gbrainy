/*
 * Copyright (C) 2010-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;

namespace gbrainyTest
{
	public class TestGame : Game
	{
		public TestGame ()
		{

		}

		public override string Question {
			get { return "Question"; }
		}

		public override string Name {
			get { return "TestGame"; }
		}

		public string PossibleAnswersExpression {
			get { return Answer.GetMultiOptionsExpression (); }
		}

		protected override void Initialize () {}
		
	}

	[TestFixture]
	public class GameTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
		}

		// Test individual attributes (MatchAll follows a different logic path)
		[Test]
		public void Trim ()
		{
			TestGame game = new TestGame ();

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.None;
			game.Answer.Correct = "icon";
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (false, game.CheckAnswer (" icon "));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.Trim;
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (true, game.CheckAnswer (" icon "));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (false, game.CheckAnswer (" icon "));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (true, game.CheckAnswer (" icon "));
		}

		[Test]
		public void IgnoreCase ()
		{
			TestGame game = new TestGame ();

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.None;
			game.Answer.Correct = "icon";
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (false, game.CheckAnswer ("ICON"));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreCase;
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (true, game.CheckAnswer ("ICON"));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (false, game.CheckAnswer ("ICON"));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreCase | GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, game.CheckAnswer ("icon"));
			Assert.AreEqual (true, game.CheckAnswer ("ICON"));
		}

		[Test]
		public void IgnoreSpaces ()
		{
			TestGame game = new TestGame ();

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.None;
			game.Answer.Correct = "10 pm";
			Assert.AreEqual (true, game.CheckAnswer ("10 pm"));
			Assert.AreEqual (false, game.CheckAnswer ("10pm"));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreSpaces;
			Assert.AreEqual (true, game.CheckAnswer ("10 pm"));
			Assert.AreEqual (true, game.CheckAnswer ("10pm"));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, game.CheckAnswer ("10 pm"));
			Assert.AreEqual (false, game.CheckAnswer ("10pm"));

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.IgnoreSpaces | GameAnswerCheckAttributes.MatchAll;
			Assert.AreEqual (true, game.CheckAnswer ("10 pm"));
			Assert.AreEqual (true, game.CheckAnswer ("10pm"));
		}

		[Test]
		public void MatchAllInOder ()
		{
			TestGame game = new TestGame ();

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.MatchAllInOrder;
			game.Answer.CheckExpression = "[0-9]+";
			game.Answer.Correct = "10 | 20 | 30";

			Assert.AreEqual (true, game.CheckAnswer ("10 20 30"));
			Assert.AreEqual (false, game.CheckAnswer ("30 20 10"));
		}

		[Test]
		public void MatchAll ()
		{
			TestGame game = new TestGame ();

			game.Answer.CheckAttributes = GameAnswerCheckAttributes.MatchAll;
			game.Answer.CheckExpression = "[0-9]+";
			game.Answer.Correct = "10 | 20 | 30";
			Assert.AreEqual (true, game.CheckAnswer ("10 20 30"));
			Assert.AreEqual (true, game.CheckAnswer ("30 20 10"));
		}

		// Test attributes as used in real games

		[Test]
		public void DefaultAnswer ()
		{
			TestGame game = new TestGame ();

			game.Answer.Correct = "icon";
			Assert.AreEqual (true, game.CheckAnswer ("icon"));

			game.Answer.Correct = "icona";
			Assert.AreEqual (true, game.CheckAnswer ("icona"));
		}

		[Test]
		public void DefaultAnswerOptions ()
		{
			TestGame game = new TestGame ();

			game.Answer.Correct = "option1 | option2";
			Assert.AreEqual (true, game.CheckAnswer ("option1"));
			Assert.AreEqual (true, game.CheckAnswer ("option2"));
			Assert.AreEqual (true, game.CheckAnswer (" option2 "));

			Assert.AreEqual (false, game.CheckAnswer ("option3"));
		}

		[Test]
		public void CheckPuzzleTimeNowAnswer ()
		{
			TestGame game = new TestGame ();
			game.Answer.Correct = "10 PM";
			game.Answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.IgnoreCase | GameAnswerCheckAttributes.IgnoreSpaces;

			Assert.AreEqual (true, game.CheckAnswer ("10 PM"));
			Assert.AreEqual (true, game.CheckAnswer ("10 pm"));
			Assert.AreEqual (true, game.CheckAnswer ("10pm"));
			Assert.AreEqual (true, game.CheckAnswer ("10Pm"));
			Assert.AreEqual (true, game.CheckAnswer (" 10Pm "));

			Assert.AreEqual (false, game.CheckAnswer ("10 P"));
			Assert.AreEqual (false, game.CheckAnswer ("10"));
		}

		[Test]
		public void CheckCalculationOperator ()
		{
			TestGame game = new TestGame ();
			game.Answer.Correct = "+ | -";
			game.Answer.CheckExpression = "[+*-/]";
			game.Answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.MatchAllInOrder;

			Assert.AreEqual (true, game.CheckAnswer ("+ i -"));
			Assert.AreEqual (true, game.CheckAnswer ("+ and -"));
			Assert.AreEqual (true, game.CheckAnswer ("+ -"));
			Assert.AreEqual (true, game.CheckAnswer ("+-"));

			Assert.AreEqual (false, game.CheckAnswer ("- +"));
		}

		[Test]
		public void CheckPuzzleBuildTriangle ()
		{
			TestGame game = new TestGame ();
	
			game.Answer.Correct = "A | B | C";
			game.Answer.CheckExpression = "[ABCDF]";
			game.Answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.IgnoreCase | GameAnswerCheckAttributes.MatchAll;

			Assert.AreEqual (true, game.CheckAnswer ("A B C"));
			Assert.AreEqual (true, game.CheckAnswer ("C B A"));
			Assert.AreEqual (true, game.CheckAnswer ("B C A"));
			Assert.AreEqual (true, game.CheckAnswer ("A B C"));
			Assert.AreEqual (true, game.CheckAnswer ("C A B"));
			Assert.AreEqual (true, game.CheckAnswer ("a b c"));

			Assert.AreEqual (false, game.CheckAnswer ("B C C"));
			Assert.AreEqual (false, game.CheckAnswer ("B C"));
			Assert.AreEqual (false, game.CheckAnswer ("BC"));
		}


		[Test]
		public void CheckPuzzlePercentage ()
		{
			TestGame game = new TestGame ();
	
			game.Answer.Correct = "10";
			game.Answer.CheckExpression = "[0-9]+";

			Assert.AreEqual (true, game.CheckAnswer ("10%"));
			Assert.AreEqual (true, game.CheckAnswer ("10 %"));
			Assert.AreEqual (true, game.CheckAnswer ("10"));

			game.Answer.Correct = "9";
			Assert.AreEqual (true, game.CheckAnswer ("9%"));
			Assert.AreEqual (true, game.CheckAnswer ("9 %"));
			Assert.AreEqual (true, game.CheckAnswer ("9"));
		}

		[Test]
		public void TwoNumbersAnswer ()
		{
			TestGame game = new TestGame ();
			game.Answer.Correct = "10 | 20";
			game.Answer.CheckExpression = "[0-9]+";
			game.Answer.CheckAttributes = GameAnswerCheckAttributes.Trim | GameAnswerCheckAttributes.MatchAll;

			// Right answers
			Assert.AreEqual (true, game.CheckAnswer ("10 and 20"));
			Assert.AreEqual (true, game.CheckAnswer ("10 i 20"));
			Assert.AreEqual (true, game.CheckAnswer ("10 y 20"));
			Assert.AreEqual (true, game.CheckAnswer ("10 20"));
			Assert.AreEqual (true, game.CheckAnswer (" 10 20 "));

			Assert.AreEqual (true, game.CheckAnswer ("20 and 10"));
			Assert.AreEqual (true, game.CheckAnswer ("20 i 10"));
			Assert.AreEqual (true, game.CheckAnswer ("20 y 10"));
			Assert.AreEqual (true, game.CheckAnswer ("20 10"));
			Assert.AreEqual (true, game.CheckAnswer (" 20 10 "));

			// Invalid answers
			Assert.AreEqual (false, game.CheckAnswer (" 10 30 "));
			Assert.AreEqual (false, game.CheckAnswer ("10"));
			Assert.AreEqual (false, game.CheckAnswer ("20"));
			Assert.AreEqual (false, game.CheckAnswer ("10 2"));
		}
	}
}
