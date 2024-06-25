using Domain.Common;

namespace Domain.Entities.UKG
{
    public class WorkedShift : BaseEntity
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int? ScheduledShiftId { get; set; } //could pick shifts that occur before punch in date at earliest date
        public DateTime PunchIn { get; set; }
        public DateTime PunchOut { get; set; }
        public string? Exception { get; set; }
        public TimeSpan? HoursLate { get; set; }
        public string? Facility { get; set; }
        public string? Department { get; set; }
        public string? Shift { get; set; }
        public required Employee Employee { get; set; }
        public required ScheduleShift ScheduleShift { get; set; }
    }
}
