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
                // �]�w�P��
                var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

                var builder = WebApplication.CreateBuilder(args);

                // ���U Controllter
                builder.Services
                    .AddControllers(config =>
                    {
                        // �ۭq�n�J���� Filter
                        config.Filters.Add<SysAuthFilter>();

                        // �ϥΪ̾ާ@ Filter
                        config.Filters.Add<SysOperateFIlter>();

                        // �����ۭq�ҥ~ Filter
                        config.Filters.Add<CustExceptionFilter>();
                    })
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        #region �B�z Modal Binding ���~
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

                            string validErrorMsgTotal = validErrorMsgs.Length > 0 ? string.Join(" || ", validErrorMsgs) : "��J Paramter ���~";

                            var result = new ApiJsonResponse()
                            {
                                StatusCode = ApiResultStatus.CUST_ERROR,
                                ErrorMessage = validErrorMsgTotal
                            };

                            return new BadRequestObjectResult(result);
                        };
                        #endregion
                    });

                // �`�J EF Core Sql Server Provider & �s�u�r��
                builder.Services
                    .AddDbContext<DemoContext>(options =>
                    {
                        options.UseSqlServer("name=ConnectionStrings:MusicShopContextDBString");
                    });

                // ��� NLog 
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                // �ۭq�ݭn DI
                builder.Services.AddSingleton<JwtHelper>();
                builder.Services.AddScoped<DbContext, DemoContext>();
                builder.Services.AddScoped<IDemoUow, DemoUow>();
                builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                builder.Services.AddScoped<IAccountRepository, AccountRepository>();
                builder.Services.AddScoped<IAccountService, AccountService>();

                // ���U CORS 
                builder.Services.AddCors(options =>
                {
                    // �������ҡA���i�ϥ� *
                    options.AddPolicy(MyAllowSpecificOrigins,
                        policy =>
                        {
                            policy.WithOrigins("*");
                        });
                });

                // Swagger
                builder.Services.AddSwaggerGen((swaggerGenOptions) =>
                {
                    #region �]�w Swagger API

                    // API �A��²��
                    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "NetCoreWebApiDemo",
                        Version = "v1",
                    });

                    // ��� Authorization ���s�M����
                    swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    // �����N�ШD�[�W Authorization
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

                    // Ū�� XML �ɮײ��� API ����
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    swaggerGenOptions.IncludeXmlComments(xmlPath);

                    #endregion
                });

                var app = builder.Build();

                // ���ε{���h�Ũҥ~�B�z
                app.UseMiddleware<SysErrorHandlerMiddleware>();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // �ҥ� CORS
                app.UseCors(MyAllowSpecificOrigins);

                // �j��N http �ন https
                app.UseHttpsRedirection();

                // ���v
                app.UseAuthentication();

                // �v������
                app.UseAuthorization();

                // �����ݩʸ��ѱ��
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