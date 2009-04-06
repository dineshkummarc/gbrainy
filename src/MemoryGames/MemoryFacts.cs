/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;
using Mono.Unix;
using System.Collections.Generic;

public class MemoryFacts : Memory
{
	const int total_questions = 4;
	string question;
	Fact[] facts;

	struct Fact
	{
		public string fact;
		public string[] questions;
		public int[] answers;
		public int Length;
	
		public void Initialize (int items)
		{
			questions = new string [items];
			answers = new int [items];
			Length = items;
		}
	}

	public override string Name {
		get {return Catalog.GetString ("Memorize facts");}
	}

	public override string MemoryQuestion {
		get { return question;}
	}

	public override void Initialize ()
	{
		int fact_idx, quest_idx, questions;
		ArrayListIndicesRandom indices;

		switch (CurrentDifficulty) {
		case Difficulty.Easy:
			questions = 1;
			break;
		default:
		case Difficulty.Medium:
			questions = 2;
			break;
		case Difficulty.Master:
			questions = 3;
			break;
		}

		indices = new ArrayListIndicesRandom (total_questions);
		indices.Initialize ();

		facts = new Fact [questions];
		base.Initialize ();

		for (int i = 0; i < facts.Length; i++)
			facts[i] = GetFact (indices[i]);

		fact_idx = random.Next (questions);
		quest_idx = random.Next (facts [fact_idx].Length);
		question = facts [fact_idx].questions [quest_idx];
		right_answer = (facts [fact_idx].answers [quest_idx]).ToString ();

		// Since this particular test requires to read and understand text
		// lets give the user twice time to be able to understand the text properly
		TotalTime = TotalTime * 2;
	}

	Fact GetFact (int index)
	{
		Fact fact = new Fact ();

		switch (index) {
		case 0:
			fact.Initialize (2);
			fact.answers [0] = 2 + random.Next (14);
			fact.answers [1] = 1914 + random.Next (50);
			fact.fact = String.Format (
				// Translators: {0} is replaced by a number, {1} by a year (like 1940)
				Catalog.GetString ("Shiny Cars had already announced a {0} days production halt next month, but before that it had not cut production since {1}."),
				fact.answers [0], fact.answers [1]);
			fact.questions [0] = Catalog.GetString ("For how many days did Shiny Cars halt its production?");
			fact.questions [1] = Catalog.GetString ("In what year did Shiny Cars last halt its production?");
			break;
		case 1:
			fact.Initialize (2);
			fact.answers [0] = 10 + random.Next (30);
			fact.answers [1] = 1914 + random.Next (50);
			fact.fact = String.Format (
				// Translators: {0} is replaced by a number, {1} by a year (like 1940)
				Catalog.GetString ("Shiny Cars sales fell {0}% this past December, the worse decline since {1}."),
				fact.answers [0], fact.answers [1]);
			fact.questions [0] = Catalog.GetString ("By how much did company sales fall last December?");
			fact.questions [1] = Catalog.GetString ("In what year did Shiny Cars record a sales total lower than that of last December?");
			break;
		case 2:
			fact.Initialize (1);
			fact.answers [0] = 10 + random.Next (30);
			fact.fact = String.Format (Catalog.GetString ("About {0}% of Shiny Cars produced worldwide are sold in Europe"),
				fact.answers [0]);
			fact.questions [0] = Catalog.GetString ("What percentage of all Shiny Cars produced worldwide are sold in Europe?");
			break;
		case 3:
			fact.Initialize (2);
			fact.answers [0] = 10 + random.Next (30);
			fact.answers [1] = 100 - (1 + random.Next (10)) - fact.answers [0];
			fact.fact = String.Format (Catalog.GetString ("About {0}% of Shiny Cars use diesel, {1}% use gasoline and the remainder use electric."),
				fact.answers [0], fact.answers [1]);
			fact.questions [0] = Catalog.GetString ("What percentage of Shiny Cars use diesel?");
			fact.questions [1] = Catalog.GetString ("What percentage of Shiny Cars use gasoline?");
			break;
		default:
			throw new Exception ("Invalid index value");
		}

		return fact;
	}

	public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height)
	{

	}
	
	public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height)
	{
		string text = string.Empty;

		base.DrawObjectToMemorize (gr, area_width, area_height);
	
		for (int i = 0; i < facts.Length; i++)
		{
			text += facts[i].fact;
			text += "\n\n";
		}
		gr.DrawStringWithWrapping (0.3, DrawAreaY + 0.2, text);
	}

	public override bool CheckAnswer (string answer)
	{	
		if (String.Compare (answer, right_answer, true) == 0) 
			return true;

		if (String.Compare (answer, right_answer + "%", true) == 0) 
			return true;

		return false;
	}
}
