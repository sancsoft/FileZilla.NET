/******************************************************************************
 * Filename: AdminLogin.aspx.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Support administration login
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/AdminLogin.aspx.cs $
 * 
 * 1     3/11/12 10:12a Mterry
******************************************************************************/
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

public partial class AdminLogin : System.Web.UI.Page
{
	WebSettings settings;
	protected void Page_Load(object sender, EventArgs e)
	{
		// this page requires an authenticated user
		Master.ForceAuthentication();

		// access the settings
		settings = new WebSettings();

		if (!Page.IsPostBack)
		{
		}

		// Wire the events
		LoginButton.Click += new EventHandler(LoginButton_Click);

		// set the focus
		AccPasswordTextBox.Focus();
	}

	void LoginButton_Click(object sender, EventArgs e)
	{
		string password = AccPasswordTextBox.Text;

		ActivityLog log = new ActivityLog();

		// create a filezilla object using the active configuration
		Filezilla fz = new Filezilla(settings.Lookup("ConfigurationFile"));
		if (!fz.AuthenticateAdministration(password))
		{
			log.LogActivity(LogAction.Login, "REMOTEADMIN", "authentication failed");

			LoginValidator.IsValid = false;
			return;
		}

		log.LogActivity(LogAction.Login, "REMOTEADMIN");

		// Set the session vars
		Session["FZNET_ADMIN"] = true;

		// Redirect to the administration home page
		Response.Redirect("Admin.aspx");
	}
}
