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
using Gdk;
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

	public Game puzzle;
	public Modes mode;
	private GameSession session;
	private ArrayListIndicesRandom random_indices;
	private const int tips_count = 10;
	private const int tips_shown = 4;
	private System.Timers.Timer timer;
	private int countdown_time;
	private EventHandler finish;

	public GameDrawingArea ()
	{
		mode = Modes.Welcome;
		puzzle = null;
		session = null;
	}

	public GameSession GameSession {
		set {
			session = value;
			random_indices = new ArrayListIndicesRandom (tips_count);
			random_indices.Initialize ();
		}
	}

	private void DrawBackground (CairoContextEx gr)
	{
		gr.Save ();
		gr.Color = new Cairo.Color (1, 1, 1);
		gr.Paint ();	
		gr.Stroke ();
		gr.Restore ();		
	}

	private void DrawBand (CairoContextEx gr, double x, double y)
	{
		gr.Save ();
		gr.Rectangle (x, y, 1 - 0.06, 0.06);
		gr.Color = new Cairo.Color (0, 0, 0.2, 0.2);
		gr.Fill ();
		gr.Restore ();		
	}

	private void DrawWelcome (CairoContextEx gr, int area_width, int area_height)
	{
		double y = 0.07;
		double space = 0.21;
		double line_space = 0.06;
		ImageSurface image;

		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		gr.Color = new Cairo.Color (0, 0, 0, 1);
		gr.SetFontSize (0.035);

		gr.MoveTo (0.05, y);
		gr.ShowText (String.Format (Catalog.GetString ("Welcome to gbrainy {0}"), Defines.VERSION));
		gr.Stroke ();

		gr.SetFontSize (0.03);
		gr.DrawStringWithWrapping (0.05, y + 0.08, line_space, Catalog.GetString ("gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained. It includes:"));

		y = 0.26;
		image = new ImageSurface (Defines.DATA_DIR + "logic-games-80.png");
		if (image.Width > 0) {
			gr.Save ();
			gr.Translate (0.05, y);
			gr.Scale (0.8 / area_width, 0.8 / area_height);
			gr.SetSourceSurface (image, 0, 0);
			gr.Paint ();
			gr.Restore ();
		}
		gr.DrawStringWithWrapping (0.21, y + 0.03, line_space, Catalog.GetString ("Logic puzzles. Designed to challenge your reasoning and thinking skills."));

		y += space;
		image = new ImageSurface (Defines.DATA_DIR + "math-games-80.png");
		if (image.Width > 0) {
			gr.Save ();
			gr.Translate (0.05, y);
			gr.Scale (0.8 / area_width, 0.8 / area_height);
			gr.SetSourceSurface (image, 0, 0);
			gr.Paint ();
			gr.Restore ();
		}
		gr.DrawStringWithWrapping (0.21, y + 0.03, line_space, Catalog.GetString ("Mental calculation. Based on arithmetical operations that test your mental calculation abilities."));

		y += space;
		image = new ImageSurface (Defines.DATA_DIR + "memory-games-80.png");
		if (image.Width > 0) {
			gr.Save ();
			gr.Translate (0.05, y);
			gr.Scale (0.8 / area_width, 0.8 / area_height);
			gr.SetSourceSurface (image, 0, 0);
			gr.Paint ();
			gr.Restore ();
		}
		gr.DrawStringWithWrapping (0.21, y + 0.03, line_space, Catalog.GetString ("Memory trainers. To prove and enhance your short term memory."));
		gr.Stroke ();

		gr.DrawStringWithWrapping (0.05, y + 0.23, line_space, Catalog.GetString ("Use the Settings to adjust the difficulty level of the game."));
		gr.Stroke ();
	}

	private void TimerUpdater (object source, ElapsedEventArgs e)
	{
		lock (this) {
			if (countdown_time == 1) {
				timer.Enabled = false;
				timer.Dispose ();
				Application.Invoke ( delegate { 
					finish (this, EventArgs.Empty);
				});
			}
			countdown_time--;
			Application.Invoke ( delegate { QueueDraw (); });
		}
	}

	public void OnDrawCountDown (EventHandler OnFinish)
	{
		mode = Modes.CountDown;
		QueueDraw ();
		
		countdown_time = 3;
		timer = new System.Timers.Timer ();
		timer.Elapsed += TimerUpdater;
		timer.Interval = (1 * 1000); // 1 seconds
		timer.Enabled = true;
		finish = OnFinish;
	}

	private void DrawCountDown (CairoContextEx gr, int area_width, int area_height)
	{
		gr.Scale (area_width, area_height);

		gr.Color = new Cairo.Color (0.8, 0.8, 0.8);
		gr.Paint ();

		gr.LineWidth = 0.01;
		gr.Color = new Cairo.Color (0, 0, 0, 1);

		gr.SetFontSize (0.033);
		gr.DrawTextCentered (0.5, 0.1, Catalog.GetString ("Get ready to memorize the next objects..."));
		gr.Stroke ();

		gr.SetFontSize (0.4);
		gr.MoveTo (0.37, 0.63);
		gr.ShowText (countdown_time.ToString ());
		gr.Stroke ();

		gr.Arc (0.5, 0.5, 0.25, 0, 2 * Math.PI);
		gr.Stroke ();
		gr.Arc (0.5, 0.5, 0.28, 0, 2 * Math.PI);
		gr.Stroke ();

	}

	private void DrawScores (CairoContextEx gr, int area_width, int area_height)
	{
		double y = 0.08, x = 0.05;
		double space_small = 0.06;
		string str;

		gr.Scale (area_width, area_height);
		gr.Color = new Cairo.Color (1, 1, 1);
		gr.Paint ();	
		gr.Color = new Cairo.Color (0, 0, 0, 1);

		gr.SetFontSize (0.03);
		gr.MoveTo (x, y);
		gr.ShowText (Catalog.GetString ("Score"));		
		DrawBand (gr, 0.03, y - 0.04);

		gr.SetFontSize (0.03);
		y += 0.08;
		gr.MoveTo (x, y);
		gr.ShowText (String.Format (Catalog.GetString ("Your total score is {0}%"), session.TotalScore));

		y += space_small;	
		gr.MoveTo (x, y);

		if (session.LogicGamesPlayed == 0)
			str = Catalog.GetString ("No logic puzzle games played");
		else
			str = String.Format (Catalog.GetString ("Logic puzzle score is {0}%"), session.LogicScore);

		gr.ShowText (str);

		y += space_small;
		gr.MoveTo (x, y);

		if (session.MathGamesPlayed == 0)
			str = Catalog.GetString ("No mental calculation games played");
		else
			str = String.Format (Catalog.GetString ("Mental calculation score is {0}%"), session.MathScore);

		gr.ShowText (str);

		y += space_small;
		gr.MoveTo (x, y);

		if (session.MemoryGamesPlayed == 0)
			str = Catalog.GetString ("No memory games played");
		else
			str = String.Format (Catalog.GetString ("Memory score is {0}%"),  session.MemoryScore);

		gr.ShowText (str);

		y += 0.08;
		gr.SetFontSize (0.03);
		gr.MoveTo (x, y);
		gr.ShowText (Catalog.GetString ("Game statistics"));
		DrawBand (gr, 0.03, y - 0.04);

		gr.SetFontSize (0.03);
		y += 0.08;
		gr.MoveTo (x, y);
		gr.ShowText (String.Format (Catalog.GetString ("Total games won: {0} ({1} played)"), session.GamesWon, session.GamesPlayed));	
		y += space_small;
		gr.MoveTo (x, y);
		gr.ShowText (String.Format (Catalog.GetString ("Total time played {0} (average per game {1})"), session.GameTime, session.TimePerGame));
		y += 0.08;

		gr.SetFontSize (0.03);
		gr.MoveTo (x, y);
		gr.ShowText (Catalog.GetString ("Tips for your next games"));
		DrawBand (gr, 0.03, y - 0.04);

		gr.SetFontSize (0.03);
		y += 0.08;
		for (int i = 0; i < tips_shown; i++)
		{
			y = gr.DrawStringWithWrapping (x, y, space_small, "- " + GetTip ((int) random_indices[i]));
			if (y > 0.85)
				break;

			y += space_small;
		}
	
		gr.Stroke ();

	}

	private String GetTip (int tip)
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
		case 7:
			return Catalog.GetString ("Use the Custom Game Selection to choose exactly which games you want to play.");
		case 8:
			return Catalog.GetString ("Use the Settings to adjust the difficulty level of the game.");
		case 9:
			return Catalog.GetString ("Association of elements is a common technique for remembering things.");
		}

		return string.Empty;
	}

	protected override bool OnExposeEvent (Gdk.EventExpose args)
	{
		if(!IsRealized)
			return false;

		int w, h;
		args.Window.GetSize (out w, out h);
		Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
		CairoContextEx cr = new CairoContextEx (cc.Handle);

		switch (mode) {
		case Modes.Welcome:
			DrawWelcome (cr, w, h);
			break;
		case Modes.Scores:
			DrawScores (cr, w, h);
			break;	
		case Modes.Puzzle:
			puzzle.Draw (cr, w, h);
			break;
		case Modes.CountDown:
			DrawCountDown (cr, w, h);
			break;
		}

		((IDisposable)cc).Dispose();
		((IDisposable)cr).Dispose();
		return base.OnExposeEvent(args);
	}

}


