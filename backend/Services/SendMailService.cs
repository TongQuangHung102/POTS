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
    }
}
