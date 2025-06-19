using backend.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace backend.DataAccess.DAO
{
    public class PaymentDAO
    {
        private readonly MyDbContext _dbContext;

        public PaymentDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _dbContext.Payments
                .Include(p => p.User)
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }
        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }


        public async Task<bool> UpdatePaymentStatusAsync(string transactionId, string status, decimal price, string? reason = null, string? note = null)
        {
            var payment = await GetPaymentByTransactionIdAsync(transactionId);
            if (payment == null) return false;

            payment.PaymentStatus = status;
            payment.PaymentDate = DateTime.UtcNow.AddHours(7);
            payment.FailureReason = reason;
            payment.ManualReviewerNote = note;
            payment.ReceivedAmount = price;
            if(status == "Confirmed")
            {
                payment.ConfirmedAt = DateTime.UtcNow.AddHours(7);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePaymentReviewAsync(int id , bool isReview)
        {
            var payment = await GetPaymentByIdAsync(id);
            if (payment == null) return false;

            payment.IsManuallyReviewed = isReview;
            if (isReview)
            {
                payment.ManualReviewerNote = payment.ManualReviewerNote + "Đã hoàn tiền thừa";
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Payment>> GetPaymentsByParentIdAsync(int userId)
        {
            return await _dbContext.Payments
                .Where(p => p.UserId == userId)
                .Include(p => p.Student)
                .Include(p => p.Plan)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasStudentPaidForPlanAsync(int studentId, int planId)
        {
            return await _dbContext.Payments.AnyAsync(p =>
                p.StudentId == studentId &&
                p.PlanId == planId &&
                p.PaymentStatus == "Success");
        }

        public async Task<List<Payment>> GetPaymentsAsync(string? status, string? transactionId, string? userEmail, int page, int pageSize)
        {
            var query = _dbContext.Payments
                .Include(p => p.User)
                .Include(p => p.Student)
                .Include(p => p.Plan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.PaymentStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                query = query.Where(p => p.TransactionId.ToLower().Contains(transactionId.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                query = query.Where(p => p.User.Email.ToLower().Contains(userEmail.ToLower()));
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPaymentsAsync(string? status, string? transactionId, string? userEmail)
        {
            var query = _dbContext.Payments
                .Include(p => p.User)
                .Include(p => p.Student)
                .Include(p => p.Plan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.PaymentStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                query = query.Where(p => p.TransactionId.ToLower().Contains(transactionId.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                query = query.Where(p => p.User.Email.ToLower().Contains(userEmail.ToLower()));
            }

            return await query.CountAsync();
        }

        public async Task<decimal> GetDailyRevenueAsync(DateTime date, int? gradeId = null)
        {
            var query = _dbContext.Payments
                .Where(p => p.PaymentStatus == "Confirmed"
                            && p.ConfirmedAt != null
                            && p.ConfirmedAt.Value.Date == date.Date)
                .AsQueryable();

            if (gradeId.HasValue)
            {
                query = query.Where(p => p.Student.GradeId == gradeId);
            }

            return await query.SumAsync(p => p.ReceivedAmount); 

        }
        public async Task<decimal> GetTotalRevenueAsync(int? gradeId = null)
        {
            var query = _dbContext.Payments
                .Where(p => p.PaymentStatus == "Confirmed" && p.ConfirmedAt != null)
                .AsQueryable();

            if (gradeId.HasValue)
            {
                query = query.Where(p => p.Student.GradeId == gradeId);
            }

            return await query.SumAsync(p => p.ReceivedAmount);
        }
        public async Task<Payment?> GetPendingPaymentAsync(int userId, int studentId, int planId)
        {
            return await _dbContext.Payments.FirstOrDefaultAsync(p =>
                p.UserId == userId &&
                p.StudentId == studentId &&
                p.PlanId == planId &&
                p.PaymentStatus == "Pending");
        }

    }
}
