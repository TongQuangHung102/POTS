using backend.DataAccess.DAO;
using backend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAO _paymentDAO;

        public PaymentRepository(PaymentDAO paymentDAO)
        {
            _paymentDAO = paymentDAO;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            return await _paymentDAO.CreatePaymentAsync(payment);
        }

        public async Task<decimal> GetDailyRevenueAsync(DateTime date, int? gradeId)
        {
            return await _paymentDAO.GetDailyRevenueAsync(date, gradeId) ;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _paymentDAO.GetPaymentByIdAsync(id);
        }

        public async Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId)
        {
           return await _paymentDAO.GetPaymentByTransactionIdAsync(transactionId);
        }

        public async Task<List<Payment>> GetPaymentsAsync(string? status, string? transactionId, string? userEmail, int page, int pageSize)
        {
           return await _paymentDAO.GetPaymentsAsync(status, transactionId, userEmail, page, pageSize);
        }

        public async Task<List<Payment>> GetPaymentsByParentIdAsync(int userId)
        {
            return await _paymentDAO.GetPaymentsByParentIdAsync(userId);
        }

        public async Task<Payment?> GetPendingPaymentAsync(int userId, int studentId, int planId)
        {
           return await _paymentDAO.GetPendingPaymentAsync(userId, studentId, planId);
        }

        public async Task<int> GetTotalPaymentsAsync(string? status, string? transactionId, string? userEmail)
        {
            return await _paymentDAO.GetTotalPaymentsAsync(status, transactionId, userEmail);
        }

        public async Task<decimal> GetTotalRevenueAsync(int? gradeId)
        {
            return await _paymentDAO.GetTotalRevenueAsync(gradeId);
        }

        public async Task<bool> HasStudentPaidForPlanAsync(int studentId, int planId)
        {
           return await _paymentDAO.HasStudentPaidForPlanAsync(studentId, planId);
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            return await _paymentDAO.UpdatePaymentAsync(payment);
        }

        public async Task<bool> UpdatePaymentReviewAsync(int id, bool isReview)
        {
           return await _paymentDAO.UpdatePaymentReviewAsync(id, isReview);
        }

        public async Task<bool> UpdatePaymentStatusAsync(string transactionId, string status, decimal price, string? reason = null, string? note = null)
        {
           return await _paymentDAO.UpdatePaymentStatusAsync(transactionId, status,price, reason, note);    
        }
    }
}
