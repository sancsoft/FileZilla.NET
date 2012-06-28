/******************************************************************************
 * Filename: AdminUserView.aspx.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * View a user account
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/AdminUserNew.aspx.cs $
 * 
 * 8     5/13/12 12:24a Mterry
 * Safe account creation
 * 
 * 7     3/28/12 10:42p Mterry
 * Content fixes for notifications
 * 
 * 6     3/28/12 10:34p Mterry
 * notification support
 * 
 * 5     3/23/12 10:43p Mterry
 * 
 * 4     3/22/12 10:13p Mterry
 * admin testing and integration
 * 
 * 3     3/21/12 11:28p Mterry
 * create user account support
 * 
 * 2     3/18/12 12:46a Mterry
 * user creation development
 * 
 * 1     3/15/12 2:06a Mterry
 * 
 * 1     3/13/12 10:45p Mterry
 * 
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DevExpress.Data;
using System.IO;
using System.Text;
using System.Drawing;


public partial class AdminUserNew : System.Web.UI.Page
{
	/// <summary>
	/// object for accessing the site configuration
	/// </summary>
	protected WebSettings webSettings;

	/// <summary>
	/// Object for accessing the filezilla configuration
	/// </summary>
	protected Filezilla fz;

	protected void Page_Load(object sender, EventArgs e)
	{
		// wire the events
		CreateUserButton.Click += new EventHandler(CreateUserButton_Click);
		CancelButton.Click += new EventHandler(CancelButton_Click);
		UsernameDuplicate.ServerValidate += new ServerValidateEventHandler(UsernameDuplicate_ServerValidate);

		// this page requires an authenticated user
		Master.ForceAuthentication();

		// this page requires an authenticated administrator
		Master.ForceAdministrator();

		// create the configuration related objects
		webSettings = new WebSettings();
		fz = new Filezilla(webSettings.Lookup("ConfigurationFile"));

		// clear out the user create message
		CreateUserActionLabel.Text = "";

		if (!Page.IsPostBack)
		{
			UsernameTextBox.Text = "";
			PasswordTextBox.Text = "";
			RootFolderLiteral.Text = webSettings.Lookup("NewUserRoot");
		}
	}

	/// <summary>
	/// Return to the administration page
	/// </summary>
	/// <param name="sender">source</param>
	/// <param name="e">args</param>
	void CancelButton_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Admin.aspx");
	}

	/// <summary>
	/// Create a new account
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void CreateUserButton_Click(object sender, EventArgs e)
	{
		bool fileRead = false;
		bool fileWrite = false;
		bool fileDelete  = false;
		bool fileAppend = false;
		bool dirCreate = false;
		bool dirDelete = false;
		bool dirList = false;
		bool dirSubdirs = false;
		bool autoCreate = false;

		// check the validation 
		if (!UsernameDuplicate.IsValid)
		{
			return;
		}

		try
		{
			// get the target username
			string username = UsernameTextBox.Text.Trim().ToLower();
			string validChars = "abcdefghijklmnopqrstuvwxyz_0123456789";
			// check each character to make sure it is permissable
			foreach (char ch in username)
			{
				if (validChars.Contains(ch) == false)
				{
					throw new Exception("Usernames may only contain letters, numbers or underscore.");
				}
			}

			// check for duplicates - don't allow creation
			if (fz.UserExists(username))
			{
				throw new Exception("Username already exists.  Duplicate accounts cannot be created."); 
			}

			// get the password - generate one if it is blank
			string password = PasswordTextBox.Text.Trim();
			if (password == "")
			{
				password = GeneratePassword();
                PasswordTextBox.Text = password;
			}
			else
			{
				if ( password.Length < WebConvert.ToInt32(webSettings.Lookup("", "6"), 6))
				{
					throw new Exception("Requested password does not meet criteria");
				}
			}

			bool enabled = EnabledCheckBox.Checked;

			// parse out permissions
			if (PermissionsList.SelectedValue == "RO")
			{
				fileRead = true;
				fileWrite = false;
				fileDelete = false;
				fileAppend = false;
				dirCreate = false;
				dirDelete = false;
				dirList = true;
				dirSubdirs = true;
			}
			else if (PermissionsList.SelectedValue == "RW")
			{
				fileRead = true;
				fileWrite = true;
				fileDelete = true;
				fileAppend = true;
				dirCreate = true;
				dirDelete = true;
				dirList = true;
				dirSubdirs = true;
			}
			else // custom permissions
			{
				fileRead = FileReadCheckBox.Checked;
				fileWrite = FileWriteCheckBox.Checked;
				fileDelete = FileDeleteCheckBox.Checked;
				fileAppend = FileAppendCheckBox.Checked;
				dirCreate = DirCreateCheckBox.Checked;
				dirDelete = DirDeleteCheckBox.Checked;
				dirList = DirListCheckBox.Checked;
				dirSubdirs = DirSubdirsCheckBox.Checked;
			}

			// create the user account
			if (!fz.CreateUser(username, password, enabled, fileRead, fileWrite, fileDelete, fileAppend, dirCreate, dirDelete, dirList, dirSubdirs, autoCreate))
			{
				throw new Exception("Create user failed on FileZilla object.");
			}
			fz.WriteConfiguration(webSettings.Lookup("ConfigurationFile"));

			// force the server to re-read the settings file
			fz.ForceServerConfigurationLoad();

			// message the user
			CreateUserActionLabel.Text = "Account '" + UsernameTextBox.Text + "' created with password '" + password + "'.";
			CreateUserActionLabel.ForeColor = ColorTranslator.FromHtml("#339933");

			// send the notifications
			AccountNotification(UsernameTextBox.Text, password, NotificationTextBox.Text.Trim());
		}
		catch (Exception exception)
		{
			// message the user
			CreateUserActionLabel.Text = "Exception: " + exception.Message;
			CreateUserActionLabel.ForeColor = ColorTranslator.FromHtml("#cc3333");
		}


	}

	/// <summary>
	/// Randomly generate a password
	/// </summary>
	/// <returns>password</returns>
	public string GeneratePassword()
	{
		char ch;

		// use a string builder for character manipulation
		StringBuilder pw = new StringBuilder();
		// seed the random number generator with time
		Random r = new Random();

		// unambiguous alphabetic character set - removes o, l 
		string unambiguousAlpha = "abcdefghijkmnpqrstuvwxyz";
		// unambiguous alphabetic number set - removes 0, 1 
		string unambiguousNumeric = "23456789";

		// make a password of format #a#a#a#a
		for (int i = 0; i < 4; i++)
		{
			ch = unambiguousNumeric[Convert.ToInt32(r.Next(unambiguousNumeric.Length - 1))];
			pw.Append(ch);
			ch = unambiguousAlpha[Convert.ToInt32(r.Next(unambiguousAlpha.Length - 1))];
			pw.Append(ch);
		}

		return pw.ToString();
	}

	/// <summary>
	/// Make sure that the account doesn't already exist
	/// </summary>
	/// <param name="source"></param>
	/// <param name="args"></param>
	void UsernameDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
	{
		args.IsValid = true;
		string name = args.Value.Trim().ToLower();

		// Check for an existing account
		if (fz.UserExists(name))
		{
			args.IsValid = false;
		}
	}

	/// <summary>
	/// Send the new account invitation
	/// </summary>
	/// <param name="username">account</param>
	/// <param name="password">password (unencrypted)</param>
	/// <returns></returns>
	protected void AccountNotification(string username, string password, string recipients)
	{
		WebSettings ws = new WebSettings();

		string subject = "[FILEZILLA] Account Information (" + username + ")";
		StringBuilder sb = new StringBuilder();

		// add the welcome text to the message
		sb.Append(ws.Lookup("Welcome"));
		sb.Append("\n");
		sb.Append("\n");

		// message the user
		sb.Append("A new account for the " + ws.Lookup("InstanceName") + " FileZilla system has been created for your use.\n");
		sb.Append("\n");

		// add in the account information
		sb.Append("Username: " + username + "\n");
		sb.Append("Password: " + password + "\n");
		sb.Append("\n");

		sb.Append("FileZilla.NET Access Links\n");
		sb.Append("Web Interface: " + ws.Lookup("HTTPUrl") + "\n");
		sb.Append("FTP Interface: " + ws.Lookup("FTPUrl") + "\n");
		sb.Append("\n");

		// send out notifications only if specified
		if (recipients.Trim() != "")
		{
			// send to the designate recipients
			EMail.Send(subject, sb.ToString(), recipients);
			// send to the administrator
			sb.Append("\nNotifications: " + recipients + "\n");
			EMail.Send(subject, sb.ToString(), ws.Lookup("EmailRecipients"));
		}
	}
}