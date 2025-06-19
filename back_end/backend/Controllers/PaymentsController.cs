using backend.Dtos.Payment;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            var payment = await _paymentService.CreatePaymentAsync(dto);

            if (payment == null)
            {
                return StatusCode(500, "Không thể tạo hoặc cập nhật giao dịch.");
            }

            return Ok(new
            {
                payment.PaymentId,
                payment.TransactionId,
                payment.Amount,
            });
        }

        [HttpGet("status/{paymentId}")]
        public async Task<IActionResult> CheckPaymentStatus(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
                return NotFound();

            return Ok(new
            {
                status = payment.PaymentStatus,
                confirmedAt = payment.ConfirmedAt,
                reason = payment.FailureReason,
                note = payment.ManualReviewerNote
            });
        }

        [HttpGet("get-all-payment")]
        public async Task<IActionResult> GetPayments(
        [FromQuery] string? status,
        [FromQuery] string? transactionId,
        [FromQuery] string? userEmail,
        [FromQuery] int page,
        [FromQuery] int pageSize)
        {
            try
            {
                var result = await _paymentService.GetPaymentsAsync(status, transactionId, userEmail, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy danh sách thanh toán");
            }
        }

        [Authorize(Roles = "2")]
        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments(
            [FromQuery] string? status,
            [FromQuery] string? transactionId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("Không xác định được phụ huynh.");

                var result = await _paymentService.GetPaymentsAsync(status, transactionId, email, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy lịch sử thanh toán.");
            }
        }


        [HttpPut("review")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewRequest request)
        {
            var success = await _paymentService.UpdatePaymentReviewAsync(request.PaymentId, request.IsReview);

            if (!success)
            {
                return BadRequest("Cập nhật thất bại.");
            }

            return Ok("Cập nhật thành công.");
        }
    }
}
