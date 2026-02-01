using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Application.Helper
{
    public class MailSender
    {
        public async static Task SendAsync(
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
