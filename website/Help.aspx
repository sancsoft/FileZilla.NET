<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div style="margin:40px;">
    <h1>FileZilla.NET Help</h1>
    <hr />
    <table style="padding:0px;border-collapse:collapse;">
        <tr>
            <td style="width:50%;vertical-align:top;">
                <div style="margin-right:20px;">
                    <h2>Common Tasks</h2>
                    <h3>Downloading Files</h3>
                    <p>Select the file to download and click the download buttton, press enter or double click the desired file.</p>
                    <h3>Uploading Files</h3>
                    <p>Navigate to the destination folder. Click the browse button and select the file to upload from local storage. Click the upload link to send the file to the current folder.</p>
                    <h3>Finding Files</h3>
                    <p>Use the folder tree to locate files within the directory hierarchy.  The filter box may be used to limit the displayed files to matches by name or extension.</p>
                </div>
            </td>
            <td style="width:50%;vertical-align:top;">
                <div style="margin-left:20px;">
                    <h2>Common Questions</h2>
                    <h3>What is Silverlight?</h3>
                    <p>Silverlight is a cross-platform browser plug-in by Microsoft that is used to support advanced upload features.  Silverlight is used to implement uploading multiple files, displaying
                    upload progress and block transfer.</p>
                    <p>Silverlight is not required for transferring files.  A link to install Silverlight on supported platforms is provided if it is not currrently enabled or installed.</p>
                    <h3>Operating on Multiple Files</h3>
                    <p>Most operations such as download and delete may be performed on one file at a time.  Use of an FTP client is recommended for transfering large numbers of files or directories.
                    Uploading multiple files is supported but requires Microsoft Silverlight.</p>
                </div>
            </td>
        </tr>
    </table>
    <hr />
    <h2 style="margin-top:20px;">Browser Layout</h2>
    <p>Files and folders are accessed through the browser display.</p>
    <img src="images/HelpLayout.png" width="808" height="369" style="display:block;margin:10px auto 20px auto;" alt="" title="Browser Layout" />
    <table style="padding:0px;border-collapse:collapse;">
        <tr>
            <td style="width:50%;vertical-align:top;">
                <div style="margin-right:20px;">
                    <h3>Folder Container</h3>
                    <p>The folder container displays a tree of the available share directories. Click a folder to view the available files in the file container.  
                    Click the + on a directory to expand and view sub-directories.</p>
                    <h3>File Container</h3>
                    <p>The file container displays icons representing the files within the selected folder.</p>
                    <h3>File</h3>
                    <p>Each file is represented by an icon.  A thumbnail is generated and displayed for each file that can be intepreted as an image.  Icons are defined for common file types.  Long filenames
                    are displayed as tooltips by hovering with the mouse.</p>
                </div>
            </td>
            <td style="width:50%;vertical-align:top;">
                <div style="margin-left:20px;">
                    <h3>Toolbar</h3>
                    <p>The toolbar provides access to the available actions.  Actions that are disabled are grayed out or will not appear on the toolbar.  The items on the toolbar are described below.</p>
                    <h3>Upload Panel</h3>
                    <p>The upload panel is available if the current user has write access to the file server.  Use the browse button to select a file on your local machine to upload to the file server.</p>
                </div>
            </td>
        </tr>
    </table>
    <hr />
    <h2 style="margin-top:20px;">Browser Toolbar</h2>
    <img src="images/HelpToolbar.png" width="835" height="103" style="display:block;margin:10px auto 20px auto;" alt="" title="Browser Toolbar" />
    <table style="padding:0px;border-collapse:collapse;">
        <tr>
            <td style="width:50%;vertical-align:top;margin-right:20px;">
                <div style="margin-right:20px;">
                    <h3>Path Box</h3>
                    <p>The path box provides a read-only display of the path to the actively displayed folder.</p>
                    <h3>Create Button</h3>
                    <p>The create button makes a new folder underneath the currently selected folder.  The name is entered on the tree display in the location where the folder will be created.</p>
                    <h3>Rename Button</h3>
                    <p>The rename button supports changing the name of the selected folder or file.</p>
                    <h3>Move Button</h3>
                    <p>The move button supports moving the selected folder or file to a new location in the shared file storage.</p>
                </div>
            </td>
            <td style="width:50%;vertical-align:top;margin-left:20px;">
                <div style="margin-left:20px;">
                    <h3>Delete Button</h3>
                    <p>The delete button removes the selected file from the remote server.</p>
                    <h3>Refresh Button</h3>
                    <p>The refresh button forces the contents of the current folder to be re-scanned and displayed.</p>
                    <h3>Download Button</h3>
                    <p>The download button may be used to start the download of the selected file.</p>
                    <h3>Filter Box</h3>
                    <p>The filter box is used to limit the files displayed in the file container.  Wildcards are supported.  For example *.jpg will display only the files with the .jpg extension.</p>
                </div>
            </td>
        </tr>
    </table>
</div>
</asp:Content>


