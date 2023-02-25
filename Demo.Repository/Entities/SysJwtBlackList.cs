using System;
using System.Collections.Generic;

namespace Demo.Repository.Entities
{
    public partial class SysJwtBlackList
    {
        public Guid SysJwtBlackListId { get; set; }
        public string JwtToken { get; set; } = null!;
        public DateTime CreateDtm { get; set; }
    }
}
