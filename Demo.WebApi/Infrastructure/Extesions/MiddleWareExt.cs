using Demo.Common.ConstVariables;
using Demo.Common.Helpers;
using Demo.Repository.Datas;
using Demo.Repository.DemoDb.Implements.Interfaces;
using Demo.Repository.DemoDb.Implements;
using Demo.Repository.Infrastructures;
using Demo.Services.Implements.Interfaces;
using Demo.Services.Implements;
using Demo.WebApi.Infrastructure.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace Demo.WebApi.Infrastructure.Extesions
{
    public static class MiddleWareExt
    {
        /// <summary>
        /// 找出最底層的 Exception
        /// </summary>
        public static Exception GetInnerException(this Exception exception)
        {
            if (exception.InnerException == null)
            {
                return exception;
            }

            return exception.InnerException.GetInnerException();
        }


        /// <summary>
        /// 自訂 ModelBinding 錯誤訊息
        /// </summary>
        public static void AddModelBindingErrorEvent(this ApiBehaviorOptions options)
        {
            options.SuppressModelStateInvalidFilter = true;

            options.InvalidModelStateResponseFactory = (actionContext) =>
            {
                var problemDetails = new ValidationProblemDetails(actionContext.ModelState);

                var validErrorMsgs = problemDetails.Errors
                    .ToLookup(x => x.Key)
                    .Select(x =>
                    {
                        string errorMsg = $"{x.Key} : ";
                        foreach (var fs in x.ToList())
                        {
                            errorMsg += string.Join(" / ", fs.Value);
                        }

                        return errorMsg;
                    })
                    .ToArray();

                string validErrorMsgTotal = validErrorMsgs.Length > 0 ? string.Join(" || ", validErrorMsgs) : "輸入 Paramter 錯誤";

                var result = new ApiJsonResponse()
                {
                    StatusCode = ApiResultStatus.CUST_ERROR,
                    ErrorMessage = validErrorMsgTotal
                };

                return new BadRequestObjectResult(result);

            };
        }

        /// <summary>
        /// 自訂需要加入的DI
        /// </summary>
        public static void AddDi(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddSingleton<JwtHelper>();
            serviceDescriptors.AddScoped<DbContext, DemoContext>();
            serviceDescriptors.AddScoped<IDemoUow, DemoUow>();
            serviceDescriptors.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            serviceDescriptors.AddScoped<IAccountRepository, AccountRepository>();
            serviceDescriptors.AddScoped<IAccountService, AccountService>();
        }

        /// <summary>
        /// 加入 AllowAny Cors設定
        /// </summary>
        public static void AddAllowAnyCors(this IServiceCollection serviceDescriptors, string allowAnyOriginsName)
        {
            serviceDescriptors.AddCors(options =>
            {
                options.AddPolicy(allowAnyOriginsName,
                    policy =>
                    {
                        // 正式環境，不可使用 *
                        policy.WithOrigins("*")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });
        }

        /// <summary>
        /// 加入 Swagger 設定
        /// </summary>
        public static void AddSwaggerDoc(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddSwaggerGen((swaggerGenOptions) =>
            {
                // API 服務簡介
                swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NetCoreWebApiDemo",
                    Version = "v1",
                });

                // 顯示 Authorization 按鈕和頁面
                swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // 幫忙將請求加上 Authorization
                swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>(){}
                        }
                    });

                // 讀取 XML 檔案產生 API 說明
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swaggerGenOptions.IncludeXmlComments(xmlPath);
            });
        }
    }
}
