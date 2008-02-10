/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Timers;
using Gtk;
using System.Collections;

public class MemoryIndications : Memory
{
	class Indication 
	{
		public Type type;
		public object obj;

		public Indication (Type type, object obj)
		{
			this.type = type;
			this.obj = obj;
		}

		public enum Type
		{
			Start,
			Turn,
			End,
		}

		public enum TurnDirection
		{
			Right,
			Left,
			Down,
			Up
		}

		public void Draw (Cairo.Context gr, ref double x, ref double y, Indication next_prev)
		{
			double line_length = 0.050;
			double points = 0.050;

			if (type == Type.Start) {
				gr.Rectangle (x, y, points, points);
				DrawingHelpers.DrawTextCentered (gr, x + points /2 , y + points /2, ((int)obj).ToString ());
				switch ((TurnDirection) next_prev.obj) {
				case TurnDirection.Right:
					x += points;
					y += points / 2;
					break;
				case TurnDirection.Left:
					y += points / 2;
					break;
				case TurnDirection.Down:
					y += points;
					x += points / 2;
					break;
				case TurnDirection.Up:
					x += points / 2;
					break;
				}

				gr.Stroke ();
			
			} else if (type == Type.Turn) {
				gr.MoveTo (x, y);
				switch ((TurnDirection) obj) {
				case TurnDirection.Right:
					x += line_length;
					break;
				case TurnDirection.Left:
					x -= line_length;
					break;
				case TurnDirection.Up:
					y -= line_length;
					break;
				case TurnDirection.Down:
					y += line_length;
					break;
				}

				gr.LineTo (x, y);
				gr.Stroke ();
			} else if (type == Type.End) {
				switch ((TurnDirection) next_prev.obj) {
				case TurnDirection.Right:
					y -= points / 2; 
					break;
				case TurnDirection.Left:
					x -= points;
					y -= points / 2;
					break;
				case TurnDirection.Down:
					x -= points / 2; 
					break;
				case TurnDirection.Up:
					x -= points / 2; 
					y -= points;
					break;
				}
				gr.Rectangle (x, y, points, points);
				DrawingHelpers.DrawTextCentered (gr, x + points /2 , y + points /2, ((int)obj).ToString ());
			}
		}	
	
		public override string ToString ()
		{
			switch (type) {
			case Type.Start:
				return String.Format (Catalog.GetString ("Start in point number {0}"), (int) obj);
			case Type.Turn: {
				switch ((TurnDirection) obj) {
				case TurnDirection.Right:
					return Catalog.GetString ("Turn right");
				case TurnDirection.Left:
					return Catalog.GetString ("Turn left");
				case TurnDirection.Up:
					return Catalog.GetString ("Go up");
				case TurnDirection.Down:
					return Catalog.GetString ("Go down");
				}
				break;
			}
			case Type.End:
				return String.Format (Catalog.GetString ("End in point {0}"), obj);
			}
			return null;
		}
	}

	private Indication[] indications;
	private Indication[] indications_wrongA;
	private Indication[] indications_wrongB;
	private Indication[] indications_wrongC;
	private ArrayListIndicesRandom answers;
	const int steps = 7;

	public override string Name {
		get {return Catalog.GetString ("Memorize indications");}
	}

	public override string MemoryQuestion {
		get { 
			return String.Format (Catalog.GetString ("Which of the follow graphics represents the indications previously given?"));}
	}

	public override void Initialize ()
	{			
		indications = new Indication [steps];
		Indication.TurnDirection second_turn = (Indication.TurnDirection) 2 +  random.Next (2);
		
		indications[0] = new Indication (Indication.Type.Start, 0);
		indications[1] = new Indication (Indication.Type.Turn, random.Next (2)); // right or left
		indications[2] = new Indication (Indication.Type.Turn, second_turn); // up or down
		indications[3] = new Indication (Indication.Type.Turn, random.Next (2)); // right or left

		if (second_turn == Indication.TurnDirection.Up)
			indications[4] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Up);
		else
			indications[4] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Down);

		indications[5] = new Indication (Indication.Type.Turn, random.Next (2)); // right or left
		indications[6] = new Indication (Indication.Type.End, 1);
		
		indications_wrongA = CopyAnswer ();
		indications_wrongB = CopyAnswer ();
		indications_wrongC = CopyAnswer ();

		if ((Indication.TurnDirection) indications[3].obj == Indication.TurnDirection.Right) {
			indications_wrongA[3] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Left);
		}
		else {
			indications_wrongA[3] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Right);
		}

		if ((Indication.TurnDirection) indications[5].obj == Indication.TurnDirection.Right) {
			indications_wrongB[5] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Left);
		}
		else {
			indications_wrongB[5] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Right);
		}
		if ((Indication.TurnDirection) indications[1].obj == Indication.TurnDirection.Right) {
			indications_wrongC[1] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Left);
		}
		else {
			indications_wrongC[1] = new Indication (Indication.Type.Turn, Indication.TurnDirection.Right);
		}
		
		base.Initialize ();

		answers = new ArrayListIndicesRandom (4);
		answers.Initialize ();

		for (int i = 0; i < answers.Count; i++) {
			if ((int) answers [i] == 0) {
				right_answer += (char) (65 + i);
				break;
			}
		}

		//for (int i = 0; i < indications.Length; i++)
		//	Console.WriteLine ("{0}",  indications[i].ToString ());
	}

	private Indication[] CopyAnswer ()
	{
		Indication[] answer = new Indication [steps];
		for (int i = 0; i < steps; i++)
			answer[i] = new Indication (indications[i].type, indications[i].obj);

		return answer;
	}

	private void DrawPossibleAnswers (Cairo.Context gr, double x, double y, Indication[] indications)
	{		
		for (int i = 0; i < indications.Length - 1; i++)
			indications[i].Draw (gr, ref x, ref y, indications[i + 1]);

		indications[indications.Length - 1].Draw (gr, ref x, ref y, indications[indications.Length - 2]);
	}

	private Indication[] WhichAnswer (object answer)
	{
		switch ((int) answer) {
		case 0:
			return indications;
		case 1:
			return indications_wrongA;
		case 2:
			return indications_wrongB;
		case 3:
			return indications_wrongC;
		}

		return null;
	}

	public override void DrawPossibleAnswers (Cairo.Context gr, int area_width, int area_height)
	{
		double x, y;

		x = 0.22; y = 0.3;
		DrawPossibleAnswers (gr, x, y, WhichAnswer (answers[0]));
		gr.MoveTo (x, y + 0.2);
		//gr.ShowText ("Figure A");
		gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), "A"));

		x = 0.7; y = 0.3;
		DrawPossibleAnswers (gr, x, y, WhichAnswer (answers[1]));
		gr.MoveTo (x, y + 0.2);
		gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), "B"));

		x = 0.22; y = 0.7;
		DrawPossibleAnswers (gr, x, y, WhichAnswer (answers[2]));
		gr.MoveTo (x, y + 0.2);
		gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), "C"));

		x = 0.7; y = 0.7;
		DrawPossibleAnswers (gr, x, y, WhichAnswer (answers[3]));
		gr.MoveTo (x, y + 0.2);
		gr.ShowText (String.Format (Catalog.GetString ("Figure {0}"), "D"));
	}
	
	public override void DrawObjectToMemorize (Cairo.Context gr, int area_width, int area_height)
	{
		base.DrawObjectToMemorize (gr, area_width, area_height);

		for (int i = 0; i < indications.Length; i++)
		{
			gr.MoveTo (0.3, 0.2 + i * 0.08);
			gr.ShowText (indications[i].ToString ());
			gr.Stroke ();
		}
	}
}


