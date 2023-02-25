namespace Demo.Services.Dtos.AccountDtos
{
    public class UserRoleInfoDto
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public IEnumerable<string> UserRoles { get; set; } = new List<string>();
    }

}
