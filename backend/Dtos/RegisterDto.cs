using System.ComponentModel.DataAnnotations;

namespace backend.Dtos
{
    public class RegisterDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int Role { get; set; }

        public DateTime CreateAt { get; set; }

        public string FullName { get; set; }

        public bool IsActive { get; set; } = true;

        public string? EmailVerificationToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
