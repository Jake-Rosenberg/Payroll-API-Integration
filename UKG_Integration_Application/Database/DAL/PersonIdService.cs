using Domain.Entities.UKG;
using System.Linq.Expressions;
using UKG_Integration_Application.Database.BaseRepo;

namespace UKG_Integration_Application.Database.DAL
{
    public interface IPersonIdService
    {
        List<int> GetActiveOfficerIds();
        List<int> GetAllInactiveEmployees();
        List<int> GetAllActiveEmployees();
        List<int> GetMISEmployees();
        List<int> GetAllDbEmployeeIds();
    }

    public class PersonIdService(IBaseRepository<Employee> baseRepository) : IPersonIdService
    {
        private readonly IBaseRepository<Employee> _employeeRepository = baseRepository;

        public List<int> GetActiveOfficerIds()
        {
            Expression<Func<Employee, bool>> condition = e => e.TitleId.Equals("5H04") && e.EmploymentStatus.Equals("Active");

            var officers = _employeeRepository.FindByCondition(condition);

            var officerIds = officers.Select(e => e.PersonId).ToList();

            return officerIds;
        }

        public List<int> GetAllInactiveEmployees()
        {
            Expression<Func<Employee, bool>> condition = e => e.EmploymentStatus.Equals("Inactive");

            var employees = _employeeRepository.FindByCondition(condition);

            var employeeIds = employees.Select(e => e.PersonId).ToList();

            return employeeIds;
        }

        public List<int> GetAllActiveEmployees()
        {
            Expression<Func<Employee, bool>> condition = e => e.EmploymentStatus.Equals("Active");

            var employees = _employeeRepository.FindByCondition(condition);

            var employeeIds = employees.Select(e => e.PersonId).ToList();

            return employeeIds;
        }

        public List<int> GetMISEmployees()
        {
            Expression<Func<Employee, bool>> condition = e => e.Facility.Equals("MS") && e.TitleId.Equals("5H04");

            var employees = _employeeRepository.FindByCondition(condition);

            var employeeIds = employees.Select(e => e.PersonId).ToList();

            return employeeIds;
        }

        public List<int> GetAllDbEmployeeIds()
        {
            var ids = _employeeRepository.GetAllAsync().Result.Select(r => r.PersonId).ToList();
            return ids;
        }
    }
}
