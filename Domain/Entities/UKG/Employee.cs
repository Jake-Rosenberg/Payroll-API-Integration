using Domain.Common;

namespace Domain.Entities.UKG
{
    public class Employee : BaseEntity
    {
        public required int PersonId { get; set; } // EMP_COMMON_ID
        public string? Payroll { get; set; } // PEOPLE_PERSON_NUMBER
        public string? FirstName { get; set; } // PEOPLE_FIRST_NAME
        public string? LastName { get; set; } // PEOPLE_LAST_NAME
        public string? Department { get; set; } // EMP_COMMON_PRIMARY_ORG 
        public string? Facility { get; set; } // EMP_COMMON_PRIMARY_ORG 
        public string? Shift { get; set; } // EMP_COMMON_PRIMARY_ORG 
        public string? Title { get; set; } // EMP_COMMON_PRIMARY_JOB_TITLE
        public string? Email { get; set; } // PEOPLE_USER_ACCOUNT_NAME
        public string? EmploymentStatus { get; set; } // PEOPLE_EMP_STATUS
        public string? TitleId { get; set; } // EMP_COMMON_PRIMARY_ORG_DESCRIPTION
        public DateTime HireDate { get; set; } // PEOPLE_HIRE_DATE
        public string? PayRuleOrg { get; set; } // PEOPLE_PAYRULE

        // Navigation Properties
        public ICollection<D_LatePaycodeEdits>? D_Lates { get; set; } = [];
        public HoursWorked? HoursWorked { get; set; }
        public ICollection<WorkedShift>? WorkedShift { get; set; } = [];
        public ICollection<PaycodeEdit>? PaycodeEdit { get; set; } = [];
        public ICollection<ScheduleShift>? ScheduleShift { get; set; } = [];
    }
}
