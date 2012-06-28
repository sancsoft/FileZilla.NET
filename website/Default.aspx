<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="DevExpress.Web.v11.2, Version=11.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxFileManager" TagPrefix="dx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <dx:ASPxFileManager ID="FileManager" runat="server" Height="550">
        <ClientSideEvents SelectedFileOpened="function(s, e) {e.file.Download();e.processOnServer = false;}" />        
    </dx:ASPxFileManager>
    <asp:Panel ID="SilverlightPanel" runat="server" CssClass="notice" Visible="false">
        Microsoft Silverlight is not available.  Install and enable Silverlight to support advanced file upload features. <a href="http://www.microsoft.com/silverlight/">&laquo;&nbsp;Install&nbsp;Silverlight&nbsp;&raquo;</a>
    </asp:Panel>
</asp:Content>
