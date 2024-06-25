using Domain.Common;

namespace Domain.Entities.UKG
{
    public class D_LatePaycodeEdits : BaseEntity
    {
        public int TransactionId { get; set; }
        public int PersonId { get; set; }
        public TimeSpan HoursLate { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime PunchIn { get; set; }
        public string? Facility { get; set; }
        public string? Department { get; set; }
        public string? Shift { get; set; }
        public required Employee Employee { get; set; }
    }
}
