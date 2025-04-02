using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Auth
{
    public class RegisterDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int Role { get; set; }
    }
}
