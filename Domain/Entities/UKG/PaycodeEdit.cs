using Domain.Common;

namespace Domain.Entities.UKG
{
    public class PaycodeEdit : BaseEntity
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int TransactionId { get; set; }
        public DateOnly ApplyDate { get; set; }
        public required string Paycode { get; set; }
        public required Employee Employee { get; set; }
    }
}
