/******************************************************************************
 * Filename: Filezilla.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Access the filezilla configuration and translate the information into a 
 * form useful for controlling the web interface
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/Filezilla.cs $
 * 
 * 10    3/28/12 10:34p Mterry
 * notification support
 * 
 * 9     3/21/12 11:28p Mterry
 * create user account support
 * 
 * 8     3/18/12 12:46a Mterry
 * user creation development
 * 
 * 7     3/13/12 11:08p Mterry
 * support reloading the configuration on password changes
 * 
 * 6     3/13/12 10:46p Mterry
 * account view and set password support under development
 * 
 * 5     3/12/12 11:32p Mterry
 * admin interface development and binding
 * 
 * 4     3/11/12 3:25p Mterry
 * get user
 * 
 * 3     3/11/12 12:18a Mterry
 * check for enabled; administration authentication
 * 
 * 2     3/05/12 11:38p Mterry
 * link to permissions
 * 
 * 1     2/26/12 12:20p Mterry
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

/// <summary>
/// Summary description for Filezilla
/// </summary>
public class Filezilla
{
	protected XmlDocument config;


	public Filezilla( string configurationFile )
	{
		// load the configuration xml
		config = new XmlDocument();
		config.Load(configurationFile);
	}

	
	/// <summary>
	/// Calculated the md5 encrypted version of a string
	/// </summary>
	/// <param name="str">string to encrypt</param>
	/// <returns>encrypted string in hascii</returns>
	static public string GetMd5Sum(string str)
	{
		// First we need to convert the string into bytes, which
		// means using a text encoder.
		Encoder enc = System.Text.Encoding.UTF8.GetEncoder();

		// Create a buffer large enough to hold the string
		byte[] unicodeText = new byte[str.Length];
		enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

		// Now that we have a byte array we can ask the CSP to hash it
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] result = md5.ComputeHash(unicodeText);

		// Build the final string by converting each byte
		// into hex and appending it to a StringBuilder
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < result.Length; i++)
		{
			sb.Append(result[i].ToString("x2"));
		}

		// return the encrypted string in hex ascii (hascii)
		return sb.ToString();
	}

	/// <summary>
	/// Normalize the definition of a folder to something we can work with
	/// </summary>
	/// <param name="rawFolder">raw folder definition</param>
	/// <returns>usable path</returns>
	public string NormalizeFolderName(string rawFolder)
	{
		string normFolder = rawFolder;

		if (normFolder.Length > 0)
		{
			// if we have specified a drive - give back the root of the drive
			if (normFolder.Substring(normFolder.Length - 1, 1) == ":")
			{
				normFolder += "\\";
			}
		}
		else
		{
			normFolder = ".";
		}
		return normFolder;
	}

	/// <summary>
	/// Attempt to authenticate using the provided account and password
	/// </summary>
	/// <param name="username">user name for login</param>
	/// <param name="password">clear text password</param>
	/// <returns>true if login is valid</returns>
	public bool Authenticate(string username, string password)
	{
		bool ret = false;

		try
		{
			// encrypt the provided password
			string hashedword = GetMd5Sum(password);
			
			// look up the account
			XmlNode user = config.SelectSingleNode("/FileZillaServer/Users/User[@Name='" + username + "']");

			// verify that the user account is enabled
			XmlNode enabled = user.SelectSingleNode("./Option[@Name='Enabled']");
			if (enabled.ChildNodes[0].Value != "1")
			{
				throw new Exception("Account is disabled");
			}

			// get the stored password for the account
			XmlNode pass = user.SelectSingleNode("./Option[@Name='Pass']");

			// check the encrypted password supplied for an exact match
			if (hashedword == pass.ChildNodes[0].Value)
			{
				ret = true;
			}
		}
		catch (Exception)
		{
			ret = false;
		}
		return ret;
	}

	/// <summary>
	/// Authenticate as the server administrator - for some reason, this isn't encrypted
	/// </summary>
	/// <param name="password"></param>
	/// <returns></returns>
	public bool AuthenticateAdministration(string password)
	{
		bool ret = false;
		try
		{
			// look up the administration password
			XmlNode pass = config.SelectSingleNode("/FileZillaServer/Settings/Item[@name='Admin Password']");

			// check the encrypted password supplied for an exact match
			if (password == pass.ChildNodes[0].Value)
			{
				ret = true;
			}
		}
		catch (Exception)
		{
			ret = false;
		}
		return ret;
	}

	/// <summary>
	/// Retrieve this user's home directory from the configuration.  Throws an exception if the 
	/// user is not defined
	/// </summary>
	/// <param name="username">account</param>
	/// <returns>home directory for user</returns>
	public string HomeDirectory(string username)
	{
		// look up the user in the configuration
		XmlNode user = config.SelectSingleNode("/FileZillaServer/Users/User[@Name='" + username + "']");
		/// get a collection of all accessible folders
		XmlNodeList folders = user.SelectNodes("./Permissions/Permission");
		// walk the collection of accessible folders
		foreach (XmlNode folder in folders)
		{
			// the home directory is marked with an option IsHome = 1
			if (folder.SelectSingleNode("./Option[@Name='IsHome']").ChildNodes[0].Value == "1")
			{
				// get the directory associated with this folder - Dir attribute on the permission entry
				XmlNode root = folder.SelectSingleNode("./@Dir");
				// return the directory value as our root
				return folder.SelectSingleNode("./@Dir").ChildNodes[0].Value;
			}
		}
		// the home directory for the user could not be found
		throw new Exception("Home directory not found for account '" + username + "'");
	}

	/// <summary>
	/// Check an permission flag 
	/// </summary>
	/// <param name="username">account</param>
	/// <param name="foldername">dir for permissions</param>
	/// <param name="permission">specific permission</param>
	/// <returns>true if permission value is 1</returns>
	protected bool CheckPermission(string username, string foldername, string permission)
	{
		bool ret = false;				// assume no permission

		try
		{
			// look up the user in the configuration
			XmlNode user = config.SelectSingleNode("/FileZillaServer/Users/User[@Name='" + username + "']");
			// get the target folder
			XmlNode folder = user.SelectSingleNode("./Permissions/Permission[@Dir='" + foldername + "']");
			// get the permission for the folder
			ret = (folder.SelectSingleNode("./Option[@Name='" + permission + "']").ChildNodes[0].Value == "1");
		}
		catch (Exception)
		{
			// if we encounter an error - permission could not be found
			ret = false;
		}
		return ret;
	}

	public bool AllowFileRead(string username, string foldername)
	{
		return CheckPermission(username, foldername, "FileRead");
	}

	public bool AllowFileWrite(string username, string foldername)
	{
		return CheckPermission(username, foldername, "FileWrite");
	}

	public bool AllowFileDelete(string username, string foldername)
	{
		return CheckPermission(username, foldername, "FileDelete");
	}

	public bool AllowDirCreate(string username, string foldername)
	{
		return CheckPermission(username, foldername, "DirCreate");
	}

	public bool AllowDirDelete(string username, string foldername)
	{
		return CheckPermission(username, foldername, "DirDelete");
	}

	public bool AllowDirList(string username, string foldername)
	{
		return CheckPermission(username, foldername, "DirList");
	}

	public bool AllowDirSubdirs(string username, string foldername)
	{
		return CheckPermission(username, foldername, "DirSubdirs");
	}

	public bool IsHome(string username, string foldername)
	{
		return CheckPermission(username, foldername, "IsHome");
	}

	public bool AutoCreate(string username, string foldername)
	{
		return CheckPermission(username, foldername, "AutoCreate");
	}

	/// <summary>
	/// Retrieve a user object by user name.  This will throw if the 
	/// user cannot be found
	/// </summary>
	/// <param name="userName">target user</param>
	/// <returns>filezilla user object</returns>
	public FilezillaUser GetUser(string userName)
	{
		FilezillaUser fzu = new FilezillaUser();
	
		// look up the user in the configuration
		XmlNode userNode = config.SelectSingleNode("/FileZillaServer/Users/User[@Name='" + userName + "']");
		if (!fzu.Read(userNode))
		{
			throw new Exception("Unable to parse user record");
		}

		return fzu;
	}

	/// <summary>
	/// Check to see if a valid user exists
	/// </summary>
	/// <param name="userName"></param>
	/// <returns></returns>
	public bool UserExists(string userName)
	{
		FilezillaUser fzu = new FilezillaUser();

		// look up the user in the configuration
		XmlNode userNode = config.SelectSingleNode("/FileZillaServer/Users/User[@Name='" + userName + "']");
		return (userNode != null);
	}

	/// <summary>
	/// Add the users in the file to a list of users
	/// </summary>
	/// <param name="userList">list to receive users</param>
	/// <returns>count of users added</returns>
	public int AddUsersToList(ref List<FilezillaUser> userList)
	{
		int count = 0;

		/// get a collection of all users
		XmlNodeList users = config.SelectNodes("/FileZillaServer/Users/User");

		// walk the collection of accessible folders
		foreach (XmlNode user in users)
		{
			FilezillaUser fzu = new FilezillaUser();
			if (fzu.Read(user))
			{
				userList.Add(fzu);
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// Force the server to reload the configuration
	/// </summary>
	/// <returns>true if configuration reloaded</returns>
	public bool ForceServerConfigurationLoad()
	{
		WebSettings ws = new WebSettings();
		string serverExe = ws.Lookup("ServerExecutable");
		Process.Start(serverExe, "/reload-config");
		return true;
	}

	/// <summary>
	/// Write the XML back to the configuration file
	/// </summary>
	/// <param name="filename">destination</param>
	public void WriteConfiguration(string filename)
	{
		config.Save(filename);
	}

	/// <summary>
	/// Notify the system administration email address of account information
	/// </summary>
	/// <param name="action">action description</param>
	/// <param name="username">account</param>
	/// <param name="password">password (unencrypted)</param>
	/// <returns></returns>
	protected void AccountNotification(string action, string username, string password)
	{
		WebSettings ws = new WebSettings();

		string subject = "[FILEZILLA] Account Notification (" + username + ")";
		StringBuilder sb = new StringBuilder();

		sb.Append( "FileZilla account notification message for the system administrator:\n");
		sb.Append( "Action: " + action + "\n" );
		sb.Append( "Username: " + username + "\n");
		sb.Append( "Password: " + password + "\n");
		EMail.Send( subject, sb.ToString(), ws.Lookup( "EmailRecipients" ) );
	}

	/// <summary>
	/// Assign the supplied password to the supplied user account
	/// </summary>
	/// <param name="username">user account</param>
	/// <param name="password">new password</param>
	/// <returns>true if assignment succeeded</returns>
	public bool AssignUserPassword(string username, string password)
	{
		bool ret = false;

		try
		{
			// encrypt the provided password
			string hashedword = GetMd5Sum(password);

			// look up the account
			XmlNode user = config.SelectSingleNode("/FileZillaServer/Users/User[@Name='" + username + "']");

			// get the stored password for the account
			XmlNode pass = user.SelectSingleNode("./Option[@Name='Pass']");

			// assign the encrypted password
			pass.ChildNodes[0].Value = hashedword;

			// notify the administrator
			AccountNotification("Assign User Password", username, password);

			ret = true;
		}
		catch (Exception)
		{
			ret = false;
		}
		return ret;
	}

	/// <summary>
	/// Create a new user account
	/// </summary>
	/// <param name="username"></param>
	/// <param name="password"></param>
	/// <returns></returns>
	public bool CreateUser(string username, string password, bool enabled, bool fileRead, bool fileWrite, bool fileDelete, bool fileAppend, 
		bool dirCreate, bool dirDelete, bool dirList, bool dirSubdirs, bool autoCreate)
	{
		WebSettings ws = new WebSettings();

		try
		{
			// create the new user 
			FilezillaUser fzu = new FilezillaUser();
			fzu.Username = username;
			fzu.Password = password;
			fzu.Enabled = enabled;

			// define the user's home directory
			string homeDir = ws.Lookup("NewUserRoot") + "\\" + username;

			// create the directory if it doesn't already exist
			if (!System.IO.Directory.Exists(homeDir))
			{
				System.IO.Directory.CreateDirectory(homeDir);
			}

			// create the user's home directory permission
			FilezillaPermission fzp = new FilezillaPermission();
			fzp.Dir = homeDir;
			fzp.OptionIsHome = true;
			fzp.OptionFileRead = fileRead;
			fzp.OptionFileWrite = fileWrite;
			fzp.OptionFileDelete = fileDelete;
			fzp.OptionFileAppend = fileAppend;
			fzp.OptionDirCreate = dirCreate;
			fzp.OptionDirDelete = dirDelete;
			fzp.OptionDirList = dirList;
			fzp.OptionDirSubdirs = dirSubdirs;
			fzp.OptionAutoCreate = autoCreate;

			// add the home directory to the user8
			fzu.Permissions.Add(fzp);

			// look up the user in the configuration
			XmlNode usersNode = config.SelectSingleNode("/FileZillaServer/Users");
			XmlNode newUserNode = fzu.Create(config);
			usersNode.AppendChild(newUserNode);

			// notify the administrator
			AccountNotification("Create User", username, password);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}
}