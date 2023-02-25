using Demo.Services.Dtos.AccountDtos;

namespace Demo.Services.Implements.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// 取出人員角色資料
        /// </summary>
        UserRoleInfoDto GetUserRoleInfo(UserRoleInfoDto getAccountRolesDto);

        /// <summary>
        /// 取得更換 RefToken 的人員角色資料
        /// </summary>
        UserRoleInfoDto GetChangeRefTokenUserRoleInfo(string refToken);

        /// <summary>
        /// 建立 RefToken
        /// </summary>
        string CreateRefToken(string userId);

        /// <summary>
        /// 產生指定人員的 JWT
        /// </summary>
        string CreateJwt(UserRoleInfoDto userRole);

        /// <summary>
        /// 更新 RefToken
        /// </summary>
        string UpdateRefToken(string refToken);

        /// <summary>
        /// 處理登出
        /// </summary>
        void Logout(string jwt, string email);

        /// <summary>
        /// 建立註冊人員資料
        /// </summary>
        void CreateRegisterUserInfo(RegisterDto registerDto);

        /// <summary>
        /// 取得註冊人員清單
        /// </summary>
        GetRegisterUsersDto GetRegisterUsers();

        /// <summary>
        /// 同意註冊
        /// </summary>
        void ApproveRegister(ApproveRegisterDto ApproveRegisterDto);

        /// <summary>
        /// 是否在登出名單中
        /// </summary>
        bool IsLogout(string jwt);
    }
}
