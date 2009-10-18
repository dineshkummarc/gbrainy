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
using System.Timers;

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
