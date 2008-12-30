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
using Gtk;
using Mono.Unix;
using System.Collections;

public class CustomGameDialog : GtkDialog
{
	static ListStore games_store;
	[Glade.Widget] Gtk.TreeView treeview;
	[Glade.Widget] Box preview_vbox;
	[Glade.Widget] Label preview_question;
	CairoPreview drawing_area;
	GameManager manager;
	int ngames, npos;
	Type [] custom_games;

	public CustomGameDialog (GameManager manager) : base ("customgame")
	{
		Game game;
		Type[] games;
		GameManager gm;

		ngames = 0;
		this.manager = manager;
		gm = new GameManager ();
		gm.GameType = GameSession.Types.AllGames;
		games = gm.CustomGames;
		dialog = null;

		drawing_area = new CairoPreview ();
		preview_vbox.Add (drawing_area);
		drawing_area.Visible = true;

		// Define columns
		TreeViewColumn name_column = new TreeViewColumn (Catalog.GetString("Game Name"), 
                	new CellRendererText(), "text", 0);

		name_column.Expand = true;
		treeview.AppendColumn (name_column);

		TreeViewColumn type_column = new TreeViewColumn (Catalog.GetString("Type"), 
                	new CellRendererText(), "text", 1);

		type_column.Expand = true;
		treeview.AppendColumn (type_column);

		CellRendererToggle toggle_cell = new CellRendererToggle();
		TreeViewColumn toggle_column = new TreeViewColumn(Catalog.GetString("Enabled"), 
                	toggle_cell, "active", 2);	
		toggle_cell.Activatable = true;
		toggle_cell.Toggled += OnActiveToggled;
		toggle_column.Expand = false;
		treeview.CursorChanged += OnCursorChanged;
		treeview.AppendColumn (toggle_column);

		if (games_store == null) {
			games_store = new ListStore (typeof(string), typeof (string), typeof(bool), typeof (Game));
			         
			// Data
			string type;
			for (int i = 0; i < games.Length; i++)
			{	
				game =  (Game) Activator.CreateInstance (games [i], true);
				switch (game.Type) {
				case Game.Types.LogicPuzzle:
					type = Catalog.GetString ("Logic");
					break;
				case Game.Types.MemoryTrainer:
					type = Catalog.GetString ("Memory");
					break;
				case Game.Types.MathTrainer:
					type = Catalog.GetString ("Mental Calculation");
					break;
				default:
					type = string.Empty;
					break;
				}
				
				games_store.AppendValues (game.Name, type, true, game);
			}
		}

		treeview.Model = games_store;
		game =  (Game) Activator.CreateInstance (games [0], true);
		game.Initialize ();
		drawing_area.puzzle = game;
		preview_question.Markup = game.Question;
		treeview.ColumnsAutosize ();
	}

	public int NumOfGames {
		get { return ngames;}
	}

	private void OnCursorChanged (object o, EventArgs args) 
        {
		TreeIter iter;
            
            	if (!treeview.Selection.GetSelected (out iter)) {
			return;
            	}

                Game game = games_store.GetValue (iter, 3) as Game;
		game.Initialize ();
		preview_question.Markup = game.Question;
		drawing_area.puzzle = game;
		drawing_area.QueueDraw ();
	}

        private void OnActiveToggled (object o, ToggledArgs args) 
        {
		TreeIter iter;

		if (!games_store.GetIter (out iter, new TreePath (args.Path)))
			return;

                bool enabled = !(bool) games_store.GetValue (iter, 2);
		games_store.SetValue (iter, 2, enabled);
	}

	void OnSelectAll (object sender, EventArgs args)
	{
		games_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)  {
			games_store.SetValue (iter, 2, true);
			return false;
		});
	}

	void OnUnSelectAll (object sender, EventArgs args)
	{
		games_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)  {
			games_store.SetValue (iter, 2, false);
			return false;
		});
	}

	void OnOK (object sender, EventArgs args)
	{
		ngames = 0;
		npos = 0;

		games_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)  {
			if ((bool) games_store.GetValue (iter, 2) == true)
				ngames++;

			return false;
		});

		if (ngames == 0)
			return;

		custom_games = new Type [ngames];
		games_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)  {
			Game game = games_store.GetValue (iter, 3) as Game;
			bool enabled = (bool) games_store.GetValue (iter, 2);

			if (enabled == true) {
				custom_games[npos] = game.GetType ();
				npos++;
			}
			return false;
		});

		
		manager.CustomGames = custom_games;
	}

	public class CairoPreview : DrawingArea 
	{
		public Game puzzle = null;

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			if(!IsRealized)
				return false;

			int w, h, nw, nh;
			double x = 0, y = 0;
			Cairo.Context cc = Gdk.CairoHelper.Create (args.Window);
			CairoContextEx cr = new CairoContextEx (cc.Handle, this);
			args.Window.GetSize (out w, out h);

			nh = nw = Math.Min (w, h);

			if (nw < w) {
				x = (w - nw) / 2;
			}

			if (nh < h) {
				y = (h - nh) / 2;
			}

			cr.Translate (x, y);
			puzzle.DrawPreview (cr, nw, nh);
			((IDisposable)cc).Dispose();
			((IDisposable)cr).Dispose();
   			return base.OnExposeEvent(args);
		}
	}	
}
