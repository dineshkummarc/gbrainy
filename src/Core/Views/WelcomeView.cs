/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Core.Views
{
	public class WelcomeView : IDrawable
	{
		public WelcomeView ()
		{

		}

		public void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double y = 0.03;
			const double space = 0.17;
			const double image_size = 0.14;

			gr.Scale (area_width, area_height);
			gr.DrawBackground ();
			//gr.SetPangoNormalFontSize ();
			gr.Color = new Cairo.Color (0, 0, 0, 1);

			gr.MoveTo (0.05, y);
			// Translators: {0} is the version number of the program
			gr.ShowPangoText (String.Format (Catalog.GetString ("Welcome to gbrainy {0}"), Defines.VERSION), true, -1, 0);
			gr.Stroke ();

			gr.DrawStringWithWrapping (0.05, y + 0.07, Catalog.GetString ("gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained. It includes:"));

			y = 0.22;
			gr.DrawImageFromAssembly ("logic-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
			gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
				Catalog.GetString ("Logic puzzles. Challenge your reasoning and thinking skills."), 
				rtl ? 0.65 : -1);

			y += space;
			gr.DrawImageFromAssembly ("math-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
			gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
				Catalog.GetString ("Mental calculation. Arithmetical operations that test your mental calculation abilities."),
				rtl ? 0.65 : -1);

			y += space;
			gr.DrawImageFromAssembly ("memory-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
			gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
				Catalog.GetString ("Memory trainers. To prove your short term memory."),
				rtl ? 0.65 : -1);

			y += space;
			gr.DrawImageFromAssembly ("verbal-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
			gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
				Catalog.GetString ("Verbal analogies. Challenge your verbal aptitude."),
				rtl ? 0.65 : -1);

			gr.DrawStringWithWrapping (0.05, y + 0.17,  Catalog.GetString ("Use the Settings to adjust the difficulty level of the game."));
			gr.Stroke ();
		}
	}
}
