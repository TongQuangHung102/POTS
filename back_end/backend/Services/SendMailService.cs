// File: backend/Services/SendMailService.cs
using System.Net;
using System.Net.Mail;

namespace backend.Services
{
    public class SendMailService
    {
        private readonly string _smtpHost = "smtp.gmail.com"; 
        private readonly string _smtpUser = "vinhdubai10@gmail.com"; 
        private readonly string _smtpPassword = "llwy gvmd ctcx idxk";

        public async Task SendPasswordResetEmailAsync(string email, string newPassword)
        {
            var subject = "Your New Password";
            var body = $"Your new password is: {newPassword}";

            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, body);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task SendConfirmationEmailAsync(string email, string token)
        {
            string confirmationLink = $"https://localhost:7259/api/Auth/Confirm-email?token={token}";
            var subject = "Confirm Your Email";
            var body = $"Click this link to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>";

            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, body);
                mailMessage.IsBodyHtml = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task SendEmailLinkAccountAsync(string email, string code)
        {
            var subject = "Code liên kết tài khoản";
            var body = $"Mã liên kết là {code}";

            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, body);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task SendEmailNotificationAsync(string email, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, body);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
