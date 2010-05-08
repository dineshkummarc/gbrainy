using System;
using System.Collections.Generic;
using NUnit.Framework;

using gbrainy.Core.Main.Verbal;

namespace gbrainyTest
{
	[TestFixture]
	public class AnalogiesFactoryTest
	{
		[TestFixtureSetUp]
		public void Construct ()
		{
			AnalogiesFactory.Read ("test_analogies.xml");
		}

		[Test]
		public void MultipleOptionsWithIngore ()
		{
			Dictionary <int, Analogy> analogies;

			// Checks also the <ignore> parameter
			analogies = AnalogiesFactory.Get (Analogy.Type.MultipleOptions);
			Assert.AreEqual (2, analogies.Count);
		}

		[Test]
		public void PairOfWordsOptions ()
		{
			Dictionary <int, Analogy> analogies;

			analogies = AnalogiesFactory.Get (Analogy.Type.PairOfWordsOptions);
			Assert.AreEqual (1, analogies.Count);
		}

		[Test]
		public void QuestionAnswer ()
		{
			Dictionary <int, Analogy> analogies;

			analogies = AnalogiesFactory.Get (Analogy.Type.QuestionAnswer);
			Assert.AreEqual (1, analogies.Count);
		}

		[Test]
		public void PairOfWordsCompare ()
		{
			Dictionary <int, Analogy> analogies;

			analogies = AnalogiesFactory.Get (Analogy.Type.PairOfWordsCompare);
			Assert.AreEqual (2, analogies.Count);
		}
	}
}
