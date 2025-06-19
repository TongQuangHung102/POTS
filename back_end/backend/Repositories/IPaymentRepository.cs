using backend.Models;

namespace backend.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<bool> UpdatePaymentStatusAsync(string transactionId, string status, decimal price, string? reason = null, string? note = null);
        Task<List<Payment>> GetPaymentsByParentIdAsync(int userId);
        Task<bool> HasStudentPaidForPlanAsync(int studentId, int planId);
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<List<Payment>> GetPaymentsAsync(string? status, string? transactionId, string? userEmail, int page, int pageSize);
        Task<int> GetTotalPaymentsAsync(string? status, string? transactionId, string? userEmail);
        Task<bool> UpdatePaymentReviewAsync(int id, bool isReview);
        Task<decimal> GetDailyRevenueAsync(DateTime date, int? gradeId);
        Task<decimal> GetTotalRevenueAsync(int? gradeId);
        Task<Payment?> GetPendingPaymentAsync(int userId, int studentId, int planId);
        Task<Payment> UpdatePaymentAsync(Payment payment);
    }
}
