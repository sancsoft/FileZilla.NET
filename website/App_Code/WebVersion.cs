/******************************************************************************
 * Filename: WebVersion.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Provide access to the version history information for the web site project 
 * stored in App_Data/history.xml
 * 
 * The label of the latest version is cached in the Application object the 
 * first time that it is requested to avoid thrashing the history file.
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/WebVersion.cs $
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
using System.Xml.Linq;
using System.Text;

public static class WebVersion
{
    const string UnknownVersionLabel = "0.0.0.0";

    /// <summary>
    /// Get the latest version number as a string label by reading the contents of the
    /// history.xml file in App_Data
    /// </summary>
    /// <returns>version label (major.minor.patch.build)</returns>
    public static string Latest( )
    {
        string strVersion;

        // get the version out of the application object
        if ((strVersion = WebConvert.ToString( HttpContext.Current.Application["WEB_VERSION"], "" )) == "" )
        {
            // version not defined - read it out of the history file
            try
            {
                // get version information from the history.xml file in App_Data
                XDocument versionXML = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/history.xml"));

                // get the latest version by highest build number
                var ver = (from v in versionXML.Descendants("version")
                           orderby v.Attribute("build").Value descending
                           select new
                           {
                               major = v.Attribute("major").Value,
                               minor = v.Attribute("minor").Value,
                               patch = v.Attribute("patch").Value,
                               build = v.Attribute("build").Value
                           }).First();
                strVersion = ver.major + "." + ver.minor + "." + ver.patch + "." + ver.build;
            }
            catch (Exception)
            {
                // can't get the latest version - default it to all zeros (an invalid version)
                strVersion = UnknownVersionLabel;
            }

            // store the version in the application object
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["WEB_VERSION"] = strVersion;
            HttpContext.Current.Application.UnLock();
        }
        return strVersion;
    }
}