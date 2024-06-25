using AutoMapper;
using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using Refit;
using UKG_Integration_Application.API_UKG;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Services.Interfaces;

namespace UKG_Integration_Application.Services
{
    public interface IEmployeeDataService
    {
        Task<List<Employee>> GetEmployeeData(List<int> ids);
    }

    public class EmployeeDataService(
        IUkgApi ukgApi,
        ILogger<EmployeeDataService> logger,
        IBatchingService batchingService,
        IBaseRepository<Employee> baseRepo,
        IMapper mapper) : IEmployeeDataService
    {
        private readonly IUkgApi _ukgApi = ukgApi;
        private readonly ILogger<EmployeeDataService> _logger = logger;
        private readonly IBatchingService _batchingService = batchingService;
        private readonly IMapper _mapper = mapper;

        // TODO: see if you can refactor this class so that it's just a template for all queries
        private async Task<List<EmpObject>> EmployeeDataCall(List<int> ids)
        {

            _logger.LogInformation("Employee Data Service call initiated.");

            var employees = ids;

            try
            {
                var response = await _ukgApi.PostEmployeeData(
                    new Body(
                        // SELECT
                        [
                            new (null, "EMP_COMMON_ID"),
                            new (null, "PEOPLE_PERSON_NUMBER"),
                            new (null, "PEOPLE_FIRST_NAME"),
                            new (null, "PEOPLE_LAST_NAME"),
                            new (null, "EMP_COMMON_PRIMARY_ORG"),
                            new (null, "EMP_COMMON_PRIMARY_ORG_DESCRIPTION"),
                            new (null, "EMP_COMMON_PRIMARY_JOB_TITLE"),
                            new (null, "PEOPLE_EMP_STATUS"),
                            new (null, "PEOPLE_HIRE_DATE"),
                            new (null, "PEOPLE_PAYRULE"),
                            new (null, "PEOPLE_USER_ACCOUNT_NAME")
                        ],
                        // FROM
                        new From(
                            new EmployeeSet(
                                new DateRange(
                                    new SymbolicPeriod(
                                        1
                                    ),
                                    null,
                                    null
                                ),
                                new Employees(
                                    employees
                                )
                            ),
                            "EMP"
                        ),
                        // OPTIONS
                        null,
                        // WHERE
                        null,
                        // GROUP BY
                        null
                    )
                );

                _logger.LogInformation("Record count from Employee Data call: {count}", response.metadata.totalElements.ToString());

                List<EmpObject> details = response.data.children;

                _logger.LogInformation("Employee Data call successful.");
                return details;
            }
            catch( ApiException ex )
            {
                _logger.LogError(ex, "Employee Data call error: {message}, CONTENT:{content}", ex.Message, ex.Content);
                throw ex;
            }
        }

        public async Task<List<Employee>> GetEmployeeData(List<int> ids)
        {
            _logger.LogInformation("Employee data FUNCTION initiated.");

            var allMappedData = new List<Employee>();

            var batchSize = 400;
            int offset = 0;

            try
            {
                var pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);

                while (pagedIds.Count != 0)
                {
                    _logger.LogInformation("Batch offset = {offset}", offset);

                    // API Call
                    var employeeDataBatch = await EmployeeDataCall(pagedIds);
                    offset += batchSize;

                    // Map Results
                    var mappedData = _mapper.Map<IEnumerable<Employee>>(employeeDataBatch);

                    // Add to list and eval next batch
                    allMappedData.AddRange(mappedData);
                    pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);
                }

                _logger.LogInformation("Employee data returned successfully. Result count: {count}", allMappedData.Count);
                return allMappedData;

            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Employee data FUNCTION exception: {message}", ex.Message);
                throw ex;
            }
        }
    }
}
