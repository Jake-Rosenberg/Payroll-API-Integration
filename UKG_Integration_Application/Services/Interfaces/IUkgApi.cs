using Refit;
using UKG_Integration_Application.API_UKG;
using UKG_Integration_Application.API_UKG.DTOs;

namespace UKG_Integration_Application.Services.Interfaces
{
    public interface IUkgApi
    {
        // Execute Hyperfind
        [Post("/v1/commons/hyperfind/execute")]
        Task<HyperfindDTO> PostExecuteHyperfind(
            [Body] ExecuteHyperfindBody executeHyperfindBody);

        // Employee Data Multi-read
        [Post("/v1/commons/data/multi_read")]
        Task<EmployeeDTO> PostEmployeeData(
            [Body] Body employeeDataBody);

        // Late-Punch Data Multi-read
        [Post("/v1/commons/data/multi_read")]
        Task<D_LateDTO> PostLatePunchData(
            [Body] Body latePunchDataBody);

        // HoursWorked Data Multi-read
        [Post("/v1/commons/data/multi_read")]
        Task<HoursWorkedDTO> PostHoursWorkedData(
            [Body] Body hoursWorkedDataBody);

        // Timecard Multi-read
        [Post("/v1/timekeeping/timecard/multi_read")]
        Task<List<TimecardDTO>> PostTimecardData(
            [Body] TcBody timeCardDataBody);
    }

    // Execute Hyperfind
    public record ExecuteHyperfindBody(DateRange DateRange, Hyperfind Hyperfind);
    public record Hyperfind(string Qualifier);
    public record SymbolicPeriod(int? Id);
    public record DateRange(SymbolicPeriod? SymbolicPeriod, string? StartDate, string? EndDate);

    // Retrieve Data Call
    public record Body(IReadOnlyList<Select> Select, From From, Options? Options, IReadOnlyList<Where>? Where, IReadOnlyList<GroupBy>? GroupBy);
    public record Select(string? Alias, string Key);
    public record From(EmployeeSet EmployeeSet, string View);
    public record Options(bool IncludeColumnTotals);
    public record Where(string? Alias, string Key, string Operator, IReadOnlyList<string>? Values);
    public record GroupBy(string? Alias, string Key);
    public record EmployeeSet(DateRange DateRange, Employees Employees);
    public record Employees(IReadOnlyList<int> Ids);

    // Timecard Call
    public record TcBody(IReadOnlyList<string> Select, TcWhere Where, TcOptions multiReadOptions);
    public record TcWhere(DateRange DateRange, Employees Employees);
    public record TcOptions(bool breakSpanAtMidnight, bool combineWorkShiftAtMidnight);
}
