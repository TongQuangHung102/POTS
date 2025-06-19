namespace backend.Dtos.Auth
{
    public class UserGoogleDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int? Role { get; set; }
        public string? GoogleId { get; set; }
    }
}
