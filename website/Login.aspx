<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>
<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type="text/javascript">
        function checkForSilverlight() {
            var hidSilverlight = document.getElementById('<%= IsSilverlightHidden.ClientID%>');
            if (hidSilverlight) {
                hidSilverlight.value = Silverlight.isInstalled();
            }
        }
    </script>
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
                        <td style="width:25%" class="FormField">Account *</td>
                        <td style="width:75%" class="FormValue">
                            <asp:TextBox ID="AccAccountTextBox" runat="server" Width="180px" MaxLength="128" />
                            <asp:RequiredFieldValidator ID="AccAccountTextBoxRequired" runat="server"
                                ControlToValidate="AccAccountTextBox" ErrorMessage="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormField">Password *</td>
                        <td class="FormValue">
                            <asp:TextBox ID="AccPasswordTextBox" runat="server" Width="180px" MaxLength="128" TextMode="Password" />
                            <asp:RequiredFieldValidator ID="AccPasswordTextBoxRequired" runat="server"
                                ControlToValidate="AccPasswordTextBox" ErrorMessage="Required" />
                            <asp:HiddenField ID="IsSilverlightHidden" runat="server" Value="false" />
                        </td>
                    </tr>
                </table>
                <div style="text-align:right;">
                    <asp:Button ID="LoginButton" Text="Log In" runat="server" CssClass="FormButton" OnClientClick="javscript:checkForSilverlight();" />
                </div>
            <asp:CustomValidator ID="LoginValidator" runat="server"
                ErrorMessage="The account and/or password is incorrect or the account has been disabled. Please try again or contact your system administrator." />
            </asp:Panel>
        </div>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div style="margin:20px;">
        <div style="width:50%;float:left;">
            <div style="margin:10px;padding-right:20px;">
                <hr style="margin-top:20px;" />
                <h3>Need an Account?</h3>
                <p>Please direct your account request to the system administrator via electronic mail to 
                <asp:Literal ID="NeedAccountEmailLiteral" Text="UNDISCLOSED" runat="server"/>.  Please include 
                your desired account name and reason for use.</p>
                <hr />
                <h3>Forget Your Password?</h3>
                <p>For security purposes, your password cannot be retrieved online.  Please contact <asp:Literal ID="ForgetPasswordLiteral" Text="UNDISCLOSED" runat="server"/> 
                to have your password reset.</p>
                <hr style="margin-top:20px;" />
                <h3>Why not use FTP?</h3>
                <p>FTP is a more efficient, secure and reliable method for file transfer than HTTP.  Please consider using an FTP client application to access the system, particularly if you need to
                transfer many or large files.</p>
                <p></p>
                <p>FileZilla offers a free, open source FTP client for Windows, Mac and Linux at <a href="http://filezilla-project.org/download.php?type=client">filezilla-project.org</a>.  
                If you prefer a different tool, the files provided through this server may be accessed with any standard FTP client.</p>
            </div>
        </div>
        <div style="float:right;width:50%">
            <div style="margin:10px;margin-top:20px;padding:20px;border:1px solid black; background-color:#eeeeee;">
                <h2>About FileZilla.NET</h2>
                <a href="http://github.com/sancsoft/FileZilla.NET" target="_blank"><img src="images/octocat.png" alt="github" title="github" style="height:96px;width:96px;border:0;float:right;margin-left:10px;" /></a>
                <p>FileZilla.NET is a browser-based client interface to the FileZilla FTP server, providing support for uploading or downloading files over the web.</p>
                <p>Additional information, source code, technical support and updates are available on the project web site at 
                <a href="https://github.com/sancsoft/FileZilla.NET" target="_blank">github.com/sancsoft/FileZilla.NET</a></p>
                <p>FileZilla is the free FTP solution, available as both a client or server. FileZilla is open source software distributed free of charge under the terms of the 
                GNU General Public License. Additional information and license terms are available at the <a href="http://filezilla-project.org" target="_blank">FileZilla web site</a>.</p>
                <p>We encourage you to support the FileZilla project by <a href="http://filezilla-project.org/donate.php">donating online</a>.</p>
                <p>Hosting, support and development of custom applications are available from <a href="http://www.sancsoft.com">Sanctuary Software Studio, Inc.</a></p>
            </div>
        </div>
    </div>
</asp:Content>

