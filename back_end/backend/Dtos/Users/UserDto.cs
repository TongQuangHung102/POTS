﻿namespace backend.Dtos.Users
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int? Role { get; set; }
        public bool IsActive { get; set; }
        public string? Password { get; set; }
    }
}
