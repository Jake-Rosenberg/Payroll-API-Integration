using Domain.Entities.UKG;

namespace UKG_Integration_Application.API_UKG.DTOs
{
    public class EmployeeTk // this is the overarching object
    {
        public int id { get; set; }
        //public string qualifier { get; set; }
        //public string name { get; set; }
    }

    public class ExceptionTk
    {
        public ExceptionTypeTk exceptionType { get; set; }
        public double violationInHours { get; set; } // Hours Late
    }

    public class ExceptionTypeTk
    {
        public int id { get; set; }
        public string displayName { get; set; }
    }

    public class OrgJobTk
    {
        //public int id { get; set; }
        public string qualifier { get; set; }
    }

    //public class TimecardListDTO
    //{
    //    public List<TimecardDTO> timecardDTOs { get; set; }
    //}

    public class TimecardDTO
    {
        public EmployeeTk employee { get; set; }
        public DateTime startDate { get; set; }
        public List<WorkedShiftDTO> workedShifts { get; set; }
        public List<PaycodeEditDTO> payCodeEdits { get; set; }
        public List<ScheduleShiftDTO> scheduleShifts { get; set; }
    }
    public class PaycodeEditDTO
    {
        public EmployeeTk employee { get; set; } // needed for join
        public PaycodeTk paycode { get; set; }
        public int id { get; set; } // primary key
        public int itemId { get; set; } // transaction ID
        public string applyDate { get; set; } // join to worked shift date
    }

    // NOTE: maybe you can eliminate the D_Late call and just use / filter by these
    public class PaycodeTk
    {
        public int id { get; set; }
        public string qualifier { get; set; }
    }

    public class ScheduleShiftDTO
    {
        public int id { get; set; } //Scheduled Shift ID
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
        public EmployeeTk employee { get; set; } // needed for join
        public List<SegmentTk> segments { get; set; }
        public string eventDate { get; set; }
        public int employeeId { get; set; } //Person Id
    }

    public class SegmentTk
    {
        public int id { get; set; }
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
        public string type { get; set; } //"TRANSFER_SEGMENT" or "REGULAR_SEGMENT"
    }

    public class StartPunchTk
    {
        public List<ExceptionTk> exceptions { get; set; }
    }

    public class WorkedShiftDTO
    {
        public int id { get; set; }
        public EmployeeTk employee { get; set; } //needed for join
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
        //public DateTime roundedStartDateTime { get; set; }
        //public DateTime roundedEndDateTime { get; set; }
        public bool consolidatedScheduleSegments { get; set; } // whether it's consolitaded or not
        public List<WorkedSpanTk> workedSpans { get; set; }
        public double shiftTotalHours { get; set; }
        public List<int> scheduledShiftIds { get; set; }
    }

    public class WorkedSpanTk
    {
        public OrgJobTk primaryOrgJob { get; set; }
        public int orderNumber { get; set; } // might want where order number is 1
        public StartPunchTk startPunch { get; set; }
    }
}
