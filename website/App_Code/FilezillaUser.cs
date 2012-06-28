/******************************************************************************
 * Filename: FilezillaUser.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Object representing a filezilla user account
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/FilezillaUser.cs $
 * 
 * 4     3/21/12 11:28p Mterry
 * create user account support
 * 
 * 3     3/12/12 11:32p Mterry
 * admin interface development and binding
 * 
 * 2     3/11/12 3:26p Mterry
 * create permission list in constructor
 * 
 * 1     3/11/12 12:17a Mterry
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for FilezillaUser
/// </summary>
public class FilezillaUser
{
	public string Username { get; set; }
	public string Password { get; set; }
	public bool Enabled { get; set; }
	public string Home { get { return GetHomeDirectory();} }
	public List<FilezillaPermission> Permissions;

	public FilezillaUser()
	{
		Username = "";
		Password = "";
		Enabled = false;
		Permissions = new List<FilezillaPermission>();
	}

	/// <summary>
	/// Create a node containing the user
	/// </summary>
	/// <param name="doc">XML configuration document</param>
	/// <returns>base node of new permission</returns>
	public XmlNode Create(XmlDocument doc)
	{
		// create the permission node
		XmlNode user = doc.CreateNode(XmlNodeType.Element, "User", null);

		// assign the user name attribute
		XmlAttribute userName = doc.CreateAttribute("Name");
		userName.Value = Username;
		user.Attributes.Append(userName);

		// create the options
		XmlNode opt;
		opt = CreateOption(doc, "Pass", Filezilla.GetMd5Sum(Password));
		user.AppendChild(opt);
		opt = CreateOption(doc, "Group", "");
		user.AppendChild(opt);
		opt = CreateOption(doc, "Bypass server userlimit", false);
		user.AppendChild(opt);
		opt = CreateOption(doc, "User Limit", "0");
		user.AppendChild(opt);
		opt = CreateOption(doc, "IP Limit", "0");
		user.AppendChild(opt);
		opt = CreateOption(doc, "Enabled", Enabled);
		user.AppendChild(opt);
		opt = CreateOption(doc, "Comments", "FileZilla.NET user");
		user.AppendChild(opt);
		opt = CreateOption(doc, "ForceSsl", "0");
		user.AppendChild(opt);

		// create the ip filters
		XmlNode ipfilter;
		ipfilter = doc.CreateNode(XmlNodeType.Element, "IpFilter", null);
		ipfilter.AppendChild(doc.CreateNode(XmlNodeType.Element, "Disallowed", null));
		ipfilter.AppendChild(doc.CreateNode(XmlNodeType.Element, "Allowed", null));
		user.AppendChild(ipfilter);

		// create the permissions
		XmlNode permissions;
		permissions = doc.CreateNode(XmlNodeType.Element, "Permissions", null);
		foreach (FilezillaPermission p in Permissions)
		{
			permissions.AppendChild(p.Create(doc));
		}
		user.AppendChild(permissions);

		return user;
	}

	/// <summary>
	/// Creaate a string-based option for xml
	/// </summary>
	/// <param name="doc">XML configuration document</param>
	/// <param name="optionName">name attribute of option</param>
	/// <param name="optionValue">value of option</param>
	/// <returns></returns>
	public XmlNode CreateOption(XmlDocument doc, string optionName, string optionValue)
	{
		XmlNode opt = doc.CreateNode(XmlNodeType.Element, "Option", null);
		XmlAttribute name = doc.CreateAttribute("Name");
		XmlText value = doc.CreateTextNode(optionValue);
		name.Value = optionName;
		opt.Attributes.Append(name);
		opt.AppendChild(value);
		return opt;
	}

	/// <summary>
	/// Creaate a boolean option for xml
	/// </summary>
	/// <param name="doc">XML configuration document</param>
	/// <param name="optionName">name attribute of option</param>
	/// <param name="optionValue">value of option</param>
	/// <returns></returns>
	public XmlNode CreateOption(XmlDocument doc, string optionName, bool optionValue)
	{
		return CreateOption(doc, optionName, (optionValue == true) ? "1" : "0");
	}

	/// <summary>
	/// Load the object from an XML node
	/// </summary>
	/// <param name="node">xml root node of object</param>
	/// <returns>result of loading</returns>
	public bool Read(XmlNode user)
	{
		try
		{
			// get the username
			Username = user.SelectSingleNode("./@Name").ChildNodes[0].Value;

			// get the options that are supported
			Password = user.SelectSingleNode("./Option[@Name='Pass']").ChildNodes[0].Value;
			Enabled = (user.SelectSingleNode("./Option[@Name='Enabled']").ChildNodes[0].Value == "1");

			// get the shared folders
			XmlNodeList folders = user.SelectNodes("./Permissions/Permission");
			foreach (XmlNode folder in folders)
			{
				FilezillaPermission fzp = new FilezillaPermission();
				if ( fzp.Read(folder) )
				{
					Permissions.Add(fzp);
				}
			}
		}
		catch (Exception)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Get the home directory for the user
	/// </summary>
	/// <returns>home directory</returns>
	public string GetHomeDirectory()
	{
		string home = "n/a";
		foreach (FilezillaPermission perm in Permissions)
		{
			if (perm.OptionIsHome)
			{
				home = perm.Dir;
				break;
			}
		}
		return home;
	}
}