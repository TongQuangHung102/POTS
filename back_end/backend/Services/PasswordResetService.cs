using backend.Helpers;
using backend.Repositories;

namespace backend.Services
{
    public class PasswordResetService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SendMailService _sendMailService;
        private readonly PasswordEncryption _passwordEncryption;

        public PasswordResetService(IAuthRepository authRepository, SendMailService sendMailService, PasswordEncryption passwordEncryption)
        {
            _authRepository = authRepository;
            _sendMailService = sendMailService;
            _passwordEncryption = passwordEncryption;
        }

        public async Task ResetPasswordAsync(string email)
        {
            var user = await _authRepository.GetUserByEmail(email);
            if (user == null) throw new Exception("User not found");

            var newPassword = _passwordEncryption.GenerateRandomPassword();
          
            await _sendMailService.SendPasswordResetEmailAsync(email, newPassword);

            var encrytionPassword = _passwordEncryption.HashPassword(newPassword);
            
            await _authRepository.UpdatePasswordAsync(email, encrytionPassword);
        }
    }
}
