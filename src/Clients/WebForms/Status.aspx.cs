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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Diagnostics;

namespace WebForms
{
    public partial class Status : System.Web.UI.Page
    {
	//protected System.Web.UI.WebControls.ListView ListView1;

	public class PerfCounter
	{
		public string Category { get; set; }
		public string Counter { get; set; }

		public PerfCounter (string category, string counter)
		{
			Category =  category;
			Counter = counter;
		}
	}

	static PerfCounter [] PerfCounters =
	{
		new PerfCounter ("Mono Memory", "Allocated Objects"),
		new PerfCounter ("ASP.NET", "Requests Total"),

	};


        public void Page_Load (object sender, EventArgs e)
	{
		updated_label.Text = "Updated: " + DateTime.Now;

		// Sessions
		foreach (WebSession session in Global.Sessions.Values)
		{
			TableRow r = new TableRow ();

			// Session ID
			TableCell c = new TableCell ();
	                c.Controls.Add (new LiteralControl (session.Session.SessionID.ToString ()));
	                r.Cells.Add (c);

			// Date
			c = new TableCell ();
	                c.Controls.Add (new LiteralControl (session.Started.ToString ()));
			r.Cells.Add (c);

			sessions_table.Rows.Add (r);
		}

            	total_label.Text = "Total sessions: " + Global.Sessions.Count;

		// Games
		string text = Game.CreateManager ().GetGamesSummary ();
		text = text.Replace (Environment.NewLine, "<br/>");
		games_label.Text = text;

		// Assemblies
		foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
		{
			TableRow r = new TableRow ();
			AssemblyName name = asm.GetName ();

			// Name
			TableCell c = new TableCell ();
	                c.Controls.Add (new LiteralControl (name.Name));
	                r.Cells.Add (c);

			// Version
			c = new TableCell ();
	                c.Controls.Add (new LiteralControl (name.Version.ToString ()));
			r.Cells.Add (c);

			assemblies_table.Rows.Add (r);
		}

		// Performace counters
		foreach (PerfCounter perf in PerfCounters)
		{
			TableRow r = new TableRow ();

			// Category
			TableCell c = new TableCell ();
	                c.Controls.Add (new LiteralControl (perf.Category));
	                r.Cells.Add (c);

			// Name
			c = new TableCell ();
	                c.Controls.Add (new LiteralControl (perf.Counter));
			r.Cells.Add (c);

			// Value
			c = new TableCell ();
	                c.Controls.Add (new LiteralControl (ReadCounter (perf.Category, perf.Counter)));
			r.Cells.Add (c);

			counters_table.Rows.Add (r);
		}
        }

	string ReadCounter (string category, string counter)
	{
		PerformanceCounter pc = new PerformanceCounter (category, counter);
		pc.NextValue ();
		return pc.NextValue ().ToString ();
	}
    }
}
