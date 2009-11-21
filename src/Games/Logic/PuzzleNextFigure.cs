/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;
using gbrainy.Core.Libraries;

namespace gbrainy.Games.Logic
{
	public class PuzzleNextFigure : Game
	{
		private const double figure_size = 0.2;
		private ArrayListIndicesRandom random_indices;

		public enum CerclePosition 
		{
			None		= 0,
			Top		= 2,
			Right		= 4,
			Bottom		= 8,
			Left		= 16,
		}

		enum Figures
		{
			First,
			Second,
			Third,
			Last
		};

		public override string Name {
			get {return Catalog.GetString ("Next figure");}
		}

		public override string Question {
			get {return String.Format (
				Catalog.GetString ("Which is the next logical figure in the sequence? Answer {0}, {1} or {2}."),
				GetPossibleAnswer (0), GetPossibleAnswer (1), GetPossibleAnswer (2));} 
		}


		public override string Answer {
			get { 
				string answer = base.Answer + " ";

				answer += String.Format (Catalog.GetString ("From first figure, the top circle advances by two positions clockwise, while the left circle goes backwards one position."));
				return answer;
			}
		}

		public override void Initialize ()
		{
			random_indices = new ArrayListIndicesRandom ((int) Figures.Last);
			random_indices.Initialize ();

			for (int i = 0; i < (int) Figures.Last; i++)
			{
				if ((int) random_indices[i] == (int) Figures.Third) {
					right_answer = GetPossibleAnswer (i);
					break;
				}
			}
		}

		static private void DrawDiamon (CairoContextEx gr, double x, double y, CerclePosition cercles)
		{	
			double distance = 0.04;

			gr.MoveTo (x + figure_size / 2, y);
			gr.LineTo (x, y + figure_size / 2);
			gr.LineTo (x + figure_size / 2, y + figure_size);
			gr.LineTo (x + figure_size, y + figure_size / 2);
			gr.LineTo (x + figure_size / 2, y);
			gr.Stroke ();

			if ((cercles & CerclePosition.Top) == CerclePosition.Top) {
				gr.Arc (x + figure_size / 2, y + distance, 0.01, 0, 2 * Math.PI);	
				gr.Stroke ();
			}

			if ((cercles & CerclePosition.Right) == CerclePosition.Right) {
				gr.Arc (x + figure_size - distance, y + figure_size / 2, 0.01, 0, 2 * Math.PI);	
				gr.Stroke ();
			}

			if ((cercles & CerclePosition.Bottom) == CerclePosition.Bottom) {
				gr.Arc (x + figure_size / 2, y + figure_size - distance, 0.01, 0, 2 * Math.PI);	
				gr.Stroke ();
			}

			if ((cercles & CerclePosition.Left) == CerclePosition.Left) {
				gr.Arc (x + distance, y + figure_size / 2, 0.01, 0, 2 * Math.PI);
				gr.Stroke ();
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX;
			double y = DrawAreaY;
			double space_figures = figure_size + 0.1;
		
			base.Draw (gr, area_width, area_height, rtl);

			DrawDiamon (gr, x, y, CerclePosition.Top | CerclePosition.Left);
			DrawDiamon (gr, x + space_figures , y, CerclePosition.Bottom);
			DrawDiamon (gr, x + space_figures * 2, y, CerclePosition.Top | CerclePosition.Right);
		
			y += figure_size + 0.06;
			gr.MoveTo (x, y);
			gr.ShowPangoText (Catalog.GetString ("Possible answers are:"));
			gr.Stroke ();
			y += 0.10;

			for (int i = 0; i < (int) Figures.Last; i++)
			{
			 	switch ((Figures) random_indices[i]) {
				case Figures.First:
					DrawDiamon (gr, x, y, CerclePosition.Right | CerclePosition.Left);
					break;
				case Figures.Second:
					DrawDiamon (gr, x, y, CerclePosition.Top | CerclePosition.Right);
					break;
				case Figures.Third:
					DrawDiamon (gr, x, y, CerclePosition.Bottom | CerclePosition.Top);
					break;
				}
			
				gr.MoveTo (x + 0.02, y + 0.25);
				gr.ShowPangoText (GetPossibleFigureAnswer (i));
				x += space_figures;			
			}

			gr.Stroke ();
		}
	}
}
