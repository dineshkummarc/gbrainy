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

using System;
using Cairo;
using System.Text;
using Mono.Unix;
using System.Timers;
using Gtk;

public abstract class Memory : Game
{
	protected int time_left;
	protected const int total_time = 5 /*seconds*/ * 10;
	protected System.Timers.Timer timer;
	private bool request_answer = false;
	private bool buttons_active;

	public override bool ButtonsActive {
		get { return buttons_active;}
	}

	public abstract string MemoryQuestion {
		get;
	}

	public override void Initialize ()
	{	
		time_left = total_time;
		timer = new System.Timers.Timer ();
		timer.Elapsed += TimerUpdater;
		timer.Interval = (1 * 100); // 0.1 seconds
		timer.Enabled = true;
		buttons_active = false;
	}

	private void TimerUpdater (object source, ElapsedEventArgs e)
	{
		if (time_left == 0) {
			lock (this) {
				timer.Enabled = false;
				request_answer = true;
				buttons_active = true;
			}
			Application.Invoke (delegate {	
				App.UpdateQuestion (MemoryQuestion);
				App.ActiveButtons (buttons_active);
			});
		} else {
			lock (this) {
				time_left--;
			}
		}
		Application.Invoke (delegate {	App.QueueDraw (); });
	}

	public override void Finish ()
	{
		timer.Enabled = false;
	}		

	public override void Draw (Cairo.Context gr, int area_width, int area_height)
	{	
		gr.Scale (area_width, area_height);
		DrawBackground (gr);
		PrepareGC (gr);

		if (request_answer) {
			DrawPossibleAnswers (gr, area_width, area_height);
		} else {
			DrawObjectToMemorize (gr, area_width, area_height);			
		}		
	}

	public virtual void DrawPossibleAnswers (Cairo.Context gr, int area_width, int area_height) {}

	public virtual void DrawObjectToMemorize (Cairo.Context gr, int area_width, int area_height) 
	{
		double percentage;

		percentage = 100 - ((time_left * 100) / total_time);
		DrawTimeBar (gr, 0.1, 0.2, percentage);
	}

	public void DrawTimeBar (Cairo.Context gr, double x, double y, double percentage)
	{
		double width = 0.04, height = 0.6;
		double w = 0.003, h = 0.003;
		
		gr.Save ();	
		gr.MoveTo (x, y);
		gr.LineTo (x, y + height);
		gr.LineTo (x + width, y + height);
		gr.LineTo (x + width, y);
		gr.LineTo (x, y);
		gr.Stroke ();

		x+= w;
		y+= h;
		width -= w * 2;
		height -= h * 2;
		y += height * (100 - percentage) / 100;
		height *= percentage / 100;

		gr.Color = new Cairo.Color (1, 0, 0);
		gr.MoveTo (x, y);
		gr.LineTo (x, y + height);
		gr.LineTo (x + width, y + height);
		gr.LineTo (x + width, y);
		gr.LineTo (x, y);
		gr.Fill ();
		gr.Restore ();
		
	}

}


