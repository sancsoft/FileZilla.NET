/******************************************************************************
 * Filename: Admin.aspx.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Home page for the administration web interface
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/Admin.aspx.cs $
 * 
 * 5     3/15/12 2:07a Mterry
 * 
 * 4     3/13/12 10:46p Mterry
 * account view and set password support under development
 * 
 * 3     3/12/12 11:32p Mterry
 * admin interface development and binding
 * 
 * 2     3/11/12 3:26p Mterry
 * testing user and permission objects
 * 
 * 1     3/11/12 10:12a Mterry
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
using System.Data;

public partial class Admin : System.Web.UI.Page
{
	/// <summary>
	/// object for accessing the site configuration
	/// </summary>
	protected WebSettings webSettings;

	/// <summary>
	/// Object for accessing the filezilla configuration
	/// </summary>
	protected Filezilla fz;

	protected void Page_Load(object sender, EventArgs e)
	{
		NewAccountButton.Click += new EventHandler(NewAccountButton_Click);
		// this page requires an authenticated user
		Master.ForceAuthentication();

		// this page requires an authenticated administrator
		Master.ForceAdministrator();

		// create the configuration related objects
		webSettings = new WebSettings();
		fz = new Filezilla(webSettings.Lookup("ConfigurationFile"));

		// load the user grid display
		List<FilezillaUser> userList = new List<FilezillaUser>();
		int count = fz.AddUsersToList(ref userList);
		UserGridView.DataSource = userList;
		UserGridView.DataBind();

		DataSet ds = new DataSet();
		ds.ReadXml(MapPath("~/App_Data/settings.xml"));
		SettingsGridView.DataSource = ds.Tables[0].DefaultView;
		SettingsGridView.DataBind();

		if (!Page.IsPostBack)
		{
			UserGridView.SortBy(UserGridView.Columns["Username"], ColumnSortOrder.Ascending);
			UserGridView.SettingsPager.PageSize = 100;
			SettingsGridView.SortBy(SettingsGridView.Columns["key"], ColumnSortOrder.Ascending);
			SettingsGridView.SettingsPager.PageSize = 100;
		}
	}

	/// <summary>
	/// Create a new user request
	/// </summary>
	/// <param name="sender">source</param>
	/// <param name="e">args</param>
	void NewAccountButton_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/AdminUserNew.aspx");
	}

}
