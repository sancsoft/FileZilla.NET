/******************************************************************************
 * Filename: WebSettings.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Provide access to the global settingsinformation for the web site project 
 * stored in App_Data/settings.xml
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/WebSettings.cs $
 * 
 * 1     2/26/12 12:20p Mterry
******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Text;

public class WebSettings
{
	protected XmlDocument settingsXML;
	/// <summary>
	/// Construct the web settings object.  If the settings are not loaded, load them
	/// </summary>
	public WebSettings()
	{
		settingsXML = new XmlDocument();
		settingsXML.Load(HttpContext.Current.Server.MapPath("~/App_Data/settings.xml"));
	}

	/// <summary>
	/// Get a setting as a string value; throw an exception if its not found
	/// </summary>
	/// <param name="key">key</param>
	/// <returns>value</returns>
	public string Lookup(string key)
	{
		XmlNode setting = settingsXML.SelectSingleNode( "/settings/setting[@key='" + key + "']" );
		return setting.ChildNodes[0].Value;
	}

	/// <summary>
	/// Get a setting as a string value or use default if setting is undefined
	/// </summary>
	/// <param name="key">key</param>
	/// <param name="defaultValue">value to use if setting not found</param>
	/// <returns>value</returns>
	public string Lookup(string key, string defaultValue)
	{
		string ret;
		try
		{
			ret = Lookup(key);
		}
		catch (Exception)
		{
			ret = defaultValue;
		}
		return ret;
	}
}

