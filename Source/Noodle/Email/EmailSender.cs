using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Noodle.Email
{
    /// <summary>
    /// Implementations of this interface can send e-mails
    /// </summary>
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        /// <param name="handleErrors">if set to <c>true</c> [handle errors].</param>
        public void SendEmail(EmailAccountSettings emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null, bool handleErrors = true)
        {
            SendEmail(emailAccount, subject, body,
                new MailAddress(fromAddress, fromName), new MailAddress(toAddress, toName),
                bcc, cc, handleErrors);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From address</param>
        /// <param name="to">To address</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        /// <param name="handleErrors">if set to <c>true</c> [handle errors].</param>
        public virtual void SendEmail(EmailAccountSettings emailAccount, string subject, string body,
            MailAddress from, MailAddress to,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null, bool handleErrors = true)
        {
            var message = new MailMessage();
            message.From = from;
            message.To.Add(to);
            if (null != bcc)
            {
                foreach (var address in bcc.Where(bccValue => !bccValue.IsNullOrWhiteSpace()))
                {
                    message.Bcc.Add(address.Trim());
                }
            }
            if (null != cc)
            {
                foreach (var address in cc.Where(ccValue => !ccValue.IsNullOrWhiteSpace()))
                {
                    message.CC.Add(address.Trim());
                }
            }
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
            smtpClient.Host = emailAccount.Host;
            smtpClient.Port = emailAccount.Port;
            smtpClient.EnableSsl = emailAccount.EnableSsl;
            smtpClient.Credentials = emailAccount.UseDefaultCredentials 
                ? CredentialCache.DefaultNetworkCredentials 
                : new NetworkCredential(emailAccount.Username, emailAccount.Password);

            try
            {
                smtpClient.Send(message);
            }catch(Exception ex)
            {
                if(handleErrors)
                {
                    // TODO: Error notifier
                    //_logger.Error("Send email error", ex);
                }else
                {
                    throw;
                }
            }
        }
    }
}
