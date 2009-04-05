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

using Cairo;
using Mono.Unix;
using System;

public class PuzzleCounting : Game
{
	enum GameType
	{
		Machine,
		Fence,
		Present,
		Total
	}

	string question, answer;
	GameType gametype;

	public override string Name {
		get {return Catalog.GetString ("Counting");}
	}

	public override string Question {
		get {return question; }
	}

	public override string Answer {
		get { return base.Answer + " " + answer;}
	}

	public override void Initialize ()
	{
		int ans, var, total;

		gametype = (GameType) random.Next ((int) GameType.Total);

		switch ((int) gametype)
		{
		case (int) GameType.Machine:
			var = 2 + random.Next (5);
			total = 50 + random.Next (100);
			question = String.Format (
				// Translators: {0} and {1} are always numbers greater than 1
				Catalog.GetString ("We have a {0} meters piece of fabric. Machine A takes {1} seconds to cut 1 meter of this fabric. How many seconds does Machine A take to cut the entire piece of fabric into 1 meter pieces?"),
				total, var);
			answer = String.Format (
				// Translators: {0} is always a number greater than 1
				Catalog.GetString ("With the {0} cut, Machine A creates two 1 meter pieces."), (total - 1));
	
			ans = (total - 1) * var;
			break;

		case (int) GameType.Fence:
			total = 20 + random.Next (20);
			ans = 4 * total - 4;
			question = String.Format (
				// Translators: {0} is always a number greater than 20
				Catalog.GetString ("A fence is built to enclose a square shaped region. {0} fence poles are used in each side of the square. How many fence poles are used in total?"),
				total);
				// Translators: {0} is always a number greater than 20
			answer = String.Format (
				Catalog.GetString ("There are {0} fence poles since the poles on the corners of the square are shared."), ans);
			break;

		case (int) GameType.Present:
			int present = 5 + random.Next (20);
			total = present + 2;
			ans = total;
			question = String.Format (
				// Translators: {0} is always a number greater than 5
				Catalog.GetString ("Wrapping an anniversary present costs one euro. The anniversary present costs {0} euros more than the cost to wrap it. How much does it cost to both purchase and wrap the present?"),
				present);
			answer = String.Format (
				Catalog.GetString ("Individually, the present costs one euro more to purchase than to wrap."));
			break;
		default:
			throw new Exception ("Unexpected value");
		}

		right_answer = (ans).ToString ();
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		base.Draw (gr, area_width, area_height);

		if (gametype == GameType.Present) {
			gr.DrawImageFromAssembly ("present.svg", 0.2, 0.4, 0.6, 0.2);
		} else {
			if (gametype == GameType.Fence)
			{
				double x105, y105;
				const double x = 0.35, y = 0.2;
				const double figure_size = 0.4;

				x105 = figure_size * Math.Cos (105 * Math.PI / 180);
				y105 = figure_size * Math.Sin (105 * Math.PI / 180);
				gr.MoveTo (x, y);
				gr.LineTo (x + x105, y + y105);
				gr.LineTo (x + x105 + figure_size, y + y105);
				gr.Stroke ();
				gr.MoveTo (x + figure_size, y);
				gr.LineTo (x + figure_size + x105, y + y105);
				gr.Stroke ();
				gr.MoveTo (x, y);
				gr.LineTo (x + figure_size, y);
				gr.Stroke ();
			}
		}
	}
}
