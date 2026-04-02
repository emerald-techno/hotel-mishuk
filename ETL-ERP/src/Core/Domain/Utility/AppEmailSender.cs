using MimeKit;
using System.Net;
using System.Net.Mail;

namespace Domain.Utility
{
    public class AppEmailSender:IAppEmailSender
    {
        public readonly SmtpConfig Config;

        public AppEmailSender()
        {
            Config = new SmtpConfig();
        }

        public AppEmailSender(SmtpConfig smtpConfig)
        {
            Config = smtpConfig ?? Utility.AppSettings.SmtpConfig;
        }


        public async Task<(bool success, string errorMsg)> SendEmailAsync(
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            SmtpConfig config = null,
            bool isHtmlEnable = true)
        {
            var from = new MailboxAddress(Config.SmtpEmailSenderName, Config.SmtpEmailAddress);
            var to = new MailboxAddress(recipientName, recipientEmail);

            if (config == null) config = new SmtpConfig();
            return await SendEmailAsync(from, new string[] { recipientEmail }, subject, body, config, isHtmlEnable);

        }



        public async Task<(bool success, string errorMsg)> SendEmailAsync(string senderName, string senderEmail, string recipientName, string recipientEmail, string subject, string body, SmtpConfig config = null, bool isHtmlEnable = true)
        {
            var from = new MailboxAddress(senderName, senderEmail);
            var to = new MailboxAddress(recipientName, recipientEmail);

            return await SendEmailAsync(from, new string[] { recipientEmail }, subject, body, config, isHtmlEnable);
        }



        public async Task<(bool success, string errorMsg)> SendEmailAsync(MailboxAddress sender, string[] recipients, string subject, string body, SmtpConfig config = null, bool isHtmlEnable = true)
        {
            try
            {
                if (config == null)
                    config = Utility.AppSettings.SmtpConfig;// Config;

                //using (var client = new SmtpClient())
                //{

                var recipient = recipients.FirstOrDefault() ?? throw new InvalidOperationException();
                var senderEmail = new MailAddress(config.SmtpEmailAddress, config.SmtpEmailAddress);
                var receiverEmail = new MailAddress(recipient, recipient);
                var smtp = new SmtpClient
                {
                    Host = config.SmtpEmailHost,
                    Port = Convert.ToInt32(config.SmtpEmailPort),
                    EnableSsl = config.SmtpUseSSL,

                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    //Credentials = new NetworkCredential(senderEmail.Address, config.SmtpEmailPassword),
                    Credentials = new NetworkCredential(Config.SmtpEmailSenderName, config.SmtpEmailPassword),
                };
                //senderEmail.Address = "notice@emeraldtechnobd.com";


                var mail = new MailMessage(senderEmail, receiverEmail)
                {
                    
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };


                //smtp.Send(mail);


                try
                {
                    Thread T1 = new Thread(delegate ()
                    {
                        try
                        {
                            smtp.Send(mail);
                        }
                        catch (Exception ex) { }
                    });
                    T1.Start();
                }
                catch
                { }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
