using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.ViewModels.AccountViewModels
{
    public class GetRegisterUserVm
    {
        public IEnumerable<GetRegisterUserVm_UserInfo> UsersInfo { get; set; } = new List<GetRegisterUserVm_UserInfo>();
    }

    public class GetRegisterUserVm_UserInfo
    {
        public string RegisterUserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
