<%@ Page Language="C#" MasterPageFile = "MasterPage.master" Inherits="gbrainy.Clients.WebForms.Default" %>
<%@ Import Namespace="System.Data" %>

<asp:content id="main_content" ContentPlaceHolderID ="main_placeholder" runat="server">

	<b>Welcome to gbrainy.com</b>
	<br/>

	<br/>
	<br/>
		gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained. gbrainy includes the following games:
	<br/>
	<br/>

	<!-- Logic -->
	<span class="WelcomeLeft">
	 	<img src = "images/logic-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		Logic puzzles. Challenge your reasoning and thinking skills.
	</span>
	<br/>
	<br/>

	<!-- Calculation -->
	<span class="WelcomeLeft">
	 	<img src = "images/math-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		Mental calculation. Arithmetical operations that test your mental calculation abilities.
	</span>
	<br/>
	<br/>

	<!-- Verbal -->
	<span class="WelcomeLeft">
	 	<img src = "images/verbal-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		Verbal analogies. Challenge your verbal aptitude.
	</span>

	<br/>
	<br/>
	<asp:Button id="start_button" Text="Start a new game" OnClick="OnStartGame" runat="server"/>
	in
	<asp:DropDownList id = "languages_drop" ViewStateMode="Enabled" runat="server">
	</asp:DropDownList>
	language
	<p>
		<small>
		This web site is based on in the open source project called <a href="http://live.gnome.org/gbrainy">gbrainy</a>. Contact e-mail address: jmas at softcatala dot org
		</small>
	</p>
</asp:content>
