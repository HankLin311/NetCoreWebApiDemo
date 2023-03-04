using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Params.AccountParams
{
    public class LoginParam
    {
        [Required(ErrorMessage = "不可為空", AllowEmptyStrings = false)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "不可為空", AllowEmptyStrings = false)]
        public string Password { get; set; } = null!;
    }
}
