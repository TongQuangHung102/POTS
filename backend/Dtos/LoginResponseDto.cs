namespace backend.Dtos
{
    public class LoginResponseDto
    {
        public string Username { get; set; }
        public int Role { get; set; }
        public string Token { get; set; }
    }
}
