/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Core.Main
{
	public class GameLocator
	{
		public Type TypeOf { get; set; }
		public int Variant { get; set; }
		public GameTypes GameType { get; set; }
		public bool IsGame { get; set; }

		public GameLocator (Type type, int variant, GameTypes game_type, bool isGame)
		{
			TypeOf = type;
			Variant = variant;
			GameType = game_type;
			IsGame = isGame;
		}
	}
}
