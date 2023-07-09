using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Demo.WebApi.Infrastructure.Filters;
using Demo.WebApi.Infrastructure.MiddleWare;
using Demo.Repository.Datas;
using Demo.WebApi.Infrastructure.Extesions;
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

            logger.Debug("start API");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Controllter
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

                        options.AddModelBindingErrorEvent();
                    })
                     ;

                // EF Core Sql Server Provider & 連線字串
                builder.Services
                    .AddDbContext<DemoContext>(options =>
                    {
                        options.UseSqlServer("name=ConnectionStrings:MusicShopContextDBString");
                    });

                // 改用 NLog 
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                // 自訂 DI
                builder.Services.AddDi();

                // 自訂 CORS 
                builder.Services.AddAllowAnyCors(builder.Configuration["CorsStrings:AllowAnyOrigins"]);

                // 自訂 Swagger
                builder.Services.AddSwaggerDoc();

                // 啟動 
                var app = builder.Build();

                // 應用程式層級例外處理
                app.UseMiddleware<SysErrorHandlerMiddleware>();

                // 如果是開發環境下
                if (app.Environment.IsDevelopment())
                {
                    // 使用 Swagger
                    app.UseSwagger();
                    app.UseSwaggerUI();

                    // 啟用 CORS
                    app.UseCors(builder.Configuration["CorsStrings:AllowAnyOrigins"]);
                }

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