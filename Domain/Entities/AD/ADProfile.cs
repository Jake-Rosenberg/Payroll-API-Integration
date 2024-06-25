using Domain.Common;

namespace Domain.Entities.AD
{
    public class ADProfile : BaseEntity
    {
        public int Id { get; set; }
        public string? Payroll { get; set; }
        public string? UserPrincipalName { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Title { get; set; }
        public string? Department { get; set; }
        public bool? Enabled { get; set; }
        public DateTimeOffset? LastPasswordChange { get; set; }
        public string? CompanyName { get; set; }
    }
}