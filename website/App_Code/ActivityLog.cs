/******************************************************************************
 * Filename: ActivityLog.cs
 * Project:  FileZilla.NET
 * 
 * Description:
 * Log actions and user activity to files 
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/ActivityLog.cs $
 * 
 * 1     3/07/12 12:13a Mterry
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
using System.IO;


/// <summary>
/// Identifies the logged actions
/// </summary>
public enum LogAction { Login, Logout, Browse, Download, Upload, Move, Delete, CreateFolder, Rename };

/// <summary>
/// Support logging activity to files.
/// </summary>
public class ActivityLog
{
	const string LogSeparator = "\t";
	DateTime timestamp;
	
	/// <summary>
	/// Construct an activity log object
	/// </summary>
	public ActivityLog()
	{
		timestamp = DateTime.Now;
	}

	/// <summary>
	/// Generate the filename for the applicable log file
	/// </summary>
	/// <param name="ts">timestamp for file generation</param>
	/// <returns>filename for log storage</returns>
	public string LogFilename(DateTime ts)
	{
		return string.Format("log{0:0000}-{1:00}-{2:00}.txt", ts.Year, ts.Month, ts.Day);
	}

	/// <summary>
	/// Append a line to the applicable log file
	/// </summary>
	/// <param name="logEntry">line to add to log</param>
	public void LogAppend(string logEntry)
	{
		// use the timestamp captured when the object was created to identify the file
		string fname = LogFilename(timestamp);
		WebSettings ws = new WebSettings();

		// create a full path to the target log file
		string logpath = ws.Lookup("LogPath", HttpContext.Current.Server.MapPath("~/App_Data"));
		logpath = Path.Combine(logpath, fname);

		// append to the log file or create it if it doesn't exist
		if (!File.Exists(logpath))
		{
			using (StreamWriter sw = File.CreateText(logpath))
			{
				sw.WriteLine(logEntry);
			}
		}
		else
		{
			using (StreamWriter sw = File.AppendText(logpath))
			{
				sw.WriteLine(logEntry);
			}
		}
	}

	/// <summary>
	/// Generate a standard log entry line
	/// </summary>
	/// <param name="action">action identifier</param>
	/// <param name="username">user</param>
	/// <param name="target">target informatio for action</param>
	public void LogActivity(LogAction action, string username, string target = "", string msg = "")
	{

		// build a string containing the log entry
		StringBuilder sb = new StringBuilder();
		
		// capture the timestamp and add as the first column
		sb.Append(timestamp.ToShortDateString() + " " + timestamp.ToShortTimeString() + LogSeparator);

		// add the ip address
		sb.Append( HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + LogSeparator );

		// add the current user
		sb.Append( username + LogSeparator );

		// add the action
		sb.Append( action.ToString() + LogSeparator );

		// add the target information
		sb.Append( target + LogSeparator );

		// add the message
		sb.Append(msg + LogSeparator);

		// add the entry to the log file
		LogAppend(sb.ToString());
	}
}