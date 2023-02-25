using System;
using System.Collections.Generic;

namespace Demo.Repository.Entities
{
    public partial class SysRefTokenList
    {
        public Guid SysRefTokenListId { get; set; }
        public Guid UserId { get; set; }
        public Guid RefToken { get; set; }
        public DateTime CreateDtm { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
