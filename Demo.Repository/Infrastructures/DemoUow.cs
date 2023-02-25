using Demo.Repository.Entities;
using Demo.Repository.implements.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demo.Repository.Infrastructures
{
    public class DemoUow : IDemoUow
    {
        private bool disposedValue;

        // 建構子注入放入屬性
        public DbContext Context { get; private set; }
        public IGenericRepository<User> UserRepository { get; private set; }
        public IGenericRepository<Role> RoleRepository { get; private set; }
        public IGenericRepository<LogOperator> LogOperatorRepository { get; private set; }
        public IGenericRepository<SysJwtBlackList> SysJwtBlackListRepository { get; private set; }
        public IGenericRepository<SysRefTokenList> SysRefTokenListRepository { get; private set; }
        public IGenericRepository<RegisterUser> RegisterUserRepository { get; private set; }
        public IAccountRepository AccountRepository { get; private set; }

        public DemoUow(DbContext context,
            IGenericRepository<User> userRepository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<LogOperator> logOperatorRepository,
            IGenericRepository<SysRefTokenList> sysRefTokenListRepository,
            IGenericRepository<SysJwtBlackList> sysJwtBlackListRepository,
            IGenericRepository<RegisterUser> registerUserRepository,
            IAccountRepository accountRepository
            )
        {
            Context = context;
            UserRepository = userRepository;
            RoleRepository = roleRepository;
            LogOperatorRepository = logOperatorRepository;
            SysRefTokenListRepository = sysRefTokenListRepository;
            SysJwtBlackListRepository = sysJwtBlackListRepository;
            RegisterUserRepository = registerUserRepository;
            AccountRepository = accountRepository;
        }

        /// <summary>
        /// 儲存操作
        /// </summary>
        public int SaveChange()
        {
            return Context.SaveChanges();
        }

        /// <summary>
        /// IDisposable 實作
        /// </summary>
        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 處置受控狀態 (受控物件)
                    Context.Dispose();
                }

                // 已釋放
                disposedValue = true;
            }
        }
    }
}
