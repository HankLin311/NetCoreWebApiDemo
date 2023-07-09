using Demo.Common.ConstVariables;
using Demo.Common.Enums;
using Demo.Common.Helpers;
using Demo.WebApi.Infrastructure.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Demo.WebApi.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SysRoleFilter : Attribute, IAuthorizationFilter
    {
        readonly IEnumerable<DemoRole> _sysRoles;

        public SysRoleFilter(DemoRole sysRole)
        {
            _sysRoles = new DemoRole[] { sysRole };
        }

        public SysRoleFilter(params DemoRole[] sysRoles)
        {
            _sysRoles = sysRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 服務定位器注入
            JwtHelper? _jwtHelper = context.HttpContext.RequestServices.GetService(typeof(JwtHelper)) as JwtHelper;
            ILogger<SysRoleFilter>? _logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<SysRoleFilter>)) as ILogger<SysRoleFilter>;

            if (_jwtHelper is not null && _logger is not null)
            {
                // 取得登入資訊
                if (!_jwtHelper.TryGetJwtPayload(context.HttpContext.Request, out JwtPayloadDto jwtPayloadDto))
                {
                    throw new Exception("驗證角色權限時，取不到 JWT 資訊");
                }

                // 判斷登入角色是否符合
                if (!jwtPayloadDto.Roles.Any(x => _sysRoles.Contains(x)))
                {
                    string userEmail = jwtPayloadDto.Sub;
                    string path = context.HttpContext.Request.Path;
                    string errorMsg = "角色權限不足";

                    // 紀錄 Log
                    _logger.WriteError(userEmail, path, errorMsg);

                    // 回傳 Response
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = new JsonResult(new ApiJsonResponse
                    {
                        StatusCode = ApiResultStatus.CUST_ERROR,
                        ErrorMessage = errorMsg
                    });
                }
            }
            else
            {
                throw new Exception("SysRoleFilter DI注入失敗");
            }
        }
    }
}
