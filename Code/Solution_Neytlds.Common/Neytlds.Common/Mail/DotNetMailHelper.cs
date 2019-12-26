using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Neytlds.Common.Mail
{
    /// <summary>
    /// 基于 System.Net.Mail.SmtpClient 的邮件操作（已过时）
    /// </summary>
    public class DotNetMailHelper
    {
        public DotNetMailHelper() { }
        public DotNetMailHelper(string sendEMailHost, string sendEMailAddress, string sendEMailPwd)
        {
            SendEmailHost = sendEMailHost;
            SendEmailAddress = sendEMailAddress;
            SendEmailPwd = sendEMailPwd;
        }
        public string SendEmailHost { get; set; }
        public string SendEmailAddress { get; set; }
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
        /// <param name="title">邮件名称</param>
        /// <param name="content">邮件内容</param>
        public void SendEmail(List<string> msgToEmail, string title, string content)
        {
            var client = new SmtpClient
            {
                Host = SendEmailHost,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(SendEmailAddress, SendEmailPwd),
                EnableSsl = true
            };
            var milMessage = new MailMessage
            {
                Subject = title,
                SubjectEncoding = Encoding.UTF8,
                Body = content,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.High,
                From = new MailAddress(SendEmailAddress),
            };
            msgToEmail.ForEach(e => milMessage.To.Add(new MailAddress(e)));
            client.Send(milMessage);
        }
    }
}
