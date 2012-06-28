<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>
<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="margin:20px;">
        <h1>Administration</h1>
        <div style="width:50%;float:left;margin-bottom:20px;">
            <div style="margin:10px;padding-right:20px;">
                <p>The web interface provides limited support for account management and review of activity. Full administration
                support is available through the FileZilla FTP Server Interface client application.</p>
                <div style="float:right;"><asp:Button ID="NewAccountButton" runat="server" Text="New Account" CssClass="FormButton" /></div>
                <br style="clear:right;" />
                <h3>Accounts</h3>
	            <dxwgv:ASPxGridView ID="UserGridView" runat="server" width="100%" KeyFieldName="Username" AutoGenerateColumns="False">
		            <Columns>
			            <dxwgv:GridViewDataTextColumn Caption="User" FieldName="Username" VisibleIndex="0" Width="70%">
				            <DataItemTemplate>	
					            <a href="AdminUserView.aspx?ID=<%# Eval( "Username" ) %>"><%# WebConvert.ToString( Eval( "Username" ), "&nbsp;" )%></a>
				            </DataItemTemplate>
			            </dxwgv:GridViewDataTextColumn>
			            <dxwgv:GridViewDataTextColumn Caption="Enabled?" FieldName="Enabled" VisibleIndex="1" Width="30%">
				            <DataItemTemplate>	
					            <a href="AdminUserView.aspx?ID=<%# Eval( "Username" ) %>"><%# (WebConvert.ToBoolean(Eval( "Enabled" ), false)) ? "Yes" : "No" %></a>
				            </DataItemTemplate>
			            </dxwgv:GridViewDataTextColumn>
		            </Columns>
		            <Settings ShowFilterRow="true" />
	            </dxwgv:ASPxGridView>
            </div>
        </div>
        <div style="float:right;width:50%;margin-bottom:20px;">
            <div style="margin:10px;padding-right:20px;">
                <h3>Web Settings</h3>
	            <dxwgv:ASPxGridView ID="SettingsGridView" runat="server" width="100%" KeyFieldName="key" AutoGenerateColumns="True">
		            <Columns>
			            <dxwgv:GridViewDataTextColumn Caption="Setting" FieldName="key" VisibleIndex="0" Width="40%">
				            <DataItemTemplate>	
					            <%# Eval( "key" ) %>
				            </DataItemTemplate>
			            </dxwgv:GridViewDataTextColumn>
			            <dxwgv:GridViewDataTextColumn Caption="Value" FieldName="setting_Text" VisibleIndex="1" Width="60%">
				            <DataItemTemplate>	
					            <%# Eval( "setting_Text" ) %>
				            </DataItemTemplate>
			            </dxwgv:GridViewDataTextColumn>
		            </Columns>
		            <Settings ShowFilterRow="true" />
	            </dxwgv:ASPxGridView>
            </div>
        </div>
    </div>
</asp:Content>
