/******************************************************************************
 * Filename: Login.aspx.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Support login against the FileZilla configuration 
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/Login.aspx.cs $
 * 
 * 4     3/11/12 12:17a Mterry
 * check for enabled
 * 
 * 3     3/07/12 11:17p Mterry
 * updates based on inital alpha testing
 * 
 * 2     3/07/12 12:14a Mterry
 * logging and landing page information
 * 
 * 1     2/26/12 12:20p Mterry
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

public partial class Login : System.Web.UI.Page
{
	WebSettings settings;
	protected void Page_Load(object sender, EventArgs e)
	{
		// access the settings
		settings = new WebSettings();
		
		if (!Page.IsPostBack)
		{
			string contactEmail = settings.Lookup("ContactEMail");
			// load the literals from the settings
			NeedAccountEmailLiteral.Text = "<a href=\"mailto:" + contactEmail + "\">" + contactEmail + "</a>";
			ForgetPasswordLiteral.Text = "<a href=\"mailto:" + contactEmail + "\">" + contactEmail + "</a>";
		}

		// Wire the events
		LoginButton.Click += new EventHandler(LoginButton_Click);

		// set the focus
		AccAccountTextBox.Focus();
	}

	void LoginButton_Click(object sender, EventArgs e)
	{
		string user = AccAccountTextBox.Text.Trim().ToLower();
		string password = AccPasswordTextBox.Text;

		ActivityLog log = new ActivityLog();

		// create a filezilla object using the active configuration
		Filezilla fz = new Filezilla(settings.Lookup("ConfigurationFile"));
		if (!fz.Authenticate(user, password))
		{
			log.LogActivity(LogAction.Login, user, "authentication failed");

			LoginValidator.IsValid = false;
			return;
		}

		log.LogActivity(LogAction.Login, user);

		// Set the session vars
		Session["FZNET_USER"] = user;
		Session["FZNET_SILVERLIGHT"] = IsSilverlightHidden.Value;

		// Redirect to the home page
		Response.Redirect("Default.aspx");
	}
}
