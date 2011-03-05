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

using Cairo;
using System;

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

public class PuzzleSample : Game
{
	public override string Name {
		get {return "Puzzle sample";}
	}

	public override string Question {
		get {return "In a party all the people is introduced to the rest. There are 28 handshakes. How many people is in the party?";}
	}

	protected override void Initialize ()
	{
		Answer.Correct = "8";
	}

	public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
	{
		base.Draw (gr, area_width, area_height, rtl);

		gr.Color = new Color (0.4, 0.4, 0.4);
		gr.DrawTextCentered (0.5, DrawAreaY, "This is an extension sample");
	}
}
