using backend.Models;

namespace backend.Services
{
    public interface ISendMailService
    {
        Task SendPasswordResetEmailAsync(string email, string newPassword);
        Task SendConfirmationEmailAsync(string email, string token);
        Task SendEmailLinkAccountAsync(string email, string code);
        Task SendEmailNotificationAsync(string email, string subject, string body);
        Task SendPaymentNotificationAsync(string email, PaymentIssueType type, decimal? price, string? transactionId);
    }
}
