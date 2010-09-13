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
using System.Text;
using System.Collections.Generic;

namespace gbrainy.Core.Main.Xml
{
	public class LocalizableString
	{
		public string String { get; set; }
		public string Value { get; set; }
		public int ValueComputed { get; set; }
		public string PluralString { get; set; }

		public bool IsPlural ()
		{
			if (String.IsNullOrEmpty (String) == true ||
				String.IsNullOrEmpty (PluralString) == true ||
				String.IsNullOrEmpty (Value) == true)
				return false;
			else
				return true;
		}
	}

	public class DrawingObject
	{

	}

	public class ImageDrawingObject : DrawingObject
	{
		public string Filename { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
	};

	public class TextDrawingObject : DrawingObject
	{
		public enum Sizes
		{
			Small,
			Medium,
			Large,
			XLarge,
			XXLarge,
		}

		public string Text { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public bool Centered { get; set; }
		public Sizes Size { get; set; }

		public TextDrawingObject ()
		{
			Size = Sizes.Medium;
		}
	};

	public class GameXmlDefinitionVariant
	{
		public LocalizableString Question { get; set; }
		public string Tip { get; set; }
		public LocalizableString Rationale { get; set; }
		public string Answer { get; set; }
		public string Variables { get; set; }
		public GameAnswerCheckAttributes CheckAttributes { get; set; }
		public string AnswerCheckExpression  { get; set; }
		public string AnswerShow { get; set; }

		List <DrawingObject> drawing_objects;

		public DrawingObject [] DrawingObjects {
			get {
				if (drawing_objects == null)
					return null;
	
				return drawing_objects.ToArray ();
			}
		}

		public void AddDrawingObject (DrawingObject obj)
		{
			if (drawing_objects == null)
				drawing_objects = new List <DrawingObject> ();

			drawing_objects.Add (obj);
		}

		public override string ToString ()
		{
			StringBuilder str = new StringBuilder ();

			str.AppendLine ("Question: " + Question);
			str.AppendLine ("Tip: " + Tip);
			str.AppendLine ("Rationale: " + Rationale);
			str.AppendLine ("Answer: " + Answer);
			str.AppendLine ("CheckAttributes: " + CheckAttributes);
			str.AppendLine ("AnswerCheckExpression: " + AnswerCheckExpression);

			return str.ToString ();
		}
	}

	// Container for a game defined in an Xml file
	public class GameXmlDefinition : GameXmlDefinitionVariant
	{
		public string Name { get; set; }
		public GameDifficulty Difficulty { get; set; }
		public GameTypes Type { get; set; }

		public List <GameXmlDefinitionVariant> Variants { get; set; }

		public GameXmlDefinition ()
		{
			Difficulty = GameDifficulty.Medium;
			Type = GameTypes.LogicPuzzle; // TODO: temporary, should be mandatory in games.xml
			Variants = new List <GameXmlDefinitionVariant> ();
		}

		public void NewVariant ()
		{
			Variants.Add (new GameXmlDefinitionVariant ());
		}

		public override string ToString ()
		{
			StringBuilder str = new StringBuilder ();

			str.AppendLine ("Name: " + Name);
			str.AppendLine ("Difficulty: " + Difficulty);
			str.AppendLine (base.ToString ());

			foreach (GameXmlDefinitionVariant variant in Variants)
			{
				str.AppendLine ("---");
				str.AppendLine (variant.ToString ());
				str.AppendLine ("---");
			}

			return str.ToString ();
		}
	}
}
