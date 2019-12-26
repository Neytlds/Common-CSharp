using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neytlds.Common.Mail
{
	/// <summary>
	/// 基于 MailKit 的邮件操作
	/// </summary>
	public class MailKitHelper
    {
		/// <summary>
		/// Constructor
		/// </summary>
		public MailKitHelper() { }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sendEMailHost">发件服务地址</param>
		/// <param name="sendEMailAddress">发件人邮箱地址</param>
		/// <param name="sendEMailPwd">发件人邮箱密码</param>
		public MailKitHelper(string sendEMailHost, string sendEMailAddress, string sendEMailPwd)
		{
			SendEmailHost = sendEMailHost;
			SendEmailAddress = sendEMailAddress;
			SendEmailPwd = sendEMailPwd;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sendEMailHost">发件服务地址</param>
		/// <param name="sendEMailAddress">发件人邮箱地址</param>
		/// <param name="sendEmailName">发件人姓名</param>
		/// <param name="sendEmailUserName">发件邮箱用户名</param>
		/// <param name="sendEMailPwd">发件人邮箱密码</param>
		public MailKitHelper(string sendEMailHost, string sendEMailAddress, string sendEmailName, string sendEmailUserName, string sendEMailPwd)
		{
			SendEmailHost = sendEMailHost;
			SendEmailAddress = sendEMailAddress;
			SendEmailName = sendEmailName;
			SendEmailUserName = sendEmailUserName;
			SendEmailPwd = sendEMailPwd;
		}
		/// <summary>
		/// 发件服务地址
		/// </summary>
		public string SendEmailHost { get; set; }
		/// <summary>
		/// 发件服务端口
		/// </summary>
		public int Port { get; set; } = 587;
		/// <summary>
		/// 发件邮箱地址
		/// </summary>
		public string SendEmailAddress { get; set; }
		/// <summary>
		/// 发件人姓名
		/// </summary>
		public string SendEmailName { get; set; }
		/// <summary>
		/// 发件邮箱用户名
		/// </summary>
		public string SendEmailUserName { get; set; }
		/// <summary>
		/// 发件邮箱密码
		/// </summary>
		public string SendEmailPwd { get; set; }
		/// <summary>
		/// 异步发送邮件
		/// </summary>
		/// <param name="msgToEmail">收件人邮箱列表</param>
		/// <param name="title">邮件名称</param>
		/// <param name="content">邮件内容</param>
		public void SendEmailAsync(List<string> msgToEmail, string title, string content)
		{
			Action<List<string>, string, string> action = SendEmail;
			action.BeginInvoke(msgToEmail, title, content, null, null);
		}
		/// <summary>
		/// 同步发送邮件
		/// </summary>
		/// <param name="msgToEmail">收件人邮箱列表</param>
		/// <param name="title">邮件主题</param>
		/// <param name="content">邮件内容</param>
		public void SendEmail(List<string> msgToEmail, string title, string content)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(SendEmailName, SendEmailAddress));
			msgToEmail.ForEach(e => message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", e)));
			message.Subject = title;
			message.Body = new TextPart("plain")
			{
				Text = content
			};

			using var client = new SmtpClient
			{
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				ServerCertificateValidationCallback = (s, c, h, e) => true
			};

			client.Connect(SendEmailHost, Port, false);

			// Note: only needed if the SMTP server requires authentication
			client.Authenticate(SendEmailUserName, SendEmailPwd);

			client.Send(message);
			client.Disconnect(true);
		}
    }
}
