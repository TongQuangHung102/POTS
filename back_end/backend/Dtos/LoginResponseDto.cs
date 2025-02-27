namespace backend.Dtos
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public int? Role { get; set; }
        public int? GradeId { get; set; }
        public string Token { get; set; }
    }
}
