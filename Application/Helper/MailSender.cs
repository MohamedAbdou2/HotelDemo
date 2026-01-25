using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Security.Cryptography;

namespace Application.Helper
{
    public class MailSender
    {
        public async Task SendAsync(
         string to,
         string subject,
         string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(SmtpSettings.Email));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            smtp.Connect(SmtpSettings.Host, SmtpSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(SmtpSettings.Email, SmtpSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return;
        }
    }
}
