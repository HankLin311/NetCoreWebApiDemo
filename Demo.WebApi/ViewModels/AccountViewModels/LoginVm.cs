using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.ViewModels.AccountViewModels
{
    public class LoginVm
    {
        public string Jwt { get; set; } = null!;
        public string RefToken { get; set; } = null!;
    }
}
