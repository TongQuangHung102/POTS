namespace backend.Dtos
{
    public class UserGoogleDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public DateTime CreateAt { get; set; }
        public string? GoogleId { get; set; }
        public bool IsActive { get; set; }
    }
}
