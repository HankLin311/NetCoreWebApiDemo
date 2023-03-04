using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Demo.WebApi.Infrastructure.Attributes;
using Demo.Common.ConstVariables;
using Demo.WebApi.Infrastructure.ApiResponse;
using Demo.Common.Helpers;
using Demo.Repository.Infrastructures;

namespace Demo.WebApi.Infrastructure.Filters
{
    /// <summary>
    /// 登入權限驗證
    /// </summary>
    public class SysAuthFilter : IAuthorizationFilter
    {
        private readonly ILogger<SysAuthFilter> _logger;
        private readonly JwtHelper _jwtHelper;
        private readonly IDemoUow _demoUow;

        public SysAuthFilter(ILogger<SysAuthFilter> logger, JwtHelper JwtHelper, IDemoUow demoUow)
        {
            _logger = logger;
            _jwtHelper = JwtHelper;
            _demoUow = demoUow;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 判斷是否可以接收匿名登入
            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<SysAllowAnonymous>().Any();
            if (!hasAllowAnonymous)
            {
                string errorMsg = string.Empty;

                // jwt 是否合法
                if (!_jwtHelper.CheckAuth(context.HttpContext.Request, out string jwt))
                {
                    errorMsg = "未登入系統，請重新登入";
                }
                // Jwt 是否加入到黑名單(登出)
                else if (IsBlockJwt(jwt))
                {
                    errorMsg = "未登入系統，請重新登入";
                }

                // 登入時是否發生錯誤
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    string path = context.HttpContext.Request.Path;

                    // 紀錄 Log
                    _logger.WriteError(string.Empty, path, errorMsg);

                    // 回傳 Response
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = new JsonResult(new ApiJsonResponse
                    {
                        StatusCode = ApiResultStatus.CUST_ERROR,
                        ErrorMessage = errorMsg
                    });
                }
            }
        }

        private bool IsBlockJwt(string jwt) 
        {
            // 查出是否有在 BlackList 中
            var blackList = _demoUow.SysJwtBlackListRepository.FindConditions(x => (x.JwtToken).Equals(jwt));

            if (blackList is null || blackList.Count() == 0)
            {
                return false;
            }

            return true;
        }
    }
}
