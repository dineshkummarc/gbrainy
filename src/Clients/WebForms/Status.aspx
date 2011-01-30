<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="gbrainy.Clients.WebForms.Status" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
	<form id="form1" runat="server">
		<b>gbrainy's status page</b>
		<br/>
		<asp:Label runat ="server" ID="updated_label"/>
		<br/><br/>
		<b>Current active list of gbrainy user's sessions</b>
		<br/><br/>
		<asp:Table id="sessions_table" GridLines="Both" CellPadding="5" CellSpacing="5"	Runat="server">
			<asp:TableHeaderRow Runat="server">
				<asp:TableHeaderCell Runat="server">Session ID</asp:TableHeaderCell>
				<asp:TableHeaderCell Runat="server">Time Started</asp:TableHeaderCell>
			</asp:TableHeaderRow>
		</asp:Table>
		<br/>
    		<asp:Label runat ="server" ID="total_label"/>
    		<br/><br/>
		<b>Games</b>
		<br/>
		<asp:Label runat ="server" ID="games_label"/>
    		<br/><br/>
		<b>gbrainy assemblies versions</b>
		<asp:Table id="assemblies_table" GridLines="Both" CellPadding="5" CellSpacing="5"	Runat="server">
			<asp:TableHeaderRow Runat="server">
				<asp:TableHeaderCell Runat="server">Assembly</asp:TableHeaderCell>
				<asp:TableHeaderCell Runat="server">Version</asp:TableHeaderCell>
			</asp:TableHeaderRow>
		</asp:Table>
		<br/><br/>

		<b>Performance Counters</b>
		<br/>
			<asp:Table id="counters_table" GridLines="Both" CellPadding="5" CellSpacing="5"	Runat="server">
			<asp:TableHeaderRow Runat="server">
				<asp:TableHeaderCell Runat="server">Category</asp:TableHeaderCell>
				<asp:TableHeaderCell Runat="server">Counter</asp:TableHeaderCell>
				<asp:TableHeaderCell Runat="server">Value</asp:TableHeaderCell>
			</asp:TableHeaderRow>
		</asp:Table>


    </form>
</body>
</html>
