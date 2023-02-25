using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Services.Dtos.AccountDtos
{
    public class GetRegisterUsersDto
    {
        public IEnumerable<GetRegisterUserDto_UsersInfo> UsersInfo { get; set; } = new List<GetRegisterUserDto_UsersInfo>();
    }

    public class GetRegisterUserDto_UsersInfo
    {
        public string RegisterUserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
