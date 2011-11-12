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
using System.Collections.Generic;
using NUnit.Framework;

using gbrainy.Core.Main;
using gbrainy.Core.Services;
using gbrainy.Core.Libraries;

namespace gbrainy.Test.Core
{
	[TestFixture]
	public class GameSessionTest : UnitTestSupport
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			RegisterDefaultServices ();
		}
		
		GameSession PrepareSession ()
		{
			ITranslations translations = new TranslationsCatalog ();
			GameSession session = new GameSession (translations);
			session.GameManager.LoadAssemblyGames ("gbrainy.Games.dll");
			return session;
		}

		[Test]
		public void Status ()
		{
			GameSession session = PrepareSession ();
			Assert.AreEqual (GameSession.SessionStatus.NotPlaying, session.Status);

			session.Type = GameSession.Types.LogicPuzzles;
			session.New ();
			Assert.AreEqual (GameSession.SessionStatus.NotPlaying, session.Status);

			session.NextGame ();
			Assert.AreEqual (GameSession.SessionStatus.Playing, session.Status);

			session.End ();
			Assert.AreEqual (GameSession.SessionStatus.Finished, session.Status);
		}

		[Test]
		public void Paused ()
		{
			GameSession session = PrepareSession ();
			session.Type = GameSession.Types.LogicPuzzles;
			session.New ();
			Assert.AreEqual (GameSession.SessionStatus.NotPlaying, session.Status);

			session.Pause ();
			Assert.AreEqual (true, session.Paused);

			session.Resume ();
			Assert.AreEqual (false, session.Paused);
		}

		[Test]
		public void ID ()
		{
			GameSession session = PrepareSession ();
			session.Type = GameSession.Types.LogicPuzzles;
			session.New ();
			Assert.AreEqual (1, session.ID);
			session.End ();

			session.New ();
			Assert.AreEqual (2, session.ID);
		}
	}
}
