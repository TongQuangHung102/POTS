using backend.Dtos.Payment;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISendMailService _sendMailService;
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly IUserSubscriptionService _userSubscriptionService;

        public PaymentService(IPaymentRepository paymentRepository, ISendMailService sendMailService, ISubscriptionPlanService subscriptionPlanService, IUserSubscriptionService userSubscriptionService)
        {
            _paymentRepository = paymentRepository;
            _sendMailService = sendMailService;
            _subscriptionPlanService = subscriptionPlanService;
            _userSubscriptionService = userSubscriptionService;
        }

        public async Task<Payment?> CreatePaymentAsync(PaymentCreateDto dto)
        {
            try
            {
                var existing = await _paymentRepository.GetPendingPaymentAsync(dto.UserId, dto.StudentId, dto.PlanId);

                var code = $"{dto.UserId}PAY{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                DateTime now = DateTime.UtcNow.AddHours(7);

                if (existing != null)
                {
                    existing.Amount = dto.Amount;
                    existing.TransactionId = code;
                    existing.CreatedAt = now;
                    
                    return await _paymentRepository.UpdatePaymentAsync(existing);
                }

                var newPayment = new Payment
                {
                    UserId = dto.UserId,
                    StudentId = dto.StudentId,
                    PlanId = dto.PlanId,
                    Amount = dto.Amount,
                    PaymentMethod = "Banking",
                    PaymentStatus = "Pending",
                    CreatedAt = now,
                    TransactionId = code
                };

                return await _paymentRepository.CreatePaymentAsync(newPayment);
            }
            catch (Exception ex)
            {
                return null;
            }
        
        }

        public async Task<bool> UpdatePaymentReviewAsync(int id, bool isReview)
        {
            try
            {
                return await _paymentRepository.UpdatePaymentReviewAsync(id, isReview);
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            try
            {
                return await _paymentRepository.GetPaymentByIdAsync(id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<Payment>> GetPaymentsByParentIdAsync(int userId)
        {
            try
            {
                return await _paymentRepository.GetPaymentsByParentIdAsync(userId);
            }
            catch (Exception ex)
            {
                return new List<Payment>();
            }
        }

        public async Task<bool> HasStudentPaidForPlanAsync(int studentId, int planId)
        {
            try
            {
                return await _paymentRepository.HasStudentPaidForPlanAsync(studentId, planId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<PaymentPageDto> GetPaymentsAsync(string? status, string? transactionId, string? userEmail, int page, int pageSize)
        {
            try
            {
                var payments = await _paymentRepository.GetPaymentsAsync(status, transactionId, userEmail, page, pageSize);
                var count = await _paymentRepository.GetTotalPaymentsAsync(status, transactionId, userEmail);

                var response = new PaymentPageDto
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalPayment = count,
                    Data = payments.Select(p => new PaymentDto
                    {
                        PaymentId = p.PaymentId,
                        Amount = p.Amount,
                        ConfirmedAt = p.ConfirmedAt,
                        CreatedAt = p.CreatedAt,
                        FailureReason = p.FailureReason,
                        IsManuallyReviewed = p.IsManuallyReviewed,
                        ManualReviewerNote = p.ManualReviewerNote,
                        PaymentDate = p.PaymentDate,
                        PaymentMethod = p.PaymentMethod,
                        PaymentStatus = p.PaymentStatus,
                        StudentEmail = p.Student?.Email,
                        UserEmail = p.User?.Email ?? "",
                        PlanName = p.Plan?.PlanName ?? "",
                        TransactionId = p.TransactionId,
                        ReceivedAmount = p.ReceivedAmount

                    }).ToList()
                };
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /*public async Task HandleSePayAsync(SePayWebhookRequest data)
        {
            var transactionId = ExtractTransactionId(data.Content);
            var isSupplement = transactionId.StartsWith("CKB");
            var now = DateTime.UtcNow.AddHours(7);
        
            //chuyển lại phần còn thiếu
            if (isSupplement)
            {
                var content = ExtractOriginalTransactionId(transactionId);

                var payment = await _paymentRepository.GetPaymentByTransactionIdAsync(content);
                if (payment == null) return;

                if (payment.PaymentStatus == "Confirmed") return;
                var plan = await _subscriptionPlanService.GetPlanDetailAsync(payment.PlanId);
                var totalAmount = payment.ReceivedAmount + data.TransferAmount;
                //Chuyển lại muộn
                if(payment.PaymentDate.AddHours(24) < now)
                {
                    //Chuyển lại muộn vẫn đủ tiền hoặc thừa tiền
                    if(totalAmount == plan.Price || totalAmount > plan.Price)
                    {
                        await _paymentRepository.UpdatePaymentStatusAsync(content, "Confirmed", totalAmount, null, "Đã chuyển đủ phần thiếu");
                        await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
                        await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenMuon, null, null);
                    }
                    //Chuyển lại không đủ tiền từ chối hỗ trợ
                    else
                    {
                        await _paymentRepository.UpdatePaymentStatusAsync(content, "Failed", totalAmount, "Chuyển thiếu tiền", "Chuyển lại vẫn thiếu - Từ chối hỗ trợ");
                    }
                }
                //chuyển lại đúng hạn
                else
                {
                    if (totalAmount == payment.Amount)
                    {
                        await _paymentRepository.UpdatePaymentStatusAsync(content, "Confirmed", totalAmount, null, "Đã chuyển đủ phần thiếu");
                        await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
                        await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenDu, null, null);
                    }
                    else if(totalAmount > payment.Amount)
                    {
                        var differenceAmount = data.TransferAmount - payment.Amount;
                        var note = $"Chuyển lần 2 - Chuyển thừa ({differenceAmount:N0}đ)";
                        await _paymentRepository.UpdatePaymentStatusAsync(transactionId, "Confirmed", data.TransferAmount, null, note);
                        await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
                        await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenThua, differenceAmount, null);
                        return;
                    }
                    else
                    {
                        await _paymentRepository.UpdatePaymentStatusAsync(content, "Failed", totalAmount, "Chuyển thiếu tiền", "Chuyển lại vẫn thiếu - Từ chối hỗ trợ");
                        
                    }
                }
                
            }
            else
            {
                var payment = await _paymentRepository.GetPaymentByTransactionIdAsync(transactionId);
                if (payment == null) return;

                if (payment.PaymentStatus == "Confirmed" || payment.PaymentStatus == "Failed") return;
                var plan = await _subscriptionPlanService.GetPlanDetailAsync(payment.PlanId);

                if (payment.CreatedAt.AddHours(1) < now) // CHUYỂN MUỘN
                {
                    if (data.TransferAmount < plan.Price)
                    {
                        var newTransaction = $"CKB{transactionId}";
                        var differenceAmount = data.TransferAmount - plan.Price;
                        await _paymentRepository.UpdatePaymentStatusAsync(transactionId, "Failed",data.TransferAmount, "Quá hạn thanh toán", "Chuyển thiếu tiền");
                        await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenMuon_Thieu, differenceAmount, newTransaction);
                        return;
                    }
                    else if (data.TransferAmount > plan.Price)
                    {
                        var differenceAmount = data.TransferAmount - plan.Price;
                        var note = $"Chuyển muộn - thừa ({differenceAmount:N0}đ)";
                        await _paymentRepository.UpdatePaymentStatusAsync(transactionId, "Confirmed",data.TransferAmount, null, note);
                        await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
                        await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenMuon_Thua, differenceAmount, null);
                        return;
                    }
                    else // chuyển muộn nhưng đúng giá
                    {
                        await _paymentRepository.UpdatePaymentStatusAsync(transactionId, "Confirmed", data.TransferAmount);
                        await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
                        await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenMuon, null, null);
                        return;
                    }
                }

                // CHUYỂN TRONG HẠN
                if (data.TransferAmount < payment.Amount)
                {
                    var newTransaction = $"CKB{transactionId}";
                    var differenceAmount = payment.Amount - data.TransferAmount;
                    await _paymentRepository.UpdatePaymentStatusAsync(transactionId, "Failed", data.TransferAmount, "Chuyển thiếu tiền");
                    await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenThieu, differenceAmount, newTransaction);
                    return;
                }
                else if (data.TransferAmount > payment.Amount)
                {
                    var differenceAmount = data.TransferAmount - payment.Amount;
                    var note = $"Chuyển thừa ({differenceAmount:N0}đ)";

                    await _paymentRepository.UpdatePaymentStatusAsync(transactionId, "Confirmed",data.TransferAmount, null, note);
                    await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
                    await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenThua, differenceAmount, null);
                    return;
                }

                // CHUYỂN ĐÚNG
                await _paymentRepository.UpdatePaymentStatusAsync(transactionId, "Confirmed", data.TransferAmount);
                await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
                await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenDu, null, null);
            }
            
        }*/
        public async Task HandleSePayAsync(SePayWebhookRequest data)
        {
            var transactionId = ExtractTransactionId(data.Content);
            var isSupplement = transactionId.StartsWith("CKB");

            if (isSupplement)
            {
               await HandleSupplementTransactionAsync(transactionId, data);
            }
            else
            {
               await HandleOriginalTransactionAsync(transactionId, data);
            }
        }

        //chuyển bù
        private async Task HandleSupplementTransactionAsync(string transactionId, SePayWebhookRequest data)
        {
            var originalId = ExtractOriginalTransactionId(transactionId);
            var payment = await _paymentRepository.GetPaymentByTransactionIdAsync(originalId);
            if (payment == null || payment.PaymentStatus == "Confirmed") return;

            var now = DateTime.UtcNow.AddHours(7);
            var plan = await _subscriptionPlanService.GetPlanDetailAsync(payment.PlanId);
            var totalAmount = payment.ReceivedAmount + data.TransferAmount;

            //chuyen bu chuyen muon
            if (payment.PaymentDate.AddHours(24) < now)
            {
                if (totalAmount >= plan.Price)
                {
                    await ConfirmPaymentAsync(payment, totalAmount, PaymentIssueType.ChuyenMuon);
                }
                else
                {
                    await FailPaymentAsync(payment, totalAmount, "Chuyển lại vẫn thiếu - Từ chối hỗ trợ");
                }
            }
            //chuyen bu chuyen dung
            else
            {
                if (totalAmount == payment.Amount)
                {
                    await ConfirmPaymentAsync(payment, totalAmount, PaymentIssueType.ChuyenDu);
                }
                else if (totalAmount > payment.Amount)
                {
                    var diff = totalAmount - payment.Amount;
                    await ConfirmPaymentAsync(payment, totalAmount, PaymentIssueType.ChuyenThua, $"Chuyển lần 2 - Chuyển thừa ({diff:N0}đ)", diff);
                }
                else
                {
                    await FailPaymentAsync(payment, totalAmount, "Chuyển lại vẫn thiếu - Từ chối hỗ trợ");
                }
            }
        }
        //chuyển chính
        private async Task HandleOriginalTransactionAsync(string transactionId, SePayWebhookRequest data)
        {
            var payment = await _paymentRepository.GetPaymentByTransactionIdAsync(transactionId);
            if (payment == null || payment.PaymentStatus == "Confirmed" || payment.PaymentStatus == "Failed") return;

            var now = DateTime.UtcNow.AddHours(7);
            var plan = await _subscriptionPlanService.GetPlanDetailAsync(payment.PlanId);

            var isLate = payment.CreatedAt.AddHours(1) < now;
            var amount = data.TransferAmount;

            if (isLate)
            {
                if (amount < plan.Price)
                {
                    var diff = plan.Price - amount;
                    var supplementId = $"CKB{transactionId}";
                    await FailPaymentAsync(payment, amount, "Chuyển thiếu tiền - Quá hạn thanh toán");
                    await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenMuon_Thieu, diff, supplementId);
                }
                else if (amount > plan.Price)
                {
                    var diff = amount - plan.Price;
                    await ConfirmPaymentAsync(payment, amount, PaymentIssueType.ChuyenMuon_Thua, $"Chuyển muộn - thừa ({diff:N0}đ)", diff);
                }
                else
                {
                    await ConfirmPaymentAsync(payment, amount, PaymentIssueType.ChuyenMuon);
                }
                return;
            }

            if (amount < payment.Amount)
            {
                var diff = payment.Amount - amount;
                var supplementId = $"CKB{transactionId}";
                await FailPaymentAsync(payment, amount, "Chuyển thiếu tiền");
                await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, PaymentIssueType.ChuyenThieu, diff, supplementId);
            }
            else if (amount > payment.Amount)
            {
                var diff = amount - payment.Amount;
                await ConfirmPaymentAsync(payment, amount, PaymentIssueType.ChuyenThua, $"Chuyển thừa ({diff:N0}đ)", diff);
            }
            else
            {
                await ConfirmPaymentAsync(payment, amount, PaymentIssueType.ChuyenDu);
            }
        }


        private async Task ConfirmPaymentAsync(Payment payment, decimal amount, PaymentIssueType type, string note = null, decimal? diff = null)
        {
            await _paymentRepository.UpdatePaymentStatusAsync(payment.TransactionId, "Confirmed", amount, null, note);
            await _userSubscriptionService.CreateOrUpdatePlanForUserAsync(payment.StudentId.Value, payment.PlanId);
            await _sendMailService.SendPaymentNotificationAsync(payment.User.Email, type, diff, null);
        }

        private async Task FailPaymentAsync(Payment payment, decimal amount, string reason)
        {
            await _paymentRepository.UpdatePaymentStatusAsync(payment.TransactionId, "Failed", amount, "Chuyển thiếu tiền", reason);
        }

        private string ExtractTransactionId(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;

            var match = Regex.Match(content.ToUpper(), @"(CKB)?\d+PAY[A-Z0-9]{5,12}");

            return match.Success ? match.Value : null;
        }
        private string ExtractOriginalTransactionId(string content)
        {
            if (content.StartsWith("CKB"))
            {
                return content.Substring(3).Trim(); // Bỏ 'CKB'
            }

            return content;
        }


    }
}
