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
using Mono.Unix;

using Cairo;
using gbrainy.Core.Libraries;

namespace gbrainy.Core.Main
{
	// Generates a single PDF document with the selected games		
	static public class PdfExporter
	{
		static public void GeneratePdf (Game [] games, int games_page, string file)
		{
			const int width = 400, height = 400, margin = 20, question_height = 100;
			int x, y, cnt;
			Game puzzle;
			
			PdfSurface pdf = new PdfSurface (file, (width + margin) * 2, 
				(height + margin + question_height) * games_page / 2);

			CairoContextEx cr = new CairoContextEx (pdf, "sans 12", 72);
			x = y = cnt = 0;

			// TODO:
			//	- Solution page
			//	- Puzzles that are not shown correctly
			for (int i = 0; i < games.Length; i++)
			{
				puzzle = games [i];
				puzzle.Begin ();

				cnt++;

				cr.Save ();
				cr.Translate (x, y);
				cr.Rectangle (0, 0, width, height + question_height);
				cr.Clip ();

				// Draw question				
				cr.SetPangoFontSize (12);
				cr.UseMarkup = true;
				cr.DrawStringWithWrapping (20, 10, puzzle.Question, width - 20);
				cr.Stroke ();
				cr.UseMarkup = false;

				// Draw from question_height up height since from 0 to question_height is the question
				// Translate adds always to previous matrix's transformation
				cr.Translate (0, question_height);						
				puzzle.DrawPreview (cr, width, height, false);
				x += width + margin;
				if (x > width + margin) {
					x = 0;
					y += height + margin + question_height;
				}
				cr.Restore ();
				cr.Stroke ();

				if (cnt >= games_page) {
					cr.ShowPage ();
					cnt = x = y = 0;
				}
			}

			if (y > 0) {
				cr.ShowPage ();
			}

			pdf.Finish ();
			((IDisposable)cr).Dispose();
			return;
		}
	}
}
