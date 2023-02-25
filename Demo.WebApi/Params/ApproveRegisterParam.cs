using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Demo.WebApi.Params
{
    public class ApproveRegisterParam
    {
        [Required(ErrorMessage = "不可為空", AllowEmptyStrings = false)]
        public string RegisterUserId { get; set; } = null!;

        [Required(ErrorMessage = "不可為空")]
        [MinLength(length: 1, ErrorMessage = "至少輸入一個角色")]
        public IEnumerable<string> Roles { get; set; } = null!;
    }
}
