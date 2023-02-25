using Microsoft.AspNetCore.Mvc;
using Demo.WebApi.Infrastructure.Attributes;
using Demo.Services.Dtos.AccountDtos;
using Demo.Common.ConstVariables;
using Demo.Common.Enums;
using Demo.WebApi.Infrastructure.Filters;
using Demo.WebApi.Infrastructure.ApiResponse;
using Demo.WebApi.Params;
using Demo.WebApi.ViewModels;
using Demo.Common.Helpers;
using Demo.Services.Implements.Interfaces;

namespace Demo.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly JwtHelper _jwtHelper;

        public AccountController(IAccountService accountService, JwtHelper jwtHelper)
        {
            _accountService = accountService;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 登入
        /// </summary>
        [HttpPost]
        [SysAllowAnonymous]
        public ApiJsonResponse<LoginVm> Login([FromBody] LoginParam loginParameter)
        {
            var userInfo =
                 _accountService.GetUserRoleInfo(new UserRoleInfoDto()
                 {
                     Email = loginParameter.Email,
                     Password = loginParameter.Password,
                 });

            string jwt = _accountService.CreateJwt(userInfo);

            string refToken = _accountService.CreateRefToken(userInfo.UserId);

            return new ApiJsonResponse<LoginVm>()
            {
                StatusCode = ApiResultStatus.SUCCESS,
                Data = new LoginVm()
                {
                    Jwt = jwt,
                    RefToken = refToken
                }
            };
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpPost]
        [SysAllowAnonymous]
        public ApiJsonResponse Logout()
        {
            if (_jwtHelper.TryGetJwtPayload(Request, out JwtPayloadDto jwtPayloadDto))
            {
                _accountService.Logout(jwtPayloadDto.Jwt, jwtPayloadDto.Sub);
            }

            return new ApiJsonResponse()
            {
                StatusCode = ApiResultStatus.SUCCESS
            };
        }

        /// <summary>
        /// 更新登入Token
        /// </summary>
        [HttpPost]
        [SysAllowAnonymous]
        public ApiJsonResponse<RefreshTokenVm> RefreshLoginToken([FromBody] RefreshTokenParam refreshTokenRequest)
        {
            var userInfo = _accountService.GetChangeRefTokenUserRoleInfo(refreshTokenRequest.RefToken);

            string newJwt = _accountService.CreateJwt(userInfo);

            string newRefToken = _accountService.UpdateRefToken(refreshTokenRequest.RefToken);

            return new ApiJsonResponse<RefreshTokenVm>()
            {
                StatusCode = ApiResultStatus.SUCCESS,
                Data = new RefreshTokenVm()
                {
                    Jwt = newJwt,
                    RefToken = newRefToken
                }
            };
        }

        /// <summary>
        /// 註冊
        /// </summary>
        [HttpPost]
        [SysAllowAnonymous]
        public ApiJsonResponse Register([FromBody] RegisterParam refreshTokenRequest)
        {
            // 建立註冊人員資料
            _accountService.CreateRegisterUserInfo(new RegisterDto()
            {
                UserName = refreshTokenRequest.UserName,
                Address = refreshTokenRequest.Address ?? string.Empty,
                Email = refreshTokenRequest.Email,
                Password = refreshTokenRequest.Password,
                PhoneNumber = refreshTokenRequest.PhoneNumber ?? string.Empty,
            });

            return new ApiJsonResponse()
            {
                StatusCode = ApiResultStatus.SUCCESS
            };
        }

        /// <summary>
        /// 取得所有註冊人員資料
        /// </summary>
        [HttpPost]
        [SysRoleFilter(DemoRole.ADMIN, DemoRole.MANAGER)]
        public ApiJsonResponse<GetRegisterUserVm> GetRegisterUsers()
        {
            var registerUsersInfo =
                _accountService.GetRegisterUsers()
                .UsersInfo
                .Select(x => new GetRegisterUserVm_UserInfo()
                {
                    Address = x.Address,
                    Email = x.Email,
                    Password = x.Password,
                    PhoneNumber = x.PhoneNumber,
                    RegisterUserId = x.RegisterUserId,
                    UserName = x.UserName
                })
                .ToList();

            return new ApiJsonResponse<GetRegisterUserVm>()
            {
                StatusCode = ApiResultStatus.SUCCESS,
                Data = new GetRegisterUserVm
                {
                    UsersInfo = registerUsersInfo
                }
            };
        }

        /// <summary>
        /// 同意註冊
        /// </summary>
        [HttpPost]
        [SysRoleFilter(DemoRole.ADMIN)]
        public ApiJsonResponse ApproveRegister([FromBody] ApproveRegisterParam refreshTokenRequest)
        {
            _accountService.ApproveRegister(new ApproveRegisterDto()
            {
                RegisterUserId = refreshTokenRequest.RegisterUserId,
                Roles = refreshTokenRequest.Roles
            });

            return new ApiJsonResponse()
            {
                StatusCode = ApiResultStatus.SUCCESS
            };
        }
    }
}
