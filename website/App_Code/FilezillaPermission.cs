/******************************************************************************
 * Filename: FilezillaPermission.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Object representing a filezilla user account
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/FilezillaPermission.cs $
 * 
 * 4     3/21/12 11:28p Mterry
 * create user account support
 * 
 * 3     3/12/12 11:32p Mterry
 * admin interface development and binding
 * 
 * 2     3/11/12 3:26p Mterry
 * aliases
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
/// Represents a Filezilla shared folder and associated permissions
/// </summary>
public class FilezillaPermission
{
	/// <summary>
	/// Properties
	/// </summary>
	public string Dir { get; set; }
	public bool OptionFileRead { get; set; }
	public bool OptionFileWrite { get; set; }
	public bool OptionFileDelete { get; set; }
	public bool OptionFileAppend { get; set; }
	public bool OptionDirCreate { get; set; }
	public bool OptionDirDelete { get; set; }
	public bool OptionDirList { get; set; }
	public bool OptionDirSubdirs { get; set; }
	public bool OptionIsHome { get; set; }
	public bool OptionAutoCreate { get; set; }

	/// <summary>
	/// Return the alias set as a delimited path
	/// </summary>
	public string AliasPath
	{
		get
		{
			StringBuilder path = new StringBuilder();
			foreach (string alias in Aliases)
			{
				if (path.Length > 0 )
				{
					path.Append( "; " );
				}
				path.Append(alias);
			}
			return path.ToString();
		}
	}

	/// <summary>
	/// Public members
	/// </summary>
	public List<String> Aliases;

	/// <summary>
	/// Default constructor
	/// </summary>
	public FilezillaPermission()
	{
		Aliases = new List<string>();
		Defaults();
	}

	/// <summary>
	///  Assign default values to the permission object
	/// </summary>
	public void Defaults()
	{
		// default the corresponding directory
		Dir = "";

		// default the options that are supported
		OptionFileRead = false;
		OptionFileWrite = false;
		OptionFileDelete = false;
		OptionFileAppend = false;
		OptionDirCreate = false;
		OptionDirDelete = false;
		OptionDirList = false;
		OptionDirSubdirs = false;
		OptionIsHome = false;
		OptionAutoCreate = false;
	}

	/// <summary>
	/// Create a node containing the permission
	/// </summary>
	/// <param name="doc">XML configuration document</param>
	/// <returns>base node of new permission</returns>
	public XmlNode Create(XmlDocument doc)
	{
		// create the permission node
		XmlNode perm = doc.CreateNode(XmlNodeType.Element, "Permission", null);

		// assign the directory attribute
		XmlAttribute dir = doc.CreateAttribute("Dir");
		dir.Value = Dir;
		perm.Attributes.Append(dir);

		// create and add the options
		perm.AppendChild(CreateOption(doc, "FileRead",OptionFileRead));
		perm.AppendChild(CreateOption(doc, "FileWrite", OptionFileWrite));
		perm.AppendChild(CreateOption(doc, "FileDelete", OptionFileDelete));
		perm.AppendChild(CreateOption(doc, "FileAppend", OptionFileAppend));
		perm.AppendChild(CreateOption(doc, "DirCreate", OptionDirCreate));
		perm.AppendChild(CreateOption(doc, "DirDelete", OptionDirDelete));
		perm.AppendChild(CreateOption(doc, "DirList", OptionDirList));
		perm.AppendChild(CreateOption(doc, "DirSubdirs", OptionDirSubdirs));
		perm.AppendChild(CreateOption(doc, "IsHome", OptionIsHome));
		perm.AppendChild(CreateOption(doc, "AutoCreate", OptionAutoCreate));

		// add the aliases
		if (Aliases.Count > 0)
		{
			XmlNode aliasesNode = doc.CreateElement("Aliases");
			foreach (string alias in Aliases)
			{
				XmlNode aliasNode = doc.CreateElement("Alias");
				aliasNode.Value = alias;
				aliasesNode.AppendChild(aliasNode);
			}
			perm.AppendChild(aliasesNode);
		}
		return perm;
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
	/// <param name="node">xml root node of permission object</param>
	/// <returns>result of loading</returns>
	public bool Read(XmlNode perm)
	{
		try
		{
			// get the corresponding directory
			Dir = perm.SelectSingleNode("./@Dir").ChildNodes[0].Value;

			// get the options that are supported
			OptionFileRead = (perm.SelectSingleNode("./Option[@Name='FileRead']").ChildNodes[0].Value == "1");
			OptionFileWrite = (perm.SelectSingleNode("./Option[@Name='FileWrite']").ChildNodes[0].Value == "1");
			OptionFileDelete = (perm.SelectSingleNode("./Option[@Name='FileDelete']").ChildNodes[0].Value == "1");
			OptionFileAppend = (perm.SelectSingleNode("./Option[@Name='FileAppend']").ChildNodes[0].Value == "1");
			OptionDirCreate = (perm.SelectSingleNode("./Option[@Name='DirCreate']").ChildNodes[0].Value == "1");
			OptionDirDelete = (perm.SelectSingleNode("./Option[@Name='DirDelete']").ChildNodes[0].Value == "1");
			OptionDirList = (perm.SelectSingleNode("./Option[@Name='DirList']").ChildNodes[0].Value == "1");
			OptionDirSubdirs = (perm.SelectSingleNode("./Option[@Name='DirSubdirs']").ChildNodes[0].Value == "1");
			OptionIsHome = (perm.SelectSingleNode("./Option[@Name='IsHome']").ChildNodes[0].Value == "1");
			OptionAutoCreate = (perm.SelectSingleNode("./Option[@Name='AutoCreate']").ChildNodes[0].Value == "1");

			// read in any defined aliases
			Aliases.Clear();
			XmlNode aliasesNode = perm.SelectSingleNode("./Aliases");
			if (aliasesNode != null)
			{
				XmlNodeList aliases = aliasesNode.SelectNodes("./Alias");
				// walk the collection of accessible folders
				foreach (XmlNode alias in aliases)
				{
					Aliases.Add(alias.ChildNodes[0].Value);
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
	/// Update the permissions in an XML node from the object
	/// </summary>
	/// <param name="doc">xml document</param>
	/// <param name="node">xml root node of permission object</param>
	/// <returns>result of update</returns>
	public bool Update(XmlDocument doc, XmlNode perm)
	{
		try
		{
			// set the corresponding directory
			perm.SelectSingleNode("./@Dir").ChildNodes[0].Value = Dir;

			// update the options that are supported
			perm.SelectSingleNode("./Option[@Name='FileRead']").ChildNodes[0].Value = (OptionFileRead) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='FileWrite']").ChildNodes[0].Value = (OptionFileWrite) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='FileDelete']").ChildNodes[0].Value = (OptionFileDelete) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='FileAppend']").ChildNodes[0].Value = (OptionFileAppend) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='DirCreate']").ChildNodes[0].Value = (OptionDirCreate) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='DirDelete']").ChildNodes[0].Value = (OptionDirDelete) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='DirList']").ChildNodes[0].Value = (OptionDirList) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='DirSubdirs']").ChildNodes[0].Value = (OptionDirSubdirs) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='IsHome']").ChildNodes[0].Value = (OptionIsHome) ? "1" : "0";
			perm.SelectSingleNode("./Option[@Name='AutoCreate']").ChildNodes[0].Value = (OptionAutoCreate) ? "1" : "0";

			// remove any alias definitions
			XmlNode aliasesNode = perm.SelectSingleNode("./Aliases");
			perm.RemoveChild(aliasesNode);

			// if there are aliases, create the node and children
			if (Aliases.Count > 0)
			{
				aliasesNode = doc.CreateElement("Aliases");
				foreach (string alias in Aliases)
				{
					XmlNode aliasNode = doc.CreateElement("Alias");
					aliasNode.Value = alias;
					aliasesNode.AppendChild(aliasNode);
				}
				perm.AppendChild(aliasesNode);
			}
		}
		catch (Exception)
		{
			return false;
		}

		return true;
	}
}