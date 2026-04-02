using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Utility
{
    public interface IAppEmailSender
    {
        //Task<(bool success, string errorMsg)> SendEmailAsync(MailboxAddress sender, string[] recipients, string subject, string body, SmtpConfig config = null, bool isHtmlEnable = true);
        Task<(bool success, string errorMsg)> SendEmailAsync(string recipientName, string recipientEmail, string subject, string body, SmtpConfig config = null, bool isHtml = true);
        Task<(bool success, string errorMsg)> SendEmailAsync(string senderName, string senderEmail, string recipientName, string recipientEmail, string subject, string body, SmtpConfig config = null, bool isHtmlEnable = true);
    }
}
