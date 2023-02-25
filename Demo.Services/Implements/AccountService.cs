using Demo.Common.Exceptions;
using Demo.Common.Helpers;
using Demo.Repository.Entities;
using Demo.Repository.Infrastructures;
using Demo.Services.Dtos.AccountDtos;
using Demo.Services.Implements.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Demo.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IDemoUow _demoUow;
        private readonly JwtHelper _jwtHelper;

        public AccountService(IDemoUow demoUow, JwtHelper jwtHelper)
        {
            _demoUow = demoUow;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 產生指定人員的 JWT
        /// </summary>
        public string CreateJwt(UserRoleInfoDto userRole)
        {
            // 建立特徵
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtClaimType.Sub, userRole.Email),
                new Claim(JwtClaimType.Jti, Guid.NewGuid().ToString())
            };

            foreach (string role in userRole.UserRoles)
            {
                claims.Add(new Claim(JwtClaimType.Roles, role));
            }

            return _jwtHelper.CreateJwt(claims);
        }

        /// <summary>
        /// 取出人員角色資料
        /// </summary>
        public UserRoleInfoDto GetUserRoleInfo(UserRoleInfoDto getAccountRolesDto)
        {
            // 取得人員和人員角色資料
            User? userRoleInfo = _demoUow.AccountRepository
                .GetUserRoleInfo(getAccountRolesDto.Email, getAccountRolesDto.Password);

            if (userRoleInfo is null)
            {
                throw new CustException("電子信箱或密碼輸入錯誤");
            }

            // 取得人員權限清單
            List<string> userRoles =
                userRoleInfo.UserRoles.Select(x => x.Role.RoleName).ToList();

            return new UserRoleInfoDto()
            {
                UserId = userRoleInfo.UserId.ToString(),
                Email = userRoleInfo.Email,
                UserRoles = userRoles
            };
        }

        /// <summary>
        /// 取得更換 RefToken 的人員角色資料
        /// </summary>
        public UserRoleInfoDto GetChangeRefTokenUserRoleInfo(string refToken)
        {
            // 查詢 refToken 是否到期
            SysRefTokenList? reftokenList = _demoUow.SysRefTokenListRepository
                .FindFirst(x => x.RefToken == Guid.Parse(refToken)
                            && DateTime.Compare(x.EffectiveDate, DateTime.Now) == 1);

            if (reftokenList is null)
            {
                throw new CustException("登入到期，請重新登入");
            }

            // 取得人員角色資料
            User? userRoleInfo = _demoUow.AccountRepository.GetUserRoleInfo(reftokenList.UserId);

            if (userRoleInfo is null)
            {
                throw new CustException("找不到使用者資料資料");
            }

            // 取得人員權限清單
            List<string> userRoles =
                userRoleInfo.UserRoles.Select(x => x.Role.RoleName).ToList();

            return new UserRoleInfoDto()
            {
                UserId = userRoleInfo.UserId.ToString(),
                Email = userRoleInfo.Email,
                UserRoles = userRoles
            };
        }

        /// <summary>
        /// 建立 RefToken
        /// </summary>
        public string CreateRefToken(string userId)
        {
            // 取得 RefToken
            Guid refToken = Guid.NewGuid();

            // 紀錄 RefToken 到資料庫
            _demoUow.SysRefTokenListRepository.Add(new SysRefTokenList()
            {
                UserId = Guid.Parse(userId),
                EffectiveDate = DateTime.Now.AddMinutes(30),
                RefToken = refToken,
                SysRefTokenListId = Guid.NewGuid()
            });

            _demoUow.SaveChange();

            return refToken.ToString();
        }

        /// <summary>
        /// 更新 RefToken
        /// </summary>
        public string UpdateRefToken(string oldRefToken)
        {
            // 取得新的 RefToken
            Guid newRefToken = Guid.NewGuid();

            // 更改成新的 RefToken 的 UUID
            SysRefTokenList? tokenList = _demoUow.SysRefTokenListRepository.FindSingle(x => x.RefToken == Guid.Parse(oldRefToken));

            if (tokenList is null)
            {
                throw new CustException("更新 RefToken 發生錯誤");
            }

            tokenList.RefToken = newRefToken;

            // 更新 RefToken 到資料庫
            _demoUow.SysRefTokenListRepository.Update(tokenList);

            _demoUow.SaveChange();

            return newRefToken.ToString();
        }

        /// <summary>
        /// 處理登出
        /// </summary>
        public void Logout(string jwt, string email)
        {
            // 將 JWT 加入黑名單
            _demoUow.SysJwtBlackListRepository.Add(new SysJwtBlackList()
            {
                SysJwtBlackListId = Guid.NewGuid(),
                JwtToken = jwt
            });

            // 取出需要移除 RefToken 的使用者
            var userInfo = _demoUow.UserRepository.FindSingle(x => (x.Email).Equals(email));

            if (userInfo is null)
            {
                // 找不到使用者，但此方法為處理登出，故不回傳錯誤
                return;
            }

            // 刪除使用者所有的 RefToken
            _demoUow.SysRefTokenListRepository.RemoveConditions(x => x.UserId == userInfo.UserId);

            _demoUow.SaveChange();
        }

        /// <summary>
        /// 建立註冊人員資料
        /// </summary>
        public void CreateRegisterUserInfo(RegisterDto registerDto)
        {
            // 確認 email 是否相同
            var userInfo = _demoUow.UserRepository.FindSingle(x => x.Email.Equals(registerDto.Email));
            
            if (userInfo is not null) 
            {
                // 權限驗證以 email 為主
                throw new CustException("此電子信箱已經註冊");
            }

            // 建立註冊人員資料
            _demoUow.RegisterUserRepository.Add(new RegisterUser()
            {
                RegisterUserId = Guid.NewGuid(),
                Address = registerDto.Address,
                Email = registerDto.Email,
                Password = registerDto.Password,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.UserName
            });

            _demoUow.SaveChange();
        }

        /// <summary>
        /// 取得註冊人員清單
        /// </summary>
        public GetRegisterUsersDto GetRegisterUsers()
        {
            // 取得註冊人員清單
            var registerUsersInfo = _demoUow.RegisterUserRepository
                .FindAll()
                .Select(x => new GetRegisterUserDto_UsersInfo()
                {
                    Address = x.Address ?? string.Empty,
                    Password = x.Password,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber ?? string.Empty,
                    UserName = x.UserName,
                    RegisterUserId = x.RegisterUserId.ToString()
                }).ToList();

            return new GetRegisterUsersDto()
            {
                UsersInfo = registerUsersInfo
            };
        }

        /// <summary>
        /// 同意註冊
        /// </summary>
        public void ApproveRegister(ApproveRegisterDto approveRegisterDto)
        {
            // 取出註冊時的 USER 資料
            RegisterUser? registerUser = _demoUow.RegisterUserRepository
                .FindSingle(x => x.RegisterUserId == Guid.Parse(approveRegisterDto.RegisterUserId));

            if (registerUser is null)
            {
                throw new CustException("找不到註冊時的 User 資料");
            }

            // 註冊使用者資料
            _demoUow.UserRepository.Add(new User()
            {
                UserId = Guid.NewGuid(),
                Address = registerUser.Address,
                Email = registerUser.Email,
                Password = registerUser.Password,
                PhoneNumber = registerUser.PhoneNumber,
                UserName = registerUser.UserName,
                UserRoles = CreateUserRoleAssociation(approveRegisterDto)
            });

            // 移除註冊資料
            _demoUow.RegisterUserRepository.Remove(registerUser);

            _demoUow.SaveChange();
        }

        /// <summary>
        /// 是否在登出名單中
        /// </summary>
        public bool IsLogout(string jwt)
        {
            // 查出是否有在 BlackList 中
            var blackList = _demoUow.SysJwtBlackListRepository.FindConditions(x => (x.JwtToken).Equals(jwt));

            if (blackList is null || blackList.Count() == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 產生使用者權限關聯
        /// </summary>
        private List<UserRole> CreateUserRoleAssociation(ApproveRegisterDto approveRegisterDto)
        {
            // 產生出 USERROLE 關聯表資料
            Guid userId = Guid.NewGuid();

            List<Role> roles = _demoUow.RoleRepository
                .FindConditions(x => approveRegisterDto.Roles.Contains(x.RoleName)).ToList();

            List<UserRole> userRoles = new List<UserRole>();

            foreach (Role role in roles)
            {
                userRoles.Add(new UserRole()
                {
                    RoleId = role.RoleId,
                    UserId = userId,
                    UserRoleId = Guid.NewGuid()
                });
            }

            return userRoles;
        }
    }
}
