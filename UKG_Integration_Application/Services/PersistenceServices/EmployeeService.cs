using Domain.Entities.UKG;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Database.DAL;

namespace UKG_Integration_Application.Services.PersistenceServices
{
    public interface IEmployeeService
    {
        Task UpdateEmployeesReactivateAsync(List<int> activeAPIEmployeeIds);
        Task UpdateEmployeesDeactivateAsync(List<int> activeAPIEmployeeIds, List<int> activeDbEmployeeIds);
        Task UpdateEmployeesAsync(List<Employee> apiEmployees);
        Task AddEmployeesAsync(List<int> activeAPIEmployeeIds, List<int> activeDbEmployeeIds, List<Employee> apiEmployees);
    }

    public class EmployeeService(
        IMSSQLContext mssqlContext,
        IPersonIdService personIdService,
        IBaseRepository<Employee> empBaseRepo) : IEmployeeService
    {
        private readonly IMSSQLContext _mssqlContext = mssqlContext;
        private readonly IPersonIdService _personIdService = personIdService;
        private readonly IBaseRepository<Employee> _empBaseRepo = empBaseRepo;

        // TODO: refactor all of the employeeService base repo stuff into the DAL

        private List<int> GetEmployeeIdsToReactivate(List<int> activeAPIEmployeeIds)
        {
            var inactiveEmployeeIds = _personIdService.GetAllInactiveEmployees();
            var employeeIdsToReactivate = inactiveEmployeeIds.Where(activeAPIEmployeeIds.Contains);
            return employeeIdsToReactivate.ToList();
        }

        public async Task UpdateEmployeesReactivateAsync(List<int> activeAPIEmployeeIds)
        {
            var employeesToReactivate = GetEmployeeIdsToReactivate(activeAPIEmployeeIds);

            var utcNow = DateTime.Now.ToUniversalTime();
            await _mssqlContext.UKG_Employees
                .Where(dbEmp => employeesToReactivate.Contains(dbEmp.PersonId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.EmploymentStatus, "Active")
                    .SetProperty(e => e.LastUpdated, utcNow));
            await _empBaseRepo.SaveChangesAsync();
        }

        public async Task UpdateEmployeesDeactivateAsync(List<int> activeAPIEmployeeIds, List<int> activeDbEmployeeIds)
        {
            var employeeIdsToDeactivate = activeDbEmployeeIds.Where(e => !activeAPIEmployeeIds.Contains(e));

            var utcNow = DateTime.Now.ToUniversalTime();

            await _mssqlContext.UKG_Employees
                .Where(dbEmp => employeeIdsToDeactivate.Contains(dbEmp.PersonId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.EmploymentStatus, "Inactive")
                    .SetProperty(e => e.LastUpdated, utcNow));
            await _empBaseRepo.SaveChangesAsync();
        }

        public async Task UpdateEmployeesAsync(List<Employee> apiEmployees)
        {
            var utcNow = DateTime.Now.ToUniversalTime();

            foreach (Employee employee in apiEmployees)
            {
                await _mssqlContext.UKG_Employees
                .Where(dbEmp => dbEmp.PersonId.Equals(employee.PersonId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.Department, employee.Department)
                    .SetProperty(e => e.Facility, employee.Facility)
                    .SetProperty(e => e.Shift, employee.Shift)
                    .SetProperty(e => e.Title, employee.Title)
                    .SetProperty(e => e.TitleId, employee.TitleId)
                    .SetProperty(e => e.LastUpdated, utcNow));
            }
            await _empBaseRepo.SaveChangesAsync();
        }

        public async Task AddEmployeesAsync(List<int> activeAPIEmployeeIds, List<int> activeDbEmployeeIds, List<Employee> apiEmployees)
        {
            var employeeIdsToReactivate = GetEmployeeIdsToReactivate(activeAPIEmployeeIds);

            var newDatabaseEmployeeIds = activeDbEmployeeIds.Concat(employeeIdsToReactivate);

            var newEmployees = apiEmployees.Where(e => !newDatabaseEmployeeIds.Contains(e.PersonId));

            await _empBaseRepo.AddRangeAsync(newEmployees, CancellationToken.None);
            await _empBaseRepo.SaveChangesAsync();
        }
    }
}
