using Demo.Common.ConstVariables;
using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Infrastructure.ApiResponse
{
    /// <summary>
    /// API JSON 輸出格式
    /// </summary>
    public class ApiJsonResponse
    {
        [Required]
        public string StatusCode { get; set; } = ApiResultStatus.SYS_ERROR;

        public string ErrorMessage { get; set; } = string.Empty;

        [Required]
        public string Dtm { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }

    /// <summary>
    /// API JSON 輸出格式 + 回傳物件
    /// </summary>
    public class ApiJsonResponse<T> where T : class
    {
        [Required]
        public string StatusCode { get; set; } = ApiResultStatus.SYS_ERROR;

        public string ErrorMessage { get; set; } = string.Empty;

        [Required]
        public string Dtm { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        public T? Data { get; set; }
    }
}
