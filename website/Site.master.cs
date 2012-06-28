/******************************************************************************
 * Filename: Site.master.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Site base master page
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/Site.master.cs $
 * 
 * 3     3/11/12 10:12a Mterry
 * administration development
 * 
 * 2     3/07/12 11:17p Mterry
 * updates based on inital alpha testing
 * 
 * 1     2/26/12 12:20p Mterry
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class SiteMaster : System.Web.UI.MasterPage
{
	/// <summary>
	/// Account of the user currently logged in
	/// </summary>
	private string userName = "";
	public string UserName { get { return userName; } }

	/// <summary>
	/// Flag indicating if an adequate version of Silverlight is available
	/// </summary>
	private bool silverlightAvailable = false;
	public bool IsSilverlightAvailable { get { return silverlightAvailable; } }

	/// <summary>
	/// Flag indicating if an adequate version of Silverlight is available
	/// </summary>
	private bool administrator = false;
	public bool IsAdministrator { get { return administrator; } }

	/// <summary>
	/// Master initialization required before child page loads
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		// get the current user from the session - initialize to blank for no user
		userName = WebConvert.ToString(Session["FZNET_USER"], "");
		// get the silverlight flag - it will only be true once it has been detected
		silverlightAvailable = WebConvert.ToBoolean(Session["FZNET_SILVERLIGHT"], false);
		// get the administration authentication flag
		administrator = WebConvert.ToBoolean(Session["FZNET_ADMIN"], false);
	}

	/// <summary>
	/// Load the master page components - customization based on this instance
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
    {
		WebSettings webSettings = new WebSettings();
		InstanceNameLabel.Text = webSettings.Lookup("InstanceName", "N/A");

		// update the footer with the running version number
		VersionLiteral.Text = WebVersion.Latest();

		// display the active user account
		AccountLiteral.Text = UserName;

		// turn the account panel on only if a user has authenticated
		AccountPanel.Visible = IsAuthenticated();

		// prefix the page title with the instance marker
		Page.Title = webSettings.Lookup("InstancePageTitle", "") + " FileZilla.NET by )|( Sanctuary Software Studio, Inc.";

		// check to see if SSL is required and redirect there if it is required
		if (WebConvert.ToBoolean(webSettings.Lookup("ForceSSL", "0"), false))
		{
			// verify that we are on a secure connection
			ForceSSL();
		}
    }

	/// <summary>
	/// Verify that a user is logged in
	/// </summary>
	/// <returns>true if user is logged in</returns>
	public bool IsAuthenticated()
	{
		return (userName != "");
	}

	/// <summary>
	/// Require that a user is logged in to access the page
	/// </summary>
	public void ForceAuthentication()
	{
		if (!IsAuthenticated())
		{
			Response.Redirect("~/Login.aspx");
		}
	}

	/// <summary>
	/// Verifies that we are connected on a secure connection using HTTPS
	/// </summary>
	public void ForceSSL()
	{
		if (WebConvert.ToString(HttpContext.Current.Request.ServerVariables["HTTPS"], "OFF").ToUpper() == "OFF")
		{
			// rebuild the url to utilize SSL
			string secureUrl = "https://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.Url.AbsolutePath;
			HttpContext.Current.Response.Redirect(secureUrl);
		}
	}

	/// <summary>
	/// Require administrator access
	/// </summary>
	public void ForceAdministrator()
	{
		if (!IsAdministrator)
		{
			Response.Redirect("~/AdminLogin.aspx");
		}
	}
}
