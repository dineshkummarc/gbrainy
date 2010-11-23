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
		static readonly int width = 400, height = 400, margin = 20, question_height = 100;
		static readonly int columns = 2, rows = 2;
		static readonly int page_margin = 20; // space between vertical and hortizontal pages
		static readonly int page_width = width + page_margin;
		static readonly int page_height = height + question_height + page_margin;

		static public void GeneratePdf (Game [] games, int games_page, string file)
		{
			PdfSurface pdf = new PdfSurface (file, page_width * columns, page_height * rows);
			CairoContextEx cr = new CairoContextEx (pdf, "sans 12", 72);

			GenerateQuestions (cr, games);
			GenerateAnswers (cr, games);

			pdf.Finish ();
			((IDisposable)cr).Dispose();
			return;
		}

		static void GenerateQuestions (CairoContextEx cr, Game [] games)
		{
			int x, y, page;
			Game puzzle;
			string str;

			x = y = page = 0;
			for (int i = 0; i < games.Length; i++)
			{
				puzzle = games [i];
				puzzle.Begin ();
				page++;

				cr.Save ();
				cr.Translate (x, y);
				cr.Rectangle (0, 0, width, height + question_height);
				cr.Clip ();

				// Translators: {0} is the game number and {1} the game question
				// The number is used as reference when looking for the game solution in the PDF
				str = String.Format (Catalog.GetString ("Game {0}. {1}"), i + 1, puzzle.Question);

				// Draw question
				cr.SetPangoFontSize (12);
				cr.UseMarkup = true;
				cr.DrawStringWithWrapping (margin, 10, str, width - margin);
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

				if (page >= columns * rows) {
					cr.ShowPage ();
					page = x = y = 0;
				}
			}

			if (y > 0)
				cr.ShowPage ();
		}

		static void GenerateAnswers (CairoContextEx cr, Game [] games)
		{
			int x, y, page;
			Game puzzle;
			string str;
			int column, row;

			x = y = page = 0;
			column = row = 0;

			// Draw solution title
			cr.SetPangoFontSize (20);
			cr.DrawStringWithWrapping (x + margin, y + margin,
				Catalog.GetString ("Solutions"), width - margin);
			y += 75;
			cr.Stroke ();

			cr.SetPangoFontSize (12);
			cr.UseMarkup = true;
			for (int i = 0; i < games.Length; i++)
			{
				// Translators: {0} is the game number and {1} the game answer
				// The number is used as reference to find the question to which this solution refers in the PDF
				str = String.Format (Catalog.GetString ("Game {0}. {1} "), i + 1, games[i].Answer);

				// Draw Solution
				cr.DrawStringWithWrapping (x + margin, y + margin, str, width - margin);
				cr.Stroke ();

				y += 75;

				// Next lateral page (right)
				if (y >= page_height * (row + 1) && x + page_width < page_width * columns) {
					column++;

					x = column * page_width;
					y = row * page_height;
					page++;
				} else {
					// No more space (right), new row
					if (y >= page_height * (row + 1) && x + page_width >= page_width * columns) {
						row++;
						column = 0;

						x = column * page_width;
						y = row * page_height;
						page++;
					}
				}

				if (page >= rows * columns) {
					cr.ShowPage ();
					page = x = y = 0;
					column = row = 0;
				}
			}

			if (y > 0)
				cr.ShowPage ();
		}
	}
}
