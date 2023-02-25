using Demo.Repository.Entities;
using Demo.Repository.Infrastructures;
using Demo.Services.Dtos.AccountDtos;

namespace Demo.Services.Facades
{
    internal class AccountFacade
    {
        private readonly IDemoUow _demoUow;

        public AccountFacade(IDemoUow demoUow) 
        {
            _demoUow = demoUow;
        }

        /// <summary>
        /// 產生使用者權限關聯
        /// </summary>
        public List<UserRole> CreateUserRoleAssociation(ApproveRegisterDto approveRegisterDto)
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