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
using System.Web;

namespace gbrainy.Clients.WebForms
{
	public partial class MasterPage : System.Web.UI.MasterPage
	{

		// Instead of Page_Load we do Page_Init (Executed before)
		// To make sure that this code is executed before Page_Load the first page load
		public void Page_Init (object o, EventArgs e)
		{
			Logger.Debug ("MasterPage.Page_Load. IsPostBack {0}", IsPostBack);

			if (IsPostBack == true)
				return;

			
		}
	}
}
