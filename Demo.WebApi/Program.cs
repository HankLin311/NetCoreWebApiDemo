using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Demo.WebApi.Infrastructure.Filters;
using Demo.WebApi.Infrastructure.MiddleWare;
using Demo.Services.Implements;
using Demo.Common.Helpers;
using Demo.Repository.implements;
using Demo.Repository.Infrastructures;
using Demo.Repository.Datas;
using Demo.Repository.implements.Interfaces;
using Demo.Services.Implements.Interfaces;
using Demo.WebApi.Infrastructure.ApiResponse;
using Demo.Common.ConstVariables;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog
            var logger = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();

            logger.Debug("init API");

            try
            {
                // 設定同源
                var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

                var builder = WebApplication.CreateBuilder(args);

                // 註冊 Controllter
                builder.Services
                    .AddControllers(config =>
                    {
                        // 自訂登入驗證 Filter
                        config.Filters.Add<SysAuthFilter>();

                        // 使用者操作 Filter
                        config.Filters.Add<SysOperateFIlter>();

                        // 捕捉自訂例外 Filter
                        config.Filters.Add<CustExceptionFilter>();
                    })
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        #region 處理 Modal Binding 錯誤
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
                        #endregion
                    });

                // 注入 EF Core Sql Server Provider & 連線字串
                builder.Services
                    .AddDbContext<DemoContext>(options =>
                    {
                        options.UseSqlServer("name=ConnectionStrings:MusicShopContextDBString");
                    });

                // 改用 NLog 
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                // 自訂需要 DI
                builder.Services.AddSingleton<JwtHelper>();
                builder.Services.AddScoped<DbContext, DemoContext>();
                builder.Services.AddScoped<IDemoUow, DemoUow>();
                builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                builder.Services.AddScoped<IAccountRepository, AccountRepository>();
                builder.Services.AddScoped<IAccountService, AccountService>();

                // 註冊 CORS 
                builder.Services.AddCors(options =>
                {
                    // 正式環境，不可使用 *
                    options.AddPolicy(MyAllowSpecificOrigins,
                        policy =>
                        {
                            policy.WithOrigins("*");
                        });
                });

                // Swagger
                builder.Services.AddSwaggerGen((swaggerGenOptions) =>
                {
                    #region 設定 Swagger API

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

                    #endregion
                });

                var app = builder.Build();

                // 應用程式層級例外處理
                app.UseMiddleware<SysErrorHandlerMiddleware>();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // 啟用 CORS
                app.UseCors(MyAllowSpecificOrigins);

                // 強制將 http 轉成 https
                app.UseHttpsRedirection();

                // 授權
                app.UseAuthentication();

                // 權限驗證
                app.UseAuthorization();

                // 對應屬性路由控制器
                app.MapControllers();

                app.Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "API Shutdown");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}