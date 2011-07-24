<%@ Page Language="C#" MasterPageFile = "MasterPage.master" Inherits="gbrainy.Clients.WebForms.AllGames" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="gbrainy.Clients.WebForms" %>

<asp:content id="main_content" ContentPlaceHolderID ="main_placeholder" runat="server">
<p>This page shows all the gbrainy's games. To play a game use the <a href="Default.aspx">main page</a></p>
<hr/>
<asp:Repeater id="games_repeater" runat="server" EnableViewState = false>
<asp:ItemTemplate>
	     <p><%#  ((GameContainer) Container.DataItem).Question %> </p>
	     <img src = "<%#  ((GameContainer) Container.DataItem).Image %>"/>
	     <br/>
	     
	     <asp:Panel Visible = "<%#  ((GameContainer) Container.DataItem).TipVisible %>" runat="server">
	     
	     <a onclick="toggleVisibleById('tip_<%#  ((GameContainer) Container.DataItem).ID%>');return false;" href="">See Tip</a>
	     
	     <div id = "tip_<%#  ((GameContainer) Container.DataItem).ID%>" style = "display:none">
	     	<br/>
	     	<%#  ((GameContainer) Container.DataItem).Tip %>
	     </div>
	     </asp:Panel>

	     <a onclick="toggleVisibleById('solution_<%#  ((GameContainer) Container.DataItem).ID%>');return false;" href="">See Solution</a>
	     <div id = "solution_<%#  ((GameContainer) Container.DataItem).ID%>" style = "display:none">
	     	<br/>
	     	<%#  ((GameContainer) Container.DataItem).Solution %>
	     </div>
	     
	     <hr/>
</asp:ItemTemplate>
</asp:Repeater>

More games 
<asp:Repeater id="nexts_repeater" runat="server" EnableViewState = false>
<asp:ItemTemplate>
		<a href ="?page=<%# Container.DataItem %>"><%# Container.DataItem %></a> &nbsp;
</asp:ItemTemplate>
</asp:Repeater>

</asp:content>

