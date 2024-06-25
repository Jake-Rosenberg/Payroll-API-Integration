using Domain.Common;

namespace Domain.Entities.UKG
{
    public class HoursWorked : BaseEntity
    {
        public int HoursWorkedId { get; set; }
        public int PersonId { get; set; }
        public double TotalHoursWorked { get; set; }
        public required Employee Employee { get; set; }
    }
}
