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
using System.Collections.Generic;
using System.IO;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;

namespace gbrainy.Clients.WebForms
{
	public class GameImage
	{
		public const int IMAGE_WIDTH = 400;
		public const int IMAGE_HEIGHT = 400;
		static public string IMAGES_DIR = "tmp/";
		
		gbrainy.Core.Main.Game game;	
		
		public GameImage (gbrainy.Core.Main.Game game)
		{
			this.game = game;
		}
		
		public bool CreateImage (string file)
		{
			return CreateImage (game, file);
		}

		// TODO: We need a parameter here because not all the views are games
		// we need to extend the concept of view to include Widgets
		public bool CreateImage (IDrawable drawable, string file)
		{
			Cairo.ImageSurface cairo_image = null;
			gbrainy.Core.Main.CairoContextEx cr = null;			

			try
			{
				cairo_image = new Cairo.ImageSurface (Cairo.Format.ARGB32, IMAGE_WIDTH, IMAGE_HEIGHT);
				cr = new gbrainy.Core.Main.CairoContextEx (cairo_image, "sans 12", 96);

				// Draw Image
				drawable.Draw (cr, IMAGE_WIDTH, IMAGE_WIDTH, false);
				cairo_image.WriteToPng (file);

				if (File.Exists (file) == false)
					Logger.Error ("Game.CreateImage. Error writting {0}", file);
				else
					Logger.Debug ("Game.CreateImage. Wrote image {0}", file);
			}
			
			catch (Exception)
			{
				Logger.Error ("Game.CreateImage. Error writting {0}", file);
				return false;
			}

			finally
			{
				if (cr != null)
					((IDisposable) cr).Dispose ();

				if (cairo_image != null)
					((IDisposable) cairo_image).Dispose ();
			}

		    	return true;
		}

		static public string GetImageFileName (string sessionid)
		{
			string file;

			file = IMAGES_DIR + sessionid + ".png";
			return file;
		}		
	
		static string ProcessWidget (Widget widget)
		{
			return String.Format ("{0},{1},{2},{3}",
			                      widget.X * IMAGE_WIDTH,
			                      widget.Y * IMAGE_HEIGHT, 
			                      (widget.X + widget.Width) * IMAGE_WIDTH,
			                      (widget.Y + widget.Height) * IMAGE_HEIGHT);
		}		
			
		public IList <GameImageAreaShape> GetShapeAreas ()
		{
			List <GameImageAreaShape> area_shapes = new List <GameImageAreaShape> ();
	
			if (game == null)
				return area_shapes;
	
			foreach (Widget widget in game.Widgets)
			{
				Container container = widget as Container;
				
				if (container != null)
				{					
					foreach (Widget child in container.Children)
					{
						area_shapes.Add (new GameImageAreaShape (ProcessWidget (child),
							"?answer=" + child.DataEx.ToString (),
							area_shapes.Count));
					}
				}
				else {
					/*area_shapes.Add (new AreaShape (ProcessWidget (widget),
							"?answer=" + child.DataEx.ToString ()));*/
				}
			}
			
			return area_shapes;
		}
			
		static public void CreateDirectory (string images_dir)
		{
			try
			{
				Directory.CreateDirectory (images_dir);
			}
			catch (Exception e)
			{
				Logger.Error ("GameImage.CreateDirectory. Error creating {0} - {1}", images_dir, e.Message);
			}			
		}
	}
}
