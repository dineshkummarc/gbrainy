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
using Gtk;
using System.Threading;
using System.Timers;

public class GameDrawingArea : DrawingArea
{
	public enum Modes
	{	
		Welcome,
		Scores,
		Puzzle,
		CountDown,
	}

	GameSession session;
	ArrayListIndicesRandom random_indices;
	const int tips_shown = 4;
	CountDown countdown;
	bool rtl;
	bool margins;
	Modes mode;
	gbrainy application;
	public Game puzzle;

	public GameDrawingArea (gbrainy application)
	{
		mode = Modes.Welcome;
		rtl = Direction == Gtk.TextDirection.Rtl;
		this.application = application;
	}

	public GameSession GameSession {
		set {
			session = value;
			random_indices = new ArrayListIndicesRandom (GameTips.Count);
			random_indices.Initialize ();
		}
	}

	public Modes Mode {
		get { return mode;}
		set {
			if (mode == Modes.CountDown && countdown != null)
				countdown.EndDrawCountDown ();

			mode = value;
		}
	}

	public bool Margins {
		set { margins = value; }
	}

	static void DrawBand (CairoContextEx gr, double x, double y)
	{
		gr.Save ();
		gr.Rectangle (x, y, 1 - 0.06, 0.06);
		gr.Color = new Cairo.Color (0, 0, 0.2, 0.2);
		gr.Fill ();
		gr.Restore ();		
	}

	public void OnDrawCountDown (EventHandler OnFinish)
	{
		mode = Modes.CountDown;
		QueueDraw ();	
		countdown = new CountDown (this, OnFinish);
	}

	public void EndDrawCountDown ()
	{
		if (countdown == null)
			return;

		countdown.EndDrawCountDown ();
		countdown = null;
	}

	private void DrawWelcome (CairoContextEx gr, int area_width, int area_height)
	{
		double y = 0.03;
		const double space = 0.17;
		const double image_size = 0.14;

		gr.Scale (area_width, area_height);
		gr.DrawBackground ();
		gr.Color = new Cairo.Color (0, 0, 0, 1);

		gr.MoveTo (0.05, y);
		gr.ShowPangoText (String.Format (Catalog.GetString ("Welcome to gbrainy {0}"), Defines.VERSION), true, -1, 0);
		gr.Stroke ();

		gr.DrawStringWithWrapping (0.05, y + 0.07, Catalog.GetString ("gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained. It includes:"));

		y = 0.22;
		gr.DrawImageFromFile (Defines.DATA_DIR + "logic-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
		gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
			Catalog.GetString ("Logic puzzles. Challenge your reasoning and thinking skills."), 
			rtl ? 0.65 : -1);

		y += space;
		gr.DrawImageFromFile (Defines.DATA_DIR + "math-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
		gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
			Catalog.GetString ("Mental calculation. Arithmetical operations that test your mental calculation abilities."),
			rtl ? 0.65 : -1);

		y += space;
		gr.DrawImageFromFile (Defines.DATA_DIR + "memory-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
		gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
			Catalog.GetString ("Memory trainers. To prove your short term memory."),
			rtl ? 0.65 : -1);

		y += space;
		gr.DrawImageFromFile (Defines.DATA_DIR + "verbal-games.svg", rtl ? 0.75 : 0.05, y, image_size, image_size);
		gr.DrawStringWithWrapping (rtl ? 0.05 : 0.23, y + 0.01, 
			Catalog.GetString ("Verbal analogies. Challenge your verbal aptitude."),
			rtl ? 0.65 : -1);

		gr.DrawStringWithWrapping (0.05, y + 0.17,  Catalog.GetString ("Use the Settings to adjust the difficulty level of the game."));
		gr.Stroke ();
	}


	static void DrawBar (CairoContextEx gr, double x, double y, double w, double h, double percentage)
	{
		double per = percentage / 100;
	
		gr.Rectangle (x, y - h * per, w, h * per);
		gr.FillGradient (x, y - h * per, w, h * per, new Cairo.Color (0, 0, 1));
		gr.MoveTo (x, (y - 0.04) - h * per);
		gr.ShowPangoText (String.Format ("{0}%", percentage));
		gr.Stroke ();

		gr.Save ();
		gr.Color = new Cairo.Color (0, 0, 0);	
		gr.MoveTo (x, y);
		gr.LineTo (x, y - h * per);
		gr.LineTo (x + w, y - h * per);
		gr.LineTo (x + w, y);
		gr.LineTo (x, y);
		gr.Stroke ();
		gr.Restore ();
	}

	private void DrawGraphicBar (CairoContextEx gr, double x, double y)
	{
		const double area_w = 0.9, area_h = 0.28;
		const double bar_w = 0.05, bar_h = area_h - 0.02;
		const double space_x = 0.09;
		
		gr.LineWidth = 0.005;

		// Axis
		gr.MoveTo (x, y);
		gr.LineTo (x, y + area_h);
		gr.LineTo (x + area_w, y + area_h);
		gr.Stroke ();

		x = x + space_x;
		DrawBar (gr, x, y + area_h, bar_w, bar_h, session.TotalScore);
		gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Total"));

		x = x + space_x * 2;
		DrawBar (gr, x, y + area_h, bar_w, bar_h, session.LogicScore);
		gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, 	Catalog.GetString ("Logic")); 

		x = x + space_x * 2;
		DrawBar (gr, x, y + area_h, bar_w, bar_h, session.MathScore);
		gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Calculation"));

		x = x + space_x * 2;
		DrawBar (gr, x, y + area_h, bar_w, bar_h, session.MemoryScore);
		gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Memory"));

		x = x + space_x * 2;
		DrawBar (gr, x, y + area_h, bar_w, bar_h, session.VerbalScore);
		gr.DrawTextCentered (x + bar_w / 2, y + area_h + 0.03, Catalog.GetString ("Verbal"));
	}

	private void DrawScores (CairoContextEx gr, int area_width, int area_height)
	{
		double y = 0.04, x = 0.05;
		const double space_small = 0.02;
		string s;

		gr.Scale (area_width, area_height);
		gr.DrawBackground ();
		gr.Color = new Cairo.Color (0, 0, 0, 1);

		gr.MoveTo (x, y);
		gr.ShowPangoText (Catalog.GetString ("Score"), false, -1, 0);
		DrawBand (gr, 0.03, y - 0.01);

		y += 0.08;
		gr.MoveTo (x, y);

		if (session.GamesPlayed >= 10) {
			if (session.TotalScore >= 90)
				s = String.Format (Catalog.GetString ("Outstanding results"));
			else if (session.TotalScore >= 80) 
				s = String.Format (Catalog.GetString ("Excellent results"));
			else if (session.TotalScore >= 50) 
				s = String.Format (Catalog.GetString ("Good results"));
			else if (session.TotalScore >= 30) 
				s = String.Format (Catalog.GetString ("Poor results"));
			else s = String.Format (Catalog.GetString ("Disappointing results"));
		} else 
			s = String.Empty;

		gr.MoveTo (x, y);
		if (s == String.Empty)
			gr.ShowPangoText (String.Format (Catalog.GetString ("Games won: {0} ({1} played)"), session.GamesWon, session.GamesPlayed));	
		else 
			gr.ShowPangoText (String.Format (Catalog.GetString ("{0}. Games won: {1} ({2} played)"), s, session.GamesWon, session.GamesPlayed));	

		y += 0.06;
		gr.MoveTo (x, y);
		gr.ShowPangoText (String.Format (Catalog.GetString ("Time played {0} (average per game {1})"), session.GameTime, session.TimePerGame));
		
		y += 0.1;
		DrawGraphicBar (gr, x, y);
		y += 0.4;

		gr.MoveTo (x, y);
		gr.ShowPangoText (Catalog.GetString ("Tips for your next games"), false, -1, 0);
		DrawBand (gr, 0.03, y - 0.01);

		y += 0.08;
		for (int i = 0; i < tips_shown; i++)
		{
			y = gr.DrawStringWithWrapping (x, y,  "- " + GameTips.GetTip (random_indices[i]));
			if (y > 0.88)
				break;

			y += space_small;
		}
	
		gr.Stroke ();
	}

	protected override bool OnExposeEvent (Gdk.EventExpose args)
	{
		if(!IsRealized)
			return false;

		int w, h, nw, nh;
		double x = 0, y = 0;
		args.Window.GetSize (out w, out h);

		Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
		CairoContextEx cr = new CairoContextEx (cc.Handle, this);

		// We want a square drawing area for the puzzles then the figures are shown as designed. 
		// For example, squares are squares. This also makes sure that proportions are kept when resizing
		nh = nw = Math.Min (w, h);

		if (nw < w)
			x = (w - nw) / 2;

		if (nh < h)
			y = (h - nh) / 2;

		if (margins)
			application.SetMargin ((int) x);
		else
			application.SetMargin (2);

		cr.Translate (x, y);
		switch (mode) {
		case Modes.Welcome:
			DrawWelcome (cr, nw, nh);
			break;
		case Modes.Scores:
			DrawScores (cr, nw, nh);
			break;	
		case Modes.Puzzle:
			puzzle.Draw (cr, nw, nh);
			break;
		case Modes.CountDown:
			CountDown.Draw (cr, nw, nh);
			break;
		}

		((IDisposable)cc).Dispose();
		((IDisposable)cr).Dispose();
		return base.OnExposeEvent(args);
	}
}

public class CountDown
{
	static int countdown_time;
	System.Timers.Timer timer;
	EventHandler finish;
	GameDrawingArea area;

	public CountDown (GameDrawingArea area, EventHandler OnFinish)
	{
		countdown_time = 3;
		timer = new System.Timers.Timer ();
		timer.Elapsed += TimerUpdater;
		timer.Interval = (1 * 1000); // 1 second
		timer.Enabled = true;
		finish = OnFinish;
		this.area = area;
	}

	public void EndDrawCountDown ()
	{
		if (timer == null)
			return;

		timer.Enabled = false;
		timer.Dispose ();
		timer = null;
	}

	static public void Draw (CairoContextEx gr, int area_width, int area_height)
	{
		gr.Scale (area_width, area_height);

		gr.Color = new Cairo.Color (0.8, 0.8, 0.8);
		gr.Paint ();

		gr.LineWidth = 0.01;
		gr.Color = new Cairo.Color (0, 0, 0, 1);

		gr.SetPangoLargeFontSize ();
		gr.DrawTextCentered (0.5, 0.1, Catalog.GetString ("Get ready to memorize the next objects..."));
		gr.Stroke ();

		gr.SetPangoFontSize (0.35);
		gr.MoveTo (0.37, 0.22);
		gr.ShowPangoText (countdown_time.ToString ());
		gr.Stroke ();

		gr.Arc (0.5, 0.5, 0.25, 0, 2 * Math.PI);
		gr.Stroke ();
		gr.Arc (0.5, 0.5, 0.28, 0, 2 * Math.PI);
		gr.Stroke ();
	}

	void TimerUpdater (object source, ElapsedEventArgs e)
	{
		lock (this) {
			if (countdown_time == 1) {
				EndDrawCountDown ();
				Application.Invoke ( delegate { 
					finish (this, EventArgs.Empty);
				});
			}
			countdown_time--;
			Application.Invoke ( delegate { area.QueueDraw (); });
		}
	}
}


class GameTips
{
	static public int Count {
		get { return 11; }
	}

	static public String GetTip (int tip)
	{
		switch (tip) {
		case 0:
			return Catalog.GetString ("Read the instructions carefully and identify the data and given clues.");
		case 1:
			return Catalog.GetString ("To score the player gbrainy uses the time and tips needed to complete each game.");
		case 2:
			return Catalog.GetString ("In logic games, elements that may seem irrelevant can be very important.");
		case 3:
			return Catalog.GetString ("Break the mental blocks and look into the boundaries of problems.");
		case 4:
			return Catalog.GetString ("Enjoy making mistakes, they are part of the learning process.");
		case 5:
			return Catalog.GetString ("Do all the problems, even the difficult ones. Improvement comes from practising.");
		case 6:
			return Catalog.GetString ("Play on a daily basis, you will notice progress soon.");
		case 7: // Translators: Custom Game Selection is a menu option
			return Catalog.GetString ("Use the 'Custom Game Selection' to choose exactly which games you want to play.");
		case 8:
			return Catalog.GetString ("Use the Settings to adjust the difficulty level of the game.");
		case 9:
			return Catalog.GetString ("Association of elements is a common technique for remembering things.");
		case 10:
			return Catalog.GetString ("Grouping elements into categories is a common technique for remembering things.");
		}

		return string.Empty;
	}
}

