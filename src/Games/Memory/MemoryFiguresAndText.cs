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

using System;
using Cairo;

using gbrainy.Core.Main;
using gbrainy.Core.Services;
using gbrainy.Core.Toolkit;

namespace gbrainy.Games.Memory
{
	public class MemoryFiguresAndText : Core.Main.Memory
	{
		FigureType [] figures;
		int rows, columns;
		const double start_x_ques = 0.25;
		const double start_x_ans = 0.25;
		const double start_y = 0.15;
		const double figure_size = 0.08;
		double rect_w, rect_h;
		int question_pos, figures_active;
		FigureTypeConverter converter;
		FigureType [] answers;
		int answer_idx;

		internal enum FigureType
		{
			Triangle,
			Square,
			Pentagon,
			Circle,
			Total
		};

		internal class FigureTypeConverter
		{
			ITranslations Translations {get; set;}

			internal FigureTypeConverter (ITranslations translations)
			{
				Translations = translations;
			}

			internal string ToString (FigureType type)
			{
				switch (type) {
				case FigureType.Triangle:
					return Translations.GetString ("Triangle");
				case FigureType.Square:
					return Translations.GetString ("Square");
				case FigureType.Pentagon:
					return Translations.GetString ("Pentagon");
				case FigureType.Circle:
					return Translations.GetString ("Circle");
				default:
					throw new InvalidOperationException ();
				}
			}
		}

		public override string Name {
			get {return Translations.GetString ("Memorize figures and text");}
		}

		public override string MemoryQuestion {
			get { return String.Format (Translations.GetString 
				("The list below enumerates the figures shown in the previous image except for one. Which is the missing figure? Answer {0}, {1}, {2} or {3}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));
			}
		}

		protected override void Initialize ()
		{
			converter = new FigureTypeConverter (Translations);

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
			case GameDifficulty.Medium:
				figures_active = 6;
				rows = 2;
				columns = 3;
				break;
			case GameDifficulty.Master:
				figures_active = 9;
				columns = rows = 3;
				break;
			default:
				throw new InvalidOperationException ();
			}

			rect_w = 0.65 / columns;
			rect_h = 0.65 / rows;
			question_pos = random.Next (figures_active);
			
			RandomizeFigures ();
			RandomizePossibleAnswers ();

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;
			Answer.SetMultiOptionAnswer (answer_idx, converter.ToString (figures[question_pos]));
			Answer.CorrectShow = converter.ToString (figures[question_pos]);
			base.Initialize ();

			DrawingAnswerObjects ();
		}

		void DrawingAnswerObjects ()
		{
			Container container = new Container (DrawAreaX + 0.2, DrawAreaY + 0.4, 0.5, answers.Length * 0.10);
			AddWidget (container);

			for (int i = 0; i < answers.Length; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.4, 0.1);
				drawable_area.X = DrawAreaX + 0.23;
				drawable_area.Y = DrawAreaY + 0.4 + i * 0.1;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int d = (int) e.Data;
					e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.07, 0.02);
					e.Context.ShowPangoText (String.Format (Translations.GetString ("{0}) {1}"), Answer.GetMultiOption (d),
						converter.ToString (answers[d])));
				};
			}
		}
		void RandomizeFigures ()
		{
			ArrayListIndicesRandom figures_random;
			int pos = 0;
			
			figures = new FigureType [figures_active];
			figures_random = new ArrayListIndicesRandom ((int) FigureType.Total);
			figures_random.Initialize ();

			for (int n = 0; n < figures_active; n++)
			{
				figures[n] = (FigureType) figures_random [pos++];

				if (pos >= figures_random.Count) {
					pos = 0;
					figures_random.Initialize ();
				}
			}
		}

		void RandomizePossibleAnswers ()
		{
			ArrayListIndicesRandom answers_random;

			answers_random = new ArrayListIndicesRandom (4);
			answers_random.Initialize ();

			answers	= new FigureType [answers_random.Count];

			for (int i = 0; i < answers.Length; i++)
			{
				answers [i] = (FigureType) answers_random [i];
				if (figures[question_pos] == answers [i])
				{
					answer_idx = i;
				}
			}
		}

		public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x= DrawAreaX, y = DrawAreaY + 0.085;
			int pos = 0;
			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, 1);

			if (Answer.Draw ==  true) {
				DrawAllFigures (gr, start_x_ans, start_y);
				return;
			}

			gr.SetPangoNormalFontSize ();
			for (int i = 0; i < figures.Length; i++)
			{
				if (i == question_pos)
					continue;

				gr.MoveTo (x, y);
				gr.ShowPangoText (converter.ToString (figures[i]));

				if ((pos + 1) % 4 == 0) {
					y += 0.1;
					x = DrawAreaX;
				} else {
					x+= 0.25;
				}
				pos++;
			}

			base.DrawPossibleAnswers (gr,  area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();
			gr.MoveTo (0, 0.4);
			gr.ShowPangoText (Translations.GetString ("Choose one of the following:"));

			gr.MoveTo (0, 0.08);
			gr.ShowPangoText (Translations.GetString ("List of images shown before"));
		}

		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			DrawAllFigures (gr, start_x_ques, start_y);
		}

		void DrawAllFigures (CairoContextEx gr, double x, double y)
		{
			int col = 1;
			FigureType fig;
			double org_x = x;

			DrawGrid (gr, x, y);
			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
			for (int figure = 0; figure < figures.Length; figure++, col++)
			{
				fig = figures[figure];
				if (fig == FigureType.Total)
					fig = FigureType.Triangle;

				DrawFigure (gr, x, y, fig);

				if (col >= columns) {
					col = 0;
					y += rect_h;
					x = org_x;
				} else {
					x += rect_w;
				}
			}
		}

		void DrawFigure (CairoContextEx gr, double x, double y, FigureType type)
		{
			double space_x, space_y;

			space_x = (rect_w - figure_size) / 2;
			space_y = (rect_h - figure_size) / 2;

			switch (type) {
			case FigureType.Triangle:
				gr.DrawEquilateralTriangle (x + space_x, y + space_y, figure_size);
				break;
			case FigureType.Square:
				gr.Rectangle (x + space_x, y + space_y, figure_size, figure_size);
				gr.Stroke ();
				break;
			case FigureType.Pentagon:
				gr.DrawPentagon (x + space_x, y + space_y, figure_size);
				break;
			case FigureType.Circle:
				gr.Arc (x + space_x + figure_size / 2, y + space_y + figure_size / 2, figure_size / 2, 0, 2 * Math.PI);
				gr.Stroke ();
				break;
			default:
				throw new InvalidOperationException ();
			}
		}

		void DrawGrid (CairoContextEx gr, double x, double y)
		{
			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {
					gr.Rectangle (x + column * rect_w, y + row * rect_h, rect_w, rect_h);
				}
			}
			gr.Stroke ();
		}
	}
}
