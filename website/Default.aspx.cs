/******************************************************************************
 * Filename: Default.aspx.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Home page for the web interface
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/Default.aspx.cs $
 * 
 * 10    5/13/12 1:01a Mterry
 * 
 * 9     5/13/12 12:45a Mterry
 * log downloads
 * 
 * 8     4/21/12 11:13p Mterry
 * large file handling
 * 
 * 7     4/21/12 10:20p Mterry
 * process large downloads a block at a time
 * 
 * 6     3/07/12 11:16p Mterry
 * updates based on inital alpha testing
 * 
 * 5     3/07/12 12:14a Mterry
 * logging and landing page information
 * 
 * 4     3/06/12 7:18p Mterry
 * custom icon support
 * 
 * 3     3/05/12 11:38p Mterry
 * link to permissions
 * 
 * 2     2/29/12 11:05p Mterry
 * additional settings to support advanced upload
 * 
 * 1     2/26/12 12:19p Mterry
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DevExpress.Web.ASPxFileManager;
using System.IO;

public partial class _Default : System.Web.UI.Page
{
	/// <summary>
	/// object for accessing the site configuration
	/// </summary>
	protected WebSettings webSettings;

	/// <summary>
	/// Object for accessing the filezilla configuration
	/// </summary>
	protected Filezilla fz;

	/// <summary>
	/// Object for logging activity
	/// </summary>
	protected ActivityLog log;

	/// <summary>
	/// The root folder for the file manager control based on the current user permissions
	/// </summary>
	protected string rootFolder;

	/// <summary>
	/// Flag to determine if files are allowed to be replaced
	/// </summary>
	protected bool allowReplace = false;

    protected void Page_Load(object sender, EventArgs e)
    {
		// this page requires an authenticated user
		Master.ForceAuthentication();

		// wire the file manager events
		FileManager.CustomThumbnail += new FileManagerThumbnailCreateEventHandler(FileManager_CustomThumbnail);
		FileManager.FileDownloading += new FileManagerFileDownloadingEventHandler(FileManager_FileDownloading);
		FileManager.FileUploading += new FileManagerFileUploadEventHandler(FileManager_FileUploading);
		FileManager.FolderCreating += new FileManagerFolderCreateEventHandler(FileManager_FolderCreating);
		FileManager.ItemDeleting += new FileManagerItemDeleteEventHandler(FileManager_ItemDeleting);
		FileManager.ItemMoving += new FileManagerItemMoveEventHandler(FileManager_ItemMoving);
		FileManager.ItemRenaming += new FileManagerItemRenameEventHandler(FileManager_ItemRenaming);

		// create the configuration related objects
		webSettings = new WebSettings();
		fz = new Filezilla(webSettings.Lookup("ConfigurationFile"));

		// create the activity related objects
		log = new ActivityLog();

		// get the root folder based on the authenticated user
		rootFolder = fz.HomeDirectory(Master.UserName);

		if (!Page.IsPostBack)
		{
			// configure the general settings of the file manager 
			FileManager.SettingsEditing.AllowCreate = WebConvert.ToBoolean(webSettings.Lookup("AllowCreate", "0"), false);
			FileManager.SettingsEditing.AllowDelete = WebConvert.ToBoolean(webSettings.Lookup("AllowDelete", "0"), false);
			FileManager.SettingsEditing.AllowMove = WebConvert.ToBoolean(webSettings.Lookup("AllowMove", "0"), false);
			FileManager.SettingsEditing.AllowRename = WebConvert.ToBoolean(webSettings.Lookup("AllowRename", "0"), false);
			FileManager.SettingsFolders.ShowFolderIcons = WebConvert.ToBoolean(webSettings.Lookup("ShowFolderIcons", "0"), false);
			FileManager.SettingsToolbar.ShowDownloadButton = WebConvert.ToBoolean(webSettings.Lookup("ShowDownloadButton", "0"), false);
			FileManager.Settings.ThumbnailFolder = webSettings.Lookup("ThumbnailFolder");
			// advanced upload mode requires Microsoft Silverlight ... really?
			if (Master.IsSilverlightAvailable)
			{
				// silverlight is available - check the system configuration to see if we want to utilize it
				FileManager.SettingsUpload.UseAdvancedUploadMode = WebConvert.ToBoolean(webSettings.Lookup("UseAdvancedUploadMode", "0"), false);
				FileManager.SettingsUpload.AdvancedModeSettings.EnableMultiSelect = WebConvert.ToBoolean(webSettings.Lookup("EnableMultiSelect", "0"), false);
			}
			else
			{
				// no silverlight - no advanced upload mode
				FileManager.SettingsUpload.UseAdvancedUploadMode = false;
				FileManager.SettingsUpload.AdvancedModeSettings.EnableMultiSelect = false;

				// if the user could be utilizing silverlight, let them know
				if (WebConvert.ToBoolean(webSettings.Lookup("UseAdvancedUploadMode", "0"), false))
				{
					SilverlightPanel.Visible = true;
				}
			}
			// set the max file size from the settings, default to 1MB
			FileManager.SettingsUpload.ValidationSettings.MaxFileSize = WebConvert.ToInt32(webSettings.Lookup("MaxFileSize","1000000"),1000000);
 
			// limit permissions based on the settings for the ftp share

			// disable creating folders if its not enabled in the configuration
			if (!fz.AllowDirCreate(Master.UserName, rootFolder))
			{
				FileManager.SettingsEditing.AllowCreate = false;
			}

			// disable deleting, moving items if its not enabled in the configuration
			if (!fz.AllowFileDelete(Master.UserName, rootFolder))
			{
				FileManager.SettingsEditing.AllowDelete = false;
				FileManager.SettingsEditing.AllowMove = false;
			}
			
			// disable upload, move, rename, delete if write is not enabled
			if (!fz.AllowFileWrite(Master.UserName, rootFolder))
			{
				FileManager.SettingsUpload.Enabled = false;
				FileManager.SettingsEditing.AllowMove = false;
				FileManager.SettingsEditing.AllowRename = false;
				FileManager.SettingsEditing.AllowDelete = false;
			}

			// assign the root folder to the user's normalized home directory
			FileManager.Settings.RootFolder = fz.NormalizeFolderName(rootFolder);


			// log the browse action
			log.LogActivity(LogAction.Browse, Master.UserName, FileManager.Settings.RootFolder, "Silverlight=" + WebConvert.ToString( Session["FZNET_SILVERLIGHT"], "false") );
		}
    }

	/// <summary>
	/// Assign a custom icon based on the file extension
	/// </summary>
	/// <param name="source">event source</param>
	/// <param name="e">args - used to get file extension</param>
	void FileManager_CustomThumbnail(object source, FileManagerThumbnailCreateEventArgs e)
	{
		string iconRoot = "~/images/icons/";
		switch (e.File.Extension)
		{
			case ".xml":
			case ".log":
			case ".mdb":
				e.ThumbnailImage.Url = iconRoot + "database.png";
				break;
			case ".swf":
			case ".fla":
			case ".as":
				e.ThumbnailImage.Url = iconRoot + "flash.png";
				break;
			case ".avi":
			case ".mpg":
			case ".mpv":
			case ".mov":
			case ".wmf":
			case ".flv":
			case ".mpeg":
				e.ThumbnailImage.Url = iconRoot + "video.png";
				break;
			case ".zip":
			case ".7z":
			case ".gz":
			case ".tar":
				e.ThumbnailImage.Url = iconRoot + "zip.png";
				break;
			case ".htm":
			case ".html":
			case ".css":
				e.ThumbnailImage.Url = iconRoot + "web.png";
				break;
			case ".asp":
			case ".asa":
			case ".inc":
			case ".aspx":
			case ".asax":
			case ".ashx":
			case ".master":
			case ".cs":
			case ".cshtml":
			case ".config":
			case ".php":
			case ".pl":
				e.ThumbnailImage.Url = iconRoot + "webServer.png";
				break;
		}
	}

	/// <summary>
	/// A file is being downloaded, check to see if the user has read access
	/// Also overrides the download function to cut the file into 64k blocks to preserve memory
	/// </summary>
	/// <param name="source">event source</param>
	/// <param name="e">event args - used to identify target for read</param>
	void FileManager_FileDownloading(object source, FileManagerFileDownloadingEventArgs e)
	{
		if (!fz.AllowFileRead(Master.UserName, rootFolder))
		{
			e.Cancel = true;
			// track the download action
			ActivityLog log = new ActivityLog();
			log.LogActivity(LogAction.Download, Master.UserName, e.File.FullName, (e.Cancel) ? "Access denied" : "");
		}
		else
		{
			// download the file a block at a time to preserve memory
			byte[] buffer = new byte[1024*1024];
			HttpResponse response = Response;
			response.Clear();
			response.AddHeader("ContentType", string.Format("application/{0}", Path.GetExtension(e.File.Name).TrimStart('.')));
			response.AddHeader("Content-Transfer-Encoding", "binary");
			response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}",Server.UrlEncode(e.File.Name)));
			long dataToRead = e.InputStream.Length;
			response.AddHeader("Content-Length", string.Format("{0}",dataToRead));
			while (dataToRead > 0)
			{
				if ( response.IsClientConnected)
				{
					int length = e.InputStream.Read(buffer, 0, 1024 * 1024);
					response.OutputStream.Write(buffer, 0, length);
					response.Flush();
					dataToRead -= length;
				}
				else
				{
					dataToRead = -1;
				}
			}
			// clean up at end of response
			response.Flush();
			// track the download action
			ActivityLog log = new ActivityLog();
			log.LogActivity(LogAction.Download, Master.UserName, e.File.FullName, (e.Cancel) ? "Access denied" : "");
			// terminate the response - we passed back the file
			response.End();
		}
		e.Cancel = true;
	}

	/// <summary>
	/// Uploading a file - log the action
	/// </summary>
	/// <param name="source">source of event</param>
	/// <param name="e">contains the new file information</param>
	void FileManager_FileUploading(object source, FileManagerFileUploadEventArgs e)
	{
		// if the file already exists, delete it so we can upload the new file
		if (File.Exists( e.File.FullName ))
		{
			// make sure that we are configured to allow replacing files
			if (WebConvert.ToBoolean(webSettings.Lookup("AllowReplace", "0"), false))
			{
				// make sure that we are allowed to delete files
				if (fz.AllowFileDelete(Master.UserName, rootFolder))
				{
					File.Delete(e.File.FullName);
				}
			}
		}
		// log the upload request
		log.LogActivity(LogAction.Upload, Master.UserName, e.File.FullName, e.ErrorText);
	}

	/// <summary>
	/// Creating a folder - log the action
	/// </summary>
	/// <param name="source">source of event</param>
	/// <param name="e">contains the new folder information</param>
	void FileManager_FolderCreating(object source, FileManagerFolderCreateEventArgs e)
	{
		log.LogActivity(LogAction.CreateFolder, Master.UserName, e.ParentFolder + @"\" + e.Name, e.ErrorText);
	}

	/// <summary>
	/// Check to see if an item can be deleted; this is used to differentiate between deleting
	/// files and deleting folders, which are separate permissions in FileZilla
	/// </summary>
	/// <param name="source">event source</param>
	/// <param name="e">event args - used to identify target for deletion</param>
	void FileManager_ItemDeleting(object source, FileManagerItemDeleteEventArgs e)
	{
		// if the item is a folder, check for permission to delete folders
		if (e.Item is FileManagerFolder)
		{
			if (!fz.AllowDirDelete(Master.UserName, rootFolder))
			{
				e.Cancel = true;
				e.ErrorText = "Folders may not be deleted.";
			}
		}
		ActivityLog log = new ActivityLog();
		log.LogActivity(LogAction.Delete, Master.UserName, e.Item.FullName, e.ErrorText);
	}

	/// <summary>
	/// Move an item - log the action
	/// </summary>
	/// <param name="source">source of event</param>
	/// <param name="e">contains the old and new location</param>
	void FileManager_ItemMoving(object source, FileManagerItemMoveEventArgs e)
	{
		log.LogActivity(LogAction.Move, Master.UserName, e.Item.FullName + " => " + e.DestinationFolder, e.ErrorText);
	}

	/// <summary>
	/// Renaming an object - log the action
	/// </summary>
	/// <param name="source">source of event</param>
	/// <param name="e">contains the old and new names</param>
	void FileManager_ItemRenaming(object source, FileManagerItemRenameEventArgs e)
	{
		log.LogActivity(LogAction.Rename, Master.UserName, e.Item.FullName + " => " + e.NewName, e.ErrorText);
	}
}
