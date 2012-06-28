<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminLogin.aspx.cs" Inherits="AdminLogin" %>
<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="margin:auto;width:600px;">
        <asp:ScriptManager ID="LoginScriptManager" runat="server" />
        <asp:UpdatePanel ID="LoginUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <div style="margin:auto;text-align:left;width:400px;margin-top:50px;margin-bottom:20px;">
            <asp:Panel ID="LoginGroup" runat="server" DefaultButton="LoginButton">
                <table class="FormTable">
                    <tr>
                        <td style="width:25%" class="FormField">Password *</td>
                        <td style="width:75%" class="FormValue">
                            <asp:TextBox ID="AccPasswordTextBox" runat="server" Width="180px" MaxLength="128" TextMode="Password" />
                            <asp:RequiredFieldValidator ID="AccPasswordTextBoxRequired" runat="server"
                                ControlToValidate="AccPasswordTextBox" ErrorMessage="Required" />
                        </td>
                    </tr>
                </table>
                <div style="text-align:right;">
                    <asp:Button ID="LoginButton" Text="Log In" runat="server" CssClass="FormButton" />
                </div>
            <asp:CustomValidator ID="LoginValidator" runat="server"
                ErrorMessage="The password is incorrect or remote administration has been disabled." />
            </asp:Panel>
        </div>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

