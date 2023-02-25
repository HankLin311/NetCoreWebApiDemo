using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Params
{
    public class RegisterParam
    {
        [Required(ErrorMessage = "不可為空", AllowEmptyStrings = false)]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "不可為空", AllowEmptyStrings = false)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "不可為空", AllowEmptyStrings = false)]
        public string Password { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
