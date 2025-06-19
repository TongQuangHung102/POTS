// File: backend/Services/SendMailService.cs
using backend.Models;
using System.Net;
using System.Net.Mail;

namespace backend.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly string _smtpHost = "smtp.gmail.com"; 
        private readonly string _smtpUser = "metaschool202@gmail.com"; 
        private readonly string _smtpPassword = "zgon islm apvt oqlk";

        public async Task SendPasswordResetEmailAsync(string email, string newPassword)
        {
            var subject = "Mật khẩu mới của bạn";

            
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; background-color: #f9f9f9;'>
            <h2 style='color: #333;'>Yêu cầu thay đổi mật khẩu</h2>
            <p style='font-size: 16px; color: #555;'>Chào bạn,</p>
            <p style='font-size: 16px; color: #555;'>Chúng tôi đã nhận được yêu cầu thay đổi mật khẩu của bạn. Mật khẩu mới của bạn như sau:</p>
            <div style='margin: 20px 0; padding: 15px; background-color: #f0f0f0; border: 1px solid #ddd; border-radius: 4px;'>
                <strong style='font-size: 18px; color: #333;'>Mật khẩu mới của bạn:</strong>
                <p style='font-size: 18px; color: #2c3e50; font-weight: bold;'>{newPassword}</p>
            </div>
            <p style='font-size: 16px; color: #555;'>Nếu bạn không yêu cầu thay đổi này, vui lòng bỏ qua email này hoặc liên hệ với bộ phận hỗ trợ.</p>
            <footer style='margin-top: 20px; text-align: center; font-size: 12px; color: #777;'>
                <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
            </footer>
        </div>
    </body>
    </html>";

            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, body)
                {
                    IsBodyHtml = true 
                };

                await smtpClient.SendMailAsync(mailMessage);
            }
        }


        public async Task SendConfirmationEmailAsync(string email, string token)
        {
            string confirmationLink = $"http://localhost:3000/confirm-email?token={token}";
            var subject = "Xác nhận email của bạn";

            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; background-color: #f9f9f9;'>
            <h2 style='color: #333;'>Xác nhận email của bạn</h2>
            <p style='font-size: 16px; color: #555;'>Chào bạn,</p>
            <p style='font-size: 16px; color: #555;'>Cảm ơn bạn đã đăng ký tài khoản. Để hoàn tất quá trình đăng ký, vui lòng nhấp vào liên kết dưới đây để xác nhận email của bạn:</p>
            <div style='margin: 20px 0; text-align: center;'>
                <a href='{confirmationLink}' style='padding: 12px 25px; background-color: #4CAF50; color: white; text-decoration: none; font-size: 18px; border-radius: 5px;'>Xác nhận Email</a>
            </div>
            <p style='font-size: 16px; color: #555;'>Nếu bạn không yêu cầu đăng ký, vui lòng bỏ qua email này.</p>
            <footer style='margin-top: 20px; text-align: center; font-size: 12px; color: #777;'>
                <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
            </footer>
        </div>
    </body>
    </html>";

            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, body)
                {
                    IsBodyHtml = true 
                };

                await smtpClient.SendMailAsync(mailMessage);
            }
        }


        public async Task SendEmailLinkAccountAsync(string email, string code)
        {
            var subject = "Mã liên kết tài khoản";

            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; background-color: #f9f9f9;'>
            <h2 style='color: #333;'>Mã liên kết tài khoản</h2>
            <p style='font-size: 16px; color: #555;'>Chào bạn,</p>
            <p style='font-size: 16px; color: #555;'>Để liên kết tài khoản của bạn, vui lòng sử dụng mã liên kết sau:</p>
            <div style='margin: 20px 0; font-size: 18px; font-weight: bold; color: #333; text-align: center;'>
                <span style='padding: 12px 25px; background-color: #4CAF50; color: white; font-size: 20px; border-radius: 5px;'>{code}</span>
            </div>
            <p style='font-size: 16px; color: #555;'>Nếu bạn không yêu cầu liên kết tài khoản, vui lòng bỏ qua email này.</p>
            <footer style='margin-top: 20px; text-align: center; font-size: 12px; color: #777;'>
                <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
            </footer>
        </div>
    </body>
    </html>";

            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, body)
                {
                    IsBodyHtml = true
                };

                await smtpClient.SendMailAsync(mailMessage);
            }
        }


        public async Task SendEmailNotificationAsync(string email, string subject, string body)
        {
            var htmlBody = $@"
    <html>
    <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; background-color: white; border-radius: 8px; box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);'>
            <header style='text-align: center; padding-bottom: 20px;'>
                <h2 style='color: #4CAF50; font-size: 24px; margin: 0;'>Thông Báo</h2>
            </header>
            <section style='padding: 20px; font-size: 16px; color: #555;'>
                <p>{body}</p>
            </section>
            <footer style='text-align: center; font-size: 12px; color: #777; margin-top: 20px;'>
                <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
            </footer>
        </div>
    </body>
    </html>";

            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage(_smtpUser, email, subject, htmlBody)
                {
                    IsBodyHtml = true 
                };

                await smtpClient.SendMailAsync(mailMessage);
            }
        }


        public async Task SendPaymentNotificationAsync(string email, PaymentIssueType type, decimal? price, string? transactionId)
        {
            string subject = "Thông báo giao dịch thanh toán";
            string body = GeneratePaymentBody(type, price, transactionId);

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

        private string GeneratePaymentBody(PaymentIssueType type, decimal? price, string? transactionId)
        {
            string? qrImgTag = null;

            if (price.HasValue && (type == PaymentIssueType.ChuyenThieu || type == PaymentIssueType.ChuyenMuon_Thieu))
            {
                qrImgTag = $"<p>Bạn có thể quét mã QR dưới đây để chuyển bổ sung:</p><img src='{GenerateQrImage(price.Value, transactionId)}' width='250'/>";
            }
            return type switch
            {
                PaymentIssueType.ChuyenThieu => $@"
                <p>Chào bạn,</p>
                <p>Bạn đã <strong>chuyển thiếu</strong> <strong>{price:N0}đ</strong> so với giá gói.</p>
                <p>Vui lòng bổ sung để hệ thống kích hoạt gói cho bạn.</p>
                <p><strong>Lưu ý: Chúng tôi sẽ không hỗ trợ nếu bạn còn chuyển khoản khác với số tiền và nội dung của mã QR</strong></p>
                {qrImgTag}
                <p>Trân trọng,<br/>QuizViệt</p>",

                PaymentIssueType.ChuyenThua => $@"
                <p>Chào bạn,</p>
                <p>Bạn đã <strong>chuyển thừa</strong> <strong>{price:N0}đ</strong>. Gói đã được kích hoạt.</p>
                <p>Vui lòng liên hệ zalo: 012345678 để được hỗ trợ hoàn tiền </p>
                <p>Trân trọng,<br/>QuizViệt</p>",

                PaymentIssueType.ChuyenMuon => $@"
                <p>Chào bạn,</p>
                <p>Bạn đã thanh toán thành công. Gói đã được kích hoạt.</p>
                <p>Để có trải nghiệm tốt nhất, vui lòng thanh toán ngay sau khi tạo đơn hàng..</p>
                <p>Trân trọng,<br/>QuizViệt</p>",

                PaymentIssueType.ChuyenMuon_Thieu => $@"
                <p>Chào bạn,</p>
                <p>Bạn đã <strong>chuyển muộn</strong> và <strong>thiếu {price:N0}đ</strong>.</p>
                <p>Vui lòng bổ sung để kích hoạt gói.</p>
                <p><strong>Lưu ý: Chúng tôi sẽ không hỗ trợ nếu bạn còn chuyển khoản khác với số tiền và nội dung của mã QR</strong></p>
                {qrImgTag}
                <p>Trân trọng,<br/>QuizViệt</p>",

                PaymentIssueType.ChuyenMuon_Thua => $@"
                <p>Chào bạn,</p>
                <p>Bạn đã <strong>chuyển muộn</strong> và <strong>chuyển thừa {price:N0}đ</strong>.</p>
                <p>Gói vẫn được kích hoạt thành công. Vui lòng chuyển khoản ngay sau khi tạo đơn hàng để có trải nghiệm tốt nhất.</p>
                <p>Vui lòng liên hệ zalo: 012345678 để được hỗ trợ hoàn tiền </p>
                <p>Trân trọng,<br/>QuizViệt</p>",

                PaymentIssueType.ChuyenDu => $@"
                <p>Chào bạn,</p>
                <p>Bạn đã thanh toán thành công. Gói đã được kích hoạt.</p>
                <p>Cảm ơn và Trân trọng,<br/>QuizViệt</p>",

                PaymentIssueType.TuChoi => $@"
                <p>Chào bạn,</p>
                <p>Chúng tôi đã nhận được giao dịch chuyển lại của bạn lần thứ hai. Tuy nhiên, số tiền vẫn chưa đủ để kích hoạt gói dịch vụ.</p>
                <p>Theo chính sách của chúng tôi, chúng tôi sẽ từ chối xử lý giao dịch này.</p>
                <p>Vui lòng kiểm tra lại và thực hiện giao dịch mới nếu bạn vẫn muốn tiếp tục sử dụng dịch vụ.</p>
                <p>Cảm ơn bạn đã quan tâm.<br/>Trân trọng,<br/>QuizViệt</p>",


                _ => "<p>Có lỗi xảy ra. Vui lòng liên hệ hỗ trợ.</p>"
            };
        }

        private string GenerateQrImage(decimal amount, string transactionId)
        {
            return $"https://qr.sepay.vn/img?acc=00001628681&bank=TPBank&amount={amount}&des={transactionId}&template=TEMPLATE&download=DOWNLOAD";
        }

    }
}
