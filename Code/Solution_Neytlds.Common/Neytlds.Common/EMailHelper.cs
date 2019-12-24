using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Neytlds.Common
{
    public class EMailHelper
    {
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
            var sendEmailAddress = "abc@efg.com";
            var sendEmailPwd = "pwd";
            var client = new SmtpClient
            {
                Host = "smtp.qq.com",
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(sendEmailAddress, sendEmailPwd),
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
                From = new MailAddress(sendEmailAddress),
            };
            msgToEmail.ForEach(e => milMessage.To.Add(new MailAddress(e)));
            client.Send(milMessage);
        }
    }
}
