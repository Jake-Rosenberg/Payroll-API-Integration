using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UKG_Integration_Application.Services
{
    public class StoredProcedureService (MSSQLContext dbContext)
    {
        private MSSQLContext _dbContext = dbContext;

        public async Task ExecuteStoredProcedure()
        {

            FormattableString sqlQuery = $"EXECUTE ;

            // Execute raw SQL query to call the stored procedure
            await _dbContext.Database.ExecuteSqlAsync(sqlQuery);
        }
    }
}
