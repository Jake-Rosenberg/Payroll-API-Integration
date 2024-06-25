using Domain.Entities.UKG;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using UKG_Integration_Application.Config;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Database.DAL;
using UKG_Integration_Application.Services;
using UKG_Integration_Application.Services.PersistenceServices;


IServiceConfiguration serviceConfiguration = new ServiceConfiguration();
var serviceProvider = serviceConfiguration.ConfigureServices();

var idHyperfindService = serviceProvider.GetRequiredService<IIdHyperfindService>();
var employeeDataService = serviceProvider.GetRequiredService<IEmployeeDataService>();
var punchBaseRepo = serviceProvider.GetRequiredService<IBaseRepository<D_LatePaycodeEdits>>();
var hoursWorkedRepo = serviceProvider.GetRequiredService<IBaseRepository<HoursWorked>>();
var workedShitService = serviceProvider.GetRequiredService<IWorkedShiftService>();
var paycodeEditService = serviceProvider.GetRequiredService<IPaycodeEditService>();
var scheduleShiftService = serviceProvider.GetRequiredService<IScheduleShiftService>();
var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();
var mssqlContext = serviceProvider.GetRequiredService<IMSSQLContext>();
var personIdService = serviceProvider.GetRequiredService<IPersonIdService>();
var dLateDataService = serviceProvider.GetRequiredService<ID_LateDataService>();
var hoursWorkedDataService = serviceProvider.GetRequiredService<IHoursWorkedDataService>();
var timecardDataService = serviceProvider.GetRequiredService<ITimecardDataService>();
var latnessReportExecutionService = serviceProvider.GetRequiredService<StoredProcedureService>();

// STEPS:

// 1. Get IDs numbers of all staff
var activeAPIEmployeeIds = await idHyperfindService.GetEmployeeIds();

// 2. Get employee data from API
var apiEmployees = await employeeDataService.GetEmployeeData(activeAPIEmployeeIds);

// 3. Get employee Ids marked Active from Db
var activeDbEmployeeIds = personIdService.GetAllActiveEmployees();

// 4. Update employee status to Active if they appear in API list
await employeeService.UpdateEmployeesReactivateAsync(activeAPIEmployeeIds);

// 5. Update employee status to Inactive if they don't appear in API list
await employeeService.UpdateEmployeesDeactivateAsync(activeAPIEmployeeIds, activeDbEmployeeIds);

// 6. Update existing employee info
await employeeService.UpdateEmployeesAsync(apiEmployees);

// 7. Add all new employees from API list
await employeeService.AddEmployeesAsync(activeAPIEmployeeIds, activeDbEmployeeIds, apiEmployees);

// 8. Get all ids from database
var officerIds = personIdService.GetActiveOfficerIds();

// 9. Make D_Late multiRead API call
var dLatePunches = await dLateDataService.GetLatePunchData(activeAPIEmployeeIds);

// 10. Add Late Punches to DB and save changes
await punchBaseRepo.AddRangeAsync(dLatePunches, CancellationToken.None);
await punchBaseRepo.SaveChangesAsync();

// 11. Make Timecard API call
var timecardData = await timecardDataService.GetTimecardData(activeAPIEmployeeIds);
var workedShifts = timecardData.Item1;
var paycodeEdits = timecardData.Item2;
var scheduleShifts = timecardData.Item3;

// 12. Add Schedule Shift data to DB and save changes
await scheduleShiftService.AddScheduleShiftsAsync(scheduleShifts);

// 13. Add Worked Shift data to DB and save changes
await workedShitService.AddOrUpdateWorkedShiftsAsync(workedShifts);

// 14. Add Paycode Edit data to DB and save changes
await paycodeEditService.AddPaycodeEditsAsync(paycodeEdits);

// 15. Execute Lateness Report subscription
await latnessReportExecutionService.ExecuteStoredProcedure();

// 16. Get ids of staff
var helpDeskIds = personIdService.GetMISEmployees();

// 17. Get total hours worked per employee
var hoursWorked = await hoursWorkedDataService.GetHoursWorkedData(helpDeskIds);

// 18. Truncate hours worked table
await mssqlContext.UKG_HoursWorked.ExecuteDeleteAsync();

// 19. Add hours worked totals to DB and save changes
await hoursWorkedRepo.AddRangeAsync(hoursWorked, CancellationToken.None);
await hoursWorkedRepo.SaveChangesAsync();