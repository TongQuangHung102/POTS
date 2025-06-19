using backend.Dtos.Payment;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly PaymentService _paymentService;


        public WebhookController(PaymentService paymentService)
        {
            _paymentService = paymentService;

        }

        [HttpPost("sepay")]
        public async Task<IActionResult> ReceiveSePayWebhook([FromBody] SePayWebhookRequest data)
        {
            await _paymentService.HandleSePayAsync(data);

            return Ok();
        }

    }
}
