<%@ Page Language="C#" MasterPageFile = "MasterPage.master" Inherits="gbrainy.Clients.WebForms.Game" %>

<asp:content id="main_content" ContentPlaceHolderID ="main_placeholder" runat="server">

<table border="1px" style ="border-style:solid; border-collapse:collapse;" width = "580px" >
<td valign = "top">
	<!-- Toolbar -->
	<table border="0px" width = "80px">
	<tr>
		<td align = "center">
			<asp:ImageButton ImageUrl = "images/allgames-32.png" OnClick="OnStartAllGames" runat="server"></asp:ImageButton>
			<div>
				<asp:Label id="allgames_label" runat="server"/>
			</div>
		</td>
	</tr>
<!--	
	<tr>
		<td align = "center">
			<asp:ImageButton ImageUrl = "images/logic-games-32.png" runat="server"></asp:ImageButton>
		</td>
	</tr>

	<tr>
		<td align = "center">
			<asp:ImageButton ImageUrl = "images/math-games-32.png" runat="server"></asp:ImageButton>
		</td>
	</tr>

	<tr>
		<td align = "center">
			<asp:ImageButton ImageUrl = "images/memory-games-32.png" runat="server"></asp:ImageButton>
		</td>
	</tr>
-->
	<tr>
		<td align = "center">
			<asp:ImageButton ImageUrl = "images/endgame-32.png" id = "endgames_button" OnClick="OnClickEndGame" runat="server"></asp:ImageButton>
			<div>
				<asp:Label id="endgames_label" runat="server"/>
			</div>
		</td>
	</tr>	
	
	</table>
	</td>
<td>
	<!-- Main game area -->
	<table border="0px"  width = "500px" >	
		<tr>
			<td>
				<asp:Label id="question" runat="server"></asp:Label>
			</td>
		</tr>
		<tr>
			<td>
				 <asp:Image id="game_image" runat="server" />
			</td>
		</tr>
		<tr>
			<td>
				<asp:TextBox id = "answer_textbox" AutoCompleteType="Disabled" EnableViewState="true" runat="server"></asp:TextBox>
				<asp:Button id = "answer_button" Text = "Answer" OnClick ="OnClickAnswer" runat="server"></asp:Button>
				<asp:Button id = "nextgame_link" OnClick ="OnClickNextGame" runat="server" />				
				<asp:Label id="result_label" runat="server"></asp:Label>
				<asp:Label id="rationale_label" runat="server"></asp:Label>
			</td>
		</tr>

		<tr>
			<td>
				<asp:Label id="status" runat="server"></asp:Label>
			</td>
		</tr>
	</table>
</td>
</table>
	<br/>
</asp:content>
