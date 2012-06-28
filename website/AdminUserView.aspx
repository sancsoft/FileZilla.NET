<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminUserView.aspx.cs" Inherits="AdminUserView" %>
<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="margin:20px;">
        <div>
            <a href="Default.aspx">Home</a> &raquo; 
            <a href="Admin.aspx">Administration</a> &raquo; 
        </div>
        <h1><asp:Literal ID="UsernameTitleLiteral" runat="server" Text="UsernameTitleLiteral" /></h1>
        <div style="width:50%;float:left;">
            <div style="margin:10px;padding-right:20px;">
                <h3>User Settings</h3>
                <table style="width:100%;border:1px solid black;">
                    <tr>
                        <td style="width:33%;" class="FormField">Username</td>
                        <td style="width:67%;" class="FormValue"><asp:Label ID="UsernameLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Password</td>
                        <td class="FormValue"><asp:Label ID="PasswordLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Enabled?</td>
                        <td class="FormValue"><asp:Label ID="EnabledLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Home Directory</td>
                        <td class="FormValue"><asp:Label ID="HomeDirectoryLabel" runat="server" /></td>
                    </tr>
                </table>
                <br style="clear:both;" />
                <h3>Permissions</h3>
                <table style="width:100%;border:1px solid black;">
                    <tr>
                        <td style="width:33%;" class="FormField">Directory</td>
                        <td style="width:67%;" class="FormValue"><asp:Label ID="DirectoryLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Aliases</td>
                        <td class="FormValue"><asp:Label ID="AliasPathLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Is Home?</td>
                        <td class="FormValue"><asp:Label ID="IsHomeLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">File Pemissions</td>
                        <td class="FormValue"><asp:Label ID="FilePermissionsLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Dir Pemissions</td>
                        <td class="FormValue"><asp:Label ID="DirPermissionsLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Auto Create?</td>
                        <td class="FormValue"><asp:Label ID="AutoCreateLabel" runat="server" /></td>
                    </tr>
                </table>
            </div>
        </div>
        <div style="float:right;width:50%">
            <div style="margin:10px;padding-right:20px;">
                <h3>Change Password</h3>
                <table style="width:100%;border:1px solid black;">
                    <tr>
                        <td style="width:33%;" class="FormField">New Password</td>
                        <td style="width:67%;" class="FormValue"><asp:TextBox ID="NewPasswordTextBox" runat="server" Text="" Width="150" />
                    </tr>
                    <tr>
                        <td style="width:33%;" class="FormField">Notify</td>
                        <td style="width:67%;" class="FormValue"><asp:TextBox ID="NotificationTextBox" runat="server" Text="" Width="250" />
                    </tr>
                </table>
                <div style="float:right;">
                    <asp:Button ID="ForcePasswordButton" runat="server" Text="Force" CssClass="FormButton" />
                    <asp:Button ID="GeneratePasswordButton" runat="server" Text="Generate"  CssClass="FormButton" CausesValidation="false" />
                </div>
                <br style="clear:right;" />
                <br />
                <asp:Label ID="PasswordActionLabel" runat="server" Text="" />
                <p>Use the <strong>Force</strong> button to specify a password or <strong>Generate</strong> to create a random password.</p>
                <p>Use the <strong>Notify</strong> field to deliver an e-mail invitation to the system containing the account and password. 
                Multiple email addresses may be supplied by separating addresses with commas.</p>
            </div>
        </div>
    </div>
</asp:Content>
