using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.ViewModels
{
    public class GetRegisterUserVm
    {
        public List<GetRegisterUserVm_UserInfo> UsersInfo { get; set; } = new List<GetRegisterUserVm_UserInfo>();
    }

    public class GetRegisterUserVm_UserInfo
    {
        [Required]
        public string RegisterUserId { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
