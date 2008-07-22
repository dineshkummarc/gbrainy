/*
 * Copyright (C) 2007 Javier M Mora <javiermm@gmail.com>
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

//Utility class for color operations

public class ColorPalette
{
	private ArrayListIndicesRandom color_order;

	private double alpha;
	public double Alpha {
		set { alpha = value; }
		get { return alpha; }
	}

	public int Count {
		get { return color_order.Count; }
	}

	// Are defined "First", "PrimaryColors", "PrimarySecundaryColors", and "Last" to
	// create iterators. So: 
	//   for (Colors.Id it= Colors.Id.First; it<Colors.Id.PrimaryColors; it++);
	//   for (Colors.Id it= Colors.Id.First; it<Colors.Id.PrimarySecundaryColors; it++);
	//   for (Colors.Id it= Colors.Id.First; it<Colors.Id.Last; it++);
	//

	public enum Id
	{
		First=0,
		Red=First,
		Green,
		Blue,
		PrimaryColors,
		Yellow=PrimaryColors, 
		Magenta,
		Orange,
		PrimarySecundaryColors,
		Black=PrimarySecundaryColors,
		Last,
		White=Last
	};

	private static string[] ColorName= new string[] {
		Catalog.GetString ("red"),
		Catalog.GetString ("green"),
		Catalog.GetString ("blue"),
		Catalog.GetString ("yellow"),
		Catalog.GetString ("magenta"),
		Catalog.GetString ("orange"),
		Catalog.GetString ("black"),
		Catalog.GetString ("white")
	};

	private static Cairo.Color[] CairoColor = new Cairo.Color[] {
		new Cairo.Color (0.81, 0.1, 0.13),
		new Cairo.Color (0.54, 0.71, 0.24),
		new Cairo.Color (0.17, 0.23 ,0.56),
		new Cairo.Color (0.94, 0.93, 0.25),
		new Cairo.Color (0.82, 0.25, 0.59),
		new Cairo.Color (1, 0.54, 0),
		new Cairo.Color (0, 0, 0),
		new Cairo.Color (.9, .9, .9)
	};

	public ColorPalette (Id id)
	{
		color_order = new ArrayListIndicesRandom((int)id);
		alpha=1;
	}

	public ColorPalette (int size)
	{
		color_order = new ArrayListIndicesRandom(size);
		alpha=1;
	}

	public void Initialize()
	{
		color_order.Initialize();
	}

	public Cairo.Color Cairo (int index) 
	{
		return Cairo (CairoColor[(int)color_order[index]]);
	}

	public Cairo.Color Cairo (Id id) 
	{
		return Cairo (CairoColor[(int)id]);
	}

	public Cairo.Color Cairo(Cairo.Color color)
	{
		return new Cairo.Color(color.R, color.G, color.B, alpha);
	}

	public string Name(int index)
	{
		return ColorName[(int)color_order[index]];
	}

	public string Name(Id id)
	{
		return ColorName[(int)id];
	}

	public int Size()
	{
		return color_order.Count;
	}
}

