<%@ Page Language="C#" MasterPageFile = "MasterPage.master" Inherits="WebForms.Game" %>

<asp:content id="main_content" ContentPlaceHolderID ="main_placeholder" runat="server">
	<table border="1px" width = "500px" >
		<tr>
			<td colspan="2"><strong>Puzzle</strong></td>
		</tr>
		<tr>
			<td>
				<asp:Label id="question" runat="server"></asp:Label>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Image id="image" runat="server" />
			</td>
		</tr>
		<tr>
			<td>
				<asp:TextBox id = "answer_textbox" AutoCompleteType="None" EnableViewState="true" runat="server"></asp:TextBox>
				<asp:Button id = "answer_button" Text = "Answer" OnClick ="OnClickAnswer" runat="server"></asp:Button>
				<asp:Label id="result_label" runat="server"></asp:Label>
				<asp:Label id="rationale_label" runat="server"></asp:Label>

				<asp:LinkButton id="nextgame_link" OnClick ="OnClickNextGame" runat="server" />
				<asp:LinkButton id="endgame_link" OnClick ="OnClickEndGame" runat="server" />
			</td>
		</tr>

		<tr>
			<td>
				<asp:Label id="status" runat="server"></asp:Label>
			</td>
		</tr>
	</table>
	<br/>
</asp:content>
