using Demo.Repository.DemoDb.Entities;
using Demo.Repository.DemoDb.Implements.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demo.Repository.Infrastructures
{
    public interface IDemoUow : IDisposable
    {
        // Uow 
        DbContext Context { get; }
        int SaveChange();

        // 通用 Repository
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<LogOperator> LogOperatorRepository { get; }
        IGenericRepository<SysJwtBlackList> SysJwtBlackListRepository { get; }
        IGenericRepository<SysRefTokenList> SysRefTokenListRepository { get; }
        IGenericRepository<RegisterUser> RegisterUserRepository { get; }

        // 專用 Repository
        IAccountRepository AccountRepository { get; }
    }
}
