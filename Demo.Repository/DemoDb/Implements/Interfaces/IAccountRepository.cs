using Demo.Repository.DemoDb.Entities;

namespace Demo.Repository.DemoDb.Implements.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// 取得人員角色資料
        /// </summary>
        User? GetUserRoleInfo(string email, string password);

        /// <summary>
        /// 取得人員角色資料
        /// </summary>
        User? GetUserRoleInfo(Guid userId);
    }
}
