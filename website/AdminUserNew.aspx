<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AdminUserNew.aspx.cs" Inherits="AdminUserNew" %>
<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="margin:20px;">
        <div>
            <a href="Default.aspx">Home</a> &raquo; 
            <a href="Admin.aspx">Administration</a> &raquo; 
        </div>
        <h1>Create User</h1>
        <div style="width:50%;float:left;">
            <div style="margin:10px;padding-right:20px;">
                <p>A standard user will be created with access to a home folder matching the username under the defined root folder of <strong><asp:Literal ID="RootFolderLiteral" runat="server" text="[root]"/></strong>.
                The user's directory will be created if it does not exist.</p> 
                <p>Usernames must be valid paths for file access and are limited to lowercase characters, numbers and the underscore.</p>
                <p>The user may be assigned Read-Only, Read/Write or Custom permissions in their home folder.</p>
                <p>Read-Only users will be granted granted File Read, Directory List and Directory SubDirectories which are used to download files over HTTP and FTP interfaces.</p>
                <p>Read/Write users will be granted File Write, File Delete, File Append, Directory Create and Directory Delete in addition to the permissions of a Read-Only user.</p>
                <p>Alternate permissions may be assigned by selecting Custom from the Permissions drop down and checking the desired permission boxes.</p>
                <p>Leave the password field blank to automatically generate a password for the account.</p>
                <p>Non-standard user accounts may be created through the FTP server interface.</p>
                <p>The Notify field may be used to send an invitation message containing links and account information to the specified email addresses.  Multiple 
                email addresses should be comma delimited.</p>
                <asp:Label ID="CreateUserActionLabel" runat="server" Text="" ForeColor="#339933" Font-Bold="true" />
                <asp:RequiredFieldValidator ID="UsernameRequired" runat="server" Display="Dynamic" ControlToValidate="UsernameTextBox" ErrorMessage="Username Required" />
                <asp:CustomValidator ID="UsernameDuplicate" runat="server" Display="Dynamic" ControlToValidate="UsernameTextBox" ErrorMessage="Duplicate - Matching Username Already Exists" />
            </div>
        </div>
        <div style="float:right;width:50%">
            <div style="margin:10px;padding-right:20px;">
                <h3>User Settings</h3>
                <table style="width:100%;border:1px solid black;">
                    <tr>
                        <td style="width:33%;" class="FormField">Username</td>
                        <td style="width:67%;" class="FormValue"><asp:TextBox ID="UsernameTextBox" runat="server" Text="username" Width="180" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Password</td>
                        <td class="FormValue"><asp:TextBox ID="PasswordTextBox" runat="server" Text="password" Width="180" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Enabled?</td>
                        <td class="FormValue"><asp:CheckBox ID="EnabledCheckBox" runat="server" Checked="true" /></td>
                    </tr>
                    <tr>
                        <td class="FormField">Permissions</td>
                        <td class="FormValue">
                            <asp:DropDownList ID="PermissionsList" runat="server">
                                <asp:ListItem Text="Read/Write" Value="RW" />
                                <asp:ListItem Text="Read Only" Value="RO" />
                                <asp:ListItem Text="Custom" Value="CU" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br style="clear:both;" />
                <h3>Custom Permissions</h3>
                <table style="width:100%;border:1px solid black;">
                    <tr>
                        <td style="width:30%;" class="FormField">File Read</td>
                        <td style="width:20%;text-align:center;" class="FormValue"><asp:CheckBox ID="FileReadCheckBox" runat="server" Checked="true" /></td>
                        <td style="width:50%;" class="FormValue">Download files from the site</td>
                    </tr>
                    <tr>
                        <td class="FormField">File Write</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="FileWriteCheckBox" runat="server" /></td>
                        <td class="FormValue">Upload files to the site</td>
                    </tr>
                    <tr>
                        <td class="FormField">File Delete</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="FileDeleteCheckBox" runat="server" /></td>
                        <td class="FormValue">Remove files from the site</td>
                    </tr>
                    <tr>
                        <td class="FormField">File Append</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="FileAppendCheckBox" runat="server" /></td>
                        <td class="FormValue"><em>Not used by the site</em></td>
                    </tr>
                    <tr>
                        <td class="FormField">Directory Create</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="DirCreateCheckBox" runat="server" /></td>
                        <td class="FormValue">Create new folders on site</td>
                    </tr>
                    <tr>
                        <td class="FormField">Directory Delete</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="DirDeleteCheckBox" runat="server" /></td>
                        <td class="FormValue">Remove folders from the site</td>
                    </tr>
                    <tr>
                        <td class="FormField">Directory List</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="DirListCheckBox" runat="server" Checked="true" /></td>
                        <td class="FormValue"><em>Not used by the site</em></td>
                    </tr>
                    <tr>
                        <td class="FormField">Directory Subdirs</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="DirSubdirsCheckBox" runat="server" Checked="true" /></td>
                        <td class="FormValue"><em>Not used by the site</em></td>
                    </tr>
                    <tr>
                        <td class="FormField">Auto Create</td>
                        <td class="FormValue" style="text-align:center;"><asp:CheckBox ID="AutoCreateCheckBox" runat="server" Checked="false" /></td>
                        <td class="FormValue"><em>Not used by the site</em></td>
                    </tr>
                </table>
                <br style="clear:right;" />
                <h3>User Invitation</h3>
                <table style="width:100%;border:1px solid black;">
                    <tr>
                        <td style="width:33%;" class="FormField">Notify</td>
                        <td style="width:67%;" class="FormValue"><asp:TextBox ID="NotificationTextBox" runat="server" Text="" Width="250" /></td>
                    </tr>
                </table>
                <br style="clear:right;" />
                <div style="float:right;">
                    <asp:Button ID="CreateUserButton" runat="server" Text="Create" CssClass="FormButton" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CssClass="FormButton" CausesValidation="false" />
                </div>
                <br style="clear:right;" />
                <br />
            </div>
        </div>
    </div>
</asp:Content>
