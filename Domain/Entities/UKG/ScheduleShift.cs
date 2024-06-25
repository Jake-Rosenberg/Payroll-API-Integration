using Domain.Common;

namespace Domain.Entities.UKG
{
    public class ScheduleShift : BaseEntity
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public DateOnly EventDate { get; set; }
        public required string Type { get; set; }

        // Navigation properties
        public required Employee Employee { get; set; }
        public WorkedShift? WorkedShift { get; set; }
    }
}