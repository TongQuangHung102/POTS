using backend.Helpers;
using backend.Repositories;

namespace backend.Services
{
    public class PasswordResetService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SendMailService _sendMailService;

        public PasswordResetService(IAuthRepository authRepository, SendMailService sendMailService)
        {
            _authRepository = authRepository;
            _sendMailService = sendMailService;
        }

 
        public async Task ResetPasswordAsync(string email)
        {
            var user = await _authRepository.GetUserByEmail(email);
            if (user == null) throw new Exception("User not found");

            var newPassword = PasswordEncryption.GenerateRandomPassword();
          
            await _sendMailService.SendPasswordResetEmailAsync(email, newPassword);

            var encrytionPassword = PasswordEncryption.HashPassword(newPassword);
            
            await _authRepository.UpdatePasswordAsync(email, encrytionPassword);
        }
    }
}
