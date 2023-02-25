using Demo.Repository.Entities;
using Demo.Repository.implements.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demo.Repository.implements
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DbContext _dbContext;

        public AccountRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 取得人員角色資料
        /// </summary>
        public User? GetUserRoleInfo(string email, string password)
        {
            return _dbContext.Set<User>()
                 .Include(x => x.UserRoles)
                 .ThenInclude(x => x.Role)
                 .SingleOrDefault(x => x.Email.Equals(email)
                                && x.Password.Equals(password));
        }

        /// <summary>
        /// 取得人員角色資料
        /// </summary>
        public User? GetUserRoleInfo(Guid userId)
        {
            return _dbContext.Set<User>()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .SingleOrDefault(x => x.UserId == userId);
        }
    }
}
