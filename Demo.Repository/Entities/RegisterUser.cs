using System;
using System.Collections.Generic;

namespace Demo.Repository.Entities
{
    public partial class RegisterUser
    {
        public Guid RegisterUserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
