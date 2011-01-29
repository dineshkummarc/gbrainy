<%@ Page Language="C#" MasterPageFile = "MasterPage.master" Inherits="WebForms.Default" %>
<%@ Import Namespace="System.Data" %>

<asp:content id="main_content" ContentPlaceHolderID ="main_placeholder" runat="server">

	<b>Welcome to gbrainy.com</b>
	<br/>

	<br/>
	<asp:Label id="intro_label" runat="server"/>
	<br/>
	<br/>

	<!-- Logic -->
	<span class="WelcomeLeft">
	 	<img src = "images/logic-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		<asp:Label id="logic_label" runat="server"/>
	</span>
	<br/>
	<br/>

	<!-- Calculation -->
	<span class="WelcomeLeft">
	 	<img src = "images/math-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		<asp:Label id="calculation_label" runat="server"/>
	</span>
	<br/>
	<br/>

	<!-- Memory -->
	<span class="WelcomeLeft">
	 	<img src = "images/memory-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		<asp:Label id="memory_label" runat="server"/>
	</span>
	<br/>
	<br/>

	<!-- Verbal -->
	<span class="WelcomeLeft">
	 	<img src = "images/verbal-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		<asp:Label id="verbal_label" runat="server"/>
	</span>

	<br/>
	<br/>
	<asp:Button id="start_button" Text="Start game!" OnClick="OnStartGame" runat="server"/>
	
	<asp:DropDownList id = "languages_drop" AutoPostBack="True" ViewStateMode="Enabled"
		onselectedindexchanged="OnSelectedIndexChanged" runat="server">
	</asp:DropDownList>

</asp:content>
