using System;
using System.Collections.Generic;

namespace Demo.Repository.DemoDb.Entities
{
    public partial class LogOperator
    {
        public Guid LogOperatorId { get; set; }
        public string? Email { get; set; }
        public string Path { get; set; } = null!;
        public DateTime CreateDtm { get; set; }
        public string? Request { get; set; }
        public string? Response { get; set; }
    }
}
