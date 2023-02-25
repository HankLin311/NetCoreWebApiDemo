using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Demo.Common.Exceptions;
using Demo.Common.Helpers;
using Demo.WebApi.Infrastructure.ApiResponse;

namespace Demo.WebApi.Infrastructure.Filters
{
    /// <summary>
    /// 自訂例外捕捉器
    /// </summary>
    public class CustExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustExceptionFilter> _logger;
        private readonly JwtHelper _jwtHelper;

        public CustExceptionFilter(ILogger<CustExceptionFilter> logger, JwtHelper jwtHelper)
        {
            _logger = logger;
            _jwtHelper = jwtHelper;
        }

        public void OnException(ExceptionContext exceptionContext)
        {
            // 取得個人資訊，如果沒取到代表是匿名登入
            _jwtHelper.TryGetJwtPayload(exceptionContext.HttpContext.Request, out JwtPayloadDto jwtPayloadDto);

            // 是否來自自訂例外錯誤
            if (exceptionContext.Exception is CustException)
            {
                // 自訂例外
                CustException custException = (CustException)exceptionContext.Exception;

                string userEmail = jwtPayloadDto.Sub;
                string path = exceptionContext.HttpContext.Request.Path;
                string errorMsg = custException.Message;
                
                // 寫入 Debug Log
                _logger.WriteError(userEmail, path, errorMsg);

                // 回傳錯誤訊息
                exceptionContext.Result = new JsonResult(new ApiJsonResponse()
                {
                    StatusCode = custException.Status,
                    ErrorMessage = errorMsg
                });

                // 回傳 Http Status
                exceptionContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                // 告訴其他 Exception 這個錯誤已經被處理過
                exceptionContext.ExceptionHandled = true;
            }
            else
            {
                throw exceptionContext.Exception;
            }
        }
    }
}
