/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Web;
using System.Web.UI;
using System.IO;
using System.Collections.Generic;

using gbrainy.Core.Main;

namespace gbrainy.Clients.WebForms
{	
	public struct GameContainer
	{
		public string Question;
		public string Solution;
		public string Image;
		public string Tip;
		public int ID;
		
		public bool TipVisible { 
			get {
				return String.IsNullOrEmpty (Tip) == false; 
			}
		}
		
		public GameContainer (string question, string solution, string image, string tip, int id)
		{
			Question = question;
			Solution = solution;
			Image = image;
			Tip = tip;
			ID = id;
		}
	};
	
	public partial class AllGames : System.Web.UI.Page
	{
		const string images_dir = "allgames_images";
		const int elements_per_page = 10;
		
		static GameManager manager;
		static bool created_games;
		static List <GameContainer> game_container = new List<GameContainer> ();
		static List <int> nexts = new List<int> ();
		
		List <GameContainer> games_container_page = new List<GameContainer> ();
		int ShowPage {set; get; }
		
		void Page_Load (Object sender, EventArgs e)
		{
			SetPageToShow ();
			CreateGames ();
			CreatePageView ();
			Bindings ();
		}
		
		void CreatePageView ()
		{
			int start = ShowPage *  elements_per_page;
			int end = start + Math.Min (game_container.Count, elements_per_page);
			
			for (int i = start; i < end; i++)
				games_container_page.Add (game_container [i]);			
		}
		
		void SetPageToShow ()
		{
			string page;
			page = Request.QueryString ["page"];
			
			if (String.IsNullOrEmpty (page))
				return;			
			
			int rslt;			
			if (int.TryParse (page, out rslt) == true)			
				ShowPage = rslt;			
		}
		
		void Bindings ()
		{
			games_repeater.DataSource = games_container_page;
			games_repeater.DataBind ();			
			nexts_repeater.DataSource = nexts;
			nexts_repeater.DataBind ();
		}
		
		void CreateGames ()
		{
			if (created_games == true)
				return;
			
			TranslationsWeb translations = new TranslationsWeb ();
			manager = Game.CreateManager ();
			GameImage.CreateDirectory (images_dir);	
			
			GameLocator [] games;
			gbrainy.Core.Main.Game game;
			
			games = manager.AvailableGames;
			
			for (int i = 0; i < games.Length; i++)
			{
				if (games [i].IsGame == false)
					continue;
				
				if (games [i].GameType == GameTypes.Memory)
					continue;
		
				game = (gbrainy.Core.Main.Game) Activator.CreateInstance (games [i].TypeOf, true);
				game.translations = translations;
				game.Variant = games [i].Variant;
				game.Begin ();								
				string file = CreateImage (game, i);
				
				game_container.Add (new GameContainer (game.Question, game.AnswerText, file, game.TipString,
				                                        game_container.Count));
			}
			
			for (int i = 0; i < game_container.Count / elements_per_page; i++)
				nexts.Add (i);
			
			created_games = true;
		}
		
		static public string CreateImage (gbrainy.Core.Main.Game game, int i)
		{
			string file = images_dir + "/" + i.ToString () + ".png";
			GameImage image = new GameImage (game);
			image.CreateImage (file);
			return file;
		}
	}
}
