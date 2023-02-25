namespace Demo.Services.Dtos.AccountDtos
{
    public class ApproveRegisterDto
    {
        public string RegisterUserId { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
