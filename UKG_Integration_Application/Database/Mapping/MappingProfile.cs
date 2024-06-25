using AutoMapper;
using Domain.Entities.UKG;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using UKG_Integration_Application.API_UKG;
using UKG_Integration_Application.API_UKG.DTOs;

namespace UKG_Integration_Application.Database.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee Mapper
            CreateMap<EmpObject, Employee>()
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => GetEmpValue(src, "EMP_COMMON_ID")))
                .ForMember(dest => dest.Payroll, opt => opt.MapFrom(src => GetEmpValue(src, "PEOPLE_PERSON_NUMBER")))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => GetEmpValue(src, "PEOPLE_FIRST_NAME")))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => GetEmpValue(src, "PEOPLE_LAST_NAME")))
                .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => GetOrgSplit(GetEmpValue(src, "EMP_COMMON_PRIMARY_ORG"), 1)))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => GetOrgSplit(GetEmpValue(src, "EMP_COMMON_PRIMARY_ORG"), 2)))
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => GetOrgSplit(GetEmpValue(src, "EMP_COMMON_PRIMARY_ORG"), 3)))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => GetJobTitlePart(GetEmpValue(src, "EMP_COMMON_PRIMARY_JOB_TITLE"), 0)))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => GetEmpValue(src, "PEOPLE_USER_ACCOUNT_NAME")))
                .ForMember(dest => dest.EmploymentStatus, opt => opt.MapFrom(src => GetEmpValue(src, "PEOPLE_EMP_STATUS")))
                .ForMember(dest => dest.TitleId, opt => opt.MapFrom(src => GetJobTitlePart(GetEmpValue(src, "EMP_COMMON_PRIMARY_JOB_TITLE"), 1)))
                .ForMember(dest => dest.HireDate, opt => opt.MapFrom(src => GetEmpValue(src, "PEOPLE_HIRE_DATE")))
                .ForMember(dest => dest.PayRuleOrg, opt => opt.MapFrom(src => GetEmpValue(src, "PEOPLE_PAYRULE")))
                .ReverseMap();

            // D_Late Mapper
            CreateMap<PunchObject, D_LatePaycodeEdits>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.key.TKTIMECARD_TRANSACTION))
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => Int32.Parse(src.coreEntityKey.EMP.id)))
                .ForMember(dest => dest.HoursLate, opt => opt.MapFrom(src => TimeSpan.ParseExact(GetPunchValue(src, "HoursLate"),
                                                                                                 "hh\\:mm",
                                                                                                 CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.ShiftStart, opt => opt.MapFrom(src => DateTime.Parse(GetPunchRawValue(src, "ShiftStart"))))
                .ForMember(dest => dest.PunchIn, opt => opt.MapFrom(src => DateTime.Parse(GetPunchRawValue(src, "PunchIn"))))
                .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => GetOrgSplit(GetPunchValue(src, "OrgPath"), 1)))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => GetOrgSplit(GetPunchValue(src, "OrgPath"), 2)))
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => GetOrgSplit(GetPunchValue(src, "OrgPath"), 3)));

            // Productivity report mapper
            CreateMap<HoursWorkedIdHourPair, HoursWorked>()
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.Attributes[0].Value))
                .ForMember(dest => dest.TotalHoursWorked, opt => opt.MapFrom(src => ParseHours(src.SummaryListDisplay[2].Sum)));

            // Lateness Mapper
            CreateMap<WorkedShiftDTO, WorkedShift>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.employee.id))
                .ForMember(dest => dest.ScheduledShiftId, opt => opt.MapFrom(src => src.scheduledShiftIds.FirstOrDefault()))
                .ForMember(dest => dest.PunchIn, opt => opt.MapFrom(src => src.startDateTime))
                .ForMember(dest => dest.PunchOut, opt => opt.MapFrom(src => src.endDateTime))
                .ForMember(dest => dest.Exception, opt => opt.MapFrom(src => src.workedSpans.FirstOrDefault().startPunch.exceptions.IsNullOrEmpty() ? null : src.workedSpans.FirstOrDefault().startPunch.exceptions.FirstOrDefault().exceptionType.displayName))
                .ForMember(dest => dest.HoursLate, opt => opt.MapFrom(src => src.workedSpans.FirstOrDefault().startPunch.exceptions.IsNullOrEmpty() ? (TimeSpan?)null : TimeSpan.FromHours(src.workedSpans.FirstOrDefault().startPunch.exceptions.FirstOrDefault().violationInHours))) // might throw an exception
                .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => src.workedSpans.FirstOrDefault().primaryOrgJob == null ? null : GetOrgSplit(src.workedSpans.FirstOrDefault().primaryOrgJob.qualifier, 1)))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.workedSpans.FirstOrDefault().primaryOrgJob == null ? null : GetOrgSplit(src.workedSpans.FirstOrDefault().primaryOrgJob.qualifier, 2)))
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.workedSpans.FirstOrDefault().primaryOrgJob == null ? null : GetOrgSplit(src.workedSpans.FirstOrDefault().primaryOrgJob.qualifier, 3)))
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduleShift, opt => opt.Ignore());

            CreateMap<PaycodeEditDTO, PaycodeEdit>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.employee.id))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.itemId))
                .ForMember(dest => dest.ApplyDate, opt => opt.MapFrom(src => DateOnly.Parse(src.applyDate)))
                .ForMember(dest => dest.Paycode, opt => opt.MapFrom(src => src.paycode.qualifier))
                .ForMember(dest => dest.Employee, opt => opt.Ignore());

            CreateMap<ScheduleShiftDTO, ScheduleShift>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.employee.id))
                .ForMember(dest => dest.ShiftStart, opt => opt.MapFrom(src => src.startDateTime))
                .ForMember(dest => dest.ShiftEnd, opt => opt.MapFrom(src => src.endDateTime))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => DateOnly.Parse(src.eventDate)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.segments.FirstOrDefault().type))
                .ForMember(dest => dest.Employee, opt => opt.Ignore());
        }

        private static string? GetEmpValue(EmpObject source, string key)
        {
            var attributeValue = source.attributes.FirstOrDefault(a => a.key == key);
            return attributeValue?.value;
        }

        private static string? GetPunchValue(PunchObject source, string alias) 
        {
            var attrubuteValue = source.attributes.FirstOrDefault(a => a.alias == alias);
            return attrubuteValue?.value;
        }

        private static string? GetPunchRawValue(PunchObject source, string alias)
        {
            var attrubuteValue = source.attributes.FirstOrDefault(a => a.alias == alias);
            return attrubuteValue?.rawValue;
        }

        private static string? GetOrgSplit(string orgPath, int index)
        {
            //Console.WriteLine(orgPath);
            if (orgPath.IsNullOrEmpty())
            {
                return null;
            }
            else
            {
                string[] parts = orgPath.Split('/');
                if (parts.Length <= index)
                {
                    return orgPath;
                }
                else
                {
                    if (index == 2 && parts[1] == "CF" && parts[index] == "-")
                    {
                        parts[index] = "CFCF";
                        return parts[index];
                    }
                    return parts[index];
                }
            }
        }
        private static double ParseHours(string timeString)
        {
            string[] parts = timeString.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            double totalHours = hours + (minutes / 60.0);
            return totalHours;
        }

        private static string? GetJobTitlePart(string jobInfoString, int index)
        {
            string[] parts = jobInfoString.Split("-");
            if (parts.Length >= 2)
            {
                return parts[index];
            }
            else
            {
                return jobInfoString; // If no "-", assign the entire value
            }
        }
    }
}
