/******************************************************************************
 * Filename: AdminUserView.aspx.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * View a user account
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/AdminUserView.aspx.cs $
 * 
 * 4     3/28/12 10:42p Mterry
 * Content fixes for notifications
 * 
 * 3     3/28/12 10:34p Mterry
 * notification support
 * 
 * 2     3/22/12 10:13p Mterry
 * admin testing and integration
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


public partial class AdminUserView : System.Web.UI.Page
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
	/// Target user object
	/// </summary>
	protected FilezillaUser fzu;

	/// <summary>
	/// Identifies the target
	/// </summary>
	protected string userName;

	protected void Page_Load(object sender, EventArgs e)
	{
		// wire the events
		GeneratePasswordButton.Click += new EventHandler(GeneratePasswordButton_Click);
		ForcePasswordButton.Click += new EventHandler(ForcePasswordButton_Click);

		// this page requires an authenticated user
		Master.ForceAuthentication();

		// this page requires an authenticated administrator
		Master.ForceAdministrator();

		// get the target user from the query string
		userName = WebConvert.ToString(Request.QueryString["ID"], "");

		// create the configuration related objects
		webSettings = new WebSettings();
		fz = new Filezilla(webSettings.Lookup("ConfigurationFile"));

		// load the user object
		fzu = fz.GetUser(userName);

		if (!Page.IsPostBack)
		{
			// load the display with the user values
			UsernameTitleLiteral.Text = fzu.Username;
			UsernameLabel.Text = fzu.Username;
			PasswordLabel.Text = fzu.Password;
			EnabledLabel.Text = (fzu.Enabled) ? "Yes" : "No";
			HomeDirectoryLabel.Text = fzu.Home;

			// load the first permission
			FilezillaPermission fzp = fzu.Permissions[0];
			DirectoryLabel.Text = fzp.Dir;
			AliasPathLabel.Text = fzp.AliasPath;
			IsHomeLabel.Text = (fzp.OptionIsHome) ? "Yes" : "No";
			FilePermissionsLabel.Text = "";
			FilePermissionsLabel.Text += DisplayPermission("Read", fzp.OptionFileRead );
			FilePermissionsLabel.Text += DisplayPermission("Write", fzp.OptionFileWrite);
			FilePermissionsLabel.Text += DisplayPermission("Delete", fzp.OptionFileDelete);
			FilePermissionsLabel.Text += DisplayPermission("Append", fzp.OptionFileAppend);
			DirPermissionsLabel.Text = "";
			DirPermissionsLabel.Text += DisplayPermission("List", fzp.OptionDirList);
			DirPermissionsLabel.Text += DisplayPermission("Create", fzp.OptionDirCreate);
			DirPermissionsLabel.Text += DisplayPermission("Delete", fzp.OptionDirDelete);
			DirPermissionsLabel.Text += DisplayPermission("Subdirs", fzp.OptionDirSubdirs);
			AutoCreateLabel.Text = (fzp.OptionAutoCreate) ? "Yes" : "No";
		}
	}

	/// <summary>
	/// Display a permission in standard format; strike it if its not available
	/// </summary>
	/// <param name="perm">Permission name</param>
	/// <param name="allow">true to allow</param>
	/// <returns>HTML to display permission</returns>
	public string DisplayPermission(string perm, bool allow)
	{
		if (allow)
		{
			return "" + perm + "&nbsp; ";
		}
		else
		{
			return "<strike>" + perm + "</strike>&nbsp; ";
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
	/// Force the user password to the specified value
	/// </summary>
	/// <param name="sender">source</param>
	/// <param name="e">event</param>
	void ForcePasswordButton_Click(object sender, EventArgs e)
	{
		string newPassword = NewPasswordTextBox.Text.Trim();
		string hashedNewPassword = Filezilla.GetMd5Sum(newPassword);

		// make sure that the new password is long enough to accept
		int minPasswordLength = WebConvert.ToInt32(webSettings.Lookup("MinPasswordLength", "8"), 8);
		if (newPassword.Length < minPasswordLength)
		{
			PasswordActionLabel.Text = "The minimum password length is " + minPasswordLength.ToString() + " characters.  The password has NOT been changed";
			PasswordActionLabel.ForeColor = ColorTranslator.FromHtml("#cc3333");
			return;
		}

		// update the display with the encrypted version
		PasswordLabel.Text = hashedNewPassword;

		// update the user object
		fzu.Password = hashedNewPassword;

		// write the modified settings file
		fz.AssignUserPassword(fzu.Username, newPassword);
		fz.WriteConfiguration(webSettings.Lookup("ConfigurationFile"));

		// force the server to re-read the settings file
		fz.ForceServerConfigurationLoad();

		// let the user know what happened
		PasswordActionLabel.Text = "The supplied password has been assigned to the account. The new password will be required by the user for the next login to web interface or FTP server.";
		PasswordActionLabel.ForeColor = ColorTranslator.FromHtml("#00aa00");
	}

	/// <summary>
	/// Generate a new password for the user; display in the new password box
	/// </summary>
	/// <param name="sender">source</param>
	/// <param name="e">event</param>
	void GeneratePasswordButton_Click(object sender, EventArgs e)
	{
		string newPassword = GeneratePassword();
		string hashedNewPassword = Filezilla.GetMd5Sum(newPassword);

		// display the new password to the administator - its lost once we hash it
		NewPasswordTextBox.Text = newPassword;
		
		// update the display with the encrypted version
		PasswordLabel.Text = hashedNewPassword;

		// update the user object
		fzu.Password = hashedNewPassword;

		// write the modified settings file
		fz.AssignUserPassword(fzu.Username, newPassword);
		fz.WriteConfiguration(webSettings.Lookup("ConfigurationFile"));

		// force the server to re-read the settings file
		fz.ForceServerConfigurationLoad();

		// send out the notifications
		AccountNotification( fzu.Username,newPassword, NotificationTextBox.Text.Trim());

		// let the user know what happened
		PasswordActionLabel.Text = "A new password has been randomly generated and assigned to the user. The assigned password is displayed in the New Password field.";
		PasswordActionLabel.ForeColor = ColorTranslator.FromHtml("#00aa00");
	}

	/// <summary>
	/// Send the new password notification message
	/// </summary>
	/// <param name="username">account</param>
	/// <param name="password">password (unencrypted)</param>
	/// <returns></returns>
	protected void AccountNotification(string username, string password, string recipients)
	{
		WebSettings ws = new WebSettings();

		string subject = "[FILEZILLA] Password Assignment (" + username + ")";
		StringBuilder sb = new StringBuilder();

		sb.Append("A new password has been assigned for your " + ws.Lookup("InstanceName" ) + " FileZilla account.\n\n");
		
		sb.Append("Username: " + username + "\n");
		sb.Append("Password: " + password + "\n");

		sb.Append("\n");
		sb.Append("Web Interface: " + ws.Lookup("HTTPUrl") + "\n");
		sb.Append("FTP Interface: " + ws.Lookup("FTPUrl") + "\n");

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