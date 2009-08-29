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
using System.Xml;
using System.IO;
using System.Collections.Generic;

using Cairo;
using Mono.Unix;
using Gtk;

public class VerbalAnalogies : Game
{
	public class Analogy
	{
		public string question;
		public string [] answers;
		public QuestionType type;
		public string tip;
		public string rationale;
		public int right;

		public Analogy ()
		{

		}
	}

	public enum QuestionType
	{
		Regular		= 0,	
		PairOfWords	= 1
	}

	static Dictionary <int, Analogy> analogies;
	Analogy current;

	public override string Name {
		get { return Catalog.GetString ("Verbal analogies");}
	}

	public override string Question {
		get {

			if (current == null)
				return String.Format (Catalog.GetString ("There was an error reading {0}. No verbal analogies available."), 
					Defines.DATA_DIR + Defines.VERBAL_ANALOGIES);

			switch (current.type) {
			case QuestionType.PairOfWords:
				return String.Format (Catalog.GetString (
					"Given pair of words '{0}', which of the possible answers has the closest in relationship to the given pair?"),
					current.question);
			default:
				return current.question;
			}
		}
	}

	public override string Answer {
		get {
			if (current == null)
				return string.Empty;

			if (String.IsNullOrEmpty (current.rationale) == false)
				return base.Answer + " " + current.rationale;

			return base.Answer;
		}
	}

	public override string Tip {
		get {
			if (current == null)
				return null;
			else
				return current.tip;
		}
	}

	public override Types Type {
		get { return Game.Types.VerbalAnalogy;}
	}

	public override void Initialize ()
	{
		if (analogies == null) {
			analogies = new Dictionary <int, Analogy> ();
			Read ();
			Console.WriteLine (Catalog.GetString ("Read {0} verbal analogies"), analogies.Count);
		}

		current = GetNext ();

		if (current == null)
			return;

		if (current.answers.Length <= 1)
			right_answer = current.answers [current.right];
		else
			right_answer = GetPossibleAnswer (current.right);
	}

	public Analogy GetNext ()
	{
		int idx;
		Analogy analogy;

		idx = random.Next (analogies.Count);
		
		try
		{
			analogies.TryGetValue (idx, out analogy);
		}

		catch (KeyNotFoundException)
		{
			analogy = null;
		}

		if (analogy!= null && analogy.answers != null) { // Randomize answers order

			ArrayListIndicesRandom indices;
			string [] answers;
			int new_right = 0;

			indices = new ArrayListIndicesRandom (analogy.answers.Length);
			answers = new string [analogy.answers.Length];

			indices.Initialize ();

			for (int i = 0; i < indices.Count; i++)
			{
				answers [i] = analogy.answers [indices[i]];
				if (indices[i] == analogy.right)
					new_right = i;
			}
			analogy.right = new_right;
			analogy.answers = answers;
		}
		
		return analogy;
	}

	public void Read ()
	{
		Analogy analogy;
		string name;
		int cnt = 0;
		List <string> answers;

		try 
		{
			StreamReader myStream = new StreamReader (Defines.DATA_DIR + Defines.VERBAL_ANALOGIES);
			XmlTextReader reader = new XmlTextReader (myStream);
			answers = new List <string> ();

			analogy = new Analogy ();
			while (reader.Read ())
			{
				name = reader.Name.ToLower ();
				switch (name) {
				case "analogy":
					if (reader.NodeType == XmlNodeType.Element) {
						analogy = new Analogy ();
						answers.Clear ();
					}
					else {
						if (reader.NodeType == XmlNodeType.EndElement) {
							analogy.answers = answers.ToArray ();
							analogies.Add (cnt++, analogy);
						}
					}
					break;
				case "_question":
					if (reader.NodeType != XmlNodeType.Element)
						return;

					string type;

					type = reader.GetAttribute ("type");
					Console.WriteLine ("Type:" + type);
		
					if (String.IsNullOrEmpty (type) == false) {
						switch (type.ToLower ()) {
						case "pairofwords":
							analogy.type = QuestionType.PairOfWords;
							break;
						default:
							analogy.type = QuestionType.Regular;
							break;
						}
					}
					analogy.question = reader.ReadElementString ();
					break;
				case "_tip":
					if (reader.NodeType == XmlNodeType.Element)
						analogy.tip = reader.ReadElementString ();

					break;
				case "_rationale":
					if (reader.NodeType == XmlNodeType.Element)
						analogy.rationale = reader.ReadElementString ();

					break;
				case "_answer":
					if (reader.NodeType != XmlNodeType.Element)
						break;
	
					string right;

					right = reader.GetAttribute ("correct");
		
					if (String.IsNullOrEmpty (right) == false)
						if (right.ToLower () == "yes")
							analogy.right = answers.Count;
					
					answers.Add (reader.ReadElementString ());
					break;
				}
			}
		}

		catch (Exception)
		{
			Console.WriteLine ("Error loading {0}", Defines.DATA_DIR + Defines.VERBAL_ANALOGIES);
		}
	}

	
	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		double x = DrawAreaX, y = DrawAreaY + 0.1;

		base.Draw (gr, area_width, area_height);

		if (current == null || current.answers == null || current.answers.Length <= 1)
			return;

		gr.SetPangoLargeFontSize ();

		gr.MoveTo (0.1, y);
		gr.ShowPangoText (Catalog.GetString ("Possible answers are:"));
		y += 0.12;

		x += 0.1;
		for (int n = 0; n < current.answers.Length; n++)
		{
			gr.MoveTo (x, y);
			gr.ShowPangoText (String.Format ("{0}) {1}", GetPossibleAnswer (n), current.answers[n].ToString ()));
			gr.Stroke ();
			y += 0.15;
		}	
	}

}


