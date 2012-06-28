/******************************************************************************
 * Filename: EMail.cs
 * Project:  Filezilla.NET
 * 
 * Description:
 * Send electronic mail messages.
 * 
 * Revision History:
 * $Log: /FileZilla.NET/website/App_Code/EMail.cs $
 * 
 * 1     3/21/12 11:27p Mterry
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
using System.Net;
using System.Net.Mail;

public static class EMail
{
	/// <summary>
	/// Send an email message.
	/// </summary>
	/// <param name="subject">Subject text for the message</param>
	/// <param name="body">Body text for the message</param>
	/// <param name="recipients">A comma-delimited list of recipients that will receive the message</param>
	/// <returns>True if the email is sent; otherwise false.</returns>
	public static bool Send(string subject, string body, string recipients)
	{
		if (recipients.Length < 1)
		{
			return false;
		}

		WebSettings ws = new WebSettings();

		// Get the configured EMail infromation from the web config
		string EmailAccount = ws.Lookup("EmailAccount");
		string EmailPassword = ws.Lookup("EmailPassword");
		string EmailSender = ws.Lookup("EmailSender");
		string EmailReplyTo = ws.Lookup("EmailReplyTo");
		string EmailSenderName = ws.Lookup("EmailSenderName");
		string EmailServer = ws.Lookup("EmailServer");
		int EmailPort = WebConvert.ToInt32(ws.Lookup("EmailPort", "25"), 25);

		// make the credentials for connecting to the server
		NetworkCredential smtpuser = new NetworkCredential(EmailAccount, EmailPassword);

		// create the mail message
		MailMessage mail = new MailMessage();
		mail.From = new MailAddress(EmailSender, EmailSenderName);
		mail.To.Add(recipients);
		mail.ReplyToList.Add(new MailAddress(EmailReplyTo));
		mail.Sender = new MailAddress(EmailSender);
		mail.Subject = subject;
		mail.Body = body;
		mail.IsBodyHtml = false;

		// connect to the smtp client and send the message
		SmtpClient client = new SmtpClient(EmailServer);
		client.UseDefaultCredentials = false;
		client.Credentials = smtpuser;
		client.Port = EmailPort;
		client.Send(mail);

		return true;
	}
}