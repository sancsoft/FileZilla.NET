/******************************************************************************
 * Filename: Logout.aspx.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Clear the session and go to the login page
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/Logout.aspx.cs $
 * 
 * 4     3/11/12 10:12a Mterry
 * administration development
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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		ActivityLog log = new ActivityLog();
		log.LogActivity(LogAction.Logout, Master.UserName);

		// clear the user out of the session
		Session.Remove("FZNET_USER");
		// clear the silverlight flag
		Session.Remove("FZNET_SILVERLIGHT");
		// clear the administration flag
		Session.Remove("FZNET_ADMIN");

		Response.Redirect("~/Login.aspx");
    }
}