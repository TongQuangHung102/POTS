using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Users
{
    public class LinkAccount
    {
        public string Email { get; set; }
        public int ParentId { get; set; }

    }
    public class VerifyLinkAccount
    {
        public int ParentId { get; set; }
        public string StudentEmail { get; set; }
        public string Code { get; set; }
    }
    
    public class CreateAccountByParent
    {
        public int ParentId { get; set;}
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int GradeId { get; set; }

    }
}
