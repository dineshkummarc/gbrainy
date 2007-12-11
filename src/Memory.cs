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
	protected const int total_time = 4 /*seconds*/ * 10;
	protected System.Timers.Timer timer;
	private bool request_answer = false;
	private bool buttons_active;
	protected bool shade = false;
	protected const int shading_time = 15;
	private LinearGradient gradient = null;
	protected double alpha;
	private bool draw_timer;

	public override bool ButtonsActive {
		get { return buttons_active;}
	}

	public abstract string MemoryQuestion {
		get;
	}

	public override Types Type {
		get { return Game.Types.MemoryTrainer;}
	}

	public override void Initialize ()
	{	
		time_left = total_time;
		timer = new System.Timers.Timer ();
		timer.Elapsed += TimerUpdater;
		timer.Interval = (1 * 100); // 0.1 seconds
		buttons_active = false;
		timer.Enabled = false;
		alpha = 1;
		draw_timer = false;
	}

	public void StartTimer ()
	{
		timer.Enabled = true;
		draw_timer = true;
	}

	private void TimerUpdater (object source, ElapsedEventArgs e)
	{
		if (shade == false && time_left == 0) {
			lock (this) {
				time_left = shading_time;
				shade = true;
				draw_timer = false;
			}
			return;	
		}	

		if (time_left == 0) {
			lock (this) {
				shade = false;
				timer.Enabled = false;
				request_answer = true;
				buttons_active = true;
			}
			if (App != null) {	
				Application.Invoke (delegate {
					App.UpdateQuestion (MemoryQuestion);
					App.ActiveInputControls (buttons_active);
				});
			}
		} else {
			lock (this) {
				time_left--;
			}
		}
		if (App != null)
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

		if (shade) {
			if (alpha > 0)
				alpha -= (1 / (double) shading_time);

			gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
			DrawObjectToMemorize (gr, area_width, area_height);
			return;
		}
		
		alpha = 1;
		gr.Color = new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha);
		if (request_answer && DrawAnswer == false) {
			DrawPossibleAnswers (gr, area_width, area_height);
		} else {
			DrawObjectToMemorize (gr, area_width, area_height);			
		}		
	}

	public override void DrawPreview (Cairo.Context gr, int width, int height)
	{
		gr.Scale (width, height);
		DrawBackground (gr);
		PrepareGC (gr);
		DrawObjectToMemorize (gr, width, height);
	}

	public virtual void DrawPossibleAnswers (Cairo.Context gr, int area_width, int area_height) {}

	public virtual void DrawObjectToMemorize (Cairo.Context gr, int area_width, int area_height)
	{
		double percentage;

		if (draw_timer == false)
			return;

		percentage = 100 - ((time_left * 100) / total_time);
		DrawTimeBar (gr, 0.1, 0.2, percentage);
	}

	public void DrawTimeBar (Cairo.Context gr, double x, double y, double percentage)
	{
		double width = 0.04, height = 0.6;
		double w = 0.003, h = 0.003;

		gr.Save ();
		gr.Color = new Color (0, 0, 0);	
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

		if (gradient == null) {
			gradient = new LinearGradient (0, 0, 0, 1);
			gradient.AddColorStop (1, new Color (0.2, 0, 0, 1));
			gradient.AddColorStop (0, new Color (1, 0, 0, 1));
		}

		gr.Source = gradient;			
		gr.MoveTo (x, y);
		gr.LineTo (x, y + height);
		gr.LineTo (x + width, y + height);
		gr.LineTo (x + width, y);
		gr.LineTo (x, y);
		gr.FillPreserve ();
		gr.Stroke ();
		gr.Restore ();
	}

}


