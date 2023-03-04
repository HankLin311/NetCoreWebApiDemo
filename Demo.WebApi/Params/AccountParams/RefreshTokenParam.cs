using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Params.AccountParams
{
    public class RefreshTokenParam
    {
        [Required(ErrorMessage = "不可為空", AllowEmptyStrings = false)]
        public string RefToken { get; set; } = null!;
    }
}
