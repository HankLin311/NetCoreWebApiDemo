using Demo.Common.ConstVariables;
using Demo.WebApi.Infrastructure.ApiResponse;
using System.Diagnostics;
using System.Text.Json;

namespace Demo.WebApi.Infrastructure.MiddleWare
{
    public class SysErrorHandlerMiddleware
    {
        private readonly ILogger<SysErrorHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public SysErrorHandlerMiddleware(
            ILogger<SysErrorHandlerMiddleware> logger, 
            RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            // MiddleWare 錯誤
            catch (Exception ex)
            {
                // 找出最底層錯誤原因
                Exception innerException = ex.GetInnerException();

                string path = context.Request.Path;
                string MethodTrace = GetTraceMethodString(ex);
                string errorMsg = innerException.ToString();

                // 紀錄 Log
                _logger.WriteCritical(path, MethodTrace, errorMsg);

                // 回傳 Response
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(new ApiJsonResponse()
                    {
                        StatusCode = ApiResultStatus.SYS_ERROR,
                        ErrorMessage = "伺服器發生問題，請聯絡管理員"
                    }));
            }
        }

        /// <summary>
        /// 取得追蹤最接近的6層Class Methode
        /// </summary>
        private static string GetTraceMethodString(Exception ex)
        {
            string traceMethodString = string.Empty;
            StackTrace stackTrace = new StackTrace(ex);
            StackFrame[] frames = stackTrace.GetFrames();

            for (int i = 0; i < frames.Length; i++)
            {
                var method = frames[i].GetMethod();

                if (i > 5)
                {
                    break;
                }
                else if (method is not null)
                {
                    var className = method.ReflectedType?.Name;
                    className = !string.IsNullOrEmpty(className) ? className + "." : string.Empty;

                    traceMethodString = $"{traceMethodString} {className}{method.Name} >";
                }
                else
                {
                    break;
                }
            }

            return traceMethodString.TrimEnd('>');
        }
    }
}
