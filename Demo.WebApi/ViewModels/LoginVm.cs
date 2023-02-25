using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.ViewModels
{
    public class LoginVm
    {
        [Required]
        public string Jwt { get; set; } = null!;
        [Required]
        public string RefToken { get; set; } = null!;
    }
}
