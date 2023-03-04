using Demo.Common.Helpers;
using Demo.Repository.DemoDb.Entities;
using Demo.Repository.Infrastructures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Demo.WebApi.Infrastructure.Filters
{
    /// <summary>
    /// 紀錄使用者操作 Log Filter
    /// </summary>
    public class SysOperateFIlter : IActionFilter
    {
        private readonly IDemoUow _demoUow;
        private readonly JwtHelper _jwtHelper;

        public SysOperateFIlter(IDemoUow demoUow, JwtHelper jwtHelper)
        {
            _demoUow = demoUow;
            _jwtHelper = jwtHelper;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            object? response = (context.Result as ObjectResult)?.Value;
            string email = string.Empty; // 匿名登入

            // 取得登入資訊
            if (_jwtHelper.TryGetJwtPayload(context.HttpContext.Request, out JwtPayloadDto jwtPayloadDto))
            {
                email = jwtPayloadDto.Sub;
            }

            // 存入 DB 操作紀錄
            _demoUow.LogOperatorRepository.Add(new LogOperator()
            {
                Email = email,
                Path = context.HttpContext.Request.Path,
                LogOperatorId = Guid.NewGuid(),
                Response = JsonSerializer.Serialize(response)
            });

            _demoUow.SaveChange();
        }

        // 進入 Action 前，記錄使用者操作
        public void OnActionExecuting(ActionExecutingContext context)
        {
            object? request = context.ActionArguments;
            string email = string.Empty; // 匿名登入

            // 取得登入資訊
            if (_jwtHelper.TryGetJwtPayload(context.HttpContext.Request, out JwtPayloadDto jwtPayloadDto)) 
            {
                email = jwtPayloadDto.Sub;
            }

            // 存入 DB 操作紀錄
            _demoUow.LogOperatorRepository.Add(new LogOperator()
            {
                Email = email,
                Path = context.HttpContext.Request.Path,
                LogOperatorId = Guid.NewGuid(),
                Request = JsonSerializer.Serialize(request)
            });

            _demoUow.SaveChange();
        }
    }
}
